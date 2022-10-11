using System.Runtime.CompilerServices;

namespace WinFormsHalloweenProject
{
    public partial class Form1 : Form
    {
        Random rand = new Random();
        int currentIndex;
        Bitmap[] images;

        Size speeds;
        public Form1()
        {
            InitializeComponent();
        }

        private void Animation_Tick(object sender, EventArgs e)
        {
            BackgroundImage = images[currentIndex];
            currentIndex = (currentIndex + 1) % images.Length;
        }

        private void Movement_Tick(object sender, EventArgs e)
        {
            Bounds = Move(5);
            Point newPosition = new Point(Math.Clamp(Bounds.X, Screen.PrimaryScreen.Bounds.Left, Screen.PrimaryScreen.Bounds.Right - Bounds.Width), Math.Clamp(Bounds.Y, Screen.PrimaryScreen.Bounds.Top, Screen.PrimaryScreen.Bounds.Bottom - Bounds.Height));
            if(Bounds.X != newPosition.X)
            {
                speeds.Width *= -1;
            }
            if(Bounds.Y != newPosition.Y)
            {
                speeds.Height *= -1;
            }
            Bounds = new Rectangle(newPosition, Bounds.Size);
        }

        private Rectangle Move(int degree)
        {
            Size random = new Size(rand.NextError(degree), rand.NextError(degree));
            return new Rectangle((Point)(((Size)Bounds.Location) + random + speeds), Bounds.Size);
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

            speeds = new Size(3, 3);


            Opacity = 50;
            //ShowInTaskbar = false;
        }

    }
    static class Extensions
    {
        public static int NextError(this Random rand, int degree) => rand.Next(-degree, degree + 1);
    }
}