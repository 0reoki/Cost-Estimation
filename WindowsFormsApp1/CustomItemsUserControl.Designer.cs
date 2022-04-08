
namespace WindowsFormsApp1
{
    partial class CustomItemsUserControl
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
            this.ciUC_qty = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ciUC_cbx = new System.Windows.Forms.ComboBox();
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
            this.tableLayoutPanel1.Controls.Add(this.ciUC_qty, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.ciUC_cbx, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 45);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // ciUC_qty
            // 
            this.ciUC_qty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ciUC_qty.Location = new System.Drawing.Point(448, 3);
            this.ciUC_qty.Name = "ciUC_qty";
            this.ciUC_qty.Size = new System.Drawing.Size(49, 26);
            this.ciUC_qty.TabIndex = 25;
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
            // ciUC_cbx
            // 
            this.ciUC_cbx.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ciUC_cbx.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ciUC_cbx.FormattingEnabled = true;
            this.ciUC_cbx.Items.AddRange(new object[] {
            "Cyclone Wire (Gauge#10, 2”x2”, 3ft x 10m) [ROLL]",
            "Gasket (6mm thk, 1m x 1m) [ROLL]",
            "Acetylene Gas [CYL] ",
            "Oxygen Gas [CYL]",
            "Rugby [CANS]",
            "Vulca Seal [LTR]",
            "Broom, Soft [PC]",
            "Concrete Epoxy [SET]",
            "Concrete Patching Compound [KGS]",
            "GI Wire (no. 16) [ROLL]",
            "Non-Shrink Grout [BAGS]",
            "40 kg Portland Cement [BAGS] ",
            "Sand [m3]",
            "Rivets 1/8” x ½” [BOX]",
            "Rivets 1-1/2” x ½” [BOX]",
            "Rope (⌀ 1/2”) [MTRS]",
            "Tape, Caution [ROLLS] ",
            "Tile Grout [BAGS]",
            "Tiles, Floor (600 x 600) [PC]",
            "Tiles, Wall (300 x 300) [PC]",
            " Broom Stick [PC]",
            "Chalk Stone [PC]",
            "Sandpaper (#100) [MTRS]",
            "Sandpaper (#100) [MTRS]",
            "Putty, Multipurpose [PAIL]",
            "Tie Wire (No. #16) [25kg/Roll]",
            "Acrylic Emulsion [GALS] ",
            "Concrete Epoxy Injection [GALS] ",
            "Concrete Primer & Sealer [PAIL] ",
            "Epopatch, Base and Hardener [SETS] ",
            "Lacquer Thinner [GALS] ",
            "Paint Brush, Bamboo 1-1/2\" [PC] ",
            "Paint, Acrylic 1 [GAL] ",
            "Paint, Epoxy Enamel White [GAL]  ",
            "Paint, Epoxy Floor Coating [GALS] ",
            "Paint, Epoxy Primer Gray [GALS] ",
            "Paint, Epoxy Reducer [GALS] ",
            "Paint Latex Gloss [GAL] ",
            "Paint Enamel [GAL] ",
            "Paint, Semi-Gloss [GALS] ",
            "Putty, Masonry [PAIL] ",
            "Rust Converter [GAL] ",
            "Skim Coat [BAGS] ",
            "Underwater Epoxy [GALS] ",
            "Stainless Welding Rod 308 (3.2mm) [KGS] ",
            "Welding Rod 6011 (3.2mm) [KGS] ",
            "Welding Rod 6011 (3.2mm) [BOX] ",
            "Welding Rod 6013 (3.2mm) [KGS] ",
            "Welding Rod 6013 (3.2mm) [BOX] ",
            "Chemical Gloves PAIR Cotton Gloves [PAIRS] ",
            "Dust Mask N95 [PC] ",
            "Mask [PC]",
            "Welding Apron [PC]",
            "Welding Mask, Auto Darkening [SETS]",
            "Adjustable Wrench Set 4”— 24” [SET] ",
            "Baby Roller (Cotton) 4” [PC] ",
            "Ball Hammer [PC] ",
            "Bench Vise [UNIT] ",
            "Blade Cutter [PC] ",
            "Camlock (Male & Female Set) 50mm DIA [SET]",
            "Chipping Gun [UNIT] ",
            "Combination Wrench Set 6mm — 32mm [SET] ",
            "Cut-off Wheel ⌀ 16\" [BOX] ",
            "Cutting Disc ⌀ 4” [BOX] ",
            "Cutting Disc ⌀ 7” [BOX] ",
            "Drill Bit [BOX]",
            "Electrical Plier [PC]",
            "Grinder, Angle 4” [UNIT] ",
            "Grinder, Angle 7” [UNIT] ",
            "Grinder, Baby [UNIT] ",
            "Grinder, Mother [UNIT] ",
            "Grinding Disc ⌀ 4” [BOX] ",
            "Grinding Disc ⌀ 7” [BOX] ",
            "Heat Gun [UNIT] ",
            "Ladder (A-Type), 6h, Aluminum [PC] ",
            "Level Bar 24\" [PC] ",
            "Paint Brush 4” [PC]",
            "1 PC Paint Brush 2” [PC] ",
            "Portable Axial Fan Blower  ⌀  8” [SET] ",
            "Power Rachet [UNIT] ",
            "Rivet Gun / Riveter [UNIT]",
            "Roller Brush 7” [PC] ",
            "Screwdriver, Flat [SET] ",
            "Screwdriver, Philip [SET] ",
            "Shovel, Pointed [PC] ",
            "Snop Ring Plier [SET] ",
            "Socket Wrench Set 19mm — 50mm [SET] ",
            "Speed Cutter [UNIT] ",
            "Steel Brush [PC] ",
            "Test Light [PC] ",
            "Test Wrench [UNIT]",
            "Torque Wrench [UNIT]",
            "Vise Grip [PC]",
            "Welding Machine (Portable) 12.3 kVA(20-300A) [UNIT] ",
            "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days [m3]",
            "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 14 Days [m3] ",
            "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 7 Days [m3]",
            "Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 3 Days [m3]",
            "Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 28 Days [m3]",
            "Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 14 Days [m3]",
            "Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 7 Days [m3]",
            "Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 3 Days [m3]",
            "Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 28 Days [m3]",
            "Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 14 Days [m3]",
            "Ready Mix Concrete, 4500PSI (31 Mpa) @ 28 Days [m3]",
            "Ready Mix Concrete, 5000PSI (34.5 Mpa) @ 28 Days[m3]",
            "GRAVEL G1 [m3]",
            "GRAVEL G2 [m3]",
            "GRAVEL G1- ½” [m3]",
            "GRAVEL G2- ½” [m3]",
            "GRAVEL ¾” [m3]",
            "Lumber 2\"x 2” x 8\' ",
            "Lumber 2\"x 2” x 10\' ",
            "Lumber 2\"x 2” x 12\' ",
            "Lumber 2\"x 3\"x 8\' ",
            "Lumber 2\"x 3\"x 10\' ",
            "Lumber 2\"x 3\"x 12\' ",
            "Lumber 2\"x4\"x 8\' ",
            "Lumber 2\"x4\"x 10\' ",
            "Lumber 2\"x4\"x 12\'",
            "PLYWOOD 1/2” [1.22m x 2.44m]",
            "PLYWOOD 3/4” [1.22m x 2.44m]",
            "PLYWOOD 1/4” [1.22m x 2.44m]",
            "PLYWOOD 1/8\"[1.22m x 2.44m]",
            "ECOBOARD 1/2\"[1.22m x 2.44m]",
            "ECOBOARD 3/4\" [1.22m x 2.44m]",
            "ECOBOARD 1/4\" [1.22m x 2.44m]",
            "ECOBOARD 1/8\" [1.22m x 2.44m]",
            "PHENOLIC BOARD- 1/2\" [1.22m x 2.44m]",
            "PHENOLIC BOARD- 3/4\" [1.22m x 2.44m]",
            "B.I. (Black Iron) Tubular 20mm x 20mm x 1.0mm thick [6m]",
            "B.I. (Black Iron) Tubular 25mm x 25mm x 1.0mm thick [6m]",
            "B.I. (Black Iron) Tubular 32mm x 32mm x 1.0mm thick [6m]",
            "B.I. (Black Iron) Tubular 50mm x 25mm x 1.0mm Nick [6m]",
            "B.I. (Black Iron) Tubular 50mm x 50mm x 1.0mm thick [6m]",
            "B.I (Black Iron) Tubular 25mm x 25mm x 1.2mm thick [6m]",
            "B.I. (Black Iron) Tubular 32mm x 32mm x 1.2mm thick [6m]",
            "B.I. (Black Iron) Tubular 50mm x 25mm x 1.2mm thick [6m]",
            "B.I. (Black Iron) Tubular 50mm x 50mm x 1.2mm thick [6m]",
            "B.I. (Black Iron) Tubular 75mm x 50mm x 1.2mm thick [6m]",
            "B.I. (Black Iron) Tubular 100mm x 50mm x 1.2mm thick [6m]",
            "B.I. (Black Iron) Tubular 150mm x 50mm x 1.2mm thick[6m]",
            "B.I. (Black Iron) Tubular 25mm x 25mm x 1.5mm thick [6m]",
            "B.I. (Black Iron) Tubular 32mm x 32mm x 1.5mm thick [6m]",
            "B.I. (Black Iron) Tubular 50mm x 25mm x 1.5mm thick [6m]",
            "B.I. (Black Iron) Tubular 50mm x 50mm x 1.5mm thick [6m]",
            "B.I. (Black Iron) Tubular 75mm x 50mm x 1.5mm thick [6m]",
            "B.I. (Black Iron) Tubular 100mm x 50mm x 1.5mm thick [6m]",
            "B.I. (Black Iron) Tubular 150mm x 50mm x 1.5mm thick [6m]",
            "Common Borrow [m3]",
            "Selected Borrow [m3]",
            "Mixed Sand & Gravel [m3]",
            "Rock [m3]",
            "Rebar GRADE 30 (⌀10mm) [6m]",
            "Rebar GRADE 30 (⌀10mm) [7.5m]",
            "Rebar GRADE 30 (⌀10mm) [9m]",
            "Rebar GRADE 30 (⌀10mm) [10.5m]",
            "Rebar GRADE 30 (⌀10mm) [12m]",
            "Rebar GRADE 30 (⌀12mm) [6m]",
            "Rebar GRADE 30 (⌀12mm) [7.5m]",
            "Rebar GRADE 30 (⌀12mm) [9m]",
            "Rebar GRADE 30 (⌀12mm) [10.5m]",
            "Rebar GRADE 30 (⌀12mm) [12m]",
            "Rebar GRADE 30 (⌀16mm) [6m]",
            "Rebar GRADE 30 (⌀16mm) [7.5m]",
            "Rebar GRADE 30 (⌀16mm) [9m]",
            "Rebar GRADE 30 (⌀16mm) [10.5m]",
            "Rebar GRADE 30 (⌀16mm) [12m]",
            "Rebar GRADE 30 (⌀20mm) [6m]",
            "Rebar GRADE 30 (⌀20mm) [7.5m]",
            "Rebar GRADE 30 (⌀20mm) [9m]",
            "Rebar GRADE 30 (⌀20mm) [10.5m]",
            "Rebar GRADE 30 (⌀20mm) [12m]",
            "Rebar GRADE 30 (⌀25mm) [6m]",
            "Rebar GRADE 30 (⌀25mm) [7.5m]",
            "Rebar GRADE 30 (⌀25mm) [9m]",
            "Rebar GRADE 30 (⌀25mm) [10.5m]",
            "Rebar GRADE 30 (⌀25mm) [12m]",
            "Rebar GRADE 40 (⌀10mm) [6m]",
            "Rebar GRADE 40 (⌀10mm) [7.5m]",
            "Rebar GRADE 40 (⌀10mm) [9m]",
            "Rebar GRADE 40 (⌀10mm) [10.5m]",
            "Rebar GRADE 40 (⌀10mm) [12m]",
            "Rebar GRADE 40 (⌀12mm) [6m]",
            "Rebar GRADE 40 (⌀12mm) [7.5m]",
            "Rebar GRADE 40 (⌀12mm) [9m]",
            "Rebar GRADE 40 (⌀12mm) [10.5m]",
            "Rebar GRADE 40 (⌀12mm) [12m]",
            "Rebar GRADE 40 (⌀16mm) [6m]",
            "Rebar GRADE 40 (⌀16mm) [7.5m]",
            "Rebar GRADE 40 (⌀16mm) [9m]",
            "Rebar GRADE 40 (⌀16mm) [10.5m]",
            "Rebar GRADE 40 (⌀16mm) [12m]",
            "Rebar GRADE 40 (⌀20mm) [6m]",
            "Rebar GRADE 40 (⌀20mm) [7.5m]",
            "Rebar GRADE 40 (⌀20mm) [9m]",
            "Rebar GRADE 40 (⌀20mm) [10.5m]",
            "Rebar GRADE 40 (⌀20mm) [12m]",
            "Rebar GRADE 40 (⌀25mm) [6m]",
            "Rebar GRADE 40 (⌀25mm) [7.5m]",
            "Rebar GRADE 40 (⌀25mm) [9m]",
            "Rebar GRADE 40 (⌀25mm) [10.5m]",
            "Rebar GRADE 40 (⌀25mm) [12m]",
            "Rebar GRADE 60 (⌀10mm) [6m]",
            "Rebar GRADE 60 (⌀10mm) [7.5m]",
            "Rebar GRADE 60 (⌀10mm) [9m]",
            "Rebar GRADE 60 (⌀10mm) [10.5m]",
            "Rebar GRADE 60 (⌀10mm) [12m]",
            "Rebar GRADE 60 (⌀12mm) [6m]",
            "Rebar GRADE 60 (⌀12mm) [7.5m]",
            "Rebar GRADE 60 (⌀12mm) [9m]",
            "Rebar GRADE 60 (⌀12mm) [10.5m]",
            "Rebar GRADE 60 (⌀12mm) [12m]",
            "Rebar GRADE 60 (⌀16mm) [6m]",
            "Rebar GRADE 60 (⌀16mm) [7.5m]",
            "Rebar GRADE 60 (⌀16mm) [9m]",
            "Rebar GRADE 60 (⌀16mm) [10.5m]",
            "Rebar GRADE 60 (⌀16mm) [12m]",
            "Rebar GRADE 60 (⌀20mm) [6m]",
            "Rebar GRADE 60 (⌀20mm) [7.5m]",
            "Rebar GRADE 60 (⌀20mm) [9m]",
            "Rebar GRADE 60 (⌀20mm) [10.5m]",
            "Rebar GRADE 60 (⌀20mm) [12m]",
            "Rebar GRADE 60 (⌀25mm) [6m]",
            "Rebar GRADE 60 (⌀25mm) [7.5m]",
            "Rebar GRADE 60 (⌀25mm) [9m]",
            "Rebar GRADE 60 (⌀25mm) [10.5m]",
            "Rebar GRADE 60 (⌀25mm) [12m]",
            "Rebar GRADE 60 (⌀28mm) [6m]",
            "Rebar GRADE 60 (⌀28mm) [7.5m]",
            "Rebar GRADE 60 (⌀28mm) [9m]",
            "Rebar GRADE 60 (⌀28mm) [10.5m]",
            "Rebar GRADE 60 (⌀28mm) [12m]",
            "Rebar GRADE 60 (⌀32mm) [6m]",
            "Rebar GRADE 60 (⌀32mm) [7.5m]",
            "Rebar GRADE 60 (⌀32mm) [9m]",
            "Rebar GRADE 60 (⌀32mm) [10.5m]",
            "Rebar GRADE 60 (⌀32mm) [12m]",
            "Rebar GRADE 60 (⌀36mm) [6m]",
            "Rebar GRADE 60 (⌀36mm) [7.5m]",
            "Rebar GRADE 60 (⌀36mm) [9m]",
            "Rebar GRADE 60 (⌀36mm) [10.5m]",
            "Rebar GRADE 60 (⌀36mm) [12m]",
            "Rebar GRADE 60 (⌀40mm) [6m]",
            "Rebar GRADE 60 (⌀40mm) [7.5m]",
            "Rebar GRADE 60 (⌀40mm) [9m]",
            "Rebar GRADE 60 (⌀40mm) [10.5m]",
            "Rebar GRADE 60 (⌀40mm) [12m]",
            "Rebar GRADE 60 (⌀50mm) [6m]",
            "Rebar GRADE 60 (⌀50mm) [7.5m]",
            "Rebar GRADE 60 (⌀50mm) [9m]",
            "Rebar GRADE 60 (⌀50mm) [10.5m]",
            "Rebar GRADE 60 (⌀50mm) [12m]"});
            this.ciUC_cbx.Location = new System.Drawing.Point(40, 3);
            this.ciUC_cbx.Name = "ciUC_cbx";
            this.ciUC_cbx.Size = new System.Drawing.Size(344, 28);
            this.ciUC_cbx.TabIndex = 23;
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
            // CustomItemsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CustomItemsUserControl";
            this.Size = new System.Drawing.Size(500, 45);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox ciUC_qty;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox ciUC_cbx;
        private System.Windows.Forms.Button button1;
    }
}
