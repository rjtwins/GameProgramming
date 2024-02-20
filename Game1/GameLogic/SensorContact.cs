using Game1.GameEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public class SensorContact
    {
        public SensorType ContactType { get; set; } = SensorType.Active;
        public string Label { get; set; }

        public Fleet Contact {  get; set; }
        public FleetGhost ContactGhost { get; set; }
        public double GameTime { get; set; }
    }
}
