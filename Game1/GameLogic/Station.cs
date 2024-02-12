using Game1.GameLogic.SubSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic
{
    public class Station : SubGameEntity
    {
        //Class
        public string ShipClass { get; set; }

        public List<SubSystemBase> SubSystems { get; set; }
    }
}
