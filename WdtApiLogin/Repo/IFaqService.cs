using System.Net.Http;

using WdtModels.ApiModels;

namespace WdtApiLogin.Repo
{
    public interface IFaqService : IRepository<Faq>
    {
    }

    public class FaqService : Repository<Faq>, IFaqService
    {
        public FaqService(HttpClient httpClient)
            : base("faq", httpClient)
        {
        }
    }

}
