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
    public partial class DialogRadioBox : Form
    {
        public string selectedString;

        public DialogRadioBox()
        {
            InitializeComponent();
        }
        public DialogRadioBox(string name, IList<string> lst)
        {
            InitializeComponent();
            this.Text = name;
            button1.Focus();
            for (int i = 0; i < lst.Count; i++)
            {
                RadioButton rdb = new RadioButton();
                rdb.Text = lst[i];
                rdb.Size = new Size(100, 30);
                
                panel.Controls.Add(rdb);
                rdb.Location = new Point(20, 20 + 35 * i);
                rdb.CheckedChanged += (s, ee) =>
                {
                    var r = s as RadioButton;
                    if (r.Checked)
                        this.selectedString = r.Text;
                };
                if (i == 0)
                {
                    rdb.Select();
                }
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
