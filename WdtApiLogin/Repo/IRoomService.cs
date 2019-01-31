using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using WdtModels.ApiModels;

namespace WdtApiLogin.Repo
{
    public interface IRoomService : IRepository<Room>
    {
    }

    public class RoomService : Repository<Room>, IRoomService
    {
        public RoomService(Lazy<HttpClient> httpClient)
            : base("rooms", httpClient)
        {
        }
    }
}
