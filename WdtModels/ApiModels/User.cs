using System.ComponentModel.DataAnnotations;

namespace WdtModels.ApiModels
{
    public class User
    {
        [Key, Display(Name = "User ID"), StringLength(8)]
        public string UserID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

    }
}
