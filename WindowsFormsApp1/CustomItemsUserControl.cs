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

        public CustomItemsUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.CiUC.Remove(this);
        }
    }
}
