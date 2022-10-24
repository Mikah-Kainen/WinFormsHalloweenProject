using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rectangle_Hueristic
{
    using RECT = Rectangle;
    static class Extensions
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
}
