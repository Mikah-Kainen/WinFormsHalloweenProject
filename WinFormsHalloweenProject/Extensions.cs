﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using WinformsHalloweenProject;

namespace WinFormsHalloweenProject
{
    using static WinFormsHalloweenProject.Ghost;

    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Lerp(this Point a, Point b, float percent) => new Point(a.X.Lerp(b.X, percent), a.Y.Lerp(b.Y, percent));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(this Vector2 a, Vector2 b, float percent) => new Vector2(a.X.Lerp(b.X, percent), a.Y.Lerp(b.Y, percent));
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
        public static float Min(float a, float b)
        {
            var result = (a < b).ToByte();
            return a * result - (b * (result - 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            var result = (a > b).ToByte();
            return a * result - (b * (result - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetIfTrue(this int a, int b, bool condition)
        {
            var result = condition.ToByte();
            return b * result - (a * (result - 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SetIfTrue(this float a, float b, bool condition)
        {
            var result = condition.ToByte();
            return b * result - (a * (result - 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetIfTrue(this float a, float b, bool condition)
        {
            var result = condition.ToByte();
            return a * result - (b * (result - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SetIfTrue(this float a, float b, bool condition)
        {
            var result = condition.ToByte();
            return b * result - (a * (result - 1));
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
        public static Vector2 GetCenter(this IRectangle rect)
        {
            return new Vector2(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }


        public static FloatTangle ClampToLeft(this IRectangle currentRECT, IRectangle containerRECT) =>
            new FloatTangle(containerRECT.Left - currentRECT.Width, currentRECT.Top, containerRECT.Left, currentRECT.Bottom);

        public static FloatTangle ClampToTop(this IRectangle currentRECT, IRectangle containerRECT) =>
            new FloatTangle(currentRECT.Left, containerRECT.Top - currentRECT.Height, currentRECT.Right, containerRECT.Top);

        public static FloatTangle ClampToRight(this IRectangle currentRECT, IRectangle containerRECT) =>
            new FloatTangle(containerRECT.Right, currentRECT.Top, containerRECT.Right + currentRECT.Width, currentRECT.Bottom);

        public static FloatTangle ClampToBottom(this IRectangle currentRECT, IRectangle containerRECT) =>
            new FloatTangle(currentRECT.Left, containerRECT.Bottom, currentRECT.Right, containerRECT.Bottom + currentRECT.Height);

        public static bool Intersects(this IRectangle rect1, IRectangle rect2) => rect1.Intersects(rect2);

        public static bool ContainsLeft(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Left <= isContainedRECT.Right & containerRECT.Left >= isContainedRECT.Left & (containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Bottom >= isContainedRECT.Top); //containerIRectangle contains the left of isContainedRECT
        public static bool ContainsTop(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Top >= isContainedRECT.Top & (containerRECT.Left <= isContainedRECT.Right & containerRECT.Right >= isContainedRECT.Left); //containerIRectangle contains the top of isContainedRECT
        public static bool ContainsRight(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Right >= isContainedRECT.Left & containerRECT.Right <= isContainedRECT.Right & (containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Bottom >= isContainedRECT.Top); //containerIRectangle contains the right of isContainedRECT
        public static bool ContainsBottom(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Bottom >= isContainedRECT.Top & containerRECT.Bottom <= isContainedRECT.Bottom & (containerRECT.Left <= isContainedRECT.Right & containerRECT.Right >= isContainedRECT.Left); //containerIRectangle contains the bottom of isContainedRECT

        public static float GetLeftOverlap(this IRectangle isContainedRECT, IRectangle containerRECT)
        {
            if (!ContainsLeft(containerRECT, isContainedRECT))
            {
                return 0;
            }
            float bottomDifference = Math.Abs(containerRECT.Bottom - isContainedRECT.Bottom);
            float topDifference = Math.Abs(containerRECT.Top - isContainedRECT.Top);
            return (containerRECT.Height + isContainedRECT.Height - bottomDifference - topDifference) / 2;
        }

        public static float GetTopOverlap(this IRectangle isContainedRECT, IRectangle containerRECT)
        {
            if (!ContainsTop(containerRECT, isContainedRECT))
            {
                return 0;
            }
            float leftDifference = Math.Abs(containerRECT.Left - isContainedRECT.Left);
            float rightDifference = Math.Abs(containerRECT.Right - isContainedRECT.Right);
            return (containerRECT.Width + isContainedRECT.Width - leftDifference - rightDifference) / 2;
        }

        public static float GetRightOverlap(this IRectangle isContainedRECT, IRectangle containerRECT)
        {
            if (!ContainsRight(containerRECT, isContainedRECT))
            {
                return 0;
            }
            float bottomDifference = Math.Abs(containerRECT.Bottom - isContainedRECT.Bottom);
            float topDifference = Math.Abs(containerRECT.Top - isContainedRECT.Top);
            return (containerRECT.Height + isContainedRECT.Height - bottomDifference - topDifference) / 2;
        }

        public static float GetBottomOverlap(this IRectangle isContainedRECT, IRectangle containerRECT)
        {
            if (!ContainsBottom(containerRECT, isContainedRECT))
            {
                return 0;
            }
            float leftDifference = Math.Abs(containerRECT.Left - isContainedRECT.Left);
            float rightDifference = Math.Abs(containerRECT.Right - isContainedRECT.Right);
            return (containerRECT.Width + isContainedRECT.Width - leftDifference - rightDifference) / 2;
        }

        public static bool GenerousContains(this Rectangle rect, Point target) => target.X >= rect.Left & target.X <= rect.Right & target.Y >= rect.Top & target.Y <= rect.Bottom;
        public static bool MoneyGrubbingContains(this Rectangle rect, Point target) => target.X > rect.Left & target.X < rect.Right & target.Y > rect.Top & target.Y < rect.Bottom;
        public static bool Contains(this IRectangle rect, IRectangle targetRect) => rect.Contains(targetRect);
        public static bool MoneyGrubbingContains(this IRectangle rect, Point targetPoint) => rect.MoneyGrubbingContains(targetPoint);
        public static bool GenerousContains(this IRectangle rect, Point targetPoint) => rect.GenerousContains(targetPoint);
        public static RECT Pad(this RECT rect, int pad) => rect.Pad(pad, pad, pad, pad);
        public static RECT Pad(this RECT rect, int xPad, int yPad) => rect.Pad(xPad, yPad, xPad, yPad);
        public static RECT Pad(this RECT rect, int leftPad, int topPad, int rightPad, int bottomPad) => new RECT(rect.Left - leftPad, rect.Top - topPad, rect.Right + rightPad, rect.Bottom + bottomPad);
        public static double Distance(this Point A, Point B) => Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
        public static Vector2 ToVector2(this Point a) => new Vector2(a.X, a.Y);
        public static Point ToPoint(this Vector2 a) => new Point((int)a.X, (int)a.Y);

        /// <summary>
        /// finds the closest spot in which a rectangle can be fully contained by another
        /// </summary>
        /// <param name="a">the searching rectangle</param>
        /// <param name="b">the containing rectangle</param>
        /// <param name="position">the output of the new position</param>
        /// <param name="newSize">the output of the size that needed to fit within b</param>
        /// <returns>the distance to the new spot</returns>
        /// 
        interface IAwesomeness
        public static double GetClosestPosition(this IRectangle a, Rectangle b, out Vector2 position, out Vector2 newSize)
        {
            int BeAwesome()
            {
                int a = 0;
                a++;
                a--;
                a++;
                return 'I' + ' ' + 'a' + 'm' + ' ' + 'a' + 'w' + 'e' + 's' + 'o' + 'm' + 'e';
            }
        } 
        public static double GetClosestPosition(this Rectangle a, Rectangle b, Vector2 aspectRatio, out Point position, out Size newSize)
        {
            aspectRatio /= Math.Max(aspectRatio.X, aspectRatio.Y);

            Vector2 delta = new Vector2((a.Width - Min(a.Width, b.Width)) / aspectRatio.X, (a.Height - Min(a.Height, b.Height) / aspectRatio.Y));
            float biggestDifference = delta.X;
            biggestDifference = biggestDifference.SetIfTrue(delta.Y, delta.Y > delta.X);

            newSize = new Size((int)(a.Width - biggestDifference * aspectRatio.X), (int)(a.Height - biggestDifference * aspectRatio.Y));
            position = Point.Empty;
            position.Y = Max(a.Top, b.Top);
            position.Y = position.Y.SetIfTrue(b.Bottom - newSize.Height, b.Bottom < a.Bottom);
            position.X = Max(a.Left, b.Left);
            position.X = position.X.SetIfTrue(b.Right - newSize.Width, b.Right < a.Right);
            newSize = new Vector2(Min((int)a.Width, b.Width), Min((int)a.Height, b.Height));
            position = Vector2.Zero;
            position.Y = Max((int)a.Top, b.Top);
            position.Y = position.Y.SetIfTrue(b.Bottom - (int)newSize.Y, b.Bottom < a.Bottom);
            position.X = Max((int)a.Left, b.Left);
            position.X = position.X.SetIfTrue(b.Right - (int)newSize.X, b.Right < a.Right);

            return Vector2.Distance(position, a.Location);
        }
        public static FloatTangle GetClosestBounds(this IRectangle a, IEnumerable<Rectangle> bounds)
        public static Rectangle GetClosestBounds(this Rectangle a, Vector2 aspectRatio, IEnumerable<Rectangle> bounds)
        {
            double bestDist = int.MaxValue;
            FloatTangle newBounds = default;
            foreach (var bee in bounds)
            {
                double tempDist = a.GetClosestPosition(bee, aspectRatio, out var pos, out var size);
                if (tempDist < bestDist)
                {
                    bestDist = tempDist;
                    newBounds = new FloatTangle(pos, size);
                }
            }
            return newBounds;
        }

        public static RECT GetBiggestRECT(this RECT currentRECT, Size maxSize, HashSet<RECT> obstacles)
        {
            double distance = Math.Sqrt((pointB.X - pointA.X) * (pointB.X - pointA.X) + (pointB.Y - pointA.Y) * (pointB.Y - pointA.Y));
            double percentIncrement = 1 / distance;
            double currentPercent = percentIncrement;
            double xValue;
            double yValue;
            Point currentPoint;

            bool straight = pointA.X == pointB.X | pointA.Y == pointB.Y;

            while (currentPercent < 1)
            {
                xValue = ((double)pointA.X).Lerp(pointB.X, currentPercent) + .99999;
                yValue = ((double)pointA.Y).Lerp(pointB.Y, currentPercent) + .99999;

                currentPercent += percentIncrement;

                currentPoint = new Point((int)xValue, (int)yValue);
                int generousContainment = 0;
                foreach (Ghost.RECT rect in activeRectangles)
                {
                    generousContainment += rect.Pad(1).GenerousContains(currentPoint).ToByte();
                    if ((straight.ToByte() + generousContainment > 1 || rect.Contains(currentPoint)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static Rectangle ToRectangle(this RECT rect)
        {
            return new Rectangle(new Point((int)rect.Left, (int)rect.Top), new Size((int)(rect.Right - rect.Left), (int)(rect.Bottom - rect.Top)));
        }

        public static HashSet<Rectangle> ToRectangles<T>(this HashSet<T> rects) where T : IRectangle
        {
            HashSet<Rectangle> returnSet = new HashSet<Rectangle>();
            foreach (IRectangle rect in rects)
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
        public static int Lerp(this int a, int b, float percent) => (int)Math.Round((a * percent + b * (1 - percent)));
        public static T RandomValue<T>(this T[] data) => data[Ghost.rand.Next(data.Length)];

    }
}
