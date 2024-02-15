using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public class CargoBay : SubSystemBase
    {
        public override SubSystemType SubsystemType => HumanRated ? SubSystemType.CargoHuman : SubSystemType.Cargo;
        public bool HumanRated { get; set; } = false;
        public double Capacity { get; set; } = 2000;

        public override string Report()
        {
            var reportString = base.Report();

            var rate = HumanRated ? "HUMAN" : "CARGO";
            reportString += $"CAPACITY: {Capacity} TON\n";
            reportString += $"RATE: {HumanRated} TON\n";

            return reportString;
        }
    }
}
