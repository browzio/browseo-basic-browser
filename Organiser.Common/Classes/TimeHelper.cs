using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;

namespace Organiser.Common.Classes
{
    #region structs for dll imports
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    /// <summary>
    /// The TimeZoneInformation structure specifies information specific to the time zone.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TimeZoneInformation
    {
        /// <summary>
        /// Current bias for local time translation on this computer, in minutes. The bias is the difference, in minutes, between Coordinated Universal Time (UTC) and local time. All translations between UTC and local time are based on the following formula: 
        /// <para>UTC = local time + bias</para>
        /// <para>This member is required.</para>
        /// </summary>
        public int bias;
        /// <summary>
        /// Pointer to a null-terminated string associated with standard time. For example, "EST" could indicate Eastern Standard Time. The string will be returned unchanged by the GetTimeZoneInformation function. This string can be empty.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string standardName;
        /// <summary>
        /// A SystemTime structure that contains a date and local time when the transition from daylight saving time to standard time occurs on this operating system. If the time zone does not support daylight saving time or if the caller needs to disable daylight saving time, the wMonth member in the SystemTime structure must be zero. If this date is specified, the DaylightDate value in the TimeZoneInformation structure must also be specified. Otherwise, the system assumes the time zone data is invalid and no changes will be applied.
        /// <para>To select the correct day in the month, set the wYear member to zero, the wHour and wMinute members to the transition time, the wDayOfWeek member to the appropriate weekday, and the wDay member to indicate the occurence of the day of the week within the month (first through fifth).</para>
        /// <para>Using this notation, specify the 2:00a.m. on the first Sunday in April as follows: wHour = 2, wMonth = 4, wDayOfWeek = 0, wDay = 1. Specify 2:00a.m. on the last Thursday in October as follows: wHour = 2, wMonth = 10, wDayOfWeek = 4, wDay = 5.</para>
        /// </summary>
        public SYSTEMTIME standardDate;
        /// <summary>
        /// Bias value to be used during local time translations that occur during standard time. This member is ignored if a value for the StandardDate member is not supplied. 
        /// <para>This value is added to the value of the Bias member to form the bias used during standard time. In most time zones, the value of this member is zero.</para>
        /// </summary>
        public int standardBias;
        /// <summary>
        /// Pointer to a null-terminated string associated with daylight saving time. For example, "PDT" could indicate Pacific Daylight Time. The string will be returned unchanged by the GetTimeZoneInformation function. This string can be empty.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string daylightName;
        /// <summary>
        /// A SystemTime structure that contains a date and local time when the transition from standard time to daylight saving time occurs on this operating system. If the time zone does not support daylight saving time or if the caller needs to disable daylight saving time, the wMonth member in the SystemTime structure must be zero. If this date is specified, the StandardDate value in the TimeZoneInformation structure must also be specified. Otherwise, the system assumes the time zone data is invalid and no changes will be applied.
        /// <para>To select the correct day in the month, set the wYear member to zero, the wHour and wMinute members to the transition time, the wDayOfWeek member to the appropriate weekday, and the wDay member to indicate the occurence of the day of the week within the month (first through fifth).</para>
        /// </summary>
        public SYSTEMTIME daylightDate;
        /// <summary>
        /// Bias value to be used during local time translations that occur during daylight saving time. This member is ignored if a value for the DaylightDate member is not supplied. 
        /// <para>This value is added to the value of the Bias member to form the bias used during daylight saving time. In most time zones, the value of this member is –60.</para>
        /// </summary>
        public int daylightBias;
    }
    #endregion

    public class DateAndTimeZone
    {
        public DateTime Date { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
    }

