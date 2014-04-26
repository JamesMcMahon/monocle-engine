using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public class Canvas : Texture
    {
        public RenderTarget2D RenderTarget2D { get; private set; }

        public Canvas(int width, int height)
        {
#if DEBUG
            if (Engine.Instance.GraphicsDevice == null)
                throw new Exception("Cannot create a canvas until the GraphicsDevice has been initialized");
#endif

            RenderTarget2D = new RenderTarget2D(Engine.Instance.GraphicsDevice, width, height);
            Texture2D = (Texture2D)RenderTarget2D;
            Rect = new Rectangle(0, 0, width, height);
            ImagePath = "";
        }

        public Canvas(Texture texture)
            : this(texture.Width, texture.Height)
        {
#if DEBUG
            if (Engine.Instance.GraphicsDevice == null)
                throw new Exception("Cannot create a canvas until the GraphicsDevice has been initialized");
#endif

            Color[] data = new Color[texture.Width * texture.Height];
            texture.Texture2D.GetData<Color>(data);
            RenderTarget2D.SetData<Color>(data);
        }

        public override void Load()
        {
            throw new Exception("Cannot load a Canvas");
        }

        public override void Unload()
        {
#if DEBUG
            if (RenderTarget2D.IsDisposed)
                throw new Exception("Canvas has already been unloaded");
#endif   
            RenderTarget2D.Dispose();
            RenderTarget2D = null;
            Texture2D = null;
        }

        public void Clear()
        {
            Draw.SetTarget(this);
            Draw.Clear();
            Engine.Instance.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
