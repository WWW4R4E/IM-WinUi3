using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatRoomASP.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomASP.Hubs
{
    public class AccountHub : Hub
    {
        private readonly ILogger<AccountHub> _logger;
        private readonly AppDbContext _context;

        public AccountHub(ILogger<AccountHub> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Login(string userName, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("LoginResult", false, "用户名或密码错误", null);
                return;
            }

            var passwordValid =  Password.VerifyPassword(user.PasswordHash, password);
            if (passwordValid)
            {
                // 如果验证成功，调用下面的方法
                var config = Context.GetHttpContext().RequestServices.GetRequiredService<IConfiguration>();
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SigningKey"]));
                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: new[] 
                    { 
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    },
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                await Clients.Client(Context.ConnectionId).SendAsync("LoginResult", true, "成功", jwtToken);
                _logger.LogInformation("用户 {UserName} 登录成功", user.UserName);
            }
            else
            {
                // 登录失败，返回错误信息
                await Clients.Client(Context.ConnectionId).SendAsync("LoginResult", false, "用户名或密码错误", null);
                _logger.LogInformation("用户 {UserName} 登录失败", user.UserName);
            }
        }
        public async Task GetDatabaseUpdates(DateTime lastUpdate)
        {
            var updates1 = _context.IMMessages
                .Where(m => m.SentAt > lastUpdate && (m.ReceiverName == Context.User.Identity.Name || m.SenderName == Context.User.Identity.Name))
                .ToList();
            var updates2 = _context.UserRelations
                .Where(r => r.User1.UserName == Context.User.Identity.Name || r.User2.UserName == Context.User.Identity.Name)
                .ToList();
            await Clients.Caller.SendAsync("HandleDatabaseUpdates", updates1, updates2);
        }
    }
}