    public class TimeHelper
    {
        public static DateAndTimeZone GetTimeOfProxy(string ip, string port, string username, string pass)
        {
            WebRequest request = WebRequest.Create(@"http://time.is/");
            if (!string.IsNullOrEmpty(ip) && !string.IsNullOrWhiteSpace(ip) && !string.IsNullOrEmpty(port) && !string.IsNullOrWhiteSpace(port))
                request.Proxy = new WebProxy(ip, Convert.ToInt32(port));
            else
                return new DateAndTimeZone() { TimeZone = TimeZoneInfo.Local };

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(pass) && !string.IsNullOrWhiteSpace(pass))
                request.Proxy.Credentials = new NetworkCredential(username, pass);    

            WebResponse response = request.GetResponse();

            DateTime dt = DateTime.Now;
            TimeZoneInfo timeZone = TimeZoneInfo.Local; 

            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                string html = stream.ReadToEnd();

                string time = html.Split(new string[] { @"<div id=""twd"">" }, StringSplitOptions.None)[1];
                time = time.Substring(0, time.IndexOf(@"</div>"));
                if (time.Contains("<span id=\"ampm\" style=\"font-size:21px;line-height:21px\">"))
                {
                    time = time.Split(new string[] { "<span id=\"ampm\" style=\"font-size:21px;line-height:21px\">" }, StringSplitOptions.None)[0] + " " +
                        time.Split(new string[] { "<span id=\"ampm\" style=\"font-size:21px;line-height:21px\">" }, StringSplitOptions.None)[1];
                    time = time.Substring(0, time.IndexOf("</span>"));
                }

                string date = html.Split(new string[] { @"title=""Click for calendar"">" }, StringSplitOptions.None)[1];
                date = date.Substring(0, date.IndexOf("</div>"));

                string timezone = html.Split(new string[] { @"<span>Time zone: </span>" }, StringSplitOptions.None)[1];
                timezone = timezone.Split(new string[] { @"<a href=""/" }, StringSplitOptions.None)[1];
                timezone = timezone.Substring(0, timezone.IndexOf("\">"));

                string id = html.Split(new string[] { @"Time zone identifier: " }, StringSplitOptions.None)[1];
                id = id.Substring(0, id.IndexOf("<"));


                foreach (var item in TimeZoneInfo.GetSystemTimeZones())
                {
                    if (item.DisplayName.Replace(":", "").Replace("0", "").Contains(timezone))
                    {
                        timeZone = item;
                        break;
                    }
                }

                dt = Convert.ToDateTime(date);
                dt = dt.Date + Convert.ToDateTime(time).TimeOfDay;
            }

            return new DateAndTimeZone() { Date = dt, TimeZone = timeZone };
        }

        public static void SetOriginalTimeZonesFromFile()
        {
            string dir = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "OriginalDateTimes");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string file = System.IO.Path.Combine(dir, "dttzinfo.txt");

            if (!File.Exists(file)) return;
            StartSetTimeAndZoneProcess(new DateAndTimeZone() { TimeZone = TimeZoneInfo.FromSerializedString(File.ReadAllText(file)) });    
        }  

        public static TimeZoneInfo GetOldTZFromFile()
        {
            string dir = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "OriginalDateTimes");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string file = System.IO.Path.Combine(dir, "dttzinfo.txt");
            if (!File.Exists(file)) return TimeZoneInfo.Local;

            return TimeZoneInfo.FromSerializedString(File.ReadAllText(file));
        }

        public static void StartSetTimeAndZoneProcess(DateAndTimeZone dt)
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "tzutil.exe",
                    Arguments = "/s \"" + dt.TimeZone.Id + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                if (process != null)
                {
                    process.WaitForExit();
                    try
                    {
                        process.Kill();
                    }
                    catch { }
                    TimeZoneInfo.ClearCachedData();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to set system time make sure your system time is up to date or refresh the session settings to try again. Reason: " + ex.Message);
            }
        }    

    }
}


//#region time
//[DllImport("kernel32.dll", SetLastError = true)]
//private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

//[DllImport("kernel32.dll", SetLastError = true)]
//private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

//[DllImport("kernel32.dll")]
//static extern uint GetLastError();

//public static SYSTEMTIME GetTimeHour()
//{
//    // Call the native GetSystemTime method 
//    // with the defined structure.
//    SYSTEMTIME stime = new SYSTEMTIME();
//    GetSystemTime(ref stime);

