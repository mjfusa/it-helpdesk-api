using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

// #if _DEBUG_
namespace ITHelpdeskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // This requires authentication
    public class TokenDebugController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenDebugController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("info")]
        public IActionResult GetTokenInfo()
        {
            // Extract the token from the Authorization header
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "Bearer token not found in Authorization header" });
            }

            var token = authHeader.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                return BadRequest(new { message = "Invalid token format" });
            }

            var jwt = handler.ReadJwtToken(token);

            var configAudience = _configuration["AzureAd:Audience"];
            var tokenAudience = jwt.Audiences.FirstOrDefault();
            var audienceMatch = string.Equals(configAudience, tokenAudience, StringComparison.OrdinalIgnoreCase);

            var info = new
            {
                TokenInfo = new
                {
                    Audience = tokenAudience,
                    Issuer = jwt.Issuer,
                    Subject = jwt.Subject,
                    ValidFrom = jwt.ValidFrom,
                    ValidTo = jwt.ValidTo,
                    Claims = jwt.Claims.Select(c => new { Type = c.Type, Value = c.Value })
                },
                ConfigInfo = new
                {
                    ExpectedAudience = configAudience,
                    AudienceMatches = audienceMatch,
                    TenantId = _configuration["AzureAd:TenantId"]
                },
                ValidationDiagnostics = new
                {
                    AudienceValidationMessage = audienceMatch
                        ? "Token audience matches configuration"
                        : $"Audience validation failed: Token audience '{tokenAudience}' does not match expected audience '{configAudience}'"
                }
            };

            return Ok(info);
        }
    }
}
// #endif
