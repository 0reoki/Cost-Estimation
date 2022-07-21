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

                    refreshSolutions(cEF);
                }
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
                foreach(List<string> schedule in cEF.structuralMembers.beamSchedule[floorCount])
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

                refreshSolutions(cEF);
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
                refreshSolutions(cEF);
            }
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
                        cEF.structuralMembers.concreteWorkSolutionsSL[floorCount][index][3] = volume  * quantity;
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

                refreshSolutions(cEF);
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
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][stairsCount].Add(
                        volume * quantity);
                }
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
                    cEF.structuralMembers.concreteWorkSolutionsC[floorCount][index][0] =
                        volume * quantity;
                }
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
            }

            refreshSolutions(cEF);
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
            print(cEF.structuralMembers.roofSolutions.Count + " :FLOOR" );
            roofWorks(cEF, floorCount, roofCount);
        }

        public void roofWorks(CostEstimationForm cEF, int floorCount, int roofCount)
        {
            List<double> outputs = new List<double>();
            foreach(var c in cEF.structuralMembers.roof[floorCount][roofCount])
            {
                print(c + " ---");
            }
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
                    print(totB + " TOTAL");
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
                    print(sixLenR + " 6m Com Raft");
                    print(sixLenP + " 6m Com Purl");
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
                    print(sixLenR + " 6m Com Raft");
                    print(sixLenP + " 6m Com Purl");
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
                //if (roofs.Contains("Corrugated G.I Sheet"))
                //{
                corrSheets = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][1]) / double.Parse(cEF.structuralMembers.roof[floorCount][roofCount][2])) * 2;
                //}
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
                outputs.Add(rounder(corrSheets));
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
            int x = 1;
            foreach (var c in cEF.structuralMembers.roofSolutions)
            {
                print("FLOOR: " + x);
                foreach (var k in c)
                {
                    foreach (var o in k)
                    {
                        print(o + "");
                    }
                    print("--other roof same floor--");
                }
                x++;
            }
        }

        public void ModifyRoofWorks(CostEstimationForm cEF, int floorCount, int index)
        {
            List<double> outputs = new List<double>();
            foreach (var c in cEF.structuralMembers.roof[floorCount][index])
            {
                print(c + " ---");
            }
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
                    print(totB + " TOTAL");
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
                    print(sixLenR + " 6m Com Raft");
                    print(sixLenP + " 6m Com Purl");
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
                    print(sixLenR + " 6m Com Raft");
                    print(sixLenP + " 6m Com Purl");
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
                //if (roofs.Contains("Corrugated G.I Sheet"))
                //{
                corrSheets = rounder(double.Parse(cEF.structuralMembers.roof[floorCount][index][1]) / double.Parse(cEF.structuralMembers.roof[floorCount][index][2])) * 2;
                //}
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
                outputs.Add(rounder(corrSheets));
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
            int x = 1;
            foreach (var c in cEF.structuralMembers.roofSolutions)
            {
                print("FLOOR: " + x);
                foreach (var k in c)
                {
                    foreach (var o in k)
                    {
                        print(o + "");
                    }
                    print("--other roof same floor--");
                }
                x++;
            }
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
                eWindow_total += eOne * eTwo;
            }

            for (int x = 0; x < eDoor.Count; x++)
            {
                double eOne = double.Parse(eDoor[x][0]);
                double eTwo = double.Parse(eDoor[x][1]);
                eDoor_total += eOne * eTwo;
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
                iWindow_total += eOne * eTwo;
            }

            for (int x = 0; x < iDoor.Count; x++)
            {
                double eOne = double.Parse(iDoor[x][0]);
                double eTwo = double.Parse(iDoor[x][1]);
                iDoor_total += eOne * eTwo;
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

        public List<double> computeCHB_reinforcement(double eCHB, double iCHB, string vSpace, string hSpace, string grade, string diam, string rLen, string tWire)
        {
            List<double> mason = new List<double>();
            double[] vertical = { 2.93, 2.13, 1.60 };
            double[] horizontal = { 3.30, 2.15, 1.72 };
            double[] diameter = { 0.616, 0.888, 1.578, 2.466, 3.853 };
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
                tieWire = rounder(chbS[x] * arrayHandler[tieIndexer, hIndexer]);
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

        //roofings -- START
        public void addRoofings(List<string> roofs, params string [] val)
        {
            foreach(var c in roofs)
            {
                print(c+" -");
            }
            if (roofs[0] == "Rafter and Purlins")
            {
                if (roofs[1] == "Wood")
                {
                    //kind - wodsandshts -// 2 Lraft - Lpurl - spaceR - spaceP
                    double raft = rounder(((double.Parse(roofs[3]) / double.Parse(roofs[4])) + 1))*2;
                    double lenRaft = rounder(double.Parse(roofs[2]) * 3.28);
                    double purl = (rounder((double.Parse(roofs[2]) / double.Parse(roofs[5])) + 1))*2;
                    double lenPurl = rounder(double.Parse(roofs[3]) * 3.28);
                    double rafB = rounder(raft * ((2 * 6 * (lenRaft)) / 12));
                    double purB = rounder(purl * ((2 * 2 * (lenPurl)) / 12));
                    double totB = rafB + purB;
                    print(totB + " TOTAL");
                }
                else if (roofs[1] == "Steel - Tubular")
                {
                    //2 Lraft(SW) - Lraft - Lpurl - spaceR - spaceP - CommR - CommP
                    double raft = rounder(((double.Parse(roofs[2]) / double.Parse(roofs[5]))+1)*2);
                    double lenRaft = rounder((raft) * (double.Parse(roofs[3])));
                    double sixLenR = rounder(lenRaft/6);
                    double purl = rounder(((double.Parse(roofs[3]) / double.Parse(roofs[6]))+1)*2);
                    double lenPurl = rounder(purl * double.Parse(roofs[4]));
                    double sixLenP = rounder(lenPurl/6);
                    print(sixLenR+" 6m Com Raft");
                    print(sixLenP + " 6m Com Purl");
                }
                else
                {
                    double raft = rounder(((double.Parse(roofs[2]) / double.Parse(roofs[5])) + 1) * 2);
                    double lenRaft = rounder((raft) * (double.Parse(roofs[3])));
                    double sixLenR = rounder(lenRaft / 6);
                    double purl = rounder(((double.Parse(roofs[3]) / double.Parse(roofs[6])) + 1) * 2);
                    double lenPurl = rounder(purl * double.Parse(roofs[4]));
                    double sixLenP = rounder(lenPurl / 6);
                    print(sixLenR + " 6m Com Raft");
                    print(sixLenP + " 6m Com Purl");
                }
            }
            else if (roofs[0] == "G.I Roof and Its Accessories")
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
                //if (roofs.Contains("Corrugated G.I Sheet"))
                //{
                    corrSheets = rounder(double.Parse(roofs[1]) / double.Parse(roofs[2]))*2;          
                //}
                if (roofs.Contains("G.I Roof Nails"))
                {
                    foreach (string str in val)
                    {
                        giNails += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 120;
                    }
                }
                if (roofs.Contains("G.I Rivets"))
                {
                    foreach (string str in val)
                    {
                        rivets += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 180;
                    }
                }
                if (roofs.Contains("G.I Washers"))
                {
                    foreach (string str in val)
                    {
                        giWashers += (corrSheets * double.Parse(table2[double.Parse(str).ToString("F")].ToString())) / 126;
                    }
                }
                if (roofs.Contains("Lead Washers"))
                {
                    foreach (string str in val)
                    {
                        leadWashers += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 75;
                    }
                }
                if (roofs.Contains("Umbrella Nails"))
                {
                    foreach (string str in val)
                    {
                        umbNails += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString())) / 120;
                    }
                }
                if (roofs.Contains("Plain G.I Strap"))
                {
                    foreach (string str in val)
                    {
                        plain_sheets += (corrSheets * double.Parse(table1[double.Parse(str).ToString("F")].ToString()));
                    }
                    plain_sheets = plain_sheets / double.Parse(table3[roofs[roofs.Count-1]].ToString());
                }
                materials.Add(rounder(corrSheets));
                materials.Add(rounder(giNails));
                materials.Add(rounder(rivets)); 
                materials.Add(rounder(giWashers));
                materials.Add(rounder(leadWashers));
                materials.Add(rounder(umbNails));
                materials.Add(rounder(plain_sheets));
            }
            else
            {
                print("sheesh");
            }
        }

        //roofings -- END
       
        

        //Helper functions --- START
        public void print(string str)
        {
            Console.WriteLine(str);
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
    }   
}
