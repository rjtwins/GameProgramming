using Game1.GameEntities;
using Game1.GameLogic.SubSystems;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public class ShipInstance : ShipDesign
    {
        //TODO: Overwrite some of the max methods used for calc if component can be turned off and that impacts calc.

        public bool IsActiveEntity { get; set; }

        public Fleet Fleet { get; set; }

        //Thermals
        public double CurrentThermalSig {  get; set; }
        public double CurrentThermalGeneration { get; set; }
        public double CurrentThermalStorage { get; set; }


        //Energy
        public double CurrentEnergyGeneration { get; set; }
        public double CurrentEnergyRequirement { get; set; }
        public double CurrentEnergyStorage { get; set; }

        //EMSig
        public double CurrentEMSig {  get; set; }

        //Thrust and Speed
        public double Thrust => Math.Min(MaxThrust, (Speed * ArmorMass));

        public double DesignMaxThrust { get; private set; }

        public double CurrentMaxThrust { get; set; }

        //Current speed in km/s set externally
        public double Speed { get; set; }

        //Intermediates:
        public double CurrentEngineLevel => Thrust / MaxThrust;

        //init from design vars in constructor:
        public ShipInstance()
        {
            DesignMaxThrust = MaxThrust;
        }

        public void StartInstance()
        {

        }

        public void Update(double timePassed)
        {



            HandleHeat(timePassed);
            HandleEnergy(timePassed);
            HandleDamageControll(timePassed);
        }


        public void HandleHeat(double timePassed)
        {
            var heatGen = SubSystems.Sum(x => x.ThermalOutput) * timePassed;
            var heatDissipated = SubSystems
                .OfType<ThermalRadiator>()
                .Where(x => x.IsOn)
                .Sum(x => x.Capacity) + 
                SubSystems
                .OfType<ThermalRecycler>()
                .Sum(x => x.Capacity);

            heatDissipated *= timePassed;

            var heatDiv = heatGen - heatDissipated;

            CurrentThermalStorage += heatDiv;
            
            if(CurrentThermalStorage > MaxThermalStorage)
            {
                //handle thermal damage...
            }

            CurrentThermalStorage = Math.Clamp(CurrentThermalStorage, 0, MaxThermalStorage);
        }

        public void HandleEnergy(double timePassed) 
        {
            var totalEnergyRequired = SubSystems.Where(x => x.IsOn).Sum(x => x.EnergyRequired) * timePassed;

        }

        public void HandleDamageControll(double timePassed)
        {

        }

        //TODO: Worry about getting hit.
        //We will worry about getting hit later.
        public void Hit()
        {

        }
    }
}
