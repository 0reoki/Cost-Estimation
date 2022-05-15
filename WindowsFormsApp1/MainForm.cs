using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class CostEstimationForm : Form
    {
        //Forms
        public Compute compute = new Compute();
        public ParametersForm pf;
        public Parameters parameters;
        public StructuralMembers structuralMembers;
        private List<Floor> floors = new List<Floor>();

        //Passed Variables

        //Local Variables
        public bool saveFileExists;
        public bool viewInitalized;
        String fileName;

        //Volume Totality Variables (Volume Totality is for pricing computation)
        //Earthwork Variables
        public double excavation_Total, backfillingAndCompaction_Total, gradingAndCompaction_Total,
                      gravelBedding_Total, soilPoisoning_Total;
        //ConcreteWorks Variables (List by struct member [Footing, Concreting, Etc.])
        public double[] cement_Total = new double[6];
        public double[] gravel_Total = new double[6];
        public double[] water_Total = new double[6];
        //StairWorks Variables        

        //Getters and Setters
        public List<Floor> Floors { get => floors; set => floors = value; }

        //General Functions -- START
        public CostEstimationForm()
        {
            InitializeComponent();
            InitializeAsync();

            //Initialize Forms that are single throughout the whole program
            viewInitalized = false;
            saveFileExists = false;
            parameters = new Parameters();
            pf = new ParametersForm(parameters, this);
            structuralMembers = new StructuralMembers(this);

            addFloor();

            //Initialize Local Variables
            fileName = null;
            excavation_Total = 0; 
            backfillingAndCompaction_Total = 0; 
            gradingAndCompaction_Total = 0;
            gravelBedding_Total = 0; 
            soilPoisoning_Total = 0;
            for(int i = 0; i < cement_Total.Length; i++)
            {
                cement_Total[i] = 0;
                gravel_Total[i] = 0;
                water_Total[i] = 0;
            }
        }
        //General Functions -- END

        //Home Functions -- START
        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //File tab button is clicked
            if (e.TabPageIndex == 0)
            {
                e.Cancel = true;
                fileMenu.Show(tabControl1, new Point(0, tabControl1.ItemSize.Height));
            }
            //View Tab button is clicked
            else if (e.TabPageIndex == 3)
            {
                initializeView();

                //Console.WriteLine(structuralMembers.concreteWorkSolutionsC[0][0][0]);
            }
            //Help tab button is clicked
            else if (e.TabPageIndex == 4)
            {
                e.Cancel = true;
            }
        }

        //View Functions -- START
        private void initializeView()
        {
            if (!viewInitalized)
            {
                treeView1.Nodes.Clear();
                treeView2.Nodes.Clear();
                treeView3.Nodes.Clear();
                //Tree View 1 - Earthworks and Concrete Works
                List<TreeNode> nodes1;
                TreeNode tn1 = new TreeNode("Earthworks");
                tn1.Name = "earthworksParent";

                TreeNode tn2 = new TreeNode("Concrete Works");
                tn2.Name = "concreteWorksParent";
                nodes1 = new List<TreeNode>() { tn1, tn2 };

                setTree(nodes1, treeView1);

                //Earthworks
                TreeNode[] found = treeView1.Nodes.Find("earthworksParent", true);

                TreeNode newChild1 = new TreeNode("1.1 Excavation " + excavation_Total.ToString());
                newChild1.Name = "excavation_Total";

                TreeNode newChild2 = new TreeNode("1.2 Back Filling and Compaction " + backfillingAndCompaction_Total.ToString());
                newChild2.Name = "backfillingAndCompaction_Total";

                TreeNode newChild3 = new TreeNode("1.3 Grading and Compaction " + gradingAndCompaction_Total.ToString());
                newChild3.Name = "gradingAndCompaction_Total";

                TreeNode newChild4 = new TreeNode("1.4 Gravel Bedding and Compaction " + gravelBedding_Total.ToString());
                newChild4.Name = "gravelBedding_Total";

                TreeNode newChild5 = new TreeNode("1.5 Soil Poisoning " + soilPoisoning_Total.ToString());
                newChild5.Name = "soilPoisoning_Total";

                found[0].Nodes.Add(newChild1);
                found[0].Nodes.Add(newChild2);
                found[0].Nodes.Add(newChild3);
                found[0].Nodes.Add(newChild4);
                found[0].Nodes.Add(newChild5);

                //Tree View 2 - 
            }
        }
        private void setTree(List<TreeNode> nodes, TreeView treeView)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                treeView.Nodes.Add(nodes[i]);
            }
            AdjustTreeViewHeight(treeView);
        }

        //Local Variables
        const int TVM_GETNEXTITEM = 0x1100 + 10;
        const int TVGN_LASTVISIBLE = 0x000A;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        extern static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        public void AdjustTreeViewHeight(TreeView treeView)
        {
            treeView.Scrollable = false;
            var nodeHandle = SendMessage(treeView.Handle, TVM_GETNEXTITEM,
                TVGN_LASTVISIBLE, IntPtr.Zero);
            var node = treeView.GetType().GetMethod("NodeFromHandle",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(treeView, new object[] { nodeHandle }) as TreeNode;
            var r = node.Bounds;
            treeView.Height = r.Top + r.Height + 4;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            string[] parents = { "Earthworks", "Concrete Works" };
            TreeViewHitTestInfo info = treeView1.HitTest(treeView1.PointToClient(Cursor.Position));
            try
            {
                if (treeView1.SelectedNode != null)
                {
                    if (Array.IndexOf(parents, info.Node.Text) < 0)
                    {
                        ViewDetailedInfoForm vf = new ViewDetailedInfoForm(info.Node.Text.Substring(0, 3), this, structuralMembers);
                        vf.ShowDialog();
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }
        //View functions -- END

        //File Menu - New
        private void fileMenu1_Click(object sender, EventArgs e)
        {
            new CostEstimationForm().Show();
        }

        //File Menu - Open
        private void fileMenu2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Open Show Estimation File";
            openDialog.Filter = "Know Estimation files (*.est)|*.est";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                //try
                //{
                    if ((myStream = openDialog.OpenFile()) != null)
                    {
                        fileName = openDialog.FileName;
                        this.Text = "Building Cost Estimation Application - " + Path.GetFileName(fileName);
                        saveFileExists = true;
                        using (myStream)
                        {
                            StreamReader reader = new StreamReader(myStream);
                            string stringFile = reader.ReadToEnd();
                            SaveToProgram(stringFile);
                        }
                    }
                /*}
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }*/
            }
            viewInitalized = false;
            initializeView();
        }

        //Save
        private void fileMenu3_Click(object sender, EventArgs e)
        {
            if(fileName == null)
            {
                saveAs();
            }
            else
            {
                SaveToFile(fileName);
            }
        }

        //File Menu - Save As
        private void fileMenu4_Click(object sender, EventArgs e)
        {
            saveAs();
        }

        private void saveAs()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "New Estimate.est";
            saveDialog.Filter = "Know Estimation files (*.est)|*.est";
            DialogResult result = saveDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                fileName = saveDialog.FileName;
                this.Text = "Building Cost Estimation Application - " + Path.GetFileName(fileName);
                saveFileExists = true;
                SaveToFile(fileName);
            }
        }

        //File Menu - Exit
        private void fileMenu5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //File Menu - Import
        private void fileMenu6_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { ValidateNames = true, Multiselect = false, Filter = "PDF|*.pdf" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //webBrowser1.Navigate(ofd.fileName);
                    webView.CoreWebView2.Navigate("file:///" + ofd.FileName);
                }
            }
        }

        private void addFloorBtn_Click(object sender, EventArgs e)
        {
            addFloor();
        }

        private void addFloor()
        {
            initializeVariables();

            //Set a ground floor
            Floor floor = new Floor(this, false);
            floors.Add(floor);
            estimationPanel.Controls.Add(floor);
        }

        private void initializeVariables()
        {
            //Initialize variables in Structural Members for every floor created
            if (Floors.Count == 0)//Ground Floor
            {
                //Footings
                List<List<string>> newList = new List<List<string>>();
                List<List<string>> newList2 = new List<List<string>>();
                structuralMembers.footingsColumn.Add(newList);
                structuralMembers.footingsWall.Add(newList2);

                //Columns
                List<string> newList3 = new List<string>();
                List<List<string>> newList4 = new List<List<string>>();
                List<List<string>> newList5 = new List<List<string>>();
                List<List<string>> newList6 = new List<List<string>>();
                structuralMembers.columnNames.Add(newList3);
                structuralMembers.column.Add(newList4);
                structuralMembers.columnLateralTies.Add(newList5);
                structuralMembers.columnSpacing.Add(newList6);

                //Beams
                List<string> newList7 = new List<string>();
                List<List<string>> newList8 = new List<List<string>>();
                List<List<List<string>>> newList9 = new List<List<List<string>>>();
                List<List<string>> newList10 = new List<List<string>>();
                structuralMembers.beamNames.Add(newList7);
                structuralMembers.beam.Add(newList8);
                structuralMembers.beamRow.Add(newList9);
                structuralMembers.beamSchedule.Add(newList10);

                //Slabs
                List<string> newList11 = new List<string>();
                List<List<string>> newList12 = new List<List<string>>();
                structuralMembers.slabNames.Add(newList11);
                structuralMembers.slab.Add(newList12);

                //Stairs
                List<List<string>> newList14 = new List<List<string>>();
                List<string> newList15 = new List<string>();
                structuralMembers.stairs.Add(newList14);
                structuralMembers.stairsNames.Add(newList15);

                //Roof
                List<List<string>> newList16 = new List<List<string>>();
                List<List<string>> newList17 = new List<List<string>>();
                List<string> newList18 = new List<string>();
                structuralMembers.roof.Add(newList16);
                structuralMembers.roofHRS.Add(newList17);
                structuralMembers.roofNames.Add(newList18);

                //Solution Variables
                List<List<double>> newList19 = new List<List<double>>();
                List<List<double>> newList20 = new List<List<double>>();
                List<List<double>> newList21 = new List<List<double>>();
                List<List<double>> newList22 = new List<List<double>>();
                structuralMembers.concreteWorkSolutionsC.Add(newList19);
                structuralMembers.concreteWorkSolutionsBR.Add(newList20);
                structuralMembers.concreteWorkSolutionsSL.Add(newList21);
                structuralMembers.concreteWorkSolutionsST.Add(newList22);
            }
            else //Upper Floors
            {
                //Columns
                List<string> newList3 = new List<string>();
                List<List<string>> newList4 = new List<List<string>>();
                List<List<string>> newList5 = new List<List<string>>();
                List<List<string>> newList6 = new List<List<string>>();
                structuralMembers.columnNames.Add(newList3);
                structuralMembers.column.Add(newList4);
                structuralMembers.columnLateralTies.Add(newList5);
                structuralMembers.columnSpacing.Add(newList6);

                //Beams
                List<string> newList7 = new List<string>();
                List<List<string>> newList8 = new List<List<string>>();
                List<List<List<string>>> newList9 = new List<List<List<string>>>();
                List<List<string>> newList10 = new List<List<string>>();
                structuralMembers.beamNames.Add(newList7);
                structuralMembers.beam.Add(newList8);
                structuralMembers.beamRow.Add(newList9);
                structuralMembers.beamSchedule.Add(newList10);

                //Slabs
                List<string> newList11 = new List<string>();
                List<List<string>> newList12 = new List<List<string>>();
                List<List<string>> newList13 = new List<List<string>>();
                structuralMembers.slabNames.Add(newList11);
                structuralMembers.slab.Add(newList12);
                structuralMembers.slabSchedule.Add(newList13);

                //Stairs
                List<List<string>> newList14 = new List<List<string>>();
                List<string> newList15 = new List<string>();
                structuralMembers.stairs.Add(newList14);
                structuralMembers.stairsNames.Add(newList15);

                //Roof
                List<List<string>> newList16 = new List<List<string>>();
                List<List<string>> newList17 = new List<List<string>>();
                List<string> newList18 = new List<string>();
                structuralMembers.roof.Add(newList16);
                structuralMembers.roofHRS.Add(newList17);
                structuralMembers.roofNames.Add(newList18);

                //Solution Variables
                List<List<double>> newList19 = new List<List<double>>();
                List<List<double>> newList20 = new List<List<double>>();
                List<List<double>> newList21 = new List<List<double>>();
                List<List<double>> newList22 = new List<List<double>>();
                structuralMembers.concreteWorkSolutionsC.Add(newList19);
                structuralMembers.concreteWorkSolutionsBR.Add(newList20);
                structuralMembers.concreteWorkSolutionsSL.Add(newList21);
                structuralMembers.concreteWorkSolutionsST.Add(newList22);
            }
        }

        public void refreshFloors()
        {
            //Remove all controls
            estimationPanel.Controls.Clear();

            //Add all controls
            for (int i = 0; i < Floors.Count; i++)
            {
                estimationPanel.Controls.Add(Floors[i]);

                if (i == 0)
                {
                    Floors[i].setLabel = "GROUND FLOOR";
                }
                else
                {
                    string label = Floors[i].getValues()[1];
                    if (label.Equals(AddOrdinal(i + 1) + " FLOOR"))
                    {
                        Floors[i].setLabel = AddOrdinal(i) + " FLOOR";
                    }
                    else
                    {
                        Floors[i].setLabel = label;
                    }
                }
            }
        }

        private void paraBtn_Click(object sender, EventArgs e)
        {
            pf.ShowDialog();
        }

        //Home Functions -- END

        //Long Functions -- START
        private void SaveToFile(String fileName)
        {
            string stringParam = "";

            //Floors -- START
            stringParam += "Floors|\n";

            int count = 1;
            foreach(Floor floor in floors)
            {
                stringParam += "Floor-" + count + "|";      //Floor #
                stringParam += floor.getValues()[0] + "|";  //Floor Count
                stringParam += floor.getValues()[1] + "|";  //Floor Name
                foreach (TreeNode parentNode in floor.nodes)
                {
                    stringParam += parentNode.Text + "|";
                    if (parentNode.Nodes.Count == 0)
                    {
                        //Parent na walang laman
                    }
                    else
                    {
                        //Parent na may laman
                        foreach(TreeNode childNode in parentNode.Nodes)
                        {
                            stringParam += childNode.Text + "|" + childNode.Name + "|";
                        }
                    }
                }
                count++;
            }
            //Floors -- END

            //Parameters -- START
            stringParam += "\nParameters|\n";

            //Earthworks
            stringParam += "Earthworks|\n" +
            parameters.earth_CF_FA + "|" + parameters.earth_CF_TH + "|" + parameters.earth_CF_TY + "|" + parameters.earth_CF_CF + "|" +
            parameters.earth_WF_FA + "|" + parameters.earth_WF_TH + "|" + parameters.earth_WF_TY + "|" + parameters.earth_WF_CF + "|" +
            parameters.earth_WTB_FA + "|" + parameters.earth_WTB_TH + "|" + parameters.earth_WTB_TY + "|" + parameters.earth_WTB_CF + "|" +
            parameters.earth_SG_AS + "|" + parameters.earth_SG_TS + "|" + parameters.earth_SG_TH + "|" +
            parameters.earth_SG_TY + "|" + parameters.earth_SG_CF + "|";
            for(int i = 0; i < parameters.earth_elevations.Count; i++)
            {
                stringParam += "Elevation-" + (i + 1) + "|";
                stringParam += parameters.earth_elevations[i][0] + "|";
                stringParam += parameters.earth_elevations[i][1] + "|";
                stringParam += parameters.earth_elevations[i][2] + "|";
                stringParam += parameters.earth_elevations[i][3] + "|";
            }

            //Formworks
            stringParam += "\nFormworks|\n" + parameters.form_SM_F_FL + "|" +
            parameters.form_SM_C_FL + "|" + parameters.form_SM_C_VS + "|" + parameters.form_SM_C_HB + "|" +
            parameters.form_SM_B_FL + "|" + parameters.form_SM_B_VS + "|" + parameters.form_SM_B_HB + "|" + parameters.form_SM_B_DB + "|" +
            parameters.form_SM_HS_VS + "|" +
            parameters.form_SM_ST_FL + "|" + parameters.form_SM_ST_VS + "|" +
            parameters.form_F_T + "|" + parameters.form_F_NU + "|" + parameters.form_F_N + "|";

            //Concrete
            stringParam += "\nConcrete|\n" +
            parameters.conc_cmIsSelected[0] + "|" + parameters.conc_CM_F_CG + "|" + parameters.conc_CM_F_GT + "|" + parameters.conc_CM_F_RM + "|" +
            parameters.conc_cmIsSelected[1] + "|" + parameters.conc_CM_C_CG + "|" + parameters.conc_CM_C_GT + "|" + parameters.conc_CM_C_RM + "|" +
            parameters.conc_cmIsSelected[2] + "|" + parameters.conc_CM_B_CG + "|" + parameters.conc_CM_B_GT + "|" + parameters.conc_CM_B_RM + "|" +
            parameters.conc_cmIsSelected[3] + "|" + parameters.conc_CM_S_CG + "|" + parameters.conc_CM_S_GT + "|" + parameters.conc_CM_S_RM + "|" +

            parameters.conc_CM_W_MEW_CM + "|" + parameters.conc_CM_W_MIW_CM + "|" + parameters.conc_CM_W_P_CM + "|" + parameters.conc_CM_W_P_PT + "|" +

            parameters.conc_cmIsSelected[4] + "|" + parameters.conc_CM_ST_CG + "|" + parameters.conc_CM_ST_GT + "|" + parameters.conc_CM_ST_RM + "|" +

            parameters.conc_CC_F + "|" + parameters.conc_CC_SS + "|" + parameters.conc_CC_SG + "|" + parameters.conc_CC_BEE + "|" +
            parameters.conc_CC_BEW + "|" + parameters.conc_CC_CEE + "|" + parameters.conc_CC_CEW + "|" ;

            //Reinforcement 
            stringParam += "\nReinforcement|\n";
            stringParam += "Tension_Bars|";
            int j = 1;
            foreach (DataRow dtRow in parameters.rein_LSL_TB_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_LSL_TB_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            j = 1;
            for(int i = 0; i < parameters.rein_LSL_TB_fc_list.Count; i++)
            {
                stringParam += "FCPrime-" + j + "|" + parameters.rein_LSL_TB_fc_list[i] + "|";
                j++;
            }
            stringParam += "Compression_Bars|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_LSL_CB_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_LSL_CB_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            j = 1;
            for (int i = 0; i < parameters.rein_LSL_CB_fc_list.Count; i++)
            {
                stringParam += "FCPrime-" + j + "|" + parameters.rein_LSL_CB_fc_list[i] + "|";
                j++;
            }
            stringParam += "Main_Bars|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_BEH_MB_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_BEH_MB_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            stringParam += "Stirrups_and_Ties|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_BEH_ST_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_BEH_ST_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            stringParam += "Weight|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_W_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_W_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            stringParam += parameters.rein_S_C_SL + "|" + parameters.rein_S_C_SZ + "|" + parameters.rein_S_C_AP + "|" + parameters.rein_S_C_MVDAB + "|" +
                      parameters.rein_S_B_T_SL + "|" + parameters.rein_S_B_T_SZ + "|" + parameters.rein_S_B_T_AP + "|" + parameters.rein_S_B_B_SL + "|" + parameters.rein_S_B_B_SZ + "|" + parameters.rein_S_B_B_AP + "|" + parameters.rein_S_B_MHDAB + "|" +
                      parameters.rein_S_S_T_SL + "|" + parameters.rein_S_S_B_SL + "|" +
                      parameters.rein_RG_C + "|" + parameters.rein_RG_F + "|" + parameters.rein_RG_B + "|" + parameters.rein_RG_ST + "|" + parameters.rein_RG_W + "|" + parameters.rein_RG_SL + "|";

            for (int col = 0; col < parameters.rein_mfIsSelected.GetLength(0); col++)
                for (int row = 0; row < parameters.rein_mfIsSelected.GetLength(1); row++)
                    stringParam += parameters.rein_mfIsSelected[col, row] + "|";

            //Paint
            stringParam += "\nPaint|\n" + parameters.paint_SCL + "|";
            for (int i = 0; i < parameters.paint_Area.Count; i++)
            {
                stringParam += "Paint_Area-" + (i + 1) + "|";
                stringParam += parameters.paint_Area[i][0] + "|";
                stringParam += parameters.paint_Area[i][1] + "|";
                stringParam += parameters.paint_Area[i][2] + "|";
            }

            //Tiles
            stringParam += "\nTiles|\n" + parameters.tiles_FS + "|" + parameters.tiles_TG + "|";
            for (int i = 0; i < parameters.tiles_Area.Count; i++)
            {
                stringParam += "Tile_Area-" + (i + 1) + "|";
                stringParam += parameters.tiles_Area[i][0] + "|";
                stringParam += parameters.tiles_Area[i][1] + "|";
                stringParam += parameters.tiles_Area[i][2] + "|";
            }

            //Masonry
            stringParam += "\nMasonry|\n" + parameters.mason_CHB_EW + "|" + parameters.mason_CHB_IW + "|";
            for (int i = 0; i < parameters.mason_exteriorWall.Count; i++)
            {
                stringParam += "Exterior_Wall-" + (i + 1) + "|";
                stringParam += parameters.mason_exteriorWall[i][0] + "|";
                stringParam += parameters.mason_exteriorWall[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_exteriorWindow.Count; i++)
            {
                stringParam += "Exterior_Window-" + (i + 1) + "|";
                stringParam += parameters.mason_exteriorWindow[i][0] + "|";
                stringParam += parameters.mason_exteriorWindow[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_exteriorDoor.Count; i++)
            {
                stringParam += "Exterior_Door-" + (i + 1) + "|";
                stringParam += parameters.mason_exteriorDoor[i][0] + "|";
                stringParam += parameters.mason_exteriorDoor[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_interiorWall.Count; i++)
            {
                stringParam += "Interior_Wall-" + (i + 1) + "|";
                stringParam += parameters.mason_interiorWall[i][0] + "|";
                stringParam += parameters.mason_interiorWall[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_interiorWindow.Count; i++)
            {
                stringParam += "Interior_Window-" + (i + 1) + "|";
                stringParam += parameters.mason_interiorWindow[i][0] + "|";
                stringParam += parameters.mason_interiorWindow[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_interiorDoor.Count; i++)
            {
                stringParam += "Interior_Door-" + (i + 1) + "|";
                stringParam += parameters.mason_interiorDoor[i][0] + "|";
                stringParam += parameters.mason_interiorDoor[i][1] + "|";
            }

            stringParam += "END|" + parameters.mason_RTW_VS + "|" + parameters.mason_RTW_HSL + "|" +
            parameters.mason_RTW_RG + "|" + parameters.mason_RTW_BD + "|" +
            parameters.mason_RTW_RL + "|" + parameters.mason_RTW_LTW + "|";


            //Stairs
            //stringParam += "\nStairs|\n";

            //Labor and Equipment
            stringParam += "\nLabor_and_Equipment|\n" + parameters.labor_RD + "|";
            for (int i = 0; i < parameters.labor_MP.Count; i++)
            {
                stringParam += "Man_Power-" + (i + 1) + "|";
                stringParam += parameters.labor_MP[i][0] + "|";
                stringParam += parameters.labor_MP[i][1] + "|";
            }
            for (int i = 0; i < parameters.labor_EQP.Count; i++)
            {
                stringParam += "Equipment-" + (i + 1) + "|";
                stringParam += parameters.labor_EQP[i][0] + "|";
                stringParam += parameters.labor_EQP[i][1] + "|";
            }

            //Misc
            stringParam += "\nMisc|\n";
            for (int i = 0; i < parameters.misc_CustomItems.Count; i++)
            {
                stringParam += "Custom_Item-" + (i + 1) + "|";
                stringParam += parameters.misc_CustomItems[i][0] + "|";
                stringParam += parameters.misc_CustomItems[i][1] + "|";
            }

            //Parameters -- END

            //Structural Members -- START
            stringParam += "Structural-Members|\n";

            //Footings
            stringParam += "Footings|\n" + "Floor-1" + "|"; //Only Ground Floor exists
            for (int i = 0; i < structuralMembers.footingColumnNames.Count; i++)
            {
                stringParam += "Column-Footing-" + (i + 1) + "|" + structuralMembers.footingColumnNames[i] + "|";
                foreach(string value in structuralMembers.footingsColumn[0][i])
                {
                    stringParam += value + "|";
                }
            }
            for (int i = 0; i < structuralMembers.footingWallNames.Count; i++)
            {
                stringParam += "Wall-Footing-" + (i + 1) + "|" + structuralMembers.footingWallNames[i] + "|";
                foreach (string value in structuralMembers.footingsWall[0][i])
                {
                    stringParam += value + "|";
                }
            }

            //Columns
            stringParam += "\nColumns|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.column)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Column-" + (k + 1) + "|";
                    stringParam += structuralMembers.columnNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Lateral-Ties" + "|";
                    foreach (string value in structuralMembers.columnLateralTies[j][k])
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Spacing" + "|";
                    foreach (string value in structuralMembers.columnSpacing[j][k])
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            //Beams
            stringParam += "\nBeams|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.beam)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Beam-" + (k + 1) + "|";
                    stringParam += structuralMembers.beamNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Beam-Rows" + "|";
                    foreach (List<string> row in structuralMembers.beamRow[j][k])
                    {
                        stringParam += row[0] + "|";
                        stringParam += row[1] + "|";
                        stringParam += row[2] + "|";
                        stringParam += row[3] + "|";
                        stringParam += row[4] + "|";
                    }
                    k++;
                }
                stringParam += "Beam-Schedules" + "|";
                foreach (List<string> schedule in structuralMembers.beamSchedule[j])
                    foreach(string value in schedule) 
                        stringParam += value + "|";
                j++;
            }
            
            //Slabs
            stringParam += "\nSlabs|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.slab)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Slab-" + (k + 1) + "|";
                    stringParam += structuralMembers.slabNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                if(j > 0)
                {
                    stringParam += "Slab-Schedules" + "|";
                    foreach (List<string> schedule in structuralMembers.slabSchedule[j - 1])
                        foreach (string value in schedule)
                            stringParam += value + "|";
                }
                j++;
            }

            //Stairs
            stringParam += "\nStairs|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.stairs)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach(List<string> member in floor)
                {
                    stringParam += "Stair-" + (k + 1) + "|";
                    stringParam += structuralMembers.stairsNames[j][k] + "|";
                    foreach(string value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            //Roof
            stringParam += "\nRoof|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.roof)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Roof-" + (k + 1) + "|";
                    stringParam += structuralMembers.roofNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Roof-Sheet" + "|";
                    foreach (string value in structuralMembers.roofHRS[j][k])
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Structural Members -- END

            //Solutions -- START
            stringParam += "\nSolutions|\n";

            //Earth Works
            stringParam += "Earthworks|\n" + "Floor-1" + "|"; //Only Ground Floor exists
            j = 0;
            foreach (List<double> solutions in structuralMembers.earthworkSolutions)
            {
                stringParam += "earthwork-" + (j + 1) + "|";
                foreach (double value in solutions)
                {
                    stringParam += value + "|";
                }
                j++;
            }
            stringParam += "Extra-Earthworks|";
            foreach(double value in structuralMembers.extraEarthworkSolutions)
            {
                stringParam += value + "|";
            }

            //Concrete Works
            stringParam += "\nConcrete-Works|\n";
            stringParam += "ConcreteF|\n";
            j = 0;
            foreach (List<double> solutions in structuralMembers.concreteWorkSolutionsF)
            {
                stringParam += "concreteF-" + (j + 1) + "|";
                foreach (double value in solutions)
                {
                    stringParam += value + "|";
                }
                j++;
            }

            stringParam += "\nConcreteC|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsC)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteC-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            stringParam += "\nConcreteBR|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsBR)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteBR-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            stringParam += "\nConcreteSL|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsSL)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteSL-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            stringParam += "\nConcreteST|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsST)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteST-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            stringParam += "\nConcreteFS|\n";
            foreach (double value in structuralMembers.concreteWorkSolutionsFS)
            {
                stringParam += value + "|";
            }

            //Solutions -- END

            //Totalities -- START
            stringParam += "\nTotalities|\n";

            //Earthworks
            stringParam += "\nEarthworks|\n";
            stringParam += excavation_Total + "|";
            stringParam += backfillingAndCompaction_Total + "|";
            stringParam += gradingAndCompaction_Total + "|";
            stringParam += gravelBedding_Total + "|";
            stringParam += soilPoisoning_Total + "|";

            //Totalities -- END

            stringParam += "\nEND";

            //Save to File
            File.WriteAllText(fileName, stringParam);
        }

        private void initializeVariablesFromFile()
        {
            //Initialize variables in Structural Members for every floor created
            if (Floors.Count == 0)//Ground Floor
            {
                //Footings
                List<List<string>> newList = new List<List<string>>();
                List<List<string>> newList2 = new List<List<string>>();
                structuralMembers.footingsColumn.Add(newList);
                structuralMembers.footingsWall.Add(newList2);
            }

            /*Solution Variables
            List<List<double>> newList19 = new List<List<double>>();
            List<List<double>> newList20 = new List<List<double>>();
            List<List<double>> newList21 = new List<List<double>>();
            List<List<double>> newList22 = new List<List<double>>();
            structuralMembers.concreteWorkSolutionsC.Add(newList19);
            structuralMembers.concreteWorkSolutionsBR.Add(newList20);
            structuralMembers.concreteWorkSolutionsSL.Add(newList21);
            structuralMembers.concreteWorkSolutionsST.Add(newList22);
            //*/
        }

        private void SaveToProgram(string stringFile)
        {
            //Remove newlines
            stringFile = stringFile.Replace("\n", String.Empty);
            //Divide data, store into tokens
            string[] tokens = stringFile.Split('|');

            //Save Floors -- START
            int i = 1;
            int j = 0;

            //Clear floor related variables
            floors.Clear();
            estimationPanel.Controls.Clear();
            /*
            foreach (Floor floor in floors)
            {
                foreach (TreeNode parentNode in floor.nodes)
                {
                    if (parentNode.Nodes.Count == 0)
                    {
                        //Parent na walang laman
                    }
                    else
                    {
                        //Parent na may laman
                        parentNode.Nodes.Clear();
                    }
                }
            }
            */

            //Clear variables in Parameters
            parameters.earth_elevations.Clear();

            parameters.rein_LSL_TB_fc_list.Clear();
            parameters.rein_LSL_CB_fc_list.Clear();
            parameters.rein_LSL_TB_fc_list.Clear();

            parameters.paint_Area.Clear();

            parameters.tiles_Area.Clear();

            parameters.mason_exteriorWall.Clear();
            parameters.mason_exteriorWindow.Clear();
            parameters.mason_exteriorDoor.Clear();
            parameters.mason_interiorWall.Clear();
            parameters.mason_interiorWindow.Clear();
            parameters.mason_interiorDoor.Clear();

            parameters.labor_MP.Clear();
            parameters.labor_EQP.Clear();

            parameters.misc_CustomItems.Clear();

            //Clear variables in AddStructForm
            structuralMembers.footingColumnNames.Clear();
            structuralMembers.footingWallNames.Clear();
            structuralMembers.footingsColumn.Clear();
            structuralMembers.footingsWall.Clear();

            structuralMembers.columnNames.Clear();
            structuralMembers.column.Clear();
            structuralMembers.columnLateralTies.Clear();
            structuralMembers.columnSpacing.Clear();

            structuralMembers.beamNames.Clear();
            structuralMembers.beam.Clear();
            structuralMembers.beamRow.Clear();
            structuralMembers.beamSchedule.Clear();

            structuralMembers.slabNames.Clear();
            structuralMembers.slab.Clear();
            structuralMembers.slabSchedule.Clear();

            structuralMembers.stairsNames.Clear();
            structuralMembers.stairs.Clear();

            structuralMembers.roofNames.Clear();
            structuralMembers.roof.Clear();
            structuralMembers.roofHRS.Clear();

            structuralMembers.earthworkSolutions.Clear();

            structuralMembers.concreteWorkSolutionsF.Clear();
            structuralMembers.concreteWorkSolutionsC.Clear();
            structuralMembers.concreteWorkSolutionsBR.Clear();
            structuralMembers.concreteWorkSolutionsSL.Clear();
            structuralMembers.concreteWorkSolutionsST.Clear();

            //Init variables for StructuralMember

            j = 0;
            while (!tokens[i].Equals("Parameters"))
            {
                initializeVariablesFromFile();
                //Set the floor
                Floor floor = new Floor(this, false);
                floors.Add(floor);
                estimationPanel.Controls.Add(floor);

                int floorMultiplier = 0;
                string floorName = "";

                j++;
                if (tokens[i].Equals("Floor-" + j))
                {
                    i++;
                    floorMultiplier = int.Parse(tokens[i]); i++;
                    floorName = tokens[i]; i++;
                }

                while (!tokens[i].Equals("COLUMNS")) //FOOTINGS
                {
                    i++;
                    if (!tokens[i].Equals("COLUMNS")){
                        TreeNode[] found = floor.treeView.Nodes.Find("footingParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);

                        if (newChild.Name[0] == 'F')
                            floor.footingCount++;
                        else
                            floor.wallFootingCount++;
                    }
                }

                while (!tokens[i].Equals("BEAMS")) //COLUMNS
                {
                    i++;
                    if (!tokens[i].Equals("BEAMS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("columnParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.columnCount++;
                    }
                }

                while(!tokens[i].Equals("SLABS")) //BEAMS
                {
                    i++;
                    if (!tokens[i].Equals("SLABS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("beamParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                    }
                }

                while (!tokens[i].Equals("STAIRS")) //SLABS
                {
                    i++;
                    if (!tokens[i].Equals("STAIRS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("slabParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                    }
                }

                while (!tokens[i].Equals("ROOFINGS")) //STAIRS
                {
                    i++;
                    if (!tokens[i].Equals("ROOFINGS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("stairsParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.stairsCount++;
                    }
                }

                while (!tokens[i].Equals("Parameters") && !tokens[i].Equals("Floor-" + (j + 1))) //ROOF
                {
                    i++;
                    if (!tokens[i].Equals("Parameters") && !tokens[i].Equals("Floor-" + (j + 1)))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("roofParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.roofCount++;
                    }
                }

                floor.setValues(floorMultiplier, floorName);
            }

            //Save Floors -- END

            //Save to Parameters -- START
            i++;

            //Earthworks
            i++;
            parameters.earth_CF_FA = tokens[i]; i++;
            parameters.earth_CF_TH = tokens[i]; i++;
            parameters.earth_CF_TY = tokens[i]; i++;
            parameters.earth_CF_CF = tokens[i]; i++;
            parameters.earth_WF_FA = tokens[i]; i++;
            parameters.earth_WF_TH = tokens[i]; i++;
            parameters.earth_WF_TY = tokens[i]; i++;
            parameters.earth_WF_CF = tokens[i]; i++;
            parameters.earth_WTB_FA = tokens[i]; i++;
            parameters.earth_WTB_TH = tokens[i]; i++;
            parameters.earth_WTB_TY = tokens[i]; i++;
            parameters.earth_WTB_CF = tokens[i]; i++;
            parameters.earth_SG_AS = tokens[i]; i++;
            parameters.earth_SG_TS = tokens[i]; i++;
            parameters.earth_SG_TH = tokens[i]; i++;
            parameters.earth_SG_TY = tokens[i]; i++;
            parameters.earth_SG_CF = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Formworks"))
            {
                j++;
                if(tokens[i].Equals("Elevation-" + j))
                {
                    i++;
                    string[] toAdd = new string[4];
                    int k = 0;
                    while (!tokens[i].Equals("Elevation-" + (j + 1)) && !tokens[i].Equals("Formworks"))
                    {
                        toAdd[k] = (tokens[i]); i++; k++;
                    }
                    parameters.earth_elevations.Add(toAdd);
                }
            }

            //Formworks
            i++;
            parameters.form_SM_F_FL = tokens[i]; i++;
            parameters.form_SM_C_FL = tokens[i]; i++;
            parameters.form_SM_C_VS = tokens[i]; i++;
            parameters.form_SM_C_HB = tokens[i]; i++;
            parameters.form_SM_B_FL = tokens[i]; i++;
            parameters.form_SM_B_VS = tokens[i]; i++;
            parameters.form_SM_B_HB = tokens[i]; i++;
            parameters.form_SM_B_DB = tokens[i]; i++;
            parameters.form_SM_HS_VS = tokens[i]; i++;
            parameters.form_SM_ST_FL = tokens[i]; i++;
            parameters.form_SM_ST_VS = tokens[i]; i++;
            parameters.form_F_T = tokens[i]; i++;
            parameters.form_F_NU = tokens[i]; i++;
            parameters.form_F_N = tokens[i]; i++;

            //Concrete
            i++;
            parameters.conc_cmIsSelected[0] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_F_CG = tokens[i]; i++;
            parameters.conc_CM_F_GT = tokens[i]; i++;
            parameters.conc_CM_F_RM = tokens[i]; i++;
            parameters.conc_cmIsSelected[1] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_C_CG = tokens[i]; i++;
            parameters.conc_CM_C_GT = tokens[i]; i++;
            parameters.conc_CM_C_RM = tokens[i]; i++;
            parameters.conc_cmIsSelected[2] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_B_CG = tokens[i]; i++;
            parameters.conc_CM_B_GT = tokens[i]; i++;
            parameters.conc_CM_B_RM = tokens[i]; i++;
            parameters.conc_cmIsSelected[3] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_S_CG = tokens[i]; i++;
            parameters.conc_CM_S_GT = tokens[i]; i++;
            parameters.conc_CM_S_RM = tokens[i]; i++;

            parameters.conc_CM_W_MEW_CM = tokens[i]; i++;
            parameters.conc_CM_W_MIW_CM = tokens[i]; i++;
            parameters.conc_CM_W_P_CM = tokens[i]; i++;
            parameters.conc_CM_W_P_PT = tokens[i]; i++;

            parameters.conc_cmIsSelected[4] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_ST_CG = tokens[i]; i++;
            parameters.conc_CM_ST_GT = tokens[i]; i++;
            parameters.conc_CM_ST_RM = tokens[i]; i++;

            parameters.conc_CC_F = tokens[i]; i++;
            parameters.conc_CC_SS = tokens[i]; i++;
            parameters.conc_CC_SG = tokens[i]; i++;
            parameters.conc_CC_BEE = tokens[i]; i++;
            parameters.conc_CC_BEW = tokens[i]; i++;
            parameters.conc_CC_CEE = tokens[i]; i++;
            parameters.conc_CC_CEW = tokens[i]; i++;

            //Reinforcements 
            i++; j = 0; i++;
            parameters.rein_LSL_TB_dt.Rows.Clear();
            while (!tokens[i].Equals("FCPrime-1") && !tokens[i].Equals("Compression_Bars"))
            {
                j++; 
                if (tokens[i].Equals("Compression_Bars"))
                {
                    break;
                }
                else if (tokens[i].Equals("Row-1"))
                {
                    parameters.rein_LSL_TB_dt.Columns.Clear();
                    int headerCount, temp;
                    temp = i; 
                    while (!tokens[temp].Equals("Row-" + (j + 1)) && !tokens[temp].Equals("FCPrime-1"))
                    {
                        temp++;
                    }
                    headerCount = temp - i;
                    for (int k = 1; k < headerCount; k++)
                    {
                        if (k == 0)
                        {
                            parameters.rein_LSL_TB_dt.Columns.Add("Bar Sizes");
                        }
                        else
                        {
                            parameters.rein_LSL_TB_dt.Columns.Add("LS" + k + " ƒ'c (MPa):");
                        }
                    }
                }

                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j) && !tokens[i].Equals("FCPrime-1"))
                {
                    i++;
                    while(!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("FCPrime-1"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }          
                    parameters.rein_LSL_TB_dt.Rows.Add(tempList.ToArray());
                }
            }
            j = 0;
            while (!tokens[i].Equals("Compression_Bars"))
            {
                j++;
                if (tokens[i].Equals("FCPrime-" + j))
                {
                    i++;
                    parameters.rein_LSL_TB_fc_list.Add(tokens[i]);
                }
                i++;
            }
            
            j = 0; i++;
            parameters.rein_LSL_CB_dt.Rows.Clear();
            while (!tokens[i].Equals("FCPrime-1") && !tokens[i].Equals("Main_Bars"))
            {
                j++;
                if (tokens[i].Equals("Main_Bars"))
                {
                    break;
                }
                else if (tokens[i].Equals("Row-1"))
                {
                    parameters.rein_LSL_CB_dt.Columns.Clear();
                    int headerCount, temp;
                    temp = i;
                    while (!tokens[temp].Equals("Row-" + (j + 1)) && !tokens[temp].Equals("FCPrime-1"))
                    {
                        temp++;
                    }
                    headerCount = temp - i;
                    for (int k = 1; k < headerCount; k++)
                    {
                        if (k == 0)
                        {
                            parameters.rein_LSL_CB_dt.Columns.Add("Bar Sizes");
                        }
                        else
                        {
                            parameters.rein_LSL_CB_dt.Columns.Add("LS" + k + " ƒ'c (MPa):");
                        }
                    }
                }

                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j) && !tokens[i].Equals("FCPrime-1"))
                {
                    i++;
                    while (!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("FCPrime-1"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }
                    parameters.rein_LSL_CB_dt.Rows.Add(tempList.ToArray());
                }
            }
            j = 0;
            while (!tokens[i].Equals("Main_Bars"))
            {
                j++;
                if (tokens[i].Equals("FCPrime-" + j))
                {
                    i++;
                    parameters.rein_LSL_CB_fc_list.Add(tokens[i]);
                }
                i++;
            }
            j = 0; i++; parameters.rein_BEH_MB_dt.Rows.Clear();
            while (!tokens[i].Equals("Stirrups_and_Ties"))
            {
                j++;
                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j))
                {
                    i++;
                    while (!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("Stirrups_and_Ties"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }
                    parameters.rein_BEH_MB_dt.Rows.Add(tempList.ToArray());
                }
            }
            j = 0; i++; parameters.rein_BEH_ST_dt.Rows.Clear();
            while (!tokens[i].Equals("Weight"))
            {
                j++;
                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j))
                {
                    i++;
                    while (!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("Weight"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }
                    parameters.rein_BEH_ST_dt.Rows.Add(tempList.ToArray());
                }
            }
            i += 3;
            parameters.rein_W_dt.Rows[0][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[1][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[2][1] = tokens[i];i += 3;
            parameters.rein_W_dt.Rows[3][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[4][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[5][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[6][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[7][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[8][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[9][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[10][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[11][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[12][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[13][1] = tokens[i]; i++;

            parameters.rein_S_C_SL = tokens[i]; i++;
            parameters.rein_S_C_SZ = tokens[i]; i++;
            parameters.rein_S_C_AP = tokens[i]; i++;
            parameters.rein_S_C_MVDAB = tokens[i]; i++;

            parameters.rein_S_B_T_SL = tokens[i]; i++;
            parameters.rein_S_B_T_SZ = tokens[i]; i++;
            parameters.rein_S_B_T_AP = tokens[i]; i++;
            parameters.rein_S_B_B_SL = tokens[i]; i++;
            parameters.rein_S_B_B_SZ = tokens[i]; i++;
            parameters.rein_S_B_B_AP = tokens[i]; i++;
            parameters.rein_S_B_MHDAB = tokens[i]; i++;

            parameters.rein_S_S_T_SL = tokens[i]; i++;
            parameters.rein_S_S_B_SL = tokens[i]; i++;

            parameters.rein_RG_C = tokens[i]; i++;
            parameters.rein_RG_F = tokens[i]; i++;
            parameters.rein_RG_B = tokens[i]; i++;
            parameters.rein_RG_ST = tokens[i]; i++;
            parameters.rein_RG_W = tokens[i]; i++;
            parameters.rein_RG_SL = tokens[i]; i++;

            for (int col = 0; col < parameters.rein_mfIsSelected.GetLength(0); col++)
                for (int row = 0; row < parameters.rein_mfIsSelected.GetLength(1); row++)
                    { parameters.rein_mfIsSelected[col, row] = bool.Parse(tokens[i]); i++; }

            //Paint
            i++;
            parameters.paint_SCL = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Tiles"))
            {
                j++;
                if (tokens[i].Equals("Paint_Area-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1], tokens[i + 2] };
                    parameters.paint_Area.Add(toAdd);
                }
                i += 3;
            }

            //Tiles
            i++;
            parameters.tiles_FS = tokens[i]; i++;
            parameters.tiles_TG = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Masonry"))
            {
                j++;
                if (tokens[i].Equals("Tile_Area-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1], tokens[i + 2] };
                    parameters.tiles_Area.Add(toAdd);
                }
                i += 3; 
            }

            //Masonry
            i++;
            parameters.mason_CHB_EW = tokens[i]; i++;
            parameters.mason_CHB_IW = tokens[i]; i++;

            
            j = 0; //Exterior Wall
            while (!tokens[i].Equals("Exterior_Window-1") && !tokens[i].Equals("Exterior_Door-1") &&
                   !tokens[i].Equals("Interior_Wall-1") && !tokens[i].Equals("Interior_Window-1") && 
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Exterior_Wall-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_exteriorWall.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Exterior Window
            while (!tokens[i].Equals("Exterior_Door-1") &&
                   !tokens[i].Equals("Interior_Wall-1") && !tokens[i].Equals("Interior_Window-1") &&
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Exterior_Window-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_exteriorWindow.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Exterior Door
            while (!tokens[i].Equals("Interior_Wall-1") && !tokens[i].Equals("Interior_Window-1") &&
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Exterior_Door-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_exteriorDoor.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Interior Wall
            while (!tokens[i].Equals("Interior_Window-1") &&
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Interior_Wall-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_interiorWall.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Interior Window
            while (!tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Interior_Window-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_interiorWindow.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Interior Window
            while (!tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Interior_Door-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_interiorDoor.Add(toAdd);
                }
                i += 2;
            }
            i++; //END
            parameters.mason_RTW_VS = tokens[i]; i++;
            parameters.mason_RTW_HSL = tokens[i]; i++;
            parameters.mason_RTW_RG = tokens[i]; i++;
            parameters.mason_RTW_BD = tokens[i]; i++;
            parameters.mason_RTW_RL = tokens[i]; i++;
            parameters.mason_RTW_LTW = tokens[i]; i++;

            //Labor and Equipment
            i++;
            parameters.labor_RD = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Equipment-1") && !tokens[i].Equals("Misc"))
            {
                j++;
                if (tokens[i].Equals("Man_Power-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.labor_MP.Add(toAdd);
                }
                i += 2;
            }
            j = 0;
            while (!tokens[i].Equals("Misc"))
            {
                j++;
                if (tokens[i].Equals("Equipment-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.labor_EQP.Add(toAdd);
                }
                i += 2;
            }

            //Misc
            i++;
            j = 0;
            while (!tokens[i].Equals("Structural-Members"))
            {
                j++;
                if (tokens[i].Equals("Custom_Item-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.misc_CustomItems.Add(toAdd);
                }
                i += 2;
            }
            //Save to Parameters -- END

            //Save to Structural-Members -- START
            i++;

            //Footings
            i++;

            int floorIndex = 0;
            int l = 0;
            j = 0;

            i++; //Only Ground Floor
            while (!tokens[i].Equals("Columns"))
            {
                if (tokens[i].Equals("Column-Footing-" + (j + 1)) && !tokens[i].Equals("Columns"))
                {
                    List<string> toAdd = new List<string>();
                    i++; 

                    structuralMembers.footingColumnNames.Add(tokens[i]); i++;

                    while (!tokens[i].Equals("Column-Footing-" + (j + 2)) &&
                        !tokens[i].Equals("Wall-Footing-" + (l + 1)) && !tokens[i].Equals("Columns"))
                    {
                        toAdd.Add(tokens[i]); i++;
                    }
                    structuralMembers.footingsColumn[0].Add(toAdd);
                    //compute.AddFootingWorks(this, j, l, true);
                    j++;
                }
                else if (tokens[i].Equals("Wall-Footing-" + (l + 1)) && !tokens[i].Equals("Columns"))
                {
                    List<string> toAdd = new List<string>();
                    i++; 

                    structuralMembers.footingWallNames.Add(tokens[i]); i++;

                    while (!tokens[i].Equals("Wall-Footing-" + (l + 2)) && !tokens[i].Equals("Columns"))
                    {
                        toAdd.Add(tokens[i]); i++;
                    }
                    structuralMembers.footingsWall[0].Add(toAdd);
                    //compute.AddFootingWorks(this, j, l, false);
                    l++;
                }
            }

            //Columns 
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Beams"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Beams"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<string>> floor1 = new List<List<string>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Column-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        List<string> ltMember = new List<string>();
                        List<string> sMember = new List<string>();
                        while (!tokens[i].Equals("Lateral-Ties") && !tokens[i].Equals("Spacing") && 
                               !tokens[i].Equals("Column-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) && 
                               !tokens[i].Equals("Beams"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        if (tokens[i].Equals("Lateral-Ties"))
                        {
                            i++;
                            while (!tokens[i].Equals("Spacing") &&
                                   !tokens[i].Equals("Column-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                                   !tokens[i].Equals("Beams"))
                            {
                                ltMember.Add(tokens[i]); i++;
                            }
                        }
                        if (tokens[i].Equals("Spacing"))
                        {
                            i++;
                            while (!tokens[i].Equals("Column-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                                   !tokens[i].Equals("Beams"))  
                            {
                                sMember.Add(tokens[i]); i++;
                            }
                        }
                        floor.Add(member);
                        floor1.Add(ltMember);
                        floor2.Add(sMember);
                        l++;
                    }
                    structuralMembers.column.Add(floor);
                    structuralMembers.columnLateralTies.Add(floor1);
                    structuralMembers.columnSpacing.Add(floor2);
                    structuralMembers.columnNames.Add(name);
                    j++;
                }
            }
            
            //Beams 
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Slabs"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Slabs"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<List<string>>> floor1 = new List<List<List<string>>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Beam-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        List<List<string>> brMember = new List<List<string>>();
                        while (!tokens[i].Equals("Beam-Rows") && !tokens[i].Equals("Beam-Schedules") &&
                               !tokens[i].Equals("Beam-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Slabs"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        if (tokens[i].Equals("Beam-Rows"))
                        {
                            i++;
                            while (!tokens[i].Equals("Beam-Schedules") &&
                               !tokens[i].Equals("Beam-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Slabs"))
                            {
                                List<string> toAdd = new List<string>();
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                brMember.Add(toAdd);
                            }
                        }
                        l++;
                        floor.Add(member);
                        floor1.Add(brMember);
                    }
                    if (tokens[i].Equals("Beam-Schedules"))
                    {
                        i++;
                        while (!tokens[i].Equals("Beam-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Slabs"))
                        {
                            List<string> toAdd = new List<string>();
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            floor2.Add(toAdd);
                        }
                    }
                    structuralMembers.beam.Add(floor);
                    structuralMembers.beamRow.Add(floor1);
                    structuralMembers.beamSchedule.Add(floor2);
                    structuralMembers.beamNames.Add(name);

                    j++;
                }
            }

            //Slabs
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Stairs"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Stairs"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Slab-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        while (!tokens[i].Equals("Slab-Schedules") &&
                               !tokens[i].Equals("Slab-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Stairs"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(member);
                    }
                    
                    if (j > 0)
                    {
                        if (tokens[i].Equals("Slab-Schedules"))
                        {
                            i++;
                            while (!tokens[i].Equals("Slab-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                                   !tokens[i].Equals("Stairs"))
                            {
                                List<string> toAdd = new List<string>();
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                floor2.Add(toAdd);
                            }
                        }
                    }
                    
                    structuralMembers.slab.Add(floor);
                    if (j > 0)
                    {
                        structuralMembers.slabSchedule.Add(floor2);
                    }
                    structuralMembers.slabNames.Add(name);
                    j++;
                }
            }

            //Stairs
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Roof"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Roof"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while(tokens[i].Equals("Stair-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        while (!tokens[i].Equals("Stair-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("Roof"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(member);
                    }
                    structuralMembers.stairs.Add(floor);
                    structuralMembers.stairsNames.Add(name);
                    j++;
                }
            }

            //Roof
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("Solutions"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Solutions"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Roof-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        List<string> hrsMember = new List<string>();
                        while (!tokens[i].Equals("Roof-Sheet"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        i++;
                        while (!tokens[i].Equals("Roof-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("Solutions"))
                        {
                            hrsMember.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(member);
                        floor2.Add(hrsMember);
                    }
                    structuralMembers.roof.Add(floor);
                    structuralMembers.roofHRS.Add(floor2);
                    structuralMembers.roofNames.Add(name);
                    j++;
                }
            }
            //Structural Members -- END

            //Solutions -- START
            i++;
            //Earthworks
            i++;

            j = 0;
            i++; //Only Ground Floor
            while (!tokens[i].Equals("Extra-Earthworks"))
            {
                List<double> toAdd = new List<double>();
                i++;

                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                structuralMembers.earthworkSolutions.Add(toAdd);
                
                j++;
            }

            while (!tokens[i].Equals("Concrete-Works"))
            {
                i++;
                int k = 0;
                while (!tokens[i].Equals("Concrete-Works"))
                {
                    structuralMembers.extraEarthworkSolutions[k] = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++; k++;
                }
            }

            //Concrete Works
            i++;

            //Concrete Footing
            i++;
            j = 0;
            while (!tokens[i].Equals("ConcreteC")) 
            {
                if (tokens[i].Equals("concreteF-" + (j + 1)) && !tokens[i].Equals("ConcreteC"))
                {
                    List<double> toAdd = new List<double>();
                    i++;

                    while (!tokens[i].Equals("concreteF-" + (j + 2)) && !tokens[i].Equals("ConcreteC"))
                    {
                        toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                    }

                    structuralMembers.concreteWorkSolutionsF.Add(toAdd);
                    j++;
                }
            }

            //Concrete Column
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("ConcreteBR"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("ConcreteBR"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    if (tokens[i].Equals("concreteC-" + (l + 1)) && !tokens[i].Equals("ConcreteBR"))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteC-" + (l + 2)) && !tokens[i].Equals("ConcreteBR"))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsC.Add(floor);
                    j++;
                }
            }

            //Concrete Beam
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("ConcreteSL"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("ConcreteSL"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    if (tokens[i].Equals("concreteBR-" + (l + 1)) && !tokens[i].Equals("ConcreteSL"))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteBR-" + (l + 2)) && !tokens[i].Equals("ConcreteSL"))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsBR.Add(floor);
                    j++;
                }
            }

            //Concrete Slab
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("ConcreteST"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("ConcreteST"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    if (tokens[i].Equals("concreteSL-" + (l + 1)) && !tokens[i].Equals("ConcreteST"))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteSL-" + (l + 2)) && !tokens[i].Equals("ConcreteST"))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsSL.Add(floor);
                    j++;
                }
            }

            //Concrete Stairs
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("ConcreteFS"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("ConcreteFS"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    if (tokens[i].Equals("concreteST-" + (l + 1)) && !tokens[i].Equals("ConcreteFS"))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteST-" + (l + 2)) && !tokens[i].Equals("ConcreteFS"))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsST.Add(floor);
                    j++;
                }
            }

            //ConcreteFS TODO
            i++;

            //Solutions -- END

            //Totalities -- START
            i++;

            //Earthworks
            i++;
            excavation_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            backfillingAndCompaction_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gradingAndCompaction_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gravelBedding_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            soilPoisoning_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //Totalities -- END

            viewInitalized = false;
            initializeView();
            //*/
            MessageBox.Show(tokens[i]);
            
            pf = new ParametersForm(parameters, this);
        }
        //Long Functions -- END

        //Extra Functions -- START
        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        private void CostEstimationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit this program?", "Exit Program", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //Do nothing
            }
            else if (dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }   
        }
        //Extra Functions -- END
    }
}
