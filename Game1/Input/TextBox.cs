using Game1.Extensions;
using Gum.Wireframe;
using info.lundin.math;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Input
{
    public class TextBox
    {
        public GraphicalUiElement Container { get; set; }
        public int CaretIndex = 0;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        private string _content { get; set; } = string.Empty;

        public bool IsFocused = false;

        public TextBox(GraphicalUiElement container)
        {
            Container = container;
            new InteractiveGUE(container).OnClick = () =>
            {
                IsFocused = true;
                //Content = string.Empty;
            };

            TextBoxManager.Textboxes.Add(this);
        }

        public void Update()
        {
            if (!IsFocused)
            {
                Container.SetProperty("TextBoxText", Content);
                return;
            }

            Debug.WriteLine(IsFocused);

            if (FlatKeyboard.Instance.TryConvertKeyboardInput(out char key))
            {
                HandleChar(key);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Back))
            {
                HandleBackspace();
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Left))
            {
                CaretIndex = Math.Max(CaretIndex - 1, 0);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Right))
            {
                CaretIndex = Math.Min(CaretIndex + 1, Content.Length);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Delete) || FlatKeyboard.Instance.IsKeyDown(Keys.Delete))
            {
                HandleDelete();
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Enter))
            {
                IsFocused = false;
            }
            
            if (IsFocused)
            {
                var caretedContent = Content.Insert(CaretIndex, "|");
                Container.SetProperty("TextBoxText", caretedContent);
            }
        }

        public void HandleChar(char c)
        {
            var keyString = c.ToString();

            Content = Content.Insert(CaretIndex, keyString);
            CaretIndex += keyString.Length;
        }

        public void HandleBackspace()
        {
            if (Content.Length == 0)
                return;

            if(CaretIndex == 0)
                return;

            Content = Content.Remove(CaretIndex - 1, 1);
            CaretIndex = Math.Max(CaretIndex - 1, 0);
        }

        public void HandleDelete()
        {
            if (Content.Length == 0)
                return;

            if (CaretIndex == Content.Length)
                return;

            Content = Content.Remove(CaretIndex, 1);
        }
    }

    public static class TextBoxManager
    {
        public static List<TextBox> Textboxes = new List<TextBox>();
        public static void Update()
        {
            if (FlatMouse.Instance.AnyButtonClicked())
                Textboxes.ForEach(x =>
                {
                    
                    x.IsFocused = x.Container.Contains(FlatMouse.Instance.GumPos);
                });

            Textboxes.ForEach(x => x.Update());
        }
    }
}
