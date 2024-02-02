using Flat.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Shapes;

namespace Flat.Entities
{
    public class CircleEntity : PolyEntity
    {
        float _radius { get; set; } = 1f;

        public CircleEntity(Game game, Vector2 position, long radius, Vector2 velocity, float angle, Color color) : base(game, null, position, velocity, angle, color)
        {
            _radius = radius;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(Position, _radius * ScaleFactor, 256, Color, ActualLineWidth);
        }
    }
}
