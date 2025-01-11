using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using processtrak_backend.Utils;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // List of routes to bypass the middleware
        var bypassRoutes = new[]
        {
            "/api/auth/register",
            "/api/auth/login",
            "/api/auth/forgot-password",
            "/api/auth/reset-password",
        };

        // Check if the current path matches any bypass route
        if (bypassRoutes.Any(route => context.Request.Path.StartsWithSegments(route)))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(GlobalVariables.JwtSecret)
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    },
                    out var validatedToken
                );

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.First(x => x.Type == "primarysid").Value;

                var userGuid = Guid.Parse(userId);
                // Attach user ID to the context
                context.Items["User"] = userGuid;
            }
            catch (Exception ex)
            {
                // Token validation failed
                Console.WriteLine($"Token validation failed: {ex.Message}");
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        else
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Authorization header is missing");
            return;
        }

        await _next(context);
    }
}
