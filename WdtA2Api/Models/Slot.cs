using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace WdtA2Api.Models
{
    public class Slot
    {
        [ForeignKey("Room"), Display(Name = "Room Id")]
        public string RoomID { get; set; }
        [JsonIgnore]
        public virtual Room Room { get; set; }

        public DateTime StartTime { get; set; }

        [Required, ForeignKey("User")]
        public string StaffID { get; set; }

        public virtual User Staff {get; set;}

        [ForeignKey("User"), Column("BookedInStudentID")]
        public string StudentID { get; set; }
        public virtual User Student { get; set; }

    }
}
