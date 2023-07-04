using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_graphic
{
    internal class Settings
    {
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static string directions;
        //private int WH = ;
        public Settings()
        {

            Width = 15;
            Height = 15;
            directions = "left";
        }
    }
}
