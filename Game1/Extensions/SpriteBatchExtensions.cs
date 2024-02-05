using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Game1.Extensions
{
    public static class SpriteBatchExtensions
    {
        public static void DrawString(this SpriteBatch spriteBatch, GameGraphicalEntity entity, SpriteFont font, string text, Vector2 offset, Color color)
        {
            var pos = entity.GetWindowSpacePos();
            pos += offset;
            spriteBatch.DrawString(font, text, pos, color);
        }
    }
}
