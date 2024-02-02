﻿using System;
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
using System.ComponentModel;

namespace Flat.Entities
{

    public abstract class Entity
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public Game Game { get; set; }
        private Camera _camera {  get; set; }
        protected float _zoom => _camera.Zoom;
        public (long x, long y) Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public Color Color { get; set; }
        public float ScaleFactor => WorldSpace ? _scaleFactor : 1;
        private float _scaleFactor = 0.1f;
        public List<SubPoly> SubEntities { get; set; } = new();

        public bool WorldSpace = true;

        protected string Name { get; set; }
        protected string Label { get; set; }

        protected Entity(Game game, (long x, long y) position, Vector2 velocity, float angle, Color color, bool worldSpace = true)
        {
            WorldSpace = worldSpace;
            Game = game;
            var x = WorldSpace ? (long)(position.x * ScaleFactor) : position.x;
            var y = WorldSpace ? (long)(position.y * ScaleFactor) : position.y;
            Position = (x, y);
            Velocity = velocity;
            Angle = angle;
            Color = color;
            _camera = Game.Services.GetService<Camera>();
            Label = $"Entity: {Guid}";
        }

        public virtual void MoveTo((long x, long y) position)
        {
            Position = position;
        }

        public virtual void Update(double deltaSeconds)
        {
            if (CheckClick())
                Clicked();
        }

        public abstract void Clicked();

        public abstract bool CheckClick();

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
            if (!WorldSpace)
                return new Vector2(Position.x, Position.y);

            var device = Game.Services.GetService<GraphicsDeviceManager>();
            var width = device.PreferredBackBufferWidth;
            var height = device.PreferredBackBufferHeight;

            var x = (_camera.Position.x * -1) + Position.x;
            var y = (_camera.Position.y * -1) + Position.y;

            var pos = new Vector2(x , y) * (float)_camera.Zoom;

            pos = new Vector2((pos.X + (width / 2)), pos.Y + (height / 2));

            //Debug.WriteLine($"camera: {_camera.Position}, worldPos: {Position}, windowPos: {pos}");

            return pos;
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        //Recursively draw
        public virtual void DrawSubEntities(SpriteBatch spriteBatch)
        {
            DrawLabel(spriteBatch);
            for (int i = 0; i < SubEntities.Count; i++)
            {
                SubEntities[i].Draw(spriteBatch);
                SubEntities[i].DrawSubEntities(spriteBatch);
            }
        }

        public abstract void DrawLabel(SpriteBatch spriteBatch);

        public virtual void Scale(float amount)
        {
            _scaleFactor = amount;
        }

        public abstract bool ShouldDraw();
    }
}
