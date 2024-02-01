using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    
    public static class Ticker
    {
        public static long TotalSeconds { get; set; } = 0;
        public static long TotalMinutes => TotalSeconds / 60;
        public static long TotalHours => TotalSeconds / 3600;

        public static void UpdateGameState()
        {

        }
    }
}
