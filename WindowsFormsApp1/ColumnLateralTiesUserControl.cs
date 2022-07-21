﻿using System;
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
    public partial class ColumnLateralTiesUserControl : UserControl
    {
        public ColumnLateralTiesUserControl(Image picture)
        {
            InitializeComponent();
            lateralTies_pb.Image = picture;
        }

        public string qty
        {
            get
            {
                return lateralTies_bx.Text;
            }
            set
            {
                lateralTies_bx.Text = value;
            }
        }
    }
}
