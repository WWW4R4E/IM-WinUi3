using System.Text.Json;
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

  public async Task SearchUser(string searchTerm)
  {
    // 查询用户名匹配的结果
    var nameResults = await _context.Users
        .Where(u => u.UserName == searchTerm)
        .Select(u => new { u.UserId, u.UserName, u.ProfilePicture })
        .ToListAsync();

    // 判断 searchTerm 是否是数字，如果是则查询用户ID匹配的结果
    var idResults = new List<dynamic>();
    if (int.TryParse(searchTerm, out int userId))
    {
      idResults = await _context.Users
          .Where(u => u.UserId == userId)
          .Select(u => new { u.UserId, u.UserName, u.ProfilePicture })
          .ToListAsync<dynamic>();
    }

    // 合并结果并去重（基于 UserId）
    var combinedResults = nameResults
        .Concat(idResults)
        .GroupBy(u => u.UserId)
        .Select(g => g.First())
        .ToList();

    if (combinedResults.Any())
    {
      var resultJson = JsonSerializer.Serialize(combinedResults);
      await Clients.Caller.SendAsync("SearchResult", resultJson);
      _logger.LogInformation("搜索用户: {SearchTerm}", searchTerm);
    }
    else
    {
      await Clients.Caller.SendAsync("SearchResult", "用户未找到。");
      _logger.LogInformation("搜索用户: {SearchTerm}", "用户未找到。");
    }
    _logger.LogInformation("搜索用户: {SearchTerm}", searchTerm);
  }
}