using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddDbContext<OnlineShopContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineShopContext") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found.")))
    .AddDbContext<OnlineShopUserContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineShopContext") ?? throw new InvalidOperationException("Connection string 'OnlineShopContext' not found.")));

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

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapRazorPages();
});

app.Run();
