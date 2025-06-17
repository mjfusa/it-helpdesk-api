using ITHelpdeskAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        // Add Microsoft Identity authentication with explicit audience validation
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                Configuration.GetSection("AzureAd").Bind(options);

                // Explicitly set audience validation parameters
                options.TokenValidationParameters.ValidAudience = Configuration["AzureAd:Audience"];
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidIssuer = $"https://login.microsoftonline.com/{Configuration["AzureAd:TenantId"]}/v2.0";

                // Add event handler for debugging token validation failures
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            }, options => { Configuration.GetSection("AzureAd").Bind(options); });

        // Add authorization policies if needed
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole",
                policy => policy.RequireRole("Administrator"));
        });

        services.AddControllers();
        // create singleton instance of HelpdeskService
        services.AddSingleton<HelpdeskService>();

        // Configure Swagger to support OAuth authentication
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "IT Helpdesk API",
                Version = "1.0.0",
                Description = "API for managing IT helpdesk cases",
            });

            // Configure Swagger to use OAuth
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"https://login.microsoftonline.com/{Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { Configuration["AzureAd:Audience"] + "/access_as_user", "Access the API as a user" },
                            { Configuration["AzureAd:Audience"] + "/Cases.Read", "Read helpdesk cases" },
                            { Configuration["AzureAd:Audience"] + "/Cases.Write", "Write helpdesk cases" }
                        }
                    }
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    new[] { Configuration["AzureAd:Audience"] + "/access_as_user" , Configuration["AzureAd:Audience"] + "/Cases.Read", Configuration["AzureAd:Audience"] + "/Cases.Write" }
                }
            });
        });

        // Add Microsoft Identity Web API integration
        // services.AddMicrosoftIdentityWebApiAuthentication(Configuration)
        //     .EnableTokenAcquisitionToCallDownstreamApi()
        //     .AddMicrosoftGraph(Configuration.GetSection("MicrosoftGraph"))
        //     .AddInMemoryTokenCaches();

        // services.AddScoped<UserService>();

        // Add any additional services here, such as database context or repositories

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("default", builder =>
            {
                builder.WithOrigins("https://it-helpdesk-101.azurewebsites.net")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                // builder.WithOrigins("https://ppcjm0mv-5001.usw3.devtunnels.ms")
                //     .AllowAnyHeader()
                //     .AllowAnyMethod();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                swagger.Servers.Add(new OpenApiServer { Url = $"https://it-helpdesk-101.azurewebsites.net" }));
                // swagger.Servers.Add(new OpenApiServer { Url = $"https://ppcjm0mv-5001.usw3.devtunnels.ms" }));
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IT Helpdesk API v1");
                c.OAuthClientId(Configuration["AzureAd:ClientId"]);
                c.OAuthAppName("IT Helpdesk API - Swagger");
                c.OAuthUsePkce();
            });
        }

        // Enable CORS
        app.UseCors("default");

        app.UseRouting();

        // Add authentication and authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}