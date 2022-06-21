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
    public partial class EquipmentUserControl : UserControl
    {
        ParametersForm parametersForm;

        public EquipmentUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;
        }

        public string set_eqUC_cbx
        {
            set
            {
                eqUC_cbx.Text = value;
            }
            get
            {
                return eqUC_cbx.Text;
            }
        }

        public string set_eqUC_qty
        {
            set
            {
                eqUC_qty.Text = value;
            }
            get
            {
                return eqUC_qty.Text;
            }
        }

        public string set_eqUC_hrs
        {
            set
            {
                eqUC_hrs.Text = value;
            }
            get
            {
                return eqUC_hrs.Text;
            }
        }

        public string set_eqUC_days
        {
            set
            {
                eqUC_days.Text = value;
            }
            get
            {
                return eqUC_days.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.EqUC.Remove(this);
        }
    }
}
