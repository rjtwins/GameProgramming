using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems.WeaponSystems
{
    public abstract class WeaponSystem : SubSystemBase
    {
        public virtual WeaponType WeaponType { get; set; }
    }
}
