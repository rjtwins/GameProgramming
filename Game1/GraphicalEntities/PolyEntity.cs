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
using Microsoft.Xna.Framework.Input;
using Game1.Input;
using System.Net.Http.Headers;
using Game1.Extensions;

namespace Game1.GraphicalEntities
{
    public class PolyEntity : GameGraphicalEntity
    {
        public Vector2[] Vertices { get; set; }
        private Polygon Polygon { get; set; }
        public bool FixLineWidth { get; set; } = true;
        public float LineWidth { get; set; } = 1f;
        protected float ActualLineWidth => LineWidth;
        private Polygon _scaledPolygon { get; set; }

        public PolyEntity(Game game, Vector2[] vertices) : base(game)
        {
            if (vertices == null)
                return;

            Vertices = vertices.Select(x => x * ScaleFactor).ToArray();
            Polygon = new Polygon(Vertices);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _scaledPolygon = Polygon;

            if (WorldSpace)
                _scaledPolygon = Polygon.TransformedCopy(Vector2.Zero, Angle, new Vector2((float)_zoom));

            spriteBatch.DrawPolygon(GetWindowPos(), _scaledPolygon, Color, ActualLineWidth);

            DrawLabel(spriteBatch);
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            if (GlobalStatic.MainFont == null)
                return;

            var windowPos = GetWindowPos();
            var x = windowPos.X;
            var y = windowPos.Y - _scaledPolygon.BoundingRectangle.Y;

            spriteBatch.DrawString(GlobalStatic.MainFont, Label, new Vector2(x, y), Color);
        }

        public override bool CheckClick()
        {
            var mousePos = Mouse.GetState().Position;
            var polygon = _scaledPolygon.TransformedCopy(GetWindowPos(), 0f, Vector2.One);

            if (!polygon.Contains(mousePos.ToVector2()))
                return false;

            Clicked();

            return true;
        }

        public override Vector2 GetWindowDim()
        {
            var scaleVector = WorldSpace ? new Vector2((float)_zoom) : Vector2.One;
            var bbox = Polygon.TransformedCopy(Vector2.Zero, Angle, scaleVector).BoundingRectangle;
            return new Vector2(bbox.Width, bbox.Height);
        }

        public override Vector2 GetWorldDim()
        {
            return new Vector2(Polygon.Right, Polygon.Top);
        }

        public override RectangleF GetWindowRect()
        {
            var pos = GetWindowPos();
            var dim = GetWindowDim();
            return new RectangleF(pos.X, pos.Y, dim.X, dim.Y);
        }

        public override RectangleD GetWorldRect()
        {
            var pos = Position;
            var dim = GetWorldDim();
            return new RectangleD(pos.x, pos.y, dim.X, dim.Y);
        }
    }
}
