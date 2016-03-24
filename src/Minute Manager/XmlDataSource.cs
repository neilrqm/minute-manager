using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MinuteManager
{
    public class XmlDataSource : IDataSource
    {
        public class YearInfo
        {
            public YearInfo() { }
            public YearInfo(int year, DateTime firstOfRidvan)
            {
                this.lsaYear = year;
                this.firstOfRidvan = firstOfRidvan;
            }
            public int lsaYear;
            public DateTime firstOfRidvan;
        }
        private string dataPath;

        public event InternalErrorEventHandler OnInternalError;

        public XmlDataSource()
        {
            dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Minute Manager\";
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            if (!File.Exists(dataPath + "yearinfo.xml"))
            {
                WriteDefaultYearData();
            }
            int x = GetCurrentLsaYear();
            return;
        }

        public List<int> YearsAvailable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Meeting AddNewMeeting()
        {
            throw new NotImplementedException();
        }

        public int CreateCurrentYear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the current LSA year.
        /// </summary>
        /// <remarks>Since the LSA year starts at Ridvan, this will not be the actual Baha'i year if the current
        /// date falls between Naw Ruz and Ridvan.  The "current year" is the year in which Ridvan last fell.
        /// The dates of Ridvan up to 189 B.E. are written to yearinfo.xml in the appdata directory, and if the
        /// system year's date is outside the range in that file then the year is guessed based on Ridvan
        /// being on April 21.</remarks>
        /// <returns>The current LSA year in B.E., or a guess if there was an error.</returns>
        public int GetCurrentLsaYear()
        {
            List<YearInfo> years = GetYearData();
            DateTime now = DateTime.Now;
            for (int i = 0; i < years.Count - 1; i++)
            {
                if (years[i + 1].firstOfRidvan > now)
                {
                    return years[i].lsaYear;
                }
            }
            InternalError("Is it really after 2032???  If so, you need to add more years to the yearinfo.xml file in the appdata folder in your emulated copy of Windows running on your crazy 3D optical computron.  If not, maybe you should check your system clock, because I'm not sure what year it is.", false);
            // if all else fails, return the number of years between now and Ridvan, 1844
            return (new DateTime(1, 1, 1)).Add(now - new DateTime(1844, 4, 21)).Year;
        }

        public Item CreateItem()
        {
            throw new NotImplementedException();
        }

        public List<Meeting> GetMeetingsForYear(int year)
        {
            throw new NotImplementedException();
        }

        public List<Member> GetMembersForYear(int year)
        {
            throw new NotImplementedException();
        }

        public void RemoveMeeting(Meeting meeting)
        {
            throw new NotImplementedException();
        }

        public void SaveMeeting(Meeting meeting)
        {
            throw new NotImplementedException();
        }

        public void SaveTempMeeting(Meeting meeting)
        {
            throw new NotImplementedException();
        }

        private void InternalError(string message, bool fatal)
        {
            if (OnInternalError != null)
            {
                OnInternalError(this, new InternalErrorEventArgs(message, fatal));
            }
        }

        private List<YearInfo> WriteDefaultYearData()
        {
            List<YearInfo> years = new List<YearInfo>();
            years.Add(new YearInfo(170, DateTime.Parse("April 21, 2013")));
            years.Add(new YearInfo(171, DateTime.Parse("April 21, 2014")));
            years.Add(new YearInfo(172, DateTime.Parse("April 21, 2015")));
            years.Add(new YearInfo(173, DateTime.Parse("April 20, 2016")));
            years.Add(new YearInfo(174, DateTime.Parse("April 20, 2017")));
            years.Add(new YearInfo(175, DateTime.Parse("April 21, 2018")));
            years.Add(new YearInfo(176, DateTime.Parse("April 21, 2019")));
            years.Add(new YearInfo(177, DateTime.Parse("April 20, 2020")));
            years.Add(new YearInfo(178, DateTime.Parse("April 20, 2021")));
            years.Add(new YearInfo(179, DateTime.Parse("April 21, 2022")));
            years.Add(new YearInfo(180, DateTime.Parse("April 21, 2023")));
            years.Add(new YearInfo(181, DateTime.Parse("April 20, 2024")));
            years.Add(new YearInfo(182, DateTime.Parse("April 20, 2025")));
            years.Add(new YearInfo(183, DateTime.Parse("April 21, 2026")));
            years.Add(new YearInfo(184, DateTime.Parse("April 21, 2027")));
            years.Add(new YearInfo(185, DateTime.Parse("April 20, 2028")));
            years.Add(new YearInfo(186, DateTime.Parse("April 20, 2029")));
            years.Add(new YearInfo(187, DateTime.Parse("April 20, 2030")));
            years.Add(new YearInfo(188, DateTime.Parse("April 21, 2031")));
            years.Add(new YearInfo(189, DateTime.Parse("April 20, 2032")));
            using (FileStream fs = new FileStream(dataPath + "yearinfo.xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<YearInfo>), new Type[] { typeof(YearInfo) });
                serializer.Serialize(fs, years);
            }
            return years;
        }
        /// <summary>
        /// Get dates of Ridvan for the next number of years.
        /// </summary>
        /// <returns>A list of objects mapping the Baha'i year to the date of Ridvan during that year.</returns>
        private List<YearInfo> GetYearData()
        {
            List<YearInfo> yearInfo;
            try
            {
                using (FileStream fs = new FileStream(dataPath + "yearinfo.xml", FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<YearInfo>), new Type[] { typeof(YearInfo) });
                    yearInfo = (List<YearInfo>)serializer.Deserialize(fs);
                }
            }
            catch
            {
                return WriteDefaultYearData();
            }
            return yearInfo;
        }
    }
}
