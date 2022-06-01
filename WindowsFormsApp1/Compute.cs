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
            //Earthworks -- END

            //Concrete Works -- START


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

                    Console.WriteLine("NICE: " + i);

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

                    Console.WriteLine("NICE: " + i);

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
                concreteGrade = cEF.parameters.conc_CM_S_CG;

                //Variables from StructMem
                double quantity, thickness, customLengthTop, customLengthBot, customLengthLeft, customLengthRight;
                string topBeamRow, topAtBeam, botBeamRow, botAtBeam, leftBeamRow, leftAtBeam, rightBeamRow, rightAtBeam;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][1], System.Globalization.CultureInfo.InvariantCulture);
                thickness = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][2], System.Globalization.CultureInfo.InvariantCulture);

                topBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][13];
                topAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][14];
                customLengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][15], System.Globalization.CultureInfo.InvariantCulture);

                botBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][16];
                botAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][17];
                customLengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][18], System.Globalization.CultureInfo.InvariantCulture);

                leftBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][19];
                leftAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][20];
                customLengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][21], System.Globalization.CultureInfo.InvariantCulture);

                rightBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][22];
                rightAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][23];
                customLengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][24], System.Globalization.CultureInfo.InvariantCulture);

                if (topBeamRow.Equals("None") || topAtBeam.Equals("None") || botBeamRow.Equals("None") || botAtBeam.Equals("None") ||
                    leftBeamRow.Equals("None") || leftAtBeam.Equals("None") || rightBeamRow.Equals("None") || rightAtBeam.Equals("None"))
                {
                    return;
                }

                if(customLengthTop <= 0)
                {
                    customLengthTop = slabCustomLengthSOG(cEF, floorCount, topBeamRow, topAtBeam);
                }
                if (customLengthBot <= 0)
                {
                    customLengthBot = slabCustomLengthSOG(cEF, floorCount, botBeamRow, botAtBeam);
                }
                if (customLengthLeft <= 0)
                {
                    customLengthLeft = slabCustomLengthSOG(cEF, floorCount, leftBeamRow, leftAtBeam);
                }
                if (customLengthRight <= 0)
                {
                    customLengthRight = slabCustomLengthSOG(cEF, floorCount, rightBeamRow, rightAtBeam);
                }

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[0])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                    }
                    else
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                        ((((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness) / 1000000000) * quantity);
                }

                refreshSolutions(cEF);
            }
            else // Suspended Slab
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_S_CG;

                //Variables from StructMem
                double quantity, thickness, customLengthTop, customLengthBot, customLengthLeft, customLengthRight;
                string slabMark, topBeamRow, topAtBeam, botBeamRow, botAtBeam, leftBeamRow, leftAtBeam, rightBeamRow, rightAtBeam;

                //Init variables from StructMem

                int i = 0;
                if (cEF.structuralMembers.slab[floorCount][slabCount][2].Equals("No. 1"))
                {
                    i++;
                }

                slabMark = cEF.structuralMembers.slab[floorCount][slabCount][1];
                quantity = 0; //TODO: dont know where kukunin
                thickness = 0;
                foreach (List<string> schedule in cEF.structuralMembers.slabSchedule[floorCount - 1])
                {
                    if (schedule[0].Equals(slabMark))
                    {
                        thickness = double.Parse(schedule[1], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                topBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][11 - i];
                topAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][12 - i];
                customLengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][13 - i], System.Globalization.CultureInfo.InvariantCulture);

                botBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][14 - i];
                botAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][15 - i];
                customLengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][16 - i], System.Globalization.CultureInfo.InvariantCulture);

                leftBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][17 - i];
                leftAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][18 - i];
                customLengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][19 - i], System.Globalization.CultureInfo.InvariantCulture);

                rightBeamRow = cEF.structuralMembers.slab[floorCount][slabCount][20 - i];
                rightAtBeam = cEF.structuralMembers.slab[floorCount][slabCount][21 - i];
                customLengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][slabCount][22 - i], System.Globalization.CultureInfo.InvariantCulture);

                if (topBeamRow.Equals("None") || topAtBeam.Equals("None") || botBeamRow.Equals("None") || botAtBeam.Equals("None") ||
                    leftBeamRow.Equals("None") || leftAtBeam.Equals("None") || rightBeamRow.Equals("None") || rightAtBeam.Equals("None") ||
                    slabMark.Equals("None"))
                {
                    return;
                }

                if (customLengthTop <= 0)
                {
                    customLengthTop = slabCustomLengthSS(cEF, floorCount, topBeamRow, topAtBeam);
                }
                if (customLengthBot <= 0)
                {
                    customLengthBot = slabCustomLengthSS(cEF, floorCount, botBeamRow, botAtBeam);
                }
                if (customLengthLeft <= 0)
                {
                    customLengthLeft = slabCustomLengthSS(cEF, floorCount, leftBeamRow, leftAtBeam);
                }
                if (customLengthRight <= 0)
                {
                    customLengthRight = slabCustomLengthSS(cEF, floorCount, rightBeamRow, rightAtBeam);
                }
                Console.WriteLine("NICE2: " + customLengthTop + " " + customLengthBot
                    + " " + customLengthLeft + " " + customLengthRight);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[0])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity);
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity);
                    }
                    else
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity);
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity);
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][slabCount].Add(
                        ((((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness) / 1000000000) * quantity);
                }

                refreshSolutions(cEF);
            }
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
                concreteGrade = cEF.parameters.conc_CM_S_CG;

                //Variables from StructMem
                double quantity, thickness, customLengthTop, customLengthBot, customLengthLeft, customLengthRight;
                string topBeamRow, topAtBeam, botBeamRow, botAtBeam, leftBeamRow, leftAtBeam, rightBeamRow, rightAtBeam;

                //Init variables from StructMem
                quantity = double.Parse(cEF.structuralMembers.slab[floorCount][index][1], System.Globalization.CultureInfo.InvariantCulture);
                thickness = double.Parse(cEF.structuralMembers.slab[floorCount][index][2], System.Globalization.CultureInfo.InvariantCulture);

                topBeamRow = cEF.structuralMembers.slab[floorCount][index][13];
                topAtBeam = cEF.structuralMembers.slab[floorCount][index][14];
                customLengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][index][15], System.Globalization.CultureInfo.InvariantCulture);

                botBeamRow = cEF.structuralMembers.slab[floorCount][index][16];
                botAtBeam = cEF.structuralMembers.slab[floorCount][index][17];
                customLengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][index][18], System.Globalization.CultureInfo.InvariantCulture);

                leftBeamRow = cEF.structuralMembers.slab[floorCount][index][19];
                leftAtBeam = cEF.structuralMembers.slab[floorCount][index][20];
                customLengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][index][21], System.Globalization.CultureInfo.InvariantCulture);

                rightBeamRow = cEF.structuralMembers.slab[floorCount][index][22];
                rightAtBeam = cEF.structuralMembers.slab[floorCount][index][23];
                customLengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][index][24], System.Globalization.CultureInfo.InvariantCulture);

                if (topBeamRow.Equals("None") || topAtBeam.Equals("None") || botBeamRow.Equals("None") || botAtBeam.Equals("None") ||
                    leftBeamRow.Equals("None") || leftAtBeam.Equals("None") || rightBeamRow.Equals("None") || rightAtBeam.Equals("None"))
                {
                    return;
                }

                if (customLengthTop <= 0)
                {
                    customLengthTop = slabCustomLengthSOG(cEF, floorCount, topBeamRow, topAtBeam);
                }
                if (customLengthBot <= 0)
                {
                    customLengthBot = slabCustomLengthSOG(cEF, floorCount, botBeamRow, botAtBeam);
                }
                if (customLengthLeft <= 0)
                {
                    customLengthLeft = slabCustomLengthSOG(cEF, floorCount, leftBeamRow, leftAtBeam);
                }
                if (customLengthRight <= 0)
                {
                    customLengthRight = slabCustomLengthSOG(cEF, floorCount, rightBeamRow, rightAtBeam);
                }

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[0])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                    }
                    else
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                        ((((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness) / 1000000000) * quantity;
                }

                refreshSolutions(cEF);
            }
            else // Suspended Slab
            {
                //Variables from Parameters
                string concreteGrade;

                //Init variables from Parameters
                concreteGrade = cEF.parameters.conc_CM_S_CG;

                //Variables from StructMem
                double quantity, thickness, customLengthTop, customLengthBot, customLengthLeft, customLengthRight;
                string slabMark, topBeamRow, topAtBeam, botBeamRow, botAtBeam, leftBeamRow, leftAtBeam, rightBeamRow, rightAtBeam;

                //Init variables from StructMem

                int i = 0;
                if (cEF.structuralMembers.slab[floorCount][index][2].Equals("No. 1"))
                {
                    i++;
                }

                slabMark = cEF.structuralMembers.slab[floorCount][index][1];
                quantity = 0; //TODO: dont know where kukunin
                thickness = 0;
                foreach (List<string> schedule in cEF.structuralMembers.slabSchedule[floorCount - 1])
                {
                    if (schedule[0].Equals(slabMark))
                    {
                        thickness = double.Parse(schedule[1], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                topBeamRow = cEF.structuralMembers.slab[floorCount][index][11 - i];
                topAtBeam = cEF.structuralMembers.slab[floorCount][index][12 - i];
                customLengthTop = double.Parse(cEF.structuralMembers.slab[floorCount][index][13 - i], System.Globalization.CultureInfo.InvariantCulture);

                botBeamRow = cEF.structuralMembers.slab[floorCount][index][14 - i];
                botAtBeam = cEF.structuralMembers.slab[floorCount][index][15 - i];
                customLengthBot = double.Parse(cEF.structuralMembers.slab[floorCount][index][16 - i], System.Globalization.CultureInfo.InvariantCulture);

                leftBeamRow = cEF.structuralMembers.slab[floorCount][index][17 - i];
                leftAtBeam = cEF.structuralMembers.slab[floorCount][index][18 - i];
                customLengthLeft = double.Parse(cEF.structuralMembers.slab[floorCount][index][19 - i], System.Globalization.CultureInfo.InvariantCulture);

                rightBeamRow = cEF.structuralMembers.slab[floorCount][index][20 - i];
                rightAtBeam = cEF.structuralMembers.slab[floorCount][index][21 - i];
                customLengthRight = double.Parse(cEF.structuralMembers.slab[floorCount][index][22 - i], System.Globalization.CultureInfo.InvariantCulture);

                if (topBeamRow.Equals("None") || topAtBeam.Equals("None") || botBeamRow.Equals("None") || botAtBeam.Equals("None") ||
                    leftBeamRow.Equals("None") || leftAtBeam.Equals("None") || rightBeamRow.Equals("None") || rightAtBeam.Equals("None") ||
                    slabMark.Equals("None"))
                {
                    return;
                }

                if (customLengthTop <= 0)
                {
                    customLengthTop = slabCustomLengthSS(cEF, floorCount, topBeamRow, topAtBeam);
                }
                if (customLengthBot <= 0)
                {
                    customLengthBot = slabCustomLengthSS(cEF, floorCount, botBeamRow, botAtBeam);
                }
                if (customLengthLeft <= 0)
                {
                    customLengthLeft = slabCustomLengthSS(cEF, floorCount, leftBeamRow, leftAtBeam);
                }
                if (customLengthRight <= 0)
                {
                    customLengthRight = slabCustomLengthSS(cEF, floorCount, rightBeamRow, rightAtBeam);
                }
                Console.WriteLine("NICE2: " + customLengthTop + " " + customLengthBot
                    + " " + customLengthLeft + " " + customLengthRight);

                //Computation -- Concrete Works
                if (cEF.parameters.conc_cmIsSelected[0])
                {
                    if (concreteGrade.Equals("CLASS AA"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[0][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2] * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS A"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[1][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2] * quantity;
                    }
                    else if (concreteGrade.Equals("CLASS B"))
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[2][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2] * quantity;
                    }
                    else
                    {
                        double volume = ((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[3][0] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1] * quantity;
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2] * quantity;
                    }
                }
                else
                {
                    cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][0] = 
                        ((((customLengthTop + customLengthBot) / 2) * ((customLengthLeft + customLengthRight) / 2) * thickness) / 1000000000) * quantity;
                }

                refreshSolutions(cEF);
            }
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
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
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
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
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
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
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
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
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
                    double volumeSteps = (riser * tread) / 2 * SL * steps;
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
                if (cEF.parameters.conc_cmIsSelected[4])
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
                            volume * cEF.structuralMembers.concreteProportion[0][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2]);
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
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2]);
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
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2]);
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
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2]);
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
                if (cEF.parameters.conc_cmIsSelected[4])
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
                            volume * cEF.structuralMembers.concreteProportion[0][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[0][2]);
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
                            volume * cEF.structuralMembers.concreteProportion[1][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[1][2]);
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
                            volume * cEF.structuralMembers.concreteProportion[2][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[2][2]);
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
                            volume * cEF.structuralMembers.concreteProportion[3][0]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][1]);
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][stairsCount].Add(
                            volume * cEF.structuralMembers.concreteProportion[3][2]);
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
                        volume);
                }
            }

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
                        double volumeSteps = (riser * tread) / 2 * SL * steps;
                        double volume = volumeWSF + volumeSteps;
                        volume /= 1000000000;
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][0] = 
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2];
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
                            volume * cEF.structuralMembers.concreteProportion[1][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2];
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
                            volume * cEF.structuralMembers.concreteProportion[2][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2];
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
                            volume * cEF.structuralMembers.concreteProportion[3][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2];
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
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] = 
                        volume;
                }
            }
            else if (cEF.structuralMembers.stairs[floorCount][index][0].Equals("L - Stairs"))
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
                if (cEF.parameters.conc_cmIsSelected[4])
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
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2];
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
                            volume * cEF.structuralMembers.concreteProportion[1][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2];
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
                            volume * cEF.structuralMembers.concreteProportion[2][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2];
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
                            volume * cEF.structuralMembers.concreteProportion[3][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2];
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
                        volume;
                }
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
                if (cEF.parameters.conc_cmIsSelected[4])
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
                            volume * cEF.structuralMembers.concreteProportion[0][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[0][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[0][2];
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
                            volume * cEF.structuralMembers.concreteProportion[1][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[1][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[1][2];
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
                            volume * cEF.structuralMembers.concreteProportion[2][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[2][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[2][2];
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
                            volume * cEF.structuralMembers.concreteProportion[3][0];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][1] = 
                            volume * cEF.structuralMembers.concreteProportion[3][1];
                        cEF.structuralMembers.concreteWorkSolutionsST[floorCount][index][2] = 
                            volume * cEF.structuralMembers.concreteProportion[3][2];
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
                        volume;
                }
            }

            refreshSolutions(cEF);
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
