using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class Star : Orbital
    {
        public List<Orbital> stars = new();
        public Star()
        {
            Color = Color.Yellow;
        }

        public override void Update(decimal deltaTime)
        {
            
        }
    }
}
