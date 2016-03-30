using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        private string tempDir;

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
            tempDir = Path.GetTempPath() + @"Minute Manager\";
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            Year x = CreateNewYear(GetCurrentLsaYear());
            Save();
            Year y = Load(x.LsaYear);
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

        /// <summary>
        /// Create new files for the current LSA year in the temp directory.
        /// </summary>
        /// <remarks>If there are already data for the given year in the temp directory, they will be overwritten.</remarks>
        /// <returns>The year number.</returns>
        public Year CreateNewYear(int year)
        {
            string yearTempDir = string.Format(@"{0}\{1}\", tempDir, year);
            if (Directory.Exists(yearTempDir))
            {
                // recursively delete the temp directory and its contents.
                Directory.Delete(yearTempDir, true);
            }
            Directory.CreateDirectory(yearTempDir);
            Year newYear = new Year(year);
            using (FileStream fs = new FileStream(yearTempDir + "year.xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Year), new Type[] { typeof(List<int>) });
                serializer.Serialize(fs, newYear);
            }
            // write empty list of meetings
            using (FileStream fs = new FileStream(yearTempDir + "meetings.xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Meeting>));
                serializer.Serialize(fs, new List<Meeting>());
            }
            // write empty list of items (todo: import open items from previous year)
            using (FileStream fs = new FileStream(yearTempDir + "items.xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));
                serializer.Serialize(fs, new List<Item>());
            }
            // write empty list of items (todo: import open items from previous year)
            using (FileStream fs = new FileStream(yearTempDir + "guidance.xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Guidance>));
                serializer.Serialize(fs, new List<Guidance>());
            }
            return newYear;
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
            // if all else fails, return the number of years between now and Ridvan, 1844
            if (now.Month == 4 && now.Day >= 19 && now.Day <= 22)
            {
                // if it's currently around Ridvan, then we're not sure what year it is.
                InternalError("Is it really after 2032???  If so, you need to add more years to the yearinfo.xml file in the appdata folder in your emulated copy of Windows running on your crazy 3D optical computron.  If not, maybe you should check your system clock, because I'm not sure what year it is.", false);
            }
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

        /// <summary>
        /// Save all the files in the temp directory to corresponding .mm files in the data directory.
        /// </summary>
        public void Save()
        {
            string[] years = Directory.GetDirectories(tempDir);
            int year;
            foreach (string y in years)
            {
                if (Int32.TryParse((new DirectoryInfo(y)).Name, out year))
                {
                    string outName = dataPath + string.Format("{0}.mm", year);
                    if (File.Exists(outName))
                    {
                        File.Delete(outName);
                    }
                    ZipFile.CreateFromDirectory(y, outName, CompressionLevel.Fastest, true);
                }
            }
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

        private Year Load(int year)
        {
            string yearTempDir = tempDir + string.Format(@"{0}\", year);
            // try to load data from the save file into the temp directory
            if (File.Exists(dataPath + string.Format("{0}.mm", year)))
            {
                if (Directory.Exists(yearTempDir))
                {
                    Directory.Delete(yearTempDir, true);
                }
                ZipFile.ExtractToDirectory(dataPath + string.Format("{0}.mm", year), tempDir);
            }
            // if there are data in the temp directory, then load the year.
            bool x = Directory.Exists(yearTempDir);
            bool y = File.Exists(yearTempDir + "year.xml");
            if (Directory.Exists(yearTempDir) && File.Exists(yearTempDir + "year.xml"))
            {
                using (FileStream fs = new FileStream(yearTempDir + "year.xml", FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Year));
                    return (Year)serializer.Deserialize(fs);
                }
            }
            // couldn't find any data for the year.
            return null;
        }

        private List<YearInfo> WriteDefaultYearData()
        {
            // Ridvan dates taken from http://www.religiouslife.emory.edu/documents/Baha_i%20Holy%20Days%2050%20year%20calendar.pdf
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
