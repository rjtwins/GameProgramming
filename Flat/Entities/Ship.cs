using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class Ship : PolyEntity
    {
        public Ship(Game game, Vector2[] vertices, Vector2 position, Vector2 velocity, float angle, Color color) : base(game, vertices, position, velocity, angle, color)
        {

        }
    }
}
