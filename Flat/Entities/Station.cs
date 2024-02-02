using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class Station : PolyEntity
    {
        public Station(Game game, Vector2[] vertices, (long x, long y) position, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, vertices, position, velocity, angle, color, worldSpace)
        {
        }
    }
}
