using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flat
{
    public static class PositionExtensions
    {
        public static (long x, long y) Add(this (long x, long y) a, Vector2 vector)
        {
            var result =((long)(a.x + vector.X), (long)(a.y + vector.Y));
            Debug.WriteLine(result);

            return result;
        }

        public static (long x, long y) Sub(this (long x, long y) a, Vector2 vector)
        {
            var result = ((long)(a.x - vector.X), (long)(a.y - vector.Y));
            
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
    }
}
