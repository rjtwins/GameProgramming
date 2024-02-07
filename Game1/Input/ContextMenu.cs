using Game1.Extensions;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Game1.Input
{
    public class ContextMenu
    {
        private Game _game { get; set; }

        public Vector2 Position { get; set; }
        public List<ContextMenuItem> Items { get; set; } = new();

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

            Items.Add(new ContextMenuItem()
            {
                Guid = Guid.NewGuid(),
                Label = "TEST123",
                Position = 0
            });
        }

        public void Update()
        {
            if (_flatMouse.IsRightButtonClicked())
            {
                HandleRightButtonClicked();
            }
            else if (_flatMouse.IsLeftButtonClicked())
            {
                HandleLeftButtonClicked();
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

            Debug.WriteLine(clicked.Tag);
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

            Items.OrderBy(x => x.Position)
                .ToList()
                .ForEach(x =>
                {
                    var text = new TextRuntime();
                    text.UseCustomFont = true;
                    text.CustomFontFile = "gum/FontCache/Font18Agency_FB_noSmooth.fnt";
                    text.SetProperty("Standards/Text.Red", 255);
                    text.SetProperty("Standards/Text.Green", 0);
                    text.SetProperty("Standards/Text.Blue", 0);
                    text.Text = x.Label;
                    text.Tag = x.Guid;
                    text.AddToManagers();
                    _contextMenu.Children.Add(text);
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
