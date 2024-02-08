using Game1;
using Game1.Extensions;
using Game1.GameEntities;
using Game1.GameLogic;
using Game1.GraphicalEntities;
using Game1.Graphics;
using Game1.Input;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
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
        private ContextMenu _contextMenu;

        private double timeSinceLastTick = 0;
        private bool firstFrame = true;

        private bool _selecting = false;
        
        private Vector2 _selectionStart = new Vector2();
        private Vector2 _selectionEnd = new Vector2();
        private Vector2 _pointerVector = new Vector2();
        Vector2[] _cross1, _cross2;

        GraphicalUiElement _currentScreen { get; set; }

        SmoothFramerate framerate = new(20);

        public Game1()
        {
            GlobalStatic.Game = this;

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
            FlatMouse.Init(this);

            _cross1 = new Vector2[2];
            _cross1[0] = new(10, 0);
            _cross1[1] = new(-10, 0);

            _cross2 = new Vector2[2];
            _cross2[0] = new(0, 10);
            _cross2[1] = new(0, -10);

            //var subShp = new SubPoly(this, ship, new Vector2(5f, 5f), vertices, 0f, Color.Red);
            //subShp.Scale(ship.ScaleFactor / 2);
            //ship.SubEntities.Add(subShp);


            var fleet = new Fleet()
            {
                Name = "Fleet 1",
            };

            var ship = new Ship()
            {
                Mass = 100,
                Fuel = 100,
                Crew = 100,
                MaxThrust = 100
            };

            fleet.Members.Add(ship);

            GameState.GameEntities.Add(fleet);

            _pointerVector = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            /*
            //Earth moon system:
            //var earth = new Planet()
            //{
            //    Radius = 6317000,
            //    X = 149600000000,
            //    Y = 0,
            //    Name = "EARTH"
            //};

            //var moon = new Moon()
            //{
            //    Radius = 6317000,
            //    X = 149600000000,
            //    Y = 384400000,
            //    Name = "MOON"
            //};

            //var sun = new Star()
            //{
            //    Radius = 696000000,
            //    X = 0,
            //    Y = 0,
            //    Name = "SUN"
            //};


            //var solarSystem = new SolarSystem()
            //{
            //    Radius = GlobalStatic.SYSTEMSIZE,
            //    X = 0,
            //    Y = 0,
            //    Name = "SOLAR SYSTEM"
            //};

            //var proxima = new Star()
            //{
            //    Radius = 696000000,
            //    X = 40208000000000000,
            //    Y = 696000000,
            //    Name = "PROXIMA"
            //};

            //var proximaSystem = new SolarSystem()
            //{
            //    Radius = GlobalStatic.SYSTEMSIZE,
            //    X = 40208000000000000,
            //    Y = 0,
            //    Name = "PROXIMA SYSTEM"
            //};

            //GameState.GameEntities.AddRange(new List<GameEntity>() { earth, sun, solarSystem, proxima });
            */

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
            //this.IsMouseVisible = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalStatic.MainFont = Content.Load<SpriteFont>("Score"); // Use the name of your sprite font file here instead of 'Score'.
            GameState.GraphicalEntities.AddRange(GameState.GameEntities.Select(x => x.GenerateGraphicalEntity()));

            _contextMenu = new(this);
        }

        protected override void Update(GameTime gameTime)
        {
            //if (firstFrame)
            //{
            //    _camera.Position = (GlobalStatic.GALAXYSIZE / 2, GlobalStatic.GALAXYSIZE / 2);
            //    _camera.Zoom = 3.5 * Math.Pow(10, -14);
            //}

            UpdateGum(gameTime);

            _contextMenu.Update();

            ProcessSelecting();

            timeSinceLastTick += gameTime.ElapsedGameTime.TotalSeconds;

            _pointerVector = Mouse.GetState().Position.ToVector2();

            if (timeSinceLastTick >= 1)
            {
                GameState.Update(timeSinceLastTick);
                timeSinceLastTick = 0;
                Debug.WriteLine(framerate.framerate);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _flatKeyboard.Update();
            _flatMouse.Update();

            if (_flatMouse.IsRightButtonClicked())
                HandleRightMouseClick();
            

            if (_flatMouse.IsLeftButtonClicked())
                HandleLeftMouseClick();

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

            if(_flatKeyboard.IsKeyDown(Keys.LeftAlt) && _flatKeyboard.IsKeyClicked(Keys.Enter))
            {
                Util.ToggleFullScreen(_graphics);
            }

            base.Update(gameTime);

            firstFrame = false;

            framerate.Update(gameTime.GetElapsedSeconds());
        }

        protected override void Draw(GameTime gameTime)
        {
            //Clear window
            GraphicsDevice.Clear(new Color(15, 15, 15));

            //On Window
            _spriteBatch.Begin();

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
                    _spriteBatch.DrawPoint(Util.WindowPosition(e.Position), e.Color, 2f);
                }
                else
                {
                    e.Draw(_spriteBatch);
                    e.DrawSubEntities(_spriteBatch);
                }

                if (GameState.SelectedEntities.Contains(e.GameEntity))
                {
                    var windowRect = e.GetSelectionRect();
                    _spriteBatch.DrawRectangle(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height, Color.Cyan);
                }
            });
            GameState.GameEntities.Where(x => x is Fleet)
                .Cast<Fleet>()
                .ToList()
                .ForEach(x => 
            {
                x.DrawVelocityVector(_spriteBatch);
                x.DrawTargetLine(_spriteBatch);
            });

            //UI stuff:
            _spriteBatch.DrawPolygon(new Vector2(GlobalStatic.Width / 2, GlobalStatic.Height / 2), _cross1, Color.Red);
            _spriteBatch.DrawPolygon(new Vector2(GlobalStatic.Width / 2, GlobalStatic.Height / 2), _cross2, Color.Red);

            DrawMousePointer();

            DrawSelecting();

            _spriteBatch.End();

            DrawGum();

            base.Draw(gameTime);
        }

        private void HandleLeftMouseClick()
        {
            if (FlatMouse.Instance.IsLeftButtonClicked() && !_flatKeyboard.IsKeyDown(Keys.LeftShift))
            {
                GameState.SelectedEntities.Clear();
            }
        }

        private void HandleRightMouseClick()
        {

        }

        private void InitGum()
        {
            SystemManagers.Default = new SystemManagers(); 
            SystemManagers.Default.Initialize(_graphics.GraphicsDevice, fullInstantiation: true);

            GlobalStatic.GumProject = GumProjectSave.Load("gum.gumx", out _);
            ObjectFinder.Self.GumProjectSave = GlobalStatic.GumProject;
            GlobalStatic.GumProject.Initialize();

            // This assumes that your project has at least 1 screen
            _currentScreen = GlobalStatic.GumProject.Screens.First().ToGraphicalUiElement(SystemManagers.Default, addToManagers: true);
        }

        private void UpdateGum(GameTime gameTime)
        {
            SystemManagers.Default.Activity(gameTime.TotalGameTime.TotalSeconds);
        }

        private void DrawGum()
        {
            SystemManagers.Default.Draw();
        }

        private void DrawMousePointer()
        {
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"{Util.WorldPosition(_flatMouse.WindowPosition.ToVector2())}", Vector2.Zero, Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"{_flatMouse.WindowPosition}", new Vector2(0, 20), Color.White);

            if (_flatKeyboard.IsKeyDown(Keys.Z))
            {
                _spriteBatch.DrawCircle(_pointerVector, 5f, 255, Color.Red);
                _spriteBatch.DrawLine(_pointerVector.X + 5, _pointerVector.Y + 5, _pointerVector.X + 10, _pointerVector.Y + 10, Color.Red);
                _spriteBatch.DrawRectangle(_pointerVector.X - 10f, _pointerVector.Y - 10f, 10, 10, Color.Red);
            }
            else if(_flatKeyboard.IsKeyDown(Keys.LeftAlt))
            {
                _spriteBatch.DrawCircle(_pointerVector, 5f, 255, Color.Red);
                _spriteBatch.DrawLine(_pointerVector.X + 5, _pointerVector.Y + 5, _pointerVector.X + 10, _pointerVector.Y + 10, Color.Red);
                _spriteBatch.DrawString(GlobalStatic.MainFont, "+", new Vector2(_pointerVector.X - 13 , _pointerVector.Y - (GlobalStatic.MainFont.MeasureString("+").Y)), Color.Red);
            }
            else if (_flatMouse.IsMiddleButtonDown())
            {
                IsMouseVisible = true;
                Mouse.SetCursor(MouseCursor.Hand);
            }
            else
            {
                IsMouseVisible = false; 
                _spriteBatch.DrawCircle(_pointerVector, 5f, 255, Color.Red);
            }
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
                _spriteBatch.DrawRectangle(selectionRect, Color.Green);
            }
        }

        private void ProcessZoom()
        {
            var mousePos = Util.WorldPosition(_flatMouse.WindowPosition.ToVector2());
            var zoom = _camera.Zoom;
            var zoomChange = 0f;

            //Zoom:
            if (_flatMouse.ScrolledUp())
            {
                if (_flatKeyboard.IsKeyDown(Keys.LeftAlt))
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
                if (_flatKeyboard.IsKeyDown(Keys.LeftAlt))
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
                Debug.WriteLine(_camera.Zoom);
            }
        }

        private void ProcessSelectionZoom()
        {
            if (!_flatKeyboard.IsKeyDown(Keys.Z))
                return;

            Rectangle selectionRect = GetSelectionRectangle();

            var zx = ((double)GlobalStatic.Width / (double)selectionRect.Width) * _camera.Zoom;
            var zy = ((double)GlobalStatic.Height / (double)selectionRect.Height) * _camera.Zoom;

            var zoom = Math.Min(zx, zy);

            var worldPos = Util.WorldPosition(selectionRect.Center.ToVector2());

            _camera.Zoom = zoom;
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
            if (_flatKeyboard.IsKeyDown(Keys.Z))
                return;

            var selectionRectangle = GetSelectionRectangle();

            var isDrawn = GameState.GraphicalEntities
                .Where(x => x.IsDrawn)
                .ToList();

            var inSelectionRectangle = GameState.GraphicalEntities
                .Where(x => x.IsDrawn)
                .Where(x => selectionRectangle.Contains((Rectangle)x.GetWindowRect()))
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
