using Game1.GameEntities;
using Game1.GameLogic.Shipbuilding;
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
        public Dictionary<Resource, List<(double time, double value)>> ResourceStockpileChangeLog { get; set;} = new ();

        public List<Shipyard> Shipyards { get; set; } = new List<Shipyard>();
        public Fleet HomeFleet { get; private set; }

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

            Enum.GetValues<Resource>().ToList().ForEach(x =>
            {
                ResourceStockpileChangeLog[x] = new();
            });

            HomeFleet = new();
            HomeFleet.SOIEntity = HostBody;
            GameState.Colonies.Add(this);
            GameState.GameEntities.Add(HomeFleet);
        }

        public override void Update(double deltaTime)
        {
            if (HostBody == null)
                return;

            AgriMod = (GlobalStatic.BasicAgri - CurrentBuildings[ColonyBuilding.AgriCenter] * 0.01 * GlobalStatic.BasicAgri);

            if (HostBody.NeedHab(Faction.Species))
                AgriMod += 0.25;
            else if (!HostBody.BreathableCheck(Faction.Species))
                AgriMod += 0.10;
            AgriPop = Population * AgriMod;

            var avialPop = Population - AgriPop;
            var popNeeded = CurrentBuildings.Select(x => x.Value * GameState.BuildingInfo[x.Key].Pop).Sum();
            var efficiency = Math.Min(avialPop / popNeeded, 1);

            //Get ic generated since last update:
            ICGeneration = CurrentBuildings[ColonyBuilding.ProductionFactory] * GameState.BuildingInfo[ColonyBuilding.ProductionFactory].ICGen * efficiency;
            var ic = (ICGeneration * deltaTime) / 86400;

            //Update build queue
            UpdateBuilding(deltaTime, ic);

            UpdateMining(deltaTime, efficiency);

            UpdateShipBuilding(deltaTime);

            HomeFleet.X = HostBody.X;
            HomeFleet.Y = HostBody.Y;

            //if(_timeSinceStockpileLog >= 2.628e6)
            //    ResourceStockpiles.ToList().ForEach(x => PreviousResourceStockpiles)
        }

        private void UpdateBuilding(double deltaTime, double ic)
        {
            var icRemaining = ic;
            //Process building:
            BuildingQueue
                .Where(x => x.Allocation > 0)
                .Where(x => x.Status != Status.Paused)
                .ToList()
                .ForEach(x =>
                {
                    if (ic <= 0)
                        return;

                    if (x.Allocation <= 0 && x.Inf == false)
                        return;

                    if (icRemaining <= 0)
                        return;

                    var icCost = GameState.BuildingInfo[x.ColonyBuilding].IC;
                    var icUsed = Math.Min(ic * x.Allocation, icCost);
                    var progressFraction = icUsed / icCost;

                    var resourceCost = GameState.BuildingInfo[x.ColonyBuilding].BuildCost.ToDictionary(x => x.Key, x => x.Value * progressFraction);

                    //Check if we have the resources for this step:
                    if (resourceCost.ToList().Any(x => ResourceStockpiles[x.Key] < x.Value))
                    {
                        x.Status = Status.WaitingForResources;
                        return;
                    }

                    //Subtract resources.
                    resourceCost.ToList().ForEach(x => ResourceStockpiles[x.Key] = ResourceStockpiles[x.Key] - x.Value);

                    icRemaining -= icUsed;

                    if (icRemaining < 0)
                        icUsed = Math.Max(0, icUsed + icRemaining);

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
        }

        private void UpdateMining(double deltaTime, double efficiency)
        {
            //Automated:
            var mine = CurrentBuildings[ColonyBuilding.AutomatedMine] * deltaTime / 86400;
            mine +=  CurrentBuildings[ColonyBuilding.Mine] * efficiency * deltaTime / 86400;

            HostBody.Resources.ToList().ForEach(x =>
            {
                if (x.Value.amount <= 0)
                    return;

                var mined = x.Value.access * mine;
                var newAmount = (int)Math.Floor(x.Value.amount - mined);

                if(newAmount < 0)
                    mined += newAmount;

                mine = Math.Max(0, mined);
                newAmount = Math.Max(0, newAmount);

                HostBody.Resources[x.Key] = (newAmount, x.Value.access);
                ResourceStockpiles[x.Key] += mined;

                //Keeping track of past values;
                ResourceStockpileChangeLog[x.Key].Add((GameState.TotalSeconds, ResourceStockpiles[x.Key]));
                var toRemove = ResourceStockpileChangeLog[x.Key].Where(y => GameState.TotalSeconds - y.time > 100000);
                toRemove.ToList().ForEach(y => ResourceStockpileChangeLog[x.Key].Remove(y));
            });
        }

        private void UpdateShipBuilding(double deltaTime)
        {
            Shipyards.ForEach(x => x.Update(deltaTime));
        }

        public Dictionary<Resource, double> GetDailyMining()
        {
            var record = Enum.GetValues<Resource>().ToDictionary(x => x, x => ResourceStockpileChangeLog[x].FirstOrDefault(y => GameState.TotalSeconds - y.time >= 86400).value);
            return record.ToDictionary(x => x.Key, x => ResourceStockpiles[x.Key] - x.Value);
        }

        public double PopInBuilding(ColonyBuilding colonyBuilding)
        {
            return CurrentBuildings[colonyBuilding] * GameState.BuildingInfo[colonyBuilding].Pop;
        }

        /// <summary>
        /// Checks if a building type can be constructed on the colony
        /// </summary>
        /// <param name="colonyBuilding">The type of building to be constructed</param>
        /// <returns>True if the building can be constructed, otherwise false</returns>
        public bool CanBuildOnColony(ColonyBuilding colonyBuilding)
        {
            switch (colonyBuilding)
            {
                case ColonyBuilding.ResearchAcademy:
                    // Check if there are no existing research academies in any colony of the same faction
                    return GameState.Colonies.Where(x => x.Faction == Faction).All(x => x.CurrentBuildings[ColonyBuilding.ResearchAcademy] == 0);
                case ColonyBuilding.SpacePort:
                    // Check if there are no existing space ports in the colony
                    return CurrentBuildings[ColonyBuilding.SpacePort] == 0;
                case ColonyBuilding.CivShipyard:
                    // Check if there are no existing civilian shipyards in the colony
                    return CurrentBuildings[ColonyBuilding.CivShipyard] == 0;
                case ColonyBuilding.MillShipyard:
                    // Check if there are no existing military shipyards in the colony
                    return CurrentBuildings[ColonyBuilding.MillShipyard] == 0;
                case ColonyBuilding.CivSlipway:
                    // Check if there is exactly one existing civilian slipway in the colony
                    return CurrentBuildings[ColonyBuilding.CivShipyard] == 1;
                case ColonyBuilding.MilSlipway:
                    // Check if there is exactly one existing military slipway in the colony
                    return CurrentBuildings[ColonyBuilding.MillShipyard] == 1;

                case ColonyBuilding.CivSlipwayCap:
                    // New civilian slipways cannot be constructed
                    return false;
                case ColonyBuilding.MilSlipwayCap:
                    // New military slipways cannot be constructed
                    return false;

                // The following buildings can be constructed without any restrictions
                //case ColonyBuilding.HabInfrastructure:
                //case ColonyBuilding.Infrastructure:
                //case ColonyBuilding.ProductionFactory:
                //case ColonyBuilding.ResearchOutpost:
                //case ColonyBuilding.ShipBuildingSupport:
                //case ColonyBuilding.Mine:
                //case ColonyBuilding.AutomatedMine:
                //case ColonyBuilding.FinancialCenter:
                //case ColonyBuilding.MassDriver:
                //case ColonyBuilding.TerraformingFacility:
                //case ColonyBuilding.GeneticsFacility:
                //case ColonyBuilding.AgriCenter:
                //case ColonyBuilding.ShuttlePort:
                //    return true;
                default:
                    // By default, new building can be constructed
                    return true;
            }
        }

    }
}
