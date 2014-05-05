using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    static public class Collide
    {
        #region Entity vs Entity

        static public bool Check(Entity a, Entity b)
        {
            if (a.Collider == null)
                return false;
            else
                return a != b && b.Collidable && a.Collider.Collide(b);
        }

        static public bool Check(Entity a, Entity b, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        static public bool Check(Entity a, Entity b, float aX, float aY)
        {
            return Check(a, b, new Vector2(aX, aY));
        }

        #endregion

        #region Entity vs Vector2

        static public bool Check(Entity a, Vector2 point)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(point);
        }

        static public bool Check(Entity a, Vector2 point, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            bool ret = Check(a, point);
            a.Position = old;
            return ret;
        }

        static public bool Check(Entity a, Vector2 point, float aX, float aY)
        {
            return Check(a, point, new Vector2(aX, aY));
        }

        #endregion

        #region Entity vs Rectangle

        static public bool Check(Entity a, Rectangle rect)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(rect);
        }

        static public bool Check(Entity a, Rectangle rect, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            bool ret = Check(a, rect);
            a.Position = old;
            return ret;
        }

        static public bool Check(Entity a, Rectangle rect, float aX, float aY)
        {
            return Check(a, rect, new Vector2(aX, aY));
        }

        #endregion

        #region Collider vs Entity List

        static public bool Check(Collider a, List<Entity> b)
        {
            for (int i = 0; i < b.Count; i++)
                if (a.Collide(b[i]))
                    return true;

            return false;
        }

        static public bool Check(Collider a, List<Entity> b, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        static public bool Check(Collider a, List<Entity> b, float aX, float aY)
        {
            return Check(a, b, new Vector2(aX, aY));
        }

        static public Entity First(Collider a, List<Entity> b)
        {
            for (int i = 0; i < b.Count; i++)
                if (a.Collide(b[i]))
                    return b[i];

            return null;
        }

        static public Entity First(Collider a, List<Entity> b, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            Entity ret = First(a, b);
            a.Position = old;
            return ret;
        }

        static public Entity First(Collider a, List<Entity> b, float aX, float aY)
        {
            return First(a, b, new Vector2(aX, aY));
        }

        static public void Into(Collider a, List<Entity> b, List<Entity> into)
        {
            for (int i = 0; i < b.Count; i++)
                if (a.Collide(b[i]))
                    into.Add(b[i]);
        }

        static public void Into(Collider a, List<Entity> b, List<Entity> into, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            Into(a, b, into);
            a.Position = old;
        }

        static public void Into(Collider a, List<Entity> b, List<Entity> into, float aX, float aY)
        {
            Into(a, b, into, new Vector2(aX, aY));
        }

        static public List<Entity> All(Collider a, List<Entity> b)
        {
            var list = new List<Entity>();
            Into(a, b, list);
            return list;
        }

        static public List<Entity> All(Collider a, List<Entity> b, Vector2 aPos)
        {
            var list = new List<Entity>();
            Into(a, b, list, aPos);
            return list;
        }

        static public List<Entity> All(Collider a, List<Entity> b, float aX, float aY)
        {
            var list = new List<Entity>();
            Into(a, b, list, aX, aY);
            return list;
        }

        #endregion

        #region Entity vs Entity Enumerable

        #region Check

        static public bool Check(Entity a, IEnumerable<Entity> b)
        {
            foreach (var e in b)
            {
                if (Check(a, e))
                    return true;
            }

            return false;
        }

        static public bool Check(Entity a, IEnumerable<Entity> b, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        static public bool Check(Entity a, IEnumerable<Entity> b, float aX, float aY)
        {
            return Check(a, b, new Vector2(aX, aY));
        }

        #endregion

        #region First

        static public Entity First(Entity a, IEnumerable<Entity> b)
        {
            foreach (var e in b)
            {
                if (Check(a, e))
                    return e;
            }

            return null;
        }

        static public Entity First(Entity a, IEnumerable<Entity> b, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            Entity ret = First(a, b);
            a.Position = old;
            return ret;
        }

        static public Entity First(Entity a, IEnumerable<Entity> b, float aX, float aY)
        {
            return First(a, b, new Vector2(aX, aY));
        }

        #endregion

        #region All

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into)
        {
            foreach (var e in b)
            {
                if (Check(a, e))
                    into.Add(e);
            }

            return into;
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into, Vector2 aPos)
        {
            Vector2 old = a.Position;
            a.Position = aPos;
            List<Entity> ret = All(a, b, into);
            a.Position = old;
            return ret;
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into, float aX, float aY)
        {
            return All(a, b, into, new Vector2(aX, aY));
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b)
        {
            return All(a, b, new List<Entity>());
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, Vector2 aPos)
        {
            return All(a, b, new List<Entity>(), aPos);
        }

        static public List<Entity> All(Entity a, IEnumerable<Entity> b, float aX, float aY)
        {
            return All(a, b, new List<Entity>(), aX, aY);
        }

        #endregion

        #endregion

        #region Misc

        [Flags]
        private enum PointSectors { Center = 0, Top = 1, Bottom = 2, TopLeft = 9, TopRight = 5, Left = 8, Right = 4, BottomLeft = 10, BottomRight = 6 };

        static private PointSectors GetSector(Rectangle rect, Vector2 point)
        {
            PointSectors sector = PointSectors.Center;

            if (point.X < rect.Left)
                sector |= PointSectors.Left;
            else if (point.X >= rect.Right)
                sector |= PointSectors.Right;

            if (point.Y < rect.Top)
                sector |= PointSectors.Top;
            else if (point.Y >= rect.Bottom)
                sector |= PointSectors.Bottom;

            return sector;
        }

        static public bool LineCheck(Rectangle rect, Vector2 from, Vector2 to)
        {
            PointSectors fromSector = GetSector(rect, from);
            PointSectors toSector = GetSector(rect, to);

            if (fromSector == PointSectors.Center || toSector == PointSectors.Center)
                return true;
            else if ((fromSector & toSector) != 0)
                return false;
            else
            {
                PointSectors both = fromSector | toSector;

                //Do line checks against the edges
                Vector2 edgeFrom;
                Vector2 edgeTo;

                if ((both & PointSectors.Top) != 0)
                {
                    edgeFrom = new Vector2(rect.Left, rect.Top);
                    edgeTo = new Vector2(rect.Right, rect.Top);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Bottom) != 0)
                {
                    edgeFrom = new Vector2(rect.Left, rect.Bottom);
                    edgeTo = new Vector2(rect.Right, rect.Bottom);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Left) != 0)
                {
                    edgeFrom = new Vector2(rect.Left, rect.Top);
                    edgeTo = new Vector2(rect.Left, rect.Bottom);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }

                if ((both & PointSectors.Right) != 0)
                {
                    edgeFrom = new Vector2(rect.Right, rect.Top);
                    edgeTo = new Vector2(rect.Right, rect.Bottom);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, from, to))
                        return true;
                }
            }

            return false;
        }

        static public bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            return true;
        }

        static public bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.Zero;

            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = a1 + t * b;

            return true;
        }

        #endregion
    }
}
