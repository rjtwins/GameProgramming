﻿using System;

namespace Game1.GameLogic.SubSystems
{
    public abstract class SubSystemBase : ICloneable
    {
        public Guid Guid {  get; set; }
        public Guid DesignGuid { get; set; }

        public Faction Faction { get; set; }

        public double Damage { get; set; } = 0;

        public bool Disabled = false;

        public bool IsOn { get; set; } = true;

        public virtual SubSystemType SubsystemType { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Mass { get; set; } = 10;

        public int BuildPoints { get; set; } = 10;

        public double EnergyGeneration { get; set; }
        public double EnergyStorage { get; set; }
        public double EnergyRequired { get; set; }

        public double ThermalOutput { get; set; }
        public double EMOutput { get; set; }

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
            DesignGuid = Guid.NewGuid();
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
