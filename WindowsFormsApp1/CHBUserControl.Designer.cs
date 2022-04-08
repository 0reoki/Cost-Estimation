
namespace WindowsFormsApp1
{
    partial class CHBUserControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.chbUC_Length_Bx = new System.Windows.Forms.TextBox();
            this.chbUC_Height_Bx = new System.Windows.Forms.TextBox();
            this.chbUC_Length_Lbl = new System.Windows.Forms.Label();
            this.chbUC_Height_Lbl = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 188F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel1.Controls.Add(this.chbUC_Height_Lbl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.chbUC_Length_Lbl, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.chbUC_Height_Bx, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.chbUC_Length_Bx, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(331, 77);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(13, 18);
            this.button1.Margin = new System.Windows.Forms.Padding(13, 18, 13, 18);
            this.button1.Name = "button1";
            this.tableLayoutPanel1.SetRowSpan(this.button1, 2);
            this.button1.Size = new System.Drawing.Size(35, 41);
            this.button1.TabIndex = 0;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chbUC_Length_Bx
            // 
            this.chbUC_Length_Bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chbUC_Length_Bx.Location = new System.Drawing.Point(252, 41);
            this.chbUC_Length_Bx.Name = "chbUC_Length_Bx";
            this.chbUC_Length_Bx.Size = new System.Drawing.Size(76, 26);
            this.chbUC_Length_Bx.TabIndex = 26;
            // 
            // chbUC_Height_Bx
            // 
            this.chbUC_Height_Bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chbUC_Height_Bx.Location = new System.Drawing.Point(252, 3);
            this.chbUC_Height_Bx.Name = "chbUC_Height_Bx";
            this.chbUC_Height_Bx.Size = new System.Drawing.Size(76, 26);
            this.chbUC_Height_Bx.TabIndex = 27;
            // 
            // chbUC_Length_Lbl
            // 
            this.chbUC_Length_Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbUC_Length_Lbl.Location = new System.Drawing.Point(62, 38);
            this.chbUC_Length_Lbl.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.chbUC_Length_Lbl.Name = "chbUC_Length_Lbl";
            this.chbUC_Length_Lbl.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chbUC_Length_Lbl.Size = new System.Drawing.Size(184, 31);
            this.chbUC_Length_Lbl.TabIndex = 28;
            this.chbUC_Length_Lbl.Text = "Length of Window 999:";
            this.chbUC_Length_Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chbUC_Height_Lbl
            // 
            this.chbUC_Height_Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbUC_Height_Lbl.Location = new System.Drawing.Point(62, 0);
            this.chbUC_Height_Lbl.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.chbUC_Height_Lbl.Name = "chbUC_Height_Lbl";
            this.chbUC_Height_Lbl.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chbUC_Height_Lbl.Size = new System.Drawing.Size(184, 31);
            this.chbUC_Height_Lbl.TabIndex = 29;
            this.chbUC_Height_Lbl.Text = "Height of Window 999:";
            this.chbUC_Height_Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CHBUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CHBUserControl";
            this.Size = new System.Drawing.Size(337, 83);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox chbUC_Height_Bx;
        private System.Windows.Forms.TextBox chbUC_Length_Bx;
        private System.Windows.Forms.Label chbUC_Height_Lbl;
        private System.Windows.Forms.Label chbUC_Length_Lbl;
    }
}
