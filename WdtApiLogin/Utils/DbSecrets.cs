using System.Diagnostics.CodeAnalysis;

namespace WdtApiLogin.Utils
{
    /// <summary>
    /// user secrets accessor class
    /// based on some code from
    /// https://medium.com/@granthair5/how-to-add-and-use-user-secrets-to-a-net-core-console-app-a0f169a8713f
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class DbSecrets
    {
        public string Uid { get; set; }

        public string Password { get; set; }
    }
}
