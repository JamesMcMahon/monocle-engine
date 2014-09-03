using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public class Tilemap : GraphicsComponent
    {
        public Canvas Canvas { get; private set; }

        private Texture tileset;
        private Rectangle clipRect;
        private Rectangle[] tileRects;

        public Tilemap(int width, int height)
            : base(false)
        {
            Canvas = new Canvas(width, height);
        }

        public void SetTileset(Texture tileset, int tileWidth, int tileHeight, int tileSep = 0)
        {
            SetTileset(tileset, tileset.Rect, tileWidth, tileHeight, tileSep);
        }

        public void SetTileset(Subtexture tileset, int tileWidth, int tileHeight, int tileSep = 0)
        {
            SetTileset(tileset.Texture, tileset.Rect, tileWidth, tileHeight, tileSep);
        }

        public void SetTileset(Texture tileset, Rectangle clipRect, int tileWidth, int tileHeight, int tileSep = 0)
        {
            this.tileset = tileset;
            this.clipRect = clipRect;

            int tilesX = 0, tilesY = 0;
            for (int i = 0; i <= clipRect.Width - tileWidth; i += tileWidth + tileSep)
                tilesX++;
            for (int i = 0; i <= clipRect.Height - tileHeight; i += tileHeight + tileSep)
                tilesY++;

            tileRects = new Rectangle[tilesX * tilesY];
            for (int j = 0; j < tilesY; j++)
                for (int i = 0; i < tilesX; i++)
                    tileRects[i + j * tilesX] = new Rectangle(clipRect.X + i * (tileWidth + tileSep), clipRect.Y + j * (tileHeight + tileSep), tileWidth, tileHeight);
        }

        public void BeginTiling(BlendState blendState)
        {
            Draw.SetTarget(Canvas);
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        }

        public void BeginTiling(BlendState blendState, int offsetX, int offsetY)
        {
            Draw.SetTarget(Canvas);
            Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.CreateTranslation(offsetX, offsetY, 0));
        }

        public void EndTiling()
        {
            Draw.SpriteBatch.End();
            Draw.ResetTarget();
        }

        public void DrawTile(int tileID, int x, int y)
        {
#if DEBUG
            if (tileset == null)
                throw new Exception("Must assign a tileset to the tilemap before drawing tiles to it. Use SetTileset()");
            if (tileID >= tileRects.Length)
                throw new Exception("The given tileID is out of range. The tileset doesn't have that many tiles in it!");
#endif
            Draw.Texture(tileset, tileRects[tileID], new Vector2(x, y), Color.White);
        }

        public void ClearTile(int x, int y)
        {
#if DEBUG
            if (tileset == null)
                throw new Exception("Must assign a tileset to the tilemap before drawing tiles to it. Use SetTileset()");
#endif
            Draw.Rect(x, y, tileRects[0].Width, tileRects[0].Height, Color.Transparent);
        }

        public void LoadCSV(string data)
        {
#if DEBUG
            if (tileset == null)
                throw new Exception("Must assign a tileset to the tilemap before loading CSV to the tilemap. Use SetTileset()");
#endif

            char[] find = new char[] { ',', '\n' };

            int x = 0;
            int y = 0;

            while (data.Length > 0)
            {
                int currentTile;
                char found = ' ';

                int at = data.IndexOfAny(find);
                if (at == -1)
                {
                    currentTile = Convert.ToInt32(data);
                    data = "";
                }
                else
                {
                    currentTile = Convert.ToInt32(data.Substring(0, at));
                    found = data[at];
                    data = data.Substring(at + 1);
                }

                if (currentTile != -1)
                    DrawTile(currentTile, x * tileRects[0].Width, y * tileRects[0].Height);

                if (found == ',')
                    x++;
                else if (found == '\n')
                {
                    x = 0;
                    y++;
                }
            }
        }

        public float Width
        {
            get { return Canvas.Width; }
        }

        public float Height
        {
            get { return Canvas.Height; }
        }

        public Vector2 HalfSize
        {
            get { return new Vector2(Width / 2, Height / 2); }
        }

        public override void Render()
        {
            Draw.SpriteBatch.Draw(Canvas.Texture2D, RenderPosition, null, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }
    }
}
