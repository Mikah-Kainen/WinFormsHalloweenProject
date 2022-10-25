namespace WinformsHalloweenProject
{
    #nullable disable
    partial class Particle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LifeTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // LifeTimer
            // 
            this.LifeTimer.Enabled = true;
            this.LifeTimer.Interval = 17;
            this.LifeTimer.Tick += new System.EventHandler(this.LifeTimer_Tick);
            // 
            // Particle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Particle";
            this.Text = "Particle";
            this.Load += new System.EventHandler(this.Particle_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer LifeTimer;
    }
}