using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace WdtA2Api.Utils
{
    /// <summary>
    /// user secrets accessor class
    /// based on some code from
    /// https://medium.com/@granthair5/how-to-add-and-use-user-secrets-to-a-net-core-console-app-a0f169a8713f
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DbSecrets
    {
        public string Uid { get; set; }

        public string Password { get; set; }
    }
}
