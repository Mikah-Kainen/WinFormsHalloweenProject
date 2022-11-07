using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static WinFormsHalloweenProject.Ghost;

namespace WinformsHalloweenProject
{
    public interface IRectangle
    {
        float X { get; set; }
        float Y { get; set; }
        float Width { get; }
        float Height { get; }
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
        bool Intersects(IRectangle other) => Top <= other.Bottom & Bottom >= other.Top & Left <= other.Right & Right >= other.Left;
        bool Intersects(Rectangle other) => Top <= other.Bottom & Bottom >= other.Top & Left <= other.Right & Right >= other.Left;
        public bool MoneyGrubbingContains(Point target) => target.X > Left & target.X < Right & target.Y > Top & target.Y < Bottom;
        public bool Contains(IRectangle other) => other.Left >= Left & other.Right <= Right & other.Top >= Top & other.Bottom <= Bottom;
        public bool GenerousContains(Point target) => Left <= target.X & Right >= target.X & Top <= target.Y & Bottom >= target.Y;

    }

    public struct FloatTangle : IRectangle
    {
        public FloatTangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
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

        public float Left => X;

        public float Right => X + Width;

        public float Top => Y;

        public float Bottom => Y + Height;

        public Vector2 Location => new Vector2(X, Y);
    }
}
