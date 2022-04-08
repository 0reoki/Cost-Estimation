
namespace WindowsFormsApp1
{
    partial class PaintAreaUserControl
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
            this.rightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.paUC_PL_bx = new System.Windows.Forms.TextBox();
            this.paUC_Area_bx = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.paUC_lbl = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.paUC_Paint_cbx = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rightPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightPanel
            // 
            this.rightPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rightPanel.ColumnCount = 2;
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanel.Controls.Add(this.paUC_PL_bx, 1, 6);
            this.rightPanel.Controls.Add(this.paUC_Area_bx, 1, 2);
            this.rightPanel.Controls.Add(this.label8, 1, 1);
            this.rightPanel.Controls.Add(this.label1, 1, 3);
            this.rightPanel.Controls.Add(this.paUC_lbl, 1, 0);
            this.rightPanel.Controls.Add(this.button1, 0, 2);
            this.rightPanel.Controls.Add(this.paUC_Paint_cbx, 1, 4);
            this.rightPanel.Controls.Add(this.label2, 1, 5);
            this.rightPanel.Location = new System.Drawing.Point(4, 3);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.RowCount = 7;
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.01109F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.99815F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.99815F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.99815F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.99815F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.99815F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.99815F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rightPanel.Size = new System.Drawing.Size(259, 266);
            this.rightPanel.TabIndex = 8;
            // 
            // paUC_PL_bx
            // 
            this.paUC_PL_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paUC_PL_bx.Location = new System.Drawing.Point(65, 230);
            this.paUC_PL_bx.Name = "paUC_PL_bx";
            this.paUC_PL_bx.Size = new System.Drawing.Size(191, 26);
            this.paUC_PL_bx.TabIndex = 32;
            // 
            // paUC_Area_bx
            // 
            this.paUC_Area_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paUC_Area_bx.Location = new System.Drawing.Point(65, 82);
            this.paUC_Area_bx.Name = "paUC_Area_bx";
            this.paUC_Area_bx.Size = new System.Drawing.Size(191, 26);
            this.paUC_Area_bx.TabIndex = 30;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(63, 42);
            this.label8.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label8.Size = new System.Drawing.Size(168, 31);
            this.label8.TabIndex = 25;
            this.label8.Text = "Area:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(63, 116);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(190, 31);
            this.label1.TabIndex = 26;
            this.label1.Text = "Paint:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // paUC_lbl
            // 
            this.paUC_lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paUC_lbl.Location = new System.Drawing.Point(63, 0);
            this.paUC_lbl.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.paUC_lbl.Name = "paUC_lbl";
            this.paUC_lbl.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.paUC_lbl.Size = new System.Drawing.Size(190, 31);
            this.paUC_lbl.TabIndex = 27;
            this.paUC_lbl.Text = "Paint Area ";
            this.paUC_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(3, 82);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 13);
            this.button1.Name = "button1";
            this.rightPanel.SetRowSpan(this.button1, 2);
            this.button1.Size = new System.Drawing.Size(56, 58);
            this.button1.TabIndex = 31;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // paUC_Paint_cbx
            // 
            this.paUC_Paint_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paUC_Paint_cbx.FormattingEnabled = true;
            this.paUC_Paint_cbx.Items.AddRange(new object[] {
            "Enamel",
            "Acrylic",
            "Latex Gloss",
            "Semi-gloss"});
            this.paUC_Paint_cbx.Location = new System.Drawing.Point(65, 156);
            this.paUC_Paint_cbx.Name = "paUC_Paint_cbx";
            this.paUC_Paint_cbx.Size = new System.Drawing.Size(188, 28);
            this.paUC_Paint_cbx.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(63, 190);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(190, 31);
            this.label2.TabIndex = 29;
            this.label2.Text = "Paint Layers:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PaintAreaUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rightPanel);
            this.Name = "PaintAreaUserControl";
            this.Size = new System.Drawing.Size(267, 272);
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rightPanel;
        private System.Windows.Forms.TextBox paUC_Area_bx;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label paUC_lbl;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox paUC_Paint_cbx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox paUC_PL_bx;
    }
}
