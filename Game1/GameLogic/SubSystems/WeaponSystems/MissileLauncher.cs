using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems.WeaponSystems
{
    public class MissileLauncher : WeaponSystem
    {
        public override WeaponType WeaponType => WeaponType.Missile;
    }
}
