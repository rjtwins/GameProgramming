using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Runtime.CompilerServices;



namespace Game2
{
    public class Game1 : Game
    {
        private OrthographicCamera _camera;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.MinimumZoom = 1;
            _camera.MaximumZoom = int.MaxValue;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font1"); // Use the name of your sprite font file here instead of 'Score'.

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            const float movementSpeed = 200;
            //_camera.Move(GetMovementDirection() * movementSpeed * gameTime.GetElapsedSeconds());




            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.OemPlus))
            {
                _camera.Zoom = _camera.Zoom * 1.2f;
            }

            if (state.IsKeyDown(Keys.OemMinus))
            {
                _camera.Zoom = Math.Max(_camera.Zoom * 0.8f, _camera.MinimumZoom);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix);

            _spriteBatch.DrawCircle(_camera.Center, 10, 100, Color.Green, 5f * (1 / _camera.Zoom));
            _spriteBatch.DrawString(font, "test123", _camera.Center, Color.Red);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private Vector2 GetMovementDirection()
        {
            var movementDirection = Vector2.Zero;
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Down))
            {
                movementDirection += Vector2.UnitY;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                movementDirection -= Vector2.UnitY;
            }
            if (state.IsKeyDown(Keys.Left))
            {
                movementDirection -= Vector2.UnitX;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                movementDirection += Vector2.UnitX;
            }
            return movementDirection;
        }
    }
}
