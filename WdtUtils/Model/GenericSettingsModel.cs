using System;
using System.Collections.Generic;
using System.Text;

namespace WdtUtils.Model
{
    public class GenericSettingsModel
    {
        public int FetchLines { get; set; }
        public int SlotDuration { get; set; }
        public int DailyStaffBookings { get; set; }
        public int DailyStudentBookings { get; set; }
        public int DailyRoomBookings { get; set; }
        public int WorkingHoursStart { get; set; }
        public int WorkingHoursEnd { get; set; }
    }
}
