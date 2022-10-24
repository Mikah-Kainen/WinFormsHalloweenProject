using Accessibility;

using System.Runtime.CompilerServices;

namespace Rectangle_Hueristic
{


    using RECT = Rectangle;


    public partial class Form1 : Form
    {
        readonly Brush[] brushes =
        {
            Brushes.Green,
            Brushes.YellowGreen,
            Brushes.Yellow,
            Brushes.Orange,
            Brushes.OrangeRed,
            Brushes.Red
        };

        Graphics gfx;
        TextBox[] boxes;
        Bitmap canvas;
        HashSet<Rectangle> rects = new HashSet<RECT>();
#nullable disable
        public Form1()
        {
            InitializeComponent();
        }
#nullable enable

        private void TypeLetter(object? sender, KeyEventArgs e)
        {
#nullable disable
            var box = (TextBox)sender;

            if (e.KeyData == Keys.Enter)
            {

                this.SelectNextControl(box, true, false, true, false);
                return;

            }
#nullable enable
        }
        private void Add_Click(object sender, EventArgs e)
        {
            int[] boxVals = new int[4];
            for (int i = 0; i < boxVals.Length; i++)
            {
                if (!int.TryParse(boxes[i].Text, out boxVals[i]))
                {
                    foreach (var box in boxes) { box.BackColor = Color.Red; }
                    FadeTimer.Start();
                    return;
                }
            }
            RECT newRect = new RECT(boxVals[0], boxVals[1], boxVals[2], boxVals[3]);
            rects.Add(newRect);
            Calculate(sender, e);
            gfx.FillRectangle(Brushes.White, newRect);
            foreach (var box in boxes) { box.Text = String.Empty; };


        }
        const int gridSize = 100;
        private void Calculate(object sender, EventArgs e)
        {
            var spaces = FindBiggestSpace(rects, canvas.Size);

            int i = spaces.Count - 1;
            for (var traveller = spaces.Last; traveller != null; traveller = traveller.Previous)
            {
                gfx.FillRectangle(brushes[Math.Min(i--, brushes.Length - 1)], traveller.Value);
            }

            DrawGolf();
            //gfx.FillRectangle(Brushes.Green, spaces.First.Value);
            i = 0;
            foreach (var space in spaces)
            {
                gfx.DrawRectangle(new Pen(Brushes.Black, spaces.Count - i++), space);
            }

            canvasBox.Image = canvas;
        }

        void DrawGolf()
        {
            for (int i = 0; i < canvas.Width; i += gridSize)
            {
                gfx.DrawLine(Pens.Gold, new Point(i, 0), new Point(i, canvas.Height));
            }
            for (int i = 0; i < canvas.Height; i += gridSize)
            {
                gfx.DrawLine(Pens.Gold, new Point(0, i), new Point(canvas.Width, i));
            }
        }

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

        private void Form1_Load(object sender, EventArgs e)
        {
            boxes = new TextBox[] { XBox, YBox, WidthBox, HeightBox };
            foreach (var box in boxes) { box.KeyDown += TypeLetter; }
            canvas = new Bitmap(canvasBox.Width, canvasBox.Height);
            gfx = Graphics.FromImage(canvas);
            gfx.Clear(Color.Green);
            DrawGolf();
            canvasBox.Image = canvas;
        }

        const int fadeSpeed = 25;
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            foreach (var box in boxes) { FadeTimer.Enabled = (box.BackColor = Color.FromArgb(255, Math.Min(box.BackColor.G + fadeSpeed, 255), Math.Min(box.BackColor.B + fadeSpeed, 255))).B! < 255; };
        }
    }
}