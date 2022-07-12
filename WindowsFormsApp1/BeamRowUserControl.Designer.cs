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
            this.clearlength_bx = new System.Windows.Forms.TextBox();
            this.length_bx = new System.Windows.Forms.TextBox();
            this.qty_bx = new System.Windows.Forms.TextBox();
            this.support_cbx = new System.Windows.Forms.ComboBox();
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
            this.tableLayoutPanel109.Controls.Add(this.clearlength_bx, 3, 0);
            this.tableLayoutPanel109.Controls.Add(this.length_bx, 2, 0);
            this.tableLayoutPanel109.Controls.Add(this.qty_bx, 1, 0);
            this.tableLayoutPanel109.Controls.Add(this.support_cbx, 4, 0);
            this.tableLayoutPanel109.Controls.Add(this.beamName_cbx, 0, 0);
            this.tableLayoutPanel109.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel109.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel109.Name = "tableLayoutPanel109";
            this.tableLayoutPanel109.RowCount = 1;
            this.tableLayoutPanel109.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel109.Size = new System.Drawing.Size(747, 40);
            this.tableLayoutPanel109.TabIndex = 41;
            // 
            // clearlength_bx
            // 
            this.clearlength_bx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clearlength_bx.Location = new System.Drawing.Point(311, 4);
            this.clearlength_bx.Name = "clearlength_bx";
            this.clearlength_bx.Size = new System.Drawing.Size(211, 26);
            this.clearlength_bx.TabIndex = 50;
            this.clearlength_bx.Text = "0";
            // 
            // length_bx
            // 
            this.length_bx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.length_bx.Location = new System.Drawing.Point(223, 4);
            this.length_bx.Name = "length_bx";
            this.length_bx.Size = new System.Drawing.Size(81, 26);
            this.length_bx.TabIndex = 49;
            this.length_bx.Text = "0";
            // 
            // qty_bx
            // 
            this.qty_bx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.qty_bx.Location = new System.Drawing.Point(135, 4);
            this.qty_bx.Name = "qty_bx";
            this.qty_bx.Size = new System.Drawing.Size(81, 26);
            this.qty_bx.TabIndex = 48;
            this.qty_bx.Text = "0";
            // 
            // support_cbx
            // 
            this.support_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.support_cbx.FormattingEnabled = true;
            this.support_cbx.Items.AddRange(new object[] {
            "1-End Support",
            "2-End Supports"});
            this.support_cbx.Location = new System.Drawing.Point(529, 4);
            this.support_cbx.Name = "support_cbx";
            this.support_cbx.Size = new System.Drawing.Size(211, 28);
            this.support_cbx.TabIndex = 47;
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
        private System.Windows.Forms.TextBox length_bx;
        private System.Windows.Forms.TextBox qty_bx;
        private System.Windows.Forms.ComboBox support_cbx;
        private System.Windows.Forms.ComboBox beamName_cbx;
        private System.Windows.Forms.TextBox clearlength_bx;
    }
}
