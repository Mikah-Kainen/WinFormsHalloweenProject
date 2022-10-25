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
        Color tint;
        int timeLeft;
        public Particle(Image backgroundImage, int lifeTime)
        {            
            InitializeComponent();
            BackgroundImage = backgroundImage;
            timeLeft = lifeTime;        

        }

        private void Particle_Load(object sender, EventArgs e)
        {

        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            timeLeft -= LifeTimer.Interval;
            if (timeLeft <= 0) this.Close();
        }
    }
}
