using Microsoft.VisualBasic;
using System.Linq;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static WinFormsHalloweenProject.Form1;

namespace WinFormsHalloweenProject
{
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
        private void Form1_Load_1(object sender, EventArgs e)
        {
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
            graph = new Graph(25, 20, Screen.PrimaryScreen.Bounds);
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
            List<RECT> currentWindows = new List<RECT>();
            Dictionary<(int, int), bool> isSizeUsed = new Dictionary<(int, int), bool>();
            for (int i = 0; i < windowHandles.Length; i++)
            {
                if (IsWindowVisible(windowHandles[i]))
                {
                    RECT temp;
                    GetWindowRect(new HandleRef(IntPtr.Zero, windowHandles[i]), out temp);
                    int width = temp.Right - temp.Left;
                    int height = temp.Bottom - temp.Top;

                    if (!isSizeUsed.ContainsKey((width, height)))
                    {
                        isSizeUsed.Add((width, height), false);
                    }
                    if (!isSizeUsed[(width, height)] && width >= minWindowWidth && height >= minWindowHeight && width <= maxWindowWidth && height <= maxWindowHeight && temp.Left < Screen.PrimaryScreen.Bounds.Right && temp.Top < Screen.PrimaryScreen.Bounds.Bottom && temp.Right > Screen.PrimaryScreen.Bounds.Left && temp.Bottom > Screen.PrimaryScreen.Bounds.Top)
                    {
                        isSizeUsed[(width, height)] = true;
                        currentWindows.Add(temp);
                    }
                }

            }

            //Add rectangle tracking to reduce the amount we need to clean up each time
            currentWindows.Remove(Bounds.ToRECT());
            foreach (RECT rect in currentWindows)
            {
                graph.SetWallState(rect.ToRectangle(), true);
            }
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
            //Point newBounds = new Point(TrueBounds.X, TrueBounds.Y);
            Point newBounds = graph.Dijkstra(new Point(TrueBounds.X, TrueBounds.Y));
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

        private Rectangle Move()
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


    }
    static class Extensions
    {
        public static int NextError(this Random rand, int degree) => rand.Next(-degree, degree + 1);

        public static double NextError(this Random rand, double degree) => rand.NextDouble() * degree * (rand.Next(0, 2) * 2 - 1);

        public static double Lerp(this double a, double b, double percent) => a * percent + b * (1 - percent);

        public static RECT ToRECT(this Rectangle rect)
        {
            RECT returnRect = new RECT();
            returnRect.Left = rect.Left;
            returnRect.Top = rect.Top; ;
            returnRect.Right = rect.Right; 
            returnRect.Bottom = rect.Bottom;
            return returnRect;
        }

        public static Rectangle ToRectangle(this RECT rect)
        {
            return new Rectangle(new Point(rect.Left, rect.Top), new Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
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