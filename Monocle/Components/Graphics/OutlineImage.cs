using Microsoft.Xna.Framework;

namespace Monocle
{
    public class OutlineImage : Image
    {
        public Color OutlineColor;

        public OutlineImage(Subtexture subtexture)
            : base(subtexture)
        {
            OutlineColor = Color.Black;
        }

        public override void Render()
        {
            for (int i = -1; i < 2; i++) 
                for (int j = -1; j < 2; j++)
                    if (i != 0 || j != 0)
                        Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition + new Vector2(i, j), ClipRect, 
                            OutlineColor, Rotation, Origin, Scale * Zoom, Effects, 0);
            Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition, ClipRect, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }
    }
}
