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
    public partial class BeamRowUserControl : UserControl
    {
        StructuralMembers sm;

        public BeamRowUserControl(StructuralMembers sm)
        {
            InitializeComponent();
            this.sm = sm;
            beamName_cbx.SelectedIndex = 0;
            endSupportLT_cbx.SelectedIndex = 0;
            endSupportRB_cbx.SelectedIndex = 0;
        }

        public string beamName
        {
            get
            {
                return beamName_cbx.Text;
            }
            set
            {
                beamName_cbx.Text = value;
            }
        }

        public string length
        {
            get
            {
                return length_bx.Text;
            }
            set
            {
                length_bx.Text = value;
            }
        }

        public string qty
        {
            get
            {
                return qty_bx.Text;
            }
            set
            {
                qty_bx.Text = value;
            }
        }

        public string endSupportLT
        {
            get
            {
                return endSupportLT_cbx.Text;
            }
            set
            {
                endSupportLT_cbx.Text = value;
            }
        }

        public string endSupportRB
        {
            get
            {
                return endSupportRB_cbx.Text;
            }
            set
            {
                endSupportRB_cbx.Text = value;
            }
        }
    }
}
