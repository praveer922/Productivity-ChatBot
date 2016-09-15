using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tasks
{
    public class Burden : IComparable<Burden>
    {
        public string taskName, category;
        public int startTime, duration, endTime;
        public DateTime date;
        public bool isFixedTask, isRepeat, isDeadline;
        public int urgency, importance;


        public Burden(string taskName, string category, int startTime, int endTime, int duration, string day, bool isRepeat, bool isDeadline)
        {
            this.taskName = taskName;
            this.category = category;
            this.startTime = startTime;
            this.endTime = endTime;
            this.duration = duration;
            this.date = processDay(day);
            this.isRepeat = isRepeat;
            this.isDeadline = isDeadline;
            if (startTime >= 0)
            {
                isFixedTask = true;
            }
            else
            {
                isFixedTask = false;
            }
            if (!isDeadline && !isFixedTask)
                urgency = -9900;
            else if (isDeadline && !isFixedTask)
                urgency = calculateUrgency(endTime);

              if (category.Equals("Health"))
              {
                  this.importance = 2;
              }
              else if (category.Equals("Work")){
                  importance = 1;
              }
              else
              {
                  importance = 0;
              }

        }

        private int calculateUrgency(int endTime)
        {
            return DateTime.Now.Hour * 100 - endTime;
        }


        public static DateTime processDay(string day)
        {
            day = day.ToLower();
            DateTime currDay = TaskList.TaskList.currDay;
            int currDayInt = (int)currDay.DayOfWeek;
            DateTime burdenDate;
            int diff;

            switch (day)
            {
                case "tomorrow":
                    burdenDate = currDay.AddDays(1);
                    break;
                case "monday":
                    burdenDate = currDay.AddDays(calculateDiff(currDayInt, 1));
                    break;
                case "tuesday":
                    burdenDate = currDay.AddDays(calculateDiff(currDayInt, 2));
                    break;
                case "wednesday":
                    burdenDate = currDay.AddDays(calculateDiff(currDayInt, 3));
                    break;
                case "thursday":
                    burdenDate = currDay.AddDays(calculateDiff(currDayInt, 4));
                    break;
                case "friday":
                    burdenDate = currDay.AddDays(calculateDiff(currDayInt, 5));
                    break;
                case "saturday":
                    burdenDate = currDay.AddDays(calculateDiff(currDayInt, 6));
                    break;
                case "sunday":
                    burdenDate = currDay.AddDays(calculateDiff(currDayInt, 7));
                    break;
                default:
                    burdenDate = currDay;
                    break;
            }
            return burdenDate;

        }
        public static int calculateDiff(int currDay, int targetDay)
        {
            if (targetDay >= currDay)
                return targetDay - currDay;
            else
            {
                return 7 - (currDay - targetDay);
            }
        }
        public string toString()
        {
            return "Task Name: " + taskName + "\n\nCategory: " + category +
                "\n\nStart time: " + startTime + "\n\nEnd Time: " + endTime
                + "\n\nDuration: " + duration
                + "\n\nDate: " + date + "\n\nIs task repeated: " + isRepeat + "\n\nIs there deadline: "
                + isDeadline + "\n\nIs task fixed: " + isFixedTask + "\n\nUrgency: " + urgency;
        }

        public int CompareTo(Burden other)
        {
             int primary = -1 * this.urgency.CompareTo(other.urgency);

             if (primary != 0)
             {
                return primary;
             }
              else
              {
                 return -1 * this.importance.CompareTo(other.importance);
            
            }
        }
    }
}
