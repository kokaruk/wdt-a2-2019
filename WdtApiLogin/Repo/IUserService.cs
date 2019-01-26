using System.Net.Http;

using WdtModels.ApiModels;

namespace WdtApiLogin.Repo
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
    /// </summary>
    public interface IUserService : IRepository<User>
    {
    }

    public class UserService : Repository<User>, IUserService
    {
        public UserService(HttpClient client) 
            : base("Users", client)
        {
        }
    }
}
