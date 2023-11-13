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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

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

        private void btnCosmetics_Click(object sender, EventArgs e)
        {
            CosmeticsShop hatsShop = new CosmeticsShop();
            hatsShop.FormBorderStyle = FormBorderStyle.None;
            hatsShop.StartPosition = FormStartPosition.Manual;
            hatsShop.Left = this.Left + (this.Width - hatsShop.Width) / 2;
            hatsShop.Top = this.Top + (this.Height - hatsShop.Height) / 2;
            hatsShop.Show();
            this.Close();
        }
    }
}
