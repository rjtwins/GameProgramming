using Gum.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public static class GlobalStatic
    {
        public static SpriteFont MainFont { get; set; }
        public static int Height { get; set; } = 0;
        public static int Width { get; set; } = 0;
        public static Game Game { get; set; } = null;
        public static GumProjectSave GumProject { get; set; } = null;

        //Size statics
        public const double SYSTEMSIZE = (100d * 149597870000d);
        //100 000 ly
        public const double GALAXYSIZE = 100000000000000000d;

    }
}
