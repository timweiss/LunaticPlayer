using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaticPlayer.Database.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Outputs a string for usage in SQL queries.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>String formatted for SQL queries</returns>
        public static string ToSqlFormat(this DateTime dt)
        {
            string dateTimeFormat = "{0}-{1}-{2} {3}:{4}:{5}.{6}";
            return string.Format(dateTimeFormat, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }
    }
}
