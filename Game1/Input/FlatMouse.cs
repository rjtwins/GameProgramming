using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Game1.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1.Input
{
    public sealed class FlatMouse
    {
        private static readonly Lazy<FlatMouse> Lazy = new Lazy<FlatMouse>(() => new FlatMouse());

        public static FlatMouse Instance
        {
            get { return Lazy.Value; }
        }

        private MouseState prevMouseState;
        private MouseState currMouseState;

        public Point WindowPosition
        {
            get { return currMouseState.Position; }
        }

        public (double x, double y) WorldPosition(Camera camera)
        {

            double cWorldx = camera.Position.x;
            double cWorldy = camera.Position.y;

            var screenPos = Mouse.GetState().Position;
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

        public FlatMouse()
        {
            prevMouseState = Mouse.GetState();
            currMouseState = prevMouseState;
        }

        public void Update()
        {
            prevMouseState = currMouseState;
            currMouseState = Mouse.GetState();
        }

        public bool IsLeftButtonDown()
        {
            return currMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightButtonDown()
        {
            return currMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMiddleButtonDown()
        {
            return currMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool ScrolledUp()
        {
            return currMouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue;
        }

        public bool ScrolledDown()
        {
            return currMouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue;
        }


        public bool IsLeftButtonClicked()
        {
            return currMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsRightButtonClicked()
        {
            return currMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released;
        }

        public bool IsMiddleButtonClicked()
        {
            return currMouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton == ButtonState.Released;
        }

        public Vector2 MouseMovement()
        {
            var x = (currMouseState.X - prevMouseState.X) * -1;
            var y = currMouseState.Y - prevMouseState.Y;
            return new Vector2(x, y);
        }
    }
}
