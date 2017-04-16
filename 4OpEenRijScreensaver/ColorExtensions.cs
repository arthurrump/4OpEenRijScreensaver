using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _4OpEenRijScreensaver
{
    public static class ColorExtensions
    {
        public static double Distance(this Color color, Color other) 
            => Math.Sqrt(
                    Math.Pow(color.R - other.R, 2) +
                    Math.Pow(color.G - other.G, 2) +
                    Math.Pow(color.B - other.B, 2)
               );
    }
}
