using Game1.GameEntities;
using Game1.GameLogic;
using Game1.Generators;
using MonoGame.Extended.Collections;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

using static Game1.Generators.NameGenerator;

namespace Game1.Generators
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

            var systemName = GenerateStarName();
            var star = GenerateStar(x, y, systemName);
            var system = new SolarSystem()
            {
                Name = GenerateStarName(),//$"System",
                X = x,
                Y = y,
                Children = new() { star },
                Radius = GlobalStatic.SYSTEMSIZE,
                Mass = GlobalStatic.MSOLL
            };

            star.Parent = system;

            return system;
        }
        
        public Star GenerateStar(decimal x, decimal y, string systemName = "")
        {
            var star = new Star()
            {
                Name = systemName + " A",
                X = x,
                Y = y,
                Radius = GlobalStatic.RSOLL,//Math.Round((decimal)Rand.NextDouble() * GlobalStatic.RSOLL * 2),
                Mass = GlobalStatic.MSOLL,
                SatelliteType = BodyType.Solar,
                BaseSurfaceTemp = GlobalStatic.TSOLL,
                CoreType = BodyCoreType.Plasma
            };

            star.Children.AddRange(GeneratePlanets(7));
            star.Children.ForEach(x => x.Parent = star);

            var planets = star.Children.OfType<Planet>().ToList();
            for (int i = 0; i < planets.Count; i++)
            {
                var planet = planets[i];
                planet.Name = $"{planet.Parent.Name} {Util.GetLetter(i)}";
            }
            return star;
        }

        public List<Planet> GeneratePlanets(int number)
        {
            List<Planet> planets = new();

            for (int i = 0; i < number; i++)
            {
                var planet = GeneratePlanet();
                planets.Add(planet);
            }

            return planets.OrderBy(x => x.Distance).ToList();
        }

        public Planet GeneratePlanet()
        {
            var planet = new Planet()
            {
                Name = "Planet",
                Distance = (float)GlobalStatic.AU,//(float)Math.Round(Math.Max(Rand.NextDouble(), 0.02) * (double)(GlobalStatic.AU * 50)),
                Radius = GlobalStatic.REARTH,
                Mass = GlobalStatic.MEARTH,
                Eccentricity = (float)Math.Clamp(Rand.NextDouble(), 0.01, 0.2),
                CoreType = Enum.GetValues<BodyCoreType>().Shuffle(Rand).First()                
            };
            planet.Children.AddRange(GenerateMoons(3));
            planet.Children.ForEach(x => x.Parent = planet);
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
                Eccentricity = (float)Math.Clamp(Rand.NextDouble(), 0.1, 0.8),
            };
        }

        public void RefinePlanet(Orbital o)
        {
            //TODO refine this.
            var star = o.GetParentSystem().Children.First() as Star;
            var distance = o.Distance;

            if(o is Moon)
            {
                var parent = o.Parent as Orbital;
                distance = parent.Distance;
            }

            distance *= 1000;

            o.BaseSurfaceTemp = Util.GetBaseSurfaceTemp(star.BaseSurfaceTemp, (double)(star.Radius * 1000m), distance);
            o.BaseSurfaceTemp += o.CoreType == BodyCoreType.Molten ? 5 : 0;

            //Sanity check for habitable planet candidates:
            bool basicHabCheck = false;
            basicHabCheck = o.BaseSurfaceTemp > -50 && o.BaseSurfaceTemp < 50;
            basicHabCheck = o.Mass > 0.01 * (float)GlobalStatic.MEARTH && o.Mass < 5 * (float)GlobalStatic.MEARTH;

            //In HZ?
            //Generate HZ planet
            if (basicHabCheck)
                RefineHabitable(o);
            //Generate non HZ planet
            else
                RefineOther(o);
        }

        public void RefineHabitable(Orbital planet)
        {
            planet.SatelliteType = BodyType.Terrestrial;
            planet.Atmosphere = new Atmos()
            {
                AtmosType = AtmosType.Normal,
                LiquidWater = 0.75f,
            };

            planet.Atmosphere.Gases[Gas.O2] = 0.21f;
            planet.Atmosphere.Gases[Gas.N2] = 0.79f;
            planet.Atmosphere.Gases[Gas.CO2] = 0.04f;
            planet.Atmosphere.Gases[Gas.H2Ov] = 0.01f;
            //planet.Atmosphere.Gases[Gas.N2] = 0.79f;
            planet.Resources = Enum.GetValues<Resource>().ToDictionary(x => x, x => (Rand.Next(0, int.MaxValue / 2), Rand.NextDouble()));
        }

        public void RefineOther(Orbital planet)
        {
            //var satTypes = Enum.GetValues<BodyType>();
            //planet.SatelliteType = satTypes[Rand.Next(satTypes.Length)];

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
