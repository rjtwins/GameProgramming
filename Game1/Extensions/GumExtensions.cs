using Gum.Wireframe;
using Microsoft.Xna.Framework;
using RenderingLibrary;

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
    }
}
