using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RESUPUB
{
    public partial class cConsole : Form
    {
        public delegate void DUpdateListBox(string s);
        public cConsole()
        {
            InitializeComponent();
        }

        private void cConsole_Load(object sender, EventArgs e)
        {

        }


        public void UpdateListBox(string s)
        {
            if (this.InvokeRequired)

            {
                this.Invoke(new DUpdateListBox(UpdateListBox), s);
            }
            else
            {
                listBox1.Items.Insert(0, s);
            }
                
        }
    }
}
