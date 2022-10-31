namespace Rectangle_Hueristic
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.XBox = new System.Windows.Forms.TextBox();
            this.YBox = new System.Windows.Forms.TextBox();
            this.XLabel = new System.Windows.Forms.Label();
            this.YLabel = new System.Windows.Forms.Label();
            this.Add = new System.Windows.Forms.Button();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.HeightBox = new System.Windows.Forms.TextBox();
            this.WidthBox = new System.Windows.Forms.TextBox();
            this.FadeTimer = new System.Windows.Forms.Timer(this.components);
            this.canvasBox = new System.Windows.Forms.PictureBox();
            this.Recycle = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.aspectRatioBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BestRectLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.canvasBox)).BeginInit();
            this.SuspendLayout();
            // 
            // XBox
            // 
            this.XBox.Location = new System.Drawing.Point(931, 12);
            this.XBox.Name = "XBox";
            this.XBox.Size = new System.Drawing.Size(125, 27);
            this.XBox.TabIndex = 0;
            // 
            // YBox
            // 
            this.YBox.Location = new System.Drawing.Point(931, 45);
            this.YBox.Name = "YBox";
            this.YBox.Size = new System.Drawing.Size(125, 27);
            this.YBox.TabIndex = 1;
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Location = new System.Drawing.Point(908, 15);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(18, 20);
            this.XLabel.TabIndex = 7;
            this.XLabel.Text = "X";
            // 
            // YLabel
            // 
            this.YLabel.AutoSize = true;
            this.YLabel.Location = new System.Drawing.Point(908, 52);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(17, 20);
            this.YLabel.TabIndex = 8;
            this.YLabel.Text = "Y";
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(952, 144);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(94, 29);
            this.Add.TabIndex = 4;
            this.Add.Text = "Add";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(872, 114);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(54, 20);
            this.HeightLabel.TabIndex = 6;
            this.HeightLabel.Text = "Height";
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(872, 85);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(49, 20);
            this.WidthLabel.TabIndex = 5;
            this.WidthLabel.Text = "Width";
            // 
            // HeightBox
            // 
            this.HeightBox.Location = new System.Drawing.Point(931, 111);
            this.HeightBox.Name = "HeightBox";
            this.HeightBox.Size = new System.Drawing.Size(125, 27);
            this.HeightBox.TabIndex = 3;
            // 
            // WidthBox
            // 
            this.WidthBox.Location = new System.Drawing.Point(931, 78);
            this.WidthBox.Name = "WidthBox";
            this.WidthBox.Size = new System.Drawing.Size(125, 27);
            this.WidthBox.TabIndex = 2;
            // 
            // FadeTimer
            // 
            this.FadeTimer.Interval = 17;
            this.FadeTimer.Tick += new System.EventHandler(this.FadeTimer_Tick);
            // 
            // canvasBox
            // 
            this.canvasBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasBox.Location = new System.Drawing.Point(0, 0);
            this.canvasBox.Name = "canvasBox";
            this.canvasBox.Size = new System.Drawing.Size(1068, 603);
            this.canvasBox.TabIndex = 10;
            this.canvasBox.TabStop = false;
            this.canvasBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.canvasBox_MouseClick);
            // 
            // Recycle
            // 
            this.Recycle.Location = new System.Drawing.Point(952, 179);
            this.Recycle.Name = "Recycle";
            this.Recycle.Size = new System.Drawing.Size(94, 29);
            this.Recycle.TabIndex = 11;
            this.Recycle.Text = "Recycle";
            this.Recycle.UseVisualStyleBackColor = true;
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(952, 214);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(94, 29);
            this.Clear.TabIndex = 12;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // aspectRatioBox
            // 
            this.aspectRatioBox.Location = new System.Drawing.Point(931, 249);
            this.aspectRatioBox.Name = "aspectRatioBox";
            this.aspectRatioBox.Size = new System.Drawing.Size(125, 27);
            this.aspectRatioBox.TabIndex = 13;
            this.aspectRatioBox.Text = "1 : 1";
            this.aspectRatioBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(837, 252);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "AspectRatio";
            // 
            // BestRectLabel
            // 
            this.BestRectLabel.AutoSize = true;
            this.BestRectLabel.BackColor = System.Drawing.Color.CornflowerBlue;
            this.BestRectLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BestRectLabel.ForeColor = System.Drawing.Color.Snow;
            this.BestRectLabel.Location = new System.Drawing.Point(363, 118);
            this.BestRectLabel.Name = "BestRectLabel";
            this.BestRectLabel.Size = new System.Drawing.Size(51, 20);
            this.BestRectLabel.TabIndex = 15;
            this.BestRectLabel.Text = "label2";
            this.BestRectLabel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 603);
            this.Controls.Add(this.BestRectLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.aspectRatioBox);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.Recycle);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.HeightBox);
            this.Controls.Add(this.WidthBox);
            this.Controls.Add(this.YBox);
            this.Controls.Add(this.XBox);
            this.Controls.Add(this.HeightLabel);
            this.Controls.Add(this.WidthLabel);
            this.Controls.Add(this.YLabel);
            this.Controls.Add(this.XLabel);
            this.Controls.Add(this.canvasBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.canvasBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox XBox;
        private TextBox YBox;
        private Label XLabel;
        private Label YLabel;
        private Button Add;
        private Label HeightLabel;
        private Label WidthLabel;
        private TextBox HeightBox;
        private TextBox WidthBox;
        private System.Windows.Forms.Timer FadeTimer;
        private PictureBox canvasBox;
        private Button Recycle;
        private Button Clear;
        private TextBox aspectRatioBox;
        private Label label1;
        private Label BestRectLabel;
    }
}