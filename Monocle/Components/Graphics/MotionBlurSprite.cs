﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class MotionBlurSprite<T> : Sprite<T>
    {
        private MotionBlurData[] blurData;
        public Vector2 BlurSpeed;

        public MotionBlurSprite(Subtexture subtexture, int frameWidth, int frameHeight, int blurs)
            : base(subtexture, frameWidth, frameHeight)
        {
            blurData = new MotionBlurData[blurs];
        }

        public override void Render()
        {
            base.Render();
            for (int i = 0; i < blurData.Length; i++)
                Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition + BlurSpeed * blurData[i].PositionMult, CurrentClip, 
                    Color * blurData[i].Alpha, Rotation, Origin, Scale * Zoom, Effects, 0);
        }

        public void SetBlurData(int index, float alpha, float positionMult)
        {
            blurData[index].Alpha = alpha;
            blurData[index].PositionMult = positionMult;
        }

        public struct MotionBlurData
        {
            public float Alpha;
            public float PositionMult;
        }
    }
}
