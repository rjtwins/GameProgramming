using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    //Entities that are not on the map directly.
    public abstract class SubGameEntity
    {
        public Guid Guid { get; set; }

        //Crew in men
        public int Crew { get; set; }

        //In metric ton
        public long Mass { get; set; }

        //in kilo liters
        public long Fuel { get; set; }
        public long MaxFuel { get; set; }
    }
}
