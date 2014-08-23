using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Monocle
{
    public class Texture
    {
        public Texture2D Texture2D { get; protected set; }
        public Rectangle Rect { get; protected set; }
        public string ImagePath { get; protected set; }

        protected Texture()
        {

        }

        public Texture(string imagePath, bool load)
        {
            ImagePath = imagePath;
            if (load)
                Load();
        }

        public Texture(int width, int height, Color color)
        {
            Texture2D = new Texture2D(Engine.Instance.GraphicsDevice, width, height);
            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;
            Texture2D.SetData<Color>(data);
            Rect = new Rectangle(0, 0, width, height);
        }

        public virtual void Load()
        {
#if DEBUG
            if (Loaded)
                throw new Exception("Texture is already loaded");
            if (Engine.Instance.GraphicsDevice == null)
                throw new Exception("Cannot load until GraphicsDevice has been initialized");
#endif

            FileStream stream = new FileStream(Engine.Instance.Content.RootDirectory + ImagePath, FileMode.Open);
            Texture2D = Texture2D.FromStream(Engine.Instance.GraphicsDevice, stream);
            stream.Close();

            Rect = new Rectangle(0, 0, Texture2D.Width, Texture2D.Height);
        }

        public virtual void Unload()
        {
#if DEBUG
            if (!Loaded)
                throw new Exception("Texture is not loaded");
#endif

            Texture2D.Dispose();
            Texture2D = null;
        }

        public bool Loaded
        {
            get { return Texture2D != null; }
        }

        public int Width
        {
            get { return Rect.Width; }
        }

        public int Height
        {
            get { return Rect.Height; }
        }

        public int TotalPixels
        {
            get { return Width * Height; }
        }
    }
}
