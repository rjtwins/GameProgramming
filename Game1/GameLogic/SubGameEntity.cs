using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    //Entities that are not on the map directly.
    public abstract class SubGameEntity
    {
        public virtual string Name { get; set; }

        public Guid Guid { get; set; }

        public Faction Faction { get; set; }

        //Crew in men
        public virtual int CrewRequired { get; set; }
        public virtual int CrewCapacity { get; set; }
        public virtual int Crew { get; set; }

        //In metric ton
        public virtual double Mass { get; set; }

        //in kilo liters
        public double Fuel { get; set; }
        public virtual double MaxFuel { get; set; }

        //kW
        public virtual double EnergyRequired { get; set; }
        public virtual double EnergyGeneration { get; set; }
        public virtual double EnergyMaxStorage { get; set; }

        //kJ
        public virtual double EnergyStorage { get; set; }

        public virtual string ToJson()
        {
            var text = System.Text.Json.JsonSerializer.Serialize(this);
            Debug.WriteLine(text);
            return text;
        }

        public static T FromJson<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public virtual string Report()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
