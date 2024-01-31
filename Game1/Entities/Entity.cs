using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Flat;
using Flat.Graphics;

namespace Game1.Entities
{

    public abstract class Entity
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public Color Color { get; set; }
        public float ScaleFactor { get; set; } = 0.01f;
        public bool FixeScreenSize { get; set; } = false;

        protected Entity(Vector2 position, Vector2 velocity, float angle, Color color)
        {
            Position = position;
            Velocity = velocity;
            Angle = angle;
            Color = color;
        }

        public virtual void MoveTo(Vector2 position)
        {
            Position = position;
        }

        public virtual void Update(GameTime gameTime)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
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

        public virtual void FixScreenSize(bool state)
        {
            FixeScreenSize = state;
        }

        public abstract void Draw(Shapes shapes);
    }
}
