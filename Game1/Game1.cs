using Game1.Components;
using Game1.Extensions;
using Game1.GameEntities;
using Game1.Generators;
using Game1.GraphicalEntities;
using Game1.Input;
using Game1.ScreenModels;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

using Myra;
using Myra.Graphics2D.UI;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
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
        private RenderTarget2D _renderTarget;

        private FlatKeyboard _flatKeyboard => FlatKeyboard.Instance;
        private FlatMouse _flatMouse => FlatMouse.Instance;

        public double timeSinceLastTick { get; private set; }

        private ContextMenu _contextMenu;

        private bool _measuring = false;
        private Vector2 _measureStart;
        private Vector2 _measureEnd;

        private bool _selecting = false;
        private Vector2 _selectionStart = new Vector2();
        private Vector2 _selectionEnd = new Vector2();
        private Vector2 _pointerVector = new Vector2();

        Vector2[] _cross1, _cross2;

        private Stopwatch _stopwatch = new();

        private double _timeSinceLastTick;

        SmoothFramerate framerate = new(20);

        Desktop _desktop;
        Dialog _messageBox;

        public Game1()
        {
            GlobalStatic.UISemaphore = new System.Threading.Semaphore(0, 1);
            GlobalStatic.UISemaphore.Release();
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
            Window.Title = "Totally realistic space empire manager";
            DisplayMode dm = _graphics.GraphicsDevice.DisplayMode;

            IsMouseVisible = false;

            GlobalStatic.Width = 1920;
            GlobalStatic.Height = 1080;

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

            _pointerVector = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            
            InitUI();

            GameStateGenerator.Generate();

            //PlanetScreen.Instance.Show();

            base.Initialize();

            GameEngine.Start();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GlobalStatic.MainFont = Content.Load<SpriteFont>("Score"); // Use the name of your sprite font file here instead of 'Score'.
            GameState.GraphicalEntities.AddRange(GameState.GameEntities.Select(x => x.GenerateGraphicalEntity()));
            _contextMenu = new(this);

            _renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);

            MyraEnvironment.Game = this;

            _desktop = new Desktop();

            // Inform Myra that external text input is available
            // So it stops translating Keys to chars
            _desktop.HasExternalTextInput = true;

            // Provide that text input
            Window.TextInput += (s, a) =>
            {
                _desktop.OnChar(a.Character);
            };

            var panel = new Panel();
            panel.Left = 0;
            panel.Top = 0;
            panel.IsModal = false;

            GlobalStatic.MyraPanel = panel;
            _desktop.Root = panel;
            GlobalStatic.MyraDesktop = _desktop;

            var system = GameState.GameEntities.OfType<SolarSystem>().FirstOrDefault();
            _camera.Position = (system.X, system.Y);


            //PlanetScreen.Instance.AddSystems(GameState.SolarSystems);
            SystemList.Instance.AddSystems(GameState.SolarSystems);

            Research.Instance.Show();
        }

        protected override void Update(GameTime gameTime)
        {
            timeSinceLastTick += gameTime.ElapsedGameTime.TotalSeconds;
            if(timeSinceLastTick > 10)
            {
                timeSinceLastTick = 0;

                //System.Diagnostics.Debug.WriteLine($"LastFrameDrawStates: {SystemManagers.Default.Renderer.SpriteRenderer.LastFrameDrawStates.Count()}");

                //foreach (var item in SystemManagers.Default.Renderer.SpriteRenderer.LastFrameDrawStates)
                //{
                //    System.Diagnostics.Debug.WriteLine($"\tChangeRecords: {item.ChangeRecord.Count}");
                //    foreach (var item2 in item.ChangeRecord)
                //    {
                //        System.Diagnostics.Debug.WriteLine($"\t\t: {item2.Texture} by {item2.ObjectRequestingChange} ");
                //    }
                //}

                //System.Diagnostics.Debug.Write("\n");

                //System.Diagnostics.Debug.WriteLine($"UpdateLayoutCallCount: {GraphicalUiElement.UpdateLayoutCallCount}");
            }

            _flatKeyboard.Update();
            _flatMouse.Update();

            UpdateUI(gameTime);

            _contextMenu.Update();

            timeSinceLastTick += gameTime.ElapsedGameTime.TotalSeconds;

            _pointerVector = Mouse.GetState().Position.ToVector2();

            GameState.UpdateGameTime(gameTime.ElapsedGameTime.TotalSeconds);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_flatKeyboard.IsKeyClicked(Keys.Space))
            {
                //_messageBox.ShowModal(_desktop);
                GameState.Paused = !GameState.Paused;
            }

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

            if(_flatKeyboard.IsKeyDown(Keys.LeftAlt) && _flatKeyboard.IsKeyClicked(Keys.Enter))
            {
                Util.ToggleFullScreen(_graphics);
            }

            if (_flatKeyboard.IsKeyClicked(Keys.S))
            {
                if (PlanetScreen.Instance.Active)
                    ScreenManager.Show(Main.Instance);
                else
                    ScreenManager.Show(PlanetScreen.Instance);
            }

            if (PlanetScreen.Instance.Active)
                PlanetScreen.Instance.Update(gameTime.ElapsedGameTime.TotalSeconds);
            if (ColonyManager.Instance.Active)
                ColonyManager.Instance.Update(gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);

            framerate.Update(gameTime.GetElapsedSeconds());

            if(_flatMouse.IsLeftButtonDoubleCLicked())
                _flatMouse.ResetLeftDoubleClick();

            if (_flatMouse.IsRightButtonDoubleCLicked())
                _flatMouse.ResetRightDoubleClick();
        }

        protected override void Draw(GameTime gameTime)
        {
            GlobalStatic.UISemaphore.WaitOne();

            var ticks = _stopwatch.ElapsedTicks;

            GraphicsDevice.Clear(new Color(0, 0, 25));
            //GraphicsDevice.SetRenderTarget(_renderTarget);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Texture, 
                samplerState: SamplerState.PointClamp);

            var inView = GameState.GraphicalEntities
                .Where(x => Main.Instance.Active)
                .Where(x => x.IsInView)
                .ToList();

            var shouldDraw = inView
                .Where(x => x.DrawFull())
                .ToList();

            var drawInfo = inView
                .Where(x => x.DrawLabel())
                .ToList();

            var hideInfo = GameState.GraphicalEntities
                .Except(drawInfo)
                .ToList();

            var drawDot = inView.Except(shouldDraw).ToList();

            foreach (var item in hideInfo)
            {
                if(item.LabelRuntime != null)
                    item.LabelRuntime.Visible = false;
            }

            foreach (var item in shouldDraw)
            {
                item.Draw(_spriteBatch);
                if (item is CircleEntity c)
                {
                    if (c.GameEntity is SolarSystem s)
                        s.Children
                            .SelectMany(x => x.Children)
                            .Select(x => x.GraphicalEntity)
                            .OfType<CircleEntity>()
                            .ToList()
                            .ForEach(x => x.DrawOrbit(_spriteBatch));

                    if (c.GameEntity is Planet p)
                        p.Children
                            .Select(x => x.GraphicalEntity)
                            .Where(x => x is CircleEntity)
                            .Cast<CircleEntity>()
                            .ToList()
                            .ForEach(x => x.DrawOrbit(_spriteBatch));
                }                    
            }

            foreach (var item in drawDot)
            {
                _spriteBatch.DrawPoint(Util.WindowPosition(item.Position), item.Color, 2f);

                //This is conditional
                if (item is CircleEntity c)
                    if (c.GameEntity?.Parent?.GraphicalEntity?.IsInView == true)
                        c.DrawOrbit(_spriteBatch);              
            }

            foreach (var item in GameState.SelectedEntities)
            {
                var windowRect = item.GraphicalEntity.GetSelectionRect();
                _spriteBatch.DrawRectangle(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height, Color.Cyan);

                if(item is Fleet f)
                    f.DrawSensors(_spriteBatch);
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
            //string gameSpeed = GameState.GameSpeed.ToString() + (GameState.Paused ? " PAUSED" : "");
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"Mouse World: {Util.WorldPosition(_flatMouse.WindowPosition.ToVector2())}", new Vector2(10, 0), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"Mouse Win: {_flatMouse.WindowPosition}", new Vector2(10, 20), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"GameSpeed :{gameSpeed}", new Vector2(10, 40), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"GameTime: {GameState.TotalSeconds}", new Vector2(10, 60), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"Draw Ticks: {_stopwatch.ElapsedTicks - ticks}", new Vector2(10, 80), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"In View: {inView.Count}", new Vector2(10, 100), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"Draw Dot: {drawDot.Count}", new Vector2(10, 120), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"Draw Full: {shouldDraw.Count}", new Vector2(10, 140), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"Zoom: {_camera.Zoom}", new Vector2(10, 160), Color.White);
            //_spriteBatch.DrawString(GlobalStatic.MainFont, $"FPS: {framerate.framerate}", new Vector2(10, 180), Color.White);
