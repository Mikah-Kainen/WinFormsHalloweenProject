using System.Runtime.CompilerServices;

namespace WinFormsHalloweenProject
{
    public partial class Form1 : Form
    {
        Random rand = new Random();
        int currentIndex;
        Bitmap[] images;

        const int degree = 3;
        const double lerpIncrement = .05;
        double lerpPercent = .5;
        Size speeds;
        Size shake; 
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
        }

        private void Animation_Tick(object sender, EventArgs e)
        {
            

            BackgroundImage = images[currentIndex];
            currentIndex = (currentIndex + 1) % images.Length;

            shake = new Size(rand.NextError(degree), rand.NextError(degree));
            lerpPercent += rand.NextError(lerpIncrement);
            lerpPercent = Math.Clamp(lerpPercent, 0, 1);
            Opacity = 1d.Lerp(0, lerpPercent);
        }

        private void Movement_Tick(object sender, EventArgs e)
        {
            Bounds = Move();
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

        private Rectangle Move()
        {            
            return new Rectangle((Point)(((Size)Bounds.Location) + shake + speeds), Bounds.Size);
        }


    }
    static class Extensions
    {
        public static int NextError(this Random rand, int degree) => rand.Next(-degree, degree + 1);

        public static double NextError(this Random rand, double degree) => rand.NextDouble() * degree * (rand.Next(0, 2) * 2 - 1);
        

        public static double Lerp(this double a, double b, double percent) => a * percent + b * (1 - percent);
    }
}