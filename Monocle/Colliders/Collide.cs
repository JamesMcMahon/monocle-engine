using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    [Flags]
    public enum PointSectors { Center = 0, Top = 1, Bottom = 2, TopLeft = 9, TopRight = 5, Left = 8, Right = 4, BottomLeft = 10, BottomRight = 6 };

    public static class Collide
    {
        #region Entity vs Entity

        public static bool Check(Entity a, Entity b)
        {
            if (a.Collider == null || b.Collider == null)
                return false;
            else
                return a != b && b.Collidable && a.Collider.Collide(b);
        }

        public static bool Check(Entity a, Entity b, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        public static bool Check(Entity a, CollidableComponent b)
        {
            if (a.Collider == null || b.Collider == null)
                return false;
            else
                return b.Collidable && b.Entity.Collidable && a.Collider.Collide(b);
        }

        public static bool Check(Entity a, CollidableComponent b, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Entity vs Entity Enumerable

        #region Check

        public static bool Check(Entity a, IEnumerable<Entity> b)
        {
            foreach (var e in b)
                if (Check(a, e))
                    return true;

            return false;
        }

        public static bool Check(Entity a, IEnumerable<Entity> b, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = Check(a, b);
            a.Position = old;
            return ret;
        }

        #endregion

        #region First

        public static Entity First(Entity a, IEnumerable<Entity> b)
        {
            foreach (var e in b)
                if (Check(a, e))
                    return e;

            return null;
        }

        public static Entity First(Entity a, IEnumerable<Entity> b, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            Entity ret = First(a, b);
            a.Position = old;
            return ret;
        }

        #endregion

        #region All

        public static List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into)
        {
            foreach (var e in b)
                if (Check(a, e))
                    into.Add(e);

            return into;
        }

        public static List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            List<Entity> ret = All(a, b, into);
            a.Position = old;
            return ret;
        }

        public static List<Entity> All(Entity a, IEnumerable<Entity> b)
        {
            return All(a, b, new List<Entity>());
        }

        public static List<Entity> All(Entity a, IEnumerable<Entity> b, Vector2 at)
        {
            return All(a, b, new List<Entity>(), at);
        }

        #endregion

        #endregion

        #region Entity vs Point

        public static bool CheckPoint(Entity a, Vector2 point)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(point);
        }

        public static bool CheckPoint(Entity a, Vector2 point, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = CheckPoint(a, point);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Entity vs Line

        public static bool CheckLine(Entity a, Vector2 from, Vector2 to)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(from, to);
        }

        public static bool CheckLine(Entity a, Vector2 from, Vector2 to, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = CheckLine(a, from, to);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Entity vs Rectangle

        public static bool CheckRect(Entity a, Rectangle rect)
        {
            if (a.Collider == null)
                return false;
            else
                return a.Collider.Collide(rect);
        }

        public static bool CheckRect(Entity a, Rectangle rect, Vector2 at)
        {
            Vector2 old = a.Position;
            a.Position = at;
            bool ret = CheckRect(a, rect);
            a.Position = old;
            return ret;
        }

        #endregion

        #region Line

        public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
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

        public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
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

        #region Circle

        public static bool CircleToLine(Vector2 cPosiition, float cRadius, Vector2 lineFrom, Vector2 lineTo)
        {
            return Vector2.DistanceSquared(cPosiition, Calc.ClosestPointOnLine(lineFrom, lineTo, cPosiition)) < cRadius * cRadius;
        }

        public static bool CircleToPoint(Vector2 cPosition, float cRadius, Vector2 point)
        {
            return Vector2.DistanceSquared(cPosition, point) < cRadius * cRadius;
        }

        public static bool CircleToRect(Vector2 cPosition, float cRadius, float rX, float rY, float rW, float rH)
        {
            return RectToCircle(rX, rY, rW, rH, cPosition, cRadius);
        }

        public static bool CircleToRect(Vector2 cPosition, float cRadius, Rectangle rect)
        {
            return RectToCircle(rect, cPosition, cRadius);
        }

        #endregion

        #region Rect

        public static bool RectToCircle(float rX, float rY, float rW, float rH, Vector2 cPosition, float cRadius)
        {
            //Check if the rectangle contains the circle's center point
            if (Collide.RectToPoint(rX, rY, rW, rH, cPosition))
                return true;

            //Check the circle against the relevant edges
            Vector2 edgeFrom;
            Vector2 edgeTo;
            PointSectors sector = GetSector(rX, rY, rW, rH, cPosition);

            if ((sector & PointSectors.Top) != 0)
            {
                edgeFrom = new Vector2(rX, rY);
                edgeTo = new Vector2(rX + rW, rY);
                if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
                    return true;
            }

            if ((sector & PointSectors.Bottom) != 0)
            {
                edgeFrom = new Vector2(rX, rY + rH);
                edgeTo = new Vector2(rX + rW, rY + rH);
                if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
                    return true;
            }

            if ((sector & PointSectors.Left) != 0)
            {
                edgeFrom = new Vector2(rX, rY);
                edgeTo = new Vector2(rX, rY + rH);
                if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
                    return true;
            }

            if ((sector & PointSectors.Right) != 0)
            {
                edgeFrom = new Vector2(rX + rW, rY);
                edgeTo = new Vector2(rX + rW, rY + rH);
                if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
                    return true;
            }

            return false;
        }

        public static bool RectToCircle(Rectangle rect, Vector2 cPosition, float cRadius)
        {
            return RectToCircle(rect.X, rect.Y, rect.Width, rect.Height, cPosition, cRadius);
        }

        public static bool RectToLine(float rX, float rY, float rW, float rH, Vector2 lineFrom, Vector2 lineTo)
        {
            PointSectors fromSector = Monocle.Collide.GetSector(rX, rY, rW, rH, lineFrom);
            PointSectors toSector = Monocle.Collide.GetSector(rX, rY, rW, rH, lineTo);

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
                    edgeFrom = new Vector2(rX, rY);
                    edgeTo = new Vector2(rX + rW, rY);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, lineFrom, lineTo))
                        return true;
                }

                if ((both & PointSectors.Bottom) != 0)
                {
                    edgeFrom = new Vector2(rX, rY + rH);
                    edgeTo = new Vector2(rX + rW, rY + rH);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, lineFrom, lineTo))
                        return true;
                }

                if ((both & PointSectors.Left) != 0)
                {
                    edgeFrom = new Vector2(rX, rY);
                    edgeTo = new Vector2(rX, rY + rH);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, lineFrom, lineTo))
                        return true;
                }

                if ((both & PointSectors.Right) != 0)
                {
                    edgeFrom = new Vector2(rX + rW, rY);
                    edgeTo = new Vector2(rX + rW, rY + rH);
                    if (Monocle.Collide.LineCheck(edgeFrom, edgeTo, lineFrom, lineTo))
                        return true;
                }
            }

            return false;
        }

        public static bool RectToLine(Rectangle rect, Vector2 lineFrom, Vector2 lineTo)
        {
            return RectToLine(rect.X, rect.Y, rect.Width, rect.Height, lineFrom, lineTo);
        }

        public static bool RectToPoint(float rX, float rY, float rW, float rH, Vector2 point)
        {
            return point.X >= rX && point.Y >= rY && point.X < rX + rW && point.Y < rY + rH;
        }

        public static bool RectToPoint(Rectangle rect, Vector2 point)
        {
            return RectToPoint(rect.X, rect.Y, rect.Width, rect.Height, point);
        }

        #endregion

        #region Sectors

        /*
         *  Bitflags and helpers for using the Cohen–Sutherland algorithm
         *  http://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
         *  
         *  Sector bitflags:
         *      1001  1000  1010
         *      0001  0000  0010
         *      0101  0100  0110
         */

        public static PointSectors GetSector(Rectangle rect, Vector2 point)
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

        public static PointSectors GetSector(float rX, float rY, float rW, float rH, Vector2 point)
        {
            PointSectors sector = PointSectors.Center;

            if (point.X < rX)
                sector |= PointSectors.Left;
            else if (point.X >= rX + rW)
                sector |= PointSectors.Right;

            if (point.Y < rY)
                sector |= PointSectors.Top;
            else if (point.Y >= rY + rH)
                sector |= PointSectors.Bottom;

            return sector;
        }

        #endregion
    }
}
