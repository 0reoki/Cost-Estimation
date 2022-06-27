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
    public partial class PaintAreaUserControl : UserControl
    {
        ParametersForm parametersForm;

        public PaintAreaUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;

            paUC_lbl.Text = "Paint Area " + (parametersForm.PaUC.Count + 1);
        }
        public string setLabel
        {
            set
            {
                paUC_lbl.Text = value;
            }
            get
            {
                return paUC_lbl.Text;
            }
        }

        public string set_paUC_Area_bx
        {
            set
            {
                paUC_Area_bx.Text = value;
            }
            get
            {
                return paUC_Area_bx.Text;
            }
        }

        public string set_paUC_Paint_cbx
        {
            set
            {
                paUC_Paint_cbx.Text = value;
            }
            get
            {
                return paUC_Paint_cbx.Text;
            }
        }

        public string set_paUC_PL_bx
        {
            set
            {
                paUC_PL_bx.Text = value;
            }
            get
            {
                return paUC_PL_bx.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.PaUC.Remove(this);

            parametersForm.refreshPaint();
        }
    }
}
