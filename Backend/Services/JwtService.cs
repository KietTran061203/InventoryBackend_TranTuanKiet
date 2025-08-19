using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace Backend.Services;

public class JwtOptions
{
    // Khai báo các thông tin cần thiết cho JWT
    public string Issuer { get; set; } = null!; // Người cung cấp token
    public string Audience { get; set; } = null!; // Đối tượng sử dụng token
    public string Secret { get; set; } = null!; // Khóa bí mật để mã hóa token
}

public class JwtService
{
    // Biến để lưu cấu hình Token JWT
    private readonly JwtOptions _opt;
    // Khởi tạo JwtService với cấu hình từ IOptions
    public JwtService(IOptions<JwtOptions> opt) => _opt = opt.Value;
    // Phương thức tạo token JWT
    public string CreateToken(ObjectId userId, string email, string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // Thông tin người dùng
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("username", username),
        };
        // Tạo khóa bí mật và thông tin xác thực để ký token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret));
        // Tạo thông tin xác thực với thuật toán mã hóa HMAC SHA256
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_opt.Issuer, _opt.Audience, claims,
            // Thời gian tạo và hết hạn của token
            expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);
        // Trả về token đã được mã hóa dưới dạng chuỗi
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
