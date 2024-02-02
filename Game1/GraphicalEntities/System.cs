using Microsoft.Xna.Framework;

namespace Game1.GraphicalEntities
{
    //Should this even be drawn???
    public class SolarSystem : Body
    {
        public SolarSystem(Game game, (double x, double y) position, Color color, bool worldSpace = true) : base(game, position, GlobalStatic.SYSTEMSIZE, Vector2.Zero, 0f, color, worldSpace)
        {

        }
    }
}
