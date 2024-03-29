﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class SubPoly : PolyEntity
    {
        public SubPoly(Game game, PolyEntity parent, Vector2 offset, Vector2[] vertices, float angle, Color color) : base(game, vertices, parent.Position.Add(offset), parent.Velocity, parent.Angle + angle, color, parent.WorldSpace)
        {
            
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            return;
        }
    }

    public class SubCircle : CircleEntity
    {
        public SubCircle(Game game, PolyEntity parent, Vector2 offset, long radius, float angle, Color color) : base(game, parent.Position.Add(offset), radius, parent.Velocity, angle, color, parent.WorldSpace)
        {

        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            return;
        }
    }
}
