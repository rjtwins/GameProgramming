using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Extensions
{
    public struct RectangleM
    {
        public static readonly RectangleM Empty;

        [DataMember]
        public decimal X;

        [DataMember]
        public decimal Y;

        [DataMember]
        public decimal Width;

        [DataMember]
        public decimal Height;

        public decimal Left => X;

        public decimal Right => X + Width;

        public decimal Top => Y;

        public decimal Bottom => Y + Height;

        public bool IsEmpty
        {
            get
            {
                if (Width.Equals(0f) && Height.Equals(0f) && X.Equals(0f))
                {
                    return Y.Equals(0f);
                }

                return false;
            }
        }

        public (decimal x, decimal y) Position
        {
            get
            {
                return (X, Y);
            }
            set
            {
                X = value.x;
                Y = value.y;
            }
        }

        public (decimal x, decimal y) Size
        {
            get
            {
                return (Width, Height);
            }
            set
            {
                Width = value.x;
                Height = value.y;
            }
        }

        public (decimal x, decimal y) Center => (X + Width * 0.5M, Y + Height * 0.5M);

        public (decimal x, decimal y) TopLeft => (X, Y);

        public (decimal x, decimal y) TopRight => (X + Width, Y);

        public (decimal x, decimal y) BottomLeft => (X, Y + Height);

        public (decimal x, decimal y) BottomRight => (X + Width, Y + Height);

        internal string DebugDisplayString => X + "  " + Y + "  " + Width + "  " + Height;

        public RectangleM(decimal x, decimal y, decimal width, decimal height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleM(Point2 position, Size2 size)
        {
            X = (decimal)position.X;
            Y = (decimal)position.Y;
            Width = (decimal)size.Width;
            Height = (decimal)size.Height;
        }

        public RectangleM((decimal x, decimal y) position, (decimal x, decimal y) size)
        {
            X = position.x;
            Y = position.y;
            Width = size.x;
            Height = size.y;
        }



        //public static void CreateFrom(Point2 minimum, Point2 maximum, out RectangleD result)
        //{
        //    result.X = minimum.X;
        //    result.Y = minimum.Y;
        //    result.Width = maximum.X - minimum.X;
        //    result.Height = maximum.Y - minimum.Y;
        //}

        //public static void Union(ref RectangleD first, ref RectangleD second, out RectangleD result)
        //{
        //    result.X = Math.Min(first.X, second.X);
        //    result.Y = Math.Min(first.Y, second.Y);
        //    result.Width = Math.Max(first.Right, second.Right) - result.X;
        //    result.Height = Math.Max(first.Bottom, second.Bottom) - result.Y;
        //}

        //public static RectangleD Union(RectangleD first, RectangleD second)
        //{
        //    Union(ref first, ref second, out var result);
        //    return result;
        //}

        //public RectangleD Union(RectangleD rectangle)
        //{
        //    Union(ref this, ref rectangle, out var result);
        //    return result;
        //}

        //public static void Intersection(ref RectangleD first, ref RectangleD second, out RectangleD result)
        //{
        //    Point2 topLeft = first.TopLeft;
        //    Point2 bottomRight = first.BottomRight;
        //    Point2 topLeft2 = second.TopLeft;
        //    Point2 bottomRight2 = second.BottomRight;
        //    Point2 minimum = Point2.Maximum(topLeft, topLeft2);
        //    Point2 maximum = Point2.Minimum(bottomRight, bottomRight2);
        //    if (maximum.X < minimum.X || maximum.Y < minimum.Y)
        //    {
        //        result = default(RectangleD);
        //    }
        //    else
        //    {
        //        result = CreateFrom(minimum, maximum);
        //    }
        //}

        //public static RectangleD Intersection(RectangleD first, RectangleD second)
        //{
        //    Intersection(ref first, ref second, out var result);
        //    return result;
        //}

        //public RectangleD Intersection(RectangleD rectangle)
        //{
        //    Intersection(ref this, ref rectangle, out var result);
        //    return result;
        //}

        //[Obsolete("RectangleD.Intersect() may be removed in the future. Use Intersection() instead.")]
        //public static RectangleD Intersect(RectangleD value1, RectangleD value2)
        //{
        //    Intersection(ref value1, ref value2, out var result);
        //    return result;
        //}

        //[Obsolete("RectangleD.Intersect() may be removed in the future. Use Intersection() instead.")]
        //public static void Intersect(ref RectangleD value1, ref RectangleD value2, out RectangleD result)
        //{
        //    Intersection(ref value1, ref value2, out result);
        //}

        //public static bool Intersects(ref RectangleD first, ref RectangleD second)
        //{
        //    if (first.X < second.X + second.Width && first.X + first.Width > second.X && first.Y < second.Y + second.Height)
        //    {
        //        return first.Y + first.Height > second.Y;
        //    }

        //    return false;
        //}

        //public static bool Intersects(RectangleD first, RectangleD second)
        //{
        //    return Intersects(ref first, ref second);
        //}

        //public bool Intersects(RectangleD rectangle)
        //{
        //    return Intersects(ref this, ref rectangle);
        //}

        public static bool Contains(ref RectangleM rectangle, ref Point2 point)
        {
            if (rectangle.X <= (decimal)point.X && (decimal)point.X < rectangle.X + rectangle.Width && rectangle.Y <= (decimal)point.Y)
            {
                return (decimal)point.Y < rectangle.Y + rectangle.Height;
            }

            return false;
        }

        public static bool Contains(RectangleM rectangle, Point2 point)
        {
            return Contains(ref rectangle, ref point);
        }

        public bool Contains(Point2 point)
        {
            return Contains(ref this, ref point);
        }

        //public void UpdateFromPoints(IReadOnlyList<Point2> points)
        //{
        //    RectangleD RectangleD = CreateFrom(points);
        //    X = RectangleD.X;
        //    Y = RectangleD.Y;
        //    Width = RectangleD.Width;
        //    Height = RectangleD.Height;
        //}

        //public float SquaredDistanceTo(Point2 point)
        //{
        //    return PrimitivesHelper.SquaredDistanceToPointFromRectangle(TopLeft, BottomRight, point);
        //}

        //public float DistanceTo(Point2 point)
        //{
        //    return (float)Math.Sqrt(SquaredDistanceTo(point));
        //}

        //public Point2 ClosestPointTo(Point2 point)
        //{
        //    PrimitivesHelper.ClosestPointToPointFromRectangle(TopLeft, BottomRight, point, out var result);
        //    return result;
        //}

        public void Inflate(decimal horizontalAmount, decimal verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2M;
            Height += verticalAmount * 2M;
        }

        public void Offset(decimal offsetX, decimal offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Offset(Vector2 amount)
        {
            X += (decimal)amount.X;
            Y += (decimal)amount.Y;
        }

        public static bool operator ==(RectangleM first, RectangleM second)
        {
            return first.Equals(ref second);
        }

        public static bool operator !=(RectangleM first, RectangleM second)
        {
            return !(first == second);
        }

        public bool Equals(RectangleM rectangle)
        {
            return Equals(ref rectangle);
        }

        public bool Equals(ref RectangleM rectangle)
        {
            if (X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width)
            {
                return Height == rectangle.Height;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleM)
            {
                return Equals((RectangleM)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (((((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Width.GetHashCode()) * 397) ^ Height.GetHashCode();
        }

        public static implicit operator RectangleM(Rectangle rectangle)
        {
            RectangleM result = default(RectangleM);
            result.X = rectangle.X;
            result.Y = rectangle.Y;
            result.Width = rectangle.Width;
            result.Height = rectangle.Height;
            return result;
        }

        public static explicit operator Rectangle(RectangleM rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static implicit operator RectangleM(RectangleF rectangle)
        {
            RectangleM result = default(RectangleM);
            result.X = (decimal)rectangle.X;
            result.Y = (decimal)rectangle.Y;
            result.Width = (decimal)rectangle.Width;
            result.Height = (decimal)rectangle.Height;
            return result;
        }

        public static explicit operator RectangleF(RectangleM rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }
    }
}
