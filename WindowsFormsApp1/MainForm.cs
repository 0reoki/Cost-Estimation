﻿using System;
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
        ParametersForm pf;
        Parameters parameters;
        private List<Floor> floors = new List<Floor>();

        //Passed Variables

        //Local Variables
        public bool saveFileExists;
        private int floorCount;
        String fileName;

        //Getters and Setters
        public List<Floor> Floors { get => floors; set => floors = value; }

        //General Functions -- START
        public CostEstimationForm()
        {
            InitializeComponent();
            InitializeAsync();

            //Initialize Forms that are single throughout the whole app
            parameters = new Parameters();
            pf = new ParametersForm(parameters, this);
            saveFileExists = false;

            //Set a ground floor
            floorCount = floors.Count;
            Floor floor = new Floor(this);
            floors.Add(floor);
            estimationPanel.Controls.Add(floor);

            //Initialize Local Variables
            fileName = null;
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
            //Help tab button is clicked
            else if (e.TabPageIndex == 4)
            {
                e.Cancel = true;
            }
        }

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
                try
                {
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
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
            floorCount = floors.Count;
            Floor floor = new Floor(this);
            floors.Add(floor);
            estimationPanel.Controls.Add(floor);
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
                    Floors[i].setLabel = AddOrdinal(i) + " FLOOR";
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

            //Parameters -- START
            stringParam += "Parameters|\n";

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
            parameters.cmIsSelected[0] + "|" + parameters.conc_CM_F_CG + "|" + parameters.conc_CM_F_GT + "|" + parameters.conc_CM_F_RM + "|" +
            parameters.cmIsSelected[1] + "|" + parameters.conc_CM_C_CG + "|" + parameters.conc_CM_C_GT + "|" + parameters.conc_CM_C_RM + "|" +
            parameters.cmIsSelected[2] + "|" + parameters.conc_CM_B_CG + "|" + parameters.conc_CM_B_GT + "|" + parameters.conc_CM_B_RM + "|" +
            parameters.cmIsSelected[3] + "|" + parameters.conc_CM_S_CG + "|" + parameters.conc_CM_S_GT + "|" + parameters.conc_CM_S_RM + "|" +

            parameters.conc_CM_W_MEW_CM + "|" + parameters.conc_CM_W_MIW_CM + "|" + parameters.conc_CM_W_P_CM + "|" + parameters.conc_CM_W_P_PT + "|" +

            parameters.cmIsSelected[4] + "|" + parameters.conc_CM_ST_CG + "|" + parameters.conc_CM_ST_GT + "|" + parameters.conc_CM_ST_RM + "|" +

            parameters.conc_CC_F + "|" + parameters.conc_CC_SS + "|" + parameters.conc_CC_SG + "|" + parameters.conc_CC_BEE + "|" +
            parameters.conc_CC_BEW + "|" + parameters.conc_CC_CEE + "|" + parameters.conc_CC_CEW + "|" ;

            //Reinforcement
            //stringParam += "\nReinforcement|\n";

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

            //Computations -- START
            stringParam += "Computations|\n";

            //Computations -- END

            //Save to File
            File.WriteAllText(fileName, stringParam);
        }

        private void SaveToProgram(string stringFile)
        {
            //Remove newlines
            stringFile = stringFile.Replace("\n", String.Empty);
            //Divide data, store into tokens
            string[] tokens = stringFile.Split('|');

            //Save to Parameters -- START
            int i = 0;
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
            int j = 0;
            while (!tokens[i].Equals("Formworks"))
            {
                j++;
                if(tokens[i].Equals("Elevation-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.earth_elevations.Add(toAdd);
                }
                i += 2;
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
            parameters.cmIsSelected[0] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_F_CG = tokens[i]; i++;
            parameters.conc_CM_F_GT = tokens[i]; i++;
            parameters.conc_CM_F_RM = tokens[i]; i++;
            parameters.cmIsSelected[1] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_C_CG = tokens[i]; i++;
            parameters.conc_CM_C_GT = tokens[i]; i++;
            parameters.conc_CM_C_RM = tokens[i]; i++;
            parameters.cmIsSelected[2] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_B_CG = tokens[i]; i++;
            parameters.conc_CM_B_GT = tokens[i]; i++;
            parameters.conc_CM_B_RM = tokens[i]; i++;
            parameters.cmIsSelected[3] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_S_CG = tokens[i]; i++;
            parameters.conc_CM_S_GT = tokens[i]; i++;
            parameters.conc_CM_S_RM = tokens[i]; i++;

            parameters.conc_CM_W_MEW_CM = tokens[i]; i++;
            parameters.conc_CM_W_MIW_CM = tokens[i]; i++;
            parameters.conc_CM_W_P_CM = tokens[i]; i++;
            parameters.conc_CM_W_P_PT = tokens[i]; i++;

            parameters.cmIsSelected[4] = bool.Parse(tokens[i]); i++;
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
            while (!tokens[i].Equals("Computations"))
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

            //Save to Computations -- START

            MessageBox.Show(tokens[i]);
            
            //Save to Computations -- END
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