using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public abstract class SubSystemBase
    {
        public string SubsystemType { get; set; }
        public string SubsystemName { get; set; }
        public long Mass { get; set; }
    }
}
