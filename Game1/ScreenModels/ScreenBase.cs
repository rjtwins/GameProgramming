using Gum.Wireframe;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ScreenModels
{
    public abstract class ScreenBase
    {
        public GraphicalUiElement Screen { get; set; }
        public bool Active { get; set; } = false;
        public virtual void UpdateResolution()
        {
            GraphicalUiElement.CanvasWidth = GlobalStatic.Width;
            GraphicalUiElement.CanvasHeight = GlobalStatic.Height;
            Screen.UpdateLayout();
        }
    }
}
