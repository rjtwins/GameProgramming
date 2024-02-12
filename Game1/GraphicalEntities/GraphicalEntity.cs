using Game1.GameEntities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Game1.Graphics;
using MonoGame.Extended;
using Game1.Extensions;
using System.Net.Http.Headers;
using MonoGameGum.GueDeriving;

namespace Game1.GraphicalEntities
{
    public abstract class GraphicalEntity : IEntity
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Game Game { get; set; }
        private Camera _camera { get; set; }
        public virtual (decimal x, decimal y) Position { get; set; }
        public virtual decimal _zoom { get; set; }
        public virtual float MinSize { get; set; } = 0.0001f;
        public virtual float Angle { get; set; }
        public virtual Color Color { get; set; }
        public bool FixedSize { get; set; }
        //public List<SubPoly> SubEntities { get; set; } = new();
        public bool WorldSpace = true;
        public bool IsDrawn { get; set; }
        public virtual bool IsInView { get
            {
                return InView();
            }
            set
            {

            }
        }

        public virtual string Label { get; set; }
        public virtual bool ShouldDrawLabel {  get; set; } = true;
        public virtual TextRuntime LabelRuntime { get; set; }

        protected GraphicalEntity((double x, double y) position, float angle, Color color, bool worldSpace = true)
        {
            WorldSpace = worldSpace;
            Game = GlobalStatic.Game;

            _camera = Game.Services.GetService<Camera>();
        }

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

        public abstract Vector2 GetWindowDim();

        public abstract Vector2 GetWorldDim();

        public abstract RectangleF GetWindowRect();

        public abstract RectangleM GetWorldRect();

        public virtual RectangleF GetSelectionRect()
        {
            var windowRect = GetWindowRect();
            windowRect.Inflate(5, 5);
            return windowRect;
        }

        public virtual bool InView()
        {
            var windowPos = Util.WindowPosition(this.Position);
            var windowDim = GetWindowDim();
            var width = windowDim.X;
            var height = windowDim.Y;
            
            if (windowPos.X + width < 0)
                return false;

            if(windowPos.Y + height < 0)
                return false;

            if(windowPos.X - height > GlobalStatic.Width) 
                return false;

            if (windowPos.Y - height > GlobalStatic.Height)
                return false;

            var size = Math.Max(width, height);

            if(size <= MinSize)
                return false;

            return true;
        }

        public virtual bool DrawFull()
        {
            var windowSpacePos = Util.WindowPosition(this.Position);
            var dims = GetWindowDim();
            var size = Math.Max(dims.X, dims.Y);

            return size > 2;
        }

        public virtual bool DrawLabel()
        {
            return DrawFull();
        }
    }
}
