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
    public partial class CustomItemsUserControl : UserControl
    {
        ParametersForm parametersForm;
        Parameters parameters;

        public CustomItemsUserControl(ParametersForm parametersForm, Parameters parameters)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;
            this.parameters = parameters;

            ciUC_cbx.Items.Clear();
            ciUC_cbx.Items.AddRange(parameters.customItemsList.ToArray());

            ciUC_cbx.DropDownWidth = DropDownWidth(ciUC_cbx);
        }

        public string set_ciUC_cbx
        {
            set
            {
                ciUC_cbx.Text = value;
            }
            get
            {
                return ciUC_cbx.Text;
            }
        }

        public string set_ciUC_qty
        {
            set
            {
                ciUC_qty.Text = value;
            }
            get
            {
                return ciUC_qty.Text;
            }
        }

        public string set_ciUC_price
        {
            set
            {
                ciUC_price.Text = value;
            }
            get
            {
                return ciUC_price.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.CiUC.Remove(this);
        }

        private void ciUC_cbx_TextChanged(object sender, EventArgs e)
        {
            if (!parameters.customItemsList.ToArray().Contains(ciUC_cbx.Text))
            {
                ciUC_price.Enabled = true;
            }
            else
            {
                string[] data = ciUC_cbx.Text.Split(new string[] { "] - " }, StringSplitOptions.None);
                string name = data[0] + "]";
                string category = data[1];

                ciUC_price.Enabled = false;

                if (category.Equals("Common Materials"))                        //1
                    ciUC_price.Text = parameters.price_CommonMaterials[name].ToString();
                else if (category.Equals("Paint and Coating"))                  //2
                    ciUC_price.Text = parameters.price_PaintAndCoating[name].ToString();
                else if (category.Equals("Welding Rod"))                        //3
                    ciUC_price.Text = parameters.price_WeldingRod[name].ToString();
                else if (category.Equals("Personal Protective Equipment"))      //4
                    ciUC_price.Text = parameters.price_PersonalProtectiveEquipment[name].ToString();
                else if (category.Equals("Tools"))                              //5
                    ciUC_price.Text = parameters.price_Tools[name].ToString();
                else if (category.Equals("Ready Mix Concrete"))                 //6
                    ciUC_price.Text = parameters.price_ReadyMixConcrete[name].ToString();
                else if (category.Equals("Gravel"))                             //7
                    ciUC_price.Text = parameters.price_Gravel[name].ToString();
                else if (category.Equals("Formworks and Lumber"))               //8
                    ciUC_price.Text = parameters.price_FormworksAndLumber[name].ToString();
                else if (category.Equals("Roof Materials"))                     //9
                    ciUC_price.Text = parameters.price_RoofMaterials[name].ToString();
                else if (category.Equals("Tubular Steel (1mm thick)"))          //10
                    ciUC_price.Text = parameters.price_TubularSteel1mm[name].ToString();
                else if (category.Equals("Tubular Steel (1.2mm thick)"))        //11
                    ciUC_price.Text = parameters.price_TubularSteel1p2mm[name].ToString();
                else if (category.Equals("Tubular Steel (1.5mm thick)"))        //12
                    ciUC_price.Text = parameters.price_TubularSteel1p5mm[name].ToString();
                else if (category.Equals("Embankment"))                         //13
                    ciUC_price.Text = parameters.price_Embankment[name].ToString();
                else if (category.Equals("Rebar Grade 33 (230 Mpa)"))           //14
                    ciUC_price.Text = parameters.price_RebarGrade33[name].ToString();
                else if (category.Equals("Rebar Grade 40 (275 Mpa)"))           //15
                    ciUC_price.Text = parameters.price_RebarGrade40[name].ToString();
                else if (category.Equals("Rebar Grade 60 (415 Mpa)"))           //16
                    ciUC_price.Text = parameters.price_RebarGrade60[name].ToString();
            }
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
