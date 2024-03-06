using Game1.GameLogic.SubSystems;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Game1.GameLogic
{
    public class ShipDesign : SubGameEntity, ICloneable
    {
        //Class
        public string ShipClass { get; set; } = string.Empty;

        public List<SubSystemBase> SubSystems { get; set; } = new();

        //in kG
        public double HullMass => SubSystems.Select(x => x.Mass).Sum();
        public double WettMass => SubSystems.Select(x => x.Mass).Sum() + MaxFuel;
        public double Volume => WettMass / 1.025;
        public double Surface => 6 * Math.Pow(Volume, 2 / 3);

        //Ton
        public double ArmorMass => WettMass + SubSystems.OfType<Armor>().Select(x => x.Weight * x.Depth * Surface).Sum();

        //In KN
        public double MaxThrust => SubSystems.Where(x => x is Engine).Cast<Engine>().Select(x => x.Thrust).Sum();
        
        //KM/s
        public double MaxSpeed => (MaxThrust / ArmorMass);

        //Thermals in KW
        public double MaxThermalSig =>
            SubSystems.OfType<ThermalRadiator>().Select(x => x.Capacity).Sum() +
            SubSystems.OfType<Engine>().Select(x => x.ThermalFlare).Sum();
        public double NominalThermalSig
        {
            get
            {
                var engines = SubSystems.OfType<Engine>().Select(x => x.ThermalFlare).Sum();
                var nominal = SubSystems
                    .Where(x => x.SubsystemType != SubSystemType.Turret)
                    .Where(x => x.SubsystemType != SubSystemType.Weapon)
                    .Where(x => x.SubsystemType != SubSystemType.Shields)
                    .Where(x =>
                    {
                        if (x is Sensor s)
                            return s.SensorType != SensorType.Active;
                        return true;
                    })
                    .Select(x => x.ThermalOutput)
                    .Sum();

                return engines + nominal;
            }
        }
        public double MinimalThermalSig
        {
            get
            {
                var minimal = SubSystems
                    .Where(x => x.SubsystemType != SubSystemType.Turret)
                    .Where(x => x.SubsystemType != SubSystemType.Weapon)
                    .Where(x => x.SubsystemType != SubSystemType.Shields)
                    .Where(x => x.SubsystemType != SubSystemType.Engine)
                    .Where(x =>
                    {
                        if (x is Sensor s)
                            return s.SensorType != SensorType.Active;
                        return true;
                    })
                    .Select(x => x.ThermalOutput)
                    .Sum();

                var reactorLevel = minimal / SubSystems.OfType<Reactor>().Select(x => x.EnergyGeneration).Sum();
                var reactorThermals = SubSystems.OfType<Reactor>().Select(x => x.ThermalOutput).Sum() * reactorLevel;

                return minimal;
            }
        }

        public double MaxThermalStorage => SubSystems.OfType<ThermalBattery>().Sum(x => x.Capacity);
        

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

        public int GetBuildPoints()
        {
            return 1000;
        }

        public object Clone()
        {
            var ghost = this.MemberwiseClone() as ShipDesign;
            ghost.SubSystems = this.SubSystems.Select(x => x.Clone() as SubSystemBase).ToList();
            return ghost;
        }
    }
}
