﻿using Flat.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat.Entities
{
    public class CircleEntity : PolyEntity
    {
        float _radius { get; set; } = 1f;

        public CircleEntity(Game game, Vector2 position, int radius, Vector2 velocity, float angle, Color color) : base(game, null, position, velocity, angle, color)
        {
            _radius = radius;
        }

        public override void Draw(Shapes shapes)
        {
            var scaleFactor = FixeScreenSize ? (1 / ScaleFactor) : ScaleFactor;

            shapes.DrawCircle(Position.X, Position.Y, _radius * scaleFactor, 100, 1f, Color);
            //base.Draw(shapes);
        }
    }
}