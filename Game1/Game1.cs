using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Data.Common;
using Flat.Graphics;
using Flat.Input;
using Flat.Physics;
using System;
using System.Runtime.CompilerServices;
using Game1.Entities;
using System.Diagnostics;
using System.Reflection;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Screen _screen;
        private Sprites _sprites;
        private Shapes _shapes;
        private Camera _camera;
        private FlatKeyboard _flatKeyboard => FlatKeyboard.Instance;
        private FlatMouse _flatMouse => FlatMouse.Instance;

        Texture2D shipSprite1, shipSprite2, shipSprite3;

        Ship ship;
        CircleEntity pointer;

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

            _graphics.PreferredBackBufferWidth = 1920; //(int)(dm.Width * 0.8f);
            _graphics.PreferredBackBufferHeight = 1080; //(int)(dm.Height * 0.8f);
            _graphics.ApplyChanges();

            // TODO: Add your initialization logic here
            this._screen = new Screen(this, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            this._sprites = new Sprites(this);
            this._shapes = new Shapes(this);
            this._camera = new Camera(_screen);
            this._camera.Zoom = 0.5f;
            //this._camera.min

            Vector2[] vertices = new Vector2[5];
            vertices[0] = new(10, 0);
            vertices[1] = new(-10, -10);
            vertices[2] = new(-5, -3);
            vertices[3] = new(-5, 3);
            vertices[4] = new(-10, 10);
            ship = new Ship(vertices, new Vector2(0, 0), new Vector2(0, 0), 0f, Color.Red);
            pointer = new(Vector2.Zero, 5, Vector2.Zero, 0f, Color.Green);
            pointer.FixScreenSize(true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _flatKeyboard.Update();
            _flatMouse.Update();

            var windowPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            var mouseOfset = new Vector2(((_graphics.PreferredBackBufferWidth / 2) - windowPos.X) * -1, ((_graphics.PreferredBackBufferHeight / 2) - windowPos.Y) * 1);

            //Zoom:
            if (_flatMouse.ScrolledUp())
            {
                this._camera.IncZoom();
            }

            if(_flatMouse.ScrolledDown())
            {
                this._camera.DecZoom();
            }

            if(_flatMouse.IsLeftButtonDown())
            {
                this._camera.Move(_flatMouse.MouseMovement() * (1 / _camera.Zoom));
            }

            float playerRotAmount = MathHelper.Pi * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_flatKeyboard.IsKeyDown(Keys.Left))
            {
                this.ship.Rotate(+playerRotAmount);
            }

            if (_flatKeyboard.IsKeyDown(Keys.Right))
            {
                this.ship.Rotate(-playerRotAmount);
            }

            if(_flatKeyboard.IsKeyDown(Keys.Space))
            {
                this.ship.ApplyForce(1);
            }

            if(_flatKeyboard.IsKeyDown(Keys.A))
            {
                this.ship.ApplyForce(1, MathHelper.ToRadians(-90), true);
            }

            if (_flatKeyboard.IsKeyDown(Keys.D))
            {
                this.ship.ApplyForce(1, MathHelper.ToRadians(90), true);
            }

            this.ship.Update(gameTime);
            this.pointer.MoveTo(_camera.Position);

            this.pointer.Scale(_camera.Zoom);
            this.pointer.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _screen.Set();
            GraphicsDevice.Clear(Color.Black);

            _shapes.Begin(_camera);

            this.ship.Draw(_shapes);
            this.pointer.Draw(_shapes);
            //_shapes.DrawCircle(0, 0, 5, 100, 0.1f, Color.Red);
            _shapes.End();

            _screen.UnSet();
            _screen.Present(this._sprites);

            base.Draw(gameTime);
        }
    }
}
