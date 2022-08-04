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
    public partial class BeamRowUserControl : UserControl
    {
        AddStructForm asF;
        string beamScheduleName;

        public BeamRowUserControl(AddStructForm asF, string beamName)
        {
            InitializeComponent();
            this.asF = asF;
            beamScheduleName = beamName;
            beamName_cbx.SelectedIndex = 0;
            support_cbx.SelectedIndex = 0;
            populateBeamScheduleNames();
        }

        public void populateBeamScheduleNames()
        {
            int i = 0;
            foreach(BeamScheduleUserControl bs in asF.bs_UC)
            {
                beamName_cbx.Items.Add(bs.name);
                i++;
                if (bs.name.Equals(beamScheduleName))
                {
                    beamName_cbx.SelectedIndex = i;
                }
            }
        }

        public void addScheduleName(string name)
        {
            beamName_cbx.Items.Add(name);
        }

        public void updateScheduleName(string oldName, string newName)
        {
            int selectedIndex = beamName_cbx.SelectedIndex;
            int index = beamName_cbx.FindString(oldName);
            beamName_cbx.Items.RemoveAt(index);
            beamName_cbx.Items.Insert(index, newName);
            beamName_cbx.SelectedIndex = selectedIndex;
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

        public string clearLength
        {
            get
            {
                return clearlength_bx.Text;
            }
            set
            {
                clearlength_bx.Text = value;
            }
        }

        public string support
        {
            get
            {
                return support_cbx.Text;
            }
            set
            {
                support_cbx.Text = value;
            }
        }

        private void length_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("The center to center distance between 2 supports or center to edge distance of a 1-support", length_bx);
        }

        private void clearlength_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("The length between the two inside surfaces of the span supports; the distance that is unsupported.", clearlength_bx);
        }
    }
}
