using Microsoft.Xna.Framework;
using System;

namespace Monocle
{
    public class Grid : Collider
    {
        public VirtualMap<bool> Data;

        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }

        public Grid(int cellsX, int cellsY, float cellWidth, float cellHeight)
        {
            Data = new VirtualMap<bool>(cellsX, cellsY);

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
                else
                    currentX++;
            }

            Data = new VirtualMap<bool>(longest, currentY);
            LoadBitstring(bitstring);
        }

        public Grid(float cellWidth, float cellHeight, bool[,] data)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;

            this.Data = new VirtualMap<bool>(data);
        }

        public Grid(float cellWidth, float cellHeight, VirtualMap<bool> data)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;

            this.Data = data;
        }

        public void Extend(int left, int right, int up, int down)
        {
            Position -= new Vector2(left * CellWidth, up * CellHeight);

            int newWidth = Data.Columns + left + right;
            int newHeight = Data.Rows + up + down;
            if (newWidth <= 0 || newHeight <= 0)
            {
                Data = new VirtualMap<bool>(0, 0);
                return;
            }

            var newData = new VirtualMap<bool>(newWidth, newHeight);

            //Center
            for (int x = 0; x < Data.Columns; x++)
            {
                for (int y = 0; y < Data.Rows; y++)
                {
                    int atX = x + left;
                    int atY = y + up;

                    if (atX >= 0 && atX < newWidth && atY >= 0 && atY < newHeight)
                        newData[atX, atY] = Data[x, y];
                }
            }

            //Left
            for (int x = 0; x < left; x++)
                for (int y = 0; y < newHeight; y++)
                    newData[x, y] = Data[0, Calc.Clamp(y - up, 0, Data.Rows - 1)];

            //Right
            for (int x = newWidth - right; x < newWidth; x++)
                for (int y = 0; y < newHeight; y++)
                    newData[x, y] = Data[Data.Columns - 1, Calc.Clamp(y - up, 0, Data.Rows - 1)];

            //Top
            for (int y = 0; y < up; y++)
                for (int x = 0; x < newWidth; x++)
                    newData[x, y] = Data[Calc.Clamp(x - left, 0, Data.Columns - 1), 0];

            //Bottom
            for (int y = newHeight - down; y < newHeight; y++)
                for (int x = 0; x < newWidth; x++)
                    newData[x, y] = Data[Calc.Clamp(x - left, 0, Data.Columns - 1), Data.Rows - 1];

            Data = newData;
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
                        Data[x, y] = false;
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
                        Data[x, y] = false;
                        x++;
                    }
                    else
                    {
                        Data[x, y] = true;
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
                    if (Data[x, y])
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
                    Data[i, j] = to;
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
                    Data[x + i, y + j] = to;
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
                    if (Data[x + i, y + j])
                        return true;
                }
            }

            return false;
        }

        public bool CheckColumn(int x)
        {
            for (int i = 0; i < CellsY; i++)
                if (!Data[x, i])
                    return false;
            return true;
        }

        public bool CheckRow(int y)
        {
            for (int i = 0; i < CellsX; i++)
                if (!Data[i, y])
                    return false;
            return true;
        }

        public bool this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 && x < CellsX && y < CellsY)
                    return Data[x, y];
                else
                    return false;
            }

            set
            {
                Data[x, y] = value;
            }
        }

        public int CellsX
        {
            get
            {
                return Data.Columns;
            }
        }

        public int CellsY
        {
            get
            {
                return Data.Rows;
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
                        if (Data[i, j])
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
            return new Grid(CellWidth, CellHeight, Data.Clone());
        }

        public override void Render(Camera camera, Color color)
        {
            if (camera == null)
            {
                for (int i = 0; i < CellsX; i++)
                    for (int j = 0; j < CellsY; j++)
                        if (Data[i, j])
                            Draw.HollowRect(AbsoluteLeft + i * CellWidth, AbsoluteTop + j * CellHeight, CellWidth, CellHeight, color);
            }
            else
            {
                int left = (int)Math.Max(0, ((camera.Left - AbsoluteLeft) / CellWidth));
                int right = (int)Math.Min(CellsX - 1, Math.Ceiling((camera.Right - AbsoluteLeft) / CellWidth));
                int top = (int)Math.Max(0, ((camera.Top - AbsoluteTop) / CellHeight));
                int bottom = (int)Math.Min(CellsY - 1, Math.Ceiling((camera.Bottom - AbsoluteTop) / CellHeight));

                for (int tx = left; tx <= right; tx++)
                    for (int ty = top; ty <= bottom; ty++)
                        if (Data[tx, ty])
                            Draw.HollowRect(AbsoluteLeft + tx * CellWidth, AbsoluteTop + ty * CellHeight, CellWidth, CellHeight, color);
            }
        }

        /*
         *  Checking against other colliders
         */

        override public bool Collide(Vector2 point)
        {
            if (point.X >= AbsoluteLeft && point.Y >= AbsoluteTop && point.X < AbsoluteRight && point.Y < AbsoluteBottom)
                return Data[(int)((point.X - AbsoluteLeft) / CellWidth), (int)((point.Y - AbsoluteTop) / CellHeight)];
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

        public static bool IsBitstringEmpty(string bitstring)
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
