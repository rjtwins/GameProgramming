using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Extensions
{
    public static class PositionExtensions
    {
        public static (long x, long y) Add(this (long x, long y) a, Vector2 vector)
        {
            var result = ((long)(a.x + vector.X), (long)(a.y + vector.Y));
            return result;
        }

        public static (long x, long y) Sub(this (long x, long y) a, Vector2 vector)
        {
            var result = ((long)(a.x - vector.X), (long)(a.y - vector.Y));

            return result;
        }

        public static (double x, double y) Add(this (double x, double y) a, Vector2 vector)
        {
            var result = ((double)(a.x + vector.X), (double)(a.y + vector.Y));
            return result;
        }

        public static (double x, double y) Sub(this (double x, double y) a, Vector2 vector)
        {
            var result = ((double)(a.x - vector.X), (double)(a.y - vector.Y));

            return result;
        }

        public static Vector2 ToVector2(this (long x, long y) tuple)
        {
            return new Vector2(tuple.x, tuple.y);
        }

        public static (long x, long y) ToLongTuple(this Vector2 vector)
        {
            return ((long)vector.X, (long)vector.Y);
        }

        public static Vector2 ToVector2(this (double x, double y) tuple)
        {
            return new Vector2((float)tuple.x, (float)tuple.y);
        }

        public static (double x, double y) ToDoubleTuple(this Vector2 vector)
        {
            return (vector.X, vector.Y);
        }
    }
}
