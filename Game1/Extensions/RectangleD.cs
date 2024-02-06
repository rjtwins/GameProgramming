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
    public struct RectangleD
    {
        public static readonly RectangleD Empty;

        [DataMember]
        public double X;

        [DataMember]
        public double Y;

        [DataMember]
        public double Width;

        [DataMember]
        public double Height;

        public double Left => X;

        public double Right => X + Width;

        public double Top => Y;

        public double Bottom => Y + Height;

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

        public (double x, double y) Position
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

        public (double x, double y) Size
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

        public (double x, double y) Center => (X + Width * 0.5f, Y + Height * 0.5f);

        public (double x, double y) TopLeft => (X, Y);

        public (double x, double y) TopRight => (X + Width, Y);

        public (double x, double y) BottomLeft => (X, Y + Height);

        public (double x, double y) BottomRight => (X + Width, Y + Height);

        internal string DebugDisplayString => X + "  " + Y + "  " + Width + "  " + Height;

        public RectangleD(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleD(Point2 position, Size2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public RectangleD((double x, double y) position, (double x, double y) size)
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

        public static bool Contains(ref RectangleD rectangle, ref Point2 point)
        {
            if (rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y)
            {
                return point.Y < rectangle.Y + rectangle.Height;
            }

            return false;
        }

        public static bool Contains(RectangleD rectangle, Point2 point)
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

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2f;
            Height += verticalAmount * 2f;
        }

        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Offset(Vector2 amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        public static bool operator ==(RectangleD first, RectangleD second)
        {
            return first.Equals(ref second);
        }

        public static bool operator !=(RectangleD first, RectangleD second)
        {
            return !(first == second);
        }

        public bool Equals(RectangleD rectangle)
        {
            return Equals(ref rectangle);
        }

        public bool Equals(ref RectangleD rectangle)
        {
            if (X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width)
            {
                return Height == rectangle.Height;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is RectangleD)
            {
                return Equals((RectangleD)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (((((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Width.GetHashCode()) * 397) ^ Height.GetHashCode();
        }

        public static implicit operator RectangleD(Rectangle rectangle)
        {
            RectangleD result = default(RectangleD);
            result.X = rectangle.X;
            result.Y = rectangle.Y;
            result.Width = rectangle.Width;
            result.Height = rectangle.Height;
            return result;
        }

        public static explicit operator Rectangle(RectangleD rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static implicit operator RectangleD(RectangleF rectangle)
        {
            RectangleD result = default(RectangleD);
            result.X = rectangle.X;
            result.Y = rectangle.Y;
            result.Width = rectangle.Width;
            result.Height = rectangle.Height;
            return result;
        }

        public static explicit operator RectangleF(RectangleD rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public override string ToString()
        {
            return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
        }
    }
}
