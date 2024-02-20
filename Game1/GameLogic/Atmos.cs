using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public class Atmos
    {
        public AtmosType AtmosType {  get; set; }

        //Pressure in atmospheres.
        public float Nitrogen { get; set; } = 0f;
        public float Oxygen { get; set; } = 0f;
        public float Argon { get; set; } = 0f;
        public float CarbonDioxide { get; set; } = 0f;
        public float CarbonMonoxide { get; set; } = 0f;
        public float Helium { get; set; } = 0f;
        public float Hydrogen { get; set; } = 0f;
        public float Neon { get; set; } = 0f;
        public float Methane { get; set; } = 0f;
        public float HydrogenSulfide { get; set; } = 0f;
        public float WaterVapor { get; set; } = 0f;
        public float NitricAcid { get; set; } = 0f;
        
        //In tons
        public float LiquidWater { get; set; } = 0f;

        public float AtmosPressure => Nitrogen + 
            Oxygen + 
            Argon + 
            CarbonDioxide + 
            CarbonMonoxide + 
            Helium + 
            Hydrogen + 
            Neon + 
            Methane + 
            HydrogenSulfide + 
            WaterVapor + 
            NitricAcid;
    }
}
