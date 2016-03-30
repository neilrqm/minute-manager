using System;
using System.Collections.Generic;
using System.Text;

namespace MinuteManager
{
    public class Year
    {
        public Year()
        {
            Members = new List<Member>();
            MeetingIds = new List<int>();
        }
        public Year(int lsaYear)
        {
            LsaYear = lsaYear;
            Members = new List<Member>();
            MeetingIds = new List<int>();
        }
        public int LsaYear
        {
            get; set;
        }
        public List<Member> Members
        {
            get;
            set;
        }
        public List<int> MeetingIds
        {
            get;
            set;
        }
    }
}