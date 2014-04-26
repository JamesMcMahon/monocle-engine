﻿using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class DrawRectangle : Component
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public Color Color;

        public DrawRectangle(float x, float y, float width, float height, Color color)
            : base(false, true)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
        }

        public override void Render()
        {
            Vector2 renderAt;
            if (Entity)
                renderAt = Entity.Position + new Vector2(X, Y);
            else
                renderAt = new Vector2(X, Y);

            Draw.Rect(renderAt.X, renderAt.Y, Width, Height, Color);
        }
    }
}
