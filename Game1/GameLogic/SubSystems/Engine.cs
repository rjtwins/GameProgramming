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
    }
}
