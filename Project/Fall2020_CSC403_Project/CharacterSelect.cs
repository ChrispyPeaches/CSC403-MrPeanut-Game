using Fall2020_CSC403_Project.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class CharacterSelect : Form
    {
        public Image SelectedCharacterImg { get; set; }

        public CharacterSelect()
        {
            InitializeComponent();
        }

        private void grimace_Click(object sender, EventArgs e)
        {
            SelectedCharacterImg = Resources.grimace;
            this.Close();
        }

        private void pepsiMan_Click(object sender, EventArgs e)
        {
            SelectedCharacterImg = Resources.pepsiMan;
            this.Close();
        }

        private void phil_Click(object sender, EventArgs e)
        {
            SelectedCharacterImg = Resources.phil;
            this.Close();
        }

        private void mrPeanut_Click(object sender, EventArgs e)
        {
            SelectedCharacterImg = Resources.mrPeanut;
            this.Close();
        }
    }
}
