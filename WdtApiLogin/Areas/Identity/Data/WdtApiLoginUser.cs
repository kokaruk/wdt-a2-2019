using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WdtApiLogin.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the WdtApiLoginUser class
    public class WdtApiLoginUser : IdentityUser
    {
        [PersonalData] public override string Email { get; set; }

        [PersonalData] public string Name { get; set; }
    }
}