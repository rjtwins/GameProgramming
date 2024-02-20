using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameLogic.SubSystems
{
    internal class Turret : SubSystemBase
    {
        public override SubSystemType SubsystemType => SubSystemType.Turret;

        public double TraverseSpeed { get; set; } = 0;
        public double Precision { get; set; } = 0f;

        public List<SubSystemBase> MountedSystems { get; set; } = new();

        public override object Clone()
        {
            var clone = this.MemberwiseClone() as Turret;
            clone.MountedSystems = this.MountedSystems.Select(x => x.Clone() as SubSystemBase).ToList();

            return clone;
        }

        public override string Report()
        {
            var reportString = base.Report();

            reportString += $"This is [Color=orange]orange[/Color] text.";

            return reportString;
        }
    }
}
