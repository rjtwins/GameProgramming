using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public class Thruster : Engine
    {
        public override SubSystemType SubsystemType => SubSystemType.Engine;

        public override string Report()
        {
            var reportString = base.Report();

            reportString += $"THRUST: {Thrust} KN\n";

            return reportString;
        }
    }
}
