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
    public partial class ViewDetailedInfoForm : Form
    {
        public ViewDetailedInfoForm(string index, CostEstimationForm cf, StructuralMembers sm)
        {
            InitializeComponent();

            int count = 0;
            int count2 = 0;
            //Earthworks -- START
            if (index.Equals("1.1")) //Excavation
            {
                this.Text = index + " Excavation";

                foreach (List<double> solution in sm.earthworkSolutions)
                {
                    if(solution[0] == 1)
                    {
                        string toLabel1 = "•" + sm.footingColumnNames[count] + " @ " + sm.footingsColumn[0][count][4] + " Set/s";
                        double value = sm.earthworkSolutions[count + count2][1] / 1000000000;
                        string toLabel2 = "Excavated volume: " + value.ToString() + " m³";

                        Label label1 = new Label();
                        label1.Text = toLabel1;
                        label1.AutoSize = true;
                        Label label2 = new Label();
                        label2.Text = toLabel2;
                        label2.AutoSize = true;
                        panelView.Controls.Add(label1);
                        panelView.Controls.Add(label2);
                        count++;
                    }
                    else
                    {
                        string toLabel1 = "•" + sm.footingWallNames[count2] + " @ " + sm.footingsWall[0][count2][6] + " Set/s";
                        double value = sm.earthworkSolutions[count + count2][1] / 1000000000;
                        string toLabel2 = "Excavated volume: " + value.ToString() + " m³";

                        Label label1 = new Label();
                        label1.Text = toLabel1;
                        label1.AutoSize = true;
                        Label label2 = new Label();
                        label2.Text = toLabel2;
                        label2.AutoSize = true;
                        panelView.Controls.Add(label1);
                        panelView.Controls.Add(label2);
                        count2++;
                    }
                }
                double value3 = sm.extraEarthworkSolutions[0]; //recompute or get from Compute.cs into structuralMembers
                string toLabel3 = "•Soil Grading Cut\nExcavated Volume: " + value3 + " m³";
                double value4 = cf.excavation_Total; 
                string toLabel4 = "Total Excavated Volume: " + value4.ToString() + " m³";
                double value5 = 0; //TODO
                string toLabel5 = "Labor Cost per Cubic Meter: " + " ₱" + value5.ToString();
                double value6 = 0; //TODO 
                string toLabel6 = "Total Cost: " + " ₱" + value6.ToString();

                Label label3 = new Label();
                label3.Text = toLabel3;
                label3.AutoSize = true;
                Label label4 = new Label();
                label4.Text = toLabel4;
                label4.AutoSize = true;
                Label label5 = new Label();
                label5.Text = toLabel5;
                label5.AutoSize = true;
                Label label6 = new Label();
                label6.Text = toLabel6;
                label6.AutoSize = true;
                panelView.Controls.Add(label3);
                panelView.Controls.Add(label4);
                panelView.Controls.Add(label5);
                panelView.Controls.Add(label6);
            }
            else if (index.Equals("1.2")) //Backfilling and Compaction
            {
                this.Text = index + " Backfilling and Compaction";


                double value1 = 0; //recompute or get from Compute.cs into structuralMembers
                double value2 = 0; //recompute or get from Compute.cs into structuralMembers
                foreach (List<double> solution in sm.earthworkSolutions)
                {
                    if (solution[0] == 1)
                    {
                        value1 += sm.earthworkSolutions[count + count2][1] / 1000000000;
                    }
                    else
                    {
                        value2 += sm.earthworkSolutions[count + count2][1] / 1000000000;
                    }
                }

                string toLabel1 = "•Excavated Soil from Column Footings " +
                                  "\nVolume: " + value1;
                string toLabel2 = "•Excavated Soil from Wall Footing and Tie Beam " + 
                                  "\nVolume: " + value2;
                double value3 = sm.extraEarthworkSolutions[2]; //recompute or get from Compute.cs into structuralMembers
                string toLabel3 = "•Volume for Soil Filling " +
                                  "\nVolume: " + value3;
                double value4 = sm.extraEarthworkSolutions[3]; //recompute or get from Compute.cs into structuralMembers
                string toLabel4 = "•Volume of Excess Soil " +
                                  "\nVolume: " + value4;

                Label label1 = new Label();
                label1.Text = toLabel1;
                label1.AutoSize = true;
                Label label2 = new Label();
                label2.Text = toLabel2;
                label2.AutoSize = true;
                Label label3 = new Label();
                label3.Text = toLabel3;
                label3.AutoSize = true;
                Label label4 = new Label();
                label4.Text = toLabel4;
                label4.AutoSize = true;
                panelView.Controls.Add(label1);
                panelView.Controls.Add(label2);
                panelView.Controls.Add(label3);
                panelView.Controls.Add(label4);

                double value5 = cf.backfillingAndCompaction_Total;
                string toLabel5 = "Total Volume: " + value5.ToString() + " m³";
                double value6 = 0; //TODO
                string toLabel6 = "Labor Cost per Cubic Meter: " + " ₱" + value6.ToString();
                double value7 = 0; //TODO 
                string toLabel7 = "Total Cost: " + " ₱" + value7.ToString();

                Label label5 = new Label();
                label4.Text = toLabel4;
                label4.AutoSize = true;
                Label label6 = new Label();
                label5.Text = toLabel5;
                label5.AutoSize = true;
                Label label7 = new Label();
                label6.Text = toLabel6;
                label6.AutoSize = true;
                panelView.Controls.Add(label5);
                panelView.Controls.Add(label6);
                panelView.Controls.Add(label7);
            }
            else if (index.Equals("1.3")) //Grading and Compaction
            {
                this.Text = index + " Grading and Compaction";
                foreach (List<double> solution in sm.earthworkSolutions)
                {
                    if (solution[0] == 1)
                    {
                        string toLabel1 = "•" + sm.footingColumnNames[count] + " @ " + sm.footingsColumn[0][count][4] + " Set/s";
                        double value = sm.earthworkSolutions[count + count2][2] / 1000000;
                        string toLabel2 = "Area: " + value.ToString() + " m²";

                        Label label1 = new Label();
                        label1.Text = toLabel1;
                        label1.AutoSize = true;
                        Label label2 = new Label();
                        label2.Text = toLabel2;
                        label2.AutoSize = true;
                        panelView.Controls.Add(label1);
                        panelView.Controls.Add(label2);
                        count++;
                    }
                    else
                    {
                        string toLabel1 = "•" + sm.footingWallNames[count2] + " @ " + sm.footingsWall[0][count2][6] + " Set/s";
                        double value = sm.earthworkSolutions[count + count2][2] / 1000000;
                        string toLabel2 = "Area: " + value.ToString() + " m²";

                        Label label1 = new Label();
                        label1.Text = toLabel1;
                        label1.AutoSize = true;
                        Label label2 = new Label();
                        label2.Text = toLabel2;
                        label2.AutoSize = true;
                        panelView.Controls.Add(label1);
                        panelView.Controls.Add(label2);
                        count2++;
                    }
                }
                string toLabel3 = "•Slab on Grade\nArea: " + cf.parameters.earth_SG_AS + " m²";
                double value4 = cf.gradingAndCompaction_Total;
                string toLabel4 = "Total Area for Compaction: " + value4.ToString() + " m²";
                double value5 = 0; //TODO
                string toLabel5 = "Labor Cost per Cubic Meter: " + " ₱" + value5.ToString();
                double value6 = 0; //TODO 
                string toLabel6 = "Total Cost: " + " ₱" + value6.ToString();

                Label label3 = new Label();
                label3.Text = toLabel3;
                label3.AutoSize = true;
                Label label4 = new Label();
                label4.Text = toLabel4;
                label4.AutoSize = true;
                Label label5 = new Label();
                label5.Text = toLabel5;
                label5.AutoSize = true;
                Label label6 = new Label();
                label6.Text = toLabel6;
                label6.AutoSize = true;
                panelView.Controls.Add(label3);
                panelView.Controls.Add(label4);
                panelView.Controls.Add(label5);
                panelView.Controls.Add(label6);
            }
            else if (index.Equals("1.4")) //Gravel Bedding and Compaction
            {
                this.Text = index + " Gravel Bedding and Compaction";
                foreach (List<double> solution in sm.earthworkSolutions)
                {
                    if (solution[0] == 1)
                    {
                        string toLabel1 = "•" + sm.footingColumnNames[count] + " @ " + sm.footingsColumn[0][count][4] + " Set/s";
                        double value = sm.earthworkSolutions[count + count2][3] / 1000000000;
                        string toLabel2 = "Volume of Gravel: " + value.ToString() + " m³";

                        Label label1 = new Label();
                        label1.Text = toLabel1;
                        label1.AutoSize = true;
                        Label label2 = new Label();
                        label2.Text = toLabel2;
                        label2.AutoSize = true;
                        panelView.Controls.Add(label1);
                        panelView.Controls.Add(label2);
                        count++;
                    }
                    else
                    {
                        string toLabel1 = "•" + sm.footingWallNames[count2] + " @ " + sm.footingsWall[0][count2][6] + " Set/s";
                        double value = sm.earthworkSolutions[count + count2][3] / 1000000000;
                        string toLabel2 = "Volume of Gravel: " + value.ToString() + " m³";

                        Label label1 = new Label();
                        label1.Text = toLabel1;
                        label1.AutoSize = true;
                        Label label2 = new Label();
                        label2.Text = toLabel2;
                        label2.AutoSize = true;
                        panelView.Controls.Add(label1);
                        panelView.Controls.Add(label2);
                        count2++;
                    }
                }
                double value3 = sm.extraEarthworkSolutions[1]; //recompute or get from Compute.cs into structuralMembers
                string toLabel3 = "•Slab on Grade\nVolume of Gravel: " + value3.ToString() + " m³";
                double value4 = cf.gravelBedding_Total;
                string toLabel4 = "Total Volume of Gravel Needed (w/5% Factor of Safety): " + value4.ToString() + " m³";
                double value5 = 0; //TODO
                string toLabel5 = "Cost of [Gravel per M3 (G-1): " + " ₱" + value5.ToString();
                double value6 = 0; //TODO 
                string toLabel6 = "Total Cost: " + " ₱" + value6.ToString();

                Label label3 = new Label();
                label3.Text = toLabel3;
                label3.AutoSize = true;
                Label label4 = new Label();
                label4.Text = toLabel4;
                label4.AutoSize = true;
                Label label5 = new Label();
                label5.Text = toLabel5;
                label5.AutoSize = true;
                Label label6 = new Label();
                label6.Text = toLabel6;
                label6.AutoSize = true;
                panelView.Controls.Add(label3);
                panelView.Controls.Add(label4);
                panelView.Controls.Add(label5);
                panelView.Controls.Add(label6);
            }
            else if (index.Equals("1.5")) //Soil Poisoning
            {
                this.Text = index + " Soil Poisoning";
                double value1 = cf.soilPoisoning_Total; //recompute or get from Compute.cs into structuralMembers
                string toLabel1 = "•Area Needed for Soil Poisoning " +
                                  "\nVolume: " + value1.ToString() + " m²";
                double value2 = 0; //TODO
                string toLabel2 = "•Cost of Soil Poison per square meter: ₱" + value2;
                double value3 = 0; //TODO
                string toLabel3 = "•Total Cost: ₱" + value3;

                Label label1 = new Label();
                label1.Text = toLabel1;
                label1.AutoSize = true;
                Label label2 = new Label();
                label2.Text = toLabel2;
                label2.AutoSize = true;
                Label label3 = new Label();
                label3.Text = toLabel3;
                label3.AutoSize = true;
                panelView.Controls.Add(label1);
                panelView.Controls.Add(label2);
                panelView.Controls.Add(label3);
            }
            //Earthworks -- END
        }
    }
}
