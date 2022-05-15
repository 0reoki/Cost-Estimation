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
    public partial class BeamScheduleUserControl : UserControl
    {
        private AddStructForm asF;
        public string type, oldName;
        private bool initialized;

        public BeamScheduleUserControl(AddStructForm asF, string type)
        {
            InitializeComponent();
            properties_cbx.SelectedIndex = 0;
            this.type = type;
            this.asF = asF;
            initialized = false;
            name_bx.Text = "B-" + (asF.bs_UC.Count + 1);
            oldName = name_bx.Text;
        }

        public string name
        {
            get
            {
                return name_bx.Text;
            }
            set
            {
                name_bx.Text = value;
            }
        }

        public string b
        {
            get
            {
                return b_bx.Text;
            }
            set
            {
                b_bx.Text = value;
            }
        }

        public string d
        {
            get
            {
                return d_bx.Text;
            }
            set
            {
                d_bx.Text = value;
            }
        }

        public string property
        {
            get
            {
                return properties_cbx.Text;
            }
            set
            {
                properties_cbx.Text = value;
            }
        }

        public string propertieDiameter1
        {
            get
            {
                return properties_dia_1_bx.Text;
            }
            set
            {
                properties_dia_1_bx.Text = value;
            }
        }

        public string propertieDiameter2
        {
            get
            {
                return properties_dia_2_bx.Text;
            }
            set
            {
                properties_dia_2_bx.Text = value;
            }
        }

        public string extSupport_qty1
        {
            get
            {
                return extSupport_qty1_bx.Text;
            }
            set
            {
                extSupport_qty1_bx.Text = value;
            }
        }

        public string extSupport_qty2
        {
            get
            {
                return extSupport_qty2_bx.Text;
            }
            set
            {
                extSupport_qty2_bx.Text = value;
            }
        }

        public string extSupport_qty3
        {
            get
            {
                return extSupport_qty3_bx.Text;
            }
            set
            {
                extSupport_qty3_bx.Text = value;
            }
        }

        public string extSupport_qty4
        {
            get
            {
                return extSupport_qty4_bx.Text;
            }
            set
            {
                extSupport_qty4_bx.Text = value;
            }
        }

        public string midspan_qty1
        {
            get
            {
                return midspan_qty1_bx.Text;
            }
            set
            {
                midspan_qty1_bx.Text = value;
            }
        }

        public string midspan_qty2
        {
            get
            {
                return midspan_qty2_bx.Text;
            }
            set
            {
                midspan_qty2_bx.Text = value;
            }
        }

        public string midspan_qty3
        {
            get
            {
                return midspan_qty3_bx.Text;
            }
            set
            {
                midspan_qty3_bx.Text = value;
            }
        }

        public string midspan_qty4
        {
            get
            {
                return midspan_qty4_bx.Text;
            }
            set
            {
                midspan_qty4_bx.Text = value;
            }
        }

        public string intSupport_qty1
        {
            get
            {
                return intSupport_qty1_bx.Text;
            }
            set
            {
                intSupport_qty1_bx.Text = value;
            }
        }

        public string intSupport_qty2
        {
            get
            {
                return intSupport_qty2_bx.Text;
            }
            set
            {
                intSupport_qty2_bx.Text = value;
            }
        }

        public string intSupport_qty3
        {
            get
            {
                return intSupport_qty3_bx.Text;
            }
            set
            {
                intSupport_qty3_bx.Text = value;
            }
        }

        public string intSupport_qty4
        {
            get
            {
                return intSupport_qty4_bx.Text;
            }
            set
            {
                intSupport_qty4_bx.Text = value;
            }
        }

        public string stirrupDiameter
        {
            get
            {
                return stirrups_dia_bx.Text;
            }
            set
            {
                stirrups_dia_bx.Text = value;
            }
        }

        public string stirrupsValue1
        {
            get
            {
                return stirrups_value1_bx.Text;
            }
            set
            {
                stirrups_value1_bx.Text = value;
            }
        }

        public string stirrupsValueAt1
        {
            get
            {
                return stirrups_valueAt1_bx.Text;
            }
            set
            {
                stirrups_valueAt1_bx.Text = value;
            }
        }

        public string stirrupsValue2
        {
            get
            {
                return stirrups_value2_bx.Text;
            }
            set
            {
                stirrups_value2_bx.Text = value;
            }
        }

        public string stirrupsValueAt2
        {
            get
            {
                return stirrups_valueAt2_bx.Text;
            }
            set
            {
                stirrups_valueAt2_bx.Text = value;
            }
        }

        public string stirrupsRest
        {
            get
            {
                return stirrups_rest_bx.Text;
            }
            set
            {
                stirrups_rest_bx.Text = value;
            }
        }

        public string webBarsDiameter
        {
            get
            {
                return webBars_dia_bx.Text;
            }
            set
            {
                webBars_dia_bx.Text = value;
            }
        }

        public string webBarsQty
        {
            get
            {
                return webBars_qty_bx.Text;
            }
            set
            {
                webBars_qty_bx.Text = value;
            }
        }

        private void name_bx_KeyUp(object sender, KeyEventArgs e)
        {
            int found = 0;
            foreach (BeamScheduleUserControl bs in asF.bs_UC)
            {
                if (bs.name == name_bx.Text)
                {
                    found++;
                }
                if (found == 2)
                {
                    MessageBox.Show("Duplicate names inside schedule are not allowed!");
                    name_bx.Text = oldName;
                    foreach (BeamRowUserControl br in asF.br_UC)
                    {
                        br.updateScheduleName(oldName, name);
                    }
                    oldName = name_bx.Text;
                    return;
                }
            }
            if (name == "")
            {
                name = oldName;
            }
            foreach (BeamRowUserControl br in asF.br_UC)
            {
                br.updateScheduleName(oldName, name);
            }
            oldName = name_bx.Text;
        }
    }
}
