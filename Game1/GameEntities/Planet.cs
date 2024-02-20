using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class Planet : Orbital
    {
        public List<Moon> Moons = new();
        public Planet() 
        {
            Color = Color.Blue;
            GameState.Planets.Add(this);
        }

        protected override void Dispose(bool disposing)
        {
            GameState.Planets.Remove(this);
            base.Dispose(disposing);
        }
    }
}
