using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public abstract class ThermalSystems : SubSystemBase
    {
        public double Capacity { get; set; }
    }

    public class ThermalRadiator : ThermalSystems
    {
    }

    public class ThermalBattery : ThermalSystems
    {
    }

    public class ThermalRecycler : ThermalSystems
    {
    }
}
