using Game1.GameEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game1.GameLogic
{
    public class Colony : AnimatableBase
    {
        public Faction Faction {  get; set; }
        public double Population { get; set; } = 10e6;
        public string Name { get; set; }

        //BPs per sec.
        public double BPGeneration { get; private set; }
        public double BGGenerationDay => BPGeneration * 86400;

        public double AgriMod { get; private set; }
        public double AgriPop { get; private set; }

        public double InfrastructureNeeded => Faction.InfrastructureMod * (Population / 1e5);
        public double PopulationSupported
        {
            get
            {
                if (!HostBody.GravCheck(Faction.Species))
                    return 0;

                if (HostBody.NeedHab(Faction.Species))
                    return CurrentBuildings[ColonyBuilding.HabInfrastructure] * 100000;

                return int.MaxValue;
            }
        }

        public Orbital HostBody { get; set; }
        public Dictionary<ColonyBuilding, double> CurrentBuildings { get; set; } = new ();
        public Dictionary<ColonyBuilding, BuildingQueueItem> BuildingQueue { get; set; } = new ();
        public Dictionary<Resource, double> ResourceStockpiles { get; set; } = new ();

        public Colony()
        {
            //Setup;
            Enum.GetValues<ColonyBuilding>().ToList().ForEach(x =>
            {
                CurrentBuildings[x] = 0;
            });

            Enum.GetValues<Resource>().ToList().ForEach(x =>
            {
                ResourceStockpiles[x] = 0;
            });
        }

        public override void Update(double deltaTime)
        {
            if (HostBody == null)
                return;

            //Todo calc agri pop.
            AgriMod = (GlobalStatic.BasicAgri - CurrentBuildings[ColonyBuilding.AgriCenter] * 0.01 * GlobalStatic.BasicAgri);
            if (HostBody.NeedHab(Faction.Species))
                AgriMod += 0.25;
            else if (!HostBody.BreathableCheck(Faction.Species))
                AgriMod += 0.10;
            AgriPop = Population * AgriMod;

            var avialPop = Population - AgriPop;
            var popNeeded = CurrentBuildings.Select(x => x.Value * GameState.BuildingInfo[x.Key].pop).Sum();
            var efficiency = Math.Min(popNeeded / Population, 1);

            //How many build points where generated over the delta time.
            BPGeneration = CurrentBuildings[ColonyBuilding.ProductionFactory] * efficiency;

            var bp = BPGeneration * deltaTime;

            //Process building:
            BuildingQueue
                .Where(x => x.Value.Allocation > 0)
                .ToList()
                .ForEach(x =>
                {
                    x.Value.Progress += bp * x.Value.Allocation;
                    var remainder = x.Value.Progress % 1;
                    var build = x.Value.Progress - remainder;
                    x.Value.Amount -= build;
                    var current = CurrentBuildings[x.Key];
                    CurrentBuildings[x.Key] = current + build;
                });
        }

        public double PopInBuilding(ColonyBuilding colonyBuilding)
        {
            return CurrentBuildings[colonyBuilding] * GameState.BuildingInfo[colonyBuilding].pop;
        }
    }
}
