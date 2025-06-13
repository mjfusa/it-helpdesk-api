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

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // // Add Microsoft Identity authentication
         services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

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
                            { "api://it-helpdesk-101.azurewebsites.net/access_as_user", "Access the API as a user" }
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
                    new[] { "api://it-helpdesk-101.azurewebsites.net/access_as_user" }
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
            });
            app.UseSwaggerUI(c => {
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