using System.Linq;
using System.Collections;
using System.Runtime.InteropServices;

using static WinFormsHalloweenProject.Ghost;
using WinformsHalloweenProject;
using System.Drawing.Imaging;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using System.Text;

namespace WinFormsHalloweenProject
{
    using Tintmap = ValueTuple<Bitmap, Color>;
    using static Particle;
    // using static Pain;
    // using ParticlePool = ObjectPool<Particle>;
    public partial class Ghost : Form
    {
        #region PInvoke
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr hwndChildAfer, IntPtr className, [MarshalAs(UnmanagedType.LPStr)] string windowTitle);

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
        ///     Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a
        ///     control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another
        ///     application.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633520%28v=vs.85%29.aspx  for more
        ///     information
        ///     </para>
        /// </summary>
        /// <param name="hWnd">
        ///     C++ ( hWnd [in]. Type: HWND )<br />A <see cref="IntPtr" /> handle to the window or control containing the text.
        /// </param>
        /// <param name="lpString">
        ///     C++ ( lpString [out]. Type: LPTSTR )<br />The <see cref="StringBuilder" /> buffer that will receive the text. If
        ///     the string is as long or longer than the buffer, the string is truncated and terminated with a null character.
        /// </param>
        /// <param name="nMaxCount">
        ///     C++ ( nMaxCount [in]. Type: int )<br /> Should be equivalent to
        ///     <see cref="StringBuilder.Length" /> after call returns. The <see cref="int" /> maximum number of characters to copy
        ///     to the buffer, including the null character. If the text exceeds this limit, it is truncated.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the length, in characters, of the copied string, not including
        ///     the terminating null character. If the window has no title bar or text, if the title bar is empty, or if the window
        ///     or control handle is invalid, the return value is zero. To get extended error information, call GetLastError.<br />
        ///     This function cannot retrieve the text of an edit control in another application.
        /// </returns>
        /// <remarks>
        ///     If the target window is owned by the current process, GetWindowText causes a WM_GETTEXT message to be sent to the
        ///     specified window or control. If the target window is owned by another process and has a caption, GetWindowText
        ///     retrieves the window caption text. If the window does not have a caption, the return value is a null string. This
        ///     behavior is by design. It allows applications to call GetWindowText without becoming unresponsive if the process
        ///     that owns the target window is not responding. However, if the target window is not responding and it belongs to
        ///     the calling application, GetWindowText will cause the calling application to become unresponsive. To retrieve the
        ///     text of a control in another process, send a WM_GETTEXT message directly instead of calling GetWindowText.<br />For
        ///     an example go to
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644928%28v=vs.85%29.aspx#sending">
        ///     Sending a
        ///     Message.
        ///     </see>
        /// </remarks>
        /// 

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static string GetText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// ///////////////////////////////////////////NEED TO MAKE A MAP FOR GHOST NAVIGATION!! GOOD lUCK FUTURE SELF
        /// First identify the open spaces on the graph
        /// Then decide which space to go to
        /// A* there with the A* returning a pixel by pixel path including the path width and the ghost calculating how much it can shake each step of the way
        /// Maybe a rectangle by rectangle path?
        /// </summary>
        #endregion


        public static Random rand = new Random();
        static Point startingPoint = new Point(10, 10);
        const double ghostScale = .5;
        const double particleScale = 1;
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
        int pathIndex = 0;
        float totalDistance = 0;
        float currentDistance = 0;
        float targetDistance = 0;
        float globalLerpFactor = 0;
        float lerpFactor = 0;
        bool vibing = false;
        PathStatus pathResult = PathStatus.Path;
        Rectangle endGoal;
        float[] distances;

        Point trueLocation;

        const int degree = 3;
        const double lerpIncrement = .05;
        double lerpPercent = .5;
        //Size speeds;
        //Size shake;

        Graph graph;
        public bool SpawnParticles = true;
        const int particleCount = 8;
        int spawnDelay = 0;
        public Particle[] Particles = new Particle[particleCount];

        int particleIndex = 0;
        bool particlesKnow = false;

#nullable disable
        public Ghost()
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
        private void Ghost_Load(object sender, EventArgs e)
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

            Bounds = new Rectangle(startingPoint, new Size((int)(BackgroundImage.Width * ghostScale), (int)(BackgroundImage.Height * ghostScale)));
            //speeds = new Size(2, 2);


            Opacity = 50;

