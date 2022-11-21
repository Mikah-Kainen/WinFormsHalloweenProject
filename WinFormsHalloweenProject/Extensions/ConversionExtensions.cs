using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using WinformsHalloweenProject;

namespace WinformsHalloweenProject.Extensions
{
    using static WinFormsHalloweenProject.Ghost;

    public static partial class Extensions
    {
        public static RECT ToRECT(this Rectangle rect)
        {
            RECT returnRect = new RECT();
            returnRect.Left = rect.Left;
            returnRect.Top = rect.Top; ;
            returnRect.Right = rect.Right;
            returnRect.Bottom = rect.Bottom;
            return returnRect;
        }
        public static Rectangle ToRectangle(this IRectangle rect)
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
        public static Vector2 ToVector2(this Point a) => new Vector2(a.X, a.Y);
        public static Point ToPoint(this Vector2 a) => new Point((int)a.X, (int)a.Y);
    }
}
