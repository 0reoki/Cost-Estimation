
namespace WindowsFormsApp1
{
    partial class Floor
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
            this.floorPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.floorDupeCountNUD = new System.Windows.Forms.NumericUpDown();
            this.floorUCDeleteBtn = new System.Windows.Forms.Button();
            this.floorLbl = new System.Windows.Forms.TextBox();
            this.floorTreeView = new System.Windows.Forms.TreeView();
            this.addStrMemBtn = new System.Windows.Forms.Button();
            this.floorPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.floorDupeCountNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // floorPanel
            // 
            this.floorPanel.AutoSize = true;
            this.floorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.floorPanel.Controls.Add(this.tableLayoutPanel1);
            this.floorPanel.Controls.Add(this.floorTreeView);
            this.floorPanel.Controls.Add(this.addStrMemBtn);
            this.floorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.floorPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.floorPanel.Location = new System.Drawing.Point(0, 0);
            this.floorPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.floorPanel.MinimumSize = new System.Drawing.Size(100, 100);
            this.floorPanel.Name = "floorPanel";
            this.floorPanel.Size = new System.Drawing.Size(303, 150);
            this.floorPanel.TabIndex = 3;
            this.floorPanel.WrapContents = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.Controls.Add(this.floorDupeCountNUD, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.floorUCDeleteBtn, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.floorLbl, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(297, 35);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // floorDupeCountNUD
            // 
            this.floorDupeCountNUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.floorDupeCountNUD.Location = new System.Drawing.Point(3, 3);
            this.floorDupeCountNUD.Name = "floorDupeCountNUD";
            this.floorDupeCountNUD.Size = new System.Drawing.Size(51, 26);
            this.floorDupeCountNUD.TabIndex = 3;
            this.floorDupeCountNUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // floorUCDeleteBtn
            // 
            this.floorUCDeleteBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.floorUCDeleteBtn.Location = new System.Drawing.Point(237, 3);
            this.floorUCDeleteBtn.Name = "floorUCDeleteBtn";
            this.floorUCDeleteBtn.Size = new System.Drawing.Size(57, 29);
            this.floorUCDeleteBtn.TabIndex = 4;
            this.floorUCDeleteBtn.Text = "X";
            this.floorUCDeleteBtn.UseVisualStyleBackColor = true;
            this.floorUCDeleteBtn.Click += new System.EventHandler(this.floorUCDeleteBtn_Click);
            // 
            // floorLbl
            // 
            this.floorLbl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.floorLbl.Location = new System.Drawing.Point(60, 6);
            this.floorLbl.Name = "floorLbl";
            this.floorLbl.ReadOnly = true;
            this.floorLbl.Size = new System.Drawing.Size(171, 26);
            this.floorLbl.TabIndex = 5;
            this.floorLbl.DoubleClick += new System.EventHandler(this.floorLbl_DoubleClick);
            this.floorLbl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floorLbl_KeyPress);
            this.floorLbl.Leave += new System.EventHandler(this.floorLbl_Leave);
            // 
            // floorTreeView
            // 
            this.floorTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.floorTreeView.Location = new System.Drawing.Point(3, 44);
            this.floorTreeView.Name = "floorTreeView";
            this.floorTreeView.Size = new System.Drawing.Size(297, 62);
            this.floorTreeView.TabIndex = 3;
            this.floorTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.floorTreeView_AfterCollapse);
            this.floorTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.floorTreeView_AfterExpand);
            this.floorTreeView.DoubleClick += new System.EventHandler(this.floorTreeView_DoubleClick);
            // 
            // addStrMemBtn
            // 
            this.addStrMemBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addStrMemBtn.AutoSize = true;
            this.addStrMemBtn.Location = new System.Drawing.Point(3, 111);
            this.addStrMemBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.addStrMemBtn.Name = "addStrMemBtn";
            this.addStrMemBtn.Size = new System.Drawing.Size(297, 35);
            this.addStrMemBtn.TabIndex = 1;
            this.addStrMemBtn.Text = "➕STRUCT. MEMBER";
            this.addStrMemBtn.UseVisualStyleBackColor = true;
            this.addStrMemBtn.Click += new System.EventHandler(this.addStrMemBtn_Click);
            // 
            // Floor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.floorPanel);
            this.MinimumSize = new System.Drawing.Size(250, 150);
            this.Name = "Floor";
            this.Size = new System.Drawing.Size(303, 150);
            this.floorPanel.ResumeLayout(false);
            this.floorPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.floorDupeCountNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel floorPanel;
        private System.Windows.Forms.TreeView floorTreeView;
        private System.Windows.Forms.Button addStrMemBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown floorDupeCountNUD;
        private System.Windows.Forms.Button floorUCDeleteBtn;
        private System.Windows.Forms.TextBox floorLbl;
    }
}
