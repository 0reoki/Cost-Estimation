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
    public partial class ManageElevForm : Form
    {
        //Forms

        //Local Variables
        private List<ManageElevUserControl> elevations = new List<ManageElevUserControl>();

        //Getters and Setters
        public List<ManageElevUserControl> Elevations { get => elevations; set => elevations = value; }


        public ManageElevForm()
        {
            InitializeComponent();
        }


        private void manageElevAddBtn_Click(object sender, EventArgs e)
        {
            ManageElevUserControl content = new ManageElevUserControl(this);
            elevations.Add(content);
            //Default Values
            manageElevPanel.Controls.Add(content);
        }

        public void refreshElevations()
        {
            //Remove all controls
            for (int i = 0; i < elevations.Count; i++)
            {
                manageElevPanel.Controls.Remove(elevations[i]);
            }

            //Add all controls
            for (int i = 0; i < elevations.Count; i++)
            {
                elevations[i].elevLabel = "Elevation " + (i + 1);
                manageElevPanel.Controls.Add(elevations[i]);
            }
        }

        public void clearElevations()
        {
            manageElevPanel.Controls.Clear();
        }

        private void ManageElevForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for(int i = 0; i < elevations.Count; i++)
            {
                if (elevations[i].elev.Equals("") || elevations[i].elevArea.Equals(""))
                {
                    MessageBox.Show("Please fill all the fields or delete empty fields.");
                    e.Cancel = (e.CloseReason == CloseReason.UserClosing);
                    break;
                }
            }
        }

        private void manageElevSaveBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
