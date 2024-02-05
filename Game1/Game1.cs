using Game1;
using Game1.GameEntities;
using Game1.GraphicalEntities;
using Game1.Graphics;
using Game1.Input;
using Gum.DataTypes;
using Gum.Managers;
using GumRuntime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using MonoGame.Extended.ViewportAdapters;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using Camera = Game1.Graphics.Camera;

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

        private bool _selecting = false;
        
        private Vector2 _selectionStart = new Vector2();
        private Vector2 _selectionEnd = new Vector2();
        private Vector2 _pointerVector = new Vector2();
        Vector2[] _cross1, _cross2;

        SmoothFramerate framerate = new(20);


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

            GlobalStatic.Width = (int)(dm.Width * 0.9f);
            GlobalStatic.Height = (int)(dm.Height * 0.9f);

            _graphics.PreferredBackBufferWidth = GlobalStatic.Width;
            _graphics.PreferredBackBufferHeight = GlobalStatic.Height;
            //_graphics.ToggleFullScreen();
            _graphics.ApplyChanges();

            _camera = new Camera();
            this.Services.AddService<Camera>(_camera);
            this.Services.AddService<GraphicsDeviceManager>(_graphics);

            _cross1 = new Vector2[2];
            _cross1[0] = new(10, 0);
            _cross1[1] = new(-10, 0);

            _cross2 = new Vector2[2];
            _cross2[0] = new(0, 10);
            _cross2[1] = new(0, -10);

            //var subShp = new SubPoly(this, ship, new Vector2(5f, 5f), vertices, 0f, Color.Red);
            //subShp.Scale(ship.ScaleFactor / 2);
            //ship.SubEntities.Add(subShp);

            _pointerVector = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            //Earth moon system:
            var earth = new Planet()
            {
                Radius = 6317000,
                X = 149600000000,
                Y = 0,
                Name = "EARTH"
            };

            //var moon = new Moon()
            //{
            //    Radius = 6317000,
            //    X = 149600000000,
            //    Y = 384400000,
            //    Name = "MOON"
            //};

            var sun = new Star()
            {
                Radius = 696000000,
                X = 0,
                Y = 0,
                Name = "SUN"
            };


            var solarSystem = new SolarSystem()
            {
                Radius = GlobalStatic.SYSTEMSIZE,
                X = 0,
                Y = 0,
                Name = "SOLAR SYSTEM"
            };

            var proxima = new Star()
            {
                Radius = 696000000,
                X = 40208000000000000,
                Y = 696000000,
                Name = "PROXIMA"
            };

            var proximaSystem = new SolarSystem()
            {
                Radius = GlobalStatic.SYSTEMSIZE,
                X = 40208000000000000,
                Y = 0,
                Name = "PROXIMA SYSTEM"
            };

            GameState.GameEntities.AddRange(new List<GameEntity>() { earth, sun, solarSystem, proxima });

            //GameState.MiscGraphicalEntities.Add(sun);
            //GameState.MiscGraphicalEntities.Add(earth);
            //GameState.MiscGraphicalEntities.Add(moon);
            //GameState.MiscGraphicalEntities.Add(proxima);
            //GameState.MiscGraphicalEntities.Add(proximaSystem);
            //GameState.MiscGraphicalEntities.Add(solarSystem);

            var generator = new SpaceGenerator();
            var systems = generator.Generate(100);

            systems.ForEach(s =>
            {
                s.Planets.ForEach(p =>
                {
                    GameState.GameEntities.Add(p);
                });

                s.Stars.ForEach(st =>
                {
                    GameState.GameEntities.Add(st);
                });

                GameState.GameEntities.Add(s);
            });

            InitGum();

            this.IsMouseVisible = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalStatic.MainFont = Content.Load<SpriteFont>("Score"); // Use the name of your sprite font file here instead of 'Score'.
            GameState.GraphicalEntities.AddRange(GameState.GameEntities.Select(x => x.GenerateGraphicalEntity(this)));
        }

        protected override void Update(GameTime gameTime)
        {
            if (firstFrame)
                _camera.Position = (0, 0);

            UpdateGum(gameTime);

            ProcessSelecting();

            timeSinceLastTick += gameTime.ElapsedGameTime.TotalSeconds;

            if (FlatMouse.Instance.IsLeftButtonClicked())
                GameState.SelectedEntity = null;

            if (FlatMouse.Instance.IsRightButtonClicked())
            {
                GameState.RightButtonClicked = true;
                GameState.RightMouseClickedWorldLocation = FlatMouse.Instance.WorldPosition(_camera);
            }

            var mousePos = _flatMouse.WorldPosition(_camera);
            _pointerVector = Mouse.GetState().Position.ToVector2();

            GameState.CheckClick();

            if (timeSinceLastTick >= 1)
            {
                GameState.Update(timeSinceLastTick);
                timeSinceLastTick = 0;
                GameState.RightButtonClicked = false;
                Debug.WriteLine(framerate.framerate);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _flatKeyboard.Update();
            _flatMouse.Update();

            if (FlatMouse.Instance.IsLeftButtonClicked())
                GameState.SelectedEntities.Clear();

            ProcessZoom();

            ProcessPan();

            ProcessSelecting();

            /*
            //float playerRotAmount = MathHelper.Pi * (float)gameTime.ElapsedGameTime.TotalSeconds;

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
            */

            base.Update(gameTime);

            firstFrame = false;

            framerate.Update(gameTime.GetElapsedSeconds());
        }

        protected override void Draw(GameTime gameTime)
        {
            //Clear window
            GraphicsDevice.Clear(Color.Black);

            //On Window
            _spriteBatch.Begin();

            //GameState.GameEntities.ForEach(x =>
            //{
            //    if (x.GraphicalEntity == null)
            //        return;

            //    if(x.GraphicalEntity.ShouldDraw() == false)
            //    {
            //        _spriteBatch.DrawPoint(x.GraphicalEntity.GetWindowSpacePos(), x.GraphicalEntity.Color, 2f);
            //        return;
            //    }

            //    x.GraphicalEntity.Draw(_spriteBatch);
            //    x.GraphicalEntity.DrawSubEntities(_spriteBatch);
            //});

            GameState.GraphicalEntities.ForEach(e =>
            {
                if (!e.InView())
                {
                    e.IsDrawn = false;
                    return;
                }

                e.IsDrawn = true;

                if (!e.ShouldDraw())
                {
                    _spriteBatch.DrawPoint(e.GetWindowSpacePos(), e.Color, 2f);
                }

                e.Draw(_spriteBatch);
                e.DrawSubEntities(_spriteBatch);

                if (GameState.SelectedEntities.Contains(e.GameEntity))
                {
                    var windowSpace = e.GetWindowSpacePos();
                    var dimensions = e.GetDimensions();
                    _spriteBatch.DrawRectangle(windowSpace.X - dimensions.X - 5, windowSpace.Y - dimensions.X - 5, dimensions.X * 2 + 10, dimensions.Y * 2 + 10, Color.Cyan);
                }
            });


            //UI stuff:
            _spriteBatch.DrawPolygon(Vector2.Zero, _cross1, Color.Red);
            _spriteBatch.DrawPolygon(Vector2.Zero, _cross2, Color.Red);
            _spriteBatch.DrawCircle(_pointerVector, 5f, 255, Color.Red);

            DrawSelecting();

            _spriteBatch.End();

            DrawGum();

            base.Draw(gameTime);
        }

        private void InitGum()
        {
            SystemManagers.Default = new SystemManagers(); 
            SystemManagers.Default.Initialize(_graphics.GraphicsDevice, fullInstantiation: true);

            var gumProject = GumProjectSave.Load("gum\\ui.gumx", out _);
            ObjectFinder.Self.GumProjectSave = gumProject;
            gumProject.Initialize();

            //var textInstance = new TextRuntime();
            //textInstance.UseCustomFont = false;
            //textInstance.Text = "Hello world";
            //textInstance.Font = "Calibri";
            //textInstance.FontSize = 14;
            //textInstance.UseFontSmoothing = false;
            //textInstance.OutlineThickness = 0;
            //textInstance.IsBold = false;
            //textInstance.AddToManagers();

            // This assumes that your project has at least 1 screen
            var screen1 = gumProject.Screens.First().ToGraphicalUiElement(SystemManagers.Default, addToManagers: true);

            var button1 = screen1.GetGraphicalUiElementByName("ButtonInstance1");
            var button1Text = button1.GetGraphicalUiElementByName("TextInstance");

            //var rectangle = new ColoredRectangleRuntime();
            //rectangle.Width = 100;
            //rectangle.Height = 100;
            //rectangle.Color = Color.White;
            //rectangle.AddToManagers(SystemManagers.Default, null);
        }

        private void UpdateGum(GameTime gameTime)
        {
            SystemManagers.Default.Activity(gameTime.TotalGameTime.TotalSeconds);
        }

        private void DrawGum()
        {
            SystemManagers.Default.Draw();
        }

        private void ProcessSelecting()
        {
            // selectionBox is Rectangle

            if (!_flatMouse.IsLeftButtonDown() && _selectionEnd != _selectionStart)
            {
                ProcessSelectionZoom();
                ProcessSelectingEntities();
                _selectionEnd = _flatMouse.WindowPosition.ToVector2();
                _selectionStart = _flatMouse.WindowPosition.ToVector2();
            }

            if (_flatMouse.IsLeftButtonDown())
            {
                if (_selecting)
                {
                    _selectionEnd = _flatMouse.WindowPosition.ToVector2();
                }
                else
                {
                    _selecting = true;
                    _selectionStart = _flatMouse.WindowPosition.ToVector2();
                }
            }
            else
            {
                _selecting = false;
                _selectionEnd = _flatMouse.WindowPosition.ToVector2();
                _selectionStart = _flatMouse.WindowPosition.ToVector2();
            }
        }

        private void DrawSelecting()
        {
            if (_selecting)
            {
                Rectangle selectionRect = GetSelectionRectangle();
                _spriteBatch.DrawRectangle(selectionRect, Color.White);
            }
        }

        private void ProcessZoom()
        {
            var mousePos = _flatMouse.WorldPosition(_camera);
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
        }

        private void ProcessSelectionZoom()
        {
            if (!_flatKeyboard.IsKeyDown(Keys.LeftShift))
                return;


            Rectangle selectionRect = GetSelectionRectangle();

            var newZoom = GlobalStatic.Height / selectionRect.Height * _camera.Zoom;


            var worldPos = Util.WorldPosition(_camera, selectionRect.Center.ToVector2());

            _camera.Zoom = newZoom;
            _camera.Position = (worldPos.x, worldPos.y);


            //if (widthDiv < heightDiv)
            //{
            //    zoomPerc = ((float)(GlobalStatic.Height - heightDiv)) / (float)GlobalStatic.Height;
            //}
            //else
            //{
            //    zoomPerc = ((float)(GlobalStatic.Width - widthDiv)) / (float)GlobalStatic.Width;
            //}


        }

        private void ProcessSelectingEntities()
        {
            var selectionRectangle = GetSelectionRectangle();

            var isDrawn = GameState.GraphicalEntities
                .Where(x => x.IsDrawn)
                .ToList();

            var inSelectionRectangle = GameState.GraphicalEntities
                .Where(x => x.IsDrawn)
                .Where(x => selectionRectangle.Contains(x.GetWindowSpacePos()))
                .ToList();

            if(!_flatKeyboard.IsKeyDown(Keys.LeftShift))
                GameState.SelectedEntities.Clear();

            GameState.SelectedEntities.AddRange(inSelectionRectangle.Select(x => x.GameEntity));
        }

        private Rectangle GetSelectionRectangle()
        {
            Rectangle selectionRect = new Rectangle(
                (int)Math.Min(_selectionStart.X, _selectionEnd.X),
                (int)Math.Min(_selectionStart.Y, _selectionEnd.Y),
                (int)Math.Abs(_selectionStart.X - _selectionEnd.X),
                (int)Math.Abs(_selectionStart.Y - _selectionEnd.Y));

            return selectionRect;
        }

        private void ProcessPan()
        {
            if (_flatMouse.IsMiddleButtonDown())
            {
                var x = (long)(_flatMouse.MouseMovement().X * (1 / _camera.Zoom));
                var y = (long)(_flatMouse.MouseMovement().Y * -1 * (1 / _camera.Zoom));

                _camera.Position = (_camera.Position.x + x, _camera.Position.y + y);
            }
        }
    }
}
