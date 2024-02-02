using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class Star : Body
    {
        public Star(Game game, (long x, long y) position, long radius, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, position, radius, velocity, angle, color, worldSpace)
        {

        }
    }
}
