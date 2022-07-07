using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.Models;
using P225Allup.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                FatherName = registerVM.FatherName,
                Age = registerVM.Age,
                Email = registerVM.Email,
                UserName = registerVM.UserName
            };

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                return View();
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

            return RedirectToAction("login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View();
            }

            //if (!await _userManager.CheckPasswordAsync(appUser, loginVM.Password))
            //{
            //    ModelState.AddModelError("", "Email Or Password Is InCorrect");
            //    return View();
            //}

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabiniz Bloklanib");
                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View();
            }

            return RedirectToAction("index", "home");

        }

        [Authorize(Roles ="Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users
                .Include(u=>u.Orders).ThenInclude(o=>o.OrderItems).ThenInclude(oi=>oi.Product)
                .FirstOrDefaultAsync(u=>u.UserName == User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                FatherName = appUser.FatherName,
                Age = appUser.Age,
                UserName = appUser.UserName,
                Email = appUser.Email
            };

            MemberVM memberVM = new MemberVM
            {
                ProfileVM = profileVM,
                Orders = appUser.Orders.ToList()
            };

            return View(memberVM);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Update(ProfileVM profileVM)
        {
            if (!ModelState.IsValid) return View("Profile");

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.Name = profileVM.Name;
            appUser.SurName = profileVM.SurName;
            appUser.FatherName = profileVM.FatherName;
            appUser.UserName = profileVM.UserName;
            appUser.Email = profileVM.Email;
            appUser.Age = profileVM.Age;

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                return View("Profile");
            }

            if (profileVM.CurrentPassword != null)
            {
                if (profileVM.NewPassword == null)
                {
                    ModelState.AddModelError("NewPassword", "Is Requered");
                    ModelState.AddModelError("ConfirmPassword", "Is Requered");

                    return View("Profile");
                }

                if (!await _userManager.CheckPasswordAsync(appUser, profileVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current Password Is InCorrect");
                    return View("Profile");
                }

                identityResult = await _userManager.ChangePasswordAsync(appUser, profileVM.CurrentPassword, profileVM.NewPassword);

                if (!identityResult.Succeeded)
                {
                    foreach (var item in identityResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }

                    return View("Profile");
                }
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }

        #region Create Role
        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });

        //    return Content("Roles Successfuly Created");
        //}
        #endregion

        #region Create Super Admin
        //public async Task<IActionResult> CreateSuperAdmin()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        Email = "superadmin@allup.com",
        //        UserName = "SuperAdmin",
        //        Name = "Hamid",
        //        SurName = "Mamamdov",
        //        FatherName = "Vaqif",
        //        Age = 32
        //    };

        //    await _userManager.CreateAsync(appUser, "SuperAdmin@123");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Content("Super Admin Was Successfully Created");
        //}
        #endregion
    }
}
