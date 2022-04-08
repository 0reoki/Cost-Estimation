using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ParametersForm : Form
    {
        //Forms
        ManageElevForm mef;
        Parameters parameters;
        CostEstimationForm costEstimationForm;

        //Local Variables
        private List<PaintAreaUserControl> paUC;
        private List<TileAreaUserControl> taUC;
        private List<CHBUserControl> chbUC;
        private List<ManPowerUserControl> mpUC;
        private List<EquipmentUserControl> eqUC;
        private List<CustomItemsUserControl> ciUC;
        private Size form, tabs, panel;

        //Passed Variables / Getters and Setters
        public List<PaintAreaUserControl> PaUC { get => paUC; set => paUC = value; }
        public List<TileAreaUserControl> TaUC { get => taUC; set => taUC = value; }
        public List<CHBUserControl> ChbUC { get => chbUC; set => chbUC = value; }
        public List<ManPowerUserControl> MpUC { get => mpUC; set => mpUC = value; }
        public List<EquipmentUserControl> EqUC { get => eqUC; set => eqUC = value; }
        public List<CustomItemsUserControl> CiUC { get => ciUC; set => ciUC = value; }

        //Parameters General function -- START
        public ParametersForm(Parameters parameters, CostEstimationForm costEstimationForm)
        {
            InitializeComponent();

            //Setup forms
            this.parameters = parameters;

            //Initialize User Controls
            mef = new ManageElevForm();
            PaUC = new List<PaintAreaUserControl>();
            TaUC = new List<TileAreaUserControl>();
            ChbUC = new List<CHBUserControl>();
            MpUC = new List<ManPowerUserControl>();
            EqUC = new List<EquipmentUserControl>();
            CiUC = new List<CustomItemsUserControl>();

            //Save file?
            if (costEstimationForm.saveFileExists)
            {
                SetDefaultValuesFromFile();
            }
            else
            {
                SetDefaultValues();
            }

            //Combo Box Initialize for better Item Viewing 
            conc_CM_F_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_F_RM_cbx);
            conc_CM_C_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_C_RM_cbx);
            conc_CM_B_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_B_RM_cbx);
            conc_CM_S_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_S_RM_cbx);
            conc_CM_ST_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_ST_RM_cbx);

            //Init datagridview for Reinforcements
            /*
            var deleteButton = new DataGridViewButtonColumn();
            deleteButton.Name = "dataGridViewDeleteButton";
            deleteButton.HeaderText = "Delete";
            deleteButton.Text = "Delete";
            deleteButton.UseColumnTextForButtonValue = true;
            this.rein_LSL_TB_dg.Columns.Add(deleteButton);
            */
            //For Compression Bars

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void ParametersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < PaUC.Count; i++)
            {
                if (PaUC[i].set_paUC_Area_bx.Equals(""))
                {
                    MessageBox.Show("Please fill all the fields or delete empty fields on Paint.");
                    e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                    return;
                }
            }
            for (int i = 0; i < TaUC.Count; i++)
            {
                if (TaUC[i].set_taUC_bx.Equals(""))
                {
                    MessageBox.Show("Please fill all the fields or delete empty fields on Tiles.");
                    e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                    return;
                }
            }
            for (int i = 0; i < ChbUC.Count; i++)
            {
                if (ChbUC[i].height_Bx.Equals("") || ChbUC[i].length_Bx.Equals(""))
                {
                    MessageBox.Show("Please fill all the fields or delete empty fields on Masonry.");
                    e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                    return;
                }
            }
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to save these parameters?", "Save Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                saveEveryParameters();
            }
            else if (dialogResult == DialogResult.No)
            {
                e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            }
        }
        //Parameters General function -- END

        //Earthwork Functions -- START
        private void earth_ElevBtn_Click(object sender, EventArgs e)
        {
            mef.ShowDialog();
        }

        private void earth_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setEarthworkDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Earthwork Functions -- END

        //Formwork Functions -- START
        private void form_F_T_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            string value = form_F_T_cbx.Text;
            if (value.Equals("Plywood"))
            {
                form_F_NU_bx.Text = "2";
            }
            else if (value.Equals("Phenolic Board"))
            {
                form_F_NU_bx.Text = "5";
            }
            else
            {
                form_F_NU_bx.Text = "12";
            }
        }

        private void form_resetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setFormworkDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Formwork Functions -- END

        //Concrete Functions -- START
        private void conc_CM_rb_CheckedChanged(object sender, EventArgs e)
        {
            //Footings
            if (conc_CM_F_CG_rb.Checked)
            {
                conc_CM_F_CG_cbx.Enabled = true;
                conc_CM_F_GT_cbx.Enabled = true;
                conc_CM_F_RM_cbx.Enabled = false;
            } 
            else
            {
                conc_CM_F_CG_cbx.Enabled = false;
                conc_CM_F_GT_cbx.Enabled = false;
                conc_CM_F_RM_cbx.Enabled = true;
            }

            //Columns
            if (conc_CM_C_CG_rb.Checked)
            {
                conc_CM_C_CG_cbx.Enabled = true;
                conc_CM_C_GT_cbx.Enabled = true;
                conc_CM_C_RM_cbx.Enabled = false;
            }
            else
            {
                conc_CM_C_CG_cbx.Enabled = false;
                conc_CM_C_GT_cbx.Enabled = false;

                conc_CM_C_RM_cbx.Enabled = true;
            }

            //Beams
            if (conc_CM_B_CG_rb.Checked)
            {
                conc_CM_B_CG_cbx.Enabled = true;
                conc_CM_B_GT_cbx.Enabled = true;
                conc_CM_B_RM_cbx.Enabled = false;
            }
            else
            {
                conc_CM_B_CG_cbx.Enabled = false;
                conc_CM_B_GT_cbx.Enabled = false;
                conc_CM_B_RM_cbx.Enabled = true;
            }

            //Slabs
            if (conc_CM_S_CG_rb.Checked)
            {
                conc_CM_S_CG_cbx.Enabled = true;
                conc_CM_S_GT_cbx.Enabled = true;
                conc_CM_S_RM_cbx.Enabled = false;
            }
            else
            {
                conc_CM_S_CG_cbx.Enabled = false;
                conc_CM_S_GT_cbx.Enabled = false;
                conc_CM_S_RM_cbx.Enabled = true;
            }

            //Stairs
            if (conc_CM_ST_CG_rb.Checked)
            {
                conc_CM_ST_CG_cbx.Enabled = true;
                conc_CM_ST_GT_cbx.Enabled = true;
                conc_CM_ST_RM_cbx.Enabled = false;
            }
            else
            {
                conc_CM_ST_CG_cbx.Enabled = false;
                conc_CM_ST_GT_cbx.Enabled = false;
                conc_CM_ST_RM_cbx.Enabled = true;
            }
        }

        private void conc_CM_F_RM_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.conc_CM_F_RM_cbx, conc_CM_F_RM_cbx.Items[conc_CM_F_RM_cbx.SelectedIndex].ToString());
            toolTip1.Show(conc_CM_F_RM_cbx.SelectedItem.ToString(), conc_CM_F_RM_cbx);
        }

        private void conc_CM_C_RM_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(conc_CM_C_RM_cbx.SelectedItem.ToString(), conc_CM_C_RM_cbx);
        }

        private void conc_CM_B_RM_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(conc_CM_B_RM_cbx.SelectedItem.ToString(), conc_CM_B_RM_cbx);
        }

        private void conc_CM_S_RM_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(conc_CM_S_RM_cbx.SelectedItem.ToString(), conc_CM_S_RM_cbx);
        }

        private void conc_CM_ST_RM_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(conc_CM_ST_RM_cbx.SelectedItem.ToString(), conc_CM_ST_RM_cbx);
        }

        private void conc_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setConcreteDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Concrete Functions -- END

        //Reinforcement Functions -- START

        private void rein_LSL_TB_addBtn_Click(object sender, EventArgs e)
        {
            if(rein_LSL_tabControl.SelectedIndex == 0)
            {
                //Tension Bars
                //TODO: combobox if delete or add
                //if delete, remove last row

                //add
                //TODO: combobox if column or row
                //if column

                //if row
                this.rein_LSL_TB_dg.Columns.Add("Tension Bars", "Bar Sizes");
                this.rein_LSL_TB_dg.Columns.Add("Tension Bars", "Lapped Splice 1");
                rein_LSL_TB_dg.Columns[1].Width = 90;
                LSLBarsUserControl lol = new LSLBarsUserControl();
                rein_LSL_TB_fc_Panel.Controls.Add(lol);
                this.rein_LSL_TB_dg.Rows.Add(" ", " ");
            }
            else
            {
                //Compression Bars
                
            }
        }

        private void rein_LSL_TB_dg_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == rein_LSL_TB_dg.NewRowIndex || e.RowIndex < 0)
                return;

        }
        //Rreinforcement Functions -- END

        //Paint Functions -- START
        private void paint_PA_AddBtn_Click(object sender, EventArgs e)
        {
            PaintAreaUserControl content = new PaintAreaUserControl(this);
            PaUC.Add(content);
            //Default Values
            content.set_paUC_Paint_cbx = "Enamel";
            content.set_paUC_PL_bx = "2";
            paint_PA_Panel.Controls.Add(content);
        }

        public void refreshPaint()
        {
            //Remove all controls
             paint_PA_Panel.Controls.Clear();
            
            //Add all controls
            for (int i = 0; i < PaUC.Count; i++)
            {
                PaUC[i].setLabel = "Paint Area " + (i + 1);
                paint_PA_Panel.Controls.Add(PaUC[i]);
            }
        }
        public void clearPaint()
        {
            paint_PA_Panel.Controls.Clear();
        }

        private void paint_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setPaintDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Paint Functions -- END

        //Tiles -- START
        private void tiles_TA_AddBtn_Click(object sender, EventArgs e)
        {
            TileAreaUserControl content = new TileAreaUserControl(this);
            TaUC.Add(content);
            //Default Values
            content.set_tdUC_cbx = "600 x 600";
            content.set_taUC_cbx = "Regular";
            tiles_TA_Panel.Controls.Add(content);
        }

        public void refreshTiles()
        {
            //Remove all controls
            tiles_TA_Panel.Controls.Clear();

            //Add all controls
            for (int i = 0; i < TaUC.Count; i++)
            {
                TaUC[i].setLabel = "Tile Area " + (i + 1);
                tiles_TA_Panel.Controls.Add(TaUC[i]);
            }
        }
        public void clearTiles()
        {
            tiles_TA_Panel.Controls.Clear();
        }

        private void tiles_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setTilesDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Tiles -- END

        //Masonry -- START
        private void mason_CHB_EW_AddBtn_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>() { "Wall", "Window", "Door" };
            string dlgName = "Exterior Wall";
            DialogRadioBox dlg = new DialogRadioBox(dlgName, list);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int wallCount = 1, windowCount = 1, doorCount = 1;
                for (int i = 0; i < ChbUC.Count; i++)
                {
                    if (ChbUC[i].wallType.Equals("Exterior"))
                    {
                        if (ChbUC[i].chbType.Equals("Wall")) wallCount++;
                        else if (ChbUC[i].chbType.Equals("Window")) windowCount++;
                        else doorCount++;
                    }
                }
                if (dlg.selectedString.Equals("Wall"))
                {
                    //Add user control for wall
                    int index = -1;
                    index = ChbUC.FindIndex(x => x.chbType == "Window");
                    if (index == -1)
                    {
                        index = ChbUC.FindIndex(x => x.chbType == "Door");
                    }
                    CHBUserControl content = new CHBUserControl(this);
                    content.height_Lbl = "Height of Wall " + wallCount;
                    content.length_Lbl = "Length of Wall " + wallCount;
                    content.height_Bx = ""; 
                    content.length_Bx = ""; 
                    content.wallType = "Exterior";
                    content.chbType = "Wall";
                    if(index == -1)
                    {
                        ChbUC.Add(content);
                    }
                    else
                    {
                        ChbUC.Insert(index, content);
                    }
                    mason_CHB_EW_Panel.Controls.Add(content);
                }
                else if (dlg.selectedString.Equals("Window"))
                {
                    //Add user control for window
                    int index = -1;
                    index = ChbUC.FindIndex(x => x.chbType == "Door");
                    CHBUserControl content = new CHBUserControl(this);
                    content.height_Lbl = "Height of Window " + windowCount;
                    content.length_Lbl = "Length of Window " + windowCount;
                    content.height_Bx = ""; 
                    content.length_Bx = "";
                    content.wallType = "Exterior";
                    content.chbType = "Window";
                    if (index == -1)
                    {
                        ChbUC.Add(content);
                    }
                    else
                    {
                        ChbUC.Insert(index, content);
                    }
                    mason_CHB_EW_Panel.Controls.Add(content);
                }
                else
                {
                    //Add user control for door
                    CHBUserControl content = new CHBUserControl(this);
                    content.height_Lbl = "Height of Door " + doorCount;
                    content.length_Lbl = "Length of Door " + doorCount;
                    content.height_Bx = ""; 
                    content.length_Bx = "";
                    content.wallType = "Exterior";
                    content.chbType = "Door";
                    ChbUC.Add(content);
                    mason_CHB_EW_Panel.Controls.Add(content);
                }
                refreshCHB();
            }
        }

        private void mason_CHB_IW_AddBtn_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>() { "Wall", "Window", "Door" };
            string dlgName = "Interior Wall";
            DialogRadioBox dlg = new DialogRadioBox(dlgName, list);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int wallCount = 1, windowCount = 1, doorCount = 1;
                for (int i = 0; i < ChbUC.Count; i++)
                {
                    if (ChbUC[i].wallType.Equals("Interior"))
                    {
                        if (ChbUC[i].chbType.Equals("Wall")) wallCount++;
                        else if (ChbUC[i].chbType.Equals("Window")) windowCount++;
                        else doorCount++;
                    }
                }

                if (dlg.selectedString.Equals("Wall"))
                {
                    //Add user control for wall
                    int index = -1;
                    index = ChbUC.FindIndex(x => x.chbType == "Window");
                    if (index == -1)
                    {
                        index = ChbUC.FindIndex(x => x.chbType == "Door");
                    }
                    CHBUserControl content = new CHBUserControl(this);
                    content.height_Lbl = "Height of Wall " + wallCount;
                    content.length_Lbl = "Length of Wall " + wallCount;
                    content.height_Bx = ""; 
                    content.length_Bx = ""; 
                    content.wallType = "Interior";
                    content.chbType = "Wall";
                    if (index == -1)
                    {
                        ChbUC.Add(content);
                    }
                    else
                    {
                        ChbUC.Insert(index, content);
                    }
                    mason_CHB_IW_Panel.Controls.Add(content);
                }
                else if (dlg.selectedString.Equals("Window"))
                {
                    //Add user control for window
                    int index = -1;
                    index = ChbUC.FindIndex(x => x.chbType == "Door");
                    CHBUserControl content = new CHBUserControl(this);
                    content.height_Lbl = "Height of Window " + windowCount;
                    content.length_Lbl = "Length of Window " + windowCount;
                    content.height_Bx = ""; 
                    content.length_Bx = ""; 
                    content.wallType = "Interior";
                    content.chbType = "Window";
                    if (index == -1)
                    {
                        ChbUC.Add(content);
                    }
                    else
                    {
                        ChbUC.Insert(index, content);
                    }
                    mason_CHB_IW_Panel.Controls.Add(content);
                }
                else
                {
                    //Add user control for door
                    CHBUserControl content = new CHBUserControl(this);
                    content.height_Lbl = "Height of Door " + doorCount;
                    content.length_Lbl = "Length of Door " + doorCount;
                    content.height_Bx = ""; 
                    content.length_Bx = ""; 
                    content.wallType = "Interior";
                    content.chbType = "Door";
                    ChbUC.Add(content);
                    mason_CHB_IW_Panel.Controls.Add(content);
                }
                refreshCHB();
            }
        }

        public void refreshCHB()
        {
            mason_CHB_EW_Panel.Controls.Clear();
            mason_CHB_IW_Panel.Controls.Clear();

            //Add all controls
            int wallCountE = 1, windowCountE = 1, doorCountE = 1;
            int wallCountI = 1, windowCountI = 1, doorCountI = 1;
            for (int i = 0; i < ChbUC.Count; i++)
            {
                if (ChbUC[i].wallType.Equals("Exterior"))
                {
                    if (ChbUC[i].chbType.Equals("Wall"))
                    {
                        //Add user control for wall
                        ChbUC[i].height_Lbl = "Height of Wall " + wallCountE;
                        ChbUC[i].length_Lbl = "Length of Wall " + wallCountE;
                        ChbUC[i].wallType = "Exterior";
                        ChbUC[i].chbType = "Wall";
                        mason_CHB_EW_Panel.Controls.Add(ChbUC[i]);
                        wallCountE++;
                    }
                    else if (ChbUC[i].chbType.Equals("Window"))
                    {
                        //Add user control for window
                        ChbUC[i].height_Lbl = "Height of Window " + windowCountE;
                        ChbUC[i].length_Lbl = "Length of Window " + windowCountE;
                        ChbUC[i].wallType = "Exterior";
                        ChbUC[i].chbType = "Window";
                        mason_CHB_EW_Panel.Controls.Add(ChbUC[i]);
                        windowCountE++;
                    }
                    else
                    {
                        //Add user control for door
                        ChbUC[i].height_Lbl = "Height of Door " + doorCountE;
                        ChbUC[i].length_Lbl = "Length of Door " + doorCountE;
                        ChbUC[i].wallType = "Exterior";
                        ChbUC[i].chbType = "Door";
                        mason_CHB_EW_Panel.Controls.Add(ChbUC[i]);
                        doorCountE++;
                    }
                }
                else
                {
                    if (ChbUC[i].chbType.Equals("Wall"))
                    {
                        //Add user control for wall
                        ChbUC[i].height_Lbl = "Height of Wall " + wallCountI;
                        ChbUC[i].length_Lbl = "Length of Wall " + wallCountI;
                        ChbUC[i].wallType = "Interior";
                        ChbUC[i].chbType = "Wall";
                        mason_CHB_IW_Panel.Controls.Add(ChbUC[i]);
                        wallCountI++;
                    }
                    else if (ChbUC[i].chbType.Equals("Window"))
                    {
                        //Add user control for window
                        ChbUC[i].height_Lbl = "Height of Window " + windowCountI;
                        ChbUC[i].length_Lbl = "Length of Window " + windowCountI;
                        ChbUC[i].wallType = "Interior";
                        ChbUC[i].chbType = "Window";
                        mason_CHB_IW_Panel.Controls.Add(ChbUC[i]);
                        windowCountI++;
                    }
                    else
                    {
                        //Add user control for door
                        ChbUC[i].height_Lbl = "Height of Door " + doorCountI;
                        ChbUC[i].length_Lbl = "Length of Door " + doorCountI;
                        ChbUC[i].wallType = "Interior";
                        ChbUC[i].chbType = "Door";
                        mason_CHB_IW_Panel.Controls.Add(ChbUC[i]);
                        doorCountI++;
                    }
                }                               
            }
        }

        public void clearCHB()
        {
            mason_CHB_EW_Panel.Controls.Clear();
            mason_CHB_IW_Panel.Controls.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setMasonryDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }

        private void mason_RTW_RG_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            //rebar changed
            mason_RTW_BD_cbx.Items.Clear();
            if (mason_RTW_RG_cbx.SelectedIndex == 2)
            {
                mason_RTW_BD_cbx.Items.AddRange(new string[] { "10mm", "12mm", "16mm", "20mm", "25mm", "28mm", "32mm", "36mm", "40mm", "50mm" });
                mason_RTW_BD_cbx.SelectedIndex = 1;
            }
            else
            {
                mason_RTW_BD_cbx.Items.AddRange(new string[] { "10mm", "12mm", "16mm", "20mm", "25mm" });
                mason_RTW_BD_cbx.SelectedIndex = 1;
            }
        }
        //Masonry -- END

        //Labor and Equipment Functions -- START
        private void labor_MP_AddBtn_Click(object sender, EventArgs e)
        {
            ManPowerUserControl content = new ManPowerUserControl(this);
            MpUC.Add(content);
            //Default Values
            content.set_mpUC_cbx = "Foreman";
            content.set_mpUC_qty = "1";
            labor_MP_Panel.Controls.Add(content);
        }

        private void labor_EQP_AddBtn_Click(object sender, EventArgs e)
        {
            EquipmentUserControl content = new EquipmentUserControl(this);
            EqUC.Add(content);
            //Default Values
            content.set_eqUC_cbx = "Backhoe Crawler (0.75 - 1.0 cu.m)";
            content.set_eqUC_qty = "1";
            labor_EQP_Panel.Controls.Add(content);
        }

        private void labor_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setLaborDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Labor and Equipment Functions -- END

        //Misc Functions -- START
        private void misc_AddItemsBtn_Click(object sender, EventArgs e)
        {
            CustomItemsUserControl content = new CustomItemsUserControl(this);
            CiUC.Add(content);
            //Default Values
            content.set_ciUC_cbx = "Vulca Seal";
            content.set_ciUC_qty = "3";
            misc_Panel.Controls.Add(content);
        }

        private void misc_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                setMiscDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Misc Functions -- END

        //Default Values -- START
        private void SetDefaultValues()
        {
            //Earthworks
            setEarthworkDefaultValues();

            //Formworks
            setFormworkDefaultValues();

            //Concrete
            setConcreteDefaultValues();

            //Paint
            setPaintDefaultValues();

            //Tiles
            setTilesDefaultValues();

            //Masonry
            setMasonryDefaultValues();

            //Labor and Equipment
            setLaborDefaultValues();

            //Misc
            setMiscDefaultValues();
        }

        private void SetDefaultValuesFromFile()
        {
            //Earthworks;
            earth_CF_FA_bx.Text = parameters.earth_CF_FA;
            earth_CF_TH_bx.Text = parameters.earth_CF_TH;
            earth_CF_TY_cbx.Text = parameters.earth_CF_TY;
            earth_CF_CF_bx.Text = parameters.earth_CF_CF;

            earth_WF_FA_bx.Text = parameters.earth_WF_FA;
            earth_WF_TH_bx.Text = parameters.earth_WF_TH;
            earth_WF_TY_cbx.Text = parameters.earth_WF_TY;
            earth_WF_CF_bx.Text = parameters.earth_WF_CF;

            earth_WTB_FA_bx.Text = parameters.earth_WTB_FA;
            earth_WTB_TH_bx.Text = parameters.earth_WTB_TH;
            earth_WTB_TY_cbx.Text = parameters.earth_WTB_TY;
            earth_WTB_CF_bx.Text = parameters.earth_WTB_CF;

            earth_SG_AS_bx.Text = parameters.earth_SG_AS;
            earth_SG_TS_bx.Text = parameters.earth_SG_TS;
            earth_SG_TH_bx.Text = parameters.earth_SG_TH;
            earth_SG_TY_cbx.Text = parameters.earth_SG_TY;
            earth_SG_CF_bx.Text = parameters.earth_SG_CF;
            if (parameters.earth_elevations != null)
            {
                for(int i = 0; i < parameters.earth_elevations.Count; i++)
                {
                    ManageElevUserControl content = new ManageElevUserControl(mef);
                    content.elevLabel = "Elevation " + (i + 1);
                    content.elev = parameters.earth_elevations[i][0];
                    content.elevArea = parameters.earth_elevations[i][1];
                    mef.Elevations.Add(content);
                }
                mef.refreshElevations();
            }

            //Formworks
            form_SM_F_FL_cbx.Text = parameters.form_SM_F_FL;
            form_SM_C_FL_cbx.Text = parameters.form_SM_C_FL;
            form_SM_C_VS_cbx.Text = parameters.form_SM_C_VS;
            form_SM_C_HB_cbx.Text = parameters.form_SM_C_HB;
            form_SM_B_FL_cbx.Text = parameters.form_SM_B_FL;
            form_SM_B_VS_cbx.Text = parameters.form_SM_B_VS;
            form_SM_B_HB_cbx.Text = parameters.form_SM_B_HB;
            form_SM_B_DB_cbx.Text = parameters.form_SM_B_DB;
            form_SM_HS_VS_cbx.Text = parameters.form_SM_HS_VS;
            form_SM_ST_FL_cbx.Text = parameters.form_SM_ST_FL;
            form_SM_ST_VS_cbx.Text = parameters.form_SM_ST_VS;
            form_F_T_cbx.Text = parameters.form_F_T;
            form_F_NU_bx.Text = parameters.form_F_NU;
            form_F_N_bx.Text = parameters.form_F_N;

            //Concrete
            conc_CM_F_CG_cbx.Text = parameters.conc_CM_F_CG;
            conc_CM_F_GT_cbx.Text = parameters.conc_CM_F_GT;
            conc_CM_F_RM_cbx.Text = parameters.conc_CM_F_RM;
            if(parameters.cmIsSelected[0]) { conc_CM_F_CG_rb.Select(); } else { conc_CM_F_RM_rb.Select(); }


            conc_CM_C_CG_cbx.Text = parameters.conc_CM_C_CG;
            conc_CM_C_GT_cbx.Text = parameters.conc_CM_C_GT;
            conc_CM_C_RM_cbx.Text = parameters.conc_CM_C_RM;
            if (parameters.cmIsSelected[1]) { conc_CM_C_CG_rb.Select(); } else { conc_CM_C_RM_rb.Select(); }

            conc_CM_B_CG_cbx.Text = parameters.conc_CM_B_CG;
            conc_CM_B_GT_cbx.Text = parameters.conc_CM_B_GT;
            conc_CM_B_RM_cbx.Text = parameters.conc_CM_B_RM;
            if (parameters.cmIsSelected[2]) { conc_CM_B_CG_rb.Select(); } else { conc_CM_B_RM_rb.Select(); }

            conc_CM_S_CG_cbx.Text = parameters.conc_CM_S_CG;
            conc_CM_S_GT_cbx.Text = parameters.conc_CM_S_GT;
            conc_CM_S_RM_cbx.Text = parameters.conc_CM_S_RM;
            if (parameters.cmIsSelected[3]) { conc_CM_S_CG_rb.Select(); } else { conc_CM_S_RM_rb.Select(); }

            conc_CM_W_MEW_CM_cbx.Text = parameters.conc_CM_W_MEW_CM;
            conc_CM_W_MIW_CM_cbx.Text = parameters.conc_CM_W_MIW_CM;
            conc_CM_W_P_CM_cbx.Text = parameters.conc_CM_W_P_CM;
            conc_CM_W_P_PT_cbx.Text = parameters.conc_CM_W_P_PT;

            conc_CM_ST_CG_cbx.Text = parameters.conc_CM_ST_CG;
            conc_CM_ST_GT_cbx.Text = parameters.conc_CM_ST_GT;
            conc_CM_ST_RM_cbx.Text = parameters.conc_CM_ST_RM;
            if (parameters.cmIsSelected[4]) { conc_CM_ST_CG_rb.Select(); } else { conc_CM_ST_RM_rb.Select(); }

            conc_CC_F_bx.Text = parameters.conc_CC_F;
            conc_CC_SS_bx.Text = parameters.conc_CC_SS;
            conc_CC_SG_bx.Text = parameters.conc_CC_SG;
            conc_CC_BEE_bx.Text = parameters.conc_CC_BEE;
            conc_CC_BEW_bx.Text = parameters.conc_CC_BEW;
            conc_CC_CEE_bx.Text = parameters.conc_CC_CEE;
            conc_CC_CEW_bx.Text = parameters.conc_CC_CEW;

            //Paint
            paint_SCL_bx.Text = parameters.paint_SCL;
            if (parameters.paint_Area != null)
            {
                for (int i = 0; i < parameters.paint_Area.Count; i++)
                {
                    PaintAreaUserControl content = new PaintAreaUserControl(this);
                    content.setLabel = "Paint Area " + (i + 1);
                    content.set_paUC_Area_bx = parameters.paint_Area[i][0];
                    content.set_paUC_Paint_cbx = parameters.paint_Area[i][1];
                    content.set_paUC_PL_bx = parameters.paint_Area[i][2];
                    PaUC.Add(content);
                }
                refreshPaint();
            }

            //Tiles
            tiles_FS_bx.Text = parameters.tiles_FS;
            tiles_TG_cbx.Text = parameters.tiles_TG;
            if (parameters.tiles_Area != null)
            {
                for (int i = 0; i < parameters.tiles_Area.Count; i++)
                {
                    TileAreaUserControl content = new TileAreaUserControl(this);
                    content.setLabel = "Tile Area " + (i + 1);
                    content.set_taUC_bx = parameters.tiles_Area[i][0];
                    content.set_tdUC_cbx = parameters.tiles_Area[i][1];
                    content.set_taUC_cbx = parameters.tiles_Area[i][2];
                    TaUC.Add(content);
                }
                refreshTiles();
            }

            //Masonry TODO: SetDefaultValuesFromFile
            mason_CHB_EW_cbx.Text = parameters.mason_CHB_EW;
            mason_CHB_IW_cbx.Text = parameters.mason_CHB_IW;
            //for loop
            int wallCountE = 1, windowCountE = 1, doorCountE = 1;
            int wallCountI = 1, windowCountI = 1, doorCountI = 1;
            for(int i = 0; i < parameters.mason_exteriorWall.Count; i++)
            {
                //Add user control for wall
                CHBUserControl content = new CHBUserControl(this);
                ChbUC.Add(content);
                content.height_Lbl = "Height of Wall " + wallCountE;
                content.length_Lbl = "Length of Wall " + wallCountE;
                content.height_Bx = parameters.mason_exteriorWall[i][0];
                content.length_Bx = parameters.mason_exteriorWall[i][1];
                content.wallType = "Exterior";
                content.chbType = "Wall";
                mason_CHB_EW_Panel.Controls.Add(content);
                wallCountE++;
            }
            for(int i = 0; i < parameters.mason_exteriorWindow.Count; i++)
            {
                //Add user control for window
                CHBUserControl content = new CHBUserControl(this);
                ChbUC.Add(content);
                content.height_Lbl = "Height of Window " + windowCountE;
                content.length_Lbl = "Length of Window " + windowCountE;
                content.height_Bx = parameters.mason_exteriorWindow[i][0];
                content.length_Bx = parameters.mason_exteriorWindow[i][1];
                content.wallType = "Exterior";
                content.chbType = "Window";
                mason_CHB_EW_Panel.Controls.Add(content);
                windowCountE++;
            }
            for (int i = 0; i < parameters.mason_exteriorDoor.Count; i++)
            {
                //Add user control for door
                CHBUserControl content = new CHBUserControl(this);
                ChbUC.Add(content);
                content.height_Lbl = "Height of Door " + doorCountE;
                content.length_Lbl = "Length of Door " + doorCountE;
                content.height_Bx = parameters.mason_exteriorDoor[i][0];
                content.length_Bx = parameters.mason_exteriorDoor[i][1];
                content.wallType = "Exterior";
                content.chbType = "Door";
                mason_CHB_EW_Panel.Controls.Add(content);
                doorCountE++;
            }
            for (int i = 0; i < parameters.mason_interiorWall.Count; i++)
            {
                //Add user control for wall
                CHBUserControl content = new CHBUserControl(this);
                ChbUC.Add(content);
                content.height_Lbl = "Height of Wall " + wallCountI;
                content.length_Lbl = "Length of Wall " + wallCountI;
                content.height_Bx = parameters.mason_interiorWall[i][0];
                content.length_Bx = parameters.mason_interiorWall[i][1];
                content.wallType = "Interior";
                content.chbType = "Wall";
                mason_CHB_IW_Panel.Controls.Add(content);
                wallCountI++;
            }
            for (int i = 0; i < parameters.mason_interiorWindow.Count; i++)
            {
                //Add user control for window
                CHBUserControl content = new CHBUserControl(this);
                ChbUC.Add(content);
                content.height_Lbl = "Height of Window " + windowCountI;
                content.length_Lbl = "Length of Window " + windowCountI;
                content.height_Bx = parameters.mason_interiorWindow[i][0];
                content.length_Bx = parameters.mason_interiorWindow[i][1];
                content.wallType = "Interior";
                content.chbType = "Window";
                mason_CHB_IW_Panel.Controls.Add(content);
                windowCountI++;
            }
            for (int i = 0; i < parameters.mason_interiorDoor.Count; i++)
            {
                //Add user control for door
                CHBUserControl content = new CHBUserControl(this);
                ChbUC.Add(content);
                content.height_Lbl = "Height of Door " + doorCountI;
                content.length_Lbl = "Length of Door " + doorCountI;
                content.height_Bx = parameters.mason_interiorDoor[i][0];
                content.length_Bx = parameters.mason_interiorDoor[i][1];
                content.wallType = "Interior";
                content.chbType = "Door";
                mason_CHB_IW_Panel.Controls.Add(content);
                doorCountI++;
            }
            refreshCHB();
            mason_RTW_VS_cbx.Text = parameters.mason_RTW_VS;
            mason_RTW_HSL_cbx.Text = parameters.mason_RTW_HSL;
            mason_RTW_RG_cbx.Text = parameters.mason_RTW_RG;
            mason_RTW_BD_cbx.Text = parameters.mason_RTW_BD;
            mason_RTW_RL_cbx.Text = parameters.mason_RTW_RL;
            mason_RTW_LTW_cbx.Text = parameters.mason_RTW_LTW;

            //Labor and Equipment
            labor_RD_cbx.Text = parameters.labor_RD;
            if (parameters.labor_MP != null)
            {
                for (int i = 0; i < parameters.labor_MP.Count; i++)
                {
                    ManPowerUserControl content = new ManPowerUserControl(this);
                    content.set_mpUC_cbx = parameters.labor_MP[i][0];
                    content.set_mpUC_qty = parameters.labor_MP[i][1];
                    MpUC.Add(content);
                    labor_MP_Panel.Controls.Add(MpUC[i]);
                }
            }
            if (parameters.labor_EQP != null)
            {
                for (int i = 0; i < parameters.labor_EQP.Count; i++)
                {
                    EquipmentUserControl content = new EquipmentUserControl(this);
                    content.set_eqUC_cbx = parameters.labor_EQP[i][0];
                    content.set_eqUC_qty = parameters.labor_EQP[i][1];
                    EqUC.Add(content);
                    labor_EQP_Panel.Controls.Add(EqUC[i]);
                }
            }

            //Misc
            if (parameters.misc_CustomItems != null)
            {
                for (int i = 0; i < parameters.misc_CustomItems.Count; i++)
                {
                    CustomItemsUserControl content = new CustomItemsUserControl(this);
                    content.set_ciUC_cbx = parameters.misc_CustomItems[i][0];
                    content.set_ciUC_qty = parameters.misc_CustomItems[i][1];
                    CiUC.Add(content);
                    misc_Panel.Controls.Add(CiUC[i]);
                }
            }
        }

        private void setEarthworkDefaultValues()
        {
            earth_CF_FA_bx.Text = "250";
            earth_CF_TH_bx.Text = "100";
            earth_CF_TY_cbx.Text = "G1";
            earth_CF_CF_bx.Text = "30%";

            earth_WF_FA_bx.Text = "100";
            earth_WF_TH_bx.Text = "100";
            earth_WF_TY_cbx.Text = "G1";
            earth_WF_CF_bx.Text = "30%";

            earth_WTB_FA_bx.Text = "100";
            earth_WTB_TH_bx.Text = "100";
            earth_WTB_TY_cbx.Text = "G1";
            earth_WTB_CF_bx.Text = "30%";

            earth_SG_AS_bx.Text = "75";
            earth_SG_TS_bx.Text = "100";
            earth_SG_TH_bx.Text = "100";
            earth_SG_TY_cbx.Text = "G1";
            earth_SG_CF_bx.Text = "30%";
            if(mef != null)
            {
                mef.Elevations.Clear();
                mef.clearElevations();
            }
        }


        private void setFormworkDefaultValues()
        {
            form_SM_F_FL_cbx.Text = "2\" x 2\" x (8', 10', 12')";
            form_SM_C_FL_cbx.Text = "2\" x 2\" x (8', 10', 12')";
            form_SM_C_VS_cbx.Text = "2\" x 2\" x (8', 10', 12')";
            form_SM_C_HB_cbx.Text = "2\" x 2\" x (8', 10', 12')";
            form_SM_B_FL_cbx.Text = "2\" x 3\" x (8', 10', 12')";
            form_SM_B_VS_cbx.Text = "2\" x 3\" x (8', 10', 12')";
            form_SM_B_HB_cbx.Text = "2\" x 2\" x (8', 10', 12')";
            form_SM_B_DB_cbx.Text = "2\" x 2\" x (8', 10', 12')";
            form_SM_HS_VS_cbx.Text = "2\" x 3\" x (8', 10', 12')";
            form_SM_ST_FL_cbx.Text = "2\" x 3\" x (8', 10', 12')";
            form_SM_ST_VS_cbx.Text = "2\" x 3\" x (8', 10', 12')";
            form_F_T_cbx.Text = "Plywood";
            form_F_NU_bx.Text = "2";
            form_F_N_bx.Text = "0.215 kg/1 m2";
        }

        private void setConcreteDefaultValues()
        {
            conc_CM_F_CG_cbx.Text = "CLASS AA";
            conc_CM_F_GT_cbx.Text = "G1";
            conc_CM_F_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_F_CG_rb.Select();

            conc_CM_C_CG_cbx.Text = "CLASS AA";
            conc_CM_C_GT_cbx.Text = "G1";
            conc_CM_C_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_C_CG_rb.Select();

            conc_CM_B_CG_cbx.Text = "CLASS AA";
            conc_CM_B_GT_cbx.Text = "G1";
            conc_CM_B_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_B_CG_rb.Select();

            conc_CM_S_CG_cbx.Text = "CLASS AA";
            conc_CM_S_GT_cbx.Text = "G1";
            conc_CM_S_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_S_CG_rb.Select();

            conc_CM_W_MEW_CM_cbx.Text = "CLASS A";
            conc_CM_W_MIW_CM_cbx.Text = "CLASS A";
            conc_CM_W_P_CM_cbx.Text = "CLASS A";
            conc_CM_W_P_PT_cbx.Text = "20mm";

            conc_CM_ST_CG_cbx.Text = "CLASS AA";
            conc_CM_ST_GT_cbx.Text = "G1";
            conc_CM_ST_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_ST_CG_rb.Select();

            conc_CC_F_bx.Text = "75";
            conc_CC_SS_bx.Text = "40";
            conc_CC_SG_bx.Text = "20";
            conc_CC_BEE_bx.Text = "40";
            conc_CC_BEW_bx.Text = "40";
            conc_CC_CEE_bx.Text = "40";
            conc_CC_CEW_bx.Text = "40";
        }

        private void setPaintDefaultValues()
        {
            paint_SCL_bx.Text = "2";
            if (paUC != null)
            {
                paUC.Clear();
                clearPaint();
            }
        }

        private void setTilesDefaultValues()
        {
            tiles_FS_bx.Text = "10%";
            tiles_TG_cbx.Text = "2kg";
            if (taUC != null)
            {
                taUC.Clear();
                clearTiles();
            }
        }

        private void setMasonryDefaultValues()
        {
            mason_CHB_EW_cbx.SelectedIndex = 0;
            mason_CHB_IW_cbx.SelectedIndex = 0;
            mason_RTW_VS_cbx.SelectedIndex = 0;
            mason_RTW_HSL_cbx.SelectedIndex = 0;
            mason_RTW_RG_cbx.SelectedIndex = 0;
            mason_RTW_BD_cbx.SelectedIndex = 0;
            mason_RTW_RL_cbx.SelectedIndex = 0;
            mason_RTW_LTW_cbx.SelectedIndex = 0;
            if (chbUC != null)
            {
                chbUC.Clear();
                clearCHB();
            }
        }

        private void setLaborDefaultValues()
        {
            labor_RD_cbx.Text = "Manila Rate";
            if (mpUC != null)
            {
                mpUC.Clear();
                labor_MP_Panel.Controls.Clear();
            }
            if (eqUC != null)
            {
                eqUC.Clear();
                labor_EQP_Panel.Controls.Clear();
            }
        }

        private void setMiscDefaultValues()
        {
            if (ciUC != null)
            {
                ciUC.Clear();
                misc_Panel.Controls.Clear();
            }
        }

        private void ParametersForm_Load(object sender, EventArgs e)
        {

        }

        //Default Values -- END

        //Extra Functions -- START
        private void saveEveryParameters()
        {
            //Earthworks
            List<string[]> elevations = new List<string[]>();
            for(int i = 0; i < mef.Elevations.Count; i++)
            {
                string[] toAdd = { mef.Elevations[i].elev, mef.Elevations[i].elevArea };
                elevations.Add(toAdd);
            }

            parameters.setEarthworkParameters(
                earth_CF_FA_bx.Text,
                earth_CF_TH_bx.Text,
                earth_CF_TY_cbx.SelectedItem.ToString(),
                earth_CF_CF_bx.Text,

                earth_WF_FA_bx.Text,
                earth_WF_TH_bx.Text,
                earth_WF_TY_cbx.SelectedItem.ToString(),
                earth_WF_CF_bx.Text,

                earth_WTB_FA_bx.Text,
                earth_WTB_TH_bx.Text,
                earth_WTB_TY_cbx.SelectedItem.ToString(),
                earth_WTB_CF_bx.Text,

                earth_SG_AS_bx.Text,
                earth_SG_TS_bx.Text,
                earth_SG_TH_bx.Text,
                earth_SG_TY_cbx.SelectedItem.ToString(),
                earth_SG_CF_bx.Text,
                elevations
            );

            //Formworks
            parameters.setFormworkParameters(
                form_SM_F_FL_cbx.SelectedItem.ToString(),
                form_SM_C_FL_cbx.SelectedItem.ToString(),
                form_SM_C_VS_cbx.SelectedItem.ToString(),
                form_SM_C_HB_cbx.SelectedItem.ToString(),
                form_SM_B_FL_cbx.SelectedItem.ToString(),
                form_SM_B_VS_cbx.SelectedItem.ToString(),
                form_SM_B_HB_cbx.SelectedItem.ToString(),
                form_SM_B_DB_cbx.SelectedItem.ToString(),
                form_SM_HS_VS_cbx.SelectedItem.ToString(),
                form_SM_ST_FL_cbx.SelectedItem.ToString(),
                form_SM_ST_VS_cbx.SelectedItem.ToString(),
                form_F_T_cbx.SelectedItem.ToString(),
                form_F_NU_bx.Text,
                form_F_N_bx.Text
            );

            //Concrete
            bool[] cmIsSelected = { conc_CM_F_CG_rb.Checked, conc_CM_C_CG_rb.Checked,
                    conc_CM_C_CG_rb.Checked, conc_CM_S_CG_rb.Checked, conc_CM_ST_CG_rb.Checked };
            parameters.setConcreteParameters(
                cmIsSelected,
                conc_CM_F_CG_cbx.SelectedItem.ToString(),
                conc_CM_F_GT_cbx.SelectedItem.ToString(),
                conc_CM_F_RM_cbx.SelectedItem.ToString(),

                conc_CM_C_CG_cbx.SelectedItem.ToString(),
                conc_CM_C_GT_cbx.SelectedItem.ToString(),
                conc_CM_C_RM_cbx.SelectedItem.ToString(),

                conc_CM_B_CG_cbx.SelectedItem.ToString(),
                conc_CM_B_GT_cbx.SelectedItem.ToString(),
                conc_CM_B_RM_cbx.SelectedItem.ToString(),

                conc_CM_S_CG_cbx.SelectedItem.ToString(),
                conc_CM_S_GT_cbx.SelectedItem.ToString(),
                conc_CM_S_RM_cbx.SelectedItem.ToString(),

                conc_CM_W_MEW_CM_cbx.SelectedItem.ToString(),
                conc_CM_W_MIW_CM_cbx.SelectedItem.ToString(),
                conc_CM_W_P_CM_cbx.SelectedItem.ToString(),
                conc_CM_W_P_PT_cbx.SelectedItem.ToString(),

                conc_CM_ST_CG_cbx.SelectedItem.ToString(),
                conc_CM_ST_GT_cbx.SelectedItem.ToString(),
                conc_CM_ST_RM_cbx.SelectedItem.ToString(),

                conc_CC_F_bx.Text,
                conc_CC_SS_bx.Text,

                conc_CC_SG_bx.Text,
                conc_CC_BEE_bx.Text,
                conc_CC_BEW_bx.Text,
                conc_CC_CEE_bx.Text,
                conc_CC_CEW_bx.Text
            );

            //Paint
            List<string[]> paint_Area = new List<string[]>();
            for (int i = 0; i < paUC.Count; i++)
            {
                string[] toAdd = { paUC[i].set_paUC_Area_bx, paUC[i].set_paUC_Paint_cbx, paUC[i].set_paUC_PL_bx };
                paint_Area.Add(toAdd);
            }
            parameters.setPaintParameters(paint_SCL_bx.Text, paint_Area);

            //Tiles
            List<string[]> tiles_Area = new List<string[]>();
            for (int i = 0; i < taUC.Count; i++)
            {
                string[] toAdd = { taUC[i].set_taUC_bx, taUC[i].set_tdUC_cbx, taUC[i].set_taUC_cbx };
                tiles_Area.Add(toAdd);
            }
            parameters.setTilesParameters(tiles_FS_bx.Text, tiles_TG_cbx.Text, tiles_Area
            );

            //Masonry
            List<string[]> exteriorWall = new List<string[]>();
            List<string[]> exteriorWindow = new List<string[]>();
            List<string[]> exteriorDoor = new List<string[]>();
            List<string[]> interiorWall = new List<string[]>();
            List<string[]> interiorWindow = new List<string[]>();
            List<string[]> interiorDoor = new List<string[]>();
            for (int i = 0; i < ChbUC.Count; i++)
            {
                if (ChbUC[i].wallType.Equals("Exterior"))
                {
                    if (ChbUC[i].chbType.Equals("Wall"))
                    {
                        string[] toAdd = { ChbUC[i].height_Bx, ChbUC[i].length_Bx };
                        exteriorWall.Add(toAdd);
                    }
                    else if (ChbUC[i].chbType.Equals("Window"))
                    {
                        string[] toAdd = { ChbUC[i].height_Bx, ChbUC[i].length_Bx };
                        exteriorWindow.Add(toAdd);
                    }
                    else
                    {
                        string[] toAdd = { ChbUC[i].height_Bx, ChbUC[i].length_Bx };
                        exteriorDoor.Add(toAdd);
                    }
                }
                else
                {
                    if (ChbUC[i].chbType.Equals("Wall"))
                    {
                        string[] toAdd = { ChbUC[i].height_Bx, ChbUC[i].length_Bx };
                        interiorWall.Add(toAdd);
                    }
                    else if (ChbUC[i].chbType.Equals("Window"))
                    {
                        string[] toAdd = { ChbUC[i].height_Bx, ChbUC[i].length_Bx };
                        interiorWindow.Add(toAdd);
                    }
                    else
                    {
                        string[] toAdd = { ChbUC[i].height_Bx, ChbUC[i].length_Bx };
                        interiorDoor.Add(toAdd);
                    }
                }
            }
            parameters.setMasonryParameters(
                mason_CHB_EW_cbx.Text,
                mason_CHB_IW_cbx.Text,
                exteriorWall,
                exteriorWindow,
                exteriorDoor,
                interiorWall,
                interiorWindow,
                interiorDoor,
                mason_RTW_VS_cbx.Text,
                mason_RTW_HSL_cbx.Text,
                mason_RTW_RG_cbx.Text,
                mason_RTW_BD_cbx.Text,
                mason_RTW_RL_cbx.Text,
                mason_RTW_LTW_cbx.Text
            );

            //Labor
            List<string[]> labor_MP = new List<string[]>();
            for (int i = 0; i < mpUC.Count; i++)
            {
                string[] toAdd = { mpUC[i].set_mpUC_cbx, mpUC[i].set_mpUC_qty };
                labor_MP.Add(toAdd);
            }
            List<string[]> labor_EQP = new List<string[]>();
            for (int i = 0; i < eqUC.Count; i++)
            {
                string[] toAdd = { eqUC[i].set_eqUC_cbx, eqUC[i].set_eqUC_qty };
                labor_EQP.Add(toAdd);
            }
            parameters.setLaborParameters(
                labor_RD_cbx.Text, labor_MP, labor_EQP
            );

            //Misc
            List<string[]> misc_CustomItems = new List<string[]>();
            for (int i = 0; i < ciUC.Count; i++)
            {
                string[] toAdd = { ciUC[i].set_ciUC_cbx, ciUC[i].set_ciUC_qty };
                misc_CustomItems.Add(toAdd);
            }
            parameters.setMiscParameters(misc_CustomItems);
        }

        int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0;
            int temp = 0;
            Label label1 = new Label();

            foreach (var obj in myCombo.Items)
            {
                label1.Text = obj.ToString();
                temp = label1.PreferredWidth;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            label1.Dispose();
            return maxWidth;
        }

        private void paramTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        //Extra Functions -- END
    }
}
