﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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


        public static RECT ClampToLeft(this RECT currentRECT, RECT containerRECT) => 
            new RECT(containerRECT.Left - currentRECT.Width - 1, currentRECT.Top, containerRECT.Left - 1, currentRECT.Bottom);

        public static RECT ClampToTop(this RECT currentRECT, RECT containerRECT) =>
            new RECT(currentRECT.Left, containerRECT.Top - currentRECT.Height - 1, currentRECT.Right, containerRECT.Top - 1);

        public static RECT ClampToRight(this RECT currentRECT, RECT containerRECT) =>
            new RECT(containerRECT.Right + 1, currentRECT.Top, containerRECT.Right + currentRECT.Width + 1, currentRECT.Bottom);

        public static RECT ClampToBottom(this RECT currentRECT, RECT containerRECT) =>
            new RECT(currentRECT.Left, containerRECT.Bottom + 1, currentRECT.Right, containerRECT.Bottom + currentRECT.Height + 1);

        public static bool Intersects(this RECT rect1, RECT rect2) => rect1.Top <= rect2.Bottom & rect1.Bottom >= rect2.Top & rect1.Left <= rect2.Right & rect1.Right >= rect2.Left;

        public static bool ContainsLeft(this RECT containerRECT, RECT isContainedRECT) => containerRECT.Left <= isContainedRECT.Right & containerRECT.Left >= isContainedRECT.Left & (containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Bottom >= isContainedRECT.Top); //containerRECT contains the left of isContainedRECT
        public static bool ContainsTop(this RECT containerRECT, RECT isContainedRECT) => containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Top >= isContainedRECT.Top & (containerRECT.Left <= isContainedRECT.Right & containerRECT.Right >= isContainedRECT.Left); //containerRECT contains the top of isContainedRECT
        public static bool ContainsRight(this RECT containerRECT, RECT isContainedRECT) => containerRECT.Right >= isContainedRECT.Left & containerRECT.Right <= isContainedRECT.Right & (containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Bottom >= isContainedRECT.Top); //containerRECT contains the right of isContainedRECT
        public static bool ContainsBottom(this RECT containerRECT, RECT isContainedRECT) => containerRECT.Bottom >= isContainedRECT.Top & containerRECT.Bottom <= isContainedRECT.Bottom & (containerRECT.Left <= isContainedRECT.Right & containerRECT.Right >= isContainedRECT.Left); //containerRECT contains the bottom of isContainedRECT

        public static int GetLeftOverlap(this RECT isContainedRECT, RECT containerRECT)
        {
            if(!ContainsLeft(containerRECT, isContainedRECT))
            {
                return 0;
            }
            int bottomDifference = Math.Abs(containerRECT.Bottom - isContainedRECT.Bottom);
            int topDifference = Math.Abs(containerRECT.Top - isContainedRECT.Top);
            return (containerRECT.Height + isContainedRECT.Height - bottomDifference - topDifference) / 2;
        }

        public static int GetTopOverlap(this RECT isContainedRECT, RECT containerRECT)
        {
            if (!ContainsTop(containerRECT, isContainedRECT))
            {
                return 0;
            }
            int leftDifference = Math.Abs(containerRECT.Left - isContainedRECT.Left);
            int rightDifference = Math.Abs(containerRECT.Right - isContainedRECT.Right);
            return (containerRECT.Width + isContainedRECT.Width - leftDifference - rightDifference) / 2;
        }

        public static int GetRightOverlap(this RECT isContainedRECT, RECT containerRECT)
        {
            if (!ContainsRight(containerRECT, isContainedRECT))
            {
                return 0;
            }
            int bottomDifference = Math.Abs(containerRECT.Bottom - isContainedRECT.Bottom);
            int topDifference = Math.Abs(containerRECT.Top - isContainedRECT.Top);
            return (containerRECT.Height + isContainedRECT.Height - bottomDifference - topDifference) / 2;
        }

        public static int GetBottomOverlap(this RECT isContainedRECT, RECT containerRECT)
        {
            if (!ContainsBottom(containerRECT, isContainedRECT))
            {
                return 0;
            }
            int leftDifference = Math.Abs(containerRECT.Left - isContainedRECT.Left);
            int rightDifference = Math.Abs(containerRECT.Right - isContainedRECT.Right);
            return (containerRECT.Width + isContainedRECT.Width - leftDifference - rightDifference) / 2;
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
        /// 
        interface IAwesomeness
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

            return a.Location.Distance(position); 
        }
        public static Rectangle GetClosestBounds(this Rectangle a, Vector2 aspectRatio, IEnumerable<Rectangle> bounds)
        {
            double bestDist = int.MaxValue;
            Rectangle newBounds = Rectangle.Empty;
            foreach(var bee in bounds)
            {
                double tempDist = a.GetClosestPosition(bee, aspectRatio, out var pos, out var size);
                if (tempDist < bestDist)
                {
                    bestDist = tempDist;
                    newBounds = new Rectangle(pos, size);
                }
            }
            return newBounds;
        }

        public static RECT GetBiggestRECT(this RECT currentRECT, Vector2 maxSize, HashSet<RECT> obstacles)
        {
            double distance = Math.Sqrt((maxSize.X - currentRECT.Width) * (maxSize.X - currentRECT.Width) + (maxSize.Y - currentRECT.Height) * (maxSize.Y - currentRECT.Height));
            RECT biggestRECT = currentRECT;
            double percentIncrement = 1 / distance;
            double currentPercent = percentIncrement;

            double newWidth = currentRECT.Width;
            double newHeight = currentRECT.Height;

            RECT potentialBiggestRECT;

            while (currentPercent < 1)
            {

                newWidth = ((double)currentRECT.Width).Lerp(maxSize.X, currentPercent) + .99999;
                newHeight = ((double)currentRECT.Height).Lerp(maxSize.Y, currentPercent) + .99999;

                currentPercent += percentIncrement;

                potentialBiggestRECT = new RECT((int)(biggestRECT.Left - (newWidth - biggestRECT.Width) / 2), (int)(biggestRECT.Top - (newHeight - biggestRECT.Height) / 2), (int)(biggestRECT.Right + (newWidth - biggestRECT.Width) / 2), (int)(biggestRECT.Bottom + (newHeight - biggestRECT.Height) / 2));
                foreach (RECT obstacle in obstacles)
                {
                    if(potentialBiggestRECT.Intersects(obstacle))
                    {
                        return biggestRECT;
                    }
                    biggestRECT = potentialBiggestRECT;
                }
            }
            return biggestRECT;
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
