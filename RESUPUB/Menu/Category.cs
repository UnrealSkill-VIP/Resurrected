using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Menu
{
    class Category
    {
        List<Item> items = new List<Item>();

        public int Id { get; set; }
        public string Name { get; set; }

        public Category()
        {

        }
    }
}
