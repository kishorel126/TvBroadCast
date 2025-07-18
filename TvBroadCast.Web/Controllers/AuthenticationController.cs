using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TvBroadCast.Domain.Entities;
using TvBroadCast.Web.Hubs;
using TvBroadCast.Web.Models.ViewModel;

namespace TvBroadCast.Web.Controllers
{
    
    public class AuthenticationController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<BroadcastHub> _hubContext;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager , IEmailSender emailSender , IHubContext<BroadcastHub> hubContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = _emailSender;
            _hubContext = hubContext;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var role = await _userManager.GetRolesAsync(user);

                    if (role.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (role.Contains("Approver"))
                    {
                        return RedirectToAction("Index", "BroadCast");
                    }
                    else if (role.Contains("Scheduler"))
                    {
                        return RedirectToAction("Index", "BroadCast");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found.");

            }

            TempData["ShowForgetPassword"] = true;

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {

                TempData["ErrorMessage"] = "User with this email already exists!!!";
                return RedirectToAction("Register", "Authentication");
            }

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }
            }

            return View(model);

        }

        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Authentication");
            }
            return RedirectToAction("Index", "Home");
        }


        //Forget Password Code Generator
        [HttpPost]
        public async Task<IActionResult> SendForgotPasswordCode([FromBody] ForgotPwdCodeRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new { success = false, message = "Invalid Email" });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // For security, do not reveal if email is not found or not confirmed.
                return Ok(new { success = true, message = "If this email is registered, a reset code has been sent." });
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);


            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"Your reset password code is: <b>{code}</b><br/>Please copy this code to reset your password.<br/><br/>If you did not request this, please ignore this email.");

            return Ok(new { success = true, message = "Reset password code sent to your email." });
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordRequestViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Code) || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // For security, do not reveal if email is not found.
                return Ok(new { success = false, message = "Password reset failed." });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);

            if (result.Succeeded)
            {

                // Log user activity

                return Ok(new { success = true, message = "Password reset successfully." });
            }

            // Converting the error into a message
            var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = errorMsg });
        }
    }
}
