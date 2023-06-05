using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data.Identity;
using Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class AuthController : AcadnetController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            ILogger<AuthController> logger,
            SignInManager<User> signInManager
        )
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public IActionResult Login(string returnUrl = default!)
        {
            return View(nameof(Login), returnUrl);
        }

        public IActionResult Signup(string returnUrl = default!)
        {
            return View(nameof(Signup), returnUrl);
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #region ExternalAuth
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                _logger.LogError($"Error from external provider: {remoteError}");
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Error loading external login information.");
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User logged in with {info.LoginProvider} provider.");
                // redirect to returnUrl
                return Redirect(returnUrl ?? "/");
            }
            if (result.IsLockedOut)
            {
                _logger.LogError("User account locked out.");
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                var user = new User { UserName = email, Email = email, FirstName = firstName!, LastName = lastName! };
                var createResult = await _signInManager.UserManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    createResult = await _signInManager.UserManager.AddLoginAsync(user, info);
                    if (createResult.Succeeded)
                    {
                        createResult = await _signInManager.UserManager.AddToRoleAsync(user, UserRole.User);
                        if (createResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, false);
                            _logger.LogInformation($"User created an account using {info.LoginProvider} provider.");
                            // redirect to returnUrl
                            return Redirect(returnUrl ?? "/");
                        }
                    }
                }
                foreach (var error in createResult.Errors)
                {
                    AddError(error.Description);
                }
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
        }
        #endregion
    }
}