#endif
            _spriteBatch.End();
            
            DrawUI();

            //_desktop.Render();

            _spriteBatch.Begin(sortMode: SpriteSortMode.Texture,
                samplerState: SamplerState.PointClamp);
            DrawMousePointer();
            _spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(null);

            //_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            //_spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
            //_spriteBatch.End();

            GlobalStatic.UISemaphore.Release();
            
            base.Draw(gameTime);
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
        }

        private void HandleLeftMouseDoubleClick()
        {
            var entity = Util.FindUnderCursor();

            if (entity == null)
                return;

            var dims = entity.GraphicalEntity.GetWindowDim();
            _camera.Position = (entity.X, entity.Y);

            var zoomx = ((decimal)(GlobalStatic.Width - 500) / (decimal)dims.X);
            var zoomy = ((decimal)(GlobalStatic.Height - 500) / (decimal)dims.Y);

            _camera.Zoom *= Math.Min(zoomx, zoomy);

            GameState.Focus = entity;
            PlanetScreen.Instance.SelectedBody = entity;
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

        private void InitUI()
        {
            SystemManagers.Default = new SystemManagers(); 
            SystemManagers.Default.Initialize(_graphics.GraphicsDevice, fullInstantiation: true);
            //SystemManagers.Default.Renderer.SinglePixelTexture = Texture2D.FromFile(_graphics.GraphicsDevice, "C:\\Users\\jorre\\source\\repos\\rjtwins\\GameProgramming\\Game1\\Content\\2d\\FontTest.png");
            //SystemManagers.Default.Renderer.SinglePixelSourceRectangle = new System.Drawing.Rectangle(0, 0, 1, 1);

            GlobalStatic.GumProject = GumProjectSave.Load("gum.gumx", out _);
            ObjectFinder.Self.GumProjectSave = GlobalStatic.GumProject;
            GlobalStatic.GumProject.Initialize();

            new SystemList();

            new MainMenu();
            //new UIScrollEventHandler();
            new Main();
            new ShipDesign();
            new PlanetScreen();
            new ColonyManager();
            new Research();

            Main.Instance.Hide();
            Main.Instance.HideTopBar();
            ShipDesign.Instance.Hide();
            MainMenu.Instance.Hide();
            PlanetScreen.Instance.Hide();
            ColonyManager.Instance.Hide();
            Research.Instance.Hide();

            //MainMenu.Instance.Show();

            Main.Instance.ShowTopBar();
            //Main.Instance.Show();
        }

        private void UpdateUI(GameTime gameTime)
        {
            //For clicks on GUM ui elements
            InteractiveGUE.Update();

            SystemManagers.Default.Activity(gameTime.TotalGameTime.TotalSeconds);

            ScrollView.ActiveListViews.ForEach(x => x.Update());
            //UIScrollEventHandler.Instance.Update();
            Main.Instance.Update(gameTime.TotalGameTime.TotalSeconds);
            Research.Instance.Update(gameTime.TotalGameTime.TotalSeconds);
        }

        private void DrawUI()
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

            //Debug.WriteLine($"Start: {_measureStart} End:{_measureEnd}");
        }

        private void DrawMeasuring()
        {
            if (!_flatKeyboard.IsKeyDown(Keys.M))
                return;

            var start = _measureStart;
            var end = _measureEnd;

            var length = (decimal)Util.Distance(start, end);
            
            //var angle = Util.AngleBetweenPoints(new decimal[] { (decimal)start.X, (decimal)start.Y }, new decimal[] { (decimal)end.X, (decimal)end.Y });
            var worldLength = Math.Round(length * 1 / _camera.Zoom);
            string lengthString = worldLength.ToString("e");


            _spriteBatch.DrawCircle(start, 5f, 250, Color.Red, 1f);
            _spriteBatch.DrawLine(start, end, Color.Green, 1f);
            _spriteBatch.DrawString(GlobalStatic.MainFont, $"{lengthString} km", new Vector2(end.X + 10, end.Y + 10), Color.Red);
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
                .Where(x => x.IsInView)
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
