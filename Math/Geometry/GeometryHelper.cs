using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Geometry
{
    public static class GeometryHelper
    {
        #region GetAdjacentPoint
        /// <summary>
        /// GetAdjacentPoint 获取某个方向上的相邻点
        /// </summary>       
        public static Point GetAdjacentPoint(Point current, CompassDirections direction)
        {
            switch (direction)
            {
                case CompassDirections.North:
                    {
                        return new Point(current.X, current.Y - 1);
                    }
                case CompassDirections.South:
                    {
                        return new Point(current.X, current.Y + 1);
                    }
                case CompassDirections.East:
                    {
                        return new Point(current.X + 1, current.Y);
                    }
                case CompassDirections.West:
                    {
                        return new Point(current.X - 1, current.Y);
                    }
                case CompassDirections.NorthEast:
                    {
                        return new Point(current.X + 1, current.Y - 1);
                    }
                case CompassDirections.NorthWest:
                    {
                        return new Point(current.X - 1, current.Y - 1);
                    }
                case CompassDirections.SouthEast:
                    {
                        return new Point(current.X + 1, current.Y + 1);
                    }
                case CompassDirections.SouthWest:
                    {
                        return new Point(current.X - 1, current.Y + 1);
                    }
                default:
                    {
                        return current;
                    }
            }
        }
        #endregion     

        #region GetDistanceSquare
        /// <summary>
        /// GetDistanceSquare 获取两个点之间距离的平方。
        /// </summary>      
        public static int GetDistanceSquare(Point pt1, Point pt2)
        {
            int deltX = pt1.X - pt2.X;
            int deltY = pt1.Y - pt2.Y;

            return deltX * deltX + deltY * deltY;
        }
        #endregion

        #region GetDirectionBetween
        /// <summary>
        /// GetDirectionBetween 获取从起点到终点的方向
        /// </summary>       
        public static CompassDirections GetDirectionBetween(Point origin, Point dest, int torrence)
        {
            int deltX = dest.X - origin.X;
            int deltY = dest.Y - origin.Y;

            if (deltX > torrence && deltY < -torrence)
            {
                return CompassDirections.NorthEast;
            }

            if (deltX > torrence && deltY > torrence)
            {
                return CompassDirections.SouthEast;
            }

            if (deltX < -torrence && deltY < -torrence)
            {
                return CompassDirections.NorthWest;
            }

            if (deltX < -torrence && deltY > torrence)
            {
                return CompassDirections.SouthWest;
            }

            if (Math.Abs(deltX) < torrence && deltY < -torrence)
            {
                return CompassDirections.North;
            }

            if (Math.Abs(deltX) < torrence && deltY > torrence)
            {
                return CompassDirections.South;
            }

            if (deltX > torrence && Math.Abs(deltY) < torrence)
            {
                return CompassDirections.East;
            }

            if (deltX < -torrence && Math.Abs(deltY) < torrence)
            {
                return CompassDirections.West;
            }

            return CompassDirections.NotSet;
        }
        #endregion        
    }
}
