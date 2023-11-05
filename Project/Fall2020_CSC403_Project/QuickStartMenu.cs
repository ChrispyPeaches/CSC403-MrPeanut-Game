using Fall2020_CSC403_Project.code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class QuickStartMenu : Form
    {
        private Controller.GameData gameData = new Controller.GameData();
        public QuickStartMenu()
        {
            InitializeComponent();
        }

        private void QuickStartMenu_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string fileName = Game.Instance.player.Name;
            gameData.UpdateData(fileName);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
