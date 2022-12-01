using System.Numerics;
using WinformsHalloweenProject.RectangleStuff;

namespace WinformsHalloweenProject.Extensions
{
    using static WinFormsHalloweenProject.Ghost;

    public static partial class Extensions
    {
        #region IntersectsFunctions
        public static bool Intersects(this IRectangle rect1, IRectangle rect2) => rect1.Intersects(rect2);
        public static bool IntersectsLeft(this IRectangle a, IRectangle other, float marginOfError) => Math.Abs(a.Left - other.Right) <= marginOfError;
        public static bool IntersectsRight(this IRectangle a, IRectangle other, float marginOfError) => Math.Abs(a.Right - other.Left) <= marginOfError;
        public static bool IntersectsTop(this IRectangle a, IRectangle other, float marginOfError) => Math.Abs(a.Top - other.Bottom) <= marginOfError;
        public static bool IntersectsBottom(this IRectangle a, IRectangle other, float marginOfError) => Math.Abs(a.Bottom - other.Top) <= marginOfError;
        #endregion

        #region ContainsFunctions
        /// <summary>
        /// container's right intersects with contained left
        /// </summary>
        public static bool ContainsLeft(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Left <= isContainedRECT.Right & containerRECT.Left >= isContainedRECT.Left & containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Bottom >= isContainedRECT.Top; //containerIRectangle contains the left of isContainedRECT        
        /// <summary>
        /// container's bottom intersects with contained top
        /// </summary>
        public static bool ContainsTop(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Top >= isContainedRECT.Top & containerRECT.Left <= isContainedRECT.Right & containerRECT.Right >= isContainedRECT.Left; //containerIRectangle contains the top of isContainedRECT

        /// <summary>
        /// container's left intersects with contained right
        /// </summary>
        public static bool ContainsRight(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Right >= isContainedRECT.Left & containerRECT.Right <= isContainedRECT.Right & containerRECT.Top <= isContainedRECT.Bottom & containerRECT.Bottom >= isContainedRECT.Top; //containerIRectangle contains the right of isContainedRECT

        /// <summary>
        /// container's top intersects with contained bottom
        /// </summary>
        public static bool ContainsBottom(this IRectangle containerRECT, IRectangle isContainedRECT) => containerRECT.Bottom >= isContainedRECT.Top & containerRECT.Bottom <= isContainedRECT.Bottom & containerRECT.Left <= isContainedRECT.Right & containerRECT.Right >= isContainedRECT.Left; //containerIRectangle contains the bottom of isContainedRECT

        public static bool GenerousContains(this IRectangle rect, Vector2 target) => target.X >= rect.Left & target.X <= rect.Right & target.Y >= rect.Top & target.Y <= rect.Bottom;
        public static bool MoneyGrubbingContains(this Rectangle rect, Point target) => target.X > rect.Left & target.X < rect.Right & target.Y > rect.Top & target.Y < rect.Bottom;

        public static bool GenerousContains(this Rectangle rect, Point target) => target.X >= rect.Left & target.X <= rect.Right & target.Y >= rect.Top & target.Y <= rect.Bottom;
        public static bool MoneyGrubbingContains(this IRectangle rect, Vector2 target) => target.X > rect.Left & target.X < rect.Right & target.Y > rect.Top & target.Y < rect.Bottom;
        public static bool Contains(this IRectangle rect, IRectangle targetRect) => rect.Contains(targetRect);
        public static bool MoneyGrubbingContains(this IRectangle rect, Point targetPoint) => rect.MoneyGrubbingContains(targetPoint);
        public static bool GenerousContains(this IRectangle rect, Point targetPoint) => rect.GenerousContains(targetPoint);
        #endregion

        #region PadFunctions
        public static RECT Pad(this RECT rect, int pad) => rect.Pad(pad, pad, pad, pad);
        public static RECT Pad(this RECT rect, int xPad, int yPad) => rect.Pad(xPad, yPad, xPad, yPad);
        public static RECT Pad(this RECT rect, int leftPad, int topPad, int rightPad, int bottomPad) => new RECT(rect.Left - leftPad, rect.Top - topPad, rect.Right + rightPad, rect.Bottom + bottomPad);
        #endregion

        #region GetFunctions
        public static Point GetCenter(this Rectangle targetRectangle)
        {
            return new Point(targetRectangle.Left + targetRectangle.Width / 2, targetRectangle.Top + targetRectangle.Height / 2);
        }
        public static Vector2 GetCenter(this IRectangle rect)
        {
            return new Vector2(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        [Flags]
        enum Directions : byte
        {
            None,
            Left = 1,
            Right = 2,
            Horizontal = Left | Right,
            Top = 4,
            Bottom = 8,
            Vertical = Top | Bottom
        }
        static Stack<Directions> dieRections = new Stack<Directions>();
        static Dictionary<Directions, Func<RECT, FloatTangle, bool>> mods = new Dictionary<Directions, Func<RECT, FloatTangle, bool>>();
        /// <summary>
        /// this function is NOT multithread safe!
        /// </summary>
        /// <param name="a"></param>
        /// <param name="startingLocation"></param>
        /// <param name="maxSize"></param>
        /// <param name="obstacles"></param>
        /// <returns></returns>
        public static FloatTangle GetLargestBounds(this FloatTangle a, Vector2 startingLocation, Vector2 previousLocation, Vector2 maxSize, IEnumerable<RECT> obstacles, Rectangle ScreenBounds)
        {
            Vector2 aspectRatio = maxSize / Max(maxSize.X, maxSize.Y);

            var amountMoved = startingLocation - previousLocation;
            a.X += amountMoved.X; a.Y += amountMoved.Y;

            Directions fails = Directions.None;

            mods[Directions.Left] = (r, f) => r.ContainsRight(a);
            mods[Directions.Right] = (r, f) => r.ContainsLeft(a);
            mods[Directions.Top] = (r, f) => r.ContainsBottom(a);
            mods[Directions.Bottom] = (r, f) => r.ContainsTop(a);

            if (a.MoneyGrubbingContains(startingLocation))
            {
                foreach (var obstacle in obstacles)
                {
                    if (obstacle.Intersects(a))
                    {
                        a = new FloatTangle(startingLocation, Vector2.Zero);
                        break;
                    }
                }
            }
            else
            {
                a = new FloatTangle(startingLocation, Vector2.Zero);

            }

            while (a.Width < maxSize.X & !fails.HasFlag(Directions.Vertical) & !fails.HasFlag(Directions.Horizontal))
            {
                if (a.Left <= ScreenBounds.Left)
                {
                    fails |= Directions.Left;
                }
                if (a.Right >= ScreenBounds.Right)
                {
                    fails |= Directions.Right;
                }
                if (a.Top <= ScreenBounds.Top)
                {
                    fails |= Directions.Top;
                }
                if (a.Bottom >= ScreenBounds.Bottom)
                {
                    fails |= Directions.Bottom;
                }

                foreach (var obstacle in obstacles)
                {
                    if (obstacle.Intersects(a))
                    {
                        foreach (var pair in mods)
                        {
                            if (pair.Value(obstacle, a))
                            {
                                a.Left += fails.GetInverseNthBit(0) * (fails.GetNthBit(1) + 1) * aspectRatio.X;
                                a.Right -= fails.GetInverseNthBit(1) * (fails.GetNthBit(0) + 1) * aspectRatio.X;
                                a.Top += fails.GetInverseNthBit(2) * (fails.GetNthBit(3) + 1) * aspectRatio.Y;
                                a.Bottom -= fails.GetInverseNthBit(3) * (fails.GetNthBit(2) + 1) * aspectRatio.Y;
                                fails |= pair.Key;
                                dieRections.Push(pair.Key);
                            }
                        }
                        while (dieRections.Count > 0)
                        {
                            mods.Remove(dieRections.Pop());
                        }
                    }
                }
                a.Left -= fails.GetInverseNthBit(0) * (fails.GetNthBit(1) + 1) * aspectRatio.X;
                a.Right += fails.GetInverseNthBit(1) * (fails.GetNthBit(0) + 1) * aspectRatio.X;
                a.Top -= fails.GetInverseNthBit(2) * (fails.GetNthBit(3) + 1) * aspectRatio.Y;
                a.Bottom += fails.GetInverseNthBit(3) * (fails.GetNthBit(2) + 1) * aspectRatio.Y;
                //a.Left -= aspectRatio / 2;
            }
            return a;
        }
        public static double GetClosestPosition(this IRectangle a, Rectangle b, Vector2 aspectRatio, out Vector2 position, out Vector2 newSize)
        {
            aspectRatio /= Math.Max(aspectRatio.X, aspectRatio.Y);

            Vector2 delta = new Vector2((a.Width - Min(a.Width, b.Width)) / aspectRatio.X, a.Height - Min(a.Height, b.Height) / aspectRatio.Y);
            float biggestDifference = delta.X;
            biggestDifference = biggestDifference.SetIfTrue(delta.Y, delta.Y > delta.X);

            newSize = new Vector2(a.Width - biggestDifference * aspectRatio.X, a.Height - biggestDifference * aspectRatio.Y);
            position = Vector2.Zero;
            position.Y = Max(a.Top, b.Top);
            position.Y = position.Y.SetIfTrue(b.Bottom - newSize.Y, b.Bottom < a.Bottom);
            position.X = Max(a.Left, b.Left);
            position.X = position.X.SetIfTrue(b.Right - newSize.X, b.Right < a.Right);
            newSize = new Vector2(Min((int)a.Width, b.Width), Min((int)a.Height, b.Height));
            position = Vector2.Zero;
            position.Y = Max((int)a.Top, b.Top);
            position.Y = position.Y.SetIfTrue(b.Bottom - (int)newSize.Y, b.Bottom < a.Bottom);
            position.X = Max((int)a.Left, b.Left);
            position.X = position.X.SetIfTrue(b.Right - (int)newSize.X, b.Right < a.Right);

            return Vector2.Distance(position, a.Location);
        }
        public static FloatTangle GetClosestBounds(this IRectangle a, Vector2 aspectRatio, IEnumerable<Rectangle> bounds)
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
        public static FloatTangle GetBiggestRECT(this FloatTangle currentRECT, Vector2 maxSize, HashSet<RECT> obstacles)
        {
            Vector2 center = new Vector2(currentRECT.Left + currentRECT.Width / 2, currentRECT.Top + currentRECT.Height / 2);

            double distance = Math.Sqrt(maxSize.X * maxSize.X + maxSize.Y * maxSize.Y);
            FloatTangle biggestRECT = new FloatTangle(new Vector2(center.X, center.Y), Vector2.Zero);
            double percentIncrement = 1 / distance;
            double currentPercent = percentIncrement;

            double newWidth = 0;
            double newHeight = 0;

            FloatTangle potentialBiggestRECT;

            bool maxWidthReached = false;
            bool maxHeightReached = false;

            while (currentPercent < 1)
            {
                if (maxWidthReached & maxHeightReached)
                {
                    return biggestRECT;
                }
                if (newWidth < maxSize.X)
                {
                    newWidth = 0d.Lerp(maxSize.X, currentPercent);
                }
                else
                {
                    newWidth = maxSize.X;
                    maxWidthReached = true;
                }
                if (newHeight < maxSize.Y)
                {
                    newHeight = 0d.Lerp(maxSize.Y, currentPercent);
                }
                else
                {
                    newHeight = maxSize.Y;
                    maxHeightReached = true;
                }

                currentPercent += percentIncrement;
                if (newWidth < biggestRECT.Width)
                {

                }
                if (newHeight < biggestRECT.Height)
                {

                }
                potentialBiggestRECT = new FloatTangle((float)(center.X - newWidth / 2), (float)(center.Y - newHeight / 2), (float)(center.X + newWidth / 2), (float)(center.Y + newHeight / 2));
                foreach (RECT obstacle in obstacles)
                {
                    if (potentialBiggestRECT.Intersects(obstacle))
                    {
                        return biggestRECT;
                    }
                    biggestRECT = potentialBiggestRECT;
                }
            }
            return biggestRECT;
        }
        #endregion
    }
}
