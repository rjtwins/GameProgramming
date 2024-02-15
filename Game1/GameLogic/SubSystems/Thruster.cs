using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    public class Thruster : Engine
    {
        public override SubSystemType SubsystemType => SubSystemType.Engine;
    }
}
