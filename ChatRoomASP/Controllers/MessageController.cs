using Microsoft.AspNetCore.Mvc;
using ChatRoomASP.Models;
using Microsoft.Data.Sqlite; // 添加 Sqlite 命名空间引用

namespace ChatRoomASP.Controllers
{
  public class MessageController : Controller
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public MessageController(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    // GET: MessageController
    public ActionResult Index()
    {
      var messages = _context.IMMessages.ToList();
      return View(messages);
    }

    // GET: MessageController/Create
    public ActionResult Create()
    {
      return View();
    }

    public ActionResult Users()
    {
      var users = _context.IMUsers.ToList(); // 假设数据库存在 Users 表
      return View(users);
    }

    // POST: MessageController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IMMessage message)
    {
      if (ModelState.IsValid)
      {
        _context.IMMessages.Add(message);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
      }
      return View(message);
    }

    // POST: MessageController/ExecuteSql
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ExecuteSql(string sqlStatement)
    {
      if (string.IsNullOrWhiteSpace(sqlStatement))
      {
        ViewBag.SqlMessage = "SQL statement cannot be empty.";
        ViewBag.SqlSuccess = false;
        return View("Create");
      }

      try
      {
        using (var connection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"))) // 替换为 SqliteConnection
        {
          connection.Open();
          using (var command = new SqliteCommand(sqlStatement, connection)) // 替换为 SqliteCommand
          {
            command.ExecuteNonQuery();
          }
        }
        ViewBag.SqlMessage = "SQL statement executed successfully.";
        ViewBag.SqlSuccess = true;
      }
      catch (Exception ex)
      {
        ViewBag.SqlMessage = $"Error executing SQL statement: {ex.Message}";
        ViewBag.SqlSuccess = false;
      }

      return View("Create");
    }
  }
}