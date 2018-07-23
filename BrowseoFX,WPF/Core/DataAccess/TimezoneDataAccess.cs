using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.DataAccess
{
    public class TimezoneDataAccess
    {
        public int SetTimeZoneByProcess(string timeZoneDisplayName)
        {
            var tzSI = -1;
            try
            {
                var tzI = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(tz=>tz.DisplayName == timeZoneDisplayName);
                var timeZoneId = tzI.Id;
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "tzutil.exe",
                    Arguments = "/s \"" + timeZoneId + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                if (process != null)
                {
                    process.WaitForExit();
                    try
                    {
                        process.Kill();
                        process.Dispose();
                    }
                    catch { }
                    TimeZoneInfo.ClearCachedData();
                }
                tzSI = TimeZoneInfo.GetSystemTimeZones().IndexOf(tzI);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to set system time make sure your system time is up to date or refresh the session settings to try again. Reason: " + ex.Message);
            }

            return tzSI;
        }
    }
}
