using System;
using System.Net.Http;

namespace WdtApiLogin.Repo
{
    public interface IApiService
    {
        IFaqService Faq { get; }

        IUserService Users { get; }
    }

    public class ApiService : IApiService
    {
        private readonly Lazy<HttpClient> _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = new Lazy<HttpClient>(() => httpClient);
            Users = new UserService(this._httpClient);
            Faq = new FaqService(this._httpClient);
        }
        
        public IFaqService Faq { get; }

        public IUserService Users { get; }
    }
}
