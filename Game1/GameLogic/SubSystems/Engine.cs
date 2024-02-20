using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public class Engine : SubSystemBase
    {
        public override SubSystemType SubsystemType => SubSystemType.Engine;

        //Kilo Newton
        public double Thrust { get; set; } = 10;

        //kg/s
        public double FuelConsumption { get; set; } = 10;

        //Thermal sig, engines directly adding to thermal sig apart from their internal thermal output.
        public double ThermalFlare { get; set; } = 10;

        public override string Report()
        {
            var reportString = base.Report();

            reportString += $"THRUST: {Thrust} KN\n";
            reportString += $"FUEL CONSUMPTION: {FuelConsumption} TON\\s\n";


            return reportString;
        }
    }
}
