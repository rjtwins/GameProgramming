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

        public void Update(double timePassed)
        {
            var ic = IC * timePassed / 86400;
            var pointsTillCompletion = (CurrentlyBuilding.GetBuildPoints() - ProgressBP);

            //Not done building
            if (ic < pointsTillCompletion)
            {
                ProgressBP += ic;
                return;
            }
            //Done building:
            //TODO: Trigger event!!

            ic -= pointsTillCompletion;

            if (Queue.Count == 0)
                return;

            CurrentlyBuilding = Queue.First();
            Queue.Remove(CurrentlyBuilding);
            ProgressBP = ic;
        }
    }
}
