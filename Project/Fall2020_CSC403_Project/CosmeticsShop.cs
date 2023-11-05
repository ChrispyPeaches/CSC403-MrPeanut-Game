using Fall2020_CSC403_Project.Properties;
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
        IList<Bitmap> hats;
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
            var a = typeof(CosmeticsResources).GetProperties();

            IList<PropertyInfo> hatProps = a
                .Where(prop => prop.Name.Contains("hat"))
                .ToList();

            hats = hatProps
                .Select(prop => prop.GetValue(null) as Bitmap)
                .ToList();

            currentHatsIndex = 0;
        }

        /// <summary>
        /// Put the currently selected hat in the hat display box
        /// </summary>
        private void DisplayCurrentHat()
        {
            btnSelectedHat.BackgroundImage = hats.ElementAt(currentHatsIndex);
        }

        /// <summary>
        /// Display the player image with the new cosmetics
        /// </summary>
        private void RedrawPlayer()
        {

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
