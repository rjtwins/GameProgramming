using Game1.GameEntities;
using Game1.GameLogic.Research;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Game1.GameLogic
{
    public class Faction
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Color color { get; set; }

        public List<Fleet> Fleets { get; set; } = new();
        //public List<Colony> Colonies { get; set; } = new();

        public List<SensorContact> SensorContacts { get; set; } = new ();
        public List<FleetGhost> DetectedFleets { get; set; }
        public Species Species { get; set; }
        public List<ResearchNode> ResearchNodes { get; set; } = new();
        public List<ResearchNode> CurrentNode { get; set; } = new();

        //Tech mods:
        public double InfrastructureMod { get; set; } = 1d;

        public Faction()
        {
            Guid = Guid.NewGuid();
        }
    }
}
