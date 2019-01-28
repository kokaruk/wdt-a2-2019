using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WdtModels.ApiModels
{
    public class Room
    {
        [Key, StringLength(10)]
        public string RoomID { get; set; }

        public virtual ICollection<Slot> Slots { get; set; }
    }
}
