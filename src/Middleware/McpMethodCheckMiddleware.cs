using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ITHelpdeskAPI.Middleware
{
    /// <summary>
    /// Middleware to intercept GET requests to MCP endpoints and return a helpful error message
    /// instead of letting them fall through to a generic 404 error.
    /// </summary>
    public class McpMethodCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _mcpEndpoint;

        public McpMethodCheckMiddleware(RequestDelegate next, string mcpEndpoint)
        {
            _next = next;
            _mcpEndpoint = mcpEndpoint;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if this is a GET request to the MCP endpoint
            if (context.Request.Method == "GET" && context.Request.Path.StartsWithSegments(_mcpEndpoint))
            {
                // Special handling for VS Code requests
                string userAgent = context.Request.Headers["User-Agent"].ToString();
                bool isVsCodeClient = userAgent.Contains("VSCode") || userAgent.Contains("Code-OSS") || 
                                     context.Request.Headers["X-VSCode-MCP"].ToString() == "true";
                
                if (isVsCodeClient)
                {
                    // Return a standard JSON-RPC error that VS Code will understand
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/json";
                    
                    var response = new
                    {
                        jsonrpc = "2.0",
                        id = "",
                        error = new {
                            code = -32000,
                            message = "VS Code MCP client detected. Please use POST with JSON-RPC 2.0 payload."
                        }
                    };
                    
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
                // For regular clients requesting JSON
                else if (context.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    context.Response.ContentType = "application/json";
                    
                    var response = new
                    {
                        error = "Method Not Allowed",
                        message = "MCP endpoints require POST requests with JSON-RPC 2.0 payload. GET requests are not supported.",
                        documentation = "For more information, visit /mcp-docs.html"
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
                // Option 2: Redirect to documentation page for browser requests
                else
                {
                    context.Response.Redirect("/mcp-docs.html");
                    return;
                }
            }

            // Continue processing the request
            await _next(context);
        }
    }
}
