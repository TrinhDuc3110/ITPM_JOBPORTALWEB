using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using Microsoft.Extensions.Configuration; 
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JOBPORTALWEB.INFRASTRUCTURE.Services
{
    // Triển khai IAuthService
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        
        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user, IList<string> roles)
        {
            // 1. Khởi tạo Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            // Thêm Role Claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // 2. Lấy Key và Signing Credentials
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured in appsettings.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            // 3. Tính toán Thời hạn (Đã sửa lỗi Format/Null)
            var minutesString = _config["Jwt:AccessTokenMinutes"];
            
            // Cố gắng parse, nếu lỗi thì dùng mặc định 60 phút
            if (!double.TryParse(minutesString, out double minutes))
            {
                minutes = 60; 
            }
            var expires = DateTime.UtcNow.AddMinutes(minutes); 

            // 4. Tạo Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            // 5. Viết Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}