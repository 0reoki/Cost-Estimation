namespace WindowsFormsApp1
{
    partial class ViewDetailedInfoForm
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
            this.panelView = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // panelView
            // 
            this.panelView.AutoScroll = true;
            this.panelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelView.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelView.Location = new System.Drawing.Point(0, 0);
            this.panelView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelView.Name = "panelView";
            this.panelView.Size = new System.Drawing.Size(804, 696);
            this.panelView.TabIndex = 34;
            this.panelView.WrapContents = false;
            // 
            // ViewDetailedInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 696);
            this.Controls.Add(this.panelView);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 320);
            this.Name = "ViewDetailedInfoForm";
            this.Text = "ViewDetailedInfoForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel panelView;
    }
}