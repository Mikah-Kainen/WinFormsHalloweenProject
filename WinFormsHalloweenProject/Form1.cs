using Microsoft.VisualBasic;
using System.Linq;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rectangle_Hueristic;

using static WinFormsHalloweenProject.Form1;
using WinformsHalloweenProject;

namespace WinFormsHalloweenProject
{
    using static RectangleHueristic;
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public bool Contains(Point targetPoint) => Left < targetPoint.X & Right > targetPoint.X & Top < targetPoint.Y & Bottom > targetPoint.Y;

            //public static implicit operator Rectangle(RECT rect) => rect.ToRectangle();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hwnd);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumedWindow lpEnumFunc, ArrayList lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumedWindow callBack, ArrayList lParam);


        private delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

        private static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            windowHandles.Add(windowHandle);
            return true;
        }

        private static IntPtr[] GetAllWindows()
        {
            ArrayList windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumWindows(callBackPtr, windowHandles);


            //foreach (IntPtr windowHandle in windowHandles.ToArray())
            //{
            //    EnumChildWindows(windowHandle, callBackPtr, windowHandles);
            //}

            return windowHandles.Cast<IntPtr>().ToArray();
        }



        /// <summary>
        /// ///////////////////////////////////////////NEED TO MAKE A MAP FOR GHOST NAVIGATION!! GOOD lUCK FUTURE SELF
        /// First identify the open spaces on the graph
        /// Then decide which space to go to
        /// A* there with the A* returning a pixel by pixel path including the path width and the ghost calculating how much it can shake each step of the way
        /// Maybe a rectangle by rectangle path?
        /// </summary>



        Random rand = new Random();
        int currentIndex;
        Bitmap[] images;



        const int leftOffset = 10;
        const int rightOffset = 7;
        const int topOffset = 65;
        const int bottomOffset = 7;

        Rectangle TrueBounds => new Rectangle(Bounds.X + leftOffset, Bounds.Y + topOffset, Bounds.Width - leftOffset - rightOffset, Bounds.Height - topOffset - bottomOffset);

        const int minWindowWidth = 50;
        const int minWindowHeight = 50;

        int maxWindowWidth = Screen.PrimaryScreen.Bounds.Width - 100;
        int maxWindowHeight = Screen.PrimaryScreen.Bounds.Height - 100;

        HashSet<RECT> CurrentWindows = new HashSet<RECT>();

        const int degree = 3;
        const double lerpIncrement = .05;
        double lerpPercent = .5;
        Size speeds;
        Size shake;

        Graph graph;
        public Form1()
        {
            InitializeComponent();
        }
        readonly Color[] tints =
        {
            Color.Red,
            Color.OrangeRed,
            Color.Yellow,
            Color.Green,
            Color.Orange,
            Color.Black,
            Color.Purple,
            Color.Blue,
        };
        readonly Bitmap[] particles =
        {
            Particle.lep
        };
        Dictionary<(Bitmap, Color), Bitmap> tintCache = new Dictionary<(Bitmap, Color), Bitmap>();
        private void Form1_Load_1(object sender, EventArgs e)
        {

            //Thread.Sleep(5000);
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LawnGreen;
            TransparencyKey = BackColor;
            images = new Bitmap[]
            {
                  Dog1,
                  Dog2,
                  Dog3,
                  Dog4,
                  Dog5,
                  Dog6,
                  Dog7,
                  Dog8,
                  Dog9,
                  Dog10,
            };

            speeds = new Size(2, 2);


            Opacity = 50;
            ClientSize = BackgroundImage.Size;

            //ShowInTaskbar = false;
            graph = new Graph(Screen.PrimaryScreen.Bounds);
        }

        static void CreateParticle()
        {


            Particle particle = new Particle(Dog1, 3000);
            Application.Run(particle);
        }

        private void Animation_Tick(object sender, EventArgs e)
        {
            BackgroundImage = images[currentIndex];
            currentIndex = (currentIndex + 1) % images.Length;

            shake = new Size(rand.NextError(degree), rand.NextError(degree));
            lerpPercent += rand.NextError(lerpIncrement);
            lerpPercent = Math.Clamp(lerpPercent, 0, 1);
            Opacity = 1d.Lerp(0, lerpPercent);


            IntPtr[] windowHandles = GetAllWindows();
            HashSet<RECT> PreviousWindows = CurrentWindows;
            CurrentWindows = new HashSet<RECT>();
            for (int i = 0; i < windowHandles.Length; i++)
            {
                if (IsWindowVisible(windowHandles[i]))
                {
                    RECT temp;
                    GetWindowRect(new HandleRef(IntPtr.Zero, windowHandles[i]), out temp);
                    int width = temp.Right - temp.Left;
                    int height = temp.Bottom - temp.Top;


                    if (width >= minWindowWidth & height >= minWindowHeight & width <= maxWindowWidth & height <= maxWindowHeight & temp.Left < Screen.PrimaryScreen.Bounds.Right & temp.Top < Screen.PrimaryScreen.Bounds.Bottom & temp.Right > Screen.PrimaryScreen.Bounds.Left & temp.Bottom > Screen.PrimaryScreen.Bounds.Top && !CurrentWindows.Contains(temp))
                    {
                        CurrentWindows.Add(temp);
                    }
                }
            }
            CurrentWindows.Remove(Bounds.ToRECT());
            bool diff = false;
            foreach (RECT rect in CurrentWindows)
            {
                if (PreviousWindows.Contains(rect))
                {
                    PreviousWindows.Remove(rect);
                }
                else
                {
                    diff = true;
                    break;
                }
            }
            if (diff | PreviousWindows.Count > 0)
            {
                //graph.SetGraph(CurrentWindows);
            }

            HashSet<RECT> imaginaryWindows = new HashSet<RECT>();
            imaginaryWindows.Add(new RECT(10, 10, 30, 30));
            imaginaryWindows.Add(new RECT(20, 20, 40, 40));
            imaginaryWindows.Add(new RECT(50, 950, 100, 1150));
            Point[] targetPath = graph.GetPath(imaginaryWindows, new Point(10, 1000));


            //Add rectangle tracking to reduce the amount we need to clean up each time

            //foreach (RECT rect in currentWindows)
            //{
            //    graph.SetWallState(rect.ToRectangle(), true);
            //}
        }

        private void Movement_Tick(object sender, EventArgs e)
        {


            //IntPtr windowHandle = GetForegroundWindow();
            //RECT currentWindow;
            //bool result = GetWindowRect(new HandleRef(this, windowHandle), out currentWindow);
            //currentWindow = (new Rectangle(0, 0, 0, 0)).ToRECT();

            Bounds = Move();
            Point newPosition;
            //Point newBounds = Declamp(TrueBounds.Location, currentWindow.Left - TrueBounds.Width, currentWindow.Right, currentWindow.Top - TrueBounds.Height, currentWindow.Bottom);
            Point newBounds = new Point(TrueBounds.X, TrueBounds.Y);
            //Point newBounds = graph.Dijkstra(new Point(TrueBounds.X, TrueBounds.Y));
            newPosition = new Point(TrueBounds.X.Lerp(newBounds.X, 50), TrueBounds.Y.Lerp(newBounds.Y, 50));
            var oldNewPosition = newPosition;
            newPosition = new Point(Math.Clamp(newPosition.X, Screen.PrimaryScreen.Bounds.Left, Screen.PrimaryScreen.Bounds.Right - TrueBounds.Width), Math.Clamp(newPosition.Y, Screen.PrimaryScreen.Bounds.Top, Screen.PrimaryScreen.Bounds.Bottom - TrueBounds.Height));
            if (oldNewPosition.X != newPosition.X)
            {
                speeds.Width *= -1;
            }
            if (oldNewPosition.Y != newPosition.Y)
            {
                speeds.Height *= -1;
            }

            Bounds = new Rectangle((Point)((Size)newPosition - new Size(leftOffset, topOffset)), Bounds.Size);
        }

        private new Rectangle Move()
        {
            return new Rectangle((Point)(((Size)TrueBounds.Location) - new Size(leftOffset, topOffset) + shake + speeds), Bounds.Size);
        }

        public Point Declamp(Point val, int xMin, int xMax, int yMin, int yMax)
        {
            Point returnVal = val;
            Point averages = new Point((xMax + xMin) / 2, (yMax + yMin) / 2);
            if (val.X > xMin && val.X < xMax && val.Y > yMin && val.Y < yMax)
            {
                int shouldXMin = val.X - xMin;
                int shouldXMax = xMax - val.X;
                int shouldYMin = val.Y - yMin;
                int shouldYMax = yMax - val.Y;

                bool isXMin = shouldXMin < shouldXMax;
                bool isYMin = shouldYMin < shouldYMax;

                int bestXScore = isXMin ? shouldXMin : shouldXMax;
                int bestYScore = isYMin ? shouldYMin : shouldYMax;

                if (bestXScore < bestYScore)
                {
                    if (isXMin)
                    {
                        returnVal.X = xMin;
                    }
                    else
                    {
                        returnVal.X = xMax;
                    }
                }
                else
                {
                    if (isYMin)
                    {
                        returnVal.Y = yMin;
                    }
                    else
                    {
                        returnVal.Y = yMax;
                    }
                }

            }
            return returnVal;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ThreadStart legitParticle = new ThreadStart(CreateParticle);
            Thread thread = new Thread(legitParticle);
            thread.Start();
        }
    }
    static class Extensions
    {
        public static int NextError(this Random rand, int degree) => rand.Next(-degree, degree + 1);

        public static double NextError(this Random rand, double degree) => rand.NextDouble() * degree * (rand.Next(0, 2) * 2 - 1);

        public static double Lerp(this double a, double b, double percent) => b * percent + a * (1 - percent);

        public static RECT ToRECT(this Rectangle rect)
        {
            RECT returnRect = new RECT();
            returnRect.Left = rect.Left;
            returnRect.Top = rect.Top; ;
            returnRect.Right = rect.Right;
            returnRect.Bottom = rect.Bottom;
            return returnRect;
        }

        public static Point GetCenter(this Rectangle targetRectangle)
        {
            return new Point(targetRectangle.Left + targetRectangle.Width / 2, targetRectangle.Top + targetRectangle.Height / 2);
        }

        public static Rectangle ToRectangle(this RECT rect)
        {
            return new Rectangle(new Point(rect.Left, rect.Top), new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
        }

        public static HashSet<Rectangle> ToRectangles(this HashSet<RECT> rects)
        {
            HashSet<Rectangle> returnSet = new HashSet<Rectangle>();
            foreach (RECT rect in rects)
            {
                returnSet.Add(rect.ToRectangle());
            }
            return returnSet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="percent">0 - 100</param>
        /// <returns></returns>
        public static int Lerp(this int a, int b, int percent) => (a * percent + b * (100 - percent)) / 100;
    }
}