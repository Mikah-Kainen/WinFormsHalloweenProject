using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        bool init = false;
        //  Bitmap realBackgroundImage;
#nullable disable
        public Particle()
        {
            InitializeComponent();
        }

        public Particle SetData(Bitmap backgroundImage, int lifeTime, int spawnTime, Point location, Size moveVector, float scale = .1f)
        {
            InitializeComponent();

            scaleDown = (int)(1 / scale);

            BackColor = Color.Lime;
            BackgroundImage = (Bitmap)backgroundImage.Clone();
            BackgroundImageLayout = ImageLayout.Zoom;
            originalTime = timeLeft = lifeTime;
            originalSpawnTime = spawnTime;
            this.spawnTime = 0;
            Opacity = 0;
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = BackgroundImage.Size / scaleDown;
            this.location = location;
            TransparencyKey = BackColor;
            this.moveVector = moveVector;
            init = true;
            return this;
        }
#nullable enable

        private void Particle_Load(object sender, EventArgs e)
        {
            Location = location;
        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            if (!init) return;
            Location += moveVector;
            ClientSize = BackgroundImage.Size / scaleDown;
            Opacity = spawnTime <= originalSpawnTime? (spawnTime += LifeTimer.Interval) / (double)originalSpawnTime : (timeLeft -= LifeTimer.Interval)/ (double)originalTime;
            if (timeLeft <= 0)
            {
                ObjectPool<Particle>.Instance.Return(this);
                Close();
            }
        }
    }
}