//    return stime;

//    // Show the current time.           
//    //MessageBox.Show("Current Time: " +
//    //    stime.wHour.ToString() + ":"
//    //    + stime.wMinute.ToString());
//}
//public static void SetTime(SYSTEMTIME hour)
//{
//    // Call the native GetSystemTime method 
//    // with the defined structure.
//    //SYSTEMTIME systime = new SYSTEMTIME();
//    //GetSystemTime(ref systime);

//    // Set the system clock ahead one hour.
//    //systime.wHour = hour;//(ushort)(systime.wHour + 1 % 24);
//    var ret = SetSystemTime(ref hour);
//    uint s = GetLastError();
//    //MessageBox.Show("New time: " + systime.wHour.ToString() + ":"
//    //    + systime.wMinute.ToString());
//}
//#endregion

//#region timezones
///// <summary>
///// [Win32 API call]
///// The GetTimeZoneInformation function retrieves the current time-zone parameters. 
///// These parameters control the translations between Coordinated Universal Time (UTC) 
///// and local time.
///// </summary>
///// <param name="lpTimeZoneInformation">[out] Pointer to a TIME_ZONE_INFORMATION structure to receive the current time-zone parameters.</param>
///// <returns>
///// If the function succeeds, the return value is one of the following values.
///// <list type="table">
///// <listheader>
///// <term>Return code/value</term>
///// <description>Description</description>
///// </listheader>
///// <item>
///// <term>TIME_ZONE_ID_UNKNOWN == 0</term>
///// <description>
///// The system cannot determine the current time zone. This error is also returned if you call the SetTimeZoneInformation function and supply the bias values but no transition dates. 
///// This value is returned if daylight saving time is not used in the current time zone, because there are no transition dates.
///// </description>
///// </item>
///// <item>
///// <term>TIME_ZONE_ID_STANDARD == 1</term>
///// <description>
///// The system is operating in the range covered by the StandardDate member of the TIME_ZONE_INFORMATION structure.
///// </description>
///// </item>
///// <item>
///// <term>TIME_ZONE_ID_DAYLIGHT == 2</term>
///// <description>
///// The system is operating in the range covered by the DaylightDate member of the TIME_ZONE_INFORMATION structure.
///// </description>
///// </item>
///// </list>
///// If the function fails, the return value is TIME_ZONE_ID_INVALID. To get extended error information, call GetLastError.
///// </returns>
//[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
//private static extern int GetTimeZoneInformation(out TimeZoneInformation lpTimeZoneInformation);

///// <summary>
///// [Win32 API call]
///// The SetTimeZoneInformation function sets the current time-zone parameters. 
///// These parameters control translations from Coordinated Universal Time (UTC) 
///// to local time.
///// </summary>
///// <param name="lpTimeZoneInformation">[in] Pointer to a TIME_ZONE_INFORMATION structure that contains the time-zone parameters to set.</param>
///// <returns>
///// If the function succeeds, the return value is nonzero.
///// If the function fails, the return value is zero. To get extended error information, call GetLastError.
///// </returns>
//[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
//private static extern bool SetTimeZoneInformation([In] ref TimeZoneInformation lpTimeZoneInformation);

///// <summary>
///// Sets new time-zone information for the local system.
///// </summary>
///// <param name="tzi">Struct containing the time-zone parameters to set.</param>
//public static void SetTimeZone(TimeZoneInformation tzi)
//{
//    // set local system timezone
//    SetTimeZoneInformation(ref tzi);
//}

///// <summary>
///// Gets current timezone information for the local system.
///// </summary>
///// <returns>Struct containing the current time-zone parameters.</returns>
//public static TimeZoneInformation GetTimeZone()
//{
//    // create struct instance
//    TimeZoneInformation tzi;

//    // retrieve timezone info
//    int currentTimeZone = GetTimeZoneInformation(out tzi);

//    return tzi;
//}
//#endregion
