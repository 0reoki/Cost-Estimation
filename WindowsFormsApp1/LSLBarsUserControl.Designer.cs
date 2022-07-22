
namespace KnowEst
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
            this.lslUC_bx = new System.Windows.Forms.TextBox();
            this.lslUC_lbl = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.lslUC_bx, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lslUC_lbl, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(164, 69);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // lslUC_bx
            // 
            this.lslUC_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lslUC_bx.Location = new System.Drawing.Point(3, 37);
            this.lslUC_bx.Name = "lslUC_bx";
            this.lslUC_bx.Size = new System.Drawing.Size(158, 26);
            this.lslUC_bx.TabIndex = 17;
            // 
            // lslUC_lbl
            // 
            this.lslUC_lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lslUC_lbl.Location = new System.Drawing.Point(1, 0);
            this.lslUC_lbl.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.lslUC_lbl.Name = "lslUC_lbl";
            this.lslUC_lbl.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lslUC_lbl.Size = new System.Drawing.Size(160, 30);
            this.lslUC_lbl.TabIndex = 14;
            this.lslUC_lbl.Text = "LS99 ƒ\'c (MPa):";
            this.lslUC_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
        private System.Windows.Forms.Label lslUC_lbl;
        private System.Windows.Forms.TextBox lslUC_bx;
    }
}
