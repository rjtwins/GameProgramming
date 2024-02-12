using Gum.Wireframe;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Input
{
    public class UIClickEventHandler
    {
        public static UIClickEventHandler Instance { get; private set; } = new UIClickEventHandler();

        FlatMouse _flatMouse => FlatMouse.Instance;
        public Dictionary<GraphicalUiElement, Action> Elements = new();

        public UIClickEventHandler()
        {
            
        }

        public void AddElement(GraphicalUiElement element, Action onClick)
        {
            this.Elements.Add(element, onClick);
        }

        public void RemoveElement(GraphicalUiElement element)
        {
            this.Elements.Remove(element);
        }

        public void Update()
        {
            if (!_flatMouse.IsLeftButtonClicked())
                return;

            var pos = _flatMouse.WindowPosition;
            var point = new PointF(pos.X, pos.Y);

            var clicked = Elements
                .Where(x => x.Key.Visible)
                .Where(x =>
                {
                    var box = new RectangleF(x.Key.AbsoluteLeft, x.Key.AbsoluteTop, x.Key.GetAbsoluteWidth(), x.Key.GetAbsoluteHeight());
                    return box.Contains(point);
                })
                .OrderBy(x => x.Key.Z)
                .Where(x => x.Value != null)
                .ToList()
                .FirstOrDefault();

            clicked.Value?.Invoke();
        }
    }
}
