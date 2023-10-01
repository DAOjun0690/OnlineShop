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


//// ���|�N�{���X�q�ϥΰO���餺���֨��ܧ� Azure ���� Redis �֨��A�ӥB�|�ϥ� AZURE_REDIS_CONNECTIONSTRING ���e�� �C
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration["AZURE_REDIS_CONNECTIONSTRING"];
//    options.InstanceName = "SampleInstance";
//});


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
    .AddEntityFrameworkStores<OnlineShopContext>();

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



// �T�{ �W���ɮ׸�Ƨ� �O�_�s�b�A���b���ܡA�N��s�W
#if DEBUG
string uplaodFolder = app.Configuration["UploadFolder"];
#elif RELEASE
string uplaodFolder = "\\mounts\\" + app.Configuration["UploadFolder"];
#endif
if (!Directory.Exists(uplaodFolder))
{
    //�s�W��Ƨ�
    Directory.CreateDirectory(uplaodFolder);
}

// ��l�Ƹ��
await SeedData.SeedDatabase(app.Services.CreateAsyncScope().ServiceProvider);

app.Run();
