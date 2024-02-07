using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
namespace Game1.GraphicalEntities
{
    internal class DotEntity : CircleEntity
    {
        public override double Radius => 5f;

        public DotEntity(Game game) : base(game)
        {

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            var radius = Radius;

            if (WorldSpace)
                radius = Radius * _zoom;

            var pos = WorldSpace ? GetWindowPos() : new Vector2((float)Position.x, (float)Position.y);

            spriteBatch.DrawPoint(pos.X, pos.Y, Color.Red, ActualLineWidth);

            DrawLabel(spriteBatch);
            //Debug.WriteLine($"radius: {radius}, pos: {GetWindowSpacePos()}");
        }
    }
}
