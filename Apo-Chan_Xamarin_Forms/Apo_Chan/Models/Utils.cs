using Apo_Chan.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.IO;

namespace Apo_Chan.Models
{
    public static class Utils
    {
        /*===========================================================================*/
        /**
         * check if "end" DateTime is after "start" DateTime
         *
         * @param startDate Start date of appointment
         * @param startTime Start time of appointment
         * @param endDate End date of appointment
         * @param endTime End time of appointment
         * @return true if the end DateTime later than the start DateTime.
         */
        //
        public static bool CheckDateTimeContinuity(ReportItem report)
        {
            DateTime start = new DateTime(report.ReportStartDate.Year, report.ReportStartDate.Month, report.ReportStartDate.Day,
                report.ReportStartTime.Hours, report.ReportStartTime.Minutes, report.ReportStartTime.Seconds);

            DateTime end = new DateTime(report.ReportEndDate.Year, report.ReportEndDate.Month, report.ReportEndDate.Day,
                report.ReportEndTime.Hours, report.ReportEndTime.Minutes, report.ReportEndTime.Seconds);

            if (end.CompareTo(start) < 0)
            {
                end = start.Add(TimeSpan.FromHours(1));
                report.ReportEndDate = end.Date;
                report.ReportEndTime = end.TimeOfDay;
                return false;
            }
            return true;
        }

        public static void ConvertToLocalDateTime(ReportItem report)
        {
            report.ReportStartDate = report.ReportStartDate.ToUniversalTime().Date.Add(report.ReportStartTime).ToLocalTime();
            report.ReportEndDate = report.ReportEndDate.ToUniversalTime().Date.Add(report.ReportEndTime).ToLocalTime();

            report.ReportStartTime = report.ReportStartDate.TimeOfDay;
            report.ReportEndTime = report.ReportEndDate.TimeOfDay;
        }

        public static void ConvertToUtcDateTime(ReportItem report)
        {
            report.ReportStartDate
                = report.ReportStartDate.Date.Add(report.ReportStartTime).ToUniversalTime();
            report.ReportEndDate
                = report.ReportEndDate.Date.Add(report.ReportEndTime).ToUniversalTime();
            report.ReportStartTime = report.ReportStartDate.TimeOfDay;
            report.ReportEndTime = report.ReportEndDate.TimeOfDay;
        }

        public static Image ImageFromStream(Stream stream)
        {
            return new Image { Source = ImageSource.FromStream(() => stream) };
        }
        public static Image ImageFromBase64(string base64picture)
        {
            if(base64picture == null) { return null; }
            byte[] imageBytes = Convert.FromBase64String(base64picture);
            return new Image
            {
                Source = ImageSource.FromStream(() => new MemoryStream(imageBytes))
            };
        }
    }

    public static class UnixTime
    {
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /*===========================================================================*/
        /**
         * 現在時刻からUnixTimeを計算する.
         *
         * @return UnixTime.
         */
        public static long Now()
        {
            return (FromDateTime(DateTime.UtcNow));
        }

        /*===========================================================================*/
        /**
         * UnixTimeからDateTimeに変換.
         *
         * @param [in] unixTime 変換したいUnixTime.
         * @return 引数時間のDateTime.
         */
        public static DateTime FromUnixTime(long unixTime)
        {
            return UNIX_EPOCH.AddSeconds(unixTime).ToLocalTime();
        }

        /*===========================================================================*/
        /**
         * 指定時間をUnixTimeに変換する.
         *
         * @param [in] dateTime DateTimeオブジェクト.
         * @return UnixTime.
         */
        public static long FromDateTime(DateTime dateTime)
        {
            double nowTicks = (dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            return (long)nowTicks;
        }
    }
}
