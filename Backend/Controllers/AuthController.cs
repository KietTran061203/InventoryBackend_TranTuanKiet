using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using BCrypt.Net;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MongoContext _db;
    private readonly JwtService _jwt;

    public AuthController(MongoContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var exists = await _db.Users.Find(u => u.Email == dto.Email).AnyAsync();
        if (exists) return Conflict("Email already registered");

        var user = new User
        {
            Id = ObjectId.GenerateNewId(),
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        await _db.Users.InsertOneAsync(user);

        var token = _jwt.CreateToken(user.Id, user.Email, user.Username);
        return Ok(new AuthResponse(token, user.Username, user.Email));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users.Find(u => u.Email == dto.Email.Trim().ToLower()).FirstOrDefaultAsync();
        if (user is null) return Unauthorized("Invalid credentials");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return Unauthorized("Invalid credentials");

        var token = _jwt.CreateToken(user.Id, user.Email, user.Username);
        return Ok(new AuthResponse(token, user.Username, user.Email));
    }
}
