﻿
namespace WindowsFormsApp1
{
    partial class EquipmentUserControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.eqUC_days = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.eqUC_hrs = new System.Windows.Forms.TextBox();
            this.eqUC_qty = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.eqUC_cbx = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 290F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.eqUC_days, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.eqUC_hrs, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.eqUC_qty, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.eqUC_cbx, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(820, 45);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(661, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(96, 31);
            this.label2.TabIndex = 29;
            this.label2.Text = "Days:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // eqUC_days
            // 
            this.eqUC_days.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eqUC_days.Location = new System.Drawing.Point(763, 3);
            this.eqUC_days.Name = "eqUC_days";
            this.eqUC_days.Size = new System.Drawing.Size(54, 26);
            this.eqUC_days.TabIndex = 28;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(501, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(96, 31);
            this.label1.TabIndex = 27;
            this.label1.Text = "Hours:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // eqUC_hrs
            // 
            this.eqUC_hrs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eqUC_hrs.Location = new System.Drawing.Point(603, 3);
            this.eqUC_hrs.Name = "eqUC_hrs";
            this.eqUC_hrs.Size = new System.Drawing.Size(54, 26);
            this.eqUC_hrs.TabIndex = 26;
            // 
            // eqUC_qty
            // 
            this.eqUC_qty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eqUC_qty.Location = new System.Drawing.Point(443, 3);
            this.eqUC_qty.Name = "eqUC_qty";
            this.eqUC_qty.Size = new System.Drawing.Size(54, 26);
            this.eqUC_qty.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(341, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label8.Size = new System.Drawing.Size(96, 31);
            this.label8.TabIndex = 24;
            this.label8.Text = "QTY:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // eqUC_cbx
            // 
            this.eqUC_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eqUC_cbx.FormattingEnabled = true;
            this.eqUC_cbx.Items.AddRange(new object[] {
            "Crawler Loader (80kW/ 1.5 - 2.0 cu.m) [hr]",
            "",
            "Crawler Dozer (125kW) [hr]",
            "",
            "Wheel Loader (20 - 3.0 Cum) [hr]",
            "",
            "Backhoe Crawler (0.75- 1.0 cum) [hr]",
            "",
            "Backhoe / Pavement Breaker (1.5 cum) [hr]",
            "",
            "Motor Grader (90 - 100 kW)  [hr]",
            "",
            "Pneumatic Tire Roller (20 - 24 Mr) [hr]",
            "",
            "Vibratory Drum Roller (10 - 14MT) [hr]",
            "",
            "Dump Truck (8.0 -12.0 cu.m) [hr]",
            "",
            "Cargo Truck Small (5 - 8 MT) [hr]",
            "",
            "Cargo Truck Small (10 -15 MT) [hr]",
            "",
            "Transit Mixer (5.0 -8.0 cum) [hr]",
            "",
            "Concrete Batching Plant (80 -100 cu.m/hr) [hr]",
            "",
            "Concrete Trimmer/Slipform Paver (1 meter width) [hr]",
            "",
            "Asphalt Distributor Truck (2500 - 3500 Gallons) [hr]",
            "",
            "Asphalt Finisher (3-meter width) [hr]",
            "",
            "Truck with Boom, small (6-10 MT) [hr]",
            "",
            "Truck with Boom, small (12 - 15 MT) [hr]",
            "Crawler Crane (21-25 MT)  [hr]",
            "",
            "Diesel Pile Hammer [hr]",
            "",
            "Vibratory Pile Driver [hr]",
            "",
            "Bagger Mixer (1-2 Bags) [hr]",
            "",
            "Concrete Vibrator [hr]",
            "",
            "Air Compressor  (Small) [hr]",
            "",
            "Air Compressor (Big) [hr]",
            "",
            "Bar Cutter [hr]",
            "Bar Bender [hr]",
            "",
            "Jack Hammer [hr]",
            "",
            "Tamping Rammer [hr]",
            "",
            "Welding Machine, Portable, 300A [hr]",
            "",
            "Welding Machine, 600A [hr]",
            "",
            "Generator Set 15-25kVA [hr]",
            "",
            "Generator Set 50 kVA  [hr]",
            "",
            "Sump Pump (Dewatering) 0.75  –2HP- [hr]",
            "",
            "Sump Pump (Dewatering) 5HP [hr]",
            "",
            "Road Paint Stripper [hr]"});
            this.eqUC_cbx.Location = new System.Drawing.Point(53, 3);
            this.eqUC_cbx.Name = "eqUC_cbx";
            this.eqUC_cbx.Size = new System.Drawing.Size(284, 28);
            this.eqUC_cbx.TabIndex = 23;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(44, 29);
            this.button1.TabIndex = 0;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // EquipmentUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EquipmentUserControl";
            this.Size = new System.Drawing.Size(820, 45);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox eqUC_days;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox eqUC_hrs;
        private System.Windows.Forms.TextBox eqUC_qty;
        private System.Windows.Forms.ComboBox eqUC_cbx;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
    }
}
