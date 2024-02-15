using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    internal class CrewBerths : SubSystemBase
    {
        public override SubSystemType SubsystemType => SubSystemType.CrewBerths;

        public int Capacity { get; set; } = 1;

        public override string Report()
        {
            var reportString = base.Report();

            reportString += $"CAPACITY: {Capacity} TON\n";

            return reportString;
        }
    }
}
