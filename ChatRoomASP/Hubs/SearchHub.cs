using ChatRoomASP.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatRoomASP.Hubs;

public class SearchHub : Hub
{
    private readonly ILogger<SearchHub> _logger;
    private readonly AppDbContext _context;
    public SearchHub(ILogger<SearchHub> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // TODO 添加群聊搜索
    public async Task SearchUserName(string Name)
    {
        var user = await _context.Users.Where(u => u.UserName == Name).FirstOrDefaultAsync();

        if (user != null)
        {
            await Clients.Caller.SendAsync("SearchResult", user);
            _logger.LogInformation("搜索用户: {Name}", user.UserName);
        }
        else
        {
            await Clients.Caller.SendAsync("SearchResult", "用户未找到。");
            _logger.LogInformation("搜索用户: {Name}", "用户未找到。");
        }
    }

    public async Task SearchUserId(int Id)
    {
        var user = await _context.Users.Where(u => u.UserId == Id).FirstOrDefaultAsync();
        if (user != null)
        {
            await Clients.Caller.SendAsync("SearchResult", user);
            _logger.LogInformation("搜索用户: {Name}", user.UserName);
        }
        else
        {
            await Clients.Caller.SendAsync("SearchResult", "用户未找到。");
            _logger.LogInformation("搜索用户: {Name}", "用户未找到。");
        }
    }
}
