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
        public void Excavation(CostEstimationForm cEF, int structMemCount)
        {
            double excavation = 0;
            double length, width, thickness, quantity, depth, lrDiameter, lrQuantity, lrHookType,
                   trDiameter, trQuantity, trHookType;
            length = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][0], System.Globalization.CultureInfo.InvariantCulture);
            width = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][1], System.Globalization.CultureInfo.InvariantCulture);
            thickness = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][2], System.Globalization.CultureInfo.InvariantCulture);
            quantity = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][3], System.Globalization.CultureInfo.InvariantCulture);
            depth = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][4], System.Globalization.CultureInfo.InvariantCulture);
            lrDiameter = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][5], System.Globalization.CultureInfo.InvariantCulture);
            lrQuantity = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][6], System.Globalization.CultureInfo.InvariantCulture);
            lrHookType = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][7], System.Globalization.CultureInfo.InvariantCulture);
            trDiameter = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][8], System.Globalization.CultureInfo.InvariantCulture);
            trQuantity = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][9], System.Globalization.CultureInfo.InvariantCulture);
            trHookType = double.Parse(cEF.structuralMembers.footingColumnIsolated[0][structMemCount][10], System.Globalization.CultureInfo.InvariantCulture);

            double area = length * width * thickness;

            excavation = length;
            //TODO variables for sub totalities instead of excavation(final totality for earthworks)
            cEF.excavation_Total += excavation;
        }
    }
}
