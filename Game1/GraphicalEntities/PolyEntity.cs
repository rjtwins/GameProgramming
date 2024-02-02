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

namespace Game1.GraphicalEntities
{
    public class PolyEntity : Entity
    {
        public Vector2[] Vertices { get; set; }
        private Polygon Polygon { get; set; }
        public bool FixLineWidth { get; set; } = true;
        public float LineWidth { get; set; } = 2f;
        protected float ActualLineWidth => LineWidth;
        private Polygon _scaledPolygon { get; set; }

        public PolyEntity(Game game, Vector2[] vertices, (double x, double y) position, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, position, velocity, angle, color, worldSpace)
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

            spriteBatch.DrawPolygon(GetWindowSpacePos(), _scaledPolygon, Color, ActualLineWidth);
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            if (GlobalStatic.MainFont == null)
                return;

            var windowPos = GetWindowSpacePos();
            var x = windowPos.X;
            var y = windowPos.Y - _scaledPolygon.BoundingRectangle.Y;

            spriteBatch.DrawString(GlobalStatic.MainFont, Label, new Vector2(x, y), Color);
        }

        public override bool ShouldDraw()
        {
            return true;
        }

        public override bool CheckClick()
        {
            var mousePos = Mouse.GetState().Position;
            var polygon = _scaledPolygon.TransformedCopy(GetWindowSpacePos(), 0f, Vector2.One);

            if (!polygon.Contains(mousePos.ToVector2()))
                return false;

            Clicked();

            return true;
        }
    }
}
