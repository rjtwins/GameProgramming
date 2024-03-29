﻿using Flat;
using Flat.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Shapes;
using Flat.Input;
using Microsoft.Xna.Framework.Input;
using GameLogic;

namespace Flat.Entities
{
    public class PolyEntity : Entity
    {
        public Vector2[] Vertices {  get; set; }
        private Polygon Polygon { get; set; }
        public bool FixLineWidth { get; set; } = true;
        public float LineWidth { get; set; } = 2f;
        protected float ActualLineWidth => LineWidth;
        private Polygon _scaledPolygon {  get; set; }

        public PolyEntity(Game game, Vector2[] vertices, (long x, long y) position, Vector2 velocity, float angle, Color color, bool worldSpace = true) : base(game, position, velocity, angle, color, worldSpace)
        {
            if (vertices == null)
                return;

            Vertices = vertices.Select(x => x * ScaleFactor).ToArray();
            Polygon = new Polygon(Vertices);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _scaledPolygon = Polygon;
            if (WorldSpace)
                _scaledPolygon = Polygon.TransformedCopy(Vector2.Zero, Angle, new Vector2(_zoom));// = new Polygon(Vertices.Select(x => x * _zoom).ToArray());

            spriteBatch.DrawPolygon(GetWindowSpacePos(), _scaledPolygon, Color, ActualLineWidth);
        }

        public override void DrawLabel(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Label))
                return;

            if (GlobalStatic.MainFont == null)
                return;

            var windowPos = GetWindowSpacePos();
            var x = windowPos.X;
            var y = windowPos.Y - _scaledPolygon.BoundingRectangle.Y;

            spriteBatch.DrawString(GlobalStatic.MainFont, Label, new Vector2(x, y), Color);
        }

        public override bool ShouldDraw()
        {
            return true;
        }

        public override void Clicked()
        {
            bool right = FlatMouse.Instance.IsRightButtonClicked();

            if(!right)
                GameState.
        }

        public override bool CheckClick()
        {
            if (!FlatMouse.Instance.IsLeftButtonClicked() && !FlatMouse.Instance.IsRightButtonClicked())
                return false;

            var mousePos = Mouse.GetState().Position;
            var polygon = _scaledPolygon.TransformedCopy(GetWindowSpacePos(), 0f, Vector2.Zero);

            if (!polygon.Contains(mousePos.ToVector2()))
                return false;

            return true;
        }
    }
}
