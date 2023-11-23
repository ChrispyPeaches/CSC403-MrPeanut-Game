using Fall2020_CSC403_Project.code;
using System;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class QuickStartMenu : Form
    {
        private FrmLevel frmLevel;
        public QuickStartMenu(FrmLevel frmLevel)
        {
            InitializeComponent();
            this.frmLevel = frmLevel;
        }

        private void QuickStartMenu_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Controller.GameData gameData = new Controller.GameData();
            string fileName = Game.Instance.player.Name;
            gameData.UpdateData(fileName, frmLevel.currentRow, frmLevel.currentCol);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
