using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class TileGrid : Component
    {
        public Vector2 Position;
        public Color Color = Color.White;
        public int VisualExtend = 0;
        public VirtualMap<MTexture> Tiles;
        public Camera ClipCamera;
        public float Alpha = 1f;

        public TileGrid(int tileWidth, int tileHeight, int tilesX, int tilesY) 
            : base(false, true)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Tiles = new VirtualMap<MTexture>(tilesX, tilesY);
        }

        #region Properties

        public int TileWidth
        {
            get; private set;
        }

        public int TileHeight
        {
            get; private set;
        }

        public int TilesX
        {
            get
            {
                return Tiles.Columns;
            }
        }

        public int TilesY
        {
            get
            {
                return Tiles.Rows;
            }
        }

        #endregion

        public void Populate(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            for (int x = 0; x < tiles.GetLength(0) && x + offsetX < TilesX; x++)
                for (int y = 0; y < tiles.GetLength(1) && y + offsetY < TilesY; y++)
                    Tiles[x + offsetX, y + offsetY] = tileset[tiles[x, y]];
        }

        public void Overlay(Tileset tileset, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            for (int x = 0; x < tiles.GetLength(0) && x + offsetX < TilesX; x++)
                for (int y = 0; y < tiles.GetLength(1) && y + offsetY < TilesY; y++)
                    if (tiles[x, y] >= 0)
                        Tiles[x + offsetX, y + offsetY] = tileset[tiles[x, y]];
        }

        public void Extend(int left, int right, int up, int down)
        {
            Position -= new Vector2(left * TileWidth, up * TileHeight);

            int newWidth = TilesX + left + right;
            int newHeight = TilesY + up + down;
            if (newWidth <= 0 || newHeight <= 0)
            {
                Tiles = new VirtualMap<MTexture>(0, 0);
                return;
            }

            var newTiles = new VirtualMap<MTexture>(newWidth, newHeight);

            //Center
            for (int x = 0; x < TilesX; x++)
            {
                for (int y = 0; y < TilesY; y++)
                {
                    int atX = x + left;
                    int atY = y + up;

                    if (atX >= 0 && atX < newWidth && atY >= 0 && atY < newHeight)
                        newTiles[atX, atY] = Tiles[x, y];
                }
            }

            //Left
            for (int x = 0; x < left; x++)
                for (int y = 0; y < newHeight; y++)
                    newTiles[x, y] = Tiles[0, Calc.Clamp(y - up, 0, TilesY - 1)];

            //Right
            for (int x = newWidth - right; x < newWidth; x++)
                for (int y = 0; y < newHeight; y++)
                    newTiles[x, y] = Tiles[TilesX - 1, Calc.Clamp(y - up, 0, TilesY - 1)];

            //Top
            for (int y = 0; y < up; y++)
                for (int x = 0; x < newWidth; x++)
                    newTiles[x, y] = Tiles[Calc.Clamp(x - left, 0, TilesX - 1), 0];

            //Bottom
            for (int y = newHeight - down; y < newHeight; y++)
                for (int x = 0; x < newWidth; x++)
                    newTiles[x, y] = Tiles[Calc.Clamp(x - left, 0, TilesX - 1), TilesY - 1];

            Tiles = newTiles;
        }

        public void FillRect(int x, int y, int columns, int rows, MTexture tile)
        {
            int left = Math.Max(0, x);
            int top = Math.Max(0, y);
            int right = Math.Min(TilesX, x + columns);
            int bottom = Math.Min(TilesY, y + rows);

            for (int tx = left; tx < right; tx++)
                for (int ty = top; ty < bottom; ty++)
                    Tiles[tx, ty] = tile;
        }

        public void Clear()
        {
            for (int tx = 0; tx < TilesX; tx++)
                for (int ty = 0; ty < TilesY; ty++)
                    Tiles[tx, ty] = null;
        }

        public Rectangle GetClippedRenderTiles()
        {
            var pos = Entity.Position + Position;

            int left, top, right, bottom;
            if (ClipCamera == null)
            {
                //throw new Exception("NULL CLIP: " + Entity.GetType().ToString());
                left = -VisualExtend;
                top = -VisualExtend;
                right = TilesX + VisualExtend;
                bottom = TilesY + VisualExtend;
            }
            else
            {
                var camera = ClipCamera;
                left = (int)Math.Max(0, Math.Floor((camera.Left - pos.X) / TileWidth) - VisualExtend);
                top = (int)Math.Max(0, Math.Floor((camera.Top - pos.Y) / TileHeight) - VisualExtend);
                right = (int)Math.Min(TilesX, Math.Ceiling((camera.Right - pos.X) / TileWidth) + VisualExtend);
                bottom = (int)Math.Min(TilesY, Math.Ceiling((camera.Bottom - pos.Y) / TileHeight) + VisualExtend);
            }

            // clamp
            left = Math.Max(left, 0);
            top = Math.Max(top, 0);
            right = Math.Min(right, TilesX);
            bottom = Math.Min(bottom, TilesY);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public override void Render()
        {
            RenderAt(Entity.Position + Position);
        }

        public void RenderAt(Vector2 position)
        {
            if (Alpha <= 0)
                return;

            var clip = GetClippedRenderTiles();
            var color = Color * Alpha;
            MTexture tile;

            for (int tx = clip.Left; tx < clip.Right; tx++)
                for (int ty = clip.Top; ty < clip.Bottom; ty++)
                {
                    tile = Tiles[tx, ty];
                    if (tile != null)
                        tile.Draw(position + new Vector2(tx * TileWidth, ty * TileHeight), Vector2.Zero, color);
                }
        }

    }
}
