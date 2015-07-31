using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
    public class Grid : Collider
    {
        private bool[,] data;

        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }

        public Grid(int cellsX, int cellsY, float cellWidth, float cellHeight)
        {
            data = new bool[cellsX, cellsY];

            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public Grid(float cellWidth, float cellHeight, string bitstring)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;

            //Find minimal size from bitstring
            int longest = 0;
            int currentX = 0;
            int currentY = 1;
            for (int i = 0; i < bitstring.Length; i++)
            {
                if (bitstring[i] == '\n')
                {
                    currentY++;
                    longest = Math.Max(currentX, longest);
                    currentX = 0;
                }
                else if (bitstring[i] == '0' || bitstring[i] == '1')
                    currentX++;
            }

            data = new bool[longest, currentY];
            LoadBitstring(bitstring);
        }

        public Grid(float cellWidth, float cellHeight, bool[,] data)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;

            this.data = (bool[,])data.Clone();
        }

        public void LoadBitstring(string bitstring)
        {
            int x = 0;
            int y = 0;

            for (int i = 0; i < bitstring.Length; i++)
            {
                if (bitstring[i] == '\n')
                {
                    while (x < CellsX)
                    {
                        data[x, y] = false;
                        x++;
                    }

                    x = 0;
                    y++;

                    if (y >= CellsY)
                        return;
                }
                else if (x < CellsX)
                {
                    if (bitstring[i] == '0')
                    {
                        data[x, y] = false;
                        x++;
                    }
                    else if (bitstring[i] == '1')
                    {
                        data[x, y] = true;
                        x++;
                    }
                }
            }
        }

        public string GetBitstring()
        {
            string bits = "";
            for (int y = 0; y < CellsY; y++)
            {
                if (y != 0)
                    bits += "\n";

                for (int x = 0; x < CellsX; x++)
                {
                    if (data[x, y])
                        bits += "1";
                    else
                        bits += "0";
                }
            }

            return bits;
        }

        public void Clear(bool to = false)
        {
            for (int i = 0; i < CellsX; i++)
                for (int j = 0; j < CellsY; j++)
                    data[i, j] = to;
        }

        public void SetRect(int x, int y, int width, int height, bool to = true)
        {
            if (x < 0)
            {
                width += x;
                x = 0;
            }

            if (y < 0)
            {
                height += y;
                y = 0;
            }

            if (x + width > CellsX)
                width = CellsX - x;

            if (y + height > CellsY)
                height = CellsY - y;

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    data[x + i, y + j] = to;
        }

        public bool CheckRect(int x, int y, int width, int height)
        {
            if (x < 0)
            {
                width += x;
                x = 0;
            }

            if (y < 0)
            {
                height += y;
                y = 0;
            }

            if (x + width > CellsX)
                width = CellsX - x;

            if (y + height > CellsY)
                height = CellsY - y;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (data[x + i, y + j])
                        return true;
                }
            }

            return false;
        }

        public bool CheckColumn(int x)
        {
            for (int i = 0; i < CellsY; i++)
                if (!data[x, i])
                    return false;
            return true;
        }

        public bool CheckRow(int y)
        {
            for (int i = 0; i < CellsX; i++)
                if (!data[i, y])
                    return false;
            return true;
        }

        public bool this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 && x < CellsX && y < CellsY)
                    return data[x, y];
                else
                    return false;
            }

            set
            {
                data[x, y] = value;
            }
        }

        public int CellsX
        {
            get
            {
                return data.GetLength(0);
            }
        }

        public int CellsY
        {
            get
            {
                return data.GetLength(1);
            }
        }

        public override float Width
        {
            get
            {
                return CellWidth * CellsX;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override float Height
        {
            get
            {
                return CellHeight * CellsY;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsEmpty
        {
            get
            {
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        if (data[i, j])
                            return false;
                return true;
            }
        }

        public override float Left
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public override float Top
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public override float Right
        {
            get { return Position.X + Width; }
            set { Position.X = value - Width; }
        }

        public override float Bottom
        {
            get { return Position.Y + Height; }
            set { Position.Y = value - Height; }
        }

        public override Collider Clone()
        {
            return new Grid(CellWidth, CellHeight, data);
        }

        public override void Render(Color color)
        {
            for (int i = 0; i < CellsX; i++)
                for (int j = 0; j < CellsY; j++)
                    if (data[i, j])
                        Draw.HollowRect(AbsoluteLeft + i * CellWidth, AbsoluteTop + j * CellHeight, CellWidth, CellHeight, color);
        }

        /*
         *  Checking against other colliders
         */

        override public bool Collide(Vector2 point)
        {
            if (point.X >= AbsoluteLeft && point.Y >= AbsoluteTop && point.X < AbsoluteRight && point.Y < AbsoluteBottom)
                return data[(int)((point.X - AbsoluteLeft) / CellWidth), (int)((point.Y - AbsoluteTop) / CellHeight)];
            else
                return false;
        }

        public override bool Collide(Rectangle rect)
        {
            if (rect.Intersects(Bounds))
            {
                int x = (int)((rect.Left - AbsoluteLeft) / CellWidth);
                int y = (int)((rect.Top - AbsoluteTop) / CellHeight);
                int w = (int)((rect.Right - AbsoluteLeft - 1) / CellWidth) - x + 1;
                int h = (int)((rect.Bottom - AbsoluteTop - 1) / CellHeight) - y + 1;

                return CheckRect(x, y, w, h);
            }
            else
                return false;
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            from -= AbsolutePosition;
            to -= AbsolutePosition;
            from /= new Vector2(CellWidth, CellHeight);
            to /= new Vector2(CellWidth, CellHeight);

            bool steep = Math.Abs(to.Y - from.Y) > Math.Abs(to.X - from.X);
            if (steep)
            {
                float temp = from.X;
                from.X = from.Y;
                from.Y = temp;

                temp = to.X;
                to.X = to.Y;
                to.Y = temp;
            }
            if (from.X > to.X)
            {
                Vector2 temp = from;
                from = to;
                to = temp;
            }

            float error = 0;
            float deltaError = Math.Abs(to.Y - from.Y) / (to.X - from.X);

            int yStep = (from.Y < to.Y) ? 1 : -1;
            int y = (int)from.Y;
            int toX = (int)to.X;

            for (int x = (int)from.X; x <= toX; x++)
            {
                if (steep)
                {
                    if (this[y, x])
                        return true;
                }
                else
                {
                    if (this[x, y])
                        return true;
                }

                error += deltaError;
                if (error >= .5f)
                {
                    y += yStep;
                    error -= 1.0f;
                }
            }

            return false;
        }

        public override bool Collide(Hitbox hitbox)
        {
            return Collide(hitbox.Bounds);
        }

        public override bool Collide(Grid grid)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Circle circle)
        {
            return false;
        }

        public override bool Collide(ColliderList list)
        {
            return list.Collide(this);
        }

        /*
         *  Static utilities
         */

        static public bool IsBitstringEmpty(string bitstring)
        {
            for (int i = 0; i < bitstring.Length; i++)
            {
                if (bitstring[i] == '1')
                    return false;
            }

            return true;
        }
    }
}
