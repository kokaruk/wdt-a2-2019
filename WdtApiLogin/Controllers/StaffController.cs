using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WdtApiLogin.Areas.Identity.Data;
using WdtApiLogin.Models;
using WdtApiLogin.Repo;
using WdtModels.ApiModels;
using WdtUtils.Model;
using WdtUtils;

// ReSharper disable StyleCop.SA1201
namespace WdtApiLogin.Controllers
{
    [Authorize(Roles = UserConstants.Staff)]
    public class StaffController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IOptions<GenericSettingsModel> _genericSettings;
        private readonly UserManager<WdtApiLoginUser> _userManager;

        public StaffController(
            IApiService apiService,
            UserManager<WdtApiLoginUser> userManager,
            IOptions<GenericSettingsModel> genericSettings)
        {
            _apiService = apiService;
            _userManager = userManager;
            _genericSettings = genericSettings;
        }

        [TempData] public string GlobalStatusMessage { get; set; }

        [ViewData] public IEnumerable<SelectListItem> Rooms { get; set; }

        private IndexViewModel IndexOutput { get; } = new IndexViewModel();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("RoomID, StartTime")] Slot slot)
        {
            if (slot == null)
            {
                return NotFound();
            }

            try
            {
                var candidateId = $"{slot.RoomID}/{slot.StartTime:s}";
                var slotCandidate = await _apiService.Slots.GetAsync(candidateId);
                if (!string.IsNullOrWhiteSpace(slotCandidate.StudentID))
                {
                    GlobalStatusMessage = "Error deleting slot";
                    return NotFound();
                }

                var removed = await _apiService.Slots.RemoveAsync(candidateId);
                GlobalStatusMessage = $"Deleted slot room {removed.RoomID} at {removed.StartTime:hh:mm tt}";
            }
            catch (HttpRequestException)
            {
                GlobalStatusMessage = "Error deleting slot";
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Create()
        {
            ViewBag.MinDate = _genericSettings.Value.WorkingHoursEnd.MinDate();
            ViewBag.MinHour = _genericSettings.Value.WorkingHoursStart;
            ViewBag.MaxHour = _genericSettings.Value.WorkingHoursEnd;
            var rooms = await _apiService.Room.GetAllAsync();
            Rooms = rooms.Select(r => new SelectListItem {Text = r.RoomID});

            var input = TempData.Get<InputModel>("input");

            return input != null ? View(input) : View();
        }

        private async Task<string> CanCreate(Slot slot)
        {
            if (await _apiService.Slots.SlotExist(slot))
                return "Error. Slot already exists";
            var bookedSlot = await _apiService.Slots.StaffBookedThisTime(slot);
            if (bookedSlot != null)
                return $"Error. You already have a booking at {slot.StartTime:hh:mm tt} in room {bookedSlot.RoomID}";
            if (await _apiService.Slots.StaffMemberOverBookedForThisDay(slot, _genericSettings.Value.DailyStaffBookings)
            )
                return
                    $"Error. Staff member reached daily booking limit of {_genericSettings.Value.DailyStaffBookings}";
            if (await _apiService.Room.RoomReachedDailyLimit(slot, _genericSettings.Value.DailyRoomBookings))
                return $"Error. RoomID {slot.RoomID} reached maximum daily capacity";
            return string.Empty;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] InputModel input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var slotTime = input.StartDate + input.StartTime.TimeOfDay;
                    var slot = new Slot
                    {
                        RoomID = input.RoomID,
                        StaffID = user.UserName,
                        StartTime = DateTime.Parse($"{slotTime:yyyy-MM-dd HH:00:00}")
                    };

                    var msg = await CanCreate(slot);

                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        await _apiService.Slots.AddAsync(slot);
                        GlobalStatusMessage = "Successfully published new slot";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        GlobalStatusMessage = msg;
                        TempData.Put("input", input);

                        return RedirectToAction(nameof(Create));
                    }
                }
                catch (HttpRequestException)
                {
                    GlobalStatusMessage = "Error creating new slot.";
                    return RedirectToAction(nameof(Create));
                }
            }
            else
            {
                ViewBag.MinDate = _genericSettings.Value.WorkingHoursEnd.MinDate();
                ViewBag.MinHour = _genericSettings.Value.WorkingHoursStart;
                ViewBag.MaxHour = _genericSettings.Value.WorkingHoursEnd;

                var rooms = await _apiService.Room.GetAllAsync();
                Rooms = rooms.Select(r => new SelectListItem {Text = r.RoomID});

                return View(input);
            }
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var slots = await _apiService.Slots.FindAllAsync(
                slot => slot.StaffID == user.UserName && slot.StartTime > DateTime.Now);
            IndexOutput.Slots = slots.OrderBy(s => s.StartTime);
            return View(IndexOutput);
        }

        public async Task<IActionResult> RoomAvailability()
        {
            ViewBag.MinDate = _genericSettings.Value.WorkingHoursEnd.MinDate();
            var input = TempData.Get<CheckViewModel>("CheckView");
            var rooms = await _apiService.Room.GetAllAsync();
            Rooms = rooms.Select(r => new SelectListItem {Text = r.RoomID});
            return input != null ? View(input) : View();
        }

        [HttpPost, ActionName("Check")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoomAvailability([Bind] CheckViewModel input)
        {
            if (ModelState.IsValid)
            {
                var room = await _apiService.Room.GetAsync(input.RoomId);
                var slots = room.Slots.Where(s => s.StartTime.Date == input.StartDate.Date);
                input.Slots = slots;
                TempData.Put("CheckView", input);
                return RedirectToAction(nameof(RoomAvailability));
            }
            else
            {
                TempData.Put("CheckView", input);
                return RedirectToAction(nameof(RoomAvailability));
            }
        }

        public class InputModel
        {
            [Display(Name = "Room")]
            [Required(ErrorMessage = "Room Id can't be empty")]
            [StringLength(2, ErrorMessage = "maximum 2 characters")]
            public string RoomID { get; set; }

            [Display(Name = "Slot Date")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd-MM-yyy}", ApplyFormatInEditMode = true)]
            public DateTime StartDate { get; set; }

            [Display(Name = "Slot Time")]
            [DataType(DataType.Time)]
            [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
            [CorrectTimeRange(9, 15)]
            public DateTime StartTime { get; set; }
        }

        public class IndexViewModel
        {
            public Slot Slot { get; set; }
            public IEnumerable<Slot> Slots { get; set; } = new List<Slot>();

            public InputModel InputModel { get; set; }
        }

        public class CheckViewModel
        {
            [Display(Name = "Room")]
            [Required(ErrorMessage = "Room Id can't be empty")]
            [StringLength(2, ErrorMessage = "maximum 2 characters")]
            public string RoomId { get; set; }

            [Display(Name = "Slot Date")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd-MM-yyy}", ApplyFormatInEditMode = true)]
            [Required]
            public DateTime StartDate { get; set; }

            public Room Room { get; set; }

            public IEnumerable<Slot> Slots { get; set; }
        }
    }
}