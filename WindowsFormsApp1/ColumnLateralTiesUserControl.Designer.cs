
namespace WindowsFormsApp1
{
    partial class ColumnLateralTiesUserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnLateralTiesUserControl));
            this.tableLayoutPanel84 = new System.Windows.Forms.TableLayoutPanel();
            this.label238 = new System.Windows.Forms.Label();
            this.lateralTies_bx = new System.Windows.Forms.TextBox();
            this.lateralTies_pb = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel84.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lateralTies_pb)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel84
            // 
            this.tableLayoutPanel84.ColumnCount = 3;
            this.tableLayoutPanel84.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.88304F));
            this.tableLayoutPanel84.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.35088F));
            this.tableLayoutPanel84.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.18129F));
            this.tableLayoutPanel84.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel84.Controls.Add(this.lateralTies_pb, 0, 0);
            this.tableLayoutPanel84.Controls.Add(this.label238, 1, 0);
            this.tableLayoutPanel84.Controls.Add(this.lateralTies_bx, 2, 0);
            this.tableLayoutPanel84.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel84.Name = "tableLayoutPanel84";
            this.tableLayoutPanel84.RowCount = 1;
            this.tableLayoutPanel84.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel84.Size = new System.Drawing.Size(172, 35);
            this.tableLayoutPanel84.TabIndex = 34;
            // 
            // label238
            // 
            this.label238.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label238.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label238.Location = new System.Drawing.Point(35, 0);
            this.label238.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label238.Name = "label238";
            this.label238.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label238.Size = new System.Drawing.Size(65, 35);
            this.label238.TabIndex = 35;
            this.label238.Text = "QTY:";
            this.label238.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lateralTies_bx
            // 
            this.lateralTies_bx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lateralTies_bx.Location = new System.Drawing.Point(106, 3);
            this.lateralTies_bx.Name = "lateralTies_bx";
            this.lateralTies_bx.Size = new System.Drawing.Size(63, 26);
            this.lateralTies_bx.TabIndex = 34;
            this.lateralTies_bx.Text = "0";
            // 
            // lateralTies_pb
            // 
            this.lateralTies_pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lateralTies_pb.Image = ((System.Drawing.Image)(resources.GetObject("lateralTies_pb.Image")));
            this.lateralTies_pb.Location = new System.Drawing.Point(3, 3);
            this.lateralTies_pb.Name = "lateralTies_pb";
            this.lateralTies_pb.Size = new System.Drawing.Size(28, 29);
            this.lateralTies_pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.lateralTies_pb.TabIndex = 36;
            this.lateralTies_pb.TabStop = false;
            // 
            // ColumnLateralTiesUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel84);
            this.Name = "ColumnLateralTiesUserControl";
            this.Size = new System.Drawing.Size(178, 41);
            this.tableLayoutPanel84.ResumeLayout(false);
            this.tableLayoutPanel84.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lateralTies_pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel84;
        private System.Windows.Forms.PictureBox lateralTies_pb;
        private System.Windows.Forms.Label label238;
        private System.Windows.Forms.TextBox lateralTies_bx;
    }
}
