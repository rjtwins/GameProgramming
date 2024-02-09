﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    internal class Ship : Station
    {
        //In KN
        public long MaxThrust { get; set; }
        public long Thrust { get; set; }

        //In kilo liters per second
        public long FuelConsumption { get; set; }

        //In seconds.
        public long BurnLeft => Fuel / (FuelConsumption * (MaxThrust / Thrust));
        public long HalfBurnLeft => BurnLeft / 2;
        public long TotalBurn => MaxFuel / (FuelConsumption * (MaxThrust / Thrust));
        public long HalfTotalBurn => TotalBurn / 2;

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