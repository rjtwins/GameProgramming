using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1.Input
{
    public sealed class FlatKeyboard
    {
        private static readonly Lazy<FlatKeyboard> Lazy = new Lazy<FlatKeyboard>(() => new FlatKeyboard());

        public static FlatKeyboard Instance
        {
            get { return Lazy.Value; }
        }

        private KeyboardState prevKeyboardState;
        private KeyboardState currKeyboardState;

        public FlatKeyboard()
        {
            prevKeyboardState = Keyboard.GetState();
            currKeyboardState = prevKeyboardState;
        }

        public void Update()
        {
            prevKeyboardState = currKeyboardState;
            currKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return currKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyClicked(Keys key)
        {
            return currKeyboardState.IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);
        }
    }
}
