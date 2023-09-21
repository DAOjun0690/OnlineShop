using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG 
builder.Services
    .AddDbContext<OnlineShopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineShopContext") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found.")))
    .AddDbContext<OnlineShopUserContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineShopContext") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found.")));
#elif RELEASE
builder.Services
    .AddDbContext<OnlineShopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found.")))
    .AddDbContext<OnlineShopUserContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found.")));

//// 它會將程式碼從使用記憶體內部快取變更為 Azure 中的 Redis 快取，而且會使用 AZURE_REDIS_CONNECTIONSTRING 先前的 。
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
//    options.InstanceName = "SampleInstance";
//});
#endif

// 啟用 Session
builder.Services.AddSession();

builder.Services.AddDefaultIdentity<OnlineShopUser>(options =>
{
    // 密碼長度
    options.Password.RequiredLength = 4;
    // 包含小寫英文
    options.Password.RequireLowercase = false;
    // 包含大寫英文
    options.Password.RequireUppercase = false;
    // 包含符號
    options.Password.RequireNonAlphanumeric = false;
    // 包含數字
    options.Password.RequireDigit = false;
})
    .AddRoles<IdentityRole>() //角色
    .AddEntityFrameworkStores<OnlineShopUserContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

    endpoints.MapRazorPages();
});

// 初始化資料
await SeedData.SeedDatabase(app.Services.CreateScope().ServiceProvider);

app.Run();
