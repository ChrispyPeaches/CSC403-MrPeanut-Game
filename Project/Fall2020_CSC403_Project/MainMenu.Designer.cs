using System.Drawing;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    partial class MainMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Button btnNew;
        private Button btnContinue;
        private Button btnExit;
        private Button btnCosmetics;

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
            this.btnNew = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnCosmetics = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // Calculate the X and Y coordinates for centering the buttons
            int buttonX = (this.ClientSize.Width - 200) / 2;
            int buttonY = (this.ClientSize.Height - 60 * 4 - 20 * 3) / 2; // Adjust for 4 buttons with 20 pixels spacing

            // btnNew
            this.btnNew.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Location = new System.Drawing.Point(buttonX, buttonY);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(200, 60);
            this.btnNew.Anchor = AnchorStyles.None; // Center both horizontally and vertically
            this.btnNew.Dock = DockStyle.None; // Set DockStyle to None
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            // btnContinue
            this.btnContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContinue.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnContinue.Location = new System.Drawing.Point(buttonX, buttonY + 80); // Adjust the Y-coordinate based on button height and spacing
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(200, 60);
            this.btnContinue.Anchor = AnchorStyles.None; // Center both horizontally and vertically
            this.btnContinue.Dock = DockStyle.None; // Set DockStyle to None
            this.btnContinue.TabIndex = 1;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);

            // btnExit
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(buttonX, buttonY + 160); // Adjust the Y-coordinate based on button height and spacing
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(200, 60);
            this.btnExit.Anchor = AnchorStyles.None; // Center both horizontally and vertically
            this.btnExit.Dock = DockStyle.None; // Set DockStyle to None
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // btnCosmetics
            this.btnCosmetics.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCosmetics.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCosmetics.Location = new System.Drawing.Point(buttonX, buttonY + 240); // Adjust the Y-coordinate based on button height and spacing
            this.btnCosmetics.Name = "btnCosmetics";
            this.btnCosmetics.Size = new System.Drawing.Size(200, 60);
            this.btnCosmetics.Anchor = AnchorStyles.None; // Center both horizontally and vertically
            this.btnCosmetics.Dock = DockStyle.None; // Set DockStyle to None
            this.btnCosmetics.TabIndex = 3;
            this.btnCosmetics.Text = "Cosmetics";
            this.btnCosmetics.UseVisualStyleBackColor = true;
            this.btnCosmetics.Click += new System.EventHandler(this.btnCosmetics_Click);


            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Fall2020_CSC403_Project.Properties.Resources.wall;
            this.ClientSize = new System.Drawing.Size(1342, 777);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCosmetics);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainMenu";
            this.Text = "Main Menu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.ResumeLayout(false);

        }


    }
    #endregion
}