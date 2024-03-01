using Game1.GameEntities;
using Game1.GameLogic;
using Game1.GameLogic.SubSystems;
using Game1.ScreenModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Generators
{
    public static class GameStateGenerator
    {
        public static void Generate()
        {
            SetupGasInfo();
            SetupBuildingInf();

            var generator = new SpaceGenerator();
            var systems = generator.Generate(250);
            var species = new Species();
            species.MinGrav = 0.2;
            species.MaxGrav = 1.2;
            species.MaxGrav = 1.5;
            species.MinAtmos = 0.5;

            var faction1 = new Faction()
            {
                Name = "one",
                Species = species,
            };

            var faction2 = new Faction()
            {
                Name = "two",
                Species = species
            };


            //TODO: Make this recursive.
            systems.ForEach(s =>
            {
                s.Children.ForEach(st =>
                {
                    GameState.GameEntities.Add(st);
                    st.Children.ForEach(p =>
                    {
                        GameState.GameEntities.Add(p);
                        p.Children.ForEach(m =>
                        {
                            GameState.GameEntities.Add(m);
                        });
                    });
                });

                GameState.GameEntities.Add(s);
            });
            GameState.GameEntities.OfType<Planet>().ToList().ForEach(x =>
            {
                x.Colony = new();
                x.Colony.HostBody = x;
                x.Colony.Faction = faction1;
                x.Colony.Name = x.Name + " Colony";
                x.Colony.CurrentBuildings[ColonyBuilding.ProductionFactory] = 100;
                x.Colony.BuildingQueue.Add(new BuildingQueueItem()
                {
                    Allocation = 0.5,
                    ColonyBuilding = ColonyBuilding.ProductionFactory,
                    Amount = 10,
                    Progress = 0
                });

                x.Colony.BuildingQueue.Add(new BuildingQueueItem()
                {
                    Allocation = 0.5,
                    ColonyBuilding = ColonyBuilding.Mine,
                    Amount = 10,
                    Progress = 0,
                    Inf = true
                });
            });

            var fleet = new Fleet()
            {
                Name = "Fleet 1",
            };

            var fleet2 = new Fleet()
            {
                Name = "Fleet 2",
                X = 1000,
                Y = 1000
            };

            var ship = new ShipInstance()
            {
                Name = "New Orleans",
                ShipClass = "Heavy Cruiser",
                Mass = 200,
                Fuel = 200,
                Crew = 200,
            };

            var ship2 = new ShipInstance()
            {
                Name = "Indianapolis",
                ShipClass = "Light Cruiser",
                Mass = 100,
                Fuel = 100,
                Crew = 100,
            };

            var ship3 = new GameLogic.ShipDesign()
            {
                Name = "New Orleans",
                ShipClass = "Heavy Cruiser",
                Mass = 200,
                Fuel = 200,
                Crew = 200,
            };

            var ship4 = new GameLogic.ShipDesign()
            {
                Name = "Indianapolis",
                ShipClass = "Light Cruiser",
                Mass = 100,
                Fuel = 100,
                Crew = 100,
            };

            fleet.Members.Add(ship);
            fleet.Faction = faction1;

            fleet2.Members.Add(ship2);
            fleet2.Faction = faction2;

            GameState.GameEntities.Add(fleet);
            GameState.GameEntities.Add(fleet2);

            GameState.Factions.Add(faction1);
            GameState.Factions.Add(faction2);

            var system = systems.First();
            fleet.X = system.X + 1000000;
            fleet.Y = system.Y;

            fleet2.X = system.X + 1000000;
            fleet2.Y = system.Y + 1000;

            fleet.SOIEntity = system;
            fleet2.SOIEntity = system;

            ScreenModels.ShipDesign.Instance.ActiveDesign = ship3;
            GameState.ShipDesigns.Add(ship3);
            GameState.ShipDesigns.Add(ship4);

            var engine = new Engine()
            {
                Name = "Dummy Engine",
                Mass = 1000,
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
                Resolution = 1,
                Range = 99e7,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
                SensorType = SensorType.Active
            };

            var sensor2 = new Sensor()
            {
                Name = "Dummy Sensor2",
                Mass = 100,
                Resolution = 50,
                Range = 99e4,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
                SensorType = SensorType.Optical
            };

            var sensor3 = new Sensor()
            {
                Name = "Dummy Sensor2",
                Mass = 100,
                Resolution = 50,
                Range = 99e4,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
                SensorType = SensorType.IR
            };

            var sensor4 = new Sensor()
            {
                Name = "Dummy Sensor2",
                Mass = 100,
                Resolution = 50,
                Range = 99e4,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
                SensorType = SensorType.EM
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
                Name = "Dummy Turret",
                Mass = 100,
                Cost1 = 1,
                Cost2 = 2,
                Cost3 = 3,
            };

            ship.SubSystems.Add((SubSystemBase)sensor.Clone());
            ship.SubSystems.Add((SubSystemBase)sensor2.Clone());
            ship.SubSystems.Add((SubSystemBase)sensor3.Clone());
            ship.SubSystems.Add((SubSystemBase)sensor4.Clone());
            ship2.SubSystems.Add((SubSystemBase)engine.Clone());

            GameState.SubSystems.Add(engine);
            GameState.SubSystems.Add(cargo);
            GameState.SubSystems.Add(armor);
            GameState.SubSystems.Add(sensor);
            GameState.SubSystems.Add(sensor2);
            GameState.SubSystems.Add(sensor3);
            GameState.SubSystems.Add(sensor4);
            GameState.SubSystems.Add(crewBerths);
            GameState.SubSystems.Add(fuelTank);
            GameState.SubSystems.Add(thruster);
            GameState.SubSystems.Add(turret);

            //PlanetScreen.Instance.ListedPlanets = GameState.Planets;
            //PlanetScreen.Instance.ListedSystems = GameState.SolarSystems;

            //Refine planets:

            GameState.Planets.ForEach(planet =>
            {
                generator.RefinePlanet(planet);
            });

            GameState.Planets.SelectMany(x => x.Children).OfType<Orbital>().ToList().ForEach(x => generator.RefinePlanet(x)); 
        }

        public static void SetupGasInfo()
        {
            var json = System.IO.File.ReadAllText("Content\\GasInfo.json");
            GameState.GasInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<Gas, GasInfo>>(json);

            //Enum.GetValues<Gas>().ToList().ForEach(x =>
            //{
            //    GameState.GasInfo[x] = new GasInfo();
            //});

            //GameState.GasInfo[Gas.H2Ov].AGHF = 0;
            //GameState.GasInfo[Gas.H2Ov].GHF = 16;
            //GameState.GasInfo[Gas.CO2].AGHF = 0;
            //GameState.GasInfo[Gas.CO2].GHF = 17;

            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(GameState.GasInfo);

      }

        public static void SetupBuildingInf()
        {
            //Enum.GetValues<ColonyBuilding>().ToList().ForEach(x =>
            //{
            //    GameState.BuildingInfo[x] = new BuildingInfo();
            //});
            
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(GameState.BuildingInfo);


            var json = System.IO.File.ReadAllText("Content\\BuildingInfo.json");
            GameState.BuildingInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<ColonyBuilding, BuildingInfo>>(json);
        }
    }
}
