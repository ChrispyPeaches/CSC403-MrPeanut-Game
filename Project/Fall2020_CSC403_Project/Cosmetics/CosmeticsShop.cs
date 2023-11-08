using Fall2020_CSC403_Project.Cosmetics;
using Fall2020_CSC403_Project.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class CosmeticsShop : Form
    {
        IList<Hat> hats;
        int currentHatsIndex;

        public CosmeticsShop()
        {
            InitializeComponent();

            GetAvailableHats();
            DisplayCurrentHat();
        }

        /// <summary>
        /// Gather a list of selectable hats from the Cosmetics Resources collection
        /// </summary>
        private void GetAvailableHats()
        {
            IList<PropertyInfo> hatProperties = typeof(Cosmetics.CosmeticsResources)
                .GetProperties()
                .Where(prop => prop.Name.Contains("hat"))
                .ToList();

            hats = hatProperties
                .Where(prop => prop.PropertyType.Equals(typeof(string)))
                .Select(prop => JsonConvert.DeserializeObject<Hat>((string)prop.GetValue(null)))
                .ToList();

            // Gather the matching images for each hat
            IList<PropertyInfo> hatImages = hatProperties
                .Where(prop => prop.PropertyType.Equals(typeof(Bitmap)))
                .ToList();

            foreach (Hat hat in hats)
            {
                hat.Image = (Bitmap)hatImages
                                .First(hatImage => hatImage.Name == hat.FileName)
                                .GetValue(null);
            }

            currentHatsIndex = 0;
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

            Bitmap hatImage = (Bitmap)btnSelectedHat.BackgroundImage;
            Bitmap playerImage = new Bitmap(Resources.player);

            using (var graphics = Graphics.FromImage(playerImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                var hatStartingPoint = new Point(
                    (playerImage.Width - hatImage.Width) / 2,
                    currentHat.YCoordinate
                    );

                graphics.DrawImage(hatImage, hatStartingPoint.X, hatStartingPoint.Y);

                pictureBox_player.BackgroundImage = playerImage;
            }
        }

        /// <summary>
        /// When the up arrow is clicked, show the last hat in the list with wraparound indexing
        /// </summary>
        private void btnUpArrow_Click(object sender, EventArgs e)
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
        /// When the down arrow is clicked, show the next hat in the list with wraparound indexing
        /// </summary>
        private void btnDownArrow_Click(object sender, EventArgs e)
        {
            currentHatsIndex++;
            currentHatsIndex = currentHatsIndex % hats.Count;
            DisplayCurrentHat();
            RedrawPlayer();
        }

        /// <summary>
        /// Close the form
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
