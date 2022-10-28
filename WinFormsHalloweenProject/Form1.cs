using Microsoft.VisualBasic;
using System.Linq;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rectangle_Hueristic;

using static WinFormsHalloweenProject.Form1;
using WinformsHalloweenProject;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace WinFormsHalloweenProject
{
    using Tintmap = ValueTuple<Bitmap, Color>;
    using static Particle;
    using static RectangleHueristic;
    // using ParticlePool = ObjectPool<Particle>;
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
        private static extern IntPtr FindWindowEx(IntPtr parentHandle,
                                          IntPtr hwndChildAfer,
                                          IntPtr className,
                                          [MarshalAs(UnmanagedType.LPStr)] string windowTitle);


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



        public static Random rand = new Random();
        static Point startingPoint = new Point(10, 10);
        static Size startingSize = new Size(800 / 5, 450 / 5);
        static Size speed = new Size(1, 1);
        int currentIndex = 0;
        Bitmap[] images;


        Size oldLocation;
        public Size MovementVector;


        const int leftOffset = 10;
        const int rightOffset = 7;
        const int topOffset = 65;
        const int bottomOffset = 7;

        Rectangle TrueBounds => new Rectangle(Bounds.X + leftOffset, Bounds.Y + topOffset, Bounds.Width - leftOffset - rightOffset, Bounds.Height - topOffset - bottomOffset);

        const int minWindowWidth = 50;
        const int minWindowHeight = 50;

        readonly int maxWindowWidth = Screen.PrimaryScreen.Bounds.Width - 100;
        readonly int maxWindowHeight = Screen.PrimaryScreen.Bounds.Height - 100;


        public HashSet<RECT> CurrentWindows = new HashSet<RECT>();
        public Point[] CurrentPath = { startingPoint, startingPoint, startingPoint };

        const int degree = 3;
        const double lerpIncrement = .05;
        double lerpPercent = .5;
        Size speeds;
        Size shake;

        Graph graph;
        public bool SpawnParticles = true;
        const int particleCount = 8;
        int spawnDelay = 0;
        public Particle[] Particles = new Particle[particleCount];

        int particleIndex = 0;
        bool particlesKnow = false;

#nullable disable
        public Form1()
        {
            InitializeComponent();
        }
#nullable enable
        static readonly Color[] tints =
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
        static readonly Bitmap[] particlesTextures =
        {
            DaPumpkin,
            LeBat,
            DaPumpkinALT
        };

        public readonly Dictionary<Tintmap, (Bitmap template, LinkedList<Bitmap> maps)> particleCache = new Dictionary<Tintmap, (Bitmap, LinkedList<Bitmap>)>();
        private void Form1_Load_1(object sender, EventArgs e)
        {
            //ParticlePool.Instance.Populate(8, () => new Particle());
            for (int i = 0; i++ < particleCount;)
            {
                ThreadStart legitParticle = new ThreadStart(CreateParticle);
                Thread thread = new Thread(legitParticle);
                thread.Start();
                //CreateParticle();
            }

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

            Bounds = new Rectangle(startingPoint, startingSize);
            speeds = new Size(2, 2);


            Opacity = 50;
            //ClientSize = BackgroundImage.Size;

            //ShowInTaskbar = false;
            graph = new Graph(Screen.PrimaryScreen.Bounds);
     
        }

        void CreateParticle()
        {
            Thread.Sleep(spawnDelay += 175);
    
            Particle particle = new Particle();
            Particles[particleIndex++] = particle;
            SetParticle(particle);
            Console.WriteLine(particleIndex - 1);


            //particle.Show();
            Application.Run(particle);



            //ParticlePool.Instance.Borrow<Particle>(). ;

            //TODO: place ghost above particles            
            //try
            //{

            //application.run dies if not using dog...fun
            //}
            //finally 
            //{
            //    ;
            //}
        }

        public void SetParticle(Particle particle)
        {
            Tintmap particleKey = (particlesTextures.RandomValue(), tints.RandomValue());
            try
            {
               
                Color chosenTint = particleKey.Item2;
                if (!particleCache.TryGetValue(particleKey, out var particleTexture))
                {
                    particleCache.Add(particleKey, new(particleKey.Item1, new LinkedList<Bitmap>()));
                    particleTexture.template = (Bitmap)particleKey.Item1.Clone();

                    Graphics gfx = Graphics.FromImage(particleTexture.template);
                    float[][] colorMatrixElements = {
                    new float[] {chosenTint.R / 255f * 2,  0,  0,  0,  0},        // red scaling factor
                    new float[] {0, chosenTint.G / 255f * 2,  0,  0,  0},        // green scaling factor
                    new float[] {0,  0, chosenTint.B / 255f * 2,  0,  0},        // blue scaling factor
                    new float[] {0,  0,  0,  1,  0},
                    new float[] {0,  0,  0,  0,  1}};
                    //TODO: learn how colormatrixes work -Edden
                    ColorMatrix matrix = new ColorMatrix(colorMatrixElements);
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    var bounds = new Rectangle(Point.Empty, particleTexture.template.Size);
                    gfx.DrawImage(particleTexture.template, bounds, bounds.X, bounds.Y, bounds.Width, bounds.Height, GraphicsUnit.Pixel, attributes);
                    particleTexture.maps = new LinkedList<Bitmap>();
                    particleTexture.maps.AddFirst((Bitmap)particleTexture.template.Clone());
                    particleCache[particleKey] = particleTexture;

                }
                else if (particleTexture.maps.Count == 0)
                {
                    particleTexture.maps.AddFirst((Bitmap)particleTexture.template.Clone());
                }
                var chosenTexture = particleTexture.maps.First.Value;
                particleTexture.maps.RemoveFirst();

                particle.SetData(particleKey, chosenTexture, this, 500, 500, new Point(Bounds.X + Bounds.Width / 2 - (int)(chosenTexture.Width * .05f), Bounds.Bottom - (int)(chosenTexture.Height * .1f)), MovementVector);
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine($"Multiple particles accessing cache at once {DateTime.Now}");
                particle.SetData(new Point(Bounds.X + Bounds.Width / 2), MovementVector);
                return;
            }
        }



        private void Animation_Tick(object sender, EventArgs e)
        {
            // SpawnParticles = false;
            BackgroundImage = images[currentIndex];
            currentIndex = (currentIndex + 1) % images.Length;

            shake = new Size(rand.NextError(degree), rand.NextError(degree));
            lerpPercent += rand.NextError(lerpIncrement);
            lerpPercent = Math.Clamp(lerpPercent, 0, 1);
            Opacity = 1d.Lerp(0, lerpPercent);
                        
            if (!particlesKnow)
            {
                particlesKnow = true;
                foreach (var particle in Particles)
                {
                    if (particle == null)
                    {
                        particlesKnow = false;
                        continue;
                    }
                    particle.Ghost = this;
                }
            }

            //HashSet<RECT> imaginaryWindows = new HashSet<RECT>();
            //imaginaryWindows.Add(new RECT(10, 10, 30, 30));
            //imaginaryWindows.Add(new RECT(20, 20, 40, 40));
            //imaginaryWindows.Add(new RECT(50, 950, 100, 1150));
            //Point[] targetPath = graph.GetPath(imaginaryWindows, new Point(10, 1000));

            //if (CurrentPath.Length == 0)
            //{
            //    CurrentPath = new Point[] { startingPoint, startingPoint, startingPoint };
            //}

        }

        private void Movement_Tick(object sender, EventArgs e)
        {
            //IntPtr windowHandle = GetForegroundWindow();
            //RECT currentWindow;
            //bool result = GetWindowRect(new HandleRef(this, windowHandle), out currentWindow);
            //currentWindow = (new Rectangle(0, 0, 0, 0)).ToRECT();


            IntPtr[] windowHandles = GetAllWindows();
            HashSet<RECT> PreviousWindows = CurrentWindows;
            CurrentWindows = new HashSet<RECT>();
            int particleCount = 0;
            for (int i = 0; i < windowHandles.Length; i++)
            {
                if (IsWindowVisible(windowHandles[i]))
                {
                    RECT temp;
                    GetWindowRect(new HandleRef(IntPtr.Zero, windowHandles[i]), out temp);
                    int width = temp.Right - temp.Left;
                    int height = temp.Bottom - temp.Top;
                    bool isParticle = false;
                    //foreach (Ghost ghost in Ghosts) // dont forget to add this once there are multiple ghosts
                    //{
                    if (width >= minWindowWidth & height >= minWindowHeight & width <= maxWindowWidth & height <= maxWindowHeight & temp.Left < Screen.PrimaryScreen.Bounds.Right & temp.Top < Screen.PrimaryScreen.Bounds.Bottom & temp.Right > Screen.PrimaryScreen.Bounds.Left & temp.Bottom > Screen.PrimaryScreen.Bounds.Top && !CurrentWindows.Contains(temp))
                    {
                        IntPtr currentWindow = IntPtr.Zero;
                        do
                        {
                            currentWindow = FindWindowEx(IntPtr.Zero, currentWindow, IntPtr.Zero, "GhostParticle");
                        } while (currentWindow != IntPtr.Zero);
                        foreach (Particle particle in Particles)
                        {
                            if (particle != null)
                            {
                                RECT scaledRect = particle.Bounds.ToRECT().Pad(particle.XSpeed * 5, particle.YSpeed * 5);
                                if (scaledRect.Contains(temp))
                                {
                                    particleCount++;
                                    isParticle = true;
                                    break;
                                }
                            }
                        }
                        if (!isParticle)
                        {
                            CurrentWindows.Add(temp);
                        }
                    }
                }
                //}                
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
                CurrentPath = graph.GetPath(CurrentWindows, TrueBounds.GetCenter());
            }
            MovementVector = oldLocation - (Size)Location;
            oldLocation = (Size)Location;

            Point newPosition;
            //Point newBounds = Declamp(TrueBounds.Location, currentWindow.Left - TrueBounds.Width, currentWindow.Right, currentWindow.Top - TrueBounds.Height, currentWindow.Bottom);
            //Point newBounds = new Point(TrueBounds.X, TrueBounds.Y);
            //Point newBounds = CurrentPath[1];
            if (CurrentPath.Count() > 1)
            {
                double distance = Distance(TrueBounds.Location, CurrentPath[1]);
                newPosition = new Point((int)((double)TrueBounds.X).Lerp(CurrentPath[1].X, 1 / distance * 10 * speed.Width), (int)((double)TrueBounds.Y).Lerp(CurrentPath[1].Y, 1 / distance * 10 * speed.Height));
            }
            else
            {
                newPosition = Move().Location;
            }
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var particle in Particles) { particle.Close(); }
        }
        public static double Distance(Point A, Point B) => Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));

        Process[] oldProcesses = new Process[0];
        Process[] currentProcesses = new Process[0];

        private void DebugTimer_Tick(object sender, EventArgs e)
        {
            //List<Process> processDelta = new List<Process>();
            //oldProcesses = currentProcesses;
            //currentProcesses = Process.GetProcesses();
            //foreach(Process currentProcess in currentProcesses)
            //{
            //    bool wasFound = false;
            //    foreach (Process oldProcess in oldProcesses)
            //    {
            //        if(currentProcess == oldProcess)
            //        {
            //            wasFound = true;
            //        }
            //    }
            //    if(!wasFound)
            //    {
            //        processDelta.Add(currentProcess);
            //    }
            //}
            //if(processDelta.Count > 0)
            //{

            //}
            Process targetProcess = Process.GetProcessesByName("WinformsHalloweenProject").FirstOrDefault();
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

        public static bool Contains(this RECT rect, RECT targetRect) => targetRect.Left >= rect.Left & targetRect.Right <= rect.Right & targetRect.Top >= rect.Top & targetRect.Bottom <= rect.Bottom;
        public static RECT Pad(this RECT rect, int pad) => rect.Pad(pad, pad, pad, pad);
        public static RECT Pad(this RECT rect, int xPad, int yPad) => rect.Pad(xPad, yPad, xPad, yPad);
        public static RECT Pad(this RECT rect, int leftPad, int topPad, int rightPad, int bottomPad) => new RECT(rect.Left - leftPad, rect.Top - topPad, rect.Right + rightPad, rect.Bottom + bottomPad);

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
        public static T RandomValue<T>(this T[] data) => data[Form1.rand.Next(data.Length)];
    }
}