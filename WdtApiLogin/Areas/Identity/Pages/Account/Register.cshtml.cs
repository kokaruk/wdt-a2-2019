using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<WdtApiLoginUser> _signInManager;
        private readonly UserManager<WdtApiLoginUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IApiService _apiService;

        public RegisterModel(
            UserManager<WdtApiLoginUser> userManager,
            SignInManager<WdtApiLoginUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IApiService apiService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _apiService = apiService;
        }

        [TempData]
        public string GlobalStatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {

            private const string RegExErrMessage = "Staff starts with a letter ‘e’ followed by 5 numbers. <br>"
                                                   + "Student starts with a letter ‘s’ followed by 7 numbers";

            private const string StringLengthError = "The {0} must be at least {2} and at max {1} characters long. <br>" + RegExErrMessage;

            [Required]
            [DataType(DataType.Text)]
            [StringLength(8, ErrorMessage = StringLengthError, MinimumLength = 6)]
            [Display(Name = "User ID")]
            [RegularExpression(@"^e\d{5}$|^s\d{7}$", ErrorMessage = RegExErrMessage)]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(24, MinimumLength = 2)]
            [Display(Name = "Name")]
            [RegularExpression(@"^[\w\s]+$", ErrorMessage = "Letters Only")]
            public string Name { get; set; }

            [Required]
            [StringLength(
                100,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 2)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                try
                {
                    var apiUser = await this._apiService.Users.FindAsync(u => u.UserID == Input.UserName);
                    if (apiUser != null)
                    {
                        GlobalStatusMessage = "Error. User Already Exists";
                        ModelState.AddModelError(string.Empty, "Error. User Already Exists");
                        
                        // return this.RedirectToPage();
                        return Page();
                    }
                }
                catch (HttpRequestException)
                {
                    // exception caught, user doesn't exist
                }
                
                var user = new WdtApiLoginUser
                               {
                                   UserName = Input.UserName,
                                                   Email = Input.UserName.StartsWith('s') 
                                                               ? Input.UserName + "@student.rmit.edu.au" 
                                                               : Input.UserName + "@rmit.edu.au",
                                                   Name = Input.Name.Trim()
                               };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    try
                    {
                        var apiUser = new User { Email = user.Email, Name = user.Name, UserID = user.UserName };
                        var response = await this._apiService.Users.AddAsync(apiUser);
                        await _userManager.AddToRoleAsync(user, user.Email.GetUserRoleFromUserName());
                    }
                    catch (HttpRequestException)
                    {
                        // exception caught can't create user for some reason
                        var deleteResult = await this._userManager.DeleteAsync(user);
                    }
                    finally
                    {
                        _logger.LogInformation("User created a new account with password.");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        GlobalStatusMessage = "Successfully Created new user account!";
                    }

                    return LocalRedirect(returnUrl);

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
