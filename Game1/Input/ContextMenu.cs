using Game1.Extensions;
using Game1.GameEntities;
using Game1.GameLogic;
using Game1.Graphics;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Game1.Input
{
    public class ContextMenu
    {
        private Game _game { get; set; }
        public Vector2 Position { get; set; }
        public List<ContextMenuItem> Items { get; set; } = new();
        public GameEntity TargetEntity { get; set; }
        private FlatMouse _flatMouse => FlatMouse.Instance;
        private FlatKeyboard _flatKeyboard => FlatKeyboard.Instance;

        private ContainerRuntime _contextMenu;

        public ContextMenu(Game game)
        {
            _game = game;
            _contextMenu = new ContainerRuntime();
            _contextMenu.ChildrenLayout = Gum.Managers.ChildrenLayout.TopToBottomStack;
            _contextMenu.X = 0;
            _contextMenu.Y = 0;
            _contextMenu.Visible = false;
            _contextMenu.AddToManagers();
        }

        public void GenerateContextMenuItems()
        {
            Items.Clear();

            if (GameState.SelectedEntities.Any(x => x is Fleet))
                Items.AddRange(GetFleetCommands());

            //Items.Add(new ContextMenuItem()
            //{
            //    Guid = Guid.NewGuid(),
            //    Label = "test 123",
            //    Position = 0
            //});
        }

        private IEnumerable<ContextMenuItem> GetFleetCommands()
        {
            List<ContextMenuItem> contextMenuItems = new List<ContextMenuItem>();

            var fleets = GameState.SelectedEntities.Where(x => x is Fleet).Cast<Fleet>().ToList();

            var moveOrder = new ContextMenuItem()
            {
                Label = "Move To",
                Position = 0,
                Guid = Guid.NewGuid(),
                OnClick = () =>
                {
                    fleets.ForEach(x => x.Orders.Add(new Order()
                    {
                        OrderType = OrderType.MoveTo,
                        Owner = x,
                        Position = Util.WorldPosition(this.Position)
                    }));
                }
            };
            var teleportOrder = new ContextMenuItem()
            {
                Label = "Teleport To",
                Position = 1,
                Guid = Guid.NewGuid(),
                OnClick = () =>
                {
                    fleets.ForEach(x =>
                    {
                        var pos = Util.WorldPosition(this.Position);
                        x.X = pos.x;
                        x.Y = pos.y;
                    });
                }
            };
            var moveToTest = new ContextMenuItem()
            {
                Label = "Intercept Test",
                Position = 1,
                Guid = Guid.NewGuid(),
                OnClick = () =>
                {
                    var fleet = fleets.First();
                    var body = TargetEntity == null ? Util.FindUnderPos(Position) : TargetEntity;

                    if (body == null)
                        return;

                    if (!(body is Orbital o))
                        return;

                    var result = Util.AproxIntercept(fleet, o);
                    var camera = GlobalStatic.Game.Services.GetService<Camera>();

                    camera.Position = result.pos;

                    fleet.Orders.Add(new GameLogic.Order()
                    {
                        OrderType = GameLogic.OrderType.MoveTo,
                        Position = result.pos,
                        Guid = Guid.NewGuid()
                    });
                }
            };

            contextMenuItems.Add(moveOrder);
            contextMenuItems.Add(teleportOrder);
            contextMenuItems.Add(moveToTest);

            return contextMenuItems;
        }

        public void Update()
        {
            if (_flatMouse.IsRightButtonClicked())
            {
                HandleRightButtonClicked();
            }
            else if (_flatMouse.IsLeftButtonClicked())
            {
                //HandleLeftButtonClicked();
                _contextMenu.Children.Cast<GraphicalUiElement>().ToList().ForEach(x =>
                {
                    InteractiveGUE.UnRegister(x);
                });

                _contextMenu.Children.Clear();
                _contextMenu.Visible = false;
            }
        }

        private void HandleLeftButtonClicked()
        {
            if (!_contextMenu.Visible)
                return;

            var mousePos = _flatMouse.WindowPosition;

            if (!_contextMenu.Contains(mousePos.ToVector2()))
                return;

            var clicked = _contextMenu
                .Children
                .Where(x => x is GraphicalUiElement)
                .Cast<GraphicalUiElement>()
                .OrderByDescending(x => x.Z).ThenByDescending(x => x.Width * x.Height)
                .FirstOrDefault(x => x.Contains(mousePos.ToVector2()));

            if (clicked == null)
                return;

            //Debug.WriteLine(clicked.Tag);
            ContextMenuItem? item = Items.FirstOrDefault(x => x.Guid == (Guid)clicked.Tag);

            if (item == null)
                return;

            if (item.Value.OnClick == null)
                return;

            item.Value.OnClick();
        }

        private void HandleRightButtonClicked()
        {
            var mousePos = _flatMouse.WindowPosition;
            var contextEntities = GameState.SelectedEntities.Where(x => x.GraphicalEntity.GetSelectionRect().Contains(mousePos));

            GenerateContextMenuItems();

            if (Items.Count == 0)
                return;

            _contextMenu.Children.Clear();
            _contextMenu.Visible = true;
            _contextMenu.X = _flatMouse.WindowPosition.X + 10;
            _contextMenu.Y = _flatMouse.WindowPosition.Y + 10;
            this.Position = _flatMouse.WindowPosition.ToVector2();
            TargetEntity = Util.FindUnderCursor();

            Items.OrderBy(x => x.Position)
                .ToList()
                .ForEach(x =>
                {
                    var text = new TextRuntime();
                    //text.UseCustomFont = true;
                    //text.CustomFontFile = "gum/FontCache/Font18Agency_FB_noSmooth.fnt";

                    text.UseCustomFont = false;
                    text.Font = "Calibri Light";
                    text.FontSize = 18;
                    text.UseFontSmoothing = false;

                    text.SetProperty("Red", 255);
                    text.SetProperty("Green", 0);
                    text.SetProperty("Blue", 0);
                    text.Text = x.Label;
                    text.Tag = x.Guid;
                    text.AddToManagers();
                    _contextMenu.Children.Add(text);

                    var interactable = new InteractiveGUE(text);
                    interactable.OnClick = x.OnClick;
                });
        }
    }


    public struct ContextMenuItem
    {
        public Guid Guid { get; set; }
        public int Position { get; set; } //Order in menu;
        public string Label { get; set; }
        public Action OnClick { get; set; }
    }
}
