using Microsoft.Xna.Framework;

namespace Monocle
{
    public class FlashingImage : Image
    {
        private Color[] colors;
        private int flashIndex;
        private float flashTimer;
        private float flashDelay;

        public FlashingImage(Texture texture, Rectangle? clipRect = null)
            : base(texture, clipRect, true)
        {

        }

        public FlashingImage(Subtexture subTexture, Rectangle? clipRect = null)
            : base(subTexture, clipRect, true)
        {

        }

        public override void Update()
        {
            if (flashTimer > 0)
            {
                flashTimer -= Engine.DeltaTime;
                if (flashTimer <= 0)
                {
                    flashTimer += flashDelay;
                    flashIndex = (flashIndex + 1) % colors.Length;
                    Color = colors[flashIndex];
                }
            }
        }

        public void StartFlashing(float delay, params Color[] colors)
        {
            flashTimer = flashDelay = delay;
            flashIndex = 0;
            Color = colors[0];
            this.colors = colors;
        }

        public void StopFlashing()
        {
            flashTimer = 0;
        }
    }
}
