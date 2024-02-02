using Microsoft.Xna.Framework;

namespace Flat.Entities
{
    //Should this even be drawn???
    public class SolarSystem : Body
    {
        public SolarSystem(Game game, (long x, long y) position, long radius, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, position, radius, velocity, angle, color, worldSpace)
        {

        }
    }
}
