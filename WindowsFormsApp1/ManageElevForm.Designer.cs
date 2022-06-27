
namespace WindowsFormsApp1
{
    partial class ManageElevForm
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
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.manageElevSaveBtn = new System.Windows.Forms.Button();
            this.manageElevAddBtn = new System.Windows.Forms.Button();
            this.manageElevPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel11.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel11.ColumnCount = 2;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.20216F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.79784F));
            this.tableLayoutPanel11.Controls.Add(this.manageElevSaveBtn, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.manageElevAddBtn, 0, 0);
            this.tableLayoutPanel11.Location = new System.Drawing.Point(51, 327);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 1;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(265, 42);
            this.tableLayoutPanel11.TabIndex = 29;
            // 
            // manageElevSaveBtn
            // 
            this.manageElevSaveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.manageElevSaveBtn.Location = new System.Drawing.Point(202, 3);
            this.manageElevSaveBtn.Name = "manageElevSaveBtn";
            this.manageElevSaveBtn.Size = new System.Drawing.Size(60, 36);
            this.manageElevSaveBtn.TabIndex = 28;
            this.manageElevSaveBtn.Text = "Save";
            this.manageElevSaveBtn.UseVisualStyleBackColor = true;
            this.manageElevSaveBtn.Click += new System.EventHandler(this.manageElevSaveBtn_Click);
            // 
            // manageElevAddBtn
            // 
            this.manageElevAddBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.manageElevAddBtn.Location = new System.Drawing.Point(136, 3);
            this.manageElevAddBtn.Name = "manageElevAddBtn";
            this.manageElevAddBtn.Size = new System.Drawing.Size(60, 36);
            this.manageElevAddBtn.TabIndex = 27;
            this.manageElevAddBtn.Text = "Add";
            this.manageElevAddBtn.UseVisualStyleBackColor = true;
            this.manageElevAddBtn.Click += new System.EventHandler(this.manageElevAddBtn_Click);
            // 
            // manageElevPanel
            // 
            this.manageElevPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.manageElevPanel.AutoScroll = true;
            this.manageElevPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.manageElevPanel.Location = new System.Drawing.Point(12, 12);
            this.manageElevPanel.Name = "manageElevPanel";
            this.manageElevPanel.Size = new System.Drawing.Size(304, 309);
            this.manageElevPanel.TabIndex = 30;
            this.manageElevPanel.WrapContents = false;
            // 
            // ManageElevForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 381);
            this.Controls.Add(this.manageElevPanel);
            this.Controls.Add(this.tableLayoutPanel11);
            this.Name = "ManageElevForm";
            this.Text = "Manage Elevations";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManageElevForm_FormClosing);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private System.Windows.Forms.Button manageElevSaveBtn;
        private System.Windows.Forms.Button manageElevAddBtn;
        private System.Windows.Forms.FlowLayoutPanel manageElevPanel;
    }
}