using Game1.Input;
using Game1.ScreenModels;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Game1.Extensions
{
    public static class GumExtensions
    {
        public static bool Contains(this GraphicalUiElement element, Vector2 point)
        {
            return 
                point.X > element.GetAbsoluteLeft() &&
                point.X < element.GetAbsoluteRight() &&
                point.Y > element.GetAbsoluteTop() &&
                point.Y < element.GetAbsoluteBottom();
        }

        //public static void OnClick(this GraphicalUiElement element, Action action)
        //{
        //    UIClickEventHandler.Instance.AddElement(element, action);
        //}

        //public static void OnDoubleClick(this GraphicalUiElement element, Action action)
        //{
        //    UIClickEventHandler.Instance.AddElement(element, action, true);
        //}

        //public static void OnHover(this GraphicalUiElement element, Action action)
        //{
        //    //UIClickEventHandler.Instance.AddElement(element, action);
        //}

        //public static void OnLeaveHover(this GraphicalUiElement element, Action action)
        //{
        //    //UIClickEventHandler.Instance.AddElement(element, action);
        //}
    }

    public class InteractiveGUE
    {
        public static List<InteractiveGUE> Registered = new List<InteractiveGUE>();

        public Action OnClick;
        public Action OnRightClick;
        public Action OnDoubleClick;
        public Action OnRightDoubleClick;
        public Action OnMiddleClick;

        GraphicalUiElement _graphicalUiElement;

        public InteractiveGUE(GraphicalUiElement graphicalUiElement)
        {
            _graphicalUiElement = graphicalUiElement;
            Registered.Add(this);
        }

        public static void UnRegister(InteractiveGUE interactiveGUE)
        {
            Registered.Remove(interactiveGUE);
        }

        public static void UnRegister(GraphicalUiElement graphicalUiElement)
        {
            Registered.RemoveAll(x => x._graphicalUiElement == graphicalUiElement);
        }

        public static void Update()
        {
            var collection = Registered.ToArray();
            for (int i = 0; i < collection.Length; i++)
            {
                collection[i].UpdateMember();
            }
        }

        public void UnRegister()
        {
            Registered.Remove(this);
            this._graphicalUiElement = null;
        }

        public void UpdateMember()
        {
            HandleClicks();
        }

        //TODO: Handle Scroll event here
        private void HandleScroll()
        {

        }

        private void HandleClicks()
        {
            if (_graphicalUiElement == null)
                return;

            var topParent = _graphicalUiElement.GetTopParent();

            if (!_graphicalUiElement.GetTopParent().Visible)
                return;

            var screen = ScreenManager.Screens.FirstOrDefault(x => x.Screen.ContainedElements.Contains(topParent));

            if (screen != null && !screen.Active)
                return;

            if (!_graphicalUiElement.GetTopParent().Visible)
                return;

            if (!_graphicalUiElement.Visible)
                return;

            if (OnDoubleClick != null && FlatMouse.Instance.IsLeftButtonDoubleCLicked() && ContainsMouse())
            {
                OnDoubleClick?.Invoke();
                return;
            }

            if (OnRightDoubleClick != null && FlatMouse.Instance.IsRightButtonDoubleCLicked() && ContainsMouse())
            {
                OnDoubleClick?.Invoke();
                return;
            }

            if (OnClick != null && FlatMouse.Instance.IsLeftButtonClicked() && ContainsMouse())
            {
                OnClick?.Invoke();
                return;
            }

            if (OnRightClick != null && FlatMouse.Instance.IsRightButtonClicked() && ContainsMouse())
            {
                OnRightClick?.Invoke();
                return;
            }

            if (OnMiddleClick != null && FlatMouse.Instance.IsMiddleButtonClicked() && ContainsMouse())
            {
                OnRightClick?.Invoke();
                return;
            }
        }

        private bool ContainsMouse()
        {
            var pos = FlatMouse.Instance.GumPos;
            var over = _graphicalUiElement.HasCursorOver(pos.X, pos.Y);
            return over;
        }

        ~InteractiveGUE()
        {
            UnRegister();
        }
    }
}
