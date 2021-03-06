﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WdtModels.ApiModels
{
    public class Slot
    {
        [JsonIgnore] public virtual Room Room { get; set; }

        [ForeignKey("Room"), Display(Name = "Room")]
        public string RoomID { get; set; }

        public virtual User Staff { get; set; }

        [Required, ForeignKey("User")] public string StaffID { get; set; }

        [Display(Name = "Start Time"), DisplayFormat(DataFormatString = "{0:d-MM-yyy   hh tt}")]
        public DateTime StartTime { get; set; }

        public virtual User Student { get; set; }

        [ForeignKey("User"), Column("BookedInStudentID")]
        public string StudentID { get; set; }
    }
}