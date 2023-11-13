using Fall2020_CSC403_Project.Cosmetics;
using Fall2020_CSC403_Project.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class CosmeticsShop : Form
    {
        public Image cosmeticPlayerImage { get; set; }

        IList<Hat> hats;
        int currentHatsIndex = 0;

        /// <summary>
        /// The amount of pixels to adjust the hat height by when clicking up or down in the hat adjustment menu
        /// </summary>
        int hatAdjustmentAmount = 2;

        public CosmeticsShop()
        {
            cosmeticPlayerImage = Resources.mrPeanut;
            InitializeComponent();

            GetAvailableHats();
            DisplayCurrentHat();
        }

        /// <summary>
        /// Gather a list of selectable hats from the Cosmetics Resources collection
        /// </summary>
        private void GetAvailableHats()
        {
            IList<PropertyInfo> hatProperties = typeof(CosmeticsResources)
                .GetProperties()
                .Where(prop => prop.Name.Contains("hat"))
                .ToList();

            // Gather the images for hats
            IList<PropertyInfo> hatImages = hatProperties
                .Where(prop => prop.PropertyType.Equals(typeof(Bitmap)))
                .ToList();

            // Gather data for each hat
            hats = hatProperties
                .Where(prop => prop.PropertyType.Equals(typeof(string)))
                .Select(prop => 
                { 
                    Hat hat = JsonConvert.DeserializeObject<Hat>((string)prop.GetValue(null));
                    hat.PreferenceResourceName = prop.Name;
                    hat.Image = (Bitmap)hatImages
                                .First(hatImage => hatImage.Name == hat.ImageResourceName)
                                .GetValue(null);
                    return hat;
                })
                .ToList();

            // Add an option for no hat
            hats.Insert(0, new Hat() { Name = "Bald" });

            // Set to no hat by default
            currentHatsIndex = 0;

            RedrawPlayer();
        }

        /// <summary>
        /// Put the currently selected hat in the hat display box
        /// </summary>
        private void DisplayCurrentHat()
        {
            btnSelectedHat.BackgroundImage = hats.ElementAt(currentHatsIndex).Image;
        }

        /// <summary>
        /// Display the player image with the new cosmetics
        /// </summary>
        private void RedrawPlayer()
        {
            Hat currentHat = hats.ElementAt(currentHatsIndex);

            // Redraw player without a hat
            if (currentHat.Image == null)
            {
                pictureBox_player.BackgroundImage = new Bitmap(cosmeticPlayerImage);
                ShouldDisplayAdjustHatHeightMenu(false);
            }
            // Redraw player with a hat
            else
            {
                Bitmap hatImage = (Bitmap)btnSelectedHat.BackgroundImage;
                Bitmap playerImage = new Bitmap(cosmeticPlayerImage);

                using (var graphics = Graphics.FromImage(playerImage))
                {
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                    graphics.DrawImage(hatImage, currentHat.XCoordinate, currentHat.YCoordinate);

                    pictureBox_player.BackgroundImage = playerImage;
                }

                ShouldDisplayAdjustHatHeightMenu(true);
            }
        }

        #region Choose Hat Menu

        /// <summary>
        /// When the up arrow for choosing a hat is clicked, show the last hat in the list with wraparound indexing
        /// </summary>
        private void btn_ChooseHat_UpArrow_Click(object sender, EventArgs e)
        {
            currentHatsIndex--;
            if (currentHatsIndex < 0)
            {
                currentHatsIndex = hats.Count - 1;
            }
            currentHatsIndex = currentHatsIndex % hats.Count;
            DisplayCurrentHat();
            RedrawPlayer();
        }

        /// <summary>
        /// When the down arrow for choosing a hat is clicked, show the next hat in the list with wraparound indexing
        /// </summary>
        private void btn_ChooseHat_DownArrow_Click(object sender, EventArgs e)
        {
            currentHatsIndex++;
            currentHatsIndex = currentHatsIndex % hats.Count;
            DisplayCurrentHat();
            RedrawPlayer();
        }

        #endregion

        #region Adjust Hat Height Menu

        private void btn_AdjustHatHeight_UpArrow_Click(object sender, EventArgs e)
        {
            hats.ElementAt(currentHatsIndex).YCoordinate -= hatAdjustmentAmount;
            RedrawPlayer();
        }

        private void btn_AdjustHatHeight_DownArrow_Click(object sender, EventArgs e)
        {

            hats.ElementAt(currentHatsIndex).YCoordinate += hatAdjustmentAmount;
            RedrawPlayer();
        }

        private void btn_AdjustHatHeight_RightArrow_Click(object sender, EventArgs e)
        {
            hats.ElementAt(currentHatsIndex).XCoordinate += hatAdjustmentAmount;
            RedrawPlayer();
        }

        private void btn_AdjustHatHeight_LeftArrow_Click(object sender, EventArgs e)
        {
            hats.ElementAt(currentHatsIndex).XCoordinate -= hatAdjustmentAmount;
            RedrawPlayer();
        }

        /// <summary>
        /// If the adjust hat height menu is hidden, display it, if it's displayed, hide it
        /// </summary>
            public void ShouldDisplayAdjustHatHeightMenu(bool showMenu)
        {
            btn_AdjustHatHeight_UpArrow.Visible = showMenu;
            btn_AdjustHatHeight_DownArrow.Visible = showMenu;
            btn_AdjustHatHeight_LeftArrow.Visible = showMenu;
            btn_AdjustHatHeight_RightArrow.Visible = showMenu;
            btn_AdjustHat_Title.Visible = showMenu;
        }

        #endregion

        /// <summary>
        /// Close the form
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            SaveNewPlayerImage(new Bitmap(pictureBox_player.BackgroundImage));
            SaveHatPreferences();
            Close();
        }

        /// <summary>
        /// Save the hat preferences to a save file
        /// </summary>
        public void SaveHatPreferences()
        {
            /*foreach (Hat hat in hats.Where(hat => hat.Image != null))
            {
                
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        private async void SaveNewPlayerImage(Bitmap playerImage)
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string savesDirectoryPath = Path.Combine(appDirectory, "..", "..", "Saves");

            if (!Directory.Exists(savesDirectoryPath))
            {
                Directory.CreateDirectory(savesDirectoryPath);
            }
            using (FileStream playerImageStream = File.Create(Path.Combine(savesDirectoryPath, "playerImage.png")))
            {
                playerImage.Save(
                    playerImageStream,
                    System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void btnCharacterSelect_Click(object sender, EventArgs e)
        {
            CharacterSelect characterSelect = new CharacterSelect();
            characterSelect.ShowDialog();
            cosmeticPlayerImage = characterSelect.SelectedCharacterImg;
            RedrawPlayer();
        }
    }
}
