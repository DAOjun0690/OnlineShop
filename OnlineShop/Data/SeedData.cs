using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Core.Models;

namespace OnlineShop.Data;

/// <summary>
/// 產生預設資料
/// </summary>
public static class SeedData
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<OnlineShopContext>();
        await context.Database.MigrateAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<OnlineShopUser>>();
        var userStore = serviceProvider.GetRequiredService<IUserStore<OnlineShopUser>>();
        var emailStore = (IUserEmailStore<OnlineShopUser>)userStore;

        // 如果角色不存在，則新增
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // 如果管理帳號不存在，則新增
        if (await userManager.FindByNameAsync("admin") == null)
        {
            var user = new OnlineShopUser
            {
                UserName = "admin",
                Email = "admin@admin",
                Name = "admin",
                DOB = DateTime.UtcNow.AddHours(8),
                Gender = GenderType.Unknown,
                RegistrationDate = DateTime.Today
            };

            await userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, "password");
            if (result.Succeeded)
            {
                // 建立使用者，與給予其權限
                userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }

        // 檢查 分類 是否有資料
        if (!context.Category.Any())
        {
            List<Category> categorys = new List<Category> {
                new Category()
                {
                    Name = "紙膠帶"
                },
                new Category()
                {
                    Name = "印章"
                }
            };

            context.Category.AddRange(categorys);
            context.SaveChanges();
        }

        // 確認 上傳檔案資料夾 是否存在，不在的話，將其新增
        string uplaodFolder = Path.Join(Directory.GetCurrentDirectory(), "UploadFolder");
        if (!Directory.Exists(uplaodFolder))
        {
            //新增資料夾
            Directory.CreateDirectory(uplaodFolder);
        }
    }
}