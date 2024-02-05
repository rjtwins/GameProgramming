using Game1.GraphicalEntities;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public abstract class BodyBase
    {
        public GameGraphicalEntity GraphicalEntity { get; set; }
        public BodyBase Parent { get; set; }
        public Guid Guid { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public double LocalX { get; set; } = 0;
        public double LocalY { get; set; } = 0;

        //Names and stuff
        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }

        //In 1000kg (ton)
        public long Mass { get; set; }

        //kM/s
        public long Velocity { get; set; }

        //Radials against galaxtic plane x axis;
        public float Angle { get; set; }

        public double Radius { get; set; } = 0d;

        public abstract GameGraphicalEntity GenerateGraphicalEntity(Game game);
    }
}
