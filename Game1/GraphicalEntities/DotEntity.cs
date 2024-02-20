using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
namespace Game1.GraphicalEntities
{
    internal class DotEntity : CircleEntity
    {
        public override decimal Radius => 5M;

        public DotEntity() : base()
        {
            LineWidth = 5f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var radius = Radius;

            if (WorldSpace)
                radius = Radius * _zoom;

            var pos = WorldSpace ? Util.WindowPosition(Position) : new Vector2((float)Position.x, (float)Position.y);

            spriteBatch.DrawPoint(pos.X, pos.Y, Color.Red, ActualLineWidth);

            //DrawLabel(spriteBatch);
            //Debug.WriteLine($"radius: {radius}, pos: {GetWindowSpacePos()}");
        }
    }
}
