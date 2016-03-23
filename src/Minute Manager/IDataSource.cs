using System;
using System.Collections.Generic;
using System.Text;

namespace MinuteManager
{
    public interface IDataSource
    {
        event InternalErrorEventHandler OnInternalError;
        List<int> YearsAvailable { get; }
        int CreateCurrentYear();
        int GetCurrentLsaYear();
        List<Meeting> GetMeetingsForYear(int year);
        List<Member> GetMembersForYear(int year);
        Meeting AddNewMeeting();
        void RemoveMeeting(Meeting meeting);
        Item CreateItem();
        void SaveMeeting(Meeting meeting);
        void SaveTempMeeting(Meeting meeting);
    }
}
