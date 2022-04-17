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
        //Local Variables
        Compute compute = new Compute();

        //Passed Variables
        public string structMemName;
        public string oldStructMemName;
        private CostEstimationForm costEstimationForm;
        private List<TreeNode> nodes;
        private int floorCount;
        private int memberCount;
        private bool isNew;

        public AddStructForm(CostEstimationForm costEstimationForm, int floorCount, List<TreeNode> nodes, bool isNew, int index, string parentNode)
        {
            InitializeComponent();

            //Init variables
            this.costEstimationForm = costEstimationForm;
            this.floorCount = floorCount;
            this.nodes = nodes;
            this.isNew = isNew;
            this.memberCount = index;
            oldStructMemName = "";

            //Init components
            if (floorCount != 0)
            {
                addstruct_cbx.Items.Clear();
                addstruct_cbx.Items.Add("Column");
                addstruct_cbx.Items.Add("Beam");
            }
            addstruct_cbx.SelectedIndex = 0;
            foot_FT_cbx.SelectedIndex = 0;
            foot_IF_LR_HT_cbx.SelectedIndex = 0;
            foot_IF_TR_HT_cbx.SelectedIndex = 0;
            setDefaultStructMemName();

            //existing node?
            if (!isNew)
            {
                //Disable Comboboxes
                addstruct_cbx.Enabled = false;
                foot_FT_cbx.Enabled = false;
                
                //Populate
                if (parentNode.Equals("FOOTINGS"))
                {
                    setFootingValues();
                }
                
            }

            //Remove tab control tabpages
            addstructTabControl.Appearance = TabAppearance.FlatButtons;
            addstructTabControl.ItemSize = new Size(0, 1);
            addstructTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in addstructTabControl.TabPages)
            {
                tab.Text = "";
            }
            footingTabControl.Appearance = TabAppearance.FlatButtons;
            footingTabControl.ItemSize = new Size(0, 1);
            footingTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in footingTabControl.TabPages)
            {
                tab.Text = "";
            }
        }

        private void earth_SaveBtn_Click(object sender, EventArgs e)
        {
            structMemName = addstruct_Name_bx.Text;
            if (addstruct_cbx.Text.Equals("Footing (Column)"))
            {
                if (foot_FT_cbx.SelectedIndex == 0)
                {
                    if (isNew)
                    {
                        if (costEstimationForm.structuralMembers.footingColumnNames.Contains(structMemName))
                        {
                            MessageBox.Show("Name already exists!");
                            return;
                        }

                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(foot_IF_D_L_bx.Text);
                            members.Add(foot_IF_D_W_bx.Text);
                            members.Add(foot_IF_D_T_bx.Text);
                            members.Add(foot_IF_D_Q_bx.Text);
                            members.Add(foot_IF_D_D_bx.Text);
                            members.Add(foot_IF_LR_D_bx.Text);
                            members.Add(foot_IF_LR_Q_bx.Text);
                            members.Add(foot_IF_LR_HT_cbx.Text);
                            members.Add(foot_IF_TR_D_bx.Text);
                            members.Add(foot_IF_TR_Q_bx.Text);
                            members.Add(foot_IF_TR_HT_cbx.Text);

                            members.Add(foot_FT_cbx.Text); //isolated or combined

                            costEstimationForm.structuralMembers.footingColumn[floorCount].Add(members);
                            costEstimationForm.structuralMembers.footingColumnNames.Add(structMemName);

                            compute.Excavation(costEstimationForm, nodes[0].Nodes.Count);
                            MessageBox.Show("eto ang sagot sa tanong: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                        }
                    }
                    else
                    {
                        if (costEstimationForm.structuralMembers.footingColumnNames.Contains(structMemName))
                        {
                            if (structMemName.Equals(oldStructMemName))
                            {
                                //
                            }
                            else
                            {
                                //TODO find if may dalawa 
                            }
                        }
                        
                        try
                        {
                            costEstimationForm.structuralMembers.footingColumnNames.Insert(memberCount, addstruct_Name_bx.Text);
                            
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(0, foot_IF_D_L_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(1, foot_IF_D_W_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(2, foot_IF_D_T_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(3, foot_IF_D_Q_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(4, foot_IF_D_D_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(5, foot_IF_LR_D_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(6, foot_IF_LR_Q_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(7, foot_IF_LR_HT_cbx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(8, foot_IF_TR_D_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(9, foot_IF_TR_Q_bx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(10, foot_IF_TR_HT_cbx.Text);
                            costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount].Insert(11, foot_FT_cbx.Text);
                            
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {

                }
            }
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

        //TODO add other structural members from opened node
        private void setFootingValues()
        {
            if (costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][11].Equals("Isolated Footing"))
            {
                foot_FT_cbx.SelectedIndex = 0;
                oldStructMemName = costEstimationForm.structuralMembers.footingColumnNames[memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.footingColumnNames[memberCount];
                foot_IF_D_L_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][0];
                foot_IF_D_W_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][1];
                foot_IF_D_T_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][2];
                foot_IF_D_Q_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][3];
                foot_IF_D_D_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][4];
                foot_IF_LR_D_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][5];
                foot_IF_LR_Q_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][6];
                foot_IF_LR_HT_cbx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][7];
                foot_IF_TR_D_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][8];
                foot_IF_TR_Q_bx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][9];
                foot_IF_TR_HT_cbx.Text = costEstimationForm.structuralMembers.footingColumn[floorCount][memberCount][10];
            }
            else
            {
                foot_FT_cbx.SelectedIndex = 1;
            }
        }

        private void AddStructForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
