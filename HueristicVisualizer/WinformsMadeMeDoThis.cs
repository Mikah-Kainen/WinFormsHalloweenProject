using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rectangle_Hueristic
{
    using RECT = Rectangle;
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte ToByte(this bool val) => ToByte(&val);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte ToByte(bool* val) => *(byte*)val;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetArea(RECT rect) => rect.Width * rect.Height;
    }

    class RectangleComparer : IComparer<RECT>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(RECT x, RECT y)
        {
            int yArea = y.Width * y.Height;
            int xArea = x.Width * x.Height;

            byte a = (xArea < yArea).ToByte();
            byte b = (xArea > yArea).ToByte();

            return a - b;
        }

        public static RectangleComparer Instance { get; } = new RectangleComparer();
    }

    public static class RectangleHueristic
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<RECT> FindBiggestSpace(HashSet<RECT> rectangles, Size bounds)
        {
            var comparer = RectangleComparer.Instance;
            List<RECT> rects = rectangles.ToList<RECT>();
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
