﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Core.Models;
using OnlineShop.Core.ViewModel;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<OnlineShopUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<OnlineShopUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 顯示 帳戶列表
    /// </summary>
    /// <param name="pageNumber">第幾頁</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> UserList(int pageNumber = 1)
    {
        IList<OnlineShopUserViewModel> userViewModels = new List<OnlineShopUserViewModel>();
        var AllUsers = _userManager.Users.OrderByDescending(x => x.RegistrationDate).ToList();
        foreach (var user in AllUsers)
        {
            userViewModels.Add(new OnlineShopUserViewModel
            {
                User = user,
                RoleName = string.Join("", await _userManager.GetRolesAsync(user))
            });
        }

        // 列表一頁顯示筆數
        int pageSize = 10;
        return View(PaginatedList<OnlineShopUserViewModel>.Create(userViewModels, pageNumber, pageSize));
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
            return RedirectToAction("UserList");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("UserList");

    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        ViewData["Roles"] = new SelectList(_roleManager.Roles, "Name", "Name", userRoles.FirstOrDefault());

        return View(user);
    }

    /// <summary>
    /// 更新使用者 基本資料 & 授權
    /// </summary>
    /// <param name="viewData">要更新 user Data 的 Model bind</param>
    /// <param name="selectedRole">設定的角色權限</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> EditUser(OnlineShopUser viewData, string selectedRole)
    {
        // 根據使用者ID查找使用者
        var user = await _userManager.FindByIdAsync(viewData.Id);
        if (user == null)
        {
            // 如果找不到使用者，返回NotFound結果
            return new NotFoundResult();
        }

        // 更新使用者資訊
        user.Name = user.Name;

        var resultU = await _userManager.UpdateAsync(user);

        // Update user's role
        var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);
        var resultR = await _userManager.AddToRoleAsync(user, selectedRole);

        if (resultU.Succeeded && resultR.Succeeded)
        {
            // 如果更新成功，返回Ok結果
            //return new OkResult();
            return RedirectToAction(nameof(UserList));
        }
        else
        {
            // 如果更新失敗，返回BadRequest結果並附帶錯誤信息
            return new BadRequestObjectResult(resultU.Errors);
        }

    }
}
