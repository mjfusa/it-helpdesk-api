using ITHelpdeskAPI.Services;
using ITHelpdeskAPI.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;

namespace ITHelpdeskAPI
{
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddNewtonsoftJson(); // Add Newtonsoft.Json support for better JSON handling
            
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

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "IT Helpdesk API",
                Version = "1.0.0",
                Description = "API for managing IT helpdesk cases",
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
            }
                
                 
                 
                 );
                // swagger.Servers.Add(new OpenApiServer { Url = $"https://it-helpdesk-101.azurewebsites.net" }));
            });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IT Helpdesk API v1"));
        }

        // Enable CORS
        app.UseCors();
        
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMcp("/mcp"); // Map MCP endpoints
        });
    }
}
}