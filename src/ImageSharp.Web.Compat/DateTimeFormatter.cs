using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SixLabors.ImageSharp.Web
{
    internal static class DateTimeFormatter
    {
        private static readonly DateTimeFormatInfo FormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;
        private static readonly string[] MonthNames = DateTimeFormatter.FormatInfo.AbbreviatedMonthNames;
        private static readonly string[] DayNames = DateTimeFormatter.FormatInfo.AbbreviatedDayNames;
        private static readonly int Rfc1123DateLength = "ddd, dd MMM yyyy HH:mm:ss GMT".Length;
        private static readonly int QuotedRfc1123DateLength = DateTimeFormatter.Rfc1123DateLength + 2;
        private const int AsciiNumberOffset = 48;
        private const string Gmt = "GMT";
        private const char Comma = ',';
        private const char Space = ' ';
        private const char Colon = ':';
        private const char Quote = '"';

        public static string ToRfc1123String(this DateTimeOffset dateTime)
        {
            return dateTime.ToRfc1123String(false);
        }

        public static string ToRfc1123String(this DateTimeOffset dateTime, bool quoted)
        {
            DateTime utcDateTime = dateTime.UtcDateTime;
            var target = new StringBuilder(quoted ? DateTimeFormatter.QuotedRfc1123DateLength : DateTimeFormatter.Rfc1123DateLength);
            if (quoted)
            {
                target.Append(Quote);
            }

            target.Append(DateTimeFormatter.DayNames[(int)utcDateTime.DayOfWeek]);
            target.Append(Comma);
            target.Append(Space);
            DateTimeFormatter.AppendNumber(ref target, utcDateTime.Day);
            target.Append(Space);
            target.Append(DateTimeFormatter.MonthNames[utcDateTime.Month - 1]);
            target.Append(Space);
            DateTimeFormatter.AppendYear(ref target, utcDateTime.Year);
            target.Append(Space);
            DateTimeFormatter.AppendTimeOfDay(ref target, utcDateTime.TimeOfDay);
            target.Append(Space);
            target.Append(Gmt);
            if (quoted)
            {
                target.Append(Quote);
            }

            return target.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendYear(ref StringBuilder target, int year)
        {
            target.Append(DateTimeFormatter.GetAsciiChar(year / 1000));
            target.Append(DateTimeFormatter.GetAsciiChar((year % 1000) / 100));
            target.Append(DateTimeFormatter.GetAsciiChar((year % 100) / 10));
            target.Append(DateTimeFormatter.GetAsciiChar(year % 10));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendTimeOfDay(ref StringBuilder target, TimeSpan timeOfDay)
        {
            DateTimeFormatter.AppendNumber(ref target, timeOfDay.Hours);
            target.Append(Colon);
            DateTimeFormatter.AppendNumber(ref target, timeOfDay.Minutes);
            target.Append(Colon);
            DateTimeFormatter.AppendNumber(ref target, timeOfDay.Seconds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendNumber(ref StringBuilder target, int number)
        {
            target.Append(DateTimeFormatter.GetAsciiChar(number / 10));
            target.Append(DateTimeFormatter.GetAsciiChar(number % 10));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char GetAsciiChar(int value)
        {
            return (char)(AsciiNumberOffset + value);
        }
    }
}
