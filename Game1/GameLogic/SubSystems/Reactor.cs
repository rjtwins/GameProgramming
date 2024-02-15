using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public class Reactor : SubSystemBase
    {
        public override SubSystemType SubsystemType => SubSystemType.Reactor;

        public override string Report()
        {
            var reportString = base.Report();

            reportString += $"GENERATION: {EnergyGeneration} KW\n";
            reportString += $"OPERATING: {EnergyRequired} TON\n";
            reportString += $"STORAGE: {EnergyStorage} TON\n";

            return reportString;
        }
    }
}
