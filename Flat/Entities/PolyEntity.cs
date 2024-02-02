using Flat;
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
    public class PolyEntity : Entity
    {
        public Vector2[] Vertices {  get; set; }
        private Polygon Polygon { get; set; }

        public PolyEntity(Game game, Vector2[] vertices, Vector2 position, Vector2 velocity, float angle, Color color) : base(game, position, velocity, angle, color)
        {
            Vertices = vertices;
            if (vertices == null)
                return;
            Polygon = new Polygon(Vertices);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var scaleFactor = FixeScreenSize ? (1 / ScaleFactor) : ScaleFactor;

            FlatTransform transform = new FlatTransform(Position, Angle, scaleFactor);
            spriteBatch.DrawPolygon(Position, Polygon, Color);
            //shapes.DrawPolygon(Vertices, transform, 1f, Color);
        }
    }
}
