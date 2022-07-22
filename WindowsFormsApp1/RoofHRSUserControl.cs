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
    public partial class RoofHRSUserControl : UserControl
    {
        public RoofHRSUserControl()
        {
            InitializeComponent();
            roof_GI_D_EC_cbx.SelectedIndex = 0;
        }

        public string value
        {
            get
            {
                return roof_GI_D_EC_cbx.Text;
            }
            set
            {
                roof_GI_D_EC_cbx.Text = value;
            }
        }
    }
}
