using Gum.Wireframe;
using RenderingLibrary;
using System.Collections.Generic;
using System.Linq;

namespace Game1.Input
{
    public class UIScrollEventHandler
    {
        public static UIScrollEventHandler Instance { get; private set; }

        private int myVar;

        public int getMyvar()
        {
            return myVar;
        }

        public void setMyvar(int value)
        {
            myVar = value;
        }




        public List<GraphicalUiElement> ScrollElements = new List<GraphicalUiElement>();

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

            if (element == null)
                return;

            if (element?.Children.Count() == 0)
                return;

            if (!(element.Children.FirstOrDefault() is GraphicalUiElement))
                return;

            var child = (element.Children.First() as GraphicalUiElement);

            var debug1 = child.GetAbsoluteBottom();
            var debug2 = element.GetAbsoluteBottom();

            if (child.GetAbsoluteBottom() < element.GetAbsoluteBottom() && div < 0)
                return;

            child.Y += div;

            if(child.GetAbsoluteTop() - 20 >= element.GetAbsoluteTop())
                child.Y -= div;
        }

        private GraphicalUiElement FindActiveElement()
        {
            return ScrollElements
                .Where(x => x.Visible)
                .Where(x => x.IsPointInside(FlatMouse.Instance.WindowPosition.X, FlatMouse.Instance.WindowPosition.Y))
                .OrderBy(x => x.Z)
                .FirstOrDefault();
        }
    }
}
