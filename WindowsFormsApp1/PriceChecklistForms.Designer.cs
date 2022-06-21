namespace WindowsFormsApp1
{
    partial class PriceChecklistForms
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
            this.pcl_15_cb = new System.Windows.Forms.CheckBox();
            this.pcl_14_cb = new System.Windows.Forms.CheckBox();
            this.pcl_13_cb = new System.Windows.Forms.CheckBox();
            this.pcl_12_cb = new System.Windows.Forms.CheckBox();
            this.pcl_1_cb = new System.Windows.Forms.CheckBox();
            this.pcl_11_cb = new System.Windows.Forms.CheckBox();
            this.priceCL_OKBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 1;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 653F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel2, 0, 8);
            this.tableLayoutPanel10.Controls.Add(this.pcl_15_cb, 0, 5);
            this.tableLayoutPanel10.Controls.Add(this.pcl_14_cb, 0, 4);
            this.tableLayoutPanel10.Controls.Add(this.pcl_13_cb, 0, 3);
            this.tableLayoutPanel10.Controls.Add(this.pcl_12_cb, 0, 2);
            this.tableLayoutPanel10.Controls.Add(this.pcl_1_cb, 0, 0);
            this.tableLayoutPanel10.Controls.Add(this.pcl_11_cb, 0, 1);
            this.tableLayoutPanel10.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 9;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(474, 455);
            this.tableLayoutPanel10.TabIndex = 27;
            // 
            // pcl_15_cb
            // 
            this.pcl_15_cb.AutoSize = true;
            this.pcl_15_cb.Location = new System.Drawing.Point(3, 158);
            this.pcl_15_cb.Name = "pcl_15_cb";
            this.pcl_15_cb.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.pcl_15_cb.Size = new System.Drawing.Size(180, 24);
            this.pcl_15_cb.TabIndex = 83;
            this.pcl_15_cb.Text = "1.5 Soil Poisoning";
            this.pcl_15_cb.UseVisualStyleBackColor = true;
            this.pcl_15_cb.CheckedChanged += new System.EventHandler(this.pcl_15_cb_CheckedChanged);
            // 
            // pcl_14_cb
            // 
            this.pcl_14_cb.AutoSize = true;
            this.pcl_14_cb.Location = new System.Drawing.Point(3, 128);
            this.pcl_14_cb.Name = "pcl_14_cb";
            this.pcl_14_cb.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.pcl_14_cb.Size = new System.Drawing.Size(310, 24);
            this.pcl_14_cb.TabIndex = 82;
            this.pcl_14_cb.Text = "1.4 Gravel Bedding and Compaction";
            this.pcl_14_cb.UseVisualStyleBackColor = true;
            this.pcl_14_cb.CheckedChanged += new System.EventHandler(this.pcl_14_cb_CheckedChanged);
            // 
            // pcl_13_cb
            // 
            this.pcl_13_cb.AutoSize = true;
            this.pcl_13_cb.Location = new System.Drawing.Point(3, 98);
            this.pcl_13_cb.Name = "pcl_13_cb";
            this.pcl_13_cb.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.pcl_13_cb.Size = new System.Drawing.Size(258, 24);
            this.pcl_13_cb.TabIndex = 81;
            this.pcl_13_cb.Text = "1.3 Grading and Compaction";
            this.pcl_13_cb.UseVisualStyleBackColor = true;
            this.pcl_13_cb.CheckedChanged += new System.EventHandler(this.pcl_13_cb_CheckedChanged);
            // 
            // pcl_12_cb
            // 
            this.pcl_12_cb.AutoSize = true;
            this.pcl_12_cb.Location = new System.Drawing.Point(3, 68);
            this.pcl_12_cb.Name = "pcl_12_cb";
            this.pcl_12_cb.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.pcl_12_cb.Size = new System.Drawing.Size(281, 24);
            this.pcl_12_cb.TabIndex = 80;
            this.pcl_12_cb.Text = "1.2 Back Filling and Compaction";
            this.pcl_12_cb.UseVisualStyleBackColor = true;
            this.pcl_12_cb.CheckedChanged += new System.EventHandler(this.pcl_12_cb_CheckedChanged);
            // 
            // pcl_1_cb
            // 
            this.pcl_1_cb.AutoSize = true;
            this.pcl_1_cb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pcl_1_cb.Location = new System.Drawing.Point(3, 3);
            this.pcl_1_cb.Name = "pcl_1_cb";
            this.pcl_1_cb.Size = new System.Drawing.Size(181, 29);
            this.pcl_1_cb.TabIndex = 79;
            this.pcl_1_cb.Text = "1.0 Earthworks";
            this.pcl_1_cb.UseVisualStyleBackColor = true;
            this.pcl_1_cb.Click += new System.EventHandler(this.pcl_1_cb_Click);
            // 
            // pcl_11_cb
            // 
            this.pcl_11_cb.AutoSize = true;
            this.pcl_11_cb.Location = new System.Drawing.Point(3, 38);
            this.pcl_11_cb.Name = "pcl_11_cb";
            this.pcl_11_cb.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.pcl_11_cb.Size = new System.Drawing.Size(158, 24);
            this.pcl_11_cb.TabIndex = 78;
            this.pcl_11_cb.Text = "1.1 Excavation";
            this.pcl_11_cb.UseVisualStyleBackColor = true;
            this.pcl_11_cb.CheckedChanged += new System.EventHandler(this.pcl_11_cb_CheckedChanged);
            // 
            // priceCL_OKBtn
            // 
            this.priceCL_OKBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.priceCL_OKBtn.Location = new System.Drawing.Point(373, 3);
            this.priceCL_OKBtn.Name = "priceCL_OKBtn";
            this.priceCL_OKBtn.Size = new System.Drawing.Size(81, 34);
            this.priceCL_OKBtn.TabIndex = 84;
            this.priceCL_OKBtn.Text = "OK";
            this.priceCL_OKBtn.UseVisualStyleBackColor = true;
            this.priceCL_OKBtn.Click += new System.EventHandler(this.priceCL_OKBtn_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.7653F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.23469F));
            this.tableLayoutPanel2.Controls.Add(this.priceCL_OKBtn, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 188);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(471, 45);
            this.tableLayoutPanel2.TabIndex = 85;
            // 
            // PriceChecklistForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(535, 663);
            this.Controls.Add(this.tableLayoutPanel10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PriceChecklistForms";
            this.Text = "Price Check List";
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.CheckBox pcl_11_cb;
        private System.Windows.Forms.CheckBox pcl_15_cb;
        private System.Windows.Forms.CheckBox pcl_14_cb;
        private System.Windows.Forms.CheckBox pcl_13_cb;
        private System.Windows.Forms.CheckBox pcl_12_cb;
        private System.Windows.Forms.CheckBox pcl_1_cb;
        private System.Windows.Forms.Button priceCL_OKBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}