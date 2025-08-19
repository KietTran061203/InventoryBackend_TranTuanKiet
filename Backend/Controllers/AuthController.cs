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
    // Biến thể hiện của MongoContext và JwtService
    private readonly MongoContext _db;
    private readonly JwtService _jwt;

    public AuthController(MongoContext db, JwtService jwt)
    {
        // Khởi tạo các biến với các đối tượng được tiêm vào thông qua Dependency Injection
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // Kiểm tra xem email đã đăng ký hay chưa
        var exists = await _db.Users.Find(u => u.Email == dto.Email).AnyAsync();
        if (exists) return Conflict("Email đã được đăng ký");
        // Tạo người dùng mới với thông tin từ DTO
        var user = new User
        {
            Id = ObjectId.GenerateNewId(),
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        // Thêm người dùng vào cơ sở dữ liệu MongoDB
        await _db.Users.InsertOneAsync(user);
        // Tạo token JWT cho người dùng mới
        var token = _jwt.CreateToken(user.Id, user.Email, user.Username);
        // Trả về thông tin người dùng và token
        return Ok(new AuthResponse(token, user.Username, user.Email));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // Tìm người dùng trong cơ sở dữ liệu theo email
        var user = await _db.Users.Find(u => u.Email == dto.Email.Trim().ToLower()).FirstOrDefaultAsync();
        if (user is null) return Unauthorized("Thông tin không hợp lệ");
        // Kiểm tra mật khẩu bằng cách so sánh với hash đã lưu
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return Unauthorized("Thông tin không hợp lệ");
        // Tạo token JWT cho người dùng đã đăng nhập
        var token = _jwt.CreateToken(user.Id, user.Email, user.Username);
        // Trả về thông tin người dùng và token
        return Ok(new AuthResponse(token, user.Username, user.Email));
    }
}
