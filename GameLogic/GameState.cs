using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public static class GameState
    {
        public static List<Entity> WorldEntities = new();

        public static List<Entity> SelectedEntities = new();

        public static double TotalSeconds { get; set; } = 0;
        public static double TotalMinutes => TotalSeconds / 60;
        public static double TotalHours => TotalSeconds / 3600;

        public static int GameSpeed { get; set; } = 1;

        public static void Update(double deltaSeconds)
        {
            TotalSeconds = TotalSeconds + (deltaSeconds * GameSpeed);
            //Debug.WriteLine(TotalSeconds);
            WorldEntities.ForEach(x => x.Update(deltaSeconds));
        }
    }
}
