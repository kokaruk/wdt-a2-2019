using System.ComponentModel.DataAnnotations;

namespace WdtModels.ApiModels
{
    public class AccessLevel
    {
        [Key]
        public string Name { get; set; }
    }
}
