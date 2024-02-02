using Flat.Entities;
using Flat.Graphics;
using Flat.Input;
using Game1.Extensions;
using GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //private Screen _screen;
        private Sprites _sprites;
        //private Shapes _shapes;
        //private Camera _camera;
        private OrthographicCamera _camera;
        BoxingViewportAdapter _viewportAdapter;
        private FlatKeyboard _flatKeyboard => FlatKeyboard.Instance;
        private FlatMouse _flatMouse => FlatMouse.Instance;
        private SpriteFont font;

        private double timeSinceLastTick = 0;
        private bool firstFrame = true;

        private List<Entity> _entities = new();

        private double _baseZ = 1;

        Entity pointer;

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

            _graphics.PreferredBackBufferWidth = (int)(dm.Width * 0.8f);
            _graphics.PreferredBackBufferHeight = (int)(dm.Height * 0.8f);
            _graphics.ApplyChanges();


            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            
            _baseZ = _viewportAdapter.Viewport.Height;

            _camera = new OrthographicCamera(_viewportAdapter);

            // TODO: Add your initialization logic here
            //this._screen = new Screen(this, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            //this._sprites = new Sprites(this);
            //this._shapes = new Shapes(this);
            //this._camera = new Camera(_screen);
            //this._camera.Zoom = 1f;

            this.Services.AddService<OrthographicCamera>(_camera);
            //this.Services.AddService<Screen>(_screen);
            //this.Services.AddService<Shapes>(_shapes);
            this.Services.AddService<GraphicsDeviceManager>(_graphics);
            //this._camera.min

            Vector2[] vertices = new Vector2[5];
            vertices[0] = new(10, 0);
            vertices[1] = new(-10, -10);
            vertices[2] = new(-5, -3);
            vertices[3] = new(-5, 3);
            vertices[4] = new(-10, 10);

            var ship = new Ship(this, vertices, new Vector2(0, 0), new Vector2(0, 0), 0f, Color.Red);

            this._entities.Add(ship);

            var subShp = new SubPoly(this, ship, new Vector2(5f, 5f), vertices, 0f, Color.Red);
            subShp.ScaleFactor = ship.ScaleFactor / 2;
            ship.SubEntities.Add(subShp);

            pointer =  new PolyEntity(this, vertices, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), new Vector2(0, 0), 0f, Color.Red);

            //pointer.FixScreenSize(true);

            //Earth moon system:
            var earth = new Planet(this, Vector2.Zero, 6317000, Vector2.Zero, 0f, Color.Blue);
            var moon = new Planet(this, new Vector2(0, 384400000), 1737000, Vector2.Zero, 0f, Color.Gray);

            _entities.Add(earth);
            _entities.Add(moon);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Score"); // Use the name of your sprite font file here instead of 'Score'.
        }

        protected override void Update(GameTime gameTime)
        {
            if (firstFrame)
                _camera.LookAt(new Vector2(0, 0));

            timeSinceLastTick += gameTime.ElapsedGameTime.TotalSeconds;

            if(timeSinceLastTick >= 1)
            {
                GameState.Update(timeSinceLastTick);
                timeSinceLastTick = 0;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _flatKeyboard.Update();
            _flatMouse.Update();

            var windowPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            var mouseOfset = new Vector2(((_graphics.PreferredBackBufferWidth / 2) - windowPos.X) * -1, ((_graphics.PreferredBackBufferHeight / 2) - windowPos.Y) * 1);

            //Zoom:
            if (_flatMouse.ScrolledUp())
            {
                this._camera.Zoom *= 1.2f;
                Debug.WriteLine(this._camera.Zoom);
            }

            if(_flatMouse.ScrolledDown())
            {
                this._camera.Zoom *= 0.8f;
                Debug.WriteLine(this._camera.Zoom);
            }

            if(_flatMouse.IsLeftButtonDown())
            {
                this._camera.Move(new Vector2(_flatMouse.MouseMovement().X, _flatMouse.MouseMovement().Y * -1) * (1 / _camera.Zoom));
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

            ////Move screen
            //_screen.Set();

            ////Draw shapes on screen:
            //_shapes.Begin(_camera);

            //On world:
            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            //_spriteBatch.DrawRectangle(new RectangleF(250, 250, 50, 50), Color.Black, 1f);
            _entities.ForEach(e =>
            {
                e.Draw(_spriteBatch);
                e.DrawSubEntities(_spriteBatch);
            });

            _spriteBatch.End();

            //On Window
            _spriteBatch.Begin();
            this.pointer.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
