
namespace WindowsFormsApp1
{
    partial class ColumnSpacingUserControl
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
            this.tableLayoutPanel80 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label235 = new System.Windows.Forms.Label();
            this.label234 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel80.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel80
            // 
            this.tableLayoutPanel80.ColumnCount = 4;
            this.tableLayoutPanel80.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel80.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel80.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel80.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel80.Controls.Add(this.textBox1, 1, 0);
            this.tableLayoutPanel80.Controls.Add(this.label235, 0, 0);
            this.tableLayoutPanel80.Controls.Add(this.label234, 2, 0);
            this.tableLayoutPanel80.Controls.Add(this.textBox2, 3, 0);
            this.tableLayoutPanel80.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel80.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel80.Name = "tableLayoutPanel80";
            this.tableLayoutPanel80.RowCount = 1;
            this.tableLayoutPanel80.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel80.Size = new System.Drawing.Size(401, 38);
            this.tableLayoutPanel80.TabIndex = 31;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(103, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(90, 26);
            this.textBox1.TabIndex = 37;
            this.textBox1.Text = "0";
            // 
            // label235
            // 
            this.label235.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label235.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label235.Location = new System.Drawing.Point(1, 0);
            this.label235.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label235.Name = "label235";
            this.label235.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label235.Size = new System.Drawing.Size(96, 38);
            this.label235.TabIndex = 36;
            this.label235.Text = "QTY:";
            this.label235.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label234
            // 
            this.label234.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label234.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label234.Location = new System.Drawing.Point(201, 0);
            this.label234.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label234.Name = "label234";
            this.label234.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label234.Size = new System.Drawing.Size(96, 38);
            this.label234.TabIndex = 35;
            this.label234.Text = "Spacing:";
            this.label234.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.Location = new System.Drawing.Point(303, 3);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(90, 26);
            this.textBox2.TabIndex = 34;
            this.textBox2.Text = "0";
            // 
            // ColumnSpacingUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel80);
            this.Name = "ColumnSpacingUserControl";
            this.Size = new System.Drawing.Size(401, 38);
            this.Load += new System.EventHandler(this.ColumnSpacingUserControl_Load);
            this.tableLayoutPanel80.ResumeLayout(false);
            this.tableLayoutPanel80.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel80;
        private System.Windows.Forms.Label label235;
        private System.Windows.Forms.Label label234;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
    }
}
