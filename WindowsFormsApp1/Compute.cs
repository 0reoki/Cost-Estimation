using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Compute 
    {
        //General Functions -- START
        private void refreshSolutions(CostEstimationForm cEF)
        {
            //Footing -- START
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
            double factorOfSafety = (cEF.gravelBedding_Total + toAddtoGB) * 0.05;
            cEF.gravelBedding_Total = cEF.gravelBedding_Total + toAddtoGB + factorOfSafety;
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
                // kailangan bumili lol
                // Do something
            }
            //Footing -- END

            //Stairs -- START


            //Stairs -- END
        }
        //General Functions -- END

        //Footing Computation Functions -- START
        public void AddFootingWorks(CostEstimationForm cEF, int footingCount, int wallFootingCount, bool isFooting)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.earthworkSolutions.Add(newList);
            List<double> newList2 = new List<double>();
            cEF.structuralMembers.concreteWorkSolutionsF.Add(newList2);
            if (isFooting)
            {
                cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(1);
                cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(1);
            }
            else
            {
                cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(2);
                cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(2);
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
            if (isFooting)
            {
                if (cEF.structuralMembers.footingsColumn[0][footingCount][0].Equals("Isolated Footing"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((length * width * thickness) / 1000000000) * quantity);
                    }

                    refreshSolutions(cEF);
                }
                else //Combined Footing
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((length * width * thickness) / 1000000000) * quantity);
                    }

                    refreshSolutions(cEF);
                }
            }
            else // Wall Footing
            {
                if (cEF.structuralMembers.footingsWall[0][wallFootingCount][0].Equals("Rectangular"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((length * wfBase * thickness) / 1000000000) * quantity);
                    }

                    refreshSolutions(cEF);
                }
                else //Trapezoidal
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS A"))
                        {
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                        }
                        else if (concreteGrade.Equals("CLASS B"))
                        {
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                        }
                        else
                        {
                            double volume = length * ((wfBaseT + wfBaseU) / 2) * thickness;
                            volume /= 1000000000;
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                            cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                                volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[footingCount + wallFootingCount].Add(
                            ((length * ((wfBaseT + wfBaseU) / 2) * thickness) / 1000000000) * quantity);
                    }

                    refreshSolutions(cEF);
                }
            }
        }

        //Modify
        public void modifyFootingWorks(CostEstimationForm cEF, int structMemCount, int count, bool isFooting)
        {
            if (isFooting)
            {
                if (cEF.structuralMembers.footingsColumn[0][structMemCount][0].Equals("Isolated Footing"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                    int i = count;

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
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * width * thickness) / 1000000000) * quantity;
                    }

                    refreshSolutions(cEF);
                }
                else //Combined Footing
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                    int i = count;

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
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * width * thickness) / 1000000000) * quantity;
                    }

                    refreshSolutions(cEF);
                }
            }
            else // Wall Footing
            {
                if (cEF.structuralMembers.footingsWall[0][structMemCount][0].Equals("Rectangular"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                    int i = count;

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
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * wfBase * thickness) / 1000000000) * quantity;
                    }

                    refreshSolutions(cEF);
                }
                else //Trapezoidal
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;
                    string concreteGrade;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    concreteGrade = cEF.parameters.conc_CM_F_CG;

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
                    int i = count;

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
                        }
                    }
                    else
                    {
                        cEF.structuralMembers.concreteWorkSolutionsF[i][1] =
                            ((length * ((wfBaseT + wfBaseU) / 2) * thickness) / 1000000000) * quantity;
                    }

                    refreshSolutions(cEF);
                }
            }
        }
        //Footing Computation Functions  -- END

        //Column Computation Functions -- STAR
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
            string concreteGrade;

            //Init variables from Parameters
            concreteGrade = cEF.parameters.conc_CM_C_CG;

            //Variables from StructMem
            double baseC, depth, height, quantity;

            //Init variables from StructMem
            baseC = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][1], System.Globalization.CultureInfo.InvariantCulture);
            depth = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][2], System.Globalization.CultureInfo.InvariantCulture);
            height = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][3], System.Globalization.CultureInfo.InvariantCulture);
            quantity = double.Parse(cEF.structuralMembers.column[floorCount][columnCount][4], System.Globalization.CultureInfo.InvariantCulture);

            //Computation -- Concrete Works
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
                }
            }
            else
            {
                cEF.structuralMembers.concreteWorkSolutionsC[floorCount][columnCount].Add(
                    ((baseC * depth * height) / 1000000000) * quantity);
            }

            refreshSolutions(cEF);


            /* Will use if calculation is different from ground and upper floor
            int i = count;
            if (cEF.structuralMembers.column[floorCount][columnCount][0].Equals("Ground"))
            {

            }
            else
            {

            }
            */
        }

        //Modify
        public void ModifyColumnWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            //Variables from Parameters
            string concreteGrade;

            //Init variables from Parameters
            concreteGrade = cEF.parameters.conc_CM_C_CG;

            //Variables from StructMem
            double baseC, depth, height, quantity;

            //Init variables from StructMem
            baseC = double.Parse(cEF.structuralMembers.column[floorCount][index][1], System.Globalization.CultureInfo.InvariantCulture);
            depth = double.Parse(cEF.structuralMembers.column[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);
            height = double.Parse(cEF.structuralMembers.column[floorCount][index][3], System.Globalization.CultureInfo.InvariantCulture);
            quantity = double.Parse(cEF.structuralMembers.column[floorCount][index][4], System.Globalization.CultureInfo.InvariantCulture);

            //Computation -- Concrete Works
            //Computation -- Concrete Works
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
                }
            }
            else
            {
                cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                    ((baseC * depth * height) / 1000000000) * quantity;
            }

            refreshSolutions(cEF);


            /* Will use if calculation is different from ground and upper floor
            int i = count;
            if (cEF.structuralMembers.column[floorCount][columnCount][0].Equals("Ground"))
            {

            }
            else
            {

            }
            */
        }
        //Column Computation Functions -- END

        //Beam Computation Functions -- STARt
        public void AddBeamWorks(CostEstimationForm cEF, int floorCount, int beamCount, string beamType)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.concreteWorkSolutionsBR[floorCount].Add(newList);
            if(!cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Any())
            {
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Add(0);
                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount].Add(0);
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
            double baseB, depth, length, quantity;

            //Init variables from StructMem
            foreach(List<string> beamRow in cEF.structuralMembers.beamRow[floorCount][beamCount])
            {
                foreach(List<string> schedule in cEF.structuralMembers.beamSchedule[floorCount])
                {
                    if (beamType.Equals(schedule[0]))
                    {
                        if (beamRow[0].Equals(schedule[1]))
                        {
                            //Init variables from Beam Row and Beam Schedule
                            baseB = double.Parse(schedule[2], System.Globalization.CultureInfo.InvariantCulture);
                            depth = double.Parse(schedule[3], System.Globalization.CultureInfo.InvariantCulture);
                            length = double.Parse(beamRow[1], System.Globalization.CultureInfo.InvariantCulture);
                            quantity = double.Parse(beamRow[2], System.Globalization.CultureInfo.InvariantCulture);

                            //Computation -- Concrete Works
                            if (cEF.parameters.conc_cmIsSelected[2])
                            {
                                if (concreteGrade.Equals("CLASS AA"))
                                {
                                    double volume = baseB * depth * length * quantity; ;
                                    volume /= 1000000000;
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][0];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][1] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][1];
                                    cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][2] +=
                                        volume * cEF.structuralMembers.concreteProportion[0][2];
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
                                }
                            }
                            else
                            {
                                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][beamCount][0] +=
                                    ((baseB * depth * length) / 1000000000) * quantity;
                            }

                            refreshSolutions(cEF);
                        }
                    }
                }
            }
        }

        //Modify
        public void ModifyBeamWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            //Variables from Parameters
            string concreteGrade;

            //Init variables from Parameters
            concreteGrade = cEF.parameters.conc_CM_B_CG;

            //Variables from StructMem
            double baseB, depth, length, quantity;

            //Init variables from StructMem
            cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] = 0;
            cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][1] = 0;
            cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][2] = 0;
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
                            length = double.Parse(beamRow[1], System.Globalization.CultureInfo.InvariantCulture);
                            quantity = double.Parse(beamRow[2], System.Globalization.CultureInfo.InvariantCulture);

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
                                }
                            }
                            else
                            {
                                cEF.structuralMembers.concreteWorkSolutionsBR[floorCount][index][0] +=
                                    ((baseB * depth * length) / 1000000000) * quantity;
                            }

                            refreshSolutions(cEF);
                        }
                    }
                }
            }
        }
        //Beam Computation Functions -- END

        //Slab Computation Functions -- START
        public void AddSlabWorks(CostEstimationForm cEF, int floorCount, int slabCount)
        {
            List<double> newList = new List<double>();
            //cEF.structuralMembers.slabSolutions.Add(newList);
            slabWorks(cEF, floorCount, slabCount);
        }

        public void slabWorks(CostEstimationForm cEF, int floorCount, int slabCount)
        {
            double length;
            length = double.Parse(cEF.structuralMembers.stairs[floorCount][slabCount][1], System.Globalization.CultureInfo.InvariantCulture);
        }

        public void ModifySlabWorks(CostEstimationForm cEF, int floorCount, int index)
        {

        }
        //Slab Computation Functions -- END

        //Stairs Computation Functions -- START
        public void AddStairsWorks(CostEstimationForm cEF, int floorCount, int stairsCount)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.concreteWorkSolutionsST[floorCount].Add(newList);
            stairsWorks(cEF, floorCount, stairsCount);
        }

        public void stairsWorks(CostEstimationForm cEF, int floorCount, int stairsCount)
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
                if (cEF.parameters.conc_cmIsSelected[4])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2]);
                    }
                    
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2]);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2]);
                    }
                    else
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2]);
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
                    double volumeSteps = (riser * tread) / 2 * steps;
                    double volume = volumeWSF + volumeSteps;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                        volume);
                }
            }
            else if (cEF.structuralMembers.stairs[floorCount][stairsCount][0].Equals("L - Stairs"))
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
                if (cEF.parameters.conc_cmIsSelected[4])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2]);
                    }

                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2]);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2]);
                    }
                    else
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2]);
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
                    double volumeSteps = (riser * tread) / 2 * steps;
                    double volume = volumeWSF + volumeSteps;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                        volume);
                }
            }
            else
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
                if (cEF.parameters.conc_cmIsSelected[4])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2]);
                    }

                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2]);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2]);
                    }
                    else
                    {
                        double WSL = Math.Ceiling(Math.Sqrt(Math.Pow((tread * steps), 2) + Math.Pow((riser * steps), 2)));
                        while (WSL % 25 != 0)
                        {
                            WSL++;
                        }
                        double volumeWSF = SL * WSL * Thc;
                        double volumeSteps = (riser * tread) / 2 * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2]);
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
                    double volumeSteps = (riser * tread) / 2 * steps;
                    double volume = volumeWSF + volumeSteps;
                    volume /= 1000000000;
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                        volume);
                }
            }

            refreshSolutions(cEF);
        }

        public void ModifyStairsWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            
        }
        //Stairs Computation Functions -- END

        //Roof Computation Functions -- START
        public void AddRoofWorks(CostEstimationForm cEF, int floorCount, int roofCount)
        {
            List<double> newList = new List<double>();
            //cEF.structuralMembers.roofSolutions.Add(newList);
            roofWorks(cEF, floorCount, roofCount);
        }

        public void roofWorks(CostEstimationForm cEF, int floorCount, int roofCount)
        {
            double length;
            length = double.Parse(cEF.structuralMembers.stairs[floorCount][roofCount][1], System.Globalization.CultureInfo.InvariantCulture);
        }

        public void ModifyRoofWorks(CostEstimationForm cEF, int floorCount, int index)
        {

        }
        //Roof Computation Functions -- END
    }
}
