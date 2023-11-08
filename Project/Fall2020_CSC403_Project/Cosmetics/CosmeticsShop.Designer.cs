namespace Fall2020_CSC403_Project
{
    partial class CosmeticsShop
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CosmeticsShop));
            this.pictureBox_player = new System.Windows.Forms.PictureBox();
            this.btn_ChooseHat_UpArrow = new System.Windows.Forms.Button();
            this.btn_ChooseHat_DownArrow = new System.Windows.Forms.Button();
            this.btnSelectedHat = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_AdjustHatHeight_DownArrow = new System.Windows.Forms.Button();
            this.btn_AdjustHatHeight_UpArrow = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_player)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_player
            // 
            this.pictureBox_player.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_player.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_player.BackgroundImage")));
            this.pictureBox_player.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_player.ErrorImage = null;
            this.pictureBox_player.InitialImage = null;
            this.pictureBox_player.Location = new System.Drawing.Point(16, 256);
            this.pictureBox_player.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox_player.Name = "pictureBox_player";
            this.pictureBox_player.Size = new System.Drawing.Size(383, 367);
            this.pictureBox_player.TabIndex = 1;
            this.pictureBox_player.TabStop = false;
            // 
            // btn_ChooseHat_UpArrow
            // 
            this.btn_ChooseHat_UpArrow.BackColor = System.Drawing.Color.Transparent;
            this.btn_ChooseHat_UpArrow.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.up_arrow;
            this.btn_ChooseHat_UpArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_ChooseHat_UpArrow.FlatAppearance.BorderSize = 0;
            this.btn_ChooseHat_UpArrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ChooseHat_UpArrow.Location = new System.Drawing.Point(433, 5);
            this.btn_ChooseHat_UpArrow.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ChooseHat_UpArrow.Name = "btn_ChooseHat_UpArrow";
            this.btn_ChooseHat_UpArrow.Size = new System.Drawing.Size(87, 75);
            this.btn_ChooseHat_UpArrow.TabIndex = 2;
            this.btn_ChooseHat_UpArrow.UseVisualStyleBackColor = false;
            this.btn_ChooseHat_UpArrow.Click += new System.EventHandler(this.btnUpArrow_Click);
            // 
            // btn_ChooseHat_DownArrow
            // 
            this.btn_ChooseHat_DownArrow.BackColor = System.Drawing.Color.Transparent;
            this.btn_ChooseHat_DownArrow.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.down_arrow;
            this.btn_ChooseHat_DownArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_ChooseHat_DownArrow.FlatAppearance.BorderSize = 0;
            this.btn_ChooseHat_DownArrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ChooseHat_DownArrow.Location = new System.Drawing.Point(433, 91);
            this.btn_ChooseHat_DownArrow.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ChooseHat_DownArrow.Name = "btn_ChooseHat_DownArrow";
            this.btn_ChooseHat_DownArrow.Size = new System.Drawing.Size(87, 75);
            this.btn_ChooseHat_DownArrow.TabIndex = 3;
            this.btn_ChooseHat_DownArrow.UseVisualStyleBackColor = false;
            this.btn_ChooseHat_DownArrow.Click += new System.EventHandler(this.btnDownArrow_Click);
            // 
            // btnSelectedHat
            // 
            this.btnSelectedHat.BackColor = System.Drawing.Color.Linen;
            this.btnSelectedHat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSelectedHat.FlatAppearance.BorderColor = System.Drawing.Color.Linen;
            this.btnSelectedHat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Linen;
            this.btnSelectedHat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Linen;
            this.btnSelectedHat.ForeColor = System.Drawing.Color.Transparent;
            this.btnSelectedHat.Location = new System.Drawing.Point(185, 5);
            this.btnSelectedHat.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectedHat.Name = "btnSelectedHat";
            this.btnSelectedHat.Size = new System.Drawing.Size(240, 161);
            this.btnSelectedHat.TabIndex = 0;
            this.btnSelectedHat.UseVisualStyleBackColor = false;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.exit_sign;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Location = new System.Drawing.Point(5, 5);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(116, 75);
            this.btnExit.TabIndex = 4;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Font = new System.Drawing.Font("Bookshelf Symbol 7", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(185, 173);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(240, 58);
            this.button1.TabIndex = 5;
            this.button1.Text = "Choose Hat";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button2.Font = new System.Drawing.Font("Bookshelf Symbol 7", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(501, 492);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 105);
            this.button2.TabIndex = 6;
            this.button2.Text = "Adjust Hat Height";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // btn_AdjustHatHeight_DownArrow
            // 
            this.btn_AdjustHatHeight_DownArrow.BackColor = System.Drawing.Color.Transparent;
            this.btn_AdjustHatHeight_DownArrow.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.down_arrow;
            this.btn_AdjustHatHeight_DownArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_AdjustHatHeight_DownArrow.FlatAppearance.BorderSize = 0;
            this.btn_AdjustHatHeight_DownArrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_AdjustHatHeight_DownArrow.Location = new System.Drawing.Point(407, 547);
            this.btn_AdjustHatHeight_DownArrow.Margin = new System.Windows.Forms.Padding(4);
            this.btn_AdjustHatHeight_DownArrow.Name = "btn_AdjustHatHeight_DownArrow";
            this.btn_AdjustHatHeight_DownArrow.Size = new System.Drawing.Size(87, 75);
            this.btn_AdjustHatHeight_DownArrow.TabIndex = 8;
            this.btn_AdjustHatHeight_DownArrow.UseVisualStyleBackColor = false;
            // 
            // btn_AdjustHatHeight_UpArrow
            // 
            this.btn_AdjustHatHeight_UpArrow.BackColor = System.Drawing.Color.Transparent;
            this.btn_AdjustHatHeight_UpArrow.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.up_arrow;
            this.btn_AdjustHatHeight_UpArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_AdjustHatHeight_UpArrow.FlatAppearance.BorderSize = 0;
            this.btn_AdjustHatHeight_UpArrow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_AdjustHatHeight_UpArrow.Location = new System.Drawing.Point(407, 461);
            this.btn_AdjustHatHeight_UpArrow.Margin = new System.Windows.Forms.Padding(4);
            this.btn_AdjustHatHeight_UpArrow.Name = "btn_AdjustHatHeight_UpArrow";
            this.btn_AdjustHatHeight_UpArrow.Size = new System.Drawing.Size(87, 75);
            this.btn_AdjustHatHeight_UpArrow.TabIndex = 7;
            this.btn_AdjustHatHeight_UpArrow.UseVisualStyleBackColor = false;
            // 
            // CosmeticsShop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.shopkeeper_background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1225, 638);
            this.Controls.Add(this.btn_AdjustHatHeight_DownArrow);
            this.Controls.Add(this.btn_AdjustHatHeight_UpArrow);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btn_ChooseHat_DownArrow);
            this.Controls.Add(this.btn_ChooseHat_UpArrow);
            this.Controls.Add(this.pictureBox_player);
            this.Controls.Add(this.btnSelectedHat);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CosmeticsShop";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_player)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox_player;
        private System.Windows.Forms.Button btn_ChooseHat_UpArrow;
        private System.Windows.Forms.Button btn_ChooseHat_DownArrow;
        private System.Windows.Forms.Button btnSelectedHat;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btn_AdjustHatHeight_DownArrow;
        private System.Windows.Forms.Button btn_AdjustHatHeight_UpArrow;
    }
}