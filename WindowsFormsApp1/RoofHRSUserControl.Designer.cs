﻿namespace WindowsFormsApp1
{
    partial class RoofHRSUserControl
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
            this.tableLayoutPanel84 = new System.Windows.Forms.TableLayoutPanel();
            this.label267 = new System.Windows.Forms.Label();
            this.roof_GI_D_EC_cbx = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel84.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel84
            // 
            this.tableLayoutPanel84.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel84.ColumnCount = 2;
            this.tableLayoutPanel84.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.34615F));
            this.tableLayoutPanel84.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.65385F));
            this.tableLayoutPanel84.Controls.Add(this.roof_GI_D_EC_cbx, 0, 0);
            this.tableLayoutPanel84.Controls.Add(this.label267, 0, 0);
            this.tableLayoutPanel84.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel84.Name = "tableLayoutPanel84";
            this.tableLayoutPanel84.RowCount = 1;
            this.tableLayoutPanel84.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel84.Size = new System.Drawing.Size(312, 35);
            this.tableLayoutPanel84.TabIndex = 34;
            // 
            // label267
            // 
            this.label267.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label267.Location = new System.Drawing.Point(1, 0);
            this.label267.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.label267.Name = "label267";
            this.label267.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label267.Size = new System.Drawing.Size(203, 31);
            this.label267.TabIndex = 35;
            this.label267.Text = "Height of Roof Sheet:";
            this.label267.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // roof_GI_D_EC_cbx
            // 
            this.roof_GI_D_EC_cbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.roof_GI_D_EC_cbx.FormattingEnabled = true;
            this.roof_GI_D_EC_cbx.Items.AddRange(new object[] {
            "1.5",
            "1.8",
            "2.1",
            "2.4",
            "2.7",
            "3.0",
            "3.6"});
            this.roof_GI_D_EC_cbx.Location = new System.Drawing.Point(210, 3);
            this.roof_GI_D_EC_cbx.Name = "roof_GI_D_EC_cbx";
            this.roof_GI_D_EC_cbx.Size = new System.Drawing.Size(99, 28);
            this.roof_GI_D_EC_cbx.TabIndex = 36;
            // 
            // RoofHRSUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel84);
            this.Name = "RoofHRSUserControl";
            this.Size = new System.Drawing.Size(318, 41);
            this.tableLayoutPanel84.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel84;
        private System.Windows.Forms.Label label267;
        private System.Windows.Forms.ComboBox roof_GI_D_EC_cbx;
    }
}
