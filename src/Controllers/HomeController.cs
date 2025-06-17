using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ITHelpdeskAPI.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize] // Require authentication for the root controller
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Get user information from the claims
            var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var userName = User.FindFirst("name")?.Value ?? User.FindFirst("preferred_username")?.Value ?? "authenticated user";
            
            return Ok(new
            {
                Message = "IT Helpdesk API - Root Endpoint",
                Description = "Welcome to the IT Helpdesk API. This endpoint requires authentication.",
                Timestamp = DateTime.UtcNow,
                User = new
                {
                    Id = userId,
                    Name = userName,
                    IsAuthenticated = User.Identity != null && User.Identity.IsAuthenticated
                },
                Links = new[]
                {
                    new { Href = "/api/helpdesk", Rel = "helpdesk", Method = "GET" },
                    new { Href = "/swagger", Rel = "documentation", Method = "GET" }
                }
            });
        }
    }
}
