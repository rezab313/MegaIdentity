using MegaIdentity.Models;
using MegaIdentity.Services;
using MegaIdentity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace MegaIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Login(model); 
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", string.Join(Environment.NewLine, result.ToString()));
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Register(model);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", result.ToString());
            }
            return View();
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangePassword(HttpContext.User, model);

                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", result.ToString());
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Address()
        { 
            return View(await _userService.Address(User)?? new Dtos.AddressDto());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Address(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateAddress(User, model);

                if (result)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", result.ToString());
            }
            return View();
        }


    }
}
