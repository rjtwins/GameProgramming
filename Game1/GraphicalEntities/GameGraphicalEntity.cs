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
        public override double _zoom => _camera.Zoom;

        public override (double x, double y) Position
        {
            get
            {
                var x = WorldSpace ? (GameEntity.X * ScaleFactor) : GameEntity.X;
                var y = WorldSpace ? (GameEntity.Y * ScaleFactor) : GameEntity.Y;

                return (x, y);
            }
        }

        //public Vector2 Velocity { get; set; }
        public override float Angle => GameEntity.Angle;
        public override Color Color => GameEntity.Color;

        //public List<SubPoly> SubEntities { get; set; } = new();

        public override string Label => GameEntity.Name;

        protected GameGraphicalEntity(Game game) : base(game, (0,0), 0f, Color.Black, true)
        {
            Game = game;
            _camera = Game.Services.GetService<Camera>();
        }

        public override void Clicked()
        {

        }
    }
}
