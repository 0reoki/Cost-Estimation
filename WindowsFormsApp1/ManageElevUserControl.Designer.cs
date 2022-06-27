
namespace WindowsFormsApp1
{
    partial class ManageElevUserControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.elevLbl = new System.Windows.Forms.Label();
            this.elev_Area_bx = new System.Windows.Forms.TextBox();
            this.elev_Elevations_bx = new System.Windows.Forms.TextBox();
            this.elevUCDeleteBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.50293F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.49708F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.elevLbl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.elev_Area_bx, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.elev_Elevations_bx, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.elevUCDeleteBtn, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(296, 69);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(57, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(102, 31);
            this.label1.TabIndex = 18;
            this.label1.Text = "Area:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // elevLbl
            // 
            this.elevLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.elevLbl.Location = new System.Drawing.Point(57, 0);
            this.elevLbl.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.elevLbl.Name = "elevLbl";
            this.elevLbl.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.elevLbl.Size = new System.Drawing.Size(102, 31);
            this.elevLbl.TabIndex = 17;
            this.elevLbl.Text = "Elevations:";
            this.elevLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // elev_Area_bx
            // 
            this.elev_Area_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elev_Area_bx.Location = new System.Drawing.Point(165, 37);
            this.elev_Area_bx.Name = "elev_Area_bx";
            this.elev_Area_bx.Size = new System.Drawing.Size(128, 26);
            this.elev_Area_bx.TabIndex = 16;
            // 
            // elev_Elevations_bx
            // 
            this.elev_Elevations_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elev_Elevations_bx.Location = new System.Drawing.Point(165, 3);
            this.elev_Elevations_bx.Name = "elev_Elevations_bx";
            this.elev_Elevations_bx.Size = new System.Drawing.Size(128, 26);
            this.elev_Elevations_bx.TabIndex = 15;
            // 
            // elevUCDeleteBtn
            // 
            this.elevUCDeleteBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.elevUCDeleteBtn.Location = new System.Drawing.Point(3, 13);
            this.elevUCDeleteBtn.Margin = new System.Windows.Forms.Padding(3, 13, 3, 13);
            this.elevUCDeleteBtn.Name = "elevUCDeleteBtn";
            this.tableLayoutPanel1.SetRowSpan(this.elevUCDeleteBtn, 2);
            this.elevUCDeleteBtn.Size = new System.Drawing.Size(50, 43);
            this.elevUCDeleteBtn.TabIndex = 4;
            this.elevUCDeleteBtn.Text = "X";
            this.elevUCDeleteBtn.UseVisualStyleBackColor = true;
            this.elevUCDeleteBtn.Click += new System.EventHandler(this.elevUCDeleteBtn_Click);
            // 
            // ManageElevUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ManageElevUserControl";
            this.Size = new System.Drawing.Size(305, 79);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button elevUCDeleteBtn;
        private System.Windows.Forms.TextBox elev_Area_bx;
        private System.Windows.Forms.TextBox elev_Elevations_bx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label elevLbl;
    }
}
