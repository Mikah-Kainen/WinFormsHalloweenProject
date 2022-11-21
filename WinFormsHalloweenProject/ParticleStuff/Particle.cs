using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsHalloweenProject;

namespace WinformsHalloweenProject
{
    public partial class Particle : Form
    {
        int spawnTime;
        int originalSpawnTime;
        int timeLeft;
        int totalTime;
        int originalTime;
   //     Point newLocation;
        float scaleDown;
        Size moveVector;
        public int ID { get; private set; }

        Bitmap newImage;
        Size newSize;

        public int XSpeed => moveVector.Width;
        public int YSpeed => moveVector.Height;
        // bool init = false;
        internal Ghost Ghost { get; set; }
        //  int ticks;
        (Bitmap, Color) textureKey;
        int timeDiff;
        public bool Setting = false;

        //  Bitmap realBackgroundImage;
#nullable disable
        public Particle(int timeDiff, int iD, Ghost ghost)
        {
            this.Name = "GhostParticle";
            this.Text = "GhostParticle";
            InitializeComponent();

            this.timeDiff = timeDiff;
            ID = iD;
            Ghost = ghost;
            BackgroundImageLayout = ImageLayout.Zoom;
            FormBorderStyle = FormBorderStyle.None;
            LifeTimer.Interval = 17;
            BackColor = Color.Lime;
            TransparencyKey = BackColor;
        }

        public void SetData(Point Location, Size moveVector)
        {
            Setting = true;
            this.Location = Location;
            this.moveVector = moveVector;
            if (BackgroundImage != null)
            {
                BackgroundImage = (Bitmap)BackgroundImage.Clone();
            }
        }
        public void SetData((Bitmap, Color) textureKey, Bitmap backgroundImage, int lifeTime, int spawnTime, Size moveVector, float scale = .1f)
        {
            scaleDown = scale;

            this.textureKey = textureKey;
            newImage = backgroundImage;
            newSize = new Size((int)(newImage.Width * scaleDown), (int)(newImage.Height * scaleDown));




            originalTime = timeLeft = lifeTime;
            originalSpawnTime = spawnTime;
            this.spawnTime = 0;            
            this.moveVector = moveVector;
            Setting = false;
        }
#nullable enable

        private void Particle_Load(object sender, EventArgs e)
        {
         //   Location = newLocation;
        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Normal) { WindowState = FormWindowState.Normal; Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine("TAKE THAT LORENZO!"); Console.ForegroundColor = ConsoleColor.White; }
            //if (!init) return;

            //LifeTimer.dur
            //Console.WriteLine(ticks += LifeTimer.Interval);
            Location += moveVector;
            int diff;
            if ((diff = Ghost.TotalTime - totalTime) < timeDiff)
            {
                Console.WriteLine($"Particle: {ID} delaying tick, because it is {timeDiff - diff} ms ahead of global time");
                return;
            }
            else if (diff - timeDiff > Ghost.ParticleTimingCeiling)
            {
                Ghost.TotalTime = totalTime + timeDiff;
                Console.WriteLine($"Global tick reverted, because it is ahead of Particle {ID} by {diff - timeDiff} ms");
            }
            totalTime += LifeTimer.Interval;
            Opacity = spawnTime <= originalSpawnTime ? (spawnTime += LifeTimer.Interval) / (double)originalSpawnTime : (timeLeft -= LifeTimer.Interval) / (double)originalTime;
            if (timeLeft <= 0 && Ghost != null)
            {
                ////     ObjectPool<Particle>.Instance.Return(this);
                Setting = true;
                Console.WriteLine($"Setting Particle {ID}");
                while (Setting) ;
                //{
                //    //Console.WriteLine($"Waiting to set Particle {ID}");
                //}
                BackgroundImage = newImage;
                ClientSize = newSize;
                Location =  new Point(Ghost.Bounds.X + Ghost.Bounds.Width / 2 - (int)(newImage.Width * scaleDown / 2), Ghost.Bounds.Bottom - (int)(newImage.Height * scaleDown));
                Opacity = 0;
                Console.WriteLine($"Set Particle {ID}");
                //try
                //{
                //    Setting = true;
                //}
                //catch (Exception fail)
                //{
                //    Console.WriteLine($"SetParticle completely failed via {fail}!");
                //    SetData(new Point(Ghost.Bounds.X + Ghost.Bounds.Width / 2), Ghost.MovementVector);
                //}
                Thread.Sleep(175);
            }
            // LifeTimer.Interval = 100;
        }
    }
}
