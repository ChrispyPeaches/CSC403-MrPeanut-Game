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
using static Fall2020_CSC403_Project.Controller;

namespace Fall2020_CSC403_Project
{
    public partial class SavesSelect : Form
    {
        public bool selectExistingSave { get; set; }
        public string newFileName { get; set; }
        private TextBox txtNewFileName { get; set; }
        private Controller.GameData gameData = new Controller.GameData();

        public SavesSelect(bool continueGame)
        {
            this.selectExistingSave = continueGame;
            InitializeComponent();

            if (this.selectExistingSave)
            {
                InitializeForExistingSave();
            }
            else
            {
                InitializeForNewSave();
            }
        }

        private void InitializeForExistingSave()
        {
            string savesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "..", "..", "Saves");
            string[] fileNames = Directory.GetFiles(savesDirectory, "*.json");

            int buttonWidth = 175;
            int buttonHeight = 40;
            int spacingX = 10;
            int spacingY = 10; 
            int columns = 5;

            for (int i = 0; i < fileNames.Length; i++)
            {
                int row = i / columns;
                int column = i % columns;

                Button fileButton = new Button();
                fileButton.Text = Path.GetFileName(fileNames[i]);
                fileButton.Tag = fileNames[i];
                fileButton.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
                fileButton.Size = new Size(buttonWidth, buttonHeight);
                fileButton.Location = new Point(column * (buttonWidth + spacingX), row * (buttonHeight + spacingY));
                fileButton.Click += FileButton_Click;

                Controls.Add(fileButton);
            }
        }


        private void InitializeForNewSave()
        {
            Label lblNewFileName = new Label();
            lblNewFileName.Text = "Enter a new file name:";
            lblNewFileName.Location = new Point(10, 10);
            txtNewFileName = new TextBox();  
            txtNewFileName.Location = new Point(10, 40);
            Button btnCreateNewSave = new Button();
            btnCreateNewSave.Text = "Create New Save";
            btnCreateNewSave.Location = new Point(10, 70);
            btnCreateNewSave.Click += BtnCreateNewSave_Click;
            Controls.Add(lblNewFileName);
            Controls.Add(txtNewFileName);
            Controls.Add(btnCreateNewSave);
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            string existingFileName = Path.GetFileNameWithoutExtension((string)((Button)sender).Tag);
            var saveDataToReturn = Controller.GameData.RetrieveData(existingFileName);
            Game game = new Game(saveDataToReturn);
            DialogResult = DialogResult.OK;
            Close();
        }

        private async void BtnCreateNewSave_Click(object sender, EventArgs e)
        {
            string newFileName = txtNewFileName.Text;
            Task saveTask = Task.Run(() => gameData.SaveData(newFileName));
            await saveTask;
            var saveDataToReturn = Controller.GameData.RetrieveData(newFileName);
            Game game = new Game(saveDataToReturn);
            DialogResult = DialogResult.OK;
            Close();
        }

    }
}
