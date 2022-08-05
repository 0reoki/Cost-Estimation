using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowEst
{
    public class Compute 
    {
        //General Functions -- START
        private List<Floor> floors = new List<Floor>();
        public List<Floor> Floors { get => floors; set => floors = value; }
        public void refreshSolutions(CostEstimationForm cEF)
        {
            //Earthworks -- START
            cEF.excavation_Total = 0;
            cEF.gradingAndCompaction_Total = 0;
            cEF.gravelBedding_Total = 0;
            cEF.backfillingAndCompaction_Total = 0;
            double concreting = 0;
            foreach (List<double> toAdd in cEF.structuralMembers.earthworkSolutions)
            {
                cEF.excavation_Total += toAdd[1];
                cEF.gradingAndCompaction_Total += toAdd[2];
                cEF.gravelBedding_Total += toAdd[3];
                cEF.backfillingAndCompaction_Total += toAdd[1];
                concreting += toAdd[4];
            }

            cEF.excavation_Total = cEF.excavation_Total / 1000000000;
            cEF.gradingAndCompaction_Total = cEF.gradingAndCompaction_Total / 1000000;
            cEF.gravelBedding_Total = cEF.gravelBedding_Total / 1000000000;
            cEF.soilPoisoning_Total = double.Parse(cEF.parameters.earth_SG_AS, System.Globalization.CultureInfo.InvariantCulture);
            cEF.backfillingAndCompaction_Total = cEF.backfillingAndCompaction_Total / 1000000000;
            double gravelBeddingTotal = cEF.gravelBedding_Total;
            concreting = concreting / 1000000000; 

            //Misc Calculations

            //Excavation
            double totalCut = 0;
            foreach (string[] elev in cEF.parameters.earth_elevations)
            {
                if (elev[2].Equals("CUT"))
                {
                    double area = double.Parse(elev[1], System.Globalization.CultureInfo.InvariantCulture);
                    double thickness = double.Parse(elev[3], System.Globalization.CultureInfo.InvariantCulture);
                    totalCut += (area * thickness);
                }
            }
            cEF.structuralMembers.extraEarthworkSolutions[0] = totalCut;
            cEF.excavation_Total += totalCut; //Soil Grading Cut

            //Grading and Compaction
            cEF.gradingAndCompaction_Total += double.Parse(cEF.parameters.earth_SG_AS, System.Globalization.CultureInfo.InvariantCulture);

            //Gravel Bedding
            double toAddtoGB = 0;
            double gravelBedding = double.Parse(cEF.parameters.earth_SG_TH, System.Globalization.CultureInfo.InvariantCulture); 
            double sogArea = double.Parse(cEF.parameters.earth_SG_AS, System.Globalization.CultureInfo.InvariantCulture); 
            string value = cEF.parameters.earth_SG_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            double compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;  
            toAddtoGB = gravelBedding + (gravelBedding * compactionAllowance);
            toAddtoGB /= 1000;
            toAddtoGB *= sogArea;
            //double factorOfSafety = (cEF.gravelBedding_Total + toAddtoGB) * 0.05;
            cEF.gravelBedding_Total = cEF.gravelBedding_Total + toAddtoGB /*+ factorOfSafety */;
            cEF.structuralMembers.extraEarthworkSolutions[1] = toAddtoGB; //Slab On Grade

            //Backfilling and Compaction
            double totalFill = 0;
            foreach (string[] elev in cEF.parameters.earth_elevations)
            {
                if (elev[2].Equals("FILL"))
                {
                    double area = double.Parse(elev[1], System.Globalization.CultureInfo.InvariantCulture);
                    double thickness = double.Parse(elev[3], System.Globalization.CultureInfo.InvariantCulture);
                    totalFill += (area * thickness);
                }
            }
            double totalExcessSoil = concreting + gravelBeddingTotal + totalCut;
            cEF.backfillingAndCompaction_Total = 
                cEF.backfillingAndCompaction_Total + totalFill - totalExcessSoil;

            cEF.structuralMembers.extraEarthworkSolutions[2] = totalFill; //Slab On Grade
            cEF.structuralMembers.extraEarthworkSolutions[3] = totalExcessSoil; //Slab On Grade

            if (totalExcessSoil > totalFill)
            {
                // Di kailangan bumili lol
            }
            else
            {
                // Do something sa cost
            }
            //Earthworks -- END

            //Concrete Works -- START
            //Refresh all Column, Beams, Slabs TODO

            //Concrete Works -- END
        }
        //General Functions -- END

        //Footing Computation Functions -- START
        public void AddFootingWorks(CostEstimationForm cEF, int footingCount, int wallFootingCount, bool isFooting)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.earthworkSolutions.Add(newList);
            List<double> newList2 = new List<double>();
            cEF.structuralMembers.concreteWorkSolutionsF.Add(newList2);
            List<List<double>> newList3 = new List<List<double>>();
            if (isFooting)
            {
                cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(1);
                cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(1);
                cEF.structuralMembers.footingReinforcements.Add(newList3);
            }
            else
            {
                cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(2);
                cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(2);
                cEF.structuralMembers.wallFootingReinforcements.Add(newList3);
            }
            footingWorks(cEF, footingCount, wallFootingCount, isFooting);            
        }

        public void ModifyFootingWorks(CostEstimationForm cEF, int structMemCount, int count, bool isFooting)
        {
            modifyFootingWorks(cEF, structMemCount, count, isFooting);
        }

        //Add 
        public void footingWorks(CostEstimationForm cEF, int footingCount, int wallFootingCount, bool isFooting)
        {
            cEF.structuralMembers.per_col.Add(0);
            cEF.structuralMembers.per_wal.Add(0);
            if (isFooting)
            {
                if (cEF.structuralMembers.footingsColumn[0][footingCount][0].Equals("Isolated Footing"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccF = double.Parse(cEF.parameters.conc_CC_F, System.Globalization.CultureInfo.InvariantCulture);
                    for (int i = 0; i < 7; i++)
                        manufacturedLength[i] = cEF.parameters.rein_mfIsSelected[0, i];

                    //Variables from StructMem
                    double length, width, thickness, quantity, depth, lrDiameter, lrQuantity, lrHookType,
                       trDiameter, trQuantity, trHookType;
                    
                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    width = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][11], System.Globalization.CultureInfo.InvariantCulture);

                    //Computation Earth Works

                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        (length + formworkAllowance * 2) * (width + formworkAllowance * 2) * (depth + gravelBedding) * quantity);

                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2)) * quantity);

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2) * 
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity);

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        length * width * thickness * quantity);

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((length * width * thickness) / 1000000000) * quantity);
                    }                    
                    //Computation -- Formworks **                                     
                    cEF.structuralMembers.per_col[footingCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;

                    //Computation -- Rebars
                    //1
                    double qtyL = quantity * lrQuantity;
                    double qtyT = quantity * trQuantity;

                    //2
                    double hl_L = 0;
                    double hl_T = 0;
                    for (int i = 0; i < cEF.parameters.rein_BEH_MB_dt.Rows.Count; i++)
                    {
                        if (lrHookType == 90.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 135.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccF) + (2 * hl_L);
                    double LB_qtyT = width - (2 * ccF) + (2 * hl_T);
                    LB_qtyL /= 1000;
                    LB_qtyT /= 1000;
                    //3
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double Le = 0;
                    double Lw = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter ->  Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    for (int i = 0; i < 7; i++)
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (i * 1.5);
                        if (manufacturedLength[i])
                        {
                            //Longitudinal
                            qtyP = Lm / LB_qtyL;
                            qtyM = qtyL / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                            {
                                Lw = 0;
                            }
                            else 
                            {
                                Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                            }
                            totalWaste_L = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                {
                                    if (chosenML_L.Any())
                                    {
                                        if(totalWaste_L < chosenML_L[7])
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_L.Clear();
                                        chosenML_L.Add(Lm);
                                        chosenML_L.Add(qtyL);
                                        chosenML_L.Add(LB_qtyL);
                                        chosenML_L.Add(qtyP);
                                        chosenML_L.Add(qtyM);
                                        chosenML_L.Add(Lw);
                                        chosenML_L.Add(Le);
                                        chosenML_L.Add(totalWaste_L);
                                        chosenML_L.Add(lrDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_L.Clear();
                                chosenML_L.Add(Lm);
                                chosenML_L.Add(qtyL);
                                chosenML_L.Add(LB_qtyL);
                                chosenML_L.Add(qtyP);
                                chosenML_L.Add(qtyM);
                                chosenML_L.Add(Lw);
                                chosenML_L.Add(Le);
                                chosenML_L.Add(totalWaste_L);
                                chosenML_L.Add(lrDiameter);
                            }

                            //Transverse
                            qtyP = Lm / LB_qtyT;
                            qtyM = qtyT / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                            {
                                Lw = 0;
                            }
                            else
                            {
                                Lw = Lm - LB_qtyT * Math.Floor(qtyP);
                            }
                            totalWaste_T = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                {
                                    if (chosenML_T.Any())
                                    {
                                        if (totalWaste_T < chosenML_T[7])
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_T.Clear();
                                chosenML_T.Add(Lm);
                                chosenML_T.Add(qtyT);
                                chosenML_T.Add(LB_qtyT);
                                chosenML_T.Add(qtyP);
                                chosenML_T.Add(qtyM);
                                chosenML_T.Add(Lw);
                                chosenML_T.Add(Le);
                                chosenML_T.Add(totalWaste_T);
                                chosenML_T.Add(trDiameter);
                            }
                        }
                    }
                    //4 -> Costing of material -> in MainForm.cs function initializeView()
                    //5
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }

                    //Longitudinal
                    for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(lrDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_L[0] * chosenML_L[4] * Wd;
                    chosenML_L.Add(weight);

                    //Transverse
                    for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * chosenML_T[4] * Wd;
                    chosenML_T.Add(weight);

                    //6 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.footingReinforcements[footingCount].Add(chosenML_L);
                    cEF.structuralMembers.footingReinforcements[footingCount].Add(chosenML_T);
                }
                else //Combined Footing
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccF = double.Parse(cEF.parameters.conc_CC_F, System.Globalization.CultureInfo.InvariantCulture);
                    for(int i = 0; i < 7; i++)
                        manufacturedLength[i] = cEF.parameters.rein_mfIsSelected[0, i];

                    double length, width, thickness, quantity, depth, lrDiameter, lrQuantity, lrSpacing, lrHookType,
                       trDiameter, trQuantity, trSpacing, trHookType, urDiameter, urQuantity, urSpacing, urHookType;

                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    width = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrSpacing = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][11], System.Globalization.CultureInfo.InvariantCulture);
                    trSpacing = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][12], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][13], System.Globalization.CultureInfo.InvariantCulture);
                    urDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][14], System.Globalization.CultureInfo.InvariantCulture);
                    urQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][15], System.Globalization.CultureInfo.InvariantCulture);
                    urSpacing = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][16], System.Globalization.CultureInfo.InvariantCulture);
                    urHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][footingCount][17], System.Globalization.CultureInfo.InvariantCulture);

                    //Computation -- Earth Works

                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        (length + formworkAllowance * 2) * (width + formworkAllowance * 2) * (depth + gravelBedding) * quantity);

                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2)) * quantity);

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2) *
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity);

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        length * width * thickness * quantity);

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((length * width * thickness) / 1000000000) * quantity);
                    }
                    //Computation -- Formworks **                   
                    cEF.structuralMembers.per_col[footingCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;

                    //Computation -- Rebars
                    //1
                    double qtyL = 0;
                    double qtyU = 0;
                    double qtyT = 0;
                    if (lrQuantity == 0)
                        qtyL = quantity * (Math.Ceiling((width - (2 * ccF)) / lrSpacing + 1));
                    else
                        qtyL = quantity * lrQuantity;
                    if (urQuantity == 0)
                        qtyU = quantity * (Math.Ceiling((width - (2 * ccF)) / urSpacing + 1));
                    else
                        qtyU = quantity * urQuantity;
                    if (trQuantity == 0)
                        qtyT = quantity * (Math.Ceiling((length - (2 * ccF)) / trSpacing + 1));
                    else
                        qtyT = quantity * trQuantity;

                    //2
                    double hl_L = 0;
                    double hl_U = 0;
                    double hl_T = 0;
                    for (int i = 0; i < cEF.parameters.rein_BEH_MB_dt.Rows.Count; i++)
                    {
                        if (lrHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        } 
                        else if (lrHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (urHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(urDiameter.ToString()))
                            {
                                hl_U = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (urHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(urDiameter.ToString()))
                            {
                                hl_U = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (urHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(urDiameter.ToString()))
                            {
                                hl_U = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccF) + (2 * hl_L);
                    double LB_qtyU = length - (2 * ccF) + (2 * hl_U);
                    double LB_qtyT = width - (2 * ccF) + (2 * hl_T);
                    LB_qtyL /= 1000;
                    LB_qtyU /= 1000;
                    LB_qtyT /= 1000;

                    //3
                    double largestML = 0;
                    for(int i = 6; i >= 0; i--)
                    {
                        if (manufacturedLength[i])
                        {
                            switch (i)
                            {
                                case 0:
                                    largestML = 6; break;
                                case 1:
                                    largestML = 7.5; break;
                                case 2:
                                    largestML = 9.0; break;
                                case 3:
                                    largestML = 10.5; break;
                                case 4:
                                    largestML = 12.0; break;
                                case 5:
                                    largestML = 13.5; break;
                                case 6:
                                    largestML = 15.0; break;
                                default:
                                    largestML = 0; break;
                            }
                            break;
                        }
                    }
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double Le = 0;
                    double Lw = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_U = 0;
                    double previousTotalWaste_U = 0;

                    double totalWaste_LU = 0;
                    double previousTotalWaste_LU = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[U] -> LB of QTY[U] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_U = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    for (int i = 0; i < 7; i++) 
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_U = totalWaste_U;
                        previousTotalWaste_LU = totalWaste_LU;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (i * 1.5);
                        if (manufacturedLength[i])
                        {
                            if ((LB_qtyL + LB_qtyT) <= largestML && lrDiameter == urDiameter && lrDiameter == trDiameter) //a)
                            {
                                if ((qtyL + qtyU) < qtyT) //Case 1
                                {
                                    //(Longitudinal + Upper)
                                    qtyP = Lm / (LB_qtyL + LB_qtyT);
                                    qtyM = (qtyL + qtyU) / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                                    }
                                    totalWaste_LU = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_LU != 0 || Double.IsNaN(previousTotalWaste_LU)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_LU < previousTotalWaste_LU || (Double.IsNaN(previousTotalWaste_LU) && !Double.IsNaN(totalWaste_LU)))
                                        {
                                            if (chosenML_L.Any())
                                            {
                                                if (totalWaste_LU < chosenML_L[7])
                                                {
                                                    chosenML_L.Clear();
                                                    chosenML_L.Add(Lm);
                                                    chosenML_L.Add(qtyL + qtyU);
                                                    chosenML_L.Add(LB_qtyL + LB_qtyT);
                                                    chosenML_L.Add(qtyP);
                                                    chosenML_L.Add(qtyM);
                                                    chosenML_L.Add(Lw);
                                                    chosenML_L.Add(Le);
                                                    chosenML_L.Add(totalWaste_LU);
                                                    chosenML_L.Add(lrDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL + qtyU);
                                                chosenML_L.Add(LB_qtyL + LB_qtyT);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_LU);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_LU == 0 && !Double.IsNaN(totalWaste_LU) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_L.Clear();
                                        chosenML_L.Add(Lm);
                                        chosenML_L.Add(qtyL + qtyU);
                                        chosenML_L.Add(LB_qtyL + LB_qtyT);
                                        chosenML_L.Add(qtyP);
                                        chosenML_L.Add(qtyM);
                                        chosenML_L.Add(Lw);
                                        chosenML_L.Add(Le);
                                        chosenML_L.Add(totalWaste_LU);
                                        chosenML_L.Add(lrDiameter);
                                    }

                                    //Transverse
                                    qtyP = Lm / LB_qtyT;
                                    qtyM = (qtyT - qtyL - qtyU) / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - (LB_qtyL + LB_qtyT) * Math.Floor(qtyP);
                                    }
                                    totalWaste_T = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                        {
                                            if (chosenML_T.Any())
                                            {
                                                if (totalWaste_T < chosenML_T[7])
                                                {
                                                    chosenML_T.Clear();
                                                    chosenML_T.Add(Lm);
                                                    chosenML_T.Add(qtyT - qtyL - qtyU);
                                                    chosenML_T.Add(LB_qtyT);
                                                    chosenML_T.Add(qtyP);
                                                    chosenML_T.Add(qtyM);
                                                    chosenML_T.Add(Lw);
                                                    chosenML_T.Add(Le);
                                                    chosenML_T.Add(totalWaste_T);
                                                    chosenML_T.Add(trDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_T.Clear();
                                                chosenML_T.Add(Lm);
                                                chosenML_T.Add(qtyT - qtyL - qtyU);
                                                chosenML_T.Add(LB_qtyT);
                                                chosenML_T.Add(qtyP);
                                                chosenML_T.Add(qtyM);
                                                chosenML_T.Add(Lw);
                                                chosenML_T.Add(Le);
                                                chosenML_T.Add(totalWaste_T);
                                                chosenML_T.Add(trDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT - qtyL - qtyU);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                                else if (qtyL + qtyU >= qtyT) //Case 2
                                {
                                    //(Longitudinal + Upper)
                                    qtyP = Lm / LB_qtyL;
                                    qtyM = (qtyL + qtyU - qtyT) / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                                    }
                                    totalWaste_LU = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_LU != 0 || Double.IsNaN(previousTotalWaste_LU)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_LU < previousTotalWaste_LU || (Double.IsNaN(previousTotalWaste_LU) && !Double.IsNaN(totalWaste_LU)))
                                        {
                                            if (chosenML_L.Any())
                                            {
                                                if (totalWaste_LU < chosenML_L[7])
                                                {
                                                    chosenML_L.Clear();
                                                    chosenML_L.Add(Lm);
                                                    chosenML_L.Add(qtyL + qtyU - qtyT);
                                                    chosenML_L.Add(LB_qtyL);
                                                    chosenML_L.Add(qtyP);
                                                    chosenML_L.Add(qtyM);
                                                    chosenML_L.Add(Lw);
                                                    chosenML_L.Add(Le);
                                                    chosenML_L.Add(totalWaste_LU);
                                                    chosenML_L.Add(lrDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL + qtyU - qtyT);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_LU);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_LU == 0 && !Double.IsNaN(totalWaste_LU) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_L.Clear();
                                        chosenML_L.Add(Lm);
                                        chosenML_L.Add(qtyL + qtyU - qtyT);
                                        chosenML_L.Add(LB_qtyL);
                                        chosenML_L.Add(qtyP);
                                        chosenML_L.Add(qtyM);
                                        chosenML_L.Add(Lw);
                                        chosenML_L.Add(Le);
                                        chosenML_L.Add(totalWaste_LU);
                                        chosenML_L.Add(lrDiameter);
                                    }

                                    //Transverse
                                    qtyP = Lm / (LB_qtyL + LB_qtyT);
                                    qtyM = qtyT / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - (LB_qtyL + LB_qtyT) * Math.Floor(qtyP);
                                    }
                                    totalWaste_T = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                        {
                                            if (chosenML_T.Any())
                                            {
                                                if (totalWaste_T < chosenML_T[7])
                                                {
                                                    chosenML_T.Clear();
                                                    chosenML_T.Add(Lm);
                                                    chosenML_T.Add(qtyT);
                                                    chosenML_T.Add(LB_qtyL + LB_qtyT);
                                                    chosenML_T.Add(qtyP);
                                                    chosenML_T.Add(qtyM);
                                                    chosenML_T.Add(Lw);
                                                    chosenML_T.Add(Le);
                                                    chosenML_T.Add(totalWaste_T);
                                                    chosenML_T.Add(trDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_T.Clear();
                                                chosenML_T.Add(Lm);
                                                chosenML_T.Add(qtyT);
                                                chosenML_T.Add(LB_qtyL + LB_qtyT);
                                                chosenML_T.Add(qtyP);
                                                chosenML_T.Add(qtyM);
                                                chosenML_T.Add(Lw);
                                                chosenML_T.Add(Le);
                                                chosenML_T.Add(totalWaste_T);
                                                chosenML_T.Add(trDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyL + LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (LB_qtyL + LB_qtyT > largestML || lrDiameter != urDiameter || lrDiameter != trDiameter) //b)
                            {
                                //(Longitudinal
                                qtyP = Lm / LB_qtyL;
                                qtyM = qtyL / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                {
                                    Lw = 0;
                                }
                                else
                                {
                                    Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                                }
                                totalWaste_L = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(LB_qtyL);
                                    chosenML_L.Add(qtyP);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(Lw);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }

                                //Upper
                                qtyP = Lm / LB_qtyU;
                                qtyM = qtyU / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                {
                                    Lw = 0;
                                }
                                else
                                {
                                    Lw = Lm - LB_qtyU * Math.Floor(qtyP);
                                }
                                totalWaste_U = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_U != 0 || Double.IsNaN(previousTotalWaste_U)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_U < previousTotalWaste_U || (Double.IsNaN(previousTotalWaste_U) && !Double.IsNaN(totalWaste_U)))
                                    {
                                        if (chosenML_U.Any())
                                        {
                                            if (totalWaste_U < chosenML_U[7])
                                            {
                                                chosenML_U.Clear();
                                                chosenML_U.Add(Lm);
                                                chosenML_U.Add(qtyU);
                                                chosenML_U.Add(LB_qtyU);
                                                chosenML_U.Add(qtyP);
                                                chosenML_U.Add(qtyM);
                                                chosenML_U.Add(Lw);
                                                chosenML_U.Add(Le);
                                                chosenML_U.Add(totalWaste_U);
                                                chosenML_U.Add(urDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_U.Clear();
                                            chosenML_U.Add(Lm);
                                            chosenML_U.Add(qtyU);
                                            chosenML_U.Add(LB_qtyU);
                                            chosenML_U.Add(qtyP);
                                            chosenML_U.Add(qtyM);
                                            chosenML_U.Add(Lw);
                                            chosenML_U.Add(Le);
                                            chosenML_U.Add(totalWaste_U);
                                            chosenML_U.Add(urDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_U == 0 && !Double.IsNaN(totalWaste_U) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_U.Clear();
                                    chosenML_U.Add(Lm);
                                    chosenML_U.Add(qtyU);
                                    chosenML_U.Add(LB_qtyU);
                                    chosenML_U.Add(qtyP);
                                    chosenML_U.Add(qtyM);
                                    chosenML_U.Add(Lw);
                                    chosenML_U.Add(Le);
                                    chosenML_U.Add(totalWaste_U);
                                    chosenML_U.Add(urDiameter);
                                }

                                //Transverse
                                qtyP = Lm / LB_qtyT;
                                qtyM = qtyT / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                {
                                    Lw = 0;
                                }
                                else
                                {
                                    Lw = Lm - LB_qtyT * Math.Floor(qtyP);
                                }
                                totalWaste_T = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                    {
                                        if (chosenML_T.Any())
                                        {
                                            if (totalWaste_T < chosenML_T[7])
                                            {
                                                chosenML_T.Clear();
                                                chosenML_T.Add(Lm);
                                                chosenML_T.Add(qtyT);
                                                chosenML_T.Add(LB_qtyT);
                                                chosenML_T.Add(qtyP);
                                                chosenML_T.Add(qtyM);
                                                chosenML_T.Add(Lw);
                                                chosenML_T.Add(Le);
                                                chosenML_T.Add(totalWaste_T);
                                                chosenML_T.Add(trDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_T.Clear();
                                    chosenML_T.Add(Lm);
                                    chosenML_T.Add(qtyT);
                                    chosenML_T.Add(LB_qtyT);
                                    chosenML_T.Add(qtyP);
                                    chosenML_T.Add(qtyM);
                                    chosenML_T.Add(Lw);
                                    chosenML_T.Add(Le);
                                    chosenML_T.Add(totalWaste_T);
                                    chosenML_T.Add(trDiameter);
                                }
                            }
                        }
                    }
                    //4 -> Costing of material -> in MainForm.cs function initializeView()
                    //5
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }

                    //Longitudinal and Upper
                    //If upper is not empty, then it is case b which produces 3 chosen manufactured length
                    if (chosenML_U.Any())
                    {
                        //Longitudinal
                        for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                        {
                            if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(lrDiameter + "mm"))
                            {
                                Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        weight = chosenML_L[0] * chosenML_L[4] * Wd;
                        chosenML_L.Add(weight);

                        //Upper
                        for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                        {
                            if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(urDiameter + "mm"))
                            {
                                Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        weight = chosenML_U[0] * chosenML_U[4] * Wd;
                        chosenML_U.Add(weight);
                    }
                    else
                    {
                        //Longitudinal and Upper
                        for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                        {
                            if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(lrDiameter + "mm"))
                            {
                                Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        weight = chosenML_L[0] * chosenML_L[4] * Wd;
                        chosenML_L.Add(weight);
                    }

                    //Transverse
                    for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * chosenML_T[4] * Wd;
                    chosenML_T.Add(weight);

                    //6 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.footingReinforcements[footingCount].Add(chosenML_L);
                    if(chosenML_U.Any())
                        cEF.structuralMembers.footingReinforcements[footingCount].Add(chosenML_U);
                    cEF.structuralMembers.footingReinforcements[footingCount].Add(chosenML_T);
                }
            }
            else // Wall Footing
            {
                if (cEF.structuralMembers.footingsWall[0][wallFootingCount][0].Equals("Rectangular"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccWF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccWF = double.Parse(cEF.parameters.conc_CC_SG, System.Globalization.CultureInfo.InvariantCulture);
                    for (int i = 0; i < 7; i++)
                        manufacturedLength[i] = cEF.parameters.rein_mfIsSelected[2, i];

                    //Variables from StructMem
                    double length, lengthF2F, wfBase, thickness, depth, quantity, lrDiameter, lrQuantity, lrSpacing, lrHookType,
                       trDiameter, trQuantity, trSpacing, trHookType;

                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    lengthF2F = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    wfBase = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    lrSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][11], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][12], System.Globalization.CultureInfo.InvariantCulture);
                    trSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][13], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][14], System.Globalization.CultureInfo.InvariantCulture);

                    //Computation -- Earth Works

                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        (lengthF2F + formworkAllowance * 2) * (wfBase + formworkAllowance * 2) * (depth + gravelBedding) * quantity);
                    
                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((lengthF2F + formworkAllowance * 2) * (wfBase + formworkAllowance * 2)) * quantity);

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((length + formworkAllowance * 2) * (wfBase + formworkAllowance * 2) *
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity);

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        length * wfBase * thickness * quantity);

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0]) 
                    { 
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((length * wfBase * thickness) / 1000000000) * quantity);
                    }

                    //Computation -- Rebars
                    //1 -> determine ccWF

                    //2
                    double qtyL = 0;
                    double qtyT = 0;
                    if (lrQuantity == 0)
                        qtyL = quantity * (Math.Ceiling((wfBase - (2 * ccWF)) / lrSpacing + 1));
                    else
                        qtyL = quantity * lrQuantity;
                    if (trQuantity == 0)
                        qtyT = quantity * (Math.Ceiling((length - (2 * ccWF)) / trSpacing + 1));
                    else
                        qtyT = quantity * trQuantity;

                    //3
                    double hl_L = 0;
                    double hl_T = 0;
                    for (int i = 0; i < cEF.parameters.rein_BEH_MB_dt.Rows.Count; i++)
                    {
                        if (lrHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccWF) + (2 * hl_L);
                    double LB_qtyT = wfBase - (2 * ccWF) + (2 * hl_T);
                    LB_qtyL /= 1000;
                    LB_qtyT /= 1000;

                    //4
                    double largestML = 0;
                    for (int i = 6; i >= 0; i--)
                    {
                        if (manufacturedLength[i])
                        {
                            switch (i)
                            {
                                case 0:
                                    largestML = 6; break;
                                case 1:
                                    largestML = 7.5; break;
                                case 2:
                                    largestML = 9.0; break;
                                case 3:
                                    largestML = 10.5; break;
                                case 4:
                                    largestML = 12.0; break;
                                case 5:
                                    largestML = 13.5; break;
                                case 6:
                                    largestML = 15.0; break;
                                default:
                                    largestML = 0; break;
                            }
                            break;
                        }
                    }
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double qtyCM = 0;
                    double Le = 0;
                    double Lw = 0;
                    double Lx = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    //Capture SL base on MPa selected by user
                    double MPa = 0;
                    double sl = 0;
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            MPa = 27.579;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            MPa = 24.1316;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            MPa = 17.236;
                        }
                        else
                        {
                            MPa = 13.789;
                        }
                    }
                    else
                    {
                        string selectedReadyMix = cEF.parameters.conc_CM_F_RM;
                        string stringMPa = selectedReadyMix.Substring(29, 4);
                        if (stringMPa.Contains("M"))
                            stringMPa = stringMPa.Substring(0, 2);
                        MPa = double.Parse(stringMPa, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    int k = 0;
                    foreach(string Fc in cEF.parameters.rein_LSL_TB_fc_list)
                    {
                        if (Fc.Equals(MPa.ToString()))
                            break;
                        k++;
                    }
                    for (int j = 0; j < cEF.parameters.rein_LSL_TB_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_LSL_TB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                        {
                            sl = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[j][k + 1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                    sl /= 1000;

                    for (int i = 0; i < 7; i++)
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (i * 1.5);
                        if (manufacturedLength[i])
                        {
                            if (LB_qtyL < largestML) 
                            {
                                //Longitudinal Reinforcement
                                qtyP = Lm / LB_qtyL;
                                qtyM = qtyL / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                    Lw = 0;
                                else
                                    Lw = Lm - (LB_qtyL) * Math.Floor(qtyP);
                                totalWaste_L = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(LB_qtyL);
                                    chosenML_L.Add(qtyP);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(Lw);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }
                            }
                            else 
                            {
                                //Longitudinal Reinforcement
                                qtyM = LB_qtyL / (Lm - sl);
                                Lx = (qtyM - Math.Floor(qtyM)) * (Lm - sl);
                                Le = Lm - Lx;
                                totalWaste_L = Le * qtyL;
                                qtyCM = qtyL * Math.Ceiling(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(sl);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(qtyCM);
                                                chosenML_L.Add(Lx);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(sl);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(qtyCM);
                                            chosenML_L.Add(Lx);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(sl);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(qtyCM);
                                    chosenML_L.Add(Lx);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }
                            }
                            //Transverse Reinforcement
                            qtyP = Lm / LB_qtyT;
                            qtyM = qtyT / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                                Lw = 0;
                            else
                                Lw = Lm - (LB_qtyT) * Math.Floor(qtyP);
                            totalWaste_T = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                {
                                    if (chosenML_T.Any())
                                    {
                                        if (totalWaste_T < chosenML_T[7])
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_T.Clear();
                                chosenML_T.Add(Lm);
                                chosenML_T.Add(qtyT);
                                chosenML_T.Add(LB_qtyT);
                                chosenML_T.Add(qtyP);
                                chosenML_T.Add(qtyM);
                                chosenML_T.Add(Lw);
                                chosenML_T.Add(Le);
                                chosenML_T.Add(totalWaste_T);
                                chosenML_T.Add(trDiameter);
                            }
                        }
                    }

                    //5 -> Costing of material -> in MainForm.cs function initializeView()
                    //6
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }

                    //Longitudinal
                    for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(lrDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_L[0] * Math.Ceiling(chosenML_L[4]) * Wd;
                    chosenML_L.Add(weight);

                    print("Weight L: " + weight + " and WD: " + Wd + " and LM: " + chosenML_L[0] + " and QTYM: " + chosenML_L[4]);

                    //Transverse
                    for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * Math.Ceiling(chosenML_T[4]) * Wd;
                    chosenML_T.Add(weight);

                    //7 - 8 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.wallFootingReinforcements[wallFootingCount].Add(chosenML_L);
                    cEF.structuralMembers.wallFootingReinforcements[wallFootingCount].Add(chosenML_T);
                }
                else //Trapezoidal
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccWF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccWF = double.Parse(cEF.parameters.conc_CC_SG, System.Globalization.CultureInfo.InvariantCulture);
                    for (int i = 0; i < 7; i++)
                        manufacturedLength[i] = cEF.parameters.rein_mfIsSelected[2, i];

                    //Variables from StructMem
                    double length, lengthF2F, wfBaseT, wfBaseU, thickness, depth, quantity, lrDiameter, lrQuantity, lrSpacing, lrHookType,
                       trDiameter, trQuantity, trSpacing, trHookType;

                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    lengthF2F = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    wfBaseT = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    wfBaseU = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    lrSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][11], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][12], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][13], System.Globalization.CultureInfo.InvariantCulture);
                    trSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][14], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsWall[0][wallFootingCount][15], System.Globalization.CultureInfo.InvariantCulture);
                    
                    
                    //Computation -- Earth Works
                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        (lengthF2F + formworkAllowance * 2) * (wfBaseT + formworkAllowance * 2) * (depth + gravelBedding) * quantity);

                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((lengthF2F + formworkAllowance * 2) * (wfBaseT + formworkAllowance * 2)) * quantity);

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        ((length + formworkAllowance * 2) * (wfBaseT + formworkAllowance * 2) *
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity);

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(
                        length * wfBaseT * thickness * quantity);

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = (((wfBaseT + wfBaseU) / 2) * thickness) * length * quantity;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][0]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][1]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][2]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = (((wfBaseT + wfBaseU) / 2) * thickness) * length * quantity;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = (((wfBaseT + wfBaseU) / 2) * thickness) * length * quantity;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][0]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][1]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][2]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                        else
                        {
                            double volume = (((wfBaseT + wfBaseU) / 2) * thickness) * length * quantity;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][0]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][1]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][2]);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * quantity);
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((((wfBaseT + wfBaseU) / 2) * thickness) * length * quantity) / 1000000000);
                    }
                    //Computation -- Formworks **                    
                    double c = Math.Sqrt(Math.Pow((((wfBaseT / 1000) - (wfBaseU / 1000)) / 2), 2) + Math.Pow((thickness / 1000), 2));                    
                    cEF.structuralMembers.per_wal[wallFootingCount] = ((((c * (length / 1000)) + 0.2) + ((((wfBaseT / 1000) + (wfBaseU / 1000)) / 2) * (thickness / 1000))) * 2) * quantity;

                    //Computation -- Rebars
                    //1 -> determine ccWF

                    //2
                    double qtyL = 0;
                    double qtyT = 0;
                    if (lrQuantity == 0)
                        qtyL = quantity * (Math.Ceiling((wfBaseT - (2 * ccWF)) / lrSpacing + 1));
                    else
                        qtyL = quantity * lrQuantity;
                    if (trQuantity == 0)
                        qtyT = quantity * (Math.Ceiling((length - (2 * ccWF)) / trSpacing + 1));
                    else
                        qtyT = quantity * trQuantity;

                    print("qtyL: " + qtyL);
                    print("qtyT: " + qtyT);

                    //3
                    double hl_L = 0;
                    double hl_T = 0;
                    for (int i = 0; i < cEF.parameters.rein_BEH_MB_dt.Rows.Count; i++)
                    {
                        if (lrHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[i][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[i][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccWF) + (2 * hl_L);
                    double LB_qtyT = (wfBaseU - 2 * ccWF * 0.4142135624) + 600 + ((wfBaseT - wfBaseU) * Math.Sqrt(2)) + (2 * hl_T); //Math.Tan(22.5) inaccurate
                    LB_qtyL /= 1000;
                    LB_qtyT /= 1000;

                    print("LB_qtyL: " + LB_qtyL);
                    print("LB_qtyT: " + LB_qtyT);

                    //4
                    double largestML = 0;
                    for (int i = 6; i >= 0; i--)
                    {
                        if (manufacturedLength[i])
                        {
                            switch (i)
                            {
                                case 0:
                                    largestML = 6; break;
                                case 1:
                                    largestML = 7.5; break;
                                case 2:
                                    largestML = 9.0; break;
                                case 3:
                                    largestML = 10.5; break;
                                case 4:
                                    largestML = 12.0; break;
                                case 5:
                                    largestML = 13.5; break;
                                case 6:
                                    largestML = 15.0; break;
                                default:
                                    largestML = 0; break;
                            }
                            break;
                        }
                    }
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double qtyCM = 0;
                    double Le = 0;
                    double Lw = 0;
                    double Lx = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    //Capture SL base on MPa selected by user
                    double MPa = 0;
                    double sl = 0;
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            MPa = 27.579;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            MPa = 24.1316;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            MPa = 17.236;
                        }
                        else
                        {
                            MPa = 13.789;
                        }
                    }
                    else
                    {
                        string selectedReadyMix = cEF.parameters.conc_CM_F_RM;
                        string stringMPa = selectedReadyMix.Substring(29, 4);
                        if (stringMPa.Contains("M"))
                            stringMPa = stringMPa.Substring(0, 2);
                        MPa = double.Parse(stringMPa, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    int k = 0;
                    foreach (string Fc in cEF.parameters.rein_LSL_TB_fc_list)
                    {
                        if (Fc.Equals(MPa.ToString()))
                            break;
                        k++;
                    }
                    for (int j = 0; j < cEF.parameters.rein_LSL_TB_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_LSL_TB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                        {
                            sl = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[j][k + 1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                    sl /= 1000;

                    for (int i = 0; i < 7; i++)
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (i * 1.5);
                        if (manufacturedLength[i])
                        {
                            if (LB_qtyL < largestML)
                            {
                                //Longitudinal Reinforcement
                                qtyP = Lm / LB_qtyL;
                                qtyM = qtyL / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                    Lw = 0;
                                else
                                    Lw = Lm - (LB_qtyL) * Math.Floor(qtyP);
                                totalWaste_L = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(LB_qtyL);
                                    chosenML_L.Add(qtyP);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(Lw);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }

                                print("============ Longitudinal ============");
                                print("Lm: " + Lm);
                                print("qtyL: " + qtyL);
                                print("LB_qtyL: " + LB_qtyL);
                                print("qtyP: " + qtyP);
                                print("qtyM: " + qtyM);
                                print("Le: " + Le);
                                print("Lw: " + Lw);
                                print("totalWaste_L: " + totalWaste_L);
                                print("previousTotalWaste_L: " + previousTotalWaste_L);
                            }
                            else
                            {
                                //Longitudinal Reinforcement
                                qtyM = LB_qtyL / (Lm - sl);
                                Lx = (qtyM - Math.Floor(qtyM)) * (Lm - sl);
                                Le = Lm - Lx;
                                totalWaste_L = Le * qtyL;
                                qtyCM = qtyL * Math.Ceiling(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(sl);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(qtyCM);
                                                chosenML_L.Add(Lx);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(sl);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(qtyCM);
                                            chosenML_L.Add(Lx);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(sl);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(qtyCM);
                                    chosenML_L.Add(Lx);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }

                                print("============ Longitudinal ============");
                                print("Lm: " + Lm);
                                print("qtyL: " + qtyL);
                                print("LB_qtyL: " + LB_qtyL);
                                print("SL: " + sl);
                                print("qtyM: " + qtyM);
                                print("Lx: " + Lx);
                                print("Le: " + Le);
                                print("qtyCM: " + qtyCM);
                                print("totalWaste_L: " + totalWaste_L);
                                print("previousTotalWaste_L: " + previousTotalWaste_L);
                            }
                            //Transverse Reinforcement
                            qtyP = Lm / LB_qtyT;
                            qtyM = qtyT / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                                Lw = 0;
                            else
                                Lw = Lm - (LB_qtyT) * Math.Floor(qtyP);
                            totalWaste_T = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                {
                                    if (chosenML_T.Any())
                                    {
                                        if (totalWaste_T < chosenML_T[7])
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_T.Clear();
                                chosenML_T.Add(Lm);
                                chosenML_T.Add(qtyT);
                                chosenML_T.Add(LB_qtyT);
                                chosenML_T.Add(qtyP);
                                chosenML_T.Add(qtyM);
                                chosenML_T.Add(Lw);
                                chosenML_T.Add(Le);
                                chosenML_T.Add(totalWaste_T);
                                chosenML_T.Add(trDiameter);
                            }

                            print("============ Transverse ============");
                            print("Lm: " + Lm);
                            print("qtyT: " + qtyT);
                            print("LB_qtyT: " + LB_qtyT);
                            print("qtyP: " + qtyP);
                            print("qtyM: " + qtyM);
                            print("Le: " + Le);
                            print("Lw: " + Lw);
                            print("totalWaste_T: " + totalWaste_T);
                            print("previousTotalWaste_T: " + previousTotalWaste_T);
                        }
                    }

                    //5 -> Costing of material -> in MainForm.cs function initializeView()
                    //6
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }
                    
                    //Longitudinal
                    for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(lrDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_L[0] * Math.Ceiling(chosenML_L[4]) * Wd;
                    chosenML_L.Add(weight);

                    print("Weight L: " + weight + " and WD: " + Wd + " and LM: " + chosenML_L[0] + " and QTYM: " + chosenML_L[4]);

                    //Transverse
                    for (int i = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[i][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[i][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * Math.Ceiling(chosenML_T[4]) * Wd;
                    chosenML_T.Add(weight);

                    //7 - 8 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.wallFootingReinforcements[wallFootingCount].Add(chosenML_L);
                    cEF.structuralMembers.wallFootingReinforcements[wallFootingCount].Add(chosenML_T);
                }
            }
            //Computation -- add formworks [FOOTING]
            recomputeFW_Footings(cEF);
            refreshSolutions(cEF);
        }        
        //Modify
        public void modifyFootingWorks(CostEstimationForm cEF, int structMemCount, int count, bool isFooting)
        {
            if (isFooting)
            {
                if (cEF.structuralMembers.footingsColumn[0][structMemCount][0].Equals("Isolated Footing"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccF = double.Parse(cEF.parameters.conc_CC_F, System.Globalization.CultureInfo.InvariantCulture);
                    for (int j = 0; j < 7; j++)
                        manufacturedLength[j] = cEF.parameters.rein_mfIsSelected[0, j];

                    //Variables from StructMem
                    double length, width, thickness, quantity, depth, lrDiameter, lrQuantity, lrHookType,
                       trDiameter, trQuantity, trHookType;

                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    width = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][11], System.Globalization.CultureInfo.InvariantCulture);

                    //Computation -- Earth Works
                    int i = 0;
                    int wfC = 0;
                    foreach(List<double> solution in cEF.structuralMembers.earthworkSolutions)
                    {
                        if (solution[0] == 2)
                            wfC++;

                        if (count == i - wfC && count <= i)
                            break;
                        i++;
                    }

                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[i][1] =
                        (length + formworkAllowance * 2) * (width + formworkAllowance * 2) * (depth + gravelBedding) * quantity;
                    
                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[i][2] =
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2)) * quantity;

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[i][3] =
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2) *
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity;

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[i][4] =
                        length * width * thickness * quantity;

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] = 
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * width * thickness) / 1000000000) * quantity;
                    }
                    //Computation -- Formworks **                                                            
                    cEF.structuralMembers.per_col[structMemCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;

                    //Computation -- Rebars
                    //1
                    double qtyL = quantity * lrQuantity;
                    double qtyT = quantity * trQuantity;

                    //2
                    double hl_L = 0;
                    double hl_T = 0;
                    for (int j = 0; j < cEF.parameters.rein_BEH_MB_dt.Rows.Count; j++)
                    {
                        if (lrHookType == 90.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 135.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180.0)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccF) + (2 * hl_L);
                    double LB_qtyT = width - (2 * ccF) + (2 * hl_T);
                    LB_qtyL /= 1000;
                    LB_qtyT /= 1000;
                    //3
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double Le = 0;
                    double Lw = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter ->  Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    for (int j = 0; j < 7; j++)
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (j * 1.5);
                        if (manufacturedLength[j])
                        {
                            //Longitudinal
                            qtyP = Lm / LB_qtyL;
                            qtyM = qtyL / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                            {
                                Lw = 0;
                            }
                            else
                            {
                                Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                            }
                            totalWaste_L = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                {
                                    if (chosenML_L.Any())
                                    {
                                        if (totalWaste_L < chosenML_L[7])
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_L.Clear();
                                        chosenML_L.Add(Lm);
                                        chosenML_L.Add(qtyL);
                                        chosenML_L.Add(LB_qtyL);
                                        chosenML_L.Add(qtyP);
                                        chosenML_L.Add(qtyM);
                                        chosenML_L.Add(Lw);
                                        chosenML_L.Add(Le);
                                        chosenML_L.Add(totalWaste_L);
                                        chosenML_L.Add(lrDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_L.Clear();
                                chosenML_L.Add(Lm);
                                chosenML_L.Add(qtyL);
                                chosenML_L.Add(LB_qtyL);
                                chosenML_L.Add(qtyP);
                                chosenML_L.Add(qtyM);
                                chosenML_L.Add(Lw);
                                chosenML_L.Add(Le);
                                chosenML_L.Add(totalWaste_L);
                                chosenML_L.Add(lrDiameter);
                            }

                            //Transverse
                            qtyP = Lm / LB_qtyT;
                            qtyM = qtyT / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                            {
                                Lw = 0;
                            }
                            else
                            {
                                Lw = Lm - LB_qtyT * Math.Floor(qtyP);
                            }
                            totalWaste_T = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                {
                                    if (chosenML_T.Any())
                                    {
                                        if (totalWaste_T < chosenML_T[7])
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_T.Clear();
                                chosenML_T.Add(Lm);
                                chosenML_T.Add(qtyT);
                                chosenML_T.Add(LB_qtyT);
                                chosenML_T.Add(qtyP);
                                chosenML_T.Add(qtyM);
                                chosenML_T.Add(Lw);
                                chosenML_T.Add(Le);
                                chosenML_T.Add(totalWaste_T);
                                chosenML_T.Add(trDiameter);
                            }
                        }
                    }
                    //4 -> Costing of material -> in MainForm.cs function initializeView()
                    //5
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }

                    //Longitudinal
                    for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(lrDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_L[0] * chosenML_L[4] * Wd;
                    chosenML_L.Add(weight);

                    //Transverse
                    for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * chosenML_T[4] * Wd;
                    chosenML_T.Add(weight);

                    //6 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.footingReinforcements[structMemCount].Clear();
                    cEF.structuralMembers.footingReinforcements[structMemCount].Add(chosenML_L);
                    cEF.structuralMembers.footingReinforcements[structMemCount].Add(chosenML_T);
                }
                else //Combined Footing
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccF = double.Parse(cEF.parameters.conc_CC_F, System.Globalization.CultureInfo.InvariantCulture);
                    for (int j = 0; j < 7; j++)
                        manufacturedLength[j] = cEF.parameters.rein_mfIsSelected[0, j];

                    double length, width, thickness, quantity, depth, lrDiameter, lrQuantity, lrSpacing, lrHookType,
                       trDiameter, trQuantity, trSpacing, trHookType, urDiameter, urQuantity, urSpacing, urHookType;

                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    width = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrSpacing = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][11], System.Globalization.CultureInfo.InvariantCulture);
                    trSpacing = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][12], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][13], System.Globalization.CultureInfo.InvariantCulture);
                    urDiameter = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][14], System.Globalization.CultureInfo.InvariantCulture);
                    urQuantity = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][15], System.Globalization.CultureInfo.InvariantCulture);
                    urSpacing = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][16], System.Globalization.CultureInfo.InvariantCulture);
                    urHookType = double.Parse(cEF.structuralMembers.footingsColumn[0][structMemCount][17], System.Globalization.CultureInfo.InvariantCulture);

                    //Computation -- Earth Works
                    int i = 0;
                    int wfC = 0;
                    foreach (List<double> solution in cEF.structuralMembers.earthworkSolutions)
                    {
                        if (solution[0] == 2)
                            wfC++;

                        if (count == i - wfC && count <= i)
                            break;
                        i++;
                    }

                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[i][1] =
                        (length + formworkAllowance * 2) * (width + formworkAllowance * 2) * (depth + gravelBedding) * quantity;

                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[i][2] =
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2)) * quantity;

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[i][3] =
                        ((length + formworkAllowance * 2) * (width + formworkAllowance * 2) *
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity;

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[i][4] =
                        length * width * thickness * quantity;

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else
                        {
                            double volume = length * width * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * width * thickness) / 1000000000) * quantity;
                    }
                    //Computation -- Formworks **
                    cEF.structuralMembers.per_col[structMemCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;

                    //Computation -- Rebars
                    //1
                    double qtyL = 0;
                    double qtyU = 0;
                    double qtyT = 0;
                    if (lrQuantity == 0)
                        qtyL = quantity * (Math.Ceiling((width - (2 * ccF)) / lrSpacing + 1));
                    else
                        qtyL = quantity * lrQuantity;
                    if (urQuantity == 0)
                        qtyU = quantity * (Math.Ceiling((width - (2 * ccF)) / urSpacing + 1));
                    else
                        qtyU = quantity * urQuantity;
                    if (trQuantity == 0)
                        qtyT = quantity * (Math.Ceiling((length - (2 * ccF)) / trSpacing + 1));
                    else
                        qtyT = quantity * trQuantity;

                    //2
                    double hl_L = 0;
                    double hl_U = 0;
                    double hl_T = 0;
                    for (int j = 0; j < cEF.parameters.rein_BEH_MB_dt.Rows.Count; j++)
                    {
                        if (lrHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (urHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(urDiameter.ToString()))
                            {
                                hl_U = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (urHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(urDiameter.ToString()))
                            {
                                hl_U = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (urHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(urDiameter.ToString()))
                            {
                                hl_U = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccF) + (2 * hl_L);
                    double LB_qtyU = length - (2 * ccF) + (2 * hl_U);
                    double LB_qtyT = width - (2 * ccF) + (2 * hl_T);
                    LB_qtyL /= 1000;
                    LB_qtyU /= 1000;
                    LB_qtyT /= 1000;

                    //3
                    double largestML = 0;
                    for (int j = 6; j >= 0; j--)
                    {
                        if (manufacturedLength[j])
                        {
                            switch (j)
                            {
                                case 0:
                                    largestML = 6; break;
                                case 1:
                                    largestML = 7.5; break;
                                case 2:
                                    largestML = 9.0; break;
                                case 3:
                                    largestML = 10.5; break;
                                case 4:
                                    largestML = 12.0; break;
                                case 5:
                                    largestML = 13.5; break;
                                case 6:
                                    largestML = 15.0; break;
                                default:
                                    largestML = 0; break;
                            }
                            break;
                        }
                    }
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double Le = 0;
                    double Lw = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_U = 0;
                    double previousTotalWaste_U = 0;

                    double totalWaste_LU = 0;
                    double previousTotalWaste_LU = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[U] -> LB of QTY[U] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_U = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    for (int j = 0; j < 7; j++)
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_U = totalWaste_U;
                        previousTotalWaste_LU = totalWaste_LU;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (j * 1.5);
                        if (manufacturedLength[j])
                        {
                            if ((LB_qtyL + LB_qtyT) <= largestML && lrDiameter == urDiameter && lrDiameter == trDiameter) //a)
                            {
                                if ((qtyL + qtyU) < qtyT) //Case 1
                                {
                                    //(Longitudinal + Upper)
                                    qtyP = Lm / (LB_qtyL + LB_qtyT);
                                    qtyM = (qtyL + qtyU) / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                                    }
                                    totalWaste_LU = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_LU != 0 || Double.IsNaN(previousTotalWaste_LU)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_LU < previousTotalWaste_LU || (Double.IsNaN(previousTotalWaste_LU) && !Double.IsNaN(totalWaste_LU)))
                                        {
                                            if (chosenML_L.Any())
                                            {
                                                if (totalWaste_LU < chosenML_L[7])
                                                {
                                                    chosenML_L.Clear();
                                                    chosenML_L.Add(Lm);
                                                    chosenML_L.Add(qtyL + qtyU);
                                                    chosenML_L.Add(LB_qtyL + LB_qtyT);
                                                    chosenML_L.Add(qtyP);
                                                    chosenML_L.Add(qtyM);
                                                    chosenML_L.Add(Lw);
                                                    chosenML_L.Add(Le);
                                                    chosenML_L.Add(totalWaste_LU);
                                                    chosenML_L.Add(lrDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL + qtyU);
                                                chosenML_L.Add(LB_qtyL + LB_qtyT);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_LU);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_LU == 0 && !Double.IsNaN(totalWaste_LU) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_L.Clear();
                                        chosenML_L.Add(Lm);
                                        chosenML_L.Add(qtyL + qtyU);
                                        chosenML_L.Add(LB_qtyL + LB_qtyT);
                                        chosenML_L.Add(qtyP);
                                        chosenML_L.Add(qtyM);
                                        chosenML_L.Add(Lw);
                                        chosenML_L.Add(Le);
                                        chosenML_L.Add(totalWaste_LU);
                                        chosenML_L.Add(lrDiameter);
                                    }

                                    //Transverse
                                    qtyP = Lm / LB_qtyT;
                                    qtyM = (qtyT - qtyL - qtyU) / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - (LB_qtyL + LB_qtyT) * Math.Floor(qtyP);
                                    }
                                    totalWaste_T = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                        {
                                            if (chosenML_T.Any())
                                            {
                                                if (totalWaste_T < chosenML_T[7])
                                                {
                                                    chosenML_T.Clear();
                                                    chosenML_T.Add(Lm);
                                                    chosenML_T.Add(qtyT - qtyL - qtyU);
                                                    chosenML_T.Add(LB_qtyT);
                                                    chosenML_T.Add(qtyP);
                                                    chosenML_T.Add(qtyM);
                                                    chosenML_T.Add(Lw);
                                                    chosenML_T.Add(Le);
                                                    chosenML_T.Add(totalWaste_T);
                                                    chosenML_T.Add(trDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_T.Clear();
                                                chosenML_T.Add(Lm);
                                                chosenML_T.Add(qtyT - qtyL - qtyU);
                                                chosenML_T.Add(LB_qtyT);
                                                chosenML_T.Add(qtyP);
                                                chosenML_T.Add(qtyM);
                                                chosenML_T.Add(Lw);
                                                chosenML_T.Add(Le);
                                                chosenML_T.Add(totalWaste_T);
                                                chosenML_T.Add(trDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT - qtyL - qtyU);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                                else if (qtyL + qtyU >= qtyT) //Case 2
                                {
                                    //(Longitudinal + Upper)
                                    qtyP = Lm / LB_qtyL;
                                    qtyM = (qtyL + qtyU - qtyT) / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                                    }
                                    totalWaste_LU = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_LU != 0 || Double.IsNaN(previousTotalWaste_LU)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_LU < previousTotalWaste_LU || (Double.IsNaN(previousTotalWaste_LU) && !Double.IsNaN(totalWaste_LU)))
                                        {
                                            if (chosenML_L.Any())
                                            {
                                                if (totalWaste_LU < chosenML_L[7])
                                                {
                                                    chosenML_L.Clear();
                                                    chosenML_L.Add(Lm);
                                                    chosenML_L.Add(qtyL + qtyU - qtyT);
                                                    chosenML_L.Add(LB_qtyL);
                                                    chosenML_L.Add(qtyP);
                                                    chosenML_L.Add(qtyM);
                                                    chosenML_L.Add(Lw);
                                                    chosenML_L.Add(Le);
                                                    chosenML_L.Add(totalWaste_LU);
                                                    chosenML_L.Add(lrDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL + qtyU - qtyT);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_LU);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_LU == 0 && !Double.IsNaN(totalWaste_LU) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_L.Clear();
                                        chosenML_L.Add(Lm);
                                        chosenML_L.Add(qtyL + qtyU - qtyT);
                                        chosenML_L.Add(LB_qtyL);
                                        chosenML_L.Add(qtyP);
                                        chosenML_L.Add(qtyM);
                                        chosenML_L.Add(Lw);
                                        chosenML_L.Add(Le);
                                        chosenML_L.Add(totalWaste_LU);
                                        chosenML_L.Add(lrDiameter);
                                    }

                                    //Transverse
                                    qtyP = Lm / (LB_qtyL + LB_qtyT);
                                    qtyM = qtyT / Math.Floor(qtyP);
                                    Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                    if (qtyP < 1)
                                    {
                                        Lw = 0;
                                    }
                                    else
                                    {
                                        Lw = Lm - (LB_qtyL + LB_qtyT) * Math.Floor(qtyP);
                                    }
                                    totalWaste_T = Le + Lw * Math.Floor(qtyM);
                                    if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                        {
                                            if (chosenML_T.Any())
                                            {
                                                if (totalWaste_T < chosenML_T[7])
                                                {
                                                    chosenML_T.Clear();
                                                    chosenML_T.Add(Lm);
                                                    chosenML_T.Add(qtyT);
                                                    chosenML_T.Add(LB_qtyL + LB_qtyT);
                                                    chosenML_T.Add(qtyP);
                                                    chosenML_T.Add(qtyM);
                                                    chosenML_T.Add(Lw);
                                                    chosenML_T.Add(Le);
                                                    chosenML_T.Add(totalWaste_T);
                                                    chosenML_T.Add(trDiameter);
                                                }
                                            }
                                            else
                                            {
                                                chosenML_T.Clear();
                                                chosenML_T.Add(Lm);
                                                chosenML_T.Add(qtyT);
                                                chosenML_T.Add(LB_qtyL + LB_qtyT);
                                                chosenML_T.Add(qtyP);
                                                chosenML_T.Add(qtyM);
                                                chosenML_T.Add(Lw);
                                                chosenML_T.Add(Le);
                                                chosenML_T.Add(totalWaste_T);
                                                chosenML_T.Add(trDiameter);
                                            }
                                        }
                                    }
                                    else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyL + LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (LB_qtyL + LB_qtyT > largestML || lrDiameter != urDiameter || lrDiameter != trDiameter) //b)
                            {
                                //(Longitudinal
                                qtyP = Lm / LB_qtyL;
                                qtyM = qtyL / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                {
                                    Lw = 0;
                                }
                                else
                                {
                                    Lw = Lm - LB_qtyL * Math.Floor(qtyP);
                                }
                                totalWaste_L = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(LB_qtyL);
                                    chosenML_L.Add(qtyP);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(Lw);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }

                                //Upper
                                qtyP = Lm / LB_qtyU;
                                qtyM = qtyU / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                {
                                    Lw = 0;
                                }
                                else
                                {
                                    Lw = Lm - LB_qtyU * Math.Floor(qtyP);
                                }
                                totalWaste_U = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_U != 0 || Double.IsNaN(previousTotalWaste_U)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_U < previousTotalWaste_U || (Double.IsNaN(previousTotalWaste_U) && !Double.IsNaN(totalWaste_U)))
                                    {
                                        if (chosenML_U.Any())
                                        {
                                            if (totalWaste_U < chosenML_U[7])
                                            {
                                                chosenML_U.Clear();
                                                chosenML_U.Add(Lm);
                                                chosenML_U.Add(qtyU);
                                                chosenML_U.Add(LB_qtyU);
                                                chosenML_U.Add(qtyP);
                                                chosenML_U.Add(qtyM);
                                                chosenML_U.Add(Lw);
                                                chosenML_U.Add(Le);
                                                chosenML_U.Add(totalWaste_U);
                                                chosenML_U.Add(urDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_U.Clear();
                                            chosenML_U.Add(Lm);
                                            chosenML_U.Add(qtyU);
                                            chosenML_U.Add(LB_qtyU);
                                            chosenML_U.Add(qtyP);
                                            chosenML_U.Add(qtyM);
                                            chosenML_U.Add(Lw);
                                            chosenML_U.Add(Le);
                                            chosenML_U.Add(totalWaste_U);
                                            chosenML_U.Add(urDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_U == 0 && !Double.IsNaN(totalWaste_U) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_U.Clear();
                                    chosenML_U.Add(Lm);
                                    chosenML_U.Add(qtyU);
                                    chosenML_U.Add(LB_qtyU);
                                    chosenML_U.Add(qtyP);
                                    chosenML_U.Add(qtyM);
                                    chosenML_U.Add(Lw);
                                    chosenML_U.Add(Le);
                                    chosenML_U.Add(totalWaste_U);
                                    chosenML_U.Add(urDiameter);
                                }

                                //Transverse
                                qtyP = Lm / LB_qtyT;
                                qtyM = qtyT / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                {
                                    Lw = 0;
                                }
                                else
                                {
                                    Lw = Lm - LB_qtyT * Math.Floor(qtyP);
                                }
                                totalWaste_T = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                    {
                                        if (chosenML_T.Any())
                                        {
                                            if (totalWaste_T < chosenML_T[7])
                                            {
                                                chosenML_T.Clear();
                                                chosenML_T.Add(Lm);
                                                chosenML_T.Add(qtyT);
                                                chosenML_T.Add(LB_qtyT);
                                                chosenML_T.Add(qtyP);
                                                chosenML_T.Add(qtyM);
                                                chosenML_T.Add(Lw);
                                                chosenML_T.Add(Le);
                                                chosenML_T.Add(totalWaste_T);
                                                chosenML_T.Add(trDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_T.Clear();
                                    chosenML_T.Add(Lm);
                                    chosenML_T.Add(qtyT);
                                    chosenML_T.Add(LB_qtyT);
                                    chosenML_T.Add(qtyP);
                                    chosenML_T.Add(qtyM);
                                    chosenML_T.Add(Lw);
                                    chosenML_T.Add(Le);
                                    chosenML_T.Add(totalWaste_T);
                                    chosenML_T.Add(trDiameter);
                                }
                            }
                        }
                    }
                    //4 -> Costing of material -> in MainForm.cs function initializeView()
                    //5
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }

                    //Longitudinal and Upper
                    //If upper is not empty, then it is case b which produces 3 chosen manufactured length
                    if (chosenML_U.Any())
                    {
                        //Longitudinal
                        for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                        {
                            if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(lrDiameter + "mm"))
                            {
                                Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        weight = chosenML_L[0] * chosenML_L[4] * Wd;
                        chosenML_L.Add(weight);

                        //Upper
                        for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                        {
                            if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(urDiameter + "mm"))
                            {
                                Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        weight = chosenML_U[0] * chosenML_U[4] * Wd;
                        chosenML_U.Add(weight);
                    }
                    else
                    {
                        //Longitudinal and Upper
                        for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                        {
                            if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(lrDiameter + "mm"))
                            {
                                Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        weight = chosenML_L[0] * chosenML_L[4] * Wd;
                        chosenML_L.Add(weight);
                    }

                    //Transverse
                    for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * chosenML_T[4] * Wd;
                    chosenML_T.Add(weight);

                    //6 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.footingReinforcements[structMemCount].Clear();
                    cEF.structuralMembers.footingReinforcements[structMemCount].Add(chosenML_L);
                    if (chosenML_U.Any())
                        cEF.structuralMembers.footingReinforcements[structMemCount].Add(chosenML_U);
                    cEF.structuralMembers.footingReinforcements[structMemCount].Add(chosenML_T);
                }
            }
            else // Wall Footing
            {
                if (cEF.structuralMembers.footingsWall[0][structMemCount][0].Equals("Rectangular"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccWF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccWF = double.Parse(cEF.parameters.conc_CC_SG, System.Globalization.CultureInfo.InvariantCulture);
                    for (int j = 0; j < 7; j++)
                        manufacturedLength[j] = cEF.parameters.rein_mfIsSelected[2, j];

                    //Variables from StructMem
                    double length, lengthF2F, wfBase, thickness, depth, quantity, lrDiameter, lrQuantity, lrSpacing, lrHookType,
                       trDiameter, trQuantity, trSpacing, trHookType;

                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    lengthF2F = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    wfBase = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    lrSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][11], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][12], System.Globalization.CultureInfo.InvariantCulture);
                    trSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][13], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][14], System.Globalization.CultureInfo.InvariantCulture);

                    //Computation -- Earth Works
                    int i = 0;
                    int fC = 0;
                    foreach (List<double> solution in cEF.structuralMembers.earthworkSolutions)
                    {
                        if (solution[0] == 1)
                            fC++;

                        if (count == i - fC && count <= i)
                            break;
                        i++;
                    }

                    cEF.structuralMembers.earthworkSolutions[i][1] =
                        (lengthF2F + formworkAllowance * 2) * (wfBase + formworkAllowance * 2) * (depth + gravelBedding) * quantity;

                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[i][2] = 
                        ((lengthF2F + formworkAllowance * 2) * (wfBase + formworkAllowance * 2)) * quantity;

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[i][3] =
                        ((length + formworkAllowance * 2) * (wfBase + formworkAllowance * 2) *
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity;

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[i][4] =
                        length * wfBase * thickness * quantity;

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else
                        {
                            double volume = length * wfBase * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * wfBase * thickness) / 1000000000) * quantity;
                    }


                    //Computation -- Rebars
                    //1 -> determine ccWF

                    //2
                    double qtyL = 0;
                    double qtyT = 0;
                    if (lrQuantity == 0)
                        qtyL = quantity * (Math.Ceiling((wfBase - (2 * ccWF)) / lrSpacing + 1));
                    else
                        qtyL = quantity * lrQuantity;
                    if (trQuantity == 0)
                        qtyT = quantity * (Math.Ceiling((length - (2 * ccWF)) / trSpacing + 1));
                    else
                        qtyT = quantity * trQuantity;

                    //3
                    double hl_L = 0;
                    double hl_T = 0;
                    for (int j = 0; j < cEF.parameters.rein_BEH_MB_dt.Rows.Count; j++)
                    {
                        if (lrHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccWF) + (2 * hl_L);
                    double LB_qtyT = wfBase - (2 * ccWF) + (2 * hl_T);
                    LB_qtyL /= 1000;
                    LB_qtyT /= 1000;

                    //4
                    double largestML = 0;
                    for (int j = 6; j >= 0; j--)
                    {
                        if (manufacturedLength[j])
                        {
                            switch (j)
                            {
                                case 0:
                                    largestML = 6; break;
                                case 1:
                                    largestML = 7.5; break;
                                case 2:
                                    largestML = 9.0; break;
                                case 3:
                                    largestML = 10.5; break;
                                case 4:
                                    largestML = 12.0; break;
                                case 5:
                                    largestML = 13.5; break;
                                case 6:
                                    largestML = 15.0; break;
                                default:
                                    largestML = 0; break;
                            }
                            break;
                        }
                    }
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double qtyCM = 0;
                    double Le = 0;
                    double Lw = 0;
                    double Lx = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    //Capture SL base on MPa selected by user
                    double MPa = 0;
                    double sl = 0;
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            MPa = 27.579;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            MPa = 24.1316;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            MPa = 17.236;
                        }
                        else
                        {
                            MPa = 13.789;
                        }
                    }
                    else
                    {
                        string selectedReadyMix = cEF.parameters.conc_CM_F_RM;
                        string stringMPa = selectedReadyMix.Substring(29, 4);
                        if (stringMPa.Contains("M"))
                            stringMPa = stringMPa.Substring(0, 2);
                        MPa = double.Parse(stringMPa, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    int k = 0;
                    foreach (string Fc in cEF.parameters.rein_LSL_TB_fc_list)
                    {
                        if (Fc.Equals(MPa.ToString()))
                            break;
                        k++;
                    }
                    for (int j = 0; j < cEF.parameters.rein_LSL_TB_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_LSL_TB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                        {
                            sl = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[j][k + 1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                    sl /= 1000;

                    for (int j = 0; j < 7; j++)
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (j * 1.5);
                        if (manufacturedLength[j])
                        {
                            if (LB_qtyL < largestML)
                            {
                                //Longitudinal Reinforcement
                                qtyP = Lm / LB_qtyL;
                                qtyM = qtyL / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                    Lw = 0;
                                else
                                    Lw = Lm - (LB_qtyL) * Math.Floor(qtyP);
                                totalWaste_L = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(LB_qtyL);
                                    chosenML_L.Add(qtyP);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(Lw);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }
                            }
                            else
                            {
                                //Longitudinal Reinforcement
                                qtyM = LB_qtyL / (Lm - sl);
                                Lx = (qtyM - Math.Floor(qtyM)) * (Lm - sl);
                                Le = Lm - Lx;
                                totalWaste_L = Le * qtyL;
                                qtyCM = qtyL * Math.Ceiling(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(sl);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(qtyCM);
                                                chosenML_L.Add(Lx);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(sl);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(qtyCM);
                                            chosenML_L.Add(Lx);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(sl);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(qtyCM);
                                    chosenML_L.Add(Lx);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }
                            }
                            //Transverse Reinforcement
                            qtyP = Lm / LB_qtyT;
                            qtyM = qtyT / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                                Lw = 0;
                            else
                                Lw = Lm - (LB_qtyT) * Math.Floor(qtyP);
                            totalWaste_T = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                {
                                    if (chosenML_T.Any())
                                    {
                                        if (totalWaste_T < chosenML_T[7])
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_T.Clear();
                                chosenML_T.Add(Lm);
                                chosenML_T.Add(qtyT);
                                chosenML_T.Add(LB_qtyT);
                                chosenML_T.Add(qtyP);
                                chosenML_T.Add(qtyM);
                                chosenML_T.Add(Lw);
                                chosenML_T.Add(Le);
                                chosenML_T.Add(totalWaste_T);
                                chosenML_T.Add(trDiameter);
                            }
                        }
                    }

                    //5 -> Costing of material -> in MainForm.cs function initializeView()
                    //6
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }

                    //Longitudinal
                    for (int j = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(lrDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_L[0] * Math.Ceiling(chosenML_L[4]) * Wd;
                    chosenML_L.Add(weight);

                    print("Weight L: " + weight + " and WD: " + Wd + " and LM: " + chosenML_L[0] + " and QTYM: " + chosenML_L[4]);

                    //Transverse
                    for (int j = 0; i < cEF.parameters.rein_W_dt.Rows.Count; i++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * Math.Ceiling(chosenML_T[4]) * Wd;
                    chosenML_T.Add(weight);

                    //7 - 8 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.wallFootingReinforcements[structMemCount].Clear();
                    cEF.structuralMembers.wallFootingReinforcements[structMemCount].Add(chosenML_L);
                    cEF.structuralMembers.wallFootingReinforcements[structMemCount].Add(chosenML_T);
                }
                else //Trapezoidal
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance, ccWF;
                    string concreteGrade;
                    bool[] manufacturedLength = new bool[7];

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;
                    ccWF = double.Parse(cEF.parameters.conc_CC_SG, System.Globalization.CultureInfo.InvariantCulture);
                    for (int j = 0; j < 7; j++)
                        manufacturedLength[j] = cEF.parameters.rein_mfIsSelected[2, j];

                    //Variables from StructMem
                    double length, lengthF2F, wfBaseT, wfBaseU, thickness, depth, quantity, lrDiameter, lrQuantity, lrSpacing, lrHookType,
                       trDiameter, trQuantity, trSpacing, trHookType;

                    //Init variables from StructMem
                    length = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][1], System.Globalization.CultureInfo.InvariantCulture);
                    lengthF2F = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][2], System.Globalization.CultureInfo.InvariantCulture);
                    wfBaseT = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][3], System.Globalization.CultureInfo.InvariantCulture);
                    wfBaseU = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][4], System.Globalization.CultureInfo.InvariantCulture);
                    thickness = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][5], System.Globalization.CultureInfo.InvariantCulture);
                    depth = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][6], System.Globalization.CultureInfo.InvariantCulture);
                    quantity = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][7], System.Globalization.CultureInfo.InvariantCulture);
                    lrDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][8], System.Globalization.CultureInfo.InvariantCulture);
                    lrQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][9], System.Globalization.CultureInfo.InvariantCulture);
                    lrSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][10], System.Globalization.CultureInfo.InvariantCulture);
                    lrHookType = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][11], System.Globalization.CultureInfo.InvariantCulture);
                    trDiameter = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][12], System.Globalization.CultureInfo.InvariantCulture);
                    trQuantity = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][13], System.Globalization.CultureInfo.InvariantCulture);
                    trSpacing = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][14], System.Globalization.CultureInfo.InvariantCulture);
                    trHookType = double.Parse(cEF.structuralMembers.footingsWall[0][structMemCount][15], System.Globalization.CultureInfo.InvariantCulture);

                    //Computation -- Earth Works
                    int i = 0;
                    int fC = 0;
                    foreach (List<double> solution in cEF.structuralMembers.earthworkSolutions)
                    {
                        if (solution[0] == 1)
                            fC++;

                        if (count == i - fC && count <= i)
                            break;
                        i++;
                    }

                    cEF.structuralMembers.earthworkSolutions[i][1] =
                        (lengthF2F + formworkAllowance * 2) * (wfBaseT + formworkAllowance * 2) * (depth + gravelBedding) * quantity;

                    //Grading and Compaction
                    cEF.structuralMembers.earthworkSolutions[i][2] = 
                        ((lengthF2F + formworkAllowance * 2) * (wfBaseT + formworkAllowance * 2)) * quantity;

                    //Gravel Bedding
                    cEF.structuralMembers.earthworkSolutions[i][3] = 
                        ((length + formworkAllowance * 2) * (wfBaseT + formworkAllowance * 2) *
                        (gravelBedding + (gravelBedding * compactionAllowance))) * quantity;

                    //Backfilling and Compaction
                    //Concreting
                    cEF.structuralMembers.earthworkSolutions[i][4] =
                        length * wfBaseT * thickness * quantity;

                    //Computation -- Concrete Works
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                        else
                        {
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][2] =
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][3] =
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                            cEF.structuralMembers.concreteWorkSolutionsF[i][4] = volume * quantity;
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * ((wfBaseT + wfBaseU) / 2) * thickness) / 1000000000) * quantity;
                    }
                    //Computation -- Formworks **                    
                    double c = Math.Sqrt(Math.Pow((((wfBaseT / 1000) - (wfBaseU / 1000)) / 2), 2) + Math.Pow((thickness / 1000), 2));                    
                    cEF.structuralMembers.per_wal[structMemCount] = ((((c * (length / 1000)) + 0.2) + ((((wfBaseT / 1000) + (wfBaseU / 1000)) / 2) * (thickness / 1000))) * 2) * quantity;

                    //Computation -- Rebars
                    //1 -> determine ccWF

                    //2
                    double qtyL = 0;
                    double qtyT = 0;
                    if (lrQuantity == 0)
                        qtyL = quantity * (Math.Ceiling((wfBaseT - (2 * ccWF)) / lrSpacing + 1));
                    else
                        qtyL = quantity * lrQuantity;
                    if (trQuantity == 0)
                        qtyT = quantity * (Math.Ceiling((length - (2 * ccWF)) / trSpacing + 1));
                    else
                        qtyT = quantity * trQuantity;

                    print("qtyL: " + qtyL);
                    print("qtyT: " + qtyT);

                    //3
                    double hl_L = 0;
                    double hl_T = 0;
                    for (int j = 0; j < cEF.parameters.rein_BEH_MB_dt.Rows.Count; j++)
                    {
                        if (lrHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (lrHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                            {
                                hl_L = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }

                        if (trHookType == 90)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 135)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                        else if (trHookType == 180)
                        {
                            if (cEF.parameters.rein_BEH_MB_dt.Rows[j][0].Equals(trDiameter.ToString()))
                            {
                                hl_T = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[j][3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    double LB_qtyL = length - (2 * ccWF) + (2 * hl_L);
                    double LB_qtyT = (wfBaseU - 2 * ccWF * 0.4142135624) + 600 + ((wfBaseT - wfBaseU) * Math.Sqrt(2)) + (2 * hl_T); //Math.Tan(22.5) inaccurate
                    LB_qtyL /= 1000;
                    LB_qtyT /= 1000;

                    print("LB_qtyL: " + LB_qtyL);
                    print("LB_qtyT: " + LB_qtyT);

                    //4
                    double largestML = 0;
                    for (int j = 6; j >= 0; j--)
                    {
                        if (manufacturedLength[j])
                        {
                            switch (j)
                            {
                                case 0:
                                    largestML = 6; break;
                                case 1:
                                    largestML = 7.5; break;
                                case 2:
                                    largestML = 9.0; break;
                                case 3:
                                    largestML = 10.5; break;
                                case 4:
                                    largestML = 12.0; break;
                                case 5:
                                    largestML = 13.5; break;
                                case 6:
                                    largestML = 15.0; break;
                                default:
                                    largestML = 0; break;
                            }
                            break;
                        }
                    }
                    //Temporary variables
                    double qtyP = 0;
                    double Lm = 0;
                    double qtyM = 0;
                    double qtyCM = 0;
                    double Le = 0;
                    double Lw = 0;
                    double Lx = 0;

                    double totalWaste_L = 0;
                    double previousTotalWaste_L = 0;

                    double totalWaste_T = 0;
                    double previousTotalWaste_T = 0;

                    //Lm -> QTY[L] -> LB of QTY[L] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_L = new List<double>();

                    //Lm -> QTY[T] -> LB of QTY[T] -> qtyP -> qtyM -> Lw -> Le -> Total Waste -> Diameter -> Weight
                    List<double> chosenML_T = new List<double>();

                    //Capture SL base on MPa selected by user
                    double MPa = 0;
                    double sl = 0;
                    if (cEF.parameters.conc_cmIsSelected[0])
                    {
                        if (concreteGrade.Equals("CLASS AA"))
                        {
                            MPa = 27.579;
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            MPa = 24.1316;
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            MPa = 17.236;
                        }
                        else
                        {
                            MPa = 13.789;
                        }
                    }
                    else
                    {
                        string selectedReadyMix = cEF.parameters.conc_CM_F_RM;
                        string stringMPa = selectedReadyMix.Substring(29, 4);
                        if (stringMPa.Contains("M"))
                            stringMPa = stringMPa.Substring(0, 2);
                        MPa = double.Parse(stringMPa, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    int k = 0;
                    foreach (string Fc in cEF.parameters.rein_LSL_TB_fc_list)
                    {
                        if (Fc.Equals(MPa.ToString()))
                            break;
                        k++;
                    }
                    for (int j = 0; j < cEF.parameters.rein_LSL_TB_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_LSL_TB_dt.Rows[j][0].Equals(lrDiameter.ToString()))
                        {
                            sl = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[j][k + 1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    }
                    sl /= 1000;

                    for (int j = 0; j < 7; j++)
                    {
                        previousTotalWaste_L = totalWaste_L;
                        previousTotalWaste_T = totalWaste_T;
                        Lm = 6 + (j * 1.5);
                        if (manufacturedLength[j])
                        {
                            if (LB_qtyL < largestML)
                            {
                                //Longitudinal Reinforcement
                                qtyP = Lm / LB_qtyL;
                                qtyM = qtyL / Math.Floor(qtyP);
                                Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                                if (qtyP < 1)
                                    Lw = 0;
                                else
                                    Lw = Lm - (LB_qtyL) * Math.Floor(qtyP);
                                totalWaste_L = Le + Lw * Math.Floor(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(LB_qtyL);
                                                chosenML_L.Add(qtyP);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(Lw);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(LB_qtyL);
                                            chosenML_L.Add(qtyP);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(Lw);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(LB_qtyL);
                                    chosenML_L.Add(qtyP);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(Lw);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }

                                print("============ Longitudinal ============");
                                print("Lm: " + Lm);
                                print("qtyL: " + qtyL);
                                print("LB_qtyL: " + LB_qtyL);
                                print("qtyP: " + qtyP);
                                print("qtyM: " + qtyM);
                                print("Le: " + Le);
                                print("Lw: " + Lw);
                                print("totalWaste_L: " + totalWaste_L);
                                print("previousTotalWaste_L: " + previousTotalWaste_L);
                            }
                            else
                            {
                                //Longitudinal Reinforcement
                                qtyM = LB_qtyL / (Lm - sl);
                                Lx = (qtyM - Math.Floor(qtyM)) * (Lm - sl);
                                Le = Lm - Lx;
                                totalWaste_L = Le * qtyL;
                                qtyCM = qtyL * Math.Ceiling(qtyM);
                                if ((previousTotalWaste_L != 0 || Double.IsNaN(previousTotalWaste_L)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    if (totalWaste_L < previousTotalWaste_L || (Double.IsNaN(previousTotalWaste_L) && !Double.IsNaN(totalWaste_L)))
                                    {
                                        if (chosenML_L.Any())
                                        {
                                            if (totalWaste_L < chosenML_L[7])
                                            {
                                                chosenML_L.Clear();
                                                chosenML_L.Add(Lm);
                                                chosenML_L.Add(qtyL);
                                                chosenML_L.Add(sl);
                                                chosenML_L.Add(qtyM);
                                                chosenML_L.Add(qtyCM);
                                                chosenML_L.Add(Lx);
                                                chosenML_L.Add(Le);
                                                chosenML_L.Add(totalWaste_L);
                                                chosenML_L.Add(lrDiameter);
                                            }
                                        }
                                        else
                                        {
                                            chosenML_L.Clear();
                                            chosenML_L.Add(Lm);
                                            chosenML_L.Add(qtyL);
                                            chosenML_L.Add(sl);
                                            chosenML_L.Add(qtyM);
                                            chosenML_L.Add(qtyCM);
                                            chosenML_L.Add(Lx);
                                            chosenML_L.Add(Le);
                                            chosenML_L.Add(totalWaste_L);
                                            chosenML_L.Add(lrDiameter);
                                        }
                                    }
                                }
                                else if (previousTotalWaste_L == 0 && !Double.IsNaN(totalWaste_L) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                                {
                                    chosenML_L.Clear();
                                    chosenML_L.Add(Lm);
                                    chosenML_L.Add(qtyL);
                                    chosenML_L.Add(sl);
                                    chosenML_L.Add(qtyM);
                                    chosenML_L.Add(qtyCM);
                                    chosenML_L.Add(Lx);
                                    chosenML_L.Add(Le);
                                    chosenML_L.Add(totalWaste_L);
                                    chosenML_L.Add(lrDiameter);
                                }

                                print("============ Longitudinal ============");
                                print("Lm: " + Lm);
                                print("qtyL: " + qtyL);
                                print("LB_qtyL: " + LB_qtyL);
                                print("SL: " + sl);
                                print("qtyM: " + qtyM);
                                print("Lx: " + Lx);
                                print("Le: " + Le);
                                print("qtyCM: " + qtyCM);
                                print("totalWaste_L: " + totalWaste_L);
                                print("previousTotalWaste_L: " + previousTotalWaste_L);
                            }
                            //Transverse Reinforcement
                            qtyP = Lm / LB_qtyT;
                            qtyM = qtyT / Math.Floor(qtyP);
                            Le = (Math.Ceiling(qtyM) - qtyM) * Lm;
                            if (qtyP < 1)
                                Lw = 0;
                            else
                                Lw = Lm - (LB_qtyT) * Math.Floor(qtyP);
                            totalWaste_T = Le + Lw * Math.Floor(qtyM);
                            if ((previousTotalWaste_T != 0 || Double.IsNaN(previousTotalWaste_T)) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                if (totalWaste_T < previousTotalWaste_T || (Double.IsNaN(previousTotalWaste_T) && !Double.IsNaN(totalWaste_T)))
                                {
                                    if (chosenML_T.Any())
                                    {
                                        if (totalWaste_T < chosenML_T[7])
                                        {
                                            chosenML_T.Clear();
                                            chosenML_T.Add(Lm);
                                            chosenML_T.Add(qtyT);
                                            chosenML_T.Add(LB_qtyT);
                                            chosenML_T.Add(qtyP);
                                            chosenML_T.Add(qtyM);
                                            chosenML_T.Add(Lw);
                                            chosenML_T.Add(Le);
                                            chosenML_T.Add(totalWaste_T);
                                            chosenML_T.Add(trDiameter);
                                        }
                                    }
                                    else
                                    {
                                        chosenML_T.Clear();
                                        chosenML_T.Add(Lm);
                                        chosenML_T.Add(qtyT);
                                        chosenML_T.Add(LB_qtyT);
                                        chosenML_T.Add(qtyP);
                                        chosenML_T.Add(qtyM);
                                        chosenML_T.Add(Lw);
                                        chosenML_T.Add(Le);
                                        chosenML_T.Add(totalWaste_T);
                                        chosenML_T.Add(trDiameter);
                                    }
                                }
                            }
                            else if (previousTotalWaste_T == 0 && !Double.IsNaN(totalWaste_T) && !Double.IsNaN(qtyM) && !Double.IsInfinity(qtyM))
                            {
                                chosenML_T.Clear();
                                chosenML_T.Add(Lm);
                                chosenML_T.Add(qtyT);
                                chosenML_T.Add(LB_qtyT);
                                chosenML_T.Add(qtyP);
                                chosenML_T.Add(qtyM);
                                chosenML_T.Add(Lw);
                                chosenML_T.Add(Le);
                                chosenML_T.Add(totalWaste_T);
                                chosenML_T.Add(trDiameter);
                            }

                            print("============ Transverse ============");
                            print("Lm: " + Lm);
                            print("qtyT: " + qtyT);
                            print("LB_qtyT: " + LB_qtyT);
                            print("qtyP: " + qtyP);
                            print("qtyM: " + qtyM);
                            print("Le: " + Le);
                            print("Lw: " + Lw);
                            print("totalWaste_T: " + totalWaste_T);
                            print("previousTotalWaste_T: " + previousTotalWaste_T);
                        }
                    }

                    //5 -> Costing of material -> in MainForm.cs function initializeView()
                    //6
                    //Temporary variables
                    double weight = 0;
                    double Wd = 0;

                    //If no chosen ml, computation is wrong and set proper list
                    if (!chosenML_L.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_L.Add(0);
                        }
                    }
                    if (!chosenML_T.Any())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            chosenML_T.Add(0);
                        }
                    }

                    //Longitudinal
                    for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(lrDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_L[0] * Math.Ceiling(chosenML_L[4]) * Wd;
                    chosenML_L.Add(weight);

                    print("Weight L: " + weight + " and WD: " + Wd + " and LM: " + chosenML_L[0] + " and QTYM: " + chosenML_L[4]);

                    //Transverse
                    for (int j = 0; j < cEF.parameters.rein_W_dt.Rows.Count; j++)
                    {
                        if (cEF.parameters.rein_W_dt.Rows[j][0].ToString().Equals(trDiameter + "mm"))
                        {
                            Wd = double.Parse(cEF.parameters.rein_W_dt.Rows[j][1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    weight = chosenML_T[0] * Math.Ceiling(chosenML_T[4]) * Wd;
                    chosenML_T.Add(weight);

                    //7 - 8 -> Costing of Labor -> in MainForm.cs function initializeView()
                    cEF.structuralMembers.wallFootingReinforcements[structMemCount].Clear();
                    cEF.structuralMembers.wallFootingReinforcements[structMemCount].Add(chosenML_L);
                    cEF.structuralMembers.wallFootingReinforcements[structMemCount].Add(chosenML_T);
                }
            }
            //Computation -- modify formworks [FOOTINGS]            
            recomputeFW_Footings(cEF);
            refreshSolutions(cEF);
        }
        public void recomputeFW_Footings(CostEstimationForm cEF)
        {
            try
            {
                double footingC = 0;
                double footingW = 0;
                double frame_Biggest = 0;
                int index_Biggest = 0;
                foreach (var x in cEF.structuralMembers.per_col)
                {
                    if (frame_Biggest < x)
                    {
                        frame_Biggest = x;
                        index_Biggest = cEF.structuralMembers.per_col.IndexOf(x);
                    }
                    footingC += x;
                }
                foreach (var x in cEF.structuralMembers.per_wal)
                {
                    footingW += x;
                }
                List<string> fW_dim = dimensionFilterer(cEF.parameters.form_SM_F_FL);
                double framework_L = Math.Round(((4 * (double.Parse(fW_dim[0]) * double.Parse(fW_dim[2]) * (((double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][1]) / 1000) + 0.1) * 3.28))) / 12), 2);
                double framework_W = Math.Round(((4 * (double.Parse(fW_dim[0]) * double.Parse(fW_dim[2]) * (((double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][2]) / 1000)) * 3.28))) / 12), 2);
                double framework_vertical1st = rounder(((double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][1]) / 1000) + 0.1) / 0.7 + 1) * 2;
                double framework_vertical2nd = rounder(((double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][2]) / 1000)) / 0.7 + 1) * 2;
                double framework_verTOTAL = framework_vertical2nd + framework_vertical1st;
                double multip;
                if (double.Parse(fW_dim[2]) == 2)
                {
                    multip = 0.1;
                }
                else
                {
                    multip = 0.15;
                }
                double framework_c = (double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][3]) / 1000) - (multip);
                double framework_bdft = Math.Round(((framework_verTOTAL * (double.Parse(fW_dim[0]) * double.Parse(fW_dim[2]) * (framework_c * 3.28))) / 12), 2);
                double total_BDFT = framework_L + framework_W + framework_bdft;
                double framework_Multi = Math.Round(total_BDFT / ((((double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][1]) / 1000) + 0.1) * (double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][3]) / 1000) * 2) + ((double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][2]) / 1000) * (double.Parse(cEF.structuralMembers.footingsColumn[0][index_Biggest][3]) / 1000) * 2)), 2);
                double framework_TOTALBOARD = (framework_Multi * footingC) * (1 / double.Parse(cEF.parameters.form_F_NU));
                double framework_WOOD = rounder((framework_TOTALBOARD * 12) / (double.Parse(fW_dim[0]) * double.Parse(fW_dim[2]) * double.Parse(fW_dim[4])));
                double ply_col = rounder((footingC / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                double ply_wal = rounder((footingW / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                List<double> passer = new List<double>();
                passer.Add(ply_col);
                passer.Add(framework_WOOD);
                passer.Add(ply_wal);
                cEF.structuralMembers.footings_comps = passer;
                refreshSolutions(cEF);
            }
            catch
            {
                print("Null catcher footings");
            }
            
        }
        //Footing Computation Functions  -- END

        //Column Computation Functions -- START
        public void AddColumnWorks(CostEstimationForm cEF, int floorCount, int columnCount)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.concreteWorkSolutionsC[floorCount].Add(newList);
            columnWorks(cEF, floorCount, columnCount);
        }

        //Add 
        public void columnWorks(CostEstimationForm cEF, int floorCount, int columnCount)
        {
            //Variables from Parameters
            double columnExposedEarth, columnExposedWeather;
            string concreteGrade;

            //Init variables from Parameters
            concreteGrade = cEF.parameters.conc_CM_C_CG;
            columnExposedEarth = double.Parse(cEF.parameters.conc_CC_CEE, System.Globalization.CultureInfo.InvariantCulture);
            columnExposedWeather = double.Parse(cEF.parameters.conc_CC_CEW, System.Globalization.CultureInfo.InvariantCulture);

            //Variables from StructMem
            double baseC, depth, height, clearHeight, quantity, slabElevation, footingThickness, footingDepth;
            string connectionF, connectionS;

            //Init variables from StructMem
            baseC = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][1], System.Globalization.CultureInfo.InvariantCulture);
            depth = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][2], System.Globalization.CultureInfo.InvariantCulture);
            height = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][3], System.Globalization.CultureInfo.InvariantCulture);
            clearHeight = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][4], System.Globalization.CultureInfo.InvariantCulture);
            quantity = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][5], System.Globalization.CultureInfo.InvariantCulture);

            connectionF = cEF.structuralMembers.column[floorCount][columnCount][6];
            connectionS = cEF.structuralMembers.column[floorCount][columnCount][7];

            footingThickness = footingDepth = 0;
            int j = 0;
            foreach (string name in cEF.structuralMembers.footingColumnNames)
            {
                if (name.Equals(connectionF))
                {
                    footingThickness = double.Parse(cEF.structuralMembers.footingsColumn[0][j][3], System.Globalization.CultureInfo.InvariantCulture);
                    footingDepth = double.Parse(cEF.structuralMembers.footingsColumn[0][j][5], System.Globalization.CultureInfo.InvariantCulture);
                }
                j++;
            }

            slabElevation = 0;
            j = 0;
            foreach (string name in cEF.structuralMembers.slabNames[0])
            {
                if (name.Equals(connectionS))
                {
                    slabElevation = double.Parse(cEF.structuralMembers.slab[0][j][3], System.Globalization.CultureInfo.InvariantCulture);
                }
                j++;
            }
            
            //Computation -- Concrete Works
            if (cEF.structuralMembers.column[floorCount][columnCount][0].Equals("Ground"))
            {
                if (cEF.parameters.conc_cmIsSelected[1])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) * 
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) *
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) *
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                    else
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) *
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                }
                else
                {
                    double volume1 = (footingDepth + slabElevation - footingThickness) *
                                            (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                            (depth + 2 * (columnExposedEarth - columnExposedWeather));
                    double volume2 = baseC * depth * (height - slabElevation);
                    double volume = (volume1 + volume2) * quantity;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                }
                
            } 
            else
            {
                if (cEF.parameters.conc_cmIsSelected[1])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = baseC * depth * height * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = baseC * depth * height * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = baseC * depth * height * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                    else
                    {
                        double volume = baseC * depth * height * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(volume);
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                        ((baseC * depth * height) / 1000000000) * quantity);
                }
            }
            //Computation -- add formwork [COLUMN]
            recomputeFW_Column(cEF);
            refreshSolutions(cEF);
        }        

        //Modify
        public void ModifyColumnWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            //Variables from Parameters
            double columnExposedEarth, columnExposedWeather;
            string concreteGrade;

            //Init variables from Parameters
            concreteGrade = cEF.parameters.conc_CM_C_CG;
            columnExposedEarth = double.Parse(cEF.parameters.conc_CC_CEE, System.Globalization.CultureInfo.InvariantCulture);
            columnExposedWeather = double.Parse(cEF.parameters.conc_CC_CEW, System.Globalization.CultureInfo.InvariantCulture);

            //Variables from StructMem
            double baseC, depth, height, clearHeight, quantity, slabElevation, footingThickness, footingDepth;
            string connectionF, connectionS;

            //Init variables from StructMem
            baseC = double.Parse(cEF.structuralMembers.column[floorCount][index][1], System.Globalization.CultureInfo.InvariantCulture);
            depth = double.Parse(cEF.structuralMembers.column[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
            height = double.Parse(cEF.structuralMembers.column[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
            clearHeight = double.Parse(cEF.structuralMembers.column[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);
            quantity = double.Parse(cEF.structuralMembers.column[floorCount][index][5], System.Globalization.CultureInfo.InvariantCulture);

            connectionF = cEF.structuralMembers.column[floorCount][index][6];
            connectionS = cEF.structuralMembers.column[floorCount][index][7];

            footingThickness = footingDepth = 0;
            int j = 0;
            foreach (string name in cEF.structuralMembers.footingColumnNames)
            {
                if (name.Equals(connectionF))
                {
                    footingThickness = double.Parse(cEF.structuralMembers.footingsColumn[0][j][3], System.Globalization.CultureInfo.InvariantCulture);
                    footingDepth = double.Parse(cEF.structuralMembers.footingsColumn[0][j][5], System.Globalization.CultureInfo.InvariantCulture);
                }
                j++;
            }

            slabElevation = 0;
            j = 0;
            foreach (string name in cEF.structuralMembers.slabNames[0])
            {
                if (name.Equals(connectionS))
                {
                    slabElevation = double.Parse(cEF.structuralMembers.slab[0][j][3], System.Globalization.CultureInfo.InvariantCulture);
                }
                j++;
            }

            //Computation -- Concrete Works
            if (cEF.structuralMembers.column[floorCount][index][0].Equals("Ground"))
            {
                if (cEF.parameters.conc_cmIsSelected[1])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) *
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[0][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) *
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[0][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) *
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[0][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                    else
                    {
                        double volume1 = (footingDepth + slabElevation - footingThickness) *
                                         (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                         (depth + 2 * (columnExposedEarth - columnExposedWeather));
                        double volume2 = baseC * depth * (height - slabElevation);
                        double volume = (volume1 + volume2) * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[0][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                }
                else
                {
                    double volume1 = (footingDepth + slabElevation - footingThickness) *
                                            (baseC + 2 * (columnExposedEarth - columnExposedWeather)) *
                                            (depth + 2 * (columnExposedEarth - columnExposedWeather));
                    double volume2 = baseC * depth * (height - slabElevation);
                    double volume = (volume1 + volume2) * quantity;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] = volume;
                }
            }
            else // Upper
            {
                if (cEF.parameters.conc_cmIsSelected[1])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = baseC * depth * height * quantity; ;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[0][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = baseC * depth * height * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[1][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[1][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[1][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = baseC * depth * height * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[2][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[2][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[2][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                    else
                    {
                        double volume = baseC * depth * height * quantity;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[3][0];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[3][1];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[3][2];
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][3] = volume;
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                        ((baseC * depth * height) / 1000000000) * quantity;
                }
            }
            //Computation - modify formworks [COLUMN]
            recomputeFW_Column(cEF);
            refreshSolutions(cEF);
        }

        public void recomputeFW_Column(CostEstimationForm cEF)
        {
            try
            {
                double column_area = 0;
                double col_bdftH = 0;
                double col_bdftV = 0;
                double col_bdftD = 0;
                List<double> holder = new List<double>();
                List<double> wood_holder = new List<double>();
                List<double> scaf_holder = new List<double>();
                List<double> scaf_holder2 = new List<double>();
                List<double> scaf_holder3 = new List<double>();
                List<double> holder_post = new List<double>();
                List<string> scaf_dim = dimensionFilterer(cEF.parameters.form_SM_B_VS);//0 2 4
                List<string> scaf_dimHORI = dimensionFilterer(cEF.parameters.form_SM_B_HB);//0 2 4
                List<string> scaf_dimDIA = dimensionFilterer(cEF.parameters.form_SM_B_DB);//0 2 4 CHANGE THIS LATER WHEN UI CHANGE
                List<string> post_holder = dimensionFilterer(cEF.parameters.form_SM_B_FL);//0 2 4                
                double[] vertical_col = { 4.70, 7.00, 9.35 };
                double[] horizontal_col = { 21.00, 31.67, 42.25 };
                double[] dia_col = { 11.70, 17.50, 23.35 };
                double[] post_perPLY = { 20.33, 30.50 };
                int scaf_indexer;
                int scaf_HorINDEXER;
                int scaf_diaINDEXER;
                int dimIndexer;

                if (post_holder[2] == "2")//POST INDEXER
                {
                    dimIndexer = 0;
                }
                else
                {
                    dimIndexer = 1;
                }

                if (scaf_dim[2] == "2")//SCAF VERT INDEXER
                {
                    scaf_indexer = 0;
                }
                else if (scaf_dim[2] == "3")
                {
                    scaf_indexer = 1;
                }
                else
                {
                    scaf_indexer = 2;
                }

                if (scaf_dimHORI[2] == "2")//SCAF HORI INDEXER
                {
                    scaf_HorINDEXER = 0;
                }
                else if (scaf_dimHORI[2] == "3")
                {
                    scaf_HorINDEXER = 1;
                }
                else
                {
                    scaf_HorINDEXER = 2;
                }

                if (scaf_dimDIA[2] == "2")//SCAF DIA INDEXER
                {
                    scaf_diaINDEXER = 0;
                }
                else if (scaf_dimDIA[2] == "3")
                {
                    scaf_diaINDEXER = 1;
                }
                else
                {
                    scaf_diaINDEXER = 2;
                }

                for (int i = 0; i < cEF.structuralMembers.column.Count; i++)//area per floor
                {
                    for (int j = 0; j < cEF.structuralMembers.column[i].Count; j++)
                    {
                        column_area += (2 * ((double.Parse(cEF.structuralMembers.column[i][j][1]) / 1000) + (double.Parse(cEF.structuralMembers.column[i][j][2]) / 1000)) + 0.2) * (double.Parse(cEF.structuralMembers.column[i][j][3]) / 1000) * (double.Parse(cEF.structuralMembers.column[i][j][5]));
                        col_bdftV += ((double.Parse(cEF.structuralMembers.column[i][j][3]) / 1000) * (double.Parse(cEF.structuralMembers.column[i][j][5]))) * vertical_col[scaf_indexer];
                        col_bdftH += ((double.Parse(cEF.structuralMembers.column[i][j][3]) / 1000) * (double.Parse(cEF.structuralMembers.column[i][j][5]))) * horizontal_col[scaf_HorINDEXER];
                        col_bdftD += ((double.Parse(cEF.structuralMembers.column[i][j][3]) / 1000) * (double.Parse(cEF.structuralMembers.column[i][j][5]))) * dia_col[scaf_diaINDEXER];
                    }
                    scaf_holder.Add(rounder(((col_bdftV * 12) / (double.Parse(scaf_dim[0]) * double.Parse(scaf_dim[2]) * double.Parse(scaf_dim[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU))));
                    scaf_holder2.Add(rounder(((col_bdftH * 12) / (double.Parse(scaf_dimHORI[0]) * double.Parse(scaf_dimHORI[2]) * double.Parse(scaf_dimHORI[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU))));
                    scaf_holder3.Add(rounder(((col_bdftD * 12) / (double.Parse(scaf_dimDIA[0]) * double.Parse(scaf_dimDIA[2]) * double.Parse(scaf_dimDIA[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU))));
                    holder.Add(column_area);
                    col_bdftH = 0;
                    col_bdftV = 0;
                    col_bdftD = 0;
                    column_area = 0;
                }


                cEF.structuralMembers.col_scafV = scaf_holder;
                cEF.structuralMembers.col_scafH = scaf_holder2;
                cEF.structuralMembers.col_scafD = scaf_holder3;
                cEF.structuralMembers.col_area = holder;
                foreach (var a in cEF.structuralMembers.col_area)//woods per floor
                {
                    wood_holder.Add(rounder(((a / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU)))));
                }
                cEF.structuralMembers.col_woods = wood_holder;
                foreach (var wood in cEF.structuralMembers.col_woods)//Post
                {
                    holder_post.Add(rounder(((wood * post_perPLY[dimIndexer]) * 12) / (double.Parse(post_holder[0]) * double.Parse(post_holder[2]) * double.Parse(post_holder[4]))));
                }
                cEF.structuralMembers.col_post = holder_post;
            }
            catch (Exception ex)
            {
                print("Column: " + ex);
            }

            //Rebar
            List<List<List<string>>> fCS = new List<List<List<string>>>();
            try
            {
                for (int i = 0; i < cEF.structuralMembers.footingsColumn.Count; i++)
                {
                    List<List<string>> fcs1 = new List<List<string>>();
                    for (int j = 0; j < cEF.structuralMembers.footingsColumn[i].Count; j++)
                    {
                        List<string> fcs2 = new List<string>();
                        string name = cEF.structuralMembers.footingColumnNames[j];
                        string depth = "";
                        string longD = "";
                        string transD = "";
                        if (cEF.structuralMembers.footingsColumn[i][j][0] == "Isolated Footing")
                        {
                            depth = cEF.structuralMembers.footingsColumn[i][j][5];
                            longD = cEF.structuralMembers.footingsColumn[i][j][6];
                            transD = cEF.structuralMembers.footingsColumn[i][j][9];
                        }
                        else
                        {
                            depth = cEF.structuralMembers.footingsColumn[i][j][5];
                            longD = cEF.structuralMembers.footingsColumn[i][j][10];
                            transD = cEF.structuralMembers.footingsColumn[i][j][14];
                        }
                        fcs2.Add(name);
                        fcs2.Add(depth);
                        fcs2.Add(longD);
                        fcs2.Add(transD);
                        fcs1.Add(fcs2);
                    }
                    fCS.Add(fcs1);
                }
            }
            catch(Exception ex)
            {
                print("Column collection data rebar: " + ex);
            }                        
            try
            {
                List<List<List<string>>> columnMain = new List<List<List<string>>>();
                double colmain_weight = 0;
                for (int i = 0; i < cEF.structuralMembers.column.Count; i++)
                {
                    List<List<string>> col_holder = new List<List<string>>();
                    for (int j = 0; j < cEF.structuralMembers.column[i].Count; j++)
                    {
                        string connection = "";
                        double height = 0;
                        double diameter = 0;
                        double qtyCOL = 0;
                        double qtyDIA = 0;
                        string type = "";
                        string level = cEF.structuralMembers.column[i][j][0];
                        if (level == "Ground")
                        {
                            connection = cEF.structuralMembers.column[i][j][6];
                            height = double.Parse(cEF.structuralMembers.column[i][j][3]);
                            diameter = double.Parse(cEF.structuralMembers.column[i][j][8]);
                            qtyCOL = double.Parse(cEF.structuralMembers.column[i][j][5]);
                            qtyDIA = double.Parse(cEF.structuralMembers.column[i][j][9]);
                            type = cEF.structuralMembers.column[i][j][10];
                        }
                        else
                        {
                            height = double.Parse(cEF.structuralMembers.column[i][j][3]);
                            diameter = double.Parse(cEF.structuralMembers.column[i][j][6]);
                            qtyCOL = double.Parse(cEF.structuralMembers.column[i][j][5]);
                            qtyDIA = double.Parse(cEF.structuralMembers.column[i][j][7]);
                            type = cEF.structuralMembers.column[i][j][8];
                        }

                        double sl = 0;
                        if (type == "Lapped Splice" || type == "Welded Splice (Lap)")
                        {
                            string mix = spliceMixGetter(cEF.parameters.conc_CM_C_RM);
                            if (cEF.parameters.conc_cmIsSelected[1])
                            {
                                if (cEF.parameters.conc_CM_C_CG.Equals("CLASS AA"))
                                {
                                    mix = "27.6";
                                }
                                else if (cEF.parameters.conc_CM_C_CG.Equals("CLASS A"))
                                {
                                    mix = "24.1";
                                }
                                else if (cEF.parameters.conc_CM_C_CG.Equals("CLASS B"))
                                {
                                    mix = "17.2";
                                }
                                else
                                {
                                    mix = "13.8";
                                }                                                                                           
                            }                                                        
                            int index = cEF.parameters.rein_LSL_CB_fc_list.IndexOf(mix);
                            if (index >= 0)
                            {
                                for (int r = 0; r < cEF.parameters.rein_LSL_CB_dt.Rows.Count; r++)
                                {
                                    if ((cEF.parameters.rein_LSL_CB_dt.Rows[r][0]).ToString() == diameter.ToString())
                                    {
                                        sl = double.Parse(cEF.parameters.rein_LSL_CB_dt.Rows[r][index + 1].ToString());
                                        break;
                                    }
                                }
                            }
                            print(mix + " COL MIX");
                        }

                        double hl = 0;
                        for (int r = 0; r < cEF.parameters.rein_BEH_MB_dt.Rows.Count; r++)
                        {
                            if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == diameter.ToString())
                            {
                                hl = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][1].ToString());
                                break;
                            }
                        }

                        double ccs = double.Parse(cEF.parameters.conc_CC_F);

                        double lb = 0;
                        if (level == "Ground")
                        {
                            // connection_data  = name0 - depth1 - longD2 - transD3
                            List<string> connection_data = new List<string>();
                            bool fc_found = false;
                            for (int ft = 0; ft < fCS.Count; ft++)
                            {
                                for (int o = 0; o < fCS[ft].Count; o++)
                                {
                                    if (fCS[ft][o][0] == connection)
                                    {
                                        fc_found = true;
                                        connection_data = fCS[ft][o];
                                        break;
                                    }
                                }
                            }
                            if (!fc_found)
                            {
                                connection_data.Add("0");
                                connection_data.Add("0");
                                connection_data.Add("0");
                                connection_data.Add("0");
                            }
                            lb = (height + double.Parse(connection_data[1]) + (0.5 * sl) + hl - double.Parse(connection_data[2]) - double.Parse(connection_data[3]) - ccs) / 1000;
                        }
                        else
                        {
                            lb = (height + (0.5 * sl)) / 1000;
                        }
                        List<string> best_colm = columnMainRebarHelper(cEF, lb, qtyCOL, qtyDIA);
                        best_colm.Add(diameter.ToString());
                        string diaW = diameter + "mm";
                        double colkgm = 0;
                        for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
                        {
                            if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == diaW)
                            {
                                colkgm = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                                break;
                            }
                        }
                        colmain_weight += (colkgm * double.Parse(best_colm[0]) * double.Parse(best_colm[1])) * double.Parse(cEF.floors[i].getValues()[0]);
                        col_holder.Add(best_colm);
                    }
                    columnMain.Add(col_holder);
                }
                cEF.structuralMembers.Column_mainRebar = columnMain;
                cEF.structuralMembers.totalweightkgm_Colmain = colmain_weight;
            }
            catch (Exception ex)
            {
                print("Column main: " + ex);
            }

            //LATERAL TIES
            try
            {
                List<List<List<List<string>>>> masterlats = new List<List<List<List<string>>>>();
                for (int i = 0; i < cEF.structuralMembers.column.Count; i++)
                {
                    List<List<List<string>>> lats = new List<List<List<string>>>();
                    for (int j = 0; j < cEF.structuralMembers.column[i].Count; j++)
                    {
                        string connection = "";
                        double height = 0;
                        double diameter = 0;
                        double qtyCOL = 0;
                        double qtyDIA = 0;
                        string type = "";
                        double ccs = double.Parse(cEF.parameters.conc_CC_F);
                        double qtyte1 = 0;
                        double qtytatrest = 0;
                        double qtytq = 0;

                        string level = cEF.structuralMembers.column[i][j][0];
                        if (level == "Ground")
                        {
                            connection = cEF.structuralMembers.column[i][j][6];
                            height = double.Parse(cEF.structuralMembers.column[i][j][3]);
                            diameter = double.Parse(cEF.structuralMembers.column[i][j][8]);
                            qtyCOL = double.Parse(cEF.structuralMembers.column[i][j][5]);
                            qtyDIA = double.Parse(cEF.structuralMembers.column[i][j][9]);
                            type = cEF.structuralMembers.column[i][j][10];

                            //
                            double left = double.Parse(cEF.structuralMembers.column[i][j][15]);
                            double right = double.Parse(cEF.structuralMembers.column[i][j][16]);
                            double atrest = double.Parse(cEF.structuralMembers.column[i][j][14]);
                            double clear_h = double.Parse(cEF.structuralMembers.column[i][j][4]);
                            double srest = double.Parse(cEF.structuralMembers.column[i][j][19]);
                            double stj = double.Parse(cEF.structuralMembers.column[i][j][12]);
                            List<string> connection_data = new List<string>();
                            bool fc_found = false;
                            for (int ft = 0; ft < fCS.Count; ft++)
                            {
                                for (int o = 0; o < fCS[ft].Count; o++)
                                {
                                    if (fCS[ft][o][0] == connection)
                                    {
                                        fc_found = true;
                                        connection_data = fCS[ft][o];
                                        break;
                                    }
                                }
                            }
                            if (!fc_found)
                            {
                                connection_data.Add("0");//name
                                connection_data.Add("0");//depth
                                connection_data.Add("0");//longd
                                connection_data.Add("0");//transd                                
                            }
                            double summation = 0;
                            double qtySUM = 0;
                            for (int o = 0; o < cEF.structuralMembers.columnSpacing[i][j].Count;)
                            {
                                qtySUM += double.Parse(cEF.structuralMembers.columnSpacing[i][j][o]);
                                summation += double.Parse(cEF.structuralMembers.columnSpacing[i][j][o]) * double.Parse(cEF.structuralMembers.columnSpacing[i][j][o + 1]);
                                o += 2;
                            }
                            if (right != 0 && atrest != 0)
                            {
                                qtyte1 = rounder((double.Parse(connection_data[1]) - (left * right) - ccs - double.Parse(connection_data[2]) - double.Parse(connection_data[3]) - diameter) / atrest);
                                qtytatrest = rounder(((clear_h + double.Parse(connection_data[1]) - ((qtyte1 * atrest) + (left * right)) - summation - ccs - double.Parse(connection_data[2]) - double.Parse(connection_data[3]) - diameter) / srest) + 1);
                                qtytq = rounder(((height - clear_h) / stj) - 1);
                            }
                            else
                            {
                                qtytatrest = rounder(((clear_h - 2 * (summation) - ccs - double.Parse(connection_data[2]) - double.Parse(connection_data[3]) - diameter) / srest) + 1);
                                qtytq = rounder(((height - clear_h) / stj) - 1);
                            }
                            double ccc = double.Parse(cEF.parameters.conc_CC_CEW);
                            double lat_d = double.Parse(cEF.structuralMembers.column[i][j][17]);
                            double hl = 0;
                            double hl2 = 0;
                            double hl3 = 0;
                            for (int r = 0; r < cEF.parameters.rein_BEH_ST_dt.Rows.Count; r++)
                            {
                                if ((cEF.parameters.rein_BEH_ST_dt.Rows[r][0]).ToString() == lat_d.ToString())
                                {
                                    hl = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][2].ToString());
                                    hl2 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][1].ToString());
                                    hl3 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][3].ToString());
                                    break;
                                }
                            }
                            double rl = 0;
                            if (lat_d >= 6 && lat_d <= 16)
                            {
                                rl = 2 * lat_d;
                            }
                            else if (lat_d >= 20 && lat_d <= 56)
                            {
                                rl = 2.5 * lat_d;
                            }
                            List<double> produced_lb = new List<double>();
                            string config = cEF.structuralMembers.column[i][j][18];
                            double b = double.Parse(cEF.structuralMembers.column[i][j][1]);
                            double d = double.Parse(cEF.structuralMembers.column[i][j][2]);
                            double red = 0;
                            double green = 0;
                            if (config == "Lateral Ties Config 1")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1));
                            }
                            else if (config == "Lateral Ties Config 2")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = 4 * (Math.Sqrt(Math.Pow(((b / 2) - ccc), 2) + Math.Pow(((d / 2) - ccc), 2))) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                            }
                            else if (config == "Lateral Ties Config 3")
                            {
                                red = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][0]);
                                green = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][1]);
                                double blue = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][2]);
                                double orange = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][3]);
                                double sb = (b - 2 * (ccc + lat_d) - diameter * (blue + 2 * orange)) / (blue + 2 * orange - 1);
                                double sd = (d - 2 * (ccc + lat_d) - diameter * (red + 2 * (green))) / (red + 2 * (green) - 1);
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = 2 * (d) + 2 * ((diameter * blue) + sb * (blue - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb3 = 2 * (b) + 2 * ((diameter * red) + sd * (red - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);

                            }
                            else if (config == "Lateral Ties Config 4")
                            {
                                red = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][0]);
                                green = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][1]);
                                double blue = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][2]);
                                double orange = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][3]);
                                double sb = (b - 2 * (ccc + lat_d) - diameter * (blue + 2 * orange)) / (blue + 2 * orange - 1);
                                double sd = (d - 2 * (ccc + lat_d) - diameter * (red + 2 * (green))) / (red + 2 * (green) - 1);
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = 4 * (Math.Sqrt(Math.Pow(((b / 2) - ccc), 2) + Math.Pow(((d / 2) - ccc), 2))) + 2 * (hl) - 3 * (rl);
                                double lb3 = 2 * (d) + 2 * ((diameter * blue) + sb * (blue - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb4 = 2 * (b) + 2 * ((diameter * red) + sd * (red - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);
                                produced_lb.Add(rounder(lb4) / 1000);
                            }
                            else if (config == "Lateral Ties Config 5")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = b - 2 * (ccc) + (hl) + (hl2);
                                double lb3 = d - 2 * (ccc) + (hl) + (hl2);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);
                            }
                            else if (config == "Lateral Ties Config 6")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = b - 2 * (ccc) + 2 * (hl3);
                                double lb3 = d - 2 * (ccc) + 2 * (hl3);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);
                            }
                            else
                            {
                                print("no config");
                            }

                            double rv = 0;
                            double joint_d = double.Parse(cEF.structuralMembers.column[i][j][11]);
                            if (joint_d >= 6 && joint_d <= 16)
                            {
                                rv = 2 * joint_d;
                            }
                            else if (joint_d >= 20 && joint_d <= 56)
                            {
                                rv = 2.5 * joint_d;
                            }
                            double jl = 0;
                            double jl2 = 0;
                            double jl3 = 0;
                            for (int r = 0; r < cEF.parameters.rein_BEH_ST_dt.Rows.Count; r++)
                            {
                                if ((cEF.parameters.rein_BEH_ST_dt.Rows[r][0]).ToString() == joint_d.ToString())
                                {
                                    jl = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][2].ToString());
                                    jl2 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][1].ToString());
                                    jl3 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][3].ToString());
                                    break;
                                }
                            }

                            List<double> produced_qtytq = new List<double>();
                            if (config == "Lateral Ties Config 1" || config == "Lateral Ties Config 2" || config == "Lateral Ties Config 3" || config == "Lateral Ties Config 4")
                            {
                                foreach (var lb in produced_lb)
                                {
                                    double lb_qtytq1 = (lb * 1000) - 2 * (hl - jl) - 3 * (rv) + 3 * (rl);
                                    produced_qtytq.Add(rounder(lb_qtytq1) / 1000);
                                }
                            }
                            else if (config == "Lateral Ties Config 5")
                            {
                                double lba = produced_lb[0] * 1000;
                                double lbb = produced_lb[1] * 1000;
                                double lbc = produced_lb[2] * 1000;
                                double lb_qtytq1 = lba - 2 * (hl - jl) - 3 * (rv) + 3 * (rl);
                                double lb_qtytq2 = lbb - (hl - jl) - (hl2 - jl2);
                                double lb_qtytq3 = lbc - (hl - jl) - (hl2 - jl2);
                                produced_qtytq.Add(rounder(lb_qtytq1) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq2) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq3) / 1000);
                            }
                            else if (config == "Lateral Ties Config 6")
                            {
                                double lba = produced_lb[0] * 1000;
                                double lbb = produced_lb[1] * 1000;
                                double lbc = produced_lb[2] * 1000;
                                double lb_qtytq1 = lba - 2 * (hl - jl) - 3 * (rv) + 3 * (rl);
                                double lb_qtytq2 = lbb - 2 * (hl3 - jl3);
                                double lb_qtytq3 = lbc - 2 * (hl3 - jl3);
                                produced_qtytq.Add(rounder(lb_qtytq1) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq2) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq3) / 1000);
                            }
                            else
                            {
                                print("no config 2nd");
                            }
                            List<List<string>> lbt = lateralTiesA(produced_lb, qtyte1, left, qtySUM, qtytatrest, config, qtyCOL, green, red, level, cEF, lat_d);
                            List<List<string>> lbtq = lateralTiesB(produced_qtytq, qtyte1, left, qtySUM, qtytatrest, config, qtyCOL, green, red, level, cEF, qtytq, joint_d);
                            var combinedLBS = lbt.Concat(lbtq);
                            lbt = combinedLBS.ToList();
                            lats.Add(lbt);
                        }
                        else // upper floors 
                        {
                            height = double.Parse(cEF.structuralMembers.column[i][j][3]);
                            diameter = double.Parse(cEF.structuralMembers.column[i][j][6]);
                            qtyCOL = double.Parse(cEF.structuralMembers.column[i][j][5]);
                            qtyDIA = double.Parse(cEF.structuralMembers.column[i][j][7]);
                            type = cEF.structuralMembers.column[i][j][8];

                            //
                            double clear_h = double.Parse(cEF.structuralMembers.column[i][j][4]);
                            double summation = 0;
                            double qtySUM = 0;
                            for (int o = 0; o < cEF.structuralMembers.columnSpacing[i][j].Count;)
                            {
                                qtySUM += double.Parse(cEF.structuralMembers.columnSpacing[i][j][o]);
                                summation += double.Parse(cEF.structuralMembers.columnSpacing[i][j][o]) * double.Parse(cEF.structuralMembers.columnSpacing[i][j][o + 1]);
                                o += 2;
                            }
                            double srest = double.Parse(cEF.structuralMembers.column[i][j][13]);
                            double stj = double.Parse(cEF.structuralMembers.column[i][j][10]);
                            qtytatrest = rounder(((clear_h - 2 * summation) / srest) + 1);
                            qtytq = rounder(((height - clear_h) / stj) - 1);
                            print("qtytatrest: " + qtytatrest);
                            print("qtyq: " + qtytq);

                            double ccc = double.Parse(cEF.parameters.conc_CC_CEW);
                            double lat_d = double.Parse(cEF.structuralMembers.column[i][j][11]);
                            double hl = 0;
                            double hl2 = 0;
                            double hl3 = 0;
                            for (int r = 0; r < cEF.parameters.rein_BEH_ST_dt.Rows.Count; r++)
                            {
                                if ((cEF.parameters.rein_BEH_ST_dt.Rows[r][0]).ToString() == lat_d.ToString())
                                {
                                    hl = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][2].ToString());
                                    hl2 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][1].ToString());
                                    hl3 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][3].ToString());
                                    break;
                                }
                            }
                            double rl = 0;
                            if (lat_d >= 6 && lat_d <= 16)
                            {
                                rl = 2 * lat_d;
                            }
                            else if (lat_d >= 20 && lat_d <= 56)
                            {
                                rl = 2.5 * lat_d;
                            }
                            List<double> produced_lb = new List<double>();
                            string config = cEF.structuralMembers.column[i][j][12];
                            double b = double.Parse(cEF.structuralMembers.column[i][j][1]);
                            double d = double.Parse(cEF.structuralMembers.column[i][j][2]);
                            double red = 0;
                            double green = 0;
                            if (config == "Lateral Ties Config 1")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1) / 1000);
                            }
                            else if (config == "Lateral Ties Config 2")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = 4 * (Math.Sqrt(Math.Pow(((b / 2) - ccc), 2) + Math.Pow(((d / 2) - ccc), 2))) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                            }
                            else if (config == "Lateral Ties Config 3")
                            {
                                red = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][0]);
                                green = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][1]);
                                double blue = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][2]);
                                double orange = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][3]);
                                double sb = (b - 2 * (ccc + lat_d) - diameter * (blue + 2 * orange)) / (blue + 2 * orange - 1);
                                double sd = (d - 2 * (ccc + lat_d) - diameter * (red + 2 * (green))) / (red + 2 * (green) - 1);
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = 2 * (d) + 2 * ((diameter * blue) + sb * (blue - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb3 = 2 * (b) + 2 * ((diameter * red) + sd * (red - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);

                            }
                            else if (config == "Lateral Ties Config 4")
                            {
                                red = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][0]);
                                green = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][1]);
                                double blue = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][2]);
                                double orange = double.Parse(cEF.structuralMembers.columnLateralTies[i][j][3]);
                                double sb = (b - 2 * (ccc + lat_d) - diameter * (blue + 2 * orange)) / (blue + 2 * orange - 1);
                                double sd = (d - 2 * (ccc + lat_d) - diameter * (red + 2 * (green))) / (red + 2 * (green) - 1);
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = 4 * (Math.Sqrt(Math.Pow(((b / 2) - ccc), 2) + Math.Pow(((d / 2) - ccc), 2))) + 2 * (hl) - 3 * (rl);
                                double lb3 = 2 * (d) + 2 * ((diameter * blue) + sb * (blue - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb4 = 2 * (b) + 2 * ((diameter * red) + sd * (red - 1) + 2 * (lat_d)) - 4 * (ccc) + 2 * (hl) - 3 * (rl);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);
                                produced_lb.Add(rounder(lb4) / 1000);
                            }
                            else if (config == "Lateral Ties Config 5")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = b - 2 * (ccc) + (hl) + (hl2);
                                double lb3 = d - 2 * (ccc) + (hl) + (hl2);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);
                            }
                            else if (config == "Lateral Ties Config 6")
                            {
                                double lb1 = 2 * (b + d) - 8 * (ccc) + 2 * (hl) - 3 * (rl);
                                double lb2 = b - 2 * (ccc) + 2 * (hl3);
                                double lb3 = d - 2 * (ccc) + 2 * (hl3);
                                produced_lb.Add(rounder(lb1) / 1000);
                                produced_lb.Add(rounder(lb2) / 1000);
                                produced_lb.Add(rounder(lb3) / 1000);
                            }
                            else
                            {
                                print("no config");
                            }
                            double rv = 0;
                            double joint_d = double.Parse(cEF.structuralMembers.column[i][j][9]);
                            if (joint_d >= 6 && joint_d <= 16)
                            {
                                rv = 2 * joint_d;
                            }
                            else if (joint_d >= 20 && joint_d <= 56)
                            {
                                rv = 2.5 * joint_d;
                            }
                            double jl = 0;
                            double jl2 = 0;
                            double jl3 = 0;
                            for (int r = 0; r < cEF.parameters.rein_BEH_ST_dt.Rows.Count; r++)
                            {
                                if ((cEF.parameters.rein_BEH_ST_dt.Rows[r][0]).ToString() == joint_d.ToString())
                                {
                                    jl = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][2].ToString());
                                    jl2 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][1].ToString());
                                    jl3 = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][3].ToString());
                                    break;
                                }
                            }

                            List<double> produced_qtytq = new List<double>();
                            if (config == "Lateral Ties Config 1" || config == "Lateral Ties Config 2" || config == "Lateral Ties Config 3" || config == "Lateral Ties Config 4")
                            {
                                foreach (var lb in produced_lb)
                                {
                                    double lb_qtytq1 = (lb * 1000) - 2 * (hl - jl) - 3 * (rv) + 3 * (rl);
                                    produced_qtytq.Add(rounder(lb_qtytq1) / 1000);
                                }
                            }
                            else if (config == "Lateral Ties Config 5")
                            {
                                double lba = produced_lb[0] * 1000;
                                double lbb = produced_lb[1] * 1000;
                                double lbc = produced_lb[2] * 1000;
                                double lb_qtytq1 = lba - 2 * (hl - jl) - 3 * (rv) + 3 * (rl);
                                double lb_qtytq2 = lbb - (hl - jl) - (hl2 - jl2);
                                double lb_qtytq3 = lbc - (hl - jl) - (hl2 - jl2);
                                produced_qtytq.Add(rounder(lb_qtytq1) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq2) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq3) / 1000);
                            }
                            else if (config == "Lateral Ties Config 6")
                            {
                                double lba = produced_lb[0] * 1000;
                                double lbb = produced_lb[1] * 1000;
                                double lbc = produced_lb[2] * 1000;
                                double lb_qtytq1 = lba - 2 * (hl - jl) - 3 * (rv) + 3 * (rl);
                                double lb_qtytq2 = lbb - 2 * (hl3 - jl3);
                                double lb_qtytq3 = lbc - 2 * (hl3 - jl3);
                                produced_qtytq.Add(rounder(lb_qtytq1) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq2) / 1000);
                                produced_qtytq.Add(rounder(lb_qtytq3) / 1000);
                            }
                            else
                            {
                                print("no config 2nd");
                            }
                            List<List<string>> lbt = lateralTiesA(produced_lb, 0, 0, qtySUM, qtytatrest, config, qtyCOL, green, red, level, cEF, lat_d);
                            List<List<string>> lbtq = lateralTiesB(produced_qtytq, 0, 0, qtySUM, qtytatrest, config, qtyCOL, green, red, level, cEF, qtytq, joint_d);
                            var combinedLBS = lbt.Concat(lbtq);
                            lbt = combinedLBS.ToList();
                            lats.Add(lbt);
                        }
                    }
                    masterlats.Add(lats);
                }

                double lateralties_weight = 0;
                for (int i = 0; i < masterlats.Count; i++)
                {
                    for (int j = 0; j < masterlats[i].Count; j++)
                    {
                        for (int n = 0; n < masterlats[i][j].Count; n++)
                        {
                            string diam = masterlats[i][j][n][2] + "mm";
                            double lcm = double.Parse(masterlats[i][j][n][0]);
                            double qtym = double.Parse(masterlats[i][j][n][1]);
                            double wd = 0;
                            for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
                            {
                                if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == diam)
                                {
                                    wd = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                                    break;
                                }
                            }
                            lateralties_weight += ((lcm * qtym) * wd) * double.Parse(cEF.floors[i].getValues()[0]);
                        }
                    }
                }
                cEF.structuralMembers.Column_lateralRebar = masterlats;
                cEF.structuralMembers.totalweightkgm_Colties = lateralties_weight;                
            }
            catch (Exception ex)
            {                
                print("Column Lateral ties: " + ex);
            }
            refreshSolutions(cEF);
        }
        //Column Computation Functions -- END

        //Beam Computation Functions -- STARt
        public void AddBeamWorks(CostEstimationForm cEF, int floorCount, int beamCount, string beamType)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.concreteWorkSolutionsBR[floorCount].Add(newList);
            if(!cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Any() && cEF.parameters.conc_cmIsSelected[2])
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Add(0);
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Add(0);
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Add(0);
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Add(0);
            } 
            else if (!cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Any() && !cEF.parameters.conc_cmIsSelected[2])
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Add(0);
            }
            beamWorks(cEF, floorCount, beamCount, beamType);
        }

        //Add 
        public void beamWorks(CostEstimationForm cEF, int floorCount, int beamCount, string beamType)
        {
            //Variables from Parameters
            string concreteGrade;

            //Init variables from Parameters
            concreteGrade = cEF.parameters.conc_CM_B_CG;

            //Variables from StructMem
            double baseB, depth, length, quantity, beamQuantity;

            beamQuantity = double.Parse(cEF.structuralMembers.beam[floorCount][beamCount][2], System.Globalization.CultureInfo.InvariantCulture);

            //Init variables from StructMem
            foreach (List<string> beamRow in cEF.structuralMembers.beamRow[floorCount][beamCount])
            {
                foreach (List<string> schedule in cEF.structuralMembers.beamSchedule[floorCount])
                {
                    if (beamType.Equals(schedule[0]))
                    {
                        if (beamRow[0].Equals(schedule[1]))
                        {
                            //Init variables from Beam Row and Beam Schedule
                            baseB = double.Parse(schedule[2], System.Globalization.CultureInfo.InvariantCulture);
                            depth = double.Parse(schedule[3], System.Globalization.CultureInfo.InvariantCulture);
                            length = double.Parse(beamRow[3], System.Globalization.CultureInfo.InvariantCulture);
                            quantity = double.Parse(beamRow[1], System.Globalization.CultureInfo.InvariantCulture);

                            //Computation -- Concrete Works
                            if (cEF.parameters.conc_cmIsSelected[2])
                            {
                                if (concreteGrade.Equals("CLASS AA"))
                                {
                                    double volume = baseB * depth * length * quantity; 
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][3] += volume;
                                }
                                else if (concreteGrade.Equals("CLASS A"))
                                {
                                    double volume = baseB * depth * length * quantity;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[1][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[1][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[1][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][3] += volume;
                                }
                                else if (concreteGrade.Equals("CLASS B"))
                                {
                                    double volume = baseB * depth * length * quantity;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[2][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[2][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[2][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][3] += volume;
                                }
                                else
                                {
                                    double volume = baseB * depth * length * quantity;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[3][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[3][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[3][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][3] += volume;
                                }
                            }
                            else
                            {
                                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] +=
                                    ((baseB * depth * length) / 1000000000) * quantity;
                            }
                        }
                    }
                }
            }

            if (cEF.parameters.conc_cmIsSelected[2])
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] *= beamQuantity;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][1] *= beamQuantity;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][2] *= beamQuantity;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][3] *= beamQuantity;
            }
            else
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] *= beamQuantity;
            }
            //Computation -- add Formworks [BEAM]
            recomputeFW_Beam(cEF);
            refreshSolutions(cEF);
        }

        //Modify
        public void ModifyBeamWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            //Variables from Parameters
            string concreteGrade;

            //Init variables from Parameters
            concreteGrade = cEF.parameters.conc_CM_B_CG;

            //Variables from StructMem
            double baseB, depth, length, quantity, beamQuantity;

            beamQuantity = double.Parse(cEF.structuralMembers.beam[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);

            //Init variables from StructMem
            if (cEF.parameters.conc_cmIsSelected[2])
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] = 0;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][1] = 0;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][2] = 0;
            }
            else
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] = 0;
            }
            foreach (List<string> beamRow in cEF.structuralMembers.beamRow[floorCount][index])
            {
                foreach (List<string> schedule in cEF.structuralMembers.beamSchedule[floorCount])
                {
                    if (cEF.structuralMembers.beam[floorCount][index][0].Equals(schedule[0]))
                    {
                        if (beamRow[0].Equals(schedule[1]))
                        {
                            //Init variables from Beam Row and Beam Schedule
                            baseB = double.Parse(schedule[2], System.Globalization.CultureInfo.InvariantCulture);
                            depth = double.Parse(schedule[3], System.Globalization.CultureInfo.InvariantCulture);
                            length = double.Parse(beamRow[3], System.Globalization.CultureInfo.InvariantCulture);
                            quantity = double.Parse(beamRow[1], System.Globalization.CultureInfo.InvariantCulture);

                            //Computation -- Concrete Works
                            if (cEF.parameters.conc_cmIsSelected[2])
                            {
                                if (concreteGrade.Equals("CLASS AA"))
                                {
                                    double volume = baseB * depth * length * quantity; ;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][3] += volume;
                                }
                                else if (concreteGrade.Equals("CLASS A"))
                                {
                                    double volume = baseB * depth * length * quantity;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[1][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[1][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[1][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][3] += volume;
                                }
                                else if (concreteGrade.Equals("CLASS B"))
                                {
                                    double volume = baseB * depth * length * quantity;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[2][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[2][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[2][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][3] += volume;
                                }
                                else
                                {
                                    double volume = baseB * depth * length * quantity;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[3][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[3][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[3][2];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][3] += volume;
                                }
                            }
                            else
                            {
                                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] +=
                                    ((baseB * depth * length) / 1000000000) * quantity;
                            }
                        }
                    }
                }
            }
            
            

            if (cEF.parameters.conc_cmIsSelected[2])
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] *= beamQuantity;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][1] *= beamQuantity;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][2] *= beamQuantity;
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][3] *= beamQuantity;
            }
            else
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] *= beamQuantity;
            }

            //Computation -- modify Formworks [BEAM]
            recomputeFW_Beam(cEF);
            refreshSolutions(cEF);
        }

        public void recomputeFW_Beam(CostEstimationForm cEF)
        {
            try
            {
                double vertTracker = 0;
                double total_ftbArea = 0;
                double RB_vertTracker = 0;
                double RB_total_gradeArea = 0;
                double total_gradeArea = 0;
                double RB_form = 0;
                double tie_form = 0;
                double tie_frame = 0;
                double grade_form = 0;
                double grade_frame = 0;
                double vertical_beam = 0;
                double horizontal_beam = 0;
                double RB_horizontal_beam = 0;
                double RB_vertical_beam = 0;
                double tiehand = 0;
                double[] equi_vert = { 4.00, 6.00, 8.00 };
                double[] equi_hori = { 4.70, 7.00, 9.35 };
                List<string> vertDim = dimensionFilterer(cEF.parameters.form_SM_C_VS);
                List<string> horiDim = dimensionFilterer(cEF.parameters.form_SM_C_HB);
                List<double> beam_handler = new List<double>();
                List<double> grade_handler = new List<double>();
                List<double> vertical_handler = new List<double>();
                List<double> horizontal_handler = new List<double>();
                List<double> RB_handler = new List<double>();
                List<double> RB_vertical_handler = new List<double>();
                List<double> RB_horizontal_handler = new List<double>();
                List<double> tiearea = new List<double>();
                List<double> gradearea = new List<double>();
                List<double> rbarea = new List<double>();
                for (int i = 0; i < cEF.structuralMembers.beam.Count; i++)
                {
                    tiearea.Add(0);
                    gradearea.Add(0);
                    rbarea.Add(0);
                    grade_handler.Add(0);
                    vertical_handler.Add(0);
                    horizontal_handler.Add(0);
                    RB_handler.Add(0);
                    RB_horizontal_handler.Add(0);
                    RB_vertical_handler.Add(0);
                    double ftb_area = 0;
                    for (int j = 0; j < cEF.structuralMembers.beam[i].Count; j++)
                    {
                        if (cEF.structuralMembers.beam[i][j][0] == "Footing Tie Beam")//Beams connected to Footings beam
                        {

                            List<List<string>> sched_gatherer = new List<List<string>>();
                            foreach (var sched in cEF.structuralMembers.beamSchedule[i])
                            {

                                if (sched[0] == "Footing Tie Beam")//Sched connected to footing beam
                                {
                                    List<string> gatherer = new List<string>();
                                    gatherer.Add(sched[1]);//Sched Name
                                    gatherer.Add(sched[2]);//Base
                                    gatherer.Add(sched[3]);//Depth                                
                                    sched_gatherer.Add(gatherer);
                                }
                            }
                            double qty = double.Parse(cEF.structuralMembers.beam[i][j][2]);
                            foreach (List<string> rows in cEF.structuralMembers.beamRow[i][j])//Rows connected to footing beam
                            {
                                string r_name = rows[0];
                                double r_quantity = double.Parse(rows[1]);
                                double r_length = double.Parse(rows[2]);
                                ftb_area = 0;
                                foreach (List<string> gatheredSched in sched_gatherer)
                                {
                                    if (gatheredSched[0] == r_name)
                                    {
                                        ftb_area += ((2 * (double.Parse(gatheredSched[2]) / 1000) + (double.Parse(gatheredSched[1]) / 1000) + 0.1) * (r_quantity * (r_length / 1000))) * qty;
                                    }
                                }
                                total_ftbArea += ftb_area;
                                tiehand = total_ftbArea;
                            }
                        }
                        //
                        else if (cEF.structuralMembers.beam[i][j][0] == "Grade Beam" || cEF.structuralMembers.beam[i][j][0] == "Suspended Beam")//Beams connected to GRADE BEAM
                        {
                            List<List<string>> sched_gatherer = new List<List<string>>();
                            foreach (var sched in cEF.structuralMembers.beamSchedule[i])
                            {
                                if (sched[0] == "Grade Beam" || sched[0] == "Suspended Beam")//Sched connected to footing beam
                                {
                                    List<string> gatherer = new List<string>();
                                    gatherer.Add(sched[1]);//Sched Name
                                    gatherer.Add(sched[2]);//Base
                                    gatherer.Add(sched[3]);//Depth                                
                                    sched_gatherer.Add(gatherer);
                                }
                            }
                            double qty = double.Parse(cEF.structuralMembers.beam[i][j][2]);
                            foreach (List<string> rows in cEF.structuralMembers.beamRow[i][j])//Rows connected to footing beam
                            {
                                string r_name = rows[0];
                                double r_quantity = double.Parse(rows[1]);
                                double r_length = double.Parse(rows[2]);
                                ftb_area = 0;
                                foreach (List<string> gatheredSched in sched_gatherer)
                                {
                                    if (gatheredSched[0] == r_name)
                                    {
                                        vertTracker += (r_quantity * (r_length / 1000)) * qty;
                                        ftb_area += ((2 * (double.Parse(gatheredSched[2]) / 1000) + (double.Parse(gatheredSched[1]) / 1000) + 0.1) * (r_quantity * (r_length / 1000))) * qty;
                                    }
                                }
                                total_gradeArea += ftb_area;
                            }
                        }
                        //
                        else if (cEF.structuralMembers.beam[i][j][0] == "Roof Beam")//roof beam
                        {
                            List<List<string>> sched_gatherer = new List<List<string>>();
                            foreach (var sched in cEF.structuralMembers.beamSchedule[i])
                            {
                                if (sched[0] == "Roof Beam")//Sched connected to footing beam
                                {
                                    List<string> gatherer = new List<string>();
                                    gatherer.Add(sched[1]);//Sched Name
                                    gatherer.Add(sched[2]);//Base
                                    gatherer.Add(sched[3]);//Depth                                
                                    sched_gatherer.Add(gatherer);
                                }
                            }
                            double qty = double.Parse(cEF.structuralMembers.beam[i][j][2]);
                            foreach (List<string> rows in cEF.structuralMembers.beamRow[i][j])//Rows connected to footing beam
                            {
                                string r_name = rows[0];
                                double r_quantity = double.Parse(rows[1]);
                                double r_length = double.Parse(rows[2]);
                                ftb_area = 0;
                                foreach (List<string> gatheredSched in sched_gatherer)
                                {
                                    if (gatheredSched[0] == r_name)
                                    {
                                        RB_vertTracker += (r_quantity * (r_length / 1000)) * qty;
                                        ftb_area += ((2 * (double.Parse(gatheredSched[2]) / 1000) + (double.Parse(gatheredSched[1]) / 1000) + 0.1) * (r_quantity * (r_length / 1000))) * qty;
                                    }
                                }
                                RB_total_gradeArea += ftb_area;
                            }
                        }
                    }
                    //VERT
                    double vertindex;
                    double horiindex;
                    if (vertDim[2] == "2")
                    {
                        vertindex = equi_vert[0];
                    }
                    else if (vertDim[2] == "3")
                    {
                        vertindex = equi_vert[1];
                    }
                    else
                    {
                        vertindex = equi_vert[2];
                    }
                    //HORI
                    if (horiDim[2] == "2")
                    {
                        horiindex = equi_hori[0];
                    }
                    else if (horiDim[2] == "3")
                    {
                        horiindex = equi_hori[1];
                    }
                    else
                    {
                        horiindex = equi_hori[2];
                    }
                    horizontal_beam = rounder((((vertTracker * horiindex) * 12) / (double.Parse(horiDim[0]) * double.Parse(horiDim[2]) * double.Parse(horiDim[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    vertical_beam = rounder((((vertTracker * vertindex) * 12) / (double.Parse(vertDim[0]) * double.Parse(vertDim[2]) * double.Parse(vertDim[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    RB_horizontal_beam = rounder((((RB_vertTracker * horiindex) * 12) / (double.Parse(horiDim[0]) * double.Parse(horiDim[2]) * double.Parse(horiDim[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    RB_vertical_beam = rounder((((RB_vertTracker * vertindex) * 12) / (double.Parse(vertDim[0]) * double.Parse(vertDim[2]) * double.Parse(vertDim[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    grade_form = rounder((total_gradeArea / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU))); ;
                    RB_form = rounder((RB_total_gradeArea / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU))); ;
                    grade_handler[i] = grade_form;
                    RB_handler[i] = RB_form;
                    vertical_handler[i] = vertical_beam;
                    horizontal_handler[i] = horizontal_beam;
                    RB_vertical_handler[i] = RB_vertical_beam;
                    RB_horizontal_handler[i] = RB_horizontal_beam;
                    gradearea[i] = total_gradeArea;
                    tiearea[i] = tiehand;
                    rbarea[i] = RB_total_gradeArea;
                    total_gradeArea = 0;
                    vertTracker = 0;
                    RB_vertTracker = 0;
                    RB_total_gradeArea = 0;
                    tiehand = 0;
                }
                //
                List<string> beam_lumber = dimensionFilterer(cEF.parameters.form_SM_C_FL);
                double[] beam_frame = { 18.66, 28.00 };
                double frame;
                if (beam_lumber[2] == "2")
                {
                    frame = beam_frame[0];
                }
                else
                {
                    frame = beam_frame[1];
                }
                tie_form = rounder((total_ftbArea / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                tie_frame = rounder(((tie_form * frame) * 12) / (double.Parse(beam_lumber[0]) * double.Parse(beam_lumber[2]) * double.Parse(beam_lumber[4])));
                beam_handler.Add(tie_form);//formwork footing
                beam_handler.Add(tie_frame);//framework footing                                                                                 
                cEF.structuralMembers.beams_comps = beam_handler;
                cEF.structuralMembers.beams_grade = grade_handler;
                cEF.structuralMembers.beams_RB = RB_handler;
                cEF.structuralMembers.beams_vertical = vertical_handler;
                cEF.structuralMembers.beams_horizontal = horizontal_handler;
                cEF.structuralMembers.beams_RBV = RB_vertical_handler;
                cEF.structuralMembers.beams_RBH = RB_horizontal_handler;
                cEF.structuralMembers.beams_tiearea = tiearea;
                cEF.structuralMembers.beams_gradarea = gradearea;
                cEF.structuralMembers.beams_RBarea = rbarea;

                List<double> beamFRAME = new List<double>();
                foreach (var grade in cEF.structuralMembers.beams_grade)
                {
                    beamFRAME.Add(rounder(((grade * frame) * 12) / (double.Parse(beam_lumber[0]) * double.Parse(beam_lumber[2]) * double.Parse(beam_lumber[4]))));
                }
                List<double> roofFRAME = new List<double>();
                foreach (var grade in cEF.structuralMembers.beams_RB)
                {
                    roofFRAME.Add(rounder(((grade * frame) * 12) / (double.Parse(beam_lumber[0]) * double.Parse(beam_lumber[2]) * double.Parse(beam_lumber[4]))));
                }
                cEF.structuralMembers.beams_gradeFrame = beamFRAME;
                cEF.structuralMembers.beams_RBframe = roofFRAME;
            }
            catch (Exception ex)
            {
                print("Beam Reinforcement: "+ex);
            }
            try
            {
                //Beam MAIN reinforcements                
                List<List<List<string>>> main_rein = new List<List<List<string>>>(); // type - name - mtop - mbot - etopA - etopB - ebott 
                string ccs;

                for (int i = 0; i < cEF.structuralMembers.beamSchedule.Count; i++)//floor
                {
                    List<List<string>> mainrein_holder_1 = new List<List<string>>();
                    for (int j = 0; j < cEF.structuralMembers.beamSchedule[i].Count; j++)
                    {
                        List<string> mainrein_holder_2 = new List<string>();
                        string beam_type = cEF.structuralMembers.beamSchedule[i][j][0];
                        string beam_name = cEF.structuralMembers.beamSchedule[i][j][1];
                        double cont_top = double.Parse(cEF.structuralMembers.beamSchedule[i][j][11]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][12]);
                        double cont_bott = (double.Parse(cEF.structuralMembers.beamSchedule[i][j][9]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][10]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][17]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][18])) / 2;
                        double extra_topA = double.Parse(cEF.structuralMembers.beamSchedule[i][j][7]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][8]) - cont_top;
                        double extra_topB = double.Parse(cEF.structuralMembers.beamSchedule[i][j][15]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][16]) - cont_top;
                        double extra_bott = double.Parse(cEF.structuralMembers.beamSchedule[i][j][13]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][14]) - cont_bott;
                        mainrein_holder_2.Add(beam_type);
                        mainrein_holder_2.Add(beam_name);
                        mainrein_holder_2.Add(cont_top.ToString());
                        mainrein_holder_2.Add(cont_bott.ToString());
                        mainrein_holder_2.Add(extra_topA.ToString());
                        mainrein_holder_2.Add(extra_topB.ToString());
                        mainrein_holder_2.Add(extra_bott.ToString());
                        mainrein_holder_2.Add(cEF.structuralMembers.beamSchedule[i][j][5].ToString());
                        mainrein_holder_2.Add(cEF.structuralMembers.beamSchedule[i][j][6].ToString());
                        mainrein_holder_1.Add(mainrein_holder_2);
                    }
                    main_rein.Add(mainrein_holder_1);                    //save this
                }

                List<List<List<string>>> eachBeamRow = new List<List<List<string>>>();// type - name - mtop - mbot - etopA - etopB - ebott -sltop - slbot -lbsTop -lbsBot
                double sl_top = 0;
                double sl_bot = 0;
                double hook_top = 0;
                double hook_bot = 0;
                int hook_index = 0;
                for (int i = 0; i < cEF.structuralMembers.beamRow.Count; i++)
                {
                    List<List<string>> mainrein_holder_1 = new List<List<string>>();
                    for (int j = 0; j < cEF.structuralMembers.beamRow[i].Count; j++)
                    {
                        int quantity_tracker = 0;
                        string hook_type = cEF.structuralMembers.beam[i][j][3];
                        if (hook_type == "90")
                        {
                            hook_index = 1;
                        }
                        else if (hook_type == "135")
                        {
                            hook_index = 2;
                        }
                        else
                        {
                            hook_index = 3;
                        }

                        for (int n = 0; n < cEF.structuralMembers.beamRow[i][j].Count; n++)
                        {
                            double qty = double.Parse(cEF.structuralMembers.beamRow[i][j][n][1]);
                            List<string> mainrein_holder_2 = new List<string>();
                            List<string> lbs = new List<string>();
                            for (int sched = 0; sched < main_rein[i].Count; sched++)
                            {
                                if (main_rein[i][sched][1] == cEF.structuralMembers.beamRow[i][j][n][0] && main_rein[i][sched][0] == cEF.structuralMembers.beam[i][j][0])
                                {
                                    mainrein_holder_2 = main_rein[i][sched];
                                    lbs = main_rein[i][sched];
                                    break;
                                }
                            }
                            for (int reps = 0; reps < qty; reps++)
                            {
                                quantity_tracker++;
                                List<string> mainrein_holder_3 = new List<string>();
                                if (mainrein_holder_2[0] == "Footing Tie Beam" || mainrein_holder_2[0] == "Grade Beam")
                                {
                                    ccs = cEF.parameters.conc_CC_BEE;
                                }
                                else
                                {
                                    ccs = cEF.parameters.conc_CC_BEW;
                                }
                                if (cEF.structuralMembers.beam[i][j][5] == "Mechanical" || cEF.structuralMembers.beam[i][j][5] == "Welded Splice (Butt)")
                                {
                                    sl_top = 0;
                                    sl_bot = 0;
                                }
                                else
                                {
                                    string mix = spliceMixGetter(cEF.parameters.conc_CM_B_RM);
                                    if (cEF.parameters.conc_cmIsSelected[2])
                                    {
                                        if (cEF.parameters.conc_CM_B_CG.Equals("CLASS AA"))
                                        {
                                            mix = "27.6";
                                        }
                                        else if (cEF.parameters.conc_CM_B_CG.Equals("CLASS A"))
                                        {
                                            mix = "24.1";
                                        }
                                        else if (cEF.parameters.conc_CM_B_CG.Equals("CLASS B"))
                                        {
                                            mix = "17.2";
                                        }
                                        else
                                        {
                                            mix = "13.8";
                                        }
                                    }
                                    print(mix + " MIX");
                                    int index = cEF.parameters.rein_LSL_TB_fc_list.IndexOf(mix);
                                    if (index >= 0)
                                    {
                                        for (int r = 0; r < cEF.parameters.rein_LSL_TB_dt.Rows.Count; r++)
                                        {
                                            if (sl_top != 0 && sl_bot != 0)
                                            {
                                                break;
                                            }
                                            if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == mainrein_holder_2[7])
                                            {
                                                sl_top = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                            }
                                            if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == mainrein_holder_2[8])
                                            {
                                                sl_bot = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sl_top = 0;
                                        sl_bot = 0;
                                    }
                                }
                                if (hook_index > 0)
                                {
                                    for (int r = 0; r < cEF.parameters.rein_BEH_MB_dt.Rows.Count; r++)
                                    {
                                        if (hook_top != 0 && hook_bot != 0)
                                        {
                                            break;
                                        }
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == mainrein_holder_2[7])
                                        {
                                            hook_top = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][hook_index].ToString());
                                        }
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == mainrein_holder_2[8])
                                        {
                                            hook_bot = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][hook_index].ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    hook_top = 0;
                                    hook_bot = 0;
                                }

                                double total_quantity = 0;
                                foreach (var row in cEF.structuralMembers.beamRow[i][j])
                                {
                                    total_quantity += double.Parse(row[1]);
                                }

                                double lb_top;
                                double lb_bot;
                                double lb_top_extraA;
                                double lb_top_extraB;
                                double lb_bot_extraA;
                                double TR = reinGetter(cEF.structuralMembers.beam[i][j][6]);
                                double BR = reinGetter(cEF.structuralMembers.beam[i][j][7]);
                                if (total_quantity != 1)
                                {
                                    if (cEF.structuralMembers.beamRow[i][j][n][4] == "1-End Support")
                                    {
                                        lb_top = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top + (0.5 * sl_top) - double.Parse(ccs);
                                        lb_bot = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_bot + (0.5 * sl_bot) - double.Parse(ccs);
                                        lb_top_extraA = 0;
                                        lb_top_extraB = (TR - 1) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) + double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]);
                                        lb_bot_extraA = 0;
                                    }
                                    else
                                    {
                                        if (quantity_tracker == 0)
                                        {
                                            lb_top = (1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_top + (0.5 * sl_top) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs);
                                            lb_bot = (1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_bot + (0.5 * sl_bot) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs);
                                            lb_top_extraA = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top + (TR - 1) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) - double.Parse(ccs);
                                            lb_top_extraB = (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + (TR - 0.5) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        }
                                        else
                                        {
                                            lb_top = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + sl_top;
                                            lb_bot = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + sl_bot;
                                            lb_top_extraA = (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + (TR - 0.5) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                            lb_top_extraB = (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + (TR - 0.5) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        }
                                        lb_bot_extraA = double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) * (1 - (2 * BR));
                                    }
                                }
                                else
                                {
                                    if (cEF.structuralMembers.beamRow[i][j][n][4] == "1-End Support")
                                    {
                                        lb_top = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_bot = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_bot - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_top_extraA = 0;
                                        lb_top_extraB = (2 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_top + (TR - 2) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) - double.Parse(ccs);
                                        lb_bot_extraA = 0;
                                    }
                                    else
                                    {
                                        lb_top = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_bot = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_bot - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_top_extraA = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_top_extraB = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_bot_extraA = double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) * (1 - (2 * BR));
                                    }
                                }
                                mainrein_holder_3.Add(lb_top.ToString());
                                mainrein_holder_3.Add(lb_bot.ToString());
                                mainrein_holder_3.Add(lb_top_extraA.ToString());
                                mainrein_holder_3.Add(lb_top_extraB.ToString());
                                mainrein_holder_3.Add(lb_bot_extraA.ToString());
                                mainrein_holder_3.Add(cEF.structuralMembers.beam[i][j][2].ToString());
                                var concatted2ndLayer = mainrein_holder_2.Concat(mainrein_holder_3);
                                mainrein_holder_2 = concatted2ndLayer.ToList();
                                if (cEF.structuralMembers.beamRow[i][j][n][4] == "2-End Supports" && quantity_tracker == total_quantity)
                                {
                                    List<string> lbs_handler = new List<string>();
                                    lbs_handler.Add(((1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_top + (0.5 * sl_top) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs)).ToString());
                                    lbs_handler.Add(((1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_bot + (0.5 * sl_bot) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs)).ToString());
                                    lbs_handler.Add((double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top + (TR - 1) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) - double.Parse(ccs)).ToString());
                                    lbs_handler.Add(((0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + (TR - 0.5) * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])).ToString());
                                    lbs_handler.Add(lb_bot_extraA.ToString());
                                    lbs_handler.Add(cEF.structuralMembers.beam[i][j][2].ToString());
                                    var concatLBS = lbs.Concat(lbs_handler);
                                    lbs = concatLBS.ToList();
                                    mainrein_holder_1.Add(lbs);
                                }
                                else
                                {
                                    mainrein_holder_1.Add(mainrein_holder_2);
                                }
                            }
                        }
                    }
                    eachBeamRow.Add(mainrein_holder_1); // save this
                }
                cEF.structuralMembers.beamdias = eachBeamRow;

                List<string> dbholder = new List<string>();
                List<string> topholder = new List<string>();
                List<string> botholder = new List<string>();
                List<List<List<List<string>>>> finalMainRebar = new List<List<List<List<string>>>>();
                for (int i = 0; i < eachBeamRow.Count; i++)
                {
                    List<List<List<string>>> outer_holder = new List<List<List<string>>>();
                    for (int j = 0; j < eachBeamRow[i].Count; j++)
                    {
                        List<List<string>> final_holder = new List<List<string>>();
                        if (eachBeamRow[i][j][7] == eachBeamRow[i][j][8])// lb qty
                        {
                            dbholder = beamMainML(0
                                , eachBeamRow[i][j][9]
                                , eachBeamRow[i][j][10]
                                , eachBeamRow[i][j][2]
                                , eachBeamRow[i][j][3]
                                , eachBeamRow[i][j][14]
                                , cEF
                                , eachBeamRow[i][j][11]
                                , eachBeamRow[i][j][12]
                                , eachBeamRow[i][j][13]
                                , eachBeamRow[i][j][4]
                                , eachBeamRow[i][j][5]
                                , eachBeamRow[i][j][6]);
                            final_holder.Add(dbholder);
                        }
                        else
                        {
                            dbholder = beamMainML(1
                                , eachBeamRow[i][j][9]
                                , eachBeamRow[i][j][10]
                                , eachBeamRow[i][j][2]
                                , eachBeamRow[i][j][3]
                                , eachBeamRow[i][j][14]
                                , cEF
                                , eachBeamRow[i][j][11]
                                , eachBeamRow[i][j][12]
                                , eachBeamRow[i][j][13]
                                , eachBeamRow[i][j][4]
                                , eachBeamRow[i][j][5]
                                , eachBeamRow[i][j][6]);
                            final_holder.Add(dbholder);
                        }
                        if (eachBeamRow[i][j][11] == eachBeamRow[i][j][12])
                        {
                            topholder = beamMainML(2
                                , eachBeamRow[i][j][9]
                                , eachBeamRow[i][j][10]
                                , eachBeamRow[i][j][2]
                                , eachBeamRow[i][j][3]
                                , eachBeamRow[i][j][14]
                                , cEF
                                , eachBeamRow[i][j][11]
                                , eachBeamRow[i][j][12]
                                , eachBeamRow[i][j][13]
                                , eachBeamRow[i][j][4]
                                , eachBeamRow[i][j][5]
                                , eachBeamRow[i][j][6]);
                            final_holder.Add(topholder);
                        }
                        else
                        {
                            topholder = beamMainML(3
                                , eachBeamRow[i][j][9]
                                , eachBeamRow[i][j][10]
                                , eachBeamRow[i][j][2]
                                , eachBeamRow[i][j][3]
                                , eachBeamRow[i][j][14]
                                , cEF
                                , eachBeamRow[i][j][11]
                                , eachBeamRow[i][j][12]
                                , eachBeamRow[i][j][13]
                                , eachBeamRow[i][j][4]
                                , eachBeamRow[i][j][5]
                                , eachBeamRow[i][j][6]);
                            final_holder.Add(topholder);
                        }
                        botholder = beamMainML(4
                                , eachBeamRow[i][j][9]
                                , eachBeamRow[i][j][10]
                                , eachBeamRow[i][j][2]
                                , eachBeamRow[i][j][3]
                                , eachBeamRow[i][j][14]
                                , cEF
                                , eachBeamRow[i][j][11]
                                , eachBeamRow[i][j][12]
                                , eachBeamRow[i][j][13]
                                , eachBeamRow[i][j][4]
                                , eachBeamRow[i][j][5]
                                , eachBeamRow[i][j][6]);
                        final_holder.Add(botholder);
                        outer_holder.Add(final_holder);
                    }
                    finalMainRebar.Add(outer_holder);
                }
                cEF.structuralMembers.Beam_mainRebar = finalMainRebar;
                double totalweight = 0;
                for (int i = 0; i < finalMainRebar.Count; i++)
                {
                    for (int j = 0; j < finalMainRebar[i].Count; j++)
                    {
                        string diatop = eachBeamRow[i][j][7] + "mm";
                        string diabot = eachBeamRow[i][j][8] + "mm";
                        double kgmTop = 0;
                        double kgmBot = 0;
                        for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
                        {
                            if (kgmTop != 0 && kgmBot != 0)
                            {
                                break;
                            }
                            if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == diatop)
                            {
                                kgmTop = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                            }
                            if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == diabot)
                            {
                                kgmBot = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                            }
                        }

                        if (eachBeamRow[i][j][7] == eachBeamRow[i][j][8])
                        {
                            totalweight += (double.Parse(finalMainRebar[i][j][0][0]) * double.Parse(finalMainRebar[i][j][0][1]) * kgmTop) * double.Parse(cEF.floors[i].getValues()[0]);
                        }
                        else
                        {
                            totalweight += ((double.Parse(finalMainRebar[i][j][0][0]) * double.Parse(finalMainRebar[i][j][0][1]) * kgmTop) + (double.Parse(finalMainRebar[i][j][0][2]) * double.Parse(finalMainRebar[i][j][0][3]) * kgmBot)) * double.Parse(cEF.floors[i].getValues()[0]);
                        }

                        if (eachBeamRow[i][j][11] == eachBeamRow[i][j][12])
                        {
                            totalweight += (double.Parse(finalMainRebar[i][j][1][0]) * double.Parse(finalMainRebar[i][j][1][1]) * kgmTop) * double.Parse(cEF.floors[i].getValues()[0]);
                        }
                        else
                        {
                            totalweight += ((double.Parse(finalMainRebar[i][j][1][0]) * double.Parse(finalMainRebar[i][j][1][1]) * kgmTop) + (double.Parse(finalMainRebar[i][j][1][2]) * double.Parse(finalMainRebar[i][j][1][3]) * kgmBot)) * double.Parse(cEF.floors[i].getValues()[0]);
                        }
                        totalweight += (double.Parse(finalMainRebar[i][j][2][0]) * double.Parse(finalMainRebar[i][j][2][1]) * kgmBot) * double.Parse(cEF.floors[i].getValues()[0]);
                    }
                }
                cEF.structuralMembers.totalweightkgm_main = totalweight;
            }
            catch(Exception ex)
            {
                print("Beam main rebar: " + ex);
            }
            try
            {
                //beam stirrups                               
                List<List<List<List<string>>>> stirrupsols = new List<List<List<List<string>>>>();
                for (int i = 0; i < cEF.structuralMembers.beamRow.Count; i++)
                {
                    List<List<List<string>>> sbs3 = new List<List<List<string>>>();
                    for (int j = 0; j < cEF.structuralMembers.beamRow[i].Count; j++)
                    {
                        List<List<string>> sbs2 = new List<List<string>>();
                        for (int n = 0; n < cEF.structuralMembers.beamRow[i][j].Count; n++)
                        {                                             
                            string qtyA = "";
                            string qtyB = "";
                            string spacingA = "";
                            string spacingB = "";
                            string rest = "";
                            string props = "";
                            double b = 0;
                            double d = 0;
                            double dum = 0;
                            double dbs = 0;
                            double qtyint = 0;
                            double stirccs = 0;
                            int sh_index = 0;
                            double s = 0;
                            double lbn = 0;
                            for (int sched = 0; sched < cEF.structuralMembers.beamSchedule[i].Count; sched++)
                            {
                                if (cEF.structuralMembers.beamRow[i][j][n][0] == cEF.structuralMembers.beamSchedule[i][sched][1] && cEF.structuralMembers.beam[i][j][0] == cEF.structuralMembers.beamSchedule[i][sched][0])
                                {
                                    qtyA = cEF.structuralMembers.beamSchedule[i][sched][20];
                                    qtyB = cEF.structuralMembers.beamSchedule[i][sched][22];
                                    spacingA = cEF.structuralMembers.beamSchedule[i][sched][21];
                                    spacingB = cEF.structuralMembers.beamSchedule[i][sched][23];
                                    rest = cEF.structuralMembers.beamSchedule[i][sched][24];
                                    props = cEF.structuralMembers.beamSchedule[i][sched][4];
                                    b = double.Parse(cEF.structuralMembers.beamSchedule[i][sched][2]);
                                    d = double.Parse(cEF.structuralMembers.beamSchedule[i][sched][3]);
                                    dum = double.Parse(cEF.structuralMembers.beamSchedule[i][sched][5]);
                                    dbs = double.Parse(cEF.structuralMembers.beamSchedule[i][sched][19]);
                                    qtyint = double.Parse(cEF.structuralMembers.beamSchedule[i][sched][15]);
                                    if (cEF.structuralMembers.beamSchedule[i][sched][0] == "Footing Tie Beam" || cEF.structuralMembers.beamSchedule[i][sched][0] == "Grade Beam")
                                    {
                                        stirccs = double.Parse(cEF.parameters.conc_CC_BEE);
                                    }
                                    else
                                    {
                                        stirccs = double.Parse(cEF.parameters.conc_CC_BEW);
                                    }
                                    if (cEF.structuralMembers.beam[i][j][4] == "90")
                                    {
                                        sh_index = 1;
                                    }
                                    else if (cEF.structuralMembers.beam[i][j][4] == "135")
                                    {
                                        sh_index = 2;
                                    }
                                    else
                                    {
                                        sh_index = 3;
                                    }
                                    print(sh_index + " index");
                                    double stirHook = 0;
                                    for (int r = 0; r < cEF.parameters.rein_BEH_ST_dt.Rows.Count; r++)
                                    {
                                        if ((cEF.parameters.rein_BEH_ST_dt.Rows[r][0]).ToString() == dbs.ToString())
                                        {
                                            stirHook = double.Parse(cEF.parameters.rein_BEH_ST_dt.Rows[r][sh_index].ToString());
                                            break;
                                        }
                                    }
                                    s = (b - dum - 2 * (dbs + stirccs)) / (qtyint - 1);
                                    if (cEF.structuralMembers.beamSchedule[i][sched][4] == "No. 1")
                                    {
                                        lbn = (2 * (d + dum + s * ((qtyint) - 1) + stirHook) - 4 * (stirccs) - 11 * (dbs)) / 1000;
                                    }
                                    else
                                    {
                                        lbn = (2 * (d + dum + s * ((qtyint) - 2) + stirHook) - 4 * (stirccs) - 11 * (dbs)) / 1000;
                                    }
                                    break;
                                }
                            }
                            double qtyc = 0;
                            int determiner = 0;
                            if (cEF.structuralMembers.beamRow[i][j][n][4] == "2-End Supports")
                            {
                                qtyc = rounder((double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) - 2 * (double.Parse(qtyA) * double.Parse(spacingA) + double.Parse(qtyB) * double.Parse(spacingB))) / double.Parse(rest)) + 1;
                                determiner = 2;
                            }
                            else
                            {
                                qtyc = rounder((double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) - (double.Parse(qtyA) * double.Parse(spacingA) + double.Parse(qtyB) * double.Parse(spacingB))) / double.Parse(rest)) + 1;
                                determiner = 1;
                            }
                            string rowQ = cEF.structuralMembers.beamRow[i][j][n][1];
                            string page_quan = cEF.structuralMembers.beam[i][j][2];
                            List<string> wastes = stiruphelper(determiner, lbn.ToString(), rowQ, qtyA, qtyB, qtyc.ToString(), page_quan, props, cEF);
                            wastes.Add(dbs.ToString());
                            sbs2.Add(wastes);
                        }
                        sbs3.Add(sbs2);
                    }
                    stirrupsols.Add(sbs3);
                }
                cEF.structuralMembers.Beam_stirRebar = stirrupsols;

                double totalweight_stir = 0;
                for (int i = 0; i < stirrupsols.Count; i++)
                {
                    for (int j = 0; j < stirrupsols[i].Count; j++)
                    {
                        for (int n = 0; n < stirrupsols[i][j].Count; n++)
                        {
                            string diatop = stirrupsols[i][j][n][2] + "mm";
                            double kgmTop = 0;
                            for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
                            {
                                if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == diatop)
                                {
                                    kgmTop = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                                    break;
                                }
                            }
                            totalweight_stir += (kgmTop * double.Parse(stirrupsols[i][j][n][0]) * double.Parse(stirrupsols[i][j][n][1])) * double.Parse(cEF.floors[i].getValues()[0]);
                        }
                    }
                }
                cEF.structuralMembers.totalweightkgm_stir = totalweight_stir;

            }
            catch (Exception ex)
            {
                print("Beam stirrups: " + ex);
            }

            try
            {
                //Webs
                List<List<List<List<string>>>> webbeam = new List<List<List<List<string>>>>();
                for (int i = 0; i < cEF.structuralMembers.beamRow.Count; i++)
                {
                    List<List<List<string>>> web_holder3 = new List<List<List<string>>>();
                    for (int j = 0; j < cEF.structuralMembers.beamRow[i].Count; j++)
                    {
                        List<List<string>> web_holder2 = new List<List<string>>();
                        for (int n = 0; n < cEF.structuralMembers.beamRow[i][j].Count; n++)
                        {
                            double W_dia = 0;
                            double W_dQTY = 0;
                            for (int sched = 0; sched < cEF.structuralMembers.beamSchedule[i].Count; sched++)
                            {
                                if (cEF.structuralMembers.beamRow[i][j][n][0] == cEF.structuralMembers.beamSchedule[i][sched][1] && cEF.structuralMembers.beamSchedule[i][sched][0] == cEF.structuralMembers.beam[i][j][0])
                                {
                                    W_dia = double.Parse(cEF.structuralMembers.beamSchedule[i][sched][25]);
                                    W_dQTY = double.Parse(cEF.structuralMembers.beamSchedule[i][sched][26]);
                                    break;
                                }
                            }
                            string mix = spliceMixGetter(cEF.parameters.conc_CM_B_RM);
                            if (cEF.parameters.conc_cmIsSelected[2])
                            {
                                if (cEF.parameters.conc_CM_B_CG.Equals("CLASS AA"))
                                {
                                    mix = "27.6";
                                }
                                else if (cEF.parameters.conc_CM_B_CG.Equals("CLASS A"))
                                {
                                    mix = "24.1";
                                }
                                else if (cEF.parameters.conc_CM_B_CG.Equals("CLASS B"))
                                {
                                    mix = "17.2";
                                }
                                else
                                {
                                    mix = "13.8";
                                }
                            }

                            int index = cEF.parameters.rein_LSL_TB_fc_list.IndexOf(mix);
                            double sl = 0;
                            if (index >= 0)
                            {
                                for (int r = 0; r < cEF.parameters.rein_LSL_TB_dt.Rows.Count; r++)
                                {
                                    if (sl != 0)
                                    {
                                        break;
                                    }
                                    if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == W_dia.ToString())
                                    {
                                        sl = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                    }
                                }
                            }
                            else
                            {
                                sl = 0;
                            }
                            double lb = (sl + double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) / 1000;
                            List<string> web_holder = webBeamHelper(cEF, lb, double.Parse(cEF.structuralMembers.beamRow[i][j][n][1]), double.Parse(cEF.structuralMembers.beam[i][j][2]), W_dQTY);
                            web_holder.Add(W_dia.ToString());
                            web_holder2.Add(web_holder);
                        }
                        web_holder3.Add(web_holder2);
                    }
                    webbeam.Add(web_holder3);
                }
                cEF.structuralMembers.Beam_webRebar = webbeam;

                double totalweight_web = 0;
                for (int i = 0; i < webbeam.Count; i++)
                {
                    for (int j = 0; j < webbeam[i].Count; j++)
                    {
                        for (int n = 0; n < webbeam[i][j].Count; n++)
                        {
                            string diatop = webbeam[i][j][n][2] + "mm";
                            double kgmTop = 0;
                            for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
                            {
                                if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == diatop)
                                {
                                    kgmTop = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                                    break;
                                }
                            }
                            totalweight_web += (kgmTop * double.Parse(webbeam[i][j][n][0]) * double.Parse(webbeam[i][j][n][1])) * double.Parse(cEF.floors[i].getValues()[0]);
                        }
                    }
                }
                cEF.structuralMembers.totalweightkgm_web = totalweight_web;
            }
            catch(Exception ex)
            {
                print("Beam web: " + ex);
            }
            refreshSolutions(cEF);            
        }
        //Beam Computation Functions -- END

        //Slab Computation Functions -- START
        public void AddSlabWorks(CostEstimationForm cEF, int floorCount, int slabCount)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.concreteWorkSolutionsSL[floorCount].Add(newList); 
            slabWorks(cEF, floorCount, slabCount);
        }

        public void slabWorks(CostEstimationForm cEF, int floorCount, int slabCount)
        {
            if (cEF.structuralMembers.slab[floorCount][slabCount][0].Equals("Slab on Grade"))
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_S_SOG_CG;

                //Variables from StructMem
                double quantity, thickness, lengthTop, lengthBot, lengthLeft, lengthRight;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][1], System.Globalization.CultureInfo.InvariantCulture);
                thickness = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][2], System.Globalization.CultureInfo.InvariantCulture);
                lengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][11], System.Globalization.CultureInfo.InvariantCulture);
                lengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][13], System.Globalization.CultureInfo.InvariantCulture);
                lengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][15], System.Globalization.CultureInfo.InvariantCulture);
                lengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][17], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[3])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                    else
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                        ((((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness) / 1000000000) * quantity);
                }
            }
            else // Suspended Slab
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_S_SS_CG;

                //Variables from StructMem
                double quantity, thickness, lengthTop, lengthBot, lengthLeft, lengthRight;
                string slabMark;

                //Init variables from StructMem

                int i = 0;
                if (cEF.structuralMembers.slab[floorCount][slabCount][2].Equals("No. 1"))
                {
                    i++;
                }

                quantity = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][2], System.Globalization.CultureInfo.InvariantCulture);
                slabMark = cEF.structuralMembers.slab[floorCount][slabCount][1]; 
                thickness = 0;
                foreach (List<string> schedule in cEF.structuralMembers.slabSchedule[floorCount - 1])
                {
                    if (schedule[0].Equals(slabMark))
                    {
                        thickness = double.Parse(schedule[1], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                lengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][13 - i], System.Globalization.CultureInfo.InvariantCulture);
                lengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][15 - i], System.Globalization.CultureInfo.InvariantCulture);
                lengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][17 - i], System.Globalization.CultureInfo.InvariantCulture);
                lengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][19 - i], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[4])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                    else
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * quantity);
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                        ((((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness) / 1000000000) * quantity);
                }

            }
            //Computation -- add Formworks [SLAB]
            recomputeFW_slabs(cEF);
            refreshSolutions(cEF);
        }
        
        public double slabCustomLengthSOG(CostEstimationForm cEF, int floorCount, string beamRowName, string atBeam)
        {
            string[] substrings = atBeam.Split('(');
            string AtBeam = substrings[0].Substring(0, substrings[0].Length - 1);
            string count = substrings[1].Substring(0, substrings[1].Length - 1);
            int countAtBeam = int.Parse(count);

            int beamIndex = 0;
            foreach (string beamName in cEF.structuralMembers.beamNames[floorCount])
            {
                if (beamName.Equals(beamRowName))
                {
                    break;
                }
                beamIndex++;
            }

            int counter = 0;
            foreach (List<string> beamRow in cEF.structuralMembers.beamRow[floorCount][beamIndex])
            {
                if (beamRow[0].Equals(AtBeam))
                {
                    counter++;
                }
                if (counter == countAtBeam)
                {
                    return double.Parse(beamRow[1], System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return 0;
        }

        public double slabCustomLengthSS(CostEstimationForm cEF, int floorCount, string beamRowName, string atBeam)
        {
            string[] substrings = atBeam.Split('(');
            string AtBeam = substrings[0].Substring(0, substrings[0].Length - 1);
            string count = substrings[1].Substring(0, substrings[1].Length - 1);
            int countAtBeam = int.Parse(count);

            string[] substrings2 = beamRowName.Split('(');
            string beamNameSub = substrings2[0].Substring(0, substrings2[0].Length);
            string beamType = substrings2[1].Substring(0, substrings2[1].Length - 1);

            int floorCount2 = floorCount;

            if(beamType.Equals("Roof Beam"))
            {
                floorCount2--;
            }

            int beamIndex = 0;
            foreach (string beamName in cEF.structuralMembers.beamNames[floorCount2])
            {
                if (beamName.Equals(beamNameSub))
                {
                    break;
                }
                beamIndex++;
            }

            int counter = 0;
            foreach (List<string> beamRow in cEF.structuralMembers.beamRow[floorCount2][beamIndex])
            {
                if (beamRow[0].Equals(AtBeam))
                {
                    counter++;
                }
                if (counter == countAtBeam)
                {
                    return double.Parse(beamRow[1], System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            return 0;
        }

        public void ModifySlabWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            if (cEF.structuralMembers.slab[floorCount][index][0].Equals("Slab on Grade"))
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_S_SOG_CG;

                //Variables from StructMem
                double quantity, thickness, lengthTop, lengthBot, lengthLeft, lengthRight;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.slab[floorCount][index][1], System.Globalization.CultureInfo.InvariantCulture);
                thickness = double.Parse(cEF.structuralMembers.slab[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                lengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][index][11], System.Globalization.CultureInfo.InvariantCulture);
                lengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][index][13], System.Globalization.CultureInfo.InvariantCulture);
                lengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][index][15], System.Globalization.CultureInfo.InvariantCulture);
                lengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][index][17], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[3])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                    else
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                        ((((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness) / 1000000000) * quantity;
                }
            }
            else // Suspended Slab
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_S_SS_CG;

                //Variables from StructMem
                double quantity, thickness, lengthTop, lengthBot, lengthLeft, lengthRight;
                string slabMark;

                //Init variables from StructMem

                int i = 0;
                if (cEF.structuralMembers.slab[floorCount][index][2].Equals("No. 1"))
                {
                    i++;
                }

                quantity = double.Parse(cEF.structuralMembers.slab[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                slabMark = cEF.structuralMembers.slab[floorCount][index][1];
                thickness = 0;
                foreach (List<string> schedule in cEF.structuralMembers.slabSchedule[floorCount - 1])
                {
                    if (schedule[0].Equals(slabMark))
                    {
                        thickness = double.Parse(schedule[1], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                lengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][index][13 - i], System.Globalization.CultureInfo.InvariantCulture);
                lengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][index][15 - i], System.Globalization.CultureInfo.InvariantCulture);
                lengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][index][17 - i], System.Globalization.CultureInfo.InvariantCulture);
                lengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][index][19 - i], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[4])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                    else
                    {
                        double volume = ((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] =
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] =
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] =
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume * quantity;
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                        ((((lengthTop + lengthBot) / 2) * ((lengthLeft + lengthRight) / 2) * thickness) / 1000000000) * quantity;
                }
            }
            //Computation -- modify Formworks [SLAB]
            recomputeFW_slabs(cEF);
            refreshSolutions(cEF);
        }

        public void recomputeFW_slabs(CostEstimationForm cEF)
        {
            try
            {
                List<double> area = new List<double>();
                List<double> form = new List<double>();
                List<double> scaf = new List<double>();
                List<string> scafDim_h = dimensionFilterer(cEF.parameters.form_SM_HS_VS);
                area.Add(0);
                form.Add(0);
                scaf.Add(0);
                for (int i = 1; i < cEF.structuralMembers.slab.Count; i++)
                {
                    double totalarea = 0;
                    area.Add(0);
                    form.Add(0);
                    scaf.Add(0);
                    for (int j = 0; j < cEF.structuralMembers.slab[i].Count; j++)
                    {
                        if (cEF.structuralMembers.slab[i][j][0] == "Suspended Slab")
                        {
                            double quantity = double.Parse(cEF.structuralMembers.slab[i][j][2]);
                            double top = double.Parse(cEF.structuralMembers.slab[i][j][13]) / 1000;
                            double left = double.Parse(cEF.structuralMembers.slab[i][j][17]) / 1000;                            
                            totalarea += (top * left) * quantity;
                        }
                    }
                    double multip;
                    if (scafDim_h[2] == "2")
                    {
                        multip = 6.1;
                    }
                    else if (scafDim_h[2] == "3")
                    {
                        multip = 9.1;
                    }
                    else
                    {
                        multip = 12.1;
                    }
                    area[i] = totalarea;
                    form[i] = rounder((totalarea / (1.2 * 2.4)));
                    scaf[i] = rounder(((totalarea * multip) * 12) / (double.Parse(scafDim_h[0]) * double.Parse(scafDim_h[2]) * double.Parse(scafDim_h[4])));
                }
                cEF.structuralMembers.slab_area = area;
                cEF.structuralMembers.slab_form = form;
                cEF.structuralMembers.slab_scaf = scaf;                                            
            }
            catch (Exception ex)
            {
                print("Slab formworks: " + ex);
            }

            try
            {
                //Rebars slab on grade                
                List<List<List<string>>> slabONgrade = new List<List<List<string>>>();
                double slabWeight = 0;
                for (int i = 0; i < cEF.structuralMembers.slab.Count; i++)
                {
                    List<List<string>> slabholder = new List<List<string>>();
                    for (int j = 0; j < cEF.structuralMembers.slab[i].Count; j++)
                    {
                        if (cEF.structuralMembers.slab[i][j][0] == "Slab on Grade")
                        {
                            double top = double.Parse(cEF.structuralMembers.slab[i][j][10]);
                            double bot = double.Parse(cEF.structuralMembers.slab[i][j][12]);
                            double left = double.Parse(cEF.structuralMembers.slab[i][j][14]);
                            double right = double.Parse(cEF.structuralMembers.slab[i][j][16]);
                            double la = (top + bot) / 2;
                            double lb = (right + left) / 2;
                            double ccs = double.Parse(cEF.parameters.conc_CC_SG);
                            double qtyA = 0;
                            double qtyB = 0;
                            double longD = double.Parse(cEF.structuralMembers.slab[i][j][4]);
                            double transD = double.Parse(cEF.structuralMembers.slab[i][j][5]);
                            double longS = double.Parse(cEF.structuralMembers.slab[i][j][6]);
                            double transS = double.Parse(cEF.structuralMembers.slab[i][j][7]);
                            if (la >= lb)
                            {
                                qtyA = rounder(((lb - 2 * (ccs)) / longS) + 1);
                                qtyB = rounder(((la - 2 * (ccs)) / transS) + 1);
                            }
                            else
                            {
                                qtyA = rounder(((lb - 2 * (ccs)) / transS) + 1);
                                qtyB = rounder(((la - 2 * (ccs)) / longS) + 1);
                            }
                            string type_Long = cEF.structuralMembers.slab[i][j][8];
                            string type_Trans = cEF.structuralMembers.slab[i][j][9];
                            double sl_long = 0;
                            double sl_trans = 0;
                            if (type_Long == "Lapped Splice" || type_Long == "Welded Splice (Lap)")
                            {
                                string mix = spliceMixGetter(cEF.parameters.conc_CM_S_SOG_RM);
                                if (cEF.parameters.conc_cmIsSelected[3])
                                {
                                    if (cEF.parameters.conc_CM_S_SOG_CG.Equals("CLASS AA"))
                                    {
                                        mix = "27.6";
                                    }
                                    else if (cEF.parameters.conc_CM_S_SOG_CG.Equals("CLASS A"))
                                    {
                                        mix = "24.1";
                                    }
                                    else if (cEF.parameters.conc_CM_S_SOG_CG.Equals("CLASS B"))
                                    {
                                        mix = "17.2";
                                    }
                                    else
                                    {
                                        mix = "13.8";
                                    }
                                }
                                print(mix + " MIX");
                                int index = cEF.parameters.rein_LSL_TB_fc_list.IndexOf(mix);
                                if (index >= 0)
                                {
                                    for (int r = 0; r < cEF.parameters.rein_LSL_TB_dt.Rows.Count; r++)
                                    {
                                        if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == longD.ToString())
                                        {
                                            sl_long = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                            break;
                                        }
                                    }
                                }
                            }
                            if (type_Trans == "Lapped Splice" || type_Trans == "Welded Splice (Lap)")
                            {
                                string mix = spliceMixGetter(cEF.parameters.conc_CM_S_SOG_RM);
                                if (cEF.parameters.conc_cmIsSelected[3])
                                {
                                    if (cEF.parameters.conc_CM_S_SOG_CG.Equals("CLASS AA"))
                                    {
                                        mix = "27.6";
                                    }
                                    else if (cEF.parameters.conc_CM_S_SOG_CG.Equals("CLASS A"))
                                    {
                                        mix = "24.1";
                                    }
                                    else if (cEF.parameters.conc_CM_S_SOG_CG.Equals("CLASS B"))
                                    {
                                        mix = "17.2";
                                    }
                                    else
                                    {
                                        mix = "13.8";
                                    }
                                }
                                int index = cEF.parameters.rein_LSL_TB_fc_list.IndexOf(mix);
                                if (index >= 0)
                                {
                                    for (int r = 0; r < cEF.parameters.rein_LSL_TB_dt.Rows.Count; r++)
                                    {
                                        if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == transD.ToString())
                                        {
                                            sl_trans = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                            break;
                                        }
                                    }
                                }
                            }
                            double ELA = (la - (2 * (ccs))) / 1000;
                            double ELB = (lb - (2 * (ccs))) / 1000;
                            List<string> slabsBests = slabELPER(cEF, ELA, ELB, qtyA, qtyB, sl_long, sl_trans, la, lb);
                            slabsBests.Add(longD.ToString());
                            slabsBests.Add(transD.ToString());
                            double weightmultipLong = 0;
                            double weightmultipTrans = 0;
                            for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
                            {
                                if (weightmultipLong != 0 && weightmultipTrans != 0)
                                {
                                    break;
                                }
                                if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == (longD.ToString() + "mm"))
                                {
                                    weightmultipLong = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                                }
                                if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == (transD.ToString() + "mm"))
                                {
                                    weightmultipTrans = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                                }
                            }
                            if (la >= lb)
                            {
                                slabWeight += (weightmultipLong * double.Parse(slabsBests[0]) * double.Parse(slabsBests[1])
                                    + weightmultipTrans * double.Parse(slabsBests[2]) * double.Parse(slabsBests[3])) * double.Parse(cEF.floors[i].getValues()[0]);
                            }
                            else
                            {
                                slabWeight += (weightmultipLong * double.Parse(slabsBests[2]) * double.Parse(slabsBests[3])
                                    + weightmultipTrans * double.Parse(slabsBests[0]) * double.Parse(slabsBests[1])) * double.Parse(cEF.floors[i].getValues()[0]);
                            }
                            slabholder.Add(slabsBests);
                        }
                    }
                    slabONgrade.Add(slabholder);
                }
                cEF.structuralMembers.Slab_ongradeRebar = slabONgrade;
                cEF.structuralMembers.totalweightkgm_slabongrade = slabWeight;
            }
            catch(Exception ex)
            {
                print("Slab rebar on grade: " + ex);
            }            
            /// suspended slabs
            try
            {
                List<List<List<double>>> suspendedSlabRes = new List<List<List<double>>>();
                for (int i = 1; i < cEF.structuralMembers.slab.Count; i++)
                {
                    List<List<double>> slabholder = new List<List<double>>();
                    for (int j = 0; j < cEF.structuralMembers.slab[i].Count; j++)
                    {
                        if (cEF.structuralMembers.slab[i][j][0] == "Suspended Slab")
                        {
                            string connection = cEF.structuralMembers.slab[i][j][1];
                            string slab_dets = cEF.structuralMembers.slab[i][j][4];
                            double top_Len = 0;
                            double top_CL = 0;
                            double bottom_Len = 0;
                            double bottom_CL = 0;
                            double left_Len = 0;
                            double left_CL = 0;
                            double right_Len = 0;
                            double right_CL = 0;
                            if (slab_dets == "No. 1")
                            {
                                top_Len = double.Parse(cEF.structuralMembers.slab[i][j][12]);
                                top_CL = double.Parse(cEF.structuralMembers.slab[i][j][13]);
                                bottom_Len = double.Parse(cEF.structuralMembers.slab[i][j][14]);
                                bottom_CL = double.Parse(cEF.structuralMembers.slab[i][j][15]);
                                left_Len = double.Parse(cEF.structuralMembers.slab[i][j][16]);
                                left_CL = double.Parse(cEF.structuralMembers.slab[i][j][17]);
                                right_Len = double.Parse(cEF.structuralMembers.slab[i][j][18]);
                                right_CL = double.Parse(cEF.structuralMembers.slab[i][j][19]);
                            }
                            else 
                            {
                                top_Len = double.Parse(cEF.structuralMembers.slab[i][j][13]);
                                top_CL = double.Parse(cEF.structuralMembers.slab[i][j][14]);
                                bottom_Len = double.Parse(cEF.structuralMembers.slab[i][j][15]);
                                bottom_CL = double.Parse(cEF.structuralMembers.slab[i][j][16]);
                                left_Len = double.Parse(cEF.structuralMembers.slab[i][j][17]);
                                left_CL = double.Parse(cEF.structuralMembers.slab[i][j][18]);
                                right_Len = double.Parse(cEF.structuralMembers.slab[i][j][19]);
                                right_CL = double.Parse(cEF.structuralMembers.slab[i][j][20]);
                            }

                            //computations
                            double lh = ((top_Len + bottom_Len) / 2);
                            double lv = ((left_Len + right_Len) / 2);
                            double lch = ((top_CL + bottom_CL) / 2);
                            double lcv = ((left_CL + right_CL) / 2);
                            print("LH: " + lh);
                            print("LV: " + lv);
                            print("LCH: " + lch);
                            print("LCV: " + lcv);

                            List<string> schedtraits_hanlder = new List<string>();
                            bool there_is_connection = false;
                            for (int sched = 0; sched < cEF.structuralMembers.slabSchedule[i - 1].Count; sched++)
                            {
                                if (cEF.structuralMembers.slabSchedule[i - 1][sched][0] == connection)
                                {
                                    schedtraits_hanlder = cEF.structuralMembers.slabSchedule[i - 1][sched];
                                    there_is_connection = true;
                                    break;
                                }
                            }
                            if (there_is_connection)
                            {
                                double Bmid_top = double.Parse(schedtraits_hanlder[5]);
                                double Bmid_bot = double.Parse(schedtraits_hanlder[6]);
                                double Bint_top = double.Parse(schedtraits_hanlder[7]);
                                double Bint_bot = double.Parse(schedtraits_hanlder[8]);

                                double Rmid_top = double.Parse(schedtraits_hanlder[12]);
                                double Rmid_bot = double.Parse(schedtraits_hanlder[13]);
                                double Rint_top = double.Parse(schedtraits_hanlder[14]);
                                double Rint_bot = double.Parse(schedtraits_hanlder[15]);
                                

                                string str_w = schedtraits_hanlder[16];
                                double eks = 0;
                                if(str_w == "1-WAY")
                                {
                                    eks = 0;
                                }
                                else
                                {
                                    eks = 1;
                                }
                                double qtyVA = 0;
                                double qtyVB = 0;
                                double qtyHA = 0;
                                double qtyHB = 0;                                
                                if (slab_dets == "No. 1") // step 2 
                                {                                    
                                    double v = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                    if (lv >= lh)
                                    {
                                        if (v <= 0)
                                        {
                                            qtyVA = (lch / Bint_bot) + 1;
                                            qtyVB = (lch / Bmid_top) + 1;
                                            qtyHA = (lcv / Rint_bot) + 1;
                                            qtyHB = (lcv / Rmid_top) + 1;
                                        }
                                        else
                                        {
                                            qtyVA = (1 - (eks * v)) * ((lch / Bmid_bot) + 1);
                                            qtyVB = (lch / Bmid_top) + 1;
                                            qtyHA = (1 - v) * ((lcv / Rmid_bot) + 1);
                                            qtyHB = (lcv / Rmid_top) + 1;
                                        }
                                    }
                                    else
                                    {
                                        if (v <= 0)
                                        {
                                            qtyVA = (lch / Rint_bot) + 1;
                                            qtyVB = (lch / Rmid_top) + 1;
                                            qtyHA = (lcv / Bint_bot) + 1;
                                            qtyHB = (lcv / Bmid_top) + 1;
                                        }
                                        else
                                        {                                            
                                            qtyVA = (1 - v) * ((lch / Rmid_bot) + 1);
                                            qtyVB = (lch / Rmid_top) + 1;
                                            qtyHA = (1 - (eks * v)) * ((lcv / Bmid_bot) + 1);
                                            qtyHB = (lcv / Bmid_top) + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if(lv >= lh)
                                    {
                                        qtyVA = (lch / Bint_bot) + 1;
                                        qtyVB = (lch / Bmid_top) + 1;
                                        qtyHA = (lcv/ Rint_bot) + 1;
                                        qtyHB = (lcv / Rmid_top) + 1;
                                    }
                                    else
                                    {
                                        qtyVA = (lch / Rint_bot) + 1;
                                        qtyVB = (lch / Rmid_top) + 1;
                                        qtyHA = (lcv / Bint_bot) + 1;
                                        qtyHB = (lcv / Bmid_top) + 1;
                                    }
                                }
                                if (Double.IsInfinity(qtyVA))
                                {
                                    qtyVA = 0;
                                }
                                if (Double.IsInfinity(qtyVB))
                                {
                                    qtyVB = 0;
                                }
                                if (Double.IsInfinity(qtyHA))
                                {
                                    qtyHA = 0;
                                }
                                if (Double.IsInfinity(qtyHB))
                                {
                                    qtyHB = 0;
                                }
                                qtyVA = rounder(qtyVA);
                                qtyVB = rounder(qtyVB);
                                qtyHA = rounder(qtyHA);
                                qtyHB = rounder(qtyHB);
                                print(qtyVA + " :qty VA");
                                print(qtyVB + " :qty VB");
                                print(qtyHA + " :qty HA");
                                print(qtyHB + " :qty HB");
                                
                                double qtyVE = 0;
                                double qtyHE = 0;
                                if (slab_dets == "No. 1") // step 3
                                {
                                    double v = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                    if (lv >= lh)
                                    {
                                        if (v <= 0)
                                        {                                            
                                            qtyVE = (lch / Bmid_bot) - qtyVA + 1;
                                            qtyHE = (lcv / Rmid_bot) - qtyHA + 1;
                                        }
                                        else
                                        {
                                            qtyVE = (eks * v) * ((lch / Bmid_bot) + 1);
                                            qtyHE = v * ((lcv / Rmid_bot) + 1);
                                        }
                                    }
                                    else
                                    {
                                        if (v <= 0)
                                        {
                                            qtyVE = (lch / Rmid_bot) - qtyVA + 1;
                                            qtyHE = (lcv / Bmid_bot) - qtyHA + 1;//
                                        }
                                        else
                                        {
                                            qtyVE = v * ((lch / Rmid_bot) + 1);
                                            qtyHE = (eks * v) * ((lcv / Bmid_bot)+1);
                                        }
                                    }
                                }
                                else
                                {
                                    if (lv >= lh)
                                    {
                                        qtyVE = (lch / Bmid_bot) - qtyVA + 1;
                                        qtyHE = (lcv / Rmid_bot) - qtyHA + 1;
                                    }
                                    else
                                    {
                                        qtyVE = (lch / Rmid_bot) - qtyVA + 1;
                                        qtyHE = (lcv / Bmid_bot) - qtyHA + 1;
                                    }
                                }
                                if (Double.IsInfinity(qtyVE))
                                {
                                    qtyVE = 0;
                                }
                                if (Double.IsInfinity(qtyHE))
                                {
                                    qtyHE = 0;
                                }                                
                                qtyVE = rounder(qtyVE);
                                qtyHE = rounder(qtyHE);
                                print(qtyVE + " :qty VExtra");                                                                
                                print(qtyHE + " :qty HExtra");                                

                                //step 4
                                string slab_postion = cEF.structuralMembers.slab[i][j][3];
                                double Bext_top = double.Parse(schedtraits_hanlder[3]);
                                double Bext_bot = double.Parse(schedtraits_hanlder[4]);
                                double Rext_top = double.Parse(schedtraits_hanlder[10]);
                                double Rext_bot = double.Parse(schedtraits_hanlder[11]);
                                //Upper/Bottom Edge
                                double qtyVCUTA = 0;
                                double qtyVCUTB = 0;
                                double qtyHCUTA = 0;
                                double qtyHCUTB = 0;
                                if (slab_postion == "Upper/Bottom Edge") 
                                {
                                    if (slab_dets == "No. 1") // step 3
                                    {
                                        double v = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                        if (lv < lh)
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Rext_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Bint_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = 0;
                                                qtyVCUTB = v * ((lch / Rmid_bot) + 1);
                                                qtyHCUTA = (eks * v) * ((lcv / Bmid_bot) + 1);
                                                qtyHCUTB = (eks * v) * ((lcv / Bmid_bot) + 1);
                                            }
                                        }
                                        else
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch/Bext_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Rint_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = 0;
                                                qtyVCUTB = (eks * v) * ((lch/Bmid_bot) + 1);
                                                qtyHCUTA = v * ((lcv / Rmid_bot) + 1);
                                                qtyHCUTB = v * ((lcv / Rmid_bot) + 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lv < lh)
                                        {
                                            qtyVCUTA = (lch / Rext_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Bint_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                        }
                                        else
                                        {
                                            qtyVCUTA = (lch / Bext_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Rint_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                        }
                                    }
                                }
                                else if(slab_postion == "Left/Right Edge")//Left/Right Edge
                                {
                                    if (slab_dets == "No. 1") // step 3
                                    {
                                        double v = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                        if (lv < lh)
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Rint_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Bext_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = v * ((lch / Rmid_bot) + 1);
                                                qtyVCUTB = v * ((lch / Rmid_bot) + 1);
                                                qtyHCUTA = 0;
                                                qtyHCUTB = (eks * v) * ((lcv / Bmid_bot) + 1);
                                            }
                                        }
                                        else
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Bint_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Rext_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = (eks * v) * ((lch / Bmid_bot) + 1);
                                                qtyVCUTB = (eks * v) * ((lch / Bmid_bot) + 1);
                                                qtyHCUTA = 0;
                                                qtyHCUTB = v * ((lcv / Rmid_bot) + 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lv < lh)
                                        {
                                            qtyVCUTA = (lch / Rint_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Bext_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                        }
                                        else
                                        {
                                            qtyVCUTA = (lch / Bint_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Rext_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                        }
                                    }
                                }
                                else if (slab_postion == "Corner")//Corner
                                {
                                    if (slab_dets == "No. 1") // step 3
                                    {
                                        double v = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                        if (lv < lh)
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Rext_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Bext_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = 0;
                                                qtyVCUTB = v * ((lch / Rmid_bot) + 1);
                                                qtyHCUTA = 0;
                                                qtyHCUTB = (eks * v) * ((lcv / Bmid_bot) + 1);
                                            }
                                        }
                                        else
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Bext_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Rext_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = 0;
                                                qtyVCUTB = (eks * v) * ((lch / Bmid_bot) + 1);
                                                qtyHCUTA = 0;
                                                qtyHCUTB = v * ((lcv / Rmid_bot) + 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lv < lh)
                                        {
                                            qtyVCUTA = (lch / Rext_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Bext_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                        }
                                        else
                                        {
                                            qtyVCUTA = (lch / Bext_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Rext_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                        }
                                    }
                                }                                
                                else if (slab_postion == "Interior")//Interior
                                {
                                    if (slab_dets == "No. 1") // step 3
                                    {
                                        double v = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                        if (lv < lh)
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Rint_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Bint_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = v * ((lch / Rmid_bot) + 1);
                                                qtyVCUTB = v * ((lch / Rmid_bot) + 1);
                                                qtyHCUTA = (eks * v) * ((lcv / Bmid_bot) + 1);
                                                qtyHCUTB = (eks * v) * ((lcv / Bmid_bot) + 1);
                                            }
                                        }
                                        else
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Bint_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Rint_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = (eks * v) * ((lch / Bmid_bot) + 1);
                                                qtyVCUTB = (eks * v) * ((lch / Bmid_bot) + 1);
                                                qtyHCUTA = v * ((lcv / Rmid_bot) + 1);
                                                qtyHCUTB = v * ((lcv / Rmid_bot) + 1);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lv < lh)
                                        {
                                            qtyVCUTA = (lch / Rint_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Rint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Bint_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                        }
                                        else
                                        {
                                            qtyVCUTA = (lch / Bint_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Bint_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Rint_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                        }
                                    }
                                }
                                else if (slab_postion == "Isolated")//Isolated
                                {
                                    if (slab_dets == "No. 1") // step 3
                                    {
                                        double v = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                        if (lv < lh)
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Rext_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Rext_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Bext_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = 0;
                                                qtyVCUTB = 0;
                                                qtyHCUTA = 0;
                                                qtyHCUTB = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (v <= 0)
                                            {
                                                qtyVCUTA = (lch / Bext_bot) - qtyVB + 1;
                                                qtyVCUTB = (lch / Bext_bot) - qtyVB + 1;
                                                qtyHCUTA = (lcv / Rext_bot) - qtyHB + 1;
                                                qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                            }
                                            else
                                            {
                                                qtyVCUTA = 0;
                                                qtyVCUTB = 0;
                                                qtyHCUTA = 0;
                                                qtyHCUTB = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (lv < lh)
                                        {
                                            qtyVCUTA = (lch / Rext_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Rext_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Bext_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Bint_bot) - qtyHB + 1;
                                        }
                                        else
                                        {
                                            qtyVCUTA = (lch / Bext_bot) - qtyVB + 1;
                                            qtyVCUTB = (lch / Bext_bot) - qtyVB + 1;
                                            qtyHCUTA = (lcv / Rext_bot) - qtyHB + 1;
                                            qtyHCUTB = (lcv / Rint_bot) - qtyHB + 1;
                                        }
                                    }
                                }
                                if (Double.IsInfinity(qtyVCUTA))
                                {
                                    qtyVCUTA = 0;
                                }
                                if (Double.IsInfinity(qtyVCUTB))
                                {
                                    qtyVCUTB = 0;
                                }
                                if (Double.IsInfinity(qtyHCUTA))
                                {
                                    qtyHCUTA = 0;
                                }
                                if (Double.IsInfinity(qtyHCUTB))
                                {
                                    qtyHCUTB = 0;
                                }
                                qtyVCUTA = rounder(qtyVCUTA);
                                qtyVCUTB = rounder(qtyVCUTB);
                                qtyHCUTA = rounder(qtyHCUTA);
                                qtyHCUTB = rounder(qtyHCUTB);
                                print("qty VCUTA: " + qtyVCUTA);
                                print("qty VCUTB: " + qtyVCUTB);
                                print("qty HCUTA: " + qtyHCUTA);
                                print("qty HCUTB: " + qtyHCUTB);                                

                                //step 5
                                string top_long = "";
                                string top_trans = "";
                                string bot_long = "";
                                string bot_trans = "";
                                if (slab_dets == "No. 1")
                                {
                                    top_long = cEF.structuralMembers.slab[i][j][8];
                                    top_trans = cEF.structuralMembers.slab[i][j][9];
                                    bot_long = cEF.structuralMembers.slab[i][j][10];
                                    bot_trans = cEF.structuralMembers.slab[i][j][11];
                                }
                                else
                                {
                                    top_long = cEF.structuralMembers.slab[i][j][9];
                                    top_trans = cEF.structuralMembers.slab[i][j][10];
                                    bot_long = cEF.structuralMembers.slab[i][j][11];
                                    bot_trans = cEF.structuralMembers.slab[i][j][12];
                                }

                                //STEP 5
                                List<string> str_splices = new List<string>();
                                str_splices.Add(top_long);
                                str_splices.Add(top_trans);
                                str_splices.Add(bot_long);
                                str_splices.Add(bot_trans);
                                List<string> splice_equivalent = new List<string>();                                
                                for (int spl = 0; spl < str_splices.Count; spl++)
                                {
                                    string bar_size = "";
                                    if(spl == 0 || spl == 2)
                                    {
                                        bar_size = schedtraits_hanlder[2];
                                    }
                                    else
                                    {
                                        bar_size = schedtraits_hanlder[9];
                                    }                                    
                                    if (str_splices[spl] == "Lapped Splice" || str_splices[spl] == "Welded Splice (Lap)")
                                    {                                        
                                        string mix = spliceMixGetter(cEF.parameters.conc_CM_S_SS_RM);                                        
                                        if (cEF.parameters.conc_cmIsSelected[4])
                                        {
                                            if (cEF.parameters.conc_CM_S_SS_CG.Equals("CLASS AA"))
                                            {
                                                mix = "27.6";
                                            }
                                            else if (cEF.parameters.conc_CM_S_SS_CG.Equals("CLASS A"))
                                            {
                                                mix = "24.1";
                                            }
                                            else if (cEF.parameters.conc_CM_S_SS_CG.Equals("CLASS B"))
                                            {
                                                mix = "17.2";
                                            }
                                            else
                                            {
                                                mix = "13.8";
                                            }
                                        }
                                        print(mix + " MIX");
                                        int index = cEF.parameters.rein_LSL_TB_fc_list.IndexOf(mix);
                                        if (index >= 0)
                                        {
                                            for (int r = 0; r < cEF.parameters.rein_LSL_TB_dt.Rows.Count; r++)
                                            {
                                                if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == bar_size)
                                                {
                                                    splice_equivalent.Add(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            splice_equivalent.Add("0");
                                        }
                                    }
                                    else
                                    {
                                        splice_equivalent.Add("0");
                                    }
                                }

                                double slv_bot = 0;
                                double slv_top = 0;
                                double slh_bot = 0;
                                double slh_top = 0;

                                double equi_top_long = double.Parse(splice_equivalent[0]);
                                double equi_top_trans = double.Parse(splice_equivalent[1]);
                                double equi_bot_long = double.Parse(splice_equivalent[2]);                                
                                double equi_bot_trans = double.Parse(splice_equivalent[3]);

                                if (lv >= lh)
                                {
                                    slv_bot = equi_bot_long;
                                    slv_top = equi_top_long;
                                    slh_bot = equi_bot_trans;
                                    slh_top = equi_top_trans;
                                }
                                else
                                {
                                    slv_bot = equi_bot_trans;
                                    slv_top = equi_top_trans;
                                    slh_bot = equi_bot_long;
                                    slh_top = equi_top_long;
                                }
                                print("slv bot: " + slv_bot);
                                print("slv top: " + slv_top);
                                print("slh bot: " + slh_bot);
                                print("slh top: " + slh_top);
                                //STEP 6
                                double cc_beam_weather = double.Parse(cEF.parameters.conc_CC_BEW);
                                double hook_lh = 0;
                                double hook_lv = 0;
                                double lv_contA = 0;
                                double lv_contB = 0;
                                double lh_contA = 0;
                                double lh_contB = 0;
                                if (lv > lh)
                                {
                
                                    for (int r = 0; r < cEF.parameters.rein_BEH_MB_dt.Rows.Count; r++)
                                    {
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == schedtraits_hanlder[9])
                                        {
                                            hook_lh = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][1].ToString());
                                        }
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == schedtraits_hanlder[2])
                                        {
                                            hook_lv = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][1].ToString());
                                        }
                                    }                                    
                                }
                                else
                                {
                                    for (int r = 0; r < cEF.parameters.rein_BEH_MB_dt.Rows.Count; r++)
                                    {
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == schedtraits_hanlder[9])
                                        {
                                            hook_lv = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][1].ToString());
                                        }
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == schedtraits_hanlder[2])
                                        {
                                            hook_lh = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][1].ToString());
                                        }
                                    }
                                }

                                if (slab_postion == "Upper/Bottom Edge")
                                {
                                    lv_contA = (hook_lv + (2 * lv) + (0.5 * slv_bot) - lcv - cc_beam_weather) / 1000;
                                    lv_contB = (hook_lv + (2 * lv) + (0.5 * slv_top) - lcv - cc_beam_weather) / 1000;
                                    lh_contA = ((2 * lh) + slh_bot - lch) / 1000;
                                    lh_contB = ((2 * lh) + slh_top - lch) / 1000;
                                }
                                else if (slab_postion == "Left/Right Edge")//Left/Right Edge
                                {
                                    lv_contA = ((2 * lv) + slv_bot - lcv) / 1000;
                                    lv_contB = ((2 * lv) + slv_top - lcv) / 1000;
                                    lh_contA = (hook_lh + (2 * lh) + (0.5 * slh_bot) - lch - cc_beam_weather) / 1000;
                                    lh_contB = (hook_lh + (2 * lh) + (0.5 * slh_top) - lch - cc_beam_weather) / 1000;
                                }
                                else if (slab_postion == "Corner")//Corner
                                {
                                    lv_contA = (hook_lv + (2 * lv) + (0.5 * slv_bot) - lcv - cc_beam_weather) / 1000;
                                    lv_contB = (hook_lv + (2 * lv) + (0.5 * slv_top) - lcv - cc_beam_weather) / 1000;
                                    lh_contA = (hook_lh + (2 * lh) + (0.5 * slh_bot) - lch - cc_beam_weather) / 1000;
                                    lh_contB = (hook_lh + (2 * lh) + (0.5 * slh_top) - lch - cc_beam_weather) / 1000;
                                }
                                else if (slab_postion == "Interior")//Interior
                                {
                                    lv_contA = ((2 * lv) + slv_bot - lcv) / 1000;//
                                    lv_contB = ((2 * lv) + slv_top - lcv) / 1000;//
                                    lh_contA = ((2 * lh) + slh_bot - lch) / 1000;
                                    lh_contB = ((2 * lh) + slh_top - lch) / 1000;
                                }
                                else if (slab_postion == "Isolated")//Isolated
                                {
                                    lv_contA = ((2 * hook_lv) + (2 * lv) - lcv - (2 * cc_beam_weather)) / 1000;
                                    lv_contB = ((2 * hook_lv) + (2 * lv) - lcv - (2 * cc_beam_weather)) / 1000;
                                    lh_contA = ((2 * hook_lh) + (2 * lh) - lch - (2 * cc_beam_weather)) / 1000;
                                    lh_contB = ((2 * hook_lh) + (2 * lh) - lch - (2 * cc_beam_weather)) / 1000;
                                }
                                if (Double.IsInfinity(lv_contA))
                                {
                                    lv_contA = 0;
                                }
                                if (Double.IsInfinity(lv_contB))
                                {
                                    lv_contB = 0;
                                }
                                if (Double.IsInfinity(lh_contA))
                                {
                                    lh_contA = 0;
                                }
                                if (Double.IsInfinity(lh_contB))
                                {
                                    lh_contB = 0;
                                }
                                print("lv contA: " + lv_contA);
                                print("lv contB: " + lv_contB);
                                print("lh contA: " + lh_contA);
                                print("lh contB: " + lh_contB);                                

                                //STEP 7
                                double lv_extra_rebars = 0;                                                                
                                double lh_extra_rebars = 0;
                                double thicc = double.Parse(schedtraits_hanlder[1]);
                                double cc_suspended_slab = double.Parse(cEF.parameters.conc_CC_SS);
                                if(slab_dets == "No. 1")
                                {
                                    if (slab_postion == "Upper/Bottom Edge")
                                    {
                                        lv_extra_rebars = (hook_lv + (1.5 * lv) + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1) - (0.5 * lcv) - cc_beam_weather) / 1000;
                                        lh_extra_rebars = (lh + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1)) / 1000;
                                    }
                                    else if (slab_postion == "Left/Right Edge")//Left/Right Edge
                                    {
                                        lv_extra_rebars = (lv + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1)) / 1000;
                                        lh_extra_rebars = (hook_lh + (1.5 * lh) + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1) - (0.5 * lch) - cc_beam_weather) / 1000;
                                    }
                                    else if (slab_postion == "Corner")//Corner
                                    {
                                        lv_extra_rebars = (hook_lv + (1.5 * lv) + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1) - (0.5 * lcv) - cc_beam_weather) / 1000;
                                        lh_extra_rebars = (hook_lh + (1.5 * lh) + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1) - (0.5 * lch) - cc_beam_weather) / 1000;
                                    }
                                    else if (slab_postion == "Interior")//Interior
                                    {
                                        lv_extra_rebars = (lv + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1)) / 1000;
                                        lh_extra_rebars = (lh + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1)) / 1000;
                                    }
                                    else if (slab_postion == "Isolated")//Isolated
                                    {
                                        lv_extra_rebars = ((2 * hook_lv) + (2 * lv) + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1) - lcv - (2 * cc_beam_weather)) / 1000;
                                        lh_extra_rebars = ((2 * hook_lh) + (2 * lh) + ((2 * thicc) - (4 * cc_suspended_slab)) * (Math.Sqrt(2) - 1) - lch - (2 * cc_beam_weather)) / 1000;
                                    }
                                }
                                else
                                {
                                    double no2_la = double.Parse(cEF.structuralMembers.slab[i][j][5]);
                                    double no2_lb = double.Parse(cEF.structuralMembers.slab[i][j][6]);
                                    lv_extra_rebars = lcv * (1 - no2_la - no2_lb);
                                    lh_extra_rebars = lch* (1 - no2_la - no2_lb);                                    
                                }
                                lv_extra_rebars = Math.Round(lv_extra_rebars, 2);
                                lh_extra_rebars = Math.Round(lh_extra_rebars, 2);
                                if (Double.IsInfinity(lv_extra_rebars))
                                {
                                    lv_extra_rebars = 0;
                                }
                                if (Double.IsInfinity(lh_extra_rebars))
                                {
                                    lh_extra_rebars = 0;
                                }
                                print("lv extra rebars: " + lv_extra_rebars);
                                print("lh extra rebars: " + lh_extra_rebars);
                                
                                //STEP 8
                                double lv_cut_A = 0;
                                double lv_cut_B = 0;
                                double lh_cut_A = 0;
                                double lh_cut_B = 0;

                                double z1 = 0;
                                double z2 = 0;
                                if (slab_dets == "No. 1")
                                {
                                    z1 = reinGetter(cEF.structuralMembers.slab[i][j][5]);
                                    z2 = reinGetter(cEF.structuralMembers.slab[i][j][6]);
                                }
                                else
                                {
                                    z1 = reinGetter(cEF.structuralMembers.slab[i][j][7]);
                                    z2 = reinGetter(cEF.structuralMembers.slab[i][j][8]);
                                }
                                if (slab_postion == "Upper/Bottom Edge")
                                {
                                    lv_cut_A = (hook_lv + lcv * (z1 - 1) + lv - cc_beam_weather)/1000;
                                    lv_cut_B = (lcv * (z2 - 0.5) + (0.5 * lv)) / 1000;
                                    lh_cut_A = (lch * (z1 - 0.5) + (0.5 * lh)) / 1000;
                                    lh_cut_B = (lch * (z2 - 0.5) + (0.5 * lh)) / 1000;
                                }
                                else if (slab_postion == "Left/Right Edge")//Left/Right Edge
                                {
                                    lv_cut_A = (lcv * (z1 - 0.5) + (0.5 * lv)) / 1000;
                                    lv_cut_B = (lcv * (z2 - 0.5) + (0.5 * lv)) / 1000;
                                    lh_cut_A = (hook_lh + lch * (z1 - 1) + lh - cc_beam_weather) / 1000;
                                    lh_cut_B = (lch * (z2 - 0.5) + (0.5 * lh)) / 1000;
                                }
                                else if (slab_postion == "Corner")//Corner
                                {
                                    lv_cut_A = (hook_lv + lcv * (z1 - 1) + lv - cc_beam_weather) / 1000;
                                    lv_cut_B = (lcv * (z2 - 0.5) + (0.5 * lv)) / 1000;
                                    lh_cut_A = (hook_lh + lch * (z1 - 1) + lh - cc_beam_weather) / 1000;
                                    lh_cut_B = (lch * (z2 - 0.5) + (0.5 * lh)) / 1000;
                                }
                                else if (slab_postion == "Interior")//Interior
                                {
                                    lv_cut_A = (lcv * (z1 - 0.5) + (0.5 * lv)) / 1000;
                                    lv_cut_B = (lcv * (z2 - 0.5) + (0.5 * lv)) / 1000;
                                    lh_cut_A = (lch * (z1 - 0.5) + (0.5 * lh)) / 1000;
                                    lh_cut_B = (lch * (z2 - 0.5) + (0.5 * lh)) / 1000;
                                }
                                else if (slab_postion == "Isolated")//Isolated
                                {
                                    lv_cut_A = (hook_lv + lcv * (z1 - 1) + lv - cc_beam_weather) / 1000;
                                    lv_cut_B = (hook_lv + lcv * (z2 - 1) + lv - cc_beam_weather) / 1000;
                                    lh_cut_A = (hook_lh + lch * (z1 - 1) + lh - cc_beam_weather) / 1000;
                                    lh_cut_B = (hook_lh + lch * (z2 - 1) + lh - cc_beam_weather) / 1000;
                                }
                                if (Double.IsInfinity(lv_cut_A))
                                {
                                    lv_cut_A = 0;
                                }
                                if (Double.IsInfinity(lv_cut_B))
                                {
                                    lv_cut_B = 0;
                                }
                                if (Double.IsInfinity(lh_cut_A))
                                {
                                    lh_cut_A = 0;
                                }
                                if (Double.IsInfinity(lh_cut_B))
                                {
                                    lh_cut_B = 0;
                                }
                                print("lv cut_A: " + lv_cut_A);
                                print("lv cut_B: " + lv_cut_B);
                                print("lh cut_A: " + lh_cut_A);
                                print("lh cut_B: " + lh_cut_B);

                                //STEP 9
                                double SLAB_quantity = double.Parse(cEF.structuralMembers.slab[i][j][2]);
                                List<double> v_lens = new List<double>();//Vs
                                v_lens.Add(lv_contA);
                                v_lens.Add(lv_contB);
                                v_lens.Add(lv_extra_rebars);
                                v_lens.Add(lv_cut_A);
                                v_lens.Add(lv_cut_B);

                                List<double> v_quantities = new List<double>();//Vs
                                v_quantities.Add(qtyVA);
                                v_quantities.Add(qtyVB);
                                v_quantities.Add(qtyVE);
                                v_quantities.Add(qtyVCUTA);
                                v_quantities.Add(qtyVCUTB);

                                List<double> h_lens = new List<double>();//Hs
                                h_lens.Add(lh_contA);
                                h_lens.Add(lh_contB);
                                h_lens.Add(lh_extra_rebars);
                                h_lens.Add(lh_cut_A);
                                h_lens.Add(lh_cut_B);

                                List<double> h_quantities = new List<double>();//Vs
                                h_quantities.Add(qtyHA);
                                h_quantities.Add(qtyHB);
                                h_quantities.Add(qtyHE);
                                h_quantities.Add(qtyHCUTA);
                                h_quantities.Add(qtyHCUTB);                                
                                
                                List<double> vs1 = new List<double>(); // v first
                                List<double> hs1 = new List<double>(); // h first
                                if (lv < lh)
                                {
                                    vs1.Add(double.Parse(schedtraits_hanlder[9]));
                                    hs1.Add(double.Parse(schedtraits_hanlder[2]));
                                }
                                else
                                {
                                    vs1.Add(double.Parse(schedtraits_hanlder[2]));
                                    hs1.Add(double.Parse(schedtraits_hanlder[9]));
                                }
                                List<double> vs2 = rebarsSuspendedSlabElper(v_lens, v_quantities, SLAB_quantity, cEF);
                                var vs = vs1.Concat(vs2);
                                List<double> vs_result = vs.ToList();

                                List<double> hs2 = rebarsSuspendedSlabElper(h_lens, h_quantities, SLAB_quantity, cEF);
                                var hs = hs1.Concat(hs2);
                                List<double> hs_result = hs.ToList();
                                slabholder.Add(vs_result);
                                slabholder.Add(hs_result);
                            }
                        }
                    }
                    suspendedSlabRes.Add(slabholder);
                }
                cEF.structuralMembers.Slab_suspendedRebar = suspendedSlabRes;

                foreach (var a in suspendedSlabRes)
                {
                    print("FLOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOR");
                    foreach (var b in a)
                    {
                        print("OTHEEEEEEEEEEEEEEEEEEEEEEEEEEER");
                        foreach (var c in b)
                        {
                            print(c + " -");
                        }
                    }
                }

                double suspended_slab_weight = 0;
                for (int i = 0; i < suspendedSlabRes.Count; i++)
                {
                    for (int j = 0; j < suspendedSlabRes[i].Count; j++)
                    {
                        double diameter = 0;
                        for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
                        {
                            if (diameter!= 0)
                            {
                                break;
                            }
                            if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == (suspendedSlabRes[i][j][0] + "mm"))
                            {
                                diameter = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                            }                            
                        }
                        for (int n = 1; n < suspendedSlabRes[i][j].Count; n += 2)
                        {
                            suspended_slab_weight += ((suspendedSlabRes[i][j][n] * suspendedSlabRes[i][j][n+1]) * diameter) * double.Parse(cEF.floors[i+1].getValues()[0]);
                        }
                        
                    }
                }
                cEF.structuralMembers.totalweightkgm_suspendedslab = suspended_slab_weight;
                print("Suspended Slab weight: " + suspended_slab_weight);
                


            }
            catch (Exception ex)
            {
                print("Suspended slab rebar: " + ex);
            }
            

            refreshSolutions(cEF);
        }
        //Slab Computation Functions -- END

        //Stairs Computation Functions -- START
        public void AddStairsWorks(CostEstimationForm cEF, int floorCount, int stairsCount)
        {
            List<double> newList = new List<double>();            
            cEF.structuralMembers.concreteWorkSolutionsST[floorCount].Add(newList);

            List<double[,]> newList2 = new List<double[,]>();
            cEF.structuralMembers.stairs_Rebar[floorCount].Add(newList2);

            stairsWorks(cEF, floorCount, stairsCount);
        }

        public async void stairsWorks(CostEstimationForm cEF, int floorCount, int stairsCount)
        {
            if (cEF.structuralMembers.stairs[floorCount][stairsCount][0].Equals("Straight Stairs"))
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_ST_CG;

                //Variables from StructMem
                double quantity, steps, SL, riser, tread, Thc;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][1], System.Globalization.CultureInfo.InvariantCulture);
                steps = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][2], System.Globalization.CultureInfo.InvariantCulture);
                SL = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][3], System.Globalization.CultureInfo.InvariantCulture);
                riser = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][4], System.Globalization.CultureInfo.InvariantCulture);
                tread = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][5], System.Globalization.CultureInfo.InvariantCulture);
                Thc = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][6], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[5])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }

                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                }

                else
                {
                    double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                    while (WSL % 25 != 0)
                    {
                        WSL++;
                    }
                    double volumeWSF = SL * WSL * Thc;
                    double volumeSteps = (riser * tread) / 2 * SL * steps;
                    double volume = volumeWSF + volumeSteps;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                        volume * quantity);
                }

                //kev straight                
                double WaistSlab1, WaistSlab2, WaistSlab3, WaistSlab4, TempBars, Landing,
                       WS_BarNumber, LShaped_num, LShaped_Length, NoseBar;

                double Lapping, Length;
                double LapLength, HookLength, WS_Diameter, Riser, Tread, Steps,
                       WidthLanding, WS_Spacing, StairWidth, BeamWidth
                       ;

                string[] stairParameterValues = cEF.parameters.stair[floorCount][stairsCount].getValues();
                LapLength = double.Parse(stairParameterValues[1]);
                HookLength = double.Parse(stairParameterValues[2]);
                WS_Diameter = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][7], System.Globalization.CultureInfo.InvariantCulture);
                Riser = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][4], System.Globalization.CultureInfo.InvariantCulture);
                Tread = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][5], System.Globalization.CultureInfo.InvariantCulture);
                Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][2], System.Globalization.CultureInfo.InvariantCulture);
                WS_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][8], System.Globalization.CultureInfo.InvariantCulture);
                StairWidth = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][3], System.Globalization.CultureInfo.InvariantCulture);
                BeamWidth = double.Parse(stairParameterValues[3]);
                //==========Calculation===========
                Lapping = LapLength * WS_Diameter;
                Length = nearestValue(Math.Sqrt(Math.Pow(Riser * Steps, 2) + Math.Pow(Tread * Steps, 2)), 25);
                WaistSlab1 = nearestValue(Lapping + Length + (HookLength * WS_Diameter), 25);
                WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + StairWidth + Length, 25);
                WaistSlab3 = nearestValue((HookLength * WS_Diameter) + StairWidth + BeamWidth + (LapLength * 10), 25);
                WaistSlab4 = rounder(2 * ((Length / WS_Spacing) + 1));
                double bar_pcs = rounder((StairWidth / WS_Spacing) + 1);
                //Steps Rebar
                LShaped_num = rounder((StairWidth / WS_Spacing) + 1) * Steps;
                LShaped_Length = Tread + Riser;
                NoseBar = StairWidth;

                double main_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][7]);
                double temp_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][9]);
                double chair_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][11]);
                double nose_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][13]);                

                double[,] SwaistSlab1 = stairsRebarsElper(cEF, WaistSlab1, bar_pcs, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab1);
                double[,] SwaistSlab2 = stairsRebarsElper(cEF, WaistSlab2, bar_pcs, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab2);
                double[,] SwaistSlab3 = stairsRebarsElper(cEF, WaistSlab3, bar_pcs, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab3);
                double[,] SdistBars = stairsRebarsElper(cEF, StairWidth, WaistSlab4, temp_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SdistBars);
                double[,] chairBars = stairsRebarsElper(cEF, LShaped_Length, LShaped_num, chair_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(chairBars);
                double[,] noseBars = stairsRebarsElper(cEF, StairWidth, Steps, nose_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(noseBars);

                string[] stringArray = cEF.parameters.stair[floorCount][stairsCount].getValues();
                stringArray[5] = bestLMSgetter(SwaistSlab1);
                stringArray[6] = bestLMSgetter(SwaistSlab2);
                stringArray[7] = bestLMSgetter(SwaistSlab3);
                stringArray[8] = bestLMSgetter(SdistBars);
                stringArray[9] = bestLMSgetter(chairBars);
                stringArray[10] = bestLMSgetter(noseBars);
                cEF.parameters.stair[floorCount][stairsCount].setStraightStairsValues(stringArray[1], stringArray[2], stringArray[3],
                            stringArray[4], stringArray[5], stringArray[6], stringArray[7], stringArray[8], stringArray[9], stringArray[10]);
            }
            else if (cEF.structuralMembers.stairs[floorCount][stairsCount][0].Equals("L-Stairs"))
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_ST_CG;
                
                //Variables from StructMem
                double quantity, stepsFF, steps2FF, SL, riser, tread, wsThc, landingThc;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][1], System.Globalization.CultureInfo.InvariantCulture);
                stepsFF = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][2], System.Globalization.CultureInfo.InvariantCulture);
                steps2FF = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][3], System.Globalization.CultureInfo.InvariantCulture);
                SL = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][4], System.Globalization.CultureInfo.InvariantCulture);
                riser = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][5], System.Globalization.CultureInfo.InvariantCulture);
                tread = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][6], System.Globalization.CultureInfo.InvariantCulture);
                wsThc = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][7], System.Globalization.CultureInfo.InvariantCulture);
                landingThc = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][8], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[5])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                }
                else
                {
                    double steps = stepsFF + steps2FF;
                    double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                    while (WSL1 % 25 != 0)
                    {
                        WSL1++;
                    }
                    double volumeWSF1 = SL * WSL1 * wsThc;
                    double volumeSteps = (riser * tread) / 2 * SL * steps;
                    double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                    while (WSL2 % 25 != 0)
                    {
                        WSL2++;
                    }
                    double volumeWSF2 = SL * WSL2 * wsThc;
                    double landing = (SL * SL) * landingThc;
                    double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                        volume * quantity);
                }

                //kev L

                double F_WaistSlab1, F_WaistSlab2, F_WaistSlab3, F_WaistSlab4, TempBars,
                       Landing_Length, Landing_Number, Landing_Spacing, TB_Spacing,
                       F_WS_BarNumber,
                       S_WaistSlab1, S_WaistSlab2, S_WaistSlab3, S_WaistSlab4,
                       S_WS_BarNumber, CB_Length, CB_Number;
                double Lapping, F_Length, S_Length;
                double LapLength, HookLength, WS_Diameter,
                       Tread, Riser, F_Steps, S_Steps,
                       WidthLanding, StairWidth, BeamWidth,
                       WS_Spacing, CB_Spacing;

                string[] stairParameterValues = cEF.parameters.stair[floorCount][stairsCount].getValues();


                LapLength = double.Parse(stairParameterValues[1]);
                WS_Diameter = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][9], System.Globalization.CultureInfo.InvariantCulture);
                Riser = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][5], System.Globalization.CultureInfo.InvariantCulture);
                Tread = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][6], System.Globalization.CultureInfo.InvariantCulture);
                F_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][2], System.Globalization.CultureInfo.InvariantCulture);
                S_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][3], System.Globalization.CultureInfo.InvariantCulture);
                HookLength = double.Parse(stairParameterValues[2]);
                StairWidth = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][4], System.Globalization.CultureInfo.InvariantCulture);
                BeamWidth = double.Parse(stairParameterValues[3]);
                WS_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][10], System.Globalization.CultureInfo.InvariantCulture);
                Landing_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][14], System.Globalization.CultureInfo.InvariantCulture);
                CB_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][16], System.Globalization.CultureInfo.InvariantCulture);
                TB_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][12], System.Globalization.CultureInfo.InvariantCulture);
                //==========FIRST FLIGHT===========
                Lapping = LapLength * WS_Diameter;
                F_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * F_Steps, 2) + Math.Pow(Tread * F_Steps, 2)), 25);
                F_WaistSlab1 = nearestValue(Lapping + F_Length + (HookLength * WS_Diameter), 25);
                F_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + StairWidth + F_Length, 25);
                F_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + StairWidth + BeamWidth + (LapLength * WS_Diameter), 25);
                F_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);
                F_WaistSlab4 = rounder(2 * ((F_Length / TB_Spacing) + 1));
                Landing_Length = StairWidth;
                Landing_Number = (StairWidth / Landing_Spacing) + 1;

                //=========SECOND FLIGHT===========
                S_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * S_Steps, 2) + Math.Pow(Tread * S_Steps, 2)), 25);
                S_WaistSlab1 = nearestValue(Lapping + S_Length + (HookLength * WS_Diameter), 25);
                S_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + StairWidth + S_Length, 25);
                S_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + StairWidth + BeamWidth + (LapLength * WS_Diameter), 25);
                S_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);
                S_WaistSlab4 = rounder(2 * ((S_Length / TB_Spacing) + 1));
                CB_Length = Tread + Riser;
                CB_Number = rounder((StairWidth / CB_Spacing) + 1) * (F_Steps + S_Steps);//F_Steps + S_Steps;                
                double S_NoseBar = F_Steps + S_Steps;
                //

                double main_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][9]);
                double temp_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][11]);
                double landing_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][13]);
                double chair_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][15]);
                double nose_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][17]);

                //For solutions on structural members                
                double[,] FwaistSlab1 = stairsRebarsElper(cEF, F_WaistSlab1, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FwaistSlab1);
                double[,] FwaistSlab2 = stairsRebarsElper(cEF, F_WaistSlab2, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FwaistSlab2);
                double[,] FwaistSlab3 = stairsRebarsElper(cEF, F_WaistSlab3, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FwaistSlab3);
                double[,] SwaistSlab1 = stairsRebarsElper(cEF, S_WaistSlab1, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab1);
                double[,] SwaistSlab2 = stairsRebarsElper(cEF, S_WaistSlab2, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab2);
                double[,] SwaistSlab3 = stairsRebarsElper(cEF, S_WaistSlab3, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab3);
                double[,] FdistBars = stairsRebarsElper(cEF, StairWidth, (F_WaistSlab4 + S_WaistSlab4), temp_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FdistBars);
                double[,] Flanding = stairsRebarsElper(cEF, Landing_Length, Landing_Number, landing_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(Flanding);
                //                
                double[,] chairBars = stairsRebarsElper(cEF, CB_Length, CB_Number, chair_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(chairBars);
                double[,] noseBars = stairsRebarsElper(cEF, StairWidth, S_NoseBar, nose_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(noseBars);

                string[] stringArray = cEF.parameters.stair[floorCount][stairsCount].getValues();
                stringArray[5] = bestLMSgetter(FwaistSlab1);
                stringArray[6] = bestLMSgetter(FwaistSlab2);
                stringArray[7] = bestLMSgetter(FwaistSlab3);
                stringArray[8] = bestLMSgetter(SwaistSlab1);
                stringArray[9] = bestLMSgetter(SwaistSlab2);
                stringArray[10] = bestLMSgetter(SwaistSlab3);
                stringArray[11] = bestLMSgetter(FdistBars);
                stringArray[12] = bestLMSgetter(Flanding);
                stringArray[13] = bestLMSgetter(chairBars);
                stringArray[14] = bestLMSgetter(noseBars);
                cEF.parameters.stair[floorCount][stairsCount].setLStairsValues(stringArray[1], stringArray[2], stringArray[3],
                            stringArray[4], stringArray[5], stringArray[6], stringArray[7], stringArray[8], stringArray[9],
                            stringArray[10], stringArray[11], stringArray[12], stringArray[13], stringArray[14]);
            }
            else
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_ST_CG;

                //Variables from StructMem
                double quantity, stepsFF, steps2FF, SL, riser, tread, wsThc, landingW, gap, landingThc;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][1], System.Globalization.CultureInfo.InvariantCulture);
                stepsFF = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][2], System.Globalization.CultureInfo.InvariantCulture);
                steps2FF = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][3], System.Globalization.CultureInfo.InvariantCulture);
                SL = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][4], System.Globalization.CultureInfo.InvariantCulture);
                riser = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][5], System.Globalization.CultureInfo.InvariantCulture);
                tread = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][6], System.Globalization.CultureInfo.InvariantCulture);
                wsThc = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][7], System.Globalization.CultureInfo.InvariantCulture);
                landingW = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][8], System.Globalization.CultureInfo.InvariantCulture);
                gap = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][9], System.Globalization.CultureInfo.InvariantCulture);
                landingThc = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][10], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[5])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                    else
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * quantity);
                    }
                }
                else
                {
                    double steps = stepsFF + steps2FF;
                    double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                    while (WSL1 % 25 != 0)
                    {
                        WSL1++;
                    }
                    double volumeWSF1 = SL * WSL1 * wsThc;
                    double volumeSteps = (riser * tread) / 2 * SL * steps;
                    double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                    while (WSL2 % 25 != 0)
                    {
                        WSL2++;
                    }
                    double volumeWSF2 = SL * WSL2 * wsThc;
                    double landing = ((SL * 2) + gap) * landingW * landingThc;
                    double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                        volume * quantity);
                }

                //kev u 
                //Vars  

                double F_WaistSlab1, F_WaistSlab2, F_WaistSlab3, F_TempBars, F_Landing,
                       F_WS_BarNumber, F_L_BarNumber,
                       S_WaistSlab1, S_WaistSlab2, S_WaistSlab3, S_TempBars, S_Landing,
                       S_WS_BarNumber, S_ChairBarsLength, S_ChairBarNumber,
                       S_NoseBar;
                double Lapping, F_Length, S_Length;

                string[] stairParameterValues = cEF.parameters.stair[floorCount][stairsCount].getValues();


                double Riser, F_Steps, S_Steps, Tread,
                       LapLength,
                       HookLength, WS_Diameter, WS_Spacing,
                       WidthLanding,
                       BeamWidth,
                       StairWidth, TB_Spacing,
                       Gap,
                       L_MainBars, L_Spacing,
                       C_Spacing
                       ;

                //ParameterValues
                HookLength = double.Parse(stairParameterValues[2]);
                LapLength = double.Parse(stairParameterValues[1]);
                BeamWidth = double.Parse(stairParameterValues[3]);

                //AddStructValues
                WS_Diameter = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][11], System.Globalization.CultureInfo.InvariantCulture);
                WS_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][12], System.Globalization.CultureInfo.InvariantCulture);
                WidthLanding = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][8], System.Globalization.CultureInfo.InvariantCulture);
                Riser = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][5], System.Globalization.CultureInfo.InvariantCulture);
                Tread = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][6], System.Globalization.CultureInfo.InvariantCulture);
                F_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][2], System.Globalization.CultureInfo.InvariantCulture);
                S_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][3], System.Globalization.CultureInfo.InvariantCulture);
                StairWidth = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][4], System.Globalization.CultureInfo.InvariantCulture);
                TB_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][14], System.Globalization.CultureInfo.InvariantCulture);
                Gap = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][9], System.Globalization.CultureInfo.InvariantCulture);
                L_MainBars = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][15], System.Globalization.CultureInfo.InvariantCulture);
                L_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][16], System.Globalization.CultureInfo.InvariantCulture);
                C_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][18], System.Globalization.CultureInfo.InvariantCulture);
                //===============FIRST FLIGHT =============================//
                //CalculatedValues 1 -> 5
                Lapping = LapLength * WS_Diameter;
                F_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * F_Steps, 2) + Math.Pow(Tread * F_Steps, 2)), 25);

                F_WaistSlab1 = nearestValue(Lapping + F_Length + (HookLength * WS_Diameter), 25);
                F_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + WidthLanding + F_Length, 25);
                F_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + WidthLanding + BeamWidth + (LapLength * WS_Diameter), 25);
                F_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);                
                F_TempBars = rounder(2 * ((F_Length / TB_Spacing) + 1));                
                F_Landing = (StairWidth * 2) + Gap;
                F_L_BarNumber = rounder((WidthLanding / L_Spacing) + 1);

                //===============SECOND FLIGHT =============================//
                //CalculatedValues 6 -> n
                S_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * S_Steps, 2) + Math.Pow(Tread * S_Steps, 2)), 25);

                S_WaistSlab1 = nearestValue(Lapping + S_Length + (HookLength * WS_Diameter), 25);
                S_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + WidthLanding + S_Length, 25);
                S_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + WidthLanding + BeamWidth + (LapLength * WS_Diameter), 25);
                S_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);                
                S_TempBars = rounder(2 * ((S_Length / TB_Spacing) + 1));                
                //===========Steps========================
                S_ChairBarsLength = Tread + Riser;
                S_ChairBarNumber = rounder((StairWidth / C_Spacing) + 1) * (F_Steps + S_Steps);
                S_NoseBar = (F_Steps + S_Steps);
                              

                double main_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][11]);
                double temp_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][13]);
                double landing_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][15]);
                double chair_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][17]);                
                double nose_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][stairsCount][19]);
                List<double> results = new List<double>();                

                //For solutions on structural members                
                double[,] FwaistSlab1 = stairsRebarsElper(cEF, F_WaistSlab1, F_WS_BarNumber, main_bars_diam, quantity);               
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FwaistSlab1);                
                double[,] FwaistSlab2 = stairsRebarsElper(cEF, F_WaistSlab2, F_WS_BarNumber, main_bars_diam, quantity);                
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FwaistSlab2);
                double[,] FwaistSlab3 = stairsRebarsElper(cEF, F_WaistSlab3, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FwaistSlab3);
                double[,] SwaistSlab1 = stairsRebarsElper(cEF, S_WaistSlab1, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab1);
                double[,] SwaistSlab2 = stairsRebarsElper(cEF, S_WaistSlab2, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab2);
                double[,] SwaistSlab3 = stairsRebarsElper(cEF, S_WaistSlab3, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(SwaistSlab3);
                double[,] FdistBars = stairsRebarsElper(cEF, StairWidth, (F_TempBars + S_TempBars), temp_bars_diam, quantity);                
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(FdistBars);
                double[,] Flanding = stairsRebarsElper(cEF, F_Landing, F_L_BarNumber, landing_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(Flanding);
                //                
                double[,] chairBars = stairsRebarsElper(cEF, S_ChairBarsLength, S_ChairBarNumber, chair_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(chairBars);                
                double[,] noseBars = stairsRebarsElper(cEF, StairWidth, S_NoseBar, nose_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][stairsCount].Add(noseBars);

                //For parameter manipulation
                string[] stringArray = cEF.parameters.stair[floorCount][stairsCount].getValues();                                
                stringArray[5] = bestLMSgetter(FwaistSlab1);
                stringArray[6] = bestLMSgetter(FwaistSlab2);
                stringArray[7] = bestLMSgetter(FwaistSlab3);
                stringArray[8] = bestLMSgetter(SwaistSlab1);
                stringArray[9] = bestLMSgetter(SwaistSlab2);
                stringArray[10] = bestLMSgetter(SwaistSlab3);
                stringArray[11] = bestLMSgetter(FdistBars);
                stringArray[12] = bestLMSgetter(Flanding);
                stringArray[13] = bestLMSgetter(chairBars);
                stringArray[14] = bestLMSgetter(noseBars);
                cEF.parameters.stair[floorCount][stairsCount].setUStairsValues(stringArray[1], stringArray[2], stringArray[3],
                            stringArray[4], stringArray[5], stringArray[6], stringArray[7], stringArray[8], stringArray[9],
                            stringArray[10], stringArray[11], stringArray[12], stringArray[13], stringArray[14]);
                
                
            }

            //Computation -- add Formworks [STAIRS]
            recomputeFW_stairs(cEF);
            refreshSolutions(cEF);            
        }
        

        public void ModifyStairsWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            if (cEF.structuralMembers.stairs[floorCount][index][0].Equals("Straight Stairs"))
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_ST_CG;

                //Variables from StructMem
                double quantity, steps, SL, riser, tread, Thc;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.stairs[floorCount][index][1], System.Globalization.CultureInfo.InvariantCulture);
                steps = double.Parse(cEF.structuralMembers.stairs[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                SL = double.Parse(cEF.structuralMembers.stairs[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
                riser = double.Parse(cEF.structuralMembers.stairs[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);
                tread = double.Parse(cEF.structuralMembers.stairs[floorCount][index][5], System.Globalization.CultureInfo.InvariantCulture);
                Thc = double.Parse(cEF.structuralMembers.stairs[floorCount][index][6], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[5])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }

                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                }

                else
                {
                    double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                    while (WSL % 25 != 0)
                    {
                        WSL++;
                    }
                    double volumeWSF = SL * WSL * Thc;
                    double volumeSteps = (riser * tread) / 2 * SL * steps;
                    double volume = volumeWSF + volumeSteps;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] =
                        volume * quantity;
                }
                ///
                double WaistSlab1, WaistSlab2, WaistSlab3, WaistSlab4, TempBars, Landing,
                       WS_BarNumber, LShaped_num, LShaped_Length, NoseBar;

                double Lapping, Length;
                double LapLength, HookLength, WS_Diameter, Riser, Tread, Steps,
                       WidthLanding, WS_Spacing, StairWidth, BeamWidth
                       ;
                string[] stairParameterValues = cEF.parameters.stair[floorCount][index].getValues();
                LapLength = double.Parse(stairParameterValues[1]);
                HookLength = double.Parse(stairParameterValues[2]);
                WS_Diameter = double.Parse(cEF.structuralMembers.stairs[floorCount][index][7], System.Globalization.CultureInfo.InvariantCulture);
                Riser = double.Parse(cEF.structuralMembers.stairs[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);
                Tread = double.Parse(cEF.structuralMembers.stairs[floorCount][index][5], System.Globalization.CultureInfo.InvariantCulture);
                Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                WS_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][8], System.Globalization.CultureInfo.InvariantCulture);
                StairWidth = double.Parse(cEF.structuralMembers.stairs[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
                BeamWidth = double.Parse(stairParameterValues[3]);
                //==========Calculation===========
                Lapping = LapLength * WS_Diameter;
                Length = nearestValue(Math.Sqrt(Math.Pow(Riser * Steps, 2) + Math.Pow(Tread * Steps, 2)), 25);
                WaistSlab1 = nearestValue(Lapping + Length + (HookLength * WS_Diameter), 25);
                WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + StairWidth + Length, 25);
                WaistSlab3 = nearestValue((HookLength * WS_Diameter) + StairWidth + BeamWidth + (LapLength * 10), 25);
                WaistSlab4 = rounder(2 * ((Length / WS_Spacing) + 1));
                double bar_pcs = rounder((StairWidth / WS_Spacing) + 1);
                //Steps Rebar
                LShaped_num = rounder((StairWidth / WS_Spacing) + 1) * steps;
                LShaped_Length = Tread + Riser;
                NoseBar = StairWidth;
                
                double main_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][7]);
                double temp_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][9]);
                double chair_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][11]);
                double nose_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][13]);

                double[,] SwaistSlab1 = stairsRebarsElper(cEF, WaistSlab1, bar_pcs, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][0] = SwaistSlab1;
                double[,] SwaistSlab2 = stairsRebarsElper(cEF, WaistSlab2, bar_pcs, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][1] = SwaistSlab2 ;
                double[,] SwaistSlab3 = stairsRebarsElper(cEF, WaistSlab3, bar_pcs, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][2] = SwaistSlab3;
                double[,] SdistBars = stairsRebarsElper(cEF, StairWidth, WaistSlab4, temp_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][3] = SdistBars;
                double[,] chairBars = stairsRebarsElper(cEF, LShaped_Length, LShaped_num, chair_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][4] = chairBars;
                double[,] noseBars = stairsRebarsElper(cEF, StairWidth, Steps, nose_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][5] = noseBars;
                string[] stringArray = cEF.parameters.stair[floorCount][index].getValues();
                stringArray[5] = bestLMSgetter(SwaistSlab1);
                stringArray[6] = bestLMSgetter(SwaistSlab2);
                stringArray[7] = bestLMSgetter(SwaistSlab3);
                stringArray[8] = bestLMSgetter(SdistBars);
                stringArray[9] = bestLMSgetter(chairBars);
                stringArray[10] = bestLMSgetter(noseBars);
                cEF.parameters.stair[floorCount][index].setStraightStairsValues(stringArray[1], stringArray[2], stringArray[3],
                            stringArray[4], stringArray[5], stringArray[6], stringArray[7], stringArray[8], stringArray[9], stringArray[10]);
                //
            }
            else if (cEF.structuralMembers.stairs[floorCount][index][0].Equals("L-Stairs"))
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_ST_CG;

                //Variables from StructMem
                double quantity, stepsFF, steps2FF, SL, riser, tread, wsThc, landingThc;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.stairs[floorCount][index][1], System.Globalization.CultureInfo.InvariantCulture);
                stepsFF = double.Parse(cEF.structuralMembers.stairs[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                steps2FF = double.Parse(cEF.structuralMembers.stairs[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
                SL = double.Parse(cEF.structuralMembers.stairs[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);
                riser = double.Parse(cEF.structuralMembers.stairs[floorCount][index][5], System.Globalization.CultureInfo.InvariantCulture);
                tread = double.Parse(cEF.structuralMembers.stairs[floorCount][index][6], System.Globalization.CultureInfo.InvariantCulture);
                wsThc = double.Parse(cEF.structuralMembers.stairs[floorCount][index][7], System.Globalization.CultureInfo.InvariantCulture);
                landingThc = double.Parse(cEF.structuralMembers.stairs[floorCount][index][8], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[5])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = (SL * SL) * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                }
                else
                {
                    double steps = stepsFF + steps2FF;
                    double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                    while (WSL1 % 25 != 0)
                    {
                        WSL1++;
                    }
                    double volumeWSF1 = SL * WSL1 * wsThc;
                    double volumeSteps = (riser * tread) / 2 * SL * steps;
                    double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                    while (WSL2 % 25 != 0)
                    {
                        WSL2++;
                    }
                    double volumeWSF2 = SL * WSL2 * wsThc;
                    double landing = (SL * SL) * landingThc;
                    double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] =
                        volume * quantity;
                    
                    print(volumeSteps + " * " + volumeWSF1 + " * " + volumeSteps + " * " + volumeWSF2 + " = " +
                        ((volumeWSF1 + volumeSteps + volumeWSF2 + landing) / 1000000000) * quantity);
                }

                //
                double F_WaistSlab1, F_WaistSlab2, F_WaistSlab3, F_WaistSlab4, TempBars,
                       Landing_Length, Landing_Number, Landing_Spacing, TB_Spacing,
                       F_WS_BarNumber,
                       S_WaistSlab1, S_WaistSlab2, S_WaistSlab3, S_WaistSlab4,
                       S_WS_BarNumber, CB_Length, CB_Number;
                double Lapping, F_Length, S_Length;
                double LapLength, HookLength, WS_Diameter,
                       Tread, Riser, F_Steps, S_Steps,
                       WidthLanding, StairWidth, BeamWidth,
                       WS_Spacing, CB_Spacing;

                string[] stairParameterValues = cEF.parameters.stair[floorCount][index].getValues();
                LapLength = double.Parse(stairParameterValues[1]);
                WS_Diameter = double.Parse(cEF.structuralMembers.stairs[floorCount][index][9], System.Globalization.CultureInfo.InvariantCulture);
                Riser = double.Parse(cEF.structuralMembers.stairs[floorCount][index][5], System.Globalization.CultureInfo.InvariantCulture);
                Tread = double.Parse(cEF.structuralMembers.stairs[floorCount][index][6], System.Globalization.CultureInfo.InvariantCulture);
                F_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                S_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
                HookLength = double.Parse(stairParameterValues[2]);
                StairWidth = double.Parse(cEF.structuralMembers.stairs[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);
                BeamWidth = double.Parse(stairParameterValues[3]);
                WS_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][10], System.Globalization.CultureInfo.InvariantCulture);
                Landing_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][14], System.Globalization.CultureInfo.InvariantCulture);
                CB_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][16], System.Globalization.CultureInfo.InvariantCulture);
                TB_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][12], System.Globalization.CultureInfo.InvariantCulture);

                //==========FIRST FLIGHT===========
                Lapping = LapLength * WS_Diameter;
                F_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * F_Steps, 2) + Math.Pow(Tread * F_Steps, 2)), 25);
                F_WaistSlab1 = nearestValue(Lapping + F_Length + (HookLength * WS_Diameter), 25);
                F_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + StairWidth + F_Length, 25);
                F_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + StairWidth + BeamWidth + (LapLength * WS_Diameter), 25);
                F_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);
                F_WaistSlab4 = rounder(2 * ((F_Length / TB_Spacing) + 1));
                Landing_Length = StairWidth;
                Landing_Number = (StairWidth / Landing_Spacing) + 1;

                //=========SECOND FLIGHT===========
                S_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * S_Steps, 2) + Math.Pow(Tread * S_Steps, 2)), 25);
                S_WaistSlab1 = nearestValue(Lapping + S_Length + (HookLength * WS_Diameter), 25);
                S_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + StairWidth + S_Length, 25);
                S_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + StairWidth + BeamWidth + (LapLength * WS_Diameter), 25);
                S_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);
                S_WaistSlab4 = rounder(2 * ((S_Length / TB_Spacing) + 1));
                CB_Length = Tread + Riser;
                CB_Number = rounder((StairWidth / CB_Spacing) + 1) * (F_Steps + S_Steps);
                double S_NoseBar = F_Steps + S_Steps;
                //

                double main_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][9]);
                double temp_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][11]);
                double landing_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][13]);
                double chair_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][15]);
                double nose_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][17]);

                //For solutions on structural members                
                double[,] FwaistSlab1 = stairsRebarsElper(cEF, F_WaistSlab1, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][0] = FwaistSlab1;
                double[,] FwaistSlab2 = stairsRebarsElper(cEF, F_WaistSlab2, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][1] = FwaistSlab2;
                double[,] FwaistSlab3 = stairsRebarsElper(cEF, F_WaistSlab3, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][2] = FwaistSlab3;
                double[,] SwaistSlab1 = stairsRebarsElper(cEF, S_WaistSlab1, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][3] = SwaistSlab1;
                double[,] SwaistSlab2 = stairsRebarsElper(cEF, S_WaistSlab2, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][4] = SwaistSlab2;
                double[,] SwaistSlab3 = stairsRebarsElper(cEF, S_WaistSlab3, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][5] = SwaistSlab3;
                double[,] FdistBars = stairsRebarsElper(cEF, StairWidth, (F_WaistSlab4 + S_WaistSlab4), temp_bars_diam, quantity);                
                cEF.structuralMembers.stairs_Rebar[floorCount][index][6] = FdistBars;
                double[,] Flanding = stairsRebarsElper(cEF, Landing_Length, Landing_Number, landing_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][7] = Flanding;
                //                               
                double[,] chairBars = stairsRebarsElper(cEF, CB_Length, CB_Number, chair_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][8] = chairBars;
                double[,] noseBars = stairsRebarsElper(cEF, StairWidth, S_NoseBar, nose_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][9] = noseBars;

                string[] stringArray = cEF.parameters.stair[floorCount][index].getValues();
                stringArray[5] = bestLMSgetter(FwaistSlab1);
                stringArray[6] = bestLMSgetter(FwaistSlab2);
                stringArray[7] = bestLMSgetter(FwaistSlab3);
                stringArray[8] = bestLMSgetter(SwaistSlab1);
                stringArray[9] = bestLMSgetter(SwaistSlab2);
                stringArray[10] = bestLMSgetter(SwaistSlab3);
                stringArray[11] = bestLMSgetter(FdistBars);
                stringArray[12] = bestLMSgetter(Flanding);
                stringArray[13] = bestLMSgetter(chairBars);
                stringArray[14] = bestLMSgetter(noseBars);
                cEF.parameters.stair[floorCount][index].setLStairsValues(stringArray[1], stringArray[2], stringArray[3],
                            stringArray[4], stringArray[5], stringArray[6], stringArray[7], stringArray[8], stringArray[9],
                            stringArray[10], stringArray[11], stringArray[12], stringArray[13], stringArray[14]);

            }
            else
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_ST_CG;

                //Variables from StructMem
                double quantity, stepsFF, steps2FF, SL, riser, tread, wsThc, landingW, gap, landingThc;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.stairs[floorCount][index][1], System.Globalization.CultureInfo.InvariantCulture);
                stepsFF = double.Parse(cEF.structuralMembers.stairs[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                steps2FF = double.Parse(cEF.structuralMembers.stairs[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
                SL = double.Parse(cEF.structuralMembers.stairs[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);
                riser = double.Parse(cEF.structuralMembers.stairs[floorCount][index][5], System.Globalization.CultureInfo.InvariantCulture);
                tread = double.Parse(cEF.structuralMembers.stairs[floorCount][index][6], System.Globalization.CultureInfo.InvariantCulture);
                wsThc = double.Parse(cEF.structuralMembers.stairs[floorCount][index][7], System.Globalization.CultureInfo.InvariantCulture);
                landingW = double.Parse(cEF.structuralMembers.stairs[floorCount][index][8], System.Globalization.CultureInfo.InvariantCulture);
                gap = double.Parse(cEF.structuralMembers.stairs[floorCount][index][9], System.Globalization.CultureInfo.InvariantCulture);
                landingThc = double.Parse(cEF.structuralMembers.stairs[floorCount][index][10], System.Globalization.CultureInfo.InvariantCulture);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[5])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                    else
                    {
                        double steps = stepsFF + steps2FF;
                        double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                        while (WSL1 % 25 != 0)
                        {
                            WSL1++;
                        }
                        double volumeWSF1 = SL * WSL1 * wsThc;
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                        while (WSL2 % 25 != 0)
                        {
                            WSL2++;
                        }
                        double volumeWSF2 = SL * WSL2 * wsThc;
                        double landing = ((SL * 2) + gap) * landingW * landingThc;
                        double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][3] = volume * quantity;
                    }
                }
                else
                {
                    double steps = stepsFF + steps2FF;
                    double WSL1 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * stepsFF), 2) + Math.Pow((riser * stepsFF), 2)));
                    while (WSL1 % 25 != 0)
                    {
                        WSL1++;
                    }
                    double volumeWSF1 = SL * WSL1 * wsThc;
                    double volumeSteps = (riser * tread) / 2 * SL * steps;
                    double WSL2 = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps2FF), 2) + Math.Pow((riser * steps2FF), 2)));
                    while (WSL2 % 25 != 0)
                    {
                        WSL2++;
                    }
                    double volumeWSF2 = SL * WSL2 * wsThc;
                    double landing = ((SL * 2) + gap) * landingW * landingThc;
                    double volume = volumeWSF1 + volumeSteps + volumeWSF2 + landing;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] =
                        volume * quantity;
                }
                //

                double F_WaistSlab1, F_WaistSlab2, F_WaistSlab3, F_TempBars, F_Landing,
                       F_WS_BarNumber, F_L_BarNumber,
                       S_WaistSlab1, S_WaistSlab2, S_WaistSlab3, S_TempBars, S_Landing,
                       S_WS_BarNumber, S_ChairBarsLength, S_ChairBarNumber,
                       S_NoseBar;
                double Lapping, F_Length, S_Length;

                string[] stairParameterValues = cEF.parameters.stair[floorCount][index].getValues();


                double Riser, F_Steps, S_Steps, Tread,
                       LapLength,
                       HookLength, WS_Diameter, WS_Spacing,
                       WidthLanding,
                       BeamWidth,
                       StairWidth, TB_Spacing,
                       Gap,
                       L_MainBars, L_Spacing,
                       C_Spacing
                       ;

                //ParameterValues
                HookLength = double.Parse(stairParameterValues[2]);
                LapLength = double.Parse(stairParameterValues[1]);
                BeamWidth = double.Parse(stairParameterValues[3]);

                //AddStructValues
                WS_Diameter = double.Parse(cEF.structuralMembers.stairs[floorCount][index][11], System.Globalization.CultureInfo.InvariantCulture);
                WS_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][12], System.Globalization.CultureInfo.InvariantCulture);
                WidthLanding = double.Parse(cEF.structuralMembers.stairs[floorCount][index][8], System.Globalization.CultureInfo.InvariantCulture);
                Riser = double.Parse(cEF.structuralMembers.stairs[floorCount][index][5], System.Globalization.CultureInfo.InvariantCulture);
                Tread = double.Parse(cEF.structuralMembers.stairs[floorCount][index][6], System.Globalization.CultureInfo.InvariantCulture);
                F_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
                S_Steps = double.Parse(cEF.structuralMembers.stairs[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
                StairWidth = double.Parse(cEF.structuralMembers.stairs[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);
                TB_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][14], System.Globalization.CultureInfo.InvariantCulture);
                Gap = double.Parse(cEF.structuralMembers.stairs[floorCount][index][9], System.Globalization.CultureInfo.InvariantCulture);
                L_MainBars = double.Parse(cEF.structuralMembers.stairs[floorCount][index][15], System.Globalization.CultureInfo.InvariantCulture);
                L_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][16], System.Globalization.CultureInfo.InvariantCulture);
                C_Spacing = double.Parse(cEF.structuralMembers.stairs[floorCount][index][18], System.Globalization.CultureInfo.InvariantCulture);
                //===============FIRST FLIGHT =============================//
                //CalculatedValues 1 -> 5
                Lapping = LapLength * WS_Diameter;
                F_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * F_Steps, 2) + Math.Pow(Tread * F_Steps, 2)), 25);

                F_WaistSlab1 = nearestValue(Lapping + F_Length + (HookLength * WS_Diameter), 25);
                F_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + WidthLanding + F_Length, 25);
                F_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + WidthLanding + BeamWidth + (LapLength * WS_Diameter), 25);
                F_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);
                F_TempBars = rounder(2 * ((F_Length / TB_Spacing) + 1));
                F_Landing = (StairWidth * 2) + Gap;
                F_L_BarNumber = rounder((WidthLanding / L_Spacing) + 1);

                //===============SECOND FLIGHT =============================//
                //CalculatedValues 6 -> n
                S_Length = nearestValue(Math.Sqrt(Math.Pow(Riser * S_Steps, 2) + Math.Pow(Tread * S_Steps, 2)), 25);

                S_WaistSlab1 = nearestValue(Lapping + S_Length + (HookLength * WS_Diameter), 25);
                S_WaistSlab2 = nearestValue((2 * HookLength * WS_Diameter) + WidthLanding + S_Length, 25);
                S_WaistSlab3 = nearestValue((HookLength * WS_Diameter) + WidthLanding + BeamWidth + (LapLength * WS_Diameter), 25);
                S_WS_BarNumber = rounder((StairWidth / WS_Spacing) + 1);
                S_TempBars = rounder(2 * ((S_Length / TB_Spacing) + 1));
                //===========Steps========================
                S_ChairBarsLength = Tread + Riser;
                S_ChairBarNumber = rounder((StairWidth / C_Spacing) + 1) * (F_Steps + S_Steps);
                S_NoseBar = (F_Steps + S_Steps);                               

                double main_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][11]);
                double temp_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][13]);
                double landing_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][15]);
                double chair_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][17]);
                double nose_bars_diam = double.Parse(cEF.structuralMembers.stairs[floorCount][index][19]);
                List<double> results = new List<double>();                

                //For solutions on structural members                
                double[,] FwaistSlab1 = stairsRebarsElper(cEF, F_WaistSlab1, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][0] = FwaistSlab1;
                double[,] FwaistSlab2 = stairsRebarsElper(cEF, F_WaistSlab2, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][1] = FwaistSlab2;
                double[,] FwaistSlab3 = stairsRebarsElper(cEF, F_WaistSlab3, F_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][2] = FwaistSlab3;
                double[,] SwaistSlab1 = stairsRebarsElper(cEF, S_WaistSlab1, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][3] = SwaistSlab1;
                double[,] SwaistSlab2 = stairsRebarsElper(cEF, S_WaistSlab2, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][4] = SwaistSlab2;
                double[,] SwaistSlab3 = stairsRebarsElper(cEF, S_WaistSlab3, S_WS_BarNumber, main_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][5] = SwaistSlab3;
                double[,] FdistBars = stairsRebarsElper(cEF, StairWidth, (F_TempBars + S_TempBars), temp_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][6] = FdistBars;
                double[,] Flanding = stairsRebarsElper(cEF, F_Landing, F_L_BarNumber, landing_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][7] = Flanding;
                //
                double[,] chairBars = stairsRebarsElper(cEF, S_ChairBarsLength, S_ChairBarNumber, chair_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][8] = chairBars;
                double[,] noseBars = stairsRebarsElper(cEF, StairWidth, S_NoseBar, nose_bars_diam, quantity);
                cEF.structuralMembers.stairs_Rebar[floorCount][index][9] = noseBars;

                string[] stringArray = cEF.parameters.stair[floorCount][index].getValues();
                stringArray[5] = bestLMSgetter(FwaistSlab1);
                stringArray[6] = bestLMSgetter(FwaistSlab2);
                stringArray[7] = bestLMSgetter(FwaistSlab3);
                stringArray[8] = bestLMSgetter(SwaistSlab1);
                stringArray[9] = bestLMSgetter(SwaistSlab2);
                stringArray[10] = bestLMSgetter(SwaistSlab3);
                stringArray[11] = bestLMSgetter(FdistBars);
                stringArray[12] = bestLMSgetter(Flanding);
                stringArray[13] = bestLMSgetter(chairBars);
                stringArray[14] = bestLMSgetter(noseBars);
                cEF.parameters.stair[floorCount][index].setUStairsValues(stringArray[1], stringArray[2], stringArray[3],
                            stringArray[4], stringArray[5], stringArray[6], stringArray[7], stringArray[8], stringArray[9],
                            stringArray[10], stringArray[11], stringArray[12], stringArray[13], stringArray[14]);
                
            }
            //Computation -- modify Formworks [STAIRS]
            recomputeFW_stairs(cEF);
            refreshSolutions(cEF);
        }

        public void recomputeFW_stairs(CostEstimationForm cEF)
        {
            try
            {
                List<string> stairsLumber = dimensionFilterer(cEF.parameters.form_SM_ST_FL);
                List<string> stairsScaf = dimensionFilterer(cEF.parameters.form_SM_ST_VS);
                List<double> Uform_h = new List<double>();
                List<double> Uframe_h = new List<double>();
                List<double> Uscaf_h = new List<double>();
                List<double> Lform_h = new List<double>();
                List<double> Lframe_h = new List<double>();
                List<double> Lscaf_h = new List<double>();
                List<double> Sform_h = new List<double>();
                List<double> Sframe_h = new List<double>();
                List<double> Sscaf_h = new List<double>();
                List<double> UAREA_h = new List<double>();
                List<double> LAREA_h = new List<double>();
                List<double> SAREA_h = new List<double>();
                for (int i = 0; i < cEF.structuralMembers.stairs.Count; i++)
                {
                    //USTAIRS                
                    double UfirstFlightL = 0;
                    double UfirstFlightarea = 0;
                    double UfirstRB = 0;
                    double UsecondFlightL = 0;
                    double UsecondFlightarea = 0;
                    double UsecondRB = 0;
                    double UlandingFW = 0;
                    double UstepsFW = 0;
                    double Utotalsteps = 0;
                    double UFRAME = 0;
                    double USCAF = 0;
                    double Utotalarea = 0;
                    //USTAIRS

                    //LSTAIRS
                    double LfirstFlightL = 0;
                    double LfirstFlightarea = 0;
                    double LfirstRB = 0;
                    double LsecondFlightL = 0;
                    double LsecondFlightarea = 0;
                    double LsecondRB = 0;
                    double LlandingFW = 0;
                    double LstepsFW = 0;
                    double Ltotalsteps = 0;
                    double LFRAME = 0;
                    double LSCAF = 0;
                    double Ltotalarea = 0;
                    //LSTAIRS

                    //SSTAIRS
                    double SfirstFlightL = 0;
                    double SfirstFlightarea = 0;
                    double SfirstRB = 0;
                    double SstepsFW = 0;
                    double SFRAME = 0;
                    double SSCAF = 0;
                    double Stotalarea = 0;
                    //SSTAIRS
                    Uform_h.Add(0);
                    Uframe_h.Add(0);
                    Uscaf_h.Add(0);
                    Lform_h.Add(0);
                    Lframe_h.Add(0);
                    Lscaf_h.Add(0);
                    Sform_h.Add(0);
                    Sframe_h.Add(0);
                    Sscaf_h.Add(0);
                    UAREA_h.Add(0);
                    LAREA_h.Add(0);
                    SAREA_h.Add(0);
                    for (int j = 0; j < cEF.structuralMembers.stairs[i].Count; j++)
                    {
                        if (cEF.structuralMembers.stairs[i][j][0] == "U-Stairs")
                        {
                            double quantity = double.Parse(cEF.structuralMembers.stairs[i][j][1]);
                            double stepsFF = double.Parse(cEF.structuralMembers.stairs[i][j][2]);
                            double steps2FF = double.Parse(cEF.structuralMembers.stairs[i][j][3]);
                            double SL = double.Parse(cEF.structuralMembers.stairs[i][j][4]) / 1000;
                            double riser = double.Parse(cEF.structuralMembers.stairs[i][j][5]) / 1000;
                            double tread = double.Parse(cEF.structuralMembers.stairs[i][j][6]) / 1000;
                            double wsThc = double.Parse(cEF.structuralMembers.stairs[i][j][7]) / 1000;
                            double landingW = double.Parse(cEF.structuralMembers.stairs[i][j][8]) / 1000;
                            double gap = double.Parse(cEF.structuralMembers.stairs[i][j][9]) / 1000;
                            double landingThc = double.Parse(cEF.structuralMembers.stairs[i][j][10]) / 1000;
                            Utotalsteps = stepsFF + steps2FF;

                            UfirstFlightL = stairsRounder(Math.Round((Math.Sqrt(Math.Pow(((riser) * stepsFF), 2) + (Math.Pow(((tread) * stepsFF), 2)))), 3));
                            UfirstFlightarea = UfirstFlightL * SL;
                            UfirstRB = (UfirstFlightL * (riser + 0.1)) * 2;
                            UsecondFlightL = stairsRounder(Math.Round((Math.Sqrt(Math.Pow(((riser) * steps2FF), 2) + (Math.Pow(((tread) * steps2FF), 2)))), 3));
                            UsecondFlightarea = UsecondFlightL * SL;
                            UsecondRB = (UsecondFlightL * (riser + 0.1)) * 2;
                            UlandingFW =
                                (((SL * 2) + gap) * landingW) +
                                (2 * (landingW * landingThc)) +
                                (((SL * 2) + gap) * landingThc);
                            UstepsFW = (SL * riser) * (stepsFF + steps2FF);
                            double Uframe_first = (rounder(SL / 0.6) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (UfirstFlightL * 3.28))) / 12;
                            double Uframe_second = (rounder(SL / 0.6) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (UsecondFlightL * 3.28))) / 12;
                            double Uframe_brace = ((Utotalsteps * ((double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (3.28)))) / 12) * rounder(SL / 0.6);
                            double Uback_first = (rounder(SL / 0.3) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (UfirstFlightL * 3.28))) / 12;
                            double Uback_second = (rounder(SL / 0.3) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (UsecondFlightL * 3.28))) / 12;
                            double Uback_brace = ((2 * ((double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (SL * 3.28)))) / 12) * Utotalsteps;
                            UFRAME += (Uframe_first + Uframe_second + Uframe_brace + Uback_first + Uback_second + Uback_brace) * quantity;
                            USCAF += ((UfirstFlightL + SL * 2 + gap + UsecondFlightL) * 12) * quantity;
                            Utotalarea += (UfirstFlightarea + UfirstRB + UsecondFlightarea + UsecondRB + UlandingFW + UstepsFW) * quantity;
                        }
                        else if (cEF.structuralMembers.stairs[i][j][0] == "L-Stairs")
                        {
                            double quantity = double.Parse(cEF.structuralMembers.stairs[i][j][1]);
                            double stepsFF = double.Parse(cEF.structuralMembers.stairs[i][j][2]);
                            double steps2FF = double.Parse(cEF.structuralMembers.stairs[i][j][3]);
                            double SL = double.Parse(cEF.structuralMembers.stairs[i][j][4]) / 1000;
                            double riser = double.Parse(cEF.structuralMembers.stairs[i][j][5]) / 1000;
                            double tread = double.Parse(cEF.structuralMembers.stairs[i][j][6]) / 1000;
                            double wsThc = double.Parse(cEF.structuralMembers.stairs[i][j][7]) / 1000;
                            double landingThc = double.Parse(cEF.structuralMembers.stairs[i][j][8]) / 1000;
                            Ltotalsteps = stepsFF + steps2FF;
                            LfirstFlightL = stairsRounder(Math.Round((Math.Sqrt(Math.Pow(((riser) * stepsFF), 2) + (Math.Pow(((tread) * stepsFF), 2)))), 3));
                            LfirstFlightarea = LfirstFlightL * SL;
                            LfirstRB = (LfirstFlightL * (riser + 0.1)) * 2;
                            LsecondFlightL = stairsRounder(Math.Round((Math.Sqrt(Math.Pow(((riser) * steps2FF), 2) + (Math.Pow(((tread) * steps2FF), 2)))), 3));
                            LsecondFlightarea = LsecondFlightL * SL;
                            LsecondRB = (LsecondFlightL * (riser + 0.1)) * 2;
                            LlandingFW = Math.Pow(SL, 2) + (3 * (landingThc * SL));
                            LstepsFW = (SL * riser) * (stepsFF + steps2FF);
                            double Lframe_first = (rounder(SL / 0.6) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (LfirstFlightL * 3.28))) / 12;
                            double Lframe_second = (rounder(SL / 0.6) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (LsecondFlightL * 3.28))) / 12;
                            double Lframe_brace = ((Ltotalsteps * ((double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (3.28)))) / 12) * rounder(SL / 0.6);
                            double Lback_first = (rounder(SL / 0.3) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (LfirstFlightL * 3.28))) / 12;
                            double Lback_second = (rounder(SL / 0.3) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (LsecondFlightL * 3.28))) / 12;
                            double Lback_brace = ((2 * ((double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (SL * 3.28)))) / 12) * Ltotalsteps;
                            LFRAME += (Lframe_first + Lframe_second + Lframe_brace + Lback_first + Lback_second + Lback_brace) * quantity;
                            LSCAF += ((LfirstFlightL + SL + LsecondFlightL) * 12) * quantity;
                            Ltotalarea += (LfirstFlightarea + LfirstRB + LsecondFlightarea + LsecondRB + LlandingFW + LstepsFW) * quantity;
                        }
                        else
                        {
                            double quantity = double.Parse(cEF.structuralMembers.stairs[i][j][1]);
                            double steps = double.Parse(cEF.structuralMembers.stairs[i][j][2]);
                            double SL = double.Parse(cEF.structuralMembers.stairs[i][j][3]) / 1000;
                            double riser = double.Parse(cEF.structuralMembers.stairs[i][j][4]) / 1000;
                            double tread = double.Parse(cEF.structuralMembers.stairs[i][j][5]) / 1000;
                            double Thc = double.Parse(cEF.structuralMembers.stairs[i][j][6]) / 1000;
                            SfirstFlightL = stairsRounder(Math.Round((Math.Sqrt(Math.Pow(((riser) * steps), 2) + (Math.Pow(((tread) * steps), 2)))), 3));
                            SfirstFlightarea = SfirstFlightL * SL;
                            SfirstRB = (SfirstFlightL * (riser + 0.1)) * 2;
                            SstepsFW = (SL * riser) * (steps);
                            Stotalarea += (SfirstFlightarea + SfirstRB + SstepsFW) * quantity;
                            double Sframe_first = (rounder(SL / 0.6) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (SfirstFlightL * 3.28))) / 12;
                            double Sframe_brace = ((steps * ((double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (3.28)))) / 12) * rounder(SL / 0.6);
                            double Sback_first = (rounder(SL / 0.3) * (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (SfirstFlightL * 3.28))) / 12;
                            double Sback_brace = ((2 * ((double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * (SL * 3.28)))) / 12) * steps;
                            SFRAME += (Sframe_first + Sframe_brace + Sback_first + Sback_brace) * quantity;
                            SSCAF += ((SfirstFlightL) * 12) * quantity;
                            Stotalarea += (SfirstFlightarea + SfirstRB + SstepsFW) * quantity;
                        }

                    }
                    Uform_h[i] = rounder((Utotalarea / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    Uframe_h[i] = rounder(((UFRAME * 12) / (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * double.Parse(stairsLumber[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    Uscaf_h[i] = rounder((((USCAF * 12) / (double.Parse(stairsScaf[0]) * double.Parse(stairsScaf[2]) * double.Parse(stairsScaf[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU))));
                    Lform_h[i] = rounder((Ltotalarea / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    Lframe_h[i] = rounder(((LFRAME * 12) / (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * double.Parse(stairsLumber[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    Lscaf_h[i] = rounder((((LSCAF * 12) / (double.Parse(stairsScaf[0]) * double.Parse(stairsScaf[2]) * double.Parse(stairsScaf[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU))));
                    Sform_h[i] = rounder((Stotalarea / (1.2 * 2.4)) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    Sframe_h[i] = rounder(((SFRAME * 12) / (double.Parse(stairsLumber[0]) * double.Parse(stairsLumber[2]) * double.Parse(stairsLumber[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU)));
                    Sscaf_h[i] = rounder((((SSCAF * 12) / (double.Parse(stairsScaf[0]) * double.Parse(stairsScaf[2]) * double.Parse(stairsScaf[4]))) * (1 / double.Parse(cEF.parameters.form_F_NU))));
                    UAREA_h[i] = Utotalarea;
                    LAREA_h[i] = Ltotalarea;
                    SAREA_h[i] = Stotalarea;
                }
                cEF.structuralMembers.UstairsFORM = Uform_h;
                cEF.structuralMembers.UstairsFRAME = Uframe_h;
                cEF.structuralMembers.UstairsSCAF = Uscaf_h;
                cEF.structuralMembers.LstairsFORM = Lform_h;
                cEF.structuralMembers.LstairsFRAME = Lframe_h;
                cEF.structuralMembers.LstairsSCAF = Lscaf_h;
                cEF.structuralMembers.SstairsFORM = Sform_h;
                cEF.structuralMembers.SstairsFRAME = Sframe_h;
                cEF.structuralMembers.SstairsSCAF = Sscaf_h;
                cEF.structuralMembers.UAREA = UAREA_h;
                cEF.structuralMembers.LAREA = LAREA_h;
                cEF.structuralMembers.SAREA = SAREA_h;
                refreshSolutions(cEF);
            }
            catch
            {
                print("Null catcher Stairs");
            }
        }
        //Stairs Computation Functions -- END

        //Roof Computation Functions -- START
        public void AddRoofWorks(CostEstimationForm cEF, int floorCount, int roofCount)
        {
            List<List<double>> newList = new List<List<double>>();
            int curr_FC = cEF.structuralMembers.roofSolutions.Count;
            if(curr_FC != floorCount+1)
            {
                for(int i = curr_FC; i < floorCount+1; i++)
                {                           
                    cEF.structuralMembers.roofSolutions.Add(newList);
                }                
            }
            else
            {
                print("same floor");
            }            
            roofWorks(cEF, floorCount, roofCount);
        }

        public void roofWorks(CostEstimationForm cEF, int floorCount, int roofCount)
        {
            List<double> outputs = new List<double>();
            if (cEF.structuralMembers.roof[floorCount][roofCount][0] == "Rafter and Purlins")
            {
                if (cEF.structuralMembers.roof[floorCount][roofCount][1] == "Wood")
                {
                    //kind - wodsandshts -// 2 Lraft - Lpurl - spaceR - spaceP
                    double raft = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][3]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][4])) + 1)) * 2;
                    double lenRaft = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][2]) * 3.28);
                    double purl = (rounder((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][2]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][5])) + 1)) * 2;
                    double lenPurl = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][3]) * 3.28);
                    double rafB = rounder(raft * ((2 * 6 * (lenRaft)) / 12));
                    double purB = rounder(purl * ((2 * 2 * (lenPurl)) / 12));
                    double totB = rafB + purB;
                    outputs.Add(1);//rafter and purlians
                    outputs.Add(1);//wood
                    outputs.Add(totB);//total                    
                }
                else if (cEF.structuralMembers.roof[floorCount][roofCount][1] == "Steel - Tubular")
                {
                    //2 Lraft(SW) - Lraft - Lpurl - spaceR - spaceP - CommR - CommP
                    double raft = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][2]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][5])) + 1) * 2);
                    double lenRaft = rounder((raft) * (double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][3])));
                    double sixLenR = rounder(lenRaft / 6);
                    double purl = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][3]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][6])) + 1) * 2);
                    double lenPurl = rounder(purl * double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][4]));
                    double sixLenP = rounder(lenPurl / 6);
                    outputs.Add(1);
                    outputs.Add(2);                    
                    outputs.Add(sixLenR);
                    outputs.Add(sixLenP);                    
                }
                else
                {
                    double raft = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][2]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][5])) + 1) * 2);
                    double lenRaft = rounder((raft) * (double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][3])));
                    double sixLenR = rounder(lenRaft / 6);
                    double purl = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][3]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][6])) + 1) * 2);
                    double lenPurl = rounder(purl * double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][4]));
                    double sixLenP = rounder(lenPurl / 6);
                    outputs.Add(1);
                    outputs.Add(3);
                    outputs.Add(sixLenR);
                    outputs.Add(sixLenP);
                }
            }
            else if (cEF.structuralMembers.roof[floorCount][roofCount][0] == "G.I Roof and Its Accessories")
            {
                ListDictionary table1 = new ListDictionary();
                table1.Add("1.50", 14);
                table1.Add("1.80", 14);
                table1.Add("2.10", 18);
                table1.Add("2.40", 18);
                table1.Add("2.70", 22);
                table1.Add("3.00", 22);
                table1.Add("3.60", 26);
                ListDictionary table2 = new ListDictionary();
                table2.Add("1.50", 28);
                table2.Add("1.80", 28);
                table2.Add("2.10", 36);
                table2.Add("2.40", 36);
                table2.Add("2.70", 44);
                table2.Add("3.00", 44);
                table2.Add("3.60", 52);
                ListDictionary table3 = new ListDictionary();
                table3.Add("2\" x 3\"", 384);
                table3.Add("2\" x 4\"", 342);
                table3.Add("2\" x 5\"", 312);
                table3.Add("2\" x 6\"", 288);
                double corrSheets = 0;
                double giNails = 0;
                double rivets = 0;
                double giWashers = 0;
                double leadWashers = 0;
                double umbNails = 0;
                double plain_sheets = 0;                
                List<double> materials = new List<double>();                
                // corr - ginail - girivet - giwash - leadwash - umbnails - strap               
                corrSheets = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][1]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][2])) * 2;
                if (cEF.structuralMembers.roof[floorCount][roofCount].Contains("G.I Roof Nails"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][roofCount])
                    {
                        giNails += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 120;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][roofCount].Contains("G.I Rivets"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][roofCount])
                    {
                        rivets += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 180;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][roofCount].Contains("G.I Washers"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][roofCount])
                    {
                        giWashers += (corrSheets * double.Parse(table2[double.Parse(str).ToString("F")].ToString())) / 126;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][roofCount].Contains("Lead Washers"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][roofCount])
                    {
                        leadWashers += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 75;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][roofCount].Contains("Umbrella Nails"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][roofCount])
                    {
                        umbNails += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 120;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][roofCount].Contains("Plain G.I Strap"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][roofCount])
                    {
                        plain_sheets += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString()));
                    }
                    plain_sheets = plain_sheets / double.Parse(table3[cEF.structuralMembers.roof[floorCount][roofCount][cEF.structuralMembers.roof[floorCount][roofCount].Count - 1]].ToString());
                }
                outputs.Add(2);
                if (cEF.structuralMembers.roof[floorCount][roofCount].Contains("Corrugated G.I Sheet"))
                {
                    outputs.Add(rounder(corrSheets));
                }
                else
                {
                    outputs.Add(0);
                }                    
                outputs.Add(rounder(giNails));
                outputs.Add(rounder(rivets));
                outputs.Add(rounder(giWashers));
                outputs.Add(rounder(leadWashers));
                outputs.Add(rounder(umbNails));
                outputs.Add(rounder(plain_sheets));
            }
            else
            {
                double gutter = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][1]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][2])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][3])));
                double flashing = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][4]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][5])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][6])));
                double ridge = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][7]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][8])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][9])));
                double valley = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][10]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][11])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][12])));
                double hipped = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][13]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][14])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][15])));
                outputs.Add(3);
                outputs.Add(gutter + flashing + ridge + valley + hipped);
            }
            cEF.structuralMembers.roofSolutions[floorCount].Add(outputs);
        }

        public void ModifyRoofWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            List<double> outputs = new List<double>();
            /*foreach (var c in cEF.structuralMembers.roof[floorCount][index])
            {
                print(c + " ---");
            }*/
            if (cEF.structuralMembers.roof[floorCount][index][0] == "Rafter and Purlins")
            {
                if (cEF.structuralMembers.roof[floorCount][index][1] == "Wood")
                {
                    //kind - wodsandshts -// 2 Lraft - Lpurl - spaceR - spaceP
                    double raft = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][index][3]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][4])) + 1)) * 2;
                    double lenRaft = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][index][2]) * 3.28);
                    double purl = (rounder((double.Parse(cEF.structuralMembers.roof[floorCount][index][2]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][5])) + 1)) * 2;
                    double lenPurl = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][index][3]) * 3.28);
                    double rafB = rounder(raft * ((2 * 6 * (lenRaft)) / 12));
                    double purB = rounder(purl * ((2 * 2 * (lenPurl)) / 12));
                    double totB = rafB + purB;
                    outputs.Add(1);//rafter and purlians
                    outputs.Add(1);//wood
                    outputs.Add(totB);//total                   
                }
                else if (cEF.structuralMembers.roof[floorCount][index][1] == "Steel - Tubular")
                {
                    //2 Lraft(SW) - Lraft - Lpurl - spaceR - spaceP - CommR - CommP
                    double raft = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][index][2]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][5])) + 1) * 2);
                    double lenRaft = rounder((raft) * (double.Parse(cEF.structuralMembers.roof[floorCount][index][3])));
                    double sixLenR = rounder(lenRaft / 6);
                    double purl = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][index][3]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][6])) + 1) * 2);
                    double lenPurl = rounder(purl * double.Parse(cEF.structuralMembers.roof[floorCount][index][4]));
                    double sixLenP = rounder(lenPurl / 6);
                    outputs.Add(1);
                    outputs.Add(2);
                    outputs.Add(sixLenR);
                    outputs.Add(sixLenP);
                }
                else
                {
                    double raft = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][index][2]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][5])) + 1) * 2);
                    double lenRaft = rounder((raft) * (double.Parse(cEF.structuralMembers.roof[floorCount][index][3])));
                    double sixLenR = rounder(lenRaft / 6);
                    double purl = rounder(((double.Parse(cEF.structuralMembers.roof[floorCount][index][3]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][6])) + 1) * 2);
                    double lenPurl = rounder(purl * double.Parse(cEF.structuralMembers.roof[floorCount][index][4]));
                    double sixLenP = rounder(lenPurl / 6);
                    outputs.Add(1);
                    outputs.Add(3);
                    outputs.Add(sixLenR);
                    outputs.Add(sixLenP);
                }
            }
            else if (cEF.structuralMembers.roof[floorCount][index][0] == "G.I Roof and Its Accessories")
            {
                ListDictionary table1 = new ListDictionary();
                table1.Add("1.50", 14);
                table1.Add("1.80", 14);
                table1.Add("2.10", 18);
                table1.Add("2.40", 18);
                table1.Add("2.70", 22);
                table1.Add("3.00", 22);
                table1.Add("3.60", 26);
                ListDictionary table2 = new ListDictionary();
                table2.Add("1.50", 28);
                table2.Add("1.80", 28);
                table2.Add("2.10", 36);
                table2.Add("2.40", 36);
                table2.Add("2.70", 44);
                table2.Add("3.00", 44);
                table2.Add("3.60", 52);
                ListDictionary table3 = new ListDictionary();
                table3.Add("2\" x 3\"", 384);
                table3.Add("2\" x 4\"", 342);
                table3.Add("2\" x 5\"", 312);
                table3.Add("2\" x 6\"", 288);
                double corrSheets = 0;
                double giNails = 0;
                double rivets = 0;
                double giWashers = 0;
                double leadWashers = 0;
                double umbNails = 0;
                double plain_sheets = 0;
                List<double> materials = new List<double>();
                // corr - ginail - girivet - giwash - leadwash - umbnails - strap                             
                corrSheets = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][index][1]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][2])) * 2;                
                if (cEF.structuralMembers.roof[floorCount][index].Contains("G.I Roof Nails"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][index])
                    {
                        giNails += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 120;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][index].Contains("G.I Rivets"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][index])
                    {
                        rivets += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 180;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][index].Contains("G.I Washers"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][index])
                    {
                        giWashers += (corrSheets * double.Parse(table2[double.Parse(str).ToString("F")].ToString())) / 126;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][index].Contains("Lead Washers"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][index])
                    {
                        leadWashers += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 75;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][index].Contains("Umbrella Nails"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][index])
                    {
                        umbNails += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 120;
                    }
                }
                if (cEF.structuralMembers.roof[floorCount][index].Contains("Plain G.I Strap"))
                {
                    foreach (string str in cEF.structuralMembers.roofHRS[floorCount][index])
                    {
                        plain_sheets += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString()));
                    }
                    plain_sheets = plain_sheets / double.Parse(table3[cEF.structuralMembers.roof[floorCount][index][cEF.structuralMembers.roof[floorCount][index].Count - 1]].ToString());
                }
                outputs.Add(2);
                if (cEF.structuralMembers.roof[floorCount][index].Contains("Corrugated G.I Sheet"))
                {
                    outputs.Add(rounder(corrSheets));
                }
                else
                {
                    outputs.Add(0);
                }
                outputs.Add(rounder(giNails));
                outputs.Add(rounder(rivets));
                outputs.Add(rounder(giWashers));
                outputs.Add(rounder(leadWashers));
                outputs.Add(rounder(umbNails));
                outputs.Add(rounder(plain_sheets));
            }
            else
            {
                double gutter = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][index][1]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][2])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][index][3])));
                double flashing = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][index][4]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][5])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][index][6])));
                double ridge = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][index][7]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][8])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][index][9])));
                double valley = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][index][10]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][11])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][index][12])));
                double hipped = rounder((double.Parse(cEF.structuralMembers.roof[floorCount][index][13]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][14])) / (.90 / double.Parse(cEF.structuralMembers.roof[floorCount][index][15])));
                outputs.Add(3);
                outputs.Add(gutter + flashing + ridge + valley + hipped);
            }
            cEF.structuralMembers.roofSolutions[floorCount][index]= outputs;            
        }
        //Roof Computation Functions -- END

        //Masonry computation function -- START
        public List<double> computeMasonry(CostEstimationForm cEF, List<string[]> eWall, List<string[]> eWindow, List<string[]> eDoor, List<string[]> iWall, List<string[]> iWindow, List<string[]> iDoor, string eDimCHB, string iDimCHB)
        {
            double eWall_total = 0;
            double eWindow_total = 0;
            double eDoor_total = 0;
            double eCHB_area = 0;
            double eCHB_total;

            double iWall_total = 0;
            double iWindow_total = 0;
            double iDoor_total = 0;
            double iCHB_area = 0;
            double iCHB_total;

            List<double> masterMasonry = new List<double>();

            //exterior
            for (int x = 0; x < eWall.Count; x++)
            {
                double eOne = double.Parse(eWall[x][0]);
                double eTwo = double.Parse(eWall[x][1]);
                eWall_total += (eOne * eTwo) / 1000000;
            }

            for (int x = 0; x < eWindow.Count; x++)
            {
                double eOne = double.Parse(eWindow[x][0]);
                double eTwo = double.Parse(eWindow[x][1]);
                eWindow_total += (eOne * eTwo) / 1000000;
            }

            for (int x = 0; x < eDoor.Count; x++)
            {
                double eOne = double.Parse(eDoor[x][0]);
                double eTwo = double.Parse(eDoor[x][1]);
                eDoor_total += (eOne * eTwo) / 1000000;
            }
            eCHB_area = eWall_total - eWindow_total - eDoor_total;
            eCHB_total = eCHB_area * 12.5;
            eCHB_total = rounder((eCHB_total * 0.05) + eCHB_total);
            //interior
            for (int x = 0; x < iWall.Count; x++)
            {
                double eOne = double.Parse(iWall[x][0]);
                double eTwo = double.Parse(iWall[x][1]);
                iWall_total += (eOne * eTwo) / 1000000;
            }

            for (int x = 0; x < iWindow.Count; x++)
            {
                double eOne = double.Parse(iWindow[x][0]);
                double eTwo = double.Parse(iWindow[x][1]);
                iWindow_total += (eOne * eTwo) / 1000000;
            }

            for (int x = 0; x < iDoor.Count; x++)
            {
                double eOne = double.Parse(iDoor[x][0]);
                double eTwo = double.Parse(iDoor[x][1]);
                iDoor_total += (eOne * eTwo) / 1000000;
            }
            iCHB_area = iWall_total - iWindow_total - iDoor_total;
            iCHB_total = iCHB_area * 12.5;
            iCHB_total = rounder((iCHB_total * 0.05) + iCHB_total);

            masterMasonry.Add(eWall_total);
            masterMasonry.Add(eWindow_total);
            masterMasonry.Add(eDoor_total);
            masterMasonry.Add(eCHB_area);
            masterMasonry.Add(eCHB_total);
            masterMasonry.Add(iWall_total);
            masterMasonry.Add(iWindow_total);
            masterMasonry.Add(iDoor_total);
            masterMasonry.Add(iCHB_area);
            masterMasonry.Add(iCHB_total);
            cEF.extCHBdimension = eDimCHB;
            cEF.intCHBdimension = iDimCHB;
            return masterMasonry;
        }

        //Concrete wall function... still part of masonry
        public List<double> computeConcreteWall_mortar(CostEstimationForm cEF, string extM, string intM, string plasterCM, string plasterPT)
        {
            double[] cementDimA = { .792, .522, .394, .328, .0435 };
            double[] cementDimB = { 1.526, 1.018, .763, .633, .0844 };
            double[] cementDimC = { 2.260, 1.500, 1.125, .938, .1250 };
            double[] plasterClass = { 18.0, 12.0, 9.0, 7.5 };
            string[] mixtures = { extM, intM };
            string[] dimensions = { cEF.extCHBdimension, cEF.intCHBdimension };
            double[] dimCHB = { cEF.masonrysSolutionP1[3], cEF.masonrysSolutionP1[8] };
            double[] openings = { cEF.masonrysSolutionP1[0], cEF.masonrysSolutionP1[1], cEF.masonrysSolutionP1[2], cEF.masonrysSolutionP1[5], cEF.masonrysSolutionP1[6], cEF.masonrysSolutionP1[7] };
            List<double> cementAndsands = new List<double>();
            double cement;
            double sand;
            double plasterArea;
            double plasterCement;
            int pClassifier;

            for (int pass = 0; pass < 2; pass++)
            {
                int col;
                if (mixtures[pass] == "CLASS A")
                {
                    col = 0;
                }
                else if (mixtures[pass] == "CLASS B")
                {
                    col = 1;
                }
                else if (mixtures[pass] == "CLASS C")
                {
                    col = 2;
                }
                else
                {
                    col = 3;
                }

                if (dimensions[pass] == ".10 x .20 x .40")
                {
                    cement = rounder(dimCHB[pass] * cementDimA[col]);
                    sand = sandRounder(dimCHB[pass] * cementDimA[4]);
                }
                else if (dimensions[pass] == ".15 x .20 x .40")
                {
                    cement = rounder(dimCHB[pass] * cementDimB[col]);
                    sand = sandRounder(dimCHB[pass] * cementDimB[4]);
                }
                else
                {
                    cement = rounder(dimCHB[pass] * cementDimC[col]);
                    sand = sandRounder(dimCHB[pass] * cementDimC[4]);
                }
                cementAndsands.Add(cement);
                cementAndsands.Add(sand - (sand % .1));

            }

            //Plaster computation            
            for (int x = 1; x < 5; x += 3)
            {
                plasterArea = openings[x - 1] - (openings[x] + openings[x + 1]);
                if (plasterCM == "CLASS A")
                {
                    pClassifier = 0;
                    plasterCement = (plasterArea * (double.Parse(filterer(plasterPT)) / 1000)) * plasterClass[pClassifier];
                }
                else if (plasterCM == "CLASS B")
                {
                    pClassifier = 1;
                    plasterCement = (plasterArea * (double.Parse(filterer(plasterPT)) / 1000)) * plasterClass[pClassifier];
                }
                else if (plasterCM == "CLASS C")
                {
                    pClassifier = 2;
                    plasterCement = (plasterArea * (double.Parse(filterer(plasterPT)) / 1000)) * plasterClass[pClassifier];
                }
                else
                {
                    pClassifier = 3;
                    plasterCement = (plasterArea * (double.Parse(filterer(plasterPT)) / 1000)) * plasterClass[pClassifier];
                }
                cementAndsands.Add(plasterArea);
                cementAndsands.Add(rounder(plasterCement * 2));
                double handler = sandRounder(plasterArea * (double.Parse(filterer(plasterPT)) / 1000) * 2);
                cementAndsands.Add(handler - (handler % .1));
            }
            return cementAndsands;
        }

        public List<double> computeCHB_reinforcement(CostEstimationForm cEF, double eCHB, double iCHB, string vSpace, string hSpace, string grade, string diam, string rLen, string tWire)
        {
            List<double> mason = new List<double>();
            double[] vertical = { 2.93, 2.13, 1.60 };
            double[] horizontal = { 3.30, 2.15, 1.72 };
            //double[] diameter = { 0.616, 0.888, 1.578, 2.466, 3.853 };
            double[] diameter = { double.Parse(cEF.parameters.rein_W_dt.Rows[2][1].ToString()), double.Parse(cEF.parameters.rein_W_dt.Rows[3][1].ToString()), double.Parse(cEF.parameters.rein_W_dt.Rows[4][1].ToString()), double.Parse(cEF.parameters.rein_W_dt.Rows[5][1].ToString()), double.Parse(cEF.parameters.rein_W_dt.Rows[6][1].ToString()) };            
            double[,] tieXL = new double[,] { { .054, .039, .024 }, { .065, .047, .029 }, { .086, .063, .039 } };
            double[,] tieLX = new double[,] { { .036, .026, .020 }, { .044, .032, .024 }, { .057, .042, .032 } };
            double[,] tieLXXX = new double[,] { { .027, .020, .015 }, { .033, .024, .018 }, { .044, .032, .024 } };
            double[] chbS = { eCHB, iCHB };
            int vIndexer;
            int hIndexer;
            int diamIndexer;
            int tieIndexer;
            double vBAR;
            double hBAR;
            double reinforceCHB;
            double reinforceCHBweight;
            double tieWire;
            double[,] arrayHandler;
            //Vertical
            if (vSpace == ".40")
            {
                vIndexer = 0;
            }
            else if (vSpace == ".60")
            {
                vIndexer = 1;
            }
            else
            {
                vIndexer = 2;
            }
            //Horizontal
            if (hSpace == "2")
            {
                hIndexer = 0;
            }
            else if (hSpace == "3")
            {
                hIndexer = 1;
            }
            else
            {
                hIndexer = 2;
            }
            //Rebar Length
            if (diam == "10mm")
            {
                diamIndexer = 0;
            }
            else if (diam == "12mm")
            {
                diamIndexer = 1;
            }
            else if (diam == "16mm")
            {
                diamIndexer = 2;
            }
            else if (diam == "20mm")
            {
                diamIndexer = 3;
            }
            else
            {
                diamIndexer = 4;
            }
            //Tie Wire
            if (vSpace == ".40")
            {
                arrayHandler = tieXL;
            }
            else if (vSpace == ".60")
            {
                arrayHandler = tieLX;
            }
            else
            {
                arrayHandler = tieLXXX;
            }

            if (tWire == "25cm")
            {
                tieIndexer = 0;
            }
            else if (tWire == "30cm")
            {
                tieIndexer = 1;
            }
            else
            {
                tieIndexer = 2;
            }

            for (int x = 0; x < 2; x++)
            {
                vBAR = chbS[x] * vertical[vIndexer];
                hBAR = chbS[x] * horizontal[hIndexer];
                reinforceCHB = rounder((vBAR + hBAR) / double.Parse(filterer(rLen)));
                reinforceCHBweight = rounder(reinforceCHB * diameter[diamIndexer] * double.Parse(filterer(rLen)));
                print("VBAR: " + vBAR);
                print("hBAR: " + hBAR);
                print("CHB pieces: "+reinforceCHB);
                print("weight lol:"+ reinforceCHBweight);
                print("diameter inx: " + diameter[diamIndexer]);
                print("rlen: " + double.Parse(filterer(rLen)));
                tieWire = rounder(chbS[x] * arrayHandler[tieIndexer, hIndexer]);
                print("TW: " + tieWire);
                mason.Add(vBAR);
                mason.Add(hBAR);
                mason.Add(reinforceCHB);
                mason.Add(reinforceCHBweight);
                mason.Add(tieWire);                
            }
            return mason;
        }
        //Masonry computation function -- END

        //Tiles computation function -- START
        public void computeTiles(CostEstimationForm cEF)
        {
            //Init variables for tiles
            double sixhundred_AREA = 0;
            double sixhundred_PCS = 0;
            double sixhundred_ADH_REG = 0;
            double sixhundred_ADH_HVY = 0;            
            double sixhundred_GRO = 0;
            double threehundred_AREA = 0;
            double threehundred_PCS = 0;
            double threehundred_ADH_REG = 0;
            double threehundred_ADH_HVY = 0;
            double threehundred_GRO = 0;
            List<double> six_handler = new List<double>();
            List<double> three_handler = new List<double>();
            for (int x = 0; x < cEF.parameters.tiles_Area.Count; x++)
            {
                //Init variables for computing tiles
                List<string> dimension = dimensionFilterer(cEF.parameters.tiles_Area[x][1]);
                double dim1 = double.Parse(dimension[0]) / 1000;
                double dim2 = double.Parse(dimension[1]) / 1000;
                double area = double.Parse(cEF.parameters.tiles_Area[x][0]);
                double total_tiles = rounder(area / (dim1 * dim2));
                total_tiles = rounder(total_tiles + (total_tiles / double.Parse(filterer(cEF.parameters.tiles_FS))));//tiles
                double adhesive_bag = rounder(area / 3);//adhesive bag
                double grout_bag = rounder(area / 4);//grout bag                
                

                if (cEF.parameters.tiles_Area[x][1] == "600 x 600")
                {
                    sixhundred_AREA += area;
                    sixhundred_PCS += total_tiles;                    
                    sixhundred_GRO += grout_bag;
                    if (cEF.parameters.tiles_Area[x][2] == "Regular")
                    {
                        sixhundred_ADH_REG += adhesive_bag;
                    }
                    else
                    {
                        sixhundred_ADH_HVY += adhesive_bag;
                    }
                }
                else
                {
                    threehundred_AREA += area;
                    threehundred_PCS += total_tiles;                    
                    threehundred_GRO += grout_bag;
                    if (cEF.parameters.tiles_Area[x][2] == "Regular")
                    {
                        threehundred_ADH_REG += adhesive_bag;
                    }
                    else
                    {
                        threehundred_ADH_HVY += adhesive_bag;
                    }
                }
            }
            six_handler.Add(sixhundred_AREA);
            six_handler.Add(sixhundred_PCS);
            six_handler.Add(sixhundred_ADH_REG);
            six_handler.Add(sixhundred_ADH_HVY);
            six_handler.Add(sixhundred_GRO);
            cEF.sixhun = six_handler;
            three_handler.Add(threehundred_AREA);
            three_handler.Add(threehundred_PCS);
            three_handler.Add(threehundred_ADH_REG);
            three_handler.Add(threehundred_ADH_HVY);
            three_handler.Add(threehundred_GRO);
            cEF.threehun = three_handler;
        }
        //Tiles computation function -- END

        //Paints computataion function -- START
        public void computePaints(CostEstimationForm cEF)
        {
            //Init variables for each paints
            double enam_neut = 0;
            double enam_skim = 0;
            double enam_primer = 0;
            double enam_paint = 0;
            double acry_neut = 0;
            double acry_skim = 0;
            double acry_primer = 0;
            double acry_paint = 0;
            double late_neut = 0;
            double late_skim = 0;
            double late_primer = 0;
            double late_paint = 0;
            double semi_neut = 0;
            double semi_skim = 0;
            double semi_primer = 0;
            double semi_paint = 0;
            double enam_Area = 0;
            double acry_Area = 0;
            double late_Area = 0;
            double semi_Area = 0;
            List<double> enamel_handler = new List<double>();
            List<double> acry_handler = new List<double>();
            List<double> late_handler = new List<double>();
            List<double> semi_handler = new List<double>();
            //compute every paint area
            for (int x = 0; x < cEF.parameters.paint_Area.Count; x++)
            {            
                double sLayer = double.Parse(cEF.parameters.paint_SCL, System.Globalization.CultureInfo.InvariantCulture);
                double pArea = double.Parse(cEF.parameters.paint_Area[x][0], System.Globalization.CultureInfo.InvariantCulture);
                double pLayer = double.Parse(cEF.parameters.paint_Area[x][2], System.Globalization.CultureInfo.InvariantCulture);            
                double neutGal = rounder((pArea / 20)/3);//neutralizer
                double skimBags = rounder(pArea / 20);//skim coating            
                double primerGal = rounder(pArea / 20);//primer             
                double paintGal = rounder((pArea / 25) * pLayer);//paint gallon                         
                string paintKind = cEF.parameters.paint_Area[x][1];
                if (paintKind == "Enamel")
                {
                    enam_neut += neutGal;
                    enam_skim += skimBags;
                    enam_primer += primerGal;
                    enam_paint += paintGal;
                    enam_Area += pArea;
                }
                else if (paintKind == "Acrylic")
                {
                    acry_neut += neutGal;
                    acry_skim += skimBags;
                    acry_primer += primerGal;
                    acry_paint += paintGal;
                    acry_Area += pArea;
                }
                else if (paintKind == "Latex Gloss")
                {
                    late_neut += neutGal;
                    late_skim += skimBags;
                    late_primer += primerGal;
                    late_paint += paintGal;
                    late_Area += pArea;
                }
                else if (paintKind == "Semi-gloss")
                {
                    semi_neut += neutGal;
                    semi_skim += skimBags;
                    semi_primer += primerGal;
                    semi_paint += paintGal;
                    semi_Area += pArea;
                }
            }
            //add each paints values to its corresponding kind
            enamel_handler.Add(enam_Area);
            enamel_handler.Add(enam_neut);
            enamel_handler.Add(enam_skim);
            enamel_handler.Add(enam_primer);
            enamel_handler.Add(enam_paint);
            cEF.enamel = enamel_handler;
            //
            acry_handler.Add(acry_Area);
            acry_handler.Add(acry_neut);
            acry_handler.Add(acry_skim);
            acry_handler.Add(acry_primer);
            acry_handler.Add(acry_paint);
            cEF.acrylic = acry_handler;
            //
            late_handler.Add(late_Area);
            late_handler.Add(late_neut);
            late_handler.Add(late_skim);
            late_handler.Add(late_primer);
            late_handler.Add(late_paint);
            cEF.latex = late_handler;
            //
            semi_handler.Add(semi_Area);
            semi_handler.Add(semi_neut);
            semi_handler.Add(semi_skim);
            semi_handler.Add(semi_primer);
            semi_handler.Add(semi_paint);
            cEF.gloss = semi_handler;
        }
        //Paints computataion function -- END

        //FormworksWORK -- START


        //FormworksWORK -- END



        //Helper functions --- START
        public void print(string str)
        {
            Console.WriteLine(str);
        }
        public double nearestValue(double val, int multiple)
        {         
                double roundedValue = (multiple * (Math.Truncate(val / multiple))) + multiple;
                return roundedValue;
        }
        public double rounder(double val)
        {
            double rounded_val = val;
            if ((val % 1) != 0)
            {
                 rounded_val = Math.Truncate(val) + 1;
                return rounded_val;
            }
            else
            {
                return rounded_val;
            }
        }

        public string filterer(string str)
        {
            string retStr = "";
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                {
                    retStr += c;
                }
                else
                {
                    break;
                }
            }
            return retStr;
        }

        public static List<string> dimensionFilterer (string str)
        {
            List<string> dims = new List<string>();
            string retStr = "";
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                {
                    retStr += c;
                }
                else
                {
                    if(!Char.IsWhiteSpace(c))
                    {
                        dims.Add(retStr);
                        retStr = "";
                    }
                }
            }
            dims.Add(retStr);
            return dims;
        }

        public double sandRounder(double x)
        {
            if (x % 10 > 0)
            {
                x = (int)x + ((x - (int)x) + .10);
                return x;
            }
            else
            {
                return x;
            }            
        }
        //Helper functions -- END

        public double stairsRounder(double x)
        {
            x = x * 1000;            
            if ((x % 5) == 0)
            {
                return x;
            }
            else
            {
                for (; ;x++)
                {                    
                    if ((x % 5) == 0)
                    {
                        x = x / 1000;
                        break;
                    }
                }                
                return x;
            }
        }

        public string spliceMixGetter(string mix)
        {
            string getted = "";
            for(int i = 0; i< mix.Length; i++)
            {
                if (mix[i].Equals('('))
                {
                    i++;
                    for( int j = i; j < mix.Length; j++)
                    {
                        if (Char.IsDigit(mix[j]) || mix[j].Equals('.'))
                        {
                            getted += mix[j];
                        }
                        else
                        {
                            return getted;                            
                        }                                        
                    }
                }                
            }
            return getted;
        }

        public double reinGetter(string str)
        {
            List<double> two = new List<double>();
            string num = "";
            bool fraction = false;
            foreach (char a in str)
            {
                if (a.Equals('/'))
                {
                    two.Add(double.Parse(num));
                    fraction = true;
                    num = "";
                }
                else if(Char.IsDigit(a))
                {
                    num += a;
                }
            }
            if (fraction)
            {
                two.Add(double.Parse(num));
                double rein = two[0] / two[1];
                return rein;
            }
            else
            {
                return double.Parse(str);
            }            
        }
        
        public List<string> beamMainML(int sol, string lbtop, string lbbot, string mtop, string mbot, string qty, CostEstimationForm cEF, string extraA, string extraB, string extraBot ,string etopA, string etopB, string ebot)
        {
            List<string> summary = new List<string>();
            double qtypn = 0;
            double lw = 0;
            double qtymn = 0;
            double lem = 0;
            double totalwaste = 0;            
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };

            double bestlm = 0;
            double bestqtym = 0;
            double bestTW = 0;

            double bestlm2 = 0;
            double bestqtym2 = 0;
            double bestTW2 = 0;

            double lbtop_d = double.Parse(lbtop)/1000;
            double lbbot_d = double.Parse(lbbot)/1000;
            double mtop_d = double.Parse(mtop);
            double mbot_d = double.Parse(mbot);
            double qty_d = double.Parse(qty);
            double extraAd = double.Parse(extraA)/1000;
            double extraBd = double.Parse(extraB)/1000;
            double extraBotd = double.Parse(extraBot) / 1000;

            double etopA_d = double.Parse(etopA);
            double etopB_d = double.Parse(etopB);
            double ebot_d = double.Parse(ebot);

            for (int i = 0; i < 7; i++)
            {                
                if(sol == 0)
                {
                    if (cEF.parameters.rein_mfIsSelected[4, i])
                    {                        
                        qtypn = mls[i] / lbtop_d;
                        lw = (qtypn - Math.Floor(qtypn)) * lbtop_d;
                        qtymn = ((mtop_d + mbot_d)/ Math.Floor(qtypn)) * qty_d;
                        lem = (rounder(qtymn) - qtymn) * mls[i];
                        totalwaste = lem + lw * (Math.Floor(qtymn));                        
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestqtym = qtymn;
                            bestlm = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestqtym = qtymn;
                                bestlm = mls[i];
                                print(bestlm.ToString());
                            }
                        }                        
                    }                    
                }
                else if(sol == 1)
                {
                    if (cEF.parameters.rein_mfIsSelected[4, i])
                    {
                        qtypn = mls[i] / lbtop_d;
                        lw = (qtypn - Math.Floor(qtypn)) * lbtop_d;
                        qtymn = ((mtop_d) / Math.Floor(qtypn)) * qty_d;
                        lem = (rounder(qtymn) - qtymn) * mls[i];
                        totalwaste = lem + lw * (Math.Floor(qtymn));
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestqtym = qtymn;
                            bestlm = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestqtym = qtymn;
                                bestlm = mls[i];                                
                            }
                        }
                        ///
                        qtypn = mls[i] / lbbot_d;
                        lw = (qtypn - Math.Floor(qtypn)) * lbbot_d;
                        qtymn = ((mbot_d) / Math.Floor(qtypn)) * qty_d;
                        lem = (rounder(qtymn) - qtymn) * mls[i];
                        totalwaste = lem + lw * (Math.Floor(qtymn));
                        if (bestTW2 == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW2 = totalwaste;
                            bestqtym2 = qtymn;
                            bestlm2 = mls[i];
                        }
                        else
                        {
                            if (bestTW2 > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW2 = totalwaste;
                                bestqtym2 = qtymn;
                                bestlm2 = mls[i];                                
                            }
                        }
                    }
                }
                else if(sol == 2)
                {
                    if (cEF.parameters.rein_mfIsSelected[4, i])
                    {
                        qtypn = mls[i] / extraAd;
                        lw = (qtypn - Math.Floor(qtypn)) * extraAd;
                        qtymn = ((etopA_d + etopB_d) / Math.Floor(qtypn)) * qty_d;
                        lem = (rounder(qtymn) - qtymn) * mls[i];
                        totalwaste = lem + lw * (Math.Floor(qtymn));
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestqtym = qtymn;
                            bestlm = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestqtym = qtymn;
                                bestlm = mls[i];
                                print(bestlm.ToString());
                            }
                        }
                    }
                }
                else if(sol == 3)
                {
                    if (cEF.parameters.rein_mfIsSelected[4, i])
                    {
                        qtypn = mls[i] / extraAd;
                        lw = (qtypn - Math.Floor(qtypn)) * extraAd;
                        qtymn = ((etopA_d) / Math.Floor(qtypn)) * qty_d;
                        lem = (rounder(qtymn) - qtymn) * mls[i];
                        totalwaste = lem + lw * (Math.Floor(qtymn));
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestqtym = qtymn;
                            bestlm = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestqtym = qtymn;
                                bestlm = mls[i];
                            }
                        }
                        ///
                        qtypn = mls[i] / extraBd;
                        lw = (qtypn - Math.Floor(qtypn)) * extraBd;
                        qtymn = ((etopB_d) / Math.Floor(qtypn)) * qty_d;
                        lem = (rounder(qtymn) - qtymn) * mls[i];
                        totalwaste = lem + lw * (Math.Floor(qtymn));
                        if (bestTW2 == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW2 = totalwaste;
                            bestqtym2 = qtymn;
                            bestlm2 = mls[i];
                        }
                        else
                        {
                            if (bestTW2 > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW2 = totalwaste;
                                bestqtym2 = qtymn;
                                bestlm2 = mls[i];
                            }
                        }                        
                    }
                }
                else if(sol == 4)
                {
                    if (cEF.parameters.rein_mfIsSelected[4, i])
                    {
                        qtypn = mls[i] / extraBotd;
                        lw = (qtypn - Math.Floor(qtypn)) * extraBotd;
                        qtymn = ((ebot_d) / Math.Floor(qtypn)) * qty_d;
                        lem = (rounder(qtymn) - qtymn) * mls[i];
                        totalwaste = lem + lw * (Math.Floor(qtymn));
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestqtym = qtymn;
                            bestlm = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestqtym = qtymn;
                                bestlm = mls[i];                                
                            }
                        }
                    }
                }
            }            
            summary.Add(bestlm.ToString());
            summary.Add(rounder(bestqtym).ToString());
            summary.Add(bestlm2.ToString());
            summary.Add(rounder(bestqtym2).ToString());
            return summary;
        }

        public List<string> stiruphelper (int sup, string lb, string rowQ, string qtyA, string qtyB, string qtyC, string Pquantity, string props, CostEstimationForm cEF)
        {
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };
            double qtyp = 0;
            double qtym = 0;
            double lw = 0;
            double le = 0;
            double totalwaste = 0;
            double m = 0;
            double bestTW = 0;
            double bestLCM = 0;
            double bestQTYM = 0;
            if (props == "No. 1")
            {
                m = 1;
            }
            else
            {
                m = 2;
            }
            for (int i = 0; i < 7; i++)
            {
                if (cEF.parameters.rein_mfIsSelected[4, i])
                {
                    if (double.Parse(lb) == 0)
                    {
                        qtyp = 0;
                    }
                    else
                    {
                        qtyp = mls[i] / double.Parse(lb);
                    }
                    if (sup == 2)
                    {
                        qtym = ((m * double.Parse(rowQ) * (2 * (double.Parse(qtyA) + double.Parse(qtyB)) + double.Parse(qtyC))) / Math.Floor(qtyp)) * double.Parse(Pquantity);
                    }
                    else
                    {
                        qtym = ((m * double.Parse(rowQ) * ((double.Parse(qtyA) + double.Parse(qtyB)) + double.Parse(qtyC))) / Math.Floor(qtyp)) * double.Parse(Pquantity);
                    }
                    lw = (qtyp - Math.Floor(qtyp)) * double.Parse(lb);
                    le = (rounder(qtym) - qtym) * mls[i];
                    totalwaste = le + lw * Math.Floor(qtym);
                    if (bestTW == 0 && !Double.IsNaN(totalwaste))
                    {
                        bestTW = totalwaste;
                        bestQTYM = qtym;
                        bestLCM = mls[i];
                    }
                    else
                    {
                        if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestQTYM = qtym;
                            bestLCM = mls[i];                            
                        }
                    }
                }
            }
            List<string> stirups = new List<string>();
            stirups.Add(bestLCM.ToString());
            stirups.Add(rounder(bestQTYM).ToString());
            return stirups;
        }

        public List<string> webBeamHelper(CostEstimationForm cEF, double lb, double qtyRow, double qtyWhole, double qtydia)
        {
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };
            double bestTW = 0;
            double bestLM = 0;
            double bestQTYM = 0;            
            for (int i = 0; i < 7; i++)
            {
                if (cEF.parameters.rein_mfIsSelected[4, i])
                {
                    double qtyp = mls[i] / lb;
                    double qtym = ((qtyRow * qtydia) / Math.Floor(qtyp)) * qtyWhole;
                    double lw = (qtyp - Math.Floor(qtyp)) * lb;
                    double le = (rounder(qtym) - qtym) * mls[i];
                    double totalwaste = le + lw * Math.Floor(qtym);
                    if (bestTW == 0 && !Double.IsNaN(totalwaste))
                    {
                        bestTW = totalwaste;
                        bestQTYM = qtym;
                        bestLM = mls[i];
                    }
                    else
                    {
                        if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestQTYM = qtym;
                            bestLM = mls[i];
                        }
                    }
                }
            }            
            List<string> webs = new List<string>();
            webs.Add(bestLM.ToString());
            webs.Add(rounder(bestQTYM).ToString());
            return webs;
        }
        //slabELPER(cEF, ELA, ELB, qtyA, qtyB, sl_long, sl_trans);
        public List<string> slabELPER(CostEstimationForm cEF, double ELA, double ELB, double qtyA, double qtyB, double sl_long, double sl_trans, double la, double lb)
        {
            
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };

            double largestLM = 0;
            for (int i = 0; i < 7; i++)
            {
                if (cEF.parameters.rein_mfIsSelected[5, i])
                {
                    if(largestLM < mls[i])
                    {
                        largestLM = mls[i];
                    }
                }
            }

            double qtyp = 0;
            double lw = 0;
            double qtym = 0;
            double le = 0;
            double lbin = 0;
            double totalwaste = 0;

            double bestTW = 0;
            double bestQTYM = 0;
            double bestLM = 0;
            double bestQTYCM = 0;

            double bestTW2 = 0;
            double bestQTYM2 = 0;
            double bestLM2 = 0;
            double bestQTYCM2 = 0;

            double Adivisor = 0;
            double Bdivisor = 0;

            double qtycm = 0;

            List<string> slabs = new List<string>();
            slabs.Add("0");
            slabs.Add("0");
            slabs.Add("0");
            slabs.Add("0");

            if (la >= lb)
            {
                Adivisor = sl_long/1000;
                Bdivisor = sl_trans/1000;
            }
            else
            {
                Adivisor = sl_trans/1000;
                Bdivisor = sl_long/1000;
            }

            bool categ1 = false;
            bool categ2 = false;
            for (int i = 0; i < 7; i++)
            {
                if (cEF.parameters.rein_mfIsSelected[5, i])
                {
                    if(ELA < largestLM)
                    {                        
                        qtyp = mls[i] / ELA;
                        if(qtyp < 1)
                        {
                            lw = 0;
                        }
                        else
                        {
                            lw = mls[i] - ELA * Math.Floor(qtyp);
                        }
                        qtym = qtyA / Math.Floor(qtyp);
                        le = (rounder(qtym) - qtym) * mls[i];
                        totalwaste = le + lw * Math.Floor(qtym);                        
                    }
                    else
                    {
                        categ1 = true;
                        qtym = ELA / (mls[i] - Adivisor);
                        lbin = (qtym - Math.Floor(qtym)) * (mls[i] - Adivisor);
                        le = mls[i] - lbin;
                        totalwaste = le * qtyA;
                        qtycm = qtyA * rounder(qtym);
                    }

                    if (bestTW == 0 && !Double.IsNaN(totalwaste))
                    {
                        bestTW = totalwaste;
                        bestQTYM = qtym;
                        bestLM = mls[i];
                        bestQTYCM = qtycm;
                    }
                    else
                    {
                        if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestQTYM = qtym;
                            bestLM = mls[i];
                            bestQTYCM = qtycm;
                        }
                    }

                    /////////////

                    if (ELB < largestLM)
                    {                        
                        qtyp = mls[i] / ELB;
                        if (qtyp < 1)
                        {
                            lw = 0;
                        }
                        else
                        {
                            lw = mls[i] - ELB * Math.Floor(qtyp);
                        }
                        qtym = qtyB / Math.Floor(qtyp);
                        le = (rounder(qtym) - qtym) * mls[i];
                        totalwaste = le + lw * Math.Floor(qtym);                        
                    }
                    else
                    {
                        categ2 = true;
                        qtym = ELB / (mls[i] - Bdivisor);
                        lbin = (qtym - Math.Floor(qtym)) * (mls[i] - Bdivisor);
                        le = mls[i] - lbin;
                        totalwaste = le * qtyB;
                        qtycm = qtyB * rounder(qtym);
                    }
                    if (bestTW2 == 0 && !Double.IsNaN(totalwaste))
                    {
                        bestTW2 = totalwaste;
                        bestQTYM2 = qtym;
                        bestLM2 = mls[i];
                        bestQTYCM2 = qtycm;
                    }
                    else
                    {
                        if (bestTW2 > totalwaste && !Double.IsNaN(totalwaste))
                        {
                            bestTW2 = totalwaste;
                            bestQTYM2 = qtym;
                            bestLM2 = mls[i];
                            bestQTYCM2 = qtycm;
                        }
                    }
                }
            }
            slabs[0] = bestLM.ToString();
            slabs[2] = bestLM2.ToString();
            if (categ1)
            {
                slabs[1] = bestQTYCM.ToString();
            }
            else
            {
                slabs[1] = bestQTYM.ToString();
            }
            if (categ2)
            {
                slabs[3] = bestQTYCM2.ToString();
            }
            else
            {
                slabs[3] = bestQTYM2.ToString();
            }
            return slabs;
        }

        public List<string> columnMainRebarHelper(CostEstimationForm cEF, double lb, double colQTY, double diaQTY)
        {
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };
            double bestTW = 0;
            double bestQTYM = 0;
            double bestLM = 0;
            for (int i = 0; i < 7; i++)
            {
                if (cEF.parameters.rein_mfIsSelected[3, i])
                {
                    double qtyp = mls[i] / lb;
                    double lw = (qtyp - Math.Floor(qtyp)) * lb;
                    double qtym = (diaQTY / Math.Floor(qtyp)) * colQTY;
                    double le = (rounder(qtym) - qtym) * mls[i];
                    double totalwaste = le + lw * (Math.Floor(qtym));
                    if (bestTW == 0 && !Double.IsNaN(totalwaste))
                    {
                        bestTW = totalwaste;
                        bestQTYM = qtym;
                        bestLM = mls[i];                        
                    }
                    else
                    {
                        if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestQTYM = qtym;
                            bestLM = mls[i];                            
                        }
                    }
                }
            }
            List<string> colMs = new List<string>();
            colMs.Add(bestLM.ToString());
            colMs.Add(rounder(bestQTYM).ToString());
            return colMs;
        }

        public List<List<string>> lateralTiesA(List<double> lb, double qte1, double qte2, double summation, double qtytatrest, string config, double colQTY, double green, double red, string level, CostEstimationForm cEF, double lat_d)
        {
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };
            print("qte1: " + qte1);
            print("qte2: " + qte2);
            print("summation: " + summation);
            print("atrest: " + qtytatrest);
            List<List<string>> lats = new List<List<string>>();
            for (int ct = 0; ct < lb.Count; ct++)
            {
                print("lb" + lb[ct]);
                double bestTW = 0;
                double bestQTYM = 0;
                double bestLM = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (cEF.parameters.rein_mfIsSelected[3, i])
                    {
                        double qtymn = 0;
                        double totalwaste = 0;
                        double qtypn = mls[i] / lb[ct];                        
                        double qtex = qte1 + qte2;
                        if (qtex == 0)
                        {
                            qtex = summation;
                        }
                        if (level == "Ground")
                        {                            
                            if (config == "Lateral Ties Config 1" || config == "Lateral Ties Config 2" || config == "Lateral Ties Config 3" || config == "Lateral Ties Config 4")
                            {
                                qtymn = ((qtex + summation + qtytatrest) / Math.Floor(qtypn)) * colQTY;
                            }
                            else
                            {
                                if(ct == 0)
                                {
                                    qtymn = ((qtex + summation + qtytatrest) / Math.Floor(qtypn)) * colQTY;
                                }
                                else if(ct == 1)
                                {
                                    qtymn = (((qtex + summation + qtytatrest) * green) / Math.Floor(qtypn)) * colQTY;
                                }
                                else
                                {
                                    qtymn = (((qtex + summation + qtytatrest) * red) / Math.Floor(qtypn)) * colQTY;
                                }                                
                            }
                            double lw = (qtypn - Math.Floor(qtypn)) * lb[ct];
                            double lem = (rounder(qtymn) - qtymn) * mls[i];
                            totalwaste = lem + lw * (Math.Floor(qtymn));
                        }
                        else
                        {
                            if (config == "Lateral Ties Config 1" || config == "Lateral Ties Config 2" || config == "Lateral Ties Config 3" || config == "Lateral Ties Config 4")
                            {
                                qtymn = (((2 * summation) + qtytatrest)/Math.Floor(qtypn)) * colQTY;
                            }
                            else
                            {
                                if(ct == 0)
                                {
                                    qtymn = (((summation * 2) + qtytatrest) / Math.Floor(qtypn)) * colQTY;
                                }
                                else if(ct == 1)
                                {
                                    qtymn = ((((summation * 2) + qtytatrest) * green) / Math.Floor(qtypn)) * colQTY;
                                }
                                else
                                {
                                    qtymn = ((((summation * 2) + qtytatrest) * red) / Math.Floor(qtypn)) * colQTY;
                                }                                
                            }
                            double lw = (qtypn - Math.Floor(qtypn)) * lb[ct];
                            double lem = (rounder(qtymn) - qtymn) * mls[i];
                            totalwaste = lem + lw * (Math.Floor(qtymn));
                        }                        
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestQTYM = qtymn;
                            bestLM = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestQTYM = qtymn;
                                bestLM = mls[i];
                            }
                        }
                    }
                }
                List<string> produced_ML = new List<string>();
                produced_ML.Add(bestLM.ToString());
                produced_ML.Add(rounder(bestQTYM).ToString());
                produced_ML.Add(lat_d.ToString());
                lats.Add(produced_ML);
            }
            return lats;
        }
        public List<List<string>> lateralTiesB(List<double> lb, double qte1, double qte2, double summation, double qtytatrest, string config, double colQTY, double green, double red, string level, CostEstimationForm cEF , double qtytq, double joint_d)
        {
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };
            List<List<string>> lats = new List<List<string>>();
            for (int ct = 0; ct < lb.Count; ct++)
            {
                double bestTW = 0;
                double bestQTYM = 0;
                double bestLM = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (cEF.parameters.rein_mfIsSelected[3, i])
                    {
                        double qtymn = 0;
                        double totalwaste = 0;
                        double qtypn = mls[i] / lb[ct];
                        double qtex = qte1 + qte2;
                        if (qtex == 0)
                        {
                            qtex = summation;
                        }
                        if (level == "Ground")
                        {
                            if (config == "Lateral Ties Config 1" || config == "Lateral Ties Config 2" || config == "Lateral Ties Config 3" || config == "Lateral Ties Config 4")
                            {
                                qtymn = ((qtytq) / Math.Floor(qtypn)) * colQTY;
                            }
                            else
                            {
                                if (ct == 0)
                                {
                                    qtymn = ((qtytq) / Math.Floor(qtypn)) * colQTY;
                                }
                                else if (ct == 1)
                                {
                                    qtymn = (((qtytq) * green) / Math.Floor(qtypn)) * colQTY;
                                }
                                else
                                {
                                    qtymn = (((qtytq) * red) / Math.Floor(qtypn)) * colQTY;
                                }
                            }
                            double lw = (qtypn - Math.Floor(qtypn)) * lb[ct];
                            double lem = (rounder(qtymn) - qtymn) * mls[i];
                            totalwaste = lem + lw * (Math.Floor(qtymn));
                        }
                        else
                        {
                            if (config == "Lateral Ties Config 1" || config == "Lateral Ties Config 2" || config == "Lateral Ties Config 3" || config == "Lateral Ties Config 4")
                            {
                                qtymn = ((qtytq) / Math.Floor(qtypn)) * colQTY;
                            }
                            else
                            {
                                if (ct == 0)
                                {
                                    qtymn = ((qtytq) / Math.Floor(qtypn)) * colQTY;
                                }
                                else if (ct == 1)
                                {
                                    qtymn = (((qtytq) * green) / Math.Floor(qtypn)) * colQTY;
                                }
                                else
                                {
                                    qtymn = (((qtytq) * red) / Math.Floor(qtypn)) * colQTY;
                                }
                            }
                            double lw = (qtypn - Math.Floor(qtypn)) * lb[ct];
                            double lem = (rounder(qtymn) - qtymn) * mls[i];
                            totalwaste = lem + lw * (Math.Floor(qtymn));
                        }
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestQTYM = qtymn;
                            bestLM = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestQTYM = qtymn;
                                bestLM = mls[i];
                            }
                        }
                    }
                }
                List<string> produced_ML = new List<string>();
                produced_ML.Add(bestLM.ToString());
                produced_ML.Add(rounder(bestQTYM).ToString());
                produced_ML.Add(joint_d.ToString());
                lats.Add(produced_ML);
            }
            return lats;
        }

        public List<double> rebarsSuspendedSlabElper(List<double> len_bars, List<double> slab_quantities, double qty, CostEstimationForm cEF)
        {
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0, 13.5, 15.0 };
            double bestTW = 0;
            double bestLM = 0;
            double bestQTYM = 0;
            List<double> results = new List<double>();
            for (int r = 0; r < len_bars.Count; r++)
            {        
                for (int i = 0; i < 7; i++)
                {
                    if (cEF.parameters.rein_mfIsSelected[6, i])
                    {
                        double qtypn = mls[i] / len_bars[r];
                        double lw = (qtypn - Math.Floor(qtypn)) * len_bars[r];
                        double qtymn = (slab_quantities[r] / Math.Floor(qtypn)) * qty;
                        double lem = (rounder(qtymn) - qtymn) * mls[i];
                        double totalwaste = lem + lw * (Math.Floor(qtymn));
                        //
                        if (bestTW == 0 && !Double.IsNaN(totalwaste))
                        {
                            bestTW = totalwaste;
                            bestQTYM = qtymn;
                            bestLM = mls[i];
                        }
                        else
                        {
                            if (bestTW > totalwaste && !Double.IsNaN(totalwaste))
                            {
                                bestTW = totalwaste;
                                bestQTYM = qtymn;
                                bestLM = mls[i];
                            }
                        }
                    }
                }
                results.Add(bestLM);
                results.Add(rounder(bestQTYM));
            }
            return results;
        }
        
        
        public double[,] stairsRebarsElper(CostEstimationForm cEF, double prot, double bar_quantity, double diameter, double stair_quantity)
        {
            double[] mls = { 6.0, 7.5, 9.0, 10.5, 12.0 };
            double[,] results = new double[,]{
                                                        { 123, 123, 123, 123},// 6
                                                        { 123, 123, 123, 123},// 7.5
                                                        { 123, 123, 123, 123},// 9
                                                        { 123, 123, 123, 123},// 10.5
                                                        { 123, 123, 123, 123},// 12
                                                        { 1, 0, 0, 0}
                                                    };
            double weight_equi = 0;
            for (int r = 0; r < cEF.parameters.rein_W_dt.Rows.Count; r++)
            {                
                if ((cEF.parameters.rein_W_dt.Rows[r][0]).ToString() == diameter.ToString()+"mm")
                {
                    weight_equi = double.Parse(cEF.parameters.rein_W_dt.Rows[r][1].ToString());
                    break;
                }                
            }
            double bestTW = 0;
            double bestLM = 0;
            double bestQTYM = 0;
            prot = prot / 1000;
            
            for (int i = 0; i < 5; i++)
            {
                double step1 = mls[i] / prot; // step 1                
                double step2_wholenumber = Math.Floor(step1); // step 2                
                double step2_afterdecimal = step1 - Math.Floor(step1);// step 3                
                double waste_length = step2_afterdecimal * prot; // step 4                
                double waste_per_piece = (waste_length / mls[i]); // step 5                
                double not_qtym = bar_quantity / step2_wholenumber; // step 6                
                double qtym = rounder(bar_quantity / step2_wholenumber);
                double totwaste1 = (waste_length * Math.Floor(not_qtym));                
                double totwaste2 = mls[i] - prot;                
                double totalwaste = totwaste1 + totwaste2;
                double waste_percentage = (totalwaste / (mls[i] * qtym));                
                double waste_percent_per = (totwaste2 / mls[i]);                
                double ave_waste = (waste_per_piece + waste_percentage + waste_percent_per)/3;                
                results[i, 0] = diameter;
                results[i, 1] = mls[i];
                results[i, 2] = qtym * stair_quantity;
                results[i, 3] = weight_equi;
                
                if (bestTW == 0 && !Double.IsNaN(ave_waste))
                {
                    bestTW = ave_waste;
                    bestQTYM = qtym;
                    bestLM = mls[i];
                }
                else
                {
                    if (bestTW > ave_waste && !Double.IsNaN(ave_waste))
                    {
                        bestTW = ave_waste;
                        bestQTYM = qtym;
                        bestLM = mls[i];
                    }
                }
            }

            double suggestion = 0;
            if(bestLM == 6.0)
            {
                suggestion = 0;
            }
            else if (bestLM == 7.5)
            {
                suggestion = 1;
            }
            else if (bestLM == 9.0){
                suggestion = 2;
            }
            else if (bestLM == 10.5)
            {
                suggestion = 3;
            }
            else if (bestLM == 12.0)
            {
                suggestion = 4;
            }                        
            results[5, 0] = suggestion;
            return results;
            
        }

        public string bestLMSgetter(double[,] res)
        {            
            if (res[5,0] == 0)
            {
                return "6.0m";
            }
            else if (res[5, 0] == 1)
            {
                return "7.5m";
            }
            else if (res[5, 0] == 2)
            {
                return "9.0m";
            }
            else if (res[5, 0] == 3)
            {
                return "10.5m";
            }
            else if (res[5, 0] == 4)
            {
                return "12.0m";
            }
            else
            {
                return "eh";
            }
        }
    }   
}
