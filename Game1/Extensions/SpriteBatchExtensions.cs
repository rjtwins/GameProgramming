using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Game1.Extensions
{
    public static class SpriteBatchExtensions
    {
        public static void DrawDashedCircle(this SpriteBatch spriteBatch, Vector2 center, double radius, int sides, Color color, float thickness = 1f)
        {
            List<Vector2> points = new();

            double num = Math.PI * 2.0 / (double)sides;
            double num2 = 0.0;
            double num3 = num;
            for (int i = 0; i < sides; i++)
            {
                var vec1 = new Vector2((float)(radius * Math.Cos(num2)), (float)(radius * Math.Sin(num2))) + center;
                num2 += num;

                points.Add(vec1);
            }

            for (int i = 1; i < sides; i+=2)
            {
                spriteBatch.DrawLine(points[i], points[i-1], color, thickness);
            }
        }


        public static void DrawDottedCircle(this SpriteBatch spriteBatch, Vector2 center, double radius, int nr, Color color, float size = 1f)
        {
            double num = Math.PI * 2.0 / (double)nr;
            double num2 = 0.0;
            for (int i = 0; i < nr; i++)
            {
                var pos = new Vector2((float)(radius * Math.Cos(num2)), (float)(radius * Math.Sin(num2))) + center;
                spriteBatch.DrawPoint(pos, color, size);
                num2 += num;
            }
        }
    }
}
