using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class SpritePart<T> : Sprite<T>
    {
        public float DrawX = 0;
        public float DrawY = 0;
        public float DrawWidth = 1;
        public float DrawHeight = 1;

        public SpritePart(Texture texture, Rectangle? clipRect, int frameWidth, int frameHeight, int frameSep = 0)
            : base(texture, clipRect, frameWidth, frameHeight, frameSep)
        {
            
        }

        public SpritePart(Subtexture subTexture, Rectangle? clipRect, int frameWidth, int frameHeight, int frameSep = 0)
            : base(subTexture, clipRect, frameWidth, frameHeight, frameSep)
        {
            
        }

        public SpritePart(Texture texture, int frameWidth, int frameHeight, int frameSep = 0)
            : this(texture, null, frameWidth, frameHeight, frameSep)
        {

        }

        public SpritePart(Subtexture subTexture, int frameWidth, int frameHeight, int frameSep = 0)
            : this(subTexture, null, frameWidth, frameHeight, frameSep)
        {
            
        }

        public override void Render()
        {
            Rectangle clip = CurrentClip;
            clip.X += (int)(DrawX * Width);
            clip.Y += (int)(DrawY * Height);
            clip.Width = (int)(DrawWidth * clip.Width);
            clip.Height = (int)(DrawHeight * clip.Height);

            Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition, clip, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }
    }
}
