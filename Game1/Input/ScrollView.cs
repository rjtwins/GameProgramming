//using Gum.Wireframe;
//using RenderingLibrary;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Game1.Input
//{
//    public class ScrollView
//    {
//        public GraphicalUiElement Container { get; set; }
//        public GraphicalUiElement InternalList {  get; set; }
//        public bool Horizontal { get; set; } = false;


//        //This is NOT the list of elements in the internal list!!!!
//        public HashSet<GraphicalUiElement> ContainedElements { get; set; } = new ();

//        public ScrollView(GraphicalUiElement container, GraphicalUiElement internalList, bool horizontal = false) 
//        {
//            Container = container;
//            InternalList = internalList;
//            Horizontal = horizontal;
//        }

//        public static void MoveIntoView(GraphicalUiElement element)
//        {
//            //TODO: make work for horizontal

//            var scrollView = element.Tag as ScrollView;
//            if (scrollView == null)
//                return;

//            var master = scrollView.Container;
//            var innerList = scrollView.InternalList;

//            var elementY = element.GetAbsoluteCenterY();
//            var listBot = master.GetAbsoluteBottom();
//            var listTop = master.GetAbsoluteTop() + 25;

//            if (elementY > listTop && elementY < listBot)
//                return;

//            //Move down
//            if (elementY < listTop)
//                innerList.Y += listTop - (elementY - 12.5f);

//            //Move Up
//            if(elementY > listBot)
//                innerList.Y -= (elementY + 12.5f) - listBot;
//        }
//    }
//}


using Game1.Extensions;
using Game1.ScreenModels;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Input
{
    public class ScrollView
    {
        public static List<ScrollView> ActiveListViews = new();

        public GraphicalUiElement Container { get; private set; }

        public float Top => Container.GetAbsoluteTop();
        public float Bottom => Container.GetAbsoluteBottom();
        public float Left => Container.GetAbsoluteLeft();
        public float Right => Container.GetAbsoluteRight();
        public float Height => Container.GetAbsoluteHeight();
        public float Width => Container.GetAbsoluteWidth();

        public float CursorOffset = 0;
        public float MaxItemHeight => _children.Select(x => x.Height).Max();

        public int KeyDownCount = 0;
        public int KeyUpCount = 0;

        private GraphicalUiElement Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                var old = _selected;
                old?.ApplyState("UnSelected");
                _selected = value;
                _selected?.ApplyState("Selected");

                SelectionChanged?.Invoke(old, _selected);
            }
        }

        public GraphicalUiElement _selected;

        public Action<GraphicalUiElement> OnClick { get; set; }

        /// <summary>
        /// Old, New
        /// </summary>
        public Action<GraphicalUiElement, GraphicalUiElement> SelectionChanged { get; set; }

        private List<GraphicalUiElement> _children { get; set; } = new();
        public List<ListItem> Items { get; private set; } = new();

        public ScrollView(GraphicalUiElement container)
        {
            Container = container;

            CursorOffset = 0;
            ActiveListViews.Add(this);
        }

        public void AddItems(List<GraphicalUiElement> items)
        {
            _children.AddRange(items);
            CollectionChanged();
        }

        public void SetItems(List<GraphicalUiElement> items)
        {
            _children.Clear();
            _children.AddRange(items);
            CollectionChanged();
        }

        public void SetSelected(GraphicalUiElement selected)
        {
            Selected = selected;
        }

        public GraphicalUiElement GetSelected()
        {
            return Selected;
        }

        public int GetSelectedIndex()
        {
            if (!_children.Contains(Selected))
                return -1;

            return _children.IndexOf(Selected);
        }

        public void SetSelectedIndex(int index)
        {
            if (index < 0 || index >= _children.Count)
                return;

            Selected = _children[index];
        }

        public void Update()
        {
            if (!Container.Visible)
                return;

            var topParent = Container.GetTopParent();

            if (!topParent.Visible)
                return;

            var screen = ScreenManager.GetContainingThis((GraphicalUiElement)topParent);

            if (screen != null && !screen.Active)
                return;

            //if (!Container.Contains(FlatMouse.Instance.GumPos))
            //    return;

            HandleClicking();
            HandleKeyStrokes();
            HandleScrolling();
        }

        private void HandleKeyStrokes()
        {
            if (FlatKeyboard.Instance.IsKeyClicked(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                SetSelectedIndex(GetSelectedIndex() - 1);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                SetSelectedIndex(GetSelectedIndex() + 1);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                OnClick?.Invoke(_selected);
            }
            else if (FlatKeyboard.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                KeyUpCount++;
                if (KeyUpCount > 5)
                {
                    SetSelectedIndex(GetSelectedIndex() - 1);
                    KeyUpCount = 0;
                }
            }
            else if (FlatKeyboard.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                KeyDownCount++;
                if(KeyDownCount > 5)
                {
                    SetSelectedIndex(GetSelectedIndex() + 1);
                    KeyDownCount = 0;
                }
            }
        }

        private void HandleClicking()
        {
            if (!FlatMouse.Instance.AnyButtonClicked())
                return;

            var mousePos = FlatMouse.Instance.GumPos;
            var element = _children.Where(x => x.Visible).FirstOrDefault(x => x.Contains(mousePos));
            OnClick?.Invoke(element);
        }

        private void FigureOutInView()
        {
            var queue = new Queue<ListItem>(Items);
            double skipped = 0;

            while (skipped < CursorOffset)
            {
                if (queue.Count == 0)
                    break;

                var item = queue.Dequeue();
                item.Element.Visible = false;
                //item.Element.RemoveFromManagers();
                skipped += item.Height;
            }

            double drawn = 0;
            while (drawn < Height)
            {
                if (queue.Count == 0)
                    break;

                var item = queue.Dequeue();
                drawn += item.Height;
                item.Element.Visible = true;
            }

            while (queue.TryDequeue(out ListItem item))
            {
                item.Element.Visible = false;
            }
        }

        private void HandleScrolling()
        {
            if (FlatMouse.Instance.ScrolledUp())
            {
                CursorOffset -= FlatKeyboard.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) ? 40 : 20;
                CursorOffset = Math.Max(CursorOffset, -5);
                FigureOutInView();
            }
            else if (FlatMouse.Instance.ScrolledDown())
            {
                CursorOffset += FlatKeyboard.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) ? 40 : 20;
                FigureOutInView();
            }
        }

        private void CollectionChanged()
        {
            GlobalStatic.UISemaphore.WaitOne();

            Container.SuspendLayout(true);
            Container.RemoveFromManagers();

            GlobalStatic.UISemaphore.Release();

            Items.Clear();
            Items.AddRange(_children.Select(x => new ListItem(x)));

            Container.Children.Clear();
            Items.ForEach(x => Container.Children.Add(x.Element));

            if (!_children.Contains(Selected))
                Selected = null;

            FigureOutInView();

            GlobalStatic.UISemaphore.WaitOne();

            Container.AddToManagers(SystemManagers.Default, null);
            Container.ResumeLayout(true);
            Container.UpdateLayout();

            GlobalStatic.UISemaphore.Release();
        }
    }

    public class ListItem
    {
        public GraphicalUiElement Element { get; set; }
        public float Width => Element.GetAbsoluteWidth();
        public float Height => Element.GetAbsoluteHeight();
        public Vector2 Dimensions => new Vector2(Width, Height);

        public ListItem(GraphicalUiElement element)
        {
            Element = element;
        }
    }
}
