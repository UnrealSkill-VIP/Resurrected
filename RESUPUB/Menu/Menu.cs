using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Menu
{
    class Menu
    {
        public int pointX { get; set; }
        public int pointY { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        List<Category> category = new List<Category>();

        public Menu(int x, int y, int width, int height)
        {
            pointX = x;
            pointY = y;
            this.width = width;
            this.height = height;
        }
    }
}
