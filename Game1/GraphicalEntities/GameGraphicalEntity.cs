using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.ComponentModel;
using Game1.Graphics;
using Camera = Game1.Graphics.Camera;
using Game1.Input;
using Game1.GameEntities;

namespace Game1.GraphicalEntities
{

    public abstract class GameGraphicalEntity : GraphicalEntity
    {
        public GameEntity GameEntity { get; set; }
        private Camera _camera { get; set; }
        public override decimal _zoom => _camera.Zoom;

        public override (decimal x, decimal y) Position
        {
            get
            {
                return (GameEntity.X, GameEntity.Y);
            }
        }

        //public Vector2 Velocity { get; set; }
        public override float Angle => GameEntity.Angle;
        public override Color Color => GameEntity.Color;

        //public List<SubPoly> SubEntities { get; set; } = new();

        public override string Label => GameEntity.Name;

        public override bool IsInView 
        {
            get
            {
                if(GameEntity is Star)
                    return InView();

                if (GameEntity is Orbital)
                    return _isInView;

                return InView();
            }
            set
            {
                _isInView = value;
            }
        }
        private bool _isInView;

        protected GameGraphicalEntity() : base((0,0), 0f, Color.Black, true)
        {
            _camera = Game.Services.GetService<Camera>();
        }

        public override bool DrawLabel()
        {
            return GameEntity.DrawLabel();
        }
    }
}
