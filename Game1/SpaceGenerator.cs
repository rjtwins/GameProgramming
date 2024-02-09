using Autofac;
using Game1.GameEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class SpaceGenerator
    {
        public Random Rand = new Random();

        public List<SolarSystem> Generate(int number)
        {
            List<SolarSystem> systems = new();

            for (int i = 0; i < number; i++)
            {
                var system = GenerateSystem();
                systems.Add(system);
            }

            var sollSystem = new SolarSystem()
            {
                Radius = GlobalStatic.SYSTEMSIZE,
                Name = "SOLL SYSTEM",
            };

            var soll = new Star()
            {
                Name = "SOLL",
                Mass = GlobalStatic.MSOLL,
                Radius = GlobalStatic.RSOLL
            };

            var earth = new Planet()
            {
                X = GlobalStatic.AU,
                Y = 0,
                LocalX = GlobalStatic.AU,
                Name = "EARTH",
                Mass = GlobalStatic.MEARTH,
                Radius = GlobalStatic.REARTH
            };

            var luna = new Moon()
            {
                LocalX = 384400,
                Name = "LUNA",
                Mass = GlobalStatic.MLUNA,
                Radius = GlobalStatic.RLUNA
            };

            sollSystem.Stars.Add(soll);
            sollSystem.Planets.Add(earth);
            earth.Moons.Add(luna);
            earth.Parent = soll;
            luna.Parent = earth;

            systems.Add(sollSystem);

            return systems;
        }

        public SolarSystem GenerateSystem()
        {
            decimal y = 2 * GlobalStatic.GALAXYSIZE * (decimal)(Rand.NextDouble() - 0.5);
            decimal x = 2 * GlobalStatic.GALAXYSIZE * (decimal)(Rand.NextDouble() - 0.5);

            var nrStars = 1;
            var star = GenerateStar(x, y);
            //For now the planets orbit around the system center, this may change later.
            var nrPlanets = Rand.Next(25);

            var system = new SolarSystem()
            {
                Name = $"System",
                X = x,
                Y = y,
                Planets = GeneratePlanets(nrPlanets, x, y, GlobalStatic.SYSTEMSIZE),
                Stars = new() { star },
                Radius = GlobalStatic.SYSTEMSIZE,
                Mass = 2e30d
            };

            system.Planets.ForEach(x => x.Parent = system);

            return system;
        }

        //public List<Star> GenerateStars(int number)
        //{
        //    for (int i = 0; i < number; i++)
        //    {
        //        GenerateStar();
        //    }
        //}

        public Star GenerateStar(decimal x, decimal y)
        {
            return new Star()
            {
                Name = "STAR",
                X = x,
                Y = y,
                Radius = Math.Round((decimal)Rand.NextDouble() * (GlobalStatic.RSOLL * 2))
            };
        }

        public List<Planet> GeneratePlanets(int number, decimal x, decimal y, decimal systemSize)
        {
            List<Planet> planets = new();

            for (int i = 0; i < number; i++)
            {
                var lx = ((x - systemSize / 2) + (decimal)Rand.NextDouble() * systemSize);
                var ly = ((y - systemSize / 2) + (decimal)Rand.NextDouble() * systemSize);

                var planet = GeneratePlanet(lx, ly);
                planet.LocalX = (x - lx) * -1;
                planet.LocalY = (y - ly);
                planet.Name = $"Planet";
                planets.Add(planet);
            }

            return planets;
        }

        public Planet GeneratePlanet(decimal x, decimal y)
        {
            var nrMoons = 1;
            var planet = new Planet()
            {
                Name = "Planet",
                X = x,
                Y = y,
                Radius = Rand.Next(1000, 24540),
                Moons = GenerateMoons(nrMoons, x, y, 200000000),
                Mass = GlobalStatic.MEARTH
            };
            planet.Moons.ForEach(x => x.Parent = planet);

            return planet;
            
        }

        public List<Moon> GenerateMoons(int number, decimal x, decimal y, decimal systemSize)
        {
            List<Moon> moons = new();

            for (int i = 0; i < number; i++)
            {
                var lx = ((x - systemSize / 2) + (decimal)Rand.NextDouble() * systemSize);
                var ly = ((y - systemSize / 2) + (decimal)Rand.NextDouble() * systemSize);

                var moon = GenerateMoon(lx, ly);
                moon.LocalX = 384400; //(x - lx) * -1;
                moon.LocalY = 0;// (y - ly);

                moons.Add(moon);
            }

            return moons;
        }

        public Moon GenerateMoon(decimal x, decimal y)
        {
            return new Moon()
            {
                Name = "Moon",
                X = x,
                Y = y,
                Radius = GlobalStatic.RLUNA,
                Mass = GlobalStatic.MLUNA
            };
        }
    }

}
