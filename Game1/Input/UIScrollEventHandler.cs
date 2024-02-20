using Gum.Wireframe;
using RenderingLibrary;
using System.Collections.Generic;
using System.Linq;

namespace Game1.Input
{
    public class UIScrollEventHandler
    {
        public static UIScrollEventHandler Instance { get; private set; }

        public List<(GraphicalUiElement, bool horizontal)> ScrollElements = new();

        public UIScrollEventHandler()
        {
            Instance = this;
        }

        public void Update()
        {
            int div = 0;
            if (FlatMouse.Instance.ScrolledUp())
            {
                div += 25;
            }

            if(FlatMouse.Instance.ScrolledDown())
            {
                div -= 25;
            }

            if (div == 00)
                return;

            var element = FindActiveElement();

            if (element.Item1 == null)
                return;

            if (element.Item1?.Children.Count() == 0)
                return;

            if (!(element.Item1.Children.FirstOrDefault() is GraphicalUiElement))
                return;

            var child = (element.Item1.Children.First() as GraphicalUiElement);

            if (element.Item2)
            {
                //if (child.GetAbsoluteRight() < element.Item1.GetAbsoluteRight() && div < 0)
                //    return;

                child.X += (div * 2);

                //if (child.GetAbsoluteLeft() - 20 >= element.Item1.GetAbsoluteLeft())
                //    child.X -= div;
            }
            else
            {
                if (child.GetAbsoluteBottom() < element.Item1.GetAbsoluteBottom() && div < 0)
                    return;

                child.Y += div;

                if (child.GetAbsoluteTop() - 20 >= element.Item1.GetAbsoluteTop())
                    child.Y -= div;
            }
        }

        private (GraphicalUiElement, bool) FindActiveElement()
        {
            if (ScrollElements.Count == 0)
                return (null, false);

            return ScrollElements
                .Where(x => x.Item1.Visible)
                .Where(x => x.Item1.IsPointInside(FlatMouse.Instance.WindowPosition.X, FlatMouse.Instance.WindowPosition.Y))
                .OrderBy(x => x.Item1.Z)
                .FirstOrDefault();
        }
    }
}
