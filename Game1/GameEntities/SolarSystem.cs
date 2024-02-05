﻿using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class SolarSystem : BodyBase
    {
        public List<Planet> Planets { get; set; } = new();
        public List<Star> Stars { get; set; } = new();
        public SolarSystem() 
        {
            Color = Color.Gray;
        }

        public override GameGraphicalEntity GenerateGraphicalEntity(Game game)
        {
            var entity = new CircleEntity(game);
            entity.GameEntity = this;
            return entity;
        }
    }
}
