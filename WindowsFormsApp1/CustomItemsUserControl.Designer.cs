
namespace KnowEst
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
            this.ciUC_price = new System.Windows.Forms.TextBox();
            this.ciUC_qty = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ciUC_cbx = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.ciUC_price, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.ciUC_qty, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.ciUC_cbx, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(750, 45);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // ciUC_price
            // 
            this.ciUC_price.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ciUC_price.Enabled = false;
            this.ciUC_price.Location = new System.Drawing.Point(653, 3);
            this.ciUC_price.Name = "ciUC_price";
            this.ciUC_price.Size = new System.Drawing.Size(94, 26);
            this.ciUC_price.TabIndex = 26;
            // 
            // ciUC_qty
            // 
            this.ciUC_qty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ciUC_qty.Location = new System.Drawing.Point(478, 3);
            this.ciUC_qty.Name = "ciUC_qty";
            this.ciUC_qty.Size = new System.Drawing.Size(94, 26);
            this.ciUC_qty.TabIndex = 25;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(401, 0);
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
            this.ciUC_cbx.Location = new System.Drawing.Point(53, 3);
            this.ciUC_cbx.Name = "ciUC_cbx";
            this.ciUC_cbx.Size = new System.Drawing.Size(344, 28);
            this.ciUC_cbx.TabIndex = 23;
            this.ciUC_cbx.TextChanged += new System.EventHandler(this.ciUC_cbx_TextChanged);
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
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(576, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(71, 31);
            this.label1.TabIndex = 27;
            this.label1.Text = "₱";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CustomItemsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CustomItemsUserControl";
            this.Size = new System.Drawing.Size(750, 45);
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
        private System.Windows.Forms.TextBox ciUC_price;
        private System.Windows.Forms.Label label1;
    }
}
