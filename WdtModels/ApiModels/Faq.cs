using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WdtModels.ApiModels
{
    public class Faq
    {
        public int Id { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string Answer { get; set; }

        public virtual AccessLevel Access { get; set; }

        [ForeignKey("AccessLevel"), Column("AccessLevel")]
        public string AccessName { get; set; }
    }
}
