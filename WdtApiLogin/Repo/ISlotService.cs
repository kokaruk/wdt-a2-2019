using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using WdtModels.ApiModels;

namespace WdtApiLogin.Repo
{
    public interface ISlotService : IRepository<Slot>
    {
    }

    public class SlotService : Repository<Slot>, ISlotService
    {
        public SlotService(Lazy<HttpClient> httpClient)
            : base("Slots", httpClient)
        {
        }
    }
}
