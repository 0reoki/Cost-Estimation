﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class StructuralMembers
    {
        CostEstimationForm cEF;
        Compute compute = new Compute();
        //Structure 1st List -- floor, 2nd List -- str members, 3rd List -- values
        //See AddStructForm for order of values

        //Footing Variables
        public List<string> footingColumnNames = new List<string>();
        public List<List<List<string>>> footingsColumn = new List<List<List<string>>>();
        public List<string> footingWallNames = new List<string>();
        public List<List<List<string>>> footingsWall = new List<List<List<string>>>();

        //Stairs
        public List<List<string>> stairsNames = new List<List<string>>();
        public List<List<List<string>>> stairs = new List<List<List<string>>>();

        //Solution variables
        public List<List<double>> earthworkSolutions = new List<List<double>>();
        public List<List<double>> stairsSolutions = new List<List<double>>();

        public StructuralMembers(CostEstimationForm cEF)
        {
            this.cEF = cEF;
        }

        public void reComputeEarthworks()
        {
            int i = 0;
            int j = 0;
            foreach(List<double> data in earthworkSolutions)
            {
                if(data[0] == 1)
                {
                    compute.ModifyFootingWorks(cEF, i , true);
                    i++;
                }
                else
                {
                    compute.ModifyFootingWorks(cEF, j, false);
                    j++;
                }
            }
        }
    }
}
