using System;
using System.Collections.Generic;
using System.Text;

namespace MinuteManager
{
    public class InternalErrorEventArgs
    {
        public string Message { get; private set; }
        public bool Fatal { get; private set; }
        public InternalErrorEventArgs(string message, bool fatal) { Message = message; Fatal = fatal; }
    }
    public delegate void InternalErrorEventHandler(object system, InternalErrorEventArgs args);
    public class Controller
    {
        private IDataSource dataSource;
        private static Controller instance;
        private Controller()
        {
            dataSource = XmlDataSource.GetInstance();
            int currentYear = dataSource.GetCurrentLsaYear();
            if (!dataSource.YearsAvailable.Contains(currentYear))
            {
                dataSource.CreateNewYear(currentYear);
            }
        }
        public static Controller GetInstance()
        {
            if (instance == null)
            {
                instance = new Controller();
            }
            return instance;
        }

        public Meeting AddMeeting(Year year)
        {
            return dataSource.AddNewMeeting(year.LsaYear);
        }

        public event InternalErrorEventHandler OnInternalError
        {
            add { dataSource.OnInternalError += value; }
            remove { dataSource.OnInternalError -= value; }
        }

        public List<Year> YearsAvailable
        {
            get
            {
                List<Year> years = new List<Year>();
                foreach (int y in dataSource.YearsAvailable)
                {
                    years.Add(new Year(y));
                }
                return years;
            }
        }
    }
}
