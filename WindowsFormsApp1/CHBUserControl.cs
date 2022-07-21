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
    public partial class CHBUserControl : UserControl
    {
        ParametersForm parametersForm;
        public string wallType, chbType;

        public CHBUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;
        }

        public string height_Lbl {
            set
            {
                chbUC_Height_Lbl.Text = value;
            }
            get
            {
                return chbUC_Height_Lbl.Text;
            }
        }

        public string length_Lbl
        {
            set
            {
                chbUC_Length_Lbl.Text = value;
            }
            get
            {
                return chbUC_Length_Lbl.Text;
            }
        }

        public string height_Bx
        {
            set
            {
                chbUC_Height_Bx.Text = value;
            }
            get
            {
                return chbUC_Height_Bx.Text;
            }
        }

        public string length_Bx
        {
            set
            {
                chbUC_Length_Bx.Text = value;
            }
            get
            {
                return chbUC_Length_Bx.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.ChbUC.Remove(this);

            parametersForm.refreshCHB();
        }
    }
}
