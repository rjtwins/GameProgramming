using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    internal class FuelTank : SubSystemBase
    {
        public double Capacity { get; set; }

        public override string Report()
        {
            var reportString = base.Report();

            reportString += $"CAPACITY: {Capacity} TON\n";
            reportString += $"WETT MASS: {Capacity + Mass} TON\n";
            return reportString;
        }
    }
}
