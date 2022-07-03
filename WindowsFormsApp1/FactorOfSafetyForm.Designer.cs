namespace WindowsFormsApp1
{
    partial class FactorOfSafetyForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.fos_LC_bx = new System.Windows.Forms.TextBox();
            this.fos_LC_cbx = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.fos_Tiles_bx = new System.Windows.Forms.TextBox();
            this.fos_CHB_bx = new System.Windows.Forms.TextBox();
            this.fos_RMC_bx = new System.Windows.Forms.TextBox();
            this.fos_Gravel_bx = new System.Windows.Forms.TextBox();
            this.fos_Sand_bx = new System.Windows.Forms.TextBox();
            this.fos_Cement_bx = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.fos_CancelBtn = new System.Windows.Forms.Button();
            this.fos_SaveBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 3;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 211F));
            this.tableLayoutPanel10.Controls.Add(this.fos_LC_bx, 2, 2);
            this.tableLayoutPanel10.Controls.Add(this.fos_LC_cbx, 2, 1);
            this.tableLayoutPanel10.Controls.Add(this.label6, 2, 0);
            this.tableLayoutPanel10.Controls.Add(this.fos_Tiles_bx, 1, 5);
            this.tableLayoutPanel10.Controls.Add(this.fos_CHB_bx, 1, 4);
            this.tableLayoutPanel10.Controls.Add(this.fos_RMC_bx, 1, 3);
            this.tableLayoutPanel10.Controls.Add(this.fos_Gravel_bx, 1, 2);
            this.tableLayoutPanel10.Controls.Add(this.fos_Sand_bx, 1, 1);
            this.tableLayoutPanel10.Controls.Add(this.fos_Cement_bx, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel10.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel10.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel10.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel10.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.label32, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel2, 0, 6);
            this.tableLayoutPanel10.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 7;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(611, 283);
            this.tableLayoutPanel10.TabIndex = 28;
            // 
            // fos_LC_bx
            // 
            this.fos_LC_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_LC_bx.Enabled = false;
            this.fos_LC_bx.Location = new System.Drawing.Point(403, 69);
            this.fos_LC_bx.Name = "fos_LC_bx";
            this.fos_LC_bx.Size = new System.Drawing.Size(205, 26);
            this.fos_LC_bx.TabIndex = 100;
            this.fos_LC_bx.Text = "30%";
            // 
            // fos_LC_cbx
            // 
            this.fos_LC_cbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_LC_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fos_LC_cbx.FormattingEnabled = true;
            this.fos_LC_cbx.Items.AddRange(new object[] {
            "Rate",
            "Percentage",
            "Manual Add"});
            this.fos_LC_cbx.Location = new System.Drawing.Point(403, 35);
            this.fos_LC_cbx.Name = "fos_LC_cbx";
            this.fos_LC_cbx.Size = new System.Drawing.Size(205, 28);
            this.fos_LC_cbx.TabIndex = 99;
            this.fos_LC_cbx.SelectedIndexChanged += new System.EventHandler(this.fos_LC_cbx_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(401, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label6.Size = new System.Drawing.Size(193, 31);
            this.label6.TabIndex = 98;
            this.label6.Text = "Labor Calculation:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fos_Tiles_bx
            // 
            this.fos_Tiles_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_Tiles_bx.Location = new System.Drawing.Point(203, 165);
            this.fos_Tiles_bx.Name = "fos_Tiles_bx";
            this.fos_Tiles_bx.Size = new System.Drawing.Size(194, 26);
            this.fos_Tiles_bx.TabIndex = 97;
            this.fos_Tiles_bx.Text = "10%";
            // 
            // fos_CHB_bx
            // 
            this.fos_CHB_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_CHB_bx.Location = new System.Drawing.Point(203, 133);
            this.fos_CHB_bx.Name = "fos_CHB_bx";
            this.fos_CHB_bx.Size = new System.Drawing.Size(194, 26);
            this.fos_CHB_bx.TabIndex = 96;
            this.fos_CHB_bx.Text = "5%";
            // 
            // fos_RMC_bx
            // 
            this.fos_RMC_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_RMC_bx.Location = new System.Drawing.Point(203, 101);
            this.fos_RMC_bx.Name = "fos_RMC_bx";
            this.fos_RMC_bx.Size = new System.Drawing.Size(194, 26);
            this.fos_RMC_bx.TabIndex = 95;
            this.fos_RMC_bx.Text = "5%";
            // 
            // fos_Gravel_bx
            // 
            this.fos_Gravel_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_Gravel_bx.Location = new System.Drawing.Point(203, 69);
            this.fos_Gravel_bx.Name = "fos_Gravel_bx";
            this.fos_Gravel_bx.Size = new System.Drawing.Size(194, 26);
            this.fos_Gravel_bx.TabIndex = 94;
            this.fos_Gravel_bx.Text = "5%";
            // 
            // fos_Sand_bx
            // 
            this.fos_Sand_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_Sand_bx.Location = new System.Drawing.Point(203, 35);
            this.fos_Sand_bx.Name = "fos_Sand_bx";
            this.fos_Sand_bx.Size = new System.Drawing.Size(194, 26);
            this.fos_Sand_bx.TabIndex = 93;
            this.fos_Sand_bx.Text = "5%";
            // 
            // fos_Cement_bx
            // 
            this.fos_Cement_bx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_Cement_bx.Location = new System.Drawing.Point(203, 3);
            this.fos_Cement_bx.Name = "fos_Cement_bx";
            this.fos_Cement_bx.Size = new System.Drawing.Size(194, 26);
            this.fos_Cement_bx.TabIndex = 92;
            this.fos_Cement_bx.Text = "5%";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1, 130);
            this.label5.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label5.Size = new System.Drawing.Size(193, 31);
            this.label5.TabIndex = 91;
            this.label5.Text = "CHB";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1, 162);
            this.label4.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label4.Size = new System.Drawing.Size(193, 31);
            this.label4.TabIndex = 90;
            this.label4.Text = "Tiles";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label3.Size = new System.Drawing.Size(193, 31);
            this.label3.TabIndex = 89;
            this.label3.Text = "Ready Mix Concrete";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label2.Size = new System.Drawing.Size(193, 31);
            this.label2.TabIndex = 88;
            this.label2.Text = "Gravel";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1, 32);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(193, 31);
            this.label1.TabIndex = 87;
            this.label1.Text = "Sand";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label32
            // 
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(1, 0);
            this.label32.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label32.Name = "label32";
            this.label32.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label32.Size = new System.Drawing.Size(193, 31);
            this.label32.TabIndex = 86;
            this.label32.Text = "Cement";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel10.SetColumnSpan(this.tableLayoutPanel2, 3);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.Controls.Add(this.fos_CancelBtn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.fos_SaveBtn, 2, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 197);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(605, 45);
            this.tableLayoutPanel2.TabIndex = 85;
            // 
            // fos_CancelBtn
            // 
            this.fos_CancelBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_CancelBtn.Location = new System.Drawing.Point(408, 3);
            this.fos_CancelBtn.Name = "fos_CancelBtn";
            this.fos_CancelBtn.Size = new System.Drawing.Size(94, 39);
            this.fos_CancelBtn.TabIndex = 84;
            this.fos_CancelBtn.Text = "Cancel";
            this.fos_CancelBtn.UseVisualStyleBackColor = true;
            this.fos_CancelBtn.Click += new System.EventHandler(this.fos_CancelBtn_Click);
            // 
            // fos_SaveBtn
            // 
            this.fos_SaveBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fos_SaveBtn.Location = new System.Drawing.Point(508, 3);
            this.fos_SaveBtn.Name = "fos_SaveBtn";
            this.fos_SaveBtn.Size = new System.Drawing.Size(94, 39);
            this.fos_SaveBtn.TabIndex = 85;
            this.fos_SaveBtn.Text = "Save";
            this.fos_SaveBtn.UseVisualStyleBackColor = true;
            this.fos_SaveBtn.Click += new System.EventHandler(this.fos_SaveBtn_Click);
            // 
            // FactorOfSafetyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 307);
            this.Controls.Add(this.tableLayoutPanel10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FactorOfSafetyForm";
            this.Text = "FactorOfSafetyForm";
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button fos_CancelBtn;
        private System.Windows.Forms.Button fos_SaveBtn;
        private System.Windows.Forms.TextBox fos_CHB_bx;
        private System.Windows.Forms.TextBox fos_RMC_bx;
        private System.Windows.Forms.TextBox fos_Gravel_bx;
        private System.Windows.Forms.TextBox fos_Sand_bx;
        private System.Windows.Forms.TextBox fos_Cement_bx;
        private System.Windows.Forms.TextBox fos_Tiles_bx;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox fos_LC_cbx;
        private System.Windows.Forms.TextBox fos_LC_bx;
    }
}