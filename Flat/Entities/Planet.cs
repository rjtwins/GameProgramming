using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class Planet : Body
    {
        public Planet(Game game, Vector2 position, long radius, Vector2 velocity, float angle, Color color) : base(game, position, radius, velocity, angle, color)
        {

        }
    }
}
