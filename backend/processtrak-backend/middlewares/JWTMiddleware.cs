using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using processtrak_backend.Services;
using processtrak_backend.Utils;

//Add user on httpContext

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public JwtMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null && endpoint.Metadata.Any(m => m is AllowAnonymousAttribute))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

                if (!await authService.IsTokenValid(token))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                AttachUserToContext(context, token);
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

    private void AttachUserToContext(HttpContext context, string token)
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
            context.Items["User"] = userGuid;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.WriteAsync("Unauthorized").Wait();
        }
    }
}
