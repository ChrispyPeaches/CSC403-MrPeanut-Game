namespace Fall2020_CSC403_Project
{
    partial class Form1
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnUpArrow = new System.Windows.Forms.Button();
            this.btnDownArrow = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.player;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(12, 233);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(251, 273);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnUpArrow
            // 
            this.btnUpArrow.BackColor = System.Drawing.Color.Transparent;
            this.btnUpArrow.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.up_arrow;
            this.btnUpArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUpArrow.FlatAppearance.BorderSize = 0;
            this.btnUpArrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpArrow.Location = new System.Drawing.Point(198, 12);
            this.btnUpArrow.Name = "btnUpArrow";
            this.btnUpArrow.Size = new System.Drawing.Size(65, 61);
            this.btnUpArrow.TabIndex = 2;
            this.btnUpArrow.UseVisualStyleBackColor = false;
            this.btnUpArrow.Click += new System.EventHandler(this.btnUpArrow_Click);
            // 
            // btnDownArrow
            // 
            this.btnDownArrow.BackColor = System.Drawing.Color.Transparent;
            this.btnDownArrow.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.down_arrow;
            this.btnDownArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDownArrow.FlatAppearance.BorderSize = 0;
            this.btnDownArrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownArrow.Location = new System.Drawing.Point(198, 82);
            this.btnDownArrow.Name = "btnDownArrow";
            this.btnDownArrow.Size = new System.Drawing.Size(65, 61);
            this.btnDownArrow.TabIndex = 3;
            this.btnDownArrow.UseVisualStyleBackColor = false;
            this.btnDownArrow.Click += new System.EventHandler(this.btnDownArrow_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Linen;
            this.button1.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.foam_hat;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Linen;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Linen;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Linen;
            this.button1.ForeColor = System.Drawing.Color.Transparent;
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 131);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.shopkeeper_background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(919, 518);
            this.Controls.Add(this.btnDownArrow);
            this.Controls.Add(this.btnUpArrow);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnUpArrow;
        private System.Windows.Forms.Button btnDownArrow;
        private System.Windows.Forms.Button button1;
    }
}