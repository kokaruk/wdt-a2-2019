using WdtUtils.Utils;

namespace WdtUtils.Model
{
    public enum UserType
    {
        [StringValue("Generic Access")]
        Generic,
        [StringValue("Student")]
        Student,
        [StringValue("Staff")]
        Staff
    }
}