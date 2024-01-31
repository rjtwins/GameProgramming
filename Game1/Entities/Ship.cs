using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Entities
{
    internal class Ship : PolyEntity
    {
        public Ship(Vector2[] vertices, Vector2 position, Vector2 velocity, float angle, Color color) : base(vertices, position, velocity, angle, color)
        {

        }
    }
}
