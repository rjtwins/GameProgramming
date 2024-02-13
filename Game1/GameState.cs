using Game1.GameEntities;
using Game1.GameLogic;
using Game1.GraphicalEntities;
using System;
using System.Collections.Generic;
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

        public static List<Ship> ShipDesigns { get; set; } = new();

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
                _gameSpeed = Math.Min(_gameSpeed, (float)Math.Pow(2, 25));
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

            //var minTime = 0.25f;
            //var maxTime = Math.Pow(2, 25);

            //int minSubsteps = 1;
            //int maxSubsteps = 10;

            //double substepsRatio = Math.Log(GameSpeed) / Math.Log(Math.Pow(2, 25)); // Scale based on the range [0.25, 86400]

            //var subSteps = Math.Max(1, Math.Ceiling(maxSubsteps * substepsRatio));

            //var gameSeconds = 1 * (double)GameSpeed;
            //TotalSeconds += gameSeconds;

            //var stepTime = gameSeconds / subSteps;

            ////var updateEntities = GameEntities.Where(x => !(x is Orbital)).ToList();

            ////Main update loop for non orbital entities:
            //for (int h = 0; h < subSteps; h++)
            //{
            //    for (int i = 0; i < GameEntities.Count; i++)
            //    {
            //        GameEntities[i].Update((decimal)stepTime);
            //    }
            //}
        }

        public static void StartUpdateProcesses()
        {
            var Oribitals = new Task(() =>
            {
                GameEntities.Where(x => x is Orbital).Cast<Orbital>().ToList().ForEach(x => x.Init());

                while (true)
                {
                    var time = TotalSeconds;

                    //while (TotalSeconds - time < 5)
                    //{
                    //    Thread.Yield();
                    //}

                    var orbitals = GameEntities.Where(x => x is Orbital).Cast<Orbital>().ToList();
                    orbitals.ForEach(x =>
                    {
                        x.CalcPos();
                        x.UpdateInView();
                        //x.CalculateOrbitPoints();
                    });


                }
            });

            Oribitals.Start();

            var Fleets = new Task(() =>
            {
                while (true)
                {
                    var time = TotalSeconds;

                    while(TotalSeconds - time < 1)
                    {
                        Thread.Yield();
                    }

                    var fleets = GameEntities.Where(x => x is Fleet).Cast<Fleet>().ToList();
                    fleets.ForEach(x =>
                    {
                        x.Update((decimal)(TotalSeconds - time));
                    });
                }
            });

            Fleets.Start();
        }
    }
}
