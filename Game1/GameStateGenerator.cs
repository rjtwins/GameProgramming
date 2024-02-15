using Game1.GameEntities;
using Game1.GameLogic;
using Game1.GameLogic.SubSystems;
using Game1.ScreenModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class GameStateGenerator
    {
        public static void Generate()
        {
            var generator = new SpaceGenerator();
            var systems = generator.Generate(500);

            systems.ForEach(s =>
            {
                s.Planets.ForEach(p =>
                {
                    GameState.GameEntities.Add(p);
                    p.Moons.ForEach(m =>
                    {
                        GameState.GameEntities.Add(m);
                    });
                });

                s.Stars.ForEach(st =>
                {
                    GameState.GameEntities.Add(st);
                });

                GameState.GameEntities.Add(s);
            });

            var fleet = new Fleet()
            {
                Name = "Fleet 1",
            };

            var ship = new Ship()
            {
                Name = "New Orleans",
                ShipClass = "Heavy Cruiser",
                Mass = 200,
                Fuel = 200,
                Crew = 200,
            };

            var ship2 = new Ship()
            {
                Name = "Indianapolis",
                ShipClass = "Light Cruiser",
                Mass = 100,
                Fuel = 100,
                Crew = 100,
            };

            fleet.Members.Add(ship);

            GameState.GameEntities.Add(fleet);
            ShipDesign.Instance.ActiveDesign = ship;
            GameState.ShipDesigns.Add(ship);
            GameState.ShipDesigns.Add(ship2);

            var engine = new Engine()
            {
                Name = "Dummy Engine",
                Mass = 100,
                Thrust = 1000,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var cargo = new CargoBay()
            {
                Name = "Dummy Cargo",
                Capacity = 9000,
                Mass = 100,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var armor = new Armor()
            {
                Name = "Dummy Armor",
                Mass = 100,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var crewBerths = new CrewBerths()
            {
                Name = "Dummy Berths",
                Mass = 100,
                Capacity = 500,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var fuelTank = new FuelTank()
            {
                Name = "Dummy Fuel Tank",
                Mass = 100,
                Capacity = 500,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var reactor = new Reactor()
            {
                Name = "Dummy Reactor",
                Mass = 100,
                EnergyGeneration = 2000,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var sensor = new Sensor()
            {
                Name = "Dummy Sensor",
                Mass = 100,
                Resolution = 1080,
                Range = 99e7,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var thruster = new Thruster()
            {
                Name = "Dummy Thruster",
                Mass = 100,
                Thrust = 250,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            var turret = new Turret()
            {
                Name = "Dummy Berths",
                Mass = 100,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            GameState.SubSystems.Add(engine);
            GameState.SubSystems.Add(cargo);
            GameState.SubSystems.Add(armor);
            GameState.SubSystems.Add(sensor);
            GameState.SubSystems.Add(crewBerths);
            GameState.SubSystems.Add(fuelTank);
            GameState.SubSystems.Add(thruster);
            GameState.SubSystems.Add(turret);
        }
    }
}
