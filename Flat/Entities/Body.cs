using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class Body : CircleEntity
    {
        public int Mass { get; set; }
        public List<Body> Children { get; set; } = new List<Body>();

        public float Radius = 0f;

        public Body(Game game, Microsoft.Xna.Framework.Vector2 position, long radius, Microsoft.Xna.Framework.Vector2 velocity, float angle, Color color) : base(game, position, radius, velocity, angle, color)
        {

        }
    }
}
