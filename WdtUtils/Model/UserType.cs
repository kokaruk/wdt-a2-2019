using WdtUtils.Utils;

namespace WdtUtils.Model
{
    public enum UserType
    {
        [StringValue("Student")] Student,
        [StringValue("Staff")] Staff
    }

    public static class UserConstants
    {
        public const string Student = "Student";
        public const string Staff = "Staff";
    }
}