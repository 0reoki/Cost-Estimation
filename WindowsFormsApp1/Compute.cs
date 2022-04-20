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
        public void AddEarthworks(CostEstimationForm cEF, int structMemCount)
        {
            List<double> newList = new List<double>();
            cEF.structuralMembers.earthworkSolutions.Add(newList);
            AllEarthworks(cEF, structMemCount, true);
        }

        public void ModifyEarthworks(CostEstimationForm cEF, int structMemCount)
        {
            AllEarthworks(cEF, structMemCount, false);
        }

        public void AllEarthworks(CostEstimationForm cEF, int structMemCount, bool isNew)
        {
            double excavation = 0;
            
            if (cEF.structuralMembers.footingsColumn[0][structMemCount][0].Equals("Isolated Footing"))
            {
                //Variables from Parameters
                double gravelBedding, formworkAllowance;

                //Init variables from Parameters
                gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);

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


                if (isNew)
                {
                    //Computation for Excavation and add to Solutions

                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[structMemCount].Add(
                        (length + formworkAllowance * 2) * (width + formworkAllowance * 2) * (depth + gravelBedding) * quantity);



                }
                else
                {
                    //Computation for Excavation and add to Solutions

                    //Excavation
                    cEF.structuralMembers.earthworkSolutions[structMemCount][0] =
                        (length + formworkAllowance * 2) * (width + formworkAllowance * 2) * (depth + gravelBedding) * quantity;



                }

                //Refresh excav_Total
                cEF.excavation_Total = 0;
                foreach (List<double> toAdd in cEF.structuralMembers.earthworkSolutions)
                {
                    cEF.excavation_Total += toAdd[0];
                }

                //TODO variables for sub totalities instead of excavation(final totality for earthworks)
                cEF.excavation_Total = cEF.excavation_Total / 1000000000;
                cEF.excavation_Total += excavation;
            }
            else //Combined Footing
            {
                //Variables from Parameters
                double gravelBedding, formworkAllowance;

                //Init variables from Parameters
                gravelBedding = double.Parse(cEF.parameters.earth_CF_TH, System.Globalization.CultureInfo.InvariantCulture);
                formworkAllowance = double.Parse(cEF.parameters.earth_CF_FA, System.Globalization.CultureInfo.InvariantCulture);

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

                //to copy
            }
            
        }
    }
}
