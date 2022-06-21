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
    public partial class PriceChecklistForms : Form
    {
        public PriceChecklistForms(CostEstimationForm costEstimationForm)
        {
            InitializeComponent();
            pcl_1_cb.Checked = costEstimationForm.earthworksChecklist[0];
            pcl_11_cb.Checked = costEstimationForm.earthworksChecklist[1];
            pcl_12_cb.Checked = costEstimationForm.earthworksChecklist[2];
            pcl_13_cb.Checked = costEstimationForm.earthworksChecklist[3];
            pcl_14_cb.Checked = costEstimationForm.earthworksChecklist[4];
            pcl_15_cb.Checked = costEstimationForm.earthworksChecklist[5];
        }

        private void priceCL_OKBtn_Click(object sender, EventArgs e)
        {
            // Change costEstimationForm Checklist bool data
            this.DialogResult = DialogResult.OK;
        }

        private void pcl_1_cb_Click(object sender, EventArgs e)
        {
            if (pcl_1_cb.Checked)
            {
                pcl_11_cb.Checked = true;
                pcl_12_cb.Checked = true;
                pcl_13_cb.Checked = true;
                pcl_14_cb.Checked = true;
                pcl_15_cb.Checked = true;
            }
            else
            {
                pcl_11_cb.Checked = false;
                pcl_12_cb.Checked = false;
                pcl_13_cb.Checked = false;
                pcl_14_cb.Checked = false;
                pcl_15_cb.Checked = false;
            }
        }

        private void pcl_11_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_11_cb.Checked)
                pcl_1_cb.Checked = true;
            else
                if (!pcl_12_cb.Checked && !pcl_13_cb.Checked && !pcl_14_cb.Checked && !pcl_15_cb.Checked)
                    pcl_1_cb.Checked = false;
        }

        private void pcl_12_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_12_cb.Checked)
                pcl_1_cb.Checked = true;
            else
                if (!pcl_11_cb.Checked && !pcl_13_cb.Checked && !pcl_14_cb.Checked && !pcl_15_cb.Checked)
                    pcl_1_cb.Checked = false;
        }

        private void pcl_13_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_13_cb.Checked)
                pcl_1_cb.Checked = true;
            else
                if (!pcl_11_cb.Checked && !pcl_12_cb.Checked && !pcl_14_cb.Checked && !pcl_15_cb.Checked)
                    pcl_1_cb.Checked = false;
        }

        private void pcl_14_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_14_cb.Checked)
                pcl_1_cb.Checked = true;
            else
                if (!pcl_11_cb.Checked && !pcl_12_cb.Checked && !pcl_13_cb.Checked && !pcl_15_cb.Checked)
                    pcl_1_cb.Checked = false;
        }

        private void pcl_15_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_15_cb.Checked)
                pcl_1_cb.Checked = true;
            else
                if (!pcl_11_cb.Checked && !pcl_12_cb.Checked && !pcl_13_cb.Checked && !pcl_14_cb.Checked)
                    pcl_1_cb.Checked = false;
        }
    }
}
