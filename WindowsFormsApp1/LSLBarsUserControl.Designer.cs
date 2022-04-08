
namespace WindowsFormsApp1
{
    partial class LSLBarsUserControl
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.conc_CC_SG_bx = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.conc_CC_SG_bx, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label16, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(164, 69);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // conc_CC_SG_bx
            // 
            this.conc_CC_SG_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conc_CC_SG_bx.Location = new System.Drawing.Point(3, 37);
            this.conc_CC_SG_bx.Name = "conc_CC_SG_bx";
            this.conc_CC_SG_bx.Size = new System.Drawing.Size(158, 26);
            this.conc_CC_SG_bx.TabIndex = 17;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(1, 0);
            this.label16.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label16.Name = "label16";
            this.label16.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label16.Size = new System.Drawing.Size(160, 30);
            this.label16.TabIndex = 14;
            this.label16.Text = "LS99 ƒ\'c (MPa):";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LSLBarsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "LSLBarsUserControl";
            this.Size = new System.Drawing.Size(167, 77);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox conc_CC_SG_bx;
    }
}
