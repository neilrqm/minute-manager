using System;
using System.Collections.Generic;
using System.Text;

namespace MinuteManager
{
    public interface IDataSource
    {
        event InternalErrorEventHandler OnInternalError;
        List<int> YearsAvailable { get; }
        Year CreateNewYear(int year);
        int GetCurrentLsaYear();
        List<Meeting> GetMeetingsForYear(int year);
        List<Member> GetMembersForYear(int year);
        Meeting AddNewMeeting();
        void RemoveMeeting(Meeting meeting);
        Item CreateItem();
        void Save();
        void SaveTempMeeting(Meeting meeting);
    }
}
