using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowEst
{
    public partial class CostEstimationForm : Form
    {
        //Forms
        public Compute compute = new Compute();
        public ParametersForm pf;
        public Parameters parameters;
        public StructuralMembers structuralMembers;
        public List<Floor> floors = new List<Floor>();

        //Passed Variables

        //Local Variables
        public List<LaborAndEquipmentUserControl> laqUC;
        List<string> hoursList;
        List<string> daysList;
        public bool saveFileExists;
        public bool viewInitalized;
        String fileName;
        DataTable summ_BOQ_dt;
        BindingSource summ_BOQ_bs;

        //Volume Totality Variables (Volume Totality is for pricing computation)
        //If price is missing CostTotal, use CostL or CostM instead (no pair)

        //Price Checklists (Show if checklist == true)
        public bool[] earthworksChecklist = { true, true, true, true, true, true }; //1.0
        public bool[] concreteChecklist = { true, true, true, true, true, true }; //2.0
        public bool[] formworksChecklist = { true, true, true, true , true , true }; //3.0 **
        public bool[] masonryChecklist = { true, true }; //4.0
        public bool[] rebarsChecklist = { true, true, true, true, true, true, true, true };//5.0**
        public bool[] roofingsChecklist = { true, true, true}; //6.0
        public bool[] tilesChecklist = { true, true }; //7.0
        public bool[] paintsChecklist = { true, true, true, true }; //8.0

        //Factor of Safety for Price
        public string fos_Cement, fos_Sand, fos_Gravel, fos_RMC, fos_CHB, fos_Tiles, fos_LC_Type, fos_LC_Percentage;

        //Totalities -- START

        //1.0 - Earthwork 
        public double excavation_Total, backfillingAndCompaction_Total, gradingAndCompaction_Total,
                      gravelBedding_Total, soilPoisoning_Total;

        //2.0 - Concrete Works Variables - Footing, Column, Beams, SOG Slab, SS Slab, Stairs
        public double[] cement_Total = new double[6];
        public double[] sand_Total = new double[6];
        public double[] gravel_Total = new double[6];
        public double[] concreteVolume_Total = new double[6];

        //4.0 Masonry **
        //exteriorWall -> exteriorWindow -> exteriorDoor -> exteriorCHBarea -> exteriorCHBtotalpcs -> interiorWall -> interiorWindow -> interiorDoor -> interiorCHBarea ->  interiorCHBtotalpcs
        public List<double> masonrysSolutionP1 = new List<double>();

        //eLPUta ka talaga Avelino Ψ(￣∀￣)Ψ

        //extCement -> extSand -> intCement -> intSand -> extplasterArea -> extplasterCement -> extplasterSand -> intplasterArea -> intplasterCement -> intplasterSand
        public List<double> masonrysSolutionP2 = new List<double>();

        //extvBAR -> exthBAR -> extReinforcementCHB -> extReinforcementWeight -> extTieWire -> intvBAR -> inthBAR -> intReinforcementCHB -> intReinforcementWeight -> intTieWire
        public List<double> masonrysSolutionP3 = new List<double>();
        public string extCHBdimension;
        public string intCHBdimension;        

        //7.0 Tiles **
        //area -> tilesPCS -> tilesADHESIVE_regular -> tilesADHESIVE_heavy -> tilesGROUT
        public List<double> sixhun = new List<double>();
        public List<double> threehun = new List<double>();

        //8.0 Paints **
        //area -> neut -> skim -> primer ->paintGAL
        public List<double> enamel = new List<double>();
        public List<double> acrylic = new List<double>();
        public List<double> latex = new List<double>();
        public List<double> gloss = new List<double>();

        //Totalities -- END

        //Cost -- START

        //1.0  - Earthwork
        public double excavation_CostL, backfillingAndCompaction_CostL, backfillingAndCompaction_CostM, backfillingAndCompaction_CostTotal,
                      gradingAndCompaction_CostL, gravelBedding_CostM, gravelBedding_CostL, gravelBedding_CostTotal,
                      soilPoisoning_CostM, earthworks_CostTotal;
        public double earthworks_TotalCostM, earthworks_TotalCostL;

        //2.0 Concrete 
        public double concreteF_UnitM,      //Concrete Footing - Materials Unit
                      concreteF_CostM,      //Concrete Footing - Materials Cost
                      concreteF_CostL,      //Concrete Footing - Labor Cost
                      concreteF_costTotal,  //Concrete Footing - TOTAL COST
                      
                      concreteC_UnitM,      //Concrete Column - Materials Unit
                      concreteC_CostM,      //Concrete Column - Materials Cost
                      concreteC_CostL,      //Concrete Column - Labor Cost
                      concreteC_costTotal,  //Concrete Column - TOTAL COST

                      concreteBR_UnitM,      //Concrete Beam - Materials Unit
                      concreteBR_CostM,      //Concrete Beam - Materials Cost
                      concreteBR_CostL,      //Concrete Beam - Labor Cost
                      concreteBR_costTotal,  //Concrete Beam - TOTAL COST

                      concreteSOG_UnitM,      //Concrete SOG - Materials Unit
                      concreteSOG_CostM,      //Concrete SOG - Materials Cost
                      concreteSOG_CostL,      //Concrete SOG - Labor Cost
                      concreteSOG_costTotal,  //Concrete SOG - TOTAL COST

                      concreteSS_UnitM,      //Concrete SS - Materials Unit
                      concreteSS_CostM,      //Concrete SS - Materials Cost
                      concreteSS_CostL,      //Concrete SS - Labor Cost
                      concreteSS_costTotal,  //Concrete SS - TOTAL COST

                      concreteST_UnitM,      //Concrete Stairs - Materials Unit
                      concreteST_CostM,      //Concrete Stairs - Materials Cost
                      concreteST_CostL,      //Concrete Stairs - Labor Cost
                      concreteST_costTotal,  //Concrete Stairs - TOTAL COST

                      concreteFS_CostM,      //Concrete Factor of Safety - Materials Cost
                      concreteFS_costTotal,  //Concrete Factor of Safety - TOTAL COST

                      concreteMCost_total,  //Concrete Materials - Total Cost
                      concreteLCost_total,  //Concrete Labor - Total Cost                                           
                      concrete_TOTALCOST;   //Concrete - OVERALL COST


        //3.0 Formworks
        public double footing_QTY, trap_QTY, //footings - QTY
                      footing_Munit, trap_Munit, //footings - materials unit
                      footing_Mcost, trap_Mcost, //footings - materials cost
                      footing_Lunit, trap_Lunit, //footings - labor unit
                      footing_Lcost, trap_Lcost, //footings - labor cost
                      footingMLCOST, trapMLCOST, //footings - MatLab cost
                      col_Munit, //column - materials unit 
                      col_Mcost, //column - materials cost
                      col_Lunit, //column - labor unit
                      col_Lcost, //column - labor cost
                      colMLCOST, //column - MatLab cost
                      col_QTY, //column - QTY
                      beam_Munit, //beam - materials unit
                      beam_Mcost, //beam - materials cost
                      beam_Lunit, //beam - labor unit
                      beam_Lcost, //beam - labor cost
                      beamMLCOST,//beam - MatLab cost
                      beam_QTY, //beam - QTY
                      sus_Munit, //slab - materials unit
                      sus_Mcost, //slab - materials cost
                      sus_Lunit, //slab - labor unit
                      sus_Lcost, //slab - labor cost
                      sus_MLCOST,//slab - MatLab cost
                      sus_QTY, //slab - QTY
                      stairs_Munit, //stairs - materials unit
                      stairs_Mcost, //stairs - materials cost
                      stairs_Lunit, //stairs - labor unit
                      stairs_Lcost, //stairs - labor cost
                      stairs_MLCOST,//stairs - MatLab cost
                      stairs_QTY, //stairs - QTY
                      nails_Munit, //nails - materials unit
                      nails_Mcost, //nails - materials cost                      
                      nailsMLCOST, //nails - MatLab cost
                      nails_QTY, //nails - QTY
                      FW_totalMats, //formworks - MATERIALS TOTAL
                      FW_totalLab, //formworks - LABOR TOTAL
                      FW_MATOTAL;//formworks - OVERALL TOTAL     

        //4.0 Masonry **
        public double exterior_UnitM, //exterior - Materials Unit
                      exterior_CostM, //exterior - Materials Cost
                      exterior_CostL, //exterior - Labor Cost
                      exterior_costTotal, //exterior - TOTAL COST
                      interior_UnitM, //interior - Materials Unit
                      interior_CostM, //interior - Materials Cost
                      interior_CostL, //interior - Labors Unit
                      interior_costTotal,  //interior - TOTAL COST
                      masonMCost_total, //Materials - Total Cost
                      masonLCost_total, //Labor - Total Cost                                           
                      mason_TOTALCOST; // Masonry - OVERALL COST

        // exterior QTY -> masonrysSolutionP1[3]
        // interior QTY -> masonrysSolutionP1[8]

        //5.0 Rebars **
        public double reinF_QTY,         //Reinforcements Footing - Quantity
                      reinF_UnitM,       //Reinforcements Footing - Materials Unit
                      reinF_CostM,       //Reinforcements Footing - Materials Cost
                      reinF_CostL,       //Reinforcements Footing - Labor Cost
                      reinF_TotalCost,   //Reinforcements Footing - TOTAL COST
                      reinWF_QTY,        //Reinforcements Wall Footing - Quantity
                      reinWF_UnitM,      //Reinforcements Wall Footing - Materials Unit
                      reinWF_CostM,      //Reinforcements Wall Footing - Materials Cost
                      reinWF_CostL,      //Reinforcements Wall Footing - Labor Cost
                      reinWF_TotalCost;  //Reinforcements Wall Footing - TOTAL COST
                      
        public double Rbeam_qty, // BEAM
                      Rbeam_Munit,
                      Rbeam_Mcost,
                      Rbeam_Lunit,
                      Rbeam_Lcost,
                      Rbeam_TOTALCOST,
                      RSOG_qty, // SLAB ON GRADE
                      RSOG_Munit,
                      RSOG_Mcost,
                      RSOG_Lunit,
                      RSOG_Lcost,
                      RSOG_TOTALCOST,
                      RSS_qty, // SUSPENDED SLAB
                      RSS_Munit,
                      RSS_Mcost,
                      RSS_Lunit,
                      RSS_Lcost,
                      RSS_TOTALCOST,
                      RCOL_qty, //COLUMN
                      RCOL_Munit,
                      RCOL_Mcost,
                      RCOL_Lunit,
                      RCOL_Lcost,
                      RCOL_TOTALCOST,
                      RSTAIRS_qty, //STAIRS
                      RSTAIRS_Munit,
                      RSTAIRS_Mcost,
                      RSTAIRS_Lunit,
                      RSTAIRS_Lcost,
                      RSTAIRS_TOTALCOST,
                      RWALLS_qty, //WALLS
                      RWALLS_Munit,
                      RWALLS_Mcost,
                      RWALLS_Lunit,
                      RWALLS_Lcost,
                      RWALLS_TOTALCOST;

        public double rein_MCost,        //Reinforcements - Total Material Cost
                      rein_LCost,        //Reinforcements - Total Labor Cost
                      rein_TotalCost;    //Reinforcements - Total Cost

        //6.0 Roofings **
        public double rANDp_MCost,// Rafter and Purlins - Material Cost
            rANDp_LCost,// Rafter and Purlins - Labor Cost
            rANDp_costTotal,// Rafter and Purlins - TOTAL COST
            acce_MCost, // G.I Roof and Its Accessories - Material Cost
            acce_LCost,// G.I Roof and Its Accessories - Labor Cost
            acce_costTotal,// G.I Roof and Its Accessories - TOTAL COST
            tins_MCost,// Tinswork - Material Cost
            tins_LCost,// Tinswork - Labor Cost
            tins_costTotal,// Tinswork - TOTAL COST
            roof_MTOTAL,// Total Material Cost
            roof_LTOTAL,// Total Labor Cost
            roof_TOTALCOST,// Overall Total Cost    
            roof_MUNIT,
            roof_QTY;
            

        public double cocoLumber_price = 62;//static price for wood **

        //7.0 Tiles **
        public double sixhun_MUnit, //600 x 600 - Materials Unit
                      sixhun_MCost, //600 x 600 - Materials Cost
                      sixhun_LCost, //600 x 600 - Labor Cost
                      sixhun_costTotal, //600 x 600 - TOTAL COST
                      threehun_MUnit,  //300 x 300 - Materials Unit
                      threehun_MCost, //300 x 300 - Materials Cost
                      threehun_LCost, //300 x 300 - Labor Cost
                      threehun_costTotal, //300 x 300 - TOTAL COST
                      tiles_mTOTALCOST, //Materials - TOTAL COST
                      tiles_lTOTALCOST, //Labor - TOTAL COST
                      tiles_TOTALCOST; //Tiles - OVERALL COST
                                       //600 x 600 QTY -> sixhun[0]
                                       //300 x 300 QTY -> threehun[0]
        //8.0 Paints **
        public double enam_MUnit, //Enamel - Materials Unit
                      enam_MCost, //Enamel - Materials Cost
                      enam_LCost, //Enamel - Labor Cost
                      enam_TOTALCOST,//Enamel - TOTAL COST
                      acry_MUnit, //Acrylic - Materials Unit
                      acry_MCost, //Acrylic - Materials Cost
                      acry_LCost, //Acrylic - Labor Cost
                      acry_TOTALCOST, //Acrylic - TOTAL COST
                      late_MUnit, //Latex - Materials Unit
                      late_MCost, //Latex - Materials Cost
                      late_LCost, //Latex - Labor Cost
                      late_TOTALCOST,//Latex - TOTAL COST
                      semi_MUnit, //Semi-gloss - Materials Unit
                      semi_MCost, //Semi-gloss - Materials Cost
                      semi_LCost, //Semi-gloss - Labor Cost
                      semi_TOTALCOST,//Semi-gloss - TOTAL COST
                      paint_mTOTALCOST, //Materials TOTAL COST
                      paint_lTOTALCOST, //Labor TOTAL COST
                      paints_TOTALCOST; //Paints - OVERALL COST

        //Enamel QTY -> enamel[0]
        //Acrylic QTY -> acrylic[0]
        //Latex QTY -> latex[0]
        //Semi-gloss QTY -> gloss[0]
        //9.0 - Miscellaneous Items
        public double misc_TOTALCOST;

        //10.0 - Additional Labor and Equipment
        public double laborAndEqpt_TOTALCOST;

        //BOQ - FINAL COST
        public double TOTALCOST;

        //Cost -- END

        //Getters and Setters
        public List<Floor> Floors { get => floors; set => floors = value; }

        //General Functions -- START
        public CostEstimationForm()
        {
            InitializeComponent();
            InitializeAsync();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //Initialize Forms that are single throughout the whole program
            viewInitalized = false;
            saveFileExists = false;
            structuralMembers = new StructuralMembers(this);
            parameters = new Parameters();
            pf = new ParametersForm(parameters, this);

            addFloor();

            //Initialize Local Variables
            hoursList = new List<string>();
            daysList = new List<string>();
            laqUC = new List<LaborAndEquipmentUserControl>();
            fileName = null;
            
            //Factor of Safety default values
            fos_Cement = "5%";
            fos_Sand = "5%";
            fos_Gravel = "5%";
            fos_RMC = "5%"; 
            fos_CHB = "5%";
            fos_Tiles = "10%"; 
            fos_LC_Type = "Rate"; 
            fos_LC_Percentage = "30%";

            //Earthwork Variables
            excavation_Total = 0; 
            backfillingAndCompaction_Total = 0; 
            gradingAndCompaction_Total = 0;
            gravelBedding_Total = 0; 
            soilPoisoning_Total = 0;
            excavation_CostL = 0;
            backfillingAndCompaction_CostL = 0;
            backfillingAndCompaction_CostM = 0;
            backfillingAndCompaction_CostTotal = 0;
            gradingAndCompaction_CostL = 0;
            gravelBedding_CostM = 0;
            gravelBedding_CostL = 0;
            gravelBedding_CostTotal = 0;
            soilPoisoning_CostM = 0;
            earthworks_CostTotal = 0;

            //Formworks **
            footing_Munit = 0;
            footing_Mcost = 0;
            footing_Lunit = 0;
            footing_Lcost = 0;
            footingMLCOST = 0;
            footing_QTY = 0;
            trap_Munit = 0;
            trap_Mcost = 0;
            trap_Lunit = 0;
            trap_Lcost = 0;
            trapMLCOST = 0;
            trap_QTY = 0;
            col_Munit = 0;
            col_Mcost = 0;
            col_Lunit = 0;
            col_Lcost = 0;
            colMLCOST = 0;
            col_QTY = 0;
            beam_Munit = 0;
            beam_Mcost = 0;
            beam_Lunit = 0;
            beam_Lcost = 0;
            beamMLCOST = 0;
            beam_QTY = 0;
            sus_Munit = 0;
            sus_Mcost = 0;
            sus_Lunit = 0;
            sus_Lcost = 0;
            sus_MLCOST = 0;
            sus_QTY = 0;
            stairs_Munit = 0;
            stairs_Mcost = 0;
            stairs_Lunit = 0;
            stairs_Lcost = 0;
            stairs_MLCOST = 0;
            stairs_QTY = 0;
            nails_Munit = 0;
            nails_Mcost = 0;            
            nailsMLCOST = 0;
            nails_QTY = 0;
            FW_totalMats = 0;
            FW_totalLab = 0;
            FW_MATOTAL = 0;

            //Masonry **
            exterior_UnitM = 0; 
            exterior_CostM = 0;
            exterior_CostL = 0;
            exterior_costTotal = 0;
            interior_UnitM = 0;
            interior_CostM = 0;
            interior_CostL = 0;
            interior_costTotal = 0; 
            masonMCost_total = 0; 
            masonLCost_total = 0;                                            
            mason_TOTALCOST = 0;

            //Reinforcement steel **
            reinF_UnitM = 0;
            reinF_CostM = 0;
            reinF_CostL = 0;
            reinF_TotalCost = 0;
            reinWF_UnitM = 0;
            reinWF_CostM = 0;
            reinWF_CostL = 0;
            reinWF_TotalCost = 0;

            Rbeam_qty = 0;
            Rbeam_Munit = 0;
            Rbeam_Mcost = 0;
            Rbeam_Lunit = 0;
            Rbeam_Lcost = 0;
            Rbeam_TOTALCOST = 0;
            RSOG_qty = 0; 
            RSOG_Munit = 0;
            RSOG_Mcost = 0;
            RSOG_Lunit = 0;
            RSOG_Lcost = 0;
            RSOG_TOTALCOST = 0;
            RCOL_qty = 0;
            RCOL_Munit = 0;
            RCOL_Mcost = 0;
            RCOL_Lunit = 0;
            RCOL_Lcost = 0;
            RCOL_TOTALCOST = 0;
            RSS_qty = 0;
            RSS_Munit = 0;
            RSS_Mcost = 0;
            RSS_Lunit = 0;
            RSS_Lcost = 0;
            RSS_TOTALCOST = 0;
            RSTAIRS_qty = 0;
            RSTAIRS_Munit = 0;
            RSTAIRS_Mcost = 0;
            RSTAIRS_Lunit = 0;
            RSTAIRS_Lcost = 0;
            RSTAIRS_TOTALCOST = 0;
            RWALLS_qty = 0;
            RWALLS_Munit = 0;
            RWALLS_Mcost = 0;
            RWALLS_Lunit = 0;
            RWALLS_Lcost = 0;
            RWALLS_TOTALCOST = 0;

            //Roofings **
            rANDp_MCost = 0; 
            rANDp_LCost = 0; 
            rANDp_costTotal = 0; 
            acce_MCost = 0; 
            acce_LCost = 0; 
            acce_costTotal = 0; 
            tins_MCost = 0; 
            tins_LCost = 0; 
            tins_costTotal = 0; 
            roof_MTOTAL = 0; 
            roof_LTOTAL = 0; 
            roof_TOTALCOST = 0;
            roof_QTY = 0;

            //Tiles ** 
            sixhun_MUnit = 0; 
            sixhun_MCost = 0;
            sixhun_LCost = 0;
            sixhun_costTotal = 0;
            threehun_MUnit = 0;
            threehun_MCost = 0;
            threehun_LCost = 0;
            threehun_costTotal = 0;
            tiles_mTOTALCOST = 0;
            tiles_lTOTALCOST = 0;
            tiles_TOTALCOST = 0;

            //Paints ** =0;
             enam_MUnit = 0;
            enam_MCost = 0;
            enam_LCost = 0;
            enam_TOTALCOST = 0;
            acry_MUnit = 0;
            acry_MCost = 0;
            acry_LCost = 0;
            acry_TOTALCOST = 0;
            late_MUnit = 0;
            late_MCost = 0;
            late_LCost = 0;
            late_TOTALCOST = 0;
            semi_MUnit = 0;
            semi_MCost = 0;
            semi_LCost = 0;
            semi_TOTALCOST = 0;
            paint_mTOTALCOST = 0;
            paint_lTOTALCOST = 0;
            paints_TOTALCOST = 0;

            //Concrete Works Variables
            for (int i = 0; i < cement_Total.Length; i++)
            {
                cement_Total[i] = 0;
                sand_Total[i] = 0;
                gravel_Total[i] = 0;
                concreteVolume_Total[i] = 0;
            }
            concreteF_UnitM = 0;
            concreteF_CostM = 0;
            concreteF_CostL = 0;
            concreteF_costTotal = 0;

            concreteC_UnitM = 0;
            concreteC_CostM = 0;
            concreteC_CostL = 0;
            concreteC_costTotal = 0;

            concreteBR_UnitM = 0;
            concreteBR_CostM = 0;
            concreteBR_CostL = 0;
            concreteBR_costTotal = 0;

            concreteSOG_UnitM = 0;
            concreteSOG_CostM = 0;
            concreteSOG_CostL = 0;
            concreteSOG_costTotal = 0;

            concreteSS_UnitM = 0;
            concreteSS_CostM = 0;
            concreteSS_CostL = 0;
            concreteSS_costTotal = 0;

            concreteST_UnitM = 0;
            concreteST_CostM = 0;
            concreteST_CostL = 0;
            concreteST_costTotal = 0;

            concreteFS_CostM = 0;
            concreteFS_costTotal = 0;

            concreteMCost_total = 0;
            concreteLCost_total = 0;
            concrete_TOTALCOST = 0;

            //Miscellaneous Variables
            misc_TOTALCOST = 0;

            //Additional Labor and Equipment Variables
            laborAndEqpt_TOTALCOST = 0;
            //Initialize Price Parameters
            InitPriceList();

            //Remove tab control tabpages
            priceTabControl.Appearance = TabAppearance.FlatButtons;
            priceTabControl.ItemSize = new Size(0, 1);
            priceTabControl.SizeMode = TabSizeMode.Fixed;
            foreach (TabPage tab in priceTabControl.TabPages)
            {
                tab.Text = "";
            }
            price_Category_cbx.SelectedIndex = 0;

            //Initialize Summary BOQ table
            initializeSummaryTable();

            //Initialize View and BOQ additional rows
            initializeView();

            //Initialize Help
            initializeHelpTree();
        }
        //General Functions -- END

        //Home Functions -- START
        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            await help_webView.EnsureCoreWebView2Async(null);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //File tab button is clicked
            if (e.TabPageIndex == 0)
            {
                e.Cancel = true;
                fileMenu.Show(tabControl1, new Point(0, tabControl1.ItemSize.Height));
            }
            //Price Tab button is clicked
            else if (e.TabPageIndex == 2)
            {
                AdjustPriceView();

                //Console.WriteLine(structuralMembers.concreteWorkSolutionsC[0][0][0]);
            }
            //View or BOQ Tab button is clicked
            else if (e.TabPageIndex == 3 || e.TabPageIndex == 4)
            {
                initializeView();
                
                //Console.WriteLine(structuralMembers.concreteWorkSolutionsC[0][0][0]);
            }
        }

        private void tabControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle mouseRect = new Rectangle(e.X, e.Y, 1, 1);
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.GetTabRect(i).IntersectsWith(mouseRect))
                {
                    switch (i)
                    {
                        case 0: //File
                            break;
                        case 1: //Home
                            toolTip1.Show("The main window page for adding structural members and setting the parameters.", tabControl1); break;
                        case 2: //Price
                            toolTip1.Show("Contains all the materials, labor, tools, and equipment included in KnowEst. Edit and customize prices according to preference.", tabControl1); break;
                        case 3: //View
                            toolTip1.Show("Contains the pricing summary of each inputted work according to structural members.", tabControl1); break;
                        case 4: //BOQ
                             break;
                        case 5: //Help
                            toolTip1.Show(" Contains helpful guides, tutorial and information in understanding cost estimation and operating the program KnowEst.", tabControl1); break;
                    }
                    break;
                }
            }
        }

        private void paraBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Adjust and customize values of structural members, add, and include other material add-ons.", paraBtn); 
        }

        private void addFloorBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Adds another floor containing structural members.", addFloorBtn);
        }

        private void totalLbl_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Shows the overall complete total cost of the whole estimation.", totalLbl);
        }

        private void totalcostLbl_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Shows the overall complete total cost of the whole estimation.", totalcostLbl);
        }

        private void price_SettingsBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Configure or customize default values for the factor of safety and labor", price_SettingsBtn);
        }

        private void price_RestoreDefaultsBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Returns all materials to its original default price.", price_RestoreDefaultsBtn);
        }

        private void view_ConfigureBtn_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Choose what to include in the total estimated cost. Check to include and uncheck to exclude.", view_ConfigureBtn);
        }

        //--------
        private void tabControl1_MouseHover(object sender, EventArgs e)
        {
            //
        }

        //File Menu - New
        private void fileMenu1_Click(object sender, EventArgs e)
        {
            new CostEstimationForm().Show();
        }

        //File Menu - Open
        private void fileMenu2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Open Show Estimation File";
            openDialog.Filter = "Know Estimation files (*.est)|*.est";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                //try
                //{
                    if ((myStream = openDialog.OpenFile()) != null)
                    {
                        fileName = openDialog.FileName;
                        this.Text = "Know Estimation - " + Path.GetFileName(fileName);
                        saveFileExists = true;
                        using (myStream)
                        {
                            StreamReader reader = new StreamReader(myStream);
                            string stringFile = reader.ReadToEnd();
                            SaveToProgram(stringFile);
                        }
                    }
                /*}
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }*/
            }
            viewInitalized = false;
            initializeView();
        }

        //Save
        private void fileMenu3_Click(object sender, EventArgs e)
        {
            if(fileName == null)
            {
                saveAs();
            }
            else
            {
                SaveToFile(fileName);
            }
        }

        //File Menu - Save As
        private void fileMenu4_Click(object sender, EventArgs e)
        {
            saveAs();
        }

        private void saveAs()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "New Estimate.est";
            saveDialog.Filter = "KnowEst files (*.est)|*.est";
            DialogResult result = saveDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                fileName = saveDialog.FileName;
                this.Text = "Know Estimation - " + Path.GetFileName(fileName);
                saveFileExists = true;
                SaveToFile(fileName);
            }
        }

        //File Menu - Exit
        private void fileMenu5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //File Menu - Import
        private void fileMenu6_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { ValidateNames = true, Multiselect = false, Filter = "PDF|*.pdf" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //webBrowser1.Navigate(ofd.fileName);
                    webView.CoreWebView2.Navigate("file:///" + ofd.FileName);
                }
            }
        }

        private void addFloorBtn_Click(object sender, EventArgs e)
        {
            addFloor();
        }

        private void addFloor()
        {
            initializeVariables();

            //Set a ground floor
            Floor floor = new Floor(this, false);
            floors.Add(floor);
            estimationPanel.Controls.Add(floor);
        }

        private void initializeVariables()
        {
            //Initialize variables in Structural Members for every floor created
            if (Floors.Count == 0)//Ground Floor
            {
                //Footings
                List<List<string>> newList = new List<List<string>>();
                List<List<string>> newList2 = new List<List<string>>();
                structuralMembers.footingsColumn.Add(newList);
                structuralMembers.footingsWall.Add(newList2);

                //Columns
                List<string> newList3 = new List<string>();
                List<List<string>> newList4 = new List<List<string>>();
                List<List<string>> newList5 = new List<List<string>>();
                List<List<string>> newList6 = new List<List<string>>();
                structuralMembers.columnNames.Add(newList3);
                structuralMembers.column.Add(newList4);
                structuralMembers.columnLateralTies.Add(newList5);
                structuralMembers.columnSpacing.Add(newList6);

                //Beams
                List<string> newList7 = new List<string>();
                List<List<string>> newList8 = new List<List<string>>();
                List<List<List<string>>> newList9 = new List<List<List<string>>>();
                List<List<string>> newList10 = new List<List<string>>();
                structuralMembers.beamNames.Add(newList7);
                structuralMembers.beam.Add(newList8);
                structuralMembers.beamRow.Add(newList9);
                structuralMembers.beamSchedule.Add(newList10);

                //Slabs
                List<string> newList11 = new List<string>();
                List<List<string>> newList12 = new List<List<string>>();
                structuralMembers.slabNames.Add(newList11);
                structuralMembers.slab.Add(newList12);

                //Stairs
                List<List<string>> newList14 = new List<List<string>>();
                List<StairParameterUserControl> stairParamNewList = new List<StairParameterUserControl>();
                List<string> newList15 = new List<string>();
                structuralMembers.stairs.Add(newList14);
                structuralMembers.stairsNames.Add(newList15);
                parameters.stair.Add(stairParamNewList);

                //Roof
                List<List<string>> newList16 = new List<List<string>>();
                List<List<string>> newList17 = new List<List<string>>();
                List<string> newList18 = new List<string>();
                structuralMembers.roof.Add(newList16);
                structuralMembers.roofHRS.Add(newList17);
                structuralMembers.roofNames.Add(newList18);

                //Solution Variables
                List<List<double>> newList19 = new List<List<double>>();
                List<List<double>> newList20 = new List<List<double>>();
                List<List<double>> newList21 = new List<List<double>>();
                List<string> newList22 = new List<string>();
                List<List<double>> newList23 = new List<List<double>>();
                List<List<double[,]>> newList24 = new List<List<double[,]>>();
                
                structuralMembers.concreteWorkSolutionsC.Add(newList19);
                structuralMembers.concreteWorkSolutionsBR.Add(newList20);
                structuralMembers.concreteWorkSolutionsSL.Add(newList21);
                structuralMembers.concreteWorkSolutionsSLSM.Add(newList22);
                structuralMembers.concreteWorkSolutionsST.Add(newList23);
                structuralMembers.stairs_Rebar.Add(newList24);
            }
            else //Upper Floors
            {
                //Columns
                List<string> newList3 = new List<string>();
                List<List<string>> newList4 = new List<List<string>>();
                List<List<string>> newList5 = new List<List<string>>();
                List<List<string>> newList6 = new List<List<string>>();
                structuralMembers.columnNames.Add(newList3);
                structuralMembers.column.Add(newList4);
                structuralMembers.columnLateralTies.Add(newList5);
                structuralMembers.columnSpacing.Add(newList6);

                //Beams
                List<string> newList7 = new List<string>();
                List<List<string>> newList8 = new List<List<string>>();
                List<List<List<string>>> newList9 = new List<List<List<string>>>();
                List<List<string>> newList10 = new List<List<string>>();
                structuralMembers.beamNames.Add(newList7);
                structuralMembers.beam.Add(newList8);
                structuralMembers.beamRow.Add(newList9);
                structuralMembers.beamSchedule.Add(newList10);

                //Slabs
                List<string> newList11 = new List<string>();
                List<List<string>> newList12 = new List<List<string>>();
                List<List<string>> newList13 = new List<List<string>>();
                structuralMembers.slabNames.Add(newList11);
                structuralMembers.slab.Add(newList12);
                structuralMembers.slabSchedule.Add(newList13);

                //Stairs
                List<List<string>> newList14 = new List<List<string>>();
                List<StairParameterUserControl> stairParamNewList = new List<StairParameterUserControl>();
                List<string> newList15 = new List<string>();
                structuralMembers.stairs.Add(newList14);
                structuralMembers.stairsNames.Add(newList15);
                parameters.stair.Add(stairParamNewList);

                //Roof
                List<List<string>> newList16 = new List<List<string>>();
                List<List<string>> newList17 = new List<List<string>>();
                List<string> newList18 = new List<string>();
                structuralMembers.roof.Add(newList16);
                structuralMembers.roofHRS.Add(newList17);
                structuralMembers.roofNames.Add(newList18);

                //Solution Variables
                List<List<double>> newList19 = new List<List<double>>();
                List<List<double>> newList20 = new List<List<double>>();
                List<List<double>> newList21 = new List<List<double>>();
                List<string> newList22 = new List<string>();
                List<List<double>> newList23 = new List<List<double>>();
                List<List<double[,]>> newList24 = new List<List<double[,]>>();                
                structuralMembers.concreteWorkSolutionsC.Add(newList19);
                structuralMembers.concreteWorkSolutionsBR.Add(newList20);
                structuralMembers.concreteWorkSolutionsSL.Add(newList21);
                structuralMembers.concreteWorkSolutionsSLSM.Add(newList22);
                structuralMembers.concreteWorkSolutionsST.Add(newList23);
                structuralMembers.stairs_Rebar.Add(newList24);
            }
            
        }

        public void refreshFloors()
        {
            //Remove all controls
            estimationPanel.Controls.Clear();

            //Add all controls
            for (int i = 0; i < Floors.Count; i++)
            {
                estimationPanel.Controls.Add(Floors[i]);
                
                if (i == 0)
                {
                    Floors[i].setLabel = "GROUND FLOOR";
                }
                else
                {
                    string label = Floors[i].getValues()[1];
                    if (label.Equals(AddOrdinal(i + 1) + " FLOOR"))
                    {
                        Floors[i].setLabel = AddOrdinal(i) + " FLOOR";
                    }
                    else
                    {
                        Floors[i].setLabel = label;
                    }
                }
            }
        }

        private void paraBtn_Click(object sender, EventArgs e)
        {
            try
            {
                pf.setStairsValues();
                pf.ShowDialog();
            }
            catch (Exception ex)
            {
                print("Catched: " + ex);
            }
        }

        //Home Functions -- END

        //View Functions -- START
        private void initializeView()
        {
            compute.recomputeFW_Footings(this);
            compute.recomputeFW_Column(this);
            compute.recomputeFW_Beam(this);
            compute.recomputeFW_slabs(this);
            compute.recomputeFW_stairs(this);
            //Cost Computation - START

            //Earthworks
            if (earthworksChecklist[1])
                excavation_CostL = Math.Round(excavation_Total, 2) * double.Parse(parameters.price_LaborRate_Earthworks["Excavation [m3]"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            else
                excavation_CostL = 0;
            if (structuralMembers.extraEarthworkSolutions[3] < structuralMembers.extraEarthworkSolutions[2])
                backfillingAndCompaction_CostM = Math.Round((structuralMembers.extraEarthworkSolutions[2] - structuralMembers.extraEarthworkSolutions[3]), 2) * double.Parse(parameters.price_Embankment["Common Borrow [m3]"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            else
                backfillingAndCompaction_CostM = 0;
                backfillingAndCompaction_CostL = Math.Round(backfillingAndCompaction_Total, 2) * double.Parse(parameters.price_LaborRate_Earthworks["Backfilling and Compaction [m3]"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            if (earthworksChecklist[2])
                backfillingAndCompaction_CostTotal = backfillingAndCompaction_CostL + backfillingAndCompaction_CostM;
            else
                backfillingAndCompaction_CostTotal = 0;
            if (earthworksChecklist[3])
                gradingAndCompaction_CostL = Math.Round(gradingAndCompaction_Total, 2) * double.Parse(parameters.price_LaborRate_Earthworks["Grading and Compaction [m3]"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            else
                gradingAndCompaction_CostL = 0;
            string earthworks_gravelBeddingType = parameters.earth_CF_TY;
            gravelBedding_CostM = Math.Round(gravelBedding_Total, 2) * double.Parse(parameters.price_Gravel[earthworks_gravelBeddingType].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            gravelBedding_CostL = Math.Round(gravelBedding_Total, 2) * double.Parse(parameters.price_LaborRate_Earthworks["Gravel Bedding and Compaction [m3]"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            if (earthworksChecklist[4])
                gravelBedding_CostTotal = gravelBedding_CostM + gravelBedding_CostL;
            else
                gravelBedding_CostTotal = 0;
            if (earthworksChecklist[5])
                soilPoisoning_CostM = Math.Round(soilPoisoning_Total, 2) * double.Parse(parameters.price_LaborRate_Earthworks["Soil Poisoning [m2]"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            else
                soilPoisoning_CostM = 0;
            earthworks_CostTotal = excavation_CostL + backfillingAndCompaction_CostL + gradingAndCompaction_CostL +
                                   gravelBedding_CostTotal + soilPoisoning_CostM;

            //Concreting
            //Refresh Totalities and Cost to 0
            for (int i = 0; i < cement_Total.Length; i++)
            {
                cement_Total[i] = 0;
                sand_Total[i] = 0;
                gravel_Total[i] = 0;
                concreteVolume_Total[i] = 0;
            }
            concreteF_UnitM = 0;
            concreteF_CostM = 0;
            concreteF_CostL = 0;
            concreteF_costTotal = 0;

            concreteC_UnitM = 0;
            concreteC_CostM = 0;
            concreteC_CostL = 0;
            concreteC_costTotal = 0;

            concreteBR_UnitM = 0;
            concreteBR_CostM = 0;
            concreteBR_CostL = 0;
            concreteBR_costTotal = 0;

            concreteSOG_UnitM = 0;
            concreteSOG_CostM = 0;
            concreteSOG_CostL = 0;
            concreteSOG_costTotal = 0;

            concreteSS_UnitM = 0;
            concreteSS_CostM = 0;
            concreteSS_CostL = 0;
            concreteSS_costTotal = 0;

            concreteST_UnitM = 0;
            concreteST_CostM = 0;
            concreteST_CostL = 0;
            concreteST_costTotal = 0;

            concreteFS_CostM = 0;
            concreteFS_costTotal = 0;

            concreteMCost_total = 0;
            concreteLCost_total = 0;
            concrete_TOTALCOST = 0;

            string value = fos_Cement.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            double cementFS = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
            value = fos_Sand.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            double sandFS = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
            value = fos_Gravel.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            double gravelFS = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;

            if (concreteChecklist[0]) //2.1 Footing
            {
                if (parameters.conc_cmIsSelected[0]) //Not Ready Mix
                {
                    foreach (List<double> solution in structuralMembers.concreteWorkSolutionsF)
                    {
                        cement_Total[0] += solution[1];
                        sand_Total[0] += solution[2];
                        gravel_Total[0] += solution[3];
                        concreteVolume_Total[0] += solution[4];
                    }
                    concreteVolume_Total[0] = Math.Round(concreteVolume_Total[0], 2);
                    //Cost
                    double cementCost = cement_Total[0] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString());
                    double sandCost = sand_Total[0] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString());
                    double gravelCost = gravel_Total[0] * double.Parse(parameters.price_Gravel[parameters.conc_CM_F_GT].ToString());
                    concreteFS_CostM = (cementCost * cementFS) + (sandCost * sandFS) + (gravelCost * gravelFS);
                    concreteF_UnitM = cementCost + sandCost + gravelCost;
                    concreteF_CostM = concreteVolume_Total[0] * concreteF_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["FOOTING [m3]"].ToString());
                    concreteF_CostL = concreteVolume_Total[0] * laborRate;
                    concreteF_costTotal = concreteF_CostM + concreteF_CostL;
                }
                else //Ready Mix
                {
                    foreach (List<double> solution in structuralMembers.concreteWorkSolutionsF)
                    {
                        concreteVolume_Total[0] += solution[1];
                    }
                    concreteVolume_Total[0] = Math.Round(concreteVolume_Total[0], 2);
                    //Cost
                    concreteF_UnitM = double.Parse(parameters.price_ReadyMixConcrete[parameters.conc_CM_F_RM + " [m3]"].ToString());
                    concreteF_CostM = concreteVolume_Total[0] * concreteF_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["FOOTING [m3]"].ToString());
                    concreteF_CostL = concreteVolume_Total[0] * laborRate;
                    concreteF_costTotal = concreteF_CostM + concreteF_CostL;
                }
            }
            if (concreteChecklist[1]) //2.2 Columns
            {
                if (parameters.conc_cmIsSelected[1]) //Not Ready Mix
                {
                    int i = 0;
                    foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsC)
                    {
                        int floorMultiplier = int.Parse(floors[i].getValues()[0]);
                        foreach (List<double> solution in floor)
                        {
                            cement_Total[1] += solution[0] * floorMultiplier;
                            sand_Total[1] += solution[1] * floorMultiplier;
                            gravel_Total[1] += solution[2] * floorMultiplier;
                            concreteVolume_Total[1] += solution[3] * floorMultiplier;
                        }
                        i++;
                    }
                    concreteVolume_Total[1] = Math.Round(concreteVolume_Total[1], 2);
                    //Cost
                    double cementCost = cement_Total[0] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString());
                    double sandCost = sand_Total[0] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString());
                    double gravelCost = gravel_Total[0] * double.Parse(parameters.price_Gravel[parameters.conc_CM_C_GT].ToString());
                    concreteFS_CostM = (cementCost * cementFS) + (sandCost * sandFS) + (gravelCost * gravelFS);
                    concreteC_UnitM = cementCost + sandCost + gravelCost;
                    concreteC_CostM = concreteVolume_Total[1] * concreteC_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["COLUMN [m3]"].ToString());
                    concreteC_CostL = concreteVolume_Total[1] * laborRate;
                    concreteC_costTotal = concreteC_CostM + concreteC_CostL;
                }
                else //Ready Mix
                {
                    int i = 0;
                    foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsC)
                    {
                        int floorMultiplier = int.Parse(floors[i].getValues()[0]);
                        foreach (List<double> solution in floor)
                        {
                            concreteVolume_Total[1] += solution[0] * floorMultiplier;
                        }
                        i++;
                    }
                    concreteVolume_Total[1] = Math.Round(concreteVolume_Total[1], 2);
                    //Cost
                    concreteC_UnitM = double.Parse(parameters.price_ReadyMixConcrete[parameters.conc_CM_C_RM + " [m3]"].ToString());
                    concreteC_CostM = concreteVolume_Total[1] * concreteC_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["COLUMN [m3]"].ToString());
                    concreteC_CostL = concreteVolume_Total[1] * laborRate;
                    concreteC_costTotal = concreteC_CostM + concreteC_CostL;
                }
            }
            if (concreteChecklist[2]) //2.3 Beams
            {
                if (parameters.conc_cmIsSelected[2]) //Not Ready Mix
                {
                    int i = 0;
                    foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsBR)
                    {
                        int floorMultiplier = int.Parse(floors[i].getValues()[0]);
                        foreach (List<double> solution in floor)
                        {
                            cement_Total[2] += solution[0] * floorMultiplier;
                            sand_Total[2] += solution[1] * floorMultiplier;
                            gravel_Total[2] += solution[2] * floorMultiplier;
                            concreteVolume_Total[2] += solution[3] * floorMultiplier;
                        }
                        i++;
                    }
                    concreteVolume_Total[2] = Math.Round(concreteVolume_Total[2], 2);
                    //Cost
                    double cementCost = cement_Total[0] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString());
                    double sandCost = sand_Total[0] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString());
                    double gravelCost = gravel_Total[0] * double.Parse(parameters.price_Gravel[parameters.conc_CM_B_GT].ToString());
                    concreteFS_CostM = (cementCost * cementFS) + (sandCost * sandFS) + (gravelCost * gravelFS);
                    concreteBR_UnitM = cementCost + sandCost + gravelCost;
                    concreteBR_CostM = concreteVolume_Total[2] * concreteBR_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["BEAM [m3]"].ToString());
                    concreteBR_CostL = concreteVolume_Total[2] * laborRate;
                    concreteBR_costTotal = concreteBR_CostM + concreteBR_CostL;
                }
                else //Ready Mix
                {
                    int i = 0;
                    foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsBR)
                    {
                        int floorMultiplier = int.Parse(floors[i].getValues()[0]);
                        foreach (List<double> solution in floor)
                        {
                            concreteVolume_Total[2] += solution[0] * floorMultiplier;
                        }
                        i++;
                    }
                    concreteVolume_Total[2] = Math.Round(concreteVolume_Total[2], 2);
                    //Cost
                    concreteBR_UnitM = double.Parse(parameters.price_ReadyMixConcrete[parameters.conc_CM_B_RM + " [m3]"].ToString());
                    concreteBR_CostM = concreteVolume_Total[2] * concreteBR_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["BEAM [m3]"].ToString());
                    concreteBR_CostL = concreteVolume_Total[2] * laborRate;
                    concreteBR_costTotal = concreteBR_CostM + concreteBR_CostL;
                }
            }
            if (concreteChecklist[3]) //2.4 Slab
            {
                int i = 0;
                foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsSL)
                {
                    int floorMultiplier = int.Parse(floors[i].getValues()[0]);
                    if (i == 0) // SOG
                    {
                        if (parameters.conc_cmIsSelected[3]) //Not Ready Mix
                        {
                            foreach (List<double> solution in floor)
                            {
                                cement_Total[3] += solution[0] * floorMultiplier;
                                sand_Total[3] += solution[1] * floorMultiplier;
                                gravel_Total[3] += solution[2] * floorMultiplier;
                                concreteVolume_Total[3] += solution[3] * floorMultiplier;
                            }
                            concreteVolume_Total[3] = Math.Round(concreteVolume_Total[3], 2);
                            //Cost
                            double cementCost = cement_Total[0] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString());
                            double sandCost = sand_Total[0] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString());
                            double gravelCost = gravel_Total[0] * double.Parse(parameters.price_Gravel[parameters.conc_CM_S_SOG_GT].ToString());
                            concreteFS_CostM = (cementCost * cementFS) + (sandCost * sandFS) + (gravelCost * gravelFS);
                            concreteSOG_UnitM = cementCost + sandCost + gravelCost;
                            concreteSOG_CostM = concreteVolume_Total[3] * concreteSOG_UnitM;
                            double laborRate = double.Parse(parameters.price_LaborRate_Concreting["SLAB ON GRADE [m3]"].ToString());
                            concreteSOG_CostL = concreteVolume_Total[3] * laborRate;
                            concreteSOG_costTotal = concreteSOG_CostM + concreteSOG_CostL;
                        }
                        else // Ready Mix
                        {
                            foreach (List<double> solution in floor)
                            {
                                concreteVolume_Total[3] += solution[0] * floorMultiplier;
                            }
                            concreteVolume_Total[3] = Math.Round(concreteVolume_Total[3], 2);
                            //Cost
                            concreteSOG_UnitM = double.Parse(parameters.price_ReadyMixConcrete[parameters.conc_CM_S_SOG_RM + " [m3]"].ToString());
                            concreteSOG_CostM = concreteVolume_Total[3] * concreteSOG_UnitM;
                            double laborRate = double.Parse(parameters.price_LaborRate_Concreting["SLAB ON GRADE [m3]"].ToString());
                            concreteSOG_CostL = concreteVolume_Total[3] * laborRate;
                            concreteSOG_costTotal = concreteSOG_CostM + concreteSOG_CostL;
                        }
                    }
                    else // SS
                    {
                        if (parameters.conc_cmIsSelected[4]) //Not Ready Mix
                        {
                            foreach (List<double> solution in floor)
                            {
                                cement_Total[4] += solution[0] * floorMultiplier;
                                sand_Total[4] += solution[1] * floorMultiplier;
                                gravel_Total[4] += solution[2] * floorMultiplier;
                                concreteVolume_Total[4] += solution[3] * floorMultiplier;
                            }
                            concreteVolume_Total[4] = Math.Round(concreteVolume_Total[4], 2);
                            //Cost
                            double cementCost = cement_Total[0] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString());
                            double sandCost = sand_Total[0] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString());
                            double gravelCost = gravel_Total[0] * double.Parse(parameters.price_Gravel[parameters.conc_CM_S_SS_GT].ToString());
                            concreteFS_CostM = (cementCost * cementFS) + (sandCost * sandFS) + (gravelCost * gravelFS);
                            concreteSS_UnitM = cementCost + sandCost + gravelCost;
                            concreteSS_CostM = concreteVolume_Total[4] * concreteSS_UnitM;
                            double laborRate = double.Parse(parameters.price_LaborRate_Concreting["SUSPENDED SLAB [m3]"].ToString());
                            concreteSS_CostL = concreteVolume_Total[4] * laborRate;
                            concreteSS_costTotal = concreteSS_CostM + concreteSS_CostL;
                        }
                        else // Ready Mix
                        {
                            foreach (List<double> solution in floor)
                            {
                                concreteVolume_Total[4] += solution[0] * floorMultiplier;
                            }
                            concreteVolume_Total[4] = Math.Round(concreteVolume_Total[4], 2);
                            //Cost
                            concreteSS_UnitM = double.Parse(parameters.price_ReadyMixConcrete[parameters.conc_CM_S_SS_RM + " [m3]"].ToString());
                            concreteSS_CostM = concreteVolume_Total[4] * concreteSS_UnitM;
                            double laborRate = double.Parse(parameters.price_LaborRate_Concreting["SUSPENDED SLAB [m3]"].ToString());
                            concreteSS_CostL = concreteVolume_Total[4] * laborRate;
                            concreteSS_costTotal = concreteSS_CostM + concreteSS_CostL;
                        }
                    }
                    i++;
                }
            }
            if (concreteChecklist[4]) //2.5 Stairs
            {
                if (parameters.conc_cmIsSelected[5]) //Not Ready Mix
                {
                    int i = 0;
                    foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsST)
                    {
                        int floorMultiplier = int.Parse(floors[i].getValues()[0]);
                        foreach (List<double> solution in floor)
                        {
                            cement_Total[5] += solution[0] * floorMultiplier;
                            sand_Total[5] += solution[1] * floorMultiplier;
                            gravel_Total[5] += solution[2] * floorMultiplier;
                            concreteVolume_Total[5] += solution[3] * floorMultiplier;
                        }
                        i++;
                    }
                    concreteVolume_Total[5] = Math.Round(concreteVolume_Total[5], 2);
                    //Cost
                    double cementCost = cement_Total[0] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString());
                    double sandCost = sand_Total[0] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString());
                    double gravelCost = gravel_Total[0] * double.Parse(parameters.price_Gravel[parameters.conc_CM_ST_GT].ToString());
                    concreteFS_CostM = (cementCost * cementFS) + (sandCost * sandFS) + (gravelCost * gravelFS);
                    concreteST_UnitM = cementCost + sandCost + gravelCost;
                    concreteST_CostM = concreteVolume_Total[5] * concreteST_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["STAIRS [m3]"].ToString());
                    concreteST_CostL = concreteVolume_Total[5] * laborRate;
                    concreteST_costTotal = concreteST_CostM + concreteST_CostL;
                }
                else //Ready Mix
                {
                    int i = 0;
                    foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsST)
                    {
                        int floorMultiplier = int.Parse(floors[i].getValues()[0]);
                        foreach (List<double> solution in floor)
                        {
                            concreteVolume_Total[5] += solution[0] * floorMultiplier;
                        }
                        i++;
                    }
                    concreteVolume_Total[5] = Math.Round(concreteVolume_Total[5], 2);
                    //Cost
                    concreteST_UnitM = double.Parse(parameters.price_ReadyMixConcrete[parameters.conc_CM_ST_RM + " [m3]"].ToString());
                    concreteST_CostM = concreteVolume_Total[5] * concreteST_UnitM;
                    double laborRate = double.Parse(parameters.price_LaborRate_Concreting["STAIRS [m3]"].ToString());
                    concreteST_CostL = concreteVolume_Total[5] * laborRate;
                    concreteST_costTotal = concreteST_CostM + concreteST_CostL;
                }
            }
            if (concreteChecklist[5]) //2.6 Factor of Safety
            {
                value = fos_RMC.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                double readyMixFS = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) / 100;
                if (!parameters.conc_cmIsSelected[0]) //Ready Mix, Footing
                {
                    concreteFS_CostM = concreteF_CostM * readyMixFS;
                }
                if (!parameters.conc_cmIsSelected[1]) //Ready Mix, Column
                {
                    concreteFS_CostM += concreteC_CostM * readyMixFS;
                }
                if (!parameters.conc_cmIsSelected[2]) //Ready Mix, Beam
                {
                    concreteFS_CostM += concreteBR_CostM * readyMixFS;
                }
                if (!parameters.conc_cmIsSelected[3]) //Ready Mix, Slab on Grade
                {
                    concreteFS_CostM += concreteSOG_CostM * readyMixFS;
                }
                if (!parameters.conc_cmIsSelected[4]) //Ready Mix, Suspended Slab
                {
                    concreteFS_CostM += concreteSS_CostM * readyMixFS;
                }
                if (!parameters.conc_cmIsSelected[5]) //Ready Mix, Stairs
                {
                    concreteFS_CostM += concreteST_CostM * readyMixFS;
                }
                concreteFS_costTotal = concreteFS_CostM;
            }
            concreteMCost_total = concreteF_CostM + concreteC_CostM + concreteBR_CostM +
                                  concreteSOG_CostM + concreteSS_CostM + concreteST_CostM + concreteFS_CostM;
            concreteLCost_total = concreteF_CostL + concreteC_CostL + concreteBR_CostL +
                                  concreteSOG_CostL + concreteSS_CostL + concreteST_CostL;
            concrete_TOTALCOST = concreteMCost_total + concreteLCost_total;

            print("\n====== Concreting ======");
            print("Footing Total: " + concreteVolume_Total[0]);
            print("Column Total: " + concreteVolume_Total[1]);
            print("Beams Total: " + concreteVolume_Total[2]);
            print("SOG Slab Total: " + concreteVolume_Total[3]);
            print("SS Slab Total: " + concreteVolume_Total[4]);
            print("Stairs Total: " + concreteVolume_Total[5]);
            print("\n====== COST Concreting ======");
            print("=== Footing Cost ===");
            print("Material Unit Cost: " + concreteF_UnitM);
            print("Material Total Cost: " + concreteF_CostM);
            print("Labor Total Cost: " + concreteF_CostL);
            print("Total Cost: " + concreteF_costTotal);
            print("=== Column Cost ===");
            print("Material Unit Cost: " + concreteC_UnitM);
            print("Material Total Cost: " + concreteC_CostM);
            print("Labor Total Cost: " + concreteC_CostL);
            print("Total Cost: " + concreteC_costTotal);
            print("=== Beam Cost ===");
            print("Material Unit Cost: " + concreteBR_UnitM);
            print("Material Total Cost: " + concreteBR_CostM);
            print("Labor Total Cost: " + concreteBR_CostL);
            print("Total Cost: " + concreteBR_costTotal);
            print("=== Slab Cost ===");
            print("SOG Material Unit Cost: " + concreteSOG_UnitM);
            print("SOG Material Total Cost: " + concreteSOG_CostM);
            print("SOG Labor Total Cost: " + concreteSOG_CostL);
            print("Total Cost: " + concreteSOG_costTotal);
            print("SS Material Unit Cost: " + concreteSS_UnitM);
            print("SS Material Total Cost: " + concreteSS_CostM);
            print("SS Labor Total Cost: " + concreteSS_CostL);
            print("Total Cost: " + concreteSS_costTotal);
            print("=== Stairs Cost ===");
            print("Material Unit Cost: " + concreteST_UnitM);
            print("Material Total Cost: " + concreteST_CostM);
            print("Labor Total Cost: " + concreteST_CostL);
            print("Total Cost: " + concreteST_costTotal);
            print("=== Factor of Safety Cost ===");
            print("Material Total Cost: " + concreteFS_CostM);
            print("Total Cost: " + concreteFS_costTotal);
            print("=== Total Cost ===");
            print("Material Total Cost: " + concreteMCost_total);
            print("Labor Total Cost: " + concreteLCost_total);
            print("Total Cost: " + concrete_TOTALCOST);
            //Framework **

            if (formworksChecklist[0])//footing
            {
                try
                {
                    List<string> lumberdim = dimensionFilterer(parameters.form_SM_F_FL);
                    string strLumber = lumberPriceHelper(lumberdim);
                    string plyType = plywoodHelper(parameters.form_F_T);
                    footing_Mcost = (structuralMembers.footings_comps[0] * double.Parse(parameters.price_FormworksAndLumber[plyType].ToString()))
                        + (structuralMembers.footings_comps[1] * double.Parse(parameters.price_FormworksAndLumber[strLumber].ToString()));
                    double footarea = 0;
                    foreach(var a in structuralMembers.per_col)
                    {
                        footarea += a;
                    }
                    footing_Munit = Math.Round((footing_Mcost / footarea),2);
                    footing_Lcost = footarea * double.Parse(parameters.price_LaborRate_Formworks["FOOTING [m2]"].ToString());
                    footing_Lunit = double.Parse(parameters.price_LaborRate_Formworks["FOOTING [m2]"].ToString());
                    footingMLCOST = footing_Lcost + footing_Mcost;
                    footing_QTY = footarea;
                    trap_Mcost = (structuralMembers.footings_comps[2] * double.Parse(parameters.price_FormworksAndLumber[plyType].ToString()));
                    double wallarea = 0;
                    foreach(var a in structuralMembers.per_wal)
                    {
                        wallarea += a;
                    }
                    trap_Munit = Math.Round((trap_Mcost / wallarea), 2);
                    trap_Lcost = Math.Round((wallarea * double.Parse(parameters.price_LaborRate_Formworks["WALL FOOTING [m2]"].ToString())),2);
                    trap_Lunit = double.Parse(parameters.price_LaborRate_Formworks["WALL FOOTING [m2]"].ToString());
                    trapMLCOST = trap_Lcost + trap_Mcost;
                    trap_QTY = wallarea;
                }
                catch{
                    footing_Munit = 0;
                    footing_Mcost = 0;
                    footing_Lunit = 0;
                    footing_Lcost = 0;
                    footingMLCOST = 0;
                    footing_QTY = 0;
                    trap_Munit = 0;
                    trap_Mcost = 0;
                    trap_Lunit = 0;
                    trap_Lcost = 0;
                    trapMLCOST = 0;
                    trap_QTY = 0;
                }                
            }
            else
            {
                footing_QTY = 0;
                footing_Munit = 0;
                footing_Mcost = 0;
                footing_Lunit = 0;
                footing_Lcost = 0;
                footingMLCOST = 0;                
                trap_Munit = 0;
                trap_Mcost = 0;
                trap_Lunit = 0;
                trap_Lcost = 0;
                trapMLCOST = 0;
                trap_QTY = 0;
            }
            if (formworksChecklist[1])//column
            {

                try
                {
                    double totalArea = 0;
                    double formworkWood = 0;
                    double frameworkWood = 0;
                    double Vwood = 0;
                    double Hwood = 0;
                    double Dwood = 0;
                    string plyType = plywoodHelper(parameters.form_F_T);
                    List<string> lumberdim = dimensionFilterer(parameters.form_SM_B_FL);
                    List<string> lumberVert = dimensionFilterer(parameters.form_SM_B_VS);
                    List<string> lumberHori = dimensionFilterer(parameters.form_SM_B_HB);
                    List<string> lumberDiag = dimensionFilterer(parameters.form_SM_B_DB);                    
                    string strLumber = lumberPriceHelper(lumberdim);   
                    string strVert = lumberPriceHelper(lumberVert);
                    string strHori = lumberPriceHelper(lumberHori);
                    string strDiag = lumberPriceHelper(lumberDiag);
                    for (int i = 0; i < Floors.Count; i++)
                    {
                        totalArea += structuralMembers.col_area[i] * double.Parse(Floors[i].getValues()[0]);
                        formworkWood += structuralMembers.col_woods[i] * double.Parse(Floors[i].getValues()[0]);
                        frameworkWood += structuralMembers.col_post[i] * double.Parse(Floors[i].getValues()[0]);
                        Vwood += structuralMembers.col_scafV[i] * double.Parse(Floors[i].getValues()[0]);
                        Hwood += structuralMembers.col_scafH[i] * double.Parse(Floors[i].getValues()[0]);
                        Dwood += structuralMembers.col_scafD[i] * double.Parse(Floors[i].getValues()[0]);
                    }
                    col_QTY = Math.Round(totalArea,2);
                    col_Mcost = (formworkWood * double.Parse(parameters.price_FormworksAndLumber[plyType].ToString()))
                        + (frameworkWood * double.Parse(parameters.price_FormworksAndLumber[strLumber].ToString()))
                        + (Vwood * double.Parse(parameters.price_FormworksAndLumber[strVert].ToString()))
                        + (Hwood * double.Parse(parameters.price_FormworksAndLumber[strHori].ToString()))
                        + (Dwood * double.Parse(parameters.price_FormworksAndLumber[strDiag].ToString()));
                    col_Munit = Math.Round((col_Mcost / totalArea),2);
                    col_Lcost = totalArea * double.Parse(parameters.price_LaborRate_Formworks["COLUMN [m2]"].ToString());
                    col_Lunit = double.Parse(parameters.price_LaborRate_Formworks["COLUMN [m2]"].ToString());
                    colMLCOST = col_Mcost + col_Lcost;
                }
                catch
                {
                    col_Munit = 0;
                    col_Mcost = 0;
                    col_Lunit = 0;
                    col_Lcost = 0;
                    colMLCOST = 0;
                    col_QTY = 0;
                }
            }
            else
            {
                col_Munit = 0;
                col_Mcost = 0;
                col_Lunit = 0;
                col_Lcost = 0;
                colMLCOST = 0;
                col_QTY = 0;
            }
            if (formworksChecklist[2])//beams
            {
                try
                {
                    string plyType = plywoodHelper(parameters.form_F_T);
                    List<string> lumberdim = dimensionFilterer(parameters.form_SM_C_FL);
                    List<string> lumberVert = dimensionFilterer(parameters.form_SM_C_VS);
                    List<string> lumberHori = dimensionFilterer(parameters.form_SM_C_HB);
                    string strLumber = lumberPriceHelper(lumberdim);
                    string strVert = lumberPriceHelper(lumberVert);
                    string strHori = lumberPriceHelper(lumberHori);
                    double area = 0;
                    double form = 0;
                    double frame = 0;
                    double vertical = 0;
                    double horizontal = 0;
                    for (int i = 0; i < Floors.Count; i++) { 
                        area += (structuralMembers.beams_tiearea[i] * double.Parse(Floors[i].getValues()[0]))
                            +(structuralMembers.beams_gradarea[i] * double.Parse(Floors[i].getValues()[0]))
                            +(structuralMembers.beams_RBarea[i] * double.Parse(Floors[i].getValues()[0]));
                        form += (structuralMembers.beams_grade[i] * double.Parse(Floors[i].getValues()[0]))
                            + (structuralMembers.beams_RB[i] * double.Parse(Floors[i].getValues()[0]));
                        frame += (structuralMembers.beams_gradeFrame[i] * double.Parse(Floors[i].getValues()[0]))
                            + (structuralMembers.beams_RBframe[i] * double.Parse(Floors[i].getValues()[0]));
                        vertical += (structuralMembers.beams_vertical[i] * double.Parse(Floors[i].getValues()[0]))
                            + (structuralMembers.beams_RBV[i] * double.Parse(Floors[i].getValues()[0]));
                        horizontal += (structuralMembers.beams_horizontal[i] * double.Parse(Floors[i].getValues()[0]))
                            + (structuralMembers.beams_RBH[i] * double.Parse(Floors[i].getValues()[0]));
                    }
                    form += structuralMembers.beams_comps[0];
                    frame += structuralMembers.beams_comps[1];
/*                  print("//////////////////////////////////////////");
                    print("area: " + area);
                    print("form: " + form);
                    print("frame: " + frame);
                    print("vertical: " + vertical);
                    print("horizontal: " + horizontal);
                    print("//////////////////////////////////////////");*/
                    beam_QTY = area;
                    beam_Mcost = (form * double.Parse(parameters.price_FormworksAndLumber[plyType].ToString()))
                        + (frame * double.Parse(parameters.price_FormworksAndLumber[strLumber].ToString()))
                        + (vertical * double.Parse(parameters.price_FormworksAndLumber[strVert].ToString()))
                        + (horizontal * double.Parse(parameters.price_FormworksAndLumber[strHori].ToString()));
                    beam_Munit = beam_Mcost/beam_QTY; 
                    beam_Lunit = double.Parse(parameters.price_LaborRate_Formworks["BEAM [m2]"].ToString());
                    beam_Lcost = beam_QTY * double.Parse(parameters.price_LaborRate_Formworks["BEAM [m2]"].ToString());
                    beamMLCOST = beam_Mcost + beam_Lcost;
                }
                catch
                {
                    beam_Munit = 0;
                    beam_Mcost = 0;
                    beam_Lunit = 0;
                    beam_Lcost = 0;
                    beamMLCOST = 0;
                    beam_QTY = 0;
                }
            }
            else
            {
                beam_Munit = 0;
                beam_Mcost = 0;
                beam_Lunit = 0;
                beam_Lcost = 0;
                beamMLCOST = 0;
                beam_QTY = 0;
            }
            if (formworksChecklist[3])//slab
            {
                try
                {
                    List<string> scafDim = dimensionFilterer(parameters.form_SM_HS_VS);
                    string strscafdim = lumberPriceHelper(scafDim);
                    string plyType = plywoodHelper(parameters.form_F_T);
                    double area = 0;
                    double form = 0;
                    double scaf = 0;                                        
                    for (int i = 0; i < Floors.Count; i++)
                    {
                        area += structuralMembers.slab_area[i] * double.Parse(Floors[i].getValues()[0]);
                        form += structuralMembers.slab_form[i] * double.Parse(Floors[i].getValues()[0]);
                        scaf += structuralMembers.slab_scaf[i] * double.Parse(Floors[i].getValues()[0]);
                        /*print("track");
                        print((structuralMembers.slab_form[i] * double.Parse(Floors[i].getValues()[0])).ToString());*/
                    }                    
                    sus_QTY = area;
                    sus_Mcost = (form * double.Parse(parameters.price_FormworksAndLumber[plyType].ToString())) + (scaf * double.Parse(parameters.price_FormworksAndLumber[strscafdim].ToString()));
                    sus_Munit = sus_Mcost / sus_QTY;
                    sus_Lunit = double.Parse(parameters.price_LaborRate_Formworks["SUSPENDED SLAB [m2]"].ToString());
                    sus_Lcost = sus_QTY * double.Parse(parameters.price_LaborRate_Formworks["SUSPENDED SLAB [m2]"].ToString());
                    sus_MLCOST = sus_Mcost + sus_Lcost;
                    /*print(form + " - form");
                    print(scaf + " - scaf");*/                    
                }
                catch
                {
                    sus_QTY = 0;
                    sus_Munit = 0;
                    sus_Mcost = 0;
                    sus_Lunit = 0;
                    sus_Lcost = 0;
                    sus_MLCOST = 0;
                }
            }
            else
            {
                sus_QTY = 0;
                sus_Munit = 0;
                sus_Mcost = 0;
                sus_Lunit = 0;
                sus_Lcost = 0;
                sus_MLCOST = 0;
            }
            if (formworksChecklist[4])//stairs
            {
                try
                {
                    List<string> stairsLumber = dimensionFilterer(parameters.form_SM_ST_FL);
                    List<string> stairsScaf = dimensionFilterer(parameters.form_SM_ST_VS);
                    string strLumber = lumberPriceHelper(stairsLumber);
                    string strScaf = lumberPriceHelper(stairsScaf);
                    string plyType = plywoodHelper(parameters.form_F_T);
                    double forms = 0;
                    double frames = 0;
                    double scafs = 0;
                    double area = 0;
                    for(int i = 0; i < Floors.Count; i++)
                    {
                        forms += (structuralMembers.UstairsFORM[i] + structuralMembers.LstairsFORM[i] + structuralMembers.SstairsFORM[i]) * double.Parse(Floors[i].getValues()[0]);
                        frames += (structuralMembers.UstairsFRAME[i] + structuralMembers.LstairsFRAME[i] + structuralMembers.SstairsFRAME[i]) * double.Parse(Floors[i].getValues()[0]);
                        scafs += (structuralMembers.UstairsSCAF[i] + structuralMembers.LstairsSCAF[i] + structuralMembers.SstairsSCAF[i]) * double.Parse(Floors[i].getValues()[0]);
                        area += (structuralMembers.UAREA[i] + structuralMembers.LAREA[i] + structuralMembers.SAREA[i]) * double.Parse(Floors[i].getValues()[0]);
                    }
                    print(forms + " - ");
                    print(frames + " - ");
                    print(scafs + " - ");
                    print(area + " - ");
                    stairs_QTY = Math.Round(area,2);
                    stairs_Mcost = Math.Round(((forms * double.Parse(parameters.price_FormworksAndLumber[plyType].ToString()))
                        +(frames * double.Parse(parameters.price_FormworksAndLumber[strLumber].ToString()))
                        +(scafs * double.Parse(parameters.price_FormworksAndLumber[strScaf].ToString()))),2);
                    stairs_Munit = Math.Round((stairs_Mcost/stairs_QTY),2);                    
                    stairs_Lunit = Math.Round((double.Parse(parameters.price_LaborRate_Formworks["STAIRS [m2]"].ToString())),2); 
                    stairs_Lcost = double.Parse(parameters.price_LaborRate_Formworks["STAIRS [m2]"].ToString()) * stairs_QTY;
                    stairs_MLCOST = Math.Round((stairs_Mcost + stairs_Lcost),2);
                }                                
                catch
                {
                    stairs_QTY = 0;
                    stairs_Munit = 0;
                    stairs_Mcost = 0;
                    stairs_Lunit = 0;
                    stairs_Lcost = 0;
                    stairs_MLCOST = 0;
                }
            }
            else
            {
                stairs_QTY = 0;
                stairs_Munit = 0;
                stairs_Mcost = 0;
                stairs_Lunit = 0;
                stairs_Lcost = 0;
                stairs_MLCOST = 0;
            }
            if (formworksChecklist[5])//nails
            {
                try
                {
                    double totalarea = footing_QTY + trap_QTY + col_QTY + beam_QTY + sus_QTY + stairs_QTY;
                    List<string> dimhandle = steelFilterer(parameters.form_F_N);
                    string dimnails = "";
                    foreach(char a in parameters.form_F_N)
                    {
                        if (Char.IsDigit(a) || a.Equals('.'))
                        {
                            dimnails += a;
                        }
                        else
                        {
                            break;
                        }
                    }                    
                    nails_Mcost = (totalarea * double.Parse(dimnails)) * double.Parse(parameters.price_CommonMaterials["Common Nail [KG]"].ToString());
                    nails_Munit = double.Parse(parameters.price_CommonMaterials["Common Nail [KG]"].ToString());
                    nails_QTY = (totalarea * double.Parse(dimnails));
                    nailsMLCOST = nails_Mcost;
                }
                catch
                {
                    nails_Mcost = 0;
                    nails_Munit = 0;
                    nails_QTY = 0;
                    nailsMLCOST = 0;
                }
                
            }
            else
            {
                nails_Mcost = 0;
                nails_Munit = 0;
                nails_QTY = 0;
                nailsMLCOST = 0;
            }
            //Total Cost Compu

            FW_totalMats = footing_Mcost + trap_Mcost + col_Mcost + beam_Mcost + sus_Mcost + stairs_Mcost + nails_Mcost;
            FW_totalLab = footing_Lcost + trap_Lcost + col_Lcost + beam_Lcost + sus_Lcost + stairs_Lcost;
            FW_MATOTAL = FW_totalMats + FW_totalLab;
            print("========== FORMWORKS ==============");
            print("FOOTINGS -> " + "QTY:" + footing_QTY + " MU:" + footing_Munit + " MC:" + footing_Mcost + " LU" + footing_Lunit + " LC:" + footing_Lcost + " TOTAL: " + footingMLCOST);
            print("TRAPEZOID -> " + "QTY:" + trap_QTY + " MU:" + trap_Munit + " MC:" + trap_Mcost + " LU" + trap_Lunit + " LC:" + trap_Lcost + " TOTAL: " + trapMLCOST);
            print("COLUMN -> " + "QTY:" + col_QTY + " MU:" + col_Munit + " MC:" + col_Mcost + " LU" + col_Lunit + " LC:" + col_Lcost + " TOTAL: " + colMLCOST);
            print("BEAM -> " + "QTY:" + beam_QTY + " MU:" + beam_Munit + " MC:" + beam_Mcost + " LU" + beam_Lunit + " LC:" + beam_Lcost + " TOTAL: " + beamMLCOST);
            print("SLAB -> " + "QTY:" + sus_QTY + " MU:" + sus_Munit + " MC:" + sus_Mcost + " LU" + sus_Lunit + " LC:" + sus_Lcost + " TOTAL: " + sus_MLCOST);
            print("STAIRS -> " + "QTY:" + stairs_QTY + " MU:" + stairs_Munit + " MC:" + stairs_Mcost + " LU" + stairs_Lunit + " LC:" + stairs_Lcost + " TOTAL: " + stairs_MLCOST);
            print("FOOTINGS -> " + "QTY:" + nails_QTY + " MU:" + nails_Munit + " MC:" + nails_Mcost +" TOTAL: " + nailsMLCOST);

            //Masonry
            string strCHB;
            if (masonryChecklist[0])
            {
                try
                {
                    if(parameters.mason_CHB_EW == ".10 x .20 x .40")
                    {
                        strCHB = "CHB 4” (0.10 x 0.20 x 0.40) [PC]";
                    }
                    else if((parameters.mason_CHB_EW == ".15 x .20 x .40"))
                    {
                        strCHB = "CHB 6” (0.15 x 0.20 x 0.40) [PC]";
                    }
                    else
                    {
                        strCHB = "CHB 8” (0.20 x 0.20 x 0.40) [PC]";
                    }                                                                                                                                                      
                    exterior_CostM = (masonrysSolutionP1[4] * double.Parse(parameters.price_CommonMaterials[strCHB].ToString())) + 
                        (masonrysSolutionP2[0] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString())) + 
                        (masonrysSolutionP2[1] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString())) + 
                        (masonrysSolutionP2[5] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString())) + 
                        (masonrysSolutionP2[6] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString()));
                    exterior_UnitM = Math.Round(exterior_CostM / masonrysSolutionP1[3], 2);
                    exterior_CostL = masonrysSolutionP1[3] * double.Parse(parameters.price_LaborRate_Masonry["MASONRY [m2]"].ToString());                    
                    exterior_costTotal = exterior_CostM + exterior_CostL;

                    
                }
                catch
                {
                    exterior_CostM = 0;
                    exterior_UnitM = 0;
                    exterior_CostL = 0;
                    exterior_costTotal = 0;
                }                
            }
            else
            {
                exterior_CostM = 0;
                exterior_UnitM = 0;
                exterior_CostL = 0;
                exterior_costTotal = 0;
            }
            if (masonryChecklist[1])
            {
                try
                {                    
                    if (parameters.mason_CHB_IW == ".10 x .20 x .40")
                    {
                        strCHB = "CHB 4” (0.10 x 0.20 x 0.40) [PC]";                        
                    }
                    else if ((parameters.mason_CHB_IW == ".15 x .20 x .40"))
                    {
                        strCHB = "CHB 6” (0.15 x 0.20 x 0.40) [PC]";
                    }
                    else
                    {
                        strCHB = "CHB 8” (0.20 x 0.20 x 0.40) [PC]";
                    }                                                                                                                                                                           
                    interior_CostM = (masonrysSolutionP1[9] * double.Parse(parameters.price_CommonMaterials[strCHB].ToString())) + 
                        (masonrysSolutionP2[2] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString())) + 
                        (masonrysSolutionP2[3] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString())) + 
                        (masonrysSolutionP2[8] * double.Parse(parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString())) + 
                        (masonrysSolutionP2[9] * double.Parse(parameters.price_CommonMaterials["Sand [m3]"].ToString()));
                    interior_UnitM = Math.Round(interior_CostM / masonrysSolutionP1[8], 2);
                    interior_CostL = masonrysSolutionP1[8] * double.Parse(parameters.price_LaborRate_Masonry["MASONRY [m2]"].ToString());
                    interior_costTotal = interior_CostM + interior_CostL;
                }
                catch
                {
                    interior_CostM = 0;
                    interior_UnitM = 0;
                    interior_CostL = 0;
                    interior_costTotal = 0;
                }
            }
            else
            {
                interior_CostM = 0;
                interior_UnitM = 0;
                interior_CostL = 0;
                interior_costTotal = 0;
            }
            masonMCost_total = exterior_CostM + interior_CostM;
            masonLCost_total = exterior_CostL + interior_CostL;
            mason_TOTALCOST = interior_costTotal + exterior_costTotal;
            print("=== MASONRY ===");
            print("M_unit: " + exterior_UnitM + " M_cost: " + exterior_CostM + "L_cost: " + exterior_CostL + "EXT_TOTAL: "+ exterior_costTotal);
            print("M_unit: " + interior_UnitM + " M_cost: " + interior_CostM + "L_cost: " + interior_CostL + "INT_TOTAL: " + interior_costTotal);                        
            print("-- TOTAL -- ");
            print("M_total: " + masonMCost_total);
            print("L_total: " + masonLCost_total);
            print("TOTAL COST: " + mason_TOTALCOST);
            //Masonry -- END

            //Rebars START
            if (rebarsChecklist[0])
            {
                //Footing
                //Reset cost variables
                reinF_QTY = 0;
                reinF_UnitM = 0;
                reinF_CostM = 0;
                reinF_CostL = 0;
                reinF_TotalCost = 0;
                //4
                string rebarGrade = parameters.rein_RG_F;
                rebarGrade = rebarGrade.ToUpper();
                print("================== Footing Reinforcements ===================");
                foreach (List<List<double>> footing in structuralMembers.footingReinforcements)
                {
                    foreach(List<double> chosenML in footing)
                    {
                        try
                        {
                            double unitCost = 0;
                            if (rebarGrade.Contains("33"))
                                unitCost = double.Parse(parameters.price_RebarGrade33["Rebar " + rebarGrade + " (⌀" + chosenML[8] + "mm) [" + chosenML[0] + "m]"].ToString());
                            else if (rebarGrade.Contains("40"))
                                unitCost = double.Parse(parameters.price_RebarGrade40["Rebar " + rebarGrade + " (⌀" + chosenML[8] + "mm) [" + chosenML[0] + "m]"].ToString());
                            else
                                unitCost = double.Parse(parameters.price_RebarGrade60["Rebar " + rebarGrade + " (⌀" + chosenML[8] + "mm) [" + chosenML[0] + "m]"].ToString());
                            reinF_CostM += (chosenML[4] * unitCost);
                        }
                        catch(Exception ex)
                        {
                            print("Rebar Footings error: " + ex.ToString());
                        }
                    }
                }
                print("Material Cost: " + reinF_CostM);
                //6
                //parameters.price_LaborRate_Rebar["FOOTING [KG]"]
                foreach (List<List<double>> footing in structuralMembers.footingReinforcements)
                {
                    foreach (List<double> chosenML in footing)
                    {
                        try
                        {
                            double unitCost = 0;
                            unitCost = double.Parse(parameters.price_LaborRate_Rebar["FOOTING [KG]"].ToString());
                            reinF_CostL += (chosenML[9] * unitCost);
                            reinF_QTY += chosenML[9];
                        }
                        catch (Exception ex)
                        {
                            print("Rebar Footings error: " + ex.ToString());
                        }
                    }
                }
                print("Labor Cost: " + reinF_CostL);
                reinF_TotalCost = reinF_CostM + reinF_CostL;
                print("Total Cost: " + reinF_TotalCost);
            }
            else
            {
                //Reset cost variables
                reinF_QTY = 0;
                reinF_UnitM = 0;
                reinF_CostM = 0;
                reinF_CostL = 0;
                reinF_TotalCost = 0;
            }
            if (rebarsChecklist[1])
            {
                //Wall Footing
                //Reset cost variables
                reinWF_QTY = 0;
                reinWF_UnitM = 0;
                reinWF_CostM = 0;
                reinWF_CostL = 0;
                reinWF_TotalCost = 0;
                //5
                string rebarGrade = parameters.rein_RG_SL;
                rebarGrade = rebarGrade.ToUpper();
                print("================== Wall Footing Reinforcements ===================");
                foreach (List<List<double>> wallFooting in structuralMembers.wallFootingReinforcements)
                {
                    foreach (List<double> chosenML in wallFooting)
                    {
                        try
                        {
                            double unitCost = 0;
                            if (rebarGrade.Contains("33"))
                                unitCost = double.Parse(parameters.price_RebarGrade33["Rebar " + rebarGrade + " (⌀" + chosenML[8] + "mm) [" + chosenML[0] + "m]"].ToString());
                            else if (rebarGrade.Contains("40"))
                                unitCost = double.Parse(parameters.price_RebarGrade40["Rebar " + rebarGrade + " (⌀" + chosenML[8] + "mm) [" + chosenML[0] + "m]"].ToString());
                            else
                                unitCost = double.Parse(parameters.price_RebarGrade60["Rebar " + rebarGrade + " (⌀" + chosenML[8] + "mm) [" + chosenML[0] + "m]"].ToString());
                            reinWF_CostM += (Math.Ceiling(chosenML[4]) * unitCost);
                        }
                        catch (Exception ex)
                        {
                            print("Rebar Wall Footings error: " + ex.ToString());
                        }
                    }
                }
                print("Material Cost: " + reinWF_CostM);
                //7 - 8 
                foreach (List<List<double>> wallFooting in structuralMembers.wallFootingReinforcements)
                {
                    foreach (List<double> chosenML in wallFooting)
                    {
                        try
                        {
                            double unitCost = 0;
                            unitCost = double.Parse(parameters.price_LaborRate_Rebar["WALL FOOTING [KG]"].ToString());
                            reinWF_CostL += (chosenML[9] * unitCost);
                            reinWF_QTY += chosenML[9];
                        }
                        catch (Exception ex)
                        {
                            print("Rebar Wall Footings error: " + ex.ToString());
                        }
                    }
                }
                print("Labor Cost: " + reinWF_CostL);
                print("KG: " + reinWF_QTY);
                reinWF_TotalCost = reinWF_CostM + reinWF_CostL;
                print("Total Cost: " + reinWF_TotalCost);
            }
            else
            {
                //Reset cost variables
                reinWF_QTY = 0;
                reinWF_UnitM = 0;
                reinWF_CostM = 0;
                reinWF_CostL = 0;
                reinWF_TotalCost = 0;
            }
            if (rebarsChecklist[2]) // COLUMNS
            {
                double columnMainPrice = 0;
                double laterTiesPrice = 0;
                try
                {                    
                    for (int i = 0; i < structuralMembers.Column_mainRebar.Count; i++)
                    {
                        for (int j = 0; j < structuralMembers.Column_mainRebar[i].Count; j++)
                        {
                            List<string> grade = gradefilterer(parameters.rein_RG_C);
                            string dia = structuralMembers.Column_mainRebar[i][j][2];
                            string ms = structuralMembers.Column_mainRebar[i][j][0];
                            string price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                            if (ms != "0")
                            {
                                if (grade[1] == "33")
                                {
                                    columnMainPrice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                        * double.Parse(structuralMembers.Column_mainRebar[i][j][1])) * double.Parse(Floors[i].getValues()[0]);
                                }
                                else if (grade[1] == "40")
                                {
                                    columnMainPrice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                        * double.Parse(structuralMembers.Column_mainRebar[i][j][1])) * double.Parse(Floors[i].getValues()[0]);
                                }
                                else if (grade[1] == "60")
                                {
                                    columnMainPrice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                        * double.Parse(structuralMembers.Column_mainRebar[i][j][1])) * double.Parse(Floors[i].getValues()[0]);
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    print("Column Main" + ex);
                }
                try
                {
                    for (int i = 0; i < structuralMembers.Column_lateralRebar.Count; i++)
                    {                        
                        for (int j = 0; j < structuralMembers.Column_lateralRebar[i].Count; j++)
                        {
                            for (int n = 0; n < structuralMembers.Column_lateralRebar[i][j].Count; n++)
                            {
                                List<string> grade = gradefilterer(parameters.rein_RG_CLT);
                                string dia = structuralMembers.Column_lateralRebar[i][j][n][2];
                                string ms = structuralMembers.Column_lateralRebar[i][j][n][0];
                                double qty = double.Parse(structuralMembers.Column_lateralRebar[i][j][n][1]);
                                string price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                                if (ms != "0")
                                {
                                    if (grade[1] == "33")
                                    {
                                        laterTiesPrice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                            * qty) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "40")
                                    {
                                        laterTiesPrice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                            * qty) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "60")
                                    {
                                        laterTiesPrice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                            * qty) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    print("Column Lateral: " + ex);
                }
                double columnMain_Labor = structuralMembers.totalweightkgm_Colmain * double.Parse(parameters.price_LaborRate_Rebar["COLUMN [KG]"].ToString());// -- //
                double columnLats_Labor = structuralMembers.totalweightkgm_Colties * double.Parse(parameters.price_LaborRate_Rebar["COLUMN [KG]"].ToString());// -- //                
                RCOL_qty = structuralMembers.totalweightkgm_Colmain + structuralMembers.totalweightkgm_Colties;
                RCOL_Mcost = columnMainPrice + laterTiesPrice;
                RCOL_Munit = RCOL_Mcost / RCOL_qty;
                RCOL_Lunit = double.Parse(parameters.price_LaborRate_Rebar["COLUMN [KG]"].ToString());
                RCOL_Lcost = columnMain_Labor + columnLats_Labor;
                RCOL_TOTALCOST = RCOL_Mcost + RCOL_Lcost;
                print("Main weight: " + structuralMembers.totalweightkgm_Colmain);
                print("Ties weight: " + structuralMembers.totalweightkgm_Colties);
                print("============== COLUMN ===========");
                print("QTY: " + RCOL_qty);
                print("Munit: " + RCOL_Munit);
                print("Mcost: " + RCOL_Mcost);
                print("Lunit: " + RCOL_Lunit);
                print("Lcost: " + RCOL_Lcost);
                print("TOTAL COST: " + RCOL_TOTALCOST);
                print("============== COLUMN (EACH) ===========");
                print("COLUMN MAIN PRICE: " + columnMainPrice);
                print("COLUM MAIN LABOR: " + columnMain_Labor);
                print("COLUMN LATERAL PRICE: " + laterTiesPrice);
                print("COLUMN LATERAL LABOR: " + columnLats_Labor);
            }
            else
            {
                RCOL_qty = 0;
                RCOL_Munit = 0;
                RCOL_Mcost = 0;
                RCOL_Lunit = 0;
                RCOL_Lcost = 0;
                RCOL_TOTALCOST = 0;
            }
            if (rebarsChecklist[3])//BEAM
            {
                double mainbeamPrice = 0; // -- //
                double stirRebarPrice = 0;// -- //
                double webRebarPrice = 0;// -- //
                try
                {
                    for (int i = 0; i < structuralMembers.Beam_mainRebar.Count; i++)
                    {
                        for (int j = 0; j < structuralMembers.Beam_mainRebar[i].Count; j++)
                        {
                            for (int n = 0; n < structuralMembers.Beam_mainRebar[i][j].Count; n++)
                            {
                                //part 1
                                List<string> grade = gradefilterer(parameters.rein_RG_B);
                                string dia = structuralMembers.beamdias[i][j][7];
                                string ms = structuralMembers.Beam_mainRebar[i][j][n][0];
                                string price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                                if (ms != "0")
                                {
                                    if (grade[1] == "33")
                                    {
                                        mainbeamPrice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_mainRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "40")
                                    {
                                        mainbeamPrice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_mainRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]); 
                                    }
                                    else if (grade[1] == "60")
                                    {
                                        mainbeamPrice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_mainRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                }
                                //part2
                                dia = structuralMembers.beamdias[i][j][8];
                                ms = structuralMembers.Beam_mainRebar[i][j][n][2];
                                price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                                if (ms != "0")
                                {
                                    if (grade[1] == "33")
                                    {
                                        mainbeamPrice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_mainRebar[i][j][n][3])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "40")
                                    {
                                        mainbeamPrice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_mainRebar[i][j][n][3])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "60")
                                    {
                                        mainbeamPrice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_mainRebar[i][j][n][3])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                }

                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    print("Beam main: " + ex);
                }

                try
                {
                    for (int i = 0; i < structuralMembers.Beam_stirRebar.Count; i++)
                    {
                        for (int j = 0; j < structuralMembers.Beam_stirRebar[i].Count; j++)
                        {
                            for (int n = 0; n < structuralMembers.Beam_stirRebar[i][j].Count; n++)
                            {
                                //part 1
                                List<string> grade = gradefilterer(parameters.rein_RG_BS);
                                string dia = structuralMembers.Beam_stirRebar[i][j][n][2];
                                string ms = structuralMembers.Beam_stirRebar[i][j][n][0];
                                string price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                                if (ms != "0")
                                {
                                    if (grade[1] == "33")
                                    {
                                        stirRebarPrice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_stirRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "40")
                                    {
                                        stirRebarPrice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_stirRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "60")
                                    {
                                        stirRebarPrice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_stirRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                }
                            }
                        }
                    }
                }   
                catch(Exception ex)
                {
                    print("Beam stirups: " + ex);
                }

                try
                {
                    for (int i = 0; i < structuralMembers.Beam_webRebar.Count; i++)
                    {
                        for (int j = 0; j < structuralMembers.Beam_webRebar[i].Count; j++)
                        {
                            for (int n = 0; n < structuralMembers.Beam_webRebar[i][j].Count; n++)
                            {
                                //part 1
                                List<string> grade = gradefilterer(parameters.rein_RG_B);
                                string dia = structuralMembers.Beam_webRebar[i][j][n][2];
                                string ms = structuralMembers.Beam_webRebar[i][j][n][0];
                                string price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                                if (ms != "0")
                                {
                                    if (grade[1] == "33")
                                    {
                                        webRebarPrice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_webRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]); 
                                    }
                                    else if (grade[1] == "40")
                                    {
                                        webRebarPrice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_webRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "60")
                                    {
                                        webRebarPrice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                            * double.Parse(structuralMembers.Beam_webRebar[i][j][n][1])) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    print("Beam web: " + ex);
                }
                double mainrebarlabor = structuralMembers.totalweightkgm_main * double.Parse(parameters.price_LaborRate_Rebar["BEAM [KG]"].ToString());// -- //
                double stirLabor = structuralMembers.totalweightkgm_stir * double.Parse(parameters.price_LaborRate_Rebar["BEAM [KG]"].ToString());// -- //
                double webLabor = structuralMembers.totalweightkgm_web * double.Parse(parameters.price_LaborRate_Rebar["BEAM [KG]"].ToString());// -- //
                Rbeam_qty = structuralMembers.totalweightkgm_main
                        + structuralMembers.totalweightkgm_stir
                        + structuralMembers.totalweightkgm_web;
                Rbeam_Mcost = mainbeamPrice + stirRebarPrice + webRebarPrice;
                Rbeam_Munit = Rbeam_Mcost / Rbeam_qty;
                Rbeam_Lcost = mainrebarlabor + stirLabor + webRebarPrice + webLabor;
                Rbeam_Lunit = double.Parse(parameters.price_LaborRate_Rebar["BEAM [KG]"].ToString());
                print("========= BEAM REBARS =========");
                print("MQTY: " + Rbeam_qty);
                print("Munit: " + Rbeam_Munit);
                print("Mcost: " + Rbeam_Mcost);
                print("Lunit: " + Rbeam_Lunit);
                print("Lcost: " + Rbeam_Lcost);
                print("========= BEAM REBARS (EACH) =========");
                print("MAIN: " + mainbeamPrice);
                print("STIR: " + stirRebarPrice);
                print("WEB: " + webRebarPrice);
                print("MAI L: " + mainrebarlabor);
                print("STIR L: " + stirLabor);
                print("WEB L: " + webLabor);
            }
            else
            {
                Rbeam_qty = 0;
                Rbeam_Munit = 0;
                Rbeam_Mcost = 0;
                Rbeam_Lunit = 0;
                Rbeam_Lcost = 0;
                Rbeam_TOTALCOST = 0;
            }
            if (rebarsChecklist[4]) // Slab on grade
            {
                double slabOGprice = 0;
                try
                {                    
                    for(int i = 0; i < structuralMembers.Slab_ongradeRebar.Count; i++)
                    {
                        for(int j = 0; j < structuralMembers.Slab_ongradeRebar[i].Count; j++)
                        {
                            List<string> grade = gradefilterer(parameters.rein_RG_SL);
                            string dia = structuralMembers.Slab_ongradeRebar[i][j][4];
                            string ms = structuralMembers.Slab_ongradeRebar[i][j][0];
                            string price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                            if (ms != "0")
                            {
                                if (grade[1] == "33")
                                {
                                    slabOGprice += double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                        * double.Parse(structuralMembers.Slab_ongradeRebar[i][j][1]);
                                }
                                else if (grade[1] == "40")
                                {
                                    slabOGprice += double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                        * double.Parse(structuralMembers.Slab_ongradeRebar[i][j][1]);
                                }
                                else if (grade[1] == "60")
                                {
                                    slabOGprice += double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                        * double.Parse(structuralMembers.Slab_ongradeRebar[i][j][1]);
                                }
                            }
                            dia = structuralMembers.Slab_ongradeRebar[i][j][5];
                            ms = structuralMembers.Slab_ongradeRebar[i][j][2];
                            price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                            if (ms != "0")
                            {
                                if (grade[1] == "33")
                                {
                                    slabOGprice += double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                        * double.Parse(structuralMembers.Slab_ongradeRebar[i][j][3]);
                                }
                                else if (grade[1] == "40")
                                {
                                    slabOGprice += double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                        * double.Parse(structuralMembers.Slab_ongradeRebar[i][j][3]);
                                }
                                else if (grade[1] == "60")
                                {
                                    slabOGprice += double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                        * double.Parse(structuralMembers.Slab_ongradeRebar[i][j][3]);
                                }
                            }                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    print("Slab on grade: " + ex);
                }                
                double slabongradeLABOR = structuralMembers.totalweightkgm_slabongrade * double.Parse(parameters.price_LaborRate_Rebar["SLAB ON GRADE [KG]"].ToString());                
                RSOG_qty = structuralMembers.totalweightkgm_slabongrade;
                RSOG_Munit = slabOGprice/RSOG_qty;
                RSOG_Mcost = slabOGprice;
                RSOG_Lunit = double.Parse(parameters.price_LaborRate_Rebar["SLAB ON GRADE [KG]"].ToString());
                RSOG_Lcost = slabongradeLABOR;
                RSOG_TOTALCOST = RSOG_Mcost + RSOG_Lcost;
                print("======== SLAB ON GRADE ========");
                print("RSOG_qty: " + RSOG_qty);
                print("RSOG_Munit: " + RSOG_Munit);
                print("RSOG_Mcost: " + RSOG_Mcost);
                print("RSOG_Lunit: " + RSOG_Lunit);
                print("RSOG_Lcost: " + RSOG_Lcost);
                print("RSOG_TOTALCOST: " + RSOG_TOTALCOST);
            }
            else
            {
                RSOG_qty = 0;
                RSOG_Munit = 0;
                RSOG_Mcost = 0;
                RSOG_Lunit = 0;
                RSOG_Lcost = 0;
                RSOG_TOTALCOST = 0;                
            }
            if (rebarsChecklist[5])
            {
                double suspendedSlabPrice = 0;
                try
                {
                    for (int i = 0; i < structuralMembers.Slab_suspendedRebar.Count; i++)
                    {
                        for (int j = 0; j < structuralMembers.Slab_suspendedRebar[i].Count; j += 2)
                        {
                            List<string> grade = gradefilterer(parameters.rein_RG_SL);
                            string dia = structuralMembers.Slab_suspendedRebar[i][j][0].ToString();
                            for (int n = 1; n < structuralMembers.Slab_suspendedRebar[i][j].Count; n += 2)
                            {
                                string ms = structuralMembers.Slab_suspendedRebar[i][j][n].ToString();
                                string price_name = "Rebar GRADE " + grade[1] + " (⌀" + dia + "mm) [" + ms + "m]";
                                if (ms != "0")
                                {
                                    if (grade[1] == "33")
                                    {
                                        suspendedSlabPrice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                            * structuralMembers.Slab_suspendedRebar[i][j][n + 1]) * double.Parse(Floors[i+1].getValues()[0]);
                                    }
                                    else if (grade[1] == "40")
                                    {
                                        suspendedSlabPrice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                            * structuralMembers.Slab_suspendedRebar[i][j][n + 1]) * double.Parse(Floors[i+1].getValues()[0]);
                                    }
                                    else if (grade[1] == "60")
                                    {
                                        suspendedSlabPrice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                            * structuralMembers.Slab_suspendedRebar[i][j][n + 1]) * double.Parse(Floors[i+1].getValues()[0]);
                                    }
                                }
                            }
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    print("Suspended slab: " + ex);
                }
                double suspendedslabLABOR = structuralMembers.totalweightkgm_suspendedslab * double.Parse(parameters.price_LaborRate_Rebar["SUSPENDED SLAB [KG]"].ToString());
                RSS_qty = structuralMembers.totalweightkgm_suspendedslab;
                RSS_Mcost = suspendedSlabPrice;
                RSS_Munit = RSS_Mcost / RSS_qty;                
                RSS_Lunit = double.Parse(parameters.price_LaborRate_Rebar["SUSPENDED SLAB [KG]"].ToString());
                RSS_Lcost = double.Parse(parameters.price_LaborRate_Rebar["SUSPENDED SLAB [KG]"].ToString()) * RSS_qty;
                RSS_TOTALCOST = RSS_Mcost + RSS_Lcost;
                print("======== SUSPENDED SLAB ========");
                print("RSS_qty: " + RSS_qty);
                print("RSS_Munit: " + RSS_Munit);
                print("RSS_Mcost: " + RSS_Mcost);
                print("RSS_Lunit: " + RSS_Lunit);
                print("RSS_Lcost: " + RSS_Lcost);
                print("RSS_TOTALCOST: " + RSS_TOTALCOST);
            }
            else
            {
                RSS_qty = 0;
                RSS_Munit = 0;
                RSS_Mcost = 0;
                RSS_Lunit = 0;
                RSS_Lcost = 0;
                RSS_TOTALCOST = 0;
            }
            if (rebarsChecklist[6])
            {
                double stairsprice = 0;
                double stairsweight = 0;
                try
                {
                    int i = 0;
                    foreach (List<List<double[,]>> floor in structuralMembers.stairs_Rebar)
                    {
                        int j = 0;
                        foreach (List<double[,]> stair in floor)
                        {
                            int k = 0;
                            foreach (double[,] row in stair)
                            {
                                string userML = parameters.stair[i][j].getValues()[k+5];
                                int mlIndex = 0;
                                if (userML == "6.0m")
                                {
                                    mlIndex = 0;
                                }
                                else if (userML == "7.5m")
                                {
                                    mlIndex = 1;
                                }
                                else if (userML == "9.0m")
                                {
                                    mlIndex = 2;
                                }
                                else if (userML == "10.5m")
                                {
                                    mlIndex = 3;
                                }
                                else if (userML == "12.0m")
                                {
                                    mlIndex = 4;
                                }
                                List<string> grade = gradefilterer(parameters.stair[i][j].getValues()[4]);
                                double diameter = row[mlIndex, 0];
                                double manufactured_length = row[mlIndex, 1];
                                string price_name = "Rebar GRADE " + grade[1] + " (⌀" + diameter + "mm) [" + manufactured_length + "m]";
                                if (manufactured_length.ToString() != "0")
                                {
                                    if (grade[1] == "33")
                                    {
                                        stairsprice += (double.Parse(parameters.price_RebarGrade33[price_name].ToString())
                                            * row[mlIndex, 2]) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "40")
                                    {
                                        stairsprice += (double.Parse(parameters.price_RebarGrade40[price_name].ToString())
                                            * row[mlIndex, 2]) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                    else if (grade[1] == "60")
                                    {
                                        stairsprice += (double.Parse(parameters.price_RebarGrade60[price_name].ToString())
                                            * row[mlIndex, 2]) * double.Parse(Floors[i].getValues()[0]);
                                    }
                                }
                                stairsweight += (row[mlIndex, 1] * row[mlIndex, 2] * row[mlIndex, 3]) * double.Parse(Floors[i].getValues()[0]);
                                k++;
                            }
                            j++;
                        }
                        i++;
                    }
                    double stairsLabor = stairsweight * double.Parse(parameters.price_LaborRate_Rebar["STAIRS [KG]"].ToString());
                    RSTAIRS_qty = stairsweight;                    
                    RSTAIRS_Mcost = stairsprice;
                    RSTAIRS_Munit = RSTAIRS_Mcost/RSTAIRS_qty;
                    RSTAIRS_Lcost = stairsLabor;
                    RSTAIRS_Lunit = double.Parse(parameters.price_LaborRate_Rebar["STAIRS [KG]"].ToString());
                    RSTAIRS_TOTALCOST = RSTAIRS_Mcost + RSTAIRS_Lcost;
                    print("============== STAIRS ============");
                    print("RSTAIRS_qty: " + RSTAIRS_qty);
                    print("RSTAIRS_Munit: " + RSTAIRS_Munit);
                    print("RSTAIRS_Mcost: " + RSTAIRS_Mcost);
                    print("RSTAIRS_Lunit: " + RSTAIRS_Lunit);
                    print("RSTAIRS_Lcost: " + RSTAIRS_Lcost);
                    print("RSTAIRS_TOTALCOST: " + RSTAIRS_TOTALCOST);
                }
                catch(Exception ex)
                {
                    print("Stairs: " + ex);
                }
                
            }
            else
            {
                RSTAIRS_qty = 0;
                RSTAIRS_Munit = 0;
                RSTAIRS_Mcost = 0;
                RSTAIRS_Lunit = 0;
                RSTAIRS_Lcost = 0;
                RSTAIRS_TOTALCOST = 0;
            }
            if (rebarsChecklist[7])// Walls
            {                
                //extvBAR -> exthBAR -> extReinforcementCHB -> extReinforcementWeight -> extTieWire -> intvBAR -> inthBAR -> intReinforcementCHB -> intReinforcementWeight -> intTieWire
                try 
                {                        
                    List<string> grade_holder = gradefilterer(parameters.mason_RTW_RG);
                    double exterior_chb = masonrysSolutionP3[2];
                    double exterior_weight = masonrysSolutionP3[3];
                    double exterior_TW = masonrysSolutionP3[4];

                    double interior_chb = masonrysSolutionP3[7];
                    double interior_weight = masonrysSolutionP3[8];
                    double interior_TW = masonrysSolutionP3[9];

                    string rebar_length = parameters.mason_RTW_RL; 
                    string rebar_diameter = parameters.mason_RTW_BD;
                    string grade = parameters.mason_RTW_RG;

                    /*print("Exterior chb: " + masonrysSolutionP3[2]);
                    print("Exterior weit: " + masonrysSolutionP3[3]);
                    print("Exterior TW: " + masonrysSolutionP3[4]);

                    print("interior chb: " + masonrysSolutionP3[7]);
                    print("interior weit: " + masonrysSolutionP3[8]);
                    print("interior TW: " + masonrysSolutionP3[9]);

                    print("rebar l: " + rebar_length);
                    print("rebar dia: " + rebar_diameter);
                    print("grade: " + grade);*/
                    double priceTW = 0;
                    string price_name = "Rebar "+ grade + " (⌀" + rebar_diameter + ") [" + rebar_length + "]";
                    if (grade_holder[1] == "33")
                    {
                        priceTW = double.Parse(parameters.price_RebarGrade33[price_name].ToString());                            
                    }
                    else if (grade_holder[1] == "40")
                    {
                        priceTW = double.Parse(parameters.price_RebarGrade40[price_name].ToString());                            
                    }
                    else if (grade_holder[1] == "60")
                    {
                        priceTW = double.Parse(parameters.price_RebarGrade60[price_name].ToString());                            
                    }

                    double totalPriceTW = 0;
                    double totalLaborTW = 0;
                    totalPriceTW += exterior_chb * priceTW;
                    totalPriceTW += interior_chb * priceTW;
                    totalPriceTW += exterior_TW * double.Parse(parameters.price_CommonMaterials["GI Wire (no. 16) [KG]"].ToString());
                    totalPriceTW += interior_TW * double.Parse(parameters.price_CommonMaterials["GI Wire (no. 16) [KG]"].ToString());
                    
                    totalLaborTW += double.Parse(parameters.price_LaborRate_Rebar["WALLS [KG]"].ToString()) * exterior_weight;
                    totalLaborTW += double.Parse(parameters.price_LaborRate_Rebar["WALLS [KG]"].ToString()) * interior_weight;
                    RWALLS_qty = exterior_weight + interior_weight + exterior_TW + interior_TW;
                    RWALLS_Mcost = totalPriceTW;
                    RWALLS_Munit = totalPriceTW/RWALLS_qty;
                    RWALLS_Lunit = double.Parse(parameters.price_LaborRate_Rebar["WALLS [KG]"].ToString());
                    RWALLS_Lcost = totalLaborTW;
                    RWALLS_TOTALCOST = RWALLS_Mcost + RWALLS_Lcost;
                    print("=========WALLS======");
                    print("RWALLS_qty: " + RWALLS_qty);
                    print("RWALLS_Munit: " + RWALLS_Munit);
                    print("RWALLS_Mcost: " + RWALLS_Mcost);
                    print("RWALLS_Lunit: " + RWALLS_Lunit);
                    print("RWALLS_Lcost: " + RWALLS_Lcost);
                    print("RWALLS_TOTALCOST: " + RWALLS_TOTALCOST);

                }
                catch(Exception ex)
                {
                    print("WALLS: " + ex);
                }                                
            }
            else
            {
                RWALLS_qty = 0;
                RWALLS_Munit = 0;
                RWALLS_Mcost = 0;
                RWALLS_Lunit = 0;
                RWALLS_Lcost = 0;
                RWALLS_TOTALCOST = 0;
            }

            rein_MCost = reinF_CostM + reinWF_CostM + Rbeam_Mcost + RSOG_Mcost + RSS_Mcost + RCOL_Mcost + RSTAIRS_Mcost + RWALLS_Mcost;
            rein_LCost = reinF_CostL + reinWF_CostL + Rbeam_Lcost + RSOG_Lcost + RSS_Lcost + RCOL_Lcost + RSTAIRS_Lcost + RWALLS_Lcost;
            rein_TotalCost = reinF_TotalCost + reinWF_TotalCost + Rbeam_TOTALCOST + RSOG_TOTALCOST + RSS_TOTALCOST + RCOL_TOTALCOST + RSTAIRS_TOTALCOST + RWALLS_TOTALCOST;
            //Rebars END

            //Roofings -- START
            
            double labor_holder = 0;
            double sqm_roof = 0;
            List<double> p_len = new List<double>();
            List<double> r_hei = new List<double>();
            
            try
            {
                for (int i = 0; i < structuralMembers.roof.Count; i++)
                {
                    for (int j = 0; j < structuralMembers.roof[i].Count; j++)
                    {
                        if (structuralMembers.roof[i][j][0] == "G.I Roof and Its Accessories")
                        {
                            p_len.Add(double.Parse(structuralMembers.roof[i][j][1]));
                        }

                    }

                }
                for (int i = 0; i < structuralMembers.roofHRS.Count; i++)
                {
                    for (int j = 0; j < structuralMembers.roofHRS[i].Count; j++)
                    {
                        foreach (var k in structuralMembers.roofHRS[i][j])
                        {
                            r_hei.Add(double.Parse(k));
                        }
                    }
                }

                for (int i = 0; i < p_len.Count; i++)
                {
                    sqm_roof += p_len[i] * r_hei[i] * 2;
                    labor_holder += p_len[i] * r_hei[i] * double.Parse(parameters.price_LaborRate_Roofings["ROOFINGS [m2]"].ToString()) * 2;
                }
                roof_QTY = sqm_roof;
                roof_LTOTAL = labor_holder;
            }
            catch (Exception ex)
            {
                print("Roofings ex upper: "+ex);
            }

            if (roofingsChecklist[0])
            {
                double wood_price = 0;
                double steelRaft_price = 0;
                double steelPurlins_price = 0;
                double ceeRaft_price = 0;
                double ceePurlins_price = 0;
                try {
                    for (int i = 0; i < structuralMembers.roofSolutions.Count; i++)//floor
                    {
                        for (int j = 0; j < structuralMembers.roofSolutions[i].Count; j++)// roofs each floor
                        {
                            if (structuralMembers.roofSolutions[i][j][0] == 1)//rafters and purlins
                            {
                                if (structuralMembers.roofSolutions[i][j][1] == 1)//wood
                                {
                                    wood_price += structuralMembers.roofSolutions[i][j][2] * cocoLumber_price;
                                }
                                else if (structuralMembers.roofSolutions[i][j][1] == 2)//steel
                                {
                                    for (int x = 7; x < 9; x++)
                                    {
                                        if (steelFilterer(structuralMembers.roof[i][j][x])[2] == "1.0")
                                        {
                                            if (x == 7)
                                            {
                                                steelRaft_price += structuralMembers.roofSolutions[i][j][2] * double.Parse(parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular " + structuralMembers.roof[i][j][x] + " [6m]"].ToString());
                                            }
                                            else
                                            {
                                                steelPurlins_price += structuralMembers.roofSolutions[i][j][3] * double.Parse(parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular " + structuralMembers.roof[i][j][x] + " [6m]"].ToString());
                                            }
                                        }
                                        else if (steelFilterer(structuralMembers.roof[i][j][x])[2] == "1.2")
                                        {
                                            if (x == 7)
                                            {
                                                steelRaft_price += structuralMembers.roofSolutions[i][j][2] * double.Parse(parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular " + structuralMembers.roof[i][j][x] + " [6m]"].ToString());
                                            }
                                            else
                                            {
                                                steelPurlins_price += structuralMembers.roofSolutions[i][j][3] * double.Parse(parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular " + structuralMembers.roof[i][j][x] + " [6m]"].ToString());
                                            }

                                        }
                                        else if (steelFilterer(structuralMembers.roof[i][j][x])[2] == "1.5")
                                        {
                                            if (x == 7)
                                            {
                                                steelRaft_price += structuralMembers.roofSolutions[i][j][2] * double.Parse(parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular " + structuralMembers.roof[i][j][x] + " [6m]"].ToString());
                                            }
                                            else
                                            {
                                                steelPurlins_price += structuralMembers.roofSolutions[i][j][3] * double.Parse(parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular " + structuralMembers.roof[i][j][x] + " [6m]"].ToString());
                                            }
                                        }
                                    }
                                }
                                else if (structuralMembers.roofSolutions[i][j][1] == 3)//cee purlins
                                {
                                    for (int x = 7; x < 9; x++)
                                    {
                                        if (x == 7)
                                        {
                                            if (parameters.price_RoofMaterials["C- Purlins (" + structuralMembers.roof[i][j][x] + ") [6 m]"] != null)
                                            {
                                                ceeRaft_price += structuralMembers.roofSolutions[i][j][2] * double.Parse(parameters.price_RoofMaterials["C- Purlins (" + structuralMembers.roof[i][j][x] + ") [6 m]"].ToString());
                                            }
                                            else
                                            {
                                                ceeRaft_price += structuralMembers.roofSolutions[i][j][2] * double.Parse(parameters.price_RoofMaterials["C- Purlins (" + structuralMembers.roof[i][j][x] + " ) [6 m]"].ToString());
                                            }
                                        }
                                        //150mm x 50mm x 1.0mm thick
                                        else
                                        {
                                            if (parameters.price_RoofMaterials["C- Purlins (" + structuralMembers.roof[i][j][x] + ") [6 m]"] != null)
                                            {
                                                ceePurlins_price += structuralMembers.roofSolutions[i][j][3] * double.Parse(parameters.price_RoofMaterials["C- Purlins (" + structuralMembers.roof[i][j][x] + ") [6 m]"].ToString());
                                            }
                                            else
                                            {
                                                ceePurlins_price += structuralMembers.roofSolutions[i][j][3] * double.Parse(parameters.price_RoofMaterials["C- Purlins (" + structuralMembers.roof[i][j][x] + " ) [6 m]"].ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    print("Roofings: " + ex);
                }
                
                print("Wood: " + wood_price);
                print("Steel rafters: " + steelRaft_price);
                print("Steel purlins: " + steelPurlins_price);
                print("C-Purlins Raft: " + ceeRaft_price);
                print("C-Purlins Purlins: " + ceePurlins_price);
                rANDp_MCost = wood_price + steelRaft_price + steelPurlins_price + ceeRaft_price + ceePurlins_price;
                rANDp_LCost = 0;
                rANDp_costTotal = rANDp_MCost + rANDp_LCost;
            }
            else
            {
                rANDp_MCost = 0;
                rANDp_LCost = 0;
                rANDp_costTotal = 0;
            }
            if (roofingsChecklist[1])
            {
                double corrugated = 0;
                double ginails = 0;
                double rivets = 0;
                double giwashers = 0;
                double leadwashers = 0;
                double umbnails = 0;
                double plain = 0;
                for (int i = 0; i < structuralMembers.roofSolutions.Count; i++)//floor
                {
                    for (int j = 0; j < structuralMembers.roofSolutions[i].Count; j++)// roofs each floor
                    {
                        if (structuralMembers.roofSolutions[i][j][0] == 2)//acce
                        {
                            corrugated += structuralMembers.roofSolutions[i][j][1] * double.Parse(parameters.price_RoofMaterials["Corrugated G.I Sheet, Gauge 26 (0.551mmx2.44 mm) [m2]"].ToString());
                            ginails += structuralMembers.roofSolutions[i][j][2] * double.Parse(parameters.price_RoofMaterials["G.I. Roof Nails [KG]"].ToString());
                            rivets += structuralMembers.roofSolutions[i][j][3] * double.Parse(parameters.price_RoofMaterials["G.I Rivets [KG]"].ToString()); 
                            giwashers += structuralMembers.roofSolutions[i][j][4] * double.Parse(parameters.price_RoofMaterials["G.I Washers [KG]"].ToString());
                            leadwashers += structuralMembers.roofSolutions[i][j][5] * double.Parse(parameters.price_RoofMaterials["G.I Washers [KG]"].ToString());
                            umbnails += structuralMembers.roofSolutions[i][j][6] * double.Parse(parameters.price_RoofMaterials["Umbrella Nails [KG]"].ToString());
                            plain += structuralMembers.roofSolutions[i][j][7] * double.Parse(parameters.price_RoofMaterials["Plain G.I Sheet, Gauge 24 (4ft x 8 ft) [UNIT]"].ToString());

                        }
                    }
                }                
                print("corrugeted: "+corrugated);
                print("GI nails: "+ ginails);
                print("Rivets: "+rivets);
                print("GI washers: "+ giwashers);
                print("Leadwashers: "+ leadwashers);
                print("umbrella nails: "+umbnails);
                print("Plain Sheets: "+plain);
                acce_MCost = corrugated + ginails + rivets + giwashers + leadwashers + umbnails + plain;
                acce_LCost = roof_LTOTAL;
                acce_costTotal = acce_MCost + acce_LCost;
                roof_MUNIT = acce_MCost / roof_QTY;
            }
            else
            {
                acce_MCost = 0;
                acce_LCost = 0;
                acce_costTotal = 0;
            }
            if (roofingsChecklist[2])
            {
                double tinswork_price = 0;
                for (int i = 0; i < structuralMembers.roofSolutions.Count; i++)//floor
                {
                    for (int j = 0; j < structuralMembers.roofSolutions[i].Count; j++)// roofs each floor
                    {
                        if (structuralMembers.roofSolutions[i][j][0] == 3)//tinswork
                        {
                            tinswork_price += structuralMembers.roofSolutions[i][j][1] * double.Parse(parameters.price_RoofMaterials["Plain G.I Sheet, Gauge 24 (4ft x 8 ft) [UNIT]"].ToString());
                        }
                    }
                }
                print("Tinswork: "+tinswork_price);
                tins_MCost = tinswork_price;
                tins_LCost = 0;
                tins_costTotal = tins_MCost + tins_LCost;
            }
            else
            {
                tins_MCost = 0;
                tins_LCost = 0;
                tins_costTotal = 0;
            }
            print("====== Roofings =====");
            roof_MTOTAL = rANDp_MCost + acce_MCost + tins_MCost;            
            roof_TOTALCOST = roof_MTOTAL + roof_LTOTAL;
            print("RandP Mat cost: " + rANDp_MCost + " RandP Lab cost: " + rANDp_LCost + " RandP TOTAL COST: "+ rANDp_costTotal);
            print("Acce Mat cost: " + acce_MCost + " Acce Lab cost: " + acce_LCost + " Acce TOTAL COST: " + acce_costTotal);
            print("Tins Mat cost: " + tins_MCost + " Tins Lab cost: " + tins_LCost + " Tins TOTAL COST: " + tins_costTotal);
            print("--- TOTAL ---");
            print("Materials Cost: " + roof_MTOTAL);
            print("Labor Cost: " + roof_LTOTAL);
            print("TOTAL COST: " + roof_TOTALCOST);
            //Roofings -- END

            //Tiles
            if (tilesChecklist[0])
            {
                try
                {                    
                    sixhun_MCost = (sixhun[1] * double.Parse(parameters.price_CommonMaterials["Tiles, Floor (600 x 600) [PC]"].ToString())) +
                        ((sixhun[2] * double.Parse(parameters.price_CommonMaterials["25KG Tile Adhesive (Regular) [BAGS]"].ToString())) + (sixhun[3] * double.Parse(parameters.price_CommonMaterials["25 KG Tile Adhesive (Heavy duty) [BAGS]"].ToString()))) + 
                        (double.Parse(parameters.price_CommonMaterials["Tile Grout (2KG)  [BAGS]"].ToString()) * sixhun[4]);
                    sixhun_MUnit = Math.Round(sixhun_MCost / sixhun[0],2);
                    sixhun_LCost = double.Parse(parameters.price_LaborRate_Tiles["TILES [m2]"].ToString()) * sixhun[0];
                    sixhun_costTotal = sixhun_MCost + sixhun_LCost;
                }
                catch
                {
                    sixhun_MCost = 0;
                    sixhun_MUnit = 0;
                    sixhun_LCost = 0;
                    sixhun_costTotal = 0;
                }
            }
            else
            {
                sixhun_MCost = 0;
                sixhun_MUnit = 0;
                sixhun_LCost = 0;
                sixhun_costTotal = 0;
            }
            if (tilesChecklist[1])
            {
                try
                {                    
                    threehun_MCost = (threehun[1] * double.Parse(parameters.price_CommonMaterials["Tiles, Wall (300 x 300) [PC]"].ToString())) +
                        ((threehun[2] * double.Parse(parameters.price_CommonMaterials["25KG Tile Adhesive (Regular) [BAGS]"].ToString())) + (threehun[3] * double.Parse(parameters.price_CommonMaterials["25 KG Tile Adhesive (Heavy duty) [BAGS]"].ToString()))) + 
                        (double.Parse(parameters.price_CommonMaterials["Tile Grout (2KG)  [BAGS]"].ToString()) * threehun[4]);
                    threehun_MUnit = Math.Round(threehun_MCost / threehun[0],2);
                    threehun_LCost = double.Parse(parameters.price_LaborRate_Tiles["TILES [m2]"].ToString()) * threehun[0];
                    threehun_costTotal = threehun_MCost + threehun_LCost;
                }
                catch
                {
                    threehun_MCost = 0;
                    threehun_MUnit = 0;
                    threehun_LCost = 0;
                    threehun_costTotal = 0;
                }                
            }
            else
            {
                threehun_MCost = 0;
                threehun_MUnit = 0;
                threehun_LCost = 0;
                threehun_costTotal = 0;
            }
            tiles_mTOTALCOST = threehun_MCost + sixhun_MCost;
            tiles_lTOTALCOST = threehun_LCost + sixhun_LCost;
            tiles_TOTALCOST = tiles_lTOTALCOST + tiles_mTOTALCOST;
            print("=== TILES ===");
            print("600 x 600:");
            print("M_Unit: "+sixhun_MUnit+ " M_Cost: "+sixhun_MCost+ " L_Cost: "+sixhun_LCost+ " Total Cost: "+sixhun_costTotal);
            print("300 x 300:");
            print("M_Unit: " + threehun_MUnit + " M_Cost: " + threehun_MCost + " L_Cost: " + threehun_LCost + " Total Cost: " + threehun_costTotal);
            print("--- TOTAL ---");
            print("M_Total:"+ tiles_mTOTALCOST);
            print("L_Total:" + tiles_lTOTALCOST);
            print("TOTAL COST:" + tiles_TOTALCOST);
            //Tiles -- END

            //Paints
            if (paintsChecklist[0])//enamel
            {
                try
                {
                    enam_MCost = (enamel[1] * double.Parse(parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"].ToString())) + 
                        (enamel[2] * double.Parse(parameters.price_PaintAndCoating["Skim Coat [BAGS]"].ToString())) + 
                        (enamel[3] * double.Parse(parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"].ToString())) + 
                        (enamel[4] * double.Parse(parameters.price_PaintAndCoating["Paint Enamel [GAL]"].ToString()));                    
                    enam_MUnit = (enam_MCost / enamel[0]) - ((enam_MCost / enamel[0])%.01);
                    enam_LCost = double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()) * enamel[0];
                    enam_TOTALCOST = enam_MCost + enam_LCost;
                }
                catch
                {
                    enam_MCost = 0;
                    enam_MUnit = 0;
                    enam_LCost = 0;
                    enam_TOTALCOST = 0;
                }
                
            }
            else
            {
                enam_MCost = 0;
                enam_MUnit = 0;
                enam_LCost = 0;
                enam_TOTALCOST = 0;
            }
            if (paintsChecklist[1])//acrylic
            {
                try
                {
                    acry_MCost = (acrylic[1] * double.Parse(parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"].ToString())) + 
                        (acrylic[2] * double.Parse(parameters.price_PaintAndCoating["Skim Coat [BAGS]"].ToString())) + 
                        (acrylic[3] * double.Parse(parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"].ToString())) + 
                        (acrylic[4] * double.Parse(parameters.price_PaintAndCoating["Paint, Acrylic 1 [GAL]"].ToString()));
                    acry_MUnit = (acry_MCost / acrylic[0]) -((acry_MCost / acrylic[0])%.01);
                    acry_LCost = double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()) * acrylic[0];
                    acry_TOTALCOST = acry_MCost + acry_LCost;
                }
                catch
                {
                    enam_MCost = 0;
                    enam_MUnit = 0;
                    enam_LCost = 0;
                    enam_TOTALCOST = 0;
                }                
            }
            else
            {
                acry_MCost = 0;
                acry_MUnit = 0;
                acry_LCost = 0;
                acry_TOTALCOST = 0;
            }
            if (paintsChecklist[2])//latex
            {
                try
                {
                    late_MCost = (latex[1] * double.Parse(parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"].ToString())) + 
                        (latex[2] * double.Parse(parameters.price_PaintAndCoating["Skim Coat [BAGS]"].ToString())) + 
                        (latex[3] * double.Parse(parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"].ToString())) + 
                        (latex[4] * double.Parse(parameters.price_PaintAndCoating["Paint Latex Gloss [GAL]"].ToString()));
                    late_MUnit = (late_MCost / latex[0]) - ((late_MCost / latex[0])%.01);
                    late_LCost = double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()) * latex[0];
                    late_TOTALCOST = late_MCost + late_LCost;
                }
                catch
                {
                    late_MCost = 0;
                    late_MUnit = 0;
                    late_LCost = 0;
                    late_TOTALCOST = 0;
                }                
            }
            else
            {
                late_MCost = 0;
                late_MUnit = 0;
                late_LCost = 0;
                late_TOTALCOST = 0;
            }
            if (paintsChecklist[3])//semi-gloss
            {
                try
                {
                    semi_MCost = (gloss[1] * double.Parse(parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"].ToString())) + 
                        (gloss[2] * double.Parse(parameters.price_PaintAndCoating["Skim Coat [BAGS]"].ToString())) + 
                        (gloss[3] * double.Parse(parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"].ToString())) + 
                        (gloss[4] * double.Parse(parameters.price_PaintAndCoating["Paint, Semi-Gloss [GALS]"].ToString()));
                    semi_MUnit = (semi_MCost / gloss[0]) - ((semi_MCost / gloss[0])%.01);
                    semi_LCost = double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()) * gloss[0];
                    semi_TOTALCOST = semi_MCost + semi_LCost;
                }
                catch
                {
                    semi_MCost = 0;
                    semi_MUnit = 0;
                    semi_LCost = 0;
                    semi_TOTALCOST = 0;
                }                
            }
            else
            {
                semi_MCost = 0;
                semi_MUnit = 0;
                semi_LCost = 0;
                semi_TOTALCOST = 0;
            }
            paint_mTOTALCOST = enam_MCost + acry_MCost + late_MCost + semi_MCost;
            paint_lTOTALCOST = enam_LCost + acry_LCost + late_LCost + semi_LCost;
            paints_TOTALCOST = paint_mTOTALCOST + paint_lTOTALCOST;
            print("==== PAINTS ====");
            print("ENAMEL ->");
            print("M_Unit: "+enam_MUnit+" M_Cost: "+enam_MCost+ " L_Cost: "+enam_LCost+" Enamel_TOTAL: "+ enam_TOTALCOST);
            print("ACRYLIC ->");
            print("M_Unit: " + acry_MUnit + " M_Cost: " + acry_MCost + " L_Cost: " + acry_LCost + " Acrylic_TOTAL: " + acry_TOTALCOST);
            print("LATEX ->");
            print("M_Unit: " + late_MUnit + " M_Cost: " + late_MCost + " L_Cost: " + late_LCost + " Latex_TOTAL: " + late_TOTALCOST);
            print("GLOSS ->");
            print("M_Unit: " + semi_MUnit + " M_Cost: " + semi_MCost + " L_Cost: " + semi_LCost + " Semi-gloss_TOTAL: " + semi_TOTALCOST);
            print("--- TOTAL ---");
            print("Material Cost: " + paint_mTOTALCOST);
            print("Labot Cost: " + paint_lTOTALCOST);
            print("TOTAL Cost: " + paints_TOTALCOST);

            //Paints -- END

            //9.0 - Miscellaneous Items -- START NICE
            misc_TOTALCOST = 0;
            foreach (string[] data in parameters.misc_CustomItems)
            {
                if (bool.Parse(data[3]))
                {
                    misc_TOTALCOST += (double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[2], System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            //9.0 - Miscellaneous Items -- END

            //10.0 - Additional Labor and Equipment -- START
            laborAndEqpt_TOTALCOST = 0;
            //Manpower
            if (parameters.labor_RD.Equals("Manila Rate"))
            {
                foreach (string[] data in parameters.labor_MP)
                {
                    if (bool.Parse(data[4]))
                    {
                        LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_ManpowerM[data[0]]);
                        laborAndEqpt_TOTALCOST += content.totalPrice;
                    }
                }
            }
            else //Provincial
            {
                foreach (string[] data in parameters.labor_MP) 
                {
                    if (bool.Parse(data[4]))
                    {
                        LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_ManpowerP[data[0]]);
                        laborAndEqpt_TOTALCOST += content.totalPrice;
                    }
                }
            }
            //Equipment
            foreach (string[] data in parameters.labor_EQP) 
            {
                if (bool.Parse(data[4]))
                {
                    LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_Equipment[data[0]]);
                    laborAndEqpt_TOTALCOST += content.totalPrice;
                }
            }
            //10.0 - Additional Labor and Equipment -- END
            //Cost Computation - END
            //Setting View -- START
            if (!viewInitalized)
            {
                earthworks_TotalCostM = backfillingAndCompaction_CostM + gravelBedding_CostM + soilPoisoning_CostM;
                earthworks_TotalCostL = excavation_CostL + backfillingAndCompaction_CostL + gradingAndCompaction_CostL + gravelBedding_CostL;
                earthworks_CostTotal = earthworks_TotalCostM + earthworks_TotalCostL;

                //Computation of Total Cost fos_LC_Type, fos_LC_Percentage
                if (fos_LC_Type.Equals("Rate"))
                {
                    TOTALCOST = earthworks_CostTotal + concrete_TOTALCOST + FW_MATOTAL + mason_TOTALCOST +
                                rein_TotalCost + roof_TOTALCOST + tiles_TOTALCOST + paints_TOTALCOST +
                                misc_TOTALCOST + laborAndEqpt_TOTALCOST;
                }  
                else if (fos_LC_Type.Equals("Percentage"))
                {
                    TOTALCOST = earthworks_TotalCostM + concreteMCost_total + FW_totalMats + masonMCost_total +
                                rein_MCost + roof_MTOTAL + tiles_mTOTALCOST + paint_mTOTALCOST +
                                misc_TOTALCOST + laborAndEqpt_TOTALCOST;
                    string percentString = fos_LC_Percentage.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
                    double percentage = double.Parse(percentString, System.Globalization.CultureInfo.InvariantCulture) / 100;
                    TOTALCOST = TOTALCOST + (TOTALCOST * percentage);
                }
                else
                {
                    TOTALCOST = earthworks_TotalCostM + concreteMCost_total + FW_totalMats + masonMCost_total +
                                rein_MCost + roof_MTOTAL + tiles_mTOTALCOST + paint_mTOTALCOST +
                                misc_TOTALCOST + laborAndEqpt_TOTALCOST;
                }

                //Set Labels total cost
                totalcostLbl.Text = "₱" + TOTALCOST.ToString("#,##0.00");

                //Set Total Costs - Labor Cost depending on Labor Costing Type
                if (!fos_LC_Type.Equals("Rate"))
                {
                    //BOQ Hide
                    this.summ_BOQ_dg.Columns["labor1"].Visible = false;
                    this.summ_BOQ_dg.Columns["labor2"].Visible = false;

                    //1.0 - Earthworks
                    excavation_CostL = 0;
                    backfillingAndCompaction_CostTotal = backfillingAndCompaction_CostTotal - backfillingAndCompaction_CostL;
                    gravelBedding_CostTotal = gravelBedding_CostTotal - gravelBedding_CostL;
                    gradingAndCompaction_CostL = 0;
                    earthworks_CostTotal = earthworks_CostTotal - earthworks_TotalCostL;

                    //2.0 - Concrete
                    concreteF_costTotal -= concreteF_CostL;
                    concreteC_costTotal -= concreteC_CostL;
                    concreteBR_costTotal -= concreteBR_CostL;
                    concreteSOG_costTotal -= concreteSOG_CostL;
                    concreteSS_costTotal -= concreteSS_CostL;
                    concreteST_costTotal -= concreteST_CostL;
                    concrete_TOTALCOST -= concreteLCost_total;

                    //3.0 - Formworks
                    footingMLCOST -= footing_Lcost;
                    trapMLCOST -= trap_Lcost;
                    colMLCOST -= col_Lcost;
                    beamMLCOST -= beam_Lcost;
                    sus_MLCOST -= sus_Lcost;
                    stairs_MLCOST -= stairs_Lcost;
                    FW_MATOTAL -= FW_totalLab;

                    //4.0 - Masonry
                    exterior_costTotal -= exterior_CostL;
                    interior_costTotal -= interior_CostL;
                    mason_TOTALCOST -= masonLCost_total;

                    //5.0 - Rebars
                    reinF_TotalCost -= reinF_CostL;
                    reinWF_TotalCost -= reinWF_CostL;
                    Rbeam_TOTALCOST -= Rbeam_Lcost;
                    RSOG_TOTALCOST -= RSOG_Lcost;
                    RSS_TOTALCOST -= RSS_Lcost;
                    RCOL_TOTALCOST -= RCOL_Lcost;
                    RSTAIRS_TOTALCOST -= RSTAIRS_Lcost;
                    RWALLS_TOTALCOST -= RWALLS_Lcost;
                    rein_TotalCost -= rein_LCost;

                    //6.0 - Roofings
                    rANDp_costTotal -= rANDp_LCost;
                    acce_costTotal -= reinWF_CostL;
                    tins_costTotal -= tins_LCost;
                    roof_TOTALCOST -= roof_LTOTAL;

                    //7.0 - Tiles
                    sixhun_costTotal -= sixhun_LCost;
                    threehun_costTotal -= threehun_LCost;
                    tiles_TOTALCOST -= tiles_lTOTALCOST;

                    //8.0 - Paint
                    enam_TOTALCOST -= enam_LCost;
                    acry_TOTALCOST -= acry_LCost;
                    late_TOTALCOST -= late_LCost;
                    semi_TOTALCOST -= semi_LCost;
                    paints_TOTALCOST -= paint_lTOTALCOST;

                    //10.0 - Additional Labor and Equipment
                    laborAndEqpt_TOTALCOST = 0;
                }
                else
                {
                    //BOQ Show
                    this.summ_BOQ_dg.Columns["labor1"].Visible = true;
                    this.summ_BOQ_dg.Columns["labor2"].Visible = true;
                }

                //For refresh
                view_TV1.Nodes.Clear();
                view_TV2.Nodes.Clear();
                view_TV3.Nodes.Clear();
                this.summ_BOQ_dg.Rows.Clear(); 

                //Tree View 1 - Earthworks, Concrete Works, Form Works, and Masonry
                List<TreeNode> nodes1;
                TreeNode tn1 = new TreeNode("1.0 Earthworks (₱" + earthworks_CostTotal.ToString("#,##0.00") + ")");
                tn1.Name = "earthworksParent";

                TreeNode tn2 = new TreeNode("2.0 Concrete Works (₱" + concrete_TOTALCOST.ToString("#,##0.00") + ")");
                tn2.Name = "concreteWorksParent";

                TreeNode tn3 = new TreeNode("3.0 Form Works (₱" + FW_MATOTAL.ToString("#,##0.00") + ")");
                tn3.Name = "formWorksParent";

                TreeNode tn4 = new TreeNode("4.0 Masonry (₱" + mason_TOTALCOST.ToString("#,##0.00") + ")");
                tn4.Name = "masonryParent";

                nodes1 = new List<TreeNode>() { tn1, tn2, tn3, tn4 };

                setTree(nodes1, view_TV1);

                //1.0 - Earthworks -- START
                int j = 1;
                TreeNode[] found = view_TV1.Nodes.Find("earthworksParent", true);

                TreeNode newChild1 = new TreeNode("1." + j + " Excavation (₱" + excavation_CostL.ToString("#,##0.00") + ")");
                newChild1.Name = "excavation_Total";

                if (excavation_CostL != 0) j++;

                TreeNode newChild2 = new TreeNode("1." + j + " Back Filling and Compaction (₱" + backfillingAndCompaction_CostTotal.ToString("#,##0.00") + ")");
                newChild2.Name = "backfillingAndCompaction_Total";

                if (backfillingAndCompaction_CostTotal != 0) j++;

                TreeNode newChild3 = new TreeNode("1." + j + " Grading and Compaction (₱" + gradingAndCompaction_CostL.ToString("#,##0.00") + ")");
                newChild3.Name = "gradingAndCompaction_Total";

                if (gradingAndCompaction_CostL != 0) j++;

                TreeNode newChild4 = new TreeNode("1." + j + " Gravel Bedding and Compaction (₱" + gravelBedding_CostTotal.ToString("#,##0.00") + ")");
                newChild4.Name = "gravelBedding_Total";

                if (gravelBedding_CostTotal != 0) j++;

                TreeNode newChild5 = new TreeNode("1." + j + " Soil Poisoning (₱" + soilPoisoning_CostM.ToString("#,##0.00") + ")");
                newChild5.Name = "soilPoisoning_Total";

                if (excavation_CostL != 0)
                    found[0].Nodes.Add(newChild1);
                if (backfillingAndCompaction_CostTotal != 0)
                    found[0].Nodes.Add(newChild2);
                if (gradingAndCompaction_CostL != 0)
                    found[0].Nodes.Add(newChild3);
                if (gravelBedding_CostTotal != 0)
                    found[0].Nodes.Add(newChild4);
                if (soilPoisoning_CostM != 0)
                    found[0].Nodes.Add(newChild5);

                //Setting of BOQ     
                if (earthworks_CostTotal != 0)
                {
                    var index = this.summ_BOQ_dg.Rows.Add();
                    this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "1.0";
                    this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "EARTHWORKS";
                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                    int i = 1;
                    if (excavation_CostL != 0)
                    {
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "1." + i;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "Excavation";
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = excavation_Total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Earthworks["Excavation [m3]"].ToString()).ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + excavation_CostL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + excavation_CostL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        i++;
                    }
                    if (backfillingAndCompaction_CostTotal != 0)
                    {
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "1." + i;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "Backfilling and Compaction";
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = backfillingAndCompaction_Total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                        if(backfillingAndCompaction_CostM != 0)
                        {
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + double.Parse(parameters.price_Embankment["Common Borrow [m3]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + backfillingAndCompaction_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Earthworks["Backfilling and Compaction [m3]"].ToString()).ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + backfillingAndCompaction_CostL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + backfillingAndCompaction_CostTotal.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        i++;
                    }
                    if (gradingAndCompaction_CostL != 0)
                    {
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "1." + i;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "Grading and Compaction";
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = gradingAndCompaction_Total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Earthworks["Grading and Compaction [m3]"].ToString()).ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + gradingAndCompaction_CostL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + gradingAndCompaction_CostL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        i++;
                    }
                    if (gravelBedding_CostTotal != 0)
                    {
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "1." + i;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "Gravel bedding and Compaction";
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = gravelBedding_Total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m."; 
                        this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + double.Parse(parameters.price_Gravel[earthworks_gravelBeddingType].ToString()).ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + gravelBedding_CostM.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Earthworks["Gravel Bedding and Compaction [m3]"].ToString()).ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + gravelBedding_CostL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + gravelBedding_CostTotal.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        i++;
                    }
                    if (soilPoisoning_CostM != 0)
                    {
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "1." + i;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "Soil Poisoning";
                        this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = soilPoisoning_Total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                        this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Earthworks["Soil Poisoning [m2]"].ToString()).ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + soilPoisoning_CostM.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + soilPoisoning_CostM.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        i++;
                    }

                    this.summ_BOQ_dg.Rows.Add();
                    index = this.summ_BOQ_dg.Rows.Add();
                    this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + earthworks_TotalCostM.ToString("#,##0.00");
                    this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + earthworks_TotalCostL.ToString("#,##0.00");
                    this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + earthworks_CostTotal.ToString("#,##0.00");
                    this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                }
                //1.0 - Earthworks -- END
                
                //2.0 - Concrete Works -- START
                found = view_TV1.Nodes.Find("concreteWorksParent", true);

                j = 1;
                TreeNode column_newChild1 = new TreeNode("2." + j + " Footing (₱" + concreteF_CostM.ToString("#,##0.00") + ")");
                newChild1.Name = "concreting_Footing";
                if (concreteF_CostM != 0) j++;
                TreeNode column_newChild2 = new TreeNode("2." + j + " Column (₱" + concreteC_CostM.ToString("#,##0.00") + ")");
                newChild2.Name = "concreting_Column";
                if (concreteC_CostM != 0) j++;
                TreeNode column_newChild3 = new TreeNode("2." + j + " Beam (₱" + concreteBR_CostM.ToString("#,##0.00") + ")");
                newChild3.Name = "concreting_Beam";
                if (concreteBR_CostM != 0) j++;
                TreeNode column_newChild4 = new TreeNode("2." + j + " Slab (₱" + (concreteSOG_CostM + concreteSS_CostM).ToString("#,##0.00") + ")");
                newChild4.Name = "concreting_Slab";
                if ((concreteSOG_CostM + concreteSS_CostM) != 0) j++;
                TreeNode column_newChild5 = new TreeNode("2." + j + " Stairs (₱" + concreteST_CostM.ToString("#,##0.00") + ")");
                newChild5.Name = "concreting_Stairs";
                if (concreteST_CostM != 0) j++;
                TreeNode column_newChild6 = new TreeNode("2." + j + " Factor of Safety (₱" + concreteFS_CostM.ToString("#,##0.00") + ")");
                column_newChild6.Name = "concreting_FS";
                if (concreteFS_CostM != 0) j++;
                TreeNode column_newChild7 = new TreeNode("2." + j + " Labor for Concrete Works (₱" + concreteLCost_total.ToString("#,##0.00") + ")");
                column_newChild7.Name = "concreting_Labor";

                if (concreteF_CostM != 0)
                    found[0].Nodes.Add(column_newChild1);
                if (concreteC_CostM != 0)
                    found[0].Nodes.Add(column_newChild2);
                if (concreteBR_CostM != 0)
                    found[0].Nodes.Add(column_newChild3);
                if ((concreteSOG_CostM + concreteSS_CostM) != 0)
                    found[0].Nodes.Add(column_newChild4);
                if (concreteST_CostM != 0)
                    found[0].Nodes.Add(column_newChild5);
                if (concreteFS_CostM != 0)
                    found[0].Nodes.Add(column_newChild6);
                if (concreteLCost_total != 0)
                    found[0].Nodes.Add(column_newChild7);

                //Setting of BOQ     
                try
                {
                    if (concrete_TOTALCOST != 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "2.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "CONCRETE WORKS";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        if (concreteF_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "2." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            string PSI = "";
                            if (!parameters.conc_cmIsSelected[0])
                            {
                                PSI = parameters.conc_CM_F_RM.Substring(20, 7);
                            }
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "FOOTINGS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "FOOTING AND WALL FOOTING (" + PSI + ")";
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = concreteVolume_Total[0].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + concreteF_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteF_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Concreting["FOOTING [m3]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + concreteF_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concreteF_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (concreteC_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "2." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            string PSI = "";
                            if (!parameters.conc_cmIsSelected[1])
                            {
                                PSI = parameters.conc_CM_C_RM.Substring(20, 7);
                            }
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "COLUMNS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "COLUMNS (" + PSI + ")";
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = concreteVolume_Total[1].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + concreteC_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteC_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Concreting["COLUMN [m3]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + concreteC_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concreteC_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (concreteBR_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "2." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            string PSI = "";
                            if (!parameters.conc_cmIsSelected[2])
                            {
                                PSI = parameters.conc_CM_B_RM.Substring(20, 7);
                            }
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "BEAMS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "BEAMS (" + PSI + ")";
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = concreteVolume_Total[2].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + concreteBR_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteBR_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Concreting["BEAM [m3]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + concreteBR_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concreteBR_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if ((concreteSOG_costTotal + concreteSS_costTotal) != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "2." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "SLAB";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            index = this.summ_BOQ_dg.Rows.Add();
                            string PSI = "";
                            if (!parameters.conc_cmIsSelected[3])
                            {
                                PSI = parameters.conc_CM_S_SOG_RM.Substring(20, 7);
                            }
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "SLAB ON GRADE (" + PSI + ")";
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = concreteVolume_Total[3].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + concreteSOG_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteSOG_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Concreting["SLAB ON GRADE [m3]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + concreteSOG_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concreteSOG_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            index = this.summ_BOQ_dg.Rows.Add();
                            PSI = "";
                            if (!parameters.conc_cmIsSelected[4])
                            {
                                PSI = parameters.conc_CM_S_SS_RM.Substring(20, 7);
                            }
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "SUSPENDED SLAB (" + PSI + ")";
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = concreteVolume_Total[4].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + concreteSS_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteSS_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Concreting["SUSPENDED SLAB [m3]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + concreteSS_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concreteSS_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (concreteST_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "2." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            string PSI = "";
                            if (!parameters.conc_cmIsSelected[5])
                            {
                                PSI = parameters.conc_CM_ST_RM.Substring(20, 7);
                            }
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "STAIRS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "STAIRS (" + PSI + ")";
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = concreteVolume_Total[5].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "cu. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + concreteST_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteST_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Concreting["STAIRS [m3]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + concreteST_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concreteST_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (concreteFS_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "2." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "FACTOR OF SAFETY";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteFS_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concreteFS_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + concreteMCost_total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + concreteLCost_total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + concrete_TOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //2.0 - Concrete Works -- END

                //3.0 - Formworks -- START
                found = view_TV1.Nodes.Find("formWorksParent", true);

                j = 1;
                TreeNode fw_newChild1 = new TreeNode("3." + j + " Footing (₱" + (footingMLCOST + trapMLCOST).ToString("#,##0.00") + ")");
                newChild1.Name = "fw_Footing";
                if ((footingMLCOST + trapMLCOST) != 0) j++;
                TreeNode fw_newChild2 = new TreeNode("3." + j + " Column (₱" + colMLCOST.ToString("#,##0.00") + ")");
                newChild2.Name = "fw_Column";
                if (concreteC_CostM != 0) j++;
                TreeNode fw_newChild3 = new TreeNode("3." + j + " Beams (₱" + beamMLCOST.ToString("#,##0.00") + ")");
                newChild3.Name = "fw_Beams";
                if (concreteBR_CostM != 0) j++;
                TreeNode fw_newChild4 = new TreeNode("3." + j + " Suspended Slab (₱" + sus_MLCOST.ToString("#,##0.00") + ")");
                newChild4.Name = "fw_Slab";
                if (sus_MLCOST != 0) j++;
                TreeNode fw_newChild5 = new TreeNode("3." + j + " Stairs (₱" + stairs_MLCOST.ToString("#,##0.00") + ")");
                newChild5.Name = "fw_Stairs";
                if (stairs_MLCOST != 0) j++;
                TreeNode fw_newChild6 = new TreeNode("3." + j + " Nails (₱" + nailsMLCOST.ToString("#,##0.00") + ")");
                fw_newChild6.Name = "fw_Nails";

                if ((footingMLCOST + trapMLCOST) != 0)
                    found[0].Nodes.Add(fw_newChild1);
                if (colMLCOST != 0)
                    found[0].Nodes.Add(fw_newChild2);
                if (beamMLCOST != 0)
                    found[0].Nodes.Add(fw_newChild3);
                if (sus_MLCOST != 0)
                    found[0].Nodes.Add(fw_newChild4);
                if (stairs_MLCOST != 0)
                    found[0].Nodes.Add(fw_newChild5);
                if (nailsMLCOST != 0)
                    found[0].Nodes.Add(fw_newChild6);

                //Setting of BOQ     
                try
                {
                    if (FW_MATOTAL != 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "3.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "FORM WORKS";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        if ((footingMLCOST + trapMLCOST) != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "3." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "FOOTING";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = (footing_QTY + trap_QTY).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + ((footing_Mcost + trap_Mcost) / (footing_QTY + trap_QTY)).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + (footing_Mcost + trap_Mcost).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + footing_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + (footing_Lcost + trap_Lcost).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + (footingMLCOST + trapMLCOST).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (colMLCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "3." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "COLUMN";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = col_QTY.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + col_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + col_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + col_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + col_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + colMLCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (beamMLCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "3." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "BEAMS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = beam_QTY.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + beam_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + beam_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + beam_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + beam_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + beamMLCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (sus_MLCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "3." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "SUSPENDED SLAB";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = sus_QTY.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + sus_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + sus_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + sus_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + sus_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + sus_MLCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (stairs_MLCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "3." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "STAIRS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = stairs_QTY.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + stairs_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + stairs_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + stairs_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + stairs_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + stairs_MLCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (nailsMLCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "3." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "NAILS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = nails_QTY.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + nails_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + nails_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + nailsMLCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + FW_totalMats.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + FW_totalLab.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + FW_MATOTAL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //3.0 - Formworks -- END
                //*/
                //4.0 - Masonry -- START
                found = view_TV1.Nodes.Find("masonryParent", true);

                j = 1;
                TreeNode M_newChild1 = new TreeNode("4." + j + " Exterior Walls (₱" + exterior_costTotal.ToString("#,##0.00") + ")");
                newChild1.Name = "exterior_Wall_Total";
                if (exterior_costTotal != 0) j++;
                TreeNode M_newChild2 = new TreeNode("4." + j + " Interior Walls (₱" + interior_costTotal.ToString("#,##0.00") + ")");
                newChild2.Name = "interior_Wall_Total";

                if (exterior_costTotal != 0)
                    found[0].Nodes.Add(M_newChild1);
                if (interior_costTotal != 0)
                    found[0].Nodes.Add(M_newChild2);

                //Setting of BOQ     
                try
                {
                    if (mason_TOTALCOST != 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "4.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "MASONRY";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        if (exterior_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "4." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "Exterior Walls";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = masonrysSolutionP1[3].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + exterior_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + exterior_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Masonry["MASONRY [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + exterior_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + exterior_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (interior_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "4." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "Interior Walls";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = masonrysSolutionP1[8].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + interior_UnitM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + interior_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Masonry["MASONRY [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + interior_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + interior_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }

                        this.summ_BOQ_dg.Rows.Add();
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + masonMCost_total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + masonLCost_total.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + mason_TOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                } 
                catch (Exception ex)
                {
                    
                }
                //4.0 - Masonry -- END

                //Tree View 2 - Reinforcement Steel, Roofing, Tile Works, Paint Works
                List<TreeNode> nodes2;
                TreeNode tn1_2 = new TreeNode("5.0 Reinforcement Steel");
                tn1_2.Name = "reinParent";

                TreeNode tn2_2 = new TreeNode("6.0 Roofing (₱" + roof_TOTALCOST.ToString("#,##0.00") + ")");
                tn2_2.Name = "roofingParent";

                TreeNode tn3_2 = new TreeNode("7.0 Tiling Works (₱" + tiles_TOTALCOST.ToString("#,##0.00") + ")");
                tn3_2.Name = "tilingParent";

                TreeNode tn4_2 = new TreeNode("8.0 Paint Works (₱" + paints_TOTALCOST.ToString("#,##0.00") + ")");
                tn4_2.Name = "paintParent";
                
                nodes2 = new List<TreeNode>() { tn1_2, tn2_2, tn3_2, tn4_2 };

                setTree(nodes2, view_TV2);


                //5.0 - Reinforcement Steel -- START
                found = view_TV2.Nodes.Find("reinParent", true);

                j = 1;
                TreeNode R_newChild1 = new TreeNode("5." + j + " Footing (₱" + reinF_TotalCost.ToString("#,##0.00") + ")");
                R_newChild1.Name = "reinF_Total";
                if (reinF_TotalCost != 0) j++;
                TreeNode R_newChild2 = new TreeNode("5." + j + " Wall Footing (₱" + reinWF_TotalCost.ToString("#,##0.00") + ")");
                R_newChild2.Name = "reinWF_Total";
                if (reinWF_TotalCost != 0) j++;
                TreeNode R_newChild3 = new TreeNode("5." + j + " Column (₱" + RCOL_TOTALCOST.ToString("#,##0.00") + ")");
                R_newChild3.Name = "reinC_Total";
                if (RCOL_TOTALCOST != 0) j++;
                TreeNode R_newChild4 = new TreeNode("5." + j + " Beam (₱" + Rbeam_TOTALCOST.ToString("#,##0.00") + ")");
                R_newChild4.Name = "reinB_Total";
                if (Rbeam_TOTALCOST != 0) j++;
                TreeNode R_newChild5 = new TreeNode("5." + j + " Slab on Grade (₱" + RSOG_TOTALCOST.ToString("#,##0.00") + ")");
                R_newChild5.Name = "reinSOG_Total";
                if (RSOG_TOTALCOST != 0) j++;
                TreeNode R_newChild6 = new TreeNode("5." + j + " Suspended Slab (₱" + RSS_TOTALCOST.ToString("#,##0.00") + ")");
                R_newChild6.Name = "reinSS_Total";
                if (RSS_TOTALCOST != 0) j++;
                TreeNode R_newChild7 = new TreeNode("5." + j + " Stairs (₱" + RSTAIRS_TOTALCOST.ToString("#,##0.00") + ")");
                R_newChild7.Name = "reinST_Total";
                if (RSTAIRS_TOTALCOST != 0) j++;
                TreeNode R_newChild8 = new TreeNode("5." + j + " Walls (₱" + RWALLS_TOTALCOST.ToString("#,##0.00") + ")");
                R_newChild8.Name = "reinW_Total";

                if (reinF_TotalCost != 0)
                    found[0].Nodes.Add(R_newChild1);
                if (reinWF_TotalCost != 0)
                    found[0].Nodes.Add(R_newChild2);
                if (RCOL_TOTALCOST != 0)
                    found[0].Nodes.Add(R_newChild3);
                if (Rbeam_TOTALCOST != 0)
                    found[0].Nodes.Add(R_newChild4);
                if (RSOG_TOTALCOST != 0)
                    found[0].Nodes.Add(R_newChild5);
                if (RSS_TOTALCOST != 0)
                    found[0].Nodes.Add(R_newChild6);
                if (RSTAIRS_TOTALCOST != 0)
                    found[0].Nodes.Add(R_newChild7); 
                if (RWALLS_TOTALCOST != 0)
                    found[0].Nodes.Add(R_newChild8);

                //Setting of BOQ     
                try
                {
                    if (rein_TotalCost != 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "5.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "REINFORCEMENT STEEL";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        if (reinF_TotalCost != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "FOOTING";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = reinF_QTY.ToString("#,##0.00"); 
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + (reinF_CostM / reinF_QTY).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + reinF_CostM.ToString("#,##0.00"); 
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Rebar["FOOTING [KG]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + reinF_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + reinF_TotalCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (reinWF_TotalCost != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "WALL FOOTING";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = reinWF_QTY.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + (reinWF_CostM / reinWF_QTY).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + reinWF_CostM.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Rebar["WALL FOOTING [KG]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + reinWF_CostL.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + reinWF_TotalCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (RCOL_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "COLUMN";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = RCOL_qty.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + RCOL_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + RCOL_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + RCOL_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + RCOL_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + RCOL_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (Rbeam_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "BEAM";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = Rbeam_qty.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + Rbeam_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + Rbeam_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + Rbeam_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + Rbeam_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + Rbeam_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (RSOG_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "SLAB ON GRADE";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = RSOG_qty.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + RSOG_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + RSOG_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + RSOG_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + RSOG_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + RSOG_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (RSS_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "SUSPENDED SLAB";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = RSS_qty.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + RSS_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + RSS_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + RSS_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + RSS_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + RSS_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (RSTAIRS_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "STAIRS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = RSTAIRS_qty.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + RSTAIRS_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + RSTAIRS_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + RSTAIRS_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + RSTAIRS_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + RSTAIRS_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (RWALLS_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "5." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "WALLS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = RWALLS_qty.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "kg";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + RWALLS_Munit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + RWALLS_Mcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + RWALLS_Lunit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + RWALLS_Lcost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + RWALLS_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        this.summ_BOQ_dg.Rows.Add();
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + rein_MCost.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + rein_LCost.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + rein_TotalCost.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //5.0 - Reinforcement Steel -- END

                //6.0 - Roofing -- START
                found = view_TV2.Nodes.Find("roofingParent", true);

                j = 1;
                TreeNode roof_newChild1 = new TreeNode("6." + j + " G.I. and Accessories (₱" + acce_costTotal.ToString("#,##0.00") + ")");
                roof_newChild1.Name = "GI_and_Accessories_Total";
                if (acce_costTotal != 0) j++;
                TreeNode roof_newChild2 = new TreeNode("6." + j + " Rafters and Purlins (₱" + rANDp_costTotal.ToString("#,##0.00") + ")");
                roof_newChild2.Name = "Rafters_and_Purlins_Total";
                if (rANDp_costTotal != 0) j++;
                TreeNode roof_newChild3 = new TreeNode("6." + j + " Tinswork (₱" + tins_costTotal.ToString("#,##0.00") + ")");
                roof_newChild3.Name = "Tinswork_Total";

                if (acce_costTotal != 0)
                    found[0].Nodes.Add(roof_newChild1);
                if (rANDp_costTotal != 0)
                    found[0].Nodes.Add(roof_newChild2);
                if (tins_costTotal != 0)
                    found[0].Nodes.Add(roof_newChild3);

                //Setting of BOQ     
                try
                {
                    if (roof_TOTALCOST != 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "6.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "ROOFING";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        if (acce_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "6." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "G.I. and ACCESSORIES";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = roof_QTY.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + roof_MUNIT.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + acce_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Roofings["ROOFINGS [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + acce_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + acce_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (rANDp_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "6." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "RAFTERS AND PURLINS";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + rANDp_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + rANDp_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + rANDp_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (tins_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "6." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "TINSWORK";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + tins_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + tins_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + tins_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }

                        this.summ_BOQ_dg.Rows.Add();
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + roof_MTOTAL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + roof_LTOTAL.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + roof_TOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //6.0 - Roofing -- END

                //7.0 - Tile Works -- START
                found = view_TV2.Nodes.Find("tilingParent", true);

                j = 1;
                TreeNode tile_newChild1 = new TreeNode("7." + j + " 600 x 600 (₱" + sixhun_costTotal.ToString("#,##0.00") + ")");
                tile_newChild1.Name = "600_Total";
                if (sixhun_costTotal != 0) j++;
                TreeNode tile_newChild2 = new TreeNode("7." + j + " 300 x 300 (₱" + threehun_costTotal.ToString("#,##0.00") + ")");
                tile_newChild2.Name = "300_Total";

                if (sixhun_costTotal != 0)
                    found[0].Nodes.Add(tile_newChild1);
                if (threehun_costTotal != 0)
                    found[0].Nodes.Add(tile_newChild2);

                //Setting of BOQ   
                try
                {
                    if (tiles_TOTALCOST != 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "7.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "TILE WORKS";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        if (sixhun_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "7." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "600 x 600 TILE";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = sixhun[0].ToString("#,##0.00"); 
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + sixhun_MUnit.ToString("#,##0.00"); 
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + sixhun_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Tiles["TILES [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + sixhun_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + sixhun_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (threehun_costTotal != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "7." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "300 x 300 TILE";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = threehun[0].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + threehun_MUnit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + threehun_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Tiles["TILES [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + threehun_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + threehun_costTotal.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }

                        this.summ_BOQ_dg.Rows.Add();
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + tiles_mTOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + tiles_lTOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + tiles_TOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //7.0 - Tile Works -- END

                //8.0 - Paint Works -- START
                found = view_TV2.Nodes.Find("paintParent", true);

                j = 1;
                TreeNode paint_newChild1 = new TreeNode("8." + j + " ENAMEL PAINT (₱" + enam_TOTALCOST.ToString("#,##0.00") + ")");
                paint_newChild1.Name = "Enamel_Paint_Total";
                if (enam_TOTALCOST != 0) j++;
                TreeNode paint_newChild2 = new TreeNode("8." + j + " ACRYLIC PAINT (₱" + acry_TOTALCOST.ToString("#,##0.00") + ")");
                paint_newChild2.Name = "Acrylic_Paint_Total";
                if (acry_TOTALCOST != 0) j++;
                TreeNode paint_newChild3 = new TreeNode("8." + j + " LATEX PAINT (₱" + late_TOTALCOST.ToString("#,##0.00") + ")");
                paint_newChild3.Name = "Latex_Paint_Total";
                if (late_TOTALCOST != 0) j++;
                TreeNode paint_newChild4 = new TreeNode("8." + j + " SEMI-GLOSS PAINT (₱" + semi_TOTALCOST.ToString("#,##0.00") + ")");
                paint_newChild4.Name = "SemiGloss_Paint_Total";

                if (enam_TOTALCOST != 0)
                    found[0].Nodes.Add(paint_newChild1);
                if (acry_TOTALCOST != 0)
                    found[0].Nodes.Add(paint_newChild2);
                if (late_TOTALCOST != 0)
                    found[0].Nodes.Add(paint_newChild3);
                if (semi_TOTALCOST != 0)
                    found[0].Nodes.Add(paint_newChild4);

                //Setting of BOQ   
                try
                {
                    if (paints_TOTALCOST != 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "8.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "PAINT WORKS";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        if (enam_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "8." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "ENAMEL PAINT";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = enamel[0].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + enam_MUnit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + enam_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + enam_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + enam_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (acry_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "8." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "ACRYLIC PAINT";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = acrylic[0].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + acry_MUnit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + acry_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + acry_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + acry_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (late_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "8." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "LATEX PAINT";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = latex[0].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + late_MUnit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + late_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + late_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + late_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }
                        if (semi_TOTALCOST != 0)
                        {
                            index = this.summ_BOQ_dg.Rows.Add();
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "8." + i;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = "SEMI-GLOSS PAINT";
                            this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = gloss[0].ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "sq. m.";
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + semi_MUnit.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + semi_MCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString()).ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + semi_LCost.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + semi_TOTALCOST.ToString("#,##0.00");
                            this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            i++;
                        }

                        this.summ_BOQ_dg.Rows.Add();
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + paint_mTOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + paint_lTOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + paints_TOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //8.0 - Paint Works -- END

                //Tree View 3 - Miscellaneous Items and Additional Labor and Equipment
                List<TreeNode> nodes3;
                TreeNode tn1_3 = new TreeNode("9.0 Miscellaneous Items (₱" + misc_TOTALCOST.ToString("#,##0.00") + ")");
                tn1_3.Name = "miscParent";

                TreeNode tn2_3 = new TreeNode("10.0 Additional Labor and Equipment (₱" + laborAndEqpt_TOTALCOST.ToString("#,##0.00") + ")");
                tn2_3.Name = "laborAndEquipmentParent";

                nodes3 = new List<TreeNode>() { tn1_3, tn2_3 };

                setTree(nodes3, view_TV3);

                //9.0 - Miscellaneous Items -- START
                found = view_TV3.Nodes.Find("miscParent", true);

                j = 1;
                foreach (string[] data in parameters.misc_CustomItems)
                {
                    if (bool.Parse(data[3]))
                    {
                        string[] nameSplit = data[0].Split(new string[] { "] -" }, StringSplitOptions.None);
                        string name = nameSplit[0];
                        double cost = (double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[2], System.Globalization.CultureInfo.InvariantCulture));
                        TreeNode misc_newChild = new TreeNode("9." + j + " " + name + "] x " + data[1] + " (₱" + cost.ToString("#,##0.00") + ")");
                        misc_newChild.Name = "misc_" + data[0];

                        found[0].Nodes.Add(misc_newChild);
                        j++;
                    }
                }

                //Setting of BOQ   
                try
                {
                    if (misc_TOTALCOST > 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "9.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "MISCELLANEOUS ITEMS";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        foreach (string[] data in parameters.misc_CustomItems)
                        {
                            if (bool.Parse(data[3]))
                            {
                                string[] nameSplit = data[0].Split(new string[] { "[" }, StringSplitOptions.None);
                                string name = nameSplit[0];
                                string[] nameSplit2 = nameSplit[1].Split(new string[] { "] -" }, StringSplitOptions.None);
                                string category = nameSplit2[0];
                                double cost = (double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[2], System.Globalization.CultureInfo.InvariantCulture));
                                index = this.summ_BOQ_dg.Rows.Add();
                                this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "9." + i;
                                this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = name;
                                this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = data[1];
                                this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = category;
                                this.summ_BOQ_dg.Rows[index].Cells["materials1"].Value = "₱" + double.Parse(data[2]).ToString("#,##0.00");
                                this.summ_BOQ_dg.Rows[index].Cells["materials1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["materials2"].Value = "₱" + cost.ToString("#,##0.00");
                                this.summ_BOQ_dg.Rows[index].Cells["materials2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + cost.ToString("#,##0.00");
                                this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                i++;
                            }
                        }

                        this.summ_BOQ_dg.Rows.Add();
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + misc_TOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //9.0 - Miscellaneous Items -- END

                //10.0 - Additional Labor and Equipment -- START
                found = view_TV3.Nodes.Find("laborAndEquipmentParent", true);

                j = 1;
                //Manpower
                if (laborAndEqpt_TOTALCOST > 0)
                {
                    if (parameters.labor_RD.Equals("Manila Rate"))
                    {
                        foreach (string[] data in parameters.labor_MP)
                        {
                            if (bool.Parse(data[4]))
                            {
                                LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_ManpowerM[data[0]]);
                                double cost = content.totalPrice;
                                TreeNode labor_newChild = new TreeNode("10." + j + " " + data[0] + " x " + data[1] + " x " + data[2] + " HOURS x " + data[3] + " DAYS (₱" + cost.ToString("#,##0.00") + ")");
                                labor_newChild.Name = "laborAndEquipment_" + data[0];

                                found[0].Nodes.Add(labor_newChild);
                                j++;
                            }
                        }
                    }
                    else //Provincial
                    {
                        foreach (string[] data in parameters.labor_MP)
                        {
                            if (bool.Parse(data[4]))
                            {
                                LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_ManpowerP[data[0]]);
                                double cost = content.totalPrice;
                                TreeNode labor_newChild = new TreeNode("10." + j + " " + data[0] + " x " + data[1] + " x " + data[2] + " HOURS x " + data[3] + " DAYS (₱" + cost.ToString("#,##0.00") + ")");
                                labor_newChild.Name = "laborAndEquipment_" + data[0];

                                found[0].Nodes.Add(labor_newChild);
                                j++;
                            }
                        }
                    }
                }
                //Equipment
                if (misc_TOTALCOST > 0)
                {
                    foreach (string[] data in parameters.labor_EQP)
                    {
                        if (bool.Parse(data[4]))
                        {
                            LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_Equipment[data[0]]);
                            double cost = content.totalPrice;
                            TreeNode labor_newChild = new TreeNode("10." + j + " " + data[0] + " x " + data[1] + " x " + data[2] + " HOURS x " + data[3] + " DAYS (₱" + cost.ToString("#,##0.00") + ")");
                            labor_newChild.Name = "laborAndEquipment_" + data[0];

                            found[0].Nodes.Add(labor_newChild);
                            j++;
                        }
                    }
                }
                
                //Setting of BOQ   
                try
                {
                    if (laborAndEqpt_TOTALCOST > 0)
                    {
                        var index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Value = "10.0";
                        this.summ_BOQ_dg.Rows[index].Cells["item1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "ADDITIONAL LABOR AND EQUIPMENT";
                        this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                        int i = 1;
                        //Manpower
                        if (parameters.labor_RD.Equals("Manila Rate"))
                        {
                            foreach (string[] data in parameters.labor_MP)
                            {
                                if (bool.Parse(data[4]))
                                {
                                    LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_ManpowerM[data[0]]);
                                    double cost = content.totalPrice;
                                    string[] nameSplit = data[0].Split(new string[] { "[" }, StringSplitOptions.None);
                                    string name = nameSplit[0] + " [MNL]";
                                    double hours = (double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[2], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[3], System.Globalization.CultureInfo.InvariantCulture));
                                    index = this.summ_BOQ_dg.Rows.Add();
                                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "10." + i;
                                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                    this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = name;
                                    this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                    this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = hours.ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "hrs";
                                    this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_ManpowerM[data[0]].ToString()).ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + cost.ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + cost.ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                    i++;
                                }
                            }
                        }
                        else //Provincial
                        {
                            foreach (string[] data in parameters.labor_MP)
                            {
                                if (bool.Parse(data[4]))
                                {
                                    LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_ManpowerP[data[0]]);
                                    double cost = content.totalPrice;
                                    string[] nameSplit = data[0].Split(new string[] { "[" }, StringSplitOptions.None);
                                    string name = nameSplit[0] + " [PRO]";
                                    double hours = (double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[2], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[3], System.Globalization.CultureInfo.InvariantCulture));
                                    index = this.summ_BOQ_dg.Rows.Add();
                                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "10." + i;
                                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                    this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = name;
                                    this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                    this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = hours.ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "hrs";
                                    this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_ManpowerP[data[0]].ToString()).ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + cost.ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + cost.ToString("#,##0.00");
                                    this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                    i++;
                                }
                            }
                        }
                        //Equipment
                        foreach (string[] data in parameters.labor_EQP)
                        {
                            if (bool.Parse(data[4]))
                            {
                                LaborAndEquipmentUserControl content = new LaborAndEquipmentUserControl(data[0], data[1], data[2], data[3], parameters.price_Equipment[data[0]]);
                                double cost = content.totalPrice;
                                string[] nameSplit = data[0].Split(new string[] { "[" }, StringSplitOptions.None);
                                string name = nameSplit[0];
                                double hours = (double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[2], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(data[3], System.Globalization.CultureInfo.InvariantCulture));
                                index = this.summ_BOQ_dg.Rows.Add();
                                this.summ_BOQ_dg.Rows[index].Cells["description1"].Value = "10." + i;
                                this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["description1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                this.summ_BOQ_dg.Rows[index].Cells["description2"].Value = name;
                                this.summ_BOQ_dg.Rows[index].Cells["description2"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                                this.summ_BOQ_dg.Rows[index].Cells["qty1"].Value = hours.ToString("#,##0.00");
                                this.summ_BOQ_dg.Rows[index].Cells["qty1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["unit1"].Value = "hrs";
                                this.summ_BOQ_dg.Rows[index].Cells["labor1"].Value = "₱" + double.Parse(parameters.price_Equipment[data[0]].ToString()).ToString("#,##0.00");
                                this.summ_BOQ_dg.Rows[index].Cells["labor1"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["labor2"].Value = "₱" + cost.ToString("#,##0.00");
                                this.summ_BOQ_dg.Rows[index].Cells["labor2"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                                this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + cost.ToString("#,##0.00");
                                this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                i++;
                            }
                        }

                        this.summ_BOQ_dg.Rows.Add();
                        index = this.summ_BOQ_dg.Rows.Add();
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Value = "₱" + laborAndEqpt_TOTALCOST.ToString("#,##0.00");
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.summ_BOQ_dg.Rows[index].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                    }
                }
                catch (Exception ex)
                {

                }
                //10.0 - Additional Labor and Equipment -- END

                //BOQ FINAL COST
                this.summ_BOQ_dg.Rows.Add();
                int index2 = this.summ_BOQ_dg.Rows.Add();
                this.summ_BOQ_dg.Rows[index2].Cells["totalcost1"].Value = "₱" + TOTALCOST.ToString("#,##0.00");
                this.summ_BOQ_dg.Rows[index2].Cells["totalcost1"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                this.summ_BOQ_dg.Rows[index2].Cells["totalcost1"].Style.BackColor = Color.LightGreen;
                this.summ_BOQ_dg.Rows[index2].Cells["totalcost1"].Style.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);

                //Setting BOQ Cell Autosize to True
                this.summ_BOQ_dg.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                this.summ_BOQ_dg.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            //Setting View End
        }
        private void setTree(List<TreeNode> nodes, TreeView treeView)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                treeView.Nodes.Add(nodes[i]);
            }
            AdjustTreeViewHeight(treeView);
        }

        //View Variables
        const int TVM_GETNEXTITEM = 0x1100 + 10;
        const int TVGN_LASTVISIBLE = 0x000A;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        extern static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        public void AdjustTreeViewHeight(TreeView treeView)
        {
            treeView.Scrollable = true;
            var nodeHandle = SendMessage(treeView.Handle, TVM_GETNEXTITEM,
                TVGN_LASTVISIBLE, IntPtr.Zero);
            var node = treeView.GetType().GetMethod("NodeFromHandle",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(treeView, new object[] { nodeHandle }) as TreeNode;
            var r = node.Bounds;
            treeView.Height = r.Top + r.Height + 4;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            string[] parents = { "Earthworks" };
            TreeViewHitTestInfo info = view_TV1.HitTest(view_TV1.PointToClient(Cursor.Position));
            try
            {
                if (view_TV1.SelectedNode != null)
                {
                    if (Array.IndexOf(parents, info.Node.Text) < 0)
                    {
                        ViewDetailedInfoForm vf = new ViewDetailedInfoForm(info.Node.Text.Substring(0, 3), this, structuralMembers);
                        vf.ShowDialog();
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }

        private void view_ConfigureBtn_Click(object sender, EventArgs e)
        {
            PriceChecklistForms dlg = new PriceChecklistForms(this);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //Update view
                viewInitalized = false;
                initializeView();
            }
        }
        //View functions -- END

        //Price functions -- START
        private void InitPriceList() 
        {
            //1 - Common Materials
            parameters.price_CommonMaterials.Add("Cyclone Wire (Gauge#10, 2”x2”, 3ft x 10m) [ROLL]", 3250);
            parameters.price_CommonMaterials.Add("Gasket (6mm thk, 1m x 1m) [ROLL]", 2840);
            parameters.price_CommonMaterials.Add("Acetylene Gas [CYL]", 1045);
            parameters.price_CommonMaterials.Add("Oxygen Gas [CYL]", 438);
            parameters.price_CommonMaterials.Add("Rugby [CANS]", 648);
            parameters.price_CommonMaterials.Add("Vulca Seal [LTR]", 470);
            parameters.price_CommonMaterials.Add("Broom, Soft [PC]", 218);
            parameters.price_CommonMaterials.Add("Concrete Epoxy [SET]", 3000);
            parameters.price_CommonMaterials.Add("Concrete Patching Compound [KGS]", 386);
            parameters.price_CommonMaterials.Add("GI Wire (no. 16) [ROLL]", 1750);
            parameters.price_CommonMaterials.Add("GI Wire (no. 12) [KG]", 70.85);
            parameters.price_CommonMaterials.Add("GI Wire (no. 16) [KG]", 71.37);
            parameters.price_CommonMaterials.Add("Non-Shrink Grout [BAGS]", 621);
            parameters.price_CommonMaterials.Add("40 kg Portland Cement [BAGS]", 165);
            parameters.price_CommonMaterials.Add("Sand [m3]", 1400);
            parameters.price_CommonMaterials.Add("Rivets 1/8” x ½” [BOX]", 200);
            parameters.price_CommonMaterials.Add("Rivets 1-1/2” x ½” [BOX]", 250);
            parameters.price_CommonMaterials.Add("Rope (⌀ 1/2”) [MTRS]", 26);
            parameters.price_CommonMaterials.Add("Tape, Caution [ROLLS]", 800);
            parameters.price_CommonMaterials.Add("Tile Grout (2KG)  [BAGS]", 61);
            parameters.price_CommonMaterials.Add("Tiles, Floor (600 x 600) [PC]", 152);
            parameters.price_CommonMaterials.Add("Tiles, Wall (300 x 300) [PC]", 33);
            parameters.price_CommonMaterials.Add("Broom Stick [PC]", 37);
            parameters.price_CommonMaterials.Add("Chalk Stone [PC]", 7);
            parameters.price_CommonMaterials.Add("Sandpaper (#100) [MTRS]", 202);
            parameters.price_CommonMaterials.Add("Sandpaper (#50) [MTRS]", 284);
            parameters.price_CommonMaterials.Add("Common Nail [KG]", 150);
            parameters.price_CommonMaterials.Add("Concrete Nail [KG]", 170);
            parameters.price_CommonMaterials.Add("Putty, Multipurpose [PAIL]", 883);
            parameters.price_CommonMaterials.Add("Tie Wire (No. #16) [25kg/Roll]", 1800);
            parameters.price_CommonMaterials.Add("25KG Tile Adhesive (Regular) [BAGS]", 300);
            parameters.price_CommonMaterials.Add("25 KG Tile Adhesive (Heavy duty) [BAGS]", 550);
            parameters.price_CommonMaterials.Add("CHB 4” (0.10 x 0.20 x 0.40) [PC]", 20);
            parameters.price_CommonMaterials.Add("CHB 6” (0.15 x 0.20 x 0.40) [PC]", 22);
            parameters.price_CommonMaterials.Add("CHB 8” (0.20 x 0.20 x 0.40) [PC]", 26);
            foreach (DictionaryEntry dict in parameters.price_CommonMaterials)
            {
                parameters.searchList.Add(dict.Key + " - Common Materials");
                parameters.customItemsList.Add(dict.Key + " - Common Materials");
            }

            //2 - Paint and Coating
            parameters.price_PaintAndCoating.Add("Acrylic Emulsion [GALS]", 705); 
            parameters.price_PaintAndCoating.Add("Concrete Epoxy Injection [GALS]", 1834);
            parameters.price_PaintAndCoating.Add("Concrete Primer & Sealer [PAIL]", 2618);
            parameters.price_PaintAndCoating.Add("Epopatch, Base and Hardener [SETS]", 2075);
            parameters.price_PaintAndCoating.Add("Lacquer Thinner [GALS]", 359);
            parameters.price_PaintAndCoating.Add("Paint Brush, Bamboo 1-1/2” [PC]", 35);
            parameters.price_PaintAndCoating.Add("Paint, Acrylic 1 [GAL]", 650);
            parameters.price_PaintAndCoating.Add("Paint, Epoxy Enamel White [GAL]", 1100);
            parameters.price_PaintAndCoating.Add("Paint, Epoxy Floor Coating [GALS]", 2800);
            parameters.price_PaintAndCoating.Add("Paint, Epoxy Primer Gray [GALS]", 865);
            parameters.price_PaintAndCoating.Add("Paint, Epoxy Reducer [GALS]", 529);
            parameters.price_PaintAndCoating.Add("Paint Latex Gloss [GAL]", 640.2);
            parameters.price_PaintAndCoating.Add("Paint Enamel [GAL]", 660);
            parameters.price_PaintAndCoating.Add("Paint, Semi-Gloss [GALS]", 675);
            parameters.price_PaintAndCoating.Add("Putty, Masonry [PAIL]", 1396);
            parameters.price_PaintAndCoating.Add("Rust Converter [GAL]", 771);
            parameters.price_PaintAndCoating.Add("Skim Coat [BAGS]", 650);
            parameters.price_PaintAndCoating.Add("Underwater Epoxy [GALS]", 2211);
            parameters.price_PaintAndCoating.Add("Concrete neutralizer [GALS]", 475);
            foreach (DictionaryEntry dict in parameters.price_PaintAndCoating)
            {
                parameters.searchList.Add(dict.Key + " - Paint and Coating");
                parameters.customItemsList.Add(dict.Key + " - Paint and Coating");
            }

            //3 - Welding Rod
            parameters.price_WeldingRod.Add("Stainless Welding Rod 308 (3.2mm) [KGS]", 500); 
            parameters.price_WeldingRod.Add("Welding Rod 6011 (3.2mm) [KGS]", 125);
            parameters.price_WeldingRod.Add("Welding Rod 6011 (3.2mm) [BOX]", 2400);
            parameters.price_WeldingRod.Add("Welding Rod 6013 (3.2mm) [KGS]", 107);
            parameters.price_WeldingRod.Add("Welding Rod 6013 (3.2mm) [BOX]", 2000);
            foreach (DictionaryEntry dict in parameters.price_WeldingRod)
            {
                parameters.searchList.Add(dict.Key + " - Welding Rod");
                parameters.customItemsList.Add(dict.Key + " - Welding Rod");
            }

            //4 - Personal Protective Equipment
            parameters.price_PersonalProtectiveEquipment.Add("Chemical Gloves PAIR Cotton Gloves [PAIRS]", 212);
            parameters.price_PersonalProtectiveEquipment.Add("Cotton Gloves [PAIRS]", 18);
            parameters.price_PersonalProtectiveEquipment.Add("Dust Mask N95 [PC]", 28);
            parameters.price_PersonalProtectiveEquipment.Add("Gloves, Orange Palm [PAIRS]", 28);
            parameters.price_PersonalProtectiveEquipment.Add("Hard Hat w/ Headgear and chin strap [20 SET]", 221);
            parameters.price_PersonalProtectiveEquipment.Add("Overall with reflector [PC]", 1074);
            parameters.price_PersonalProtectiveEquipment.Add("Oxy-Acetylene Cutting Outfit [SET]", 17125);
            parameters.price_PersonalProtectiveEquipment.Add("PVC Apron  [PC]", 508);
            parameters.price_PersonalProtectiveEquipment.Add("Respirator mask w/ Cartridge [PC]", 2117);
            parameters.price_PersonalProtectiveEquipment.Add("Respirator, Filter Cartridge [PACK]", 656);
            parameters.price_PersonalProtectiveEquipment.Add("Safety Goggles [PC]", 130);
            parameters.price_PersonalProtectiveEquipment.Add("Safety Rubber boots [PAIR]", 391);
            parameters.price_PersonalProtectiveEquipment.Add("Safety Shoes  [PAIR]", 960);
            parameters.price_PersonalProtectiveEquipment.Add("Safety Vest [PC]", 120); 
            parameters.price_PersonalProtectiveEquipment.Add("Welding Mask  [PC]", 130);
            parameters.price_PersonalProtectiveEquipment.Add("Welding Apron [PC]", 262);
            parameters.price_PersonalProtectiveEquipment.Add("Welding Mask, Auto Darkening [SETS]", 2500);
            foreach (DictionaryEntry dict in parameters.price_PersonalProtectiveEquipment)
            {
                parameters.searchList.Add(dict.Key + " - Personal Protective Equipment");
                parameters.customItemsList.Add(dict.Key + " - Personal Protective Equipment");
            }

            //5 - Tools 
            parameters.price_Tools.Add("Adjustable Wrench Set 4”— 24” [SET]", 2468);
            parameters.price_Tools.Add("Baby Roller (Cotton) 4” [PC]", 63);
            parameters.price_Tools.Add("Ball Hammer [PC]", 671);
            parameters.price_Tools.Add("Bench Vise [UNIT]", 5122);
            parameters.price_Tools.Add("Blade Cutter [PC]", 57);
            parameters.price_Tools.Add("Camlock (Male & Female Set) 50mm DIA [SET]", 1585);
            parameters.price_Tools.Add("Chipping Gun [UNIT]", 30607);
            parameters.price_Tools.Add("Combination Wrench Set 6mm — 32mm [SET]", 3340);
            parameters.price_Tools.Add("Cut-off Wheel ⌀ 16” [BOX]", 6295);
            parameters.price_Tools.Add("Cutting Disc ⌀ 4” [BOX]", 1570);
            parameters.price_Tools.Add("Cutting Disc ⌀ 7” [BOX]", 3785);
            parameters.price_Tools.Add("Drill Bit [BOX]", 608);
            parameters.price_Tools.Add("Electrical Plier [PC]", 408);
            parameters.price_Tools.Add("Grinder, Angle 4” [UNIT]", 4017);
            parameters.price_Tools.Add("Grinder, Angle 7” [UNIT]", 9184);
            parameters.price_Tools.Add("Grinder, Baby [UNIT]", 5053);
            parameters.price_Tools.Add("Grinder, Mother [UNIT]", 9025);
            parameters.price_Tools.Add("Grinding Disc ⌀ 4” [BOX]", 929);
            parameters.price_Tools.Add("Grinding Disc ⌀ 7” [BOX]", 1875);
            parameters.price_Tools.Add("Heat Gun [UNIT]", 3500);
            parameters.price_Tools.Add("Ladder (A-Type), 6h, Aluminum [PC]", 2270);
            parameters.price_Tools.Add("Level Bar 24” [PC]", 540);
            parameters.price_Tools.Add("Paint Brush 4” [PC]", 63);
            parameters.price_Tools.Add("Paint Brush 2” [PC]", 39);
            parameters.price_Tools.Add("Portable Axial Fan Blower  ⌀  8” [SET]", 8295);
            parameters.price_Tools.Add("Power Ratchet [UNIT]", 8068);
            parameters.price_Tools.Add("Rivet Gun / Riveter [UNIT]", 820);
            parameters.price_Tools.Add("Roller Brush 7” [PC]", 97);
            parameters.price_Tools.Add("Screwdriver, Flat [SET]", 273);
            parameters.price_Tools.Add("Screwdriver, Philip [SET]", 290);
            parameters.price_Tools.Add("Shovel, Pointed [PC]", 500);
            parameters.price_Tools.Add("Snap Ring Plier [SET]", 548);
            parameters.price_Tools.Add("Socket Wrench Set 19mm — 50mm [SET]", 6329);
            parameters.price_Tools.Add("Speed Cutter [UNIT]", 10000);
            parameters.price_Tools.Add("Steel Brush [PC]", 30);
            parameters.price_Tools.Add("Test Light [PC]", 158);
            parameters.price_Tools.Add("Test Wrench [UNIT]", 5940);
            parameters.price_Tools.Add("Torque Wrench [UNIT]", 6217);
            parameters.price_Tools.Add("Vise Grip [PC]", 456);
            parameters.price_Tools.Add("Welding Machine (Portable) 12.3 kVA(20-300A) [UNIT]", 19500);
            foreach (DictionaryEntry dict in parameters.price_Tools)
            {
                parameters.searchList.Add(dict.Key + " - Tools");
                parameters.customItemsList.Add(dict.Key + " - Tools");
            }

            //6 - Ready Mix Concrete 
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days [m3]", 4540);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 14 Days [m3]", 4810);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 7 Days [m3]", 5070);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 3 Days [m3]", 5620);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 28 Days [m3]", 4640);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 14 Days [m3]", 5090);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 7 Days [m3]", 5360);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 3 Days [m3]", 5660);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 28 Days [m3]", 4910);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 14 Days [m3]", 5300);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 4500PSI (31 Mpa) @ 28 Days [m3]", 5020);
            parameters.price_ReadyMixConcrete.Add("Ready Mix Concrete, 5000PSI (34.5 Mpa) @ 28 Days [m3]", 5280);
            foreach (DictionaryEntry dict in parameters.price_ReadyMixConcrete)
            {
                parameters.searchList.Add(dict.Key + " - Ready Mix Concrete");
                parameters.customItemsList.Add(dict.Key + " - Ready Mix Concrete");
            }

            //7 - Gravel
            parameters.price_Gravel.Add("GRAVEL G1 [m3]", 530);
            parameters.price_Gravel.Add("GRAVEL G2 [m3]", 420);
            parameters.price_Gravel.Add("GRAVEL G1- ½” [m3]", 435);
            parameters.price_Gravel.Add("GRAVEL G2- ½” [m3]", 420);
            parameters.price_Gravel.Add("GRAVEL ¾” [m3]", 440);
            foreach (DictionaryEntry dict in parameters.price_Gravel)
            {
                parameters.searchList.Add(dict.Key + " - Gravel");
                parameters.customItemsList.Add(dict.Key + " - Gravel");
            }

            //8 - Formworks and Lumber
            parameters.price_FormworksAndLumber.Add("Lumber [2”x 2” x 8']", 130);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x 2” x 10']", 140);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x 2” x 12']", 145);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x 3”x 8']", 150);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x 3”x 10']", 150);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x 3”x 12']", 180);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x4”x 8']", 265);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x4”x 10']", 275);
            parameters.price_FormworksAndLumber.Add("Lumber [2”x4”x 12']", 280);
            parameters.price_FormworksAndLumber.Add("PLYWOOD 1/2” [1.22m x 2.44m]", 674);
            parameters.price_FormworksAndLumber.Add("PLYWOOD 3/4” [1.22m x 2.44m]", 1112);
            parameters.price_FormworksAndLumber.Add("PLYWOOD 1/4” [1.22m x 2.44m]", 381);
            parameters.price_FormworksAndLumber.Add("PLYWOOD 1/8”[1.22m x 2.44m]", 270);
            parameters.price_FormworksAndLumber.Add("ECOBOARD 1/2”[1.22m x 2.44m]", 2100);
            parameters.price_FormworksAndLumber.Add("ECOBOARD 3/4” [1.22m x 2.44m]", 3100);
            parameters.price_FormworksAndLumber.Add("ECOBOARD 1/4” [1.22m x 2.44m]", 1620);
            parameters.price_FormworksAndLumber.Add("ECOBOARD 1/8” [1.22m x 2.44m]", 1200);
            parameters.price_FormworksAndLumber.Add("PHENOLIC BOARD- 1/2” [1.22m x 2.44m]", 2012.64);
            parameters.price_FormworksAndLumber.Add("PHENOLIC BOARD- 3/4” [1.22m x 2.44m]", 2656.20);
            foreach (DictionaryEntry dict in parameters.price_FormworksAndLumber)
            {
                parameters.searchList.Add(dict.Key + " - Formworks and Lumber");
                parameters.customItemsList.Add(dict.Key + " - Formworks and Lumber");
            }

            //9 - Roof Materials
            parameters.price_RoofMaterials.Add("C- Purlins (75mm x 50mm x 0.7mm thick) [6 m]", 314);
            parameters.price_RoofMaterials.Add("C- Purlins (100mm x 50mm x 0.7mm thick) [6 m]", 359);
            parameters.price_RoofMaterials.Add("C- Purlins (150mm x 50mm x 0.9mm thick) [6 m]", 544);
            parameters.price_RoofMaterials.Add("C- Purlins (75mm x 50mm x 1.0mm thick) [6 m]", 409);
            parameters.price_RoofMaterials.Add("C- Purlins (100mm x 50mm x 1.0mm thick) [6 m]", 471);
            parameters.price_RoofMaterials.Add("C- Purlins (150mm x 50mm x 1.0mm thick) [6 m]", 583);
            parameters.price_RoofMaterials.Add("C- Purlins (75mm x 50mm x 1.2mm thick) [6 m]", 499);
            parameters.price_RoofMaterials.Add("C- Purlins (100mm x 50mm x 1.2mm thick) [6 m]", 572);
            parameters.price_RoofMaterials.Add("C- Purlins (150mm x 50mm x 1.2mm thick ) [6 m]", 712);
            parameters.price_RoofMaterials.Add("Corrugated G.I Sheet, Gauge 26 (0.551mmx2.44 mm) [m2]", 423);
            parameters.price_RoofMaterials.Add("Plain G.I Sheet, Gauge 24 (4ft x 8 ft) [UNIT]", 622);
            parameters.price_RoofMaterials.Add("G.I. Roof Nails [KG]", 120);
            parameters.price_RoofMaterials.Add("G.I Rivets [KG]", 152);
            parameters.price_RoofMaterials.Add("G.I Washers [KG]", 126);
            parameters.price_RoofMaterials.Add("Umbrella Nails [KG]", 120);
            foreach (DictionaryEntry dict in parameters.price_RoofMaterials)
            {
                parameters.searchList.Add(dict.Key + " - Roof Materials");
                parameters.customItemsList.Add(dict.Key + " - Roof Materials");
            }

            //10 - Tubular Steel (1mm thick)
            parameters.price_TubularSteel1mm.Add("B.I. (Black Iron) Tubular 20mm x 20mm x 1.0mm thick [6m]", 244);
            parameters.price_TubularSteel1mm.Add("B.I. (Black Iron) Tubular 25mm x 25mm x 1.0mm thick [6m]", 325);
            parameters.price_TubularSteel1mm.Add("B.I. (Black Iron) Tubular 32mm x 32mm x 1.0mm thick [6m]", 456);
            parameters.price_TubularSteel1mm.Add("B.I. (Black Iron) Tubular 50mm x 25mm x 1.0mm thick [6m]", 443);
            parameters.price_TubularSteel1mm.Add("B.I. (Black Iron) Tubular 50mm x 50mm x 1.0mm thick [6m]", 493);
            foreach (DictionaryEntry dict in parameters.price_TubularSteel1mm)
            {
                parameters.searchList.Add(dict.Key + " - Tubular Steel (1mm thick)");
                parameters.customItemsList.Add(dict.Key + " - Tubular Steel (1mm thick)");
            }

            //11 - Tubular Steel (1.2mm thick)
            parameters.price_TubularSteel1p2mm.Add("B.I. (Black Iron) Tubular 25mm x 25mm x 1.2mm thick [6m]", 399);
            parameters.price_TubularSteel1p2mm.Add("B.I. (Black Iron) Tubular 32mm x 32mm x 1.2mm thick [6m]", 633);
            parameters.price_TubularSteel1p2mm.Add("B.I. (Black Iron) Tubular 50mm x 25mm x 1.2mm thick [6m]", 482);
            parameters.price_TubularSteel1p2mm.Add("B.I. (Black Iron) Tubular 50mm x 50mm x 1.2mm thick [6m]", 605);
            parameters.price_TubularSteel1p2mm.Add("B.I. (Black Iron) Tubular 75mm x 50mm x 1.2mm thick [6m]", 748);
            parameters.price_TubularSteel1p2mm.Add("B.I. (Black Iron) Tubular 100mm x 50mm x 1.2mm thick [6m]", 893);
            parameters.price_TubularSteel1p2mm.Add("B.I. (Black Iron) Tubular 150mm x 50mm x 1.2mm thick [6m]", 1551);
            foreach (DictionaryEntry dict in parameters.price_TubularSteel1p2mm)
            {
                parameters.searchList.Add(dict.Key + " - Tubular Steel (1.2mm thick)");
                parameters.customItemsList.Add(dict.Key + " - Tubular Steel (1.2mm thick)");
            }

            //12 - Tubular Steel (1.5mm thick)
            parameters.price_TubularSteel1p5mm.Add("B.I. (Black Iron) Tubular 25mm x 25mm x 1.5mm thick [6m]", 572);
            parameters.price_TubularSteel1p5mm.Add("B.I. (Black Iron) Tubular 32mm x 32mm x 1.5mm thick [6m]", 898);
            parameters.price_TubularSteel1p5mm.Add("B.I. (Black Iron) Tubular 50mm x 25mm x 1.5mm thick [6m]", 745);
            parameters.price_TubularSteel1p5mm.Add("B.I. (Black Iron) Tubular 50mm x 50mm x 1.5mm thick [6m]", 940);
            parameters.price_TubularSteel1p5mm.Add("B.I. (Black Iron) Tubular 75mm x 50mm x 1.5mm thick [6m]", 1182);
            parameters.price_TubularSteel1p5mm.Add("B.I. (Black Iron) Tubular 100mm x 50mm x 1.5mm thick [6m]", 1329);
            parameters.price_TubularSteel1p5mm.Add("B.I. (Black Iron) Tubular 150mm x 50mm x 1.5mm thick [6m]", 1932);
            foreach (DictionaryEntry dict in parameters.price_TubularSteel1p5mm)
            {
                parameters.searchList.Add(dict.Key + " - Tubular Steel (1.5mm thick)");
                parameters.customItemsList.Add(dict.Key + " - Tubular Steel (1.5mm thick)");
            }

            //13 - Embankment
            parameters.price_Embankment.Add("Common Borrow [m3]", 392.65);
            parameters.price_Embankment.Add("Selected Borrow [m3]", 397.49);
            parameters.price_Embankment.Add("Mixed Sand & Gravel [m3]", 597.211);
            parameters.price_Embankment.Add("Rock [m3]", 613.76);
            foreach (DictionaryEntry dict in parameters.price_Embankment)
            {
                parameters.searchList.Add(dict.Key + " - Embankment");
                parameters.customItemsList.Add(dict.Key + " - Embankment");
            }

            //14 - Rebar Grade 33 (230 Mpa)
            parameters.price_RebarGrade33.Add("Compression Coupler GRADE 33 [PC]", 450);
            parameters.price_RebarGrade33.Add("Tension Coupler GRADE 33 [PC]", 450);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀10mm) [6m]", 147.84);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀10mm) [7.5m]", 184.80);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀10mm) [9m]", 221.76);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀10mm) [10.5m]", 258.72);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀10mm) [12m]", 295.68);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀12mm) [6m]", 213.12);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀12mm) [7.5m]", 266.4);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀12mm) [9m]", 319.68);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀12mm) [10.5m]", 372.96);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀12mm) [12m]", 426.24);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀16mm) [6m]", 378.72);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀16mm) [7.5m]", 473.4);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀16mm) [9m]", 568.08);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀16mm) [10.5m]", 662.76);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀16mm) [12m]", 757.44);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀20mm) [6m]", 591.84);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀20mm) [7.5m]", 739.8);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀20mm) [9m]", 887.76);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀20mm) [10.5m]", 1035.72);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀20mm) [12m]", 1183.68);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀25mm) [6m]", 924.72);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀25mm) [7.5m]", 1155.9);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀25mm) [9m]", 1387.08);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀25mm) [10.5m]", 1618.26);
            parameters.price_RebarGrade33.Add("Rebar GRADE 33 (⌀25mm) [12m]", 1849.44);
            foreach (DictionaryEntry dict in parameters.price_RebarGrade33)
            {
                parameters.searchList.Add(dict.Key + " - Rebar Grade 33 (230 Mpa)");
                parameters.customItemsList.Add(dict.Key + " - Rebar Grade 33 (230 Mpa)");
            }

            //15 - Rebar Grade 40 (275 Mpa) 
            parameters.price_RebarGrade40.Add("Compression Coupler GRADE 40 [PC]", 500);
            parameters.price_RebarGrade40.Add("Tension Coupler GRADE 40 [PC]", 500);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀10mm) [6m]", 162.62);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀10mm) [7.5m]", 203.28);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀10mm) [9m]", 243.94);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀10mm) [10.5m]", 284.59);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀10mm) [12m]", 325.25);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀12mm) [6m]", 234.43);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀12mm) [7.5m]", 293.04);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀12mm) [9m]", 351.65);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀12mm) [10.5m]", 410.26);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀12mm) [12m]", 468.86);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀16mm) [6m]", 412.80);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀16mm) [7.5m]", 516.01);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀16mm) [9m]", 619.21);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀16mm) [10.5m]", 722.41);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀16mm) [12m]", 825.61);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀20mm) [6m]", 645.11);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀20mm) [7.5m]", 806.38);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀20mm) [9m]", 967.66);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀20mm) [10.5m]", 1128.93);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀20mm) [12m]", 1290.21);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀25mm) [6m]", 1007.94);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀25mm) [7.5m]", 1259.93);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀25mm) [9m]", 1511.92);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀25mm) [10.5m]", 1763.90);
            parameters.price_RebarGrade40.Add("Rebar GRADE 40 (⌀25mm) [12m]", 2015.89);
            foreach (DictionaryEntry dict in parameters.price_RebarGrade40)
            {
                parameters.searchList.Add(dict.Key + " - Rebar Grade 40 (275 Mpa)");
                parameters.customItemsList.Add(dict.Key + " - Rebar Grade 40 (275 Mpa)");
            }

            //16 -  Rebar Grade 60 (415 Mpa)
            parameters.price_RebarGrade60.Add("Compression Coupler GRADE 60 [PC]", 600);
            parameters.price_RebarGrade60.Add("Tension Coupler GRADE 60 [PC]", 600);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀10mm) [6m]", 167.8);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀10mm) [7.5m]", 209.74);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀10mm) [9m]", 251.7);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀10mm) [10.5m]", 293.64);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀10mm) [12m]", 335.6);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀12mm) [6m]", 241.9);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀12mm) [7.5m]", 302.36);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀12mm) [9m]", 362.84);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀12mm) [10.5m]", 423.3);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀12mm) [12m]", 483.78);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀16mm) [6m]", 426.06);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀16mm) [7.5m]", 532.58);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀16mm) [9m]", 639.09);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀16mm) [10.5m]", 745.61);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀16mm) [12m]", 852.12);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀20mm) [6m]", 665.82);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀20mm) [7.5m]", 832.28);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀20mm) [9m]", 998.73);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀20mm) [10.5m]", 1165.19);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀20mm) [12m]", 2080.62);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀25mm) [6m]", 1040.31);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀25mm) [7.5m]", 1300.39);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀25mm) [9m]", 1560.47);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀25mm) [10.5m]", 1820.54);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀25mm) [12m]", 2080.62);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀28mm) [6m]", 1305.18);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀28mm) [7.5m]", 1631.48);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀28mm) [9m]", 1957.77);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀28mm) [10.5m]", 2284.07);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀28mm) [12m]", 2610.36);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀32mm) [6m]", 1719.66);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀32mm) [7.5m]", 2149.58);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀32mm) [9m]", 2579.50);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀32mm) [10.5m]", 3009.83);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀32mm) [12m]", 4352.95);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀36mm) [6m]", 2176.48);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀36mm) [7.5m]", 2720.60);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀36mm) [9m]", 3264.71);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀36mm) [10.5m]", 3808.83);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀36mm) [12m]", 4352.94);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀40mm) [6m]", 1827);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀40mm) [7.5m]", 2285);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀40mm) [9m]", 2740);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀40mm) [10.5m]", 3125);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀40mm) [12m]", 3653);

            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀50mm) [6m]", 2828);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀50mm) [7.5m]", 3535);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀50mm) [9m]", 4241);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀50mm) [10.5m]", 4750);
            parameters.price_RebarGrade60.Add("Rebar GRADE 60 (⌀50mm) [12m]", 5655);
            foreach (DictionaryEntry dict in parameters.price_RebarGrade60)
            {
                parameters.searchList.Add(dict.Key + " - Rebar Grade 60 (415 Mpa)");
                parameters.customItemsList.Add(dict.Key + " - Rebar Grade 60 (415 Mpa)");
            }
            //17 - Labor Rate - Earthworks
            parameters.price_LaborRate_Earthworks.Add("Excavation [m3]", 400);
            parameters.price_LaborRate_Earthworks.Add("Backfilling and Compaction [m3]", 400);
            parameters.price_LaborRate_Earthworks.Add("Grading and Compaction [m3]", 350);
            parameters.price_LaborRate_Earthworks.Add("Gravel Bedding and Compaction [m3]", 300);
            parameters.price_LaborRate_Earthworks.Add("Soil Poisoning [m2]", 60);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Earthworks)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Earthworks");
            }
            //18 - Labor Rate - Concreting
            parameters.price_LaborRate_Concreting.Add("FOOTING [m3]", 400);
            parameters.price_LaborRate_Concreting.Add("WALL FOOTING [m3]", 400);
            parameters.price_LaborRate_Concreting.Add("COLUMN [m3]", 450);
            parameters.price_LaborRate_Concreting.Add("STAIRS [m3]", 450);
            parameters.price_LaborRate_Concreting.Add("BEAM [m3]", 500);
            parameters.price_LaborRate_Concreting.Add("SUSPENDED SLAB [m3]", 500);
            parameters.price_LaborRate_Concreting.Add("SLAB ON GRADE [m3]", 350);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Concreting)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Concreting");
            }

            //19 - Labor Rate - Formworks
            parameters.price_LaborRate_Formworks.Add("FOOTING [m2]", 300);
            parameters.price_LaborRate_Formworks.Add("WALL FOOTING [m2]", 300);
            parameters.price_LaborRate_Formworks.Add("COLUMN [m2]", 300);
            parameters.price_LaborRate_Formworks.Add("STAIRS [m2]", 300);
            parameters.price_LaborRate_Formworks.Add("BEAM [m2]", 300);
            parameters.price_LaborRate_Formworks.Add("SUSPENDED SLAB [m2]", 300);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Formworks)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Formworks");
            }

            //20 - Labor Rate - Rebar
            parameters.price_LaborRate_Rebar.Add("FOOTING [KG]", 17);
            parameters.price_LaborRate_Rebar.Add("WALL FOOTING [KG]", 17);
            parameters.price_LaborRate_Rebar.Add("COLUMN [KG]", 15);
            parameters.price_LaborRate_Rebar.Add("STAIRS [KG]", 15);
            parameters.price_LaborRate_Rebar.Add("BEAM [KG]", 16);
            parameters.price_LaborRate_Rebar.Add("FOOTING TIE BEAM [KG]", 16);
            parameters.price_LaborRate_Rebar.Add("SLAB ON GRADE [KG]", 17);
            parameters.price_LaborRate_Rebar.Add("SUSPENDED SLAB [KG]", 18);
            parameters.price_LaborRate_Rebar.Add("WALLS [KG]", 16);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Rebar)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Rebar");
            }

            //21 - Labor Rate - Paint
            parameters.price_LaborRate_Paint.Add("PAINTER [m2]", 55);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Paint)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Paint");
            }

            //22 - Labor Rate - Tiles
            parameters.price_LaborRate_Tiles.Add("TILES [m2]", 248);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Tiles)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Tiles");
            }

            //23 - Labor Rate - Masonry
            parameters.price_LaborRate_Masonry.Add("MASONRY [m2]", 400);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Masonry)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Masonry");
            }

            //24 - Labor Rate - Roofings
            parameters.price_LaborRate_Roofings.Add("ROOFINGS [m2]", 70);
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Roofings)
            {
                parameters.searchList.Add(dict.Key + " - Labor Rate - Roofings");
            }

            //25.1 - Manpower - Manila
            parameters.price_ManpowerM.Add("Foreman [hr]", 92);
            parameters.price_ManpowerM.Add("Fitter [hr]", 68);
            parameters.price_ManpowerM.Add("Welder [hr]", 68);
            parameters.price_ManpowerM.Add("Electrician [hr]", 68);
            parameters.price_ManpowerM.Add("Carpenter [hr]", 68);
            parameters.price_ManpowerM.Add("Painter [hr]", 67);
            parameters.price_ManpowerM.Add("Mason [hr]", 69);
            parameters.price_ManpowerM.Add("Driver [hr]", 67);
            parameters.price_ManpowerM.Add("Eqpt Operator [hr]", 69);
            parameters.price_ManpowerM.Add("Helper [hr]", 65);
            foreach (DictionaryEntry dict in parameters.price_ManpowerM)
            {
                parameters.searchList.Add(dict.Key + " - Manpower - Manila");
            }

            //25.2 - Manpower - Provincial
            parameters.price_ManpowerP.Add("Foreman [hr]", 83);
            parameters.price_ManpowerP.Add("Fitter [hr]", 62);
            parameters.price_ManpowerP.Add("Welder [hr]", 61);
            parameters.price_ManpowerP.Add("Electrician [hr]", 57);
            parameters.price_ManpowerP.Add("Carpenter [hr]", 54);
            parameters.price_ManpowerP.Add("Painter [hr]", 52);
            parameters.price_ManpowerP.Add("Mason [hr]", 52);
            parameters.price_ManpowerP.Add("Driver [hr]", 50);
            parameters.price_ManpowerP.Add("Eqpt Operator [hr]", 54);
            parameters.price_ManpowerP.Add("Helper [hr]", 48);
            foreach (DictionaryEntry dict in parameters.price_ManpowerP)
            {
                parameters.searchList.Add(dict.Key + " - Manpower - Provincial");
            }

            //26 - Equipment
            parameters.price_Equipment.Add("Crawler Loader (80kW/ 1.5 - 2.0 cu.m) [hr]", 3250);
            parameters.price_Equipment.Add("Crawler Dozer (125kW) [hr]", 4035);
            parameters.price_Equipment.Add("Wheel Loader (20 - 3.0 Cum) [hr]", 3505);
            parameters.price_Equipment.Add("Backhoe Crawler (0.75- 1.0 cum) [hr]", 2990);
            parameters.price_Equipment.Add("Backhoe / Pavement Breaker (1.5 cum) [hr]", 4675);
            parameters.price_Equipment.Add("Motor Grader (90 - 100 kW)  [hr]", 2395);
            parameters.price_Equipment.Add("Pneumatic Tire Roller (20 - 24 Mr) [hr]", 3050);
            parameters.price_Equipment.Add("Vibratory Drum Roller (10 - 14MT) [hr]", 2925);
            parameters.price_Equipment.Add("Dump Truck (8.0 -12.0 cu.m) [hr]", 1875);
            parameters.price_Equipment.Add("Cargo Truck Small (5 - 8 MT) [hr]", 1345);

            parameters.price_Equipment.Add("Cargo Truck Small (10 -15 MT) [hr]", 1730);
            parameters.price_Equipment.Add("Transit Mixer (5.0 -8.0 cum) [hr]", 2130);
            parameters.price_Equipment.Add("Concrete Batching Plant (80 -100 cu.m/hr) [hr]", 4240);
            parameters.price_Equipment.Add("Concrete Trimmer/Slipform Paver (1 meter width) [hr]", 4535);
            parameters.price_Equipment.Add("Asphalt Distributor Truck (2500 - 3500 Gallons) [hr]", 2020);
            parameters.price_Equipment.Add("Asphalt Finisher (3-meter width) [hr]", 2910);
            parameters.price_Equipment.Add("Truck with Boom, small (6-10 MT) [hr]", 1605);
            parameters.price_Equipment.Add("Truck with Boom, small (12 - 15 MT) [hr]", 2155);
            parameters.price_Equipment.Add("Crawler Crane (21-25 MT) [hr]", 3170);
            parameters.price_Equipment.Add("Diesel Pile Hammer [hr]", 2142);

            parameters.price_Equipment.Add("Vibratory Pile Driver [hr]", 1023.5);
            parameters.price_Equipment.Add("Bagger Mixer (1-2 Bags) [hr]", 588);
            parameters.price_Equipment.Add("Concrete Vibrator [hr]", 436.5);
            parameters.price_Equipment.Add("Air Compressor  (Small) [hr]", 383);
            parameters.price_Equipment.Add("Air Compressor (Big) [hr]", 747.5);
            parameters.price_Equipment.Add("Bar Cutter [hr]", 306.5);
            parameters.price_Equipment.Add("Bar Bender [hr]", 403);
            parameters.price_Equipment.Add("Jack Hammer [hr]", 324.15);
            parameters.price_Equipment.Add("Tamping Rammer [hr]", 314);
            parameters.price_Equipment.Add("Welding Machine, Portable, 300A [hr]", 682);

            parameters.price_Equipment.Add("Welding Machine, 600A [hr]", 934.5);
            parameters.price_Equipment.Add("Generator Set 15-25kVA [hr]", 516.5);
            parameters.price_Equipment.Add("Generator Set 50 kVA [hr]", 893);
            parameters.price_Equipment.Add("Sump Pump (Dewatering) 0.75  –2HP- [hr]", 213.5);
            parameters.price_Equipment.Add("Sump Pump (Dewatering) 5HP [hr]", 426.5);
            parameters.price_Equipment.Add("Road Paint Stripper [hr]", 186.5);
            foreach (DictionaryEntry dict in parameters.price_Equipment)
            {
                parameters.searchList.Add(dict.Key + " - Equipment");
            }
        }

        private void initPriceListToDefault()
        {
            //1 - Common Materials
            parameters.price_CommonMaterials["Cyclone Wire (Gauge#10, 2”x2”, 3ft x 10m) [ROLL]"] = 3250;
            parameters.price_CommonMaterials["Gasket (6mm thk, 1m x 1m) [ROLL]"] = 2840;
            parameters.price_CommonMaterials["Acetylene Gas [CYL]"] = 1045;
            parameters.price_CommonMaterials["Oxygen Gas [CYL]"] = 438;
            parameters.price_CommonMaterials["Rugby [CANS]"] = 648;
            parameters.price_CommonMaterials["Vulca Seal [LTR]"] = 470;
            parameters.price_CommonMaterials["Broom, Soft [PC]"] = 218;
            parameters.price_CommonMaterials["Concrete Epoxy [SET]"] = 3000;
            parameters.price_CommonMaterials["Concrete Patching Compound [KGS]"] = 386;
            parameters.price_CommonMaterials["GI Wire (no. 16) [ROLL]"] = 1750;
            parameters.price_CommonMaterials["GI Wire (no. 12) [KG]"] = 70.85;
            parameters.price_CommonMaterials["GI Wire (no. 16) [KG]"] = 71.37;
            parameters.price_CommonMaterials["Non-Shrink Grout [BAGS]"] = 621;
            parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"] = 165;
            parameters.price_CommonMaterials["Sand [m3]"] = 1400;
            parameters.price_CommonMaterials["Rivets 1/8” x ½” [BOX]"] = 200;
            parameters.price_CommonMaterials["Rivets 1-1/2” x ½” [BOX]"] = 250;
            parameters.price_CommonMaterials["Rope (⌀ 1/2”) [MTRS]"] = 26;
            parameters.price_CommonMaterials["Tape, Caution [ROLLS]"] = 800;
            parameters.price_CommonMaterials["Tile Grout (2KG)  [BAGS]"] = 61;
            parameters.price_CommonMaterials["Tiles, Floor (600 x 600) [PC]"] = 152;
            parameters.price_CommonMaterials["Tiles, Wall (300 x 300) [PC]"] = 33;
            parameters.price_CommonMaterials["Broom Stick [PC]"] = 37;
            parameters.price_CommonMaterials["Chalk Stone [PC]"] = 7;
            parameters.price_CommonMaterials["Sandpaper (#100) [MTRS]"] = 202;
            parameters.price_CommonMaterials["Sandpaper (#50) [MTRS]"] = 284;
            parameters.price_CommonMaterials["Common Nail [KG]"] = 150;
            parameters.price_CommonMaterials["Concrete Nail [KG]"] = 170;
            parameters.price_CommonMaterials["Putty, Multipurpose [PAIL]"] = 883;
            parameters.price_CommonMaterials["Tie Wire (No. #16) [25kg/Roll]"] = 1800;
            parameters.price_CommonMaterials["25KG Tile Adhesive (Regular) [BAGS]"] = 300;
            parameters.price_CommonMaterials["25 KG Tile Adhesive (Heavy duty) [BAGS]"] = 550;
            parameters.price_CommonMaterials["CHB 4” (0.10 x 0.20 x 0.40) [PC]"] = 20;
            parameters.price_CommonMaterials["CHB 6” (0.15 x 0.20 x 0.40) [PC]"] = 22;
            parameters.price_CommonMaterials["CHB 8” (0.20 x 0.20 x 0.40) [PC]"] = 26;

            //2 - Paint and Coating
            parameters.price_PaintAndCoating["Acrylic Emulsion [GALS]"] = 705;
            parameters.price_PaintAndCoating["Concrete Epoxy Injection [GALS]"] = 1834;
            parameters.price_PaintAndCoating["Concrete Primer & Sealer [PAIL]"] = 2618;
            parameters.price_PaintAndCoating["Epopatch, Base and Hardener [SETS]"] = 2075;
            parameters.price_PaintAndCoating["Lacquer Thinner [GALS]"] = 359;
            parameters.price_PaintAndCoating["Paint Brush, Bamboo 1-1/2” [PC]"] = 35;
            parameters.price_PaintAndCoating["Paint, Acrylic 1 [GAL]"] = 650;
            parameters.price_PaintAndCoating["Paint, Epoxy Enamel White [GAL]"] = 1100;
            parameters.price_PaintAndCoating["Paint, Epoxy Floor Coating [GALS]"] = 2800;
            parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"] = 865;
            parameters.price_PaintAndCoating["Paint, Epoxy Reducer [GALS]"] = 529;
            parameters.price_PaintAndCoating["Paint Latex Gloss [GAL]"] = 640.2;
            parameters.price_PaintAndCoating["Paint Enamel [GAL]"] = 660;
            parameters.price_PaintAndCoating["Paint, Semi-Gloss [GALS]"] = 675;
            parameters.price_PaintAndCoating["Putty, Masonry [PAIL]"] = 1396;
            parameters.price_PaintAndCoating["Rust Converter [GAL]"] = 771;
            parameters.price_PaintAndCoating["Skim Coat [BAGS]"] = 650;
            parameters.price_PaintAndCoating["Underwater Epoxy [GALS]"] = 2211;
            parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"] = 475;

            //3 - Welding Rod
            parameters.price_WeldingRod["Stainless Welding Rod 308 (3.2mm) [KGS]"] = 500;
            parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [KGS]"] = 125;
            parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [BOX]"] = 2400;
            parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [KGS]"] = 107;
            parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [BOX]"] = 2000;

            //4 - Personal Protective Equipment
            parameters.price_PersonalProtectiveEquipment["Chemical Gloves PAIR Cotton Gloves [PAIRS]"] = 212;
            parameters.price_PersonalProtectiveEquipment["Cotton Gloves [PAIRS]"] = 18;
            parameters.price_PersonalProtectiveEquipment["Dust Mask N95 [PC]"] = 28;
            parameters.price_PersonalProtectiveEquipment["Gloves, Orange Palm [PAIRS]"] = 28;
            parameters.price_PersonalProtectiveEquipment["Hard Hat w/ Headgear and chin strap [20 SET]"] = 221;
            parameters.price_PersonalProtectiveEquipment["Overall with reflector [PC]"] = 1074;
            parameters.price_PersonalProtectiveEquipment["Oxy-Acetylene Cutting Outfit [SET]"] = 17125;
            parameters.price_PersonalProtectiveEquipment["PVC Apron  [PC]"] = 508;
            parameters.price_PersonalProtectiveEquipment["Respirator mask w/ Cartridge [PC]"] = 2117;
            parameters.price_PersonalProtectiveEquipment["Respirator, Filter Cartridge [PACK]"] = 656;
            parameters.price_PersonalProtectiveEquipment["Safety Goggles [PC]"] = 130;
            parameters.price_PersonalProtectiveEquipment["Safety Rubber boots [PAIR]"] = 391;
            parameters.price_PersonalProtectiveEquipment["Safety Shoes  [PAIR]"] = 960;
            parameters.price_PersonalProtectiveEquipment["Safety Vest [PC]"] = 120;
            parameters.price_PersonalProtectiveEquipment["Welding Mask  [PC]"] = 130;
            parameters.price_PersonalProtectiveEquipment["Welding Apron [PC]"] = 262;
            parameters.price_PersonalProtectiveEquipment["Welding Mask, Auto Darkening [SETS]"] = 2500;

            //5 - Tools 
            parameters.price_Tools["Adjustable Wrench Set 4”— 24” [SET]"] = 2468;
            parameters.price_Tools["Baby Roller (Cotton) 4” [PC]"] = 63;
            parameters.price_Tools["Ball Hammer [PC]"] = 671;
            parameters.price_Tools["Bench Vise [UNIT]"] = 5122;
            parameters.price_Tools["Blade Cutter [PC]"] = 57;
            parameters.price_Tools["Camlock (Male & Female Set) 50mm DIA [SET]"] = 1585;
            parameters.price_Tools["Chipping Gun [UNIT]"] = 30607;
            parameters.price_Tools["Combination Wrench Set 6mm — 32mm [SET]"] = 3340;
            parameters.price_Tools["Cut-off Wheel ⌀ 16” [BOX]"] = 6295;
            parameters.price_Tools["Cutting Disc ⌀ 4” [BOX]"] = 1570;
            parameters.price_Tools["Cutting Disc ⌀ 7” [BOX]"] = 3785;
            parameters.price_Tools["Drill Bit [BOX]"] = 608;
            parameters.price_Tools["Electrical Plier [PC]"] = 408;
            parameters.price_Tools["Grinder, Angle 4” [UNIT]"] = 4017;
            parameters.price_Tools["Grinder, Angle 7” [UNIT]"] = 9184;
            parameters.price_Tools["Grinder, Baby [UNIT]"] = 5053;
            parameters.price_Tools["Grinder, Mother [UNIT]"] = 9025;
            parameters.price_Tools["Grinding Disc ⌀ 4” [BOX]"] = 929;
            parameters.price_Tools["Grinding Disc ⌀ 7” [BOX]"] = 1875;
            parameters.price_Tools["Heat Gun [UNIT]"] = 3500;
            parameters.price_Tools["Ladder (A-Type), 6h, Aluminum [PC]"] = 2270;
            parameters.price_Tools["Level Bar 24” [PC]"] = 540;
            parameters.price_Tools["Paint Brush 4” [PC]"] = 63;
            parameters.price_Tools["Paint Brush 2” [PC]"] = 39;
            parameters.price_Tools["Portable Axial Fan Blower  ⌀  8” [SET]"] = 8295;
            parameters.price_Tools["Power Ratchet [UNIT]"] = 8068;
            parameters.price_Tools["Rivet Gun / Riveter [UNIT]"] = 820;
            parameters.price_Tools["Roller Brush 7” [PC]"] = 97;
            parameters.price_Tools["Screwdriver, Flat [SET]"] = 273;
            parameters.price_Tools["Screwdriver, Philip [SET]"] = 290;
            parameters.price_Tools["Shovel, Pointed [PC]"] = 500;
            parameters.price_Tools["Snap Ring Plier [SET]"] = 548;
            parameters.price_Tools["Socket Wrench Set 19mm — 50mm [SET]"] = 6329;
            parameters.price_Tools["Speed Cutter [UNIT]"] = 10000;
            parameters.price_Tools["Steel Brush [PC]"] = 30;
            parameters.price_Tools["Test Light [PC]"] = 158;
            parameters.price_Tools["Test Wrench [UNIT]"] = 5940;
            parameters.price_Tools["Torque Wrench [UNIT]"] = 6217;
            parameters.price_Tools["Vise Grip [PC]"] = 456;
            parameters.price_Tools["Welding Machine (Portable) 12.3 kVA(20-300A) [UNIT]"] = 19500;

            //6 - Ready Mix Concrete 
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days [m3]"] = 4540;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 14 Days [m3]"] = 4810;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 7 Days [m3]"] = 5070;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 3 Days [m3]"] = 5620;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 28 Days [m3]"] = 4640;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 14 Days [m3]"] = 5090;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 7 Days [m3]"] = 5360;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 3 Days [m3]"] = 5660;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 28 Days [m3]"] = 4910;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 14 Days [m3]"] = 5300;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4500PSI (31 Mpa) @ 28 Days [m3]"] = 5020;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 5000PSI (34.5 Mpa) @ 28 Days [m3]"] = 5280;

            //7 - Gravel
            parameters.price_Gravel["GRAVEL G1 [m3]"] = 530;
            parameters.price_Gravel["GRAVEL G2 [m3]"] = 420;
            parameters.price_Gravel["GRAVEL G1- ½” [m3]"] = 435;
            parameters.price_Gravel["GRAVEL G2- ½” [m3]"] = 420;
            parameters.price_Gravel["GRAVEL ¾” [m3]"] = 440;

            //8 - Formworks and Lumber
            parameters.price_FormworksAndLumber["Lumber [2”x 2” x 8']"] = 130;
            parameters.price_FormworksAndLumber["Lumber [2”x 2” x 10']"] = 140;
            parameters.price_FormworksAndLumber["Lumber [2”x 2” x 12']"] = 145;
            parameters.price_FormworksAndLumber["Lumber [2”x 3”x 8']"] = 150;
            parameters.price_FormworksAndLumber["Lumber [2”x 3”x 10']"] = 150;
            parameters.price_FormworksAndLumber["Lumber [2”x 3”x 12']"] = 180;
            parameters.price_FormworksAndLumber["Lumber [2”x4”x 8']"] = 265;
            parameters.price_FormworksAndLumber["Lumber [2”x4”x 10']"] = 275;
            parameters.price_FormworksAndLumber["Lumber [2”x4”x 12']"] = 280;
            parameters.price_FormworksAndLumber["PLYWOOD 1/2” [1.22m x 2.44m]"] = 674;
            parameters.price_FormworksAndLumber["PLYWOOD 3/4” [1.22m x 2.44m]"] = 1112;
            parameters.price_FormworksAndLumber["PLYWOOD 1/4” [1.22m x 2.44m]"] = 381;
            parameters.price_FormworksAndLumber["PLYWOOD 1/8”[1.22m x 2.44m]"] = 270;
            parameters.price_FormworksAndLumber["ECOBOARD 1/2”[1.22m x 2.44m]"] = 2100;
            parameters.price_FormworksAndLumber["ECOBOARD 3/4” [1.22m x 2.44m]"] = 3100;
            parameters.price_FormworksAndLumber["ECOBOARD 1/4” [1.22m x 2.44m]"] = 1620;
            parameters.price_FormworksAndLumber["ECOBOARD 1/8” [1.22m x 2.44m]"] = 1200;
            parameters.price_FormworksAndLumber["PHENOLIC BOARD- 1/2” [1.22m x 2.44m]"] = 2012.64;
            parameters.price_FormworksAndLumber["PHENOLIC BOARD- 3/4” [1.22m x 2.44m]"] = 2656.20;

            //9 - Roof Materials
            parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 0.7mm thick) [6 m]"] = 314;
            parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 0.7mm thick) [6 m]"] = 359;
            parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 0.9mm thick) [6 m]"] = 544;
            parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.0mm thick) [6 m]"] = 409;
            parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.0mm thick) [6 m]"] = 471;
            parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.0mm thick) [6 m]"] = 583;
            parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.2mm thick) [6 m]"] = 499;
            parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.2mm thick) [6 m]"] = 572;
            parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.2mm thick ) [6 m]"] = 712;
            parameters.price_RoofMaterials["Corrugated G.I Sheet, Gauge 26 (0.551mmx2.44 mm) [m2]"] = 423;
            parameters.price_RoofMaterials["Plain G.I Sheet, Gauge 24 (4ft x 8 ft) [UNIT]"] = 622;
            parameters.price_RoofMaterials["G.I. Roof Nails [KG]"] = 120;
            parameters.price_RoofMaterials["G.I Rivets [KG]"] = 152;
            parameters.price_RoofMaterials["G.I Washers [KG]"] = 126;
            parameters.price_RoofMaterials["Umbrella Nails [KG]"] = 120;

            //10 - Tubular Steel (1mm thick)
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 20mm x 20mm x 1.0mm thick [6m]"] = 244;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.0mm thick [6m]"] = 325;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.0mm thick [6m]"] = 456;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.0mm thick [6m]"] = 443;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.0mm thick [6m]"] = 493;

            //11 - Tubular Steel (1.2mm thick)
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.2mm thick [6m]"] = 399;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.2mm thick [6m]"] = 633;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.2mm thick [6m]"] = 482;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.2mm thick [6m]"] = 605;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.2mm thick [6m]"] = 748;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.2mm thick [6m]"] = 893;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.2mm thick [6m]"] = 1551;

            //12 - Tubular Steel (1.5mm thick)
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.5mm thick [6m]"] = 572;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.5mm thick [6m]"] = 898;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.5mm thick [6m]"] = 745;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.5mm thick [6m]"] = 940;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.5mm thick [6m]"] = 1182;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.5mm thick [6m]"] = 1329;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.5mm thick [6m]"] = 1932;

            //13 - Embankment
            parameters.price_Embankment["Common Borrow [m3]"] = 392.65;
            parameters.price_Embankment["Selected Borrow [m3]"] = 397.49;
            parameters.price_Embankment["Mixed Sand & Gravel [m3]"] = 597.211;
            parameters.price_Embankment["Rock [m3]"] = 613.76;

            //14 - Rebar Grade 33 (230 Mpa)
            parameters.price_RebarGrade33["Compression Coupler GRADE 33 [PC]"] = 450;
            parameters.price_RebarGrade33["Tension Coupler GRADE 33 [PC]"] = 450;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [6m]"] = 147.84;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [7.5m]"] = 184.80;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [9m]"] = 221.76;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [10.5m]"] = 258.72;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [12m]"] = 295.68;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [6m]"] = 213.12;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [7.5m]"] = 266.4;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [9m]"] = 319.68;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [10.5m]"] = 372.96;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [12m]"] = 426.24;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [6m]"] = 378.72;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [7.5m]"] = 473.4;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [9m]"] = 568.08;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [10.5m]"] = 662.76;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [12m]"] = 757.44;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [6m]"] = 591.84;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [7.5m]"] = 739.8;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [9m]"] = 887.76;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [10.5m]"] = 1035.72;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [12m]"] = 1183.68;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [6m]"] = 924.72;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [7.5m]"] = 1155.9;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [9m]"] = 1387.08;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [10.5m]"] = 1618.26;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [12m]"] = 1849.44;

            //15 - Rebar Grade 40 (275 Mpa) 
            parameters.price_RebarGrade40["Compression Coupler GRADE 40 [PC]"] = 500;
            parameters.price_RebarGrade40["Tension Coupler GRADE 40 [PC]"] = 500;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [6m]"] = 162.62;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [7.5m]"] = 203.28;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [9m]"] = 243.94;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [10.5m]"] = 284.59;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [12m]"] = 325.25;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [6m]"] = 234.43;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [7.5m]"] = 293.04;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [9m]"] = 351.65;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [10.5m]"] = 410.26;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [12m]"] = 468.86;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [6m]"] = 412.80;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [7.5m]"] = 516.01;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [9m]"] = 619.21;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [10.5m]"] = 722.41;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [12m]"] = 825.61;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [6m]"] = 645.11;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [7.5m]"] = 806.38;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [9m]"] = 967.66;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [10.5m]"] = 1128.93;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [12m]"] = 1290.21;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [6m]"] = 1007.94;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [7.5m]"] = 1259.93;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [9m]"] = 1511.92;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [10.5m]"] = 1763.90;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [12m]"] = 2015.89;

            //16 -  Rebar Grade 60 (415 Mpa)
            parameters.price_RebarGrade60["Compression Coupler GRADE 60 [PC]"] = 600;
            parameters.price_RebarGrade60["Tension Coupler GRADE 60 [PC]"] = 600;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [6m]"] = 167.8;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [7.5m]"] = 209.74;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [9m]"] = 251.7;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [10.5m]"] = 293.64;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [12m]"] = 335.6;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [6m]"] = 241.9;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [7.5m]"] = 302.36;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [9m]"] = 362.84;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [10.5m]"] = 423.3;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [12m]"] = 483.78;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [6m]"] = 426.06;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [7.5m]"] = 532.58;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [9m]"] = 639.09;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [10.5m]"] = 745.61;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [12m]"] = 852.12;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [6m]"] = 665.82;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [7.5m]"] = 832.28;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [9m]"] = 998.73;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [10.5m]"] = 1165.19;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [12m]"] = 2080.62;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [6m]"] = 1040.31;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [7.5m]"] = 1300.39;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [9m]"] = 1560.47;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [10.5m]"] = 1820.54;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [12m]"] = 2080.62;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [6m]"] = 1305.18;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [7.5m]"] = 1631.48;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [9m]"] = 1957.77;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [10.5m]"] = 2284.07;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [12m]"] = 2610.36;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [6m]"] = 1719.66;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [7.5m]"] = 2149.58;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [9m]"] = 2579.50;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [10.5m]"] = 3009.83;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [12m]"] = 4352.95;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [6m]"] = 2176.48;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [7.5m]"] = 2720.60;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [9m]"] = 3264.71;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [10.5m]"] = 3808.83;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [12m]"] = 4352.94;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [6m]"] = 1827;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [7.5m]"] = 2285;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [9m]"] = 2740;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [10.5m]"] = 3125;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [12m]"] = 3653;

            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [6m]"] = 2828;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [7.5m]"] = 3535;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [9m]"] = 4241;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [10.5m]"] = 4750;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [12m]"] = 5655;

            //17 - Labor Rate - Earthworks
            parameters.price_LaborRate_Earthworks["Excavation [m3]"] = 400;
            parameters.price_LaborRate_Earthworks["Backfilling and Compaction [m3]"] = 400;
            parameters.price_LaborRate_Earthworks["Grading and Compaction [m3]"] = 350;
            parameters.price_LaborRate_Earthworks["Gravel Bedding and Compaction [m3]"] = 300;
            parameters.price_LaborRate_Earthworks["Soil Poisoning [m2]"] = 60;

            //18 - Labor Rate - Concreting
            parameters.price_LaborRate_Concreting["FOOTING [m3]"] = 400;
            parameters.price_LaborRate_Concreting["WALL FOOTING [m3]"] = 400;
            parameters.price_LaborRate_Concreting["COLUMN [m3]"] = 450;
            parameters.price_LaborRate_Concreting["STAIRS [m3]"] = 450;
            parameters.price_LaborRate_Concreting["BEAM [m3]"] = 500;
            parameters.price_LaborRate_Concreting["SUSPENDED SLAB [m3]"] = 500;
            parameters.price_LaborRate_Concreting["SLAB ON GRADE [m3]"] = 350;

            //19 - Labor Rate - Formworks
            parameters.price_LaborRate_Formworks["FOOTING [m2]"] = 300;
            parameters.price_LaborRate_Formworks["WALL FOOTING [m2]"] = 300;
            parameters.price_LaborRate_Formworks["COLUMN [m2]"] = 300;
            parameters.price_LaborRate_Formworks["STAIRS [m2]"] = 300;
            parameters.price_LaborRate_Formworks["BEAM [m2]"] = 300;
            parameters.price_LaborRate_Formworks["SUSPENDED SLAB [m2]"] = 300;

            //20 - Labor Rate - Rebar
            parameters.price_LaborRate_Rebar["FOOTING [KG]"] = 17;
            parameters.price_LaborRate_Rebar["WALL FOOTING [KG]"] = 17;
            parameters.price_LaborRate_Rebar["COLUMN [KG]"] = 15;
            parameters.price_LaborRate_Rebar["STAIRS [KG]"] = 15;
            parameters.price_LaborRate_Rebar["BEAM [KG]"] = 16;
            parameters.price_LaborRate_Rebar["FOOTING TIE BEAM [KG]"] = 16;
            parameters.price_LaborRate_Rebar["SLAB ON GRADE [KG]"] = 17;
            parameters.price_LaborRate_Rebar["SUSPENDED SLAB [KG]"] = 18;
            parameters.price_LaborRate_Rebar["WALLS [KG]"] = 16;

            //21 - Labor Rate - Paint
            parameters.price_LaborRate_Paint["PAINTER [m2]"] = 55;

            //22 - Labor Rate - Tiles
            parameters.price_LaborRate_Tiles["TILES [m2]"] = 248;

            //23 - Labor Rate - Masonry
            parameters.price_LaborRate_Masonry["MASONRY [m2]"] = 400;

            //24 - Labor Rate - Roofings
            parameters.price_LaborRate_Roofings["ROOFINGS [m2]"] = 70;

            //25.1 - Manpower - Manila
            parameters.price_ManpowerM["Foreman [hr]"] = 92;
            parameters.price_ManpowerM["Fitter [hr]"] = 68;
            parameters.price_ManpowerM["Welder [hr]"] = 68;
            parameters.price_ManpowerM["Electrician [hr]"] = 68;
            parameters.price_ManpowerM["Carpenter [hr]"] = 68;
            parameters.price_ManpowerM["Painter [hr]"] = 67;
            parameters.price_ManpowerM["Mason [hr]"] = 69;
            parameters.price_ManpowerM["Driver [hr]"] = 67;
            parameters.price_ManpowerM["Eqpt Operator [hr]"] = 69;
            parameters.price_ManpowerM["Helper [hr]"] = 65;

            //25.2 - Manpower - Provincial
            parameters.price_ManpowerP["Foreman [hr]"] = 83;
            parameters.price_ManpowerP["Fitter [hr]"] = 62;
            parameters.price_ManpowerP["Welder [hr]"] = 61;
            parameters.price_ManpowerP["Electrician [hr]"] = 57;
            parameters.price_ManpowerP["Carpenter [hr]"] = 54;
            parameters.price_ManpowerP["Painter [hr]"] = 52;
            parameters.price_ManpowerP["Mason [hr]"] = 52;
            parameters.price_ManpowerP["Driver [hr]"] = 50;
            parameters.price_ManpowerP["Eqpt Operator [hr]"] = 54;
            parameters.price_ManpowerP["Helper [hr]"] = 48;

            //26 - Equipment
            parameters.price_Equipment["Crawler Loader (80kW/ 1.5 - 2.0 cu.m) [hr]"] = 3250;
            parameters.price_Equipment["Crawler Dozer (125kW) [hr]"] = 4035;
            parameters.price_Equipment["Wheel Loader (20 - 3.0 Cum) [hr]"] = 3505;
            parameters.price_Equipment["Backhoe Crawler (0.75- 1.0 cum) [hr]"] = 2990;
            parameters.price_Equipment["Backhoe / Pavement Breaker (1.5 cum) [hr]"] = 4675;
            parameters.price_Equipment["Motor Grader (90 - 100 kW)  [hr]"] = 2395;
            parameters.price_Equipment["Pneumatic Tire Roller (20 - 24 Mr) [hr]"] = 3050;
            parameters.price_Equipment["Vibratory Drum Roller (10 - 14MT) [hr]"] = 2925;
            parameters.price_Equipment["Dump Truck (8.0 -12.0 cu.m) [hr]"] = 1875;
            parameters.price_Equipment["Cargo Truck Small (5 - 8 MT) [hr]"] = 1345;

            parameters.price_Equipment["Cargo Truck Small (10 -15 MT) [hr]"] = 1730;
            parameters.price_Equipment["Transit Mixer (5.0 -8.0 cum) [hr]"] = 2130;
            parameters.price_Equipment["Concrete Batching Plant (80 -100 cu.m/hr) [hr]"] = 4240;
            parameters.price_Equipment["Concrete Trimmer/Slipform Paver (1 meter width) [hr]"] = 4535;
            parameters.price_Equipment["Asphalt Distributor Truck (2500 - 3500 Gallons) [hr]"] = 2020;
            parameters.price_Equipment["Asphalt Finisher (3-meter width) [hr]"] = 2910;
            parameters.price_Equipment["Truck with Boom, small (6-10 MT) [hr]"] = 1605;
            parameters.price_Equipment["Truck with Boom, small (12 - 15 MT) [hr]"] = 2155;
            parameters.price_Equipment["Crawler Crane (21-25 MT) [hr]"] = 3170;
            parameters.price_Equipment["Diesel Pile Hammer [hr]"] = 2142;

            parameters.price_Equipment["Vibratory Pile Driver [hr]"] = 1023.5;
            parameters.price_Equipment["Bagger Mixer (1-2 Bags) [hr]"] = 588;
            parameters.price_Equipment["Concrete Vibrator [hr]"] = 436.5;
            parameters.price_Equipment["Air Compressor  (Small) [hr]"] = 383;
            parameters.price_Equipment["Air Compressor (Big) [hr]"] = 747.5;
            parameters.price_Equipment["Bar Cutter [hr]"] = 306.5;
            parameters.price_Equipment["Bar Bender [hr]"] = 403;
            parameters.price_Equipment["Jack Hammer [hr]"] = 324.15;
            parameters.price_Equipment["Tamping Rammer [hr]"] = 314;
            parameters.price_Equipment["Welding Machine, Portable, 300A [hr]"] = 682;

            parameters.price_Equipment["Welding Machine, 600A [hr]"] = 934.5;
            parameters.price_Equipment["Generator Set 15-25kVA [hr]"] = 516.5;
            parameters.price_Equipment["Generator Set 50 kVA [hr]"] = 893;
            parameters.price_Equipment["Sump Pump (Dewatering) 0.75  –2HP- [hr]"] = 213.5;
            parameters.price_Equipment["Sump Pump (Dewatering) 5HP [hr]"] = 426.5;
            parameters.price_Equipment["Road Paint Stripper [hr]"] = 186.5;
        }

        private void AdjustPriceView()
        {
            //1 - Common Materials
            price_1_1.Text = parameters.price_CommonMaterials["Cyclone Wire (Gauge#10, 2”x2”, 3ft x 10m) [ROLL]"].ToString();
            price_1_2.Text = parameters.price_CommonMaterials["Gasket (6mm thk, 1m x 1m) [ROLL]"].ToString();
            price_1_3.Text = parameters.price_CommonMaterials["Acetylene Gas [CYL]"].ToString();
            price_1_4.Text = parameters.price_CommonMaterials["Oxygen Gas [CYL]"].ToString();
            price_1_5.Text = parameters.price_CommonMaterials["Rugby [CANS]"].ToString();
            price_1_6.Text = parameters.price_CommonMaterials["Vulca Seal [LTR]"].ToString();
            price_1_7.Text = parameters.price_CommonMaterials["Broom, Soft [PC]"].ToString();
            price_1_8.Text = parameters.price_CommonMaterials["Concrete Epoxy [SET]"].ToString();
            price_1_9.Text = parameters.price_CommonMaterials["Concrete Patching Compound [KGS]"].ToString();
            price_1_10.Text = parameters.price_CommonMaterials["GI Wire (no. 16) [ROLL]"].ToString();
            price_1_11.Text = parameters.price_CommonMaterials["GI Wire (no. 12) [KG]"].ToString();
            price_1_12.Text = parameters.price_CommonMaterials["GI Wire (no. 16) [KG]"].ToString();
            price_1_13.Text = parameters.price_CommonMaterials["Non-Shrink Grout [BAGS]"].ToString();
            price_1_14.Text = parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"].ToString();
            price_1_15.Text = parameters.price_CommonMaterials["Sand [m3]"].ToString();
            price_1_16.Text = parameters.price_CommonMaterials["Rivets 1/8” x ½” [BOX]"].ToString();
            price_1_17.Text = parameters.price_CommonMaterials["Rivets 1-1/2” x ½” [BOX]"].ToString();
            price_1_18.Text = parameters.price_CommonMaterials["Rope (⌀ 1/2”) [MTRS]"].ToString();
            price_1_19.Text = parameters.price_CommonMaterials["Tape, Caution [ROLLS]"].ToString();
            price_1_20.Text = parameters.price_CommonMaterials["Tile Grout (2KG)  [BAGS]"].ToString();
            price_1_21.Text = parameters.price_CommonMaterials["Tiles, Floor (600 x 600) [PC]"].ToString();
            price_1_22.Text = parameters.price_CommonMaterials["Tiles, Wall (300 x 300) [PC]"].ToString();
            price_1_23.Text = parameters.price_CommonMaterials["Broom Stick [PC]"].ToString();
            price_1_24.Text = parameters.price_CommonMaterials["Chalk Stone [PC]"].ToString();
            price_1_25.Text = parameters.price_CommonMaterials["Sandpaper (#100) [MTRS]"].ToString();
            price_1_26.Text = parameters.price_CommonMaterials["Sandpaper (#50) [MTRS]"].ToString();
            price_1_27.Text = parameters.price_CommonMaterials["Common Nail [KG]"].ToString();
            price_1_28.Text = parameters.price_CommonMaterials["Concrete Nail [KG]"].ToString();
            price_1_29.Text = parameters.price_CommonMaterials["Putty, Multipurpose [PAIL]"].ToString();
            price_1_30.Text = parameters.price_CommonMaterials["Tie Wire (No. #16) [25kg/Roll]"].ToString();
            price_1_31.Text = parameters.price_CommonMaterials["25KG Tile Adhesive (Regular) [BAGS]"].ToString();
            price_1_32.Text = parameters.price_CommonMaterials["25 KG Tile Adhesive (Heavy duty) [BAGS]"].ToString();
            price_1_33.Text = parameters.price_CommonMaterials["CHB 4” (0.10 x 0.20 x 0.40) [PC]"].ToString();
            price_1_34.Text = parameters.price_CommonMaterials["CHB 6” (0.15 x 0.20 x 0.40) [PC]"].ToString();
            price_1_35.Text = parameters.price_CommonMaterials["CHB 8” (0.20 x 0.20 x 0.40) [PC]"].ToString();

            //2 - Paint and Coating
            price_2_1.Text = parameters.price_PaintAndCoating["Acrylic Emulsion [GALS]"].ToString();
            price_2_2.Text = parameters.price_PaintAndCoating["Concrete Epoxy Injection [GALS]"].ToString();
            price_2_3.Text = parameters.price_PaintAndCoating["Concrete Primer & Sealer [PAIL]"].ToString();
            price_2_4.Text = parameters.price_PaintAndCoating["Epopatch, Base and Hardener [SETS]"].ToString();
            price_2_5.Text = parameters.price_PaintAndCoating["Lacquer Thinner [GALS]"].ToString();
            price_2_6.Text = parameters.price_PaintAndCoating["Paint Brush, Bamboo 1-1/2” [PC]"].ToString();
            price_2_7.Text = parameters.price_PaintAndCoating["Paint, Acrylic 1 [GAL]"].ToString();
            price_2_8.Text = parameters.price_PaintAndCoating["Paint, Epoxy Enamel White [GAL]"].ToString();
            price_2_9.Text = parameters.price_PaintAndCoating["Paint, Epoxy Floor Coating [GALS]"].ToString();
            price_2_10.Text = parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"].ToString();
            price_2_11.Text = parameters.price_PaintAndCoating["Paint, Epoxy Reducer [GALS]"].ToString();
            price_2_12.Text = parameters.price_PaintAndCoating["Paint Latex Gloss [GAL]"].ToString();
            price_2_13.Text = parameters.price_PaintAndCoating["Paint Enamel [GAL]"].ToString();
            price_2_14.Text = parameters.price_PaintAndCoating["Paint, Semi-Gloss [GALS]"].ToString();
            price_2_15.Text = parameters.price_PaintAndCoating["Putty, Masonry [PAIL]"].ToString();
            price_2_16.Text = parameters.price_PaintAndCoating["Rust Converter [GAL]"].ToString();
            price_2_17.Text = parameters.price_PaintAndCoating["Skim Coat [BAGS]"].ToString();
            price_2_18.Text = parameters.price_PaintAndCoating["Underwater Epoxy [GALS]"].ToString();
            price_2_19.Text = parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"].ToString();

            //3 - Welding Rod
            price_3_1.Text = parameters.price_WeldingRod["Stainless Welding Rod 308 (3.2mm) [KGS]"].ToString();
            price_3_2.Text = parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [KGS]"].ToString();
            price_3_3.Text = parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [BOX]"].ToString();
            price_3_4.Text = parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [KGS]"].ToString();
            price_3_5.Text = parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [BOX]"].ToString();

            //4 - Personal Protective Equipment
            price_4_1.Text = parameters.price_PersonalProtectiveEquipment["Chemical Gloves PAIR Cotton Gloves [PAIRS]"].ToString();
            price_4_2.Text = parameters.price_PersonalProtectiveEquipment["Cotton Gloves [PAIRS]"].ToString();
            price_4_3.Text = parameters.price_PersonalProtectiveEquipment["Dust Mask N95 [PC]"].ToString();
            price_4_4.Text = parameters.price_PersonalProtectiveEquipment["Gloves, Orange Palm [PAIRS]"].ToString();
            price_4_5.Text = parameters.price_PersonalProtectiveEquipment["Hard Hat w/ Headgear and chin strap [20 SET]"].ToString();
            price_4_6.Text = parameters.price_PersonalProtectiveEquipment["Overall with reflector [PC]"].ToString();
            price_4_7.Text = parameters.price_PersonalProtectiveEquipment["Oxy-Acetylene Cutting Outfit [SET]"].ToString();
            price_4_8.Text = parameters.price_PersonalProtectiveEquipment["PVC Apron  [PC]"].ToString();
            price_4_9.Text = parameters.price_PersonalProtectiveEquipment["Respirator mask w/ Cartridge [PC]"].ToString();
            price_4_10.Text = parameters.price_PersonalProtectiveEquipment["Respirator, Filter Cartridge [PACK]"].ToString();
            price_4_11.Text = parameters.price_PersonalProtectiveEquipment["Safety Goggles [PC]"].ToString();
            price_4_12.Text = parameters.price_PersonalProtectiveEquipment["Safety Rubber boots [PAIR]"].ToString();
            price_4_13.Text = parameters.price_PersonalProtectiveEquipment["Safety Shoes  [PAIR]"].ToString();
            price_4_14.Text = parameters.price_PersonalProtectiveEquipment["Safety Vest [PC]"].ToString();
            price_4_15.Text = parameters.price_PersonalProtectiveEquipment["Welding Mask  [PC]"].ToString();
            price_4_16.Text = parameters.price_PersonalProtectiveEquipment["Welding Apron [PC]"].ToString();
            price_4_17.Text = parameters.price_PersonalProtectiveEquipment["Welding Mask, Auto Darkening [SETS]"].ToString();

            //5 - Tools
            price_5_1.Text = parameters.price_Tools["Adjustable Wrench Set 4”— 24” [SET]"].ToString();
            price_5_2.Text = parameters.price_Tools["Baby Roller (Cotton) 4” [PC]"].ToString();
            price_5_3.Text = parameters.price_Tools["Ball Hammer [PC]"].ToString();
            price_5_4.Text = parameters.price_Tools["Bench Vise [UNIT]"].ToString();
            price_5_5.Text = parameters.price_Tools["Blade Cutter [PC]"].ToString();
            price_5_6.Text = parameters.price_Tools["Camlock (Male & Female Set) 50mm DIA [SET]"].ToString();
            price_5_7.Text = parameters.price_Tools["Chipping Gun [UNIT]"].ToString();
            price_5_8.Text = parameters.price_Tools["Combination Wrench Set 6mm — 32mm [SET]"].ToString();
            price_5_9.Text = parameters.price_Tools["Cut-off Wheel ⌀ 16” [BOX]"].ToString();
            price_5_10.Text = parameters.price_Tools["Cutting Disc ⌀ 4” [BOX]"].ToString();
            price_5_11.Text = parameters.price_Tools["Cutting Disc ⌀ 7” [BOX]"].ToString();
            price_5_12.Text = parameters.price_Tools["Drill Bit [BOX]"].ToString();
            price_5_13.Text = parameters.price_Tools["Electrical Plier [PC]"].ToString();
            price_5_14.Text = parameters.price_Tools["Grinder, Angle 4” [UNIT]"].ToString();
            price_5_15.Text = parameters.price_Tools["Grinder, Angle 7” [UNIT]"].ToString();
            price_5_16.Text = parameters.price_Tools["Grinder, Baby [UNIT]"].ToString();
            price_5_17.Text = parameters.price_Tools["Grinder, Mother [UNIT]"].ToString();
            price_5_18.Text = parameters.price_Tools["Grinding Disc ⌀ 4” [BOX]"].ToString();
            price_5_19.Text = parameters.price_Tools["Grinding Disc ⌀ 7” [BOX]"].ToString();
            price_5_20.Text = parameters.price_Tools["Heat Gun [UNIT]"].ToString();
            price_5_21.Text = parameters.price_Tools["Ladder (A-Type), 6h, Aluminum [PC]"].ToString();
            price_5_22.Text = parameters.price_Tools["Level Bar 24” [PC]"].ToString();
            price_5_23.Text = parameters.price_Tools["Paint Brush 4” [PC]"].ToString();
            price_5_24.Text = parameters.price_Tools["Paint Brush 2” [PC]"].ToString();
            price_5_25.Text = parameters.price_Tools["Portable Axial Fan Blower  ⌀  8” [SET]"].ToString();
            price_5_26.Text = parameters.price_Tools["Power Ratchet [UNIT]"].ToString(); 
            price_5_27.Text = parameters.price_Tools["Rivet Gun / Riveter [UNIT]"].ToString();
            price_5_28.Text = parameters.price_Tools["Roller Brush 7” [PC]"].ToString(); 
            price_5_29.Text = parameters.price_Tools["Screwdriver, Flat [SET]"].ToString();
            price_5_30.Text = parameters.price_Tools["Screwdriver, Philip [SET]"].ToString();
            price_5_31.Text = parameters.price_Tools["Shovel, Pointed [PC]"].ToString();
            price_5_32.Text = parameters.price_Tools["Snap Ring Plier [SET]"].ToString();
            price_5_33.Text = parameters.price_Tools["Socket Wrench Set 19mm — 50mm [SET]"].ToString();
            price_5_34.Text = parameters.price_Tools["Speed Cutter [UNIT]"].ToString();
            price_5_35.Text = parameters.price_Tools["Steel Brush [PC]"].ToString();
            price_5_36.Text = parameters.price_Tools["Test Light [PC]"].ToString();
            price_5_37.Text = parameters.price_Tools["Test Wrench [UNIT]"].ToString(); 
            price_5_38.Text = parameters.price_Tools["Torque Wrench [UNIT]"].ToString();
            price_5_39.Text = parameters.price_Tools["Vise Grip [PC]"].ToString();
            price_5_40.Text = parameters.price_Tools["Welding Machine (Portable) 12.3 kVA(20-300A) [UNIT]"].ToString();

            //6 - Ready Mix Concrete
            price_6_1.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days [m3]"].ToString();
            price_6_2.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 14 Days [m3]"].ToString();
            price_6_3.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 7 Days [m3]"].ToString();
            price_6_4.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 3 Days [m3]"].ToString();
            price_6_5.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 28 Days [m3]"].ToString();
            price_6_6.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 14 Days [m3]"].ToString();
            price_6_7.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 7 Days [m3]"].ToString();
            price_6_8.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 3 Days [m3]"].ToString();
            price_6_9.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 28 Days [m3]"].ToString();
            price_6_10.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 14 Days [m3]"].ToString();
            price_6_11.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4500PSI (31 Mpa) @ 28 Days [m3]"].ToString();
            price_6_12.Text = parameters.price_ReadyMixConcrete["Ready Mix Concrete, 5000PSI (34.5 Mpa) @ 28 Days [m3]"].ToString();

            //7 - Gravel
            price_7_1.Text = parameters.price_Gravel["GRAVEL G1 [m3]"].ToString();
            price_7_2.Text = parameters.price_Gravel["GRAVEL G2 [m3]"].ToString();
            price_7_3.Text = parameters.price_Gravel["GRAVEL G1- ½” [m3]"].ToString();
            price_7_4.Text = parameters.price_Gravel["GRAVEL G2- ½” [m3]"].ToString();
            price_7_5.Text = parameters.price_Gravel["GRAVEL ¾” [m3]"].ToString();

            //8 - Formworks and Lumber
            price_8_1.Text = parameters.price_FormworksAndLumber["Lumber [2”x 2” x 8']"].ToString();
            price_8_2.Text = parameters.price_FormworksAndLumber["Lumber [2”x 2” x 10']"].ToString();
            price_8_3.Text = parameters.price_FormworksAndLumber["Lumber [2”x 2” x 12']"].ToString();
            price_8_4.Text = parameters.price_FormworksAndLumber["Lumber [2”x 3”x 8']"].ToString();
            price_8_5.Text = parameters.price_FormworksAndLumber["Lumber [2”x 3”x 10']"].ToString();
            price_8_6.Text = parameters.price_FormworksAndLumber["Lumber [2”x 3”x 12']"].ToString();
            price_8_7.Text = parameters.price_FormworksAndLumber["Lumber [2”x4”x 8']"].ToString();
            price_8_8.Text = parameters.price_FormworksAndLumber["Lumber [2”x4”x 10']"].ToString();
            price_8_9.Text = parameters.price_FormworksAndLumber["Lumber [2”x4”x 12']"].ToString();
            price_8_10.Text = parameters.price_FormworksAndLumber["PLYWOOD 1/2” [1.22m x 2.44m]"].ToString();
            price_8_11.Text = parameters.price_FormworksAndLumber["PLYWOOD 3/4” [1.22m x 2.44m]"].ToString();
            price_8_12.Text = parameters.price_FormworksAndLumber["PLYWOOD 1/4” [1.22m x 2.44m]"].ToString();
            price_8_13.Text = parameters.price_FormworksAndLumber["PLYWOOD 1/8”[1.22m x 2.44m]"].ToString();
            price_8_14.Text = parameters.price_FormworksAndLumber["ECOBOARD 1/2”[1.22m x 2.44m]"].ToString();
            price_8_15.Text = parameters.price_FormworksAndLumber["ECOBOARD 3/4” [1.22m x 2.44m]"].ToString();
            price_8_16.Text = parameters.price_FormworksAndLumber["ECOBOARD 1/4” [1.22m x 2.44m]"].ToString();
            price_8_17.Text = parameters.price_FormworksAndLumber["ECOBOARD 1/8” [1.22m x 2.44m]"].ToString();
            price_8_18.Text = parameters.price_FormworksAndLumber["PHENOLIC BOARD- 1/2” [1.22m x 2.44m]"].ToString();
            price_8_19.Text = parameters.price_FormworksAndLumber["PHENOLIC BOARD- 3/4” [1.22m x 2.44m]"].ToString();

            //9 - ROOF MATERIALS
            price_9_1.Text = parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 0.7mm thick) [6 m]"].ToString();
            price_9_2.Text = parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 0.7mm thick) [6 m]"].ToString();
            price_9_3.Text = parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 0.9mm thick) [6 m]"].ToString();
            price_9_4.Text = parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.0mm thick) [6 m]"].ToString();
            price_9_5.Text = parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.0mm thick) [6 m]"].ToString();
            price_9_6.Text = parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.0mm thick) [6 m]"].ToString();
            price_9_7.Text = parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.2mm thick) [6 m]"].ToString();
            price_9_8.Text = parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.2mm thick) [6 m]"].ToString();
            price_9_9.Text = parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.2mm thick ) [6 m]"].ToString();
            price_9_10.Text = parameters.price_RoofMaterials["Corrugated G.I Sheet, Gauge 26 (0.551mmx2.44 mm) [m2]"].ToString();
            price_9_11.Text = parameters.price_RoofMaterials["Plain G.I Sheet, Gauge 24 (4ft x 8 ft) [UNIT]"].ToString();
            price_9_12.Text = parameters.price_RoofMaterials["G.I. Roof Nails [KG]"].ToString();
            price_9_13.Text = parameters.price_RoofMaterials["G.I Rivets [KG]"].ToString();
            price_9_14.Text = parameters.price_RoofMaterials["G.I Washers [KG]"].ToString();
            price_9_15.Text = parameters.price_RoofMaterials["Umbrella Nails [KG]"].ToString();

            //10 - Tubular Steel (1 mm Thck)
            price_10_1.Text = parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 20mm x 20mm x 1.0mm thick [6m]"].ToString();
            price_10_2.Text = parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.0mm thick [6m]"].ToString();
            price_10_3.Text = parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.0mm thick [6m]"].ToString();
            price_10_4.Text = parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.0mm thick [6m]"].ToString();
            price_10_5.Text = parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.0mm thick [6m]"].ToString();

            //11 - Tubular Steel (1.2 mm Thck)
            price_11_1.Text = parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.2mm thick [6m]"].ToString();
            price_11_2.Text = parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.2mm thick [6m]"].ToString();
            price_11_3.Text = parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.2mm thick [6m]"].ToString();
            price_11_4.Text = parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.2mm thick [6m]"].ToString();
            price_11_5.Text = parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.2mm thick [6m]"].ToString();
            price_11_6.Text = parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.2mm thick [6m]"].ToString();
            price_11_7.Text = parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.2mm thick [6m]"].ToString();

            //12 - Tubular Steel (1.5 mm Thck)
            price_12_1.Text = parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.5mm thick [6m]"].ToString();
            price_12_2.Text = parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.5mm thick [6m]"].ToString();
            price_12_3.Text = parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.5mm thick [6m]"].ToString();
            price_12_4.Text = parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.5mm thick [6m]"].ToString();
            price_12_5.Text = parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.5mm thick [6m]"].ToString();
            price_12_6.Text = parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.5mm thick [6m]"].ToString();
            price_12_7.Text = parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.5mm thick [6m]"].ToString();

            //13 - Embankments
            price_13_1.Text = parameters.price_Embankment["Common Borrow [m3]"].ToString();
            price_13_2.Text = parameters.price_Embankment["Selected Borrow [m3]"].ToString();
            price_13_3.Text = parameters.price_Embankment["Mixed Sand & Gravel [m3]"].ToString();
            price_13_4.Text = parameters.price_Embankment["Rock [m3]"].ToString();

            //14 - Rebar Grade 33
            price_14_1.Text = parameters.price_RebarGrade33["Compression Coupler GRADE 33 [PC]"].ToString();
            price_14_2.Text = parameters.price_RebarGrade33["Tension Coupler GRADE 33 [PC]"].ToString();
            price_14_3.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [6m]"].ToString();
            price_14_4.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [7.5m]"].ToString();
            price_14_5.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [9m]"].ToString();
            price_14_6.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [10.5m]"].ToString();
            price_14_7.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [12m]"].ToString();
            price_14_8.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [6m]"].ToString();
            price_14_9.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [7.5m]"].ToString();
            price_14_10.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [9m]"].ToString();
            price_14_11.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [10.5m]"].ToString();
            price_14_12.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [12m]"].ToString();
            price_14_13.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [6m]"].ToString();
            price_14_14.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [7.5m]"].ToString();
            price_14_15.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [9m]"].ToString();
            price_14_16.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [10.5m]"].ToString();
            price_14_17.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [12m]"].ToString();
            price_14_18.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [6m]"].ToString();
            price_14_19.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [7.5m]"].ToString();
            price_14_20.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [9m]"].ToString();
            price_14_21.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [10.5m]"].ToString();
            price_14_22.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [12m]"].ToString();
            price_14_23.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [6m]"].ToString();
            price_14_24.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [7.5m]"].ToString();
            price_14_25.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [9m]"].ToString();
            price_14_26.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [10.5m]"].ToString();
            price_14_27.Text = parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [12m]"].ToString();

            //15 - Rebar Grade 40
            price_15_1.Text = parameters.price_RebarGrade40["Compression Coupler GRADE 40 [PC]"].ToString();
            price_15_2.Text = parameters.price_RebarGrade40["Tension Coupler GRADE 40 [PC]"].ToString();
            price_15_3.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [6m]"].ToString();
            price_15_4.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [7.5m]"].ToString();
            price_15_5.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [9m]"].ToString();
            price_15_6.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [10.5m]"].ToString();
            price_15_7.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [12m]"].ToString();
            price_15_8.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [6m]"].ToString();
            price_15_9.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [7.5m]"].ToString();
            price_15_10.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [9m]"].ToString();
            price_15_11.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [10.5m]"].ToString();
            price_15_12.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [12m]"].ToString();
            price_15_13.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [6m]"].ToString();
            price_15_14.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [7.5m]"].ToString();
            price_15_15.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [9m]"].ToString();
            price_15_16.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [10.5m]"].ToString();
            price_15_17.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [12m]"].ToString();
            price_15_18.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [6m]"].ToString();
            price_15_19.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [7.5m]"].ToString();
            price_15_20.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [9m]"].ToString();
            price_15_21.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [10.5m]"].ToString();
            price_15_22.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [12m]"].ToString();
            price_15_23.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [6m]"].ToString();
            price_15_24.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [7.5m]"].ToString();
            price_15_25.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [9m]"].ToString();
            price_15_26.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [10.5m]"].ToString();
            price_15_27.Text = parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [12m]"].ToString();

            //16 - Rebar Grade 60
            price_16_1.Text = parameters.price_RebarGrade60["Compression Coupler GRADE 60 [PC]"].ToString();
            price_16_2.Text = parameters.price_RebarGrade60["Tension Coupler GRADE 60 [PC]"].ToString();
            price_16_3.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [6m]"].ToString();
            price_16_4.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [7.5m]"].ToString();
            price_16_5.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [9m]"].ToString();
            price_16_6.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [10.5m]"].ToString();
            price_16_7.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [12m]"].ToString();
            price_16_8.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [6m]"].ToString();
            price_16_9.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [7.5m]"].ToString();
            price_16_10.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [9m]"].ToString();
            price_16_11.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [10.5m]"].ToString();
            price_16_12.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [12m]"].ToString();
            price_16_13.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [6m]"].ToString();
            price_16_14.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [7.5m]"].ToString();
            price_16_15.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [9m]"].ToString();
            price_16_16.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [10.5m]"].ToString();
            price_16_17.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [12m]"].ToString();
            price_16_18.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [6m]"].ToString();
            price_16_19.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [7.5m]"].ToString();
            price_16_20.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [9m]"].ToString();
            price_16_21.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [10.5m]"].ToString();
            price_16_22.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [12m]"].ToString();
            price_16_23.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [6m]"].ToString();
            price_16_24.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [7.5m]"].ToString();
            price_16_25.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [9m]"].ToString();
            price_16_26.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [10.5m]"].ToString();
            price_16_27.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [12m]"].ToString();
            price_16_28.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [6m]"].ToString();
            price_16_29.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [7.5m]"].ToString();
            price_16_30.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [9m]"].ToString();
            price_16_31.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [10.5m]"].ToString();
            price_16_32.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [12m]"].ToString();
            price_16_33.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [6m]"].ToString();
            price_16_34.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [7.5m]"].ToString();
            price_16_35.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [9m]"].ToString();
            price_16_36.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [10.5m]"].ToString();
            price_16_37.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [12m]"].ToString();
            price_16_38.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [6m]"].ToString();
            price_16_39.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [7.5m]"].ToString();
            price_16_40.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [9m]"].ToString();
            price_16_41.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [10.5m]"].ToString();
            price_16_42.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [12m]"].ToString();
            price_16_43.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [6m]"].ToString();
            price_16_44.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [7.5m]"].ToString();
            price_16_45.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [9m]"].ToString();
            price_16_46.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [10.5m]"].ToString();
            price_16_47.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [12m]"].ToString();
            price_16_48.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [6m]"].ToString();
            price_16_49.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [7.5m]"].ToString();
            price_16_50.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [9m]"].ToString();
            price_16_51.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [10.5m]"].ToString();
            price_16_52.Text = parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [12m]"].ToString();

            //17 - LABOR RATE EARTHWORKS
            price_17_1.Text = parameters.price_LaborRate_Earthworks["Excavation [m3]"].ToString();
            price_17_2.Text = parameters.price_LaborRate_Earthworks["Backfilling and Compaction [m3]"].ToString();
            price_17_3.Text = parameters.price_LaborRate_Earthworks["Grading and Compaction [m3]"].ToString();
            price_17_4.Text = parameters.price_LaborRate_Earthworks["Gravel Bedding and Compaction [m3]"].ToString();
            price_17_5.Text = parameters.price_LaborRate_Earthworks["Soil Poisoning [m2]"].ToString();

            //18 - LABOR RATE CONCRETE
            price_18_1.Text = parameters.price_LaborRate_Concreting["FOOTING [m3]"].ToString();
            price_18_2.Text = parameters.price_LaborRate_Concreting["WALL FOOTING [m3]"].ToString();
            price_18_3.Text = parameters.price_LaborRate_Concreting["COLUMN [m3]"].ToString();
            price_18_4.Text = parameters.price_LaborRate_Concreting["STAIRS [m3]"].ToString();
            price_18_5.Text = parameters.price_LaborRate_Concreting["BEAM [m3]"].ToString();
            price_18_6.Text = parameters.price_LaborRate_Concreting["SUSPENDED SLAB [m3]"].ToString();
            price_18_7.Text = parameters.price_LaborRate_Concreting["SLAB ON GRADE [m3]"].ToString();

            //19 - LABOR RATE FORMWORKS
            price_19_1.Text = parameters.price_LaborRate_Formworks["FOOTING [m2]"].ToString();
            price_19_2.Text = parameters.price_LaborRate_Formworks["WALL FOOTING [m2]"].ToString();
            price_19_3.Text = parameters.price_LaborRate_Formworks["COLUMN [m2]"].ToString();
            price_19_4.Text = parameters.price_LaborRate_Formworks["STAIRS [m2]"].ToString();
            price_19_5.Text = parameters.price_LaborRate_Formworks["BEAM [m2]"].ToString();
            price_19_6.Text = parameters.price_LaborRate_Formworks["SUSPENDED SLAB [m2]"].ToString();

            //20 - LABOR RATE REBAR
            price_20_1.Text = parameters.price_LaborRate_Rebar["FOOTING [KG]"].ToString();
            price_20_2.Text = parameters.price_LaborRate_Rebar["WALL FOOTING [KG]"].ToString();
            price_20_3.Text = parameters.price_LaborRate_Rebar["COLUMN [KG]"].ToString();
            price_20_4.Text = parameters.price_LaborRate_Rebar["STAIRS [KG]"].ToString();
            price_20_5.Text = parameters.price_LaborRate_Rebar["BEAM [KG]"].ToString();
            price_20_6.Text = parameters.price_LaborRate_Rebar["FOOTING TIE BEAM [KG]"].ToString();
            price_20_7.Text = parameters.price_LaborRate_Rebar["SLAB ON GRADE [KG]"].ToString();
            price_20_8.Text = parameters.price_LaborRate_Rebar["SUSPENDED SLAB [KG]"].ToString();
            price_20_9.Text = parameters.price_LaborRate_Rebar["WALLS [KG]"].ToString();

            //21 - LABOR RATE PAINT
            price_21_1.Text = parameters.price_LaborRate_Paint["PAINTER [m2]"].ToString();

            //22 - LABOR RATE TILES
            price_22_1.Text = parameters.price_LaborRate_Tiles["TILES [m2]"].ToString();

            //23 - LABOR RATE MASONRY
            price_23_1.Text = parameters.price_LaborRate_Masonry["MASONRY [m2]"].ToString();

            //24 - LABOR RATE ROOFINGS
            price_24_1.Text = parameters.price_LaborRate_Roofings["ROOFINGS [m2]"].ToString();

            //25.1 - Manpower - Manila
            price_251_1.Text = parameters.price_ManpowerM["Foreman [hr]"].ToString();
            price_251_2.Text = parameters.price_ManpowerM["Fitter [hr]"].ToString();
            price_251_3.Text = parameters.price_ManpowerM["Welder [hr]"].ToString();
            price_251_4.Text = parameters.price_ManpowerM["Electrician [hr]"].ToString();
            price_251_5.Text = parameters.price_ManpowerM["Carpenter [hr]"].ToString();
            price_251_6.Text = parameters.price_ManpowerM["Painter [hr]"].ToString();
            price_251_7.Text = parameters.price_ManpowerM["Mason [hr]"].ToString();
            price_251_8.Text = parameters.price_ManpowerM["Driver [hr]"].ToString();
            price_251_9.Text = parameters.price_ManpowerM["Eqpt Operator [hr]"].ToString();
            price_251_10.Text = parameters.price_ManpowerM["Helper [hr]"].ToString();

            //25.2 - Manpower - Provincial
            price_252_1.Text = parameters.price_ManpowerP["Foreman [hr]"].ToString();
            price_252_2.Text = parameters.price_ManpowerP["Fitter [hr]"].ToString();
            price_252_3.Text = parameters.price_ManpowerP["Welder [hr]"].ToString();
            price_252_4.Text = parameters.price_ManpowerP["Electrician [hr]"].ToString();
            price_252_5.Text = parameters.price_ManpowerP["Carpenter [hr]"].ToString();
            price_252_6.Text = parameters.price_ManpowerP["Painter [hr]"].ToString();
            price_252_7.Text = parameters.price_ManpowerP["Mason [hr]"].ToString();
            price_252_8.Text = parameters.price_ManpowerP["Driver [hr]"].ToString();
            price_252_9.Text = parameters.price_ManpowerP["Eqpt Operator [hr]"].ToString();
            price_252_10.Text = parameters.price_ManpowerP["Helper [hr]"].ToString();

            //26 - EQUIPMENT (PER HR)
            price_26_1.Text = parameters.price_Equipment["Crawler Loader (80kW/ 1.5 - 2.0 cu.m) [hr]"].ToString();
            price_26_2.Text = parameters.price_Equipment["Crawler Dozer (125kW) [hr]"].ToString();
            price_26_3.Text = parameters.price_Equipment["Wheel Loader (20 - 3.0 Cum) [hr]"].ToString();
            price_26_4.Text = parameters.price_Equipment["Backhoe Crawler (0.75- 1.0 cum) [hr]"].ToString();
            price_26_5.Text = parameters.price_Equipment["Backhoe / Pavement Breaker (1.5 cum) [hr]"].ToString();
            price_26_6.Text = parameters.price_Equipment["Motor Grader (90 - 100 kW)  [hr]"].ToString();
            price_26_7.Text = parameters.price_Equipment["Pneumatic Tire Roller (20 - 24 Mr) [hr]"].ToString();
            price_26_8.Text = parameters.price_Equipment["Vibratory Drum Roller (10 - 14MT) [hr]"].ToString();
            price_26_9.Text = parameters.price_Equipment["Dump Truck (8.0 -12.0 cu.m) [hr]"].ToString();
            price_26_10.Text = parameters.price_Equipment["Cargo Truck Small (5 - 8 MT) [hr]"].ToString();
            price_26_11.Text = parameters.price_Equipment["Cargo Truck Small (10 -15 MT) [hr]"].ToString();
            price_26_12.Text = parameters.price_Equipment["Transit Mixer (5.0 -8.0 cum) [hr]"].ToString();
            price_26_13.Text = parameters.price_Equipment["Concrete Batching Plant (80 -100 cu.m/hr) [hr]"].ToString();
            price_26_14.Text = parameters.price_Equipment["Concrete Trimmer/Slipform Paver (1 meter width) [hr]"].ToString();
            price_26_15.Text = parameters.price_Equipment["Asphalt Distributor Truck (2500 - 3500 Gallons) [hr]"].ToString();
            price_26_16.Text = parameters.price_Equipment["Asphalt Finisher (3-meter width) [hr]"].ToString();
            price_26_17.Text = parameters.price_Equipment["Truck with Boom, small (6-10 MT) [hr]"].ToString();
            price_26_18.Text = parameters.price_Equipment["Truck with Boom, small (12 - 15 MT) [hr]"].ToString();
            price_26_19.Text = parameters.price_Equipment["Crawler Crane (21-25 MT) [hr]"].ToString();
            price_26_20.Text = parameters.price_Equipment["Diesel Pile Hammer [hr]"].ToString();
            price_26_21.Text = parameters.price_Equipment["Vibratory Pile Driver [hr]"].ToString();
            price_26_22.Text = parameters.price_Equipment["Bagger Mixer (1-2 Bags) [hr]"].ToString();
            price_26_23.Text = parameters.price_Equipment["Concrete Vibrator [hr]"].ToString();
            price_26_24.Text = parameters.price_Equipment["Air Compressor  (Small) [hr]"].ToString();
            price_26_25.Text = parameters.price_Equipment["Air Compressor (Big) [hr]"].ToString();
            price_26_26.Text = parameters.price_Equipment["Bar Cutter [hr]"].ToString();
            price_26_27.Text = parameters.price_Equipment["Bar Bender [hr]"].ToString();
            price_26_28.Text = parameters.price_Equipment["Jack Hammer [hr]"].ToString();
            price_26_29.Text = parameters.price_Equipment["Tamping Rammer [hr]"].ToString();
            price_26_30.Text = parameters.price_Equipment["Welding Machine, Portable, 300A [hr]"].ToString();
            price_26_31.Text = parameters.price_Equipment["Welding Machine, 600A [hr]"].ToString();
            price_26_32.Text = parameters.price_Equipment["Generator Set 15-25kVA [hr]"].ToString();
            price_26_33.Text = parameters.price_Equipment["Generator Set 50 kVA [hr]"].ToString();
            price_26_34.Text = parameters.price_Equipment["Sump Pump (Dewatering) 0.75  –2HP- [hr]"].ToString();
            price_26_35.Text = parameters.price_Equipment["Sump Pump (Dewatering) 5HP [hr]"].ToString();
            price_26_36.Text = parameters.price_Equipment["Road Paint Stripper [hr]"].ToString();
        }

        private void foot_FT_cbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            priceTabControl.SelectedIndex = price_Category_cbx.SelectedIndex;
        }

        private void price_SaveBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to save everything?", "Save Price List", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //1 - Common Items
                parameters.price_CommonMaterials["Cyclone Wire (Gauge#10, 2”x2”, 3ft x 10m) [ROLL]"] = price_1_1.Text;
                parameters.price_CommonMaterials["Gasket (6mm thk, 1m x 1m) [ROLL]"] = price_1_2.Text;
                parameters.price_CommonMaterials["Acetylene Gas [CYL]"] = price_1_3.Text;
                parameters.price_CommonMaterials["Oxygen Gas [CYL]"] = price_1_4.Text;
                parameters.price_CommonMaterials["Rugby [CANS]"] = price_1_5.Text;
                parameters.price_CommonMaterials["Vulca Seal [LTR]"] = price_1_6.Text;
                parameters.price_CommonMaterials["Broom, Soft [PC]"] = price_1_7.Text;
                parameters.price_CommonMaterials["Concrete Epoxy [SET]"] = price_1_8.Text;
                parameters.price_CommonMaterials["Concrete Patching Compound [KGS]"] = price_1_9.Text;
                parameters.price_CommonMaterials["GI Wire (no. 16) [ROLL]"] = price_1_10.Text;
                parameters.price_CommonMaterials["GI Wire (no. 12) [KG]"] = price_1_11.Text;
                parameters.price_CommonMaterials["GI Wire (no. 16) [KG]"] = price_1_12.Text;
                parameters.price_CommonMaterials["Non-Shrink Grout [BAGS]"] = price_1_13.Text;
                parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"] = price_1_14.Text;
                parameters.price_CommonMaterials["Sand [m3]"] = price_1_15.Text;
                parameters.price_CommonMaterials["Rivets 1/8” x ½” [BOX]"] = price_1_16.Text;
                parameters.price_CommonMaterials["Rivets 1-1/2” x ½” [BOX]"] = price_1_17.Text;
                parameters.price_CommonMaterials["Rope (⌀ 1/2”) [MTRS]"] = price_1_18.Text;
                parameters.price_CommonMaterials["Tape, Caution [ROLLS]"] = price_1_19.Text;
                parameters.price_CommonMaterials["Tile Grout (2KG)  [BAGS]"] = price_1_20.Text;
                parameters.price_CommonMaterials["Tiles, Floor (600 x 600) [PC]"] = price_1_21.Text;
                parameters.price_CommonMaterials["Tiles, Wall (300 x 300) [PC]"] = price_1_22.Text;
                parameters.price_CommonMaterials["Broom Stick [PC]"] = price_1_23.Text;
                parameters.price_CommonMaterials["Chalk Stone [PC]"] = price_1_24.Text;
                parameters.price_CommonMaterials["Sandpaper (#100) [MTRS]"] = price_1_25.Text;
                parameters.price_CommonMaterials["Sandpaper (#50) [MTRS]"] = price_1_26.Text;
                parameters.price_CommonMaterials["Common Nail [KG]"] = price_1_27.Text;
                parameters.price_CommonMaterials["Concrete Nail [KG]"] = price_1_28.Text;
                parameters.price_CommonMaterials["Putty, Multipurpose [PAIL]"] = price_1_29.Text;
                parameters.price_CommonMaterials["Tie Wire (No. #16) [25kg/Roll]"] = price_1_30.Text;
                parameters.price_CommonMaterials["25KG Tile Adhesive (Regular) [BAGS]"] = price_1_31.Text;
                parameters.price_CommonMaterials["25 KG Tile Adhesive (Heavy duty) [BAGS]"] = price_1_32.Text;
                parameters.price_CommonMaterials["CHB 4” (0.10 x 0.20 x 0.40) [PC]"] = price_1_33.Text;
                parameters.price_CommonMaterials["CHB 6” (0.15 x 0.20 x 0.40) [PC]"] = price_1_34.Text;
                parameters.price_CommonMaterials["CHB 8” (0.20 x 0.20 x 0.40) [PC]"] = price_1_35.Text;

                //2 - Paint and Coating
                parameters.price_PaintAndCoating["Acrylic Emulsion [GALS]"] = price_2_1.Text;
                parameters.price_PaintAndCoating["Concrete Epoxy Injection [GALS]"] = price_2_2.Text;
                parameters.price_PaintAndCoating["Concrete Primer & Sealer [PAIL]"] = price_2_3.Text;
                parameters.price_PaintAndCoating["Epopatch, Base and Hardener [SETS]"] = price_2_4.Text;
                parameters.price_PaintAndCoating["Lacquer Thinner [GALS]"] = price_2_5.Text;
                parameters.price_PaintAndCoating["Paint Brush, Bamboo 1-1/2” [PC]"] = price_2_6.Text;
                parameters.price_PaintAndCoating["Paint, Acrylic 1 [GAL]"] = price_2_7.Text;
                parameters.price_PaintAndCoating["Paint, Epoxy Enamel White [GAL]"] = price_2_8.Text;
                parameters.price_PaintAndCoating["Paint, Epoxy Floor Coating [GALS]"] = price_2_9.Text;
                parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"] = price_2_10.Text;
                parameters.price_PaintAndCoating["Paint, Epoxy Reducer [GALS]"] = price_2_11.Text;
                parameters.price_PaintAndCoating["Paint Latex Gloss [GAL]"] = price_2_12.Text;
                parameters.price_PaintAndCoating["Paint Enamel [GAL]"] = price_2_13.Text;
                parameters.price_PaintAndCoating["Paint, Semi-Gloss [GALS]"] = price_2_14.Text;
                parameters.price_PaintAndCoating["Putty, Masonry [PAIL]"] = price_2_15.Text;
                parameters.price_PaintAndCoating["Rust Converter [GAL]"] = price_2_16.Text;
                parameters.price_PaintAndCoating["Skim Coat [BAGS]"] = price_2_17.Text;
                parameters.price_PaintAndCoating["Underwater Epoxy [GALS]"] = price_2_18.Text;
                parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"] = price_2_19.Text;

                //3 - Welding rod
                parameters.price_WeldingRod["Stainless Welding Rod 308 (3.2mm) [KGS]"] = price_3_1.Text;
                parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [KGS]"] = price_3_2.Text;
                parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [BOX]"] = price_3_3.Text;
                parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [KGS]"] = price_3_4.Text;
                parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [BOX]"] = price_3_5.Text;

                //4 - Personal protective equipment
                parameters.price_PersonalProtectiveEquipment["Chemical Gloves PAIR Cotton Gloves [PAIRS]"] = price_4_1.Text;
                parameters.price_PersonalProtectiveEquipment["Cotton Gloves [PAIRS]"] = price_4_2.Text;
                parameters.price_PersonalProtectiveEquipment["Dust Mask N95 [PC]"] = price_4_3.Text;
                parameters.price_PersonalProtectiveEquipment["Gloves, Orange Palm [PAIRS]"] = price_4_4.Text;
                parameters.price_PersonalProtectiveEquipment["Hard Hat w/ Headgear and chin strap [20 SET]"] = price_4_5.Text;
                parameters.price_PersonalProtectiveEquipment["Overall with reflector [PC]"] = price_4_6.Text;
                parameters.price_PersonalProtectiveEquipment["Oxy-Acetylene Cutting Outfit [SET]"] = price_4_7.Text;
                parameters.price_PersonalProtectiveEquipment["PVC Apron  [PC]"] = price_4_8.Text;
                parameters.price_PersonalProtectiveEquipment["Respirator mask w/ Cartridge [PC]"] = price_4_9.Text;
                parameters.price_PersonalProtectiveEquipment["Respirator, Filter Cartridge [PACK]"] = price_4_10.Text;
                parameters.price_PersonalProtectiveEquipment["Safety Goggles [PC]"] = price_4_11.Text;
                parameters.price_PersonalProtectiveEquipment["Safety Rubber boots [PAIR]"] = price_4_12.Text;
                parameters.price_PersonalProtectiveEquipment["Safety Shoes  [PAIR]"] = price_4_13.Text;
                parameters.price_PersonalProtectiveEquipment["Safety Vest [PC]"] = price_4_14.Text;
                parameters.price_PersonalProtectiveEquipment["Welding Mask  [PC]"] = price_4_15.Text;
                parameters.price_PersonalProtectiveEquipment["Welding Apron [PC]"] = price_4_16.Text;
                parameters.price_PersonalProtectiveEquipment["Welding Mask, Auto Darkening [SETS]"] = price_4_17.Text;

                //5 - Tools 
                parameters.price_Tools["Adjustable Wrench Set 4”— 24” [SET]"] = price_5_1.Text;
                parameters.price_Tools["Baby Roller (Cotton) 4” [PC]"] = price_5_2.Text;
                parameters.price_Tools["Ball Hammer [PC]"] = price_5_3.Text;
                parameters.price_Tools["Bench Vise [UNIT]"] = price_5_4.Text;
                parameters.price_Tools["Blade Cutter [PC]"] = price_5_5.Text;
                parameters.price_Tools["Camlock (Male & Female Set) 50mm DIA [SET]"] = price_5_6.Text;
                parameters.price_Tools["Chipping Gun [UNIT]"] = price_5_7.Text;
                parameters.price_Tools["Combination Wrench Set 6mm — 32mm [SET]"] = price_5_8.Text;
                parameters.price_Tools["Cut-off Wheel ⌀ 16” [BOX]"] = price_5_9.Text;
                parameters.price_Tools["Cutting Disc ⌀ 4” [BOX]"] = price_5_10.Text;
                parameters.price_Tools["Cutting Disc ⌀ 7” [BOX]"] = price_5_11.Text;
                parameters.price_Tools["Drill Bit [BOX]"] = price_5_12.Text;
                parameters.price_Tools["Electrical Plier [PC]"] = price_5_13.Text;
                parameters.price_Tools["Grinder, Angle 4” [UNIT]"] = price_5_14.Text;
                parameters.price_Tools["Grinder, Angle 7” [UNIT]"] = price_5_15.Text;
                parameters.price_Tools["Grinder, Baby [UNIT]"] = price_5_16.Text;
                parameters.price_Tools["Grinder, Mother [UNIT]"] = price_5_17.Text;
                parameters.price_Tools["Grinding Disc ⌀ 4” [BOX]"] = price_5_18.Text;
                parameters.price_Tools["Grinding Disc ⌀ 7” [BOX]"] = price_5_19.Text;
                parameters.price_Tools["Heat Gun [UNIT]"] = price_5_20.Text;
                parameters.price_Tools["Ladder (A-Type), 6h, Aluminum [PC]"] = price_5_21.Text;
                parameters.price_Tools["Level Bar 24” [PC]"] = price_5_22.Text;
                parameters.price_Tools["Paint Brush 4” [PC]"] = price_5_23.Text;
                parameters.price_Tools["Paint Brush 2” [PC]"] = price_5_24.Text;
                parameters.price_Tools["Portable Axial Fan Blower  ⌀  8” [SET]"] = price_5_25.Text;
                parameters.price_Tools["Power Ratchet [UNIT]"] = price_5_26.Text;
                parameters.price_Tools["Rivet Gun / Riveter [UNIT]"] = price_5_27.Text;
                parameters.price_Tools["Roller Brush 7” [PC]"] = price_5_28.Text;
                parameters.price_Tools["Screwdriver, Flat [SET]"] = price_5_29.Text;
                parameters.price_Tools["Screwdriver, Philip [SET]"] = price_5_30.Text;
                parameters.price_Tools["Shovel, Pointed [PC]"] = price_5_31.Text;
                parameters.price_Tools["Snap Ring Plier [SET]"] = price_5_32.Text;
                parameters.price_Tools["Socket Wrench Set 19mm — 50mm [SET]"] = price_5_33.Text;
                parameters.price_Tools["Speed Cutter [UNIT]"] = price_5_34.Text;
                parameters.price_Tools["Steel Brush [PC]"] = price_5_35.Text;
                parameters.price_Tools["Test Light [PC]"] = price_5_36.Text;
                parameters.price_Tools["Test Wrench [UNIT]"] = price_5_37.Text;
                parameters.price_Tools["Torque Wrench [UNIT]"] = price_5_38.Text;
                parameters.price_Tools["Vise Grip [PC]"] = price_5_39.Text;
                parameters.price_Tools["Welding Machine (Portable) 12.3 kVA(20-300A) [UNIT]"] = price_5_40.Text;

                //6 - Ready Mix Concrete
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days [m3]"] = price_6_1.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 14 Days [m3]"] = price_6_2.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 7 Days [m3]"] = price_6_3.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 3 Days [m3]"] = price_6_4.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 28 Days [m3]"] = price_6_5.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 14 Days [m3]"] = price_6_6.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 7 Days [m3]"] = price_6_7.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 3 Days [m3]"] = price_6_8.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 28 Days [m3]"] = price_6_9.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 14 Days [m3]"] = price_6_10.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4500PSI (31 Mpa) @ 28 Days [m3]"] = price_6_11.Text;
                parameters.price_ReadyMixConcrete["Ready Mix Concrete, 5000PSI (34.5 Mpa) @ 28 Days [m3]"] = price_6_12.Text;

                //7 - Gravel
                parameters.price_Gravel["GRAVEL G1 [m3]"] = price_7_1.Text;
                parameters.price_Gravel["GRAVEL G2 [m3]"] = price_7_2.Text;
                parameters.price_Gravel["GRAVEL G1- ½” [m3]"] = price_7_3.Text;
                parameters.price_Gravel["GRAVEL G2- ½” [m3]"] = price_7_4.Text;
                parameters.price_Gravel["GRAVEL ¾” [m3]"] = price_7_5.Text;

                //8 - Formworks and Lumber
                parameters.price_FormworksAndLumber["Lumber [2”x 2” x 8']"] = price_8_1.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x 2” x 10']"] = price_8_2.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x 2” x 12']"] = price_8_3.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x 3”x 8']"] = price_8_4.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x 3”x 10']"] = price_8_5.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x 3”x 12']"] = price_8_6.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x4”x 8']"] = price_8_7.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x4”x 10']"] = price_8_8.Text;
                parameters.price_FormworksAndLumber["Lumber [2”x4”x 12']"] = price_8_9.Text;
                parameters.price_FormworksAndLumber["PLYWOOD 1/2” [1.22m x 2.44m]"] = price_8_10.Text;
                parameters.price_FormworksAndLumber["PLYWOOD 3/4” [1.22m x 2.44m]"] = price_8_11.Text;
                parameters.price_FormworksAndLumber["PLYWOOD 1/4” [1.22m x 2.44m]"] = price_8_12.Text;
                parameters.price_FormworksAndLumber["PLYWOOD 1/8”[1.22m x 2.44m]"] = price_8_13.Text;
                parameters.price_FormworksAndLumber["ECOBOARD 1/2”[1.22m x 2.44m]"] = price_8_14.Text;
                parameters.price_FormworksAndLumber["ECOBOARD 3/4” [1.22m x 2.44m]"] = price_8_15.Text;
                parameters.price_FormworksAndLumber["ECOBOARD 1/4” [1.22m x 2.44m]"] = price_8_16.Text;
                parameters.price_FormworksAndLumber["ECOBOARD 1/8” [1.22m x 2.44m]"] = price_8_17.Text;
                parameters.price_FormworksAndLumber["PHENOLIC BOARD- 1/2” [1.22m x 2.44m]"] = price_8_18.Text;
                parameters.price_FormworksAndLumber["PHENOLIC BOARD- 3/4” [1.22m x 2.44m]"] = price_8_19.Text;

                //9 - Roof materials
                parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 0.7mm thick) [6 m]"] = price_9_1.Text;
                parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 0.7mm thick) [6 m]"] = price_9_2.Text;
                parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 0.9mm thick) [6 m]"] = price_9_3.Text;
                parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.0mm thick) [6 m]"] = price_9_4.Text;
                parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.0mm thick) [6 m]"] = price_9_5.Text;
                parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.0mm thick) [6 m]"] = price_9_6.Text;
                parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.2mm thick) [6 m]"] = price_9_7.Text;
                parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.2mm thick) [6 m]"] = price_9_8.Text;
                parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.2mm thick ) [6 m]"] = price_9_9.Text;
                parameters.price_RoofMaterials["Corrugated G.I Sheet, Gauge 26 (0.551mmx2.44 mm) [m2]"] = price_9_10.Text;
                parameters.price_RoofMaterials["Plain G.I Sheet, Gauge 24 (4ft x 8 ft) [UNIT]"] = price_9_11.Text;
                parameters.price_RoofMaterials["G.I. Roof Nails [KG]"] = price_9_12.Text;
                parameters.price_RoofMaterials["G.I Rivets [KG]"] = price_9_13.Text;
                parameters.price_RoofMaterials["G.I Washers [KG]"] = price_9_14.Text;
                parameters.price_RoofMaterials["Umbrella Nails [KG]"] = price_9_15.Text;

                //10 - Tubular steel (1 mm Thck)
                parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 20mm x 20mm x 1.0mm thick [6m]"] = price_10_1.Text;
                parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.0mm thick [6m]"] = price_10_2.Text;
                parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.0mm thick [6m]"] = price_10_3.Text;
                parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.0mm thick [6m]"] = price_10_4.Text;
                parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.0mm thick [6m]"] = price_10_5.Text;

                //11 - Tubular steel (1.2 mm Thck)
                parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.2mm thick [6m]"] = price_11_1.Text;
                parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.2mm thick [6m]"] = price_11_2.Text;
                parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.2mm thick [6m]"] = price_11_3.Text;
                parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.2mm thick [6m]"] = price_11_4.Text;
                parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.2mm thick [6m]"] = price_11_5.Text;
                parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.2mm thick [6m]"] = price_11_6.Text;
                parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.2mm thick [6m]"] = price_11_7.Text;

                //12 - Tubular steel (1.5 mm Thck)
                parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.5mm thick [6m]"] = price_12_1.Text;
                parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.5mm thick [6m]"] = price_12_2.Text;
                parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.5mm thick [6m]"] = price_12_3.Text;
                parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.5mm thick [6m]"] = price_12_4.Text;
                parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.5mm thick [6m]"] = price_12_5.Text;
                parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.5mm thick [6m]"] = price_12_6.Text;
                parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.5mm thick [6m]"] = price_12_7.Text;


                //13 - Embankment
                parameters.price_Embankment["Common Borrow [m3]"] = price_13_1.Text;
                parameters.price_Embankment["Selected Borrow [m3]"] = price_13_2.Text;
                parameters.price_Embankment["Mixed Sand & Gravel [m3]"] = price_13_3.Text;
                parameters.price_Embankment["Rock [m3]"] = price_13_4.Text;

                //14 - Rebar Grade 33
                parameters.price_RebarGrade33["Compression Coupler GRADE 33 [PC]"] = price_14_1.Text;
                parameters.price_RebarGrade33["Tension Coupler GRADE 33 [PC]"] = price_14_2.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [6m]"] = price_14_3.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [7.5m]"] = price_14_4.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [9m]"] = price_14_5.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [10.5m]"] = price_14_6.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [12m]"] = price_14_7.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [6m]"] = price_14_8.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [7.5m]"] = price_14_9.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [9m]"] = price_14_10.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [10.5m]"] = price_14_11.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [12m]"] = price_14_12.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [6m]"] = price_14_13.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [7.5m]"] = price_14_14.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [9m]"] = price_14_15.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [10.5m]"] = price_14_16.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [12m]"] = price_14_17.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [6m]"] = price_14_18.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [7.5m]"] = price_14_19.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [9m]"] = price_14_20.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [10.5m]"] = price_14_21.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [12m]"] = price_14_22.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [6m]"] = price_14_23.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [7.5m]"] = price_14_24.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [9m]"] = price_14_25.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [10.5m]"] = price_14_26.Text;
                parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [12m]"] = price_14_27.Text;


                //15 - Rebar Grade 40 
                parameters.price_RebarGrade40["Compression Coupler GRADE 40 [PC]"] = price_15_1.Text;
                parameters.price_RebarGrade40["Tension Coupler GRADE 40 [PC]"] = price_15_2.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [6m]"] = price_15_3.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [7.5m]"] = price_15_4.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [9m]"] = price_15_5.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [10.5m]"] = price_15_6.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [12m]"] = price_15_7.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [6m]"] = price_15_8.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [7.5m]"] = price_15_9.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [9m]"] = price_15_10.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [10.5m]"] = price_15_11.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [12m]"] = price_15_12.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [6m]"] = price_15_13.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [7.5m]"] = price_15_14.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [9m]"] = price_15_15.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [10.5m]"] = price_15_16.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [12m]"] = price_15_17.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [6m]"] = price_15_18.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [7.5m]"] = price_15_19.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [9m]"] = price_15_20.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [10.5m]"] = price_15_21.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [12m]"] = price_15_22.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [6m]"] = price_15_23.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [7.5m]"] = price_15_24.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [9m]"] = price_15_25.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [10.5m]"] = price_15_26.Text;
                parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [12m]"] = price_15_27.Text;

                //16 - Rebar Grade 60 (415 Mpa)
                parameters.price_RebarGrade60["Compression Coupler GRADE 60 [PC]"] = price_16_1.Text;
                parameters.price_RebarGrade60["Tension Coupler GRADE 60 [PC]"] = price_16_2.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [6m]"] = price_16_3.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [7.5m]"] = price_16_4.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [9m]"] = price_16_5.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [10.5m]"] = price_16_6.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [12m]"] = price_16_7.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [6m]"] = price_16_8.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [7.5m]"] = price_16_9.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [9m]"] = price_16_10.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [10.5m]"] = price_16_11.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [12m]"] = price_16_12.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [6m]"] = price_16_13.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [7.5m]"] = price_16_14.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [9m]"] = price_16_15.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [10.5m]"] = price_16_16.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [12m]"] = price_16_17.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [6m]"] = price_16_18.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [7.5m]"] = price_16_19.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [9m]"] = price_16_20.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [10.5m]"] = price_16_21.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [12m]"] = price_16_22.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [6m]"] = price_16_23.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [7.5m]"] = price_16_24.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [9m]"] = price_16_25.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [10.5m]"] = price_16_26.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [12m]"] = price_16_27.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [6m]"] = price_16_28.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [7.5m]"] = price_16_29.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [9m]"] = price_16_30.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [10.5m]"] = price_16_31.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [12m]"] = price_16_32.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [6m]"] = price_16_33.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [7.5m]"] = price_16_34.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [9m]"] = price_16_35.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [10.5m]"] = price_16_36.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [12m]"] = price_16_37.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [6m]"] = price_16_38.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [7.5m]"] = price_16_39.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [9m]"] = price_16_40.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [10.5m]"] = price_16_41.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [12m]"] = price_16_42.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [6m]"] = price_16_43.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [7.5m]"] = price_16_44.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [9m]"] = price_16_45.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [10.5m]"] = price_16_46.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [12m]"] = price_16_47.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [6m]"] = price_16_48.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [7.5m]"] = price_16_49.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [9m]"] = price_16_50.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [10.5m]"] = price_16_51.Text;
                parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [12m]"] = price_16_52.Text;


                //17 - Labor Rate - Earthworks
                parameters.price_LaborRate_Earthworks["Excavation [m3]"] = price_17_1.Text;
                parameters.price_LaborRate_Earthworks["Backfilling and Compaction [m3]"] = price_17_2.Text;
                parameters.price_LaborRate_Earthworks["Grading and Compaction [m3]"] = price_17_3.Text;
                parameters.price_LaborRate_Earthworks["Gravel Bedding and Compaction [m3]"] = price_17_4.Text;
                parameters.price_LaborRate_Earthworks["Soil Poisoning [m2]"] = price_17_5.Text;

                //18 - LABOR RATE CONCRETE
                parameters.price_LaborRate_Concreting["FOOTING [m3]"] = price_18_1.Text;
                parameters.price_LaborRate_Concreting["WALL FOOTING [m3]"] = price_18_2.Text;
                parameters.price_LaborRate_Concreting["COLUMN [m3]"] = price_18_3.Text;
                parameters.price_LaborRate_Concreting["STAIRS [m3]"] = price_18_4.Text;
                parameters.price_LaborRate_Concreting["BEAM [m3]"] = price_18_5.Text;
                parameters.price_LaborRate_Concreting["SUSPENDED SLAB [m3]"] = price_18_6.Text;
                parameters.price_LaborRate_Concreting["SLAB ON GRADE [m3]"] = price_18_7.Text;

                //19 - LABOR RATE FORMWORKS
                parameters.price_LaborRate_Formworks["FOOTING [m2]"] = price_19_1.Text;
                parameters.price_LaborRate_Formworks["WALL FOOTING [m2]"] = price_19_2.Text;
                parameters.price_LaborRate_Formworks["COLUMN [m2]"] = price_19_3.Text;
                parameters.price_LaborRate_Formworks["STAIRS [m2]"] = price_19_4.Text;
                parameters.price_LaborRate_Formworks["BEAM [m2]"] = price_19_5.Text;
                parameters.price_LaborRate_Formworks["SUSPENDED SLAB [m2]"] = price_19_6.Text;

                //20 - LABOR RATE REBAR
                parameters.price_LaborRate_Rebar["FOOTING [KG]"] = price_20_1.Text;
                parameters.price_LaborRate_Rebar["WALL FOOTING [KG]"] = price_20_2.Text;
                parameters.price_LaborRate_Rebar["COLUMN [KG]"] = price_20_3.Text;
                parameters.price_LaborRate_Rebar["STAIRS [KG]"] = price_20_4.Text;
                parameters.price_LaborRate_Rebar["BEAM [KG]"] = price_20_5.Text;
                parameters.price_LaborRate_Rebar["FOOTING TIE BEAM [KG]"] = price_20_6.Text;
                parameters.price_LaborRate_Rebar["SLAB ON GRADE [KG]"] = price_20_7.Text;
                parameters.price_LaborRate_Rebar["SUSPENDED SLAB [KG]"] = price_20_8.Text;
                parameters.price_LaborRate_Rebar["WALLS [KG]"] = price_20_9.Text;

                //21 - LABOR RATE PAINT
                parameters.price_LaborRate_Paint["PAINTER [m2]"] = price_21_1.Text;

                //22 - LABOR RATE TILES
                parameters.price_LaborRate_Tiles["TILES [m2]"] = price_22_1.Text;

                //23 - LABOR RATE MASONRY
                parameters.price_LaborRate_Masonry["MASONRY [m2]"] = price_23_1.Text;

                //24 - LABOR RATE ROOFINGS
                parameters.price_LaborRate_Roofings["ROOFINGS [m2]"] = price_24_1.Text;

                //25.1 - Manpower - Manila
                parameters.price_ManpowerM["Foreman [hr]"] = price_251_1.Text;
                parameters.price_ManpowerM["Fitter [hr]"] = price_251_2.Text;
                parameters.price_ManpowerM["Welder [hr]"] = price_251_3.Text;
                parameters.price_ManpowerM["Electrician [hr]"] = price_251_4.Text;
                parameters.price_ManpowerM["Carpenter [hr]"] = price_251_5.Text;
                parameters.price_ManpowerM["Painter [hr]"] = price_251_6.Text;
                parameters.price_ManpowerM["Mason [hr]"] = price_251_7.Text;
                parameters.price_ManpowerM["Driver [hr]"] = price_251_8.Text;
                parameters.price_ManpowerM["Eqpt Operator [hr]"] = price_251_9.Text;
                parameters.price_ManpowerM["Helper [hr]"] = price_251_10.Text;

                //25.2 - Manpower - Provincial
                parameters.price_ManpowerP["Foreman [hr]"] = price_252_1.Text;
                parameters.price_ManpowerP["Fitter [hr]"] = price_252_2.Text;
                parameters.price_ManpowerP["Welder [hr]"] = price_252_3.Text;
                parameters.price_ManpowerP["Electrician [hr]"] = price_252_4.Text;
                parameters.price_ManpowerP["Carpenter [hr]"] = price_252_5.Text;
                parameters.price_ManpowerP["Painter [hr]"] = price_252_6.Text;
                parameters.price_ManpowerP["Mason [hr]"] = price_252_7.Text;
                parameters.price_ManpowerP["Driver [hr]"] = price_252_8.Text;
                parameters.price_ManpowerP["Eqpt Operator [hr]"] = price_252_9.Text;
                parameters.price_ManpowerP["Helper [hr]"] = price_252_10.Text;

                //26 - EQUIPMENT (PER HR)
                parameters.price_Equipment["Crawler Loader (80kW/ 1.5 - 2.0 cu.m) [hr]"] = price_26_1.Text;
                parameters.price_Equipment["Crawler Dozer (125kW) [hr]"] = price_26_2.Text;
                parameters.price_Equipment["Wheel Loader (20 - 3.0 Cum) [hr]"] = price_26_3.Text;
                parameters.price_Equipment["Backhoe Crawler (0.75- 1.0 cum) [hr]"] = price_26_4.Text;
                parameters.price_Equipment["Backhoe / Pavement Breaker (1.5 cum) [hr]"] = price_26_5.Text;
                parameters.price_Equipment["Motor Grader (90 - 100 kW)  [hr]"] = price_26_6.Text;
                parameters.price_Equipment["Pneumatic Tire Roller (20 - 24 Mr) [hr]"] = price_26_7.Text;
                parameters.price_Equipment["Vibratory Drum Roller (10 - 14MT) [hr]"] = price_26_8.Text;
                parameters.price_Equipment["Dump Truck (8.0 -12.0 cu.m) [hr]"] = price_26_9.Text;
                parameters.price_Equipment["Cargo Truck Small (5 - 8 MT) [hr]"] = price_26_10.Text;
                parameters.price_Equipment["Cargo Truck Small (10 -15 MT) [hr]"] = price_26_11.Text;
                parameters.price_Equipment["Transit Mixer (5.0 -8.0 cum) [hr]"] = price_26_12.Text;
                parameters.price_Equipment["Concrete Batching Plant (80 -100 cu.m/hr) [hr]"] = price_26_13.Text;
                parameters.price_Equipment["Concrete Trimmer/Slipform Paver (1 meter width) [hr]"] = price_26_14.Text;
                parameters.price_Equipment["Asphalt Distributor Truck (2500 - 3500 Gallons) [hr]"] = price_26_15.Text;
                parameters.price_Equipment["Asphalt Finisher (3-meter width) [hr]"] = price_26_16.Text;
                parameters.price_Equipment["Truck with Boom, small (6-10 MT) [hr]"] = price_26_17.Text;
                parameters.price_Equipment["Truck with Boom, small (12 - 15 MT) [hr]"] = price_26_18.Text;
                parameters.price_Equipment["Crawler Crane (21-25 MT) [hr]"] = price_26_19.Text;
                parameters.price_Equipment["Diesel Pile Hammer [hr]"] = price_26_20.Text;
                parameters.price_Equipment["Vibratory Pile Driver [hr]"] = price_26_21.Text;
                parameters.price_Equipment["Bagger Mixer (1-2 Bags) [hr]"] = price_26_22.Text;
                parameters.price_Equipment["Concrete Vibrator [hr]"] = price_26_23.Text;
                parameters.price_Equipment["Air Compressor  (Small) [hr]"] = price_26_24.Text;
                parameters.price_Equipment["Air Compressor (Big) [hr]"] = price_26_25.Text;
                parameters.price_Equipment["Bar Cutter [hr]"] = price_26_26.Text;
                parameters.price_Equipment["Bar Bender [hr]"] = price_26_27.Text;
                parameters.price_Equipment["Jack Hammer [hr]"] = price_26_28.Text;
                parameters.price_Equipment["Tamping Rammer [hr]"] = price_26_29.Text;
                parameters.price_Equipment["Welding Machine, Portable, 300A [hr]"] = price_26_30.Text;
                parameters.price_Equipment["Welding Machine, 600A [hr]"] = price_26_31.Text;
                parameters.price_Equipment["Generator Set 15-25kVA [hr]"] = price_26_32.Text;
                parameters.price_Equipment["Generator Set 50 kVA [hr]"] = price_26_33.Text;
                parameters.price_Equipment["Sump Pump (Dewatering) 0.75  –2HP- [hr]"] = price_26_34.Text;
                parameters.price_Equipment["Sump Pump (Dewatering) 5HP [hr]"] = price_26_35.Text;
                parameters.price_Equipment["Road Paint Stripper [hr]"] = price_26_36.Text;
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
            
        }

        private void price_SearchBar_bx_TextChanged(object sender, EventArgs e)
        {
            if (!price_SearchBar_bx.Text.Equals(""))
            {
                price_Search_Panel.Controls.Clear();
                IEnumerable<string> results = parameters.searchList.Where(s => s.ToLower().Contains(price_SearchBar_bx.Text.ToLower()));
                foreach (string result in results)
                {
                    Label label = new Label();
                    label.Click += searchLabel_Click;
                    label.AutoSize = true;
                    label.Text = result;
                    price_Search_Panel.Controls.Add(label);
                }
            }
        }

        protected void searchLabel_Click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            string[] data = lbl.Text.Split(new string[] { "] - " }, StringSplitOptions.None);
            string name = data[0] + "]";
            string category = data[1];
            if (category.Equals("Common Materials"))                        //1
                price_Category_cbx.SelectedIndex = 0;
            else if (category.Equals("Paint and Coating"))                  //2
                price_Category_cbx.SelectedIndex = 1;
            else if (category.Equals("Welding Rod"))                        //3
                price_Category_cbx.SelectedIndex = 2; 
            else if (category.Equals("Personal Protective Equipment"))      //4
                price_Category_cbx.SelectedIndex = 3;
            else if (category.Equals("Tools"))                              //5
                price_Category_cbx.SelectedIndex = 4;
            else if (category.Equals("Ready Mix Concrete"))                 //6
                price_Category_cbx.SelectedIndex = 5;
            else if (category.Equals("Gravel"))                             //7
                price_Category_cbx.SelectedIndex = 6;
            else if (category.Equals("Formworks and Lumber"))               //8
                price_Category_cbx.SelectedIndex = 7;
            else if (category.Equals("Roof Materials"))                     //9
                price_Category_cbx.SelectedIndex = 8;
            else if (category.Equals("Tubular Steel (1mm thick)"))          //10
                price_Category_cbx.SelectedIndex = 9;
            else if (category.Equals("Tubular Steel (1.2mm thick)"))        //11
                price_Category_cbx.SelectedIndex = 10;
            else if (category.Equals("Tubular Steel (1.5mm thick)"))        //12
                price_Category_cbx.SelectedIndex = 11;
            else if (category.Equals("Embankment"))                         //13
                price_Category_cbx.SelectedIndex = 12;
            else if (category.Equals("Rebar Grade 33 (230 Mpa)"))           //14
                price_Category_cbx.SelectedIndex = 13;
            else if (category.Equals("Rebar Grade 40 (275 Mpa)"))           //15
                price_Category_cbx.SelectedIndex = 14;
            else if (category.Equals("Rebar Grade 60 (415 Mpa)"))           //16
                price_Category_cbx.SelectedIndex = 15;
            else if (category.Equals("Labor Rate - Earthworks"))            //17
                price_Category_cbx.SelectedIndex = 16;
            else if (category.Equals("Labor Rate - Concreting"))            //18
                price_Category_cbx.SelectedIndex = 17;
            else if (category.Equals("Labor Rate - Formworks"))             //19
                price_Category_cbx.SelectedIndex = 18;
            else if (category.Equals("Labor Rate - Rebar"))                 //20
                price_Category_cbx.SelectedIndex = 19;
            else if (category.Equals("Labor Rate - Paint"))                 //21
                price_Category_cbx.SelectedIndex = 20;
            else if (category.Equals("Labor Rate - Tiles"))                 //22
                price_Category_cbx.SelectedIndex = 21;
            else if (category.Equals("Labor Rate - Masonry"))               //23
                price_Category_cbx.SelectedIndex = 22;
            else if (category.Equals("Labor Rate - Roofings"))              //24
                price_Category_cbx.SelectedIndex = 23;
            else if (category.Equals("Manpower - Manila"))                  //25.1
                price_Category_cbx.SelectedIndex = 24;
            else if (category.Equals("Manpower - Provincial"))              //25.2
                price_Category_cbx.SelectedIndex = 24;
            else if (category.Equals("Equipment"))                          //26
                price_Category_cbx.SelectedIndex = 25;
        }

        private void price_SettingsBtn_Click(object sender, EventArgs e)
        {
            FactorOfSafetyForm dlg = new FactorOfSafetyForm(this);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //Update view
                viewInitalized = false;
                initializeView();
            }
        }

        private void price_RestoreDefaultsBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to RESET price list into default?", "Reset Price List", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                initPriceListToDefault();
                AdjustPriceView();
            }
            else if (dialogResult == DialogResult.No)
            {
                //Do nothing
            }
        }
        //Price functions -- END

        //Summary functions -- START
        void initializeSummaryTable()
        {
            DataGridView summ_BOQ_dg = new DataGridView(); //Column Names
            this.summ_BOQ_dg.Columns.Add("item1", "");
            this.summ_BOQ_dg.Columns.Add("description1", "");
            this.summ_BOQ_dg.Columns.Add("description2", "");
            this.summ_BOQ_dg.Columns.Add("qty1", "");
            this.summ_BOQ_dg.Columns.Add("unit1", "");
            this.summ_BOQ_dg.Columns.Add("materials1", "UNIT COST");
            this.summ_BOQ_dg.Columns.Add("materials2", "TOTAL");
            this.summ_BOQ_dg.Columns.Add("labor1", "UNIT COST");
            this.summ_BOQ_dg.Columns.Add("labor2", "TOTAL");
            this.summ_BOQ_dg.Columns.Add("totalcost1", "TOTAL COST");
            /*for (int x = 0; x < 10; x++)
            {
                this.summ_BOQ_dg.Rows[0].Cells[x].Style.BackColor = Color.Green;
            }*/
            this.summ_BOQ_dg.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGreen;
            this.summ_BOQ_dg.EnableHeadersVisualStyles = false;

            for (int j = 0; j < this.summ_BOQ_dg.ColumnCount; j++)
            {
                this.summ_BOQ_dg.Columns[j].Width = 100;
            }

            this.summ_BOQ_dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.summ_BOQ_dg.ColumnHeadersHeight = this.summ_BOQ_dg.ColumnHeadersHeight * 2;
            this.summ_BOQ_dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            this.summ_BOQ_dg.CellPainting += new DataGridViewCellPaintingEventHandler(dataGridView1_CellPainting);
            this.summ_BOQ_dg.Paint += new PaintEventHandler(dataGridView1_Paint);
            this.summ_BOQ_dg.Scroll += new ScrollEventHandler(dataGridView1_Scroll);
            this.summ_BOQ_dg.ColumnWidthChanged += new DataGridViewColumnEventHandler(dataGridView1_ColumnWidthChanged);
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            Rectangle rtHeader = this.summ_BOQ_dg.DisplayRectangle;
            rtHeader.Height = this.summ_BOQ_dg.ColumnHeadersHeight / 2;
            this.summ_BOQ_dg.Invalidate(rtHeader);
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            Rectangle rtHeader = this.summ_BOQ_dg.DisplayRectangle;
            rtHeader.Height = this.summ_BOQ_dg.ColumnHeadersHeight / 2;
            this.summ_BOQ_dg.Invalidate(rtHeader);
        }

        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            string[] header_BOQ = {"ITEM", "DESCRIPTION", "QTY", "UNIT", "MATERIALS","LABOR",""};
            Rectangle rect;
            //Pen forBorder;
        
            int reach_nextC;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            int columnIndex = 0;
            
            for (int strIndex = 0; strIndex < header_BOQ.Length; strIndex++)
            {
                rect = this.summ_BOQ_dg.GetCellDisplayRectangle(columnIndex, -1, true);
                if(columnIndex == 1 || columnIndex == 5 || columnIndex == 7)
                {
                    reach_nextC = this.summ_BOQ_dg.GetCellDisplayRectangle(columnIndex + 1, -1, true).Width;
                    rect.X += 1;
                    rect.Y += 1;
                    rect.Width = rect.Width + reach_nextC - 2;
                    rect.Height = rect.Height / 2 - 2;
                    columnIndex +=2;
                }
                else
                {
                    rect.X += 1;
                    rect.Y += 1;
                    rect.Width = rect.Width - 2;
                    rect.Height = rect.Height / 2 - 2;
                    columnIndex +=1;
                }
                e.Graphics.FillRectangle(new SolidBrush(this.summ_BOQ_dg.ColumnHeadersDefaultCellStyle.BackColor), rect);
                e.Graphics.DrawString
                    (header_BOQ[strIndex],
                    this.summ_BOQ_dg.ColumnHeadersDefaultCellStyle.Font,
                    new SolidBrush(this.summ_BOQ_dg.ColumnHeadersDefaultCellStyle.ForeColor),
                    rect,
                    format);
                e.Graphics.DrawRectangle(Pens.Green,rect);
            }
        }   
        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex > -1)
            {
                Rectangle r2 = e.CellBounds;
                r2.Y += e.CellBounds.Height / 2;
                r2.Height = e.CellBounds.Height / 2;
                e.PaintBackground(r2, true);
                e.PaintContent(r2);
                e.Handled = true;
            }
        }

        private void summ_Export_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (fileName == null)
                saveDialog.FileName = "New Estimate.xls"; 
            else
                saveDialog.FileName = Path.GetFileNameWithoutExtension(fileName) + ".xls";
            saveDialog.Filter = "Excel Files|*.xls;*.xlsx";
            DialogResult result = saveDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                copyAlltoClipboard();

                object misValue = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Application xlexcel = new Microsoft.Office.Interop.Excel.Application();

                xlexcel.DisplayAlerts = false; // Without this you will get two confirm overwrite prompts
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlexcel.Workbooks.Add(misValue);
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // Format column D as text before pasting results, this was required for my data
                Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.get_Range("D:D").Cells;
                rng.NumberFormat = "@";

                // Format Columns 
                xlWorkSheet.Cells[1, 1] = "PROJECT:";
                xlWorkSheet.Cells[2, 1] = "LOCATION:";
                xlWorkSheet.Cells[3, 1] = "OWNER:";
                xlWorkSheet.Cells[4, 1] = "SUBJECT:";
                xlWorkSheet.Cells[1, 2] = summ_P_bx.Text;
                xlWorkSheet.Cells[2, 2] = summ_L_bx.Text;
                xlWorkSheet.Cells[3, 2] = summ_O_bx.Text;
                xlWorkSheet.Cells[4, 2] = summ_S_bx.Text;


                xlWorkSheet.Cells[6, 1] = "ITEM";

                Microsoft.Office.Interop.Excel.Range range = xlWorkSheet.Range["B6", "C6"];
                range.Value2 = "DESCRIPTION";
                range.Select();
                range.Merge();

                xlWorkSheet.Cells[6, 4] = "QTY";
                xlWorkSheet.Cells[6, 5] = "UNIT";

                Microsoft.Office.Interop.Excel.Range range2 = xlWorkSheet.Range["F6", "G6"];
                range2.Value2 = "MATERIALS";
                range2.Select();
                range2.Merge();

                xlWorkSheet.Cells[7, 6] = "UNIT COST";
                xlWorkSheet.Cells[7, 7] = "TOTAL";

                Microsoft.Office.Interop.Excel.Range range3 = xlWorkSheet.Range["H6", "I6"];
                range3.Value2 = "MATERIALS";
                range3.Select();
                range3.Merge();

                xlWorkSheet.Cells[7, 8] = "UNIT COST";
                xlWorkSheet.Cells[7, 9] = "TOTAL";

                xlWorkSheet.Cells[7, 10].Value = "TOTAL COST";

                // Paste clipboard results to worksheet range
                Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[8, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

                // Excel Style Formatting
                xlWorkSheet.Columns.AutoFit();

                xlWorkSheet.Columns[1].NumberFormat = "0.0";

                //xlWorkSheet.Columns[1].Font.Bold = true;

                xlWorkSheet.Columns[4].HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

                xlWorkSheet.Columns[6].HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

                xlWorkSheet.Columns[7].HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

                xlWorkSheet.Columns[8].HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

                xlWorkSheet.Columns[9].HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

                xlWorkSheet.Columns[10].HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                xlWorkSheet.get_Range("A6", "J7").Cells.HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;


                // Save the excel file under the captured location from the SaveFileDialog
                try
                {
                    xlWorkBook.SaveAs(saveDialog.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlexcel.DisplayAlerts = true;
                    xlWorkBook.Close(true, misValue, misValue);
                    xlexcel.Quit();
                } 
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString());
                    return;
                }

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlexcel);

                // Clear Clipboard and DataGridView selection
                Clipboard.Clear();
                summ_BOQ_dg.ClearSelection();

                // Open the newly saved excel file
                if (File.Exists(saveDialog.FileName))
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to open the exported excel file?", "Open Exported Excel File", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //Do Nothing
                    }
                }

            }
        }

        private void SaveToExcel(String fileName)
        {
            
        }
        private void copyAlltoClipboard()
        {
            summ_BOQ_dg.SelectAll();
            DataObject dataObj = summ_BOQ_dg.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        //Summary functions -- END

        //Help functions -- START
        void initializeHelpTree()
        {
            //Parent nodes
            TreeNode tn1 = new TreeNode("EARTHWORKS");
            TreeNode tn2 = new TreeNode("CONCRETE WORKS");
            TreeNode tn3 = new TreeNode("FORMWORKS");
            TreeNode tn4 = new TreeNode("PAINT WORKS");
            TreeNode tn5 = new TreeNode("TILE WORKS");
            TreeNode tn6 = new TreeNode("FOOTING REBARS");
            TreeNode tn7 = new TreeNode("WALLFOOTING REBARS");
            TreeNode tn8 = new TreeNode("COLUMN MAIN BARS");
            TreeNode tn9 = new TreeNode("COLUMN LATERAL TIES");
            TreeNode tn10 = new TreeNode("STAIRS REBARS");
            TreeNode tn11 = new TreeNode("BEAM MAIN REBARS");
            TreeNode tn12 = new TreeNode("BEAM STIRRUPS");
            TreeNode tn13 = new TreeNode("SLAB ON GRADE");
            TreeNode tn14 = new TreeNode("SUSPENDED SLAB");
            TreeNode tn15 = new TreeNode("MASONRY AND WALL REBARS");
            TreeNode tn16 = new TreeNode("ROOFINGS");


            //Init variables
            List<TreeNode> nodes;
            nodes = new List<TreeNode>() { tn1, tn2, tn3, tn4, tn5, tn6, tn7, tn8, tn9, tn10, tn11, tn12, tn13, tn14, tn15, tn16 };
            setTree(nodes);
        }
        private void setTree(List<TreeNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                help_treeView.Nodes.Add(nodes[i]);
            }
            AdjustTreeViewHeight(help_treeView);
        }

        private void help_treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)*/
            if (help_treeView.SelectedNode.ToString().Equals("TreeNode: EARTHWORKS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\EARTHWORKS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.EARTHWORKS);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            } 
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: CONCRETE WORKS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\CONCRETE WORKS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Concrete_works);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: FORMWORKS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\FORMWORKS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Formworks);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: PAINT WORKS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\PAINT WORKS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Paint_Works);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: TILE WORKS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\TILE WORKS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.TILE_WORKS);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: FOOTING REBARS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\FOOTING REBARS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Footing_Rebars);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: WALLFOOTING REBARS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\WALLFOOTING REBARS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Wall_Footing_Rebars);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: COLUMN MAIN BARS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\COLUMN MAIN BARS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Column_Main_Bars);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: COLUMN LATERAL TIES"))
            {
                String openPDFFile = Path.GetTempPath() + @"\COLUMN LATERAL TIES.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Column_Lateral_Ties);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: STAIRS REBARS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\STAIRS REBARS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.STAIRS_REBARS);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: BEAM MAIN REBARS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\BEAM MAIN REBARS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Beam_Main_Rebars);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: BEAM STIRRUPS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\BEAM STIRRUPS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Beam_Stirrups);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: SLAB ON GRADE"))
            {
                String openPDFFile = Path.GetTempPath() + @"\SLAB ON GRADE.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Slab_on_Grade);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: SUSPENDED SLAB"))
            {
                String openPDFFile = Path.GetTempPath() + @"\SUSPENDED SLAB.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Suspended_Slab);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: MASONRY AND WALL REBARS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\MASONRY AND WALL REBARS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.MASONRY_AND_WALL_REBARS);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
            else if (help_treeView.SelectedNode.ToString().Equals("TreeNode: ROOFINGS"))
            {
                String openPDFFile = Path.GetTempPath() + @"\ROOFINGS.pdf";//PDF Doc name
                System.IO.File.WriteAllBytes(openPDFFile, KnowEst.Properties.Resources.Roofings);//the resource automatically creates            
                help_webView.CoreWebView2.Navigate(openPDFFile);
            }
        }
        //Help functions -- END

        //Long Functions -- START
        private void SaveToFile(String fileName)
        {
            string stringParam = "";

            //Floors -- START
            stringParam += "Floors|\n";

            int count = 1;
            foreach(Floor floor in floors)
            {
                stringParam += "Floor-" + count + "|";      //Floor #
                stringParam += floor.getValues()[0] + "|";  //Floor Count
                stringParam += floor.getValues()[1] + "|";  //Floor Name
                foreach (TreeNode parentNode in floor.nodes)
                {
                    stringParam += parentNode.Text + "|";
                    if (parentNode.Nodes.Count == 0)
                    {
                        //Parent na walang laman
                    }
                    else
                    {
                        //Parent na may laman
                        foreach(TreeNode childNode in parentNode.Nodes)
                        {
                            stringParam += childNode.Text + "|" + childNode.Name + "|";
                        }
                    }
                }
                count++;
            }
            //Floors -- END

            //Parameters -- START
            stringParam += "\nParameters|\n";

            //Earthworks
            stringParam += "Earthworks|\n" +
            parameters.earth_CF_FA + "|" + parameters.earth_CF_TH + "|" + parameters.earth_CF_TY + "|" + parameters.earth_CF_CF + "|" +
            parameters.earth_WF_FA + "|" + parameters.earth_WF_TH + "|" + parameters.earth_WF_TY + "|" + parameters.earth_WF_CF + "|" +
            parameters.earth_WTB_FA + "|" + parameters.earth_WTB_TH + "|" + parameters.earth_WTB_TY + "|" + parameters.earth_WTB_CF + "|" +
            parameters.earth_SG_AS + "|" + parameters.earth_SG_TS + "|" + parameters.earth_SG_TH + "|" +
            parameters.earth_SG_TY + "|" + parameters.earth_SG_CF + "|";
            for(int i = 0; i < parameters.earth_elevations.Count; i++)
            {
                stringParam += "Elevation-" + (i + 1) + "|";
                stringParam += parameters.earth_elevations[i][0] + "|";
                stringParam += parameters.earth_elevations[i][1] + "|";
                stringParam += parameters.earth_elevations[i][2] + "|";
                stringParam += parameters.earth_elevations[i][3] + "|";
            }

            //Formworks
            stringParam += "\nFormworks|\n" + parameters.form_SM_F_FL + "|" +
            parameters.form_SM_C_FL + "|" + parameters.form_SM_C_VS + "|" + parameters.form_SM_C_HB + "|" +
            parameters.form_SM_B_FL + "|" + parameters.form_SM_B_VS + "|" + parameters.form_SM_B_HB + "|" + parameters.form_SM_B_DB + "|" +
            parameters.form_SM_HS_VS + "|" +
            parameters.form_SM_ST_FL + "|" + parameters.form_SM_ST_VS + "|" +
            parameters.form_F_T + "|" + parameters.form_F_NU + "|" + parameters.form_F_N + "|";

            //Concrete
            stringParam += "\nConcrete|\n" +
            parameters.conc_cmIsSelected[0] + "|" + parameters.conc_CM_F_CG + "|" + parameters.conc_CM_F_GT + "|" + parameters.conc_CM_F_RM + "|" +
            parameters.conc_cmIsSelected[1] + "|" + parameters.conc_CM_C_CG + "|" + parameters.conc_CM_C_GT + "|" + parameters.conc_CM_C_RM + "|" +
            parameters.conc_cmIsSelected[2] + "|" + parameters.conc_CM_B_CG + "|" + parameters.conc_CM_B_GT + "|" + parameters.conc_CM_B_RM + "|" +
            parameters.conc_cmIsSelected[3] + "|" + parameters.conc_CM_S_SOG_CG + "|" + parameters.conc_CM_S_SOG_GT + "|" + parameters.conc_CM_S_SOG_RM + "|" +
            parameters.conc_cmIsSelected[4] + "|" + parameters.conc_CM_S_SS_CG + "|" + parameters.conc_CM_S_SS_GT + "|" + parameters.conc_CM_S_SS_RM + "|" +

            parameters.conc_CM_W_MEW_CM + "|" + parameters.conc_CM_W_MIW_CM + "|" + parameters.conc_CM_W_P_CM + "|" + parameters.conc_CM_W_P_PT + "|" +

            parameters.conc_cmIsSelected[5] + "|" + parameters.conc_CM_ST_CG + "|" + parameters.conc_CM_ST_GT + "|" + parameters.conc_CM_ST_RM + "|" +

            parameters.conc_CC_F + "|" + parameters.conc_CC_SS + "|" + parameters.conc_CC_SG + "|" + parameters.conc_CC_BEE + "|" +
            parameters.conc_CC_BEW + "|" + parameters.conc_CC_CEE + "|" + parameters.conc_CC_CEW + "|" ;

            //Reinforcement 
            stringParam += "\nReinforcement|\n";
            stringParam += "Tension_Bars|";
            int j = 1;
            foreach (DataRow dtRow in parameters.rein_LSL_TB_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_LSL_TB_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            j = 1;
            for(int i = 0; i < parameters.rein_LSL_TB_fc_list.Count; i++)
            {
                stringParam += "FCPrime-" + j + "|" + parameters.rein_LSL_TB_fc_list[i] + "|";
                j++;
            }
            stringParam += "Compression_Bars|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_LSL_CB_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_LSL_CB_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            j = 1;
            for (int i = 0; i < parameters.rein_LSL_CB_fc_list.Count; i++)
            {
                stringParam += "FCPrime-" + j + "|" + parameters.rein_LSL_CB_fc_list[i] + "|";
                j++;
            }
            stringParam += "Main_Bars|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_BEH_MB_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_BEH_MB_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            stringParam += "Stirrups_and_Ties|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_BEH_ST_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_BEH_ST_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            stringParam += "Weight|"; j = 1;
            foreach (DataRow dtRow in parameters.rein_W_dt.Rows)
            {
                stringParam += "Row-" + j + "|";
                foreach (DataColumn dc in parameters.rein_W_dt.Columns)
                {
                    stringParam += dtRow[dc].ToString() + "|";
                }
                j++;
            }
            stringParam += parameters.rein_RG_C + "|" + parameters.rein_RG_CLT + "|" + parameters.rein_RG_F + "|" + parameters.rein_RG_B + "|" + parameters.rein_RG_BS + "|" + parameters.rein_RG_ST + "|" + parameters.rein_RG_W + "|" + parameters.rein_RG_SL + "|";

            for (int col = 0; col < parameters.rein_mfIsSelected.GetLength(0); col++)
                for (int row = 0; row < parameters.rein_mfIsSelected.GetLength(1); row++)
                    stringParam += parameters.rein_mfIsSelected[col, row] + "|";

            //Paint
            stringParam += "\nPaint|\n" + parameters.paint_SCL + "|";
            for (int i = 0; i < parameters.paint_Area.Count; i++)
            {
                stringParam += "Paint_Area-" + (i + 1) + "|";
                stringParam += parameters.paint_Area[i][0] + "|";
                stringParam += parameters.paint_Area[i][1] + "|";
                stringParam += parameters.paint_Area[i][2] + "|";
            }

            //Tiles
            stringParam += "\nTiles|\n" + parameters.tiles_FS + "|" + parameters.tiles_TG + "|";
            for (int i = 0; i < parameters.tiles_Area.Count; i++)
            {
                stringParam += "Tile_Area-" + (i + 1) + "|";
                stringParam += parameters.tiles_Area[i][0] + "|";
                stringParam += parameters.tiles_Area[i][1] + "|";
                stringParam += parameters.tiles_Area[i][2] + "|";
            }

            //Masonry
            stringParam += "\nMasonry|\n" + parameters.mason_CHB_EW + "|" + parameters.mason_CHB_IW + "|";
            for (int i = 0; i < parameters.mason_exteriorWall.Count; i++)
            {
                stringParam += "Exterior_Wall-" + (i + 1) + "|";
                stringParam += parameters.mason_exteriorWall[i][0] + "|";
                stringParam += parameters.mason_exteriorWall[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_exteriorWindow.Count; i++)
            {
                stringParam += "Exterior_Window-" + (i + 1) + "|";
                stringParam += parameters.mason_exteriorWindow[i][0] + "|";
                stringParam += parameters.mason_exteriorWindow[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_exteriorDoor.Count; i++)
            {
                stringParam += "Exterior_Door-" + (i + 1) + "|";
                stringParam += parameters.mason_exteriorDoor[i][0] + "|";
                stringParam += parameters.mason_exteriorDoor[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_interiorWall.Count; i++)
            {
                stringParam += "Interior_Wall-" + (i + 1) + "|";
                stringParam += parameters.mason_interiorWall[i][0] + "|";
                stringParam += parameters.mason_interiorWall[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_interiorWindow.Count; i++)
            {
                stringParam += "Interior_Window-" + (i + 1) + "|";
                stringParam += parameters.mason_interiorWindow[i][0] + "|";
                stringParam += parameters.mason_interiorWindow[i][1] + "|";
            }
            for (int i = 0; i < parameters.mason_interiorDoor.Count; i++)
            {
                stringParam += "Interior_Door-" + (i + 1) + "|";
                stringParam += parameters.mason_interiorDoor[i][0] + "|";
                stringParam += parameters.mason_interiorDoor[i][1] + "|";
            }

            stringParam += "END|" + parameters.mason_RTW_VS + "|" + parameters.mason_RTW_HSL + "|" +
            parameters.mason_RTW_RG + "|" + parameters.mason_RTW_BD + "|" +
            parameters.mason_RTW_RL + "|" + parameters.mason_RTW_LTW + "|";

            //Stairs
            stringParam += "\nStairs|\n";
            j = 0;
            foreach (List<StairParameterUserControl> floor in parameters.stair)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (StairParameterUserControl stair in floor)
                {
                    stringParam += "Stair-" + (k + 1) + "|";
                    string[] values = stair.getValues();
                    foreach (string value in values)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            //Labor and Equipment
            stringParam += "\nLabor_and_Equipment|\n" + parameters.labor_RD + "|";
            for (int i = 0; i < parameters.labor_MP.Count; i++)
            {
                stringParam += "Man_Power-" + (i + 1) + "|";
                stringParam += parameters.labor_MP[i][0] + "|";
                stringParam += parameters.labor_MP[i][1] + "|";
                stringParam += parameters.labor_MP[i][2] + "|";
                stringParam += parameters.labor_MP[i][3] + "|";
                stringParam += parameters.labor_MP[i][4] + "|";
            }
            for (int i = 0; i < parameters.labor_EQP.Count; i++)
            {
                stringParam += "Equipment-" + (i + 1) + "|";
                stringParam += parameters.labor_EQP[i][0] + "|";
                stringParam += parameters.labor_EQP[i][1] + "|";
                stringParam += parameters.labor_EQP[i][2] + "|";
                stringParam += parameters.labor_EQP[i][3] + "|";
                stringParam += parameters.labor_EQP[i][4] + "|";
            }

            //Misc
            stringParam += "\nMisc|\n";
            for (int i = 0; i < parameters.misc_CustomItems.Count; i++)
            {
                stringParam += "Custom_Item-" + (i + 1) + "|";
                stringParam += parameters.misc_CustomItems[i][0] + "|";
                stringParam += parameters.misc_CustomItems[i][1] + "|";
                stringParam += parameters.misc_CustomItems[i][2] + "|";
                stringParam += parameters.misc_CustomItems[i][3] + "|";
            }

            //Price List
            stringParam += "\nPrice-List|\n";
            stringParam += "Price-Checklist|\n";
            //Checklist
            foreach(bool checkMark in earthworksChecklist)
            {
                stringParam += checkMark + "|"; 
            }
            foreach(bool checkMark in concreteChecklist)
            {
                stringParam += checkMark + "|";
            }
            foreach (bool checkMark in formworksChecklist)
            {
                stringParam += checkMark + "|";
            }
            foreach (bool checkMark in masonryChecklist)
            {
                stringParam += checkMark + "|";
            }
            foreach (bool checkMark in rebarsChecklist)
            {
                stringParam += checkMark + "|";
            }
            foreach (bool checkMark in roofingsChecklist)
            {
                stringParam += checkMark + "|";
            }
            foreach (bool checkMark in tilesChecklist)
            {
                stringParam += checkMark + "|";
            }
            foreach(bool checkMark in paintsChecklist)
            {
                stringParam += checkMark + "|";
            }

            //Factor of Safety
            stringParam += "\n" + fos_Cement + "|" + fos_Sand + "|" + fos_Gravel + "|" + fos_RMC + "|" + fos_CHB + "|" + 
                           fos_Tiles + "|" + fos_LC_Type + "|" + fos_LC_Percentage + "|";

            //1
            stringParam += "\nCommon-Materials|\n";
            foreach (DictionaryEntry dict in parameters.price_CommonMaterials)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //2
            stringParam += "\nPaint-and-Coating|\n";
            foreach (DictionaryEntry dict in parameters.price_PaintAndCoating)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //3
            stringParam += "\nWelding Rod|\n";
            foreach (DictionaryEntry dict in parameters.price_WeldingRod)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //4
            stringParam += "\nPersonal-Protective-Equipment|\n";
            foreach (DictionaryEntry dict in parameters.price_PersonalProtectiveEquipment)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //5
            stringParam += "\nTools|\n";
            foreach (DictionaryEntry dict in parameters.price_Tools)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //6
            stringParam += "\nReady-Mix-Concrete|\n";
            foreach (DictionaryEntry dict in parameters.price_ReadyMixConcrete)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //7
            stringParam += "\nGravel|\n";
            foreach (DictionaryEntry dict in parameters.price_Gravel)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //8
            stringParam += "\nFormworks-and-Lumber|\n";
            foreach (DictionaryEntry dict in parameters.price_FormworksAndLumber)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //9
            stringParam += "\nRoof-Materials|\n";
            foreach (DictionaryEntry dict in parameters.price_RoofMaterials)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //10
            stringParam += "\nTubular-Steel-1mm|\n";
            foreach (DictionaryEntry dict in parameters.price_TubularSteel1mm)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //11
            stringParam += "\nTubular-Steel-1p2mm|\n";
            foreach (DictionaryEntry dict in parameters.price_TubularSteel1p2mm)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //12
            stringParam += "\nTubular-Steel-1p5mm|\n";
            foreach (DictionaryEntry dict in parameters.price_TubularSteel1p5mm)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //13
            stringParam += "\nEmbankment|\n";
            foreach (DictionaryEntry dict in parameters.price_Embankment)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //14
            stringParam += "\nRebar-Grade-33|\n";
            foreach (DictionaryEntry dict in parameters.price_RebarGrade33)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //15
            stringParam += "\nRebar-Grade-40|\n";
            foreach (DictionaryEntry dict in parameters.price_RebarGrade40)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //16
            stringParam += "\nRebar-Grade-60|\n";
            foreach (DictionaryEntry dict in parameters.price_RebarGrade60)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //17
            stringParam += "\nLabor-Rate-Earthworks|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Earthworks)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //18
            stringParam += "\nLabor-Rate-Concreting|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Concreting)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //19
            stringParam += "\nLabor-Rate-Formworks|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Formworks)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //20
            stringParam += "\nLabor-Rate-Rebar|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Rebar)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //21
            stringParam += "\nLabor-Rate-Paint|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Paint)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //22
            stringParam += "\nLabor-Rate-Tiles|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Tiles)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //23
            stringParam += "\nLabor-Rate-Masonry|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Masonry)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //24
            stringParam += "\nLabor-Rate-Roofings|\n";
            foreach (DictionaryEntry dict in parameters.price_LaborRate_Roofings)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //25.1
            stringParam += "\nManpower-Manila|\n";
            foreach (DictionaryEntry dict in parameters.price_ManpowerM)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //25.2
            stringParam += "\nManpower-Provincial|\n";
            foreach (DictionaryEntry dict in parameters.price_ManpowerP)
            {
                stringParam += dict.Value.ToString() + "|";
            }
            //26
            stringParam += "\nEquipment|\n";
            foreach (DictionaryEntry dict in parameters.price_Equipment)
            {
                stringParam += dict.Value.ToString() + "|";
            }

            //BOQ
            stringParam += "\nBOQ|\n";
            stringParam += summ_P_bx.Text + "|";
            stringParam += summ_L_bx.Text + "|";
            stringParam += summ_O_bx.Text + "|";
            stringParam += summ_S_bx.Text + "|";

            //Parameters -- END

            //Structural Members -- START
            stringParam += "\nStructural-Members|\n";

            //Footings
            stringParam += "Footings|\n" + "Floor-1" + "|"; //Only Ground Floor exists
            for (int i = 0; i < structuralMembers.footingColumnNames.Count; i++)
            {
                stringParam += "Column-Footing-" + (i + 1) + "|" + structuralMembers.footingColumnNames[i] + "|";
                foreach(string value in structuralMembers.footingsColumn[0][i])
                {
                    stringParam += value + "|";
                }
            }
            for (int i = 0; i < structuralMembers.footingWallNames.Count; i++)
            {
                stringParam += "Wall-Footing-" + (i + 1) + "|" + structuralMembers.footingWallNames[i] + "|";
                foreach (string value in structuralMembers.footingsWall[0][i])
                {
                    stringParam += value + "|";
                }
            }

            //Columns
            stringParam += "\nColumns|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.column)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Column-" + (k + 1) + "|";
                    stringParam += structuralMembers.columnNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Lateral-Ties" + "|";
                    foreach (string value in structuralMembers.columnLateralTies[j][k])
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Spacing" + "|";
                    foreach (string value in structuralMembers.columnSpacing[j][k])
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            //Beams
            stringParam += "\nBeams|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.beam)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Beam-" + (k + 1) + "|";
                    stringParam += structuralMembers.beamNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Beam-Rows" + "|";
                    foreach (List<string> row in structuralMembers.beamRow[j][k])
                    {
                        stringParam += row[0] + "|";
                        stringParam += row[1] + "|";
                        stringParam += row[2] + "|";
                        stringParam += row[3] + "|";
                        stringParam += row[4] + "|";
                    }
                    k++;
                }
                stringParam += "Beam-Schedules" + "|";
                foreach (List<string> schedule in structuralMembers.beamSchedule[j])
                    foreach(string value in schedule) 
                        stringParam += value + "|";
                j++;
            }
            
            //Slabs
            stringParam += "\nSlabs|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.slab)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Slab-" + (k + 1) + "|";
                    stringParam += structuralMembers.slabNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                if(j > 0)
                {
                    stringParam += "Slab-Schedules" + "|";
                    foreach (List<string> schedule in structuralMembers.slabSchedule[j - 1])
                        foreach (string value in schedule)
                            stringParam += value + "|";
                }
                j++;
            }

            //Stairs
            stringParam += "\nStairs|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.stairs)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach(List<string> member in floor)
                {
                    stringParam += "Stair-" + (k + 1) + "|";
                    stringParam += structuralMembers.stairsNames[j][k] + "|";
                    foreach(string value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            //Roof
            stringParam += "\nRoof|\n";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.roof)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "Roof-" + (k + 1) + "|";
                    stringParam += structuralMembers.roofNames[j][k] + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    stringParam += "Roof-Sheet" + "|";
                    foreach (string value in structuralMembers.roofHRS[j][k])
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Structural Members -- END

            //Solutions -- START
            stringParam += "\nSolutions|\n";

            //Earth Works
            stringParam += "Earthworks|\n" + "Floor-1" + "|"; //Only Ground Floor exists
            j = 0;
            foreach (List<double> solutions in structuralMembers.earthworkSolutions)
            {
                stringParam += "earthwork-" + (j + 1) + "|";
                foreach (double value in solutions)
                {
                    stringParam += value + "|";
                }
                j++;
            }
            stringParam += "Extra-Earthworks|";
            foreach(double value in structuralMembers.extraEarthworkSolutions)
            {
                stringParam += value + "|";
            }

            //Concrete Works
            stringParam += "\nConcrete-Works|\n";
            stringParam += "ConcreteF|\n";
            j = 0;
            foreach (List<double> solutions in structuralMembers.concreteWorkSolutionsF)
            {
                stringParam += "concreteF-" + (j + 1) + "|";
                foreach (double value in solutions)
                {
                    stringParam += value + "|";
                }
                j++;
            }

            stringParam += "\nConcreteC|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsC)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteC-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            stringParam += "\nConcreteBR|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsBR)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteBR-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            stringParam += "\nConcreteSL|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsSL)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteSL-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }

            stringParam += "\nConcreteST|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.concreteWorkSolutionsST)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "concreteST-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Solutions -- END

            //Totalities -- START
            stringParam += "\nTotalities|\n";

            //1.0 - Earthworks
            stringParam += "\nEarthworks|\n";
            //Totalities
            stringParam += excavation_Total + "|";
            stringParam += backfillingAndCompaction_Total + "|";
            stringParam += gradingAndCompaction_Total + "|";
            stringParam += gravelBedding_Total + "|";
            stringParam += soilPoisoning_Total + "|";
            //Cost
            stringParam += excavation_CostL + "|";
            stringParam += backfillingAndCompaction_CostL + "|";
            stringParam += gradingAndCompaction_CostL + "|";
            stringParam += gravelBedding_CostM + "|";
            stringParam += gravelBedding_CostL + "|";
            stringParam += gravelBedding_CostTotal + "|";
            stringParam += soilPoisoning_CostM + "|";
            stringParam += earthworks_CostTotal + "|";

            //2.0 - Concrete
            //stringParam += "\nConcrete|\n";
            //Totalities
            //Cost
            
            //3.0 - Formworks
            stringParam += "\nFormworks|\n";
            //Footings
            stringParam += "per_col|\n";
            foreach (double solution in structuralMembers.per_col)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nper_wal|\n";
            foreach (double solution in structuralMembers.per_wal)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nfootings_comps|\n";
            foreach (double solution in structuralMembers.footings_comps)
            {
                stringParam += solution + "|";
            }
            //Columns
            stringParam += "\ncol_area|\n";
            foreach (double solution in structuralMembers.col_area)
            {
                stringParam += solution + "|";
            }
            stringParam += "\ncol_woods|\n";
            foreach (double solution in structuralMembers.col_woods)
            {
                stringParam += solution + "|";
            }
            stringParam += "\ncol_post|\n";
            foreach (double solution in structuralMembers.col_post)
            {
                stringParam += solution + "|";
            }
            stringParam += "\ncol_scafV|\n";
            foreach (double solution in structuralMembers.col_scafV)
            {
                stringParam += solution + "|";
            }
            stringParam += "\ncol_scafH|\n";
            foreach (double solution in structuralMembers.col_scafH)
            {
                stringParam += solution + "|";
            }
            stringParam += "\ncol_scafD|\n";
            foreach (double solution in structuralMembers.col_scafD)
            {
                stringParam += solution + "|";
            }
            //Beams
            stringParam += "\nbeams_comps|\n";
            foreach (double solution in structuralMembers.beams_comps)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_tiearea|\n";
            foreach (double solution in structuralMembers.beams_tiearea)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_gradarea|\n";
            foreach (double solution in structuralMembers.beams_gradarea)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_grade|\n";
            foreach (double solution in structuralMembers.beams_grade)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_gradeFrame|\n";
            foreach (double solution in structuralMembers.beams_gradeFrame)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_vertical|\n";
            foreach (double solution in structuralMembers.beams_vertical)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_horizontal|\n";
            foreach (double solution in structuralMembers.beams_horizontal)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_RB|\n";
            foreach (double solution in structuralMembers.beams_RB)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_RBarea|\n";
            foreach (double solution in structuralMembers.beams_RBarea)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_RBframe|\n";
            foreach (double solution in structuralMembers.beams_RBframe)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_RBV|\n";
            foreach (double solution in structuralMembers.beams_RBV)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nbeams_RBH|\n";
            foreach (double solution in structuralMembers.beams_RBH)
            {
                stringParam += solution + "|";
            }
            //Slabs
            stringParam += "\nslab_area|\n";
            foreach (double solution in structuralMembers.slab_area)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nslab_form|\n";
            foreach (double solution in structuralMembers.slab_form)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nslab_scaf|\n";
            foreach (double solution in structuralMembers.slab_scaf)
            {
                stringParam += solution + "|";
            }
            //Stairs
            stringParam += "\nUstairsFORM|\n";
            foreach (double solution in structuralMembers.UstairsFORM)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nUstairsFRAME|\n";
            foreach (double solution in structuralMembers.UstairsFRAME)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nUstairsSCAF|\n";
            foreach (double solution in structuralMembers.UstairsSCAF)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nLstairsFORM|\n";
            foreach (double solution in structuralMembers.LstairsFORM)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nLstairsFRAME|\n";
            foreach (double solution in structuralMembers.LstairsFRAME)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nLstairsSCAF|\n";
            foreach (double solution in structuralMembers.LstairsSCAF)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nSstairsFORM|\n";
            foreach (double solution in structuralMembers.SstairsFORM)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nSstairsFRAME|\n";
            foreach (double solution in structuralMembers.SstairsFRAME)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nSstairsSCAF|\n";
            foreach (double solution in structuralMembers.SstairsSCAF)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nUAREA|\n";
            foreach (double solution in structuralMembers.UAREA)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nLAREA|\n";
            foreach (double solution in structuralMembers.LAREA)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nSAREA|\n";
            foreach (double solution in structuralMembers.SAREA)
            {
                stringParam += solution + "|";
            }

            //4.0 - Masonry 
            stringParam += "\nMasonry|\n";
            //Totalities
            stringParam += "\nSolutionP1|\n";
            foreach (double solution in masonrysSolutionP1)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nSolutionP2|\n";
            foreach (double solution in masonrysSolutionP2)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nSolutionP3|\n";
            foreach (double solution in masonrysSolutionP3)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nInteriorAndExterior|\n";
            stringParam += extCHBdimension + "|";
            stringParam += intCHBdimension + "|";
            //Cost
            stringParam += exterior_UnitM + "|";
            stringParam += exterior_CostM + "|";
            stringParam += exterior_CostL + "|";
            stringParam += exterior_costTotal + "|";
            stringParam += interior_UnitM + "|";
            stringParam += interior_CostM + "|";
            stringParam += interior_CostL + "|";
            stringParam += interior_costTotal + "|";
            stringParam += masonMCost_total + "|";
            stringParam += masonLCost_total + "|";
            stringParam += mason_TOTALCOST + "|";

            //5.0 - Rebars (DITO DOS)
            //Totalities
            stringParam += "\nRebars|\n";
            //Footing
            stringParam += "RebarsF|\n";
            j = 0;
            foreach (List<List<double>> member in structuralMembers.footingReinforcements)
            {
                stringParam += "member-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> row in member)
                {
                    stringParam += "row-" + (k + 1) + "|";
                    foreach (double value in row)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Wall Footing
            stringParam += "\nRebarsWF|\n";
            j = 0;
            foreach (List<List<double>> member in structuralMembers.wallFootingReinforcements)
            {
                stringParam += "member-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> row in member)
                {
                    stringParam += "row-" + (k + 1) + "|";
                    foreach (double value in row)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Column Main
            stringParam += "\nRebarsCM|\n";
            stringParam += structuralMembers.totalweightkgm_Colmain + "|";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.Column_mainRebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Column Lateral Ties
            stringParam += "\nRebarsCT|\n";
            stringParam += structuralMembers.totalweightkgm_Colties + "|";
            j = 0;
            foreach (List<List<List<string>>> floor in structuralMembers.Column_lateralRebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<List<string>> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    int l = 0;
                    foreach (List<string> row in member)
                    {
                        stringParam += "row-" + (l + 1) + "|";
                        foreach (string value in row)
                        {
                            stringParam += value + "|";
                        }
                        l++;
                    }
                    k++;
                }
                j++;
            }
            //Beam Main 1
            stringParam += "\nRebarsBM1|\n";
            stringParam += structuralMembers.totalweightkgm_main + "|";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.beamdias)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Beam Main 2
            stringParam += "\nRebarsBM2|\n";
            j = 0;
            foreach (List<List<List<string>>> floor in structuralMembers.Beam_mainRebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<List<string>> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    int l = 0;
                    foreach (List<string> row in member)
                    {
                        stringParam += "row-" + (l + 1) + "|";
                        foreach (string value in row)
                        {
                            stringParam += value + "|";
                        }
                        l++;
                    }
                    k++;
                }
                j++;
            }
            //Beam Stirrups
            stringParam += "\nRebarsBS|\n";
            stringParam += structuralMembers.totalweightkgm_stir + "|";
            j = 0;
            foreach (List<List<List<string>>> floor in structuralMembers.Beam_stirRebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<List<string>> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    int l = 0;
                    foreach (List<string> row in member)
                    {
                        stringParam += "row-" + (l + 1) + "|";
                        foreach (string value in row)
                        {
                            stringParam += value + "|";
                        }
                        l++;
                    }
                    k++;
                }
                j++;
            }
            //Beam Web
            stringParam += "\nRebarsBW|\n";
            stringParam += structuralMembers.totalweightkgm_web + "|";
            j = 0;
            foreach (List<List<List<string>>> floor in structuralMembers.Beam_webRebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<List<string>> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    int l = 0;
                    foreach (List<string> row in member)
                    {
                        stringParam += "row-" + (l + 1) + "|";
                        foreach (string value in row)
                        {
                            stringParam += value + "|";
                        }
                        l++;
                    }
                    k++;
                }
                j++;
            }
            //Slab on Grade
            stringParam += "\nRebarsSOG|\n";
            stringParam += structuralMembers.totalweightkgm_slabongrade + "|";
            j = 0;
            foreach (List<List<string>> floor in structuralMembers.Slab_ongradeRebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<string> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    foreach (string value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Suspended Slab
            stringParam += "\nRebarsSS|\n";
            stringParam += structuralMembers.totalweightkgm_suspendedslab + "|";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.Slab_suspendedRebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    foreach (double value in member)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Stairs
            stringParam += "\nRebarsST|\n";
            j = 0;
            foreach (List<List<double[,]>> floor in structuralMembers.stairs_Rebar)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double[,]> member in floor)
                {
                    stringParam += "member-" + (k + 1) + "|";
                    int l = 0;
                    foreach (double[,] rows in member)
                    {
                        stringParam += "rows-" + (l + 1) + "|";
                        for (int x = 0; x <= rows.GetUpperBound(0); x++)
                        {
                            stringParam += "row-" + (x + 1) + "|";
                            for (int y = 0; y <= rows.GetUpperBound(1); y++)
                                stringParam += rows[x, y] + "|";
                        }
                        l++;
                    }
                    k++;
                }
                j++;
            }

            //6.0 - Roofings
            stringParam += "\nRoofings|\n";
            //Totalities
            stringParam += "Solutions|\n";
            j = 0;
            foreach (List<List<double>> floor in structuralMembers.roofSolutions)
            {
                stringParam += "Floor-" + (j + 1) + "|";
                int k = 0;
                foreach (List<double> solutions in floor)
                {
                    stringParam += "roof-" + (k + 1) + "|";
                    foreach (double value in solutions)
                    {
                        stringParam += value + "|";
                    }
                    k++;
                }
                j++;
            }
            //Cost
            stringParam += "\nCost|\n";
            stringParam += rANDp_MCost + "|";
            stringParam += rANDp_LCost + "|";
            stringParam += rANDp_costTotal + "|";
            stringParam += acce_MCost + "|";
            stringParam += acce_LCost + "|";
            stringParam += acce_costTotal + "|";
            stringParam += tins_MCost + "|";
            stringParam += tins_LCost + "|";
            stringParam += tins_costTotal + "|";
            stringParam += roof_MTOTAL + "|";
            stringParam += roof_LTOTAL + "|";
            stringParam += roof_TOTALCOST + "|";

            //7.0 - Tiles
            stringParam += "\nTiles|\n";
            //Totalities
            stringParam += "\nSixhun|\n";
            foreach (double solution in sixhun)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nThreehun|\n";
            foreach (double solution in threehun)
            {
                stringParam += solution + "|";
            }
            //Cost
            stringParam += "\nCost|\n";
            stringParam += sixhun_MUnit + "|";
            stringParam += sixhun_MCost + "|";
            stringParam += sixhun_LCost + "|";
            stringParam += sixhun_costTotal + "|";
            stringParam += threehun_MUnit + "|";
            stringParam += threehun_MCost + "|";
            stringParam += threehun_LCost + "|";
            stringParam += threehun_costTotal + "|";
            stringParam += tiles_mTOTALCOST + "|";
            stringParam += tiles_lTOTALCOST + "|";
            stringParam += tiles_TOTALCOST + "|";

            //8.0 - Paints
            stringParam += "\nPaints|\n";
            //Totalities
            stringParam += "\nEnamel|\n";
            foreach (double solution in enamel)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nAcrylic|\n";
            foreach (double solution in acrylic)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nLatex|\n";
            foreach (double solution in latex)
            {
                stringParam += solution + "|";
            }
            stringParam += "\nGloss|\n";
            foreach (double solution in gloss)
            {
                stringParam += solution + "|";
            }
            //Cost
            stringParam += "\nCost|\n";
            stringParam += enam_MUnit + "|";
            stringParam += enam_MCost + "|";
            stringParam += enam_LCost + "|";
            stringParam += enam_TOTALCOST + "|";
            stringParam += acry_MUnit + "|";
            stringParam += acry_MCost + "|";
            stringParam += acry_LCost + "|";
            stringParam += acry_TOTALCOST + "|";
            stringParam += late_MUnit + "|";
            stringParam += late_MCost + "|";
            stringParam += late_LCost + "|";
            stringParam += late_TOTALCOST + "|";
            stringParam += semi_MUnit + "|";
            stringParam += semi_MCost + "|";
            stringParam += semi_LCost + "|";
            stringParam += semi_TOTALCOST + "|";
            stringParam += paint_mTOTALCOST + "|";
            stringParam += paint_lTOTALCOST + "|";
            stringParam += paints_TOTALCOST + "|";

            //Totalities
            //Cost

            //Totalities -- END

            stringParam += "\nEND";

            //Save to File
            File.WriteAllText(fileName, stringParam);
        }

        private void initializeVariablesFromFile()
        {
            //Initialize variables in Structural Members for every floor created
            if (Floors.Count == 0)//Ground Floor
            {
                //Footings
                List<List<string>> newList = new List<List<string>>();
                List<List<string>> newList2 = new List<List<string>>();
                structuralMembers.footingsColumn.Add(newList);
                structuralMembers.footingsWall.Add(newList2);
            }

            /*Solution Variables
            List<List<double>> newList19 = new List<List<double>>();
            List<List<double>> newList20 = new List<List<double>>();
            List<List<double>> newList21 = new List<List<double>>();
            List<List<double>> newList22 = new List<List<double>>();
            structuralMembers.concreteWorkSolutionsC.Add(newList19);
            structuralMembers.concreteWorkSolutionsBR.Add(newList20);
            structuralMembers.concreteWorkSolutionsSL.Add(newList21);
            structuralMembers.concreteWorkSolutionsST.Add(newList22);
            //*/
        }

        private void SaveToProgram(string stringFile)
        {
            //Remove newlines
            stringFile = stringFile.Replace("\n", String.Empty);
            //Divide data, store into tokens
            string[] tokens = stringFile.Split('|');

            //Save Floors -- START
            int i = 1;
            int j = 0;

            //Clear floor related variables
            floors.Clear();
            estimationPanel.Controls.Clear();
            /*
            foreach (Floor floor in floors)
            {
                foreach (TreeNode parentNode in floor.nodes)
                {
                    if (parentNode.Nodes.Count == 0)
                    {
                        //Parent na walang laman
                    }
                    else
                    {
                        //Parent na may laman
                        parentNode.Nodes.Clear();
                    }
                }
            }
            */

            //Clear variables in Parameters
            parameters.earth_elevations.Clear();

            parameters.rein_LSL_TB_fc_list.Clear();
            parameters.rein_LSL_CB_fc_list.Clear();
            parameters.rein_LSL_TB_fc_list.Clear();

            parameters.paint_Area.Clear();

            parameters.tiles_Area.Clear();

            parameters.mason_exteriorWall.Clear();
            parameters.mason_exteriorWindow.Clear();
            parameters.mason_exteriorDoor.Clear();
            parameters.mason_interiorWall.Clear();
            parameters.mason_interiorWindow.Clear();
            parameters.mason_interiorDoor.Clear();

            parameters.labor_MP.Clear();
            parameters.labor_EQP.Clear();

            parameters.misc_CustomItems.Clear();

            //Clear variables in AddStructForm
            structuralMembers.footingColumnNames.Clear();
            structuralMembers.footingWallNames.Clear();
            structuralMembers.footingsColumn.Clear();
            structuralMembers.footingsWall.Clear();

            structuralMembers.columnNames.Clear();
            structuralMembers.column.Clear();
            structuralMembers.columnLateralTies.Clear();
            structuralMembers.columnSpacing.Clear();

            structuralMembers.beamNames.Clear();
            structuralMembers.beam.Clear();
            structuralMembers.beamRow.Clear();
            structuralMembers.beamSchedule.Clear();

            structuralMembers.slabNames.Clear();
            structuralMembers.slab.Clear();
            structuralMembers.slabSchedule.Clear();

            structuralMembers.stairsNames.Clear();
            structuralMembers.stairs.Clear();
            parameters.stair.Clear();

            structuralMembers.roofNames.Clear();
            structuralMembers.roof.Clear();
            structuralMembers.roofHRS.Clear();

            structuralMembers.earthworkSolutions.Clear();

            structuralMembers.concreteWorkSolutionsF.Clear();
            structuralMembers.concreteWorkSolutionsC.Clear();
            structuralMembers.concreteWorkSolutionsBR.Clear();
            structuralMembers.concreteWorkSolutionsSL.Clear();
            structuralMembers.concreteWorkSolutionsSLSM.Clear();
            structuralMembers.concreteWorkSolutionsST.Clear();

            //footings
            structuralMembers.per_col.Clear();
            structuralMembers.per_wal.Clear();
            structuralMembers.footings_comps.Clear();// formworkFC - frameworkFC - formworkWF [FOOTINGS]
            //columns
            structuralMembers.col_area.Clear();
            structuralMembers.col_woods.Clear();
            structuralMembers.col_post.Clear();
            structuralMembers.col_scafV.Clear();
            structuralMembers.col_scafH.Clear();
            structuralMembers.col_scafD.Clear();
            //beams
            structuralMembers.beams_comps.Clear();//tieForm - tieFrame  --
            structuralMembers.beams_tiearea.Clear();//tie area
            structuralMembers.beams_gradarea.Clear();//beam area
            structuralMembers.beams_grade.Clear();//grade form
            structuralMembers.beams_gradeFrame.Clear();//grade frame
            structuralMembers.beams_vertical.Clear();//grade vertical
            structuralMembers.beams_horizontal.Clear();//grade horizontal
            structuralMembers.beams_RB.Clear();//roof form
            structuralMembers.beams_RBarea.Clear();//roof area
            structuralMembers.beams_RBframe.Clear();//roof frame
            structuralMembers.beams_RBV.Clear();//roof vertical
            structuralMembers.beams_RBH.Clear();//roof horizontal
            //slab
            structuralMembers.slab_area.Clear();
            structuralMembers.slab_form.Clear();
            structuralMembers.slab_scaf.Clear();
            //stairs
            structuralMembers.UstairsFORM.Clear();
            structuralMembers.UstairsFRAME.Clear();
            structuralMembers.UstairsSCAF.Clear();
            structuralMembers.LstairsFORM.Clear();
            structuralMembers.LstairsFRAME.Clear();
            structuralMembers.LstairsSCAF.Clear();
            structuralMembers.SstairsFORM.Clear();
            structuralMembers.SstairsFRAME.Clear();
            structuralMembers.SstairsSCAF.Clear();
            structuralMembers.UAREA.Clear();
            structuralMembers.LAREA.Clear();
            structuralMembers.SAREA.Clear();

            masonrysSolutionP1.Clear();
            masonrysSolutionP2.Clear();
            masonrysSolutionP3.Clear();

            structuralMembers.footingReinforcements.Clear();
            structuralMembers.wallFootingReinforcements.Clear();
            structuralMembers.Column_mainRebar.Clear();
            structuralMembers.Column_lateralRebar.Clear();
            structuralMembers.beamdias.Clear();
            structuralMembers.Beam_mainRebar.Clear();
            structuralMembers.Beam_stirRebar.Clear();
            structuralMembers.Beam_webRebar.Clear();
            structuralMembers.Slab_ongradeRebar.Clear();
            structuralMembers.Slab_suspendedRebar.Clear();
            structuralMembers.stairs_Rebar.Clear();

            structuralMembers.roofSolutions.Clear();

            sixhun.Clear();
            threehun.Clear();

            enamel.Clear();
            acrylic.Clear();
            latex.Clear();
            gloss.Clear();

            //Init variables for StructuralMember

            j = 0;
            while (!tokens[i].Equals("Parameters"))
            {
                initializeVariablesFromFile();
                //Set the floor
                Floor floor = new Floor(this, false);
                floors.Add(floor);
                estimationPanel.Controls.Add(floor);

                int floorMultiplier = 0;
                string floorName = "";

                j++;
                if (tokens[i].Equals("Floor-" + j))
                {
                    i++;
                    floorMultiplier = int.Parse(tokens[i]); i++;
                    floorName = tokens[i]; i++;
                }

                while (!tokens[i].Equals("COLUMNS")) //FOOTINGS
                {
                    i++;
                    if (!tokens[i].Equals("COLUMNS")){
                        TreeNode[] found = floor.treeView.Nodes.Find("footingParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);

                        if (newChild.Name[0] == 'F')
                            floor.footingCount++;
                        else
                            floor.wallFootingCount++;
                    }
                }

                while (!tokens[i].Equals("BEAMS")) //COLUMNS
                {
                    i++;
                    if (!tokens[i].Equals("BEAMS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("columnParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.columnCount++;
                    }
                }

                while(!tokens[i].Equals("SLABS")) //BEAMS
                {
                    i++;
                    if (!tokens[i].Equals("SLABS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("beamParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.beamCount++;
                    }
                }

                while (!tokens[i].Equals("STAIRS")) //SLABS
                {
                    i++;
                    if (!tokens[i].Equals("STAIRS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("slabParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.slabCount++;
                    }
                }

                while (!tokens[i].Equals("ROOFINGS")) //STAIRS
                {
                    i++;
                    if (!tokens[i].Equals("ROOFINGS"))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("stairsParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.stairsCount++;
                    }
                }

                while (!tokens[i].Equals("Parameters") && !tokens[i].Equals("Floor-" + (j + 1))) //ROOF
                {
                    i++;
                    if (!tokens[i].Equals("Parameters") && !tokens[i].Equals("Floor-" + (j + 1)))
                    {
                        TreeNode[] found = floor.treeView.Nodes.Find("roofParent", true);
                        TreeNode newChild = new TreeNode(tokens[i]); i++;
                        newChild.Name = tokens[i];

                        found[0].Nodes.Add(newChild);
                        floor.AdjustTreeViewHeight(floor.treeView);
                        floor.roofCount++;
                    }
                }

                floor.setValues(floorMultiplier, floorName);
            }

            //Save Floors -- END

            //Save to Parameters -- START
            i++;

            //Earthworks
            i++;
            parameters.earth_CF_FA = tokens[i]; i++;
            parameters.earth_CF_TH = tokens[i]; i++;
            parameters.earth_CF_TY = tokens[i]; i++;
            parameters.earth_CF_CF = tokens[i]; i++;
            parameters.earth_WF_FA = tokens[i]; i++;
            parameters.earth_WF_TH = tokens[i]; i++;
            parameters.earth_WF_TY = tokens[i]; i++;
            parameters.earth_WF_CF = tokens[i]; i++;
            parameters.earth_WTB_FA = tokens[i]; i++;
            parameters.earth_WTB_TH = tokens[i]; i++;
            parameters.earth_WTB_TY = tokens[i]; i++;
            parameters.earth_WTB_CF = tokens[i]; i++;
            parameters.earth_SG_AS = tokens[i]; i++;
            parameters.earth_SG_TS = tokens[i]; i++;
            parameters.earth_SG_TH = tokens[i]; i++;
            parameters.earth_SG_TY = tokens[i]; i++;
            parameters.earth_SG_CF = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Formworks"))
            {
                j++;
                if(tokens[i].Equals("Elevation-" + j))
                {
                    i++;
                    string[] toAdd = new string[4];
                    int k = 0;
                    while (!tokens[i].Equals("Elevation-" + (j + 1)) && !tokens[i].Equals("Formworks"))
                    {
                        toAdd[k] = (tokens[i]); i++; k++;
                    }
                    parameters.earth_elevations.Add(toAdd);
                }
            }

            //Formworks
            i++;
            parameters.form_SM_F_FL = tokens[i]; i++;
            parameters.form_SM_C_FL = tokens[i]; i++;
            parameters.form_SM_C_VS = tokens[i]; i++;
            parameters.form_SM_C_HB = tokens[i]; i++;
            parameters.form_SM_B_FL = tokens[i]; i++;
            parameters.form_SM_B_VS = tokens[i]; i++;
            parameters.form_SM_B_HB = tokens[i]; i++;
            parameters.form_SM_B_DB = tokens[i]; i++;
            parameters.form_SM_HS_VS = tokens[i]; i++;
            parameters.form_SM_ST_FL = tokens[i]; i++;
            parameters.form_SM_ST_VS = tokens[i]; i++;
            parameters.form_F_T = tokens[i]; i++;
            parameters.form_F_NU = tokens[i]; i++;
            parameters.form_F_N = tokens[i]; i++;

            //Concrete
            i++;
            parameters.conc_cmIsSelected[0] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_F_CG = tokens[i]; i++;
            parameters.conc_CM_F_GT = tokens[i]; i++;
            parameters.conc_CM_F_RM = tokens[i]; i++;
            parameters.conc_cmIsSelected[1] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_C_CG = tokens[i]; i++;
            parameters.conc_CM_C_GT = tokens[i]; i++;
            parameters.conc_CM_C_RM = tokens[i]; i++;
            parameters.conc_cmIsSelected[2] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_B_CG = tokens[i]; i++;
            parameters.conc_CM_B_GT = tokens[i]; i++;
            parameters.conc_CM_B_RM = tokens[i]; i++;
            parameters.conc_cmIsSelected[3] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_S_SOG_CG = tokens[i]; i++;
            parameters.conc_CM_S_SOG_GT = tokens[i]; i++;
            parameters.conc_CM_S_SOG_RM = tokens[i]; i++;
            parameters.conc_cmIsSelected[4] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_S_SS_CG = tokens[i]; i++;
            parameters.conc_CM_S_SS_GT = tokens[i]; i++;
            parameters.conc_CM_S_SS_RM = tokens[i]; i++;

            parameters.conc_CM_W_MEW_CM = tokens[i]; i++;
            parameters.conc_CM_W_MIW_CM = tokens[i]; i++;
            parameters.conc_CM_W_P_CM = tokens[i]; i++;
            parameters.conc_CM_W_P_PT = tokens[i]; i++;

            parameters.conc_cmIsSelected[5] = bool.Parse(tokens[i]); i++;
            parameters.conc_CM_ST_CG = tokens[i]; i++;
            parameters.conc_CM_ST_GT = tokens[i]; i++;
            parameters.conc_CM_ST_RM = tokens[i]; i++;

            parameters.conc_CC_F = tokens[i]; i++;
            parameters.conc_CC_SS = tokens[i]; i++;
            parameters.conc_CC_SG = tokens[i]; i++;
            parameters.conc_CC_BEE = tokens[i]; i++;
            parameters.conc_CC_BEW = tokens[i]; i++;
            parameters.conc_CC_CEE = tokens[i]; i++;
            parameters.conc_CC_CEW = tokens[i]; i++;

            //Reinforcements 
            i++; j = 0; i++;
            parameters.rein_LSL_TB_dt.Rows.Clear();
            while (!tokens[i].Equals("FCPrime-1") && !tokens[i].Equals("Compression_Bars"))
            {
                j++; 
                if (tokens[i].Equals("Compression_Bars"))
                {
                    break;
                }
                else if (tokens[i].Equals("Row-1"))
                {
                    parameters.rein_LSL_TB_dt.Columns.Clear();
                    int headerCount, temp;
                    temp = i; 
                    while (!tokens[temp].Equals("Row-" + (j + 1)) && !tokens[temp].Equals("FCPrime-1"))
                    {
                        temp++;
                    }
                    headerCount = temp - i;
                    for (int k = 1; k < headerCount; k++)
                    {
                        if (k == 0)
                        {
                            parameters.rein_LSL_TB_dt.Columns.Add("Bar Sizes");
                        }
                        else
                        {
                            parameters.rein_LSL_TB_dt.Columns.Add("LS" + k + " ƒ'c (MPa):");
                        }
                    }
                }

                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j) && !tokens[i].Equals("FCPrime-1"))
                {
                    i++;
                    while(!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("FCPrime-1"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }          
                    parameters.rein_LSL_TB_dt.Rows.Add(tempList.ToArray());
                }
            }
            j = 0;
            while (!tokens[i].Equals("Compression_Bars"))
            {
                j++;
                if (tokens[i].Equals("FCPrime-" + j))
                {
                    i++;
                    parameters.rein_LSL_TB_fc_list.Add(tokens[i]);
                }
                i++;
            }
            
            j = 0; i++;
            parameters.rein_LSL_CB_dt.Rows.Clear();
            while (!tokens[i].Equals("FCPrime-1") && !tokens[i].Equals("Main_Bars"))
            {
                j++;
                if (tokens[i].Equals("Main_Bars"))
                {
                    break;
                }
                else if (tokens[i].Equals("Row-1"))
                {
                    parameters.rein_LSL_CB_dt.Columns.Clear();
                    int headerCount, temp;
                    temp = i;
                    while (!tokens[temp].Equals("Row-" + (j + 1)) && !tokens[temp].Equals("FCPrime-1"))
                    {
                        temp++;
                    }
                    headerCount = temp - i;
                    for (int k = 1; k < headerCount; k++)
                    {
                        if (k == 0)
                        {
                            parameters.rein_LSL_CB_dt.Columns.Add("Bar Sizes");
                        }
                        else
                        {
                            parameters.rein_LSL_CB_dt.Columns.Add("LS" + k + " ƒ'c (MPa):");
                        }
                    }
                }

                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j) && !tokens[i].Equals("FCPrime-1"))
                {
                    i++;
                    while (!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("FCPrime-1"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }
                    parameters.rein_LSL_CB_dt.Rows.Add(tempList.ToArray());
                }
            }
            j = 0;
            while (!tokens[i].Equals("Main_Bars"))
            {
                j++;
                if (tokens[i].Equals("FCPrime-" + j))
                {
                    i++;
                    parameters.rein_LSL_CB_fc_list.Add(tokens[i]);
                }
                i++;
            }
            j = 0; i++; parameters.rein_BEH_MB_dt.Rows.Clear();
            while (!tokens[i].Equals("Stirrups_and_Ties"))
            {
                j++;
                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j))
                {
                    i++;
                    while (!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("Stirrups_and_Ties"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }
                    parameters.rein_BEH_MB_dt.Rows.Add(tempList.ToArray());
                }
            }
            j = 0; i++; parameters.rein_BEH_ST_dt.Rows.Clear();
            while (!tokens[i].Equals("Weight"))
            {
                j++;
                List<string> tempList = new List<string>();
                if (tokens[i].Equals("Row-" + j))
                {
                    i++;
                    while (!tokens[i].Equals("Row-" + (j + 1)) && !tokens[i].Equals("Weight"))
                    {
                        tempList.Add(tokens[i]);
                        i++;
                    }
                    parameters.rein_BEH_ST_dt.Rows.Add(tempList.ToArray());
                }
            }
            i += 3;
            parameters.rein_W_dt.Rows[0][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[1][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[2][1] = tokens[i];i += 3;
            parameters.rein_W_dt.Rows[3][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[4][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[5][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[6][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[7][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[8][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[9][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[10][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[11][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[12][1] = tokens[i]; i += 3;
            parameters.rein_W_dt.Rows[13][1] = tokens[i]; i++;

            parameters.rein_RG_C = tokens[i]; i++;
            parameters.rein_RG_CLT = tokens[i]; i++;
            parameters.rein_RG_F = tokens[i]; i++;
            parameters.rein_RG_B = tokens[i]; i++;
            parameters.rein_RG_BS = tokens[i]; i++;
            parameters.rein_RG_ST = tokens[i]; i++;
            parameters.rein_RG_W = tokens[i]; i++;
            parameters.rein_RG_SL = tokens[i]; i++;

            for (int col = 0; col < parameters.rein_mfIsSelected.GetLength(0); col++)
                for (int row = 0; row < parameters.rein_mfIsSelected.GetLength(1); row++)
                    { parameters.rein_mfIsSelected[col, row] = bool.Parse(tokens[i]); i++; }

            //Paint
            i++;
            parameters.paint_SCL = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Tiles"))
            {
                j++;
                if (tokens[i].Equals("Paint_Area-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1], tokens[i + 2] };
                    parameters.paint_Area.Add(toAdd);
                }
                i += 3;
            }

            //Tiles
            i++;
            parameters.tiles_FS = tokens[i]; i++;
            parameters.tiles_TG = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Masonry"))
            {
                j++;
                if (tokens[i].Equals("Tile_Area-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1], tokens[i + 2] };
                    parameters.tiles_Area.Add(toAdd);
                }
                i += 3; 
            }

            //Masonry
            i++;
            parameters.mason_CHB_EW = tokens[i]; i++;
            parameters.mason_CHB_IW = tokens[i]; i++;

            
            j = 0; //Exterior Wall
            while (!tokens[i].Equals("Exterior_Window-1") && !tokens[i].Equals("Exterior_Door-1") &&
                   !tokens[i].Equals("Interior_Wall-1") && !tokens[i].Equals("Interior_Window-1") && 
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Exterior_Wall-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_exteriorWall.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Exterior Window
            while (!tokens[i].Equals("Exterior_Door-1") &&
                   !tokens[i].Equals("Interior_Wall-1") && !tokens[i].Equals("Interior_Window-1") &&
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Exterior_Window-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_exteriorWindow.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Exterior Door
            while (!tokens[i].Equals("Interior_Wall-1") && !tokens[i].Equals("Interior_Window-1") &&
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Exterior_Door-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_exteriorDoor.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Interior Wall
            while (!tokens[i].Equals("Interior_Window-1") &&
                   !tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Interior_Wall-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_interiorWall.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Interior Window
            while (!tokens[i].Equals("Interior_Door-1") && !tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Interior_Window-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_interiorWindow.Add(toAdd);
                }
                i += 2;
            }
            j = 0; //Interior Window
            while (!tokens[i].Equals("END"))
            {
                j++;
                if (tokens[i].Equals("Interior_Door-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1] };
                    parameters.mason_interiorDoor.Add(toAdd);
                }
                i += 2;
            }
            i++; //END
            parameters.mason_RTW_VS = tokens[i]; i++;
            parameters.mason_RTW_HSL = tokens[i]; i++;
            parameters.mason_RTW_RG = tokens[i]; i++;
            parameters.mason_RTW_BD = tokens[i]; i++;
            parameters.mason_RTW_RL = tokens[i]; i++;
            parameters.mason_RTW_LTW = tokens[i]; i++;


            //Stairs
            i++;
            j = 0;
            int l = 0;
            while (!tokens[i].Equals("Labor_and_Equipment"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Labor_and_Equipment"))
                {
                    List<StairParameterUserControl> floor = new List<StairParameterUserControl>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Stair-" + (l + 1)) && !tokens[i].Equals("Labor_and_Equipment") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        i++;
                        string type = tokens[i]; i++;
                        StairParameterUserControl content = new StairParameterUserControl(type); 
                        List<string> toAdd = new List<string>();
                        while (!tokens[i].Equals("Stair-" + (l + 2)) && !tokens[i].Equals("Labor_and_Equipment") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(tokens[i]); i++;
                        }

                        if (type.Equals("Straight Stairs"))
                        {
                            content.setStraightStairsValues(toAdd[0], toAdd[1], toAdd[2], toAdd[3], toAdd[4], toAdd[5], toAdd[6], toAdd[7], toAdd[8], toAdd[9]);
                        } 
                        else if (type.Equals("U-Stairs"))
                        {
                            content.setUStairsValues(toAdd[0], toAdd[1], toAdd[2], toAdd[3], toAdd[4], toAdd[5], toAdd[6], toAdd[7], toAdd[8], toAdd[9], toAdd[10], toAdd[11], toAdd[12], toAdd[13]);
                        }
                        else
                        {
                            content.setLStairsValues(toAdd[0], toAdd[1], toAdd[2], toAdd[3], toAdd[4], toAdd[5], toAdd[6], toAdd[7], toAdd[8], toAdd[9], toAdd[10], toAdd[11], toAdd[12], toAdd[13]);
                        }
                        l++;
                        floor.Add(content);
                    }
                    parameters.stair.Add(floor);
                    j++;
                }
            }

            //Labor and Equipment
            i++;
            parameters.labor_RD = tokens[i]; i++;
            j = 0;
            while (!tokens[i].Equals("Equipment-1") && !tokens[i].Equals("Misc"))
            {
                j++;
                if (tokens[i].Equals("Man_Power-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1], tokens[i + 2], tokens[i + 3], tokens[i + 4] };
                    parameters.labor_MP.Add(toAdd);
                }
                i += 5;
            }
            j = 0;
            Console.WriteLine(tokens[i]);
            while (!tokens[i].Equals("Misc"))
            {
                j++;
                if (tokens[i].Equals("Equipment-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1], tokens[i + 2], tokens[i + 3], tokens[i + 4] };
                    parameters.labor_EQP.Add(toAdd);
                }
                i += 5;
            }

            //Misc
            i++;
            j = 0;
            while (!tokens[i].Equals("Price-List"))
            {
                j++;
                if (tokens[i].Equals("Custom_Item-" + j))
                {
                    i++;
                    string[] toAdd = { tokens[i], tokens[i + 1], tokens[i + 2], tokens[i + 3] };
                    parameters.misc_CustomItems.Add(toAdd);
                }
                i += 4;
            }

            //Price-List 
            i++; i++;

            //Checklist
            earthworksChecklist[0] = bool.Parse(tokens[i]); i++;
            earthworksChecklist[1] = bool.Parse(tokens[i]); i++;
            earthworksChecklist[2] = bool.Parse(tokens[i]); i++;
            earthworksChecklist[3] = bool.Parse(tokens[i]); i++;
            earthworksChecklist[4] = bool.Parse(tokens[i]); i++;
            earthworksChecklist[5] = bool.Parse(tokens[i]); i++;
            concreteChecklist[0] = bool.Parse(tokens[i]); i++;
            concreteChecklist[1] = bool.Parse(tokens[i]); i++;
            concreteChecklist[2] = bool.Parse(tokens[i]); i++;
            concreteChecklist[3] = bool.Parse(tokens[i]); i++;
            concreteChecklist[4] = bool.Parse(tokens[i]); i++;
            concreteChecklist[5] = bool.Parse(tokens[i]); i++;
            formworksChecklist[0] = bool.Parse(tokens[i]); i++;
            formworksChecklist[1] = bool.Parse(tokens[i]); i++;
            formworksChecklist[2] = bool.Parse(tokens[i]); i++;
            formworksChecklist[3] = bool.Parse(tokens[i]); i++;
            formworksChecklist[4] = bool.Parse(tokens[i]); i++;
            formworksChecklist[5] = bool.Parse(tokens[i]); i++;
            masonryChecklist[0] = bool.Parse(tokens[i]); i++;
            masonryChecklist[1] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[0] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[1] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[2] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[3] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[4] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[5] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[6] = bool.Parse(tokens[i]); i++;
            rebarsChecklist[7] = bool.Parse(tokens[i]); i++;
            roofingsChecklist[0] = bool.Parse(tokens[i]); i++;
            roofingsChecklist[1] = bool.Parse(tokens[i]); i++;
            roofingsChecklist[2] = bool.Parse(tokens[i]); i++;
            tilesChecklist[0] = bool.Parse(tokens[i]); i++;
            tilesChecklist[1] = bool.Parse(tokens[i]); i++;
            paintsChecklist[0] = bool.Parse(tokens[i]); i++;
            paintsChecklist[1] = bool.Parse(tokens[i]); i++;
            paintsChecklist[2] = bool.Parse(tokens[i]); i++;
            paintsChecklist[3] = bool.Parse(tokens[i]); i++;

            //Factor of Safety for Price
            fos_Cement = tokens[i]; i++;
            fos_Sand = tokens[i]; i++;
            fos_Gravel = tokens[i]; i++;
            fos_RMC = tokens[i]; i++;
            fos_CHB = tokens[i]; i++;
            fos_Tiles = tokens[i]; i++;
            fos_LC_Type = tokens[i]; i++;
            fos_LC_Percentage = tokens[i]; i++;

            //1
            i++;
            parameters.price_CommonMaterials["Cyclone Wire (Gauge#10, 2”x2”, 3ft x 10m) [ROLL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Gasket (6mm thk, 1m x 1m) [ROLL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Acetylene Gas [CYL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Oxygen Gas [CYL]"] = 
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Rugby [CANS]"] = 
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Vulca Seal [LTR]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Broom, Soft [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Concrete Epoxy [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Concrete Patching Compound [KGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["GI Wire (no. 16) [ROLL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["GI Wire (no. 12) [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["GI Wire (no. 16) [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Non-Shrink Grout [BAGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["40 kg Portland Cement [BAGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Sand [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Rivets 1/8” x ½” [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Rivets 1-1/2” x ½” [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Rope (⌀ 1/2”) [MTRS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Tape, Caution [ROLLS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Tile Grout (2KG)  [BAGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Tiles, Floor (600 x 600) [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Tiles, Wall (300 x 300) [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Broom Stick [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Chalk Stone [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Sandpaper (#100) [MTRS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Sandpaper (#50) [MTRS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Common Nail [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Concrete Nail [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Putty, Multipurpose [PAIL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["Tie Wire (No. #16) [25kg/Roll]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["25KG Tile Adhesive (Regular) [BAGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["25 KG Tile Adhesive (Heavy duty) [BAGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["CHB 4” (0.10 x 0.20 x 0.40) [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["CHB 6” (0.15 x 0.20 x 0.40) [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_CommonMaterials["CHB 8” (0.20 x 0.20 x 0.40) [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //2
            i++;
            parameters.price_PaintAndCoating["Acrylic Emulsion [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Concrete Epoxy Injection [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Concrete Primer & Sealer [PAIL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Epopatch, Base and Hardener [SETS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Lacquer Thinner [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint Brush, Bamboo 1-1/2” [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint, Acrylic 1 [GAL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint, Epoxy Enamel White [GAL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint, Epoxy Floor Coating [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint, Epoxy Primer Gray [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint, Epoxy Reducer [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint Latex Gloss [GAL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint Enamel [GAL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Paint, Semi-Gloss [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Putty, Masonry [PAIL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Rust Converter [GAL]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Skim Coat [BAGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Underwater Epoxy [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PaintAndCoating["Concrete neutralizer [GALS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //3
            i++;
            parameters.price_WeldingRod["Stainless Welding Rod 308 (3.2mm) [KGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [KGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_WeldingRod["Welding Rod 6011 (3.2mm) [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [KGS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_WeldingRod["Welding Rod 6013 (3.2mm) [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //4
            i++;
            parameters.price_PersonalProtectiveEquipment["Chemical Gloves PAIR Cotton Gloves [PAIRS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Cotton Gloves [PAIRS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Dust Mask N95 [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Gloves, Orange Palm [PAIRS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Hard Hat w/ Headgear and chin strap [20 SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Overall with reflector [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Oxy-Acetylene Cutting Outfit [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["PVC Apron  [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Respirator mask w/ Cartridge [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Respirator, Filter Cartridge [PACK]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Safety Goggles [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Safety Rubber boots [PAIR]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Safety Shoes  [PAIR]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Safety Vest [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Welding Mask  [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Welding Apron [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_PersonalProtectiveEquipment["Welding Mask, Auto Darkening [SETS]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //5
            i++;
            parameters.price_Tools["Adjustable Wrench Set 4”— 24” [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Baby Roller (Cotton) 4” [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Ball Hammer [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Bench Vise [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Blade Cutter [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Camlock (Male & Female Set) 50mm DIA [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Chipping Gun [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Combination Wrench Set 6mm — 32mm [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Cut-off Wheel ⌀ 16” [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Cutting Disc ⌀ 4” [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Cutting Disc ⌀ 7” [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Drill Bit [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Electrical Plier [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Grinder, Angle 4” [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Grinder, Angle 7” [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Grinder, Baby [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Grinder, Mother [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Grinding Disc ⌀ 4” [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Grinding Disc ⌀ 7” [BOX]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Heat Gun [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Ladder (A-Type), 6h, Aluminum [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Level Bar 24” [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Paint Brush 4” [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Paint Brush 2” [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Portable Axial Fan Blower  ⌀  8” [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Power Ratchet [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Rivet Gun / Riveter [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Roller Brush 7” [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Screwdriver, Flat [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Screwdriver, Philip [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Shovel, Pointed [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Snap Ring Plier [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Socket Wrench Set 19mm — 50mm [SET]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Speed Cutter [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Steel Brush [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Test Light [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Test Wrench [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Torque Wrench [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Vise Grip [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Tools["Welding Machine (Portable) 12.3 kVA(20-300A) [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //6
            i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 28 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 14 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 7 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3000PSI (20.7 Mpa) @ 3 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 28 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 14 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 7 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 3500PSI (24.1 Mpa) @ 3 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 28 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4000PSI (27.6 Mpa) @ 14 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 4500PSI (31 Mpa) @ 28 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ReadyMixConcrete["Ready Mix Concrete, 5000PSI (34.5 Mpa) @ 28 Days [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //7
            i++;
            parameters.price_Gravel["GRAVEL G1 [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Gravel["GRAVEL G2 [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Gravel["GRAVEL G1- ½” [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Gravel["GRAVEL G2- ½” [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Gravel["GRAVEL ¾” [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //8
            i++;
            parameters.price_FormworksAndLumber["Lumber [2”x 2” x 8']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x 2” x 10']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x 2” x 12']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x 3”x 8']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x 3”x 10']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x 3”x 12']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x4”x 8']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x4”x 10']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["Lumber [2”x4”x 12']"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["PLYWOOD 1/2” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["PLYWOOD 3/4” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["PLYWOOD 1/4” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["PLYWOOD 1/8”[1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["ECOBOARD 1/2”[1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["ECOBOARD 3/4” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["ECOBOARD 1/4” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["ECOBOARD 1/8” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["PHENOLIC BOARD- 1/2” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_FormworksAndLumber["PHENOLIC BOARD- 3/4” [1.22m x 2.44m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //9
            i++;
            parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 0.7mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 0.7mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 0.9mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.0mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.0mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.0mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (75mm x 50mm x 1.2mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (100mm x 50mm x 1.2mm thick) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["C- Purlins (150mm x 50mm x 1.2mm thick ) [6 m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["Corrugated G.I Sheet, Gauge 26 (0.551mmx2.44 mm) [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["Plain G.I Sheet, Gauge 24 (4ft x 8 ft) [UNIT]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["G.I. Roof Nails [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["G.I Rivets [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["G.I Washers [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RoofMaterials["Umbrella Nails [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //10
            i++;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 20mm x 20mm x 1.0mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.0mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.0mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.0mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.0mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //11
            i++;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.2mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.2mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.2mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.2mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.2mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.2mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p2mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.2mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //12
            i++;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 25mm x 25mm x 1.5mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 32mm x 32mm x 1.5mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 25mm x 1.5mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 50mm x 50mm x 1.5mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 75mm x 50mm x 1.5mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 100mm x 50mm x 1.5mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_TubularSteel1p5mm["B.I. (Black Iron) Tubular 150mm x 50mm x 1.5mm thick [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //13
            i++;
            parameters.price_Embankment["Common Borrow [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Embankment["Selected Borrow [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Embankment["Mixed Sand & Gravel [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Embankment["Rock [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //14
            i++;
            parameters.price_RebarGrade33["Compression Coupler GRADE 33 [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Tension Coupler GRADE 33 [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀10mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀12mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀16mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀20mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade33["Rebar GRADE 33 (⌀25mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //15
            i++;
            parameters.price_RebarGrade40["Compression Coupler GRADE 40 [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Tension Coupler GRADE 40 [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀10mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀12mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀16mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀20mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade40["Rebar GRADE 40 (⌀25mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //16
            i++;
            parameters.price_RebarGrade60["Compression Coupler GRADE 60 [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Tension Coupler GRADE 60 [PC]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀10mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀12mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀16mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀20mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀25mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀28mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀32mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀36mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀40mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [6m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [7.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [9m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [10.5m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_RebarGrade60["Rebar GRADE 60 (⌀50mm) [12m]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //17
            i++;
            parameters.price_LaborRate_Earthworks["Excavation [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Earthworks["Backfilling and Compaction [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Earthworks["Grading and Compaction [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Earthworks["Gravel Bedding and Compaction [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Earthworks["Soil Poisoning [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //18
            i++;
            parameters.price_LaborRate_Concreting["FOOTING [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Concreting["WALL FOOTING [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Concreting["COLUMN [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Concreting["STAIRS [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Concreting["BEAM [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Concreting["SUSPENDED SLAB [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Concreting["SLAB ON GRADE [m3]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //19
            i++;
            parameters.price_LaborRate_Formworks["FOOTING [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Formworks["WALL FOOTING [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Formworks["COLUMN [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Formworks["STAIRS [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Formworks["BEAM [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Formworks["SUSPENDED SLAB [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //20
            i++;
            parameters.price_LaborRate_Rebar["FOOTING [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["WALL FOOTING [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["COLUMN [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["STAIRS [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["BEAM [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["FOOTING TIE BEAM [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["SLAB ON GRADE [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["SUSPENDED SLAB [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_LaborRate_Rebar["WALLS [KG]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //21
            i++;
            parameters.price_LaborRate_Paint["PAINTER [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //22
            i++;
            parameters.price_LaborRate_Tiles["TILES [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //23
            i++;
            parameters.price_LaborRate_Masonry["MASONRY [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //24
            i++;
            parameters.price_LaborRate_Roofings["ROOFINGS [m2]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //25.1
            i++;
            parameters.price_ManpowerM["Foreman [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Fitter [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Welder [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Electrician [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Carpenter [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Painter [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Mason [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Driver [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Eqpt Operator [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerM["Helper [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //25.2
            i++;
            parameters.price_ManpowerP["Foreman [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Fitter [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Welder [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Electrician [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Carpenter [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Painter [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Mason [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Driver [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Eqpt Operator [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_ManpowerP["Helper [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //26
            i++;
            parameters.price_Equipment["Crawler Loader (80kW/ 1.5 - 2.0 cu.m) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Crawler Dozer (125kW) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Wheel Loader (20 - 3.0 Cum) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Backhoe Crawler (0.75- 1.0 cum) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Backhoe / Pavement Breaker (1.5 cum) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Motor Grader (90 - 100 kW)  [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Pneumatic Tire Roller (20 - 24 Mr) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Vibratory Drum Roller (10 - 14MT) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Dump Truck (8.0 -12.0 cu.m) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Cargo Truck Small (5 - 8 MT) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Cargo Truck Small (10 -15 MT) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Transit Mixer (5.0 -8.0 cum) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Concrete Batching Plant (80 -100 cu.m/hr) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Concrete Trimmer/Slipform Paver (1 meter width) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Asphalt Distributor Truck (2500 - 3500 Gallons) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Asphalt Finisher (3-meter width) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Truck with Boom, small (6-10 MT) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Truck with Boom, small (12 - 15 MT) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Crawler Crane (21-25 MT) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Diesel Pile Hammer [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Vibratory Pile Driver [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Bagger Mixer (1-2 Bags) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Concrete Vibrator [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Air Compressor  (Small) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Air Compressor (Big) [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Bar Cutter [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Bar Bender [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Jack Hammer [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Tamping Rammer [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Welding Machine, Portable, 300A [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Welding Machine, 600A [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Generator Set 15-25kVA [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Generator Set 50 kVA [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Sump Pump (Dewatering) 0.75  –2HP- [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Sump Pump (Dewatering) 5HP [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            parameters.price_Equipment["Road Paint Stripper [hr]"] =
                double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            AdjustPriceView();

            //BOQ
            i++;
            summ_P_bx.Text = tokens[i]; i++;
            summ_L_bx.Text = tokens[i]; i++;
            summ_O_bx.Text = tokens[i]; i++;
            summ_S_bx.Text = tokens[i]; i++;
            
            //Save to Parameters -- END -- Structural-Members (for 24)

            //Save to Structural-Members -- START
            i++;

            //Footings
            i++;

            int floorIndex = 0;
            l = 0;
            j = 0;

            i++; //Only Ground Floor
            while (!tokens[i].Equals("Columns"))
            {
                if (tokens[i].Equals("Column-Footing-" + (j + 1)) && !tokens[i].Equals("Columns"))
                {
                    List<string> toAdd = new List<string>();
                    i++; 

                    structuralMembers.footingColumnNames.Add(tokens[i]); i++;

                    while (!tokens[i].Equals("Column-Footing-" + (j + 2)) &&
                        !tokens[i].Equals("Wall-Footing-" + (l + 1)) && !tokens[i].Equals("Columns"))
                    {
                        toAdd.Add(tokens[i]); i++;
                    }
                    structuralMembers.footingsColumn[0].Add(toAdd);
                    //compute.AddFootingWorks(this, j, l, true);
                    j++;
                }
                else if (tokens[i].Equals("Wall-Footing-" + (l + 1)) && !tokens[i].Equals("Columns"))
                {
                    List<string> toAdd = new List<string>();
                    i++; 

                    structuralMembers.footingWallNames.Add(tokens[i]); i++;

                    while (!tokens[i].Equals("Wall-Footing-" + (l + 2)) && !tokens[i].Equals("Columns"))
                    {
                        toAdd.Add(tokens[i]); i++;
                    }
                    structuralMembers.footingsWall[0].Add(toAdd);
                    //compute.AddFootingWorks(this, j, l, false);
                    l++;
                }
            }

            //Columns 
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Beams"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Beams"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<string>> floor1 = new List<List<string>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Column-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        List<string> ltMember = new List<string>();
                        List<string> sMember = new List<string>();
                        while (!tokens[i].Equals("Lateral-Ties") && !tokens[i].Equals("Spacing") && 
                               !tokens[i].Equals("Column-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) && 
                               !tokens[i].Equals("Beams"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        if (tokens[i].Equals("Lateral-Ties"))
                        {
                            i++;
                            while (!tokens[i].Equals("Spacing") &&
                                   !tokens[i].Equals("Column-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                                   !tokens[i].Equals("Beams"))
                            {
                                ltMember.Add(tokens[i]); i++;
                            }
                        }
                        if (tokens[i].Equals("Spacing"))
                        {
                            i++;
                            while (!tokens[i].Equals("Column-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                                   !tokens[i].Equals("Beams"))  
                            {
                                sMember.Add(tokens[i]); i++;
                            }
                        }
                        floor.Add(member);
                        floor1.Add(ltMember);
                        floor2.Add(sMember);
                        l++;
                    }
                    structuralMembers.column.Add(floor);
                    structuralMembers.columnLateralTies.Add(floor1);
                    structuralMembers.columnSpacing.Add(floor2);
                    structuralMembers.columnNames.Add(name);
                    j++;
                }
            }
            
            //Beams 
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Slabs"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Slabs"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<List<string>>> floor1 = new List<List<List<string>>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Beam-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        List<List<string>> brMember = new List<List<string>>();
                        while (!tokens[i].Equals("Beam-Rows") && !tokens[i].Equals("Beam-Schedules") &&
                               !tokens[i].Equals("Beam-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Slabs"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        if (tokens[i].Equals("Beam-Rows"))
                        {
                            i++;
                            while (!tokens[i].Equals("Beam-Schedules") &&
                               !tokens[i].Equals("Beam-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Slabs"))
                            {
                                List<string> toAdd = new List<string>();
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                brMember.Add(toAdd);
                            }
                        }
                        l++;
                        floor.Add(member);
                        floor1.Add(brMember);
                    }
                    if (tokens[i].Equals("Beam-Schedules"))
                    {
                        i++;
                        while (!tokens[i].Equals("Beam-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Slabs"))
                        {
                            List<string> toAdd = new List<string>();
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            toAdd.Add(tokens[i]); i++;
                            floor2.Add(toAdd);
                        }
                    }
                    structuralMembers.beam.Add(floor);
                    structuralMembers.beamRow.Add(floor1);
                    structuralMembers.beamSchedule.Add(floor2);
                    structuralMembers.beamNames.Add(name);

                    j++;
                }
            }

            //Slabs
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Stairs"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Stairs"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Slab-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        while (!tokens[i].Equals("Slab-Schedules") &&
                               !tokens[i].Equals("Slab-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                               !tokens[i].Equals("Stairs"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(member);
                    }
                    
                    if (j > 0)
                    {
                        if (tokens[i].Equals("Slab-Schedules"))
                        {
                            i++;
                            while (!tokens[i].Equals("Slab-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) &&
                                   !tokens[i].Equals("Stairs"))
                            {
                                List<string> toAdd = new List<string>();
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                toAdd.Add(tokens[i]); i++;
                                floor2.Add(toAdd);
                            }
                        }
                    }
                    
                    structuralMembers.slab.Add(floor);
                    if (j > 0)
                    {
                        structuralMembers.slabSchedule.Add(floor2);
                    }
                    structuralMembers.slabNames.Add(name);
                    j++;
                }
            }

            //Stairs
            i++;
            j = 0;
            l = 0;

            while (!tokens[i].Equals("Roof"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Roof"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while(tokens[i].Equals("Stair-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        while (!tokens[i].Equals("Stair-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("Roof"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(member);
                    }
                    structuralMembers.stairs.Add(floor);
                    structuralMembers.stairsNames.Add(name);
                    j++;
                }
            }

            //Roof
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("Solutions"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Solutions"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    List<List<string>> floor2 = new List<List<string>>();
                    List<string> name = new List<string>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("Roof-" + (l + 1)))
                    {
                        i++;
                        name.Add(tokens[i]); i++;
                        List<string> member = new List<string>();
                        List<string> hrsMember = new List<string>();
                        while (!tokens[i].Equals("Roof-Sheet"))
                        {
                            member.Add(tokens[i]); i++;
                        }
                        i++;
                        while (!tokens[i].Equals("Roof-" + (l + 2)) && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("Solutions"))
                        {
                            hrsMember.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(member);
                        floor2.Add(hrsMember);
                    }
                    structuralMembers.roof.Add(floor);
                    structuralMembers.roofHRS.Add(floor2);
                    structuralMembers.roofNames.Add(name);
                    j++;
                }
            }
            //Structural Members -- END

            //Solutions -- START
            i++;
            //Earthworks
            i++;

            j = 0;
            i++; //Only Ground Floor
            while (!tokens[i].Equals("Extra-Earthworks"))
            {
                List<double> toAdd = new List<double>();
                i++;

                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                structuralMembers.earthworkSolutions.Add(toAdd);
                
                j++;
            }

            while (!tokens[i].Equals("Concrete-Works"))
            {
                i++;
                int k = 0;
                while (!tokens[i].Equals("Concrete-Works"))
                {
                    structuralMembers.extraEarthworkSolutions[k] = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++; k++;
                }
            }

            //Concrete Works
            i++;

            //Concrete Footing
            i++;
            j = 0;
            while (!tokens[i].Equals("ConcreteC")) 
            {
                if (tokens[i].Equals("concreteF-" + (j + 1)) && !tokens[i].Equals("ConcreteC"))
                {
                    List<double> toAdd = new List<double>();
                    i++;

                    while (!tokens[i].Equals("concreteF-" + (j + 2)) && !tokens[i].Equals("ConcreteC"))
                    {
                        toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                    }

                    structuralMembers.concreteWorkSolutionsF.Add(toAdd);
                    j++;
                }
            }

            //Concrete Column
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("ConcreteBR"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("ConcreteBR"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("concreteC-" + (l + 1)) && !tokens[i].Equals("ConcreteBR") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteC-" + (l + 2)) && !tokens[i].Equals("ConcreteBR") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsC.Add(floor);
                    j++;
                }
            }

            //Concrete Beam
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("ConcreteSL"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("ConcreteSL"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("concreteBR-" + (l + 1)) && !tokens[i].Equals("ConcreteSL") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteBR-" + (l + 2)) && !tokens[i].Equals("ConcreteSL") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsBR.Add(floor);
                    j++;
                }
            }

            //Concrete Slab
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("ConcreteST"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("ConcreteST"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("concreteSL-" + (l + 1)) && !tokens[i].Equals("ConcreteST") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteSL-" + (l + 2)) && !tokens[i].Equals("ConcreteST") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsSL.Add(floor);
                    j++;
                }
            }

            //Concrete Stairs
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("Totalities"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Totalities"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("concreteST-" + (l + 1)) && !tokens[i].Equals("Totalities") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("concreteST-" + (l + 2)) && !tokens[i].Equals("Totalities") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.concreteWorkSolutionsST.Add(floor);
                    j++;
                }
            }

            //Solutions -- END

            //Totalities -- START
            i++;

            //Earthworks
            i++;
            //Totalities
            excavation_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            backfillingAndCompaction_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gradingAndCompaction_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gravelBedding_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            soilPoisoning_Total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            //Cost
            excavation_CostL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            backfillingAndCompaction_CostL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gradingAndCompaction_CostL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gravelBedding_CostM = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gravelBedding_CostL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            gravelBedding_CostTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            soilPoisoning_CostM = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            earthworks_CostTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //3.0 - Formworks
            i++;
            //Footings
            i++;
            while (!tokens[i].Equals("per_wal"))
            {
                structuralMembers.per_col.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("footings_comps"))
            {
                structuralMembers.per_wal.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("col_area"))
            {
                structuralMembers.footings_comps.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            //Columns
            i++;
            while (!tokens[i].Equals("col_woods"))
            {
                structuralMembers.col_area.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("col_post"))
            {
                structuralMembers.col_woods.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("col_scafV"))
            {
                structuralMembers.col_post.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("col_scafH"))
            {
                structuralMembers.col_scafV.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("col_scafD"))
            {
                structuralMembers.col_scafH.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_comps"))
            {
                structuralMembers.col_scafD.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            //Beams
            i++;
            while (!tokens[i].Equals("beams_tiearea"))
            {
                structuralMembers.beams_comps.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_gradarea"))
            {
                structuralMembers.beams_tiearea.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_grade"))
            {
                structuralMembers.beams_gradarea.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_gradeFrame"))
            {
                structuralMembers.beams_grade.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_vertical"))
            {
                structuralMembers.beams_gradeFrame.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_horizontal"))
            {
                structuralMembers.beams_vertical.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_RB"))
            {
                structuralMembers.beams_horizontal.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_RBarea"))
            {
                structuralMembers.beams_RB.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_RBframe"))
            {
                structuralMembers.beams_RBarea.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_RBV"))
            {
                structuralMembers.beams_RBframe.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("beams_RBH"))
            {
                structuralMembers.beams_RBV.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("slab_area"))
            {
                structuralMembers.beams_RBH.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            //Slabs
            i++;
            while (!tokens[i].Equals("slab_form"))
            {
                structuralMembers.slab_area.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("slab_scaf"))
            {
                structuralMembers.slab_form.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("UstairsFORM"))
            {
                structuralMembers.slab_scaf.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            //Stairs
            i++;
            while (!tokens[i].Equals("UstairsFRAME"))
            {
                structuralMembers.UstairsFORM.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("UstairsSCAF"))
            {
                structuralMembers.UstairsFRAME.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("LstairsFORM"))
            {
                structuralMembers.UstairsSCAF.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("LstairsFRAME"))
            {
                structuralMembers.LstairsFORM.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("LstairsSCAF"))
            {
                structuralMembers.LstairsFRAME.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("SstairsFORM"))
            {
                structuralMembers.LstairsSCAF.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("SstairsFRAME"))
            {
                structuralMembers.SstairsFORM.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("SstairsSCAF"))
            {
                structuralMembers.SstairsFRAME.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("UAREA"))
            {
                structuralMembers.SstairsSCAF.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("LAREA"))
            {
                structuralMembers.UAREA.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("SAREA"))
            {
                structuralMembers.LAREA.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("Masonry"))
            {
                structuralMembers.SAREA.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            //*/
            //4.0 - Masonry
            i++;
            //Totalities
            i++;
            while (!tokens[i].Equals("SolutionP2"))
            {
                masonrysSolutionP1.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("SolutionP3"))
            {
                masonrysSolutionP2.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("InteriorAndExterior"))
            {
                masonrysSolutionP3.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            extCHBdimension = tokens[i]; i++;
            intCHBdimension = tokens[i]; i++;
            //Cost
            exterior_UnitM = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            exterior_CostM = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            exterior_CostL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            exterior_costTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            interior_UnitM = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            interior_CostM = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            interior_CostL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            interior_costTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            masonMCost_total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            masonLCost_total = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            mason_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //5.0 - Rebars
            //Totalities
            i++;
            //Footing
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("RebarsWF"))
            {
                if (tokens[i].Equals("member-" + (j + 1)) && !tokens[i].Equals("RebarsWF"))
                {
                    List<List<double>> member = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("row-" + (l + 1)) && !tokens[i].Equals("RebarsWF") && !tokens[i].Equals("member-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("row-" + (l + 2)) && !tokens[i].Equals("RebarsWF") && !tokens[i].Equals("member-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        member.Add(toAdd);
                    }
                    structuralMembers.footingReinforcements.Add(member);
                    j++;
                }
            }
            //Wall Footing
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("RebarsCM"))
            {
                if (tokens[i].Equals("member-" + (j + 1)) && !tokens[i].Equals("RebarsCM"))
                {
                    List<List<double>> member = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("row-" + (l + 1)) && !tokens[i].Equals("RebarsCM") && !tokens[i].Equals("member-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("row-" + (l + 2)) && !tokens[i].Equals("RebarsCM") && !tokens[i].Equals("member-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        member.Add(toAdd);
                    }
                    structuralMembers.wallFootingReinforcements.Add(member);
                    j++;
                }
            }
            //Column Main
            i++;
            structuralMembers.totalweightkgm_Colmain = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("RebarsCT"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsCT"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsCT") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<string> toAdd = new List<string>();
                        i++;

                        while (!tokens[i].Equals("member-" + (l + 2)) && !tokens[i].Equals("RebarsCT") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.Column_mainRebar.Add(floor);
                    j++;
                }
            }
            //Column Lateral Ties
            i++;
            structuralMembers.totalweightkgm_Colties = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            j = 0;
            l = 0;
            int m = 0;
            while (!tokens[i].Equals("RebarsBM1"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsBM1"))
                {
                    List<List<List<string>>> floor = new List<List<List<string>>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsBM1") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<List<string>> member = new List<List<string>>();
                        i++;
                        m = 0;
                        while (tokens[i].Equals("row-" + (m + 1)) && !tokens[i].Equals("RebarsBM1") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                        {
                            List<string> row = new List<string>();
                            i++;

                            while (!tokens[i].Equals("row-" + (m + 2)) && !tokens[i].Equals("RebarsBM1") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                            {
                                row.Add(tokens[i]); i++;
                            }
                            m++;
                            member.Add(row);
                        }
                        l++;
                        floor.Add(member);
                    }
                    structuralMembers.Column_lateralRebar.Add(floor);
                    j++;
                }
            }
            //Beam Main 1
            i++;
            structuralMembers.totalweightkgm_main = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("RebarsBM2"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsBM2"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsBM2") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<string> toAdd = new List<string>();
                        i++;

                        while (!tokens[i].Equals("member-" + (l + 2)) && !tokens[i].Equals("RebarsBM2") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.beamdias.Add(floor);
                    j++;
                }
            }
            //Beam Main 2
            i++;
            j = 0;
            l = 0;
            m = 0;
            while (!tokens[i].Equals("RebarsBS"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsBS"))
                {
                    List<List<List<string>>> floor = new List<List<List<string>>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsBS") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<List<string>> member = new List<List<string>>();
                        i++;
                        m = 0;
                        while (tokens[i].Equals("row-" + (m + 1)) && !tokens[i].Equals("RebarsBS") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                        {
                            List<string> row = new List<string>();
                            i++;

                            while (!tokens[i].Equals("row-" + (m + 2)) && !tokens[i].Equals("RebarsBS") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                            {
                                row.Add(tokens[i]); i++;
                            }
                            m++;
                            member.Add(row);
                        }
                        l++;
                        floor.Add(member);
                    }
                    structuralMembers.Beam_mainRebar.Add(floor);
                    j++;
                }
            }
            //Beam Stirrups
            i++;
            structuralMembers.totalweightkgm_stir = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            j = 0;
            l = 0;
            m = 0;
            while (!tokens[i].Equals("RebarsBW"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsBW"))
                {
                    List<List<List<string>>> floor = new List<List<List<string>>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsBW") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<List<string>> member = new List<List<string>>();
                        i++;
                        m = 0;
                        while (tokens[i].Equals("row-" + (m + 1)) && !tokens[i].Equals("RebarsBW") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                        {
                            List<string> row = new List<string>();
                            i++;

                            while (!tokens[i].Equals("row-" + (m + 2)) && !tokens[i].Equals("RebarsBW") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                            {
                                row.Add(tokens[i]); i++;
                            }
                            m++;
                            member.Add(row);
                        }
                        l++;
                        floor.Add(member);
                    }
                    structuralMembers.Beam_stirRebar.Add(floor);
                    j++;
                }
            }
            //Beam Web
            i++;
            structuralMembers.totalweightkgm_web = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            j = 0;
            l = 0;
            m = 0;
            while (!tokens[i].Equals("RebarsSOG"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsSOG"))
                {
                    List<List<List<string>>> floor = new List<List<List<string>>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsSOG") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<List<string>> member = new List<List<string>>();
                        i++;
                        m = 0;
                        while (tokens[i].Equals("row-" + (m + 1)) && !tokens[i].Equals("RebarsSOG") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                        {
                            List<string> row = new List<string>();
                            i++;

                            while (!tokens[i].Equals("row-" + (m + 2)) && !tokens[i].Equals("RebarsSOG") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                            {
                                row.Add(tokens[i]); i++;
                            }
                            m++;
                            member.Add(row);
                        }
                        l++;
                        floor.Add(member);
                    }
                    structuralMembers.Beam_webRebar.Add(floor);
                    j++;
                }
            }
            //Slab on Grade
            i++;
            structuralMembers.totalweightkgm_slabongrade = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("RebarsSS"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsSS"))
                {
                    List<List<string>> floor = new List<List<string>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsSS") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<string> toAdd = new List<string>();
                        i++;

                        while (!tokens[i].Equals("member-" + (l + 2)) && !tokens[i].Equals("RebarsSS") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(tokens[i]); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.Slab_ongradeRebar.Add(floor);
                    j++;
                }
            }
            //Suspended Slab
            i++;
            structuralMembers.totalweightkgm_suspendedslab = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("RebarsST"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("RebarsST"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("RebarsST") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("member-" + (l + 2)) && !tokens[i].Equals("RebarsST") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        l++;
                        floor.Add(toAdd);
                    }
                    structuralMembers.Slab_suspendedRebar.Add(floor);
                    j++;
                }
            }
            //Stairs
            i++;
            j = 0;
            l = 0;
            m = 0;
            int n = 0;
            while (!tokens[i].Equals("Roofings"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Roofings"))
                {
                    List<List<double[,]>> floor = new List<List<double[,]>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("member-" + (l + 1)) && !tokens[i].Equals("Roofings") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<double[,]> member = new List<double[,]>();
                        i++;
                        m = 0;
                        while (tokens[i].Equals("rows-" + (m + 1)) && !tokens[i].Equals("Roofings") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)))
                        {
                            double[,] rows = new double[6,4];
                            i++;

                            n = 0;
                            while (tokens[i].Equals("row-" + (n + 1)) && !tokens[i].Equals("Roofings") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)) && !tokens[i].Equals("rows-" + (m + 2)))
                            {
                                i++;

                                int o = 0;
                                while (!tokens[i].Equals("row-" + (n + 2)) && !tokens[i].Equals("Roofings") && !tokens[i].Equals("Floor-" + (j + 2)) && !tokens[i].Equals("member-" + (l + 2)) && !tokens[i].Equals("rows-" + (m + 2)))
                                {
                                    rows[n, o] = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++; o++;
                                }
                                n++;
                            }
                            m++;
                            member.Add(rows);
                        }
                        l++;
                        floor.Add(member);
                    }
                    structuralMembers.stairs_Rebar.Add(floor);
                    j++;
                }
            }

            //6.0 - Roofings
            i++;
            //Totalities
            i++;
            j = 0;
            l = 0;
            while (!tokens[i].Equals("Cost"))
            {
                if (tokens[i].Equals("Floor-" + (j + 1)) && !tokens[i].Equals("Cost"))
                {
                    List<List<double>> floor = new List<List<double>>();
                    i++;
                    l = 0;
                    while (tokens[i].Equals("roof-" + (l + 1)) && !tokens[i].Equals("Cost") && !tokens[i].Equals("Floor-" + (j + 2)))
                    {
                        List<double> toAdd = new List<double>();
                        i++;

                        while (!tokens[i].Equals("roof-" + (l + 2)) && !tokens[i].Equals("Cost") && !tokens[i].Equals("Floor-" + (j + 2)))
                        {
                            toAdd.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
                        }
                        floor.Add(toAdd);
                        l++;
                    }
                    structuralMembers.roofSolutions.Add(floor);
                    j++;
                }
            }
            //Cost
            i++;
            rANDp_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            rANDp_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            rANDp_costTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            acce_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            acce_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            acce_costTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            tins_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            tins_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            tins_costTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            roof_MTOTAL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            roof_LTOTAL = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            roof_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //7.0 - Tiles
            i++;
            //Totalities
            i++;
            while (!tokens[i].Equals("Threehun"))
            {
                sixhun.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("Cost"))
            {
                threehun.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            //Cost
            i++;
            sixhun_MUnit = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            sixhun_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            sixhun_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            sixhun_costTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            threehun_MUnit = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            threehun_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            threehun_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            threehun_costTotal = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            tiles_mTOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            tiles_lTOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            tiles_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //8.0 - Paints
            i++;
            //Totalities
            i++;
            while (!tokens[i].Equals("Acrylic"))
            {
                enamel.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("Latex"))
            {
                acrylic.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("Gloss"))
            {
                latex.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            i++;
            while (!tokens[i].Equals("Cost"))
            {
                gloss.Add(double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture)); i++;
            }
            //Cost
            i++;
            enam_MUnit = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            enam_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            enam_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            enam_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            acry_MUnit = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            acry_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            acry_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            acry_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            late_MUnit = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            late_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            late_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            late_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            semi_MUnit = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            semi_MCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            semi_LCost = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            semi_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            paint_mTOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            paint_lTOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;
            paints_TOTALCOST = double.Parse(tokens[i], System.Globalization.CultureInfo.InvariantCulture); i++;

            //Totalities -- END
            viewInitalized = false;
            //*/
            //MessageBox.Show(tokens[i]);

            pf = new ParametersForm(parameters, this);
        }
        //Long Functions -- END

        //Extra Functions -- START
        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        private void CostEstimationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit this program?", "Exit Program", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //Do nothing
            }
            else if (dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }   
        }

        public void print(string str)
        {
            Console.WriteLine(str);
        }

        public static List<string> steelFilterer(string str)
        {
            List<string> dims = new List<string>();
            string retStr = "";
            foreach (char c in str)
            {
                if (Char.IsDigit(c)||c == '.')
                {
                    retStr += c;
                }
                else
                {
                    if (c == 'x')
                    {
                        dims.Add(retStr);
                        retStr = "";
                    }
                }
            }
            dims.Add(retStr);
            return dims;
        }

        public static List<string> dimensionFilterer(string str)
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
                    if (!Char.IsWhiteSpace(c))
                    {
                        dims.Add(retStr);
                        retStr = "";
                    }
                }
            }
            dims.Add(retStr);
            return dims;
        }

        public string lumberPriceHelper(List<string> dim)
        {
            string strLumber;
            if (dim[2] == "2")
            {
                strLumber = "Lumber [" + dim[0] + "”x " + dim[2] + "” x " + dim[4] + "']";
            }
            else
            {
                strLumber = "Lumber [" + dim[0] + "”x " + dim[2] + "”x " + dim[4] + "']";
            }
            return strLumber;
        }

        public string plywoodHelper(string str)
        {
            if(str == "Plywood")
            {                
                return "PLYWOOD 1/2” [1.22m x 2.44m]";
            }
            else if (str == "Ecoboard")
            {                
                return "ECOBOARD 1/2”[1.22m x 2.44m]";
            }
            else
            {                
                return "PHENOLIC BOARD- 1/2” [1.22m x 2.44m]";
            }
        }

        public List<string> gradefilterer(string grade)
        {
            List<string> passgrade = new List<string>();
            string iter = "";
            foreach(char a in grade)
            {
                if (Char.IsWhiteSpace(a))
                {
                    passgrade.Add(iter);
                    iter = "";
                }
                else
                {
                    iter += a;
                }
            }
            passgrade.Add(iter);
            return passgrade;
        }
        //Extra Functions -- END
    }
}

