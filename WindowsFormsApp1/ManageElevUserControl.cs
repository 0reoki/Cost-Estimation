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
    public partial class ManageElevUserControl : UserControl
    {
        //Local Variables

        //Passed Variables
        private ManageElevForm manageElevForm;

        public ManageElevUserControl(ManageElevForm manageElevForm)
        {
            InitializeComponent();
            this.manageElevForm = manageElevForm;
            elevLbl.Text = "Elevation " + (manageElevForm.Elevations.Count + 1);
        }
        
        public string elevLabel
        {
            set
            {
                elevLbl.Text = value;
            }
        }

        public string elev
        {
            set
            {
                elev_Elevations_bx.Text = value;
            }
            get
            {
                return elev_Elevations_bx.Text;
            }
        }

        public string elevArea
        {
            set
            {
                elev_Area_bx.Text = value;
            }
            get
            {
                return elev_Area_bx.Text;
            }
        }
        private void elevUCDeleteBtn_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            manageElevForm.Elevations.Remove(this);

            manageElevForm.refreshElevations();
        }
    }
}
