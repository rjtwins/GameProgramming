using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class SubPoly : PolyEntity
    {
        public SubPoly(Game game, PolyEntity parent, Vector2 offset, Vector2[] vertices, float angle, Color color) : base(game, vertices, parent.Position + offset, parent.Velocity, parent.Angle + angle, color)
        {
            
        }
    }
}
