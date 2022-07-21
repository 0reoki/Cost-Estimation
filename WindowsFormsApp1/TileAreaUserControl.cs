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
    public partial class TileAreaUserControl : UserControl
    {
        ParametersForm parametersForm;

        public TileAreaUserControl(ParametersForm parametersForm)
        {
            InitializeComponent();
            this.parametersForm = parametersForm;

            taUC_lbl.Text = "Tile Area " + (parametersForm.TaUC.Count + 1);
        }

        public string setLabel
        {
            set
            {
                taUC_lbl.Text = value;
            }
            get
            {
                return taUC_lbl.Text;
            }
        }

        public string set_taUC_bx
        {
            set
            {
                taUC_Area_bx.Text = value;
            }
            get
            {
                return taUC_Area_bx.Text;
            }
        }

        public string set_tdUC_cbx
        {
            set
            {
                tdUC_cbx.Text = value;
            }
            get
            {
                return tdUC_cbx.Text;
            }
        }

        public string set_taUC_cbx
        {
            set
            {
                taUC_cbx.Text = value;
            }
            get
            {
                return taUC_cbx.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            parametersForm.TaUC.Remove(this);

            parametersForm.refreshTiles();
        }
    }
}