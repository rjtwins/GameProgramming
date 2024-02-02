using Game1;
using Game1.GraphicalEntities;
using Game1.Graphics;
using Game1.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Camera _camera;

        private FlatKeyboard _flatKeyboard => FlatKeyboard.Instance;
        private FlatMouse _flatMouse => FlatMouse.Instance;

        private double timeSinceLastTick = 0;
        private bool firstFrame = true;

        PolyEntity pointer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = true;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            DisplayMode dm = _graphics.GraphicsDevice.DisplayMode;

            GlobalStatic.Width = (int)(dm.Width * 1f);
            GlobalStatic.Height = (int)(dm.Height * 1f);

            _graphics.PreferredBackBufferWidth = GlobalStatic.Width;
            _graphics.PreferredBackBufferHeight = GlobalStatic.Height;
            _graphics.ToggleFullScreen();
            _graphics.ApplyChanges();

            _camera = new Camera();
            this.Services.AddService<Camera>(_camera);
            this.Services.AddService<GraphicsDeviceManager>(_graphics);

            Vector2[] vertices = new Vector2[5];
            vertices[0] = new(100, 0);
            vertices[1] = new(-100, -100);
            vertices[2] = new(-50, -30);
            vertices[3] = new(-50, 30);
            vertices[4] = new(-100, 100);

            var ship = new Ship(this, vertices, (0, 0), new Vector2(0, 0), 0f, Color.Red);

            GameState.WorldEntities.Add(ship);

            var subShp = new SubPoly(this, ship, new Vector2(5f, 5f), vertices, 0f, Color.Red);
            subShp.Scale(ship.ScaleFactor / 2);
            ship.SubEntities.Add(subShp);

            pointer = new CircleEntity(this, (_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), 5, Vector2.Zero, 0f, Color.Red, false);
            pointer.FixLineWidth = false;
            pointer.WorldSpace = true;
            pointer.Label = "+";
            GameState.WorldEntities.Add(pointer);

            //Earth moon system:
            var earth = new Planet(this, (149600000000, 0), 6317000, Vector2.Zero, 0f, Color.Blue);
            earth.Label = "EARTH";
            var moon = new Planet(this, (149600000000, 384400000), 1737000, Vector2.Zero, 0f, Color.Gray);
            moon.Label = "MOON";
            var sun = new Star(this, (0, 0), 696000000, Vector2.Zero, 0f, Color.Yellow);
            sun.Label = "SUN";
            var proxima = new Star(this, (40208000000000000, 0), 696000000, Vector2.Zero, 0f, Color.Yellow);
            proxima.Label = "PROXIMA";
            var solarSystem = new SolarSystem(this, (0, 0), Color.Gray);
            solarSystem.Label = "SOLAR SYSTEM";
            var proximaSystem = new SolarSystem(this, (40208000000000000, 0), Color.Gray);
            proximaSystem.Label = "PROXIMA SYSTEM";

            GameState.WorldEntities.Add(sun);
            GameState.WorldEntities.Add(earth);
            GameState.WorldEntities.Add(moon);
            GameState.WorldEntities.Add(proxima);
            GameState.WorldEntities.Add(proximaSystem);
            GameState.WorldEntities.Add(solarSystem);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalStatic.MainFont = Content.Load<SpriteFont>("Score"); // Use the name of your sprite font file here instead of 'Score'.
        }

        protected override void Update(GameTime gameTime)
        {
            if (firstFrame)
                _camera.Position = (0, 0);

            timeSinceLastTick += gameTime.ElapsedGameTime.TotalSeconds;

            if (FlatMouse.Instance.IsLeftButtonClicked())
                GameState.SelectedEntity = null;

            if (FlatMouse.Instance.IsRightButtonClicked())
            {
                GameState.RightButtonClicked = true;
                GameState.RightMouseClickedWorldLocation = FlatMouse.Instance.WorldPosition(_camera);
            }

            var mousePos = _flatMouse.WorldPosition(_camera);
            pointer.Position = mousePos;

            GameState.CheckClick();

            if(timeSinceLastTick >= 1)
            {
                GameState.Update(timeSinceLastTick);
                timeSinceLastTick = 0;
                GameState.RightButtonClicked = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _flatKeyboard.Update();
            _flatMouse.Update();
            var zoom = _camera.Zoom;
            var zoomChange = 0f;

            //Zoom:
            if (_flatMouse.ScrolledUp())
            {
                if (_flatKeyboard.IsKeyDown(Keys.LeftShift))
                {
                    zoomChange = 1f;
                    _camera.Zoom = Math.Min(_camera.Zoom *= 2f, 1);
                }
                else
                {
                    zoomChange = 0.2f;
                    _camera.Zoom = Math.Min(_camera.Zoom *= 1.2f, 1);
                }
            }

            mousePos = _flatMouse.WorldPosition(_camera);
            pointer.Position = mousePos;

            if (_flatMouse.ScrolledDown())
            {
                if (_flatKeyboard.IsKeyDown(Keys.LeftShift))
                {
                    zoomChange = -0.9f;
                    _camera.Zoom = Math.Min(_camera.Zoom *= 0.4f, 1);
                }
                else
                {
                    zoomChange = -0.2f;
                    _camera.Zoom = Math.Min(_camera.Zoom *= 0.8f, 1);
                }
            }

            if (zoomChange != 0f)
            {
                var mouseX = mousePos.x;
                var mouseY = mousePos.y;

                var camPos = _camera.Position;
                var camX = camPos.x;
                var camY = camPos.y;

                var divx = (mouseX - camX) * zoomChange;
                var divy = (mouseY - camY) * zoomChange;

                var newX = camX + divx;
                var newY = camY + divy;

                //Debug.WriteLine($"mousePos: {mousePos} camPos: {_camera.Position} div: {(divx, divy)}");

                _camera.Position = (newX, newY);
            }

            if (_flatMouse.IsLeftButtonDown())
            {
                var x = (long)(_flatMouse.MouseMovement().X * (1 / _camera.Zoom));
                var y = (long)(_flatMouse.MouseMovement().Y * -1 * (1 / _camera.Zoom));

                _camera.Position = (_camera.Position.x + x, _camera.Position.y + y);                
            }

            float playerRotAmount = MathHelper.Pi * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //if (_flatKeyboard.IsKeyDown(Keys.Left))
            //{
            //    this.ship.Rotate(+playerRotAmount);
            //}

            //if (_flatKeyboard.IsKeyDown(Keys.Right))
            //{
            //    this.ship.Rotate(-playerRotAmount);
            //}

            //if(_flatKeyboard.IsKeyDown(Keys.Space))
            //{
            //    this.ship.ApplyForce(1);
            //}

            //if(_flatKeyboard.IsKeyDown(Keys.A))
            //{
            //    this.ship.ApplyForce(1, MathHelper.ToRadians(-90), true);
            //}

            //if (_flatKeyboard.IsKeyDown(Keys.D))
            //{
            //    this.ship.ApplyForce(1, MathHelper.ToRadians(90), true);
            //}

            base.Update(gameTime);

            firstFrame = false;
        }

        protected override void Draw(GameTime gameTime)
        {
            //Clear window
            GraphicsDevice.Clear(Color.Black);

            //On Window
            _spriteBatch.Begin();

            GameState.WorldEntities.ForEach(e =>
            {
                e.Draw(_spriteBatch);
                e.DrawSubEntities(_spriteBatch);
            });

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
