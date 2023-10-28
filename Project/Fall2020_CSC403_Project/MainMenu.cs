using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Fall2020_CSC403_Project.Controller;
using Fall2020_CSC403_Project.code;
using System.IO;
using Refit;

namespace Fall2020_CSC403_Project
{
    public partial class MainMenu : Form
    {
        private string SavesDirectoryPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "..", "..", "Saves");

        private IOpenAIApi openAiApi;
        public MainMenu(IOpenAIApi openAiApi)
        {
            InitializeComponent();
            btnNew.Location = new Point((this.ClientSize.Width - btnNew.Width) / 2, (this.ClientSize.Height - btnNew.Height) / 2 - 60);
            btnContinue.Location = new Point((this.ClientSize.Width - btnContinue.Width) / 2, (this.ClientSize.Height - btnContinue.Height) / 2);
            if (!Directory.Exists(SavesDirectoryPath))
            {
                btnContinue.Enabled = false;
                btnContinue.Visible = false;
            }
            btnExit.Location = new Point((this.ClientSize.Width - btnExit.Width) / 2, (this.ClientSize.Height - btnExit.Height) / 2 + 60);

            this.openAiApi = openAiApi;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            SavesSelect savesSelectForm = new SavesSelect(false);
            savesSelectForm.StartPosition = FormStartPosition.Manual;
            savesSelectForm.Left = this.Left + (this.Width - savesSelectForm.Width) / 2;
            savesSelectForm.Top = this.Top + (this.Height - savesSelectForm.Height) / 2;
            savesSelectForm.ShowDialog();

            FrmLevel frmLevel = new FrmLevel(openAiApi);
            this.ShowInTaskbar = false;
            this.Opacity = 0;
            frmLevel.ShowDialog();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(SavesDirectoryPath))
            {
                SavesSelect savesSelectForm = new SavesSelect(true);
                savesSelectForm.StartPosition = FormStartPosition.Manual;
                savesSelectForm.Left = this.Left + (this.Width - savesSelectForm.Width) / 2;
                savesSelectForm.Top = this.Top + (this.Height - savesSelectForm.Height) / 2;
                savesSelectForm.ShowDialog();
            }

            FrmLevel frmLevel = new FrmLevel(openAiApi);
            this.ShowInTaskbar = false;
            this.Opacity = 0;
            frmLevel.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }
    }
}
