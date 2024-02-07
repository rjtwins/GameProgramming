using System;
using Game1.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public static class Util
    {
        public static void ToggleFullScreen(GraphicsDeviceManager graphics)
        {
            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();

            DisplayMode dm = graphics.GraphicsDevice.DisplayMode;

            GlobalStatic.Width = (int)(dm.Width * 1f);
            GlobalStatic.Height = (int)(dm.Height * 1f);
            graphics.PreferredBackBufferWidth = GlobalStatic.Width;
            graphics.PreferredBackBufferHeight = GlobalStatic.Height;
            graphics.ApplyChanges();
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"min\" is greater than the value of \"max\".");
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("The value of \"min\" is greater than the value of \"max\".");
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static void Normalize(ref float x, ref float y)
        {
            float invLen = 1f / MathF.Sqrt(x * x + y * y);
            x *= invLen;
            y *= invLen;
        }

        //public static Vector2 TransformSlow(Vector2 position, FlatTransform transform)
        //{
        //    // Scale:
        //    float sx = position.X * transform.ScaleX;
        //    float sy = position.Y * transform.ScaleY;

        //    // Rotation:

        //    /*
        //     * x2 = cosβ x1 − sinβ y1 
        //     * y2 = sinβ x1 + cosβ y1
        //     */

        //    float rx = sx * transform.Cos - sy * transform.Sin;
        //    float ry = sx * transform.Sin + sy * transform.Cos;

        //    // Translation:
        //    float tx = rx + transform.PosX;
        //    float ty = ry + transform.PosY;

        //    return new Vector2(tx, ty);
        //}

        public static float Distance(Vector2 a, Vector2 b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public static float DistanceSquared(Vector2 a, Vector2 b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return dx * dx + dy * dy;
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            // a · b = ax × bx + ay × by
            return a.X * b.X + a.Y * b.Y;
        }

        public static (double x, double y) WorldPosition(Vector2 screenPos)
        {
            var camera = GlobalStatic.Game.Services.GetService<Camera>();

            double cWorldx = camera.Position.x;
            double cWorldy = camera.Position.y;

            double mx = screenPos.X;
            double my = screenPos.Y;
            double divx = mx - GlobalStatic.Width / 2;
            double divy = my - GlobalStatic.Height / 2;

            divx *= (1 / camera.Zoom);
            divy *= (1 / camera.Zoom);

            var worldx = cWorldx + divx;
            var worldy = cWorldy + divy;

            return (worldx, worldy);
        }

        public static Vector2 WindowPosition((double x, double y) worldPos)
        {
            var camera = GlobalStatic.Game.Services.GetService<Camera>();
            var width = GlobalStatic.Width;
            var height = GlobalStatic.Height;

            var x = (camera.Position.x * -1 + worldPos.x) * camera.Zoom + width / 2;
            var y = (camera.Position.y * -1 + worldPos.y) * camera.Zoom + height / 2;

            var pos = new Vector2((float)x, (float)y);

            return pos;
        }

        public static double DotProduct(double[] v1, double[] v2)
        {
            return v1[0] * v2[0] + v1[1] * v2[1];
        }

        public static double Magnitude(double[] v)
        {
            return Math.Sqrt(v[0] * v[0] + v[1] * v[1]);
        }

        public static double AngleBetweenPoints(double[] A, double[] B)
        {
            // Calculate the differences in coordinates
            double deltaX = B[0] - A[0];
            double deltaY = B[1] - A[1];

            // Calculate the angle using arctan
            double angleRad = Math.Atan2(deltaY, deltaX);
            double angleDeg = angleRad * (180.0 / Math.PI);

            //// Ensure the angle is positive
            //if (angleDeg < 0)
            //{
            //    angleDeg += 360.0;
            //}

            return angleRad;
        }
    }
}
