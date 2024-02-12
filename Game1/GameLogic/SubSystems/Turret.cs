using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    internal class Turret : SubSystemBase
    {
        public double TraverseSpeed { get; set; } = 0;
        public double Precision { get; set; } = 0f;

        public List<SubSystemBase> MountedSystems { get; set; }
    }
}
