using System.ComponentModel.DataAnnotations;

namespace WdtModels.ApiModels
{
    public class AccessLevel
    {
        [Key, StringLength(30)] public string Name { get; set; }
    }
}