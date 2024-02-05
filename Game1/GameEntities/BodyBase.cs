using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public abstract class BodyBase
    {
        public Guid Guid { get; set; }
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;

        public double LocalX { get; set; } = 0;
        public double LocalY { get; set; } = 0;

        //Names and stuff;
        public string Name { get; set; }
        public string Description { get; set; }

        //In 1000kg (ton)
        public long Mass { get; set; }

        //kM/s
        public long Velocity { get; set; }

        //Radials against galaxtic plane x axis;
        public double Angle { get; set; }

        public double Radius { get; set; }
    }
}
