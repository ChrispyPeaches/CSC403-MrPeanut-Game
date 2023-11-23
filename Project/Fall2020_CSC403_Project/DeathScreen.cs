using System;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class DeathScreen : Form
    { 
        public DeathScreen()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
    }
}
