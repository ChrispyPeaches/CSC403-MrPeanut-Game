using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class SavesSelect : Form
    {
        public Image selectedCharacterImg { get; set; }
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
            string[] fileNames = Directory.GetFiles(Settings.Default.SavesDirectory, "*.json");

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
                fileButton.Text = Path.GetFileNameWithoutExtension(fileNames[i]);
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
            string imageLocation = Path.GetFullPath(Path.Combine(Settings.Default.AppDataDirectory, "playerImage.png"));
            
            Image playerImage = null;
            if (File.Exists(imageLocation))
            {
                playerImage = Image.FromFile(imageLocation);
            }

            Label lblNewFileName = new Label
            {
                Text = "Hero Name:",
                Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                AutoSize = true
            };

            lblNewFileName.Location = new Point((ClientSize.Width - lblNewFileName.Width) / 2, 10);

            txtNewFileName = new TextBox
            {
                Location = new Point((ClientSize.Width - 175) / 2, lblNewFileName.Bottom + 10),
                Size = new Size(175, 30)
            };

            Button btnCreateNewSave = new Button
            {
                Text = "Create Save",
                Location = new Point((ClientSize.Width - 175) / 2, txtNewFileName.Bottom + 10),
                Size = new Size(175, 40)
            };

            PictureBox pictureBox = new PictureBox
            {
                Image = playerImage ?? Resources.mrPeanut,
                //Image = Image.FromFile(imageLocation),
                Location = new Point((ClientSize.Width - 175) / 2, btnCreateNewSave.Bottom + 10),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(200, 350)
            };

            btnCreateNewSave.Click += BtnCreateNewSave_Click;

            Controls.Add(lblNewFileName);
            Controls.Add(txtNewFileName);
            Controls.Add(btnCreateNewSave);
            Controls.Add(pictureBox);
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
