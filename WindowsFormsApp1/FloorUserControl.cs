using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Floor : UserControl
    {
        //Forms
        CostEstimationForm costEstimationForm;

        //Local Variables
        const int TVM_GETNEXTITEM = 0x1100 + 10;
        const int TVGN_LASTVISIBLE = 0x000A;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        extern static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        private int floorCount;
        public List<TreeNode> nodes;
        public int footingCount, wallFootingCount, columnCount, beamCount, slabCount, stairsCount, roofCount;
        
        public string setLabel
        {
            set
            {
                floorLbl.Text = value;
            }
        }

        public string[] getValues()
        {
            int floorDupes = (int)floorDupeCountNUD.Value;
            string[] values = { floorDupes.ToString(), floorLbl.Text };
            return values;
        }
        
        public string setFloorLabel
        {
            set
            {
                floorLbl.Text = value;
            }
        }

        public TreeView treeView
        {
            get
            {
                return floorTreeView;
            }
        }

        public void setValues(int floorCount, string floorLabel)
        {
            floorDupeCountNUD.Value = floorCount;
            floorLbl.Text = floorLabel;
        }

        private void setTree(List<TreeNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                floorTreeView.Nodes.Add(nodes[i]);
            }
            AdjustTreeViewHeight(floorTreeView);
        }

        public void setCounts()
        {
            if (floorCount == 0)
            {
                foreach (TreeNode node in nodes[0].Nodes)
                {
                    if (node.Name[0] == 'F')
                        footingCount++;
                    else
                        wallFootingCount++;
                }
                foreach (TreeNode node in nodes[1].Nodes)
                {
                    columnCount++;
                }
                foreach (TreeNode node in nodes[2].Nodes)
                {
                    beamCount++;
                }
                foreach (TreeNode node in nodes[3].Nodes)
                {
                    slabCount++;
                }
                foreach (TreeNode node in nodes[4].Nodes)
                {
                    stairsCount++;
                }
                foreach (TreeNode node in nodes[5].Nodes)
                {
                    roofCount++;
                }
            }
            else
            {
                foreach (TreeNode node in nodes[0].Nodes)
                {
                    columnCount++;
                }
                foreach (TreeNode node in nodes[1].Nodes)
                {
                    beamCount++;
                }
                foreach (TreeNode node in nodes[2].Nodes)
                {
                    slabCount++;
                }
                foreach (TreeNode node in nodes[3].Nodes)
                {
                    stairsCount++;
                }
                foreach (TreeNode node in nodes[4].Nodes)
                {
                    roofCount++;
                }
            }
        }

        public Floor(CostEstimationForm costEstimationForm, bool fromFile)
        {
            InitializeComponent();

            //Parent nodes
            TreeNode tn1 = new TreeNode("FOOTINGS");
            tn1.Name = "footingParent";
            TreeNode tn2 = new TreeNode("COLUMNS");
            tn2.Name = "columnParent";
            TreeNode tn3 = new TreeNode("BEAMS");
            tn3.Name = "beamParent";
            TreeNode tn4 = new TreeNode("SLABS");
            tn4.Name = "slabParent";
            TreeNode tn5 = new TreeNode("STAIRS");
            tn5.Name = "stairsParent";
            TreeNode tn6 = new TreeNode("ROOFINGS");
            tn6.Name = "roofParent";

            //Init variables
            this.costEstimationForm = costEstimationForm;
            floorCount = costEstimationForm.Floors.Count;
            footingCount = wallFootingCount = columnCount = beamCount = slabCount = stairsCount = roofCount = 0;

            //SaveFile?
            if (fromFile)
            {

            }
            else //Add Parent nodes and set tree
            {
                if (floorCount == 0)
                {
                    floorDupeCountNUD.Value = 1;
                    floorDupeCountNUD.Enabled = false;
                    floorUCDeleteBtn.Enabled = false;
                    floorLbl.Text = "GROUND FLOOR";
                    nodes = new List<TreeNode>() { tn1, tn2, tn3, tn4, tn5, tn6 };
                    setTree(nodes);
                }
                else
                {
                    floorLbl.Text = AddOrdinal(floorCount) + " FLOOR";
                    nodes = new List<TreeNode>() { tn2, tn3, tn4, tn5, tn6 };
                    setTree(nodes);
                }
            }
        }

        private void addStrMemBtn_Click(object sender, EventArgs e)
        {
            AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, footingCount, wallFootingCount, columnCount, beamCount, slabCount, stairsCount, roofCount, nodes, true, -1, "NEW", false);
            if (asForm.ShowDialog() == DialogResult.OK)
            {
                //TODO add other structural members
                if (asForm.structuralMemberType.Equals("Footing (Column)"))
                {
                    footingCount++;
                    TreeNode[] found = floorTreeView.Nodes.Find("footingParent", true);
                    TreeNode newChild = new TreeNode(asForm.structMemName); 
                    newChild.Name = "F-" + (footingCount);

                    found[0].Nodes.Add(newChild);
                    AdjustTreeViewHeight(floorTreeView);
                }
                else if (asForm.structuralMemberType.Equals("Footing (Wall)"))
                {
                    wallFootingCount++;
                    TreeNode[] found = floorTreeView.Nodes.Find("footingParent", true);
                    TreeNode newChild = new TreeNode(asForm.structMemName);    
                    newChild.Name = "WF-" + (wallFootingCount);

                    found[0].Nodes.Add(newChild);
                    AdjustTreeViewHeight(floorTreeView);
                }
                else if (asForm.structuralMemberType.Equals("Column"))
                {
                    columnCount++;
                    TreeNode[] found = floorTreeView.Nodes.Find("columnParent", true);
                    TreeNode newChild = new TreeNode(asForm.structMemName);
                    newChild.Name = "C-" + (columnCount);

                    found[0].Nodes.Add(newChild);
                    AdjustTreeViewHeight(floorTreeView);
                }
                else if (asForm.structuralMemberType.Equals("Beam"))
                {
                    beamCount++;
                    TreeNode[] found = floorTreeView.Nodes.Find("beamParent", true);
                    TreeNode newChild = new TreeNode(asForm.structMemName);
                    newChild.Name = "BR-" + (beamCount);

                    found[0].Nodes.Add(newChild);
                    AdjustTreeViewHeight(floorTreeView);
                }
                else if (asForm.structuralMemberType.Equals("Slab"))
                {
                    slabCount++;
                    TreeNode[] found = floorTreeView.Nodes.Find("slabParent", true);
                    TreeNode newChild = new TreeNode(asForm.structMemName);
                    newChild.Name = "S-" + (slabCount);

                    found[0].Nodes.Add(newChild);
                    AdjustTreeViewHeight(floorTreeView);
                }
                else if (asForm.structuralMemberType.Equals("Stairs"))
                {
                    stairsCount++;
                    TreeNode[] found = floorTreeView.Nodes.Find("stairsParent", true);
                    TreeNode newChild = new TreeNode(asForm.structMemName);
                    newChild.Name = "ST-" + (stairsCount);

                    found[0].Nodes.Add(newChild);
                    AdjustTreeViewHeight(floorTreeView);
                }
                else if (asForm.structuralMemberType.Equals("Roofing (Gable)"))
                {
                    roofCount++;
                    TreeNode[] found = floorTreeView.Nodes.Find("roofParent", true);
                    TreeNode newChild = new TreeNode(asForm.structMemName);
                    newChild.Name = "R-" + (roofCount);

                    found[0].Nodes.Add(newChild);
                    AdjustTreeViewHeight(floorTreeView);
                }
            }
        }

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

        private void floorTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            AdjustTreeViewHeight(floorTreeView);
        }

        private void floorTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            AdjustTreeViewHeight(floorTreeView);
        }

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

        private void floorUCDeleteBtn_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            costEstimationForm.Floors.Remove(this);

            //Remove Lists when this Floor is deleted
            if (floorCount == 0)//Ground Floor
            {
                //Footings
                costEstimationForm.structuralMembers.footingsColumn.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.footingsWall.RemoveAt(floorCount);

                //Columns
                costEstimationForm.structuralMembers.columnNames.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.column.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.columnLateralTies.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.columnSpacing.RemoveAt(floorCount);

                //Beams
                costEstimationForm.structuralMembers.beam.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.beamRow.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.beamSchedule.RemoveAt(floorCount);

                //Slabs
                costEstimationForm.structuralMembers.slabNames.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.slab.RemoveAt(floorCount);

                //Stairs
                costEstimationForm.structuralMembers.stairs.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.stairsNames.RemoveAt(floorCount);

                //Roof
                List<string> newList18 = new List<string>();
                costEstimationForm.structuralMembers.roof.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.roofHRS.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.roofNames.RemoveAt(floorCount);

                //Solution Variables
                costEstimationForm.structuralMembers.concreteWorkSolutionsC.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.concreteWorkSolutionsBR.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.concreteWorkSolutionsSL.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.concreteWorkSolutionsST.RemoveAt(floorCount);
            }
            else //Upper Floors
            {
                //Columns
                costEstimationForm.structuralMembers.columnNames.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.column.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.columnLateralTies.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.columnSpacing.RemoveAt(floorCount);

                //Beams
                costEstimationForm.structuralMembers.beam.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.beamRow.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.beamSchedule.RemoveAt(floorCount);

                //Slabs
                costEstimationForm.structuralMembers.slabNames.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.slab.RemoveAt(floorCount);

                //Stairs
                costEstimationForm.structuralMembers.stairs.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.stairsNames.RemoveAt(floorCount);

                //Roof
                List<string> newList18 = new List<string>();
                costEstimationForm.structuralMembers.roof.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.roofHRS.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.roofNames.RemoveAt(floorCount);

                //Solution Variables
                costEstimationForm.structuralMembers.concreteWorkSolutionsC.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.concreteWorkSolutionsBR.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.concreteWorkSolutionsSL.RemoveAt(floorCount);
                costEstimationForm.structuralMembers.concreteWorkSolutionsST.RemoveAt(floorCount);
            }
            costEstimationForm.refreshFloors();
        }

        private void floorLbl_DoubleClick(object sender, EventArgs e)
        {
            floorLbl.ReadOnly = false;
        }

        private void floorLbl_Leave(object sender, EventArgs e)
        {
            floorLbl.ReadOnly = true;
        }

        private void floorLbl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                floorLbl.ReadOnly = true;
            }
        }

        private void floorTreeView_DoubleClick(object sender, EventArgs e)
        {
            string[] parents = { "FOOTINGS", "COLUMNS", "BEAMS", "SLABS", "STAIRS", "ROOFINGS" };
            TreeViewHitTestInfo info = floorTreeView.HitTest(floorTreeView.PointToClient(Cursor.Position));
            try
            {
                if(floorTreeView.SelectedNode != null)
                {
                    if (Array.IndexOf(parents, info.Node.Text) < 0)
                    {
                        if(floorTreeView.SelectedNode.Parent.Text.Equals("FOOTINGS"))
                        {
                            int footingCount = 0;
                            int wallFootingCount = 0;

                            List<string> footingAndWallFootingNames = new List<string>();
                            footingAndWallFootingNames.AddRange(costEstimationForm.structuralMembers.footingColumnNames);
                            footingAndWallFootingNames.AddRange(costEstimationForm.structuralMembers.footingWallNames);

                            foreach (TreeNode member in nodes[0].Nodes)
                            {
                                TreeNode[] found = floorTreeView.Nodes.Find(member.Name, true);

                                if (found[0].Name[0] == 'F') //Footing (Column)
                                {
                                    if (member.Text.Equals(info.Node.Text))
                                    {
                                        AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, this.footingCount, this.wallFootingCount, this.columnCount, this.beamCount, this.slabCount, this.stairsCount, this.roofCount, nodes, false, footingCount, "FOOTINGS", true);
                                        if (asForm.ShowDialog() == DialogResult.OK)
                                        {
                                            if (asForm.structuralMemberType.Equals("Footing (Column)"))
                                            {
                                                TreeNode[] found2 = floorTreeView.Nodes.Find("footingParent", true);
                                                int i = found2[0].Nodes.IndexOf(info.Node);

                                                TreeNode newChild = new TreeNode(asForm.structMemName);   
                                                newChild.Name = (info.Node.Name);

                                                found2[0].Nodes.RemoveAt(i);
                                                found2[0].Nodes.Insert(i, newChild);
                                            }
                                        }
                                        return;
                                    }
                                    footingCount++;
                                }
                                else //Footing (Wall)
                                {
                                    if (member.Text.Equals(info.Node.Text))
                                    {
                                        AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, this.footingCount, this.wallFootingCount, this.columnCount, this.beamCount, this.slabCount, this.stairsCount, this.roofCount, nodes, false, wallFootingCount, "FOOTINGS", false);
                                        if (asForm.ShowDialog() == DialogResult.OK)
                                        {
                                            if (asForm.structuralMemberType.Equals("Footing (Wall)"))
                                            {
                                                TreeNode[] found2 = floorTreeView.Nodes.Find("footingParent", true);
                                                int i = found2[0].Nodes.IndexOf(info.Node);

                                                TreeNode newChild = new TreeNode(asForm.structMemName);    
                                                newChild.Name = (info.Node.Name);

                                                found2[0].Nodes.RemoveAt(i);
                                                found2[0].Nodes.Insert(i, newChild);
                                            }
                                        }
                                        return;
                                    }
                                    wallFootingCount++;
                                }
                            }
                        }
                        else if (floorTreeView.SelectedNode.Parent.Text.Equals("COLUMNS"))
                        {
                            int columnCount = 0, parentNodeIndex;
                            if (floorCount == 0)
                            {
                                parentNodeIndex = 1;
                            }
                            else
                            {
                                parentNodeIndex = 0;
                            }
                            foreach (TreeNode member in nodes[parentNodeIndex].Nodes)
                            {
                                TreeNode[] found = floorTreeView.Nodes.Find(member.Name, true);

                                if (member.Text.Equals(info.Node.Text))
                                {
                                    AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, this.footingCount, this.wallFootingCount, this.columnCount, this.beamCount, this.slabCount, this.stairsCount, this.roofCount, nodes, false, columnCount, "COLUMNS", false);
                                    if (asForm.ShowDialog() == DialogResult.OK)
                                    {
                                        if (asForm.structuralMemberType.Equals("Column"))
                                        {
                                            TreeNode[] found2 = floorTreeView.Nodes.Find("columnParent", true);
                                            int i = found2[0].Nodes.IndexOf(info.Node);

                                            TreeNode newChild = new TreeNode(asForm.structMemName);
                                            newChild.Name = (info.Node.Name);

                                            found2[0].Nodes.RemoveAt(i);
                                            found2[0].Nodes.Insert(i, newChild);
                                        }
                                    }
                                    return;
                                }
                                columnCount++;
                            }
                        }
                        else if (floorTreeView.SelectedNode.Parent.Text.Equals("BEAMS"))
                        {
                            int beamsCount = 0, parentNodeIndex;
                            if (floorCount == 0)
                            {
                                parentNodeIndex = 2;
                            }
                            else
                            {
                                parentNodeIndex = 1;
                            }
                            foreach (TreeNode member in nodes[parentNodeIndex].Nodes)
                            {
                                TreeNode[] found = floorTreeView.Nodes.Find(member.Name, true);

                                if (member.Text.Equals(info.Node.Text))
                                {
                                    AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, this.footingCount, this.wallFootingCount, this.columnCount, this.beamCount, this.slabCount, this.stairsCount, this.roofCount, nodes, false, beamsCount, "BEAMS", false);
                                    if (asForm.ShowDialog() == DialogResult.OK)
                                    {
                                        if (asForm.structuralMemberType.Equals("Beam"))
                                        {
                                            TreeNode[] found2 = floorTreeView.Nodes.Find("beamParent", true);
                                            int i = found2[0].Nodes.IndexOf(info.Node);

                                            TreeNode newChild = new TreeNode(asForm.structMemName);
                                            newChild.Name = (info.Node.Name);

                                            found2[0].Nodes.RemoveAt(i);
                                            found2[0].Nodes.Insert(i, newChild);
                                        }
                                    }
                                    return;
                                }
                                beamsCount++;
                            }
                        }
                        else if (floorTreeView.SelectedNode.Parent.Text.Equals("SLABS"))
                        {
                            int slabCount = 0, parentNodeIndex;
                            if (floorCount == 0)
                            {
                                parentNodeIndex = 3;
                            }
                            else
                            {
                                parentNodeIndex = 2;
                            }
                            foreach (TreeNode member in nodes[parentNodeIndex].Nodes)
                            {
                                TreeNode[] found = floorTreeView.Nodes.Find(member.Name, true);

                                if (member.Text.Equals(info.Node.Text))
                                {
                                    AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, this.footingCount, this.wallFootingCount, this.columnCount, this.beamCount, this.slabCount, this.stairsCount, this.roofCount, nodes, false, slabCount, "SLABS", false);
                                    if (asForm.ShowDialog() == DialogResult.OK)
                                    {
                                        if (asForm.structuralMemberType.Equals("Slab"))
                                        {
                                            TreeNode[] found2 = floorTreeView.Nodes.Find("slabParent", true);
                                            int i = found2[0].Nodes.IndexOf(info.Node);

                                            TreeNode newChild = new TreeNode(asForm.structMemName);
                                            newChild.Name = (info.Node.Name);

                                            found2[0].Nodes.RemoveAt(i);
                                            found2[0].Nodes.Insert(i, newChild);
                                        }
                                    }
                                    return;
                                }
                                slabCount++;
                            }
                        }
                        else if (floorTreeView.SelectedNode.Parent.Text.Equals("STAIRS"))
                        {
                            int stairsCount = 0, parentNodeIndex;
                            if(floorCount == 0)
                            {
                                parentNodeIndex = 4;
                            }
                            else
                            {
                                parentNodeIndex = 3;
                            }
                            foreach (TreeNode member in nodes[parentNodeIndex].Nodes)
                            {
                                TreeNode[] found = floorTreeView.Nodes.Find(member.Name, true);

                                if (member.Text.Equals(info.Node.Text))
                                {
                                    AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, this.footingCount, this.wallFootingCount, this.columnCount, this.beamCount, this.slabCount, this.stairsCount, this.roofCount, nodes, false, stairsCount, "STAIRS", false);
                                    if (asForm.ShowDialog() == DialogResult.OK)
                                    {
                                        if (asForm.structuralMemberType.Equals("Stairs"))
                                        {
                                            TreeNode[] found2 = floorTreeView.Nodes.Find("stairsParent", true);
                                            int i = found2[0].Nodes.IndexOf(info.Node);

                                            TreeNode newChild = new TreeNode(asForm.structMemName);   
                                            newChild.Name = (info.Node.Name);

                                            found2[0].Nodes.RemoveAt(i);
                                            found2[0].Nodes.Insert(i, newChild);
                                        }
                                    }
                                    return;
                                }
                                stairsCount++;
                            }
                        }
                        else if (floorTreeView.SelectedNode.Parent.Text.Equals("ROOFINGS"))
                        {
                            int roofCount = 0, parentNodeIndex;
                            if (floorCount == 0)
                            {
                                parentNodeIndex = 5;
                            }
                            else
                            {
                                parentNodeIndex = 4;
                            }
                            foreach (TreeNode member in nodes[parentNodeIndex].Nodes)
                            {
                                TreeNode[] found = floorTreeView.Nodes.Find(member.Name, true);

                                if (member.Text.Equals(info.Node.Text))
                                {
                                    AddStructForm asForm = new AddStructForm(costEstimationForm, floorCount, this.footingCount, this.wallFootingCount, this.columnCount, this.beamCount, this.slabCount, this.stairsCount, this.roofCount, nodes, false, roofCount, "ROOF", false);
                                    if (asForm.ShowDialog() == DialogResult.OK)
                                    {
                                        if (asForm.structuralMemberType.Equals("Roofing (Gable)"))
                                        {
                                            TreeNode[] found2 = floorTreeView.Nodes.Find("roofParent", true);
                                            int i = found2[0].Nodes.IndexOf(info.Node);

                                            TreeNode newChild = new TreeNode(asForm.structMemName);
                                            newChild.Name = (info.Node.Name);

                                            found2[0].Nodes.RemoveAt(i);
                                            found2[0].Nodes.Insert(i, newChild);
                                        }
                                    }
                                    return;
                                }
                                roofCount++;
                            }
                        }
                    }
                }
            } 
            catch (NullReferenceException ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }
    }
}

//How to add child
/*Example - start
TreeNode newChild = new TreeNode("F-1");
newChild.Name = "newChild1";
TreeNode newChild2 = new TreeNode("B-1");
newChild2.Name = "newChild2";
TreeNode newChild3 = new TreeNode("C-1");
newChild3.Name = "newChild3";
TreeNode newChild4 = new TreeNode("SL-1");
newChild4.Name = "newChild4";
TreeNode newChild5 = new TreeNode("ST-1");
newChild5.Name = "newChild5";
TreeNode newChild6 = new TreeNode("R-1");
newChild6.Name = "newChild6";

TreeNode[] nodesF = { newChild };
TreeNode[] nodesB = { newChild2 };
TreeNode[] nodesC = { newChild3 };
TreeNode[] nodesSL = { newChild4 };
TreeNode[] nodesST = { newChild5 };
TreeNode[] nodesR = { newChild6 };

tn1.Nodes.AddRange(nodesF);
tn2.Nodes.AddRange(nodesB);
tn3.Nodes.AddRange(nodesC);
tn4.Nodes.AddRange(nodesSL);
tn5.Nodes.AddRange(nodesST);
tn6.Nodes.AddRange(nodesR);
//Example - end
*/
