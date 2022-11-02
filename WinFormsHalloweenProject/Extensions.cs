using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace WinFormsHalloweenProject
{
    using static WinFormsHalloweenProject.Ghost;

    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Lerp(this Point a, Point b, double percent) => new Point(a.X.Lerp(b.X, percent), a.Y.Lerp(b.Y, percent));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte ToByte(this bool val) => ToByte(&val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte ToByte(bool* val) => *(byte*)val;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetArea(this Rectangle rect) => rect.Width * rect.Height;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            var result = (a < b).ToByte();
            return a * result - (b * (result - 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            var result = (a > b).ToByte();
            return a * result - (b * (result - 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetIfTrue(this int a, int b, bool condition)
        {
            var result = condition.ToByte();
            return a * result - (b * (result - 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetIfTrue(this float a, float b, bool condition)
        {
            var result = condition.ToByte();
            return a * result - (b * (result - 1));
        }

        public static int NextError(this Random rand, int degree) => rand.Next(-degree, degree + 1);

        public static double NextError(this Random rand, double degree) => rand.NextDouble() * degree * (rand.Next(0, 2) * 2 - 1);

        public static double Lerp(this double a, double b, double percent) => b * percent + a * (1 - percent);
        public static float Lerp(this float a, float b, float percent) => b * percent + a * (1 - percent);

        public static RECT ToRECT(this Rectangle rect)
        {
            RECT returnRect = new RECT();
            returnRect.Left = rect.Left;
            returnRect.Top = rect.Top; ;
            returnRect.Right = rect.Right;
            returnRect.Bottom = rect.Bottom;
            return returnRect;
        }

        public static Point GetCenter(this Rectangle targetRectangle)
        {
            return new Point(targetRectangle.Left + targetRectangle.Width / 2, targetRectangle.Top + targetRectangle.Height / 2);
        }

        public static bool GenerousContains(this Rectangle rect, Point target) => target.X >= rect.Left & target.X <= rect.Right & target.Y >= rect.Top & target.Y <= rect.Bottom;
        public static bool MoneyGrubbingContains(this Rectangle rect, Point target) => target.X > rect.Left & target.X < rect.Right & target.Y > rect.Top & target.Y < rect.Bottom;
        public static bool Contains(this RECT rect, RECT targetRect) => targetRect.Left >= rect.Left & targetRect.Right <= rect.Right & targetRect.Top >= rect.Top & targetRect.Bottom <= rect.Bottom;
        public static bool GenerousContains(this RECT rect, Point targetPoint) => rect.Left <= targetPoint.X & rect.Right >= targetPoint.X & rect.Top <= targetPoint.Y & rect.Bottom >= targetPoint.Y;
        public static RECT Pad(this RECT rect, int pad) => rect.Pad(pad, pad, pad, pad);
        public static RECT Pad(this RECT rect, int xPad, int yPad) => rect.Pad(xPad, yPad, xPad, yPad);
        public static RECT Pad(this RECT rect, int leftPad, int topPad, int rightPad, int bottomPad) => new RECT(rect.Left - leftPad, rect.Top - topPad, rect.Right + rightPad, rect.Bottom + bottomPad);
        public static double Distance(this Point A, Point B) => Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));


        /// <summary>
        /// finds the closest spot in which a rectangle can be fully contained by another
        /// </summary>
        /// <param name="a">the searching rectangle</param>
        /// <param name="b">the containing rectangle</param>
        /// <param name="position">the output of the new position</param>
        /// <param name="newSize">the output of the size that needed to fit within b</param>
        /// <returns>the distance to the new spot</returns>
        public static double GetClosestPosition(this Rectangle a, Rectangle b, out Point position, out Size newSize)
        {
            newSize = new Size(Min(a.Width, b.Width), Min(a.Height, b.Height));
            position = Point.Empty;
            position.Y = Max(a.Top, b.Top);
            position.Y = position.Y.SetIfTrue(b.Bottom - newSize.Height, b.Bottom < a.Bottom);
            position.X = Max(a.Left, b.Left);
            position.X = position.Y.SetIfTrue(b.Right - newSize.Width, b.Right < a.Right);

            return a.Location.Distance(position); 
        }
        public static Rectangle GetClosestBounds(this Rectangle a, IEnumerable<Rectangle> bounds)
        {
            double bestDist = int.MaxValue;
            Rectangle newBounds = Rectangle.Empty;
            foreach(var bee in bounds)
            {
                double tempDist = a.GetClosestPosition(bee, out var pos, out var size);
                if (tempDist < bestDist)
                {
                    bestDist = tempDist;
                    newBounds = new Rectangle(pos, size);
                }
            }
            return newBounds;
        }
        public static Rectangle ToRectangle(this RECT rect)
        {
            return new Rectangle(new Point(rect.Left, rect.Top), new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
        }

        public static HashSet<Rectangle> ToRectangles(this HashSet<RECT> rects)
        {
            HashSet<Rectangle> returnSet = new HashSet<Rectangle>();
            foreach (RECT rect in rects)
            {
                returnSet.Add(rect.ToRectangle());
            }
            return returnSet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="percent">0 - 100</param>
        /// <returns></returns>
        public static int Lerp(this int a, int b, double percent) => (int)Math.Round((a * percent + b * (1 - percent)));
        public static T RandomValue<T>(this T[] data) => data[Ghost.rand.Next(data.Length)];

    }
}
