﻿using Autofac;
using Game1.GameEntities;
using Game1.Graphics;
using Game1.Input;
using Game1.ScreenModels;
using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System;
using System.Linq;

namespace Game1
{
    public static class Util
    {
        public static void ToggleFullScreen(GraphicsDeviceManager graphics)
        {
            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();

            DisplayMode dm = graphics.GraphicsDevice.DisplayMode;

            if (graphics.IsFullScreen)
            {
                GlobalStatic.Width = (int)(dm.Width * 1f);
                GlobalStatic.Height = (int)(dm.Height * 1f);
            }
            else
            {
                GlobalStatic.Width = (int)(dm.Width * 0.8f);
                GlobalStatic.Height = (int)(dm.Height * 0.8f);
            }

            graphics.PreferredBackBufferWidth = GlobalStatic.Width;
            graphics.PreferredBackBufferHeight = GlobalStatic.Height;

            graphics.ApplyChanges();

            Main.Instance.UpdateResolution();
            ShipDesign.Instance.UpdateResolution();
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

        public static float Distance(Vector2 a, Vector2 b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public static decimal Distance((decimal x, decimal y) a, (decimal x, decimal y) b)
        {
            decimal dx = b.x - a.x;
            decimal dy = b.y - a.y;
            return (decimal)Math.Sqrt((double)(dx * dx + dy * dy));
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
            runtime.FontSize = 14;
            runtime.UseFontSmoothing = false;
            runtime.SetProperty("Red", r);
            runtime.SetProperty("Green", g);
            runtime.SetProperty("Blue", b);
            runtime.SetProperty("Alpha", a);
            runtime.HorizontalAlignment = RenderingLibrary.Graphics.HorizontalAlignment.Center;
            runtime.VerticalAlignment = RenderingLibrary.Graphics.VerticalAlignment.Center;

            return runtime;
        }

        public const double G = 6.67430e-11; // Gravitational constant in m^3/kg/s^2
        
        //This works WAY to well i have to say. (only if ships have enough thrust!!
        public static ((decimal x, decimal y) pos, double time) AproxIntercept(Fleet f, Orbital o)
        {
            return FindIntercept(f, o);

            //decimal distance = Distance((f.X, f.Y), (o.X, o.Y));
            //var time = distance / f.GetMaxThrust();

            //var posOGlobal = o.GlobalCoordinatesAtTime(GameState.TotalSeconds + (double)time);
            //distance = Distance((f.X, f.Y), posOGlobal);
            //time = distance / f.GetMaxThrust();

            //return (posOGlobal, (double)time);
        }

        public static ((decimal x, decimal y) pos, double time) FindIntercept(Fleet f, Orbital o)
        {
            decimal distance = Distance((f.X, f.Y), (o.X, o.Y));
            double time = (double)(distance / f.GetMaxThrust());

            var posTime = o.GlobalCoordinatesAtTime(GameState.TotalSeconds + time);
            distance = Distance((f.X, f.Y), posTime);
            time = (double)(distance / f.GetMaxThrust());

            double t0 = 0.0;
            double error = double.MaxValue;
            double t = 0.0;

            for (int i = 0; i < 100; i++) // just avoiding infinite loop in case t/planet_orbit_period>=~0.5
            {
                t0 = t;
                var postx = o.GlobalCoordinatesAtTime(GameState.TotalSeconds + t);
                var distx = Distance((f.X, f.Y), postx);
                t = (double)distx / f.GetMaxThrust();
                //error = Math.Abs(t - t0);

                if (Math.Abs(t - t0) * f.GetMaxThrust() <= 1000) break;
            }

            var loc = o.GlobalCoordinatesAtTime(GameState.TotalSeconds + t);
            return (loc, t);
        }

        //public static ((decimal x, decimal y) pos, double time) FindIntercept(Fleet f, Orbital o, (decimal x, decimal y) loc, double deltaTime, double startTime)
        //{
        //    var minTime = startTime - deltaTime;
        //    var maxTime = startTime + deltaTime;
        //    var timeStep = deltaTime / 25;
        //    var prevDis = Distance((f.X, f.Y), loc);

        //    List<(double time, double error)> times = new();

        //    for (double i = minTime; i <= maxTime; i += timeStep)
        //    {
        //        var actualLoc = o.GlobalCoordinatesAtTime(GameState.TotalSeconds + i);
        //        var actualLocDis = Distance((f.X, f.Y), actualLoc);
        //        var actualLocTime = (double)(actualLocDis / f.GetMaxThrust());

        //        var timeError = //Math.Abs(actualLocTime - i);
        //        times.Add((actualLocTime, timeError));

        //        Debug.WriteLine(((int)timeError).ToString("e1"));

        //    }

        //    times = times.OrderBy(x => x.error).ToList();
        //    var best = times.First();

        //    Debug.WriteLine($"FINAL: {best.error.ToString("e1")}");

        //    if (best.error < 300)
        //        return (o.GlobalCoordinatesAtTime(times.First().time), times.First().time);

        //    return FindIntercept(f, o, o.GlobalCoordinatesAtTime(times.First().time), best.error, best.time);
        //}

        public static GameEntity FindUnderCursor()
        {
            var mousePos = FlatMouse.Instance.WindowPosition.ToVector2();
            return FindUnderPos(mousePos);
        }

        public static GameEntity FindUnderPos(Vector2 pos)
        {
            var camera = GlobalStatic.Game.Services.GetService<Camera>();
            RectangleF rect = new RectangleF(pos.X - 10f, pos.Y - 10f, 20, 20);

            var entity = GameState.GameEntities
                .Where(x => x.GraphicalEntity.IsInView)
                .Where(x =>
                {
                    var winPos = Util.WindowPosition(x.GraphicalEntity.Position);
                    return rect.Contains(new Point2(winPos.X, winPos.Y));
                }).MaxBy(x => x.GraphicalEntity.GetWorldDim().Length());

            return entity;
        }

        public static void Save()
        {

        }
    }
}
