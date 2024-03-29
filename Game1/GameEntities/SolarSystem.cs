﻿using Game1.GraphicalEntities;
using Game1.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.GameEntities
{
    public class SolarSystem : GameEntity
    {
        public SolarSystem()
        {
            Color = Color.FloralWhite;
            GameState.SolarSystems.Add(this);
        }

        public override GameGraphicalEntity GenerateGraphicalEntity()
        {
            var entity = new CircleEntity();
            entity.GameEntity = this;
            this.GraphicalEntity = entity;
            return entity;
        }

        public override bool DrawLabel()
        {
            var camera = GlobalStatic.Game.Services.GetService<Camera>();
            var diagWorldDis = (1 / camera.Zoom) * (decimal)Math.Sqrt(GlobalStatic.Width * GlobalStatic.Width + GlobalStatic.Height * GlobalStatic.Height);
            if (diagWorldDis > 2000000000 * GlobalStatic.AU)
                return false;

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            GameState.SolarSystems.Remove(this);
            base.Dispose(disposing);
        }
    }
}
