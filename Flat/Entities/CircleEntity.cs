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
using System.Diagnostics;

namespace Flat.Entities
{
    public class CircleEntity : PolyEntity
    {
        float _radius { get; set; } = 1f;

        public CircleEntity(Game game, (long x, long y) position, long radius, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, null, position, velocity, angle, color, worldSpace)
        {
            _radius = radius;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var radius = _radius;
            if (WorldSpace)
                radius = _radius * _zoom;

            spriteBatch.DrawCircle(GetWindowSpacePos(), radius * ScaleFactor, 256, Color, ActualLineWidth);

            //Debug.WriteLine($"radius: {radius}, pos: {GetWindowSpacePos()}");
        }

        public Vector2[] GetWindowBoundingBox()
        {
            var center = GetWindowSpacePos();
            var x1 = center.X - _radius * _zoom;
            var x2 = center.X + _radius * _zoom;
            var y1 = center.Y + _radius * _zoom;
            var y2 = center.Y - _radius * _zoom;
            var width = x2 - x1;
            var height = y2 - y1;

            return (new Vector2[3] { new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(width, height) });
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            if (GlobalStatic.MainFont == null)
                return;

            var windowPos = GetWindowSpacePos();
            var x = windowPos.X;
            
            var radius = _radius;
            if (WorldSpace)
                radius = _radius * _zoom;

            var y = windowPos.Y - radius * ScaleFactor - 20;

            spriteBatch.DrawString(GlobalStatic.MainFont, Label, new Vector2(x, y), Color);
        }
    }
}
