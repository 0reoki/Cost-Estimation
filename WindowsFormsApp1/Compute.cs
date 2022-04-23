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
            cEF.excavation_Total += totalCut;

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

        //Earthworks functions -- START
        public void AddFootingWorks(CostEstimationForm cEF, int footingCount, int wallFootingCount, bool isFooting)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.earthworkSolutions.Add(newList);
            if (isFooting)
            {
                cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(1);
            }
            else
            {
                cEF.structuralMembers.earthworkSolutions[footingCount + wallFootingCount].Add(2);
            }
            footingWorks(cEF, footingCount, wallFootingCount, isFooting);
        }

        public void ModifyFootingWorks(CostEstimationForm cEF, int structMemCount, bool isFooting)
        {
            footingWorks(cEF, structMemCount, isFooting);
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

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //Computation

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


                    refreshSolutions(cEF);
                }
                else //Combined Footing
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //to copy
                }
            }
            else // Wall Footing
            {
                if (cEF.structuralMembers.footingsWall[0][wallFootingCount][0].Equals("Rectangular"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //Computation

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

                    refreshSolutions(cEF);
                }
                else //Trapezoidal
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //Computation

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

                    refreshSolutions(cEF);
                }
            }
        }

        //Modify
        public void footingWorks(CostEstimationForm cEF, int structMemCount, bool isFooting)
        {
            if (isFooting)
            {
                if (cEF.structuralMembers.footingsColumn[0][structMemCount][0].Equals("Isolated Footing"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //Computation
                    //Check index of footing in earthworkSolutions
                    int i = 0;
                    foreach(List<double> data in cEF.structuralMembers.earthworkSolutions)
                    {
                        if(data[0] == 1)
                        {
                            i++;
                        }
                        if(structMemCount == i)
                        {
                            break;
                        }
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

                    refreshSolutions(cEF);
                }
                else //Combined Footing
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_CF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //TODO: to copy
                }
            }
            else // Wall Footing
            {
                if (cEF.structuralMembers.footingsWall[0][structMemCount][0].Equals("Rectangular"))
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //Computation
                    //Check index of wall footing in earthworkSolutions
                    int i = 0;
                    foreach (List<double> data in cEF.structuralMembers.earthworkSolutions)
                    {
                        if (data[0] == 2)
                        {
                            i++;
                        }
                        if (structMemCount == i)
                        {
                            break;
                        }
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

                    refreshSolutions(cEF);
                }
                else //Trapezoidal
                {
                    //Variables from Parameters
                    double gravelBedding, formworkAllowance, compactionAllowance;

                    //Init variables from Parameters
                    gravelBedding = double.Parse(cEF.parameters.earth_WF_TH, System.Globalization.CultureInfo.InvariantCulture);
                    formworkAllowance = double.Parse(cEF.parameters.earth_WF_FA, System.Globalization.CultureInfo.InvariantCulture);
                    string value = cEF.parameters.earth_WF_CF.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    compactionAllowance = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

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

                    //Computation
                    //Check index of wall footing in earthworkSolutions
                    int i = 0;
                    foreach (List<double> data in cEF.structuralMembers.earthworkSolutions)
                    {
                        if (data[0] == 2)
                        {
                            i++;
                        }
                        if (structMemCount == i)
                        {
                            break;
                        }
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

                    refreshSolutions(cEF);
                }
            }
        }
        //Earthworks Functions -- END

        //Stairs Functions -- START
        public void AddStairsWorks(CostEstimationForm cEF, int floorCount, int stairsCount)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.stairsSolutions.Add(newList);
            stairsWorks(cEF, floorCount, stairsCount);
        }

        public void ModifyStairsWorks(CostEstimationForm cEF, int floorCount, int structMemCount)
        {
            stairsWorks(cEF, floorCount, structMemCount);
        }

        //Add or Modify
        /*
         * Straight Stairs
            L-Stairs
            U-Stairs
         * */
        public void stairsWorks(CostEstimationForm cEF, int floorCount, int structMemCount)
        {
            if (cEF.structuralMembers.stairs[floorCount][structMemCount][0].Equals("Straight Stairs"))
            {
                double length;
                length = double.Parse(cEF.structuralMembers.stairs[floorCount][structMemCount][1], System.Globalization.CultureInfo.InvariantCulture);
                cEF.cement_Total[4] += length;
            }

        }
        //Stairs Funcitons -- END
    }
}
