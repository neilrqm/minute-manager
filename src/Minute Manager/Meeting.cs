using System;
using System.Collections.Generic;
using System.Text;

namespace MinuteManager
{
    public class Meeting
    {
        /// <summary>
        /// Empty constructor for deserialization.
        /// </summary>
        public Meeting() { }
        /// <summary>
        /// Create a new Meeting object.
        /// </summary>
        /// <param name="id">Unique identifier for this meeting</param>
        /// <param name="number">Index of the meeting within the year (e.g. the value for the first meeting of the year is 1, etc.)</param>
        public Meeting(int id, int number)
        {
            Id = id;
            MeetingNumber = number;
            Date = DateTime.Now;
            Duration = new TimeSpan(2, 0, 0).Ticks;
            Members = new List<int>();
            AgendaItems = new List<int>();
        }

        /// <summary>
        /// Unique ID number for this meeting
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Meeting number within the year (e.g. the value for the first meeting of the year is 1, etc.).
        /// </summary>
        public int MeetingNumber { get; set; }
        /// <summary>
        /// Date that the meeting started.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Duration of the meeting in ticks (convert to time with new TimeSpan(Duration).
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        /// List of member IDs of those who attended the meeting
        /// </summary>
        public List<int> Members { get; set; }
        /// <summary>
        /// List of agenda item IDs of those that were active for the meeting.
        /// </summary>
        public List<int> AgendaItems { get; set; }
    }
}
