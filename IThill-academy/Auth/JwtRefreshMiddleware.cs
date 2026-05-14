using IThill_academy.Services;

namespace IThill_academy.Auth;


public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public JwtRefreshMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, JwtService jwtService, RefreshTokenService refreshTokenService, AuthService authService)
    {
        var jwt = context.Request.Cookies["jwt"];
        var refreshToken = context.Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(jwt))
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            // если Access‑токен истёк
            if (token.ValidTo < DateTime.UtcNow && !string.IsNullOrEmpty(refreshToken))
            {
                var storedToken = refreshTokenService.GetToken(refreshToken);
                if (storedToken != null && refreshTokenService.ValidateRefreshToken(refreshToken, storedToken.UserId))
                {
                    var student = await authService.GetStudentById(Guid.Parse(storedToken.UserId));
                    if (student != null)
                    {
                        var newAccessToken = jwtService.GenerateAccessToken(student);

                        context.Response.Cookies.Append("jwt", newAccessToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddMinutes(15)
                        });
                    }
                }
            }
        }

        await _next(context);
    }
}

