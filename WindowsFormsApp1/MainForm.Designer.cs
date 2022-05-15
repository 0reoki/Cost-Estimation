
namespace WindowsFormsApp1
{
    partial class CostEstimationForm
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.estimationPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.totalCostPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.totalLbl = new System.Windows.Forms.Label();
            this.totalcostLbl = new System.Windows.Forms.Label();
            this.topPanel = new System.Windows.Forms.TableLayoutPanel();
            this.paraBtn = new System.Windows.Forms.Button();
            this.addFloorBtn = new System.Windows.Forms.Button();
            this.pdfPanel = new System.Windows.Forms.Panel();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView3 = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.fileMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fileMenu1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenu2 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenu3 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenu4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.fileMenu6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.fileMenu5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.totalCostPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.pdfPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.fileMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.ItemSize = new System.Drawing.Size(60, 25);
            this.tabControl1.Location = new System.Drawing.Point(13, 14);
            this.tabControl1.MinimumSize = new System.Drawing.Size(640, 320);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 1;
            this.tabControl1.Size = new System.Drawing.Size(1235, 656);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(1227, 623);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "File";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rightPanel);
            this.tabPage2.Controls.Add(this.pdfPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(1227, 623);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Home";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rightPanel
            // 
            this.rightPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rightPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.rightPanel.ColumnCount = 1;
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rightPanel.Controls.Add(this.estimationPanel, 0, 1);
            this.rightPanel.Controls.Add(this.totalCostPanel, 0, 2);
            this.rightPanel.Controls.Add(this.topPanel, 0, 0);
            this.rightPanel.Location = new System.Drawing.Point(901, 14);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.RowCount = 3;
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rightPanel.Size = new System.Drawing.Size(320, 599);
            this.rightPanel.TabIndex = 6;
            // 
            // estimationPanel
            // 
            this.estimationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.estimationPanel.AutoScroll = true;
            this.estimationPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.estimationPanel.Location = new System.Drawing.Point(4, 74);
            this.estimationPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.estimationPanel.Name = "estimationPanel";
            this.estimationPanel.Size = new System.Drawing.Size(312, 481);
            this.estimationPanel.TabIndex = 5;
            this.estimationPanel.WrapContents = false;
            // 
            // totalCostPanel
            // 
            this.totalCostPanel.Controls.Add(this.totalLbl);
            this.totalCostPanel.Controls.Add(this.totalcostLbl);
            this.totalCostPanel.Location = new System.Drawing.Point(4, 560);
            this.totalCostPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.totalCostPanel.Name = "totalCostPanel";
            this.totalCostPanel.Size = new System.Drawing.Size(312, 35);
            this.totalCostPanel.TabIndex = 9;
            this.totalCostPanel.WrapContents = false;
            // 
            // totalLbl
            // 
            this.totalLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalLbl.Location = new System.Drawing.Point(1, 0);
            this.totalLbl.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.totalLbl.Name = "totalLbl";
            this.totalLbl.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.totalLbl.Size = new System.Drawing.Size(164, 38);
            this.totalLbl.TabIndex = 8;
            this.totalLbl.Text = "Total Cost:";
            this.totalLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalcostLbl
            // 
            this.totalcostLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalcostLbl.Location = new System.Drawing.Point(169, 0);
            this.totalcostLbl.Margin = new System.Windows.Forms.Padding(1, 0, 3, 0);
            this.totalcostLbl.Name = "totalcostLbl";
            this.totalcostLbl.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.totalcostLbl.Size = new System.Drawing.Size(145, 38);
            this.totalcostLbl.TabIndex = 9;
            this.totalcostLbl.Text = "0 PHP";
            this.totalcostLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // topPanel
            // 
            this.topPanel.ColumnCount = 2;
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topPanel.Controls.Add(this.paraBtn, 1, 0);
            this.topPanel.Controls.Add(this.addFloorBtn, 0, 0);
            this.topPanel.Location = new System.Drawing.Point(4, 4);
            this.topPanel.MaximumSize = new System.Drawing.Size(312, 66);
            this.topPanel.MinimumSize = new System.Drawing.Size(312, 66);
            this.topPanel.Name = "topPanel";
            this.topPanel.RowCount = 1;
            this.topPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topPanel.Size = new System.Drawing.Size(312, 66);
            this.topPanel.TabIndex = 10;
            // 
            // paraBtn
            // 
            this.paraBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paraBtn.AutoSize = true;
            this.paraBtn.Location = new System.Drawing.Point(166, 10);
            this.paraBtn.Margin = new System.Windows.Forms.Padding(10);
            this.paraBtn.MaximumSize = new System.Drawing.Size(140, 50);
            this.paraBtn.MinimumSize = new System.Drawing.Size(140, 50);
            this.paraBtn.Name = "paraBtn";
            this.paraBtn.Size = new System.Drawing.Size(140, 50);
            this.paraBtn.TabIndex = 11;
            this.paraBtn.Text = "Parameters";
            this.paraBtn.UseVisualStyleBackColor = true;
            this.paraBtn.Click += new System.EventHandler(this.paraBtn_Click);
            // 
            // addFloorBtn
            // 
            this.addFloorBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addFloorBtn.AutoSize = true;
            this.addFloorBtn.Location = new System.Drawing.Point(10, 10);
            this.addFloorBtn.Margin = new System.Windows.Forms.Padding(10);
            this.addFloorBtn.MaximumSize = new System.Drawing.Size(140, 50);
            this.addFloorBtn.MinimumSize = new System.Drawing.Size(140, 50);
            this.addFloorBtn.Name = "addFloorBtn";
            this.addFloorBtn.Size = new System.Drawing.Size(140, 50);
            this.addFloorBtn.TabIndex = 10;
            this.addFloorBtn.Text = "Add Floor";
            this.addFloorBtn.UseVisualStyleBackColor = true;
            this.addFloorBtn.Click += new System.EventHandler(this.addFloorBtn_Click);
            // 
            // pdfPanel
            // 
            this.pdfPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pdfPanel.Controls.Add(this.webView);
            this.pdfPanel.Location = new System.Drawing.Point(10, 14);
            this.pdfPanel.Margin = new System.Windows.Forms.Padding(10);
            this.pdfPanel.Name = "pdfPanel";
            this.pdfPanel.Padding = new System.Windows.Forms.Padding(10);
            this.pdfPanel.Size = new System.Drawing.Size(884, 599);
            this.pdfPanel.TabIndex = 7;
            // 
            // webView
            // 
            this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Location = new System.Drawing.Point(10, 10);
            this.webView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(864, 577);
            this.webView.TabIndex = 5;
            this.webView.ZoomFactor = 1D;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1227, 623);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Price";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tableLayoutPanel4);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1227, 623);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "View";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel8, 2, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1221, 617);
            this.tableLayoutPanel4.TabIndex = 11;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel6.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.treeView2, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(409, 2);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(400, 613);
            this.tableLayoutPanel6.TabIndex = 9;
            // 
            // treeView2
            // 
            this.treeView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView2.Location = new System.Drawing.Point(4, 75);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(392, 534);
            this.treeView2.TabIndex = 11;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(392, 64);
            this.tableLayoutPanel7.TabIndex = 10;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(400, 613);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(4, 75);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(392, 534);
            this.treeView1.TabIndex = 11;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.button2, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(392, 64);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.AutoSize = true;
            this.button2.Location = new System.Drawing.Point(10, 10);
            this.button2.Margin = new System.Windows.Forms.Padding(10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(176, 44);
            this.button2.TabIndex = 10;
            this.button2.Text = "Configure ";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel8.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel8.ColumnCount = 1;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.treeView3, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel9, 0, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(815, 2);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(403, 613);
            this.tableLayoutPanel8.TabIndex = 10;
            // 
            // treeView3
            // 
            this.treeView3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView3.Location = new System.Drawing.Point(4, 75);
            this.treeView3.Name = "treeView3";
            this.treeView3.Size = new System.Drawing.Size(395, 534);
            this.treeView3.TabIndex = 11;
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 2;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 1;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(393, 64);
            this.tableLayoutPanel9.TabIndex = 10;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 29);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1227, 623);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Help";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // fileMenu
            // 
            this.fileMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.fileMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu1,
            this.fileMenu2,
            this.fileMenu3,
            this.fileMenu4,
            this.toolStripSeparator2,
            this.fileMenu6,
            this.toolStripSeparator1,
            this.fileMenu5});
            this.fileMenu.Name = "contextMenuStrip1";
            this.fileMenu.Size = new System.Drawing.Size(147, 208);
            // 
            // fileMenu1
            // 
            this.fileMenu1.Name = "fileMenu1";
            this.fileMenu1.Size = new System.Drawing.Size(146, 32);
            this.fileMenu1.Text = "New";
            this.fileMenu1.Click += new System.EventHandler(this.fileMenu1_Click);
            // 
            // fileMenu2
            // 
            this.fileMenu2.Name = "fileMenu2";
            this.fileMenu2.Size = new System.Drawing.Size(146, 32);
            this.fileMenu2.Text = "Open";
            this.fileMenu2.Click += new System.EventHandler(this.fileMenu2_Click);
            // 
            // fileMenu3
            // 
            this.fileMenu3.Name = "fileMenu3";
            this.fileMenu3.Size = new System.Drawing.Size(146, 32);
            this.fileMenu3.Text = "Save";
            this.fileMenu3.Click += new System.EventHandler(this.fileMenu3_Click);
            // 
            // fileMenu4
            // 
            this.fileMenu4.Name = "fileMenu4";
            this.fileMenu4.Size = new System.Drawing.Size(146, 32);
            this.fileMenu4.Text = "Save As";
            this.fileMenu4.Click += new System.EventHandler(this.fileMenu4_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // fileMenu6
            // 
            this.fileMenu6.Name = "fileMenu6";
            this.fileMenu6.Size = new System.Drawing.Size(146, 32);
            this.fileMenu6.Text = "Import";
            this.fileMenu6.Click += new System.EventHandler(this.fileMenu6_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // fileMenu5
            // 
            this.fileMenu5.Name = "fileMenu5";
            this.fileMenu5.Size = new System.Drawing.Size(146, 32);
            this.fileMenu5.Text = "Exit";
            this.fileMenu5.Click += new System.EventHandler(this.fileMenu5_Click);
            // 
            // CostEstimationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 683);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(640, 320);
            this.Name = "CostEstimationForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Building Cost Estimation Application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CostEstimationForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.totalCostPanel.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.pdfPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.fileMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel rightPanel;
        private System.Windows.Forms.FlowLayoutPanel totalCostPanel;
        private System.Windows.Forms.Label totalLbl;
        private System.Windows.Forms.Label totalcostLbl;
        private System.Windows.Forms.FlowLayoutPanel estimationPanel;
        private System.Windows.Forms.Panel pdfPanel;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.ContextMenuStrip fileMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenu1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu2;
        private System.Windows.Forms.ToolStripMenuItem fileMenu3;
        private System.Windows.Forms.ToolStripMenuItem fileMenu4;
        private System.Windows.Forms.ToolStripMenuItem fileMenu5;
        private System.Windows.Forms.Button addFloorBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem fileMenu6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TableLayoutPanel topPanel;
        private System.Windows.Forms.Button paraBtn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.TreeView treeView3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TreeView treeView1;
    }
}

