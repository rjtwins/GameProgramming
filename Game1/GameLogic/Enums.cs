﻿using Game1.GameEntities;
using System;

namespace Game1.GameLogic
{
    public enum Gas
    {
        N2,
        O2,
        Ar,
        CO2,
        CO,
        He,
        H,
        Ne,
        CH4,
        H2S,
        H2Ov,
        NHO3,
        GHG,
        AGG
    }
    
    //For easy checks after calc
    public enum AtmosType
    {
        None,
        Trace,
        Normal,
        HighPressure,
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

    public enum BodySubType
    {
        Frozen,
        Normal,
        Hot,
        Molten
    }

    public enum BodySizeType
    {
        Dwarf,
        Normal,
        Giant
    }

    public enum BodyType
    {
        Terrestrial,
        Barren,
        Gas,
        Solar,
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

    public enum ColonyBuilding
    {
        //IC and co:
        Infrastructure,
        HabInfrastructure,
        ProductionFactory,
        ResearchFacility,
        ShipBuildingSupport,
        Mine,
        AutomatedMine,
        FinancialCenter,
        TerraformingFacility,
        GeneticsFacility,
        AgriCenter,
        //Cargo handling:
        SpacePort,
        ShuttlePort,
        //Orbital:
        CivSlipway,
        MilSlipway,
        CivSlipwayCap,
        MilSlipwayCap
    }

    public enum BodyCoreType
    {
        Inactive,
        Molten,
        Plasma
    }
}
