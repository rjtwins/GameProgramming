using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Flat;
using Flat.Graphics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Flat.Entities
{

    public abstract class Entity
    {
        public Game Game { get; set; }
        private OrthographicCamera _camera {  get; set; }
        protected float _zoom => _camera.Zoom;
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public Color Color { get; set; }
        public float ScaleFactor { get; set; } = 0.01f;
        public List<SubPoly> SubEntities { get; set; } = new();

        protected Entity(Game game, Vector2 position, Vector2 velocity, float angle, Color color)
        {
            Game = game;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            Color = color;
            _camera = Game.Services.GetService<OrthographicCamera>();
        }

        public virtual void MoveTo(Vector2 position)
        {
            Position = position;
        }

        public virtual void Update(double deltaSeconds)
        {
            //Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Rotate(float amount)
        {
            Angle += amount;

            //Angle = (Angle % MathHelper.TwoPi) * MathHelper.TwoPi;

            //Clamp to stop floating point issues with bit numbers.
            Angle = Angle < 0f ? Angle + MathHelper.TwoPi : Angle;
            Angle = Angle >= MathHelper.TwoPi ? Angle - MathHelper.TwoPi : Angle;
        }

        public virtual void ApplyForce(float amount)
        {
            Vector2 forceDirection = new Vector2(MathF.Cos(Angle), MathF.Sin(Angle));
            this.Velocity += forceDirection * amount;
        }

        public virtual void ApplyForce(float amount, float angle, bool offEntityDirection = false)
        {
            angle = offEntityDirection ? angle + Angle : angle;
            Vector2 forceDirection = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            this.Velocity += forceDirection * amount;
        }

        public virtual Vector2 GetWindowSpacePos()
        {
            var device = Game.Services.GetService<GraphicsDeviceManager>();
            var camera = Game.Services.GetService<Camera>();
            var width = device.PreferredBackBufferWidth;
            var height = device.PreferredBackBufferHeight;

            var pos = new Vector2((camera.Position.X *-1) - Position.X, camera.Position.Y + Position.Y) * camera.Zoom;
            pos = new Vector2((pos.X + (width / 2)), pos.Y + (height / 2));

            return pos;
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        //Recursively draw
        public virtual void DrawSubEntities(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < SubEntities.Count; i++)
            {
                SubEntities[i].Draw(spriteBatch);
                SubEntities[i].DrawSubEntities(spriteBatch);
            }
        }

        public virtual void Scale(float amount)
        {
            ScaleFactor = amount;
        }
    }
}
