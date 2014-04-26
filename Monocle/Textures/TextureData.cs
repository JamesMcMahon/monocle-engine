using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
    public class TextureData
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Color[] Data;

        public TextureData(int width, int height)
        {
            Data = new Color[width * height];
            Width = width;
            Height = height;
        }

        public TextureData(Texture2D fromTexture2D)
        {
            Width = fromTexture2D.Width;
            Height = fromTexture2D.Height;

            Data = new Color[fromTexture2D.Width * fromTexture2D.Height];
            fromTexture2D.GetData<Color>(Data);
        }

        public TextureData(Texture fromTexture)
            : this(fromTexture.Texture2D)
        {

        }

        /*
         *  Pixel Manipulations
         */

        public Color GetPixel(int x, int y)
        {
#if DEBUG
            if (x >= Width || y >= Height || x < 0 || y < 0)
                throw new Exception("Getting a pixel that is out of bounds.");
#endif
            return Data[x + y * Width];
        }

        public void SetPixel(int x, int y, Color to)
        {
#if DEBUG
            if (x >= Width || y >= Height || x < 0 || y < 0)
                throw new Exception("Setting a pixel that is out of bounds.");
#endif
            Data[x + y * Width] = to;
        }

        public void SetPixelBlend(int x, int y, Color to)
        {
#if DEBUG
            if (x >= Width || y >= Height || x < 0 || y < 0)
                throw new Exception("Setting a pixel that is out of bounds.");
#endif
            Data[x + y * Width] = Color.Lerp(Data[x + y * Width], to, to.A / 255f);
        }

        public void SetRect(Rectangle rect, Color to)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                    SetPixel(i, j, to);
        }

        public void SetRectBlend(Rectangle rect, Color to)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                    SetPixelBlend(i, j, to);
        }

        public void Clear()
        {
            for (int i = 0; i < Width * Height; i++)
                Data[i] = Color.Transparent;
        }

        public void Clear(Color to)
        {
            for (int i = 0; i < Width * Height; i++)
                Data[i] = to;
        }

        public void ClearBlend(Color to)
        {
            for (int i = 0; i < Width * Height; i++)
                Data[i] = Color.Lerp(Data[i], to, to.A / 255f);
        }

        /*
         *  Copy from another TextureData
         */

        public void CopyPixel(TextureData source, int sourceX, int sourceY, int destX, int destY)
        {
            SetPixel(destX, destY, source.GetPixel(sourceX, sourceY));
        }

        public void CopyPixelBlend(TextureData source, int sourceX, int sourceY, int destX, int destY)
        {
            SetPixelBlend(destX, destY, source.GetPixel(sourceX, sourceY));
        }

        public void CopyPixelBlend(TextureData source, int sourceX, int sourceY, int destX, int destY, float alpha)
        {
            SetPixelBlend(destX, destY, source.GetPixel(sourceX, sourceY) * alpha);
        }

        public void CopyPixels(TextureData source, Rectangle sourceRect, int destX, int destY)
        {
            for (int i = 0; i < sourceRect.Width; i++)
                for (int j = 0; j < sourceRect.Height; j++)
                    CopyPixel(source, sourceRect.X + i, sourceRect.Y + j, destX + i, destY + j);
        }

        public void CopyPixelsBlend(TextureData source, Rectangle sourceRect, int destX, int destY)
        {
            for (int i = 0; i < sourceRect.Width; i++)
                for (int j = 0; j < sourceRect.Height; j++)
                    CopyPixelBlend(source, sourceRect.X + i, sourceRect.Y + j, destX + i, destY + j);
        }

        public void CopyPixelsBlend(TextureData source, Rectangle sourceRect, int destX, int destY, float alpha)
        {
            for (int i = 0; i < sourceRect.Width; i++)
                for (int j = 0; j < sourceRect.Height; j++)
                    CopyPixelBlend(source, sourceRect.X + i, sourceRect.Y + j, destX + i, destY + j, alpha);
        }

        /*
         *  Apply the data to a texture
         */

        public void Apply(Texture2D toTexture2D)
        {
#if DEBUG
            if (toTexture2D.Width != Width || toTexture2D.Height != Height)
                throw new Exception("Cannot apply a TextureData to a Texture of different size.");
#endif
            toTexture2D.SetData<Color>(Data);
        }

        public void Apply(Texture toTexture)
        {
            Apply(toTexture.Texture2D);
        }
    }
}
