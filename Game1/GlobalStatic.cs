using Game1.GameEntities;
using Game1.Graphics;
using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Game1
{
    public static class GlobalStatic
    {
        public static SpriteFont MainFont { get; set; }
        public static int Height { get; set; } = 0;
        public static int Width { get; set; } = 0;
        public static Game Game { get; set; } = null;
        public static GumProjectSave GumProject { get; set; } = null;

        public const decimal AU = 1.5e8M;
        public const decimal SYSTEMSIZE = AU * 100;
        public const decimal GALAXYSIZE = AU * 1e9M;

        public const double MSOLL = 2e30d;
        public const double MEARTH = 6e25d;
        public const double MLUNA = 7.34e22d;

        public const decimal RSOLL = 7e5M;
        public const decimal REARTH = 6371;
        public const decimal RLUNA = 1740;
    }
}
