using Accessibility;

using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;

using WinFormsHalloweenProject;

namespace Rectangle_Hueristic
{
    using static WinFormsHalloweenProject.Pain;

    //using RECT = Rectangle;
    using RECT = WinFormsHalloweenProject.Ghost.RECT;

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
        HashSet<Rectangle> rects = new HashSet<Rectangle>();
        LinkedList<Rectangle> spaces;
        Graph graph;
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

                SelectNextControl(box, true, false, true, false);
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
            Rectangle newRect = new Rectangle(boxVals[0], boxVals[1], boxVals[2], boxVals[3]);
            rects.Add(newRect);
            Calculate(sender, e);
            gfx.FillRectangle(Brushes.White, newRect);
            for (int i = 0; i < 4; i++) { boxes[i].Text = String.Empty; };


        }
        const int gridSize = 100;
        private void Calculate(object sender, EventArgs e)
        {
            BestRectLabel.Visible = false;
            spaces = FindBiggestSpace(rects, canvas.Size);

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



        private void Form1_Load(object sender, EventArgs e)
        {
            graph = new Graph(new Rectangle(Point.Empty, ClientSize), Vector2.One);
            boxes = new TextBox[] { XBox, YBox, WidthBox, HeightBox, aspectRatioBox };
            foreach (var box in boxes) { box.KeyDown += TypeLetter; }
            canvas = new Bitmap(canvasBox.Width, canvasBox.Height);
            gfx = Graphics.FromImage(canvas);
            spaces = new LinkedList<Rectangle>();
            spaces.AddFirst(new Rectangle(Point.Empty, canvas.Size));
            gfx.Clear(Color.Green);
            Recycle.Click += Calculate;
            DrawGolf();
            canvasBox.Image = canvas;
        }

        const int fadeSpeed = 25;
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            foreach (var box in boxes) { FadeTimer.Enabled = (box.BackColor = Color.FromArgb(255, Math.Min(box.BackColor.G + fadeSpeed, 255), Math.Min(box.BackColor.B + fadeSpeed, 255))).B! < 255; };
        }

        private void canvasBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Rectangle bestRect = GetBiggestRectangle(e.Location, graph.AspectRatio, spaces, out var weight);
                gfx.FillRectangle(Brushes.CornflowerBlue, bestRect);
                BestRectLabel.Text = weight.ToString();
                BestRectLabel.Location = new Point(bestRect.X + bestRect.Width / 2 - BestRectLabel.Width / 2, bestRect.Y + bestRect.Height / 2 - BestRectLabel.Height / 2);
                BestRectLabel.Visible = true;
                canvasBox.Image = canvas;
            }
            else if (e.Button == MouseButtons.Left)
            {
                HashSet<RECT> dumberRects = rects.Select(m => m.ToRECT()).ToHashSet();
                var points = graph.GetPath(dumberRects, e.Location, out var noPath, out var temp, out var recties);
                if (noPath == PathStatus.Path)
                {

                    Point oldPoint = Point.Empty;
                    foreach (var point in points)
                    {
                        gfx.FillEllipse(Brushes.Goldenrod, new Rectangle(point.X - 6, point.Y - 6, 12, 12));
                        if (!oldPoint.IsEmpty)
                        {
                            gfx.DrawLine(Pens.Gold, new Point(oldPoint.X, oldPoint.Y - 1), new Point(point.X, point.Y - 1));
                            gfx.DrawLine(Pens.Gold, new Point(oldPoint.X - 1, oldPoint.Y), new Point(point.X - 1, point.Y));
                            gfx.DrawLine(Pens.Gold, new Point(oldPoint.X + 1, oldPoint.Y), new Point(point.X + 1, point.Y));
                            gfx.DrawLine(Pens.Gold, new Point(oldPoint.X, oldPoint.Y + 1), new Point(point.X, point.Y + 1));
                            gfx.DrawLine(Pens.DarkCyan, oldPoint, point);

                            gfx.FillEllipse(Brushes.DarkCyan, new Rectangle(oldPoint.X - 5, oldPoint.Y - 5, 10, 10));
                        }
                        gfx.FillEllipse(Brushes.DarkCyan, new Rectangle(point.X - 5, point.Y - 5, 10, 10));
                        oldPoint = point;
                    }
                }
                //    gfx.FillEllipse(Brushes.Honeydew, new Rectangle(0, 0, 100, 100));
                canvasBox.Image = canvas;
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            rects.Clear();
            Calculate(sender, e);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                var vals = ((TextBox)sender).Text.Split(':', ' ').Where(m => m.Length != 0).ToArray();
                if (vals.Length == 2 && float.TryParse(vals[0], out var x) && float.TryParse(vals[1], out var y))
                {
                    graph.AspectRatio = new Vector2(x, y);
                    Calculate(sender, e);
                    return;
                }
                foreach (var box in boxes) { box.BackColor = Color.Red; }
                FadeTimer.Start();
            }
        }
    }
}