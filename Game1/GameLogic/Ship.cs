using Game1.GameLogic.SubSystems;
using System;
using System.Data;
using System.Linq;

namespace Game1.GameLogic
{
    public class Ship : Station, ICloneable
    {
        //in kG
        public double HullMass => SubSystems.Select(x => x.Mass).Sum();
        public double WettMass => SubSystems.Select(x => x.Mass).Sum() + MaxFuel;
        public double Volume => WettMass / 1.025;
        public double Surface => 6 * Math.Pow(Volume, 2/3);

        public double ArmorMass => WettMass + SubSystems.OfType<Armor>().Select(x => x.Weight * x.Depth * Surface).Sum();

        //In KN
        public double MaxThrust => SubSystems.Where(x => x is Engine).Cast<Engine>().Select(x => x.Thrust).Sum();
        //For instance
        public double Thrust { get; set; }
        public double Speed { get; set; }

        public double MaxSpeed => (MaxThrust / ArmorMass) * 1000;


        //kL/s or Ton/s
        public double FuelConsumption => SubSystems.OfType<Engine>().Select(x => x.FuelConsumption).Sum();
        //kL or Ton/s
        public override double MaxFuel => SubSystems.OfType<FuelTank>().Select(x => x.Capacity).Sum();

        //Person
        public override int CrewCapacity => SubSystems.OfType<CrewBerths>().Select(x => x.Capacity).Sum();
        public override int CrewRequired => SubSystems.Select(x => x.CrewRequired).Sum();


        //kW
        public override double EnergyGeneration => SubSystems.Select(x => x.EnergyGeneration).Sum();
        public override double EnergyRequired => SubSystems.Select(x => x.EnergyRequired).Sum();

        //kJ
        public override double EnergyMaxStorage => SubSystems.Select(x => x.EnergyGeneration).Sum();
        //Sec
        public virtual double TimeOnStores => EnergyStorage / EnergyRequired;

        public bool IsLegal()
        {
            return true;
        }

        public object Clone()
        {
            var ghost = this.MemberwiseClone() as Ship;
            ghost.SubSystems = this.SubSystems.Select(x => x.Clone() as SubSystemBase).ToList();
            return ghost;
        }
    }
}
