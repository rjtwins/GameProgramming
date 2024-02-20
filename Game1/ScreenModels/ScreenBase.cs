using Gum.Wireframe;
using MonoGame.Extended.Screens;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var camera = SystemManagers.Default.Renderer.Camera;

            var changex = (1920f - (float)GlobalStatic.Width) / 1920f;
            var changey = (1080f - (float)GlobalStatic.Height) / 1080f;

            var change = Math.Max(Math.Abs(changex), Math.Abs(changey));
            camera.Zoom = 1 + change;

            GraphicalUiElement.CanvasWidth = GlobalStatic.Width * 1 / camera.Zoom;
            GraphicalUiElement.CanvasHeight = GlobalStatic.Height * 1 / camera.Zoom;
            Screen.UpdateLayout();
        }

        public ScreenBase()
        {
            ScreenManager.Screens.Add(this);
        }

        public virtual void Hide()
        {
            Active = false;
            Screen.Visible = false;
            Screen.RemoveFromManagers();
        }

        public virtual void Show()
        {
            Screen.AddToManagers(SystemManagers.Default, null);
            Screen.Visible = true;
            this.Active = true;
        }
    }

    public static class ScreenManager
    {
        public static List<ScreenBase> Screens = new List<ScreenBase>();
    }
}
