using System;
using Game1.Graphics;
using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.GueDeriving;
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

        public static (decimal x, decimal y) WorldPosition(Vector2 screenPos)
        {
            var camera = GlobalStatic.Game.Services.GetService<Camera>();

            decimal cWorldx = camera.Position.x;
            decimal cWorldy = camera.Position.y;

            decimal mx = (decimal)screenPos.X;
            decimal my = (decimal)screenPos.Y;

            decimal width = GlobalStatic.Width / (decimal)2;
            decimal height = GlobalStatic.Height / (decimal)2;

            decimal divx = mx - width;
            decimal divy = my - height;

            divx *= ((decimal)1 / camera.Zoom);
            divy *= ((decimal)1 / camera.Zoom);

            var worldx = cWorldx + divx;
            var worldy = cWorldy + divy;

            return (worldx, worldy);
        }

        public static Vector2 WindowPosition((decimal x, decimal y) worldPos)
        {
            var camera = GlobalStatic.Game.Services.GetService<Camera>();
            var width = (decimal)GlobalStatic.Width;
            var height = (decimal)GlobalStatic.Height;

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

        public static decimal AngleBetweenPoints(decimal[] A, decimal[] B)
        {
            // Calculate the differences in coordinates
            decimal deltaX = B[0] - A[0];
            decimal deltaY = B[1] - A[1];

            // Calculate the angle using arctan
            double angleRad = Math.Atan2((double)deltaY, (double)deltaX);
            //double angleDeg = angleRad * (180.0 / Math.PI);

            //// Ensure the angle is positive
            //if (angleDeg < 0)
            //{
            //    angleDeg += 360.0;
            //}

            return (decimal)angleRad;
        }

        public static TextRuntime GetTextRuntime(string text, int r, int g, int b, int a = 255)
        {
            var runtime = new TextRuntime();
            runtime.Text = text;
            runtime.WidthUnits = DimensionUnitType.RelativeToChildren;
            runtime.HeightUnits = DimensionUnitType.RelativeToChildren;
            runtime.Width = 10;
            runtime.Height = 5;
            runtime.UseCustomFont = false;
            runtime.Font = "Calibri Light";
            runtime.FontSize = 15;
            runtime.UseFontSmoothing = false;
            runtime.SetProperty("Red", r);
            runtime.SetProperty("Green", g);
            runtime.SetProperty("Blue", b);
            runtime.SetProperty("Alpha", a);
            runtime.HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment.Center;
            runtime.VerticalAlignment = RenderingLibrary.Graphics.VerticalAlignment.Center;

            return runtime;
        }

        private const double G = 6.67430e-11; // Gravitational constant in m^3/kg/s^2

        public static (decimal x, decimal y) CircularOrbit(double massParent, double massChild, double radiusOrbit, double timeElapsed)
        {
            // Calculate angular velocity of the Moon
            double orbitalPeriodMoon = Math.Sqrt((4 * Math.PI * Math.PI * radiusOrbit * radiusOrbit * radiusOrbit) / (G * (massParent + massChild)));
            double angularVelocityMoon = 2 * Math.PI / orbitalPeriodMoon;

            // Determine the angle covered by the Moon since the start of its orbit
            double theta = angularVelocityMoon * timeElapsed;

            // Convert polar coordinates to Cartesian coordinates
            double xCoordinate = radiusOrbit * Math.Cos(theta);
            double yCoordinate = radiusOrbit * Math.Sin(theta);

            return ((decimal)xCoordinate, (decimal)yCoordinate);
        }

        public static (decimal x, decimal y) InclinedOrbit(double massParent, double massChild, double radiusOrbit, double timeElapsed, double inclinationAngle)
        {
            // Calculate angular velocity of the Moon
            double orbitalPeriodMoon = Math.Sqrt((4 * Math.PI * Math.PI * radiusOrbit * radiusOrbit * radiusOrbit) / (G * (massChild + massParent)));
            double angularVelocityMoon = 2 * Math.PI / orbitalPeriodMoon;

            // Determine the angle covered by the Moon since the start of its orbit
            double theta = angularVelocityMoon * timeElapsed;

            // Adjust theta with inclination angle
            theta -= inclinationAngle;

            // Convert polar coordinates to Cartesian coordinates
            double xCoordinate = radiusOrbit * Math.Cos(theta);
            double yCoordinate = radiusOrbit * Math.Sin(theta);

            return ((decimal)xCoordinate, (decimal)yCoordinate);
        }
    }
}
