using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public class Sensor : SubSystemBase
    {
        public override SubSystemType SubsystemType => SubSystemType.Sensor;
        public bool Active { get; set; }
        public double Range {  get; set; }
        public double Resolution { get; set; }
    }
}
