using Game1.GameEntities;
using Game1.GameLogic;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

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
            decimal y = 2 * GlobalStatic.GALAXYSIZE * (decimal)(Rand.NextDouble() - 0.5);
            decimal x = 2 * GlobalStatic.GALAXYSIZE * (decimal)(Rand.NextDouble() - 0.5);

            var nrStars = 1;
            var star = GenerateStar(x, y);
            //For now the planets orbit around the system center, this may change later.
            var nrPlanets = 7;

            var system = new SolarSystem()
            {
                Name = $"System",
                X = x,
                Y = y,
                Planets = GeneratePlanets(nrPlanets),
                Stars = new() { star },
                Radius = GlobalStatic.SYSTEMSIZE,
                Mass = GlobalStatic.MSOLL
            };

            system.Planets.ForEach(x => x.Parent = system);

            return system;
        }

        public Star GenerateStar(decimal x, decimal y)
        {
            return new Star()
            {
                Name = "STAR",
                X = x,
                Y = y,
                Radius = Math.Round((decimal)Rand.NextDouble() * (GlobalStatic.RSOLL * 2)),
                Mass = GlobalStatic.MSOLL
            };
        }

        public List<Planet> GeneratePlanets(int number)
        {
            List<Planet> planets = new();

            for (int i = 0; i < number; i++)
            {
                var planet = GeneratePlanet();
                planets.Add(planet);
            }

            return planets;
        }

        public Planet GeneratePlanet()
        {
            var nrMoons = 4;
            var planet = new Planet()
            {
                Name = "Planet",
                Distance = (float)Math.Round(Math.Max(Rand.NextDouble(), 0.02) * (double)(GlobalStatic.AU * 50)),
                Radius = GlobalStatic.REARTH,
                Moons = GenerateMoons(nrMoons),
                Mass = GlobalStatic.MEARTH,
                Eccentricity = (float)Math.Clamp(Rand.NextDouble(), 0.01, 0.2)
            };
            planet.Moons.ForEach(x => x.Parent = planet);
            return planet;
        }

        public List<Moon> GenerateMoons(int number)
        {
            List<Moon> moons = new();

            for (int i = 0; i < number; i++)
            {
                var moon = GenerateMoon();
                moons.Add(moon);
            }

            return moons;
        }

        public Moon GenerateMoon()
        {
            return new Moon()
            {
                Name = "Moon",
                Distance = 384400,
                Radius = GlobalStatic.RLUNA,
                Mass = GlobalStatic.MLUNA,
                Eccentricity = (float)Math.Clamp(Rand.NextDouble(), 0.1, 0.8)
            };
        }


        public void RefinePlanet(Planet planet)
        {
            bool isHZ = false;
            isHZ = planet.Distance > 0.4f * (float)GlobalStatic.AU && planet.Distance < 5f * (float)GlobalStatic.AU;
            isHZ = planet.Mass > 0.01 * (float)GlobalStatic.MEARTH && planet.Mass < 5 * (float)GlobalStatic.MEARTH;

            //In HZ?
            //Generate HZ planet
            if (isHZ)
                RefineHabitable(planet);
            //Generate non HZ planet
            else
                RefineOther(planet);
        }

        public void RefineHabitable(Planet planet)
        {
            planet.SatelliteType = SatelliteType.Terrestrial;
            planet.Atmosphere = new Atmos()
            {
                AtmosType = AtmosType.Liquid,
                LiquidWater = 5e15f,
                Oxygen = 0.21f,
                Nitrogen = 0.79f                
            };
            planet.Resources = Enum.GetValues<Resource>().ToDictionary(x => x, x => (Rand.Next(0, int.MaxValue / 2), Rand.NextDouble()));
        }

        public void RefineOther(Planet planet)
        {
            var satTypes = Enum.GetValues<SatelliteType>();
            planet.SatelliteType = satTypes[Rand.Next(satTypes.Length)];

            //planet.Atmosphere = new Atmos()
            //{
            //    AtmosType = AtmosType.Liquid,
            //    LiquidWater = 5e15f,
            //    Oxygen = 0.21f,
            //    Nitrogen = 0.79f
            //};

            planet.Resources = Enum.GetValues<Resource>().ToDictionary(x => x, x => (Rand.Next(0, int.MaxValue / 2), Rand.NextDouble()));
        }
    }

}
