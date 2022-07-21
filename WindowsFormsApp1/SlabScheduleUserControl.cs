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
    public partial class SlabScheduleUserControl : UserControl
    {
        private AddStructForm asF;
        public string oldName;
        private bool initialized;

        public SlabScheduleUserControl(AddStructForm asF)
        {
            InitializeComponent();
            SS_REMARK.SelectedIndex = 0;
            this.asF = asF;
            initialized = false;
            SS_SM.Text = "S-" + (asF.ss_UC.Count + 1);
            oldName = SS_SM.Text;
        }

        public string slabMark

        {
            get
            {
                return SS_SM.Text;
            }
            set
            {
                SS_SM.Text = value;
            }
        }
        public string thickness
        {
            get
            {
                return SS_T.Text;
            }
            set
            {
                SS_T.Text = value;
            }
        }
        public string RSASD_S
        {
            get
            {
                return SS_RSASD_S.Text;
            }
            set
            {
                SS_RSASD_S.Text = value;
            }
        }
        public string RSASD_ES_T
        {
            get
            {
                return SS_RSASD_ES_T.Text;
            }
            set
            {
                SS_RSASD_ES_T.Text = value;
            }
        }
        public string RSASD_ES_B
        {
            get
            {
                return SS_RSASD_ES_B.Text;
            }
            set
            {
                SS_RSASD_ES_B.Text = value;
            }
        }
        public string RSASD_MS_T
        {
            get
            {
                return SS_RSASD_MS_T.Text;
            }
            set
            {
                SS_RSASD_MS_T.Text = value;
            }
        }
        public string RSASD_MS_B
        {
            get
            {
                return SS_RSASD_MS_B.Text;
            }
            set
            {
                SS_RSASD_MS_B.Text = value;
            }
        }
        public string RSASD_IS_T
        {
            get
            {
                return SS_RSASD_IS_T.Text;
            }
            set
            {
                SS_RSASD_IS_T.Text = value;
            }
        }
        public string RSASD_IS_B
        {
            get
            {
                return SS_RSASD_IS_B.Text;
            }
            set
            {
                SS_RSASD_IS_B.Text = value;
            }
        }
        public string RSALD_S
        {
            get
            {
                return SS_RSALD_S.Text;
            }
            set
            {
                SS_RSALD_S.Text = value;
            }
        }
        public string RSALD_ES_T
        {
            get
            {
                return SS_RSALD_ES_T.Text;
            }
            set
            {
                SS_RSALD_ES_T.Text = value;
            }
        }
        public string RSALD_ES_B
        {
            get
            {
                return SS_RSALD_ES_B.Text;
            }
            set
            {
                SS_RSALD_ES_B.Text = value;
            }
        }
        public string RSALD_MS_T
        {
            get
            {
                return SS_RSALD_MS_T.Text;
            }
            set
            {
                SS_RSALD_MS_T.Text = value;
            }
        }
        public string RSALD_MS_B
        {
            get
            {
                return SS_RSALD_MS_B.Text;
            }
            set
            {
                SS_RSALD_MS_B.Text = value;
            }
        }
        public string RSALD_IS_T
        {
            get
            {
                return SS_RSALD_IS_T.Text;
            }
            set
            {
                SS_RSALD_IS_T.Text = value;
            }
        }
        public string RSALD_IS_B
        {
            get
            {
                return SS_RSALD_IS_B.Text;
            }
            set
            {
                SS_RSALD_IS_B.Text = value;
            }
        }

        public string remark
        {
            get
            {
                return SS_REMARK.Text;
            }
            set
            {
                SS_REMARK.Text = value;
            }
        }

        private void SS_SM_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void SS_SM_KeyUp(object sender, KeyEventArgs e)
        {
            int found = 0;
            foreach (SlabScheduleUserControl ss in asF.ss_UC)
            {
                if (ss.slabMark == SS_SM.Text)
                {
                    found++;
                }
                if (found == 2)
                {
                    MessageBox.Show("Duplicate names inside schedule are not allowed!");
                    SS_SM.Text = oldName;
                    asF.updateSlabMark(oldName, slabMark);
                    oldName = SS_SM.Text;
                    return;
                }
            }
            if (slabMark == "")
            {
                slabMark = oldName;
            }
            asF.updateSlabMark(oldName, slabMark);
            oldName = SS_SM.Text;
        }
    }
}
