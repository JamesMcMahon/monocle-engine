using Microsoft.Xna.Framework;

namespace Monocle
{
    public class MotionBlurImage : Image
    {
        private MotionBlurData[] blurData;
        public Vector2 BlurSpeed;

        public MotionBlurImage(Subtexture subtexture, int blurs)
            : base(subtexture)
        {
            blurData = new MotionBlurData[blurs];
        }

        public override void Render()
        {
            base.Render();
            for (int i = 0; i < blurData.Length; i++)
                Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition + BlurSpeed * blurData[i].PositionMult, ClipRect, 
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
