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

namespace Game1.GraphicalEntities
{

    public abstract class Entity
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public Game Game { get; set; }
        private Camera _camera { get; set; }
        protected double _zoom => _camera.Zoom;
        public (double x, double y) Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public Color Color { get; set; }
        public float ScaleFactor => WorldSpace ? _scaleFactor : 1;
        private float _scaleFactor = 0.1f;
        public List<SubPoly> SubEntities { get; set; } = new();

        public bool WorldSpace = true;

        public string Name { get; set; }
        public string Label { get; set; }

        protected Entity(Game game, (double x, double y) position, Vector2 velocity, float angle, Color color, bool worldSpace = true)
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

        }

        public virtual void Clicked()
        {
            bool right = FlatMouse.Instance.IsRightButtonClicked();

            if (!right)
            {
                GameState.SelectedEntities.Add(this);
                GameState.SelectedEntity = this;
            }

            Debug.WriteLine($"{Guid} was clicked.");
        }

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
            Velocity += forceDirection * amount;
        }

        public virtual void ApplyForce(float amount, float angle, bool offEntityDirection = false)
        {
            angle = offEntityDirection ? angle + Angle : angle;
            Vector2 forceDirection = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            Velocity += forceDirection * amount;
        }

        public virtual Vector2 GetWindowSpacePos()
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

        public virtual Vector2 GetLabelWidth()
        {
            return GlobalStatic.MainFont.MeasureString(Label);
        }

        public virtual void Scale(float amount)
        {
            _scaleFactor = amount;
        }

        public abstract Vector2 GetDimensions();

        public virtual bool ShouldDraw()
        {
            var windowSpacePos = this.GetWindowSpacePos();
            var dims = GetDimensions();
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
