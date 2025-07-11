using Microsoft.AspNetCore.Identity;
using ChatRoomASP.Hubs;
using ChatRoomASP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 添加SignalR服务
builder.Services.AddSignalR();

// 为控制器和视图添加服务
builder.Services.AddControllersWithViews();

// 添加CORS策略以允许所有来源、方法和头部信息
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// 配置数据库上下文
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加 Cookie 认证服务
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // 登录页面路径
        options.AccessDeniedPath = "/Account/AccessDenied"; // 无权限页面路径
    });

// 添加授权服务
builder.Services.AddAuthorization();

// 配置 JWT 令牌
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // 是否验证发行人
        ValidateAudience = false, // 是否验证受众
        ValidateLifetime = true, // 是否验证有效期
        ValidateIssuerSigningKey = true, // 是否验证签名
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"])) // 签名密钥
    };
});


// // 添加RabbitMQ客户端到服务集合
// builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

// // 注册 RabbitMQ 客户端工厂
// builder.Services.AddSingleton<IRabbitMqClient, RabbitMqClient>();

// 添加Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// 配置HTTP请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // 如果客户端也使用HTTPS，保留此行

app.UseRouting();

// 使用CORS策略
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// 注册SignalR Hub
app.MapHub<ChatHub>("/ChatHub");
app.MapHub<AccountHub>("/AccountHub");
app.MapHub<SearchHub>("/Searchhub");

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();