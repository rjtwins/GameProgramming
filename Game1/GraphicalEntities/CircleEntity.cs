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
using Game1.Extensions;

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

            var pos = WorldSpace ? GetWindowPos() : new Vector2((float)Position.x, (float)Position.y);

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

            var windowPos = WorldSpace ? GetWindowPos() : new Vector2((float)Position.x, (float)Position.y);
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

        public override Vector2 GetWindowDim()
        {
            var radius = Radius * 2;

            if (WorldSpace)
                radius = Radius * _zoom;

            var bbox = new Vector2((float)radius * 2, (float)radius * 2);
            return bbox;
        }

        public override Vector2 GetWorldDim()
        {
            return new Vector2((float)Radius * 2, (float)Radius * 2);
        }

        public override RectangleF GetWindowRect()
        {
            var pos = GetWindowPos();
            var dim = GetWindowDim();
            return new RectangleF(pos.X - dim.X / 2, pos.Y - dim.Y / 2, dim.X, dim.Y);
        }

        public override RectangleD GetWorldRect()
        {
            var pos = Position;
            var dim = GetWorldDim();
            return new RectangleD(pos.x - dim.X / 2, pos.y - dim.Y / 2, dim.X, dim.Y);
        }
    }
}
