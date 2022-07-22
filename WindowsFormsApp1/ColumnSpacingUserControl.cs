using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowEst
{
    public partial class ColumnSpacingUserControl : UserControl
    {
        public ColumnSpacingUserControl()
        {
            InitializeComponent();
        }

        private void ColumnSpacingUserControl_Load(object sender, EventArgs e)
        {

        }

        public string qty
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }
        public string spacing
        {
            get
            {
                return textBox2.Text;
            }
            set
            {
                textBox2.Text = value;
            }
        }
    }
}
