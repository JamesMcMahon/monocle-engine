using System;
using System.Xml;

namespace Monocle
{
    static public class Tiler
    {
        public enum EdgeBehavior { True, False, Wrap };

        static public int[,] Tile(bool[,] bits, Func<int> tileHandler, Tilemap tilemap, int tileWidth, int tileHeight, EdgeBehavior edges)
        {
            int boundsX = bits.GetLength(0);
            int boundsY = bits.GetLength(1);
            int[,] tiles = new int[boundsX, boundsY];

            for (TileX = 0; TileX < boundsX; TileX++)
            {
                for (TileY = 0; TileY < boundsY; TileY++)
                {
                    if (bits[TileX, TileY])
                    {
                        switch (edges)
                        {
                            case EdgeBehavior.True:
                                Left = TileX == 0 ? true : bits[TileX - 1, TileY];
                                Right = TileX == boundsX - 1 ? true : bits[TileX + 1, TileY];
                                Up = TileY == 0 ? true : bits[TileX, TileY - 1];
                                Down = TileY == boundsY - 1 ? true : bits[TileX, TileY + 1];

                                UpLeft = (TileX == 0 || TileY == 0) ? true : bits[TileX - 1, TileY - 1];
                                UpRight = (TileX == boundsX - 1 || TileY == 0) ? true : bits[TileX + 1, TileY - 1];
                                DownLeft = (TileX == 0 || TileY == boundsY - 1) ? true : bits[TileX - 1, TileY + 1];
                                DownRight = (TileX == boundsX - 1 || TileY == boundsY - 1) ? true : bits[TileX + 1, TileY + 1];
                                break;

                            case EdgeBehavior.False:
                                Left = TileX == 0 ? false : bits[TileX - 1, TileY];
                                Right = TileX == boundsX - 1 ? false : bits[TileX + 1, TileY];
                                Up = TileY == 0 ? false : bits[TileX, TileY - 1];
                                Down = TileY == boundsY - 1 ? false : bits[TileX, TileY + 1];

                                UpLeft = (TileX == 0 || TileY == 0) ? false : bits[TileX - 1, TileY - 1];
                                UpRight = (TileX == boundsX - 1 || TileY == 0) ? false : bits[TileX + 1, TileY - 1];
                                DownLeft = (TileX == 0 || TileY == boundsY - 1) ? false : bits[TileX - 1, TileY + 1];
                                DownRight = (TileX == boundsX - 1 || TileY == boundsY - 1) ? false : bits[TileX + 1, TileY + 1];
                                break;

                            case EdgeBehavior.Wrap:
                                Left = bits[(TileX + boundsX - 1) % boundsX, TileY];
                                Right = bits[(TileX + 1) % boundsX, TileY];
                                Up = bits[TileX, (TileY + boundsY - 1) % boundsY];
                                Down = bits[TileX, (TileY + 1) % boundsY];

                                UpLeft = bits[(TileX + boundsX - 1) % boundsX, (TileY + boundsY - 1) % boundsY];
                                UpRight = bits[(TileX + 1) % boundsX, (TileY + boundsY - 1) % boundsY];
                                DownLeft = bits[(TileX + boundsX - 1) % boundsX, (TileY + 1) % boundsY];
                                DownRight = bits[(TileX + 1) % boundsX, (TileY + 1) % boundsY];
                                break;
                        }

                        int tile = tileHandler();
                        if (tile != -1)
                            tilemap.DrawTile(tile, TileX * tileWidth, TileY * tileHeight);
                        tiles[TileX, TileY] = tile;
                    }
                }
            }

            return tiles;
        }

