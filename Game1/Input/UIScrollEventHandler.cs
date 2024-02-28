//using Game1.Extensions;
//using Game1.ScreenModels;
//using Gum.Wireframe;
//using RenderingLibrary;
//using System.Collections.Generic;
//using System.Linq;

//namespace Game1.Input
//{
//    public class UIScrollEventHandler
//    {
//        public static UIScrollEventHandler Instance { get; private set; }

//        public GraphicalUiElement ElementsInScrollView { get; set; }

//        public List<ScrollView> ScrollViews = new();

//        public UIScrollEventHandler()
//        {
//            Instance = this;
//        }

//        public void Update()
//        {
//            int div = 0;
//            if (FlatMouse.Instance.ScrolledUp())
//            {
//                div += 25;
//            }

//            if(FlatMouse.Instance.ScrolledDown())
//            {
//                div -= 25;
//            }

//            if (div == 00)
//                return;

//            var scrollView = FindActiveElement();

//            if (scrollView == null)
//                return;

//            if (scrollView.InternalList?.Children.Count() == 0)
//                return;

//            if(FlatKeyboard.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
//            {
//                div *= 5;
//            }

//            if (scrollView.Horizontal)
//            {
//                //if (child.GetAbsoluteRight() < element.Item1.GetAbsoluteRight() && div < 0)
//                //    return;

//                scrollView.InternalList.X += (div * 2);

//                //if (child.GetAbsoluteLeft() - 20 >= element.Item1.GetAbsoluteLeft())
//                //    child.X -= div;
//            }
//            else
//            {
//                if (scrollView.InternalList.GetAbsoluteBottom() < scrollView.Container.GetAbsoluteBottom() && div < 0)
//                    return;

//                scrollView.InternalList.Y += div;

//                if (scrollView.InternalList.GetAbsoluteTop() - 20 >= scrollView.Container.GetAbsoluteTop())
//                    scrollView.InternalList.Y -= div;
//            }

//            //if (scrollView.InternalList.Children.Count == 0)
//            //    return;

//            //var scrollContainer = element.Item1;
//            //var parentTop = scrollContainer.GetAbsoluteTop();
//            //var parentBottom = scrollContainer.GetAbsoluteBottom();

//            //child.Children
//            //    .OfType<GraphicalUiElement>()
//            //    .ToList()
//            //    .ForEach(x =>
//            //    {
//            //        var childTop = x.GetAbsoluteTop();
//            //        var childBottom = x.GetAbsoluteBottom();

//            //        if (childBottom < parentTop)
//            //            x.Z = -1;
//            //        else if (childTop > parentBottom)
//            //            x.Z = -1;

//            //        x.Z = 1;
//            //    });
//        }

//        private ScrollView FindActiveElement()
//        {
//            if (ScrollViews.Count == 0)
//                return null;

//            var mousePos = Util.WindowPosToGumPos(FlatMouse.Instance.WindowPosition.ToVector2());
            
//            return ScrollViews
//                .Where(x => x.Container.Visible)
//                .Where(x => x.Container.GetTopParent().Visible)
//                .Where(x => ScreenManager.Screens.FirstOrDefault(y => y.Screen.ContainedElements.Contains(x.Container.GetTopParent()))?.Active ?? true)
//                .Where(x => x.Container.IsPointInside(mousePos.X, mousePos.Y))
//                .OrderBy(x => x.Container.Z)
//                .FirstOrDefault();
//        }
//    }
//}
