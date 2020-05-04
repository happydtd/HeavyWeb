using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Heavy.Web.Models;
using Heavy.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Heavy.Web.Controllers
{

    //[Authorize(Roles ="Administrators")]
    [Authorize(Policy = "Administrators only")]
  
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserCreateViewModel userCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userCreateViewModel);
            }

            var user = new ApplicationUser
            {
                UserName = userCreateViewModel.UserName,
                Email = userCreateViewModel.Email,
                IdCardNo = userCreateViewModel.IdCardNo,
                BirthDate = userCreateViewModel.BirthDate
            };

            var result = await _userManager.CreateAsync(user, userCreateViewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", await _userManager.Users.ToListAsync());
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(userCreateViewModel);


        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occur when delete user");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Can not find user");
            }

            return View("Index", await _userManager.Users.ToListAsync());
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var userEditViewModel = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IdCardNo = user.IdCardNo,
                BirthDate = user.BirthDate,
                Claims = claims.Select(x=>x.Value).ToList()
            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userEditViewModel);
            }

            var user = await _userManager.FindByIdAsync(userEditViewModel.Id);

            if (user != null)
            {
                user.UserName = userEditViewModel.UserName;
                user.Email = userEditViewModel.Email;
                user.IdCardNo = userEditViewModel.IdCardNo;
                user.BirthDate = userEditViewModel.BirthDate;

                var result = await _userManager.UpdateAsync(user);

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

                    return View(userEditViewModel);

                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ManageClaims(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var vm = new ManageClaimsViewModel
            {
                UserId = Id,
                AllClaims = Heavy.Web.Data.ClaimTypes.AllClaimTypeList
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ManageClaims(ManageClaimsViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var claim = new IdentityUserClaim<string>
            {
                ClaimType = vm.ClaimId,
                ClaimValue = vm.ClaimId
            };

            user.Claims.Add(claim);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("EditUser", new { id = vm.UserId });
            }

            ModelState.AddModelError(string.Empty, "error occur when update user claim");
            return View(vm);
        }
     }
}