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
    public partial class SlabDetail1UserControl : UserControl
    {
        public SlabDetail1UserControl()
        {
            InitializeComponent();
        }

        public string SD1UC_LA
        {
            get
            {
                return SD1UC_LA_bx.Text;
            }
            set
            {
                SD1UC_LA_bx.Text = value;
            }
        }
        public string SD1UC_LB
        {
            get
            {
                return SD1UC_LB_bx.Text;
            }
            set
            {
                SD1UC_LB_bx.Text = value;
            }
        }
        public string SD1UC_LC
        {
            get
            {
                return SD1UC_LC_bx.Text;
            }
            set
            {
                SD1UC_LC_bx.Text = value;
            }
        }
        public string SD1UC_LD
        {
            get
            {
                return SD1UC_LD_bx.Text;
            }
            set
            {
                SD1UC_LD_bx.Text = value;
            }
        }
    }
}
