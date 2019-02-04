using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WdtUtils.Model;

namespace WdtUtils
{
    public static class MiscExtUtils
    {
        public static string GetUserRoleFromUserName(this string value)
        {
            return value.StartsWith('e') ? UserConstants.Staff : UserConstants.Student;
        }

        public static DateTime MinDate(this int cutoff)
        {
            return DateTime.Now.AddHours(1).Hour >= cutoff
                ? DateTime.Now.AddDays(1)
                : DateTime.Now;
        }

        public static string BuildConnectionString(this IConfiguration value)
        {
            try
            {
                var secrets = value.GetSection(nameof(DbSecrets)).Get<DbSecrets>();
                var sqlString =
                    new SqlConnectionStringBuilder(value.GetConnectionString("wdtA2"))
                    {
                        UserID = secrets.Uid, Password = secrets.Password
                    };
                return sqlString.ConnectionString;
            }
            catch (Exception)
            {
                var sqlString = new SqlConnectionStringBuilder(value.GetConnectionString("wdtA2Production"));
                return sqlString.ConnectionString;
            }
        }
    }

    // serializing temp data 
    // https://stackoverflow.com/questions/34638823/store-complex-object-in-tempdata
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string) o);
        }
    }
}