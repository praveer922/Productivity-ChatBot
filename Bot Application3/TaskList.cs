using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Tasks;

namespace TaskList
{
    public class TaskList
    {

        public SortedDictionary<DateTime, ArrayList> calendar;
        public static DateTime currDay;

       public TaskList()
        {
            calendar = new SortedDictionary<DateTime, ArrayList>();
            currDay = DateTime.Now;
        }
        public void insertTask(Burden b)
        {
            DateTime key = b.date.Date;
            ArrayList arr;
            if (!calendar.ContainsKey(key))
            {
                arr = new ArrayList();
                arr.Add(b);
                calendar.Add(key, arr);
            }
            else
            {
                arr = calendar[key];
                arr.Add(b);
                calendar[key] = arr;
            }
        }

        public void removeTask(String task)
        {
            foreach (ArrayList arr in calendar.Values)
            {
                foreach (Burden b in arr)
                {
                    if (b.taskName.Equals(task))
                    {
                        arr.Remove(b);
                        if (arr.Count == 0)
                        {
                            calendar.Remove(b.date.Date);
                        }
                        else
                        {
                            calendar[b.date.Date] = arr;
                        }
                        return;
                    }
                }
            }
        }

        public void updateTask(String task, DateTime newDate, int newStart)
        {
            Burden newburden = null;
            bool updated = false;
            foreach (ArrayList arr in calendar.Values)
            {
                if (updated) break;
                foreach (Burden b in arr)
                {
                    if (b.taskName.Equals(task))
                    {
                        newburden = new Burden(b.taskName, b.category, b.startTime, b.endTime, b.duration, "day", b.isRepeat, b.isDeadline);
                        newburden.date = newDate.Date;
                        if (newStart != -1)
                        {
                            newburden.startTime = newStart;
                        }
                        arr.Remove(b);
                        updated = true;
                        if (arr.Count == 0)
                        {
                            calendar.Remove(b.date.Date);
                        }
                        else
                        {
                            calendar[b.date.Date] = arr;
                        }
                        insertTask(newburden);
                        return;
                    }
                }
            }
           
        }

        public String nextTask()
        {
            DateTime now = DateTime.Now;
            if (calendar.ContainsKey(now.Date))
            {
                ArrayList schedule = calendar[now.Date];
                schedule.Sort();
                for (int i = now.Hour; i < 24; i++)
                {
                    if (schedule[i] != null)
                    {
                        DateTime nextTime = new DateTime(now.Year, now.Month, now.Day, i, 0, 0);
                        Burden bd = (Burden)schedule[i];
                        return bd.taskName + " " + bd.startTime;
                    }
                }
            }
            else
            {
                return "No more tasks for today";
            }
            return "";
        }

        

        public String todayTask()
        {
            DateTime now = DateTime.Now;
            if (calendar.ContainsKey(now.Date))
            {
                ArrayList schedule = calendar[now.Date];
                schedule = sort(schedule);
                //return schedule.Count.ToString();
                StringBuilder res = new StringBuilder();
                res.Append("Tasks for today: \n\n");
                Burden prev = new Burden("xyz", "abc", 1, 2, 3, "aaa", false, false);
                for (int i = now.Hour; i < 23; i++)
                {
                    if (schedule[i] != null)
                    {
                        Burden bd = (Burden)schedule[i];

                        if (!bd.taskName.Equals(prev.taskName))
                        {
                            if (i < 10)
                                res.Append("0" + i + "00: " + bd.taskName + "\n\n");
                            else
                                res.Append(i + "00: " + bd.taskName + "\n\n");
                                }
                        prev = bd;
                    }
                }
                return res.ToString();
            }
            else
            {
                return "No more tasks for today";
            }
        }

        private ArrayList sort(ArrayList a)
        {
            Burden[] sortedList = new Burden[24];
            for (int i = 0; i < 24; i++)
            {
                sortedList[i] = null;
            }
            List<Burden> variableList = new List<Burden>();
           
            for (int i = 0; i < a.Count; i++)
            {
                Burden bd = (Burden)a[i];
                if (bd.isFixedTask)
                {
                    sortedList[bd.startTime / 100] = bd;
                    if (bd.duration != -1)
                    {
                        for (int j = 0; j < bd.duration; j++)
                        {
                            sortedList[bd.startTime / 100 + j] = bd;
                        }
                    }
                }
                else
                {
                    variableList.Add(bd);
                }
            }

            variableList.Sort();
            int idx = 0;
            int currHour = DateTime.Now.Hour + 1;
            for (int i = currHour ; i < 24  && idx < variableList.Count; i++)
            {
                if(sortedList[i] == null) { 
                    Burden vlIdx = (Burden)variableList[idx];
                    int duration = vlIdx.duration;
                    int timeAvailable = 0;
                    for(int k = 0; k < duration; k++)
                    {
                        if (i + k < 24 && sortedList[i + k] == null)
                            timeAvailable++;
                        else
                            break;
                    }
                    if (duration <= timeAvailable)
                    {
                        for(int j  = 0; j < duration; j++)
                        {
                            sortedList[i + j] = vlIdx;
                        }
                    }
                    sortedList[i] = vlIdx;
                   
                    idx++;
                }
            }
            ArrayList arr =  new ArrayList(24);
            arr.AddRange(sortedList);
            return arr;
        }

        public string toString()
        {
            string final = "All tasks: " + "\n\n";
            foreach (ArrayList arr in calendar.Values)
            {
                foreach (Burden b in arr)
                {
                    final = final + b.toString() + "\n\n" + "____________________________" + "\n\n";
                }
            }
            return final;
        }
        public String showTaskOfDay(DateTime date)
        {
            if (calendar.ContainsKey(date.Date))
            {
                ArrayList schedule = calendar[date.Date];
                schedule = sort(schedule);
                StringBuilder res = new StringBuilder();
                res.Append("Tasks for " + date.ToShortDateString() + " " + date.DayOfWeek.ToString() + ": " + "\n\n");
                Burden prev = new Burden("xyz", "abc", 1, 2, 3, "aaa", false, false);
                for (int i = 0; i < 23; i++)
                {
                    if (schedule[i] != null)
                    {
                        Burden bd = (Burden)schedule[i];

                        if (!bd.taskName.Equals(prev.taskName))
                        {
                            if (i < 10)
                                res.Append("0" + i + "00: " + bd.taskName + "\n\n");
                            else
                                res.Append(i + "00: " + bd.taskName + "\n\n");
                        }
                        prev = bd;
                    }
                }
                return res.ToString();
            }
            else
            {
                return "No tasks for " + date.ToShortDateString();
            }
        }
        public String finishTask(String task)
        {
            removeTask(task);
            return "Well done! \n\n";
        }
    }
}