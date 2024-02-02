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

namespace Game1.GraphicalEntities
{
    public class CircleEntity : PolyEntity
    {
        double _radius { get; set; } = 1f;

        public CircleEntity(Game game, (double x, double y) position, double radius, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, null, position, velocity, angle, color, worldSpace)
        {
            _radius = radius;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var radius = _radius;
            if (WorldSpace)
                radius = _radius * _zoom;

            spriteBatch.DrawCircle(GetWindowSpacePos(), (float)(radius * ScaleFactor), 256, Color, ActualLineWidth);

            //Debug.WriteLine($"radius: {radius}, pos: {GetWindowSpacePos()}");
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            if (GlobalStatic.MainFont == null)
                return;

            var windowPos = GetWindowSpacePos();
            var x = windowPos.X - GetLabelWidth().X / 2;

            var radius = _radius;
            if (WorldSpace)
                radius = _radius * _zoom;

            var y = windowPos.Y - radius * (double)ScaleFactor - 20;

            spriteBatch.DrawString(GlobalStatic.MainFont, Label, new Vector2(x, (float)y), Color);
        }

        public override bool CheckClick()
        {
            return false;
        }
    }
}
