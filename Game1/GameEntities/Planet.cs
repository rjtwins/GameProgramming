﻿using Game1.GraphicalEntities;
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
        }
    }
}
