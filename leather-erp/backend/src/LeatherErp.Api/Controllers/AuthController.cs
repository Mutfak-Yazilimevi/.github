using LeatherErp.Api.Auth;
using LeatherErp.Api.Models;
using LeatherErp.Application.Common;
using LeatherErp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeatherErp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwt;

    public AuthController(AppDbContext db, JwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    /// <summary>Kullanıcı adı/parola ile giriş yapar ve JWT döndürür. (Seed: admin / admin123)</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Kullanıcı adı veya parola hatalı." });

        var (token, expires) = _jwt.CreateToken(user);
        return Ok(new LoginResponse(token, expires, user.Username, user.Role.ToString()));
    }
}
