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

// ���|�N�{���X�q�ϥΰO���餺���֨��ܧ� Azure ���� Redis �֨��A�ӥB�|�ϥ� AZURE_REDIS_CONNECTIONSTRING ���e�� �C
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
    options.InstanceName = "SampleInstance";
});
#endif

// �ҥ� Session
builder.Services.AddSession();

builder.Services.AddDefaultIdentity<OnlineShopUser>(options =>
{
    // �K�X����
    options.Password.RequiredLength = 4;
    // �]�t�p�g�^��
    options.Password.RequireLowercase = false;
    // �]�t�j�g�^��
    options.Password.RequireUppercase = false;
    // �]�t�Ÿ�
    options.Password.RequireNonAlphanumeric = false;
    // �]�t�Ʀr
    options.Password.RequireDigit = false;
})
    .AddRoles<IdentityRole>() //����
    .AddEntityFrameworkStores<OnlineShopUserContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapRazorPages();
});

// seed data �n�[�J category �� admin�޲z��
// ���[�@�Ӱ򥻺����W�h ����domain chinchin.studio
// �C��product �̭��n�� �ڦ��������Bimg ���|�B�q��

// Update-Database -Context OnlineShopContext

// ��l�Ƹ��
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<OnlineShopContext>();
SeedData.SeedDatabase(context);

app.Run();
