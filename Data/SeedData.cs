using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using System.Runtime.Intrinsics.X86;

namespace OnlineShop.Data;

public class SeedData
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<OnlineShopContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<OnlineShopUser>>();

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
        // 目前無效中
        //if (await userManager.FindByNameAsync("admin") == null)
        //{
        //    var user = new OnlineShopUser
        //    {
        //        UserName = "admin",
        //        Email = "admin@a",
        //        Name = "admin",
        //        DOB = DateTime.UtcNow.AddHours(8),
        //        Gender = GenderType.Unknown,
        //        RegistrationDate = DateTime.Today
        //    };

        //    var result = await userManager.CreateAsync(user, "123456");
        //    if (result.Succeeded)
        //    {
        //        // 建立使用者，與給予其權限
        //        userManager.AddToRoleAsync(user, "Admin").Wait();
        //    }
        //}

        context.Database.Migrate();
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
    }
}