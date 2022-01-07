using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gosu.Common
{
    public static partial class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt) =>
            dt.FirstDayOfWeek().AddDays(6);

        public static DateTime FirstDayOfMonth(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1);

        public static DateTime FirstDayOfQuarter(this DateTime dt, int quarterNum)
        {
            return new DateTime(dt.Year, quarterNum * 3 - 2, 1);
        }

        public static DateTime LastDayOfQuarter(this DateTime dt, int quarterNum)
        {
            if (quarterNum == 4)
            {
                return new DateTime(dt.Year, 12, 31);
            }
            else
            {
                return new DateTime(dt.Year, quarterNum * 3 + 1, 1).AddDays(-1);
            }
        }

        public static DateTime FirstDayOfThisYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }
        public static DateTime LastDayOfThisYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 12, 31);
        }
        public static DateTime EndOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
        }
        public static DateTime StartOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
        }
        public static DateTime MinDate(this DateTime dt)
        {
            return new DateTime(1900, 01, 01, 0, 0, 0, 0);
        }
        public static DateTime MaxDate(this DateTime dt)
        {
            return new DateTime(2900, 01, 01, 0, 0, 0, 0);
        }
        public static string MinShortDateString(this DateTime dt)
        {
            return "19000101";
        }
        public static string MaxShortDateString(this DateTime dt)
        {
            return "29000101";
        }
    }
}

