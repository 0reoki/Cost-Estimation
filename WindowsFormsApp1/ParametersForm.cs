﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowEst
{
    public partial class ParametersForm : Form
    {
        //Forms
        ManageElevForm mef;
        Parameters parameters;
        CostEstimationForm costEstimationForm;

        //Local Variables
        private List<LSLBarsUserControl> lslUC;
        private List<PaintAreaUserControl> paUC;
        private List<TileAreaUserControl> taUC;
        private List<CHBUserControl> chbUC;
        private List<ManPowerUserControl> mpUC;
        private List<EquipmentUserControl> eqUC;
        private List<CustomItemsUserControl> ciUC;
        DataTable rein_LSL_TB_dt;
        BindingSource rein_LSL_TB_bs;
        DataTable rein_LSL_CB_dt;
        BindingSource rein_LSL_CB_bs;
        DataTable rein_BEH_MB_dt;
        BindingSource rein_BEH_MB_bs;
        DataTable rein_BEH_ST_dt;
        BindingSource rein_BEH_ST_bs;
        DataTable rein_W_dt;
        BindingSource rein_W_bs;

        //Passed Variables / Getters and Setters
        public List<LSLBarsUserControl> LslUC { get => lslUC; set => lslUC = value; }
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
            this.costEstimationForm = costEstimationForm;

            //Initialize User Controls
            lslUC = new List<LSLBarsUserControl>();
            mef = new ManageElevForm();
            PaUC = new List<PaintAreaUserControl>();
            TaUC = new List<TileAreaUserControl>();
            ChbUC = new List<CHBUserControl>();
            MpUC = new List<ManPowerUserControl>();
            EqUC = new List<EquipmentUserControl>();
            CiUC = new List<CustomItemsUserControl>();

            //Init datagridview for Reinforcements
            rein_LSL_TB_dt = new DataTable();
            rein_LSL_TB_bs = new BindingSource();
            rein_LSL_CB_dt = new DataTable();
            rein_LSL_CB_bs = new BindingSource();
            rein_BEH_MB_dt = new DataTable();
            rein_BEH_MB_bs = new BindingSource();
            rein_BEH_ST_dt = new DataTable();
            rein_BEH_ST_bs = new BindingSource();
            rein_W_dt = new DataTable();
            rein_W_bs = new BindingSource();

            rein_LSL_TB_bs.DataSource = rein_LSL_TB_dt;
            rein_LSL_CB_bs.DataSource = rein_LSL_CB_dt;
            rein_BEH_MB_bs.DataSource = rein_BEH_MB_dt;
            rein_BEH_ST_bs.DataSource = rein_BEH_ST_dt;
            rein_W_bs.DataSource = rein_W_dt;

            rein_LSL_TB_dt.Columns.Add("Bar Sizes");
            rein_LSL_TB_dg.DataSource = rein_LSL_TB_dt;
            rein_LSL_TB_dg.Columns[0].Width = 45;

            rein_LSL_CB_dt.Columns.Add("Bar Sizes");
            rein_LSL_CB_dg.DataSource = rein_LSL_CB_dt;
            rein_LSL_CB_dg.Columns[0].Width = 45;

            rein_BEH_MB_dt.Columns.Add("Bar Size (mm)");
            rein_BEH_MB_dt.Columns.Add("L(mm) 90º");
            rein_BEH_MB_dt.Columns.Add("L(mm) 135º");
            rein_BEH_MB_dt.Columns.Add("L(mm) 180º");
            rein_BEH_MB_dg.DataSource = rein_BEH_MB_dt;
            rein_BEH_MB_dg.Columns[0].Width = 45;
            rein_BEH_MB_dg.Columns[1].Width = 90;
            rein_BEH_MB_dg.Columns[2].Width = 90;
            rein_BEH_MB_dg.Columns[3].Width = 90;
            rein_BEH_ST_dt.Columns.Add("Bar Size (mm)");
            rein_BEH_ST_dt.Columns.Add("L(mm) 90º");
            rein_BEH_ST_dt.Columns.Add("L(mm) 135º");
            rein_BEH_ST_dt.Columns.Add("L(mm) 180º");
            rein_BEH_ST_dg.DataSource = rein_BEH_ST_dt;
            rein_BEH_ST_dg.Columns[0].Width = 45;
            rein_BEH_ST_dg.Columns[1].Width = 90;
            rein_BEH_ST_dg.Columns[2].Width = 90;
            rein_BEH_ST_dg.Columns[3].Width = 90;

            rein_W_dt.Columns.Add("Bar Size (Diameter)");
            rein_W_dt.Columns.Add("kg/m");
            rein_W_dg.DataSource = rein_W_dt;
            rein_W_dg.Columns[0].Width = 45;
            rein_W_dg.Columns[1].Width = 90;
            rein_W_dt.Rows.Add("6mm", "");
            rein_W_dt.Rows.Add("8mm", "");
            rein_W_dt.Rows.Add("10mm", "");
            rein_W_dt.Rows.Add("12mm", "");
            rein_W_dt.Rows.Add("16mm", "");
            rein_W_dt.Rows.Add("20mm", "");
            rein_W_dt.Rows.Add("25mm", "");
            rein_W_dt.Rows.Add("28mm", "");
            rein_W_dt.Rows.Add("32mm", "");
            rein_W_dt.Rows.Add("36mm", "");
            rein_W_dt.Rows.Add("40mm", "");
            rein_W_dt.Rows.Add("44mm", "");
            rein_W_dt.Rows.Add("50mm", "");
            rein_W_dt.Rows.Add("56mm", "");
            rein_W_dg.DataSource = rein_W_dt;

            //Save file?
            if (costEstimationForm.saveFileExists)
            {
                SetDefaultValuesFromFile();
            }
            else
            {
                SetDefaultValues();
                saveEveryParameters();
            }

            //Combo Box Initialize for better Item Viewing 
            conc_CM_F_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_F_RM_cbx);
            conc_CM_C_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_C_RM_cbx);
            conc_CM_B_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_B_RM_cbx);
            conc_CM_S_SOG_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_S_SOG_RM_cbx);
            conc_CM_S_SS_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_S_SS_RM_cbx);
            conc_CM_ST_RM_cbx.DropDownWidth = DropDownWidth(conc_CM_ST_RM_cbx);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void ParametersForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < LslUC.Count; i++)
            {
                if (LslUC[i].lslUC_Value.Equals(""))
                {
                    MessageBox.Show("Please fill all the fields or delete empty fields on Reinforcements.");
                    e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                    return;
                }
            }
            foreach (DataGridViewRow rw in this.rein_LSL_TB_dg.Rows)
            {
                for (int i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null || rw.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(rw.Cells[i].Value.ToString()))
                    {
                        MessageBox.Show("Please fill all the fields or delete empty fields on Reinforcements.");
                        e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                        return;
                    }
                }
            }
            foreach (DataGridViewRow rw in this.rein_LSL_CB_dg.Rows)
            {
                for (int i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null || rw.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(rw.Cells[i].Value.ToString()))
                    {
                        MessageBox.Show("Please fill all the fields or delete empty fields on Reinforcements.");
                        e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                        return;
                    }
                }
            }
            foreach (DataGridViewRow rw in this.rein_BEH_MB_dg.Rows)
            {
                for (int i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null || rw.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(rw.Cells[i].Value.ToString()))
                    {
                        MessageBox.Show("Please fill all the fields or delete empty fields on Reinforcements.");
                        e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                        return;
                    }
                }
            }
            foreach (DataGridViewRow rw in this.rein_BEH_ST_dg.Rows)
            {
                for (int i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null || rw.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(rw.Cells[i].Value.ToString()))
                    {
                        MessageBox.Show("Please fill all the fields or delete empty fields on Reinforcements.");
                        e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                        return;
                    }
                }
            }
            foreach (DataGridViewRow rw in this.rein_W_dg.Rows)
            {
                for (int i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null || rw.Cells[i].Value == DBNull.Value || String.IsNullOrWhiteSpace(rw.Cells[i].Value.ToString()))
                    {
                        MessageBox.Show("Please fill all the fields or delete empty fields on Reinforcements.");
                        e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                        return;
                    }
                }
            }
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

            saveEveryParameters();

            costEstimationForm.structuralMembers.reComputeEarthworks();
            /* TODO: ADD THIS IF MAY CANCEL NA - QoL
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to save these parameters?", "Save Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                saveEveryParameters();
            }
            else if (dialogResult == DialogResult.No)
            {
                e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            }
            */
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
            if (conc_CM_S_SOG_CG_rb.Checked)
            {
                conc_CM_S_SOG_CG_cbx.Enabled = true;
                conc_CM_S_SOG_GT_cbx.Enabled = true;
                conc_CM_S_SOG_RM_cbx.Enabled = false;
            }
            else
            {
                conc_CM_S_SOG_CG_cbx.Enabled = false;
                conc_CM_S_SOG_GT_cbx.Enabled = false;
                conc_CM_S_SOG_RM_cbx.Enabled = true;
            }
            if (conc_CM_S_SS_CG_rb.Checked)
            {
                conc_CM_S_SS_CG_cbx.Enabled = true;
                conc_CM_S_SS_GT_cbx.Enabled = true;
                conc_CM_S_SS_RM_cbx.Enabled = false;
            }
            else
            {
                conc_CM_S_SS_CG_cbx.Enabled = false;
                conc_CM_S_SS_GT_cbx.Enabled = false;
                conc_CM_S_SS_RM_cbx.Enabled = true;
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
            toolTip1.Show(conc_CM_S_SOG_RM_cbx.SelectedItem.ToString(), conc_CM_S_SOG_RM_cbx);
        }

        private void conc_CM_S_SS_RM_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(conc_CM_S_SS_RM_cbx.SelectedItem.ToString(), conc_CM_S_SS_RM_cbx);
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

        //Reinforcements Functions -- START

        private void rein_LSL_TB_addBtn_Click(object sender, EventArgs e)
        {
            if(rein_LSL_tabControl.SelectedIndex == 0)
            {
                //Tension Bars
                List<string> list = new List<string>() { "Add New Row/Column", "Delete Last Row/Column" };
                string dlgName = "Tension Bars - Add/Delete";
                DialogRadioBox dlg = new DialogRadioBox(dlgName, list);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.selectedString.Equals("Add New Row/Column"))
                    {
                        List<string> list2 = new List<string>() { "Add Column", "Add Row" };
                        string dlgName2 = "Tension Bars - Add";
                        DialogRadioBox dlg2 = new DialogRadioBox(dlgName2, list2);
                        if (dlg2.ShowDialog() == DialogResult.OK)
                        {
                            if (dlg2.selectedString.Equals("Add Column"))
                            {
                                rein_LSL_TB_dt.Columns.Add("LS" + rein_LSL_TB_dt.Columns.Count + " ƒ'c (MPa)");
                                rein_LSL_TB_dg.DataSource = rein_LSL_TB_dt;
                                rein_LSL_TB_dg.Columns[rein_LSL_TB_dt.Columns.Count - 1].Width = 90;
                                LSLBarsUserControl content = new LSLBarsUserControl(this);
                                content.lslUC_Label = "LS" + (rein_LSL_TB_dt.Columns.Count - 1) + " ƒ'c (MPa):";
                                content.lslUC_Value = "";
                                content.barType = "Tension Bars";
                                LslUC.Add(content);
                                rein_LSL_TB_fc_Panel.Controls.Add(content);
                            }
                            else //Add row
                            {
                                rein_LSL_TB_dt.Rows.Add("");
                                rein_LSL_TB_dg.DataSource = rein_LSL_TB_dt;
                            }
                        }
                    } 
                    else //delete row/column
                    {
                        List<string> list2 = new List<string>() { "Delete Last Column", "Delete Last Row" };
                        string dlgName2 = "Tension Bars - Delete";
                        DialogRadioBox dlg2 = new DialogRadioBox(dlgName2, list2);
                        if (dlg2.ShowDialog() == DialogResult.OK)
                        {
                            if (dlg2.selectedString.Equals("Delete Last Column"))
                            {
                                try
                                {
                                    if (rein_LSL_TB_dt.Columns.Count == 1)
                                    {
                                        MessageBox.Show("No columns to delete!");
                                        return;
                                    } else
                                    {
                                        rein_LSL_TB_dt.Columns.RemoveAt(rein_LSL_TB_dt.Columns.Count - 1);
                                        rein_LSL_TB_dg.DataSource = rein_LSL_TB_dt;
                                        int index = 0;
                                        for (int i = 0; i < LslUC.Count; i++)
                                        {
                                            if (LslUC[i].barType.Equals("Tension Bars"))
                                            {
                                                index++;
                                            }
                                        }
                                        rein_LSL_TB_fc_Panel.Controls.RemoveAt(index - 1);
                                        LslUC.RemoveAt(index - 1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("No columns to delete!");
                                }
                            }
                            else //Delete Last Row
                            {
                                try
                                {
                                    rein_LSL_TB_dt.Rows.RemoveAt(rein_LSL_TB_dt.Rows.Count - 1);
                                    rein_LSL_TB_dg.DataSource = rein_LSL_TB_dt;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("No rows to delete!");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Compression Bars
                List<string> list = new List<string>() { "Add New Row/Column", "Delete Last Row/Column" };
                string dlgName = "Compression Bars - Add/Delete";
                DialogRadioBox dlg = new DialogRadioBox(dlgName, list);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.selectedString.Equals("Add New Row/Column"))
                    {
                        List<string> list2 = new List<string>() { "Add Column", "Add Row" };
                        string dlgName2 = "Compression Bars - Add";
                        DialogRadioBox dlg2 = new DialogRadioBox(dlgName2, list2);
                        if (dlg2.ShowDialog() == DialogResult.OK)
                        {
                            if (dlg2.selectedString.Equals("Add Column"))
                            {
                                rein_LSL_CB_dt.Columns.Add("LS" + rein_LSL_CB_dt.Columns.Count + " ƒ'c (MPa)");
                                rein_LSL_CB_dg.DataSource = rein_LSL_CB_dt;
                                rein_LSL_CB_dg.Columns[rein_LSL_CB_dt.Columns.Count - 1].Width = 90;
                                LSLBarsUserControl content = new LSLBarsUserControl(this);
                                content.lslUC_Label = "LS" + (rein_LSL_CB_dt.Columns.Count - 1) + " ƒ'c (MPa):";
                                content.lslUC_Value = "";
                                content.barType = "Compression Bars";
                                LslUC.Add(content);
                                rein_LSL_CB_fc_Panel.Controls.Add(content);
                            }
                            else //Add row 
                            {
                                rein_LSL_CB_dt.Rows.Add("");
                                rein_LSL_CB_dg.DataSource = rein_LSL_CB_dt;
                            }
                        }
                    }
                    else //delete row/column
                    {
                        List<string> list2 = new List<string>() { "Delete Last Column", "Delete Last Row" };
                        string dlgName2 = "Compression Bars - Delete";
                        DialogRadioBox dlg2 = new DialogRadioBox(dlgName2, list2);
                        if (dlg2.ShowDialog() == DialogResult.OK)
                        {
                            if (dlg2.selectedString.Equals("Delete Last Column"))
                            {
                                try
                                {
                                    if (rein_LSL_CB_dg.Columns.Count == 1)
                                    {
                                        MessageBox.Show("No columns to delete!");
                                        return;
                                    }
                                    else
                                    {
                                        rein_LSL_CB_dt.Columns.RemoveAt(rein_LSL_CB_dt.Columns.Count - 1);
                                        rein_LSL_CB_dg.DataSource = rein_LSL_CB_dt;
                                        int index = 0;
                                        for (int i = 0; i < LslUC.Count; i++)
                                        {
                                            if (LslUC[i].barType.Equals("Compression Bars"))
                                            {
                                                index++;
                                            }
                                        }
                                        rein_LSL_CB_fc_Panel.Controls.RemoveAt(index - 1);
                                        LslUC.RemoveAt(index - 1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("No columns to delete!");
                                }
                            }
                            else //Delete Last Row
                            {
                                try
                                {
                                    rein_LSL_CB_dt.Rows.RemoveAt(rein_LSL_CB_dt.Rows.Count);
                                    rein_LSL_CB_dg.DataSource = rein_LSL_CB_dt;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("No rows to delete!");
                                }
                            }
                        }
                    }
                }
            }
        }
        private void rein_BEH_addBtn_Click(object sender, EventArgs e)
        {
            if (rein_BEH_tabControl.SelectedIndex == 0)
            {
                //Main Bars
                List<string> list = new List<string>() { "Add New Row", "Delete Last Row" };
                string dlgName = "Bar End Hooks";
                DialogRadioBox dlg = new DialogRadioBox(dlgName, list);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.selectedString.Equals("Add New Row"))
                    {
                        rein_BEH_MB_dt.Rows.Add("", "");
                        rein_BEH_MB_dg.DataSource = rein_BEH_MB_dt;
                    } 
                    else
                    {
                        try
                        {
                            rein_BEH_MB_dt.Rows.RemoveAt(rein_BEH_MB_dt.Rows.Count - 1);
                            rein_BEH_MB_dg.DataSource = rein_BEH_MB_dt;
                        } catch (Exception ex)
                        {
                            MessageBox.Show("No rows to delete!");
                        }
                    }
                } 
            }
            else
            {
                //Stirrups and Ties
                List<string> list = new List<string>() { "Add New Row", "Delete Last Row" };
                string dlgName = "Bar End Hooks";
                DialogRadioBox dlg = new DialogRadioBox(dlgName, list);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.selectedString.Equals("Add New Row"))
                    {
                        rein_BEH_ST_dt.Rows.Add("", "");
                        rein_BEH_MB_dg.DataSource = rein_BEH_MB_dt;
                    }
                    else
                    {
                        try
                        {
                            rein_BEH_ST_dt.Rows.RemoveAt(rein_BEH_ST_dt.Rows.Count - 1);
                            rein_BEH_MB_dg.DataSource = rein_BEH_MB_dt;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("No rows to delete!");
                        }
                    }
                }
            }
        }

        private void rein_SaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rein_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                LslUC.Clear();
                rein_LSL_TB_fc_Panel.Controls.Clear();
                rein_LSL_CB_fc_Panel.Controls.Clear();

                rein_LSL_TB_dt.Rows.Clear();
                rein_LSL_TB_dt.Columns.Clear();
                rein_LSL_CB_dt.Rows.Clear();
                rein_LSL_CB_dt.Columns.Clear();

                rein_LSL_TB_dt.Columns.Add("Bar Sizes");
                rein_LSL_TB_dg.DataSource = rein_LSL_TB_dt;
                rein_LSL_TB_dg.Columns[0].Width = 45;
                rein_LSL_CB_dt.Columns.Add("Bar Sizes");
                rein_LSL_CB_dg.Columns[0].Width = 45;
                rein_LSL_CB_dg.DataSource = rein_LSL_CB_dt;

                rein_BEH_MB_dt.Rows.Clear();
                rein_BEH_MB_dg.DataSource = rein_BEH_MB_dt;
                rein_BEH_ST_dt.Rows.Clear();
                rein_BEH_ST_dg.DataSource = rein_BEH_ST_dt;
                setReinforcementsDefaultValues();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }

        private void rein_LSL_TB_dg_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //Reinforcements Functions -- END

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

        //Stairs -- START
        private void stair_ResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET every parameter in this panel?", "RESET Parameters", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                int i = 0;
                int j = 0;
                foreach (Floor floor in costEstimationForm.Floors)
                {
                    if (floor.getValues()[1].Equals(stairs_Floor_cbx.SelectedItem.ToString()))
                    {
                        foreach (string name in costEstimationForm.structuralMembers.stairsNames[i])
                        {
                            if (name.Equals(stairs_Stair_cbx.SelectedItem.ToString()))
                            {
                                string type = costEstimationForm.structuralMembers.stairs[i][j][0];
                                StairParameterUserControl content = new StairParameterUserControl(type);
                                costEstimationForm.parameters.stair[i][j] = content;
                                stairs_Panel.Controls.Clear();
                                stairs_Panel.Controls.Add(content);
                                break;
                            }
                            j++;
                        }
                        break;
                    }
                    i++;
                }
                if (stairs_Stair_cbx.SelectedItem.ToString().Equals("None"))
                {
                    stairs_Panel.Controls.Clear();
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }

        public void setStairsValues()
        {
            int lastIndexFloor = stairs_Floor_cbx.SelectedIndex;
            int lastIndexStair = stairs_Stair_cbx.SelectedIndex;
            stairs_Floor_cbx.Items.Clear();
            int j = 0;
            foreach (Floor floor in costEstimationForm.Floors)
            {
                stairs_Floor_cbx.Items.Add(floor.getValues()[1]);
                j++;
            }
            if(lastIndexFloor > stairs_Floor_cbx.Items.Count)
            {
                lastIndexFloor = 0;
                lastIndexStair = 0;
            }
            stairs_Floor_cbx.SelectedIndex = lastIndexFloor;
            stairs_Stair_cbx.SelectedIndex = lastIndexStair;
        }

        private void stairs_Floor_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            int lastIndexStair = stairs_Stair_cbx.SelectedIndex;
            stairs_Stair_cbx.Items.Clear();
            int j = 0;
            foreach (Floor floor in costEstimationForm.Floors)
            {
                if (floor.getValues()[1].Equals(stairs_Floor_cbx.SelectedItem.ToString()))
                {
                    break;
                }
                j++;
            }
            try
            {
                foreach (string name in costEstimationForm.structuralMembers.stairsNames[j])
                {
                    stairs_Stair_cbx.Items.Add(name);
                }
                if (stairs_Stair_cbx.Items.Count == 0)
                {
                    stairs_Stair_cbx.Items.Add("None");
                }
            } 
            catch(Exception ex)
            {
                //
            }
            try
            {
                stairs_Stair_cbx.SelectedIndex = lastIndexStair;
            }
            catch(Exception ex)
            {
                lastIndexStair = 0;
                stairs_Stair_cbx.SelectedIndex = lastIndexStair;
            }
        }

        private void stairs_Stair_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            int j = 0;
            foreach (Floor floor in costEstimationForm.Floors)
            {
                if (floor.getValues()[1].Equals(stairs_Floor_cbx.SelectedItem.ToString()))
                {
                    foreach (string name in costEstimationForm.structuralMembers.stairsNames[i])
                    {
                        if (name.Equals(stairs_Stair_cbx.SelectedItem.ToString()))
                        {
                            string type = costEstimationForm.structuralMembers.stairs[i][j][0];
                            stairs_Panel.Controls.Clear();
                            stairs_Panel.Controls.Add(costEstimationForm.parameters.stair[i][j]);
                            break;
                        }
                        j++;
                    }
                    break;
                }
                i++;
            }
            if (stairs_Stair_cbx.SelectedItem.ToString().Equals("None"))
            {
                stairs_Panel.Controls.Clear();
            }
        }
        //Stairs -- END

        //Labor and Equipment Functions -- START
        private void labor_MP_AddBtn_Click(object sender, EventArgs e)
        {
            ManPowerUserControl content = new ManPowerUserControl(this);
            MpUC.Add(content);
            //Default Values
            content.set_mpUC_cbx = "Foreman [hr]";
            content.set_mpUC_qty = "1";
            content.set_mpUC_hrs = "8";
            content.set_mpUC_days = "7";
            content.checkList = true;
            labor_MP_Panel.Controls.Add(content);
        }

        private void labor_EQP_AddBtn_Click(object sender, EventArgs e)
        {
            EquipmentUserControl content = new EquipmentUserControl(this);
            EqUC.Add(content);
            //Default Values
            content.set_eqUC_cbx = "Crawler Loader (80kW/ 1.5 - 2.0 cu.m) [hr]";
            content.set_eqUC_qty = "1";
            content.set_eqUC_hrs = "8";
            content.set_eqUC_days = "7";
            content.checkList = true;
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
            CustomItemsUserControl content = new CustomItemsUserControl(this, parameters);
            CiUC.Add(content);
            //Default Values
            content.set_ciUC_cbx = "Cyclone Wire (Gauge#10, 2”x2”, 3ft x 10m) [ROLL] - Common Materials";
            content.set_ciUC_qty = "3";
            content.checkList = true;
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

            //Reinforcements 
            setReinforcementsDefaultValues();

            //Paint
            setPaintDefaultValues();

            //Tiles
            setTilesDefaultValues();

            //Masonry
            setMasonryDefaultValues();

            //Stairs
            setStairsDefaultValues();

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
            if(parameters.conc_cmIsSelected[0]) { conc_CM_F_CG_rb.Select(); } else { conc_CM_F_RM_rb.Select(); }


            conc_CM_C_CG_cbx.Text = parameters.conc_CM_C_CG;
            conc_CM_C_GT_cbx.Text = parameters.conc_CM_C_GT;
            conc_CM_C_RM_cbx.Text = parameters.conc_CM_C_RM;
            if (parameters.conc_cmIsSelected[1]) { conc_CM_C_CG_rb.Select(); } else { conc_CM_C_RM_rb.Select(); }

            conc_CM_B_CG_cbx.Text = parameters.conc_CM_B_CG;
            conc_CM_B_GT_cbx.Text = parameters.conc_CM_B_GT;
            conc_CM_B_RM_cbx.Text = parameters.conc_CM_B_RM;
            if (parameters.conc_cmIsSelected[2]) { conc_CM_B_CG_rb.Select(); } else { conc_CM_B_RM_rb.Select(); }

            conc_CM_S_SOG_CG_cbx.Text = parameters.conc_CM_S_SOG_CG;
            conc_CM_S_SOG_GT_cbx.Text = parameters.conc_CM_S_SOG_GT;
            conc_CM_S_SOG_RM_cbx.Text = parameters.conc_CM_S_SOG_RM;
            if (parameters.conc_cmIsSelected[3]) { conc_CM_S_SOG_CG_rb.Select(); } else { conc_CM_S_SOG_RM_rb.Select(); }

            conc_CM_S_SS_CG_cbx.Text = parameters.conc_CM_S_SS_CG;
            conc_CM_S_SS_GT_cbx.Text = parameters.conc_CM_S_SS_GT;
            conc_CM_S_SS_RM_cbx.Text = parameters.conc_CM_S_SS_RM;
            if (parameters.conc_cmIsSelected[4]) { conc_CM_S_SS_CG_rb.Select(); } else { conc_CM_S_SS_RM_rb.Select(); }

            conc_CM_W_MEW_CM_cbx.Text = parameters.conc_CM_W_MEW_CM;
            conc_CM_W_MIW_CM_cbx.Text = parameters.conc_CM_W_MIW_CM;
            conc_CM_W_P_CM_cbx.Text = parameters.conc_CM_W_P_CM;
            conc_CM_W_P_PT_cbx.Text = parameters.conc_CM_W_P_PT;

            conc_CM_ST_CG_cbx.Text = parameters.conc_CM_ST_CG;
            conc_CM_ST_GT_cbx.Text = parameters.conc_CM_ST_GT;
            conc_CM_ST_RM_cbx.Text = parameters.conc_CM_ST_RM;
            if (parameters.conc_cmIsSelected[5]) { conc_CM_ST_CG_rb.Select(); } else { conc_CM_ST_RM_rb.Select(); }

            conc_CC_F_bx.Text = parameters.conc_CC_F;
            conc_CC_SS_bx.Text = parameters.conc_CC_SS;
            conc_CC_SG_bx.Text = parameters.conc_CC_SG;
            conc_CC_BEE_bx.Text = parameters.conc_CC_BEE;
            conc_CC_BEW_bx.Text = parameters.conc_CC_BEW;
            conc_CC_CEE_bx.Text = parameters.conc_CC_CEE;
            conc_CC_CEW_bx.Text = parameters.conc_CC_CEW;

            //Reinforcements
            rein_LSL_TB_dt = parameters.rein_LSL_TB_dt;
            rein_LSL_TB_dg.DataSource = rein_LSL_TB_dt;
            rein_LSL_TB_fc_Panel.Controls.Clear();
            for (int i = 0; i < parameters.rein_LSL_TB_fc_list.Count; i++)
            {
                LSLBarsUserControl content = new LSLBarsUserControl(this);
                content.lslUC_Label = "LS" + (i + 1) + " ƒ'c (MPa):";
                content.lslUC_Value = parameters.rein_LSL_TB_fc_list[i];
                content.barType = "Tension Bars";
                LslUC.Add(content);
                rein_LSL_TB_fc_Panel.Controls.Add(content);
            }
            rein_LSL_CB_dt = parameters.rein_LSL_CB_dt;
            rein_LSL_CB_dg.DataSource = rein_LSL_CB_dt;
            rein_LSL_CB_fc_Panel.Controls.Clear();
            for (int i = 0; i < parameters.rein_LSL_CB_fc_list.Count; i++)
            {
                LSLBarsUserControl content = new LSLBarsUserControl(this);
                content.lslUC_Label = "LS" + (i + 1) + " ƒ'c (MPa):";
                content.lslUC_Value = parameters.rein_LSL_CB_fc_list[i];
                content.barType = "Compression Bars";
                LslUC.Add(content);
                rein_LSL_CB_fc_Panel.Controls.Add(content);
            }
            rein_BEH_MB_dt = parameters.rein_BEH_MB_dt;
            rein_BEH_MB_dg.DataSource = rein_BEH_MB_dt;
            rein_BEH_ST_dt = parameters.rein_BEH_ST_dt;
            rein_BEH_ST_dg.DataSource = rein_BEH_ST_dt;
            rein_W_dt = parameters.rein_W_dt;
            rein_W_dg.DataSource = rein_W_dt;

            rein_RG_C_cbx.Text = parameters.rein_RG_C;
            rein_RG_CLT_cbx.Text = parameters.rein_RG_CLT;
            rein_RG_F_cbx.Text = parameters.rein_RG_F;
            rein_RG_B_cbx.Text = parameters.rein_RG_B;
            rein_RG_BS_cbx.Text = parameters.rein_RG_BS;
            rein_RG_ST_cbx.Text = parameters.rein_RG_ST;
            rein_RG_W_cbx.Text = parameters.rein_RG_W;
            rein_RG_SL_cbx.Text = parameters.rein_RG_SL;

            rein_ML_CF_6_chk.Checked = parameters.rein_mfIsSelected[0, 0];
            rein_ML_CF_75_chk.Checked = parameters.rein_mfIsSelected[0, 1];
            rein_ML_CF_9_chk.Checked = parameters.rein_mfIsSelected[0, 2];
            rein_ML_CF_105_chk.Checked = parameters.rein_mfIsSelected[0, 3];
            rein_ML_CF_12_chk.Checked = parameters.rein_mfIsSelected[0, 4];
            rein_ML_CF_135_chk.Checked = parameters.rein_mfIsSelected[0, 5];
            rein_ML_CF_15_chk.Checked = parameters.rein_mfIsSelected[0, 6];

            bool footTieIsChecked;
            footTieIsChecked = parameters.rein_mfIsSelected[1, 0];
            footTieIsChecked = parameters.rein_mfIsSelected[1, 1];
            footTieIsChecked = parameters.rein_mfIsSelected[1, 2];
            footTieIsChecked = parameters.rein_mfIsSelected[1, 3];
            footTieIsChecked = parameters.rein_mfIsSelected[1, 4];
            footTieIsChecked = parameters.rein_mfIsSelected[1, 5];
            footTieIsChecked = parameters.rein_mfIsSelected[1, 6];

            rein_ML_WF_6_chk.Checked = parameters.rein_mfIsSelected[2, 0];
            rein_ML_WF_75_chk.Checked = parameters.rein_mfIsSelected[2, 1];
            rein_ML_WF_9_chk.Checked = parameters.rein_mfIsSelected[2, 2];
            rein_ML_WF_105_chk.Checked = parameters.rein_mfIsSelected[2, 3];
            rein_ML_WF_12_chk.Checked = parameters.rein_mfIsSelected[2, 4];
            rein_ML_WF_135_chk.Checked = parameters.rein_mfIsSelected[2, 5];
            rein_ML_WF_15_chk.Checked = parameters.rein_mfIsSelected[2, 6];

            rein_ML_C_6_chk.Checked = parameters.rein_mfIsSelected[3, 0];
            rein_ML_C_75_chk.Checked = parameters.rein_mfIsSelected[3, 1];
            rein_ML_C_9_chk.Checked = parameters.rein_mfIsSelected[3, 2];
            rein_ML_C_105_chk.Checked = parameters.rein_mfIsSelected[3, 3];
            rein_ML_C_12_chk.Checked = parameters.rein_mfIsSelected[3, 4];
            rein_ML_C_135_chk.Checked = parameters.rein_mfIsSelected[3, 5];
            rein_ML_C_15_chk.Checked = parameters.rein_mfIsSelected[3, 6];

            rein_ML_B_6_chk.Checked = parameters.rein_mfIsSelected[4, 0];
            rein_ML_B_75_chk.Checked = parameters.rein_mfIsSelected[4, 1];
            rein_ML_B_9_chk.Checked = parameters.rein_mfIsSelected[4, 2];
            rein_ML_B_105_chk.Checked = parameters.rein_mfIsSelected[4, 3];
            rein_ML_B_12_chk.Checked = parameters.rein_mfIsSelected[4, 4];
            rein_ML_B_135_chk.Checked = parameters.rein_mfIsSelected[4, 5];
            rein_ML_B_15_chk.Checked = parameters.rein_mfIsSelected[4, 6];

            rein_ML_SG_6_chk.Checked = parameters.rein_mfIsSelected[5, 0];
            rein_ML_SG_75_chk.Checked = parameters.rein_mfIsSelected[5, 1];
            rein_ML_SG_9_chk.Checked = parameters.rein_mfIsSelected[5, 2];
            rein_ML_SG_105_chk.Checked = parameters.rein_mfIsSelected[5, 3];
            rein_ML_SG_12_chk.Checked = parameters.rein_mfIsSelected[5, 4];
            rein_ML_SG_135_chk.Checked = parameters.rein_mfIsSelected[5, 5];
            rein_ML_SG_15_chk.Checked = parameters.rein_mfIsSelected[5, 6];

            rein_ML_SS_6_chk.Checked = parameters.rein_mfIsSelected[6, 0];
            rein_ML_SS_75_chk.Checked = parameters.rein_mfIsSelected[6, 1];
            rein_ML_SS_9_chk.Checked = parameters.rein_mfIsSelected[6, 2];
            rein_ML_SS_105_chk.Checked = parameters.rein_mfIsSelected[6, 3];
            rein_ML_SS_12_chk.Checked = parameters.rein_mfIsSelected[6, 4];
            rein_ML_SS_135_chk.Checked = parameters.rein_mfIsSelected[6, 5];
            rein_ML_SS_15_chk.Checked = parameters.rein_mfIsSelected[6, 6];
            
            /*for check
            foreach (DataRow dataRow in rein_LSL_TB_dt.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    Console.WriteLine(item);
                }
            }
            */

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

            //Masonry
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

            //Stairs
            stairs_Floor_cbx.SelectedIndex = 0;
            stairs_Stair_cbx.SelectedIndex = 0;

            //Labor and Equipment
            labor_RD_cbx.Text = parameters.labor_RD;
            if (parameters.labor_MP != null)
            {
                for (int i = 0; i < parameters.labor_MP.Count; i++)
                {
                    ManPowerUserControl content = new ManPowerUserControl(this);
                    content.set_mpUC_cbx = parameters.labor_MP[i][0];
                    content.set_mpUC_qty = parameters.labor_MP[i][1];
                    content.set_mpUC_hrs = parameters.labor_MP[i][2];
                    content.set_mpUC_days = parameters.labor_MP[i][3];
                    content.checkList = bool.Parse(parameters.labor_MP[i][4]);
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
                    content.set_eqUC_hrs = parameters.labor_EQP[i][2];
                    content.set_eqUC_days = parameters.labor_EQP[i][3];
                    content.checkList = bool.Parse(parameters.labor_EQP[i][4]);
                    EqUC.Add(content);
                    labor_EQP_Panel.Controls.Add(EqUC[i]);
                }
            }

            //Misc
            if (parameters.misc_CustomItems != null)
            {
                for (int i = 0; i < parameters.misc_CustomItems.Count; i++)
                {
                    CustomItemsUserControl content = new CustomItemsUserControl(this, parameters);
                    content.set_ciUC_cbx = parameters.misc_CustomItems[i][0];
                    content.set_ciUC_qty = parameters.misc_CustomItems[i][1];
                    content.set_ciUC_price = parameters.misc_CustomItems[i][2];
                    content.checkList = bool.Parse(parameters.misc_CustomItems[i][3]);
                    CiUC.Add(content);
                    misc_Panel.Controls.Add(CiUC[i]);
                }
            }
        }

        private void setEarthworkDefaultValues()
        {
            earth_CF_FA_bx.Text = "250";
            earth_CF_TH_bx.Text = "100";
            earth_CF_TY_cbx.SelectedIndex = 0;
            earth_CF_CF_bx.Text = "30%";

            earth_WF_FA_bx.Text = "100";
            earth_WF_TH_bx.Text = "100";
            earth_WF_CF_bx.Text = "30%";

            earth_WTB_FA_bx.Text = "100";
            earth_WTB_TH_bx.Text = "100";
            earth_WTB_CF_bx.Text = "30%";

            earth_SG_AS_bx.Text = "75";
            earth_SG_TS_bx.Text = "100";
            earth_SG_TH_bx.Text = "100";
            earth_SG_CF_bx.Text = "30%";
            if(mef != null)
            {
                mef.Elevations.Clear();
                mef.clearElevations();
            }
        }


        private void setFormworkDefaultValues()
        {
            form_SM_F_FL_cbx.SelectedIndex = 0;
            form_SM_C_FL_cbx.SelectedIndex = 0;
            form_SM_C_VS_cbx.SelectedIndex = 0;
            form_SM_C_HB_cbx.SelectedIndex = 0;
            form_SM_B_FL_cbx.SelectedIndex = 0;
            form_SM_B_VS_cbx.SelectedIndex = 0;
            form_SM_B_HB_cbx.SelectedIndex = 0;
            form_SM_B_DB_cbx.SelectedIndex = 0;
            form_SM_HS_VS_cbx.SelectedIndex = 0;
            form_SM_ST_FL_cbx.SelectedIndex = 0;
            form_SM_ST_VS_cbx.SelectedIndex = 0;
            form_F_T_cbx.Text = "Plywood";
            form_F_NU_bx.Text = "2";
            form_F_N_bx.Text = "0.215 kg/1 m2";
        }

        private void setConcreteDefaultValues()
        {
            conc_CM_F_CG_cbx.Text = "CLASS AA";
            conc_CM_F_GT_cbx.SelectedIndex = 0;
            conc_CM_F_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_F_CG_rb.Select();

            conc_CM_C_CG_cbx.Text = "CLASS AA";
            conc_CM_C_GT_cbx.SelectedIndex = 0;
            conc_CM_C_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_C_CG_rb.Select();

            conc_CM_B_CG_cbx.Text = "CLASS AA";
            conc_CM_B_GT_cbx.SelectedIndex = 0;
            conc_CM_B_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_B_CG_rb.Select();

            conc_CM_S_SOG_CG_cbx.Text = "CLASS AA";
            conc_CM_S_SOG_GT_cbx.SelectedIndex = 0;
            conc_CM_S_SOG_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_S_SOG_CG_rb.Select();

            conc_CM_S_SS_CG_cbx.Text = "CLASS AA";
            conc_CM_S_SS_GT_cbx.SelectedIndex = 0;
            conc_CM_S_SS_RM_cbx.Text = "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days";
            conc_CM_S_SS_CG_rb.Select();

            conc_CM_W_MEW_CM_cbx.Text = "CLASS A";
            conc_CM_W_MIW_CM_cbx.Text = "CLASS A";
            conc_CM_W_P_CM_cbx.Text = "CLASS A";
            conc_CM_W_P_PT_cbx.Text = "20mm";

            conc_CM_ST_CG_cbx.Text = "CLASS AA";
            conc_CM_ST_GT_cbx.SelectedIndex = 0;
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

        private void setReinforcementsDefaultValues()
        {
            rein_W_dt.Rows[0][1] = "0.222";
            rein_W_dt.Rows[1][1] = "0.395";
            rein_W_dt.Rows[2][1] = "0.616";
            rein_W_dt.Rows[3][1] = "0.888";
            rein_W_dt.Rows[4][1] = "1.597";
            rein_W_dt.Rows[5][1] = "2.466";
            rein_W_dt.Rows[6][1] = "3.854";
            rein_W_dt.Rows[7][1] = "4.833";
            rein_W_dt.Rows[8][1] = "6.313";
            rein_W_dt.Rows[9][1] = "7.991";
            rein_W_dt.Rows[10][1] = "9.864";
            rein_W_dt.Rows[11][1] = "11.926";
            rein_W_dt.Rows[12][1] = "15.413";
            rein_W_dt.Rows[13][1] = "19.318";
            rein_W_dg.DataSource = rein_W_dt;

            rein_RG_C_cbx.SelectedIndex = 0;
            rein_RG_CLT_cbx.SelectedIndex = 0;
            rein_RG_F_cbx.SelectedIndex = 0;
            rein_RG_B_cbx.SelectedIndex = 0;
            rein_RG_BS_cbx.SelectedIndex = 0;
            rein_RG_ST_cbx.SelectedIndex = 0;
            rein_RG_W_cbx.SelectedIndex = 0;
            rein_RG_SL_cbx.SelectedIndex = 0;

            rein_ML_CF_6_chk.Checked = true;
            rein_ML_CF_75_chk.Checked = true;
            rein_ML_CF_9_chk.Checked = true;
            rein_ML_CF_105_chk.Checked = true;
            rein_ML_CF_12_chk.Checked = true;
            rein_ML_CF_135_chk.Checked = true;
            rein_ML_CF_15_chk.Checked = true;

            rein_ML_WF_6_chk.Checked = true;
            rein_ML_WF_75_chk.Checked = true;
            rein_ML_WF_9_chk.Checked = true;
            rein_ML_WF_105_chk.Checked = true;
            rein_ML_WF_12_chk.Checked = true;
            rein_ML_WF_135_chk.Checked = true;
            rein_ML_WF_15_chk.Checked = true;

            rein_ML_C_6_chk.Checked = true;
            rein_ML_C_75_chk.Checked = true;
            rein_ML_C_9_chk.Checked = true;
            rein_ML_C_105_chk.Checked = true;
            rein_ML_C_12_chk.Checked = true;
            rein_ML_C_135_chk.Checked = true;
            rein_ML_C_15_chk.Checked = true;

            rein_ML_B_6_chk.Checked = true;
            rein_ML_B_75_chk.Checked = true;
            rein_ML_B_9_chk.Checked = true;
            rein_ML_B_105_chk.Checked = true;
            rein_ML_B_12_chk.Checked = true;
            rein_ML_B_135_chk.Checked = true;
            rein_ML_B_15_chk.Checked = true;

            rein_ML_SG_6_chk.Checked = true;
            rein_ML_SG_75_chk.Checked = true;
            rein_ML_SG_9_chk.Checked = true;
            rein_ML_SG_105_chk.Checked = true;
            rein_ML_SG_12_chk.Checked = true;
            rein_ML_SG_135_chk.Checked = true;
            rein_ML_SG_15_chk.Checked = true;

            rein_ML_SS_6_chk.Checked = true;
            rein_ML_SS_75_chk.Checked = true;
            rein_ML_SS_9_chk.Checked = true;
            rein_ML_SS_105_chk.Checked = true;
            rein_ML_SS_12_chk.Checked = true;
            rein_ML_SS_135_chk.Checked = true;
            rein_ML_SS_15_chk.Checked = true;
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

        private void setStairsDefaultValues()
        {
            stairs_Floor_cbx.Items.Clear();
            int j = 0;
            foreach (Floor floor in costEstimationForm.Floors)
            {
                stairs_Floor_cbx.Items.Add(floor.getValues()[1]);
                j++;
            }
            if (stairs_Floor_cbx.Items.Count == 0)
            {
                stairs_Floor_cbx.Items.Add("GROUND FLOOR");
            }
            stairs_Floor_cbx.SelectedIndex = 0;
            if (stairs_Stair_cbx.Items.Count == 0)
            {
                stairs_Stair_cbx.Items.Add("None");
            }
            stairs_Stair_cbx.SelectedIndex = 0;
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
                    conc_CM_B_CG_rb.Checked, conc_CM_S_SOG_CG_rb.Checked, conc_CM_S_SS_CG_rb.Checked, 
                    conc_CM_ST_CG_rb.Checked };
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

                conc_CM_S_SOG_CG_cbx.SelectedItem.ToString(),
                conc_CM_S_SOG_GT_cbx.SelectedItem.ToString(),
                conc_CM_S_SOG_RM_cbx.SelectedItem.ToString(),

                conc_CM_S_SS_CG_cbx.SelectedItem.ToString(),
                conc_CM_S_SS_GT_cbx.SelectedItem.ToString(),
                conc_CM_S_SS_RM_cbx.SelectedItem.ToString(),

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

            //Reinforcements 
            List<string> rein_LSL_TB_fc_list = new List<string>();
            List<string> rein_LSL_CB_fc_list = new List<string>();
            for (int i = 0; i < LslUC.Count; i++)
            {
                if (LslUC[i].barType.Equals("Tension Bars"))
                {
                    rein_LSL_TB_fc_list.Add(LslUC[i].lslUC_Value);
                } 
                else
                {
                    rein_LSL_CB_fc_list.Add(LslUC[i].lslUC_Value);
                }
            }
            bool[,] rein_mfIsSelected = new bool[,] {
                                                        { rein_ML_CF_6_chk.Checked, rein_ML_CF_75_chk.Checked, rein_ML_CF_9_chk.Checked, rein_ML_CF_105_chk.Checked, rein_ML_CF_12_chk.Checked, rein_ML_CF_135_chk.Checked, rein_ML_CF_15_chk.Checked },
                                                        { true, true, true, true, true, true, true },
                                                        { rein_ML_WF_6_chk.Checked, rein_ML_WF_75_chk.Checked, rein_ML_WF_9_chk.Checked, rein_ML_WF_105_chk.Checked, rein_ML_WF_12_chk.Checked, rein_ML_WF_135_chk.Checked, rein_ML_WF_15_chk.Checked },
                                                        { rein_ML_C_6_chk.Checked, rein_ML_C_75_chk.Checked, rein_ML_C_9_chk.Checked, rein_ML_C_105_chk.Checked, rein_ML_C_12_chk.Checked, rein_ML_C_135_chk.Checked, rein_ML_C_15_chk.Checked },
                                                        { rein_ML_B_6_chk.Checked, rein_ML_B_75_chk.Checked, rein_ML_B_9_chk.Checked, rein_ML_B_105_chk.Checked, rein_ML_B_12_chk.Checked, rein_ML_B_135_chk.Checked, rein_ML_B_15_chk.Checked },
                                                        { rein_ML_SG_6_chk.Checked, rein_ML_SG_75_chk.Checked, rein_ML_SG_9_chk.Checked, rein_ML_SG_105_chk.Checked, rein_ML_SG_12_chk.Checked, rein_ML_SG_135_chk.Checked, rein_ML_SG_15_chk.Checked },
                                                        { rein_ML_SS_6_chk.Checked, rein_ML_SS_75_chk.Checked, rein_ML_SS_9_chk.Checked, rein_ML_SS_105_chk.Checked, rein_ML_SS_12_chk.Checked, rein_ML_SS_135_chk.Checked, rein_ML_SS_15_chk.Checked },
                                                    };
            parameters.setReinforcementsParameters(
                rein_LSL_TB_dt, rein_LSL_CB_dt, rein_BEH_MB_dt, rein_BEH_ST_dt,
                rein_W_dt, rein_LSL_TB_fc_list, rein_LSL_CB_fc_list,
                rein_RG_C_cbx.Text, rein_RG_CLT_cbx.Text, rein_RG_F_cbx.Text, rein_RG_B_cbx.Text, rein_RG_BS_cbx.Text, rein_RG_ST_cbx.Text, rein_RG_W_cbx.Text, rein_RG_SL_cbx.Text,
                rein_mfIsSelected
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
                string[] toAdd = { mpUC[i].set_mpUC_cbx, mpUC[i].set_mpUC_qty, mpUC[i].set_mpUC_hrs, mpUC[i].set_mpUC_days, mpUC[i].checkList.ToString() };
                labor_MP.Add(toAdd);
            }
            List<string[]> labor_EQP = new List<string[]>();
            for (int i = 0; i < eqUC.Count; i++)
            {
                string[] toAdd = { eqUC[i].set_eqUC_cbx, eqUC[i].set_eqUC_qty, eqUC[i].set_eqUC_hrs, eqUC[i].set_eqUC_days, eqUC[i].checkList.ToString() };
                labor_EQP.Add(toAdd);
            }
            parameters.setLaborParameters(
                labor_RD_cbx.Text, labor_MP, labor_EQP
            );

            //Misc
            List<string[]> misc_CustomItems = new List<string[]>();
            for (int i = 0; i < ciUC.Count; i++)
            {
                string[] toAdd = { ciUC[i].set_ciUC_cbx, ciUC[i].set_ciUC_qty, ciUC[i].set_ciUC_price, ciUC[i].checkList.ToString() };
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

        private void earth_CF_TY_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            earth_WF_TY_cbx.SelectedIndex = earth_CF_TY_cbx.SelectedIndex;
            earth_WTB_TY_cbx.SelectedIndex = earth_CF_TY_cbx.SelectedIndex;
            earth_SG_TY_cbx.SelectedIndex = earth_CF_TY_cbx.SelectedIndex;
        }
        private void earth_WF_TY_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            earth_CF_TY_cbx.SelectedIndex = earth_WF_TY_cbx.SelectedIndex;
            earth_WTB_TY_cbx.SelectedIndex = earth_WF_TY_cbx.SelectedIndex;
            earth_SG_TY_cbx.SelectedIndex = earth_WF_TY_cbx.SelectedIndex;
        }

        private void earth_WTB_TY_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            earth_CF_TY_cbx.SelectedIndex = earth_WTB_TY_cbx.SelectedIndex;
            earth_WF_TY_cbx.SelectedIndex = earth_WTB_TY_cbx.SelectedIndex;
            earth_SG_TY_cbx.SelectedIndex = earth_WTB_TY_cbx.SelectedIndex;
        }

        private void earth_SG_TY_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            earth_CF_TY_cbx.SelectedIndex = earth_SG_TY_cbx.SelectedIndex;
            earth_WF_TY_cbx.SelectedIndex = earth_SG_TY_cbx.SelectedIndex;
            earth_WF_TY_cbx.SelectedIndex = earth_SG_TY_cbx.SelectedIndex;
        }
        //Extra Functions -- END

        //Earthworks Hover -- START

        //Formworks Allowance
        private void earth_CF_FA_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Extra space that will be used by the formworks. \nThis will affect the excavation, gravel bedding and compaction volume.", earth_CF_FA_bx);
        }

        private void earth_WF_FA_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Extra space that will be used by the formworks. \nThis will affect the excavation, gravel bedding and compaction volume.", earth_WF_FA_bx);
        }

        private void earth_WTB_FA_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Extra space that will be used by the formworks. \nThis will affect the excavation, gravel bedding and compaction volume.", earth_WTB_FA_bx);
        }

        //Thickness
        private void earth_CF_TH_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Desired Gravel bedding thickness in mm", earth_CF_TH_bx);
        }

        private void earth_WF_TH_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Desired Gravel bedding thickness in mm", earth_WF_TH_bx);
        }

        private void earth_WTB_TH_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Desired Gravel bedding thickness in mm", earth_WTB_TH_bx);
        }

        private void earth_SG_TH_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Desired Gravel bedding thickness in mm", earth_SG_TH_bx);
        }

        //Type
        private void earth_CF_TY_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Type of grave to be used. This will be uniformly used for earthworks.", earth_CF_TY_cbx);
        }

        private void earth_WF_TY_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Type of grave to be used. This will be uniformly used for earthworks.", earth_WF_TY_cbx);
        }

        private void earth_WTB_TY_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Type of grave to be used. This will be uniformly used for earthworks.", earth_WTB_TY_cbx);
        }

        private void earth_SG_TY_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Type of grave to be used. This will be uniformly used for earthworks.", earth_SG_TY_cbx);
        }

        //Compaction Factor
        private void earth_CF_CF_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("The desired thickness will not always be completely achieved after the compaction of the gravel bedding. \nThe compaction factor will compensate and add a percentage of volume lost due to compaction.", earth_CF_CF_bx);
        }

        private void earth_WF_CF_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("The desired thickness will not always be completely achieved after the compaction of the gravel bedding. \nThe compaction factor will compensate and add a percentage of volume lost due to compaction.", earth_WF_CF_bx);
        }

        private void earth_WTB_CF_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("The desired thickness will not always be completely achieved after the compaction of the gravel bedding. \nThe compaction factor will compensate and add a percentage of volume lost due to compaction.", earth_WTB_CF_bx);
        }

        private void earth_SG_CF_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("The desired thickness will not always be completely achieved after the compaction of the gravel bedding. \nThe compaction factor will compensate and add a percentage of volume lost due to compaction.", earth_SG_CF_bx);
        }

        //Manage Elevations
        private void earth_ElevBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Add or remove elevations.", earth_ElevBtn);
        }

        //Reset Button
        private void earth_ResetBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Revert everything in this section to default", earth_ResetBtn);
        }
        //Earthworks Hover -- END

        //Formworks Hover -- START
        private void form_F_NU_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Multiplier for the number of times a formwork should be used.", form_F_NU_bx);
        }

        private void form_F_N_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Multiplier for the weight of nails (kg) to be purchased alongside with the formworks", form_F_N_bx);
        }

        private void form_F_T_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the type of formworks to be used", form_F_T_cbx);
        }

        //Formworks Hover -- END

        //Concrete Grade -- START
        //Footing
        private void label14_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of concrete mixture class that will yield Cement (40 kg Bags), Sand (m3) and Gravel (m3)", label14);
        }
        private void conc_CM_F_CG_cbx_MouseHover(object sender, EventArgs e)
        {
            if(conc_CM_F_CG_cbx.Text.Equals("CLASS AA"))
                toolTip1.Show("CLASS AA multiplier: \n 12.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_F_CG_cbx);
            else if (conc_CM_F_CG_cbx.Text.Equals("CLASS A"))
                toolTip1.Show("CLASS A multiplier: \n 9.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_F_CG_cbx);
            else if (conc_CM_F_CG_cbx.Text.Equals("CLASS B"))
                toolTip1.Show("CLASS B multiplier: \n 7.5 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_F_CG_cbx);
            else
                toolTip1.Show("CLASS C multiplier: \n 6.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_F_CG_cbx);
        }
        private void conc_CM_F_GT_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of Gravel Types.", conc_CM_F_GT_cbx);
        }

        //Column
        private void label17_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of concrete mixture class that will yield Cement (40 kg Bags), Sand (m3) and Gravel (m3)", label17);
        }

        private void conc_CM_C_CG_cbx_MouseHover(object sender, EventArgs e)
        {
            if (conc_CM_C_CG_cbx.Text.Equals("CLASS AA"))
                toolTip1.Show("CLASS AA multiplier: \n 12.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_C_CG_cbx);
            else if (conc_CM_C_CG_cbx.Text.Equals("CLASS A"))
                toolTip1.Show("CLASS A multiplier: \n 9.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_C_CG_cbx);
            else if (conc_CM_C_CG_cbx.Text.Equals("CLASS B"))
                toolTip1.Show("CLASS B multiplier: \n 7.5 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_C_CG_cbx);
            else
                toolTip1.Show("CLASS C multiplier: \n 6.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_C_CG_cbx);
        }

        private void conc_CM_C_GT_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of Gravel Types.", conc_CM_C_GT_cbx);
        }

        //Beams
        private void label34_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of concrete mixture class that will yield Cement (40 kg Bags), Sand (m3) and Gravel (m3)", label34);
        }

        private void conc_CM_B_CG_cbx_MouseHover(object sender, EventArgs e)
        {
            if (conc_CM_B_CG_cbx.Text.Equals("CLASS AA"))
                toolTip1.Show("CLASS AA multiplier: \n 12.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_B_CG_cbx);
            else if (conc_CM_B_CG_cbx.Text.Equals("CLASS A"))
                toolTip1.Show("CLASS A multiplier: \n 9.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_B_CG_cbx);
            else if (conc_CM_B_CG_cbx.Text.Equals("CLASS B"))
                toolTip1.Show("CLASS B multiplier: \n 7.5 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_B_CG_cbx);
            else
                toolTip1.Show("CLASS C multiplier: \n 6.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_B_CG_cbx);
        }

        private void conc_CM_B_GT_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of Gravel Types.", conc_CM_B_GT_cbx);
        }

        //Slab on Grade
        private void label53_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of concrete mixture class that will yield Cement (40 kg Bags), Sand (m3) and Gravel (m3)", label53);
        }
        private void conc_CM_S_SOG_CG_cbx_MouseHover(object sender, EventArgs e)
        {
            if (conc_CM_S_SOG_CG_cbx.Text.Equals("CLASS AA"))
                toolTip1.Show("CLASS AA multiplier: \n 12.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SOG_CG_cbx);
            else if (conc_CM_S_SOG_CG_cbx.Text.Equals("CLASS A"))
                toolTip1.Show("CLASS A multiplier: \n 9.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SOG_CG_cbx);
            else if (conc_CM_S_SOG_CG_cbx.Text.Equals("CLASS B"))
                toolTip1.Show("CLASS B multiplier: \n 7.5 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SOG_CG_cbx);
            else
                toolTip1.Show("CLASS C multiplier: \n 6.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SOG_CG_cbx);
        }

        private void conc_CM_S_SOG_GT_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of Gravel Types.", conc_CM_S_SOG_GT_cbx);
        }

        //Suspended Slab
        private void label150_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of concrete mixture class that will yield Cement (40 kg Bags), Sand (m3) and Gravel (m3)", label150);
        }

        private void conc_CM_S_SS_CG_cbx_MouseHover(object sender, EventArgs e)
        {
            if (conc_CM_S_SS_CG_cbx.Text.Equals("CLASS AA"))
                toolTip1.Show("CLASS AA multiplier: \n 12.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SS_CG_cbx);
            else if (conc_CM_S_SS_CG_cbx.Text.Equals("CLASS A"))
                toolTip1.Show("CLASS A multiplier: \n 9.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SS_CG_cbx);
            else if (conc_CM_S_SS_CG_cbx.Text.Equals("CLASS B"))
                toolTip1.Show("CLASS B multiplier: \n 7.5 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SS_CG_cbx);
            else
                toolTip1.Show("CLASS C multiplier: \n 6.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_S_SS_CG_cbx);
        }

        private void conc_CM_S_SS_GT_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of Gravel Types.", conc_CM_S_SS_GT_cbx);
        }

        //Stairs
        private void label58_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of concrete mixture class that will yield Cement (40 kg Bags), Sand (m3) and Gravel (m3)", label58);
        }

        private void conc_CM_ST_CG_cbx_MouseHover(object sender, EventArgs e)
        {
            if (conc_CM_ST_CG_cbx.Text.Equals("CLASS AA"))
                toolTip1.Show("CLASS AA multiplier: \n 12.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_ST_CG_cbx);
            else if (conc_CM_ST_CG_cbx.Text.Equals("CLASS A"))
                toolTip1.Show("CLASS A multiplier: \n 9.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_ST_CG_cbx);
            else if (conc_CM_ST_CG_cbx.Text.Equals("CLASS B"))
                toolTip1.Show("CLASS B multiplier: \n 7.5 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_ST_CG_cbx);
            else
                toolTip1.Show("CLASS C multiplier: \n 6.0 for Cement (40kg/Bag), \n 0.5 for Sand (m3), \n 1.0 for Gravel (m3)", conc_CM_ST_CG_cbx);
        }

        private void conc_CM_ST_GT_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Selection of Gravel Types.", conc_CM_ST_GT_cbx);
        }
        //Concrete Grade -- END

        //Paint Hover
        private void paint_SCL_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Number of times the skim coating should be applied to form layers.", paint_SCL_bx);
        }

        //Tiles Hover
        private void tiles_FS_bx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Additional tiles to be purchased from each computed totality. \nTo ensure that there are enough tiles considering handling and installation errors, a factor of safety is used.", tiles_FS_bx);
        }

        //Labor and Equipment Hover
        private void labor_RD_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Manila Rate uses higher rates than provincial rates. \nSelect corresponding rate of labor per day.", labor_RD_cbx);
        }

        //Masonry Hover -- START
        //Ext walls
        private void mason_CHB_EW_AddBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Adding the height and length of wall, windows, or doors in meters", mason_CHB_EW_AddBtn);
        }

        private void mason_CHB_EW_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Typical size of CHB in meters", mason_CHB_EW_cbx);
        }

        //Int Walls
        private void mason_CHB_IW_AddBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Adding the height and length of wall, windows, or doors in meters", mason_CHB_IW_AddBtn);
        }

        //Reinforcement and Tie wires of CHB
        private void mason_CHB_IW_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Typical size of CHB in meters", mason_CHB_IW_cbx);
        }

        private void mason_RTW_VS_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Vertical spacing in meters", mason_RTW_VS_cbx);
        }

        private void mason_RTW_HSL_cbx_MouseHover(object sender, EventArgs e)
        {
            //toolTip1.Show("Horizontal spacing in meters", mason_RTW_HSL_cbx);
        }

        private void mason_RTW_BD_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Bar diameter in millimeter", mason_RTW_BD_cbx);
        }

        private void mason_RTW_RL_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Rebar length in meter", mason_RTW_RL_cbx);
        }

        private void mason_RTW_LTW_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Length of tie wire in centimeter", mason_RTW_LTW_cbx);
        }

        private void rein_RG_ST_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rein_RG_ST_cbx.Text.Equals("Grade 33"))
            {
                foreach(List<StairParameterUserControl> floor in parameters.stair)
                {
                    foreach(StairParameterUserControl stair in floor)
                    {
                        if (stair.type.Equals("Straight Stairs"))
                            stair.setStraightCB(0);
                        else if (stair.type.Equals("U-Stairs"))
                            stair.setUCB(0);
                        else
                            stair.setLCB(0);
                    }
                }
            }
            else if (rein_RG_ST_cbx.Text.Equals("Grade 40"))
            {
                foreach (List<StairParameterUserControl> floor in parameters.stair)
                {
                    foreach (StairParameterUserControl stair in floor)
                    {
                        if (stair.type.Equals("Straight Stairs"))
                            stair.setStraightCB(1);
                        else if (stair.type.Equals("U-Stairs"))
                            stair.setUCB(1);
                        else
                            stair.setLCB(1);
                    }
                }
            }
            else
            {
                foreach (List<StairParameterUserControl> floor in parameters.stair)
                {
                    foreach (StairParameterUserControl stair in floor)
                    {
                        if (stair.type.Equals("Straight Stairs"))
                            stair.setStraightCB(2);
                        else if (stair.type.Equals("U-Stairs"))
                            stair.setUCB(2);
                        else
                            stair.setLCB(2);
                    }
                }
            }
        }

        private void rein_RG_W_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rein_RG_W_cbx.Text.Equals("Grade 33"))
            {
                mason_RTW_RG_cbx.SelectedIndex = 0;
            }
            else if (rein_RG_W_cbx.Text.Equals("Grade 40"))
            {
                mason_RTW_RG_cbx.SelectedIndex = 1;
            }
            else
            {
                mason_RTW_RG_cbx.SelectedIndex = 2;
            }
        }
    }
}
