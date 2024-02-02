using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class Ship
    {
        public Guid Guid { get; set; }

        //Names and stuff;
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShipClass { get; set; }


        //In 1000kg (ton)
        public int Mass { get; set; }
        //Crew in men
        public int Crew { get; set; }

        //In KN
        public long MaxThrust { get; set; }
        public long Thrust { get; set; }

        //In kM/s (kilometers per second)
        public long Speed { get; set; }

        //in kilo liters
        public long Fuel { get; set; }
        public long MaxFuel { get; set; }

        //In kilo liters per second
        public long FuelConsumption { get; set; }

        //In seconds.
        public long BurnLeft => Fuel / (FuelConsumption * (MaxThrust / Thrust));
        public long HalfBurnLeft => BurnLeft / 2;
        public long TotalBurn => MaxFuel / (FuelConsumption * (MaxThrust / Thrust));
        public long HalfTotalBurn => TotalBurn / 2;

        public Ship()
        {
            
        }

        public double CalculateMaxDistance(bool full = false)
        {
            var halfBurnTime = full ? HalfTotalBurn : HalfBurnLeft;

            // Calculate acceleration due to thrust
            double acceleration = (double)MaxThrust / Mass;

            // Calculate distance traveled during acceleration
            double distanceDuringAcceleration = 0.5 * acceleration * halfBurnTime * halfBurnTime;
            
            //Speed after acceleration
            double maxSpeed = acceleration * halfBurnTime;

            // Calculate distance traveled during deceleration
            double distanceDuringDeceleration = maxSpeed * halfBurnTime - 0.5 * acceleration * halfBurnTime * halfBurnTime;

            // Total distance traveled
            double totalDistance = distanceDuringAcceleration + distanceDuringDeceleration;

            return totalDistance;
        }
    }
}
