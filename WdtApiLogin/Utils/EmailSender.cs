using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WdtAsrConsumer.Utils
{
    /// <summary>
    ///     based o code from
    ///     https://docs.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-2.2&tabs
    ///     =visual-studio
    /// </summary>
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}
