using Heavy.Web.Models;
using Heavy.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heavy.Web.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return View(roles);
        }

        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleAddViewModel roleAddViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(roleAddViewModel);
            }

            var role = new IdentityRole
            {
                Name = roleAddViewModel.RoleName
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                

            }

            return View(roleAddViewModel);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return RedirectToAction("Index");
            }

            var roleEditViewModel = new RoleEditViewModel
            {
                Id = id,
                RoleName = role.Name,
                Users = new List<string>()
            };

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    roleEditViewModel.Users.Add(user.UserName);
                }
            }

            return View(roleEditViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditRole(RoleEditViewModel roleEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(roleEditViewModel);
            }

            var role = await _roleManager.FindByIdAsync(roleEditViewModel.Id);

            if (role != null)
            {
                role.Name = roleEditViewModel.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(roleEditViewModel);

                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occur when delete role");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Can not find role");
            }

            return View("Index", await _roleManager.Roles.ToListAsync());
        }

        public async Task<IActionResult> AddUserToRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return RedirectToAction("Index");
            }

            var vm = new UserRoleViewModel
            {
                RoleId = role.Id
            };

            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    vm.Users.Add(user);
                }
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId);
            var role = await _roleManager.FindByIdAsync(userRoleViewModel.RoleId);

            if (user != null && role != null)
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(userRoleViewModel);
                }
            }

            ModelState.AddModelError(string.Empty, "Can not find user or role");
            return View(userRoleViewModel);
        }

        //远程验证
        [AcceptVerbs("Get","Post")]
        public async Task<IActionResult> CheckRoleExist([Bind("RoleName")] string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                return Json(false);
            }

            return Json(true);
        }
    }

}
