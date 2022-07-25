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
            cEF.structuralMembers.per_col.Add(0);
            cEF.structuralMembers.per_wal.Add(0);
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
                    //Computation -- Formworks **                                     
                    cEF.structuralMembers.per_col[footingCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;                    
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
                    //Computation -- Formworks **                   
                    cEF.structuralMembers.per_col[footingCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;                    
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
                    //Computation -- Formworks **                    
                    double c = Math.Sqrt(Math.Pow((((wfBaseT / 1000) - (wfBaseU / 1000)) / 2), 2) + Math.Pow((thickness / 1000), 2));                    
                    cEF.structuralMembers.per_wal[wallFootingCount] = ((((c * (length / 1000)) + 0.2) + ((((wfBaseT / 1000) + (wfBaseU / 1000)) / 2) * (thickness / 1000))) * 2) * quantity;                                        
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
                    //Computation -- Formworks **                                                            
                    cEF.structuralMembers.per_col[structMemCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;                    
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
                    //Computation -- Formworks **
                    cEF.structuralMembers.per_col[structMemCount] = (2 * ((length / 1000) + (width / 1000)) + 0.2) * (thickness / 1000) * quantity;                    
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
                    //Computation -- Formworks **                    
                    double c = Math.Sqrt(Math.Pow((((wfBaseT / 1000) - (wfBaseU / 1000)) / 2), 2) + Math.Pow((thickness / 1000), 2));                    
                    cEF.structuralMembers.per_wal[structMemCount] = ((((c * (length / 1000)) + 0.2) + ((((wfBaseT / 1000) + (wfBaseU / 1000)) / 2) * (thickness / 1000))) * 2) * quantity;                    
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
                refreshSolutions(cEF);
            }
            catch
            {
                print("Null catcher COLUMN");
            }
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


                ////////////
                /*for (int n = 0; n < cEF.structuralMembers.beamRow[i][j].Count; n++)
                {
                    double qty = double.Parse(cEF.structuralMembers.beamRow[i][j][n][1]);
                    List<string> mainrein_holder_2 = new List<string>();
                    List<string> lbs = new List<string>();
                    for (int sched = 0; sched < main_rein[i].Count; sched++)
                    {
                        if (main_rein[i][sched][1] == cEF.structuralMembers.beamRow[i][j][n][0])
                        {
                            mainrein_holder_2 = main_rein[i][sched];
                            lbs = main_rein[i][sched];
                            break;
                        }
                    }
                    List<string> mainrein_holder_3 = new List<string>();
                    //mainrein_holder_3.Add(cEF.structuralMembers.beamRow[i][j][n][3]);  //////////                          
                    if (mainrein_holder_2[0] == "Footing Tie Beam" || mainrein_holder_2[0] == "Grade Beam")
                    {
                        ccs = cEF.parameters.conc_CC_BEE;
                    }
                    else
                    {
                        ccs = cEF.parameters.conc_CC_BEW;
                    }
                    //mainrein_holder_3.Add(ccs); ////////////
                    if (cEF.structuralMembers.beam[i][j][5] == "Mechanical" || cEF.structuralMembers.beam[i][j][5] == "Welded Splice (Butt)")
                    {
                        sl_top = 0;
                        sl_bot = 0;
                    }
                    else
                    {
                        string mix = spliceMixGetter(cEF.parameters.conc_CM_B_RM);
                        int index = cEF.parameters.rein_LSL_TB_fc_list.IndexOf(mix);
                        bool top_fnd = false;
                        bool bot_fnd = false;

                        if (index >= 0)
                        {
                            for (int r = 0; r < cEF.parameters.rein_LSL_TB_dt.Rows.Count; r++)
                            {
                                if (top_fnd && bot_fnd)
                                {
                                    break;
                                }
                                if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][5])
                                {
                                    sl_top = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                    top_fnd = true;
                                }
                                if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][6])
                                {
                                    sl_bot = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                    bot_fnd = true;
                                }
                            }
                        }
                        else
                        {
                            sl_top = 0;
                            sl_bot = 0;
                        }
                    }
                    bool hook_topFND = false;
                    bool hook_botFND = false;
                    if (hook_index > 0)
                    {
                        for (int r = 0; r < cEF.parameters.rein_BEH_MB_dt.Rows.Count; r++)
                        {
                            if (hook_topFND && hook_botFND)
                            {
                                break;
                            }
                            if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][5])
                            {
                                hook_top = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][hook_index].ToString());
                                hook_topFND = true;
                            }
                            if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][6])
                            {
                                hook_bot = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][hook_index].ToString());
                                hook_botFND = true;
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
                    if (total_quantity != 1)
                    {
                        if (cEF.structuralMembers.beamRow[i][j][n][4] == "1-End Support")
                        {
                            lb_top = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top + (0.5 * sl_top) - double.Parse(ccs);
                            lb_bot = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_bot + (0.5 * sl_bot) - double.Parse(ccs);
                        }
                        else
                        {
                            if (quantity_tracker == 0)
                            {
                                lb_top = (1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_top + (0.5 * sl_top) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs);
                                lb_bot = (1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_bot + (0.5 * sl_bot) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs);
                            }
                            else
                            {
                                lb_top = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + sl_top;
                                lb_bot = double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + sl_bot;
                            }
                        }
                    }
                    else
                    {
                        if (cEF.structuralMembers.beamRow[i][j][n][4] == "1-End Support")
                        {
                            lb_top = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                            lb_bot = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_bot - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                        }
                        else
                        {
                            lb_top = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                            lb_bot = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_bot - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                        }
                    }

                    *//*mainrein_holder_3.Add(sl_top.ToString());/////////////
                    mainrein_holder_3.Add(sl_bot.ToString());//////////////*//*
                    mainrein_holder_3.Add(lb_top.ToString());
                    mainrein_holder_3.Add(lb_bot.ToString());
                    var concatted2ndLayer = mainrein_holder_2.Concat(mainrein_holder_3);
                    mainrein_holder_2 = concatted2ndLayer.ToList();
                    for (int reps = 0; reps < qty; reps++)
                    {
                        quantity_tracker++;
                        if (cEF.structuralMembers.beamRow[i][j][n][4] == "2-End Supports" && quantity_tracker == total_quantity)
                        {
                            print(reps.ToString());
                            print(quantity_tracker + " - " + "PASOOOOOOOOOOK");
                            List<string> lbs_handler = new List<string>();
                            lbs_handler.Add(((1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_top + (0.5 * sl_top) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs)).ToString());
                            lbs_handler.Add(((1.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_bot + (0.5 * sl_bot) - (0.5 * double.Parse(cEF.structuralMembers.beamRow[i][j][n][3])) - double.Parse(ccs)).ToString());
                            var concatLBS = lbs.Concat(lbs_handler);
                            lbs = concatLBS.ToList();
                            mainrein_holder_1.Add(mainrein_holder_2);
                        }
                        mainrein_holder_1.Add(mainrein_holder_2);
                    }
                }
            }
                    eachBeamRow.Add(mainrein_holder_1); // save this
        }
        double refe = 1;
                foreach (List<List<string>> a in eachBeamRow)// type - name - mtop - mbot - etopA - etopB - ebott - Lc - ccs -sltop - slbot
                {
                    foreach (var b in a)*/
                //////////

                /*//Beam MAIN reinforcements                
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
                        double cont_bott = (double.Parse(cEF.structuralMembers.beamSchedule[i][j][9]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][10]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][17]) + double.Parse(cEF.structuralMembers.beamSchedule[i][j][18]))/2;
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
                        if(hook_type == "90")
                        {
                            hook_index = 1;
                        }
                        else if(hook_type == "135")
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
                                if (main_rein[i][sched][1] == cEF.structuralMembers.beamRow[i][j][n][0])
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
                                    int index = cEF.parameters.rein_LSL_TB_fc_list.IndexOf(mix);
                                    bool top_fnd = false;
                                    bool bot_fnd = false;

                                    if (index >= 0)
                                    {
                                        for (int r = 0; r < cEF.parameters.rein_LSL_TB_dt.Rows.Count; r++)
                                        {
                                            if (top_fnd && bot_fnd)
                                            {
                                                break;
                                            }
                                            if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][5])
                                            {
                                                sl_top = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                                top_fnd = true;
                                            }
                                            if ((cEF.parameters.rein_LSL_TB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][6])
                                            {
                                                sl_bot = double.Parse(cEF.parameters.rein_LSL_TB_dt.Rows[r][index + 1].ToString());
                                                bot_fnd = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sl_top = 0;
                                        sl_bot = 0;
                                    }
                                }
                                bool hook_topFND = false;
                                bool hook_botFND = false;
                                if (hook_index > 0)
                                {
                                    for (int r = 0; r < cEF.parameters.rein_BEH_MB_dt.Rows.Count; r++)
                                    {
                                        if (hook_topFND && hook_botFND)
                                        {
                                            break;
                                        }
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][5])
                                        {
                                            hook_top = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][hook_index].ToString());
                                            hook_topFND = true;
                                        }
                                        if ((cEF.parameters.rein_BEH_MB_dt.Rows[r][0]).ToString() == cEF.structuralMembers.beamSchedule[i][j][6])
                                        {
                                            hook_bot = double.Parse(cEF.parameters.rein_BEH_MB_dt.Rows[r][hook_index].ToString());
                                            hook_botFND = true;
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
                                double TR = reinGetter(cEF.structuralMembers.beam[0][0][6]);
                                double BR = reinGetter(cEF.structuralMembers.beam[0][0][7]);
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
                                        lb_top = 2*(double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_top - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_bot = 2 * (double.Parse(cEF.structuralMembers.beamRow[i][j][n][2]) + hook_bot - double.Parse(ccs)) - double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]);
                                        lb_top_extraA = 0;
                                        lb_top_extraB = (2* double.Parse(cEF.structuralMembers.beamRow[i][j][n][2])) + hook_top + (TR - 2)* double.Parse(cEF.structuralMembers.beamRow[i][j][n][3]) - double.Parse(ccs);
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
                
                double refe = 1;
                foreach (List<List<string>> a in eachBeamRow)// type - name - mtop - mbot - etopA - etopB - ebott - Lc - ccs -sltop - slbot
                {
                    foreach (var b in a)
                    {
                        print("-- " + refe + " --");
                        print(b[0] + " beam type");
                        print(b[1] + " beam name");
                        print(b[2] + " mtop");
                        print(b[3] + " mbott");
                        print(b[4] + " etopA");
                        print(b[5] + " etopB");
                        print(b[6] + " ebott");
                        print(b[7] + " lbTop");
                        print(b[8] + " lbBot");
                        print(b[9] + " extraTopA");
                        print(b[10] + " extraTopA");
                        print(b[11] + " extraBotA");
                        refe++;
                    }
                }*/
                refreshSolutions(cEF);
            }
            catch (Exception ex)
            {
                print(ex.ToString());
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
                            print(((top * left) * quantity).ToString());
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
                refreshSolutions(cEF);
            }
            catch
            {
                print("Null catcher slabs");
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

        //FormworksWORK -- START


        //FormworksWORK -- END



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

        public double stairsRounder(double x)
        {
            x = x * 1000;
            print(x + " lol");
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
            foreach (char a in str)
            {
                if (a.Equals('/'))
                {
                    two.Add(double.Parse(num));
                    num = "";
                }
                else if(Char.IsDigit(a))
                {
                    num += a;
                }
            }
            two.Add(double.Parse(num));
            double rein = two[0] / two[1];
            return rein;
        }
    }   
}