            //ShowInTaskbar = false;
            graph = new Graph(Screen.PrimaryScreen.Bounds, new System.Numerics.Vector2(TrueBounds.Width, TrueBounds.Height));
            trueLocation = Location;
        }

        void CreateParticle()
        {
            //return;
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
                    particleCache[particleKey] = new(particleKey.Item1, new LinkedList<Bitmap>());
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
                if (particleTexture.maps.Count == 0)
                {
                    particleTexture.maps.AddFirst((Bitmap)particleTexture.template.Clone());
                }
                var chosenTexture = particleTexture.maps.First.Value;
                particleTexture.maps.RemoveFirst();

                particle.SetData(particleKey, chosenTexture, this, 500, 500, new Point(Bounds.X + Bounds.Width / 2 - (int)(chosenTexture.Width * .05f), Bounds.Bottom - (int)(chosenTexture.Height * .1f)), MovementVector, (float)particleScale / 10);
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
            #region old 
            //IntPtr[] windows = GetAllWindows();
            //Process[] allProcesses = Process.GetProcesses();
            //List<Process> targetProcesses = new List<Process>();

            //for(int i = 0; i < allProcesses.Length; i ++)
            //{
            //    string currentProcess = allProcesses[i].ProcessName;
            //    foreach(char character in currentProcess)
            //    {
            //        if(character == 'V')
            //        {
            //            targetProcesses.Add(allProcesses[i]);
            //        }
            //    }
            //}

            // SpawnParticles = false;
            #endregion
            BackgroundImage = images[currentIndex];
            currentIndex = (currentIndex + 1) % images.Length;

            //shake = new Size(rand.NextError(degree), rand.NextError(degree));
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
        }

        bool GetPath(bool diff, ref PathStatus pathResult)
        {
            IntPtr[] windowHandles = GetAllWindows();
            List<IntPtr> particleHandles = GetParticleHandles();
            List<IntPtr> ghostHandles = GetGhostHandles();
            HashSet<RECT> PreviousWindows = CurrentWindows;
            CurrentWindows = new HashSet<RECT>();
            for (int i = 0; i < windowHandles.Length; i++)
            {
                bool isParticle = false;
                bool isGhost = false;
                foreach (IntPtr particle in particleHandles)
                {
                    if (windowHandles[i] == particle)
                    {
                        isParticle = true;
                        break;
                    }
                }
                if (isParticle) continue;

                foreach (IntPtr ghost in ghostHandles)
                {
                    if (windowHandles[i] == ghost)
                    {
                        isGhost = true;
                        break;
                    }
                }
                if (isGhost) continue;

                if (IsWindowVisible(windowHandles[i]))
                {
                    RECT temp;
                    GetWindowRect(new HandleRef(IntPtr.Zero, windowHandles[i]), out temp);
                    StringBuilder text = new StringBuilder(0, 256);
                    var handleText = GetText(windowHandles[i]);
                    int width = temp.Right - temp.Left;
                    int height = temp.Bottom - temp.Top;
                    if ((handleText != "" && handleText != "Mail" && handleText != "Alienware Command Center" && handleText != "Settings" && handleText != "Calculator" && (width >= minWindowWidth | height >= minWindowHeight) & (width <= maxWindowWidth | height <= maxWindowHeight) & (temp.Left < Screen.PrimaryScreen.Bounds.Right & temp.Top < Screen.PrimaryScreen.Bounds.Bottom & temp.Right > Screen.PrimaryScreen.Bounds.Left & temp.Bottom > Screen.PrimaryScreen.Bounds.Top)) && !CurrentWindows.Contains(temp))
                    {
                        CurrentWindows.Add(temp);
                    }
                }
            }
            //bool diff = false;
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
                CurrentPath = graph.GetPath(CurrentWindows, trueLocation, out pathResult, out endGoal);
                return true;
            }
            return false;
        }

        // const double wantedSpeed = 5;
        private void Movement_Tick(object sender, EventArgs e)
        {
            if (GetPath(false, ref pathResult))
            {
                vibing = false;
                pathIndex = -1;
                globalLerpFactor = 0.001f;
                currentDistance = 0;
                totalDistance = 0;

                if (pathResult == PathStatus.NoPath) return;

                var prevLocation = trueLocation;
                RECT evilRect = endGoal.ToRECT();
                for (int i = 0; pathResult != PathStatus.Path; i++)
                {
                    if (i == 5 || pathResult == PathStatus.NoPath)
                    {
                        pathResult = PathStatus.NoPath;
                        trueLocation = prevLocation;
                        return;
                    }
                    evilRect = new RECT(Math.Min(endGoal.Left, evilRect.Left), Math.Min(endGoal.Top, evilRect.Top), Math.Max(endGoal.Right, evilRect.Right), Math.Max(endGoal.Bottom, evilRect.Bottom));
                    trueLocation = Declamp(trueLocation, evilRect.Left, evilRect.Right, evilRect.Top, evilRect.Bottom);
                    GetPath(true, ref pathResult);
                }


                distances = new float[CurrentPath.Length - 1];
                var oldPoint = CurrentPath[0];
                for (int i = 1; i < CurrentPath.Length; i++)
                {
                    totalDistance += distances[i - 1] = (float)Distance(oldPoint, CurrentPath[i]);
                    oldPoint = CurrentPath[i];
                }
                targetDistance = 0;
            }
            else if (pathResult == PathStatus.NoPath)
            {
                Console.WriteLine("Nowhere the ghost can go...you monster");
                return;
            }
            MovementVector = oldLocation - (Size)Location;
            oldLocation = (Size)Location;

            if (vibing = vibing || endGoal.Contains(TrueBounds))
            {
                //vibe
                return;
            }

            //Console.WriteLine(vibing);
            //Console.WriteLine("i" + pathIndex);
            //Console.WriteLine("pl" + (CurrentPath.Length - 1));


            //if (Distance(trueLocation, CurrentPath[pathIndex]) <= 1)
            //{
            //    float tempDistance = 0;
            //    do
            //    {
            //        if (vibing = ++pathIndex == CurrentPath.Length) return;
            //        float prevTempDistance = 1;
            //        tempDistance += currentDistance = (float)Distance(trueLocation, CurrentPath[pathIndex]);
            //        lerpFactor = (globalLerpFactor / Math.Max(tempDistance, 1) * totalDistance);// - Math.Abs(lerpFactor / prevTempDistance * totalDistance);
            //    } while (lerpFactor > 1);

            //}
            //else
            //{

            var lerpChange = Math.Clamp(globalLerpFactor * (1 + (globalLerpFactor <= .5).ToByte() * 2 - 1) * .1f, .005f, .05f);
            //lerpFactor += (globalLerpFactor / Math.Max((float)Distance(trueLocation, CurrentPath[pathIndex]), 1) * totalDistance);
            globalLerpFactor += lerpChange;            
            currentDistance = 0f.Lerp(totalDistance, globalLerpFactor);
            if (vibing = globalLerpFactor >= 1) return;
            if (currentDistance >= targetDistance)
            {
                targetDistance += distances[++pathIndex];                
            }
            //}


            trueLocation = CurrentPath[pathIndex + 1].Lerp(trueLocation, (currentDistance - (targetDistance - distances[pathIndex])) / distances[pathIndex]);

            Location = trueLocation;

            /*
            Point newPosition = new Point(oldLocation.Width, oldLocation.Height);
            if (CurrentPath.Length > 1)
            {
                double distance = Distance(TrueBounds.Location, CurrentPath[1]);
                newPosition = new Point((int)((double)TrueBounds.X).Lerp(CurrentPath[1].X, 1 / distance * 10 * speed.Width), (int)((double)TrueBounds.Y).Lerp(CurrentPath[1].Y, 1 / distance * 10 * speed.Height));
            }
            if (newPosition.X < 1920 & newPosition.Y < 1080 & newPosition.X > 0 & newPosition.Y > 0)
            {
                Bounds = new Rectangle((Point)((Size)newPosition - new Size(leftOffset, topOffset)), Bounds.Size);
            }
            */
            #region old

            //else
            //{
            //    newPosition = Move().Location;
            //}
            //var oldNewPosition = newPosition;
            //newPosition = new Point(Math.Clamp(newPosition.X, Screen.PrimaryScreen.Bounds.Left, Screen.PrimaryScreen.Bounds.Right - TrueBounds.Width), Math.Clamp(newPosition.Y, Screen.PrimaryScreen.Bounds.Top, Screen.PrimaryScreen.Bounds.Bottom - TrueBounds.Height));
            //if (oldNewPosition.X != newPosition.X)
            //{
            //    speeds.Width *= -1;
            //}
            //if (oldNewPosition.Y != newPosition.Y)
            //{
            //    speeds.Height *= -1;
            //}
            #endregion

        }
        #region old
        //private new Rectangle Move()
        //{
        //    return new Rectangle((Point)(((Size)TrueBounds.Location) - new Size(leftOffset, topOffset) + shake + speeds), Bounds.Size);
        //}
        #endregion
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

        private void Ghost_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var particle in Particles) if (particle != null) particle.Close();
        }
        public static double Distance(Point A, Point B) => Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));

        public List<IntPtr> GetWindowHandles(string windowTitle)
        {
            List<IntPtr> windowHandles = new List<IntPtr>();
            for (IntPtr currentWindow = IntPtr.Zero; (currentWindow = FindWindowEx(IntPtr.Zero, currentWindow, IntPtr.Zero, windowTitle)) != IntPtr.Zero;) //edden approved!
            {
                windowHandles.Add(currentWindow);
            }
            return windowHandles;
        }
        public List<IntPtr> GetParticleHandles() => GetWindowHandles("Particle");

        public List<IntPtr> GetGhostHandles() => GetWindowHandles("Ghost");

    }
}