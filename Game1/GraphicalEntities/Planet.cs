using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GraphicalEntities
{
    public class Planet : Body
    {
        public Planet(Game game, (double x, double y) position, double radius, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, position, radius, velocity, angle, color, worldSpace)
        {

        }
    }
}
