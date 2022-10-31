using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsHalloweenProject
{
    using RECT = Rectangle;
    using static Pain;

    public class RectangleComparer : IComparer<RECT>
    {
        class SadnessException : Exception
        {
            public SadnessException()
                : base(":(") { }
        }

        public Vector2 Start { get; set; }
        public Vector2 AspectRatio { get; set; } = Vector2.One;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(RECT x, RECT y)
        {
            float xSize = GetRectSize(x, AspectRatio) + ((Vector2.Distance(new Vector2(x.Left + x.Width / 2, x.Top + x.Height / 2), Start) < Vector2.Distance(new Vector2(y.Left + y.Width / 2, y.Top + y.Height / 2), Start)).ToByte() * 2 - 1) / 2f;
            float ySize = GetRectSize(y, AspectRatio);

            //if (xSize == ySize) throw new SadnessException();

            int a = (xSize < ySize).ToByte();
            int b = (xSize > ySize).ToByte();

            return a - b;
        }

        public static RectangleComparer Instance { get; } = new RectangleComparer();
    }

    public static class Pain
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<RECT> FindBiggestSpace(HashSet<RECT> rectangles, Size bounds)
        {
            var comparer = RectangleComparer.Instance;
            List<RECT> rects = rectangles.ToList();
            rects.Sort(comparer);

            LinkedList<RECT> spaces = new LinkedList<RECT>();
            spaces.AddFirst(new RECT(Point.Empty, bounds));


            foreach (var rect in rects)
            {
                for (LinkedListNode<RECT> traveller = spaces.First, next; traveller != null; traveller = next)
                {
                    next = traveller.Next;
                    var space = traveller.Value;
                    if (rect.IntersectsWith(space))
                    {
                        spaces.Remove(traveller);
                        if (rect.Left > space.Left)
                        {
                            InsertInto(spaces, new RECT(space.Left, space.Top, rect.Left - space.Left, space.Height));
                        }
                        if (rect.Right < space.Right)
                        {
                            InsertInto(spaces, new RECT(rect.Right, space.Top, space.Right - rect.Right, space.Height));
                        }
                        if (rect.Top > space.Top)
                        {
                            InsertInto(spaces, new RECT(space.Left, space.Top, space.Width, rect.Top - space.Top));
                        }
                        if (rect.Bottom < space.Bottom)
                        {
                            InsertInto(spaces, new RECT(space.Left, rect.Bottom, space.Width, space.Bottom - rect.Bottom));
                        }
                    }
                }
            }
            return spaces;
        }
        /// <summary>
        /// Finds the rectangle best suited to fit an item with a certain aspect ratio at some position
        /// </summary>
        /// <param name="location">The location to search from</param>
        /// <param name="aspectRatio">The aspect ratio being searched for</param>
        /// <param name="rectangles">The candidate rectangles</param>
        /// <param name="biggestSize">Outputs the measurment of size for the chosen rectangle</param>
        /// <returns>The best suited rectangle</returns>

        public static float GetRectSize(RECT rect, Vector2 aspectRatio) => Math.Min(rect.Width * (1 / aspectRatio.X), rect.Height * (1 / aspectRatio.Y));
        public static RECT GetBiggestRectangle(Point location, Vector2 aspectRatio, IEnumerable<RECT> rectangles, out float biggestSize)
        {
            aspectRatio /= Math.Max(aspectRatio.X, aspectRatio.Y);

            RECT biggestRect = RECT.Empty;
            biggestSize = 0;
            foreach (var rect in rectangles)
            {
                float newSize;// = Math.Min(rect.Width * aspectRatio.Width, rect.Height * aspectRatio.Height);
                if (rect.GenerousContains(location) && (newSize = GetRectSize(rect, aspectRatio)) > biggestSize)
                {
                    biggestSize = newSize;
                    biggestRect = rect;
                }
            }
            return biggestRect;
        }
        static bool InsertInto(LinkedList<RECT> rects, RECT newRect)
        {
            for (LinkedListNode<RECT> traveller = rects.First, next; traveller != null; traveller = next)
            {
                next = traveller.Next;
                if (traveller.Value.Contains(newRect))
                {
                    return false;
                }
                if (RectangleComparer.Instance.Compare(traveller.Value, newRect) >= 0)
                {
                    rects.AddBefore(traveller, newRect);
                    for (; traveller != null; traveller = next)
                    {
                        next = traveller.Next;
                        if (newRect.Contains(traveller.Value))
                        {
                            rects.Remove(traveller);
                        }
                    }
                    return true;
                }
            }
            rects.AddLast(newRect);
            return true;
        }     
    }
}
