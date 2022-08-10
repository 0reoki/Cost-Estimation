using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowEst
{
    public partial class StairParameterUserControl : UserControl
    {
        public string type;

        public StairParameterUserControl(string stairType)
        {
            InitializeComponent();
            type = stairType;
            if (type.Equals("Straight Stairs"))
            {
                stairsTabControl.SelectedIndex = 0;

                straightStair_RG_cbx.SelectedIndex = 1;
                straightStairs_MB1_ML_cbx.SelectedIndex = 0;
                straightStairs_MB2_ML_cbx.SelectedIndex = 0;
                straightStairs_MB3_ML_cbx.SelectedIndex = 0;
                straightStairs_DB_ML_cbx.SelectedIndex = 0;
                straightStairs_S_MB_ML_cbx.SelectedIndex = 0;
                straightStairs_S_NB_ML_cbx.SelectedIndex = 0;
            }
            else if (type.Equals("U-Stairs"))
            {
                stairsTabControl.SelectedIndex = 1;

                UStair_RG_cbx.SelectedIndex = 1;
                UStairs_WSF1_MB1_ML_cbx.SelectedIndex = 0;
                UStairs_WSF1_MB2_ML_cbx.SelectedIndex = 0;
                UStairs_WSF1_MB3_ML_cbx.SelectedIndex = 0;
                UStairs_WSF2_MB1_ML_cbx.SelectedIndex = 0;
                UStairs_WSF2_MB2_ML_cbx.SelectedIndex = 0;
                UStairs_WSF2_MB3_ML_cbx.SelectedIndex = 0;
                UStairs_DB_ML_cbx.SelectedIndex = 0;
                UStairs_LR_ML_cbx.SelectedIndex = 0;
                UStairs_S_MB_ML_cbx.SelectedIndex = 0;
                UStairs_S_NB_ML_cbx.SelectedIndex = 0;
            }
            else //L-Stairs
            {
                stairsTabControl.SelectedIndex = 2;

                LStair_RG_cbx.SelectedIndex = 1;
                LStair_WSF1_MB1_ML_cbx.SelectedIndex = 0;
                LStair_WSF1_MB2_ML_cbx.SelectedIndex = 0;
                LStair_WSF1_MB3_ML_cbx.SelectedIndex = 0;
                LStair_WSF2_MB1_ML_cbx.SelectedIndex = 0;
                LStair_WSF2_MB2_ML_cbx.SelectedIndex = 0;
                LStair_WSF2_MB3_ML_cbx.SelectedIndex = 0;
                LStair_DB_ML_cbx.SelectedIndex = 0;
                LStair_LR_ML_cbx.SelectedIndex = 0;
                LStair_S_MB_ML_cbx.SelectedIndex = 0;
                LStair_S_NB_ML_cbx.SelectedIndex = 0;
            }
            stairsTabControl.Appearance = TabAppearance.FlatButtons;
            stairsTabControl.ItemSize = new Size(0, 1);
            stairsTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in stairsTabControl.TabPages)
            {
                tab.Text = "";
            }
        }

        public void setStraightCB(int value)
        {
            straightStair_RG_cbx.SelectedIndex = value;
        }
        public void setUCB(int value)
        {
            UStair_RG_cbx.SelectedIndex = value;
        }
        public void setLCB(int value)
        {
            LStair_RG_cbx.SelectedIndex = value;
        }

        public string[] getValues()
        {
            if (type.Equals("Straight Stairs"))
            {
                string[] values = { 
                                    type, 
                                    straightStair_LL_bx.Text, 
                                    straightStair_HL_bx.Text,
                                    straightStair_SBW_bx.Text,
                                    straightStair_RG_cbx.Text,

                                    straightStairs_MB1_ML_cbx.Text,
                                    straightStairs_MB2_ML_cbx.Text,
                                    straightStairs_MB3_ML_cbx.Text,
                                    straightStairs_DB_ML_cbx.Text,
                                    straightStairs_S_MB_ML_cbx.Text,
                                    straightStairs_S_NB_ML_cbx.Text,
                                  };
                return values;
            }
            else if (type.Equals("U-Stairs"))
            {
                string[] values = {
                                    type,
                                    UStair_LL_bx.Text,
                                    UStair_HL_bx.Text,
                                    UStair_LSBW_bx.Text,
                                    UStair_RG_cbx.Text,

                                    UStairs_WSF1_MB1_ML_cbx.Text,
                                    UStairs_WSF1_MB2_ML_cbx.Text,
                                    UStairs_WSF1_MB3_ML_cbx.Text,
                                    UStairs_WSF2_MB1_ML_cbx.Text,
                                    UStairs_WSF2_MB2_ML_cbx.Text,
                                    UStairs_WSF2_MB3_ML_cbx.Text,
                                    UStairs_DB_ML_cbx.Text,
                                    UStairs_LR_ML_cbx.Text,
                                    UStairs_S_MB_ML_cbx.Text,
                                    UStairs_S_NB_ML_cbx.Text,
                                  };
                return values;
            }
            else //L-Stairs
            {
                string[] values = {
                                    type,
                                    LStair_LL_bx.Text,
                                    LStair_HL_bx.Text,
                                    LStair_SBW_bx.Text,
                                    LStair_RG_cbx.Text,

                                    LStair_WSF1_MB1_ML_cbx.Text,
                                    LStair_WSF1_MB2_ML_cbx.Text,
                                    LStair_WSF1_MB3_ML_cbx.Text,
                                    LStair_WSF2_MB1_ML_cbx.Text,
                                    LStair_WSF2_MB2_ML_cbx.Text,
                                    LStair_WSF2_MB3_ML_cbx.Text,
                                    LStair_DB_ML_cbx.Text,
                                    LStair_LR_ML_cbx.Text,
                                    LStair_S_MB_ML_cbx.Text,
                                    LStair_S_NB_ML_cbx.Text,
                                  };
                return values;
            }
        }

        public void setStraightStairsValues(string LL, string HL, string SBW, string RG, string MB1_ML,
                                            string MB2_ML, string MB3_ML, string DB_ML, string S_MB_ML,
                                            string S_NB_ML)
        {
            straightStair_LL_bx.Text = LL; 
            straightStair_HL_bx.Text = HL;
            straightStair_SBW_bx.Text = SBW;
            straightStair_RG_cbx.Text = RG;

            straightStairs_MB1_ML_cbx.Text = MB1_ML;
            straightStairs_MB2_ML_cbx.Text = MB2_ML;
            straightStairs_MB3_ML_cbx.Text = MB3_ML;
            straightStairs_DB_ML_cbx.Text = DB_ML;
            straightStairs_S_MB_ML_cbx.Text = S_MB_ML;
            straightStairs_S_NB_ML_cbx.Text = S_NB_ML;
        }

        public void setUStairsValues(string LL, string HL, string SBW, string RG, string WSF1_MB1_ML,
                                            string WSF1_MB2_ML, string WSF1_MB3_ML, string WSF2_MB1_ML,
                                            string WSF2_MB2_ML, string WSF2_MB3_ML, string DB_ML, string LR_ML, 
                                            string S_MB_ML, string S_NB_ML)
        {
            UStair_LL_bx.Text = LL;
            UStair_HL_bx.Text = HL;
            UStair_LSBW_bx.Text = SBW;
            UStair_RG_cbx.Text = RG;

            UStairs_WSF1_MB1_ML_cbx.Text = WSF1_MB1_ML;
            UStairs_WSF1_MB2_ML_cbx.Text = WSF1_MB2_ML;
            UStairs_WSF1_MB3_ML_cbx.Text = WSF1_MB3_ML;
            UStairs_WSF2_MB1_ML_cbx.Text = WSF2_MB1_ML;
            UStairs_WSF2_MB2_ML_cbx.Text = WSF2_MB2_ML;
            UStairs_WSF2_MB3_ML_cbx.Text = WSF2_MB3_ML;
            UStairs_DB_ML_cbx.Text = DB_ML;
            UStairs_LR_ML_cbx.Text = LR_ML;
            UStairs_S_MB_ML_cbx.Text = S_MB_ML;
            UStairs_S_NB_ML_cbx.Text = S_NB_ML;
        }

        public void setLStairsValues(string LL, string HL, string SBW, string RG, string WSF1_MB1_ML,
                                            string WSF1_MB2_ML, string WSF1_MB3_ML, string WSF2_MB1_ML,
                                            string WSF2_MB2_ML, string WSF2_MB3_ML, string DB_ML, string LR_ML,
                                            string S_MB_ML, string S_NB_ML)
        {
            LStair_LL_bx.Text = LL;
            LStair_HL_bx.Text = HL;
            LStair_SBW_bx.Text = SBW;
            LStair_RG_cbx.Text = RG;

            LStair_WSF1_MB1_ML_cbx.Text = WSF1_MB1_ML;
            LStair_WSF1_MB2_ML_cbx.Text = WSF1_MB2_ML;
            LStair_WSF1_MB3_ML_cbx.Text = WSF1_MB3_ML;
            LStair_WSF2_MB1_ML_cbx.Text = WSF2_MB1_ML;
            LStair_WSF2_MB2_ML_cbx.Text = WSF2_MB2_ML;
            LStair_WSF2_MB3_ML_cbx.Text = WSF2_MB3_ML;
            LStair_DB_ML_cbx.Text = DB_ML;
            LStair_LR_ML_cbx.Text = LR_ML;
            LStair_S_MB_ML_cbx.Text = S_MB_ML;
            LStair_S_NB_ML_cbx.Text = S_NB_ML;
        }

        private void straightStairs_MB1_ML_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LStair_WSF1_MB1_ML_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Straight Stairs ML
        private void straightStairs_MB1_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", straightStairs_MB1_ML_cbx);
        }

        private void straightStairs_MB2_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", straightStairs_MB2_ML_cbx);
        }

        private void straightStairs_MB3_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", straightStairs_MB3_ML_cbx);
        }

        private void straightStairs_DB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", straightStairs_DB_ML_cbx);
        }

        private void straightStairs_S_MB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", straightStairs_S_MB_ML_cbx);
        }

        private void straightStairs_S_NB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", straightStairs_S_NB_ML_cbx);
        }

        //U-Stairs
        private void UStairs_WSF1_MB1_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_WSF1_MB1_ML_cbx);
        }

        private void UStairs_WSF1_MB2_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_WSF1_MB2_ML_cbx);
        }

        private void UStairs_WSF1_MB3_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_WSF1_MB3_ML_cbx);
        }

        private void UStairs_WSF2_MB1_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_WSF2_MB1_ML_cbx);
        }

        private void UStairs_WSF2_MB2_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_WSF2_MB2_ML_cbx);
        }

        private void UStairs_WSF2_MB3_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_WSF2_MB3_ML_cbx);
        }

        private void UStairs_DB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_DB_ML_cbx);
        }

        private void UStairs_LR_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_LR_ML_cbx);
        }

        private void UStairs_S_MB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_S_MB_ML_cbx);
        }

        private void UStairs_S_NB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", UStairs_S_NB_ML_cbx);
        }

        //L-Stairs
        private void LStair_WSF1_MB1_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_WSF1_MB1_ML_cbx);
        }

        private void LStair_WSF1_MB2_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_WSF1_MB2_ML_cbx);
        }

        private void LStair_WSF1_MB3_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_WSF1_MB3_ML_cbx);
        }

        private void LStair_WSF2_MB1_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_WSF2_MB1_ML_cbx);
        }

        private void LStair_WSF2_MB2_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_WSF2_MB2_ML_cbx);
        }

        private void LStair_WSF2_MB3_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_WSF2_MB3_ML_cbx);
        }

        private void LStair_DB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_DB_ML_cbx);
        }

        private void LStair_LR_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_LR_ML_cbx);
        }

        private void LStair_S_MB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_S_MB_ML_cbx);
        }

        private void LStair_S_NB_ML_cbx_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Select the desired manufactured length for the steel reinforcement. " +
                "\nDefault value shown has the lowest average wastage selected by the system as " +
                "the optimal choice", LStair_S_NB_ML_cbx);
        }
    }
}
