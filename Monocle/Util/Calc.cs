using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections;
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
    public static class Calc
    {
        #region Enums

        public static int EnumLength(Type e)
        {
            return Enum.GetNames(e).Length;
        }

        public static T StringToEnum<T>(string str) where T : struct
        {
            if (Enum.IsDefined(typeof(T), str))
                return (T)Enum.Parse(typeof(T), str);
            else
                throw new Exception("The string cannot be converted to the enum type.");
        }

        public static T[] StringsToEnums<T>(string[] strs) where T : struct
        {
            T[] ret = new T[strs.Length];
            for (int i = 0; i < strs.Length; i++)
                ret[i] = StringToEnum<T>(strs[i]);
            return ret;
        }

        public static bool EnumHasString<T>(string str) where T : struct
        {
            return Enum.IsDefined(typeof(T), str);
        }

        #endregion

        #region Strings

        public static bool StartsWith(this string str, string match)
        {
            return str.IndexOf(match) == 0;
        }

        public static bool EndsWith(this string str, string match)
        {
            return str.LastIndexOf(match) == str.Length - match.Length;
        }

        public static bool IsIgnoreCase(this string str, params string[] matches)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            foreach (var match in matches)
                if (str.Equals(match, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        public static string ToString(this int num, int minDigits)
        {
            string ret = num.ToString();
            while (ret.Length < minDigits)
                ret = "0" + ret;
            return ret;
        }

        public static string[] SplitLines(string text, SpriteFont font, int maxLineWidth, char newLine = '\n')
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

        public static int Count<T>(T target, T a, T b)
        {
            int num = 0;

            if (a.Equals(target))
                num++;
            if (b.Equals(target))
                num++;

            return num;
        }

        public static int Count<T>(T target, T a, T b, T c)
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

        public static int Count<T>(T target, T a, T b, T c, T d)
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

        public static int Count<T>(T target, T a, T b, T c, T d, T e)
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

        public static int Count<T>(T target, T a, T b, T c, T d, T e, T f)
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

        public static T GiveMe<T>(int index, T a, T b)
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

        public static T GiveMe<T>(int index, T a, T b, T c)
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

        public static T GiveMe<T>(int index, T a, T b, T c, T d)
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

        public static T GiveMe<T>(int index, T a, T b, T c, T d, T e)
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

        public static T GiveMe<T>(int index, T a, T b, T c, T d, T e, T f)
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

        public static Random Random = new Random();
        private static Stack<Random> randomStack = new Stack<Random>();

        public static void PushRandom(int newSeed)
        {
            randomStack.Push(Calc.Random);
            Calc.Random = new Random(newSeed);
        }

        public static void PushRandom(Random random)
        {
            randomStack.Push(Calc.Random);
            Calc.Random = random;
        }

        public static void PushRandom()
        {
            randomStack.Push(Calc.Random);
            Calc.Random = new Random();
        }

        public static void PopRandom()
        {
            Calc.Random = randomStack.Pop();
        }

        #region Choose

        public static T Choose<T>(this Random random, T a, T b)
        {
            return GiveMe<T>(random.Next(2), a, b);
        }

        public static T Choose<T>(this Random random, T a, T b, T c)
        {
            return GiveMe<T>(random.Next(3), a, b, c);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d)
        {
            return GiveMe<T>(random.Next(4), a, b, c, d);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d, T e)
        {
            return GiveMe<T>(random.Next(5), a, b, c, d, e);
        }

        public static T Choose<T>(this Random random, T a, T b, T c, T d, T e, T f)
        {
            return GiveMe<T>(random.Next(6), a, b, c, d, e, f);
        }

        public static T Choose<T>(this Random random, params T[] choices)
        {
            return choices[random.Next(choices.Length)];
        }

        public static T Choose<T>(this Random random, List<T> choices)
        {
            return choices[random.Next(choices.Count)];
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
        public static int Range(this Random random, int min, int max)
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
        public static float Range(this Random random, float min, float max)
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
        public static Vector2 Range(this Random random, Vector2 min, Vector2 max)
        {
            return min + new Vector2(random.NextFloat(max.X - min.X), random.NextFloat(max.Y - min.Y));
        }

        #endregion

        public static int Facing(this Random random)
        {
            return (random.NextFloat() < 0.5f ? -1 : 1);
        }

        public static bool Chance(this Random random, float chance)
        {
            return random.NextFloat() < chance;
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextFloat(this Random random, float max)
        {
            return random.NextFloat() * max;
        }

        public static float NextAngle(this Random random)
        {
            return random.NextFloat() * MathHelper.TwoPi;
        }

        private static int[] shakeVectorOffsets = new int[] { -1, -1, 0, 1, 1 };

        public static Vector2 ShakeVector(this Random random)
        {
            return new Vector2(random.Choose(shakeVectorOffsets), random.Choose(shakeVectorOffsets));
        }

        #endregion

        #region Lists

        public static Vector2 ClosestTo(this List<Vector2> list, Vector2 to)
        {
            Vector2 best = list[0];
            float distSq = Vector2.DistanceSquared(list[0], to);

            for (int i = 1; i < list.Count; i++)
            {
                float d = Vector2.DistanceSquared(list[i], to);
                if (d < distSq)
                {
                    distSq = d;
                    best = list[i];
                }
            }

            return best;
        }

        public static Vector2 ClosestTo(this Vector2[] list, Vector2 to)
        {
            Vector2 best = list[0];
            float distSq = Vector2.DistanceSquared(list[0], to);

            for (int i = 1; i < list.Length; i++)
            {
                float d = Vector2.DistanceSquared(list[i], to);
                if (d < distSq)
                {
                    distSq = d;
                    best = list[i];
                }
            }

            return best;
        }

        public static Vector2 ClosestTo(this Vector2[] list, Vector2 to, out int index)
        {
            index = 0;
            Vector2 best = list[0];
            float distSq = Vector2.DistanceSquared(list[0], to);

            for (int i = 1; i < list.Length; i++)
            {
                float d = Vector2.DistanceSquared(list[i], to);
                if (d < distSq)
                {
                    index = i;
                    distSq = d;
                    best = list[i];
                }
            }

            return best;
        }

        public static void Shuffle<T>(this List<T> list, Random random)
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

        public static void Shuffle<T>(this List<T> list)
        {
            list.Shuffle(Random);
        }

        public static void ShuffleSetFirst<T>(this List<T> list, Random random, T first)
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

        public static void ShuffleSetFirst<T>(this List<T> list, T first)
        {
            list.ShuffleSetFirst(Random, first);
        }

        public static void ShuffleNotFirst<T>(this List<T> list, Random random, T notFirst)
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

        public static void ShuffleNotFirst<T>(this List<T> list, T notFirst)
        {
            list.ShuffleNotFirst<T>(Random, notFirst);
        }

        #endregion

        #region Colors

        public static Color Invert(this Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B, color.A);
        }

        public static Color HexToColor(string hex)
        {
            if (hex.Length >= 6)
            {
                float r = (HexToByte(hex[0]) * 16 + HexToByte(hex[1])) / 255.0f;
                float g = (HexToByte(hex[2]) * 16 + HexToByte(hex[3])) / 255.0f;
                float b = (HexToByte(hex[4]) * 16 + HexToByte(hex[5])) / 255.0f;
                return new Color(r, g, b);
            }

            return Color.White;
        }

        #endregion

        #region Time

        public static string ShortGameplayFormat(this TimeSpan time)
        {
            if (time.TotalHours >= 1)
                return ((int)time.Hours) + ":" + time.ToString(@"mm\:ss\.fff");
            else
                return time.ToString(@"m\:ss\.fff");
        }

        public static string LongGameplayFormat(this TimeSpan time)
        {
            StringBuilder str = new StringBuilder();

            if (time.TotalDays >= 2)
            {
                str.Append((int)time.TotalDays);
                str.Append(" days, ");
            }
            else if (time.TotalDays >= 1)
                str.Append("1 day, ");

            str.Append((time.TotalHours - ((int)time.TotalDays * 24)).ToString("0.0"));
            str.Append(" hours");

            return str.ToString();
        }

        #endregion

        #region Math

        public const float Right = 0;
        public const float Up = -MathHelper.PiOver2;
        public const float Left = MathHelper.Pi;
        public const float Down = MathHelper.PiOver2;
        public const float UpRight = -MathHelper.PiOver4;
        public const float UpLeft = -MathHelper.PiOver4 - MathHelper.PiOver2;
        public const float DownRight = MathHelper.PiOver4;
        public const float DownLeft = MathHelper.PiOver4 + MathHelper.PiOver2;
        public const float DegToRad = MathHelper.Pi / 180f;
        public const float RadToDeg = 180f / MathHelper.Pi;
        public const float DtR = DegToRad;
        public const float RtD = RadToDeg;
        public const float Circle = MathHelper.TwoPi;
        public const float HalfCircle = MathHelper.Pi;
        public const float QuarterCircle = MathHelper.PiOver2;
        public const float EighthCircle = MathHelper.PiOver4;
        private const string Hex = "0123456789ABCDEF";

        public static int Digits(this int num)
        {
            int digits = 1;
            int target = 10;

            while (num >= target)
            {
                digits++;
                target *= 10;
            }

            return digits;
        }

        public static byte HexToByte(char c)
        {
            return (byte)Hex.IndexOf(char.ToUpper(c));
        }

        public static float Percent(float num, float zeroAt, float oneAt)
        {
            return MathHelper.Clamp((num - zeroAt) / oneAt, 0, 1);
        }

        public static float SignThreshold(float value, float threshold)
        {
            if (Math.Abs(value) >= threshold)
                return Math.Sign(value);
            else
                return 0;
        }

        public static float Min(params float[] values)
        {
            float min = values[0];
            for (int i = 1; i < values.Length; i++)
                min = MathHelper.Min(values[i], min);
            return min;
        }

        public static float Max(params float[] values)
        {
            float max = values[0];
            for (int i = 1; i < values.Length; i++)
                max = MathHelper.Max(values[i], max);
            return max;
        }

        public static float ToRad(this float f)
        {
            return f * DegToRad;
        }

        public static float ToDeg(this float f)
        {
            return f * RadToDeg;
        }

        public static int Axis(bool negative, bool positive, int both = 0)
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

        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float YoYo(float value)
        {
            if (value <= .5f)
                return value * 2;
            else
                return 1 - ((value - .5f) * 2);
        }

        public static float Map(float val, float min, float max, float newMin = 0, float newMax = 1)
        {
            return ((val - min) / (max - min)) * (newMax - newMin) + newMin;
        }

        public static float SineMap(float counter, float newMin, float newMax)
        {
            return Calc.Map((float)Math.Sin(counter), 01, 1, newMin, newMax);
        }

        public static float ClampedMap(float val, float min, float max, float newMin = 0, float newMax = 1)
        {
            return MathHelper.Clamp((val - min) / (max - min), 0, 1) * (newMax - newMin) + newMin;
        }

        public static float LerpSnap(float value1, float value2, float amount, float snapThreshold = .1f)
        {
            float ret = MathHelper.Lerp(value1, value2, amount);
            if (Math.Abs(ret - value2) < snapThreshold)
                return value2;
            else
                return ret;
        }

        public static float LerpClamp(float value1, float value2, float lerp)
        {
            return MathHelper.Lerp(value1, value2, MathHelper.Clamp(lerp, 0, 1));
        }

        public static Vector2 LerpSnap(Vector2 value1, Vector2 value2, float amount, float snapThresholdSq = .1f)
        {
            Vector2 ret = Vector2.Lerp(value1, value2, amount);
            if ((ret - value2).LengthSquared() < snapThresholdSq)
                return value2;
            else
                return ret;
        }


        public static Vector2 Sign(this Vector2 vec)
        {
            return new Vector2(Math.Sign(vec.X), Math.Sign(vec.Y));
        }

        public static Vector2 SafeNormalize(this Vector2 vec)
        {
            return SafeNormalize(vec, Vector2.Zero);
        }

        public static Vector2 SafeNormalize(this Vector2 vec, float length)
        {
            return SafeNormalize(vec, Vector2.Zero, length);
        }

        public static Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero)
        {
            if (vec == Vector2.Zero)
                return ifZero;
            else
            {
                vec.Normalize();
                return vec;
            }
        }

        public static Vector2 SafeNormalize(this Vector2 vec, Vector2 ifZero, float length)
        {
            if (vec == Vector2.Zero)
                return ifZero * length;
            else
            {
                vec.Normalize();
                return vec * length;
            }
        }

        public static float ReflectAngle(float angle, float axis = 0)
        {
            return -(angle + axis) - axis;
        }

        public static float ReflectAngle(float angleRadians, Vector2 axis)
        {
            return ReflectAngle(angleRadians, axis.Angle());
        }

        public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
        {
            Vector2 v = lineB - lineA;
            Vector2 w = closestTo - lineA;
            float t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }

        public static Vector2 Round(this Vector2 vec)
        {
            return new Vector2((float)Math.Round(vec.X), (float)Math.Round(vec.Y));
        }

        public static float Snap(float value, float increment)
        {
            return (float)Math.Round(value / increment) * increment;
        }

        public static float Snap(float value, float increment, float offset)
        {
            return ((float)Math.Round((value - offset) / increment) * increment) + offset;
        }

        public static float WrapAngleDeg(float angleDegrees)
        {
            return (((angleDegrees * Math.Sign(angleDegrees) + 180) % 360) - 180) * Math.Sign(angleDegrees);
        }

        public static float WrapAngle(float angleRadians)
        {
            return (((angleRadians * Math.Sign(angleRadians) + MathHelper.Pi) % (MathHelper.Pi * 2)) - MathHelper.Pi) * Math.Sign(angleRadians);
        }

        public static Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }

        public static float AngleApproach(float val, float target, float maxMove)
        {
            var diff = AngleDiff(val, target);
            if (Math.Abs(diff) < maxMove)
                return target;
            return val + MathHelper.Clamp(diff, -maxMove, maxMove);
        }

        public static float AngleLerp(float startAngle, float endAngle, float percent)
        {
            return startAngle + AngleDiff(startAngle, endAngle) * percent;
        }

        public static float Approach(float val, float target, float maxMove)
        {
            return val > target ? Math.Max(val - maxMove, target) : Math.Min(val + maxMove, target);
        }

        public static float AngleDiff(float radiansA, float radiansB)
        {
            float diff = radiansB - radiansA;

            while (diff > MathHelper.Pi) { diff -= MathHelper.TwoPi; }
            while (diff <= -MathHelper.Pi) { diff += MathHelper.TwoPi; }

            return diff;
        }

        public static float AbsAngleDiff(float radiansA, float radiansB)
        {
            return Math.Abs(AngleDiff(radiansA, radiansB));
        }

        public static int SignAngleDiff(float radiansA, float radiansB)
        {
            return Math.Sign(AngleDiff(radiansA, radiansB));
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static Color ToggleColors(Color current, Color a, Color b)
        {
            if (current == a)
                return b;
            else
                return a;
        }

        public static float ShorterAngleDifference(float currentAngle, float angleA, float angleB)
        {
            if (Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < Math.Abs(Calc.AngleDiff(currentAngle, angleB)))
                return angleA;
            else
                return angleB;
        }

        public static float ShorterAngleDifference(float currentAngle, float angleA, float angleB, float angleC)
        {
            if (Math.Abs(Calc.AngleDiff(currentAngle, angleA)) < Math.Abs(Calc.AngleDiff(currentAngle, angleB)))
                return ShorterAngleDifference(currentAngle, angleA, angleC);
            else
                return ShorterAngleDifference(currentAngle, angleB, angleC);
        }

        public static bool IsInRange<T>(this T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        public static bool IsInRange<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static T[] Array<T>(params T[] items)
        {
            return items;
        }

        public static T[] VerifyLength<T>(this T[] array, int length)
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

        public static T[][] VerifyLength<T>(this T[][] array, int length0, int length1)
        {
            array = VerifyLength<T[]>(array, length0);
            for (int i = 0; i < array.Length; i++)
                array[i] = VerifyLength<T>(array[i], length1);
            return array;
        }

        public static bool BetweenInterval(float val, float interval)
        {
            return val % (interval * 2) > interval;
        }

        public static bool OnInterval(float val, float prevVal, float interval)
        {
            return (int)(prevVal / interval) != (int)(val / interval);
        }


        #endregion

        #region Vector2

        public static Vector2 Toward(Vector2 from, Vector2 to, float length)
        {
            if (from == to)
                return Vector2.Zero;
            else
                return (to - from).SafeNormalize(length);
        }

        public static Vector2 Toward(Entity from, Entity to, float length)
        {
            return Toward(from.Position, to.Position, length);
        }

        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static float Angle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static Vector2 Clamp(this Vector2 val, float minX, float minY, float maxX, float maxY)
        {
            return new Vector2(MathHelper.Clamp(val.X, minX, maxX), MathHelper.Clamp(val.Y, minY, maxY));
        }

        public static Vector2 Floor(this Vector2 val)
        {
            return new Vector2((int)Math.Floor(val.X), (int)Math.Floor(val.Y));
        }

        public static Vector2 Ceiling(this Vector2 val)
        {
            return new Vector2((int)Math.Ceiling(val.X), (int)Math.Ceiling(val.Y));
        }

        public static Vector2 Abs(this Vector2 val)
        {
            return new Vector2(Math.Abs(val.X), Math.Abs(val.Y));
        }

        public static Vector2 Approach(Vector2 val, Vector2 target, float maxMove)
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

        public static Vector2 FourWayNormal(this Vector2 vec)
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

        public static Vector2 EightWayNormal(this Vector2 vec)
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

        public static Vector2 SnappedNormal(this Vector2 vec, float slices)
        {
            float divider = MathHelper.TwoPi / slices;

            float angle = vec.Angle();
            angle = (float)Math.Floor((angle + divider / 2f) / divider) * divider;
            return AngleToVector(angle, 1f);
        }

        public static Vector2 Snapped(this Vector2 vec, float slices)
        {
            float divider = MathHelper.TwoPi / slices;

            float angle = vec.Angle();
            angle = (float)Math.Floor((angle + divider / 2f) / divider) * divider;
            return AngleToVector(angle, vec.Length());
        }

        public static Vector2 XComp(this Vector2 vec)
        {
            return Vector2.UnitX * vec.X;
        }

        public static Vector2 YComp(this Vector2 vec)
        {
            return Vector2.UnitY * vec.Y;
        }

        public static Vector2[] ParseVector2List(string list, char seperator = '|')
        {
            var entries = list.Split(seperator);
            var data = new Vector2[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                var sides = entries[i].Split(',');
                data[i] = new Vector2(Convert.ToInt32(sides[0]), Convert.ToInt32(sides[1]));
            }

            return data;
        }

        #endregion

        #region Vector3 / Quaternion

        public static Vector2 Rotate(this Vector2 vec, float angleRadians)
        {
            return AngleToVector(vec.Angle() + angleRadians, vec.Length());
        }

        public static Vector2 RotateTowards(this Vector2 vec, float targetAngleRadians, float maxMoveRadians)
        {
            float angle = AngleApproach(vec.Angle(), targetAngleRadians, maxMoveRadians);
            return AngleToVector(angle, vec.Length());
        }

        public static Vector3 RotateTowards(this Vector3 from, Vector3 target, float maxRotationRadians)
        {
            var c = Vector3.Cross(from, target);
            var alen = from.Length();
            var blen = target.Length();
            var w = (float)Math.Sqrt((alen * alen) * (blen * blen)) + Vector3.Dot(from, target);
            var q = new Quaternion(c.X, c.Y, c.Z, w);

            if (q.Length() <= maxRotationRadians)
                return target;

            q.Normalize();
            q *= maxRotationRadians;

            return Vector3.Transform(from, q);
        }

        public static Vector2 XZ(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Z);
        }
        

        public static Vector3 Approach(this Vector3 v, Vector3 target, float amount)
        {
            if (amount > (target - v).Length())
                return target;
            return v + (target - v).SafeNormalize() * amount;
        }

        public static Vector3 SafeNormalize(this Vector3 v)
        {
            var len = v.Length();
            if (len > 0)
                return v / len;
            return Vector3.Zero;
        }
        
        #endregion

        #region CSV

        public static int[,] ReadCSVIntGrid(string csv, int width, int height)
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

        public static int[] ReadCSVInt(string csv)
        {
            if (csv == "")
                return new int[0];

            string[] values = csv.Split(',');
            int[] ret = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
                ret[i] = Convert.ToInt32(values[i].Trim());

            return ret;
        }

        /// <summary>
        /// Read positive-integer CSV with some added tricks.
        /// Use - to add inclusive range. Ex: 3-6 = 3,4,5,6
        /// Use * to add multiple values. Ex: 4*3 = 4,4,4
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static int[] ReadCSVIntWithTricks(string csv)
        {
            if (csv == "")
                return new int[0];

            string[] values = csv.Split(',');
            List<int> ret = new List<int>();

            foreach (var val in values)
            {
                if (val.IndexOf('-') != -1)
                {
                    var split = val.Split('-');
                    int a = Convert.ToInt32(split[0]);
                    int b = Convert.ToInt32(split[1]);

                    for (int i = a; i != b; i += Math.Sign(b - a))
                        ret.Add(i);
                    ret.Add(b);
                }
                else if (val.IndexOf('*') != -1)
                {
                    var split = val.Split('*');
                    int a = Convert.ToInt32(split[0]);
                    int b = Convert.ToInt32(split[1]);

                    for (int i = 0; i < b; i++)
                        ret.Add(a);
                }
                else
                    ret.Add(Convert.ToInt32(val));
            }

            return ret.ToArray();
        }

        public static string[] ReadCSV(string csv)
        {
            if (csv == "")
                return new string[0];

            string[] values = csv.Split(',');
            for (int i = 0; i < values.Length; i++)
                values[i] = values[i].Trim();

            return values;
        }

        public static string IntGridToCSV(int[,] data)
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

        public static bool[,] GetBitData(string data, char rowSep = '\n')
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

        public static void CombineBitData(bool[,] combineInto, string data, char rowSep = '\n')
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

        public static void CombineBitData(bool[,] combineInto, bool[,] data)
        {
            for (int i = 0; i < combineInto.GetLength(0); i++)
                for (int j = 0; j < combineInto.GetLength(1); j++)
                    if (data[i, j])
                        combineInto[i, j] = true;
        }

        public static int[] ConvertStringArrayToIntArray(string[] strings)
        {
            int[] ret = new int[strings.Length];
            for (int i = 0; i < strings.Length; i++)
                ret[i] = Convert.ToInt32(strings[i]);
            return ret;
        }

        public static float[] ConvertStringArrayToFloatArray(string[] strings)
        {
            float[] ret = new float[strings.Length];
            for (int i = 0; i < strings.Length; i++)
                ret[i] = Convert.ToSingle(strings[i], CultureInfo.InvariantCulture);
            return ret;
        }

        #endregion

        #region Save and Load Data

        public static bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public static bool SaveFile<T>(T obj, string filename) where T : new()
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

        public static bool LoadFile<T>(string filename, ref T data) where T : new()
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

        public static XmlDocument LoadContentXML(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(TitleContainer.OpenStream(Path.Combine(Engine.Instance.Content.RootDirectory, filename)));
            return xml;
        }

        public static XmlDocument LoadXML(string filename)
        {
            XmlDocument xml = new XmlDocument();
            using (var stream = File.OpenRead(filename))
                xml.Load(stream);
            return xml;
        }

        public static bool ContentXMLExists(string filename)
        {
            return File.Exists(Path.Combine(Engine.ContentDirectory, filename));
        }

        public static bool XMLExists(string filename)
        {
            return File.Exists(filename);
        }

        #region Attributes

        public static bool HasAttr(this XmlElement xml, string attributeName)
        {
            return xml.Attributes[attributeName] != null;
        }

        public static string Attr(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return xml.Attributes[attributeName].InnerText;
        }

        public static string Attr(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.Attributes[attributeName].InnerText;
        }

        public static int AttrInt(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        public static int AttrInt(this XmlElement xml, string attributeName, int defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName, float defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        public static Vector3 AttrVector3(this XmlElement xml, string attributeName)
        {
            var attr = xml.Attr(attributeName).Split(',');
            var x = float.Parse(attr[0].Trim(), CultureInfo.InvariantCulture);
            var y = float.Parse(attr[1].Trim(), CultureInfo.InvariantCulture);
            var z = float.Parse(attr[2].Trim(), CultureInfo.InvariantCulture);

            return new Vector3(x, y, z);
        }

        public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName)
        {
            return new Vector2(xml.AttrFloat(xAttributeName), xml.AttrFloat(yAttributeName));
        }

        public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName, Vector2 defaultValue)
        {
            return new Vector2(xml.AttrFloat(xAttributeName, defaultValue.X), xml.AttrFloat(yAttributeName, defaultValue.Y));
        }

        public static bool AttrBool(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToBoolean(xml.Attributes[attributeName].InnerText);
        }

        public static bool AttrBool(this XmlElement xml, string attributeName, bool defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrBool(xml, attributeName);
        }

        public static char AttrChar(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToChar(xml.Attributes[attributeName].InnerText);
        }

        public static char AttrChar(this XmlElement xml, string attributeName, char defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrChar(xml, attributeName);
        }

        public static T AttrEnum<T>(this XmlElement xml, string attributeName) where T : struct
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

        public static T AttrEnum<T>(this XmlElement xml, string attributeName, T defaultValue) where T : struct
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.AttrEnum<T>(attributeName);
        }

        public static Color AttrHexColor(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Calc.HexToColor(xml.Attr(attributeName));
        }

        public static Color AttrHexColor(this XmlElement xml, string attributeName, Color defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrHexColor(xml, attributeName);
        }

        public static Color AttrHexColor(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return Calc.HexToColor(defaultValue);
            else
                return AttrHexColor(xml, attributeName);
        }

        public static Vector2 Position(this XmlElement xml)
        {
            return new Vector2(xml.AttrFloat("x"), xml.AttrFloat("y"));
        }

        public static Vector2 Position(this XmlElement xml, Vector2 defaultPosition)
        {
            return new Vector2(xml.AttrFloat("x", defaultPosition.X), xml.AttrFloat("y", defaultPosition.Y));
        }

        public static int X(this XmlElement xml)
        {
            return xml.AttrInt("x");
        }

        public static int X(this XmlElement xml, int defaultX)
        {
            return xml.AttrInt("x", defaultX);
        }

        public static int Y(this XmlElement xml)
        {
            return xml.AttrInt("y");
        }

        public static int Y(this XmlElement xml, int defaultY)
        {
            return xml.AttrInt("y", defaultY);
        }

        public static int Width(this XmlElement xml)
        {
            return xml.AttrInt("width");
        }

        public static int Width(this XmlElement xml, int defaultWidth)
        {
            return xml.AttrInt("width", defaultWidth);
        }

        public static int Height(this XmlElement xml)
        {
            return xml.AttrInt("height");
        }

        public static int Height(this XmlElement xml, int defaultHeight)
        {
            return xml.AttrInt("height", defaultHeight);
        }

        public static Rectangle Rect(this XmlElement xml)
        {
            return new Rectangle(xml.X(), xml.Y(), xml.Width(), xml.Height());
        }

        public static int ID(this XmlElement xml)
        {
            return xml.AttrInt("id");
        }

        #endregion

        #region Inner Text

        public static int InnerInt(this XmlElement xml)
        {
            return Convert.ToInt32(xml.InnerText);
        }

        public static float InnerFloat(this XmlElement xml)
        {
            return Convert.ToSingle(xml.InnerText, CultureInfo.InvariantCulture);
        }

        public static bool InnerBool(this XmlElement xml)
        {
            return Convert.ToBoolean(xml.InnerText);
        }

        public static T InnerEnum<T>(this XmlElement xml) where T : struct
        {
            if (Enum.IsDefined(typeof(T), xml.InnerText))
                return (T)Enum.Parse(typeof(T), xml.InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static Color InnerHexColor(this XmlElement xml)
        {
            return Calc.HexToColor(xml.InnerText);
        }

        #endregion

        #region Child Inner Text

        public static bool HasChild(this XmlElement xml, string childName)
        {
            return xml[childName] != null;
        }

        public static string ChildText(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerText;
        }

        public static string ChildText(this XmlElement xml, string childName, string defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerText;
            else
                return defaultValue;
        }

        public static int ChildInt(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerInt();
        }

        public static int ChildInt(this XmlElement xml, string childName, int defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerInt();
            else
                return defaultValue;
        }

        public static float ChildFloat(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerFloat();
        }

        public static float ChildFloat(this XmlElement xml, string childName, float defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerFloat();
            else
                return defaultValue;
        }

        public static bool ChildBool(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerBool();
        }

        public static bool ChildBool(this XmlElement xml, string childName, bool defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].InnerBool();
            else
                return defaultValue;
        }

        public static T ChildEnum<T>(this XmlElement xml, string childName) where T : struct
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

        public static T ChildEnum<T>(this XmlElement xml, string childName, T defaultValue) where T : struct
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

        public static Color ChildHexColor(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return Calc.HexToColor(xml[childName].InnerText);
        }

        public static Color ChildHexColor(this XmlElement xml, string childName, Color defaultValue)
        {
            if (xml.HasChild(childName))
                return Calc.HexToColor(xml[childName].InnerText);
            else
                return defaultValue;
        }

        public static Color ChildHexColor(this XmlElement xml, string childName, string defaultValue)
        {
            if (xml.HasChild(childName))
                return Calc.HexToColor(xml[childName].InnerText);
            else
                return Calc.HexToColor(defaultValue);
        }

        public static Vector2 ChildPosition(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].Position();
        }

        public static Vector2 ChildPosition(this XmlElement xml, string childName, Vector2 defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName].Position(defaultValue);
            else
                return defaultValue;
        }

        #endregion

        #region Ogmo Nodes

        public static Vector2 FirstNode(this XmlElement xml)
        {
            if (xml["node"] == null)
                return Vector2.Zero;
            else
                return new Vector2((int)xml["node"].AttrFloat("x"), (int)xml["node"].AttrFloat("y"));
        }

        public static Vector2? FirstNodeNullable(this XmlElement xml)
        {
            if (xml["node"] == null)
                return null;
            else
                return new Vector2((int)xml["node"].AttrFloat("x"), (int)xml["node"].AttrFloat("y"));
        }

        public static Vector2? FirstNodeNullable(this XmlElement xml, Vector2 offset)
        {
            if (xml["node"] == null)
                return null;
            else
                return new Vector2((int)xml["node"].AttrFloat("x"), (int)xml["node"].AttrFloat("y")) + offset;
        }

        public static Vector2[] Nodes(this XmlElement xml, bool includePosition = false)
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

        public static Vector2[] Nodes(this XmlElement xml, Vector2 offset, bool includePosition = false)
        {
            var nodes = Calc.Nodes(xml, includePosition);

            for (int i = 0; i < nodes.Length; i++)
                nodes[i] += offset;

            return nodes;
        }

        public static Vector2 GetNode(this XmlElement xml, int nodeNum)
        {
            return xml.Nodes()[nodeNum];
        }

        public static Vector2? GetNodeNullable(this XmlElement xml, int nodeNum)
        {
            if (xml.Nodes().Length > nodeNum)
                return xml.Nodes()[nodeNum];
            else
                return null;
        }

        #endregion

        #region Add Stuff

        public static void SetAttr(this XmlElement xml, string attributeName, Object setTo)
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

        public static void SetChild(this XmlElement xml, string childName, Object setTo)
        {
            XmlElement ele;

            if (xml.HasChild(childName))
                ele = xml[childName];
            else
            {
                ele = xml.OwnerDocument.CreateElement(null, childName, xml.NamespaceURI);
                xml.AppendChild(ele);
            }

            ele.InnerText = setTo.ToString();
        }

        public static XmlElement CreateChild(this XmlDocument doc, string childName)
        {
            XmlElement ele = doc.CreateElement(null, childName, doc.NamespaceURI);
            doc.AppendChild(ele);
            return ele;
        }

        public static XmlElement CreateChild(this XmlElement xml, string childName)
        {
            XmlElement ele = xml.OwnerDocument.CreateElement(null, childName, xml.NamespaceURI);
            xml.AppendChild(ele);
            return ele;
        }

        #endregion

        #endregion

        #region Sorting

        public static int SortLeftToRight(Entity a, Entity b)
        {
            return (int)((a.X - b.X) * 100);
        }

        public static int SortRightToLeft(Entity a, Entity b)
        {
            return (int)((b.X - a.X) * 100);
        }

        public static int SortTopToBottom(Entity a, Entity b)
        {
            return (int)((a.Y - b.Y) * 100);
        }

        public static int SortBottomToTop(Entity a, Entity b)
        {
            return (int)((b.Y - a.Y) * 100);
        }

        public static int SortByDepth(Entity a, Entity b)
        {
            return a.Depth - b.Depth;
        }

        public static int SortByDepthReversed(Entity a, Entity b)
        {
            return b.Depth - a.Depth;
        }

        #endregion

        #region Debug

        public static void Log()
        {
            Debug.WriteLine("Log");
        }

        public static void TimeLog()
        {
            Debug.WriteLine(Engine.Scene.RawTimeActive);
        }

        public static void Log(params object[] obj)
        {
            foreach (var o in obj)
            {
                if (o == null)
                    Debug.WriteLine("null");
                else
                    Debug.WriteLine(o.ToString());
            }
        }

        public static void TimeLog(object obj)
        {
            Debug.WriteLine(Engine.Scene.RawTimeActive + " : " + obj);
        }

        public static void LogEach<T>(IEnumerable<T> collection)
        {
            foreach (var o in collection)
                Debug.WriteLine(o.ToString());
        }

        public static void Dissect(Object obj)
        {
            Debug.Write(obj.GetType().Name + " { ");
            foreach (var v in obj.GetType().GetFields())
                Debug.Write(v.Name + ": " + v.GetValue(obj) + ", ");
            Debug.WriteLine(" }");
        }

        private static Stopwatch stopwatch;

        public static void StartTimer()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public static void EndTimer()
        {
            if (stopwatch != null)
            {
                stopwatch.Stop();

                string message = "Timer: " + stopwatch.ElapsedTicks + " ticks, or " + TimeSpan.FromTicks(stopwatch.ElapsedTicks).TotalSeconds.ToString("00.0000000") + " seconds";
                Debug.WriteLine(message);
#if DESKTOP && DEBUG
                //Commands.Trace(message);
#endif
                stopwatch = null;
            }
        }

        #endregion

        #region Reflection

        public static Delegate GetMethod<T>(Object obj, string method) where T : class
        {
            var info = obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (info == null)
                return null;
            else
                return Delegate.CreateDelegate(typeof(T), obj, method);
        }

        #endregion

        public static T At<T>(this T[,] arr, Pnt at)
        {
            return arr[at.X, at.Y];
        }

        public static string ConvertPath(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }

        public static string ReadNullTerminatedString(this System.IO.BinaryReader stream)
        {
            string str = "";
            char ch;
            while ((int)(ch = stream.ReadChar()) != 0)
                str = str + ch;
            return str;
        }

        public static IEnumerator Do(params IEnumerator[] numerators)
        {
            if (numerators.Length == 0)
                yield break;
            else if (numerators.Length == 1)
                yield return numerators[0];
            else
            {
                List<Coroutine> routines = new List<Coroutine>();
                foreach (var enumerator in numerators)
                    routines.Add(new Coroutine(enumerator));

                while (true)
                {
                    bool moving = false;
                    foreach (var routine in routines)
                    {
                        routine.Update();
                        if (!routine.Finished)
                            moving = true;
                    }

                    if (moving)
                        yield return null;
                    else
                        break;
                }
            }
        }

        public static Rectangle ClampTo(this Rectangle rect, Rectangle clamp)
        {
            if (rect.X < clamp.X)
            {
                rect.Width -= (clamp.X - rect.X);
                rect.X = clamp.X;
            }

            if (rect.Y < clamp.Y)
            {
                rect.Height -= (clamp.Y - rect.Y);
                rect.Y = clamp.Y;
            }

            if (rect.Right > clamp.Right)
                rect.Width = clamp.Right - rect.X;
            if (rect.Bottom > clamp.Bottom)
                rect.Height = clamp.Bottom - rect.Y;

            return rect;
        }
    }

    public static class QuaternionExt
    {
        public static Quaternion Conjugated(this Quaternion q)
        {
            var c = q;
            c.Conjugate();
            return c;
        }

        public static Quaternion LookAt(this Quaternion q, Vector3 from, Vector3 to, Vector3 up)
        {
            return Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(from, to, up));
        }

        public static Quaternion LookAt(this Quaternion q, Vector3 direction, Vector3 up)
        {
            return Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, direction, up));
        }

        public static Vector3 Forward(this Quaternion q)
        {
            return Vector3.Transform(Vector3.Forward, q.Conjugated());
        }

        public static Vector3 Left(this Quaternion q)
        {
            return Vector3.Transform(Vector3.Left, q.Conjugated());
        }

        public static Vector3 Up(this Quaternion q)
        {
            return Vector3.Transform(Vector3.Up, q.Conjugated());
        }
    }
}
