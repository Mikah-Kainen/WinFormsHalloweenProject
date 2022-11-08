using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static WinFormsHalloweenProject.Ghost;

namespace WinformsHalloweenProject
{
    #region EddenIDK
    using TotallyNotaFloatISwear = System.Single;
    #endregion
    public interface IRectangle
    {
        float X { get; set; }
        float Y { get; set; }
        float Width { get; /*set;*/ }
        float Height { get; /*set;*/ }
        float Left { get; }
        float Right { get; }
        float Top { get; }
        float Bottom { get; }
        Vector2 Location { get; }
        Vector2 Center
        {
            get => new Vector2(X + Width / 2, Y + Height / 2);
            set
            {
                X -= value.X - Width / 2;
                Y -= value.Y - Height / 2;
            }
        }
        bool Intersects(IRectangle other) => Top < other.Bottom & Bottom > other.Top & Left < other.Right & Right > other.Left;
        bool Intersects(Rectangle other) => Top < other.Bottom & Bottom > other.Top & Left < other.Right & Right > other.Left;
        public bool MoneyGrubbingContains(Point target) => target.X > Left & target.X < Right & target.Y > Top & target.Y < Bottom;
        public bool Contains(IRectangle other) => other.Left >= Left & other.Right <= Right & other.Top >= Top & other.Bottom <= Bottom;
        public bool GenerousContains(Point target) => Left <= target.X & Right >= target.X & Top <= target.Y & Bottom >= target.Y;

    }

    public struct FloatTangle : IRectangle
    {
        public FloatTangle(float left, float top, float right, float bottom)
        {
            X = left;
            Y = top;
            Width = right - left;
            Height = bottom - top;
        }
        #region EddenIDK
        public FloatTangle(TotallyNotaFloatISwear x, TotallyNotaFloatISwear y, TotallyNotaFloatISwear width, TotallyNotaFloatISwear height, bool pad = false)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }
        #endregion
        public FloatTangle(Vector2 position, Vector2 size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Left { get => X; set { Width += X - value; X = value; } }        
        public float Right { get => X + Width; set => Width = value - X; }

        public float Top { get => Y; set { Height += Y - value; Y = value; } }
        public float Bottom { get => Y + Height; set => Height = value - Y; }

        public Vector2 Location => new Vector2(X, Y);
    }
}
