using Game1.GameEntities;
using Game1.GameLogic;
using Game1.GameLogic.SubSystems;
using Game1.GraphicalEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Game1
{
    public static class GameState
    {
        public static List<GameGraphicalEntity> GraphicalEntities { get; set; } = new();
        public static List<GameEntity> GameEntities { get; set; } = new();
        public static List<GameEntity> SelectedEntities { get; set; } = new();
        public static List<SolarSystem> SolarSystems { get; set; } = new();
        public static List<Planet> Planets { get; set; } = new();
        public static List<Moon> Moons { get; set; } = new();
        public static List<Colony> Colonies { get; set; }
        public static List<ShipDesign> ShipDesigns { get; set; } = new();
        public static List<SubSystemBase> SubSystems { get; set; } = new();
        public static List<Faction> Factions { get; set; } = new();
        public static Dictionary<ColonyBuilding, BuildingInfo> BuildingInfo { get; set; } = new();
        public static Dictionary<Gas, GasInfo> GasInfo { get; set; } = new();

        public static GameEntity Focus { get; set; }


        public static double TotalSeconds { get; set; } = 0d;
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
                _gameSpeed = Math.Min(_gameSpeed, MathF.Pow(2, 19));
                _gameSpeed = Math.Max(_gameSpeed, 1);
            }
        }
        private static float _gameSpeed = 1f;

        public static bool Paused = false;

        public static void UpdateGameTime(double deltaTime)
        {
            if (Paused)
                return;

            var gameSeconds = deltaTime * (double)GameSpeed;
            TotalSeconds += gameSeconds;
        }
    }
}
