using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Entities
{
    internal class SubPoly : PolyEntity
    {
        public SubPoly(PolyEntity parent, Vector2 offset, Vector2[] vertices, float angle, Color color) : base(vertices, parent.Position + offset, parent.Velocity, parent.Angle + angle, color)
        {
            
        }
    }
}
