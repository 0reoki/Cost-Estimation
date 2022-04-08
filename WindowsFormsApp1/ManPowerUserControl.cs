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
    public partial class ManPowerUserControl : UserControl
    {
        ParametersForm parametersForm;

        public ManPowerUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;
        }

        public string set_mpUC_cbx
        {
            set
            {
                mpUC_cbx.Text = value;
            }
            get
            {
                return mpUC_cbx.Text;
            }
        }

        public string set_mpUC_qty
        {
            set
            {
                mpUC_qty.Text = value;
            }
            get
            {
                return mpUC_qty.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.MpUC.Remove(this);
        }
    }
}
