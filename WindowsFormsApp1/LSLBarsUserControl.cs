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
    public partial class LSLBarsUserControl : UserControl
    {
        ParametersForm parametersForm;
        public string barType;

        public LSLBarsUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;
        }

        public string lslUC_Label
        {
            set
            {
                lslUC_lbl.Text = value;
            }
            get
            {
                return lslUC_lbl.Text;
            }
        }

        public string lslUC_Value
        {
            set
            {
                lslUC_bx.Text = value;
            }
            get
            {
                return lslUC_bx.Text;
            }
        }
    }
}
