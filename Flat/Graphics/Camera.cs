﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flat.Graphics
{
    public sealed class Camera
    {
        public float Zoom = 1f;
        public (long x, long y) Position = (0, 0);
    }
}
