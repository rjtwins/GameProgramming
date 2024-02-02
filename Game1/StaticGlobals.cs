﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class GlobalStatic
    {
        public static SpriteFont MainFont { get; set; }
        public static int Height { get; set; } = 0;
        public static int Width { get; set; } = 0;

        public const double SYSTEMSIZE = 200d * 149597870000d;
    }
}