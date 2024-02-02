using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GraphicalEntities
{
    public class TextEntity : Entity
    {
        private string _text;
        private SpriteFont _font;

        public TextEntity(Game game, SpriteFont font, string text, (double x, double y) position, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, position, velocity, angle, color, worldSpace)
        {
            _text = text;
            _font = font;
        }

        public override bool CheckClick()
        {
            return false;
        }

        public override void Clicked()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, _text, new Vector2((float)Position.x, (float)Position.y), Color);
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            return;
        }

        public override bool ShouldDraw()
        {
            return true;
        }
    }
}
