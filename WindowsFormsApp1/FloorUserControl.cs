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
    //TODO: FILE HANDLING
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
        private List<TreeNode> nodes;


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

        public void setValues(string floorCount, string floorLabel, List<TreeNode> nodes)
        {
            floorDupeCountNUD.Value = int.Parse(floorCount);
            floorLbl.Text = floorLabel;
            setTree(nodes);
        }

        private void setTree(List<TreeNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                floorTreeView.Nodes.Add(nodes[i]);
            }
            AdjustTreeViewHeight(floorTreeView);
        }

        public Floor(CostEstimationForm costEstimationForm)
        {
            InitializeComponent();

            //Parent nodes
            TreeNode tn1 = new TreeNode("FOOTINGS");
            tn1.Name = "footingParent";
            TreeNode tn2 = new TreeNode("BEAMS");
            tn2.Name = "beamParent";
            TreeNode tn3 = new TreeNode("COLUMNS");
            tn3.Name = "columnParent";
            TreeNode tn4 = new TreeNode("SLABS");
            tn4.Name = "slabsParent";
            TreeNode tn5 = new TreeNode("STAIRS");
            tn5.Name = "stairsParent";
            TreeNode tn6 = new TreeNode("ROOF");
            tn6.Name = "roofParent";

            this.costEstimationForm = costEstimationForm;
            floorCount = costEstimationForm.Floors.Count;

            if (costEstimationForm.saveFileExists)
            {
                //Example - start
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

                nodes = new List<TreeNode>() { tn1, tn2, tn3, tn4, tn5, tn6 };

                Console.WriteLine(string.Join(", ", nodes));

                setValues("1", "Groundfloor", nodes);
                string print = getValues()[1];
                Console.WriteLine(print);
            }
            else
            {
                nodes = new List<TreeNode>() { tn1, tn2, tn3, tn4, tn5, tn6 };
                setTree(nodes);
                if (floorCount == 0)
                {
                    floorUCDeleteBtn.Enabled = false;
                    floorLbl.Text = "GROUND FLOOR";
                }
                else
                {
                    floorLbl.Text = AddOrdinal(floorCount) + " FLOOR";
                }
            }
        }

        private void addStrMemBtn_Click(object sender, EventArgs e)
        {
            //Add node to tree, parent specific - template for later use
            TreeNode[] found = floorTreeView.Nodes.Find("footingParent", true);
            TreeNode newChild = new TreeNode("CF-1");
            newChild.Name = "newChild";

            found[0].Nodes.Add(newChild);
            AdjustTreeViewHeight(floorTreeView);            
        }

        void AdjustTreeViewHeight(TreeView treeView)
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
            string[] parents = { "FOOTINGS", "BEAMS", "COLUMNS", "SLABS", "STAIRS", "ROOF" };
            TreeViewHitTestInfo info = floorTreeView.HitTest(floorTreeView.PointToClient(Cursor.Position));
            try
            {
                if(floorTreeView.SelectedNode != null)
                {
                    if (Array.IndexOf(parents, info.Node.Text) < 0)
                    {
                        MessageBox.Show(info.Node.Text);
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
