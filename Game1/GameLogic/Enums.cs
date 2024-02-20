using Game1.GameEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    //For easy checks after calc
    public enum AtmosType
    {
        None,
        TraceGas,
        Gas,
        Liquid,
        HighPressure
    }


    public enum Resource
    {
        Water,
        Fissiles,
        Fusibles,
        Volatiles,
        Metals,
        RareMetals
    }

    public enum SatelliteType
    {
        Terrestrial,
        SuperTerrestrial,
        Gas,
        SuperGas,
        Solar
    }

    public enum SensorType
    {
        Active,
        Optical,
        EM,
        IR
    }

    public enum ArmorType
    {
        Passive,
        Reactive,
        Reflective,
    }

    public enum SubSystemType
    {
        Cargo,
        CargoHuman,
        Weapon,
        Sensor,
        Engine,
        Reactor,
        Turret,
        Armor,
        CrewBerths,
        Shields
    }

    public enum OrderType
    {
        Stop,
        MoveTo,
        InterceptObject,
        Orbit,
        Dock
    }

    public class Order
    {
        public Guid Guid { get; set; }
        public OrderType OrderType { get; set; }
        public string Label { get; set; }
        public (decimal x, decimal y) Position {  get; set; }
        public GameEntity Owner {  get; set; }
        public GameEntity Target { get; set; }
    }

    public enum WeaponType
    {
        Laser,
        Ballistic,
        Missile,
        Particle
    }
}
