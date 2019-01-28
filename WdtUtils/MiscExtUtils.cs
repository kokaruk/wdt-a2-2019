﻿using System;
using System.Data.SqlClient;

using Microsoft.Extensions.Configuration;

using WdtUtils.Model;

namespace WdtUtils
{
    public static class MiscExtUtils
    {

        public static string GetUserRoleFromUserName(this string value) => value.StartsWith('e') ? UserConstants.Staff : UserConstants.Student;

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
}
