using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GraphicalEntities
{
    public class Body : CircleEntity
    {
        public int Mass { get; set; }
        public List<Body> Children { get; set; } = new List<Body>();

        public Body(Game game, (double x, double y) position, double radius, Microsoft.Xna.Framework.Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, position, radius, velocity, angle, color, worldSpace)
        {

        }
    }
}
