namespace WindowsFormsApp1
{
    partial class LaborAndEquipmentUserControl
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
            this.label3 = new System.Windows.Forms.Label();
            this.laq_days_bx = new System.Windows.Forms.TextBox();
            this.laq_hrs_bx = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.laq_Label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.price_Label = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.laq_days_bx, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.laq_hrs_bx, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.laq_Label, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.price_Label, 6, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1100, 45);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(46, 31);
            this.label3.TabIndex = 32;
            this.label3.Text = "L..";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // laq_days_bx
            // 
            this.laq_days_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.laq_days_bx.Location = new System.Drawing.Point(603, 3);
            this.laq_days_bx.Name = "laq_days_bx";
            this.laq_days_bx.Size = new System.Drawing.Size(94, 26);
            this.laq_days_bx.TabIndex = 30;
            this.laq_days_bx.Text = "7";
            this.laq_days_bx.TextChanged += new System.EventHandler(this.laq_days_bx_TextChanged);
            // 
            // laq_hrs_bx
            // 
            this.laq_hrs_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.laq_hrs_bx.Location = new System.Drawing.Point(403, 3);
            this.laq_hrs_bx.Name = "laq_hrs_bx";
            this.laq_hrs_bx.Size = new System.Drawing.Size(94, 26);
            this.laq_hrs_bx.TabIndex = 25;
            this.laq_hrs_bx.Text = "8";
            this.laq_hrs_bx.TextChanged += new System.EventHandler(this.laq_hrs_bx_TextChanged);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(501, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label8.Size = new System.Drawing.Size(96, 31);
            this.label8.TabIndex = 24;
            this.label8.Text = "HRS   X";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // laq_Label
            // 
            this.laq_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.laq_Label.Location = new System.Drawing.Point(51, 0);
            this.laq_Label.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.laq_Label.Name = "laq_Label";
            this.laq_Label.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.laq_Label.Size = new System.Drawing.Size(346, 31);
            this.laq_Label.TabIndex = 28;
            this.laq_Label.Text = "FOREMAN [MNL] X 1";
            this.laq_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(701, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(96, 31);
            this.label1.TabIndex = 29;
            this.label1.Text = "DAYS";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // price_Label
            // 
            this.price_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.price_Label.Location = new System.Drawing.Point(801, 0);
            this.price_Label.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.price_Label.Name = "price_Label";
            this.price_Label.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.price_Label.Size = new System.Drawing.Size(296, 31);
            this.price_Label.TabIndex = 31;
            this.price_Label.Text = "P 1000";
            this.price_Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LaborAndEquipmentUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LaborAndEquipmentUserControl";
            this.Size = new System.Drawing.Size(1100, 45);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox laq_hrs_bx;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label laq_Label;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox laq_days_bx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label price_Label;
    }
}
