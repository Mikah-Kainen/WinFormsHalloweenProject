using System.Numerics;
using System.Runtime.CompilerServices;
using WinformsHalloweenProject.RectangleStuff;

namespace WinformsHalloweenProject.Extensions
{
    using static WinFormsHalloweenProject.Ghost;
    public static partial class Extensions
    {
        #region LerpFunctions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Lerp(this Point a, Point b, float percent) => new Point(a.X.Lerp(b.X, percent), a.Y.Lerp(b.Y, percent));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(this Vector2 a, Vector2 b, float percent) => new Vector2(a.X.Lerp(b.X, percent), a.Y.Lerp(b.Y, percent));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle Lerp(this IRectangle rect, IRectangle other, float factor)
        {
            rect.X = rect.X.Lerp(other.X, factor);
            rect.Y = rect.Y.Lerp(other.Y, factor);
            rect.Width = rect.Width.Lerp(other.Width, factor);
            rect.Height = rect.Height.Lerp(other.Height, factor);
            return rect;
        }
        public static int Lerp(this int a, int b, float percent) => (int)Math.Round(a * percent + b * (1 - percent));
        public static double Lerp(this double a, double b, double percent) => b * percent + a * (1 - percent);
        public static float Lerp(this float a, float b, float percent) => b * percent + a * (1 - percent);
        #endregion

        #region RandomValueFunctions
        public static int NextError(this Random rand, int degree) => rand.Next(-degree, degree + 1);
        public static double NextError(this Random rand, double degree) => rand.NextDouble() * degree * (rand.Next(0, 2) * 2 - 1);
        public static T RandomValue<T>(this T[] data) => data[rand.Next(data.Length)];
        #endregion

        #region DistanceFunctions
        public static double Distance(this Point A, Point B) => Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
        #endregion
    }
}
