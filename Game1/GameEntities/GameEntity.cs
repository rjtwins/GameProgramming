using Game1.GraphicalEntities;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum.GueDeriving;
using System;

namespace Game1.GameEntities
{
    public abstract class GameEntity
    {
        public GameGraphicalEntity GraphicalEntity { get; set; }

        protected RectangleRuntime _container;

        public GameEntity Parent { get; set; }
        public Guid Guid { get; set; }

        public bool Selected { get; set; } = false;

        public decimal X { get; set; }
        public decimal Y { get; set; }

        public decimal LocalX { get; set; } = 0;
        public decimal LocalY { get; set; } = 0;

        //Names and stuff
        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }

        //In 1kg
        public double Mass { get; set; } = 0;

        //Radials against galaxtic plane x axis;
        public float Angle { get; set; } = 0f;

        public decimal Radius { get; set; } = 0;

        public double SOI { get; set; } = 0d;

        public abstract GameGraphicalEntity GenerateGraphicalEntity();

        public virtual void Update(decimal deltaTime)
        {

        }

        public virtual bool DrawLabel()
        {
            return true;
        }
    }
}
