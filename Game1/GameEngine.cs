using Autofac;
using Game1.GameEntities;
using Game1.GameLogic;
using MonoGame.Extended.Tiled.Serialization;
using RenderingLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game1
{
    public static class GameEngine
    {
        public static ConcurrentDictionary<Guid, bool> Workers = new();
        public static bool Synced = true;
        public static double TimeSenseLastUpdate {  get; set; } = 0;

        public static void Start()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                GameState.GameEntities.Where(x => x is Orbital).Cast<Orbital>().ToList().ForEach(x => x.Init());

                while (true)
                {
                    var time = GameState.TotalSeconds;

                    while (GameState.TotalSeconds - time < 0.25)
                    {
                        Thread.Yield();
                    }

                    var orbitals = GameState.GameEntities.Where(x => x is Orbital).Cast<Orbital>().ToList();
                    orbitals.ForEach(x =>
                    {
                        x.CalcPos();
                        x.UpdateInView();
                        //x.CalculateOrbitPoints();
                    });
                }
            });

            GameState.GameEntities.OfType<Fleet>().ToList().ForEach(x => x.Animate());

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    var time = GameState.TotalSeconds;

                    while (GameState.TotalSeconds - time < 1)
                    {
                        Thread.Yield();
                    }

                    var fleets = GameState.GameEntities.Where(x => x is Fleet).Cast<Fleet>().ToList();
                    fleets.ForEach(x =>
                    {
                        x.CreateGhost();
                    });
                }
            });

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    var time = GameState.TotalSeconds;

                    while (GameState.TotalSeconds - time < 1)
                    {
                        Thread.Yield();
                    }

                    Detection.Update();
                }
            });

            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    Update();
                }
            });
        }

        public static void Update()
        {
            var time = GameState.TotalSeconds;

            while (Workers.Values.Any(x => x == false) || (GameState.TotalSeconds - time) < 1)
            {
                Synced = false;
                Thread.Yield();
            }

            TimeSenseLastUpdate = GameState.TotalSeconds - time;



            Synced = true;
        }
    }
}
