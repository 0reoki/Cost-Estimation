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
    public partial class AddStructForm : Form
    {
        //Passed Variables
        private List<TreeNode> nodes;
        private int floorCount;

        public AddStructForm(int floorCount, List<TreeNode> nodes)
        {
            InitializeComponent();

            //Init variables
            this.floorCount = floorCount;
            this.nodes = nodes;

            //Init components
            if (floorCount != 0)
            {
                addstruct_cbx.Items.Clear();
                addstruct_cbx.Items.Add("Column");
                addstruct_cbx.Items.Add("Beam");
            }
            addstruct_cbx.SelectedIndex = 0;
            setDefaultStructMemName();

            //Remove tab control tabpages
            addstructTabControl.Appearance = TabAppearance.FlatButtons;
            addstructTabControl.ItemSize = new Size(0, 1);
            addstructTabControl.SizeMode = TabSizeMode.Fixed;

            foreach (TabPage tab in addstructTabControl.TabPages)
            {
                tab.Text = "";
            }
        }

        private void earth_SaveBtn_Click(object sender, EventArgs e)
        {
            //TODO lagyan ng dialogresult na yes no before this line
            this.DialogResult = DialogResult.OK;
        }


        //Getters and Setters
        public string structuralMemberType
        {
            get
            {
                return addstruct_cbx.SelectedItem.ToString();
            }
            set
            {
                addstruct_cbx.Text = value;
            }
        }

        private void addstruct_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            setDefaultStructMemName();
        }

        //TODO add other structural members
        private void setDefaultStructMemName()
        {
            if (addstruct_cbx.Text.Equals("Footing (Column)"))
            {
                addstruct_Name_bx.Text = "F-" + (nodes[0].Nodes.Count + 1);
            }
        }
    }
}
