using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USB4._0_Automation
{
    public partial class diagram : Form
    {
        public diagram()
        {
            InitializeComponent();
        }

        private void diagram_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_mouseDown(object sender, MouseEventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.button02;
        }

        private void button1_mouseMove(object sender, MouseEventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.button02;
        }

        private void button1_mouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.button01;
        }

        private void button1_mouseUp(object sender, MouseEventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.button01;
        }

        
    }
}
