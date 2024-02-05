using Game1.GraphicalEntities;
using Microsoft.Xna.Framework;
using System;

namespace Game1.GameEntities
{
    public abstract class GameEntity
    {
        public GameGraphicalEntity GraphicalEntity { get; set; }
        public GameEntity Parent { get; set; }
        public Guid Guid { get; set; }

        public bool Selected { get; set; } = false;

        public double X { get; set; }
        public double Y { get; set; }

        public double LocalX { get; set; } = 0;
        public double LocalY { get; set; } = 0;

        //Names and stuff
        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }

        //In 1000kg (ton)
        public long Mass { get; set; } = 0;

        //kM/s
        public long Velocity { get; set; } = 0;

        //Radials against galaxtic plane x axis;
        public float Angle { get; set; } = 0f;

        public double Radius { get; set; } = 0d;

        public abstract GameGraphicalEntity GenerateGraphicalEntity(Game game);
    }
}
