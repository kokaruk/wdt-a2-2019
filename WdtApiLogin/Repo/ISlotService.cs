using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WdtApiLogin.Controllers;
using WdtModels.ApiModels;

namespace WdtApiLogin.Repo
{
    public interface ISlotService : IRepository<Slot>
    {
        Task<bool> SlotExist(Slot slot);
        Task<Slot> StaffBookedThisTime(Slot slot);
        Task<bool> StaffMemberOverBookedForThisDay(Slot slot, int dailyLimit);
        Task<bool> StudentOverBookedForThisDay(Slot slot, int dailyLimit);

    }

    public class SlotService : Repository<Slot>, ISlotService
    {
        public SlotService(Lazy<HttpClient> httpClient)
            : base("Slots", httpClient)
        {
        }

        public async Task<bool> SlotExist(Slot slot)
        {
            var externalSlot = await FindAsync(
                s => s.StartTime == slot.StartTime && s.RoomID == slot.RoomID);
            return externalSlot != null;
        }

        public async Task<Slot> StaffBookedThisTime(Slot slot)
        {
            return await FindAsync(
                s => s.StartTime == slot.StartTime && s.StaffID == slot.StaffID);
        }

        public async Task<bool> StaffMemberOverBookedForThisDay(Slot slot, int dailyLimit)
        {
            var staffBooked = await FindAllAsync(
                s => s.StartTime.Date == slot.StartTime.Date && s.StaffID == slot.StaffID);
            return staffBooked.Count() >= dailyLimit;
        }

        public async Task<bool> StudentOverBookedForThisDay(Slot slot, int dailyLimit)
        {
            var studentBooked = await FindAllAsync(
                s => s.StartTime.Date == slot.StartTime.Date && s.StudentID == slot.StudentID);
            return studentBooked.Count() >= dailyLimit;
        }
        
    }
}