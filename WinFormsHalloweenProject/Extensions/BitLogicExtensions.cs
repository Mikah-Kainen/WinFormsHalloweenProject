using System;
using System.Runtime.CompilerServices;

namespace WinformsHalloweenProject.Extensions
{
    public static partial class Extensions
    {
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
            return a * result - b * (result - 1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            var result = (a > b).ToByte();
            return a * result - b * (result - 1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            var result = (a < b).ToByte();
            return a * result - b * (result - 1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            var result = (a > b).ToByte();
            return a * result - b * (result - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetIfTrue(this int a, int b, bool condition)
        {
            var result = condition.ToByte();
            return b * result - a * (result - 1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SetIfTrue(this float a, float b, bool condition)
        {
            var result = condition.ToByte();
            return b * result - a * (result - 1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetIfTrue(this float a, float b, bool condition)
        {
            var result = condition.ToByte();
            return a * result - b * (result - 1);
        }

        static byte GetInverseNthBit(this Directions val, byte n) => ((byte)val).GetInverseNthBit(n);
        public static byte GetInverseNthBit(this byte val, byte n) => (byte)(~(val >> n) & 1);
        static byte GetNthBit(this Directions val, byte n) => ((byte)val).GetNthBit(n);
        public static byte GetNthBit(this byte val, byte n) => (byte)(val >> n & 1);
    }
}