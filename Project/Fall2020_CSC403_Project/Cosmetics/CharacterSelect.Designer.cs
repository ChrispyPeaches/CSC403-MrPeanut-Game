namespace Fall2020_CSC403_Project
{
    partial class CharacterSelect
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
            this.grimace = new System.Windows.Forms.PictureBox();
            this.pepsiMan = new System.Windows.Forms.PictureBox();
            this.phil = new System.Windows.Forms.PictureBox();
            this.mrPeanut = new System.Windows.Forms.PictureBox();
            this.customTextBox1 = new Fall2020_CSC403_Project.CustomTextBox();
            this.customTextBox2 = new Fall2020_CSC403_Project.CustomTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.grimace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pepsiMan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.phil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mrPeanut)).BeginInit();
            this.SuspendLayout();
            // 
            // grimace
            // 
            this.grimace.BackColor = System.Drawing.Color.Transparent;
            this.grimace.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.grimace;
            this.grimace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grimace.Location = new System.Drawing.Point(12, 120);
            this.grimace.Name = "grimace";
            this.grimace.Size = new System.Drawing.Size(190, 320);
            this.grimace.TabIndex = 0;
            this.grimace.TabStop = false;
            this.grimace.Click += new System.EventHandler(this.grimace_Click);
            // 
            // pepsiMan
            // 
            this.pepsiMan.BackColor = System.Drawing.Color.Transparent;
            this.pepsiMan.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.pepsiMan;
            this.pepsiMan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pepsiMan.Location = new System.Drawing.Point(259, 120);
            this.pepsiMan.Name = "pepsiMan";
            this.pepsiMan.Size = new System.Drawing.Size(190, 320);
            this.pepsiMan.TabIndex = 1;
            this.pepsiMan.TabStop = false;
            this.pepsiMan.Click += new System.EventHandler(this.pepsiMan_Click);
            // 
            // phil
            // 
            this.phil.BackColor = System.Drawing.Color.Transparent;
            this.phil.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.phil;
            this.phil.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.phil.Location = new System.Drawing.Point(533, 120);
            this.phil.Name = "phil";
            this.phil.Size = new System.Drawing.Size(190, 320);
            this.phil.TabIndex = 2;
            this.phil.TabStop = false;
            this.phil.Click += new System.EventHandler(this.phil_Click);
            // 
            // mrPeanut
            // 
            this.mrPeanut.BackColor = System.Drawing.Color.Transparent;
            this.mrPeanut.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.mrPeanut;
            this.mrPeanut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mrPeanut.Location = new System.Drawing.Point(792, 120);
            this.mrPeanut.Name = "mrPeanut";
            this.mrPeanut.Size = new System.Drawing.Size(190, 320);
            this.mrPeanut.TabIndex = 3;
            this.mrPeanut.TabStop = false;
            this.mrPeanut.Click += new System.EventHandler(this.mrPeanut_Click);
            // 
            // customTextBox1
            // 
            this.customTextBox1.BackColor = System.Drawing.Color.Transparent;
            this.customTextBox1.Font = new System.Drawing.Font("Showcard Gothic", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customTextBox1.ForeColor = System.Drawing.Color.Black;
            this.customTextBox1.Location = new System.Drawing.Point(69, 29);
            this.customTextBox1.Name = "customTextBox1";
            this.customTextBox1.Size = new System.Drawing.Size(887, 85);
            this.customTextBox1.TabIndex = 4;
            this.customTextBox1.Text = "CHOOSE YOUR CHARACTER";
            // 
            // customTextBox2
            // 
            this.customTextBox2.BackColor = System.Drawing.Color.Transparent;
            this.customTextBox2.ForeColor = System.Drawing.Color.White;
            this.customTextBox2.Location = new System.Drawing.Point(14, 451);
            this.customTextBox2.Name = "customTextBox2";
            this.customTextBox2.Size = new System.Drawing.Size(187, 45);
            this.customTextBox2.TabIndex = 5;
            this.customTextBox2.Text = "";
            // 
            // CharacterSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.wall;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(994, 564);
            this.Controls.Add(this.customTextBox2);
            this.Controls.Add(this.customTextBox1);
            this.Controls.Add(this.mrPeanut);
            this.Controls.Add(this.phil);
            this.Controls.Add(this.pepsiMan);
            this.Controls.Add(this.grimace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CharacterSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Your Character!";
            ((System.ComponentModel.ISupportInitialize)(this.grimace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pepsiMan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.phil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mrPeanut)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox grimace;
        private System.Windows.Forms.PictureBox pepsiMan;
        private System.Windows.Forms.PictureBox phil;
        private System.Windows.Forms.PictureBox mrPeanut;
        private CustomTextBox customTextBox1;
        private CustomTextBox customTextBox2;
    }
}