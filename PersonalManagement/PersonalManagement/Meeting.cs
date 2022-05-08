using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalManagement
{
    class Meeting
    {
        private DateTime dateTimeStart;
        private DateTime dateTimeEnd;
        private string description;
        private DateTime notification;
        public static List<Meeting> meetings = new List<Meeting>();

        public Meeting(DateTime start, DateTime end, string desc, DateTime notice)
        {
            if (Check(start, end, notice))
            {
                dateTimeStart = start;
                dateTimeEnd = end;
                notification = notice;
                description = desc;
                meetings.Add(this);
            }
        }

        public void SetDateTimeStart(DateTime start)
        {
            dateTimeStart = start;
        }

        public DateTime GetDateTimeStart()
        {
            return dateTimeStart;
        }

        public void SetDateTimeEnd(DateTime end)
        {
            dateTimeEnd = end;
        }

        public string GetDescription()
        {
            return description;
        }

        public void SetDescription(string desc)
        {
            description = desc;
        }

        public DateTime GetDateTimeEnd()
        {
            return dateTimeEnd;
        }

        public void SetNotification(DateTime notice)
        {
            notification = notice;
        }

        public DateTime GetNotification()
        {
            return notification;
        }

        public void DeleteMeeting()
        {
            meetings.Remove(this);
        }

        public bool Check(DateTime start, DateTime end, DateTime notice)
        {
            if (start > end || start < DateTime.Now || notice <= DateTime.Now || notice >= start)
            {
                return false;
            }
            foreach (Meeting meet in meetings)
            {
                if (meet == this)
                {
                    continue;
                }
                if ((meet.dateTimeStart <= start && meet.dateTimeEnd >= start) || (meet.dateTimeStart <= end && meet.dateTimeEnd >= end))
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetMeetings(DateTime dt)
        {
            Console.Clear();
            int i = 1;
            string result = "";
            foreach (Meeting meet in meetings)
            {
                if (meet.dateTimeStart.Year != dt.Year || meet.dateTimeStart.Month != dt.Month || meet.dateTimeStart.Day != dt.Day)
                {
                    continue;
                }
                result += "Встреча №" + i + "\n";
                result += "Дата и время начала: " + meet.GetDateTimeStart().ToString() + "\n";
                result += "Дата и время окончания: " + meet.GetDateTimeEnd().ToString() + "\n";
                result += "Описание: " + meet.GetDescription().ToString() + "\n";
                result += "Оповещение: " + meet.GetNotification().ToString() + "\n\n";
                i++;
            }
            if (i == 1)
            {
                result = "На данную дату встречи не запланированы/не проводились";
            }
            return result;
        }
    }
}
