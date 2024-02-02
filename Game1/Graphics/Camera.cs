using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Graphics
{
    public sealed class Camera
    {
        public double Zoom = 1f;
        public (double x, double y) Position = (0, 0);
    }
}
