using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        //  Bitmap realBackgroundImage;
#nullable disable
        public Particle(Bitmap backgroundImage, int lifeTime, int spawnTime, Point location, Size moveVector, float scale = .1f)
        {
            InitializeComponent();

            scaleDown = (int)(1 / scale);

            BackColor = Color.Lime;
            BackgroundImage = (Bitmap)backgroundImage.Clone();
            this.BackgroundImageLayout = ImageLayout.Zoom;
            originalTime = timeLeft = lifeTime;
            originalSpawnTime = spawnTime;
            this.spawnTime = 0;
            Opacity = 0;
            this.FormBorderStyle = FormBorderStyle.None;
            ClientSize = BackgroundImage.Size / scaleDown;
            this.location = location;
            TransparencyKey = BackColor;
            this.moveVector = moveVector;
        }
#nullable enable

        private void Particle_Load(object sender, EventArgs e)
        {
            Location = location;
        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            Location += moveVector;
            ClientSize = BackgroundImage.Size / scaleDown;
            Opacity = spawnTime <= originalSpawnTime? (spawnTime += LifeTimer.Interval) / (double)originalSpawnTime : (timeLeft -= LifeTimer.Interval)/ (double)originalTime;
            if (timeLeft <= 0) this.Close();
        }
    }
}
