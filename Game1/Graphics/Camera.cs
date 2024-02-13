using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Graphics
{
    public sealed class Camera
    {
        //Max zoom:
        private decimal _minZoomNumber => GlobalStatic.Width / (GlobalStatic.GALAXYSIZE * 4);

        public decimal Zoom 
        {
            get
            {
                return _zoom;
            }

            set
            {
                _zoom = Math.Clamp(value, _minZoomNumber, 2);
            }
        }

        private decimal _zoom = 1;

        public (decimal x, decimal y) Position
        {
            get
            {
                return (_x, _y);
            }
            set
            {
                _x = Math.Clamp(value.x, GlobalStatic.GALAXYSIZE * -1, GlobalStatic.GALAXYSIZE);
                _y = Math.Clamp(value.y, GlobalStatic.GALAXYSIZE * -1, GlobalStatic.GALAXYSIZE);
            }
        }

        private decimal _x = 0;
        private decimal _y = 0;
    }
}
