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
        Task<bool> RoomReachedDailyLimit(Slot slot, int dailyLimit);
    }

    public class RoomService : Repository<Room>, IRoomService
    {
        public RoomService(Lazy<HttpClient> httpClient)
            : base("rooms", httpClient)
        {
        }

        public async Task<bool> RoomReachedDailyLimit(Slot slot, int dailyLimit)
        {
            var room = await FindAsync(r => r.RoomID == slot.RoomID);
            return room.Slots.Count(s => s.StartTime.Date == slot.StartTime.Date) >= dailyLimit;
        }
    }
}