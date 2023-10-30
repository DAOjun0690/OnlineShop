using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Core.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddDbContext<OnlineShopContext>(options =>
    {
#if DEBUG
        //options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineShopContext") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found."));
        options.UseSqlite(builder.Configuration.GetConnectionString("UseSqlite") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found."));
#elif RELEASE
        options.UseSqlite(builder.Configuration.GetConnectionString("Azure_UseSqlite") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found."));
#endif
    });

//builder.Services
//.AddDbContext<OnlineShopContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found.")))


//// 它會將程式碼從使用記憶體內部快取變更為 Azure 中的 Redis 快取，而且會使用 AZURE_REDIS_CONNECTIONSTRING 先前的 。
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
//    options.InstanceName = "SampleInstance";
//});

// 啟用 cache
builder.Services.AddResponseCaching();
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
    .AddEntityFrameworkStores<OnlineShopContext>();


// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<IFileService, FileService>();

// 登入頁面路徑
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});
// 將過大的js css進行gzip壓縮
builder.Services.AddResponseCompression(options =>
{
    //options.EnableForHttps = true;
    options.MimeTypes = new[] { "text/html", "text/css", "application/javascript" };
});

var app = builder.Build();

// 將過大的js css進行gzip壓縮進行gzip壓縮
app.UseResponseCompression();

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
app.UseResponseCaching();
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


// 確認 上傳檔案資料夾 是否存在，不在的話，將其新增
#if DEBUG
string uploadFolder = app.Configuration["UploadFolder"];
#elif RELEASE
string uploadFolder = "\\mounts\\" + app.Configuration["UploadFolder"];
#endif
if (!Directory.Exists(uploadFolder))
{
    //新增資料夾
    Directory.CreateDirectory(uploadFolder);
}

// 初始化資料
await SeedData.SeedDatabase(app.Services.CreateAsyncScope().ServiceProvider);

app.Run();
