using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Monocle
{
    static public class Calc
    {
        #region Enums

        static public int EnumLength(Type e)
        {
            return Enum.GetNames(e).Length;
        }

        static public T StringToEnum<T>(string str) where T : struct
        {
            if (Enum.IsDefined(typeof(T), str))
                return (T)Enum.Parse(typeof(T), str);
            else
                throw new Exception("The string cannot be converted to the enum type.");
        }

        static public T[] StringsToEnums<T>(string[] strs) where T : struct
        {
            T[] ret = new T[strs.Length];
            for (int i = 0; i < strs.Length; i++)
                ret[i] = StringToEnum<T>(strs[i]);
            return ret;
        }

        static public bool EnumHasString<T>(string str) where T : struct
        {
            return Enum.IsDefined(typeof(T), str);
        }

        #endregion

        #region Strings

        static public bool StartsWith(this string str, string match)
        {
            return str.IndexOf(match) == 0;
        }

        static public bool EndsWith(this string str, string match)
        {
            return str.LastIndexOf(match) == str.Length - match.Length;
        }

        static public string ToString(this int num, int minDigits)
        {
            string ret = num.ToString();
            while (ret.Length < minDigits)
                ret = "0" + ret;
            return ret;
        }

        static public string[] SplitLines(string text, SpriteFont font, int maxLineWidth, char newLine = '\n')
        {
            List<string> lines = new List<string>();

            foreach (var forcedLine in text.Split(newLine))
            {
                string line = "";

                foreach (string word in forcedLine.Split(' '))
                {
                    if (font.MeasureString(line + " " + word).X > maxLineWidth)
                    {
                        lines.Add(line);
                        line = word;
                    }
                    else
                    {
                        if (line != "")
                            line += " ";
                        line += word;
                    }
                }

                lines.Add(line);
            }

            return lines.ToArray();
        }

        #endregion

        #region Count

        static public int Count<T>(T target, T a, T b)
        {
            int num = 0;

            if (a.Equals(target))
                num++;
            if (b.Equals(target))
                num++;

            return num;
        }

        static public int Count<T>(T target, T a, T b, T c)
        {
            int num = 0;

            if (a.Equals(target))
                num++;
            if (b.Equals(target))
                num++;
            if (c.Equals(target))
                num++;

            return num;
        }

        static public int Count<T>(T target, T a, T b, T c, T d)
        {
            int num = 0;

            if (a.Equals(target))
                num++;
            if (b.Equals(target))
                num++;
            if (c.Equals(target))
                num++;
            if (d.Equals(target))
                num++;

            return num;
        }

        static public int Count<T>(T target, T a, T b, T c, T d, T e)
        {
            int num = 0;

            if (a.Equals(target))
                num++;
            if (b.Equals(target))
                num++;
            if (c.Equals(target))
                num++;
            if (d.Equals(target))
                num++;
            if (e.Equals(target))
                num++;

            return num;
        }

        static public int Count<T>(T target, T a, T b, T c, T d, T e, T f)
        {
            int num = 0;

            if (a.Equals(target))
                num++;
            if (b.Equals(target))
                num++;
            if (c.Equals(target))
                num++;
            if (d.Equals(target))
                num++;
            if (e.Equals(target))
                num++;
            if (f.Equals(target))
                num++;

            return num;
        }

        #endregion

        #region Give Me

        static public T GiveMe<T>(int index, T a, T b)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
            }
        }

        static public T GiveMe<T>(int index, T a, T b, T c)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
            }
        }

        static public T GiveMe<T>(int index, T a, T b, T c, T d)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
            }
        }

        static public T GiveMe<T>(int index, T a, T b, T c, T d, T e)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
                case 4:
                    return e;
            }
        }

        static public T GiveMe<T>(int index, T a, T b, T c, T d, T e, T f)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");

                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
                case 4:
                    return e;
                case 5:
                    return f;
            }
        }

        #endregion

        #region Random

        static public Random Random = new Random();
        static private Stack<Random> randomStack = new Stack<Random>();

        static public void PushRandom(int newSeed)
        {
            randomStack.Push(Calc.Random);
            Calc.Random = new Random(newSeed);
        }

        static public void PushRandom(Random random)
        {
            randomStack.Push(Calc.Random);
            Calc.Random = random;
        }

        static public void PushRandom()
        {
            randomStack.Push(Calc.Random);
            Calc.Random = new Random();
        }

        static public void PopRandom()
        {
            Calc.Random = randomStack.Pop();
        }

        #region Choose

        static public T Choose<T>(this Random random, T a, T b)
        {
            return GiveMe<T>(random.Next(2), a, b);
        }

        static public T Choose<T>(this Random random, T a, T b, T c)
        {
            return GiveMe<T>(random.Next(3), a, b, c);
        }

        static public T Choose<T>(this Random random, T a, T b, T c, T d)
        {
            return GiveMe<T>(random.Next(4), a, b, c, d);
        }

        static public T Choose<T>(this Random random, T a, T b, T c, T d, T e)
        {
            return GiveMe<T>(random.Next(5), a, b, c, d, e);
        }

        static public T Choose<T>(this Random random, T a, T b, T c, T d, T e, T f)
        {
            return GiveMe<T>(random.Next(6), a, b, c, d, e, f);
        }

        static public T Choose<T>(this Random random, params T[] choices)
        {
            return choices[random.Next(choices.Length)];
        }

        #endregion

        #region Range

        /// <summary>
        /// Returns a random integer between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static public int Range(this Random random, int min, int max)
        {
            return min + random.Next(max - min);
        }

        /// <summary>
        /// Returns a random float between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static public float Range(this Random random, float min, float max)
        {
            return min + random.NextFloat(max - min);
        }

        /// <summary>
        /// Returns a random Vector2, and x- and y-values of which are between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static public Vector2 Range(this Random random, Vector2 min, Vector2 max)
        {
            return min + new Vector2(random.NextFloat(max.X - min.X), random.NextFloat(max.Y - min.Y));
        }

        #endregion

        static public bool Chance(this Random random, float chance)
        {
            return random.NextFloat() < chance;
        }

        static public float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        static public float NextFloat(this Random random, float max)
        {
            return random.NextFloat() * max;
        }

        static public float NextAngle(this Random random)
        {
            return random.NextFloat() * MathHelper.TwoPi;
        }

        #endregion

        #region Lists

        static public void Shuffle<T>(this List<T> list, Random random)
        {
            int i = list.Count;
            int j;
            T t;

            while (--i > 0)
            {
                t = list[i];
                list[i] = list[j = random.Next(i + 1)];
                list[j] = t;
            }
        }

        static public void Shuffle<T>(this List<T> list)
        {
            list.Shuffle(Random);
        }

        static public void ShuffleSetFirst<T>(this List<T> list, Random random, T first)
        {
            int amount = 0;
            while (list.Contains(first))
            {
                list.Remove(first);
                amount++;
            }

            list.Shuffle(random);

            for (int i = 0; i < amount; i++)
                list.Insert(0, first);
        }

        static public void ShuffleSetFirst<T>(this List<T> list, T first)
        {
            list.ShuffleSetFirst(Random, first);
        }

        static public void ShuffleNotFirst<T>(this List<T> list, Random random, T notFirst)
        {
            int amount = 0;
            while (list.Contains(notFirst))
            {
                list.Remove(notFirst);
                amount++;
            }

            list.Shuffle(random);

            for (int i = 0; i < amount; i++)
                list.Insert(random.Next(list.Count - 1) + 1, notFirst);
        }

        static public void ShuffleNotFirst<T>(this List<T> list, T notFirst)
        {
            list.ShuffleNotFirst<T>(Random, notFirst);
        }

        #endregion

        #region Colors

        static public Color Invert(this Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B, color.A);
        }

        static public Color HexToColor(string hex)
        {
            float r = (HexToByte(hex[0]) * 16 + HexToByte(hex[1])) / 255.0f;
            float g = (HexToByte(hex[2]) * 16 + HexToByte(hex[3])) / 255.0f;
            float b = (HexToByte(hex[4]) * 16 + HexToByte(hex[5])) / 255.0f;

            return new Color(r, g, b);
        }

        #endregion

        #region Math

        public const float RIGHT = 0;
        public const float UP = MathHelper.Pi * -.5f;
        public const float LEFT = MathHelper.Pi;
        public const float DOWN = MathHelper.Pi * .5f;
        public const float UP_RIGHT = MathHelper.Pi * -.25f;
        public const float UP_LEFT = MathHelper.Pi * -.75f;
        public const float DOWN_RIGHT = MathHelper.Pi * .25f;
        public const float DOWN_LEFT = MathHelper.Pi * .75f;
        public const float DEG_TO_RAD = (float)Math.PI / 180f;
        public const float RAD_TO_DEG = 180f / (float)Math.PI;
        public const float DtR = DEG_TO_RAD;
        public const float RtD = RAD_TO_DEG;
        private const string HEX = "0123456789ABCDEF";

        static public byte HexToByte(char c)
        {
            return (byte)HEX.IndexOf(char.ToUpper(c));
        }

        static public float Percent(float num, float zeroAt, float oneAt)
        {
            return MathHelper.Clamp((num - zeroAt) / oneAt, 0, 1);
        }

        static public float SignThreshold(float value, float threshold)
        {
            if (Math.Abs(value) >= threshold)
                return Math.Sign(value);
            else
                return 0;
        }

        static public float Min(params float[] values)
        {
            float min = values[0];
            for (int i = 1; i < values.Length; i++)
                min = MathHelper.Min(values[i], min);
            return min;
        }

        static public float Max(params float[] values)
        {
            float max = values[0];
            for (int i = 1; i < values.Length; i++)
                max = MathHelper.Max(values[i], max);
            return max;
        }

        static public float ToRad(this float f)
        {
            return f * DEG_TO_RAD;
        }

        static public float ToDeg(this float f)
        {
            return f * RAD_TO_DEG;
        }

        static public int Axis(bool negative, bool positive, int both = 0)
        {
            if (negative)
            {
                if (positive)
                    return both;
                else
                    return -1;
            }
            else if (positive)
                return 1;
            else
                return 0;
        }

        static public int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        static public float YoYo(float value)
        {
            if (value <= .5f)
                return value * 2;
            else
                return 1 - ((value - .5f) * 2);
        }

        static public float LerpSnap(float value1, float value2, float amount, float snapThreshold = .1f)
        {
            float ret = MathHelper.Lerp(value1, value2, amount);
            if (Math.Abs(ret - value2) < snapThreshold)
                return value2;
            else
                return ret;
        }

        static public Vector2 LerpSnap(Vector2 value1, Vector2 value2, float amount, float snapThresholdSq = .1f)
        {
            Vector2 ret = Vector2.Lerp(value1, value2, amount);
            if ((ret - value2).LengthSquared() < snapThresholdSq)
                return value2;
            else
                return ret;
        }

        static public Vector2 SafeNormalize(this Vector2 vec)
        {
            return SafeNormalize(vec, Vector2.Zero);
        }

        static public Vector2 SafeNormalize(this Vector2 vec, float length)
        {
            return SafeNormalize(vec, Vector2.Zero, length);
        }

        static public Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero)
        {
            if (vec == Vector2.Zero)
                return ifZero;
            else
            {
                vec.Normalize();
                return vec;
            }
        }

        static public Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero, float length)
        {
            if (vec == Vector2.Zero)
                return ifZero * length;
            else
            {
                vec.Normalize();
                return vec * length;
            }
        }

        static public float ReflectAngle(float angle, float axis = 0)
        {
            return -(angle + axis) - axis;
        }

        static public float ReflectAngle(float angleRadians, Vector2 axis)
        {
            return ReflectAngle(angleRadians, axis.Angle());
        }

        static public Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
        {
            Vector2 v = lineB - lineA;
            Vector2 w = closestTo - lineA;
            float t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }

        static public Vector2 Round(Vector2 vec)
        {
            return new Vector2((float)Math.Round(vec.X), (float)Math.Round(vec.Y));
        }

        static public float Snap(float value, float increment)
        {
            return (float)Math.Round(value / increment) * increment;
        }

        static public float Snap(float value, float increment, float offset)
        {
            return ((float)Math.Round((value - offset) / increment) * increment) + offset;
        }

        static public float WrapAngleDeg(float angleDegrees)
        {
            return (((angleDegrees * Math.Sign(angleDegrees) + 180) % 360) - 180) * Math.Sign(angleDegrees);
        }

        static public float WrapAngle(float angleRadians)
        {
            return (((angleRadians * Math.Sign(angleRadians) + MathHelper.Pi) % (MathHelper.Pi * 2)) - MathHelper.Pi) * Math.Sign(angleRadians);
        }

        static public Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }

        static public float AngleApproach(float val, float target, float maxMove)
        {
            return val + MathHelper.Clamp(AngleDiff(val, target), -maxMove, maxMove);
        }

        static public float AngleLerp(float startAngle, float endAngle, float percent)
        {
            return startAngle + AngleDiff(startAngle, endAngle) * percent;
        }

        static public float Approach(float val, float target, float maxMove)
        {
            return val > target ? Math.Max(val - maxMove, target) : Math.Min(val + maxMove, target);
        }

        static public float AngleDiff(float radiansA, float radiansB)
        {
            float diff = radiansB - radiansA;

            while (diff > MathHelper.Pi) { diff -= MathHelper.TwoPi; }
            while (diff <= -MathHelper.Pi) { diff += MathHelper.TwoPi; }

            return diff;
        }

        static public float AbsAngleDiff(float radiansA, float radiansB)
        {
            return Math.Abs(AngleDiff(radiansA, radiansB));
        }

        static public float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        static public Color ToggleColors(Color current, Color a, Color b)
        {
            if (current == a)
                return b;
            else
                return a;
        }

        static public float ShorterAngleDifference(float currentAngle, float angleA, float angleB)
        {
            if (Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < Math.Abs(Calc.AngleDiff(currentAngle, angleB)))
                return angleA;
            else
                return angleB;
        }

        static public float ShorterAngleDifference(float currentAngle, float angleA, float angleB, float angleC)
        {
            if (Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < Math.Abs(Calc.AngleDiff(currentAngle, angleB)))
                return ShorterAngleDifference(currentAngle, angleA, angleC);
            else
                return ShorterAngleDifference(currentAngle, angleB, angleC);
        }

        static public bool IsInRange<T>(this T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        static public bool IsInRange<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        static public T[] Array<T>(params T[] items)
        {
            return items;
        }

        static public T[] VerifyLength<T>(this T[] array, int length)
        {
            if (array == null)
                return new T[length];
            else if (array.Length != length)
            {
                T[] newArray = new T[length];
                for (int i = 0; i < Math.Min(length, array.Length); i++)
                    newArray[i] = array[i];
                return newArray;
            }
            else
                return array;
        }

        static public T[][] VerifyLength<T>(this T[][] array, int length0, int length1)
        {
            array = VerifyLength<T[]>(array, length0);
            for (int i = 0; i < array.Length; i++)
                array[i] = VerifyLength<T>(array[i], length1);
            return array;
        }

        #endregion

        #region Vector2

        static public Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        static public float Angle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        static public Vector2 Clamp(this Vector2 val, float minX, float minY, float maxX, float maxY)
        {
            return new Vector2(MathHelper.Clamp(val.X, minX, maxX), MathHelper.Clamp(val.Y, minY, maxY));
        }

        static public Vector2 Floor(this Vector2 val)
        {
            return new Vector2((int)val.X, (int)val.Y);
        }

        static public Vector2 Ceiling(this Vector2 val)
        {
            return new Vector2((int)Math.Ceiling(val.X), (int)Math.Ceiling(val.Y));
        }

        static public Vector2 Abs(this Vector2 val)
        {
            return new Vector2(Math.Abs(val.X), Math.Abs(val.Y));
        }

        static public Vector2 Approach(Vector2 val, Vector2 target, float maxMove)
        {
            if (maxMove == 0 || val == target)
                return val;

            Vector2 diff = target - val;
            float length = diff.Length();

            if (length < maxMove)
                return target;
            else
            {
                diff.Normalize();
                return val + diff * maxMove;
            }
        }

        static public Vector2 FourWayNormal(this Vector2 vec)
        {
            if (vec == Vector2.Zero)
                return Vector2.Zero;

            float angle = vec.Angle();
            angle = (float)Math.Floor((angle + MathHelper.PiOver2 / 2f) / MathHelper.PiOver2) * MathHelper.PiOver2;

            vec = AngleToVector(angle, 1f);
            if (Math.Abs(vec.X) < .5f)
                vec.X = 0;
            else
                vec.X = Math.Sign(vec.X);

            if (Math.Abs(vec.Y) < .5f)
                vec.Y = 0;
            else
                vec.Y = Math.Sign(vec.Y);

            return vec;
        }

        static public Vector2 EightWayNormal(this Vector2 vec)
        {
            if (vec == Vector2.Zero)
                return Vector2.Zero;

            float angle = vec.Angle();
            angle = (float)Math.Floor((angle + MathHelper.PiOver4 / 2f) / MathHelper.PiOver4) * MathHelper.PiOver4;

            vec = AngleToVector(angle, 1f);
            if (Math.Abs(vec.X) < .5f)
                vec.X = 0;
            else if (Math.Abs(vec.Y) < .5f)
                vec.Y = 0;

            return vec;
        }

        static public Vector2 SnappedNormal(this Vector2 vec, float slices)
        {
            float divider = MathHelper.TwoPi / slices;

            float angle = vec.Angle();
            angle = (float)Math.Floor((angle + divider / 2f) / divider) * divider;
            return AngleToVector(angle, 1f);
        }

        static public Vector2 Rotate(this Vector2 vec, float angleRadians)
        {
            return AngleToVector(vec.Angle() + angleRadians, vec.Length());
        }

        static public Vector2 XComp(this Vector2 vec)
        {
            return Vector2.UnitX * vec.X;
        }

        static public Vector2 YComp(this Vector2 vec)
        {
            return Vector2.UnitY * vec.Y;
        }

        #endregion

        #region CSV

        static public int[,] ReadCSVIntGrid(string csv, int width, int height)
        {
            int[,] data = new int[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    data[x, y] = -1;

            string[] lines = csv.Split('\n');
            for (int y = 0; y < height && y < lines.Length; y++)
            {
                string[] line = lines[y].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < width && x < line.Length; x++)
                    data[x, y] = Convert.ToInt32(line[x]);
            }

            return data;
        }

        static public int[] ReadCSVInt(string csv)
        {
            if (csv == "")
                return new int[0];

            string[] values = csv.Split(',');
            int[] ret = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
                ret[i] = Convert.ToInt32(values[i].Trim());

            return ret;
        }

        static public string[] ReadCSV(string csv)
        {
            if (csv == "")
                return new string[0];

            string[] values = csv.Split(',');
            for (int i = 0; i < values.Length; i++)
                values[i] = values[i].Trim();

            return values;
        }

        static public string IntGridToCSV(int[,] data)
        {
            StringBuilder str = new StringBuilder();

            List<int> line = new List<int>();
            int newLines = 0;

            for (int y = 0; y < data.GetLength(1); y++)
            {
                int empties = 0;

                for (int x = 0; x < data.GetLength(0); x++)
                {
                    if (data[x, y] == -1)
                        empties++;
                    else
                    {
                        for (int i = 0; i < newLines; i++)
                            str.Append('\n');
                        for (int i = 0; i < empties; i++)
                            line.Add(-1);
                        empties = newLines = 0;

                        line.Add(data[x, y]);
                    }
                }

                if (line.Count > 0)
                {
                    str.Append(string.Join(",", line));
                    line.Clear();
                }

                newLines++;
            }

            return str.ToString();
        }

        #endregion

        #region Data Parse 

        static public bool[,] GetBitData(string data, char rowSep = '\n')
        {
            int lengthX = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == '1' || data[i] == '0')
                    lengthX++;
                else if (data[i] == rowSep)
                    break;
            }

            int lengthY = data.Count(c => c == '\n') + 1;

            bool[,] bitData = new bool[lengthX, lengthY];
            int x = 0;
            int y = 0;
            for (int i = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case '1':
                        bitData[x, y] = true;
                        x++;
                        break;

                    case '0':
                        bitData[x, y] = false;
                        x++;
                        break;

                    case '\n':
                        x = 0;
                        y++;
                        break;

                    default:
                        break;
                }
            }

            return bitData;
        }

        static public void CombineBitData(bool[,] combineInto, string data, char rowSep = '\n')
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case '1':
                        combineInto[x, y] = true;
                        x++;
                        break;

                    case '0':
                        x++;
                        break;

                    case '\n':
                        x = 0;
                        y++;
                        break;

                    default:
                        break;
                }
            }
        }

        static public void CombineBitData(bool[,] combineInto, bool[,] data)
        {
            for (int i = 0; i < combineInto.GetLength(0); i++)
                for (int j = 0; j < combineInto.GetLength(1); j++)
                    if (data[i, j])
                        combineInto[i, j] = true;
        }

        static public int[] ConvertStringArrayToIntArray(string[] strings)
        {
            int[] ret = new int[strings.Length];
            for (int i = 0; i < strings.Length; i++)
                ret[i] = Convert.ToInt32(strings[i]);
            return ret;
        }

        static public float[] ConvertStringArrayToFloatArray(string[] strings)
        {
            float[] ret = new float[strings.Length];
            for (int i = 0; i < strings.Length; i++)
                ret[i] = Convert.ToSingle(strings[i], CultureInfo.InvariantCulture);
            return ret;
        }

        #endregion

        #region Save and Load Data

        static public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        static public bool SaveFile<T>(T obj, string filename) where T : new()
        {
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                stream.Close();
                return true;
            }
            catch
            {
                stream.Close();
                return false;
            }
        }

        static public bool LoadFile<T>(string filename, ref T data) where T : new()
        {
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T obj = (T)serializer.Deserialize(stream);
                stream.Close();
                data = obj;
                return true;
            }
            catch
            {
                stream.Close();
                return false;
            }
        }

        #endregion

        #region XML

        static public XmlDocument LoadXML(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Engine.Instance.Content.RootDirectory + filename);
            return xml;
        }

        static public bool XMLExists(string filename)
        {
            return File.Exists(Engine.Instance.Content.RootDirectory + filename);
        }

        #region Attributes

        static public bool HasAttr(this XmlElement xml, string attributeName)
        {
            return xml.Attributes[attributeName] != null;
        }

        static public string Attr(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return xml.Attributes[attributeName].InnerText;
        }

        static public string Attr(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.Attributes[attributeName].InnerText;
        }

        static public int AttrInt(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        static public int AttrInt(this XmlElement xml, string attributeName, int defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        static public float AttrFloat(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        static public float AttrFloat(this XmlElement xml, string attributeName, float defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        static public bool AttrBool(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToBoolean(xml.Attributes[attributeName].InnerText);
        }

        static public bool AttrBool(this XmlElement xml, string attributeName, bool defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrBool(xml, attributeName);
        }

        static public T AttrEnum<T>(this XmlElement xml, string attributeName) where T : struct
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            if (Enum.IsDefined(typeof(T), xml.Attributes[attributeName].InnerText))
                return (T)Enum.Parse(typeof(T), xml.Attributes[attributeName].InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        static public T AttrEnum<T>(this XmlElement xml, string attributeName, T defaultValue) where T : struct
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.AttrEnum<T>(attributeName);
        }

        static public Color AttrHexColor(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Calc.HexToColor(xml.Attr(attributeName));
        }

        static public Color AttrHexColor(this XmlElement xml, string attributeName, Color defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrHexColor(xml, attributeName);
        }

        static public Color AttrHexColor(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return Calc.HexToColor(defaultValue);
            else
                return AttrHexColor(xml, attributeName);
        }

        static public Vector2 Position(this XmlElement xml)
        {
            return new Vector2(xml.AttrFloat("x"), xml.AttrFloat("y"));
        }

        static public Vector2 Position(this XmlElement xml, Vector2 defaultPosition)
        {
            return new Vector2(xml.AttrFloat("x", defaultPosition.X), xml.AttrFloat("y", defaultPosition.Y));
        }

        static public float X(this XmlElement xml)
        {
            return xml.AttrFloat("x");
        }

        static public float X(this XmlElement xml, float defaultX)
        {
            return xml.AttrFloat("x", defaultX);
        }

        static public float Y(this XmlElement xml)
        {
            return xml.AttrFloat("y");
        }

        static public float Y(this XmlElement xml, float defaultY)
        {
            return xml.AttrFloat("y", defaultY);
        }

        static public int Width(this XmlElement xml)
        {
            return xml.AttrInt("width");
        }

        static public int Width(this XmlElement xml, int defaultWidth)
        {
            return xml.AttrInt("width", defaultWidth);
        }

        static public int Height(this XmlElement xml)
        {
            return xml.AttrInt("height");
        }

        static public int Height(this XmlElement xml, int defaultHeight)
        {
            return xml.AttrInt("height", defaultHeight);
        }

        #endregion

        #region Inner Text

        static public int InnerInt(this XmlElement xml)
        {
            return Convert.ToInt32(xml.InnerText);
        }

        static public float InnerFloat(this XmlElement xml)
        {
            return Convert.ToSingle(xml.InnerText, CultureInfo.InvariantCulture);
        }

        static public bool InnerBool(this XmlElement xml)
        {
            return Convert.ToBoolean(xml.InnerText);
        }

        static public T InnerEnum<T>(this XmlElement xml) where T : struct
        {
            if (Enum.IsDefined(typeof(T), xml.InnerText))
                return (T)Enum.Parse(typeof(T), xml.InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        static public Color InnerHexColor(this XmlElement xml)
        {
            return Calc.HexToColor(xml.InnerText);
        }

        #endregion

        #region Child Inner Text

        static public bool HasChild(this XmlElement xml, string childName)
        {
            return xml[childName] != null;
        }

        static public string ChildText(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerText;
        }

        static public string ChildText(this XmlElement xml, string childName, string defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerText;
            else
                return defaultValue;
        }

        static public int ChildInt(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerInt();
        }

        static public int ChildInt(this XmlElement xml, string childName, int defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerInt();
            else
                return defaultValue;
        }

        static public float ChildFloat(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerFloat();
        }

        static public float ChildFloat(this XmlElement xml, string childName, float defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerFloat();
            else
                return defaultValue;
        }

        static public bool ChildBool(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerBool();
        }

        static public bool ChildBool(this XmlElement xml, string childName, bool defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerBool();
            else
                return defaultValue;
        }

        static public T ChildEnum<T>(this XmlElement xml, string childName) where T : struct
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            if (Enum.IsDefined(typeof(T), xml[childName].InnerText))
                return (T)Enum.Parse(typeof(T), xml[childName].InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        static public T ChildEnum<T>(this XmlElement xml, string childName, T defaultValue) where T : struct
        {
            if (xml.HasChild(childName))
            {
                if (Enum.IsDefined(typeof(T), xml[childName].InnerText))
                    return (T)Enum.Parse(typeof(T), xml[childName].InnerText);
                else
                    throw new Exception("The attribute value cannot be converted to the enum type.");
            }
            else
                return defaultValue;
        }

        static public Color ChildHexColor(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return Calc.HexToColor(xml[childName].InnerText);
        }

        static public Color ChildHexColor(this XmlElement xml, string childName, Color defaultValue)
        {
            if (xml.HasChild(childName))
                return Calc.HexToColor(xml[childName].InnerText);
            else
                return defaultValue;
        }

        static public Color ChildHexColor(this XmlElement xml, string childName, string defaultValue)
        {
            if (xml.HasChild(childName))
                return Calc.HexToColor(xml[childName].InnerText);
            else
                return Calc.HexToColor(defaultValue);
        }

        static public Vector2 ChildPosition(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].Position();
        }

        static public Vector2 ChildPosition(this XmlElement xml, string childName, Vector2 defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].Position(defaultValue);
            else
                return defaultValue;
        }

        #endregion

        #region Ogmo Nodes

        static public Vector2 FirstNode(this XmlElement xml)
        {
            if (xml["node"] == null)
                return Vector2.Zero;
            else
                return new Vector2(xml["node"].AttrInt("x"), xml["node"].AttrInt("y"));
        }

        static public Vector2? FirstNodeNullable(this XmlElement xml)
        {
            if (xml["node"] == null)
                return null;
            else
                return new Vector2(xml["node"].AttrInt("x"), xml["node"].AttrInt("y"));
        }

        static public Vector2[] Nodes(this XmlElement xml, bool includePosition = false)
        {
            XmlNodeList nodes = xml.GetElementsByTagName("node");
            if (nodes == null)
                return includePosition ? new Vector2[] { xml.Position() } : new Vector2[0];

            Vector2[] ret;
            if (includePosition)
            {
                ret = new Vector2[nodes.Count + 1];
                ret[0] = xml.Position();
                for (int i = 0; i < nodes.Count; i++)
                    ret[i + 1] = new Vector2(Convert.ToInt32(nodes[i].Attributes["x"].InnerText), Convert.ToInt32(nodes[i].Attributes["y"].InnerText));
            }
            else
            {
                ret = new Vector2[nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                    ret[i] = new Vector2(Convert.ToInt32(nodes[i].Attributes["x"].InnerText), Convert.ToInt32(nodes[i].Attributes["y"].InnerText));
            }

            return ret;
        }

        static public Vector2 GetNode(this XmlElement xml, int nodeNum)
        {
            if (xml.Nodes().Length > nodeNum)
                return xml.Nodes()[nodeNum];
            else
                return Vector2.Zero;
        }

        static public Vector2? GetNodeNullable(this XmlElement xml, int nodeNum)
        {
            if (xml.Nodes().Length > nodeNum)
                return xml.Nodes()[nodeNum];
            else
                return null;
        }

        #endregion

        #region Add Stuff

        static public void SetAttr(this XmlElement xml, string attributeName, Object setTo)
        {
            XmlAttribute attr;

            if (xml.HasAttr(attributeName))
                attr = xml.Attributes[attributeName];
            else
            {
                attr = xml.OwnerDocument.CreateAttribute(attributeName);
                xml.Attributes.Append(attr);
            }

            attr.Value = setTo.ToString();
        }

        static public void SetChild(this XmlElement xml, string childName, Object setTo)
        {
            XmlElement ele;

            if (xml.HasChild(childName))
                ele = xml[childName];
            else
            {
                ele = xml.OwnerDocument.CreateElement(childName);
                xml.AppendChild(ele);
            }

            ele.InnerText = setTo.ToString();
        }

        static public XmlElement CreateChild(this XmlDocument doc, string childName)
        {
            XmlElement ele = doc.CreateElement(childName);
            doc.AppendChild(ele);
            return ele;
        }

        static public XmlElement CreateChild(this XmlElement xml, string childName)
        {
            XmlElement ele = xml.OwnerDocument.CreateElement(childName);
            xml.AppendChild(ele);
            return ele;
        }

        #endregion

        #endregion

        #region Sorting

        static public int SortLeftToRight(Entity a, Entity b)
        {
            return (int)((a.X - b.X) * 100);
        }

        static public int SortRightToLeft(Entity a, Entity b)
        {
            return (int)((b.X - a.X) * 100);
        }

        static public int SortTopToBottom(Entity a, Entity b)
        {
            return (int)((a.Y - b.Y) * 100);
        }

        static public int SortBottomToTop(Entity a, Entity b)
        {
            return (int)((b.Y - a.Y) * 100);
        }

        static public int SortByDepth(Entity a, Entity b)
        {
            return a.Depth - b.Depth;
        }

        static public int SortByDepthReversed(Entity a, Entity b)
        {
            return b.Depth - a.Depth;
        }

        #endregion

        #region Debug

        static public void Log()
        {
            Debug.WriteLine("Log");
        }

        static public void Log(params object[] obj)
        {
            foreach (var o in obj)
            {
                if (o == null)
                    Debug.WriteLine("null");
                else
                    Debug.WriteLine(o.ToString());
            }
        }

        static public void LogEach<T>(IEnumerable<T> collection)
        {
            foreach (var o in collection)
                Debug.WriteLine(o.ToString());
        }

        static public void Dissect(Object obj)
        {
            Debug.Write(obj.GetType().Name + " { ");
            foreach (var v in obj.GetType().GetFields())
                Debug.Write(v.Name + ": " + v.GetValue(obj) + ", ");
            Debug.WriteLine(" }");
        }

        static private Stopwatch stopwatch;

        static public void StartTimer()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        static public void EndTimer()
        {
            if (stopwatch != null)
            {
                stopwatch.Stop();

                string message = "Timer: " + stopwatch.ElapsedTicks + " ticks, or " + TimeSpan.FromTicks(stopwatch.ElapsedTicks).TotalSeconds.ToString("00.0000000") + " seconds";
                Debug.WriteLine(message);
#if DESKTOP && DEBUG
            Commands.Trace(message);
#endif
                stopwatch = null;
            }
        }

        #endregion

        #region Reflection

        static public Delegate GetMethod<T>(Object obj, string method) where T : class
        {
            var info = obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (info == null)
                return null;
            else
                return Delegate.CreateDelegate(typeof(T), obj, method);
        }

        #endregion
    }
}
