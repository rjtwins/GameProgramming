using Game1.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Game1.GraphicalEntities
{
    public interface IEntity
    {
        bool IsDrawn { get; set; }
        float Angle { get; set; }
        Color Color { get; set; }
        Game Game { get; set; }
        Guid Guid { get; set; }
        string Label { get; set; }
        (double x, double y) Position { get; }
        float ScaleFactor { get; }
        //List<SubPoly> SubEntities { get; set; }
        void Draw(SpriteBatch spriteBatch);
        void DrawLabel(SpriteBatch spriteBatch);
        void DrawSubEntities(SpriteBatch spriteBatch);
        Vector2 GetWindowDim();
        Vector2 GetLabelWidth();
        void Scale(float amount);
        bool ShouldDraw();
    }
}