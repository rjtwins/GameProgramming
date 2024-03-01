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

        //BPs per day.
        public double ICGeneration { get; private set; }
        public double ICGenerationDay => ICGeneration;

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
        public List<BuildingQueueItem> BuildingQueue { get; set; } = new ();
        public Dictionary<Resource, double> ResourceStockpiles { get; set; } = new ();
        private double _timeSinceStockpileLog = 0d;
        //public Dictionary<Resource, double> PreviousResourceStockpiles { get; set; } = new();

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

            //_timeSinceStockpileLog += deltaTime;

            //Todo calc agri pop.
            AgriMod = (GlobalStatic.BasicAgri - CurrentBuildings[ColonyBuilding.AgriCenter] * 0.01 * GlobalStatic.BasicAgri);

            if (HostBody.NeedHab(Faction.Species))
                AgriMod += 0.25;
            else if (!HostBody.BreathableCheck(Faction.Species))
                AgriMod += 0.10;
            AgriPop = Population * AgriMod;

            var avialPop = Population - AgriPop;
            var popNeeded = CurrentBuildings.Select(x => x.Value * GameState.BuildingInfo[x.Key].Pop).Sum();
            var efficiency = Math.Min(avialPop / popNeeded, 1);

            //How many build points where generated over the delta time.
            ICGeneration = CurrentBuildings[ColonyBuilding.ProductionFactory] * GameState.BuildingInfo[ColonyBuilding.ProductionFactory].ICGen * efficiency;

            var ic = (ICGeneration * deltaTime) / 86400;

            //Process building:
            BuildingQueue
                .Where(x => x.Allocation > 0)
                .ToList()
                .ForEach(x =>
                {
                    if (ic <= 0)
                        return;

                    if (x.Allocation <= 0 && x.Inf == false)
                        return;

                    var icCost = GameState.BuildingInfo[x.ColonyBuilding].IC;
                    var icUsed = ic * x.Allocation;

                    x.Progress += icUsed;
                    var build = Math.Floor(x.Progress / icCost);
                    x.Amount -= build;
                    x.Progress = x.Progress - (build * icCost);

                    var current = CurrentBuildings[x.ColonyBuilding];
                    CurrentBuildings[x.ColonyBuilding] = current + build;
                });

            var done = BuildingQueue.Where(x => x.Amount <= 0 && x.Inf == false).ToList();
            done.ForEach(x =>
            {
                if (x.PauseOnCompletion)
                    GameState.Paused = true;

                //TODO: Notify user via event?
            });

            BuildingQueue.RemoveAll(x => done.Contains(x));

            //if(_timeSinceStockpileLog >= 2.628e6)
            //    ResourceStockpiles.ToList().ForEach(x => PreviousResourceStockpiles)
        }

        public double PopInBuilding(ColonyBuilding colonyBuilding)
        {
            return CurrentBuildings[colonyBuilding] * GameState.BuildingInfo[colonyBuilding].Pop;
        }
    }
}
