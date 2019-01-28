using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Repo;

using WdtModels.ApiModels;

using WdtUtils;

namespace WdtApiLogin.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<WdtApiLoginUser> _signInManager;
        private readonly UserManager<WdtApiLoginUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IApiService _apiService;

        public ExternalLoginModel(
            SignInManager<WdtApiLoginUser> signInManager,
            UserManager<WdtApiLoginUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IApiService apiService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _apiService = apiService;
        }

        [TempData]
        public string GlobalStatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {

                var regex = new Regex(@"^e\d{5}@rmit.edu.au$|^s\d{7}@student.rmit.edu.au$");

                if (regex.IsMatch(Input.Email))
                {
                    var userName = Regex.Match(Input.Email, @"^e\d{5}|^s\d{7}").Value;

                    var user = new WdtApiLoginUser { UserName = userName, Email = Input.Email, Name = info.Principal.Identity.Name };
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            /*
                             * begin inject code
                             * need to check if user exists in outer data storage
                             */
                            try
                            {
                                var apiUser = await this._apiService.Users.FindAsync(u => u.UserID == userName);
                                if (apiUser == null)
                                {
                                    apiUser = new User { Email = user.Email, Name = user.Name, UserID = user.UserName };
                                    var response = await this._apiService.Users.AddAsync(apiUser);
                                    await _userManager.AddToRoleAsync(user, user.Email.GetUserRoleFromUserName());
                                }
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                                GlobalStatusMessage = "Successfully Created new user account!";
                                return LocalRedirect(returnUrl);
                            }
                            catch (HttpRequestException)
                            {
                                // exception caught can't create user for some reason
                                var deleteResult = await this._userManager.DeleteAsync(user);
                                GlobalStatusMessage = "Error. User Already Exists";
                                ModelState.AddModelError(string.Empty, "Error. User Already Exists");

                                // return this.RedirectToPage();
                                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                            }
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ErrorMessage = "Your email is out of allowed scope";
                    return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
