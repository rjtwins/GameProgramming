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
        private double _timeSinceLastLeft, _timeSinceLastRight, _timeSinceLastDelete, _timeSinceLastBackspace;

        public delegate void TextChangedEventHandler(string newText);
        public event TextChangedEventHandler OnTextChanged;

        public delegate void TextBoxEnterEventHandler(string newText);
        public event TextBoxEnterEventHandler OnTextBoxEnterPressed;

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

                OnTextChanged?.Invoke(_content);
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
                CaretIndex = 0;
            };

            TextBoxManager.Textboxes.Add(this);
        }

        public void Update()
        {
            if (!IsFocused)
            {
                Container.SetProperty("Text", Content);
                Container.SetProperty("TextBoxText", Content);
                return;
            }

            if (FlatKeyboard.Instance.TryConvertKeyboardInput(out char key))
            {
                HandleChar(key);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Back))
            {
                _timeSinceLastBackspace = 0;
                HandleBackspace();
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Left))
            {
                _timeSinceLastLeft = 0;
                CaretIndex = Math.Max(CaretIndex - 1, 0);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Right))
            {
                _timeSinceLastRight = 0;
                CaretIndex = Math.Min(CaretIndex + 1, Content.Length);
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Delete))
            {
                _timeSinceLastDelete = 0;
                HandleDelete();
            }
            else if (FlatKeyboard.Instance.IsKeyDown(Keys.Back))
            {
                _timeSinceLastBackspace++;
                if (_timeSinceLastBackspace > 4)
                {
                    HandleBackspace();
                    _timeSinceLastBackspace = 0;
                }
            }
            else if (FlatKeyboard.Instance.IsKeyDown(Keys.Left))
            {
                _timeSinceLastLeft++;
                if (_timeSinceLastLeft > 4)
                {
                    CaretIndex = Math.Max(CaretIndex - 1, 0);
                    _timeSinceLastLeft = 0;
                }
            }
            else if (FlatKeyboard.Instance.IsKeyDown(Keys.Right))
            {
                _timeSinceLastRight++;
                if (_timeSinceLastRight > 4)
                {
                    CaretIndex = Math.Min(CaretIndex + 1, Content.Length);
                    _timeSinceLastRight = 0;
                }
            }
            else if (FlatKeyboard.Instance.IsKeyDown(Keys.Delete))
            {
                _timeSinceLastDelete++;
                if (_timeSinceLastDelete > 4)
                {
                    HandleDelete();
                    _timeSinceLastDelete = 0;
                }
            }
            else if (FlatKeyboard.Instance.IsKeyClicked(Keys.Enter))
            {
                IsFocused = false;
                OnTextBoxEnterPressed?.Invoke(Content);
            }
            
            if (IsFocused)
            {
                CaretIndex = Math.Clamp(CaretIndex, 0, Math.Max(0, Content.Length));
                var caretedContent = Content.Insert(CaretIndex, "|");
                Container.SetProperty("Text", caretedContent);
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

        public void SetText(string text, bool preventEvents = false)
        {
            if (preventEvents)
                _content = text;
            else
                Content = text;
        }
    }

    public static class TextBoxManager
    {
        public static List<TextBox> Textboxes = new List<TextBox>();
        public static void Update()
        {
            Textboxes.ForEach(x => x.Update());
        }
    }
}
