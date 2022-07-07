using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P225Allup.Areas.Manage.ViewModels.AccountViewModels;
using P225Allup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "dashboard", new { area = "manage" });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM) 
        {
            if (!ModelState.IsValid) return View();

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if(appUser == null)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View();
            }

            if (!await _userManager.CheckPasswordAsync(appUser, loginVM.Password))
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View();
            }

            await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);

            return RedirectToAction("index", "dashboard", new { area = "manage" });
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("login", "account", new { area = "manage" });
        }
    }
}
