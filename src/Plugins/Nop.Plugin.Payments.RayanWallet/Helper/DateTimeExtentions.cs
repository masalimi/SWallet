using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.RayanWallet.Helper
{
    public static class DateTimeExtentions
    {
        public static string ToPersianDateTime(this DateTime now)
        {
            var persianCalendar = new System.Globalization.PersianCalendar();
            var year = persianCalendar.GetYear(DateTime.Now);
            var month = persianCalendar.GetMonthsInYear(year);
            var day = persianCalendar.GetDayOfMonth(DateTime.Now);
            var hour = persianCalendar.GetHour(DateTime.Now);
            var minute = persianCalendar.GetMinute(DateTime.Now);
            var second = persianCalendar.GetSecond(DateTime.Now);

            return string.Format("{3}:{4}:{5} {0}/{1}/{2}", year, month, day, hour, minute, second);
        }

        public static string ToPersianDate(this DateTime now)
        {
            var persianCalendar = new System.Globalization.PersianCalendar();
            var year = persianCalendar.GetYear(DateTime.Now);
            var month = persianCalendar.GetMonthsInYear(year);
            var day = persianCalendar.GetDayOfMonth(DateTime.Now);
            var hour = persianCalendar.GetHour(DateTime.Now);
            var minute = persianCalendar.GetMinute(DateTime.Now);
            var second = persianCalendar.GetSecond(DateTime.Now);

            return string.Format("{0}/{1}/{2}", year, month, day);
        }
    }
}