        static public int[,] Tile(bool[,] bits, bool[,] also, Func<int> tileHandler, Tilemap tilemap, int tileWidth, int tileHeight, EdgeBehavior edges)
        {
            int boundsX = bits.GetLength(0);
            int boundsY = bits.GetLength(1);
            int[,] tiles = new int[boundsX, boundsY];

            for (TileX = 0; TileX < boundsX; TileX++)
            {
                for (TileY = 0; TileY < boundsY; TileY++)
                {
                    if (bits[TileX, TileY])
                    {
                        switch (edges)
                        {
                            case EdgeBehavior.True:
                                Left = TileX == 0 ? true : bits[TileX - 1, TileY] || also[TileX - 1, TileY];
                                Right = TileX == boundsX - 1 ? true : bits[TileX + 1, TileY] || also[TileX + 1, TileY];
                                Up = TileY == 0 ? true : bits[TileX, TileY - 1] || also[TileX, TileY - 1];
                                Down = TileY == boundsY - 1 ? true : bits[TileX, TileY + 1] || also[TileX, TileY + 1];

                                UpLeft = (TileX == 0 || TileY == 0) ? true : bits[TileX - 1, TileY - 1] || also[TileX - 1, TileY - 1];
                                UpRight = (TileX == boundsX - 1 || TileY == 0) ? true : bits[TileX + 1, TileY - 1] || also[TileX + 1, TileY - 1];
                                DownLeft = (TileX == 0 || TileY == boundsY - 1) ? true : bits[TileX - 1, TileY + 1] || also[TileX - 1, TileY + 1];
                                DownRight = (TileX == boundsX - 1 || TileY == boundsY - 1) ? true : bits[TileX + 1, TileY + 1] || also[TileX + 1, TileY + 1];
                                break;

                            case EdgeBehavior.False:
                                Left = TileX == 0 ? false : bits[TileX - 1, TileY] || also[TileX - 1, TileY];
                                Right = TileX == boundsX - 1 ? false : bits[TileX + 1, TileY] || also[TileX + 1, TileY];
                                Up = TileY == 0 ? false : bits[TileX, TileY - 1] || also[TileX, TileY - 1];
                                Down = TileY == boundsY - 1 ? false : bits[TileX, TileY + 1] || also[TileX, TileY + 1];

                                UpLeft = (TileX == 0 || TileY == 0) ? false : bits[TileX - 1, TileY - 1] || also[TileX - 1, TileY - 1];
                                UpRight = (TileX == boundsX - 1 || TileY == 0) ? false : bits[TileX + 1, TileY - 1] || also[TileX + 1, TileY - 1];
                                DownLeft = (TileX == 0 || TileY == boundsY - 1) ? false : bits[TileX - 1, TileY + 1] || also[TileX - 1, TileY + 1];
                                DownRight = (TileX == boundsX - 1 || TileY == boundsY - 1) ? false : bits[TileX + 1, TileY + 1] || also[TileX + 1, TileY + 1];
                                break;

                            case EdgeBehavior.Wrap:
                                Left = bits[(TileX + boundsX - 1) % boundsX, TileY] || also[(TileX + boundsX - 1) % boundsX, TileY];
                                Right = bits[(TileX + 1) % boundsX, TileY] || also[(TileX + 1) % boundsX, TileY];
                                Up = bits[TileX, (TileY + boundsY - 1) % boundsY] || also[TileX, (TileY + boundsY - 1) % boundsY];
                                Down = bits[TileX, (TileY + 1) % boundsY] || also[TileX, (TileY + 1) % boundsY];

                                UpLeft = bits[(TileX + boundsX - 1) % boundsX, (TileY + boundsY - 1) % boundsY] || also[(TileX + boundsX - 1) % boundsX, (TileY + boundsY - 1) % boundsY];
                                UpRight = bits[(TileX + 1) % boundsX, (TileY + boundsY - 1) % boundsY] || also[(TileX + 1) % boundsX, (TileY + boundsY - 1) % boundsY];
                                DownLeft = bits[(TileX + boundsX - 1) % boundsX, (TileY + 1) % boundsY] || also[(TileX + boundsX - 1) % boundsX, (TileY + 1) % boundsY];
                                DownRight = bits[(TileX + 1) % boundsX, (TileY + 1) % boundsY] || also[(TileX + 1) % boundsX, (TileY + 1) % boundsY];
                                break;
                        }

                        int tile = tileHandler();
                        if (tile != -1)
                            tilemap.DrawTile(tile, TileX * tileWidth, TileY * tileHeight);
                        tiles[TileX, TileY] = tile;
                    }
                }
            }

            return tiles;
        }

        static public int[,] Tile(bool[,] bits, AutotileData autotileData, Tilemap tilemap, int tileWidth, int tileHeight, EdgeBehavior edges)
        {
            return Tile(bits, autotileData.TileHandler, tilemap, tileWidth, tileHeight, edges);
        }

        static public int[,] Tile(bool[,] bits, bool[,] also, AutotileData autotileData, Tilemap tilemap, int tileWidth, int tileHeight, EdgeBehavior edges)
        {
            return Tile(bits, also, autotileData.TileHandler, tilemap, tileWidth, tileHeight, edges);
        }

        static public int TileX { get; private set; }
        static public int TileY { get; private set; }
        static public bool Left { get; private set; }
        static public bool Right { get; private set; }
        static public bool Up { get; private set; }
        static public bool Down { get; private set; }
        static public bool UpLeft { get; private set; }
        static public bool UpRight { get; private set; }
        static public bool DownLeft { get; private set; }
        static public bool DownRight { get; private set; }
    }

    public class AutotileData
    {
        public int[] Center;
        public int[] Single;
        public int[] SingleHorizontalLeft;
        public int[] SingleHorizontalCenter;
        public int[] SingleHorizontalRight;
        public int[] SingleVerticalTop;
        public int[] SingleVerticalCenter;
        public int[] SingleVerticalBottom;
        public int[] Top;
        public int[] Bottom;
        public int[] Left;
        public int[] Right;
        public int[] TopLeft;
        public int[] TopRight;
        public int[] BottomLeft;
        public int[] BottomRight;
        public int[] InsideTopLeft;
        public int[] InsideTopRight;
        public int[] InsideBottomLeft;
        public int[] InsideBottomRight;

