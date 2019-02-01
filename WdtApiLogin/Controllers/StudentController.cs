using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Models;
using WdtApiLogin.Repo;
using WdtModels.ApiModels;
using WdtUtils;
using WdtUtils.Model;

namespace WdtApiLogin.Controllers
{
    [Authorize(Roles = UserConstants.Student)]
    public class StudentController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IOptions<GenericSettingsModel> _genericSettings;
        private readonly UserManager<WdtApiLoginUser> _userManager;

        public StudentController(IApiService apiService,
            UserManager<WdtApiLoginUser> userManager,
            IOptions<GenericSettingsModel> genericSettings)
        {
            _apiService = apiService;
            _userManager = userManager;
            _genericSettings = genericSettings;
        }

        private IndexViewModel IndexOutput { get; } = new IndexViewModel();

        [TempData] public string GlobalStatusMessage { get; set; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var slots = await _apiService.Slots.FindAllAsync(
                slot => slot.StudentID == user.UserName && slot.StartTime > DateTime.Now);
            IndexOutput.Slots = slots.OrderBy(s => s.StartTime);
            return View(IndexOutput);
        }

        public async Task<IActionResult> StaffAvailability()
        {
            var input = TempData.Get<CheckViewModel>("CheckView") ?? new CheckViewModel();

            try
            {
                var staffUsers = await _apiService.User.FindAllAsync(u => u.UserID.StartsWith('e'));
                input.AllStaff = staffUsers.Select(u => new SelectListItem {Text = u.Name, Value = u.UserID});
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(input);
        }

        [HttpPost, ActionName("Check")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StaffAvailability([Bind] CheckViewModel input)
        {
            try
            {
                var slots = await _apiService.Slots.FindAllAsync(s =>
                    s.StaffID == input.StaffId 
                    && s.StartTime > input.StartDate
                    && string.IsNullOrWhiteSpace(s.StudentID));
                input.Slots = slots;
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(nameof(Index));
            }
            
            TempData.Put("CheckView", input);
            return RedirectToAction(nameof(StaffAvailability));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unbook([Bind("RoomID, StartTime, StaffID")] Slot slot)
        {
            if (slot == null)
            {
                return NotFound();
            }

            try
            {
                var candidateId = $"{slot.RoomID}/{slot.StartTime:s}";
                var slotCandidate = await _apiService.Slots.GetAsync(candidateId);
                var user = await _userManager.GetUserAsync(User);

                if (slotCandidate.StaffID != slot.StaffID && slotCandidate.StudentID != user.UserName)
                {
                    GlobalStatusMessage = "Error updating slot";
                    return NotFound();
                }

                slotCandidate.StudentID = null;
                slotCandidate.Student = null;
                await _apiService.Slots.UpdateAsync(slotCandidate);
                GlobalStatusMessage = $"Unbooked slot room {slot.RoomID} at {slot.StartTime:hh:mm tt}";
            }
            catch (HttpRequestException)
            {
                const string message = "Error un-booking slot";
                GlobalStatusMessage = message;
                return NotFound(message);
            }

            return RedirectToAction(nameof(Index));
        }
        
        private async Task<string> CanBook(Slot slot)
        {
            var user = await _userManager.GetUserAsync(User);
            slot.StudentID = user.UserName;
            if (await _apiService.Slots.StudentOverBookedForThisDay(slot, _genericSettings.Value.DailyStudentBookings))
                return "Error. Student reached daily booking limit";
            return string.Empty;
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(CheckViewModel input)
        {
            if (input == null || !ModelState.IsValid) 
            {
                return NotFound();
            }

            TempData.Put("CheckView", new CheckViewModel{StaffId = input.StaffId});
            
            try
            {
                var candidateId = $"{input.Room.RoomID}/{input.StartDate:s}";
                var slotCandidate = await _apiService.Slots.GetAsync(candidateId);

                var msg = await CanBook(slotCandidate);
                
                if (string.IsNullOrWhiteSpace(msg))
                {
                    if (slotCandidate.StaffID == input.StaffId && string.IsNullOrWhiteSpace(slotCandidate.StudentID))
                    {
                        var user = await _userManager.GetUserAsync(User);
                        slotCandidate.StudentID = user.UserName;
                        await _apiService.Slots.UpdateAsync(slotCandidate);
                        GlobalStatusMessage = $"Booked slot room {slotCandidate.RoomID} at {slotCandidate.StartTime:hh:mm tt}";    
                    }
                    else
                    {
                        GlobalStatusMessage = "Error booking in slot";
                        return NotFound();
                    }
                }
                else
                {
                    GlobalStatusMessage = msg;
                    return RedirectToAction(nameof(StaffAvailability));
                }
                
            }
            catch (HttpRequestException)
            {
                const string message = "Error booking slot";
                GlobalStatusMessage = message;
                return NotFound(message);
            }
            return RedirectToAction(nameof(StaffAvailability));
        }
        
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public class IndexViewModel
        {
            public Slot Slot { get; set; }
            public IEnumerable<Slot> Slots { get; set; } = new List<Slot>();
        }

        public class CheckViewModel
        {
            [Display(Name = "Staff")] [Required] public string StaffId { get; set; }
            public IEnumerable<SelectListItem> AllStaff { get; set; }
            public IEnumerable<Slot> Slots { get; set; }
            [Required] public DateTime StartDate { get; set; }
            [Required] public Room Room { get; set; }
        }
    }
}