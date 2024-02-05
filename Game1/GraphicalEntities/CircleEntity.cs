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
        public double Radius => GameEntity.Radius;

        public CircleEntity(Game game) : base(game, null)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var radius = Radius;

            if (WorldSpace)
                radius = Radius * _zoom;

            var pos = WorldSpace ? GetWindowSpacePos() : new Vector2((float)Position.x, (float)Position.y);

            spriteBatch.DrawCircle(pos, (float)(radius), 256, Color, ActualLineWidth);

            DrawLabel(spriteBatch);
            //Debug.WriteLine($"radius: {radius}, pos: {GetWindowSpacePos()}");
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            if (GlobalStatic.MainFont == null)
                return;

            var windowPos = WorldSpace ? GetWindowSpacePos() : new Vector2((float)Position.x, (float)Position.y);
            var x = windowPos.X - GetLabelWidth().X / 2;

            var radius = Radius;
            if (WorldSpace)
                radius = Radius * _zoom;

            radius = radius * (1 / ScaleFactor);

            var y = windowPos.Y - radius * (double)ScaleFactor - 20;

            spriteBatch.DrawString(GlobalStatic.MainFont, Label, new Vector2(x, (float)y), Color);
        }

        public override bool CheckClick()
        {
            return false;
        }

        public override Vector2 GetDimensions()
        {
            var radius = Radius;

            if (WorldSpace)
                radius = Radius * _zoom;

            var bbox = new Vector2((float)radius, (float)radius);
            return bbox;
        }
    }
}
