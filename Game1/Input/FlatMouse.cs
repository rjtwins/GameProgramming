using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Timers;
using Game1.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1.Input
{
    public sealed class FlatMouse
    {
        public static FlatMouse Instance { get; private set; }


        private Game _game;
        private Camera _camera;

        private MouseState prevMouseState;
        private MouseState currMouseState;

        private int rightClickCount;
        private int leftClickCount;

        private Timer rightClickTimer = new Timer();
        private Timer leftClickTimer = new Timer();

        public Point WindowPosition
        {
            get { return currMouseState.Position; }
        }

        public Vector2 GumPos => Util.WindowPosToGumPos(WindowPosition.ToVector2());

        //public (double x, double y) WorldPosition()
        //{

        //    double cWorldx = _camera.Position.x;
        //    double cWorldy = _camera.Position.y;

        //    var screenPos = Mouse.GetState().Position;
        //    double mx = screenPos.X;
        //    double my = screenPos.Y;
        //    double divx = mx - GlobalStatic.Width / 2;
        //    double divy = my - GlobalStatic.Height / 2;

        //    divx *= (1 / _camera.Zoom);
        //    divy *= (1 / _camera.Zoom);

        //    var worldx = cWorldx + divx;
        //    var worldy = cWorldy + divy;

        //    return (worldx, worldy);
        //}

        public static void Init(Game game)
        {
            Instance = new FlatMouse(game);
        }

        private FlatMouse(Game game)
        {
            _game = game;
            prevMouseState = Mouse.GetState();
            currMouseState = prevMouseState;
            _camera = _game.Services.GetService<Camera>();
            leftClickTimer.Interval = 500;
            leftClickTimer.AutoReset = false;
            leftClickTimer.Elapsed += LeftClickTimer_Elapsed;

            rightClickTimer.Interval = 500;
            rightClickTimer.AutoReset = false;
            rightClickTimer.Elapsed += RightClickTimer_Elapsed;

        }

        private void LeftClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            leftClickCount = 0;
        }

        private void RightClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            rightClickCount = 0;
        }


        public void Update()
        {
            if (currMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                leftClickCount++;
                leftClickTimer.Start();
            }

            if (currMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
            {
                rightClickCount++;
                rightClickTimer.Start();
            }


            prevMouseState = currMouseState;
            currMouseState = Mouse.GetState();
        }

        public bool IsLeftButtonDown()
        {
            if (!IsActive()) return false;

            return currMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsRightButtonDown()
        {
            if (!IsActive()) return false;

            return currMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMiddleButtonDown()
        {
            if (!IsActive()) return false;

            return currMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool ScrolledUp()
        {
            if (!IsActive()) return false;

            return currMouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue;
        }

        public bool ScrolledDown()
        {
            if (!IsActive()) return false;

            return currMouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue;
        }


        public bool IsLeftButtonClicked()
        {
            if (!IsActive()) return false;

            return currMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released;
        }

        public bool IsRightButtonClicked()
        {
            if (!IsActive()) return false;

            return currMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released;
        }

        public bool IsMiddleButtonClicked()
        {
            if (!IsActive()) return false;

            return currMouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton == ButtonState.Released;
        }

        public bool IsLeftButtonDoubleCLicked()
        {
            if (!IsActive()) return false;

            if (leftClickCount >= 2)
            {
                return true;
            }
            return false;
        }

        public bool IsRightButtonDoubleCLicked()
        {
            if (!IsActive()) return false;

            if (rightClickCount >= 2)
            {
                return true;
            }
            return false;
        }

        public void ResetLeftDoubleClick()
        {
            leftClickCount = 0;
        }

        public void ResetRightDoubleClick()
        {
            rightClickCount = 0;
        }

        public Vector2 MouseMovement()
        {
            if (!IsActive()) return Vector2.Zero;

            var x = (currMouseState.X - prevMouseState.X) * -1;
            var y = currMouseState.Y - prevMouseState.Y;
            return new Vector2(x, y);
        }

        private bool IsActive()
        {
            if (!_game.GraphicsDevice.Viewport.Bounds.Contains(WindowPosition))
            {
                return false;
            }

            if (!_game.IsActive)
            {
                return false;
            }

            return true;
        }
    }
}
