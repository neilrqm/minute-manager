using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

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

        private static XmlDataSource instance;

        public event InternalErrorEventHandler OnInternalError;

        public static XmlDataSource GetInstance()
        {
            if (instance == null)
            {
                instance = new XmlDataSource();
            }
            return instance;
        }

        private XmlDataSource()
        {
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }
            if (!File.Exists(DataPath + "yearinfo.xml"))
            {
                WriteDefaultYearData();
            }
            if (!Directory.Exists(TempDir))
            {
                Directory.CreateDirectory(TempDir);
            }
            foreach (int y in YearsAvailable)
            {
                Load(y);
            }
        }

        static XmlDataSource()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Minute Manager\";
            TempDir = Path.GetTempPath() + @"Minute Manager\";
        }

        /// <summary>
        /// Delete all stored data and destroy the XmlDataSource instance (used for testing).
        /// </summary>
        public static void CleanData()
        {
            if (Directory.Exists(DataPath))
            {
                Directory.Delete(DataPath, true);
            }
            if (Directory.Exists(TempDir))
            {
                Directory.Delete(TempDir, true);
            }
            instance = null;
        }

        public Meeting AddNewMeeting(int year)
        {
            List<int> yearMeetingIds = GetMeetingsForYear(year);
            int globalIndex = FindHighestMeetingId() + 1;
            int yearIndex = yearMeetingIds.Count + 1;
            Meeting m = new Meeting(globalIndex, yearIndex);
            SaveTempMeeting(year, m);
            return m;
        }

        /// <summary>
        /// Create new files for the current LSA year in the temp directory.
        /// </summary>
        /// <remarks>If there are already data for the given year in the temp directory, they will be overwritten.</remarks>
        /// <returns>The year number.</returns>
        public Year CreateNewYear(int year)
        {
            string yearTempDir = string.Format(@"{0}\{1}\", TempDir, year);
            if (Directory.Exists(yearTempDir))
            {
                // recursively delete the temp directory and its contents.
                Directory.Delete(yearTempDir, true);
            }
            Directory.CreateDirectory(yearTempDir);
            Year newYear = new Year(year);
            WriteXmlFileToTemp<Year>(year, newYear, new Type[] { typeof(List<int>) });
            WriteXmlFile<List<Meeting>>(year, new List<Meeting>());
            WriteXmlFile<List<Item>>(year, new List<Item>());
            WriteXmlFile<List<Guidance>>(year, new List<Guidance>());
            Save();
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

        public List<int> GetMeetingsForYear(int year)
        {
            // read list of meetings from temp directory
            return GetMeetingObjectsForYear(year).Select(x => x.Id).ToList();
        }

        public List<int> GetMembersForYear(int year)
        {
            throw new NotImplementedException();
        }

        public void RemoveMeeting(int meeting)
        {
            throw new NotImplementedException();
        }

        public Meeting GetMeeting(int id)
        {
            List<int> years = YearsAvailable;
            years.Reverse();
            foreach (int y in years)
            {
                List<Meeting> meetings = GetMeetingObjectsForYear(y);
                foreach (Meeting m in meetings)
                {
                    if (m.Id == id)
                    {
                        return m;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Save all the files in the temp directory to corresponding .mm files in the data directory.
        /// </summary>
        public void Save()
        {
            string[] years = Directory.GetDirectories(TempDir);
            int year;
            foreach (string y in years)
            {
                if (Int32.TryParse((new DirectoryInfo(y)).Name, out year))
                {
                    string outName = DataPath + string.Format("{0}.mm", year);
                    if (File.Exists(outName))
                    {
                        File.Delete(outName);
                    }
                    ZipFile.CreateFromDirectory(y, outName, CompressionLevel.Fastest, true);
                }
            }
        }

        /// <summary>
        /// Save a meeting to a year's temporary working directory.
        /// </summary>
        /// <param name="year">The meeting's year.</param>
        /// <param name="meeting">The meeting object to save.</param>
        public void SaveTempMeeting(int year, Meeting meeting)
        {
            List<Meeting> meetings = GetMeetingObjectsForYear(year);    // this handles the case where the year hasn't been loaded into the temp directory
            for (int i = 0; i < meetings.Count; i++)
            {
                if (meetings[i].Id == meeting.Id)
                {
                    meetings[i] = meeting;
                    WriteXmlFile<List<Meeting>>(year, meetings);
                    return;
                }
            }
            meetings.Add(meeting);
            WriteXmlFile<List<Meeting>>(year, meetings);
        }


        public List<int> YearsAvailable
        {
            get
            {
                List<int> years = new List<int>();

                string[] saveFiles = Directory.GetFiles(DataPath, "*.mm", SearchOption.TopDirectoryOnly);
                foreach (string f in saveFiles)
                {
                    int year;
                    if (int.TryParse(Path.GetFileNameWithoutExtension(f), out year))
                    {
                        years.Add(year);
                    }
                }

                return years;
            }
        }

        public static string DataPath { get; private set; }
        public static string TempDir { get; private set; }


        private void WriteXmlFile<T>(int year, T o)
        {
            WriteXmlFileToTemp<T>(year, o, null);
        }

        private void WriteXmlFileToTemp<T>(int year, T o, Type[] extraTypes)
        {
            string yearTempDir = string.Format(@"{0}\{1}\", TempDir, year);
            string xmlFileName = GetSaveFilename(typeof(T));
            using (FileStream fs = new FileStream(yearTempDir + xmlFileName, FileMode.Create))
            {
                XmlSerializer serializer;
                if (extraTypes != null)
                {
                    serializer = new XmlSerializer(typeof(T), extraTypes);
                }
                else
                {
                    serializer = new XmlSerializer(typeof(T));
                }
                serializer.Serialize(fs, o);
            }
        }

        private T ReadXmlFileFromTemp<T>(int year)
        {
            string yearTempDir = string.Format(@"{0}{1}\", TempDir, year);
            string xmlFileName = GetSaveFilename(typeof(T));
            using (FileStream fs = new FileStream(yearTempDir + xmlFileName, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                // return a list of IDs extracted from the deserialized meeting objects
                return (T)serializer.Deserialize(fs);
            }
        }

        private string GetSaveFilename(Type t)
        {
            string xmlFileName;
            if (t == typeof(Year)) xmlFileName = "year.xml";
            else if (t == typeof(List<Meeting>)) xmlFileName = "meetings.xml";
            else if (t == typeof(List<Item>)) xmlFileName = "items.xml";
            else if (t == typeof(List<Guidance>)) xmlFileName = "guidance.xml";
            else
            {
                throw new Exception("Tried to read or write an XML file for an unknown data structure.");
            }
            return xmlFileName;
        }

        private List<Meeting> GetMeetingObjectsForYear(int year)
        {
            string yearTempDir = string.Format(@"{0}\{1}\", TempDir, year);
            if (!Directory.Exists(yearTempDir))
            {
                if (Load(year) == null)
                {
                    CreateNewYear(year);
                }
            }
            // read list of meetings from temp directory
            return ReadXmlFileFromTemp<List<Meeting>>(year);
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
            string yearTempDir = TempDir + string.Format(@"{0}\", year);
            // try to load data from the save file into the temp directory
            if (File.Exists(DataPath + string.Format("{0}.mm", year)))
            {
                if (Directory.Exists(yearTempDir))
                {
                    Directory.Delete(yearTempDir, true);
                }
                ZipFile.ExtractToDirectory(DataPath + string.Format("{0}.mm", year), TempDir);
            }
            // if there are data in the temp directory, then load the year.
            if (Directory.Exists(yearTempDir) && File.Exists(yearTempDir + "year.xml"))
            {
                return ReadXmlFileFromTemp<Year>(year);
            }
            // couldn't find any data for the year.
            return null;
        }

        private int FindHighestMeetingId()
        {
            int maxId = 0;
            foreach (int y in YearsAvailable)
            {
                foreach (int m in GetMeetingsForYear(y))
                {
                    maxId = m > maxId ? m : maxId;
                }
            }
            return maxId;
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
            using (FileStream fs = new FileStream(DataPath + "yearinfo.xml", FileMode.Create))
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
                using (FileStream fs = new FileStream(DataPath + "yearinfo.xml", FileMode.Open))
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
