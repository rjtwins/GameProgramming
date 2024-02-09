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

        private bool _measuring = false;
        private Vector2 _measureStart;
        private Vector2 _measureEnd;

        private bool _selecting = false;
        private Vector2 _selectionStart = new Vector2();
        private Vector2 _selectionEnd = new Vector2();

        private Vector2 _pointerVector = new Vector2();
        Vector2[] _cross1, _cross2;

        private Stopwatch _stopwatch = new();

        GraphicalUiElement _currentScreen { get; set; }

        SmoothFramerate framerate = new(20);

        public Game1()
        {
            _stopwatch.Start();

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
            var systems = generator.Generate(1000);

            systems.ForEach(s =>
            {
                s.Planets.ForEach(p =>
                {
                    GameState.GameEntities.Add(p);
                    p.Moons.ForEach(m =>
                    {
                        GameState.GameEntities.Add(m);
                    });
                });

                s.Stars.ForEach(st =>
                {
                    GameState.GameEntities.Add(st);
                });

                GameState.GameEntities.Add(s);
            });

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
            var location = GameState.GameEntities[5];

            fleet.X = 0;
            fleet.Y = 0;

            GameState.GameEntities.Add(fleet);

            _camera.Position = (fleet.X, fleet.Y);

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
            UpdateGum(gameTime);

            _contextMenu.Update();

            timeSinceLastTick += gameTime.ElapsedGameTime.TotalSeconds;

            _pointerVector = Mouse.GetState().Position.ToVector2();

            if (timeSinceLastTick >= 1)
            {
                GameState.Update(timeSinceLastTick);
                timeSinceLastTick = 0;
                //Debug.WriteLine(framerate.framerate);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _flatKeyboard.Update();
            _flatMouse.Update();

            if (_flatMouse.IsLeftButtonDoubleCLicked())
                HandleLeftMouseDoubleClick();

            if (_flatMouse.IsRightButtonClicked())
                HandleRightMouseClick();
            

            if (_flatMouse.IsLeftButtonClicked())
                HandleLeftMouseClick();

            if (_flatKeyboard.IsKeyClicked(Keys.OemPlus))
            {
                GameState.GameSpeed = GameState.GameSpeed * 2;
            }


            if (_flatKeyboard.IsKeyClicked(Keys.OemMinus))
            {
                GameState.GameSpeed = GameState.GameSpeed / 2;
            }


            ProcessZoom();

            ProcessPan();

            ProcessSelecting();

            ProcessMeasuring();

            if(GameState.Focus != null)
                _camera.Position = (GameState.Focus.X, GameState.Focus.Y);

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

            framerate.Update(gameTime.GetElapsedSeconds());
        }

        protected override void Draw(GameTime gameTime)
        {
            var ticks = _stopwatch.ElapsedTicks;

            GraphicsDevice.Clear(new Color(15, 15, 15));

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            var inView = GameState.GraphicalEntities
                .Where(x => x.InView())
                .ToList();

            var shouldDraw = inView
                .Where(x => x.DrawFull())
                .ToList();

            var drawInfo = inView
                .Where(x => x.DrawLabel())
                .ToList();

            var drawDot = inView.Except(shouldDraw).ToList();


            foreach (var item in shouldDraw)
            {
                item.Draw(_spriteBatch);
            }

            foreach (var item in drawDot)
            {
                _spriteBatch.DrawPoint(Util.WindowPosition(item.Position), item.Color, 2f);
            }

            foreach (var item in GameState.SelectedEntities)
            {
                var windowRect = item.GraphicalEntity.GetSelectionRect();
                _spriteBatch.DrawRectangle(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height, Color.Cyan);
            }

            foreach (var item in drawInfo)
            {
                item.DrawLabel(_spriteBatch);
            }

            GameState.GameEntities.Where(x => x is Fleet)
                .Cast<Fleet>()
                .ToList()
                .ForEach(x =>
            {
                x.DrawVelocityVector(_spriteBatch);
                x.DrawTargetLine(_spriteBatch);
            });

            DrawSelecting();
            DrawMeasuring();
            DrawMiscUI();

#if DEBUG
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"Mouse World: {Util.WorldPosition(_flatMouse.WindowPosition.ToVector2())}", new Vector2(10, 0), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"Mouse Win: {_flatMouse.WindowPosition}", new Vector2(10, 20), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"GameSpeed :{GameState.GameSpeed}", new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"GameTime: {GameState.TotalSeconds}", new Vector2(10, 60), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"Draw Ticks: {_stopwatch.ElapsedTicks - ticks}", new Vector2(10, 80), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"In View: {inView.Count}", new Vector2(10, 100), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"Draw Dot: {drawDot.Count}", new Vector2(10, 120), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"Draw Full: {shouldDraw.Count}", new Vector2(10, 140), Color.White);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"Zoom: {_camera.Zoom}", new Vector2(10, 160), Color.White);
#endif


            _spriteBatch.End();

            DrawGum();

            base.Draw(gameTime);

            firstFrame = false;


        }

        private void DrawMiscUI()
        {
            //UI stuff:
            _spriteBatch.DrawPolygon(new Vector2(GlobalStatic.Width / 2, GlobalStatic.Height / 2), _cross1, Color.Red);
            _spriteBatch.DrawPolygon(new Vector2(GlobalStatic.Width / 2, GlobalStatic.Height / 2), _cross2, Color.Red);
            
            var lineLenght = (float)(GlobalStatic.Width * 0.10);
            var height = (float)(GlobalStatic.Height * 0.95);
            var start = new Vector2(GlobalStatic.Width - 100 - lineLenght, height);
            var end = new Vector2(start.X + lineLenght, height);

            _spriteBatch.DrawLine(start, end, Color.Red, 2f);

            var length = Math.Round((decimal)lineLenght * 1 / _camera.Zoom);
            var text = $"{length} km";
            var textLenght = GlobalStatic.MainFont.MeasureString(text);

            _spriteBatch.DrawString(GlobalStatic.MainFont, text, new Vector2(start.X + (lineLenght / 2) - (textLenght.X / 2), height - 30), Color.Red);

            DrawMousePointer();
        }

        private void HandleLeftMouseDoubleClick()
        {
            var mousePos = _flatMouse.WindowPosition.ToVector2();
            RectangleF rect = new RectangleF(mousePos.X - 10f, mousePos.Y - 10f, 20, 20);

            var entity = GameState.GameEntities.Where(x =>
            {
                var winPos = Util.WindowPosition(x.GraphicalEntity.Position);
                return rect.Contains(new Point2(winPos.X, winPos.Y));
            }).MaxBy(x => x.GraphicalEntity.GetWorldDim().Length());

            if (entity == null)
                return;

            var dims = entity.GraphicalEntity.GetWindowDim();
            _camera.Position = (entity.X, entity.Y);

            var zoomx = ((decimal)(GlobalStatic.Width - 500) / (decimal)dims.X);
            var zoomy = ((decimal)(GlobalStatic.Height - 500) / (decimal)dims.Y);

            _camera.Zoom *= Math.Min(zoomx, zoomy);

            GameState.Focus = entity;
        }

        private void HandleLeftMouseClick()
        {
            if (FlatMouse.Instance.IsLeftButtonClicked() && !_flatKeyboard.IsKeyDown(Keys.LeftShift))
            {
                GameState.SelectedEntities.Clear();
                GameState.Focus = null;
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
            var pointerColor = Color.Red;
#if DEBUG
            if(_flatMouse.IsLeftButtonDoubleCLicked())
                pointerColor = Color.Green;
            if(_flatMouse.IsRightButtonDoubleCLicked())
                pointerColor = Color.Blue;
#endif

            if (_flatKeyboard.IsKeyDown(Keys.Z))
            {
                _spriteBatch.DrawCircle(_pointerVector, 5f, 255, pointerColor);
                _spriteBatch.DrawLine(_pointerVector.X + 5, _pointerVector.Y + 5, _pointerVector.X + 10, _pointerVector.Y + 10, pointerColor);
                _spriteBatch.DrawRectangle(_pointerVector.X - 10f, _pointerVector.Y - 10f, 10, 10, pointerColor);
            }
            else if(_flatKeyboard.IsKeyDown(Keys.LeftAlt))
            {
                _spriteBatch.DrawCircle(_pointerVector, 5f, 255, pointerColor);
                _spriteBatch.DrawLine(_pointerVector.X + 5, _pointerVector.Y + 5, _pointerVector.X + 10, _pointerVector.Y + 10, pointerColor);
                _spriteBatch.DrawString(GlobalStatic.MainFont, "+", new Vector2(_pointerVector.X - 13 , _pointerVector.Y - (GlobalStatic.MainFont.MeasureString("+").Y)), pointerColor);
            }
            else if (_flatMouse.IsMiddleButtonDown())
            {
                IsMouseVisible = true;
                Mouse.SetCursor(MouseCursor.Hand);
            }
            else
            {
                IsMouseVisible = false; 
                _spriteBatch.DrawCircle(_pointerVector, 5f, 255, pointerColor);
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

        private void ProcessMeasuring()
        {
            //if (!_flatKeyboard.IsKeyDown(Keys.M) && _selectionEnd != _selectionStart)
            //{
            //    _measureEnd = _flatMouse.WindowPosition.ToVector2();
            //    _measureStart = _flatMouse.WindowPosition.ToVector2();
            //}

            if (_flatKeyboard.IsKeyDown(Keys.M))
            {
                if (_measuring)
                {
                    _measureEnd = _flatMouse.WindowPosition.ToVector2();
                }
                else
                {
                    _measuring = true;
                    _measureStart = _flatMouse.WindowPosition.ToVector2();
                }
            }
            else
            {
                _measuring = false;
                _measureEnd = _flatMouse.WindowPosition.ToVector2();
                _measureStart = _flatMouse.WindowPosition.ToVector2();
            }

            Debug.WriteLine($"Start: {_measureStart} End:{_measureEnd}");
        }

        private void DrawMeasuring()
        {
            if (!_flatKeyboard.IsKeyDown(Keys.M))
                return;

            var start = _measureStart;
            var end = _measureEnd;

            var length = (decimal)Util.Distance(start, end);
            
            var angle = Util.AngleBetweenPoints(new decimal[] { (decimal)start.X, (decimal)start.Y }, new decimal[] { (decimal)end.X, (decimal)end.Y });
            var worldLength = Math.Round(length * 1 / _camera.Zoom);

            _spriteBatch.DrawCircle(start, 5f, 250, Color.Red, 1f);
            _spriteBatch.DrawLine(start, end, Color.Green, 1f);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"{worldLength} km", new Vector2(end.X + 10, end.Y + 10), Color.Red);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"{worldLength} km", new Vector2(start.X, start.Y), Color.Green, (float)angle, new Vector2((float)length/-2, 20), 1f, SpriteEffects.None, 0f);
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
            decimal zoomChange = 0;

            //Zoom:
            if (_flatMouse.ScrolledUp())
            {
                if (_flatKeyboard.IsKeyDown(Keys.LeftAlt))
                {
                    zoomChange = 1;
                    _camera.Zoom = Math.Min(_camera.Zoom *= 2, 1);
                }
                else
                {
                    zoomChange = 0.2M;
                    _camera.Zoom = Math.Min(_camera.Zoom *= (decimal)1.2, 1);
                }
            }

            if (_flatMouse.ScrolledDown())
            {
                if (_flatKeyboard.IsKeyDown(Keys.LeftAlt))
                {
                    zoomChange = -0.9M;
                    _camera.Zoom = Math.Min(_camera.Zoom *= (decimal)0.4, 1);
                }
                else
                {
                    zoomChange = -0.2M;
                    _camera.Zoom = Math.Min(_camera.Zoom *= (decimal)0.8, 1);
                }
            }

            if (zoomChange != 0M)
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
                //Debug.WriteLine(_camera.Zoom);
            }
        }

        private void ProcessSelectionZoom()
        {
            if (!_flatKeyboard.IsKeyDown(Keys.Z))
                return;

            Rectangle selectionRect = GetSelectionRectangle();

            var zx = ((decimal)GlobalStatic.Width / (decimal)(selectionRect.Width)) * _camera.Zoom;
            var zy = ((decimal)GlobalStatic.Height / (decimal)(selectionRect.Height)) * _camera.Zoom;

            var zoom = Math.Min(zx, zy);

            var worldPos = Util.WorldPosition(selectionRect.Center.ToVector2());

            _camera.Zoom = zoom;
            _camera.Position = (worldPos.x, worldPos.y);
        }

        private void ProcessSelectingEntities()
        {
            if (_flatKeyboard.IsKeyDown(Keys.Z))
                return;

            var selectionRectangle = GetSelectionRectangle();

            var inSelectionRectangle = GameState.GraphicalEntities
                .Where(x => x.InView())
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
                GameState.Focus = null;

                var initPos = _camera.Position;

                var divx = (decimal)_flatMouse.MouseMovement().X;
                var divy = (decimal)_flatMouse.MouseMovement().Y;

                var x = (divx * ((decimal)1 / _camera.Zoom));
                var y = (divy * (decimal)-1 * ((decimal)1 / _camera.Zoom));

                _camera.Position = (_camera.Position.x + x, _camera.Position.y + y);

                //Debug.WriteLine((divx, divy));
                //Debug.WriteLine($"init: {initPos}\nNew:{_camera.Position}");
            }
        }
    }
}
