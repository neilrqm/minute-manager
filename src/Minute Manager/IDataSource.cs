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
        List<int> GetMeetingsForYear(int year);
        List<int> GetMembersForYear(int year);
        Meeting AddNewMeeting(int year);
        void RemoveMeeting(int meeting);
        Item CreateItem();
        void Save();
        void SaveTempMeeting(int year, Meeting meeting);
        Meeting GetMeeting(int id);
    }
}
