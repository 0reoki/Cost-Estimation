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
        private List<ColumnLateralTiesUserControl> g_ltUC;
        private List<ColumnLateralTiesUserControl> u_ltUC;
        private List<ColumnSpacingUserControl> g_sUC;
        private List<ColumnSpacingUserControl> u_sUC;

        //Passed Variables
        public string structMemName;
        public string oldStructMemName;
        private CostEstimationForm costEstimationForm;
        private List<TreeNode> nodes;
        private int floorCount;
        private int memberCount, footingCount, wallFootingCount, columnCount, stairsCount, roofCount;
        private bool isNew, isFooting;

        public AddStructForm(CostEstimationForm costEstimationForm, int floorCount, int footingCount, int wallFootingCount, int columnCount, int stairsCount, int roofCount, List<TreeNode> nodes, bool isNew, int index, string parentNode, bool isFooting)
        {
            InitializeComponent();

            //Init variables
            this.costEstimationForm = costEstimationForm;
            this.floorCount = floorCount;
            this.nodes = nodes;
            this.isNew = isNew;
            this.isFooting = isFooting;
            this.memberCount = index;
            this.footingCount = footingCount;
            this.wallFootingCount = wallFootingCount;
            this.columnCount = columnCount;
            this.stairsCount = stairsCount;
            this.roofCount = roofCount;

            oldStructMemName = "";
            g_ltUC = new List<ColumnLateralTiesUserControl>();
            g_sUC = new List<ColumnSpacingUserControl>();
            u_ltUC = new List<ColumnLateralTiesUserControl>();
            u_sUC = new List<ColumnSpacingUserControl>();

            //Init components
            addstruct_cbx.SelectedIndex = 0;
            colTabControl.SelectedIndex = 0;
            if (floorCount != 0)
            {
                addstruct_cbx.Items.Clear();
                addstruct_cbx.Items.Add("Column");
                addstruct_cbx.Items.Add("Beam");
                addstruct_cbx.Items.Add("Slab");
                addstruct_cbx.Items.Add("Stairs");
                addstruct_cbx.Items.Add("Roofing (Gable)");
                addstruct_cbx.SelectedIndex = 0;
                colTabControl.SelectedIndex = 1;
            }
            //Init Footings Combo Boxes
            foot_FT_cbx.SelectedIndex = foot_IF_LR_HT_cbx.SelectedIndex = foot_IF_TR_HT_cbx.SelectedIndex = 
                foot_CF_LR_HT_cbx.SelectedIndex = foot_CF_TR_HT_cbx.SelectedIndex = foot_CF_UR_HT_cbx.SelectedIndex = 
                footW_FT_cbx.SelectedIndex = footW_R_LR_HT_cbx.SelectedIndex = footW_R_TR_HT_cbx.SelectedIndex = 
                footW_T_LR_HT_cbx.SelectedIndex = footW_T_TR_HT_cbx.SelectedIndex = 0;

            //Init Column Combo Boxes
            col_G_D_CB_cbx.SelectedIndex = col_U_D_CB_cbx.SelectedIndex = col_G_LT_LTC_cbx.SelectedIndex =
                col_U_LT_LTC_cbx.SelectedIndex = col_G_SA_cbx.SelectedIndex = col_U_SA_cbx.SelectedIndex =
                col_G_ST_cbx.SelectedIndex = col_U_ST_cbx.SelectedIndex = 0;

            //Init Stairs Combo Boxes
            stairs_ST_cbx.SelectedIndex = stairs_SS_WS_MB_cbx.SelectedIndex = stairs_SS_WS_DB_cbx.SelectedIndex =
            stairs_SS_S_MB_cbx.SelectedIndex = stairs_SS_S_NB_cbx.SelectedIndex = stairs_US_WS_MB_cbx.SelectedIndex =
            stairs_US_WS_DB_cbx.SelectedIndex = stairs_US_L_MB_cbx.SelectedIndex = stairs_US_S_MB_cbx.SelectedIndex =
            stairs_US_S_NB_cbx.SelectedIndex = stairs_LS_WS_MB_cbx.SelectedIndex = stairs_LS_WS_DB_cbx.SelectedIndex =
            stairs_LS_L_MB_cbx.SelectedIndex = stairs_LS_S_MB_cbx.SelectedIndex = stairs_LS_S_NB_cbx.SelectedIndex = 0;

            //Init Roof Combo Boxes and Radio Buttons
            roof_RP_ST_D_CLTSR_cbx.SelectedIndex = roof_RP_ST_D_CLTSP_cbx.SelectedIndex =
            roof_RP_SCP_D_CLCPR_cbx.SelectedIndex = roof_RP_SCP_D_CLCPP_cbx.SelectedIndex =
            roof_GI_D_EC_cbx.SelectedIndex = roof_GI_M_SP_cbx.SelectedIndex = roof_PGR_cbx.SelectedIndex = 0;
            
            roof_RP_W_rb.Checked = true;
            roof_GI_M_CGIS_rb.Checked = true;

            setDefaultStructMemName();
            populateColumnConnectionBelow();

            //existing node?
            if (!isNew)
            {
                //Disable Comboboxes
                addstruct_cbx.Enabled = false;
                foot_FT_cbx.Enabled = false;
                footW_FT_cbx.Enabled = false;
                stairs_ST_cbx.Enabled = false;

                //Populate
                if (parentNode.Equals("FOOTINGS"))
                {
                    setFootingValues();
                }
                if (parentNode.Equals("COLUMNS"))
                {
                    setColumnValues();
                }
                else if(parentNode.Equals("STAIRS"))
                {
                    setStairsValues();
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
            footingWTabControl.Appearance = TabAppearance.FlatButtons;
            footingWTabControl.ItemSize = new Size(0, 1);
            footingWTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in footingWTabControl.TabPages)
            {
                tab.Text = "";
            }
            colTabControl.Appearance = TabAppearance.FlatButtons;
            colTabControl.ItemSize = new Size(0, 1);
            colTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in colTabControl.TabPages)
            {
                tab.Text = "";
            }
            stairsTabControl.Appearance = TabAppearance.FlatButtons;
            stairsTabControl.ItemSize = new Size(0, 1);
            stairsTabControl.SizeMode = TabSizeMode.Fixed; 
            foreach (TabPage tab in stairsTabControl.TabPages)
            {
                tab.Text = "";
            }
            roofTabControl.Appearance = TabAppearance.FlatButtons;
            roofTabControl.ItemSize = new Size(0, 1);
            roofTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in roofTabControl.TabPages)
            {
                tab.Text = "";
            }
            roof_RP_D_TabControl.Appearance = TabAppearance.FlatButtons;
            roof_RP_D_TabControl.ItemSize = new Size(0, 1);
            roof_RP_D_TabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in roof_RP_D_TabControl.TabPages)
            {
                tab.Text = "";
            }
        }

        private void earth_SaveBtn_Click(object sender, EventArgs e)
        {
            structMemName = addstruct_Name_bx.Text;
            if (addstruct_cbx.Text.Equals("Footing (Column)"))
            {
                if (isNew)
                {
                    if (foot_FT_cbx.SelectedIndex == 0) //Isolated Footing
                    {
                        if (costEstimationForm.structuralMembers.footingColumnNames.Contains(structMemName))
                        {
                            MessageBox.Show("Name already exists!");
                            return;
                        }

                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(foot_FT_cbx.Text);
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

                            costEstimationForm.structuralMembers.footingsColumn[floorCount].Add(members);
                            costEstimationForm.structuralMembers.footingColumnNames.Add(structMemName);

                            compute.AddFootingWorks(costEstimationForm, footingCount, wallFootingCount, true);
                            MessageBox.Show("eto ang sagot sa tanong: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else //Combined Footing
                    {
                        if (costEstimationForm.structuralMembers.footingColumnNames.Contains(structMemName))
                        {
                            MessageBox.Show("Name already exists!");
                            return;
                        }

                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(foot_FT_cbx.Text);
                            members.Add(foot_CF_D_L_bx.Text);
                            members.Add(foot_CF_D_W_bx.Text);
                            members.Add(foot_CF_D_T_bx.Text);
                            members.Add(foot_CF_D_Q_bx.Text);
                            members.Add(foot_CF_D_D_bx.Text);
                            members.Add(foot_CF_LR_D_bx.Text);
                            members.Add(foot_CF_LR_Q_bx.Text);
                            members.Add(foot_CF_LR_S_bx.Text);
                            members.Add(foot_CF_LR_HT_cbx.Text);
                            members.Add(foot_CF_TR_D_bx.Text);
                            members.Add(foot_CF_TR_Q_bx.Text);
                            members.Add(foot_CF_TR_S_bx.Text);
                            members.Add(foot_CF_TR_HT_cbx.Text);
                            members.Add(foot_CF_UR_D_bx.Text);
                            members.Add(foot_CF_UR_Q_bx.Text);
                            members.Add(foot_CF_UR_S_bx.Text);
                            members.Add(foot_CF_UR_HT_cbx.Text);

                            costEstimationForm.structuralMembers.footingsColumn[floorCount].Add(members);
                            costEstimationForm.structuralMembers.footingColumnNames.Add(structMemName);

                            compute.AddFootingWorks(costEstimationForm, footingCount, wallFootingCount, true);
                            MessageBox.Show("eto ang sagot sa tanong: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                else //Opened from floors
                {
                    if (costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][0].Equals("Isolated Footing"))
                    {
                        if (costEstimationForm.structuralMembers.footingColumnNames.Contains(structMemName))
                        {
                            if (structMemName.Equals(oldStructMemName))
                            {
                                //Do nothing
                            }
                            else
                            {
                                int found = 0;
                                for (int i = 0; i < costEstimationForm.structuralMembers.footingColumnNames.Count; i++)
                                {
                                    if (costEstimationForm.structuralMembers.footingColumnNames[i].Equals(structMemName))
                                    {
                                        found++;
                                    }
                                }
                                if (found > 1)
                                {   //Duplicate found
                                    MessageBox.Show("Name already exists!");
                                    return;
                                }
                            }
                        }

                        try
                        {
                            costEstimationForm.structuralMembers.footingColumnNames[memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][1] = foot_IF_D_L_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][2] = foot_IF_D_W_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][3] = foot_IF_D_T_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][4] = foot_IF_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][5] = foot_IF_D_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][6] = foot_IF_LR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][7] = foot_IF_LR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][8] = foot_IF_LR_HT_cbx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][9] = foot_IF_TR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][10] = foot_IF_TR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][11] = foot_IF_TR_HT_cbx.Text;

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, true);
                            MessageBox.Show("eto ang sagot sa tanong2: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else
                    {
                        if (costEstimationForm.structuralMembers.footingColumnNames.Contains(structMemName))
                        {
                            if (structMemName.Equals(oldStructMemName))
                            {
                                //Do nothing
                            }
                            else
                            {
                                int found = 0;
                                for (int i = 0; i < costEstimationForm.structuralMembers.footingColumnNames.Count; i++)
                                {
                                    if (costEstimationForm.structuralMembers.footingColumnNames[i].Equals(structMemName))
                                    {
                                        found++;
                                    }
                                }
                                if (found > 1)
                                {   //Duplicate found
                                    MessageBox.Show("Name already exists!");
                                    return;
                                }
                            }
                        }

                        try
                        {
                            costEstimationForm.structuralMembers.footingColumnNames[memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][1] = foot_CF_D_L_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][2] = foot_CF_D_W_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][3] = foot_CF_D_T_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][4] = foot_CF_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][5] = foot_CF_D_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][6] = foot_CF_LR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][7] = foot_CF_LR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][8] = foot_CF_LR_S_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][9] = foot_CF_LR_HT_cbx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][10] = foot_CF_TR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][11] = foot_CF_TR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][12] = foot_CF_TR_S_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][13] = foot_CF_TR_HT_cbx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][14] = foot_CF_UR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][15] = foot_CF_UR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][16] = foot_CF_UR_S_bx.Text;
                            costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][17] = foot_CF_UR_HT_cbx.Text;

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, true);
                            MessageBox.Show("eto ang sagot sa tanong2: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            else if (addstruct_cbx.Text.Equals("Footing (Wall)"))
            {
                if (isNew)
                {
                    if (footW_FT_cbx.SelectedIndex == 0) //Rectangular
                    {
                        if (costEstimationForm.structuralMembers.footingWallNames.Contains(structMemName))
                        {
                            MessageBox.Show("Name already exists!");
                            return;
                        }

                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(footW_FT_cbx.Text);
                            members.Add(footW_R_D_L_bx.Text);
                            members.Add(footW_R_D_LF_bx.Text);
                            members.Add(footW_R_D_B_bx.Text);
                            members.Add(footW_R_D_T_bx.Text);
                            members.Add(footW_R_D_D_bx.Text);
                            members.Add(footW_R_D_Q_bx.Text);
                            members.Add(footW_R_LR_D_bx.Text);
                            members.Add(footW_R_LR_Q_bx.Text);
                            members.Add(footW_R_LR_S_bx.Text);
                            members.Add(footW_R_LR_HT_cbx.Text);
                            members.Add(footW_R_TR_D_bx.Text);
                            members.Add(footW_R_TR_Q_bx.Text);
                            members.Add(footW_R_TR_S_bx.Text);
                            members.Add(footW_R_TR_HT_cbx.Text);

                            costEstimationForm.structuralMembers.footingsWall[floorCount].Add(members);
                            costEstimationForm.structuralMembers.footingWallNames.Add(structMemName);

                            compute.AddFootingWorks(costEstimationForm, footingCount, wallFootingCount, false);
                            MessageBox.Show("eto ang sagot sa tanong3: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else //Trapezoidal
                    {
                        if (costEstimationForm.structuralMembers.footingWallNames.Contains(structMemName))
                        {
                            MessageBox.Show("Name already exists!");
                            return;
                        }

                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(footW_FT_cbx.Text);
                            members.Add(footW_T_D_L_bx.Text);
                            members.Add(footW_T_D_LF_bx.Text);
                            members.Add(footW_T_D_BT_bx.Text);
                            members.Add(footW_T_D_BU_bx.Text);
                            members.Add(footW_T_D_T_bx.Text);
                            members.Add(footW_T_D_D_bx.Text);
                            members.Add(footW_T_D_Q_bx.Text);
                            members.Add(footW_T_LR_D_bx.Text);
                            members.Add(footW_T_LR_Q_bx.Text);
                            members.Add(footW_T_LR_S_bx.Text);
                            members.Add(footW_T_LR_HT_cbx.Text);
                            members.Add(footW_T_TR_D_bx.Text);
                            members.Add(footW_T_TR_Q_bx.Text);
                            members.Add(footW_T_TR_S_bx.Text);
                            members.Add(footW_T_TR_HT_cbx.Text);

                            costEstimationForm.structuralMembers.footingsWall[floorCount].Add(members);
                            costEstimationForm.structuralMembers.footingWallNames.Add(structMemName);


                            compute.AddFootingWorks(costEstimationForm, footingCount, wallFootingCount, false);
                            MessageBox.Show("eto ang sagot sa tanong3: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                else //Opened from floors
                {
                    if (footW_FT_cbx.SelectedIndex == 0) //Rectangular
                    {
                        if (costEstimationForm.structuralMembers.footingWallNames.Contains(structMemName))
                        {
                            if (structMemName.Equals(oldStructMemName))
                            {
                                //Do nothing
                            }
                            else
                            {
                                int found = 0;
                                for (int i = 0; i < costEstimationForm.structuralMembers.footingWallNames.Count; i++)
                                {
                                    if (costEstimationForm.structuralMembers.footingWallNames[i].Equals(structMemName))
                                    {
                                        found++;
                                    }
                                }
                                if (found > 1)
                                {   //Duplicate found
                                    MessageBox.Show("Name already exists!");
                                    return;
                                }
                            }
                        }

                        try
                        {
                            costEstimationForm.structuralMembers.footingWallNames[memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][1] = footW_R_D_L_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][2] = footW_R_D_LF_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][3] = footW_R_D_B_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][4] = footW_R_D_T_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][5] = footW_R_D_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][6] = footW_R_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][7] = footW_R_LR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][8] = footW_R_LR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][9] = footW_R_LR_S_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][10] = footW_R_LR_HT_cbx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][11] = footW_R_TR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][12] = footW_R_TR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][13] = footW_R_TR_S_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][14] = footW_R_TR_HT_cbx.Text;

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, false);
                            MessageBox.Show("eto ang sagot sa tanong2: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else //Trapezoidal
                    {
                        if (costEstimationForm.structuralMembers.footingWallNames.Contains(structMemName))
                        {
                            if (structMemName.Equals(oldStructMemName))
                            {
                                //Do nothing
                            }
                            else
                            {
                                int found = 0;
                                for (int i = 0; i < costEstimationForm.structuralMembers.footingWallNames.Count; i++)
                                {
                                    if (costEstimationForm.structuralMembers.footingWallNames[i].Equals(structMemName))
                                    {
                                        found++;
                                    }
                                }
                                if (found > 1)
                                {   //Duplicate found
                                    MessageBox.Show("Name already exists!");
                                    return;
                                }
                            }
                        }

                        try
                        {
                            costEstimationForm.structuralMembers.footingWallNames[memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][1] = footW_T_D_L_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][2] = footW_T_D_LF_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][3] = footW_T_D_BT_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][4] = footW_T_D_BU_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][5] = footW_T_D_T_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][6] = footW_T_D_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][7] = footW_T_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][8] = footW_T_LR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][9] = footW_T_LR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][10] = footW_T_LR_S_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][11] = footW_T_LR_HT_cbx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][12] = footW_T_TR_D_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][13] = footW_T_TR_Q_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][14] = footW_T_TR_S_bx.Text;
                            costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][15] = footW_T_TR_HT_cbx.Text;

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, false);
                            MessageBox.Show("eto ang sagot sa tanong2: " + costEstimationForm.excavation_Total);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            else if (addstruct_cbx.Text.Equals("Column"))
            {
                if (isNew)
                {
                    //Name Validation
                    if (costEstimationForm.structuralMembers.stairsNames[floorCount].Contains(structMemName))
                    {
                        MessageBox.Show("Name already exists!");
                        return;
                    }

                    //Do whatever
                    try
                    {
                        if (floorCount == 0) //Ground Floor
                        {
                            List<string> members = new List<string>();
                            members.Add("Ground");
                            members.Add(col_G_D_B_bx.Text);
                            members.Add(col_G_D_D_bx.Text);
                            members.Add(col_G_D_H_bx.Text);
                            members.Add(col_G_D_Q_bx.Text);
                            members.Add(col_G_D_CB_cbx.Text);

                            members.Add(col_G_MR_D_bx.Text);
                            members.Add(col_G_MR_Q_bx.Text);
                            members.Add(col_G_SA_cbx.Text);
                            members.Add(col_G_ST_cbx.Text);
                            members.Add(col_G_JT_D_bx.Text);
                            members.Add(col_G_JT_S_bx.Text);
                            members.Add(col_G_CLT_D_bx.Text);

                            members.Add(col_G_CLT_S_Rest_bx.Text);
                            members.Add(col_G_CLT_S_Rest2_bx.Text);
                            members.Add(col_G_CLT_S_Rest3_bx.Text);

                            //Lateral Ties  
                            members.Add(col_G_LT_D_bx.Text);
                            members.Add(col_G_LT_LTC_cbx.Text);

                            List<string> ltMember = new List<string>();
                            foreach (ColumnLateralTiesUserControl lt in g_ltUC)
                            {
                                ltMember.Add(lt.qty);
                            }

                            //Spacing
                            members.Add(col_G_S_S_bx.Text);

                            List<string> sMember = new List<string>();
                            foreach (ColumnSpacingUserControl s in g_sUC)
                            {
                                sMember.Add(s.qty);
                                sMember.Add(s.spacing);
                            }

                            costEstimationForm.structuralMembers.column[floorCount].Add(members);
                            costEstimationForm.structuralMembers.columnLateralTies[floorCount].Add(ltMember);
                            costEstimationForm.structuralMembers.columnSpacing[floorCount].Add(sMember);
                            costEstimationForm.structuralMembers.columnNames[floorCount].Add(structMemName);

                            //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong: " +
                                costEstimationForm.structuralMembers.column[floorCount][columnCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        else //Upper Floors
                        {
                            List<string> members = new List<string>();
                            members.Add("Upper");
                            members.Add(col_U_D_B_bx.Text);
                            members.Add(col_U_D_D_bx.Text);
                            members.Add(col_U_D_H_bx.Text);
                            members.Add(col_U_D_Q_bx.Text);
                            members.Add(col_U_D_CB_cbx.Text);

                            members.Add(col_U_MR_D_bx.Text);
                            members.Add(col_U_MR_Q_bx.Text);
                            members.Add(col_U_SA_cbx.Text);
                            members.Add(col_U_ST_cbx.Text);
                            members.Add(col_U_JT_D_bx.Text);
                            members.Add(col_U_JT_S_bx.Text);

                            //Lateral Ties  
                            members.Add(col_U_LT_D_bx.Text);
                            members.Add(col_U_LT_LTC_cbx.Text);

                            List<string> ltMember = new List<string>();
                            foreach (ColumnLateralTiesUserControl lt in u_ltUC)
                            {
                                ltMember.Add(lt.qty);
                            }

                            //Spacing
                            members.Add(col_U_S_S_bx.Text);

                            List<string> sMember = new List<string>();
                            foreach (ColumnSpacingUserControl s in u_sUC)
                            {
                                sMember.Add(s.qty);
                                sMember.Add(s.spacing);
                            }

                            costEstimationForm.structuralMembers.column[floorCount].Add(members);
                            costEstimationForm.structuralMembers.columnLateralTies[floorCount].Add(ltMember);
                            costEstimationForm.structuralMembers.columnSpacing[floorCount].Add(sMember);
                            costEstimationForm.structuralMembers.columnNames[floorCount].Add(structMemName);

                            //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong: " +
                                costEstimationForm.structuralMembers.column[floorCount][columnCount][0]);

                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Do not leave any blank spaces!");
                        Console.WriteLine(ex.ToString());
                    }
                }
                else //Clicked from floors
                {
                    //Name Validation
                    if (costEstimationForm.structuralMembers.columnNames[floorCount].Contains(structMemName))
                    {
                        if (structMemName.Equals(oldStructMemName))
                        {
                            //Do nothing
                        }
                        else
                        {
                            int found = 0;
                            for (int i = 0; i < costEstimationForm.structuralMembers.columnNames[floorCount].Count; i++)
                            {
                                if (costEstimationForm.structuralMembers.columnNames[floorCount][i].Equals(structMemName))
                                {
                                    found++;
                                }
                            }
                            if (found > 1)
                            {   //Duplicate found
                                MessageBox.Show("Name already exists!");
                                return;
                            }
                        }
                    }

                    //Do whatever
                    try
                    {
                        if (floorCount == 0) // Ground Floor
                        {
                            costEstimationForm.structuralMembers.columnNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][1] = col_G_D_B_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][2] = col_G_D_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][3] = col_G_D_H_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][4] = col_G_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][5] = col_G_D_CB_cbx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][6] = col_G_MR_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][7] = col_G_MR_Q_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][8] = col_G_SA_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][9] = col_G_ST_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][10] = col_G_JT_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][11] = col_G_JT_S_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][12] = col_G_CLT_D_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][13] = col_G_CLT_S_Rest_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][14] = col_G_CLT_S_Rest2_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][15] = col_G_CLT_S_Rest3_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][16] = col_G_LT_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][17] = col_G_LT_LTC_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][18] = col_G_S_S_bx.Text;

                            List<string> ltMember = new List<string>();
                            foreach (ColumnLateralTiesUserControl lt in g_ltUC)
                            {
                                ltMember.Add(lt.qty);
                            }

                            List<string> sMember = new List<string>();
                            foreach (ColumnSpacingUserControl s in g_sUC)
                            {
                                sMember.Add(s.qty);
                                sMember.Add(s.spacing);
                            }

                            costEstimationForm.structuralMembers.columnLateralTies[floorCount][memberCount] = ltMember;
                            costEstimationForm.structuralMembers.columnSpacing[floorCount][memberCount] = sMember;
                            this.DialogResult = DialogResult.OK;
                        }
                        else //Upper
                        {
                            costEstimationForm.structuralMembers.columnNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][1] = col_U_D_B_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][2] = col_U_D_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][3] = col_U_D_H_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][4] = col_U_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][5] = col_U_D_CB_cbx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][6] = col_U_MR_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][7] = col_U_MR_Q_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][8] = col_U_SA_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][9] = col_U_ST_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][10] = col_U_JT_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][11] = col_U_JT_S_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][12] = col_U_LT_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][13] = col_U_LT_LTC_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][14] = col_U_S_S_bx.Text;

                            List<string> ltMember = new List<string>();
                            foreach (ColumnLateralTiesUserControl lt in u_ltUC)
                            {
                                ltMember.Add(lt.qty);
                            }
                            List<string> sMember = new List<string>();
                            foreach (ColumnSpacingUserControl s in u_sUC)
                            {
                                sMember.Add(s.qty);
                                sMember.Add(s.spacing);
                            }
                            costEstimationForm.structuralMembers.columnLateralTies[floorCount][memberCount] = ltMember;
                            costEstimationForm.structuralMembers.columnSpacing[floorCount][memberCount] = sMember;
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Do not leave any blank spaces!");
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else if (addstruct_cbx.Text.Equals("Beam"))
            {
                if (isNew)
                {

                }
                else //Clicked from floors
                {

                }
            }
            else if (addstruct_cbx.Text.Equals("Slab"))
            {
                if (isNew)
                {

                }
                else //Clicked from floors
                {

                }
            }
            else if (addstruct_cbx.Text.Equals("Stairs"))
            {
                if (isNew)
                {
                    //Name Validation
                    if (costEstimationForm.structuralMembers.stairsNames[floorCount].Contains(structMemName))
                    {
                        MessageBox.Show("Name already exists!");
                        return;
                    }

                    if (stairs_ST_cbx.SelectedIndex == 0) //Straight Stairs
                    {
                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(stairs_ST_cbx.Text);
                            members.Add(stairs_SS_D_Q_bx.Text);
                            members.Add(stairs_SS_D_S_bx.Text);
                            members.Add(stairs_SS_D_SL_bx.Text);
                            members.Add(stairs_SS_D_RH_bx.Text);
                            members.Add(stairs_SS_D_TW_bx.Text);
                            members.Add(stairs_SS_D_WST_bx.Text);

                            members.Add(stairs_SS_WS_MB_cbx.Text);
                            members.Add(stairs_SS_WS_MBS_bx.Text);
                            members.Add(stairs_SS_WS_DB_cbx.Text);
                            members.Add(stairs_SS_WS_DBS_bx.Text);
                            members.Add(stairs_SS_S_MB_cbx.Text);
                            members.Add(stairs_SS_S_MBS_bx.Text);
                            members.Add(stairs_SS_S_NB_cbx.Text);

                            costEstimationForm.structuralMembers.stairs[floorCount].Add(members);
                            costEstimationForm.structuralMembers.stairsNames[floorCount].Add(structMemName);

                            //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong: " +
                                costEstimationForm.structuralMembers.stairs[floorCount][stairsCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else if (stairs_ST_cbx.SelectedIndex == 1) //U-Stairs
                    {
                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(stairs_ST_cbx.Text);
                            members.Add(stairs_US_D_Q_bx.Text);
                            members.Add(stairs_US_D_SFF_bx.Text);
                            members.Add(stairs_US_D_SSF_bx.Text);
                            members.Add(stairs_US_D_SL_bx.Text);
                            members.Add(stairs_US_D_RH_bx.Text);
                            members.Add(stairs_US_D_TW_bx.Text);
                            members.Add(stairs_US_D_WST_bx.Text);
                            members.Add(stairs_US_D_LW_bx.Text);
                            members.Add(stairs_US_D_G_bx.Text);
                            members.Add(stairs_US_D_LT_bx.Text);

                            members.Add(stairs_US_WS_MB_cbx.Text);
                            members.Add(stairs_US_WS_MBS_bx.Text);
                            members.Add(stairs_US_WS_DB_cbx.Text);
                            members.Add(stairs_US_WS_DBS_bx.Text);
                            members.Add(stairs_US_L_MB_cbx.Text);
                            members.Add(stairs_US_L_MBS_bx.Text);
                            members.Add(stairs_US_S_MB_cbx.Text);
                            members.Add(stairs_US_S_MBS_bx.Text);
                            members.Add(stairs_US_S_NB_cbx.Text);

                            costEstimationForm.structuralMembers.stairs[floorCount].Add(members);
                            costEstimationForm.structuralMembers.stairsNames[floorCount].Add(structMemName);

                            //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong: " +
                                costEstimationForm.structuralMembers.stairs[floorCount][stairsCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else //L-Stairs
                    {
                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(stairs_ST_cbx.Text);
                            members.Add(stairs_LS_D_Q_bx.Text);
                            members.Add(stairs_LS_D_SFF_bx.Text);
                            members.Add(stairs_LS_D_SSF_bx.Text);
                            members.Add(stairs_LS_D_SL_bx.Text);
                            members.Add(stairs_LS_D_RH_bx.Text);
                            members.Add(stairs_LS_D_TW_bx.Text);
                            members.Add(stairs_LS_D_WST_bx.Text);
                            members.Add(stairs_LS_D_LST_bx.Text);

                            members.Add(stairs_LS_WS_MB_cbx.Text);
                            members.Add(stairs_LS_WS_MBS_bx.Text);
                            members.Add(stairs_LS_WS_DB_cbx.Text);
                            members.Add(stairs_LS_WS_DBS_bx.Text);
                            members.Add(stairs_LS_L_MB_cbx.Text);
                            members.Add(stairs_LS_L_MBS_bx.Text);
                            members.Add(stairs_LS_S_MB_cbx.Text);
                            members.Add(stairs_LS_S_MBS_bx.Text);
                            members.Add(stairs_LS_S_NB_cbx.Text);

                            costEstimationForm.structuralMembers.stairs[floorCount].Add(members);
                            costEstimationForm.structuralMembers.stairsNames[floorCount].Add(structMemName);

                            //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong: " +
                                costEstimationForm.structuralMembers.stairs[floorCount][stairsCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                else //Update
                {
                    //Name Validation
                    if (costEstimationForm.structuralMembers.stairsNames[floorCount].Contains(structMemName))
                    {
                        if (structMemName.Equals(oldStructMemName))
                        {
                            //Do nothing
                        }
                        else
                        {
                            int found = 0;
                            for (int i = 0; i < costEstimationForm.structuralMembers.stairsNames[floorCount].Count; i++)
                            {
                                if (costEstimationForm.structuralMembers.stairsNames[floorCount][i].Equals(structMemName))
                                {
                                    found++;
                                }
                            }
                            if (found > 1)
                            {   //Duplicate found
                                MessageBox.Show("Name already exists!");
                                return;
                            }
                        }
                    }

                    if (stairs_ST_cbx.SelectedIndex == 0) //Straight Stairs
                    {
                        try
                        {
                            costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][1] = stairs_SS_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][2] = stairs_SS_D_S_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][3] = stairs_SS_D_SL_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][4] = stairs_SS_D_RH_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][5] = stairs_SS_D_TW_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][6] = stairs_SS_D_WST_bx.Text;

                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][7] = stairs_SS_WS_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][8] = stairs_SS_WS_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][9] = stairs_SS_WS_DB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][10] = stairs_SS_WS_DBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][11] = stairs_SS_S_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][12] = stairs_SS_S_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][13] = stairs_SS_S_NB_cbx.Text;

                            //compute.ModifyStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong2: " +
                                costEstimationForm.structuralMembers.stairs[floorCount][memberCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else if (stairs_ST_cbx.SelectedIndex == 1) //U-Stairs
                    {
                        try
                        {
                            costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][1] = stairs_US_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][2] = stairs_US_D_SFF_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][3] = stairs_US_D_SSF_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][4] = stairs_US_D_SL_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][5] = stairs_US_D_RH_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][6] = stairs_US_D_TW_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][7] = stairs_US_D_WST_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][8] = stairs_US_D_LW_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][9] = stairs_US_D_G_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][10] = stairs_US_D_LT_bx.Text;

                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][11] = stairs_US_WS_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][12] = stairs_US_WS_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][13] = stairs_US_WS_DB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][14] = stairs_US_WS_DBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][15] = stairs_US_L_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][16] = stairs_US_L_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][17] = stairs_US_S_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][18] = stairs_US_S_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][19] = stairs_US_S_NB_cbx.Text;

                            //compute.ModifyStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong2: " +
                                costEstimationForm.structuralMembers.stairs[floorCount][memberCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else //L-Stairs
                    {
                        try
                        {
                            costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][1] = stairs_LS_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][2] = stairs_LS_D_SFF_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][3] = stairs_LS_D_SSF_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][4] = stairs_LS_D_SL_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][5] = stairs_LS_D_RH_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][6] = stairs_LS_D_TW_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][7] = stairs_LS_D_WST_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][8] = stairs_LS_D_LST_bx.Text;

                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][9] = stairs_LS_WS_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][10] = stairs_LS_WS_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][11] = stairs_LS_WS_DB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][12] = stairs_LS_WS_DBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][13] = stairs_LS_L_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][14] = stairs_LS_L_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][15] = stairs_LS_S_MB_cbx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][16] = stairs_LS_S_MBS_bx.Text;
                            costEstimationForm.structuralMembers.stairs[floorCount][memberCount][17] = stairs_LS_S_NB_cbx.Text;

                            //compute.ModifyStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong2: " +
                                costEstimationForm.structuralMembers.stairs[floorCount][memberCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            else //Roofing
            {
                if (isNew)
                {
                    //Name Validation
                    if (costEstimationForm.structuralMembers.roofNames[floorCount].Contains(structMemName))
                    {
                        MessageBox.Show("Name already exists!");
                        return;
                    }

                    //TODO Do Whatever
                    if (roof_PGR_cbx.SelectedIndex == 0) //Rafters and Purlins
                    {
                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(stairs_ST_cbx.Text);
                            members.Add(stairs_SS_D_Q_bx.Text);
                            members.Add(stairs_SS_D_S_bx.Text);
                            members.Add(stairs_SS_D_SL_bx.Text);
                            members.Add(stairs_SS_D_RH_bx.Text);
                            members.Add(stairs_SS_D_TW_bx.Text);
                            members.Add(stairs_SS_D_WST_bx.Text);

                            members.Add(stairs_SS_WS_MB_cbx.Text);
                            members.Add(stairs_SS_WS_MBS_bx.Text);
                            members.Add(stairs_SS_WS_DB_cbx.Text);
                            members.Add(stairs_SS_WS_DBS_bx.Text);
                            members.Add(stairs_SS_S_MB_cbx.Text);
                            members.Add(stairs_SS_S_MBS_bx.Text);
                            members.Add(stairs_SS_S_NB_cbx.Text);

                            costEstimationForm.structuralMembers.stairs[floorCount].Add(members);
                            costEstimationForm.structuralMembers.stairsNames[floorCount].Add(structMemName);

                            //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                            MessageBox.Show("eto ang sagot sa tanong: " +
                                costEstimationForm.structuralMembers.stairs[floorCount][stairsCount][0]);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else if (roof_PGR_cbx.SelectedIndex == 1) //G.I Roof and Its Accessories
                    {

                    }
                    else //Roof Accessories (Tinswork)
                    {

                    }
                }
                else //Update
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
            if (addstruct_cbx.Text.Equals("Footing (Column)"))
            {
                addstructTabControl.SelectedIndex = 0;
            }
            else if(addstruct_cbx.Text.Equals("Footing (Wall)"))
            {
                addstructTabControl.SelectedIndex = 1;
            }
            else if (addstruct_cbx.Text.Equals("Column"))
            {
                addstructTabControl.SelectedIndex = 2;
            }
            else if (addstruct_cbx.Text.Equals("Beam"))
            {
                addstructTabControl.SelectedIndex = 3;
            }
            else if (addstruct_cbx.Text.Equals("Slab"))
            {
                addstructTabControl.SelectedIndex = 4;
            }
            else if (addstruct_cbx.Text.Equals("Stairs"))
            {
                addstructTabControl.SelectedIndex = 5;
            }
            else if (addstruct_cbx.Text.Equals("Roofing (Gable)"))
            {
                addstructTabControl.SelectedIndex = 6;
            }
            setDefaultStructMemName();
        }

        //TODO add other structural members
        private void setDefaultStructMemName()
        {
            if(floorCount == 0) //Ground Floor
            {
                if (addstruct_cbx.Text.Equals("Footing (Column)"))
                {
                    addstruct_Name_bx.Text = "F-" + (footingCount + 1);
                }
                else if (addstruct_cbx.Text.Equals("Footing (Wall)"))
                {
                    addstruct_Name_bx.Text = "WF-" + (wallFootingCount + 1);
                }
                else if (addstruct_cbx.Text.Equals("Column"))
                {
                    addstruct_Name_bx.Text = "C-" + (nodes[1].Nodes.Count + 1);
                }
                else if (addstruct_cbx.Text.Equals("Beam"))
                {
                    addstruct_Name_bx.Text = "B-" + (nodes[2].Nodes.Count + 1);
                }
                else if (addstruct_cbx.Text.Equals("Slab"))
                {
                    addstruct_Name_bx.Text = "S-" + (nodes[3].Nodes.Count + 1) + " (A)";
                }
                else if (addstruct_cbx.Text.Equals("Stairs"))
                {
                    addstruct_Name_bx.Text = "ST-" + (nodes[4].Nodes.Count + 1);
                }
                else if (addstruct_cbx.Text.Equals("Roofing (Gable)"))
                {
                    addstruct_Name_bx.Text = "R-" + (nodes[5].Nodes.Count + 1);
                }
            }
            else //Upper Floors
            {
                if (addstruct_cbx.Text.Equals("Column"))
                {
                    addstruct_Name_bx.Text = "C-" + (nodes[0].Nodes.Count + 1);
                }
                else if (addstruct_cbx.Text.Equals("Beam"))
                {
                    addstruct_Name_bx.Text = "B-" + (nodes[1].Nodes.Count + 1);
                }
                else if (addstruct_cbx.Text.Equals("Slab"))
                {
                    addstruct_Name_bx.Text = "S-" + (nodes[2].Nodes.Count + 1) + " (A)";
                }
                else if (addstruct_cbx.Text.Equals("Stairs"))
                {
                    addstruct_Name_bx.Text = "ST-" + (nodes[3].Nodes.Count + 1);
                }
                else if (addstruct_cbx.Text.Equals("Roofing (Gable)"))
                {
                    addstruct_Name_bx.Text = "R-" + (nodes[4].Nodes.Count + 1);
                }
            }
        }

        //TODO add other structural members from opened node
        private void setFootingValues()
        {
            if (isFooting)
            {  
                if (costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][0].Equals("Isolated Footing"))
                {
                    foot_FT_cbx.SelectedIndex = 0;
                    oldStructMemName = costEstimationForm.structuralMembers.footingColumnNames[memberCount];
                    addstruct_Name_bx.Text = costEstimationForm.structuralMembers.footingColumnNames[memberCount];

                    foot_IF_D_L_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][1];
                    foot_IF_D_W_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][2];
                    foot_IF_D_T_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][3];
                    foot_IF_D_Q_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][4];
                    foot_IF_D_D_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][5];
                    foot_IF_LR_D_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][6];
                    foot_IF_LR_Q_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][7];
                    foot_IF_LR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][8];
                    foot_IF_TR_D_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][9];
                    foot_IF_TR_Q_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][10];
                    foot_IF_TR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][11];
                }
                else
                {
                    foot_FT_cbx.SelectedIndex = 1;
                    oldStructMemName = costEstimationForm.structuralMembers.footingColumnNames[memberCount];
                    addstruct_Name_bx.Text = costEstimationForm.structuralMembers.footingColumnNames[memberCount];

                    foot_CF_D_L_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][1];
                    foot_CF_D_W_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][2];
                    foot_CF_D_T_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][3];
                    foot_CF_D_Q_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][4];
                    foot_CF_D_D_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][5];
                    foot_CF_LR_D_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][6];
                    foot_CF_LR_Q_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][7];
                    foot_CF_LR_S_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][8];
                    foot_CF_LR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][9];
                    foot_CF_TR_D_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][10];
                    foot_CF_TR_Q_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][11];
                    foot_CF_TR_S_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][12];
                    foot_CF_TR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][13];
                    foot_CF_UR_D_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][14];
                    foot_CF_UR_Q_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][15];
                    foot_CF_UR_S_bx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][16];
                    foot_CF_UR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsColumn[floorCount][memberCount][17];
                }
            }
            else //Wall Footing
            {
                addstructTabControl.SelectedIndex = 1;
                addstruct_cbx.SelectedIndex = 1;
                if (costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][0].Equals("Rectangular"))
                {
                    footW_FT_cbx.SelectedIndex = 0;
                    oldStructMemName = costEstimationForm.structuralMembers.footingWallNames[memberCount];
                    addstruct_Name_bx.Text = costEstimationForm.structuralMembers.footingWallNames[memberCount];

                    footW_R_D_L_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][1];
                    footW_R_D_LF_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][2];
                    footW_R_D_B_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][3];
                    footW_R_D_T_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][4];
                    footW_R_D_D_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][5];
                    footW_R_D_Q_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][6];
                    footW_R_LR_D_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][7];
                    footW_R_LR_Q_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][8];
                    footW_R_LR_S_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][9];
                    footW_R_LR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][10];
                    footW_R_TR_D_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][11];
                    footW_R_TR_Q_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][12];
                    footW_R_TR_S_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][13];
                    footW_R_TR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][14];
                }
                else
                {
                    footW_FT_cbx.SelectedIndex = 1;
                    oldStructMemName = costEstimationForm.structuralMembers.footingWallNames[memberCount];
                    addstruct_Name_bx.Text = costEstimationForm.structuralMembers.footingWallNames[memberCount];

                    footW_T_D_L_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][1];
                    footW_T_D_LF_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][2];
                    footW_T_D_BT_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][3];
                    footW_T_D_BU_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][4];
                    footW_T_D_T_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][5];
                    footW_T_D_D_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][6];
                    footW_T_D_Q_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][7];
                    footW_T_LR_D_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][8];
                    footW_T_LR_Q_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][9];
                    footW_T_LR_S_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][10];
                    footW_T_LR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][11];
                    footW_T_TR_D_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][12];
                    footW_T_TR_Q_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][13];
                    footW_T_TR_S_bx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][14];
                    footW_T_TR_HT_cbx.Text = costEstimationForm.structuralMembers.footingsWall[floorCount][memberCount][15];
                }
            }
        }
        private void setColumnValues()
        {
            if (floorCount == 0)
            {
                addstruct_cbx.SelectedIndex = 2;
            }
            else
            {
                addstruct_cbx.SelectedIndex = 0;
            }
            if (costEstimationForm.structuralMembers.column[floorCount][memberCount][0].Equals("Ground"))
            {
                colTabControl.SelectedIndex = 0;
                oldStructMemName = costEstimationForm.structuralMembers.columnNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.columnNames[floorCount][memberCount];

                col_G_D_B_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][1];
                col_G_D_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][2];
                col_G_D_H_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][3];
                col_G_D_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][4];
                col_G_D_CB_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][5];

                col_G_MR_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][6];
                col_G_MR_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][7];
                col_G_SA_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][8];
                col_G_ST_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][9];
                col_G_JT_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][10];
                col_G_JT_S_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][11];
                col_G_CLT_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][12];

                col_G_CLT_S_Rest_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][13];
                col_G_CLT_S_Rest2_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][14];
                col_G_CLT_S_Rest3_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][15];

                col_G_LT_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][16];
                col_G_LT_LTC_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][17];
                col_G_S_S_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][18];

                int i = 0;
                foreach(string value in costEstimationForm.structuralMembers.columnLateralTies[floorCount][memberCount])
                {
                    g_ltUC[i].qty = value;
                    i++;
                }
                i = 0;
                int j = 0;
                foreach (string value in costEstimationForm.structuralMembers.columnSpacing[floorCount][memberCount])
                {
                    if (j == 0)
                    {
                        ColumnSpacingUserControl content2 = new ColumnSpacingUserControl();
                        g_sUC.Add(content2);
                        col_G_S_Panel.Controls.Add(content2);
                    }
                    if (j % 2 == 0)
                    {
                        g_sUC[i].qty = value;
                        j++;
                    }
                    else
                    {
                        g_sUC[i].spacing = value;
                        j++;
                    }
                    if (j == 2)
                    {
                        i++;
                        j = 0;
                    }
                }
            }
            else //Upper
            {
                colTabControl.SelectedIndex = 1;
                oldStructMemName = costEstimationForm.structuralMembers.columnNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.columnNames[floorCount][memberCount];
                
                col_U_D_B_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][1];
                col_U_D_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][2];
                col_U_D_H_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][3];
                col_U_D_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][4];
                col_U_D_CB_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][5];

                col_U_MR_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][6];
                col_U_MR_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][7];
                col_U_SA_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][8];
                col_U_ST_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][9];
                col_U_JT_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][10];
                col_U_JT_S_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][11];

                col_U_LT_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][12];
                col_U_LT_LTC_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][13];
                col_U_S_S_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][14];

                int i = 0;
                foreach (string value in costEstimationForm.structuralMembers.columnLateralTies[floorCount][memberCount])
                {
                    u_ltUC[i].qty = value;
                    i++;
                }
                i = 0;
                int j = 0;
                foreach (string value in costEstimationForm.structuralMembers.columnSpacing[floorCount][memberCount])
                {
                    if (j == 0)
                    {
                        ColumnSpacingUserControl content2 = new ColumnSpacingUserControl();
                        u_sUC.Add(content2);
                        col_U_S_Panel.Controls.Add(content2);
                    }
                    if (j % 2 == 0)
                    {
                        u_sUC[i].qty = value;
                        j++;
                    }
                    else
                    {
                        u_sUC[i].spacing = value;
                        j++;
                    }
                    if (j == 2)
                    {
                        i++;
                        j = 0;
                    }
                }
            }
        }

        public void populateColumnConnectionBelow()
        {
            if (floorCount == 0)
            {
                col_G_D_CB_cbx.Items.Clear();
                foreach (string name in costEstimationForm.structuralMembers.footingColumnNames)
                {
                    col_G_D_CB_cbx.Items.Add(name);
                }
                if (col_G_D_CB_cbx.SelectedItem == null || col_G_D_CB_cbx.SelectedIndex == -1)
                    col_G_D_CB_cbx.Items.Add("None");
                col_G_D_CB_cbx.SelectedIndex = 0;
            }
            else
            {
                col_U_D_CB_cbx.Items.Clear();
                foreach (string name in costEstimationForm.structuralMembers.columnNames[floorCount - 1])
                {
                    col_U_D_CB_cbx.Items.Add(name);
                }
                if (col_U_D_CB_cbx.SelectedItem == null || col_U_D_CB_cbx.SelectedIndex == -1)
                    col_U_D_CB_cbx.Items.Add("None");
                col_U_D_CB_cbx.SelectedIndex = 0;   
            }
        }

        private void setStairsValues()
        {
            if(floorCount == 0)
            {
                addstruct_cbx.SelectedIndex = 5;
            }
            else
            {
                addstruct_cbx.SelectedIndex = 3;
            }
            if (costEstimationForm.structuralMembers.stairs[floorCount][memberCount][0].Equals("Straight Stairs"))
            {
                stairs_ST_cbx.SelectedIndex = 0;
                oldStructMemName = costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount];

                stairs_SS_D_Q_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][1];
                stairs_SS_D_S_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][2];
                stairs_SS_D_SL_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][3];
                stairs_SS_D_RH_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][4];
                stairs_SS_D_TW_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][5];
                stairs_SS_D_WST_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][6];

                stairs_SS_WS_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][7];
                stairs_SS_WS_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][8];
                stairs_SS_WS_DB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][9];
                stairs_SS_WS_DBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][10];
                stairs_SS_S_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][11];
                stairs_SS_S_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][12];
                stairs_SS_S_NB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][13];
            }
            else if(costEstimationForm.structuralMembers.stairs[floorCount][memberCount][0].Equals("L-Stairs"))
            {
                stairs_ST_cbx.SelectedIndex = 1;
                oldStructMemName = costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount];

                stairs_US_D_Q_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][1];
                stairs_US_D_SFF_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][2];
                stairs_US_D_SSF_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][3];
                stairs_US_D_SL_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][4];
                stairs_US_D_RH_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][5];
                stairs_US_D_TW_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][6];
                stairs_US_D_WST_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][7];
                stairs_US_D_LW_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][8];
                stairs_US_D_G_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][9];
                stairs_US_D_LT_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][10];

                stairs_US_WS_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][11];
                stairs_US_WS_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][12];
                stairs_US_WS_DB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][13];
                stairs_US_WS_DBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][14];
                stairs_US_L_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][15];
                stairs_US_L_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][16];
                stairs_US_S_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][17];
                stairs_US_S_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][18];
                stairs_US_S_NB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][19];
            }
            else
            {
                stairs_ST_cbx.SelectedIndex = 2;
                oldStructMemName = costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.stairsNames[floorCount][memberCount];

                stairs_LS_D_Q_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][1];
                stairs_LS_D_SFF_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][2];
                stairs_LS_D_SSF_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][3];
                stairs_LS_D_SL_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][4];
                stairs_LS_D_RH_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][5];
                stairs_LS_D_TW_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][6];
                stairs_LS_D_WST_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][7];
                stairs_LS_D_LST_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][8];

                stairs_LS_WS_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][9];
                stairs_LS_WS_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][10];
                stairs_LS_WS_DB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][11];
                stairs_LS_WS_DBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][12];
                stairs_LS_L_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][13];
                stairs_LS_L_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][14];
                stairs_LS_S_MB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][15];
                stairs_LS_S_MBS_bx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][16];
                stairs_LS_S_NB_cbx.Text = costEstimationForm.structuralMembers.stairs[floorCount][memberCount][17];
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label174_Click(object sender, EventArgs e)
        {

        }

        private void col_G_LT_LTC_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (col_G_LT_LTC_cbx.SelectedIndex == 0)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_1); 
                Image picture = (Image)bmp;
                col_G_LT_pb.Image = picture;

                g_ltUC.Clear();
                col_G_LT_Panel.Controls.Clear();
            }
            else if (col_G_LT_LTC_cbx.SelectedIndex == 1)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_2); 
                Image picture = (Image)bmp;
                col_G_LT_pb.Image = picture;

                g_ltUC.Clear();
                col_G_LT_Panel.Controls.Clear();
            }
            else if (col_G_LT_LTC_cbx.SelectedIndex == 2)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_3); 
                Image picture = (Image)bmp;
                col_G_LT_pb.Image = picture;

                //Add Lateral Ties Quantity
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5); 
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture5 = (Image)bmp5;

                g_ltUC.Clear();
                col_G_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                g_ltUC.Add(ties);
                col_G_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                g_ltUC.Add(ties2);
                col_G_LT_Panel.Controls.Add(ties2);

                ColumnLateralTiesUserControl ties3 = new ColumnLateralTiesUserControl(picture4);
                g_ltUC.Add(ties3);
                col_G_LT_Panel.Controls.Add(ties3);

                ColumnLateralTiesUserControl ties4 = new ColumnLateralTiesUserControl(picture5);
                g_ltUC.Add(ties4);
                col_G_LT_Panel.Controls.Add(ties4);
            }
            else if (col_G_LT_LTC_cbx.SelectedIndex == 3)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_4); 
                Image picture = (Image)bmp;
                col_G_LT_pb.Image = picture;

                //Add Lateral Ties Quantity
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture5 = (Image)bmp5;

                g_ltUC.Clear();
                col_G_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                g_ltUC.Add(ties);
                col_G_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                g_ltUC.Add(ties2);
                col_G_LT_Panel.Controls.Add(ties2);

                ColumnLateralTiesUserControl ties3 = new ColumnLateralTiesUserControl(picture4);
                g_ltUC.Add(ties3);
                col_G_LT_Panel.Controls.Add(ties3);

                ColumnLateralTiesUserControl ties4 = new ColumnLateralTiesUserControl(picture5);
                g_ltUC.Add(ties4);
                col_G_LT_Panel.Controls.Add(ties4);
            }
            else if (col_G_LT_LTC_cbx.SelectedIndex == 4)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5); 
                Image picture = (Image)bmp;
                col_G_LT_pb.Image = picture;

                //Add Lateral Ties Quantity
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;

                g_ltUC.Clear();
                col_G_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                g_ltUC.Add(ties);
                col_G_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                g_ltUC.Add(ties2);
                col_G_LT_Panel.Controls.Add(ties2);
            }
            else
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6); 
                Image picture = (Image)bmp;
                col_G_LT_pb.Image = picture;

                //Add Lateral Ties Quantity 
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;

                g_ltUC.Clear();
                col_G_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                g_ltUC.Add(ties);
                col_G_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                g_ltUC.Add(ties2);
                col_G_LT_Panel.Controls.Add(ties2);
            }
        }

        private void col_G_LT_Add_btn_Click(object sender, EventArgs e)
        {
            ColumnSpacingUserControl content = new ColumnSpacingUserControl();
            g_sUC.Add(content);
            col_G_S_Panel.Controls.Add(content);
        }

        private void col_U_LT_Add_btn_Click(object sender, EventArgs e)
        {
            ColumnSpacingUserControl content = new ColumnSpacingUserControl();
            u_sUC.Add(content);
            col_U_S_Panel.Controls.Add(content);
        }

        private void roof_RP_W_rb_CheckedChanged(object sender, EventArgs e)
        {
            if (roof_RP_W_rb.Checked)
            {
                roof_RP_D_TabControl.SelectedIndex = 0;
            }
            else if (roof_RP_ST_rb.Checked)
            {
                roof_RP_D_TabControl.SelectedIndex = 1;
            }
            else if (roof_RP_SCP_rb.Checked)
            {
                roof_RP_D_TabControl.SelectedIndex = 2;
            }
        }

        private void roof_PGR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            roofTabControl.SelectedIndex = roof_PGR_cbx.SelectedIndex;
        }

        private void roof_GI_M_PGIS_rb_CheckedChanged(object sender, EventArgs e)
        {
            roof_GI_M_SP_cbx.Enabled = true;
        }

        private void roof_GI_M_CGIS_rb_CheckedChanged(object sender, EventArgs e)
        {
            roof_GI_M_SP_cbx.Enabled = false;
        }

        private void roof_RP_W_D_SR_bx_TextChanged(object sender, EventArgs e)
        {

        }

        private void col_U_LT_LTC_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (col_U_LT_LTC_cbx.SelectedIndex == 0)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_1);
                Image picture = (Image)bmp;
                col_U_LT_pb.Image = picture;

                u_ltUC.Clear();
                col_U_LT_Panel.Controls.Clear();
            }
            else if (col_U_LT_LTC_cbx.SelectedIndex == 1)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_2);
                Image picture = (Image)bmp;
                col_U_LT_pb.Image = picture;

                u_ltUC.Clear();
                col_U_LT_Panel.Controls.Clear();
            }
            else if (col_U_LT_LTC_cbx.SelectedIndex == 2)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_3);
                Image picture = (Image)bmp;
                col_U_LT_pb.Image = picture;

                //Add Lateral Ties Quantity
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture5 = (Image)bmp5;

                u_ltUC.Clear();
                col_U_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                u_ltUC.Add(ties);
                col_U_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                u_ltUC.Add(ties2);
                col_U_LT_Panel.Controls.Add(ties2);

                ColumnLateralTiesUserControl ties3 = new ColumnLateralTiesUserControl(picture4);
                u_ltUC.Add(ties3);
                col_U_LT_Panel.Controls.Add(ties3);

                ColumnLateralTiesUserControl ties4 = new ColumnLateralTiesUserControl(picture5);
                u_ltUC.Add(ties4);
                col_U_LT_Panel.Controls.Add(ties4);
            }
            else if (col_U_LT_LTC_cbx.SelectedIndex == 3)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_4);
                Image picture = (Image)bmp;
                col_U_LT_pb.Image = picture;

                //Add Lateral Ties Quantity
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture5 = (Image)bmp5;

                u_ltUC.Clear();
                col_U_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                u_ltUC.Add(ties);
                col_U_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                u_ltUC.Add(ties2);
                col_U_LT_Panel.Controls.Add(ties2);

                ColumnLateralTiesUserControl ties3 = new ColumnLateralTiesUserControl(picture4);
                u_ltUC.Add(ties3);
                col_U_LT_Panel.Controls.Add(ties3);

                ColumnLateralTiesUserControl ties4 = new ColumnLateralTiesUserControl(picture5);
                u_ltUC.Add(ties4);
                col_U_LT_Panel.Controls.Add(ties4);
            }
            else if (col_U_LT_LTC_cbx.SelectedIndex == 4)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture = (Image)bmp;
                col_U_LT_pb.Image = picture;

                //Add Lateral Ties Quantity
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;

                u_ltUC.Clear();
                col_U_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                u_ltUC.Add(ties);
                col_U_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                u_ltUC.Add(ties2);
                col_U_LT_Panel.Controls.Add(ties2);
            }
            else
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture = (Image)bmp;
                col_U_LT_pb.Image = picture;

                //Add Lateral Ties Quantity 
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_5);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.LT_CONFIG_6);
                Image picture3 = (Image)bmp3;

                u_ltUC.Clear();
                col_U_LT_Panel.Controls.Clear();

                ColumnLateralTiesUserControl ties = new ColumnLateralTiesUserControl(picture2);
                u_ltUC.Add(ties);
                col_U_LT_Panel.Controls.Add(ties);

                ColumnLateralTiesUserControl ties2 = new ColumnLateralTiesUserControl(picture3);
                u_ltUC.Add(ties2);
                col_U_LT_Panel.Controls.Add(ties2);
            }
        }

        private void AddStructForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void foot_FT_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            footingTabControl.SelectedIndex = foot_FT_cbx.SelectedIndex;
        }

        private void footW_FT_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            footingWTabControl.SelectedIndex = footW_FT_cbx.SelectedIndex;
        }

        private void stairs_ST_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            stairsTabControl.SelectedIndex = stairs_ST_cbx.SelectedIndex;
        }
    }
}
