using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public class Species
    {
        //in atm
        public double MinAtmos { get; set; } = 0.8d;
        public double MaxAtmos { get; set; } = 1.2d;

        //For each gas min amount needed.
        public Dictionary<Gas, (double min, double max)> GasRequired { get; set; }

        public double MinGrav { get; set; } = 0.6d;
        public double MaxGrav { get; set; } = 1.5d;

        public Species()
        {
            GasRequired = new Dictionary<Gas, (double min, double max)>();
            GasRequired[Gas.O2] = (0.1, 1.3);
        }
    }
}
