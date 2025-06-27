using Microsoft.AspNetCore.Builder;
using ITHelpdeskAPI.Middleware;

namespace ITHelpdeskAPI.Extensions
{
    /// <summary>
    /// Extensions for adding custom middleware to the application pipeline
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds middleware to intercept GET requests to MCP endpoints and return helpful error messages
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="mcpEndpoint">The MCP endpoint path (e.g., "/mcp")</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UseMcpMethodCheck(this IApplicationBuilder app, string mcpEndpoint)
        {
            return app.UseMiddleware<McpMethodCheckMiddleware>(mcpEndpoint);
        }
    }
}
