using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;

namespace Game1.GameEntities
{
    public class Moon : Orbital
    {
        public Moon()
        {
            Color = Color.Gray;
            GameState.Moons.Add(this);
        }

        public override GameGraphicalEntity GenerateGraphicalEntity()
        {
            var entity = base.GenerateGraphicalEntity();
            entity.MinSize = 0.1f;
            return entity;
        }

        protected override void Dispose(bool disposing)
        {
            GameState.Moons.Remove(this);
            base.Dispose(disposing);
        }
    }
}
