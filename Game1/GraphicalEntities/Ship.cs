using Microsoft.Xna.Framework;

namespace Game1.GraphicalEntities
{
    public class Ship : PolyEntity
    {
        public Ship(Game game, Vector2[] vertices, (double x, double y) position, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, vertices, position, velocity, angle, color, worldSpace)
        {

        }
    }
}
