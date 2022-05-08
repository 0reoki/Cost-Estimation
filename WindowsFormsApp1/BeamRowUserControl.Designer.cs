namespace WindowsFormsApp1
{
    partial class BeamRowUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel109 = new System.Windows.Forms.TableLayoutPanel();
            this.qty_bx = new System.Windows.Forms.TextBox();
            this.length_bx = new System.Windows.Forms.TextBox();
            this.endSupportRB_cbx = new System.Windows.Forms.ComboBox();
            this.endSupportLT_cbx = new System.Windows.Forms.ComboBox();
            this.beamName_cbx = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel109.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel109
            // 
            this.tableLayoutPanel109.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel109.ColumnCount = 5;
            this.tableLayoutPanel109.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.64706F));
            this.tableLayoutPanel109.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.76471F));
            this.tableLayoutPanel109.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.76471F));
            this.tableLayoutPanel109.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.41176F));
            this.tableLayoutPanel109.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.41176F));
            this.tableLayoutPanel109.Controls.Add(this.qty_bx, 2, 0);
            this.tableLayoutPanel109.Controls.Add(this.length_bx, 1, 0);
            this.tableLayoutPanel109.Controls.Add(this.endSupportRB_cbx, 4, 0);
            this.tableLayoutPanel109.Controls.Add(this.endSupportLT_cbx, 3, 0);
            this.tableLayoutPanel109.Controls.Add(this.beamName_cbx, 0, 0);
            this.tableLayoutPanel109.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel109.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel109.Name = "tableLayoutPanel109";
            this.tableLayoutPanel109.RowCount = 1;
            this.tableLayoutPanel109.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel109.Size = new System.Drawing.Size(747, 40);
            this.tableLayoutPanel109.TabIndex = 41;
            // 
            // qty_bx
            // 
            this.qty_bx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.qty_bx.Location = new System.Drawing.Point(223, 4);
            this.qty_bx.Name = "qty_bx";
            this.qty_bx.Size = new System.Drawing.Size(81, 26);
            this.qty_bx.TabIndex = 49;
            this.qty_bx.Text = "0";
            // 
            // length_bx
            // 
            this.length_bx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.length_bx.Location = new System.Drawing.Point(135, 4);
            this.length_bx.Name = "length_bx";
            this.length_bx.Size = new System.Drawing.Size(81, 26);
            this.length_bx.TabIndex = 48;
            this.length_bx.Text = "0";
            // 
            // endSupportRB_cbx
            // 
            this.endSupportRB_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.endSupportRB_cbx.FormattingEnabled = true;
            this.endSupportRB_cbx.Items.AddRange(new object[] {
            "None"});
            this.endSupportRB_cbx.Location = new System.Drawing.Point(529, 4);
            this.endSupportRB_cbx.Name = "endSupportRB_cbx";
            this.endSupportRB_cbx.Size = new System.Drawing.Size(211, 28);
            this.endSupportRB_cbx.TabIndex = 47;
            // 
            // endSupportLT_cbx
            // 
            this.endSupportLT_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.endSupportLT_cbx.FormattingEnabled = true;
            this.endSupportLT_cbx.Items.AddRange(new object[] {
            "None"});
            this.endSupportLT_cbx.Location = new System.Drawing.Point(311, 4);
            this.endSupportLT_cbx.Name = "endSupportLT_cbx";
            this.endSupportLT_cbx.Size = new System.Drawing.Size(211, 28);
            this.endSupportLT_cbx.TabIndex = 46;
            // 
            // beamName_cbx
            // 
            this.beamName_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.beamName_cbx.FormattingEnabled = true;
            this.beamName_cbx.Items.AddRange(new object[] {
            "None"});
            this.beamName_cbx.Location = new System.Drawing.Point(4, 4);
            this.beamName_cbx.Name = "beamName_cbx";
            this.beamName_cbx.Size = new System.Drawing.Size(124, 28);
            this.beamName_cbx.TabIndex = 45;
            // 
            // BeamRowUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel109);
            this.Name = "BeamRowUserControl";
            this.Size = new System.Drawing.Size(747, 40);
            this.tableLayoutPanel109.ResumeLayout(false);
            this.tableLayoutPanel109.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel109;
        private System.Windows.Forms.TextBox qty_bx;
        private System.Windows.Forms.TextBox length_bx;
        private System.Windows.Forms.ComboBox endSupportRB_cbx;
        private System.Windows.Forms.ComboBox endSupportLT_cbx;
        private System.Windows.Forms.ComboBox beamName_cbx;
    }
}
