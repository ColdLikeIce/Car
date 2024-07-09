using CommonCore.Dependency;
using HeyTripCarWeb.Share.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HeyTripCarWeb.Share
{
    public class JwtHelper
    {
        public JwtHelper()
        {
        }

        public string CreateToken(int timeout, string account, string password, JwtConfig _config)
        {
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, "u_admin"), //HttpContext.User.Identity.Name
            new Claim(ClaimTypes.Role, "r_admin"), //HttpContext.User.IsInRole("r_admin")
            new Claim(JwtRegisteredClaimNames.Jti, "admin"),
            new Claim("User", "Admin"),
            new Claim("Name", "超级管理员")
        };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                account,     //Issuer
                password,   //Audience
                claims,                          //Claims,
                DateTime.Now,                    //notBefore
                DateTime.Now.AddSeconds(timeout),    //expires
                signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return "Bearer " + token;
        }
    }
}