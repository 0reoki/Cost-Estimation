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
    public partial class SlabScheduleUserControl : UserControl
    {
        public SlabScheduleUserControl()
        {
            InitializeComponent();
            SS_REMARK.SelectedIndex = 0;
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
    }
}
