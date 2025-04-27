using System.Security.Claims;
using ChatRoomASP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoomASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppDbContext _context;

        public AccountController(ILogger<AccountController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 创建用户
                var user = new IMUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    PasswordHash = Password.HashPassword(model.Password),
                    LastActiveTime = DateTimeOffset.Now,
                    CreatedAt = DateTimeOffset.Now,
                    UpdatedAt = DateTimeOffset.Now
                };
                _context.IMUsers.Add(user);
                await _context.SaveChangesAsync();

                // 创建与自己好友关系
                var selfRelation = new UserRelation
                {
                    InitiatorUserId = user.UserId,
                    TargetUserId = user.UserId,
                    RelationTypeId = 1, // 假设1表示好友关系
                    RemarkName = "自己",
                    CreatedAt = DateTimeOffset.Now,
                    UpdatedAt = DateTimeOffset.Now
                };
                _context.UserRelations.Add(selfRelation);
                await _context.SaveChangesAsync();

                // 手动实现登录逻辑（如设置 Cookie）
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email)
                };
                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("CookieAuth", principal);

                _logger.LogInformation("创建了新用户和密码");
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // 查找用户
                var user = _context.IMUsers.FirstOrDefault(u => u.UserName == viewModel.Name);
                if (user != null && Password.VerifyPassword(viewModel.Password, user.PasswordHash))
                {
                    // 创建 Claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    // 创建 ClaimsIdentity
                    var identity = new ClaimsIdentity(claims, "CookieAuth");

                    // 创建 ClaimsPrincipal
                    var principal = new ClaimsPrincipal(identity);

                    // 登录用户（设置 Cookie）
                    await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
                    {
                        IsPersistent = viewModel.RememberMe // 是否持久化 Cookie
                    });

                    _logger.LogInformation("用户登入");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("无效的登录尝试");
                    ModelState.AddModelError(string.Empty, "无效的登录尝试");
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("用户已登录");
            return RedirectToAction("Index", "Home");
        }
    }
}