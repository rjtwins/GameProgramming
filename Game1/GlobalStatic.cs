using Game1.GameEntities;
using Game1.Graphics;
using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using System;
using System.Threading;

namespace Game1
{
    public static class GlobalStatic
    {
        public static Semaphore UISemaphore { get; set; }
        public static SpriteFont MainFont { get; set; }
        public static int Height { get; set; } = 0;
        public static int Width { get; set; } = 0;
        public static Game Game { get; set; } = null;
        public static GumProjectSave GumProject { get; set; } = null;
        public static Container MyraPanel { get; set; } = null;
        public static Desktop MyraDesktop { get; set; } = null;


        public const double G = 6.67430e-11; // Gravitational constant in m^3/kg/s^2

        //All in km.
        public const decimal AU = 1.5e8M;
        public const decimal SYSTEMSIZE = AU * 100;
        public const decimal GALAXYSIZE = AU * 1e9M;

        public const double MSOLL = 2e30d;
        public const double MEARTH = 6e24d;
        public const double MLUNA = 7.34e22d;

        public const decimal RSOLL = 7e5M;
        public const decimal REARTH = 6371;
        public const decimal RLUNA = 1740;

        //In kelvin
        public const double TSOLL = 5778;

        public const double BasicAgri = 0.15;
    }
}
