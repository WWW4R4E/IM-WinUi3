using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatRoomASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomASP.Hubs
{
    public class AccountHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("LoginResult", false, "用户名或密码错误", null);
                return;
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (passwordValid)
            {
                // 如果验证成功，调用下面的方法
                var config = Context.GetHttpContext().RequestServices.GetRequiredService<IConfiguration>();
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SigningKey"]));
                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: new[] { new Claim(ClaimTypes.Name, user.UserName) },
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                await Clients.Client(Context.ConnectionId).SendAsync("LoginResult", true, "成功", jwtToken);
            }
            else
            {
                // 登录失败，返回错误信息
                await Clients.Client(Context.ConnectionId).SendAsync("LoginResult", false, "用户名或密码错误", null);
            }
        }
    }
}