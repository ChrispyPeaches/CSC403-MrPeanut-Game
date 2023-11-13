using System.Drawing;

namespace Fall2020_CSC403_Project.Cosmetics
{
    public class Hat
    {
        public string PreferenceResourceName { get; set; }
        public string Name { get; set; }
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public string ImageResourceName { get; set; }
        public Bitmap Image { get; set; }
    }
}
