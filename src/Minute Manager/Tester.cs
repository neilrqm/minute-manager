using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinuteManager
{
    public class Tester
    {
        static XmlDataSource dataSource;
        public static void RunTests()
        {
            bool result;
            result = CreateCurrentYear();
            result = result && CreateMeetings();
        }

        static bool CreateMeetings()
        {
            int year = 172;
            XmlDataSource.CleanData();
            dataSource = XmlDataSource.GetInstance();
            dataSource.CreateNewYear(year);
            Meeting m = dataSource.AddNewMeeting(year);
            m.Date = new DateTime(2015, 5, 5, 0, 0, 0);
            m.Duration = new TimeSpan(2, 15, 0).Ticks;
            dataSource.SaveTempMeeting(year, m);
            List<int> meetings = dataSource.GetMeetingsForYear(year);
            if (meetings.Count != 1) return false;
            Meeting n = dataSource.GetMeeting(m.Id);
            if (m.Date != n.Date) return false;
            if (m.Duration != n.Duration) return false;
            return true;
        }

        static bool CreateCurrentYear()
        {
            int currentYear;
            XmlDataSource.CleanData();
            dataSource = XmlDataSource.GetInstance();
            currentYear = dataSource.GetCurrentLsaYear();
            dataSource.CreateNewYear(currentYear);
            if (!Directory.Exists(XmlDataSource.DataPath)) return false;
            if (!Directory.Exists(XmlDataSource.TempDir)) return false;
            if (!File.Exists(XmlDataSource.DataPath + "yearinfo.xml")) return false;
            if (!File.Exists(XmlDataSource.DataPath + string.Format("{0}.mm", currentYear))) return false;
            return CheckTempYearDataExists(currentYear);
        }

        static bool CheckTempYearDataExists(int year)
        {
            string yearTempPath = XmlDataSource.TempDir + string.Format(@"{0}\", year);
            if (!Directory.Exists(yearTempPath)) return false;
            if (!File.Exists(yearTempPath + "year.xml")) return false;
            if (!File.Exists(yearTempPath + "meetings.xml")) return false;
            if (!File.Exists(yearTempPath + "items.xml")) return false;
            if (!File.Exists(yearTempPath + "guidance.xml")) return false;
            return true;
        }
    }
}
