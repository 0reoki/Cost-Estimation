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
        AddStructForm asF;
        StructuralMembers sm;
        int floorCount, index;
        string type, beamScheduleName, endSupport1, endSupport2;

        public BeamRowUserControl(AddStructForm asF, StructuralMembers sm, string type, int floorCount, int index, string beamName, string endSupport1, string endSupport2)
        {
            InitializeComponent();
            this.asF = asF;
            this.sm = sm;
            this.floorCount = floorCount;
            this.index = index;
            this.type = type;
            this.beamScheduleName = beamName;
            this.endSupport1 = endSupport1;
            this.endSupport2 = endSupport2;
            beamName_cbx.SelectedIndex = 0;
            endSupportLT_cbx.SelectedIndex = 0;
            endSupportRB_cbx.SelectedIndex = 0;
            populateColumnConnection(type);
            populateBeamScheduleNames();
        }

        public void populateColumnConnection(string type)
        {
            int index1 = 0;
            int index2 = 0;
            if (type.Equals("Suspended Beam")) //Column Connection is Floor Under
            {
                foreach (string name in sm.columnNames[floorCount - 1])
                {
                    string toAdd1 = name + " (D)";
                    string toAdd2 = name + " (B)";
                    endSupportLT_cbx.Items.Add(toAdd1);
                    endSupportLT_cbx.Items.Add(toAdd2);
                    endSupportRB_cbx.Items.Add(toAdd1);
                    endSupportRB_cbx.Items.Add(toAdd2);
                }
            }
            else //Column Connection is Current Floor
            {
                foreach (string name in sm.columnNames[floorCount])
                {
                    string toAdd1 = name + " (D)";
                    string toAdd2 = name + " (B)";
                    endSupportLT_cbx.Items.Add(toAdd1);
                    endSupportLT_cbx.Items.Add(toAdd2);
                    endSupportRB_cbx.Items.Add(toAdd1);
                    endSupportRB_cbx.Items.Add(toAdd2);
                }
            }
        }
        public void populateBeamScheduleNames()
        {
            int i = 0;
            foreach(BeamScheduleUserControl bs in asF.bs_UC)
            {
                beamName_cbx.Items.Add(bs.name);
                endSupportLT_cbx.Items.Add(bs.name);
                endSupportRB_cbx.Items.Add(bs.name);
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
            endSupportLT_cbx.Items.Add(name);
            endSupportRB_cbx.Items.Add(name);
        }

        public void updateScheduleName(string oldName, string newName)
        {
            int selectedIndex = beamName_cbx.SelectedIndex;
            int index = beamName_cbx.FindString(oldName);
            beamName_cbx.Items.RemoveAt(index);
            beamName_cbx.Items.Insert(index, newName);
            beamName_cbx.SelectedIndex = selectedIndex;

            int selectedIndex1 = endSupportLT_cbx.SelectedIndex;
            int index1 = endSupportLT_cbx.FindString(oldName);
            endSupportLT_cbx.Items.RemoveAt(index1);
            endSupportLT_cbx.Items.Insert(index1, newName);
            endSupportLT_cbx.SelectedIndex = selectedIndex1;

            int selectedIndex2 = endSupportRB_cbx.SelectedIndex;
            int index2 = endSupportRB_cbx.FindString(oldName);
            endSupportRB_cbx.Items.RemoveAt(index2);
            endSupportRB_cbx.Items.Insert(index2, newName);
            endSupportRB_cbx.SelectedIndex = selectedIndex2;
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
