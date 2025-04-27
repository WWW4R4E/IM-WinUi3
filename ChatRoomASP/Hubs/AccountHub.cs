using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ChatRoomASP.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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

    public async Task Login(string userId, string password)
    {
      var user = _context.IMUsers.FirstOrDefault(u => u.UserId.ToString() == userId);
      if (user == null)
      {
        await Clients.Client(Context.ConnectionId).SendAsync("LoginResult", false, "用户名或密码错误", null);
        return;
      }

      var passwordValid = Password.VerifyPassword(user.PasswordHash, password);
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
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserId.ToString())
            },
            expires: DateTime.Now.AddHours(1),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        user.Status = 1;
        user.LastActiveTime = DateTime.Now;
        _context.IMUsers.Update(user); 
        await _context.SaveChangesAsync(); 
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
      _logger.LogInformation("用户 {UserName} 请求数据库更新", Context.User.Identity.Name);
      if (!long.TryParse(Context.User.Identity.Name, out var userId))
      {
        throw new InvalidOperationException("用户 ID 无效");
      }

      var updateMessages = await _context.IMMessages
        .Where(m =>  (m.ReceiverId == userId || m.SenderId == userId))
        .ToListAsync();
      updateMessages = updateMessages.Where(m => m.SentAt > lastUpdate).ToList();
      // 同步好友数据
      var updateUsers = await _context.UserRelations
        .Where(ur => 
            (ur.InitiatorUserId == userId || ur.TargetUserId == userId) && 
            ur.RelationTypeId == 1 // 只取好友关系
        )
        .Select(ur => new LocalUserDto // 直接映射到 LocalUser 类
        {
          UserId = ur.InitiatorUserId == userId ? ur.TargetUserId : ur.InitiatorUserId,
          Username = ur.InitiatorUserId == userId 
            ? ur.TargetUser.UserName 
            : ur.InitiatorUser.UserName,
          ProfilePicture = ur.InitiatorUserId == userId 
            ? ur.TargetUser.ProfilePicture 
            : ur.InitiatorUser.ProfilePicture,
          Status = (int)ur.TargetUser.Status, // 假设 Status 是 int 类型
          LastActiveTime = ur.TargetUser.LastActiveTime.UtcDateTime, // 转换为 DateTime
          LastInteractionTime = DateTimeOffset.Now.UtcDateTime, // 设置当前时间
          RemarkName = "", // 需要从数据库或业务逻辑获取
          RelationshipType = 1 // 固定为好友关系类型
        })
        .Distinct() // 去重
        .ToListAsync();
      // 序列化
      string updateUsersDto = JsonSerializer.Serialize(updateUsers);
      _logger.LogInformation(" 请求数据{UserName}", updateUsersDto);
      // 同步群组数据
      var updateGroups = _context.IMGroups
        .Where(g => g.Members.Any(m => m.UserId == userId))
        .ToList();

      await Clients.Caller.SendAsync("HandleDatabaseUpdates", updateUsers, updateMessages, updateGroups);
    }
  }
}