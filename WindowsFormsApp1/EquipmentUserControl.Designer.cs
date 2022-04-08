
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
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.Controls.Add(this.eqUC_qty, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.eqUC_cbx, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 45);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // eqUC_qty
            // 
            this.eqUC_qty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eqUC_qty.Location = new System.Drawing.Point(448, 3);
            this.eqUC_qty.Name = "eqUC_qty";
            this.eqUC_qty.Size = new System.Drawing.Size(49, 26);
            this.eqUC_qty.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(388, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label8.Size = new System.Drawing.Size(54, 31);
            this.label8.TabIndex = 24;
            this.label8.Text = "QTY";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // eqUC_cbx
            // 
            this.eqUC_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.eqUC_cbx.FormattingEnabled = true;
            this.eqUC_cbx.Items.AddRange(new object[] {
            "Crawler Loader (80kW / 1.5 - 2.0 cu.m)",
            "Crawler Dozer (125kW)",
            "Wheel Loader (2.0 - 3.0 cum)",
            "Backhoe Crawler (0.75 - 1.0 cu.m)",
            "Backhoe / Pavement Breaker (1.5 cu.m)",
            "Motor Grader ( 90 - 100 kW)",
            "Pneumatic Tire Roller ( 20 - 24 MT)",
            "Vibratory Drum Roller ( 10 - 14 MT)",
            "Dump Truck (8.0 - 120 cu.m)",
            "Cargo Truck Small (5 - 8 MT)",
            "Cargo Truck Small (10 - 15 MT)",
            "Transit Mixer (5.0 - 8.0 cu.m)",
            "Concrete Batching Plant (80 - 100 cu.m/hr)",
            "Concrete Trimmer/Slipform Paver (1 meter width)",
            "Asphalt Distributor Truck (2500 - 3500 Gallons)",
            "Asphalt Finisher (3 meter width)",
            "Truck with Boom, small (6 - 10 MT)",
            "Truck with Boom, small (12 - 15 MT)",
            "Truck with Boom, small (12 - 15 MT)",
            "Crawler Crane (21 - 25 MT)",
            "Diesel Pile Hammer",
            "Vibratory Pile Driver",
            "Bagger Mixer (1 - 2 bags)",
            "Concrete Vibrator",
            "Air Compressor - Small",
            "Air Compressor - Big",
            "Bar Cutter",
            "Bar Bender",
            "Jack Hammer",
            "Tamping Rammer",
            "Welding Machine, Portable, 300A",
            "Welding Machine, 600A",
            "Generator Set 15-25kVA",
            "Generator Set 50kVA",
            "Sump Pump (Dewatering), 0.75 - 2HP",
            "Sump Pump (Dewatering), 5HP",
            "Road Paint Stripper"});
            this.eqUC_cbx.Location = new System.Drawing.Point(40, 3);
            this.eqUC_cbx.Name = "eqUC_cbx";
            this.eqUC_cbx.Size = new System.Drawing.Size(344, 28);
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
            this.button1.Size = new System.Drawing.Size(31, 29);
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
            this.Size = new System.Drawing.Size(500, 45);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox eqUC_qty;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox eqUC_cbx;
        private System.Windows.Forms.Button button1;
    }
}
