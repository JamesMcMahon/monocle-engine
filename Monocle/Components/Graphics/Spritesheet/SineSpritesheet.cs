using Microsoft.Xna.Framework;

namespace Monocle
{
    public class SineSpritesheet<T> : Spritesheet<T>
    {
        public enum WaveOrientation { Vertical, Horizontal };

        public WaveOrientation Orientation = WaveOrientation.Vertical;
        public float Amplitude = 2;
        public int SliceSize = 2;
        public float CounterAdd = MathHelper.TwoPi / 60;
        public float SliceAdd = MathHelper.TwoPi / 8;
        private float counter = 0;

        public SineSpritesheet(Texture texture, Rectangle? clipRect, int frameWidth, int frameHeight, int frameSep = 0)
            : base(texture, clipRect, frameWidth, frameHeight, frameSep)
        {

        }

        public SineSpritesheet(Subtexture subTexture, Rectangle? clipRect, int frameWidth, int frameHeight, int frameSep = 0)
            : base(subTexture, clipRect, frameWidth, frameHeight, frameSep)
        {
 
        }

        public SineSpritesheet(Texture texture, int frameWidth, int frameHeight, int frameSep = 0)
            : this(texture, null, frameWidth, frameHeight, frameSep)
        {

        }

        public SineSpritesheet(Subtexture subTexture, int frameWidth, int frameHeight, int frameSep = 0)
            : this(subTexture, null, frameWidth, frameHeight, frameSep)
        {
            
        }

        public override void Update()
        {
            base.Update();
            counter = MathHelper.WrapAngle(counter + CounterAdd * Engine.DeltaTime);
        }

        public override void Render()
        {
            if (Amplitude == 0)
                base.Render();
            else if (Orientation == WaveOrientation.Vertical)
                Draw.SineTextureV(Texture, CurrentClip, RenderPosition, Origin, Scale * Zoom, Rotation, Color, Effects, counter, Amplitude, SliceSize, SliceAdd);
            else
                Draw.SineTextureH(Texture, CurrentClip, RenderPosition, Origin, Scale * Zoom, Rotation, Color, Effects, counter, Amplitude, SliceSize, SliceAdd);
        }

    }
}
