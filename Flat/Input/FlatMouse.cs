﻿using System;
using System.Net.Http.Headers;
using Flat.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flat.Input
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
            get { return this.currMouseState.Position; }
        }

        public FlatMouse()
        {
            this.prevMouseState = Mouse.GetState();
            this.currMouseState = this.prevMouseState;
        }

        public void Update()
        {
            this.prevMouseState = this.currMouseState;
            this.currMouseState = Mouse.GetState();
        }

        public bool IsLeftButtonDown()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightButtonDown()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMiddleButtonDown()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool ScrolledUp()
        {
            return this.currMouseState.ScrollWheelValue > this.prevMouseState.ScrollWheelValue;
        }

        public bool ScrolledDown() 
        {
            return this.currMouseState.ScrollWheelValue < this.prevMouseState.ScrollWheelValue;
        }


        public bool IsLeftButtonClicked()
        {
            return this.currMouseState.LeftButton == ButtonState.Pressed && this.prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsRightButtonClicked()
        {
            return this.currMouseState.RightButton == ButtonState.Pressed && this.prevMouseState.RightButton == ButtonState.Released;
        }

        public bool IsMiddleButtonClicked()
        {
            return this.currMouseState.MiddleButton == ButtonState.Pressed && this.prevMouseState.MiddleButton == ButtonState.Released;
        }

        public Vector2 MouseMovement()
        {
            var x = (currMouseState.X - prevMouseState.X) * -1;
            var y = currMouseState.Y - prevMouseState.Y;
            return new Vector2(x, y);
        }
    }
}
