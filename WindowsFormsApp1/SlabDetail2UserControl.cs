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
    public partial class SlabDetail2UserControl : UserControl
    {
        public SlabDetail2UserControl()
        {
            InitializeComponent();
        }

        public string SD2UC_LA
        {
            get
            {
                return SD2UC_LA_bx.Text;
            }
            set
            {
                SD2UC_LA_bx.Text = value;
            }
        }
        public string SD2UC_LB
        {
            get
            {
                return SD2UC_LB_bx.Text;
            }
            set
            {
                SD2UC_LB_bx.Text = value;
            }
        }

        public string SD2UC_BG
        {
            get
            {
                return SD2UC_BG_bx.Text;
            }
            set
            {
                SD2UC_BG_bx.Text = value;
            }
        }
    }
}
