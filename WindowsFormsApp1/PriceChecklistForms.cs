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
        CostEstimationForm costEstimationForm;

        public PriceChecklistForms(CostEstimationForm costEstimationForm)
        {
            InitializeComponent();
            this.costEstimationForm = costEstimationForm;
            //1.0 - Earthworks
            pcl_1_cb.Checked = costEstimationForm.earthworksChecklist[0];
            pcl_11_cb.Checked = costEstimationForm.earthworksChecklist[1];
            pcl_12_cb.Checked = costEstimationForm.earthworksChecklist[2];
            pcl_13_cb.Checked = costEstimationForm.earthworksChecklist[3];
            pcl_14_cb.Checked = costEstimationForm.earthworksChecklist[4];
            pcl_15_cb.Checked = costEstimationForm.earthworksChecklist[5];

            //2.0 - Concrete Works
            pcl_21_cb.Checked = costEstimationForm.concreteChecklist[0];
            pcl_22_cb.Checked = costEstimationForm.concreteChecklist[1];
            pcl_23_cb.Checked = costEstimationForm.concreteChecklist[2];
            pcl_24_cb.Checked = costEstimationForm.concreteChecklist[3];
            pcl_25_cb.Checked = costEstimationForm.concreteChecklist[4];
            pcl_26_cb.Checked = costEstimationForm.concreteChecklist[5];

            //3.0 - Form Works TODO

            //4.0 - Masonry
            pcl_41_cb.Checked = costEstimationForm.masonryChecklist[0];
            pcl_42_cb.Checked = costEstimationForm.masonryChecklist[1];

            //5.0 - Steel Reinforcement TODO

            //6.0 - Roofing
            pcl_61_cb.Checked = costEstimationForm.roofingsChecklist[0];
            pcl_62_cb.Checked = costEstimationForm.roofingsChecklist[1];
            pcl_63_cb.Checked = costEstimationForm.roofingsChecklist[2];

            //7.0 - Tile Works
            pcl_71_cb.Checked = costEstimationForm.tilesChecklist[0];
            pcl_72_cb.Checked = costEstimationForm.tilesChecklist[1];

            //8.0 - Paint Works
            pcl_81_cb.Checked = costEstimationForm.paintsChecklist[0];
            pcl_82_cb.Checked = costEstimationForm.paintsChecklist[1];
            pcl_83_cb.Checked = costEstimationForm.paintsChecklist[2];
            pcl_84_cb.Checked = costEstimationForm.paintsChecklist[3];

            //9.0 - Miscellaneous Items
            pcl_9_Panel.Controls.Clear();
            foreach (string[] data in costEstimationForm.parameters.misc_CustomItems)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Text = data[0];
                checkbox.AutoSize = true;
                checkbox.Checked = bool.Parse(data[3]);
                checkbox.CheckedChanged += new EventHandler(panel9_CheckHandler);
                pcl_9_Panel.Controls.Add(checkbox);
            }
            bool isAnyChecked = false;
            foreach (CheckBox checkbox in pcl_9_Panel.Controls)
            {
                if (checkbox.Checked) isAnyChecked = true;
            }
            pcl_9_cb.Checked = isAnyChecked;

            //10.0 - Additional Labor and Equipment
            pcl_101_Panel.Controls.Clear();
            //Manpower
            if (costEstimationForm.parameters.labor_RD.Equals("Manila Rate"))
            {
                foreach (string[] data in costEstimationForm.parameters.labor_MP)
                {
                    CheckBox checkbox = new CheckBox();
                    checkbox.Text = data[0];
                    checkbox.AutoSize = true;
                    checkbox.Checked = bool.Parse(data[4]);
                    checkbox.CheckedChanged += new EventHandler(panel10_CheckHandler);
                    pcl_101_Panel.Controls.Add(checkbox);
                }
            }
            else //Provincial
            {
                foreach (string[] data in costEstimationForm.parameters.labor_MP)
                {
                    CheckBox checkbox = new CheckBox();
                    checkbox.Text = data[0];
                    checkbox.AutoSize = true;
                    checkbox.Checked = bool.Parse(data[4]);
                    checkbox.CheckedChanged += new EventHandler(panel10_CheckHandler);
                    pcl_101_Panel.Controls.Add(checkbox);
                }
            }
            //Equipment
            foreach (string[] data in costEstimationForm.parameters.labor_EQP)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Text = data[0];
                checkbox.AutoSize = true;
                checkbox.Checked = bool.Parse(data[4]);
                checkbox.CheckedChanged += new EventHandler(panel10_CheckHandler);
                pcl_102_Panel.Controls.Add(checkbox);
            }
            isAnyChecked = false;
            foreach (CheckBox checkbox in pcl_101_Panel.Controls)
            {
                if (checkbox.Checked) isAnyChecked = true;
            }
            foreach (CheckBox checkbox in pcl_102_Panel.Controls)
            {
                if (checkbox.Checked) isAnyChecked = true;
            }
            pcl_10_cb.Checked = isAnyChecked;
        }

        private void priceCL_OKBtn_Click(object sender, EventArgs e)
        {
            //1.0 - Earthworks
            costEstimationForm.earthworksChecklist[0] = pcl_1_cb.Checked;
            costEstimationForm.earthworksChecklist[1] = pcl_11_cb.Checked;
            costEstimationForm.earthworksChecklist[2] = pcl_12_cb.Checked;
            costEstimationForm.earthworksChecklist[3] = pcl_13_cb.Checked;
            costEstimationForm.earthworksChecklist[4] = pcl_14_cb.Checked;
            costEstimationForm.earthworksChecklist[5] = pcl_15_cb.Checked;

            //2.0 - Concrete Works
            costEstimationForm.concreteChecklist[0] = pcl_21_cb.Checked;
            costEstimationForm.concreteChecklist[1] = pcl_22_cb.Checked;
            costEstimationForm.concreteChecklist[2] = pcl_23_cb.Checked;
            costEstimationForm.concreteChecklist[3] = pcl_24_cb.Checked;
            costEstimationForm.concreteChecklist[4] = pcl_25_cb.Checked;
            costEstimationForm.concreteChecklist[5] = pcl_26_cb.Checked;

            //3.0 - Form Works TODO

            //4.0 - Masonry
            costEstimationForm.masonryChecklist[0] = pcl_41_cb.Checked;
            costEstimationForm.masonryChecklist[1] = pcl_42_cb.Checked;

            //5.0 - Steel Reinforcement TODO

            //6.0 - Roofing
            costEstimationForm.roofingsChecklist[0] = pcl_61_cb.Checked;
            costEstimationForm.roofingsChecklist[1] = pcl_62_cb.Checked;
            costEstimationForm.roofingsChecklist[2] = pcl_63_cb.Checked;

            //7.0 - Tile Works
            costEstimationForm.tilesChecklist[0] = pcl_71_cb.Checked;
            costEstimationForm.tilesChecklist[1] = pcl_72_cb.Checked;

            //8.0 - Paint Works
            costEstimationForm.paintsChecklist[0] = pcl_81_cb.Checked;
            costEstimationForm.paintsChecklist[1] = pcl_82_cb.Checked;
            costEstimationForm.paintsChecklist[2] = pcl_83_cb.Checked;
            costEstimationForm.paintsChecklist[3] = pcl_84_cb.Checked;

            //9.0 - Miscellaneous Items
            int i = 0;
            foreach(CheckBox checkbox in pcl_9_Panel.Controls)
            {
                costEstimationForm.parameters.misc_CustomItems[i][3] = checkbox.Checked.ToString();
                i++;
            }

            //10.0 - Additional Labor and Equipment
            i = 0;
            foreach (CheckBox checkbox in pcl_101_Panel.Controls)
            {
                costEstimationForm.parameters.labor_MP[i][4] = checkbox.Checked.ToString();
                i++;
            }
            i = 0;
            foreach (CheckBox checkbox in pcl_102_Panel.Controls)
            {
                costEstimationForm.parameters.labor_EQP[i][4] = checkbox.Checked.ToString();
                i++;
            }

            this.DialogResult = DialogResult.OK;
        }

        //1.0 - Earthworks -- START
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
        //1.0 - Earthworks -- END

        //2.0 - Concrete Works -- START
        private void pcl_2_cb_Click(object sender, EventArgs e)
        {
            if (pcl_2_cb.Checked)
            {
                pcl_21_cb.Checked = true;
                pcl_22_cb.Checked = true;
                pcl_23_cb.Checked = true;
                pcl_24_cb.Checked = true;
                pcl_25_cb.Checked = true;
                pcl_26_cb.Checked = true;
            }
            else
            {
                pcl_21_cb.Checked = false;
                pcl_22_cb.Checked = false;
                pcl_23_cb.Checked = false;
                pcl_24_cb.Checked = false;
                pcl_25_cb.Checked = false;
                pcl_26_cb.Checked = false;
            }
        }

        private void pcl_21_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_21_cb.Checked)
                pcl_2_cb.Checked = true;
            else
                if (!pcl_22_cb.Checked && !pcl_23_cb.Checked && !pcl_24_cb.Checked && 
                    !pcl_25_cb.Checked && !pcl_26_cb.Checked)
                    pcl_2_cb.Checked = false;
        }

        private void pcl_22_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_22_cb.Checked)
                pcl_2_cb.Checked = true;
            else
                if (!pcl_21_cb.Checked && !pcl_23_cb.Checked && !pcl_24_cb.Checked &&
                    !pcl_25_cb.Checked && !pcl_26_cb.Checked)
                pcl_2_cb.Checked = false;
        }

        private void pcl_23_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_23_cb.Checked)
                pcl_2_cb.Checked = true;
            else
                if (!pcl_21_cb.Checked && !pcl_22_cb.Checked && !pcl_24_cb.Checked &&
                    !pcl_25_cb.Checked && !pcl_26_cb.Checked)
                pcl_2_cb.Checked = false;
        }

        private void pcl_24_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_24_cb.Checked)
                pcl_2_cb.Checked = true;
            else
                if (!pcl_21_cb.Checked && !pcl_22_cb.Checked && !pcl_23_cb.Checked &&
                    !pcl_25_cb.Checked && !pcl_26_cb.Checked)
                pcl_2_cb.Checked = false;
        }

        private void pcl_25_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_25_cb.Checked)
                pcl_2_cb.Checked = true;
            else
                if (!pcl_21_cb.Checked && !pcl_22_cb.Checked && !pcl_23_cb.Checked &&
                    !pcl_24_cb.Checked && !pcl_26_cb.Checked)
                pcl_2_cb.Checked = false;
        }

        private void pcl_26_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_26_cb.Checked)
                pcl_2_cb.Checked = true;
            else
                if (!pcl_21_cb.Checked && !pcl_22_cb.Checked && !pcl_23_cb.Checked &&
                    !pcl_24_cb.Checked && !pcl_25_cb.Checked)
                pcl_2_cb.Checked = false;
        }
        //2.0 - Concrete Works -- END

        //3.0 - Form Works -- START
        private void pcl_3_cb_Click(object sender, EventArgs e)
        {

        }
        //3.0 - Form Works -- END

        //4.0 - Masonry -- START
        private void pcl_4_cb_Click(object sender, EventArgs e)
        {
            if (pcl_4_cb.Checked)
            {
                pcl_41_cb.Checked = true;
                pcl_42_cb.Checked = true;
            }
            else
            {
                pcl_41_cb.Checked = false;
                pcl_42_cb.Checked = false;
            }
        }

        private void pcl_41_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_41_cb.Checked)
                pcl_4_cb.Checked = true;
            else
                if (!pcl_41_cb.Checked && !pcl_42_cb.Checked)
                    pcl_4_cb.Checked = false;
        }

        private void pcl_42_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_42_cb.Checked)
                pcl_4_cb.Checked = true;
            else
                if (!pcl_41_cb.Checked)
                    pcl_4_cb.Checked = false;
        }

        //4.0 - Masonry -- END

        //5.0 - Steel Reinforcement -- START
        private void pcl_5_cb_Click(object sender, EventArgs e)
        {
            
        }
        //5.0 - Steel Reinforcement -- END

        //6.0 - Roofing -- START
        private void pcl_6_cb_Click(object sender, EventArgs e)
        {
            if (pcl_6_cb.Checked)
            {
                pcl_61_cb.Checked = true;
                pcl_62_cb.Checked = true;
                pcl_63_cb.Checked = true;
            }
            else
            {
                pcl_61_cb.Checked = false;
                pcl_62_cb.Checked = false;
                pcl_63_cb.Checked = false;
            }
        }

        private void pcl_61_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_61_cb.Checked)
                pcl_6_cb.Checked = true;
            else
                if (!pcl_62_cb.Checked && !pcl_63_cb.Checked)
                    pcl_6_cb.Checked = false;
        }

        private void pcl_62_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_62_cb.Checked)
                pcl_6_cb.Checked = true;
            else
                if (!pcl_61_cb.Checked && !pcl_63_cb.Checked)
                    pcl_6_cb.Checked = false;
        }

        private void pcl_63_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_63_cb.Checked)
                pcl_6_cb.Checked = true;
            else
                if (!pcl_61_cb.Checked && !pcl_62_cb.Checked)
                    pcl_6_cb.Checked = false;
        }
        //6.0 - Roofing -- END

        //7.0 - Tile Works -- START
        private void pcl_7_cb_Click(object sender, EventArgs e)
        {
            if (pcl_7_cb.Checked)
            {
                pcl_71_cb.Checked = true;
                pcl_72_cb.Checked = true;
            }
            else
            {
                pcl_71_cb.Checked = false;
                pcl_72_cb.Checked = false;
            }
        }

        private void pcl_71_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_71_cb.Checked)
                pcl_7_cb.Checked = true;
            else
                if (!pcl_72_cb.Checked)
                    pcl_7_cb.Checked = false;
        }

        private void pcl_72_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_72_cb.Checked)
                pcl_7_cb.Checked = true;
            else
                if (!pcl_71_cb.Checked)
                    pcl_7_cb.Checked = false;
        }
        //7.0 - Tile Works -- END

        //8.0 - Paint Works -- START
        private void pcl_8_cb_Click(object sender, EventArgs e)
        {
            if (pcl_8_cb.Checked)
            {
                pcl_81_cb.Checked = true;
                pcl_82_cb.Checked = true;
                pcl_83_cb.Checked = true;
                pcl_84_cb.Checked = true;
            }
            else
            {
                pcl_81_cb.Checked = false;
                pcl_82_cb.Checked = false;
                pcl_83_cb.Checked = false;
                pcl_84_cb.Checked = false;
            }
        }

        private void pcl_81_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_81_cb.Checked)
                pcl_8_cb.Checked = true;
            else
                if (!pcl_82_cb.Checked && !pcl_83_cb.Checked && !pcl_84_cb.Checked)
                    pcl_8_cb.Checked = false;
        }

        private void pcl_82_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_82_cb.Checked)
                pcl_8_cb.Checked = true;
            else
                if (!pcl_81_cb.Checked && !pcl_83_cb.Checked && !pcl_84_cb.Checked)
                    pcl_8_cb.Checked = false;
        }

        private void pcl_83_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_83_cb.Checked)
                pcl_8_cb.Checked = true;
            else
                if (!pcl_81_cb.Checked && !pcl_82_cb.Checked && !pcl_84_cb.Checked)
                    pcl_8_cb.Checked = false;
        }

        private void pcl_84_cb_CheckedChanged(object sender, EventArgs e)
        {
            if (pcl_84_cb.Checked)
                pcl_8_cb.Checked = true;
            else
                if (!pcl_81_cb.Checked && !pcl_82_cb.Checked && !pcl_83_cb.Checked)
                    pcl_8_cb.Checked = false;
        }
        //8.0 - Paint Works -- END

        //9.0 - Miscellaneous Items -- START
        private void panel9_CheckHandler(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                pcl_9_cb.Checked = true;
            else
            {
                bool isAnyChecked = false;
                foreach (CheckBox checkbox in pcl_9_Panel.Controls)
                {
                    if (checkbox.Checked) isAnyChecked = true;
                }
                pcl_9_cb.Checked = isAnyChecked;
            }
        }

        private void pcl_9_cb_Click(object sender, EventArgs e)
        {
            if (pcl_9_cb.Checked)
            {
                foreach (CheckBox checkbox in pcl_9_Panel.Controls)
                {
                    checkbox.Checked = true;
                }
            }
            else
            {
                foreach (CheckBox checkbox in pcl_9_Panel.Controls)
                {
                    checkbox.Checked = false;
                }
            }
        }

        //9.0 - Miscellaneous Items -- END

        //10.0 - Additional Labor and Equipment -- START
        private void panel10_CheckHandler(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                pcl_10_cb.Checked = true;
            else
            {
                bool isAnyChecked = false;
                foreach (CheckBox checkbox in pcl_101_Panel.Controls)
                {
                    if (checkbox.Checked) isAnyChecked = true;
                }
                foreach (CheckBox checkbox in pcl_102_Panel.Controls)
                {
                    if (checkbox.Checked) isAnyChecked = true;
                }
                pcl_10_cb.Checked = isAnyChecked;
            }
        }

        private void pcl_10_cb_Click(object sender, EventArgs e)
        {
            if (pcl_10_cb.Checked)
            {
                foreach (CheckBox checkbox in pcl_101_Panel.Controls)
                {
                    checkbox.Checked = true;
                }
                foreach (CheckBox checkbox in pcl_102_Panel.Controls)
                {
                    checkbox.Checked = true;
                }
            }
            else
            {
                foreach (CheckBox checkbox in pcl_101_Panel.Controls)
                {
                    checkbox.Checked = false;
                }
                foreach (CheckBox checkbox in pcl_102_Panel.Controls)
                {
                    checkbox.Checked = false;
                }
            }
        }
        //10.0 - Additional Labor and Equipment -- END
    }
}
