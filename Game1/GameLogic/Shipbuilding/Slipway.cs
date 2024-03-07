using System;
using System.Collections.Generic;
using System.Linq;

namespace Game1.GameLogic.Shipbuilding
{
    public class Slipway
    {
        public Shipyard Shipyard { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public int Id { get; set; } = 0;

        //Capacity in ton.
        public int Capacity { get; set; } = 100;

        public ShipDesign CurrentlyBuilding { get; set; }
        public List<ShipDesign> Queue { get; set; } = new();

        //IC per day:
        public int IC { get; set; } = 1;
        public double ProgressBP { get; set; } = 0;
        public double ProgressPerc => ProgressBP / (CurrentlyBuilding.GetBuildPoints()) * 100;
        public double TimeUntilCompleteion => (CurrentlyBuilding.GetBuildPoints()) - ProgressBP / IC;

        public Slipway(Shipyard shipyard)
        {
            Shipyard = shipyard;
        }

        /// <summary>
        /// Updates the system based on the time passed.
        /// </summary>
        /// <param name="timePassed">The time passed in seconds.</param>
        public void Update(double timePassed)
        {
            // Calculate the change
            var change = IC * (timePassed / 86400);

            // Update the system
            UpdateRecursive(change);
        }

        /// <summary>
        /// Updates the progress of the building process.
        /// </summary>
        /// <param name="ic">The increment of progress.</param>
        private void UpdateRecursive(double ic)
        {
            // Calculate remaining build points and total resource cost
            var pointsTillCompletion = (CurrentlyBuilding.GetBuildPoints() - ProgressBP);
            var totalResourceCost = CurrentlyBuilding.GetResourceCost();

            // Calculate the build percentage and current resource cost
            var buildPerc = Math.Min(ic, pointsTillCompletion) / pointsTillCompletion;
            var currentResourceCost = totalResourceCost.ToDictionary(x => x.Key, x => x.Value * buildPerc);

            // Check and update resources
            var colony = Shipyard.Colony;

            // Check if there are enough resources for building
            if (currentResourceCost.Any(x => colony.ResourceStockpiles[x.Key] < x.Value))
            {
                // TODO: Event out of resources for building ships.
                return;
            }

            // Deduct the used resources from the colony
            currentResourceCost.ToList().ForEach(x => colony.ResourceStockpiles[x.Key] -= x.Value);

            // If the building process is not completed, update progress and return
            if (ic < pointsTillCompletion)
            {
                ProgressBP += ic;
                return;
            }

            // If the building process is completed, trigger the ShipCompleted event
            ShipCompleted(CurrentlyBuilding);

            // Deduct the completed build points from the increment
            ic -= pointsTillCompletion;

            // If there are more items in the queue, continue building the next item
            if (Queue.Count == 0)
                return;

            CurrentlyBuilding = Queue.First();
            Queue.Remove(CurrentlyBuilding);

            // Recursively call UpdateRecursive with the remaining increment
            UpdateRecursive(ic);
        }

        private void ShipCompleted(ShipDesign shipDesign)
        {
            ShipInstance ship = shipDesign.Clone() as ShipInstance;
            Shipyard.Colony.HomeFleet.Members.Add(ship);
        }
    }
}
