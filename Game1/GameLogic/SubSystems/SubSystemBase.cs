using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public abstract class SubSystemBase : ICloneable
    {
        public Guid Guid {  get; set; }
        public virtual SubSystemType SubsystemType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Mass { get; set; } = 10;

        public double EnergyGeneration { get; set; }
        public double EnergyStorage { get; set; }
        public double EnergyRequired { get; set; }

        public int CrewRequired { get; set; } = 1;
        public int CrewPriority { get; set; } = 0;

        public int HP { get; set; } = 10;

        //resource cost placeholders.
        public int Cost1 { get; set; }
        public int Cost2 { get; set; }
        public int Cost3 { get; set; }

        public bool Reinforced { get; set; }

        public SubSystemBase()
        {
            Guid = Guid.NewGuid();
        }

        public virtual string Report()
        {
            var reportString = string.Empty;
            reportString += $"MASS: {Mass}\n";
            //reportString += $"ENERGY CONSUMPTION: {EnergyRequired}\n";

            return reportString;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
