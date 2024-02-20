using Game1.GameEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public class Faction
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Color color { get; set; }

        public List<Fleet> Fleets { get; set; } = new();
        public List<Colony> Colonies { get; set; } = new();

        public List<SensorContact> SensorContacts { get; set; } = new ();
        public List<FleetGhost> DetectedFleets { get; set; }

        public Faction()
        {
            Guid = Guid.NewGuid();
        }
    }
}
