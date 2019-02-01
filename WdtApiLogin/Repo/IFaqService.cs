using System;
using System.Net.Http;
using WdtModels.ApiModels;

namespace WdtApiLogin.Repo
{
    public interface IFaqService : IRepository<Faq>
    {
    }

    public class FaqService : Repository<Faq>, IFaqService
    {
        public FaqService(Lazy<HttpClient> httpClient)
            : base("faq", httpClient)
        {
        }
    }
}