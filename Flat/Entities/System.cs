using Microsoft.Xna.Framework;

namespace Flat.Entities
{
    //Should this even be drawn???
    public class SolarSystem : Body
    {
        public SolarSystem(Game game, Vector2 position, long radius, Vector2 velocity, float angle, Color color) : base(game, position, radius, velocity, angle, color)
        {

        }
    }
}
