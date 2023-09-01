using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

//[Authorize(Roles = "Administrator")]
public class UsersController : Controller
{
    private readonly UserManager<OnlineShopUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<OnlineShopUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult RoleList()
    {
        var roles = _roleManager.Roles;
        return View(roles);
    }

    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(OnlineShopUserRole role)
    {
        //判斷角色是否已存在
        var roleExist = await _roleManager.RoleExistsAsync(role.RoleName);
        if (!roleExist)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditRole(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return NotFound();
        }
        else
        {
            ViewBag.users = await _userManager.GetUsersInRoleAsync(role.Name);
        }
        return View(role);
    }

    [HttpPost]
    public async Task<IActionResult> EditRole(IdentityRole role)
    {
        if (role == null)
        {
            return NotFound();
        }
        else
        {
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UserList()
    {
        List<OnlineShopUserViewModel> userViewModels = new List<OnlineShopUserViewModel>();
        var AllUsers = _userManager.Users.ToList();
        foreach (var user in AllUsers)
        {
            userViewModels.Add(new OnlineShopUserViewModel
            {
                User = user,
                RoleName = string.Join("", await _userManager.GetRolesAsync(user))
            });
        }

        return View(userViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("ListUsers");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("ListUsers");

    }
}
