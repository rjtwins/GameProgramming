using Game1.GameLogic;
using Game1.GraphicalEntities;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Game1.GameEntities
{
    public abstract class GameEntity: AnimatableBase, IDisposable
    {
        public GameGraphicalEntity GraphicalEntity { get; set; }

        protected GraphicalUiElement _infoContainer;
        private bool disposedValue;

        public GameEntity Parent { get; set; }
        public List<GameEntity> Children { get; set; } = new();

        public Guid Guid { get; set; } = Guid.NewGuid();
        public virtual decimal X { get; set; }
        public virtual decimal Y { get; set; }

        //Names and stuff
        public Faction Faction { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }

        //In 1kg
        public double Mass { get; set; } = 0;

        //Radials against galactic plane x axis;
        public float Angle { get; set; } = 0f;

        public decimal Radius { get; set; } = 0;

        public double SOI { get; set; } = 0d;

        public double SurfaceGravity => (GlobalStatic.G * Mass) / (double)(Math.Pow((double)Radius * 1000d, 2d));

        public abstract GameGraphicalEntity GenerateGraphicalEntity();

        public override void Update(double deltaTime)
        {

        }

        public virtual bool DrawLabel()
        {
            return true;
        }

        public List<GameEntity> GetAllChildren()
        {
            return Children.Union(Children.SelectMany(x => x.GetAllChildren())).ToList();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                this.Parent = null;
                this.GraphicalEntity.GameEntity = null;
                this.GraphicalEntity = null;
                _infoContainer = null;

                GameState.GameEntities.RemoveAll(x => x == this);

                disposedValue = true;
            }
        }

        ~GameEntity()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
