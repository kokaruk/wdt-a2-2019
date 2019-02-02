using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WdtModels.ApiModels
{
    public class Faq
    {
        public int Id { get; set; }
        [Required] public string Question { get; set; }
        [Required] public string Answer { get; set; }

        [JsonIgnore] public virtual AccessLevel Access { get; set; }

        [ForeignKey("AccessLevel"), Column("AccessLevel"), StringLength(30)]
        public string AccessName { get; set; }
    }
}