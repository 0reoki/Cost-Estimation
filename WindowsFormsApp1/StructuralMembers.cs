using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowEst
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

        //Solution Roof        
        public List<List<List<double>>> roofSolutions = new List<List<List<double>>>();

        //Solution Formworks
        //footings
        public List<double> per_col = new List<double>();
        public List<double> per_wal = new List<double>();
        public List<double> footings_comps = new List<double>();// formworkFC - frameworkFC - formworkWF [FOOTINGS]
        //columns
        public List<double> col_area = new List<double>();
        public List<double> col_woods = new List<double>();
        public List<double> col_post = new List<double>();
        public List<double> col_scafV = new List<double>();
        public List<double> col_scafH = new List<double>();
        public List<double> col_scafD = new List<double>();
        //beams
        public List<double> beams_comps = new List<double>();//tieForm - tieFrame  --
        public List<double> beams_tiearea = new List<double>();//tie area
        public List<double> beams_gradarea = new List<double>();//beam area
        public List<double> beams_grade = new List<double>();//grade form
        public List<double> beams_gradeFrame = new List<double>();//grade frame
        public List<double> beams_vertical = new List<double>();//grade vertical
        public List<double> beams_horizontal = new List<double>();//grade horizontal
        public List<double> beams_RB = new List<double>();//roof form
        public List<double> beams_RBarea = new List<double>();//roof area
        public List<double> beams_RBframe = new List<double>();//roof frame
        public List<double> beams_RBV = new List<double>();//roof vertical
        public List<double> beams_RBH = new List<double>();//roof horizontal
        //slab
        public List<double> slab_area = new List<double>();
        public List<double> slab_form = new List<double>();
        public List<double> slab_scaf = new List<double>();
        //stairs
        public List<double> UstairsFORM= new List<double>();
        public List<double> UstairsFRAME = new List<double>();
        public List<double> UstairsSCAF = new List<double>();
        public List<double> LstairsFORM = new List<double>();
        public List<double> LstairsFRAME = new List<double>();
        public List<double> LstairsSCAF = new List<double>();
        public List<double> SstairsFORM = new List<double>();
        public List<double> SstairsFRAME = new List<double>();
        public List<double> SstairsSCAF = new List<double>();
        public List<double> UAREA = new List<double>();
        public List<double> LAREA = new List<double>();
        public List<double> SAREA = new List<double>();

        //steel reinforcements
        //----- COLUMN MAIN ------//
        public List<List<List<string>>> Column_mainRebar = new List<List<List<string>>>();
        public double totalweightkgm_Colmain = 0;
        //----- COLUMN LATERAL TIES ------//
        public List<List<List<List<string>>>> Column_lateralRebar = new List<List<List<List<string>>>>();        
        public double totalweightkgm_Colties = 0;
        //----- BEAM MAIN ------//
        public List<List<List<string>>> beamdias = new List<List<List<string>>>();
        public double totalweightkgm_main = 0;
        public List<List<List<List<string>>>> Beam_mainRebar = new List<List<List<List<string>>>>();
        //----- BEAM STIRRUPS ------//
        public double totalweightkgm_stir = 0;
        public List<List<List<List<string>>>> Beam_stirRebar = new List<List<List<List<string>>>>();
        //----- BEAM WEB ------//
        public List<List<List<List<string>>>> Beam_webRebar = new List<List<List<List<string>>>>();
        public double totalweightkgm_web = 0;
        ///----- SLAB GRADE ------//
        public List<List<List<string>>> Slab_ongradeRebar = new List<List<List<string>>>();
        public double totalweightkgm_slabongrade = 0;
        ///----- SUSPENDED SLAB ------//        
        public List<List<List<double>>> Slab_suspendedRebar = new List<List<List<double>>>();
        public double totalweightkgm_suspendedslab = 0;
        //Constant variables
        //Cement
        //Sand
        //Gravel
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
            //tiles function call                       
            cEF.compute.computeTiles(cEF);
            //paints function call
            cEF.compute.computePaints(cEF);
            //Masonry function call            
            cEF.masonrysSolutionP1 = cEF.compute.computeMasonry(cEF, cEF.parameters.mason_exteriorWall, cEF.parameters.mason_exteriorWindow, cEF.parameters.mason_exteriorDoor, cEF.parameters.mason_interiorWall, cEF.parameters.mason_interiorWindow, cEF.parameters.mason_interiorDoor, cEF.parameters.mason_CHB_EW, cEF.parameters.mason_CHB_IW);
            cEF.masonrysSolutionP2 = cEF.compute.computeConcreteWall_mortar(cEF, cEF.parameters.conc_CM_W_MEW_CM, cEF.parameters.conc_CM_W_MIW_CM, cEF.parameters.conc_CM_W_P_CM, cEF.parameters.conc_CM_W_P_PT);
            cEF.masonrysSolutionP3 = cEF.compute.computeCHB_reinforcement(cEF.masonrysSolutionP1[3], cEF.masonrysSolutionP1[8], cEF.parameters.mason_RTW_VS, cEF.parameters.mason_RTW_HSL, cEF.parameters.mason_RTW_RG, cEF.parameters.mason_RTW_BD, cEF.parameters.mason_RTW_RL, cEF.parameters.mason_RTW_LTW);
            
        }
    }
}
