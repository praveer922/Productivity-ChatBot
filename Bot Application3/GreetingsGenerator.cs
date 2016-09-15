using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application3
{
    public class GreetingsGenerator
    {
        String[] motivations;

        public GreetingsGenerator()
        {
            motivations = new String[10];
            motivations[0] = "Better days are coming. They are called Saturday and Sunday;)";
            motivations[1] = "If Plan A didn't work the alphabet has 25 more letters:)";
            motivations[2] = "Stop procrastinating you lil *****";

        }

        public string getGreeting()
        {
            Random ran = new Random();
            int idx = ran.Next(0, 2);
            return "Good morning Ming Yi! Here's your motivational quote for today:\n\n " + motivations[idx];
        }
    }
}