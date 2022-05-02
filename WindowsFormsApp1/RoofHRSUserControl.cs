using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class RoofHRSUserControl : UserControl
    {
        public RoofHRSUserControl()
        {
            InitializeComponent();
        }

        public string value
        {
            get
            {
                return col_G_S_S_bx.Text;
            }
            set
            {
                col_G_S_S_bx.Text = value;
            }
        }
    }
}
