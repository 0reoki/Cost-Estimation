using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowEst
{
    public partial class ManPowerUserControl : UserControl
    {
        ParametersForm parametersForm;
        public bool checkList;

        public ManPowerUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;
            checkList = true;
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

        public string set_mpUC_hrs
        {
            set
            {
                mpUC_hrs.Text = value;
            }
            get
            {
                return mpUC_hrs.Text;
            }
        }

        public string set_mpUC_days
        {
            set
            {
                mpUC_days.Text = value;
            }
            get
            {
                return mpUC_days.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.MpUC.Remove(this);
        }
    }
}
