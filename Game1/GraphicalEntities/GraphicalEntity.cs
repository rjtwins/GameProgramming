﻿using Game1.GameEntities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Game1.Graphics;
using MonoGame.Extended;
using Game1.Extensions;

namespace Game1.GraphicalEntities
{
    public abstract class GraphicalEntity : IEntity
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Game Game { get; set; }
        private Camera _camera { get; set; }
        public virtual double _zoom { get; set; }
        public virtual  (double x, double y) Position { get; set; }
        public virtual float Angle { get; set; }
        public virtual Color Color { get; set; }
        public float ScaleFactor => WorldSpace ? _scaleFactor : 1;

        private float _scaleFactor = 1f;

        //public List<SubPoly> SubEntities { get; set; } = new();

        public bool WorldSpace = true;
        public bool IsDrawn { get; set; }
        public virtual string Label { get; set; }
        protected GraphicalEntity(Game game, (double x, double y) position, float angle, Color color, bool worldSpace = true)
        {
            WorldSpace = worldSpace;
            Game = game;

            _camera = Game.Services.GetService<Camera>();
        }

        public abstract void Clicked();

        public abstract bool CheckClick();

        public abstract void Draw(SpriteBatch spriteBatch);

        //Recursively draw
        public virtual void DrawSubEntities(SpriteBatch spriteBatch)
        {
            //
            //for (int i = 0; i < SubEntities.Count; i++)
            //{
            //    SubEntities[i].Draw(spriteBatch);
            //    SubEntities[i].DrawSubEntities(spriteBatch);
            //}
        }

        public abstract void DrawLabel(SpriteBatch spriteBatch);

        public virtual Vector2 GetLabelWidth()
        {
            return GlobalStatic.MainFont.MeasureString(Label);
        }

        public virtual void Scale(float amount)
        {
            _scaleFactor = amount;
        }
        public virtual Vector2 GetWindowPos()
        {
            if (!WorldSpace)
                return new Vector2((float)Position.x, (float)Position.y);

            var width = GlobalStatic.Width;
            var height = GlobalStatic.Height;

            var x = (_camera.Position.x * -1 + Position.x) * _camera.Zoom + width / 2;
            var y = (_camera.Position.y * -1 + Position.y) * _camera.Zoom + height / 2;

            var pos = new Vector2((float)x, (float)y);

            //Debug.WriteLine($"camera: {_camera.Position}, worldPos: {Position}, windowPos: {pos}");

            return pos;
        }

        public abstract Vector2 GetWindowDim();

        public abstract Vector2 GetWorldDim();

        public abstract RectangleF GetWindowRect();

        public abstract RectangleD GetWorldRect();

        public virtual RectangleF GetSelectionRect()
        {
            var windowRect = GetWindowRect();
            windowRect.Inflate(5, 5);
            return windowRect;
        }

        public virtual bool InView()
        {
            var windowSpacePos = this.GetWindowPos();
            var dims = GetWindowDim();
            var max = windowSpacePos + dims / 2;
            var min = windowSpacePos - dims / 2;

            if (max.X <= 0 && max.Y <= 0)
                return false;

            if (min.X >= GlobalStatic.Width && min.Y >= GlobalStatic.Height)
                return false;

            return true;
        }

        public virtual bool ShouldDraw()
        {
            var windowSpacePos = this.GetWindowPos();
            var dims = GetWindowDim();
            var max = windowSpacePos + dims / 2;
            var min = windowSpacePos - dims / 2;

            if (dims.X <= 2 && dims.Y <= 2)
                return false;

            if (max.X <= 0 && max.Y <= 0)
                return false;

            if (min.X >= GlobalStatic.Width && min.Y >= GlobalStatic.Height)
                return false;

            return true;
        }
    }
}
