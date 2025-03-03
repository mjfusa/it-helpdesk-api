using ITHelpdeskAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // create singleton instance of HelpdeskService
        services.AddSingleton<HelpdeskService>();
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
        // Add any additional services here, such as database context or repositories
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
                swagger.Servers.Add(new OpenApiServer { Url = $"https://it-helpdesk-101.azurewebsites.net" }));
            });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IT Helpdesk API v1"));
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}