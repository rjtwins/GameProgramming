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
        public static List<GameGraphicalEntity> GraphicalEntities { get; set; } = new();
        public static List<GameEntity> GameEntities { get; set; } = new();
        public static List<GameEntity> SelectedEntities { get; set; } = new();

        public static GameEntity Focus { get; set; }

        public static double TotalSeconds { get; set; } = 0;
        public static double TotalMinutes => TotalSeconds / 60;
        public static double TotalHours => TotalSeconds / 3600;

        public static float GameSpeed
        {
            get
            {
                return _gameSpeed;
            }
            set
            {
                _gameSpeed = value;
                _gameSpeed = Math.Min(_gameSpeed, 3.156e+7f);
            }
        }
        private static float _gameSpeed = 1f;

        public static void Update(double deltaSeconds)
        {
            deltaSeconds = Math.Round(deltaSeconds);
            var gameSeconds = deltaSeconds * GameSpeed;
            TotalSeconds += gameSeconds;

            //Main update loop.
            for (int i = 0; i < GameEntities.Count; i++)
            {
                GameEntities[i].Update((decimal)gameSeconds);
            }
        }
    }
}
