using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tasks;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Bot_Application3;
using TaskList;

namespace Productivity
{
    [LuisModel("1f17f38e-bd2d-44a7-b819-db55287fd32f", "94341432237d4c53b3503af3582044fe")]
    [Serializable]

    public class Productivity : LuisDialog<object>
    {
        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(MessagesController.greetingsGen.getGreeting());
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task NoneHandler(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("none");
            context.Wait(MessageReceived);
        }

        [LuisIntent("AddTask")]
        public async Task AddTask(IDialogContext context, LuisResult result)
        {
        
                Burden newBurden = getBurdenObject(result.Entities);
                MessagesController.entireList.insertTask(newBurden);
            if(newBurden.date.Equals(DateTime.Now.Date))
                await context.PostAsync(MessagesController.entireList.todayTask());
            else
            await context.PostAsync(MessagesController.entireList.showTaskOfDay(newBurden.date));
                context.Wait(MessageReceived);
            }
        

        private string getCategory(IList<EntityRecommendation> e)
        {
            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Contains("Health"))
                {
                    return "Health";
                }
                else if (er.Type.Contains("Study")) {
                    return "Study";
                }
            }
            return "Others";
        }

        private bool getIsDeadline(IList<EntityRecommendation> e)
        {
            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Equals("deadline"))
                {
                    return true;
                }
            }
                    return false;
         
        }

        private bool getIsRepeat(IList<EntityRecommendation> e)
        {
            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Equals("repeat"))
                {
                    return true;
                }

            }
                    return false;
               
        }

        private string getDay(IList<EntityRecommendation> e)
        {
            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Equals("date"))
                {
                    return er.Entity;
                }
            }
                
            return "";
        }

        private int getDuration(IList<EntityRecommendation> e)
        {
            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Equals("duration"))
                {
                    String[] startAndEnd = er.Entity.Split(new string[] { " to " }, StringSplitOptions.None);
                    if(startAndEnd.Length != 2)
                    {
                        return -1;
                    }
                    int start = convertTime(startAndEnd[0]);
                    int end = convertTime(startAndEnd[1]);
                    return (end - start) / 100;
                }
            }
            return 1;
        }

        private int getStartTimeIfDuration(IList<EntityRecommendation> e)
        {
            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Equals("duration"))
                {
                    String[] startAndEnd = er.Entity.Split(new string[] { " to " }, StringSplitOptions.None);
                   int start = convertTime(startAndEnd[0]);
                    return start;
                }
            }
            return -1;
        }

        private int getTaskStartTime(IList<EntityRecommendation> e) {

            int time = -1;

             Boolean hasDeadline = false;

            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Contains("Time"))
                {
                    time = convertTime(er.Entity);
                }
                if (er.Type.Equals("deadline"))
                {
                  hasDeadline = true; 

                }
            }
            if (hasDeadline)
            {
                return -1; 
            }
            return time;
        }

        private string getTaskName(IList<EntityRecommendation> e)
        {
            foreach (EntityRecommendation er in e)
            {
                if (er.Type != null && er.Type.Contains("TaskNames"))
                {
                    return er.Entity;
                }
            }
            return "";
        }

        private int convertTime(string time)
        {
            string timeCopy = time;
            string number = Regex.Replace(time, "[^0-9]+", string.Empty);
            if(number == "")
            {
                return -1;
            }
            int numbers = Int32.Parse(number);
            if (timeCopy.Contains("pm"))
            {
                return (numbers + 12) * 100;
            }
            else if (timeCopy.Contains("am"))
            {
                return numbers * 100;
            }
            else
                return numbers;
        }


        [LuisIntent("ShowTask")]
        public async Task ShowTask(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(MessagesController.entireList.todayTask());
            context.Wait(MessageReceived);
        }

        [LuisIntent("RemoveTask")]
        public async Task RemoveTask(IDialogContext context, LuisResult result)
        {
            Burden bd = getBurdenObject(result.Entities);

            //motivate
            bool welldone = false;


            foreach (EntityRecommendation er in result.Entities)
            {
               
                if (er.Type.Contains("done"))
                {
                    welldone = true;
                    break;

                }
            }

            if (welldone)
            {
                MessagesController.entireList.removeTask(bd.taskName);
                await context.PostAsync("Well done!");
            }
            else
            {
                MessagesController.entireList.removeTask(bd.taskName);
                await context.PostAsync("Don't Procrastinate!");
            }
        
        
            if (bd.date.Equals(DateTime.Now.Date))
                await context.PostAsync(MessagesController.entireList.todayTask());
            else
                await context.PostAsync(MessagesController.entireList.showTaskOfDay(bd.date));
            context.Wait(MessageReceived);
        }

        private Burden getBurdenObject(IList<EntityRecommendation> e)
        {
            string taskName = getTaskName(e);
            int duration = getDuration(e);
            int startTime = -1;
            if (duration != -1)
            {
                startTime = getStartTimeIfDuration(e);
            }
            else
            {
                startTime = getTaskStartTime(e);
            }
            string day = getDay(e);
            string category = getCategory(e);
            bool isRepeat = getIsRepeat(e);
            bool isDeadline = getIsDeadline(e);
            int endTime = getEndTime(e);


            return new Burden(taskName, category, startTime, endTime, duration, day, isRepeat, isDeadline);
        }

        private int getEndTime(IList<EntityRecommendation> e)
        {
            int time = -1;

            Boolean hasDeadline = false;

            foreach (EntityRecommendation er in e)
            {
                if (er.Type.Contains("Time"))
                {
                    time = convertTime(er.Entity);
                }
                if (er.Type.Equals("deadline"))
                {
                    hasDeadline = true;

                }
            }
            if (hasDeadline)
            {
                return time;
            }
            return -1;
        
    }

        [LuisIntent("UpdateTask")]
        public async Task UpdateTask(IDialogContext context, LuisResult result)
        {
            Burden bd = getBurdenObject(result.Entities);
            MessagesController.entireList.updateTask(bd.taskName, bd.date, bd.startTime);
            await context.PostAsync(MessagesController.entireList.showTaskOfDay(bd.date));
            context.Wait(MessageReceived);
        }
    }
}