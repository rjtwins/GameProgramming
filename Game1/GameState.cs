using Game1.GameEntities;
using Game1.GraphicalEntities;
using Game1.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class GameState
    {
        public static List<GameGraphicalEntity> MiscGraphicalEntities { get; set; } = new();
        public static List<GameGraphicalEntity> SelectedEntities { get; set; } = new();
        public static GameGraphicalEntity SelectedEntity { get; set; }
        public static List<BodyBase> GameEntities { get; set; } = new();

        public static double TotalSeconds { get; set; } = 0;
        public static double TotalMinutes => TotalSeconds / 60;
        public static double TotalHours => TotalSeconds / 3600;

        public static int GameSpeed { get; set; } = 1;
        public static bool RightButtonClicked { get; set; }
        public static (double x, double y) RightMouseClickedWorldLocation { get; set; }

        public static void Update(double deltaSeconds)
        {
            TotalSeconds = TotalSeconds + deltaSeconds * GameSpeed;
            
            //Debug.WriteLine(TotalSeconds);
            //MiscGraphicalEntities.ForEach(x => x.Update(deltaSeconds));

            if(RightButtonClicked)
            {
                ProcessClickActions();
            }
        }

        public static void CheckClick()
        {
            if (FlatMouse.Instance.IsLeftButtonClicked() || FlatMouse.Instance.IsRightButtonClicked())
            {
                MiscGraphicalEntities.ForEach(x => x.CheckClick());
            }
        }

        private static void ProcessClickActions()
        {
            if(SelectedEntity == null)
                return;

            double x1 = SelectedEntity.Position.x;
            double y1 = SelectedEntity.Position.y;
            double x2 = RightMouseClickedWorldLocation.x;
            double y2 = RightMouseClickedWorldLocation.y;
            double angle = Math.Atan2(y2 - y1, x2 - x1);
            
            //double angleDegrees = angle * (180.0 / Math.PI);
            //Debug.WriteLine($"MousePos: {RightMouseClickedWorldLocation}, EntityPos: {SelectedEntity.Position}, Angle {angleDegrees}");

            //SelectedEntity.Angle = (float)angle;

        }
    }
}
