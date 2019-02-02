using System;
using System.Net.Http;

namespace WdtApiLogin.Repo
{
    public interface IApiService
    {
        IFaqService Faq { get; }

        IUserService User { get; }

        ISlotService Slots { get; }

        IRoomService Room { get; }
    }

    public class ApiService : IApiService
    {
        private readonly Lazy<HttpClient> _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = new Lazy<HttpClient>(() => httpClient);
            User = new UserService(_httpClient);
            Faq = new FaqService(_httpClient);
            Slots = new SlotService(_httpClient);
            Room = new RoomService(_httpClient);
        }

        public IRoomService Room { get; }

        public IFaqService Faq { get; }

        public IUserService User { get; }

        public ISlotService Slots { get; }
    }
}