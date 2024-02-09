using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        (decimal x, decimal y) Position { get; }

        //List<SubPoly> SubEntities { get; set; }
        void Draw(SpriteBatch spriteBatch);
        void DrawLabel(SpriteBatch spriteBatch);
        bool DrawLabel();
        void DrawSubEntities(SpriteBatch spriteBatch);
        Vector2 GetWindowDim();
        Vector2 GetLabelWidth();
        bool DrawFull();
    }
}