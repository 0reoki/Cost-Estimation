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
    public partial class FactorOfSafetyForm : Form
    {
        CostEstimationForm cEF;
        public FactorOfSafetyForm(CostEstimationForm cEF)
        {
            InitializeComponent();
            this.cEF = cEF;

            fos_Cement_bx.Text = cEF.fos_Cement;
            fos_Sand_bx.Text = cEF.fos_Sand;
            fos_Gravel_bx.Text = cEF.fos_Gravel;
            fos_RMC_bx.Text = cEF.fos_RMC;
            fos_CHB_bx.Text = cEF.fos_CHB;
            fos_Tiles_bx.Text = cEF.fos_Tiles;
            fos_LC_cbx.Text = cEF.fos_LC_Type;
            fos_LC_bx.Text = cEF.fos_LC_Percentage;
        }

        private void fos_SaveBtn_Click(object sender, EventArgs e)
        {
            cEF.fos_Cement = fos_Cement_bx.Text;
            cEF.fos_Sand = fos_Sand_bx.Text;
            cEF.fos_Gravel = fos_Gravel_bx.Text;
            cEF.fos_RMC = fos_RMC_bx.Text;
            cEF.fos_CHB = fos_CHB_bx.Text;
            cEF.fos_Tiles = fos_Tiles_bx.Text;
            cEF.fos_LC_Type = fos_LC_cbx.Text;
            cEF.fos_LC_Percentage = fos_LC_bx.Text;

            this.DialogResult = DialogResult.OK;
        }

        private void fos_CancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void fos_LC_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(fos_LC_cbx.SelectedIndex == 1)
            {
                fos_LC_bx.Enabled = true;
            } 
            else
            {
                fos_LC_bx.Enabled = false;
            }
        }
    }
}
