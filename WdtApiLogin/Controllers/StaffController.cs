using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

using WdtUtils.Model;

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
            this._apiService = apiService;
            this._userManager = userManager;
            this._genericSettings = genericSettings;
        }

        [TempData]
        public string GlobalStatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [ViewData]
        public IEnumerable<SelectListItem> Rooms { get; set; }

        private ViewModel Output { get; set; } = new ViewModel();

        public async Task<IActionResult> Create()
        {
            var minDate = DateTime.Now.Hour > _genericSettings.Value.WorkingHoursEnd
                              ? DateTime.Now.AddDays(1)
                              : DateTime.Now;
            ViewBag.MinDate = minDate;
            ViewBag.MinHour = this._genericSettings.Value.WorkingHoursStart;
            ViewBag.MaxHour = this._genericSettings.Value.WorkingHoursEnd;
            var rooms = await this._apiService.Room.GetAllAsync();
            Rooms = rooms.Select(r => new SelectListItem { Text = r.RoomID });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InputModel input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await this._userManager.GetUserAsync(User);
                    var slot = new Slot
                                   {
                                       RoomID = Input.Room,
                                       StaffID = user.UserName,
                                       StartTime = DateTime.Parse($"{Input.StartTime:yyyy-MM-dd HH:00:00}")
                                   };
                    var externalSlot = await this._apiService.Slots.FindAsync(
                        s => s.StartTime == slot.StartTime && s.RoomID == slot.RoomID);
                    if (externalSlot != null)
                    {
                        GlobalStatusMessage = "Error, Slot already exists";
                        return RedirectToAction(nameof(Create));
                    }
                    else
                    {
                        await this._apiService.Slots.AddAsync(slot);
                        GlobalStatusMessage = "Successfully published new slot";
                        return RedirectToAction(nameof(Index));
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
                var minDate = DateTime.Now.Hour > _genericSettings.Value.WorkingHoursEnd
                                  ? DateTime.Now.AddDays(1)
                                  : DateTime.Now;
                ViewBag.MinDate = minDate;
                ViewBag.MinHour = this._genericSettings.Value.WorkingHoursStart;
                ViewBag.MaxHour = this._genericSettings.Value.WorkingHoursEnd;

                var rooms = await this._apiService.Room.GetAllAsync();
                Rooms = rooms.Select(r => new SelectListItem { Text = r.RoomID });

                return View();
            }
        }

        public async Task<IActionResult> Index()
        {
            var user = await this._userManager.GetUserAsync(User);
            var slots = await this._apiService.Slots.FindAllAsync(
                            slot => slot.StaffID == user.UserName && slot.StartTime > DateTime.Now);
            Output.Slots = slots.OrderBy(s => s.StartTime);
            return View(Output);
        }

        public IActionResult RoomAvailability()
        {
            return View();
        }

        public class InputModel
        {
            [Display(Name = "Room Id")]
            [Required(ErrorMessage = "Room Id can't be empty")]
            [StringLength(2, ErrorMessage = "maximum 2 characters")]
            public string Room { get; set; }

            [Display(Name = "Slot Date")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:d-MM-yyy}", ApplyFormatInEditMode = true)]
            public DateTime StartDate { get; set; }

            [Display(Name = "Slot Time")]
            [DataType(DataType.Time)]
            [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
            [CorrectTimeRange(9, 15)]
            public DateTime StartTime { get; set; }
        }

        public class ViewModel
        {
            public IEnumerable<Slot> Slots { get; set; }
        }
    }
}
