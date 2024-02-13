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
using Game1.GameEntities;
using System.ComponentModel;

namespace Game1.GraphicalEntities
{
    public class CircleEntity : PolyEntity
    {
        public virtual decimal Radius => GameEntity.Radius;

        public CircleEntity() : base(null)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var radius = Radius;

            if (WorldSpace && !FixedSize)
                radius = Radius * _zoom;

            var pos = WorldSpace ? Util.WindowPosition(Position) : new Vector2((float)Position.x, (float)Position.y);

            spriteBatch.DrawCircle(pos, (float)(radius), 256, Color, ActualLineWidth);

            //DrawLabel(spriteBatch);
        }

        public void DrawOrbit(SpriteBatch spriteBatch)
        {
            if (!(GameEntity is Orbital orbital))
                return;

            if (orbital.OrbitalPoints == null)
                return;

            var points = orbital.OrbitalPoints;
            var windowPoints = points.ToList().Select(x =>
            {
                decimal posx = ((decimal)x.X + orbital.Parent.X);
                decimal posy = ((decimal)x.Y + orbital.Parent.Y);

                var windowPos = Util.WindowPosition((posx, posy));

                return windowPos;
            }).ToArray();

            spriteBatch.DrawPolygon(Vector2.Zero, windowPoints, Color * 0.5f, 1f);
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            if (GlobalStatic.MainFont == null)
                return;

            if (!ShouldDrawLabel)
                return;

            var windowPos = WorldSpace ? Util.WindowPosition(Position) : new Vector2((float)Position.x, (float)Position.y);
            var x = windowPos.X - GetLabelWidth().X / 2;

            var radius = Radius;
            if (WorldSpace && !FixedSize)
                radius = Radius * _zoom;

            var y = (decimal)windowPos.Y - radius - 20M;

            if(LabelRuntime == null)
            {
                LabelRuntime = Util.GetTextRuntime(Label, Color.R, Color.G, Color.B);
                LabelRuntime.AddToManagers();
            }

            LabelRuntime.X = x;
            LabelRuntime.Y = (float)y;

            LabelRuntime.Visible = true;
            
            //spriteBatch.DrawString(GlobalStatic.MainFont, Label, new Vector2(x, (float)y), Color);
        }

        public override Vector2 GetWindowDim()
        {
            var radius = Radius;

            if (WorldSpace && !FixedSize)
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
            var pos = Util.WindowPosition(Position);
            var dim = GetWindowDim();
            return new RectangleF(pos.X - dim.X / 2, pos.Y - dim.Y / 2, dim.X, dim.Y);
        }

        public override RectangleM GetWorldRect()
        {
            var pos = Position;
            var dim = GetWorldDim();
            return new RectangleM(pos.x - (decimal)dim.X / 2, pos.y - (decimal)dim.Y / 2, (decimal)dim.X, (decimal)dim.Y);
        }
    }
}
