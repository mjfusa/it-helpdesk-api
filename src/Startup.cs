using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ITHelpdeskAPI.Extensions;
using ITHelpdeskAPI.Services;
using ITHelpdeskAPI.Tools;

namespace ITHelpdeskAPI
{
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

            services.AddControllers().AddNewtonsoftJson();

            // create singleton instance of HelpdeskService
            services.AddSingleton<HelpdeskService>();
            // Register MCP Tools explicitly
            services.AddScoped<HelpdeskMcpTools>();

            // Add MCP Server with HTTP transport
            services.AddMcpServer(options =>
            {
                options.ServerInfo = new ModelContextProtocol.Protocol.Implementation
                {
                    Name = "IT Helpdesk MCP Server",
                    Version = "1.0.0"
                };
            })
            .WithHttpTransport()
            .WithToolsFromAssembly(typeof(HelpdeskMcpTools).Assembly);

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
                        new[] { Configuration["AzureAd:Audience"] + "/access_as_user", Configuration["AzureAd:Audience"] + "/Cases.Read", Configuration["AzureAd:Audience"] + "/Cases.Write" }
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

            // // Add CORS policy
            // services.AddCors(options =>
            // {
            //     options.AddPolicy("default", builder =>
            //     {
            //         builder.WithOrigins("https://it-helpdesk-101.azurewebsites.net")
            //             .AllowAnyHeader()
            //             .AllowAnyMethod();
            //         // builder.WithOrigins("https://ppcjm0mv-5001.usw3.devtunnels.ms")
            //         //     .AllowAnyHeader()
            //         //     .AllowAnyMethod();
            //     });
            // });
            // Add CORS support for MCP
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
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
                    // options.SerializeAsV2 = true;
                    options.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers.Clear();

                        // Get endpoints configuration
                        var endpoints = Configuration.GetSection("Endpoints:it-helpdesk-api");
                        var runMode = endpoints["RunMode"];

                        string? activeUrl = null;
                        string? description = null;

                        // Determine URL based on RunMode
                        switch (runMode?.ToLower())
                        {
                            case "local":
                                activeUrl = endpoints["LocalUrl"];
                                description = "Local Development";
                                break;
                            case "devtunnel":
                                activeUrl = endpoints["DevTunnelUrl"];
                                description = "Dev Tunnel (External Access)";
                                break;
                            case "production":
                                activeUrl = endpoints["ProductionUrl"];
                                description = "Production";
                                break;
                            default:
                                // Fallback to local if RunMode is not specified or invalid
                                activeUrl = endpoints["LocalUrl"];
                                description = "Local Development (Default)";
                                break;
                        }

                        // Add the active server based on RunMode
                        if (!string.IsNullOrEmpty(activeUrl))
                        {
                            swagger.Servers.Add(new OpenApiServer
                            {
                                Url = activeUrl,
                                Description = description
                            });
                        }
                    });
                });
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IT Helpdesk API v1");
                    // Configure OAuth2 client for the Swagger UI
                    c.OAuthClientId(Configuration["AzureAd:ClientId"]);
                    c.OAuthAppName("IT Helpdesk API - Swagger");
                    // Not using PKCE as this is an implicit flow
                });
            }

            // Serve static files (for documentation)
            app.UseStaticFiles();

            // Middleware order: Routing → CORS → Authentication → Authorization
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            // Add middleware to handle GET requests to MCP endpoints with helpful error messages
            app.UseMcpMethodCheck("/mcp");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                // Map MCP endpoints with authentication requirement
                // To restrict to administrators only, use: .RequireAuthorization("RequireAdministratorRole")
                endpoints.MapMcp("/mcp").RequireAuthorization(); // Require any authenticated user
            });
        }
    }
}