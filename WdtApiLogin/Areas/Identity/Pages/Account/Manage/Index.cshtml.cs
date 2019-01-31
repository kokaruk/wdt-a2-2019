using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Repo;

using WdtModels.ApiModels;

namespace WdtApiLogin.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<WdtApiLoginUser> _userManager;
        private readonly SignInManager<WdtApiLoginUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IApiService _apiService;

        public IndexModel(
            UserManager<WdtApiLoginUser> userManager,
            SignInManager<WdtApiLoginUser> signInManager,
            IEmailSender emailSender,
            IApiService apiService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _apiService = apiService;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [StringLength(24, MinimumLength = 2)]
            [Display(Name = "Name")]
            [RegularExpression(@"^[\w\s]+$", ErrorMessage = "Letters Only")]
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);

            Username = userName;
            Email = email;

            Input = new InputModel
            {
                Name = user.Name
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name.Trim();
            }

            try
            {
                var apiUser = new User { Email = user.Email, Name = user.Name, UserID = user.UserName };
                var response = await this._apiService.User.UpdateAsync(apiUser.UserID, apiUser);

                await _userManager.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";

            }
            catch (HttpRequestException)
            {
                // exception caught can't create user for some reason
                StatusMessage = "Error updating your profile";
            }

            return RedirectToPage();
        }

        // public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return Page();
        //     }
        //
        //     var user = await _userManager.GetUserAsync(User);
        //     if (user == null)
        //     {
        //         return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //     }
        //
        //
        //     var userId = await _userManager.GetUserIdAsync(user);
        //     var email = await _userManager.GetEmailAsync(user);
        //     var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //     var callbackUrl = Url.Page(
        //         "/Account/ConfirmEmail",
        //         pageHandler: null,
        //         values: new { userId = userId, code = code },
        //         protocol: Request.Scheme);
        //     await _emailSender.SendEmailAsync(
        //         email,
        //         "Confirm your email",
        //         $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        //
        //     StatusMessage = "Verification email sent. Please check your email.";
        //     return RedirectToPage();
        // }
    }
}
