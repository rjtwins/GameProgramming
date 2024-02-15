//using Gum.Wireframe;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using RenderingLibrary.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Game1.Input
//{
//    public class UIClickEventHandler
//    {
//        public static UIClickEventHandler Instance { get; private set; } = new UIClickEventHandler();

//        FlatMouse _flatMouse => FlatMouse.Instance;
//        public List<(GraphicalUiElement element, (Action action, bool doubleClick) action)> Elements = new();

//        public UIClickEventHandler()
//        {
            
//        }

//        public void AddElement(GraphicalUiElement element, Action onClick, bool doubleClick = false)
//        {
//            this.Elements.Add((element, (onClick, doubleClick)));
//        }

//        public void RemoveElement(GraphicalUiElement element, bool doubleClick = false)
//        {
//            var item = this.Elements.Where(x => x.element == element)
//                .Where(x => x.action.doubleClick = doubleClick)
//                .FirstOrDefault();

//            if (item.element == null)
//                return;

//            this.Elements.Remove(item);
//        }

//        public void Update()
//        {
//            if (!_flatMouse.IsLeftButtonClicked() && !_flatMouse.IsLeftButtonDoubleCLicked())
//                return;

//            var pos = _flatMouse.WindowPosition;
//            var point = new PointF(pos.X, pos.Y);

//            var clicked = Elements
//                .Where(x => x.element.Visible)
//                .Where(x => x.action.doubleClick == _flatMouse.IsLeftButtonDoubleCLicked())
//                .Where(x =>
//                {
//                    var box = new RectangleF(x.element.AbsoluteLeft, x.element.AbsoluteTop, x.element.GetAbsoluteWidth(), x.element.GetAbsoluteHeight());
//                    return box.Contains(point);
//                })
//                .OrderBy(x => x.element.Z)
//                .Where(x => x.element != null)
//                .ToList()
//                .FirstOrDefault();

//            clicked.action.action?.Invoke();
//        }
//    }
//}
