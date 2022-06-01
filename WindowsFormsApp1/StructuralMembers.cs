using System;
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

        //Columns Variables
        public List<List<string>> columnNames = new List<List<string>>();
        public List<List<List<string>>> column = new List<List<List<string>>>();
        public List<List<List<string>>> columnLateralTies = new List<List<List<string>>>();
        public List<List<List<string>>> columnSpacing = new List<List<List<string>>>();

        //Beam Variables
        public List<List<string>> beamNames = new List<List<string>>();
        public List<List<List<string>>> beam = new List<List<List<string>>>();
        public List<List<List<List<string>>>> beamRow = new List<List<List<List<string>>>>();
        public List<List<List<string>>> beamSchedule = new List<List<List<string>>>();

        //Slab Variables
        public List<List<string>> slabNames = new List<List<string>>();
        public List<List<List<string>>> slab = new List<List<List<string>>>();
        public List<List<List<string>>> slabSchedule = new List<List<List<string>>>();

        //Stairs Variables
        public List<List<string>> stairsNames = new List<List<string>>();
        public List<List<List<string>>> stairs = new List<List<List<string>>>();

        //Roof Variables
        public List<List<string>> roofNames = new List<List<string>>();
        public List<List<List<string>>> roof = new List<List<List<string>>>();
        public List<List<List<string>>> roofHRS = new List<List<List<string>>>();

        //Solution Earthwork variables
        public List<List<double>> earthworkSolutions = new List<List<double>>();
        public List<double> extraEarthworkSolutions = new List<double>();

        //Solution Concrete variables
        public List<List<double>> concreteWorkSolutionsF = new List<List<double>>();
        public List<List<List<double>>> concreteWorkSolutionsC = new List<List<List<double>>>();
        public List<List<List<double>>> concreteWorkSolutionsBR = new List<List<List<double>>>();
        public List<List<List<double>>> concreteWorkSolutionsSL = new List<List<List<double>>>();
        public List<List<string>> concreteWorkSolutionsSLSM = new List<List<string>>();
        public List<List<List<double>>> concreteWorkSolutionsST = new List<List<List<double>>>();
        public List<double> concreteWorkSolutionsFS = new List<double>();

        //Solution Stair variables
        public List<List<double>> stairsSolutions = new List<List<double>>();

        //Constant variables
        public List<List<double>> concreteProportion = new List<List<double>>();

        public StructuralMembers(CostEstimationForm cEF)
        {
            //Initialize Variables
            this.cEF = cEF;
            for(int i = 0; i < 4; i++)
            {
                extraEarthworkSolutions.Add(0);
            }

            //Initialize Constant Variables
            double subtrahend = 3;
            double initValue = 12.0;
            for(int i = 0; i < 4; i++)
            {
                List<double> toAdd = new List<double>();
                toAdd.Add(initValue);
                toAdd.Add(0.5);
                toAdd.Add(1.0);
                if (i < 1)
                {
                    initValue = initValue - subtrahend;
                } 
                else
                {
                    initValue = initValue - (subtrahend / 2);
                }
                concreteProportion.Add(toAdd);
            }
        }

        public void reComputeEarthworks()
        {
            int i = 0;
            int j = 0;
            foreach(List<double> data in earthworkSolutions)
            {

                if(data[0] == 1)
                {
                    compute.ModifyFootingWorks(cEF, i, i, true);
                    i++;
                }
                else
                {
                    compute.ModifyFootingWorks(cEF, j, j, false);
                    j++;
                }
            }

            i = 0;
            foreach (List<List<double>> floor in concreteWorkSolutionsC)
            {
                j = 0;
                foreach(List<double> member in floor)
                {
                    compute.ModifyColumnWorks(cEF, i, j);
                    j++;
                }
                i++;
            }

            i = 0;
            foreach (List<List<double>> floor in concreteWorkSolutionsBR)
            {
                j = 0;
                foreach (List<double> member in floor)
                {
                    compute.ModifyBeamWorks(cEF, i, j);
                    j++;
                }
                i++;
            }
        }
    }
}