        public AutotileData(XmlElement xml)
        {
            Center = Calc.ReadCSVInt(xml.ChildText("Center", ""));
            Single = Calc.ReadCSVInt(xml.ChildText("Single", ""));

            SingleHorizontalLeft = Calc.ReadCSVInt(xml.ChildText("SingleHorizontalLeft", ""));
            SingleHorizontalCenter = Calc.ReadCSVInt(xml.ChildText("SingleHorizontalCenter", ""));
            SingleHorizontalRight = Calc.ReadCSVInt(xml.ChildText("SingleHorizontalRight", ""));

            SingleVerticalTop = Calc.ReadCSVInt(xml.ChildText("SingleVerticalTop", ""));
            SingleVerticalCenter = Calc.ReadCSVInt(xml.ChildText("SingleVerticalCenter", ""));
            SingleVerticalBottom = Calc.ReadCSVInt(xml.ChildText("SingleVerticalBottom", ""));

            Top = Calc.ReadCSVInt(xml.ChildText("Top", ""));
            Bottom = Calc.ReadCSVInt(xml.ChildText("Bottom", ""));
            Left = Calc.ReadCSVInt(xml.ChildText("Left", ""));
            Right = Calc.ReadCSVInt(xml.ChildText("Right", ""));

            TopLeft = Calc.ReadCSVInt(xml.ChildText("TopLeft", ""));
            TopRight = Calc.ReadCSVInt(xml.ChildText("TopRight", ""));
            BottomLeft = Calc.ReadCSVInt(xml.ChildText("BottomLeft", ""));
            BottomRight = Calc.ReadCSVInt(xml.ChildText("BottomRight", ""));

            InsideTopLeft = Calc.ReadCSVInt(xml.ChildText("InsideTopLeft", ""));
            InsideTopRight = Calc.ReadCSVInt(xml.ChildText("InsideTopRight", ""));
            InsideBottomLeft = Calc.ReadCSVInt(xml.ChildText("InsideBottomLeft", ""));
            InsideBottomRight = Calc.ReadCSVInt(xml.ChildText("InsideBottomRight", ""));
        }

        public int TileHandler()
        {
            if (Tiler.Left && Tiler.Right && Tiler.Up && Tiler.Down && Tiler.UpLeft && Tiler.UpRight && Tiler.DownLeft && Tiler.DownRight)
                return GetTileID(Center);

            else if (!Tiler.Up && !Tiler.Down)
            {
                if (Tiler.Left && Tiler.Right)
                    return GetTileID(SingleHorizontalCenter);
                else if (!Tiler.Left && !Tiler.Right)
                    return GetTileID(Single);
                else if (Tiler.Left)
                    return GetTileID(SingleHorizontalRight);
                else
                    return GetTileID(SingleHorizontalLeft);
            }
            else if (!Tiler.Left && !Tiler.Right)
            {
                if (Tiler.Up && Tiler.Down)
                    return GetTileID(SingleVerticalCenter);
                else if (Tiler.Down)
                    return GetTileID(SingleVerticalTop);
                else
                    return GetTileID(SingleVerticalBottom);
            }

            else if (Tiler.Up && Tiler.Down && Tiler.Left && !Tiler.Right)
                return GetTileID(Right);
            else if (Tiler.Up && Tiler.Down && !Tiler.Left && Tiler.Right)
                return GetTileID(Left);

            else if (Tiler.Up && !Tiler.Left && Tiler.Right && !Tiler.Down)
                return GetTileID(BottomLeft);
            else if (Tiler.Up && Tiler.Left && !Tiler.Right && !Tiler.Down)
                return GetTileID(BottomRight);
            else if (Tiler.Down && Tiler.Right && !Tiler.Left && !Tiler.Up)
                return GetTileID(TopLeft);
            else if (Tiler.Down && !Tiler.Right && Tiler.Left && !Tiler.Up)
                return GetTileID(TopRight);

            else if (Tiler.Up && Tiler.Down && !Tiler.DownRight && Tiler.DownLeft)
                return GetTileID(InsideTopLeft);
            else if (Tiler.Up && Tiler.Down && Tiler.DownRight && !Tiler.DownLeft)
                return GetTileID(InsideTopRight);
            else if (Tiler.Up && Tiler.Down && Tiler.UpLeft && !Tiler.UpRight)
                return GetTileID(InsideBottomLeft);
            else if (Tiler.Up && Tiler.Down && !Tiler.UpLeft && Tiler.UpRight)
                return GetTileID(InsideBottomRight);

            else if (!Tiler.Down)
                return GetTileID(Bottom);
            else
                return GetTileID(Top);
        }

        private int GetTileID(int[] choices)
        {
            if (choices.Length == 0)
                return -1;
            else if (choices.Length == 1)
                return choices[0];
            else
                return Calc.Random.Choose(choices);
        }
    }
}
