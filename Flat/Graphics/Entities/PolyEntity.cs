using Flat;
using Flat.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class PolyEntity : Entity
    {
        public Vector2[] Vertices {  get; set; }

        public PolyEntity(Game game, Vector2[] vertices, Vector2 position, Vector2 velocity, float angle, Color color) : base(game, position, velocity, angle, color)
        {
            Vertices = vertices;
        }

        public override void Draw(Shapes shapes)
        {
            var scaleFactor = FixeScreenSize ? (1 / ScaleFactor) : ScaleFactor;

            FlatTransform transform = new FlatTransform(Position, Angle, scaleFactor);
            shapes.DrawPolygon(Vertices, transform, 1f, Color);
        }

        public virtual void Scale(float amount)
        {
            ScaleFactor = amount;
        }
    }
}
