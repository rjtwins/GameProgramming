﻿using Game1.Input;
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

        public static bool IsInside(this GraphicalUiElement child, GraphicalUiElement master)
        {
            return
                master.GetAbsoluteTop() <= child.GetAbsoluteTop() &&
                master.GetAbsoluteLeft() <= child.GetAbsoluteLeft() &&
                master.GetAbsoluteRight() >= child.GetAbsoluteRight() &&
                master.GetAbsoluteBottom() >= child.GetAbsoluteBottom();
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

        public GraphicalUiElement GraphicalUiElement;

        public InteractiveGUE(GraphicalUiElement graphicalUiElement)
        {
            GraphicalUiElement = graphicalUiElement;
            Registered.Add(this);
        }

        public static void UnRegister(InteractiveGUE interactiveGUE)
        {
            Registered.Remove(interactiveGUE);
        }

        public static void UnRegister(GraphicalUiElement graphicalUiElement)
        {
            Registered.RemoveAll(x => x.GraphicalUiElement == graphicalUiElement);
        }

        public static void Update()
        {
            var clicked =
                FlatMouse.Instance.IsLeftButtonClicked() ||
                FlatMouse.Instance.IsLeftButtonDoubleCLicked() ||
                FlatMouse.Instance.IsRightButtonClicked() ||
                FlatMouse.Instance.IsRightButtonDoubleCLicked() ||
                FlatMouse.Instance.IsMiddleButtonClicked();

            if (!clicked)
                return;

            var collection = Registered.ToArray();            

            if (FlatMouse.Instance.IsLeftButtonClicked())
                collection.Where(x => x.OnClick != null).Where(x => x.IsClickable()).Where(x => x.ContainsMouse()).ToList().ForEach(x => x.OnClick());

            if (FlatMouse.Instance.IsLeftButtonDoubleCLicked())
                collection.Where(x => x.OnDoubleClick != null).Where(x => x.IsClickable()).Where(x => x.ContainsMouse()).ToList().ForEach(x => x.OnDoubleClick());

            if (FlatMouse.Instance.IsRightButtonClicked())
                collection.Where(x => x.OnRightClick != null).Where(x => x.IsClickable()).Where(x => x.ContainsMouse()).ToList().ForEach(x => x.OnRightClick());

            if (FlatMouse.Instance.IsRightButtonDoubleCLicked())
                collection.Where(x => x.OnRightDoubleClick != null).Where(x => x.IsClickable()).Where(x => x.ContainsMouse()).ToList().ForEach(x => x.OnRightDoubleClick());

            if (FlatMouse.Instance.IsMiddleButtonClicked())
                collection.Where(x => x.OnMiddleClick != null).Where(x => x.IsClickable()).Where(x => x.ContainsMouse()).ToList().ForEach(x => x.OnMiddleClick());
        }

        public void UnRegister()
        {
            Registered.Remove(this);
            this.GraphicalUiElement = null;
        }

        private bool IsClickable()
        {
            if (GraphicalUiElement == null)
                return false;

            if (!GraphicalUiElement.Visible)
                return false;

            var topParent = GraphicalUiElement.GetTopParent();

            if (!topParent.Visible)
                return false;

            var screen = ScreenManager.Screens.FirstOrDefault(x => x.Screen.ContainedElements.Contains(topParent));

            if (screen != null && !screen.Active)
                return false;

            if(GraphicalUiElement.Tag is ScrollView scrollView)
                if (!GraphicalUiElement.IsInside(scrollView.Container))
                    return false;

            return true;
        }

        private bool ContainsMouse()
        {
            var pos = FlatMouse.Instance.GumPos;
            //var over = 
            //    GraphicalUiElement.GetAbsoluteBottom() > pos.Y && 
            //    GraphicalUiElement.GetAbsoluteLeft() < pos.X &&
            //    GraphicalUiElement.GetAbsoluteRight() > pos.X &&
            //    GraphicalUiElement.GetAbsoluteTop() < pos.Y;
            
            var over = GraphicalUiElement.HasCursorOver(pos.X, pos.Y);
            return over;
        }

        ~InteractiveGUE()
        {
            UnRegister();
        }
    }
}
