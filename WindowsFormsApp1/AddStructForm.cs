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
    //TODO: populate combobox ng beam row according kung anong existing + new schedules 

    public partial class AddStructForm : Form
    {
        //Local Variables
        Compute compute = new Compute();
        private List<ColumnLateralTiesUserControl> g_ltUC;
        private List<ColumnLateralTiesUserControl> u_ltUC;
        private List<ColumnSpacingUserControl> g_sUC;
        private List<ColumnSpacingUserControl> u_sUC;
        public List<BeamRowUserControl> br_UC;
        public List<BeamScheduleUserControl> bs_UC;
        public List<SlabScheduleUserControl> ss_UC;
        private SlabDetail2UserControl sd1_UC;
        private SlabDetail1UserControl sd2_UC;
        private List<RoofHRSUserControl> rHRS_UC;

        //Passed Variables
        public string structMemName;
        public string oldStructMemName;
        private CostEstimationForm costEstimationForm;
        private List<TreeNode> nodes;
        private int floorCount;
        private int memberCount, footingCount, wallFootingCount, columnCount, beamCount, slabCount, stairsCount, roofCount;
        public bool isNew, isFooting;

        public AddStructForm(CostEstimationForm costEstimationForm, int floorCount, int footingCount, int wallFootingCount, int columnCount, int beamCount, int slabCount, int stairsCount, int roofCount, List<TreeNode> nodes, bool isNew, int index, string parentNode, bool isFooting)
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
            this.beamCount = beamCount;
            this.slabCount = slabCount;
            this.stairsCount = stairsCount;
            this.roofCount = roofCount;
            

            oldStructMemName = "";
            g_ltUC = new List<ColumnLateralTiesUserControl>();
            g_sUC = new List<ColumnSpacingUserControl>();
            u_ltUC = new List<ColumnLateralTiesUserControl>();
            u_sUC = new List<ColumnSpacingUserControl>();
            br_UC = new List<BeamRowUserControl>();
            bs_UC = new List<BeamScheduleUserControl>();
            ss_UC = new List<SlabScheduleUserControl>();
            sd1_UC = new SlabDetail2UserControl();
            sd2_UC = new  SlabDetail1UserControl();
            rHRS_UC = new List<RoofHRSUserControl>();

            //Init components
            addstruct_cbx.SelectedIndex = 0;
            colTabControl.SelectedIndex = 0;
            beam_BT_cbx.SelectedIndex = 0;
            slabTabControl.SelectedIndex = 0;
            if (floorCount != 0)
            {
                addstruct_cbx.Items.Clear();
                addstruct_cbx.Items.Add("Column");
                addstruct_cbx.Items.Add("Beam");
                addstruct_cbx.Items.Add("Slab");
                addstruct_cbx.Items.Add("Stairs");
                addstruct_cbx.Items.Add("Roofing (Gable)");
                beam_BT_cbx.Items.Clear();
                beam_BT_cbx.Items.Add("Suspended Beam");
                beam_BT_cbx.Items.Add("Roof Beam");

                addstruct_cbx.SelectedIndex = 0;
                colTabControl.SelectedIndex = 1;
                beam_BT_cbx.SelectedIndex = 0;
                slabTabControl.SelectedIndex = 1;
            }
            //Init Footings Combo Boxes
            foot_FT_cbx.SelectedIndex = foot_IF_LR_HT_cbx.SelectedIndex = foot_IF_TR_HT_cbx.SelectedIndex = 
                foot_CF_LR_HT_cbx.SelectedIndex = foot_CF_TR_HT_cbx.SelectedIndex = foot_CF_UR_HT_cbx.SelectedIndex = 
                footW_FT_cbx.SelectedIndex = footW_R_LR_HT_cbx.SelectedIndex = footW_R_TR_HT_cbx.SelectedIndex = 
                footW_T_LR_HT_cbx.SelectedIndex = footW_T_TR_HT_cbx.SelectedIndex = 0;

            //Init Column Combo Boxes
            col_G_D_CB_cbx.SelectedIndex = col_G_LT_LTC_cbx.SelectedIndex = col_U_LT_LTC_cbx.SelectedIndex = 
                col_G_ST_cbx.SelectedIndex = col_U_ST_cbx.SelectedIndex = 0;

            //Init Beam Combo Boxes
            beam_MBHT_cbx.SelectedIndex = beam_SHT_cbx.SelectedIndex = beam_SHT_cbx.SelectedIndex =
            beam_ST_cbx.SelectedIndex = 0;

            //Init Slab Combo Boxes
            slab_SOG_L_ST_cbx.SelectedIndex = slab_SOG_T_ST_cbx.SelectedIndex = 
            slab_SS_SM_cbx.SelectedIndex = slab_SS_SD_cbx.SelectedIndex = slab_SS_L_T_cbx.SelectedIndex =
            slab_SS_T_T_cbx.SelectedIndex = slab_SS_L_B_cbx.SelectedIndex = slab_SS_T_B_cbx.SelectedIndex =
            slab_SS_CSR_HT_cbx.SelectedIndex = 0;

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

            setDefaultStructMemName();
            populateColumnConnectionBelow();
            populateBeamRowConnections();
            
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
                else if (parentNode.Equals("COLUMNS"))
                {
                    setColumnValues();
                }
                else if (parentNode.Equals("BEAMS"))
                {
                    setBeamValues();
                }
                else if (parentNode.Equals("SLABS"))
                {
                    setSlabvalues();
                }
                else if(parentNode.Equals("STAIRS"))
                {
                    setStairsValues();
                }
                else if (parentNode.Equals("ROOF"))
                {
                    setRoofValues();
                }
            } 
            else
            {
                if (floorCount > 0) // Upper Floors only
                {
                    foreach (List<string> schedule in costEstimationForm.structuralMembers.slabSchedule[floorCount - 1])
                    {
                        insertSlabSchedule(schedule);
                    }
                }
                populateSlabMark();
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
            slabTabControl.Appearance = TabAppearance.FlatButtons;
            slabTabControl.ItemSize = new Size(0, 1);
            slabTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in slabTabControl.TabPages)
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
            roof_RP_ST_D_CLTSP_cbx.DropDownWidth = DropDownWidth(roof_RP_ST_D_CLTSP_cbx);
            roof_RP_ST_D_CLTSR_cbx.DropDownWidth = DropDownWidth(roof_RP_ST_D_CLTSR_cbx);
            roof_RP_SCP_D_CLCPR_cbx.DropDownWidth = DropDownWidth(roof_RP_SCP_D_CLCPR_cbx);
            roof_RP_SCP_D_CLCPP_cbx.DropDownWidth = DropDownWidth(roof_RP_SCP_D_CLCPP_cbx);           
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

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, memberCount, true);
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

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, memberCount, true);
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

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, memberCount, false);
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

                            compute.ModifyFootingWorks(costEstimationForm, memberCount, memberCount, false);
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
                    if (costEstimationForm.structuralMembers.columnNames[floorCount].Contains(structMemName))
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
                            members.Add(col_G_D_CH_bx.Text);
                            members.Add(col_G_D_Q_bx.Text);
                            members.Add(col_G_D_CB_cbx.Text);

                            members.Add(col_G_MR_D_bx.Text);
                            members.Add(col_G_MR_Q_bx.Text);
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

                            compute.AddColumnWorks(costEstimationForm, floorCount, columnCount);
                            this.DialogResult = DialogResult.OK;
                        }
                        else //Upper Floors
                        {
                            List<string> members = new List<string>();
                            members.Add("Upper");
                            members.Add(col_U_D_B_bx.Text);
                            members.Add(col_U_D_D_bx.Text);
                            members.Add(col_U_D_H_bx.Text);
                            members.Add(col_U_D_CH_bx.Text);
                            members.Add(col_U_D_Q_bx.Text);

                            members.Add(col_U_MR_D_bx.Text);
                            members.Add(col_U_MR_Q_bx.Text);
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

                            compute.AddColumnWorks(costEstimationForm, floorCount, columnCount);
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
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][4] = col_G_D_CH_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][5] = col_G_D_Q_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][6] = col_G_D_CB_cbx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][7] = col_G_MR_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][8] = col_G_MR_Q_bx.Text;
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

                            //compute.ModifyColumnWorks(costEstimationForm, memberCount, columnCount);
                            this.DialogResult = DialogResult.OK;
                        }
                        else //Upper
                        {
                            costEstimationForm.structuralMembers.columnNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][1] = col_U_D_B_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][2] = col_U_D_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][3] = col_U_D_H_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][4] = col_U_D_CH_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][5] = col_U_D_Q_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][6] = col_U_MR_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][7] = col_U_MR_Q_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][8] = col_U_ST_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][9] = col_U_JT_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][10] = col_U_JT_S_bx.Text;

                            costEstimationForm.structuralMembers.column[floorCount][memberCount][11] = col_U_LT_D_bx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][12] = col_U_LT_LTC_cbx.Text;
                            costEstimationForm.structuralMembers.column[floorCount][memberCount][13] = col_U_S_S_bx.Text;

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

                            //compute.ModifyColumnWorks(costEstimationForm, memberCount, columnCount);
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
                    //Name Validation
                    if (costEstimationForm.structuralMembers.beamNames[floorCount].Contains(structMemName))
                    {
                        MessageBox.Show("Name already exists!");
                        return;
                    }
                    /*
                    foreach (BeamScheduleUserControl bs in bs_UC)
                    {
                        foreach(List<string> schedule in costEstimationForm.structuralMembers.beamSchedule[floorCount])
                        {
                            if (schedule[1].Equals(bs.name))
                            {
                                MessageBox.Show("Name for schedule already exsits!");
                                return;
                            }
                        }
                    }
                    */

                    //Do Whatever
                    try
                    {
                        List<string> members = new List<string>();
                        members.Add(beam_BT_cbx.Text);
                        members.Add(beam_D_bx.Text);
                        members.Add(beam_QTY_bx.Text);
                        members.Add(beam_MBHT_cbx.Text);
                        members.Add(beam_SHT_cbx.Text);
                        members.Add(beam_ST_cbx.Text);

                        members.Add(beam_TR_CL_bx.Text);
                        members.Add(beam_BR_CL_bx.Text);

                        List<List<string>> brMember = new List<List<string>>();
                        foreach (BeamRowUserControl br in br_UC)
                        {
                            List<string> brValues = new List<string>();
                            brValues.Add(br.beamName);
                            brValues.Add(br.qty);
                            brValues.Add(br.length);
                            brValues.Add(br.clearLength);
                            brValues.Add(br.support);
                            brMember.Add(brValues);
                        }

                        foreach(List<string> bs in costEstimationForm.structuralMembers.beamSchedule[floorCount])
                        {
                            bool contains = false;
                            foreach(BeamScheduleUserControl bs2 in bs_UC)
                            {
                                if (bs[0].Equals(bs2.type))
                                {
                                    if (bs[1].Equals(bs2.name))
                                    {
                                        contains = true;
                                    }
                                }
                            }

                            if (!contains)
                            {
                                insertBeamScheduleForAdd(bs);
                            }
                        }
                        costEstimationForm.structuralMembers.beamSchedule[floorCount].Clear();
                        foreach (BeamScheduleUserControl bs in bs_UC)
                        {
                            List<string> bsMember = new List<string>();
                            bsMember.Add(bs.type);
                            bsMember.Add(bs.name);
                            bsMember.Add(bs.b);
                            bsMember.Add(bs.d);

                            bsMember.Add(bs.property);
                            bsMember.Add(bs.propertieDiameter1);
                            bsMember.Add(bs.propertieDiameter2);

                            bsMember.Add(bs.extSupport_qty1);
                            bsMember.Add(bs.extSupport_qty2);
                            bsMember.Add(bs.extSupport_qty3);
                            bsMember.Add(bs.extSupport_qty4);
                            
                            bsMember.Add(bs.midspan_qty1);
                            bsMember.Add(bs.midspan_qty2);
                            bsMember.Add(bs.midspan_qty3);
                            bsMember.Add(bs.midspan_qty4);

                            bsMember.Add(bs.intSupport_qty1);
                            bsMember.Add(bs.intSupport_qty2);
                            bsMember.Add(bs.intSupport_qty3);
                            bsMember.Add(bs.intSupport_qty4);

                            bsMember.Add(bs.stirrupDiameter);
                            bsMember.Add(bs.stirrupsValue1);
                            bsMember.Add(bs.stirrupsValueAt1);
                            bsMember.Add(bs.stirrupsValue2);
                            bsMember.Add(bs.stirrupsValueAt2);
                            bsMember.Add(bs.stirrupsRest);
                            
                            bsMember.Add(bs.webBarsDiameter);
                            bsMember.Add(bs.webBarsQty);

                            /*
                            foreach (List<string> schedule in costEstimationForm.structuralMembers.beamSchedule[floorCount])
                            {
                                if (schedule[1].Equals(bs.name))
                                {
                                    MessageBox.Show("Duplicate names inside schedule are not allowed!");
                                    costEstimationForm.structuralMembers.beamSchedule[floorCount].Clear();
                                    return;
                                }
                            }
                            */ 

                            costEstimationForm.structuralMembers.beamSchedule[floorCount].Add(bsMember);
                        }

                        costEstimationForm.structuralMembers.beam[floorCount].Add(members);
                        costEstimationForm.structuralMembers.beamNames[floorCount].Add(structMemName);
                        costEstimationForm.structuralMembers.beamRow[floorCount].Add(brMember);

                        compute.AddBeamWorks(costEstimationForm, floorCount, beamCount, beam_BT_cbx.Text);
                        this.DialogResult = DialogResult.OK;
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
                    if (costEstimationForm.structuralMembers.beamNames[floorCount].Contains(structMemName))
                    {
                        if (structMemName.Equals(oldStructMemName))
                        {
                            //Do nothing
                        }
                        else
                        {
                            int found = 0;
                            for (int i = 0; i < costEstimationForm.structuralMembers.beamNames[floorCount].Count; i++)
                            {
                                if (costEstimationForm.structuralMembers.beamNames[floorCount][i].Equals(structMemName))
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
                    int found2 = 0;

                    /*
                    foreach (BeamScheduleUserControl bs in bs_UC)
                    {
                        foreach (List<string> schedule in costEstimationForm.structuralMembers.beamSchedule[floorCount])
                        {
                            if (schedule[1].Equals(bs.name))
                            {
                                found2++;
                            }
                        }
                        if (found2 > 1)
                        {
                            MessageBox.Show("Duplicate names inside schedule are not allowed!");
                            return;
                        } 
                        else
                        {
                            found2 = 0;
                        }
                    }
                    */

                    //Do Whatever
                    costEstimationForm.structuralMembers.beamNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][0] = beam_BT_cbx.Text;
                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][1] = beam_D_bx.Text;
                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][2] = beam_QTY_bx.Text;
                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][3] = beam_MBHT_cbx.Text;
                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][4] = beam_SHT_cbx.Text;
                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][5] = beam_ST_cbx.Text;

                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][6] = beam_TR_CL_bx.Text;
                    costEstimationForm.structuralMembers.beam[floorCount][memberCount][7] = beam_BR_CL_bx.Text;

                    List<List<string>> brMember = new List<List<string>>();
                    foreach (BeamRowUserControl br in br_UC)
                    {
                        List<string> brValues = new List<string>();
                        brValues.Add(br.beamName);
                        brValues.Add(br.qty);
                        brValues.Add(br.length);
                        brValues.Add(br.clearLength);
                        brValues.Add(br.support);
                        brMember.Add(brValues);
                    }

                    /*
                    int j = 0;
                    foreach(List<string> schedule in costEstimationForm.structuralMembers.beamSchedule[floorCount])
                    {
                        if (floorCount == 0) // Ground Floor
                        {
                            if (beam_BT_cbx.SelectedIndex == 0) // Footing Tie Beam
                            {
                                if (schedule[0].Equals("Footing Tie Beam"))
                                {
                                    updateBeamSchedule(schedule, j); j++;
                                }
                            }
                            else if (beam_BT_cbx.SelectedIndex == 1) // Grade Beam
                            {
                                if (schedule[0].Equals("Grade Beam"))
                                {
                                    updateBeamSchedule(schedule, j); j++;
                                }
                            }
                            else // Roof Beam
                            {
                                if (schedule[0].Equals("Roof Beam"))
                                {
                                    updateBeamSchedule(schedule, j); j++;
                                }
                            }
                        }
                        else // Upper Floors
                        {
                            if (beam_BT_cbx.SelectedIndex == 0) // Suspended Beam
                            {
                                if (schedule[0].Equals("Suspended Beam"))
                                {
                                    updateBeamSchedule(schedule, j); j++;
                                }
                            }
                            else // Roof Beam
                            {
                                if (schedule[0].Equals("Roof Beam"))
                                {
                                    updateBeamSchedule(schedule, j); j++;
                                }
                            }
                        }
                    }
                    */
                    foreach (List<string> bs in costEstimationForm.structuralMembers.beamSchedule[floorCount])
                    {
                        bool contains = false;
                        foreach (BeamScheduleUserControl bs2 in bs_UC)
                        {
                            if (bs[0].Equals(bs2.type))
                            {
                                if (bs[1].Equals(bs2.name))
                                {
                                    contains = true;
                                }
                            }
                        }

                        if (!contains)
                        {
                            insertBeamScheduleForAdd(bs);
                        }
                    }
                    costEstimationForm.structuralMembers.beamSchedule[floorCount].Clear();
                    foreach (BeamScheduleUserControl bs in bs_UC)
                    {
                        List<string> bsMember = new List<string>();
                        bsMember.Add(bs.type);
                        bsMember.Add(bs.name);
                        bsMember.Add(bs.b);
                        bsMember.Add(bs.d);

                        bsMember.Add(bs.property);
                        bsMember.Add(bs.propertieDiameter1);
                        bsMember.Add(bs.propertieDiameter2);

                        bsMember.Add(bs.extSupport_qty1);
                        bsMember.Add(bs.extSupport_qty2);
                        bsMember.Add(bs.extSupport_qty3);
                        bsMember.Add(bs.extSupport_qty4);

                        bsMember.Add(bs.midspan_qty1);
                        bsMember.Add(bs.midspan_qty2);
                        bsMember.Add(bs.midspan_qty3);
                        bsMember.Add(bs.midspan_qty4);

                        bsMember.Add(bs.intSupport_qty1);
                        bsMember.Add(bs.intSupport_qty2);
                        bsMember.Add(bs.intSupport_qty3);
                        bsMember.Add(bs.intSupport_qty4);

                        bsMember.Add(bs.stirrupDiameter);
                        bsMember.Add(bs.stirrupsValue1);
                        bsMember.Add(bs.stirrupsValueAt1);
                        bsMember.Add(bs.stirrupsValue2);
                        bsMember.Add(bs.stirrupsValueAt2);
                        bsMember.Add(bs.stirrupsRest);

                        bsMember.Add(bs.webBarsDiameter);
                        bsMember.Add(bs.webBarsQty);

                        costEstimationForm.structuralMembers.beamSchedule[floorCount].Add(bsMember);
                    }
                    costEstimationForm.structuralMembers.beamRow[floorCount][memberCount] = brMember;
                    compute.ModifyBeamWorks(costEstimationForm, floorCount, memberCount);
                    this.DialogResult = DialogResult.OK;
                }
            }
            else if (addstruct_cbx.Text.Equals("Slab"))
            {
                if (isNew)
                {
                    //Name Validation
                    if (costEstimationForm.structuralMembers.slabNames[floorCount].Contains(structMemName))
                    {
                        MessageBox.Show("Name already exists!");
                        return;
                    }
                    /*
                    foreach (SlabScheduleUserControl ss in ss_UC)
                    {
                        foreach (List<string> schedule in costEstimationForm.structuralMembers.beamSchedule[floorCount])
                        {
                            if (schedule[1].Equals(ss.slabMark))
                            {
                                MessageBox.Show("Name for schedule already exsits!");
                                return;
                            }
                        }
                    }
                    */

                    //Do Whatever
                    try
                    {
                        if(floorCount == 0) // Slab on Grade
                        {
                            List<string> members = new List<string>();
                            members.Add("Slab on Grade");
                            members.Add(slab_SOG_Q_bx.Text);
                            members.Add(slab_SOG_T_bx.Text);
                            members.Add(slab_SOG_E_bx.Text);

                            members.Add(slab_SOG_L_D_bx.Text);
                            members.Add(slab_SOG_T_D_bx.Text);
                            members.Add(slab_SOG_L_S_bx.Text);
                            members.Add(slab_SOG_T_S_bx.Text);
                            members.Add(slab_SOG_L_ST_cbx.Text);
                            members.Add(slab_SOG_T_ST_cbx.Text);

                            members.Add(slab_SOG_SO_HL_bx.Text);
                            members.Add(slab_SOG_SO_VL_bx.Text);
                            members.Add(slab_SOG_SO_CC_bx.Text);

                            members.Add(slab_SOG_SB_T_BR_cbx.Text);
                            members.Add(slab_SOG_SB_T_AtB_cbx.Text);
                            members.Add(slab_SOG_SB_T_CL_bx.Text);

                            members.Add(slab_SOG_SB_B_BR_cbx.Text);
                            members.Add(slab_SOG_SB_B_AtB_cbx.Text);
                            members.Add(slab_SOG_SB_B_CL_bx.Text);

                            members.Add(slab_SOG_SB_L_BR_cbx.Text);
                            members.Add(slab_SOG_SB_L_AtB_cbx.Text);
                            members.Add(slab_SOG_SB_L_CL_bx.Text);

                            members.Add(slab_SOG_SB_R_BR_cbx.Text);
                            members.Add(slab_SOG_SB_R_AtB_cbx.Text);
                            members.Add(slab_SOG_SB_R_CL_bx.Text);

                            members.Add(slab_SOG_SCSD_bx.Text);
                            members.Add(slab_SOG_SOC_bx.Text);

                            costEstimationForm.structuralMembers.slab[floorCount].Add(members);
                            costEstimationForm.structuralMembers.slabNames[floorCount].Add(structMemName);

                            compute.AddSlabWorks(costEstimationForm, floorCount, slabCount);
                            this.DialogResult = DialogResult.OK;
                        }
                        else // Suspended Slab
                        {
                            List<string> members = new List<string>();
                            members.Add("Suspended Slab");
                            members.Add(slab_SS_SM_cbx.Text);

                            //Slab Detail
                            members.Add(slab_SS_SD_cbx.Text);
                            if(slab_SS_SD_cbx.SelectedIndex == 0)
                            {
                                members.Add(sd1_UC.SD2UC_LA);
                                members.Add(sd1_UC.SD2UC_LB);
                                members.Add(sd1_UC.SD2UC_BG);
                            }
                            else
                            {
                                members.Add(sd2_UC.SD1UC_LA);
                                members.Add(sd2_UC.SD1UC_LB);
                                members.Add(sd2_UC.SD1UC_LC);
                                members.Add(sd2_UC.SD1UC_LD);
                            }

                            members.Add(slab_SS_L_T_cbx.Text);
                            members.Add(slab_SS_T_T_cbx.Text);
                            members.Add(slab_SS_L_B_cbx.Text);
                            members.Add(slab_SS_T_B_cbx.Text);

                            members.Add(slab_SS_SB_T_BR_cbx.Text);
                            members.Add(slab_SS_SB_T_AtB_cbx.Text);
                            members.Add(slab_SS_SB_T_CL_bx.Text);

                            members.Add(slab_SS_SB_B_BR_cbx.Text);
                            members.Add(slab_SS_SB_B_AtB_cbx.Text);
                            members.Add(slab_SS_SB_B_CL_bx.Text);

                            members.Add(slab_SS_SB_L_BR_cbx.Text);
                            members.Add(slab_SS_SB_L_AtB_cbx.Text);
                            members.Add(slab_SS_SB_L_CL_bx.Text);

                            members.Add(slab_SS_SB_R_BR_cbx.Text);
                            members.Add(slab_SS_SB_R_AtB_cbx.Text);
                            members.Add(slab_SS_SB_R_CL_bx.Text);

                            members.Add(slab_SS_SCSD_bx.Text);
                            members.Add(slab_SS_SOC_bx.Text);

                            costEstimationForm.structuralMembers.slabSchedule[floorCount - 1].Clear();
                            foreach (SlabScheduleUserControl ss in ss_UC)
                            {
                                List<string> ssMember = new List<string>();
                                ssMember.Add(ss.slabMark);
                                ssMember.Add(ss.thickness);

                                ssMember.Add(ss.RSASD_S);
                                ssMember.Add(ss.RSASD_ES_T);
                                ssMember.Add(ss.RSASD_ES_B);
                                ssMember.Add(ss.RSASD_MS_T);
                                ssMember.Add(ss.RSASD_MS_B);
                                ssMember.Add(ss.RSASD_IS_T);
                                ssMember.Add(ss.RSASD_IS_B);

                                ssMember.Add(ss.RSALD_S);
                                ssMember.Add(ss.RSALD_ES_T);
                                ssMember.Add(ss.RSALD_ES_B);
                                ssMember.Add(ss.RSALD_MS_T);
                                ssMember.Add(ss.RSALD_MS_B);
                                ssMember.Add(ss.RSALD_IS_T);
                                ssMember.Add(ss.RSALD_IS_B);

                                ssMember.Add(ss.remark);

                                /*
                                foreach (List<string> schedule in costEstimationForm.structuralMembers.slabSchedule[floorCount - 1])
                                {
                                    if (schedule[1].Equals(ss.slabMark))
                                    {
                                        MessageBox.Show("Duplicate names inside schedule are not allowed!");
                                        costEstimationForm.structuralMembers.slabSchedule[floorCount - 1].Clear();
                                        return;
                                    }
                                }
                                */
                                costEstimationForm.structuralMembers.slabSchedule[floorCount - 1].Add(ssMember);
                            }

                            costEstimationForm.structuralMembers.slab[floorCount].Add(members);
                            costEstimationForm.structuralMembers.slabNames[floorCount].Add(structMemName);

                            compute.AddSlabWorks(costEstimationForm, floorCount, slabCount);
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
                    if (costEstimationForm.structuralMembers.slabNames[floorCount].Contains(structMemName))
                    {
                        if (structMemName.Equals(oldStructMemName))
                        {
                            //Do nothing
                        }
                        else
                        {
                            int found = 0;
                            for (int i = 0; i < costEstimationForm.structuralMembers.slabNames[floorCount].Count; i++)
                            {
                                if (costEstimationForm.structuralMembers.slabNames[floorCount][i].Equals(structMemName))
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
                    /*
                    int found2 = 0;
                    foreach (SlabScheduleUserControl ss in ss_UC)
                    {
                        foreach (List<string> schedule in costEstimationForm.structuralMembers.slabSchedule[floorCount - 1])
                        {
                            if (schedule[0].Equals(ss.slabMark))
                            {
                                found2++;
                            }
                        }
                        if (found2 > 1)
                        {
                            MessageBox.Show("Duplicate names inside schedule are not allowed!");
                            return;
                        }
                        else
                        {
                            found2 = 0;
                        }
                    }
                    */

                    //Do Whatever 
                    if (floorCount == 0) // Slab on Grade
                    {
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][1] = slab_SOG_Q_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][2] = slab_SOG_T_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][3] = slab_SOG_E_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][4] = slab_SOG_L_D_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][5] = slab_SOG_T_D_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][6] = slab_SOG_L_S_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][7] = slab_SOG_T_S_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][8] = slab_SOG_L_ST_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][9] = slab_SOG_T_ST_cbx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][10] = slab_SOG_SO_HL_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][11] = slab_SOG_SO_VL_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][12] = slab_SOG_SO_CC_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][13] = slab_SOG_SB_T_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][14] = slab_SOG_SB_T_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][15] = slab_SOG_SB_T_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][16] = slab_SOG_SB_B_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][17] = slab_SOG_SB_B_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][18] = slab_SOG_SB_B_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][19] = slab_SOG_SB_L_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][20] = slab_SOG_SB_L_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][21] = slab_SOG_SB_L_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][22] = slab_SOG_SB_R_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][23] = slab_SOG_SB_R_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][24] = slab_SOG_SB_R_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][25] = slab_SOG_SCSD_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][26] = slab_SOG_SOC_bx.Text;

                        compute.ModifySlabWorks(costEstimationForm, floorCount, memberCount);
                        this.DialogResult = DialogResult.OK;
                    }
                    else // Suspended Slab
                    {
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][1] = slab_SS_SM_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][2] = slab_SS_SD_cbx.Text;
                        int i = 0;
                        if (slab_SS_SD_cbx.Text == "No. 1")
                        {
                            costEstimationForm.structuralMembers.slab[floorCount][memberCount][3] = sd1_UC.SD2UC_LA;
                            costEstimationForm.structuralMembers.slab[floorCount][memberCount][4] = sd1_UC.SD2UC_LB;
                            costEstimationForm.structuralMembers.slab[floorCount][memberCount][5] = sd1_UC.SD2UC_BG;
                            i++;
                        }
                        else
                        {
                            costEstimationForm.structuralMembers.slab[floorCount][memberCount][3] = sd2_UC.SD1UC_LA;
                            costEstimationForm.structuralMembers.slab[floorCount][memberCount][4] = sd2_UC.SD1UC_LB;
                            costEstimationForm.structuralMembers.slab[floorCount][memberCount][5] = sd2_UC.SD1UC_LC;
                            costEstimationForm.structuralMembers.slab[floorCount][memberCount][6] = sd2_UC.SD1UC_LD;
                        }
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][7 - i] = slab_SS_L_T_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][8 - i] = slab_SS_T_T_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][9 - i] = slab_SS_L_B_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][10 - i] = slab_SS_T_B_cbx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][11 - i] = slab_SS_SB_T_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][12 - i] = slab_SS_SB_T_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][13 - i] = slab_SS_SB_T_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][14 - i] = slab_SS_SB_B_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][15 - i] = slab_SS_SB_B_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][16 - i] = slab_SS_SB_B_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][17 - i] = slab_SS_SB_L_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][18 - i] = slab_SS_SB_L_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][19 - i] = slab_SS_SB_L_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][20 - i] = slab_SS_SB_R_BR_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][21 - i] = slab_SS_SB_R_AtB_cbx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][22 - i] = slab_SS_SB_R_CL_bx.Text;

                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][23 - i] = slab_SS_SCSD_bx.Text;
                        costEstimationForm.structuralMembers.slab[floorCount][memberCount][24 - i] = slab_SS_SOC_bx.Text;

                        costEstimationForm.structuralMembers.slabSchedule[floorCount - 1].Clear();
                        foreach (SlabScheduleUserControl ss in ss_UC)
                        {
                            List<string> ssMember = new List<string>();
                            ssMember.Add(ss.slabMark);
                            ssMember.Add(ss.thickness);

                            ssMember.Add(ss.RSASD_S);
                            ssMember.Add(ss.RSASD_ES_T);
                            ssMember.Add(ss.RSASD_ES_B);
                            ssMember.Add(ss.RSASD_MS_T);
                            ssMember.Add(ss.RSASD_MS_B);
                            ssMember.Add(ss.RSASD_IS_T);
                            ssMember.Add(ss.RSASD_IS_B);

                            ssMember.Add(ss.RSALD_S);
                            ssMember.Add(ss.RSALD_ES_T);
                            ssMember.Add(ss.RSALD_ES_B);
                            ssMember.Add(ss.RSALD_MS_T);
                            ssMember.Add(ss.RSALD_MS_B);
                            ssMember.Add(ss.RSALD_IS_T);
                            ssMember.Add(ss.RSALD_IS_B);

                            ssMember.Add(ss.remark);

                            /*
                            foreach (List<string> schedule in costEstimationForm.structuralMembers.slabSchedule[floorCount - 1])
                            {
                                if (schedule[0].Equals(ss.slabMark))
                                {
                                    MessageBox.Show("Duplicate names inside schedule are not allowed!");
                                    costEstimationForm.structuralMembers.slabSchedule[floorCount - 1].Clear();
                                    return;
                                }
                            }
                            */
                            costEstimationForm.structuralMembers.slabSchedule[floorCount - 1].Add(ssMember);
                        }

                        compute.ModifySlabWorks(costEstimationForm, floorCount, memberCount);
                        this.DialogResult = DialogResult.OK;
                    }
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

                            compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
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

                            compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
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

                            compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
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

                            compute.ModifyStairsWorks(costEstimationForm, floorCount, memberCount);
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

                            compute.ModifyStairsWorks(costEstimationForm, floorCount, memberCount);
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

                            compute.ModifyStairsWorks(costEstimationForm, floorCount, memberCount);
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
            else //Roofing(here)
            {                
                if (isNew)
                {
                    //Name Validation
                    if (costEstimationForm.structuralMembers.roofNames[floorCount].Contains(structMemName))
                    {
                        MessageBox.Show("Name already exists!");
                        return;
                    }

                    //Do Whatever
                    if (roof_PGR_cbx.SelectedIndex == 0) //Rafters and Purlins
                    {
                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(roof_PGR_cbx.Text);

                            if (roof_RP_W_rb.Checked == true) //Wood
                            {
                                members.Add(roof_RP_W_rb.Text);
                                members.Add(roof_RP_W_D_LR_bx.Text);
                                members.Add(roof_RP_W_D_LP_bx.Text);
                                members.Add(roof_RP_W_D_SR_bx.Text);
                                members.Add(roof_RP_W_D_SP_bx.Text);
                            }
                            else if (roof_RP_ST_rb.Checked == true) //Steel - Tubular
                            {
                                members.Add(roof_RP_ST_rb.Text);
                                members.Add(roof_RP_ST_D_LRSW_bx.Text);
                                members.Add(roof_RP_ST_D_LR_bx.Text);
                                members.Add(roof_RP_ST_D_LP_bx.Text); 
                                members.Add(roof_RP_ST_D_SR_bx.Text);
                                members.Add(roof_RP_ST_D_SP_bx.Text);
                                members.Add(roof_RP_ST_D_CLTSR_cbx.Text);
                                members.Add(roof_RP_ST_D_CLTSP_cbx.Text);
                            }
                            else if (roof_RP_SCP_rb.Checked == true) //Steel - Cee Purlins
                            {
                                members.Add(roof_RP_SCP_rb.Text);
                                members.Add(roof_RP_SCP_D_LRSW_bx.Text);
                                members.Add(roof_RP_SCP_D_LR_bx.Text);
                                members.Add(roof_RP_SCP_D_LP_bx.Text);
                                members.Add(roof_RP_SCP_D_SR_bx.Text);
                                members.Add(roof_RP_SCP_D_SP_bx.Text);
                                members.Add(roof_RP_SCP_D_CLCPR_cbx.Text);
                                members.Add(roof_RP_SCP_D_CLCPP_cbx.Text);
                            }
                            List<string> hrsMember = new List<string>();
                            costEstimationForm.structuralMembers.roof[floorCount].Add(members);
                            costEstimationForm.structuralMembers.roofHRS[floorCount].Add(hrsMember);
                            costEstimationForm.structuralMembers.roofNames[floorCount].Add(structMemName);                            
                            compute.AddRoofWorks(costEstimationForm, floorCount, roofCount);
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
                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(roof_PGR_cbx.Text);

                            members.Add(roof_GI_D_LP_bx.Text);
                            members.Add(roof_GI_D_EC_cbx.Text);
                            List<string> hrsMember = new List<string>();
                            foreach (RoofHRSUserControl rHRS in rHRS_UC)
                            {
                                hrsMember.Add(rHRS.value);
                            }

                            if (roof_GI_M_CGIS_cb.Checked == true)
                                members.Add(roof_GI_M_CGIS_cb.Text);
                            if (roof_GI_M_GIRN_cb.Checked == true)
                                members.Add(roof_GI_M_GIRN_cb.Text);
                            if (roof_GI_M_GIR_cb.Checked == true)
                                members.Add(roof_GI_M_GIR_cb.Text);
                            if (roof_GI_M_GIW_cb.Checked == true)
                                members.Add(roof_GI_M_GIW_cb.Text);
                            if (roof_GI_M_LW_cb.Checked == true)
                                members.Add(roof_GI_M_LW_cb.Text);
                            if (roof_GI_M_UN_cb.Checked == true)
                                members.Add(roof_GI_M_UN_cb.Text);
                            if (roof_GI_M_PGIS_cb.Checked == true)
                            {
                                members.Add(roof_GI_M_PGIS_cb.Text); 
                                members.Add(roof_GI_M_SP_cbx.Text);
                            }
                            costEstimationForm.structuralMembers.roof[floorCount].Add(members);
                            costEstimationForm.structuralMembers.roofHRS[floorCount].Add(hrsMember);
                            costEstimationForm.structuralMembers.roofNames[floorCount].Add(structMemName);
                            compute.AddRoofWorks(costEstimationForm, floorCount, roofCount);
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Do not leave any blank spaces!");
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else //Roof Accessories (Tinswork)
                    {
                        try
                        {
                            List<string> members = new List<string>();
                            members.Add(roof_PGR_cbx.Text);

                            members.Add(roof_RA_D_G_TL_bx.Text);
                            members.Add(roof_RA_D_G_EL_bx.Text);
                            members.Add(roof_RA_D_G_TW_bx.Text);

                            members.Add(roof_RA_D_F_TL_bx.Text);
                            members.Add(roof_RA_D_F_EL_bx.Text);
                            members.Add(roof_RA_D_F_TW_bx.Text);

                            members.Add(roof_RA_D_RR_TL_bx.Text);
                            members.Add(roof_RA_D_RR_EL_bx.Text);
                            members.Add(roof_RA_D_RR_TW_bx.Text);

                            members.Add(roof_RA_D_VR_TL_bx.Text);
                            members.Add(roof_RA_D_VR_EL_bx.Text);
                            members.Add(roof_RA_D_VR_TW_bx.Text);

                            members.Add(roof_RA_D_HR_TL_bx.Text);
                            members.Add(roof_RA_D_HR_EL_bx.Text);
                            members.Add(roof_RA_D_HR_TW_bx.Text);

                            List<string> hrsMember = new List<string>();
                            costEstimationForm.structuralMembers.roof[floorCount].Add(members);
                            costEstimationForm.structuralMembers.roofHRS[floorCount].Add(hrsMember);
                            costEstimationForm.structuralMembers.roofNames[floorCount].Add(structMemName);
                            compute.AddRoofWorks(costEstimationForm, floorCount, roofCount);
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
                    if (costEstimationForm.structuralMembers.roofNames[floorCount].Contains(structMemName))
                    {
                        if (structMemName.Equals(oldStructMemName))
                        {
                            //Do nothing
                        }
                        else
                        {
                            int found = 0;
                            for (int i = 0; i < costEstimationForm.structuralMembers.roofNames[floorCount].Count; i++)
                            {
                                if (costEstimationForm.structuralMembers.roofNames[floorCount][i].Equals(structMemName))
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

                    if (roof_PGR_cbx.SelectedIndex == 0) //Rafter and Purlins
                    {
                        costEstimationForm.structuralMembers.roofNames[floorCount][memberCount] = addstruct_Name_bx.Text;
                        
                        if (roof_RP_W_rb.Checked == true) //Wood
                        {
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][1] = "Wood";
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][2] = roof_RP_W_D_LR_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][3] = roof_RP_W_D_LP_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][4] = roof_RP_W_D_SR_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][5] = roof_RP_W_D_SP_bx.Text;
                        }
                        else if (roof_RP_ST_rb.Checked == true)
                        {
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][1] = "Steel - Tubular";
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][2] = roof_RP_ST_D_LRSW_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][3] = roof_RP_ST_D_LR_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][4] = roof_RP_ST_D_LP_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][5] = roof_RP_ST_D_SR_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][6] = roof_RP_ST_D_SP_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][7] = roof_RP_ST_D_CLTSR_cbx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][8] = roof_RP_ST_D_CLTSP_cbx.Text;
                        }
                        else
                        {
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][1] = "Steel - Cee Purlins";
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][2] = roof_RP_SCP_D_LRSW_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][3] = roof_RP_SCP_D_LR_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][4] = roof_RP_SCP_D_LP_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][5] = roof_RP_SCP_D_SR_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][6] = roof_RP_SCP_D_SP_bx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][7] = roof_RP_SCP_D_CLCPR_cbx.Text;
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount][8] = roof_RP_SCP_D_CLCPP_cbx.Text;
                        }
                        compute.ModifyRoofWorks(costEstimationForm, floorCount, memberCount);
                        //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                        this.DialogResult = DialogResult.OK;
                    }
                    else if (roof_PGR_cbx.SelectedIndex == 1) //G.I Roof and Its Accessories
                    {
                        costEstimationForm.structuralMembers.roofNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][1] = roof_GI_D_LP_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][2] = roof_GI_D_EC_cbx.Text;

                        for(int i = 3; i < costEstimationForm.structuralMembers.roof[floorCount][memberCount].Count; i++)
                        {
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].RemoveAt(i);
                        }
                        if (roof_GI_M_CGIS_cb.Checked == true)
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add("Corrugated G.I Sheet");
                        if (roof_GI_M_GIRN_cb.Checked == true)
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add("G.I Roof Nails");
                        if (roof_GI_M_GIR_cb.Checked == true)
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add("G.I Rivets");   
                        if (roof_GI_M_GIW_cb.Checked == true)
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add("G.I Washers");
                        if (roof_GI_M_LW_cb.Checked == true)
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add("Lead Washers");
                        if (roof_GI_M_UN_cb.Checked == true)
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add("Umbrella Nails");
                        if (roof_GI_M_PGIS_cb.Checked == true)
                        {
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add("Plain G.I Strap");
                            costEstimationForm.structuralMembers.roof[floorCount][memberCount].Add(roof_GI_M_SP_cbx.Text);
                        }

                        List<string> hrsMember = new List<string>();
                        foreach (RoofHRSUserControl rHRS in rHRS_UC)
                        {
                            hrsMember.Add(rHRS.value);
                        }

                        costEstimationForm.structuralMembers.roofHRS[floorCount][memberCount] = hrsMember;
                        compute.ModifyRoofWorks(costEstimationForm, floorCount, memberCount);
                        //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                        this.DialogResult = DialogResult.OK;
                    }
                    else //Roof Accessories(Tinswork)
                    {
                        costEstimationForm.structuralMembers.roofNames[floorCount][memberCount] = addstruct_Name_bx.Text;

                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][1] = roof_RA_D_G_TL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][2] = roof_RA_D_G_EL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][3] = roof_RA_D_G_TW_bx.Text;

                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][4] = roof_RA_D_F_TL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][5] = roof_RA_D_F_EL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][6] = roof_RA_D_F_TW_bx.Text;

                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][7] = roof_RA_D_RR_TL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][8] = roof_RA_D_RR_EL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][9] = roof_RA_D_RR_TW_bx.Text;

                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][10] = roof_RA_D_VR_TL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][11] = roof_RA_D_VR_EL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][12] = roof_RA_D_VR_TW_bx.Text;

                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][13] = roof_RA_D_HR_TL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][14] = roof_RA_D_HR_EL_bx.Text;
                        costEstimationForm.structuralMembers.roof[floorCount][memberCount][15] = roof_RA_D_HR_TW_bx.Text;
                        compute.ModifyRoofWorks(costEstimationForm, floorCount, memberCount);
                        //compute.AddStairsWorks(costEstimationForm, floorCount, stairsCount);
                        this.DialogResult = DialogResult.OK;

                    }
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
                    addstruct_Name_bx.Text = "BR-" + (nodes[2].Nodes.Count + 1);
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
                    addstruct_Name_bx.Text = "BR-" + (nodes[1].Nodes.Count + 1);
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
                col_G_D_CH_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][4];
                col_G_D_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][5];
                col_G_D_CB_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][6];

                col_G_MR_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][7];
                col_G_MR_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][8];
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
                col_U_D_CH_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][4];
                col_U_D_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][5];

                col_U_MR_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][6];
                col_U_MR_Q_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][7];
                col_U_ST_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][8];
                col_U_JT_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][9];
                col_U_JT_S_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][10];

                col_U_LT_D_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][11];
                col_U_LT_LTC_cbx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][12];
                col_U_S_S_bx.Text = costEstimationForm.structuralMembers.column[floorCount][memberCount][13];

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
                col_G_D_CB_cbx.Items.Add("None");
                foreach (string name in costEstimationForm.structuralMembers.footingColumnNames)
                {
                    col_G_D_CB_cbx.Items.Add(name);
                }
                col_G_D_CB_cbx.SelectedIndex = 0;
            }
        }

        private void setBeamValues()
        {
            if (floorCount == 0)
            {
                addstruct_cbx.SelectedIndex = 3;
            }
            else
            {
                addstruct_cbx.SelectedIndex = 1;
            }

            oldStructMemName = costEstimationForm.structuralMembers.beamNames[floorCount][memberCount];
            addstruct_Name_bx.Text = costEstimationForm.structuralMembers.beamNames[floorCount][memberCount];

            beam_BT_cbx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][0];
            beam_D_bx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][1];
            beam_QTY_bx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][2];
            beam_MBHT_cbx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][3];
            beam_SHT_cbx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][4];
            beam_ST_cbx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][5];

            beam_TR_CL_bx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][6];
            beam_BR_CL_bx.Text = costEstimationForm.structuralMembers.beam[floorCount][memberCount][7];

            foreach (List<string> br in costEstimationForm.structuralMembers.beamRow[floorCount][memberCount])
            {
                BeamRowUserControl content2 = new BeamRowUserControl(this, br[0]);
                content2.beamName = br[0];
                content2.qty = br[1];
                content2.length = br[2];
                content2.clearLength = br[3];
                content2.support = br[4];
                br_UC.Add(content2);
                beam_BR_Panel.Controls.Add(content2);
            }
        }

        private void insertBeamSchedule(List<string> schedule)
        {
            BeamScheduleUserControl content2 = new BeamScheduleUserControl(this, schedule[0]);
            content2.name = schedule[1];
            content2.b = schedule[2];
            content2.d = schedule[3];

            content2.property = schedule[4];
            content2.propertieDiameter1 = schedule[5];
            content2.propertieDiameter2 = schedule[6];

            content2.extSupport_qty1 = schedule[7];
            content2.extSupport_qty2 = schedule[8];
            content2.extSupport_qty3 = schedule[9];
            content2.extSupport_qty4 = schedule[10];

            content2.midspan_qty1 = schedule[11];
            content2.midspan_qty2 = schedule[12];
            content2.midspan_qty3 = schedule[13];
            content2.midspan_qty4 = schedule[14];

            content2.intSupport_qty1 = schedule[15];
            content2.intSupport_qty2 = schedule[16];
            content2.intSupport_qty3 = schedule[17];
            content2.intSupport_qty4 = schedule[18];

            content2.stirrupDiameter = schedule[19];
            content2.stirrupsValue1 = schedule[20];
            content2.stirrupsValueAt1 = schedule[21];
            content2.stirrupsValue2 = schedule[22];
            content2.stirrupsValueAt2 = schedule[23];
            content2.stirrupsRest = schedule[24];

            content2.webBarsDiameter = schedule[25];
            content2.webBarsQty = schedule[26];

            bs_UC.Add(content2);
            beam_BS_Panel.Controls.Add(content2);
            foreach (BeamRowUserControl br in br_UC)
            {
                br.addScheduleName(schedule[1]);
            }
        }

        private void insertBeamScheduleForAdd(List<string> schedule)
        {
            BeamScheduleUserControl content2 = new BeamScheduleUserControl(this, schedule[0]);
            content2.name = schedule[1];
            content2.b = schedule[2];
            content2.d = schedule[3];

            content2.property = schedule[4];
            content2.propertieDiameter1 = schedule[5];
            content2.propertieDiameter2 = schedule[6];

            content2.extSupport_qty1 = schedule[7];
            content2.extSupport_qty2 = schedule[8];
            content2.extSupport_qty3 = schedule[9];
            content2.extSupport_qty4 = schedule[10];

            content2.midspan_qty1 = schedule[11];
            content2.midspan_qty2 = schedule[12];
            content2.midspan_qty3 = schedule[13];
            content2.midspan_qty4 = schedule[14];

            content2.intSupport_qty1 = schedule[15];
            content2.intSupport_qty2 = schedule[16];
            content2.intSupport_qty3 = schedule[17];
            content2.intSupport_qty4 = schedule[18];

            content2.stirrupDiameter = schedule[19];
            content2.stirrupsValue1 = schedule[20];
            content2.stirrupsValueAt1 = schedule[21];
            content2.stirrupsValue2 = schedule[22];
            content2.stirrupsValueAt2 = schedule[23];
            content2.stirrupsRest = schedule[24];

            content2.webBarsDiameter = schedule[25];
            content2.webBarsQty = schedule[26];

            bs_UC.Add(content2);
        }

        private void updateBeamSchedule(List<string> schedule, int key)
        {
            schedule[0] = bs_UC[key].type;
            schedule[1] = bs_UC[key].name;
            schedule[2] = bs_UC[key].b;
            schedule[3] = bs_UC[key].d;

            schedule[4] = bs_UC[key].property;
            schedule[5] = bs_UC[key].propertieDiameter1;
            schedule[6] = bs_UC[key].propertieDiameter2;

            schedule[7] = bs_UC[key].extSupport_qty1;
            schedule[8] = bs_UC[key].extSupport_qty2;
            schedule[9] = bs_UC[key].extSupport_qty3;
            schedule[10] = bs_UC[key].extSupport_qty4;

            schedule[11] = bs_UC[key].midspan_qty1;
            schedule[12] = bs_UC[key].midspan_qty2;
            schedule[13] = bs_UC[key].midspan_qty3;
            schedule[14] = bs_UC[key].midspan_qty4;

            schedule[15] = bs_UC[key].intSupport_qty1;
            schedule[16] = bs_UC[key].intSupport_qty2;
            schedule[17] = bs_UC[key].intSupport_qty3;
            schedule[18] = bs_UC[key].intSupport_qty4;

            schedule[19] = bs_UC[key].stirrupDiameter;
            schedule[20] = bs_UC[key].stirrupsValue1;
            schedule[21] = bs_UC[key].stirrupsValueAt1;
            schedule[22] = bs_UC[key].stirrupsValue2;
            schedule[23] = bs_UC[key].stirrupsValueAt2;
            schedule[24] = bs_UC[key].stirrupsRest;

            schedule[25] = bs_UC[key].webBarsDiameter;
            schedule[26] = bs_UC[key].webBarsQty;
        }
        
        private void populateBeamRowConnections()
        {
            if(floorCount == 0) //Slab on Grade
            {
                int i = 0;
                slab_SOG_SB_T_BR_cbx.Items.Clear();
                slab_SOG_SB_B_BR_cbx.Items.Clear();
                slab_SOG_SB_L_BR_cbx.Items.Clear();
                slab_SOG_SB_R_BR_cbx.Items.Clear();

                slab_SOG_SB_T_BR_cbx.Items.Add("None");
                slab_SOG_SB_B_BR_cbx.Items.Add("None");
                slab_SOG_SB_L_BR_cbx.Items.Add("None");
                slab_SOG_SB_R_BR_cbx.Items.Add("None");
                foreach(List<string> beam in costEstimationForm.structuralMembers.beam[floorCount])
                {
                    if (costEstimationForm.structuralMembers.beam[floorCount][i][0].Equals("Footing Tie Beam") ||
                           costEstimationForm.structuralMembers.beam[floorCount][i][0].Equals("Grade Beam"))
                    {
                        slab_SOG_SB_T_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i]);
                        slab_SOG_SB_B_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i]);
                        slab_SOG_SB_L_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i]);
                        slab_SOG_SB_R_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i]);
                    }
                    i++;
                }
                slab_SOG_SB_T_BR_cbx.SelectedIndex = 0;
                slab_SOG_SB_B_BR_cbx.SelectedIndex = 0;
                slab_SOG_SB_L_BR_cbx.SelectedIndex = 0;
                slab_SOG_SB_R_BR_cbx.SelectedIndex = 0;

                slab_SOG_SB_T_AtB_cbx.SelectedIndex = 0;
                slab_SOG_SB_B_AtB_cbx.SelectedIndex = 0;
                slab_SOG_SB_L_AtB_cbx.SelectedIndex = 0;
                slab_SOG_SB_R_AtB_cbx.SelectedIndex = 0;
            }
            else //Suspended Slab
            {
                int i = 0;
                slab_SS_SB_T_BR_cbx.Items.Clear();
                slab_SS_SB_B_BR_cbx.Items.Clear();
                slab_SS_SB_L_BR_cbx.Items.Clear();
                slab_SS_SB_R_BR_cbx.Items.Clear();

                slab_SS_SB_T_BR_cbx.Items.Add("None");
                slab_SS_SB_B_BR_cbx.Items.Add("None");
                slab_SS_SB_L_BR_cbx.Items.Add("None");
                slab_SS_SB_R_BR_cbx.Items.Add("None");
                foreach (List<string> beam in costEstimationForm.structuralMembers.beam[floorCount])
                {
                    if (costEstimationForm.structuralMembers.beam[floorCount][i][0].Equals("Suspended Beam"))
                    {
                        slab_SS_SB_T_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i] + "(Suspended Beam)");
                        slab_SS_SB_B_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i] + "(Suspended Beam)");
                        slab_SS_SB_L_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i] + "(Suspended Beam)");
                        slab_SS_SB_R_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount][i] + "(Suspended Beam)");
                    }
                    i++;
                }
                i = 0;
                foreach (List<string> beam in costEstimationForm.structuralMembers.beam[floorCount - 1])
                {
                    if (costEstimationForm.structuralMembers.beam[floorCount - 1][i][0].Equals("Roof Beam"))
                    {
                        slab_SS_SB_T_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount - 1][i] + "(Roof Beam)");
                        slab_SS_SB_B_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount - 1][i] + "(Roof Beam)");
                        slab_SS_SB_L_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount - 1][i] + "(Roof Beam)");
                        slab_SS_SB_R_BR_cbx.Items.Add(costEstimationForm.structuralMembers.beamNames[floorCount - 1][i] + "(Roof Beam)");
                    }
                    i++;
                }
                slab_SS_SB_T_BR_cbx.SelectedIndex = 0;
                slab_SS_SB_B_BR_cbx.SelectedIndex = 0;
                slab_SS_SB_L_BR_cbx.SelectedIndex = 0;
                slab_SS_SB_R_BR_cbx.SelectedIndex = 0;

                slab_SS_SB_T_AtB_cbx.SelectedIndex = 0;
                slab_SS_SB_B_AtB_cbx.SelectedIndex = 0;
                slab_SS_SB_L_AtB_cbx.SelectedIndex = 0;
                slab_SS_SB_R_AtB_cbx.SelectedIndex = 0;
            }
        }

        private void setSlabvalues()
        {
            if (floorCount == 0)
            {
                addstruct_cbx.SelectedIndex = 4;
            }
            else
            {
                addstruct_cbx.SelectedIndex = 2;
            }

            oldStructMemName = costEstimationForm.structuralMembers.slabNames[floorCount][memberCount];
            addstruct_Name_bx.Text = costEstimationForm.structuralMembers.slabNames[floorCount][memberCount];

            if (costEstimationForm.structuralMembers.slab[floorCount][memberCount][0].Equals("Slab on Grade"))
            {
                slab_SOG_Q_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][1];
                slab_SOG_T_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][2];
                slab_SOG_E_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][3];
                
                slab_SOG_L_D_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][4];
                slab_SOG_T_D_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][5];
                slab_SOG_L_S_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][6];
                slab_SOG_T_S_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][7];
                slab_SOG_L_ST_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][8];
                slab_SOG_T_ST_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][9];

                slab_SOG_SO_HL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][10];
                slab_SOG_SO_VL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][11];
                slab_SOG_SO_CC_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][12];

                slab_SOG_SB_T_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][13];
                slab_SOG_SB_T_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][14];
                slab_SOG_SB_T_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][15];

                slab_SOG_SB_B_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][16];
                slab_SOG_SB_B_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][17];
                slab_SOG_SB_B_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][18];

                slab_SOG_SB_L_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][19];
                slab_SOG_SB_L_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][20];
                slab_SOG_SB_L_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][21];

                slab_SOG_SB_R_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][22];
                slab_SOG_SB_R_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][23];
                slab_SOG_SB_R_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][24];

                slab_SOG_SCSD_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][25];
                slab_SOG_SOC_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][26];
            }
            else
            {
                slab_SS_SD_cbx.Enabled = false;
                slab_SS_SM_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][1];
                slab_SS_SD_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][2];
                int i = 0;
                if(slab_SS_SD_cbx.Text == "No. 1")
                {
                    var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.SLAB_DETAIL);
                    Image picture = (Image)bmp;
                    slab_SS_SD_pb.Image = picture;

                    SlabDetail2UserControl content = new SlabDetail2UserControl();
                    content.SD2UC_LA = costEstimationForm.structuralMembers.slab[floorCount][memberCount][3];
                    content.SD2UC_LB = costEstimationForm.structuralMembers.slab[floorCount][memberCount][4];
                    content.SD2UC_BG = costEstimationForm.structuralMembers.slab[floorCount][memberCount][5];
                    sd1_UC = content;
                    slab_SS_SD_Panel.Controls.Clear();
                    slab_SS_SD_Panel.Controls.Add(content);
                    i++;
                }
                else
                {
                    var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.SLAB_DETAIL_2);
                    Image picture = (Image)bmp;
                    slab_SS_SD_pb.Image = picture;

                    SlabDetail1UserControl content = new SlabDetail1UserControl();
                    sd2_UC = content;
                    content.SD1UC_LA = costEstimationForm.structuralMembers.slab[floorCount][memberCount][3];
                    content.SD1UC_LB = costEstimationForm.structuralMembers.slab[floorCount][memberCount][4];
                    content.SD1UC_LC = costEstimationForm.structuralMembers.slab[floorCount][memberCount][5];
                    content.SD1UC_LD = costEstimationForm.structuralMembers.slab[floorCount][memberCount][6];
                    slab_SS_SD_Panel.Controls.Clear();
                    slab_SS_SD_Panel.Controls.Add(content);
                }
                slab_SS_L_T_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][7 - i];
                slab_SS_T_T_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][8 - i];
                slab_SS_L_B_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][9 - i];
                slab_SS_T_B_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][10 - i];

                slab_SS_SB_T_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][11 - i];
                slab_SS_SB_T_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][12 - i];
                slab_SS_SB_T_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][13 - i];

                slab_SS_SB_B_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][14 - i];
                slab_SS_SB_B_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][15 - i];
                slab_SS_SB_B_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][16 - i];

                slab_SS_SB_L_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][17 - i];
                slab_SS_SB_L_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][18 - i];
                slab_SS_SB_L_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][19 - i];

                slab_SS_SB_R_BR_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][20 - i];
                slab_SS_SB_R_AtB_cbx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][21 - i];
                slab_SS_SB_R_CL_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][22 - i];

                slab_SS_SCSD_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][23 - i];
                slab_SS_SOC_bx.Text = costEstimationForm.structuralMembers.slab[floorCount][memberCount][24 - i];

                foreach (List<string> schedule in costEstimationForm.structuralMembers.slabSchedule[floorCount - 1])
                {
                    insertSlabSchedule(schedule);
                }
                populateSlabMark(costEstimationForm.structuralMembers.slab[floorCount][memberCount][1]);
            }
        }

        private void insertSlabSchedule(List<string> schedule)
        {
            SlabScheduleUserControl content2 = new SlabScheduleUserControl(this);

            content2.slabMark = schedule[0];
            content2.thickness = schedule[1];

            content2.RSASD_S = schedule[2];
            content2.RSASD_ES_T = schedule[3];
            content2.RSASD_ES_B = schedule[4];
            content2.RSASD_MS_T = schedule[5];
            content2.RSASD_MS_B = schedule[6];
            content2.RSASD_IS_T = schedule[7];
            content2.RSASD_IS_B = schedule[8];

            content2.RSALD_S = schedule[9];
            content2.RSALD_ES_T = schedule[10];
            content2.RSALD_ES_B = schedule[11];
            content2.RSALD_MS_T = schedule[12];
            content2.RSALD_MS_B = schedule[13];
            content2.RSALD_IS_T = schedule[14];
            content2.RSALD_IS_B = schedule[15];

            content2.remark = schedule[16];

            ss_UC.Add(content2);
            slab_SS_SS_Panel.Controls.Add(content2);
        }

        private void updateSlabSchedule(List<string> schedule, int key)
        {
            schedule[0] = ss_UC[key].slabMark;
            schedule[1] = ss_UC[key].thickness;

            schedule[2] = ss_UC[key].RSASD_S;
            schedule[3] = ss_UC[key].RSASD_ES_T;
            schedule[4] = ss_UC[key].RSASD_ES_B;
            schedule[5] = ss_UC[key].RSASD_MS_T;
            schedule[6] = ss_UC[key].RSASD_MS_B;
            schedule[7] = ss_UC[key].RSASD_IS_T;
            schedule[8] = ss_UC[key].RSASD_IS_B;

            schedule[9] = ss_UC[key].RSALD_S;
            schedule[10] = ss_UC[key].RSALD_ES_T;
            schedule[11] = ss_UC[key].RSALD_ES_B;
            schedule[12] = ss_UC[key].RSALD_MS_T;
            schedule[13] = ss_UC[key].RSALD_MS_B;
            schedule[14] = ss_UC[key].RSALD_IS_T;
            schedule[15] = ss_UC[key].RSALD_IS_B;

            schedule[16] = ss_UC[key].remark;
        }

        private void populateSlabMark()
        {
            foreach (SlabScheduleUserControl ss in ss_UC)
            {
                slab_SS_SM_cbx.Items.Add(ss.slabMark);
            }
        }

        private void populateSlabMark(string slabMark)
        {
            int i = 0;
            foreach (SlabScheduleUserControl ss in ss_UC)
            {
                slab_SS_SM_cbx.Items.Add(ss.slabMark);
                i++;
                if (ss.slabMark.Equals(slabMark))
                {
                    slab_SS_SM_cbx.SelectedIndex = i;
                }
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
            else
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
        }
        
        private void setRoofValues()
        {
            roof_PGR_cbx.Enabled = false;
            if (floorCount == 0)
            {
                addstruct_cbx.SelectedIndex = 6;
            }
            else
            {
                addstruct_cbx.SelectedIndex = 4;
            }
            if (costEstimationForm.structuralMembers.roof[floorCount][memberCount][0].Equals("Rafter and Purlins"))
            {
                roof_PGR_cbx.SelectedIndex = 0;
                oldStructMemName = costEstimationForm.structuralMembers.roofNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.roofNames[floorCount][memberCount];

                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount][1].Equals("Wood")) //Wood
                {
                    roof_RP_W_rb.Checked = true;
                    roof_RP_W_D_LR_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][2];
                    roof_RP_W_D_LP_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][3];
                    roof_RP_W_D_SR_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][4];
                    roof_RP_W_D_SP_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][5];
                }
                else if (costEstimationForm.structuralMembers.roof[floorCount][memberCount][1].Equals("Steel - Tubular"))
                {
                    roof_RP_ST_rb.Checked = true;
                    roof_RP_ST_D_LRSW_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][2];
                    roof_RP_ST_D_LR_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][3];
                    roof_RP_ST_D_LP_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][4];
                    roof_RP_ST_D_SR_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][5];
                    roof_RP_ST_D_SP_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][6];
                    roof_RP_ST_D_CLTSR_cbx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][7];
                    roof_RP_ST_D_CLTSP_cbx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][8];
                }
                else
                {
                    roof_RP_SCP_rb.Checked = true;
                    roof_RP_SCP_D_LRSW_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][2];
                    roof_RP_SCP_D_LR_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][3];
                    roof_RP_SCP_D_LP_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][4];
                    roof_RP_SCP_D_SR_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][5];
                    roof_RP_SCP_D_SP_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][6];
                    roof_RP_SCP_D_CLCPR_cbx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][7];
                    roof_RP_SCP_D_CLCPP_cbx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][8];
                }
                roof_RP_W_rb.Enabled = false;
                roof_RP_ST_rb.Enabled = false;
                roof_RP_SCP_rb.Enabled = false;
            }
            else if (costEstimationForm.structuralMembers.roof[floorCount][memberCount][0].Equals("G.I Roof and Its Accessories"))
            {
                roof_PGR_cbx.SelectedIndex = 1;
                oldStructMemName = costEstimationForm.structuralMembers.roofNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.roofNames[floorCount][memberCount];

                roof_GI_D_LP_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][1];
                roof_GI_D_EC_cbx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][2];

                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount].Contains("Corrugated G.I Sheet"))
                    roof_GI_M_CGIS_cb.Checked = true;
                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount].Contains("G.I Roof Nails"))
                    roof_GI_M_GIRN_cb.Checked = true;
                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount].Contains("G.I Rivets"))
                    roof_GI_M_GIR_cb.Checked = true;
                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount].Contains("G.I Washers"))
                    roof_GI_M_GIW_cb.Checked = true;
                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount].Contains("Lead Washers"))
                    roof_GI_M_LW_cb.Checked = true;
                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount].Contains("Umbrella Nails"))
                    roof_GI_M_UN_cb.Checked = true;
                if (costEstimationForm.structuralMembers.roof[floorCount][memberCount].Contains("Plain G.I Strap"))
                {
                    roof_GI_M_PGIS_cb.Checked = true;
                    int count = costEstimationForm.structuralMembers.roof[floorCount][memberCount].Count - 1;
                    roof_GI_M_SP_cbx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][count];
                }

                foreach (string value in costEstimationForm.structuralMembers.roofHRS[floorCount][memberCount])
                {
                    RoofHRSUserControl content2 = new RoofHRSUserControl();
                    content2.value = value;
                    rHRS_UC.Add(content2);
                    roof_GI_D_HRS_Panel.Controls.Add(content2);
                }
            }
            else
            {
                roof_PGR_cbx.SelectedIndex = 2;
                oldStructMemName = costEstimationForm.structuralMembers.roofNames[floorCount][memberCount];
                addstruct_Name_bx.Text = costEstimationForm.structuralMembers.roofNames[floorCount][memberCount];

                roof_RA_D_G_TL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][1];
                roof_RA_D_G_EL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][2];
                roof_RA_D_G_TW_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][3];
                
                roof_RA_D_F_TL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][4];
                roof_RA_D_F_EL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][5];
                roof_RA_D_F_TW_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][6];
                
                roof_RA_D_RR_TL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][7];
                roof_RA_D_RR_EL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][8];
                roof_RA_D_RR_TW_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][9];
                
                roof_RA_D_VR_TL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][10];
                roof_RA_D_VR_EL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][11];
                roof_RA_D_VR_TW_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][12];

                roof_RA_D_HR_TL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][13];
                roof_RA_D_HR_EL_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][14];
                roof_RA_D_HR_TW_bx.Text = costEstimationForm.structuralMembers.roof[floorCount][memberCount][15];
            }
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.INDIGO);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.ORANGE);
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.INDIGO);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.ORANGE);
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
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


        private void beam_BR_AddBtn_Click(object sender, EventArgs e)
        {
            BeamRowUserControl content = new BeamRowUserControl(this, "None");
            br_UC.Add(content);
            beam_BR_Panel.Controls.Add(content);
        }

        private void beam_BS_AddBtn_Click(object sender, EventArgs e)
        {
            string name = "";
            if(floorCount == 0) // Ground Floor
            {
                if(beam_BT_cbx.SelectedIndex == 0) // Footing Tie Beam
                {
                    BeamScheduleUserControl content = new BeamScheduleUserControl(this, "Footing Tie Beam");
                    bs_UC.Add(content);
                    name = content.name;
                    beam_BS_Panel.Controls.Add(content);
                }
                else if (beam_BT_cbx.SelectedIndex == 1) // Grade Beam
                {
                    BeamScheduleUserControl content = new BeamScheduleUserControl(this, "Grade Beam");
                    bs_UC.Add(content);
                    name = content.name;
                    beam_BS_Panel.Controls.Add(content);
                }
                else // Roof Beam
                {
                    BeamScheduleUserControl content = new BeamScheduleUserControl(this, "Roof Beam");
                    bs_UC.Add(content);
                    name = content.name;
                    beam_BS_Panel.Controls.Add(content);
                }
            }
            else // Upper Floors
            {
                if (beam_BT_cbx.SelectedIndex == 0) // Suspended Beam
                {
                    BeamScheduleUserControl content = new BeamScheduleUserControl(this, "Suspended Beam");
                    bs_UC.Add(content);
                    name = content.name;
                    beam_BS_Panel.Controls.Add(content);
                }
                else // Roof Beam
                {
                    BeamScheduleUserControl content = new BeamScheduleUserControl(this, "Roof Beam");
                    bs_UC.Add(content);
                    name = content.name;
                    beam_BS_Panel.Controls.Add(content);
                }
            }

            foreach(BeamRowUserControl br in br_UC)
            {
                br.addScheduleName(name);
            }
        }

        private void slab_SS_SS_AddBtn_Click(object sender, EventArgs e)
        {
            SlabScheduleUserControl content = new SlabScheduleUserControl(this);
            ss_UC.Add(content);
            slab_SS_SS_Panel.Controls.Add(content);
            slab_SS_SM_cbx.Items.Add(content.slabMark);
        }

        private void slab_SS_SS_DelBtn_Click(object sender, EventArgs e)
        {
            if(ss_UC.Count > 0)
            {
                slab_SS_SM_cbx.Items.RemoveAt(ss_UC.Count);
                slab_SS_SM_cbx.SelectedIndex = ss_UC.Count - 1;

                slab_SS_SS_Panel.Controls.RemoveAt(ss_UC.Count);
                ss_UC.RemoveAt(ss_UC.Count - 1);
            }
        }
        private void slab_SS_SD_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(slab_SS_SD_cbx.SelectedIndex == 0)
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.SLAB_DETAIL);
                Image picture = (Image)bmp;
                slab_SS_SD_pb.Image = picture;

                SlabDetail2UserControl content = new SlabDetail2UserControl();
                sd1_UC = content;
                slab_SS_SD_Panel.Controls.Clear();
                slab_SS_SD_Panel.Controls.Add(content);
            }
            else
            {
                var bmp = new Bitmap(WindowsFormsApp1.Properties.Resources.SLAB_DETAIL_2);
                Image picture = (Image)bmp;
                slab_SS_SD_pb.Image = picture;

                SlabDetail1UserControl content = new SlabDetail1UserControl();
                sd2_UC = content;
                slab_SS_SD_Panel.Controls.Clear();
                slab_SS_SD_Panel.Controls.Add(content);
            }
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

        private void slab_SOG_SB_T_BR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(slab_SOG_SB_T_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SOG_SB_T_BR_cbx.Text;
                int index = 0;
                slab_SOG_SB_T_AtB_cbx.Items.Clear();
                slab_SOG_SB_T_AtB_cbx.Items.Add("None");
                foreach(string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                {
                    if (name.Equals(beamName))
                    {
                        foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                        {
                            int count = 1;
                            for (int i = 0; i < slab_SOG_SB_T_AtB_cbx.Items.Count; i++)
                            {
                                string value = slab_SOG_SB_T_AtB_cbx.GetItemText(slab_SOG_SB_T_AtB_cbx.Items[i]);
                                if (value.Equals(beamRows[0] + " (" + count + ")"))
                                {
                                    count++;
                                }
                            }
                            slab_SOG_SB_T_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                        }
                    }
                    index++;
                }
                slab_SOG_SB_T_AtB_cbx.SelectedIndex = 0;
            }
        }

        private void slab_SOG_SB_B_BR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slab_SOG_SB_B_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SOG_SB_B_BR_cbx.Text;
                int index = 0;
                slab_SOG_SB_B_AtB_cbx.Items.Clear();
                slab_SOG_SB_B_AtB_cbx.Items.Add("None");
                foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                {
                    if (name.Equals(beamName))
                    {
                        foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                        {
                            int count = 1;
                            for (int i = 0; i < slab_SOG_SB_B_AtB_cbx.Items.Count; i++)
                            {
                                string value = slab_SOG_SB_B_AtB_cbx.GetItemText(slab_SOG_SB_B_AtB_cbx.Items[i]);
                                if (value.Equals(beamRows[0] + " (" + count + ")"))
                                {
                                    count++;
                                }
                            }
                            slab_SOG_SB_B_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                        }
                    }
                    index++;
                }
                slab_SOG_SB_B_AtB_cbx.SelectedIndex = 0;
            }
        }

        private void slab_SOG_SB_L_BR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slab_SOG_SB_L_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SOG_SB_L_BR_cbx.Text;
                int index = 0;
                slab_SOG_SB_L_AtB_cbx.Items.Clear();
                slab_SOG_SB_L_AtB_cbx.Items.Add("None");
                foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                {
                    if (name.Equals(beamName))
                    {
                        foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                        {
                            int count = 1;
                            for (int i = 0; i < slab_SOG_SB_L_AtB_cbx.Items.Count; i++)
                            {
                                string value = slab_SOG_SB_L_AtB_cbx.GetItemText(slab_SOG_SB_L_AtB_cbx.Items[i]);
                                if (value.Equals(beamRows[0] + " (" + count + ")"))
                                {
                                    count++;
                                }
                            }
                            slab_SOG_SB_L_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                        }
                    }
                    index++;
                }
                slab_SOG_SB_L_AtB_cbx.SelectedIndex = 0;
            }
        }

        private void slab_SOG_SB_R_BR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slab_SOG_SB_R_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SOG_SB_R_BR_cbx.Text;
                int index = 0;
                slab_SOG_SB_R_AtB_cbx.Items.Clear();
                slab_SOG_SB_R_AtB_cbx.Items.Add("None");
                foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                {
                    if (name.Equals(beamName))
                    {
                        foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                        {
                            int count = 1;
                            for (int i = 0; i < slab_SOG_SB_R_AtB_cbx.Items.Count; i++)
                            {
                                string value = slab_SOG_SB_R_AtB_cbx.GetItemText(slab_SOG_SB_R_AtB_cbx.Items[i]);
                                if (value.Equals(beamRows[0] + " (" + count + ")"))
                                {
                                    count++;
                                }
                            }
                            slab_SOG_SB_R_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                        }
                    }
                    index++;
                }
                slab_SOG_SB_R_AtB_cbx.SelectedIndex = 0;
            }
        }

        private void slab_SS_SB_T_BR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slab_SS_SB_T_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SS_SB_T_BR_cbx.Text;
                bool isSuspended = beamName.Contains("(Suspended Beam)");
                if (isSuspended)
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Suspended Beam)"));
                    int index = 0;
                    slab_SS_SB_T_AtB_cbx.Items.Clear();
                    slab_SS_SB_T_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_T_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_T_AtB_cbx.GetItemText(slab_SS_SB_T_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_T_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_T_AtB_cbx.SelectedIndex = 0;
                }
                else
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Roof Beam)"));
                    int index = 0;
                    slab_SS_SB_T_AtB_cbx.Items.Clear();
                    slab_SS_SB_T_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount - 1])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount - 1][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_T_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_T_AtB_cbx.GetItemText(slab_SS_SB_T_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_T_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_T_AtB_cbx.SelectedIndex = 0;
                }
            }
        }

        private void slab_SS_SB_B_AtB_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slab_SS_SB_B_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SS_SB_B_BR_cbx.Text;
                bool isSuspended = beamName.Contains("(Suspended Beam)");
                if (isSuspended)
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Suspended Beam)"));
                    int index = 0;
                    slab_SS_SB_B_AtB_cbx.Items.Clear();
                    slab_SS_SB_B_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_B_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_B_AtB_cbx.GetItemText(slab_SS_SB_B_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_B_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_B_AtB_cbx.SelectedIndex = 0;
                }
                else
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Roof Beam)"));
                    int index = 0;
                    slab_SS_SB_B_AtB_cbx.Items.Clear();
                    slab_SS_SB_B_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount - 1])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount - 1][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_B_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_B_AtB_cbx.GetItemText(slab_SS_SB_B_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_B_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_B_AtB_cbx.SelectedIndex = 0;
                }
            }
        }

        private void slab_SS_SB_L_AtB_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slab_SS_SB_L_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SS_SB_L_BR_cbx.Text;
                bool isSuspended = beamName.Contains("(Suspended Beam)");
                if (isSuspended)
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Suspended Beam)"));
                    int index = 0;
                    slab_SS_SB_L_AtB_cbx.Items.Clear();
                    slab_SS_SB_L_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_L_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_L_AtB_cbx.GetItemText(slab_SS_SB_L_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_L_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_L_AtB_cbx.SelectedIndex = 0;
                }
                else
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Roof Beam)"));
                    int index = 0;
                    slab_SS_SB_L_AtB_cbx.Items.Clear();
                    slab_SS_SB_L_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount - 1])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount - 1][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_L_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_L_AtB_cbx.GetItemText(slab_SS_SB_L_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_L_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_L_AtB_cbx.SelectedIndex = 0;
                }
            }
        }

        private void slab_SS_SB_R_BR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slab_SS_SB_R_BR_cbx.SelectedIndex != 0)
            {
                string beamName = slab_SS_SB_R_BR_cbx.Text;
                bool isSuspended = beamName.Contains("(Suspended Beam)");
                if (isSuspended)
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Suspended Beam)"));
                    int index = 0;
                    slab_SS_SB_R_AtB_cbx.Items.Clear();
                    slab_SS_SB_R_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_R_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_R_AtB_cbx.GetItemText(slab_SS_SB_R_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_R_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_R_AtB_cbx.SelectedIndex = 0;
                }
                else
                {
                    beamName = beamName.Substring(0, beamName.IndexOf("(Roof Beam)"));
                    int index = 0;
                    slab_SS_SB_R_AtB_cbx.Items.Clear();
                    slab_SS_SB_R_AtB_cbx.Items.Add("None");
                    foreach (string name in costEstimationForm.structuralMembers.beamNames[floorCount - 1])
                    {
                        if (name.Equals(beamName))
                        {
                            foreach (List<string> beamRows in costEstimationForm.structuralMembers.beamRow[floorCount - 1][index])
                            {
                                int count = 1;
                                for (int j = 0; j < slab_SS_SB_R_AtB_cbx.Items.Count; j++)
                                {
                                    string value = slab_SS_SB_R_AtB_cbx.GetItemText(slab_SS_SB_R_AtB_cbx.Items[j]);
                                    if (value.Equals(beamRows[0] + " (" + count + ")"))
                                    {
                                        count++;
                                    }
                                }
                                slab_SS_SB_R_AtB_cbx.Items.Add(beamRows[0] + " (" + count + ")");
                            }
                        }
                        index++;
                    }
                    slab_SS_SB_R_AtB_cbx.SelectedIndex = 0;
                }
            }
        }

        private void roof_GI_M_PGIS_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (roof_GI_M_PGIS_cb.Checked)
            {
                roof_GI_M_SP_cbx.Enabled = true;
            }
            else
            {
                roof_GI_M_SP_cbx.Enabled = false;
            }
        }

        private void beam_BT_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            foreach (BeamRowUserControl br in br_UC)
            {
                beam_BR_Panel.Controls.Remove(br);
            }
            br_UC.Clear();
            foreach (BeamScheduleUserControl bs in bs_UC)
            {
                beam_BS_Panel.Controls.Remove(bs);
            }
            bs_UC.Clear();

            //Populate Beam Schedule according to type
            foreach (List<string> schedule in costEstimationForm.structuralMembers.beamSchedule[floorCount])
            {
                if (floorCount == 0) // Ground Floor
                {
                    if (beam_BT_cbx.SelectedIndex == 0) // Footing Tie Beam
                    {
                        if (schedule[0].Equals("Footing Tie Beam"))
                        {
                            insertBeamSchedule(schedule);
                        }
                    }
                    else if (beam_BT_cbx.SelectedIndex == 1) // Grade Beam
                    {
                        if (schedule[0].Equals("Grade Beam"))
                        {
                            insertBeamSchedule(schedule);
                        }
                    }
                    else // Roof Beam
                    {
                        if (schedule[0].Equals("Roof Beam"))
                        {
                            insertBeamSchedule(schedule);
                        }
                    }
                }
                else // Upper Floors
                {
                    if (beam_BT_cbx.SelectedIndex == 0) // Suspended Beam
                    {
                        if (schedule[0].Equals("Suspended Beam"))
                        {
                            insertBeamSchedule(schedule);
                        }
                    }
                    else // Roof Beam
                    {
                        if (schedule[0].Equals("Roof Beam"))
                        {
                            insertBeamSchedule(schedule);
                        }
                    }
                }
            }

            if (floorCount == 0) // Ground Floor
            {
                if (beam_BT_cbx.SelectedIndex == 0) // Footing Tie Beam
                {
                    beam_D_bx.Enabled = true;
                    beam_BS_lbl.Text = "Footing Tie Beams Schedule";
                }
                else if (beam_BT_cbx.SelectedIndex == 1) // Grade Beam
                {
                    beam_D_bx.Enabled = true;
                    beam_BS_lbl.Text = "Grade Beam Schedule";
                }
                else // Roof Beam
                {
                    beam_D_bx.Enabled = false;
                    beam_BS_lbl.Text = "Roof Beam Schedule";
                }
            }
            else // Upper Floors
            {
                if (beam_BT_cbx.SelectedIndex == 0) // Suspended Beam
                {
                    beam_D_bx.Enabled = false;
                    beam_BS_lbl.Text = "Suspended Beam Schedule";
                }
                else // Roof Beam
                {
                    beam_D_bx.Enabled = false;
                    beam_BS_lbl.Text = "Roof Beam Schedule";
                }
            }
        }

        private void roof_RP_ST_D_CLTSR_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel47_Paint(object sender, PaintEventArgs e)
        {

        }

        private void roof_GI_D_HRS_AddBtn_Click(object sender, EventArgs e)
        {
            RoofHRSUserControl content = new RoofHRSUserControl();
            rHRS_UC.Add(content);
            roof_GI_D_HRS_Panel.Controls.Add(content);
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.INDIGO);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.ORANGE);
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
                Image picture3 = (Image)bmp3;
                var bmp4 = new Bitmap(WindowsFormsApp1.Properties.Resources.INDIGO);
                Image picture4 = (Image)bmp4;
                var bmp5 = new Bitmap(WindowsFormsApp1.Properties.Resources.ORANGE);
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
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
                var bmp2 = new Bitmap(WindowsFormsApp1.Properties.Resources.RED);
                Image picture2 = (Image)bmp2;
                var bmp3 = new Bitmap(WindowsFormsApp1.Properties.Resources.GREEN);
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
            //
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

        public void updateSlabMark(string oldName, string newName)
        {
            int selectedIndex = slab_SS_SM_cbx.SelectedIndex;
            int index = slab_SS_SM_cbx.FindString(oldName);
            slab_SS_SM_cbx.Items.RemoveAt(index);
            slab_SS_SM_cbx.Items.Insert(index, newName);
            slab_SS_SM_cbx.SelectedIndex = selectedIndex;
        }

        int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0, temp = 0;
            foreach (var obj in myCombo.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), myCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            return maxWidth;
        }
    }
}
