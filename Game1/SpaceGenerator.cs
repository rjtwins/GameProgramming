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

            return systems;
        }

        public SolarSystem GenerateSystem()
        {
            var x = Rand.NextDouble() * GlobalStatic.GALAXYSIZE;
            var y = Rand.NextDouble() * GlobalStatic.GALAXYSIZE;

            var nrStars = 1;
            var star = GenerateStar(x, y);
            //For now the planets orbit around the system center, this may change later.
            var nrPlanets = Rand.Next(25);

            return new SolarSystem()
            {
                Name = $"System ({x},{y})",
                X = x,
                Y = y,
                Planets = GeneratePlanets(nrPlanets, x, y, GlobalStatic.SYSTEMSIZE),
                Stars = new() { star },
                Radius = GlobalStatic.SYSTEMSIZE
            };
        }

        //public List<Star> GenerateStars(int number)
        //{
        //    for (int i = 0; i < number; i++)
        //    {
        //        GenerateStar();
        //    }
        //}

        public Star GenerateStar(double x, double y)
        {
            return new Star()
            {
                Name = "STAR",
                X = x,
                Y = y,
                Radius = Math.Round(Rand.NextDouble() * 700000000)
            };
        }

        public List<Planet> GeneratePlanets(int number, double x, double y, double systemSize)
        {
            List<Planet> planets = new();

            for (int i = 0; i < number; i++)
            {
                var lx = ((x - systemSize / 2) + Rand.NextDouble() * systemSize);
                var ly = ((y - systemSize / 2) + Rand.NextDouble() * systemSize);

                var planet = GeneratePlanet(lx, ly);
                planet.LocalX = (x - lx) * -1;
                planet.LocalY = (y - ly);
                planet.Name = $"Planet ({planet.LocalX}, {planet.LocalY})";

                planets.Add(planet);
            }

            return planets;
        }

        public Planet GeneratePlanet(double x, double y)
        {
            return new Planet()
            {
                Name = "Planet",
                X = x,
                Y = y,
                Radius = Rand.Next(10000, 24540000)
            };
        }
    }

}
