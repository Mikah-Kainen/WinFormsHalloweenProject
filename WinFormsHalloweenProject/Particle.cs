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
        int originalTime;
        Point location;
        int scaleDown;
        Size moveVector;
        public int XSpeed => moveVector.Width;
        public int YSpeed => moveVector.Height;
       // bool init = false;
        internal Ghost Ghost { get; set; }
        int ticks;
        (Bitmap, Color) textureKey;
        //  Bitmap realBackgroundImage;
#nullable disable
        public Particle()
        {
            this.Name = "GhostParticle";
            this.Text = "GhostParticle";
            InitializeComponent();
        }

        public void SetData(Point Location, Size moveVector)
        {
            this.Location = Location;
            this.moveVector = moveVector;
            if (BackgroundImage != null)
            {
                BackgroundImage = (Bitmap)BackgroundImage.Clone();
            }
        }
        public Particle SetData((Bitmap, Color) textureKey, Bitmap backgroundImage, Ghost ghost, int lifeTime, int spawnTime, Point location, Size moveVector, float scale = .1f)
        {
            this.Ghost = ghost;
            scaleDown = (int)(1 / scale);

            if (backgroundImage != null)
            {
                this.textureKey = textureKey;
                BackgroundImage = backgroundImage;
                ClientSize = BackgroundImage.Size / scaleDown;
            }

            BackColor = Color.Lime;
            BackgroundImageLayout = ImageLayout.Zoom;
            originalTime = timeLeft = lifeTime;
            originalSpawnTime = spawnTime;
            this.spawnTime = 0;
            Opacity = 0;
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = BackgroundImage.Size / scaleDown;
            Location = this.location = location;
            TransparencyKey = BackColor;
            this.moveVector = moveVector;
           // init = true;
            
            return this;
        }
#nullable enable

        private void Particle_Load(object sender, EventArgs e)
        {
            Location = location;
            LifeTimer.Interval = 17;
        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            //if (!init) return;

            //Console.WriteLine(ticks += LifeTimer.Interval);
            Location += moveVector;        
            Opacity = spawnTime <= originalSpawnTime? (spawnTime += LifeTimer.Interval) / (double)originalSpawnTime : (timeLeft -= LifeTimer.Interval)/ (double)originalTime;
            if (timeLeft <= 0 && Ghost != null)
            {
                //     ObjectPool<Particle>.Instance.Return(this);
                if (Ghost.particleCache.ContainsKey(textureKey))
                {
                    Ghost.particleCache[textureKey].maps.AddLast((Bitmap)BackgroundImage);
                }
                Ghost.SetParticle(this);
                Thread.Sleep(175);             
            }
           // LifeTimer.Interval = 100;
        }
    }
}
