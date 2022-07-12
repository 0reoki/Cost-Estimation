using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Parameters
    {
        //Parameters for Earthworks
        public string earth_CF_FA, earth_CF_TH, earth_CF_TY, earth_CF_CF,
                      earth_WF_FA, earth_WF_TH, earth_WF_TY, earth_WF_CF,
                      earth_WTB_FA, earth_WTB_TH, earth_WTB_TY, earth_WTB_CF,
                      earth_SG_AS, earth_SG_TS, earth_SG_TH, earth_SG_TY, earth_SG_CF;

        public List<string[]> earth_elevations = new List<string[]>();

        //Parameters for Formworks
        public string form_SM_F_FL,
                      form_SM_C_FL, form_SM_C_VS, form_SM_C_HB,
                      form_SM_B_FL, form_SM_B_VS, form_SM_B_HB, form_SM_B_DB,
                      form_SM_HS_VS,
                      form_SM_ST_FL, form_SM_ST_VS,
                      form_F_T, form_F_NU, form_F_N;

        //Parameters for Concrete
        public bool[] conc_cmIsSelected = { true, true, true, true, true, true };
        public string conc_CM_F_CG, conc_CM_F_GT, conc_CM_F_RM,
                      conc_CM_C_CG, conc_CM_C_GT, conc_CM_C_RM,
                      conc_CM_B_CG, conc_CM_B_GT, conc_CM_B_RM,
                      conc_CM_S_SOG_CG, conc_CM_S_SOG_GT, conc_CM_S_SOG_RM,
                      conc_CM_S_SS_CG, conc_CM_S_SS_GT, conc_CM_S_SS_RM,
                      conc_CM_W_MEW_CM, conc_CM_W_MIW_CM, conc_CM_W_P_CM, conc_CM_W_P_PT,
                      conc_CM_ST_CG, conc_CM_ST_GT, conc_CM_ST_RM,
                      conc_CC_F, conc_CC_SS, conc_CC_SG, conc_CC_BEE, conc_CC_BEW,
                      conc_CC_CEE, conc_CC_CEW;

        //Parameters for Reinforcements
        public DataTable rein_LSL_TB_dt = new DataTable();
        public List<string> rein_LSL_TB_fc_list = new List<string>();
        public DataTable rein_LSL_CB_dt = new DataTable();
        public List<string> rein_LSL_CB_fc_list = new List<string>();
        public DataTable rein_BEH_MB_dt = new DataTable();
        public DataTable rein_BEH_ST_dt = new DataTable();
        public DataTable rein_W_dt = new DataTable();
        public string rein_RG_C, rein_RG_CLT, rein_RG_F, rein_RG_B, rein_RG_BS, rein_RG_ST, rein_RG_W, rein_RG_SL;
        public bool[,] rein_mfIsSelected = new bool[,]  {
                                                            { true, true, true, true, true, true, true },
                                                            { true, true, true, true, true, true, true },
                                                            { true, true, true, true, true, true, true },
                                                            { true, true, true, true, true, true, true },
                                                            { true, true, true, true, true, true, true },
                                                            { true, true, true, true, true, true, true },
                                                            { true, true, true, true, true, true, true },
                                                        };

        //Parameters for Paint 
        public string paint_SCL;
        public List<string[]> paint_Area = new List<string[]>();

        //Parameters for Tiles
        public string tiles_FS, tiles_TG;
        public List<string[]> tiles_Area = new List<string[]>();

        //Parameters for Masonry
        public string mason_CHB_EW, mason_CHB_IW;
        public List<string[]> mason_exteriorWall = new List<string[]>();
        public List<string[]> mason_exteriorWindow = new List<string[]>();
        public List<string[]> mason_exteriorDoor = new List<string[]>();
        public List<string[]> mason_interiorWall = new List<string[]>();
        public List<string[]> mason_interiorWindow = new List<string[]>();
        public List<string[]> mason_interiorDoor = new List<string[]>();
        public string mason_RTW_VS, mason_RTW_HSL, mason_RTW_RG, mason_RTW_BD, mason_RTW_RL, mason_RTW_LTW;

        //Parameters for Labor and Equipment
        public string labor_RD;
        public List<string[]> labor_MP = new List<string[]>();
        public List<string[]> labor_EQP = new List<string[]>();

        //Parameters for Misc
        public List<string[]> misc_CustomItems = new List<string[]>();

        //Parameters for Price List
        public ListDictionary price_CommonMaterials = new ListDictionary(); //1
        public ListDictionary price_PaintAndCoating = new ListDictionary(); //2
        public ListDictionary price_WeldingRod = new ListDictionary(); //3
        public ListDictionary price_PersonalProtectiveEquipment = new ListDictionary(); //4
        public ListDictionary price_Tools = new ListDictionary(); //5
        public ListDictionary price_ReadyMixConcrete = new ListDictionary(); //6
        public ListDictionary price_Gravel = new ListDictionary(); //7
        public ListDictionary price_FormworksAndLumber = new ListDictionary(); //8
        public ListDictionary price_RoofMaterials = new ListDictionary(); //9
        public ListDictionary price_TubularSteel1mm = new ListDictionary(); //10
        public ListDictionary price_TubularSteel1p2mm = new ListDictionary(); //11
        public ListDictionary price_TubularSteel1p5mm = new ListDictionary(); //12
        public ListDictionary price_Embankment = new ListDictionary(); //13
        public ListDictionary price_RebarGrade33 = new ListDictionary(); //14 -- 230 Mpa
        public ListDictionary price_RebarGrade40 = new ListDictionary(); //15 -- 275 Mpa
        public ListDictionary price_RebarGrade60 = new ListDictionary(); //16 -- 415 Mpa
        public ListDictionary price_LaborRate_Earthworks = new ListDictionary(); //17 -- per m3
        public ListDictionary price_LaborRate_Concreting = new ListDictionary(); //18 -- per m3
        public ListDictionary price_LaborRate_Formworks = new ListDictionary(); //19 -- per m2
        public ListDictionary price_LaborRate_Rebar = new ListDictionary(); //20 -- per KG
        public ListDictionary price_LaborRate_Paint = new ListDictionary(); //21 -- per m2
        public ListDictionary price_LaborRate_Tiles = new ListDictionary(); //22 -- per m2
        public ListDictionary price_LaborRate_Masonry = new ListDictionary(); //23 -- per ?
        public ListDictionary price_LaborRate_Roofings = new ListDictionary(); //24 -- per ?
        public ListDictionary price_ManpowerM = new ListDictionary(); //25.1 -- per Hour
        public ListDictionary price_ManpowerP = new ListDictionary(); //25.2 -- per Hour
        public ListDictionary price_Equipment = new ListDictionary(); //26 -- per Hour

        public List<string> searchList = new List<string>();
        public List<string> customItemsList = new List<string>();

        public Parameters()
        {
            rein_LSL_TB_dt.Columns.Add("Bar Sizes");
            rein_LSL_CB_dt.Columns.Add("Bar Sizes");
            rein_BEH_MB_dt.Columns.Add("Bar Size (mm)");
            rein_BEH_MB_dt.Columns.Add("L(mm) 90º");
            rein_BEH_MB_dt.Columns.Add("L(mm) 135º");
            rein_BEH_MB_dt.Columns.Add("L(mm) 180º");
            rein_BEH_ST_dt.Columns.Add("Bar Size (mm)");
            rein_BEH_ST_dt.Columns.Add("L(mm) 90º");
            rein_BEH_ST_dt.Columns.Add("L(mm) 135º");
            rein_BEH_ST_dt.Columns.Add("L(mm) 180º");
            rein_W_dt.Columns.Add("Bar Size (Diameter)");
            rein_W_dt.Columns.Add("kg/m");
            rein_W_dt.Rows.Add("6mm", "");
            rein_W_dt.Rows.Add("8mm", "");
            rein_W_dt.Rows.Add("10mm", "");
            rein_W_dt.Rows.Add("12mm", "");
            rein_W_dt.Rows.Add("16mm", "");
            rein_W_dt.Rows.Add("20mm", "");
            rein_W_dt.Rows.Add("25mm", "");
            rein_W_dt.Rows.Add("28mm", "");
            rein_W_dt.Rows.Add("32mm", "");
            rein_W_dt.Rows.Add("36mm", "");
            rein_W_dt.Rows.Add("40mm", "");
            rein_W_dt.Rows.Add("44mm", "");
            rein_W_dt.Rows.Add("50mm", "");
            rein_W_dt.Rows.Add("56mm", "");
        }

        public void setEarthworkParameters
            (string earth_CF_FA, string earth_CF_TH, string earth_CF_TY, string earth_CF_CF,
             string earth_WF_FA, string earth_WF_TH, string earth_WF_TY, string earth_WF_CF,
             string earth_WTB_FA, string earth_WTB_TH, string earth_WTB_TY, string earth_WTB_CF,
             string earth_SG_AS, string earth_SG_TS, string earth_SG_TH, string earth_SG_TY, 
             string earth_SG_CF, List<string[]> earth_elevations)
        {
            this.earth_CF_FA = earth_CF_FA; 
            this.earth_CF_TH = earth_CF_TH; 
            this.earth_CF_TY = earth_CF_TY; 
            this.earth_CF_CF = earth_CF_CF;
            this.earth_WF_FA = earth_WF_FA;
            this.earth_WF_TH = earth_WF_TH;
            this.earth_WF_TY = earth_WF_TY;
            this.earth_WF_CF = earth_WF_CF;
            this.earth_WTB_FA = earth_WTB_FA;
            this.earth_WTB_TH = earth_WTB_TH;
            this.earth_WTB_TY = earth_WTB_TY;
            this.earth_WTB_CF = earth_WTB_CF;
            this.earth_SG_AS = earth_SG_AS;
            this.earth_SG_TS = earth_SG_TS;
            this.earth_SG_TH = earth_SG_TH;
            this.earth_SG_TY = earth_SG_TY;
            this.earth_SG_CF = earth_SG_CF;

            int i = 0;
            List<string[]> newEarthElev = new List<string[]>();
            foreach(string[] elev in earth_elevations)
            {
                double slabTPlusGBT = double.Parse(earth_SG_TS, System.Globalization.CultureInfo.InvariantCulture)
                    + double.Parse(earth_SG_TH, System.Globalization.CultureInfo.InvariantCulture);
                double elevation = double.Parse(elev[0], System.Globalization.CultureInfo.InvariantCulture);
                string cutOrFill = "";
                double thickness = elevation - slabTPlusGBT;
                thickness /= 1000;
                thickness = Math.Abs(thickness);
                if ((elevation - slabTPlusGBT) > 0)
                {
                    cutOrFill = "FILL";
                }
                else
                {
                    cutOrFill = "CUT";
                }
                string[] toAdd = { elev[0], elev[1], cutOrFill, thickness.ToString() };
                newEarthElev.Add(toAdd);
                i++;
            }
            this.earth_elevations = newEarthElev;
        }

        public void setFormworkParameters
            (string form_SM_F_FL,
             string form_SM_C_FL, string form_SM_C_VS, string form_SM_C_HB,
             string form_SM_B_FL, string form_SM_B_VS, string form_SM_B_HB, string form_SM_B_DB,
             string form_SM_HS_VS,
             string form_SM_ST_FL, string form_SM_ST_VS,
             string form_F_T, string form_F_NU, string form_F_N)
        {
            this.form_SM_F_FL = form_SM_F_FL;
            this.form_SM_C_FL = form_SM_C_FL;
            this.form_SM_C_VS = form_SM_C_VS;
            this.form_SM_C_HB = form_SM_C_HB;
            this.form_SM_B_FL = form_SM_B_FL;
            this.form_SM_B_VS = form_SM_B_VS;
            this.form_SM_B_HB = form_SM_B_HB;
            this.form_SM_B_DB = form_SM_B_DB;
            this.form_SM_HS_VS = form_SM_HS_VS;
            this.form_SM_ST_FL = form_SM_ST_FL;
            this.form_SM_ST_VS = form_SM_ST_VS;
            this.form_F_T = form_F_T;
            this.form_F_NU = form_F_NU;
            this.form_F_N = form_F_N;
        }

        public void setConcreteParameters
            (bool[] cmIsSelected, string conc_CM_F_CG, string conc_CM_F_GT, string conc_CM_F_RM,
             string conc_CM_C_CG, string conc_CM_C_GT, string conc_CM_C_RM,
             string conc_CM_B_CG, string conc_CM_B_GT, string conc_CM_B_RM,
             string conc_CM_S_SOG_CG, string conc_CM_S_SOG_GT, string conc_CM_S_SOG_RM,
             string conc_CM_S_SS_CG, string conc_CM_S_SS_GT, string conc_CM_S_SS_RM,
             string conc_CM_MEW_M_CM, string conc_CM_MIW_M_CM, string conc_CM_W_P_CM, string conc_CM_W_P_PT,
             string conc_CM_ST_CG, string conc_CM_ST_GT, string conc_CM_ST_RM,
             string conc_CC_F, string conc_CC_SS, string conc_CC_SG, string conc_CC_BEE,
             string conc_CC_BEW, string conc_CC_CEE, string conc_CC_CEW)
        {
            this.conc_cmIsSelected = cmIsSelected;
            this.conc_CM_F_CG = conc_CM_F_CG; 
            this.conc_CM_F_GT = conc_CM_F_GT; 
            this.conc_CM_F_RM = conc_CM_F_RM;
            this.conc_CM_C_CG = conc_CM_C_CG; 
            this.conc_CM_C_GT = conc_CM_C_GT; 
            this.conc_CM_C_RM = conc_CM_C_RM;
            this.conc_CM_B_CG = conc_CM_B_CG; 
            this.conc_CM_B_GT = conc_CM_B_GT; 
            this.conc_CM_B_RM = conc_CM_B_RM;
            this.conc_CM_S_SOG_CG = conc_CM_S_SOG_CG;
            this.conc_CM_S_SOG_GT = conc_CM_S_SOG_GT;
            this.conc_CM_S_SOG_RM = conc_CM_S_SOG_RM;
            this.conc_CM_S_SS_CG = conc_CM_S_SS_CG; 
            this.conc_CM_S_SS_GT = conc_CM_S_SS_GT; 
            this.conc_CM_S_SS_RM = conc_CM_S_SS_RM;
            this.conc_CM_W_MEW_CM = conc_CM_MEW_M_CM;
            this.conc_CM_W_MIW_CM = conc_CM_MIW_M_CM;
            this.conc_CM_W_P_CM = conc_CM_W_P_CM; 
            this.conc_CM_W_P_PT = conc_CM_W_P_PT;
            this.conc_CM_ST_CG = conc_CM_ST_CG;
            this.conc_CM_ST_GT = conc_CM_ST_GT;
            this.conc_CM_ST_RM = conc_CM_ST_RM;
            this.conc_CC_F = conc_CC_F; 
            this.conc_CC_SS = conc_CC_SS; 
            this.conc_CC_SG = conc_CC_SG; 
            this.conc_CC_BEE = conc_CC_BEE; 
            this.conc_CC_BEW = conc_CC_BEW;
            this.conc_CC_CEE = conc_CC_CEE; 
            this.conc_CC_CEW = conc_CC_CEW;
        }

        public void setReinforcementsParameters
            (DataTable rein_LSL_TB_dt, DataTable rein_LSL_CB_dt, DataTable rein_BEH_MB_dt, DataTable rein_BEH_ST_dt,
             DataTable rein_W_dt, List<string> rein_LSL_TB_fc_list, List<string> rein_LSL_CB_fc_list,
             string rein_RG_C, string rein_RG_CLT, string rein_RG_F, string rein_RG_B, string rein_RG_BS, string rein_RG_ST, string rein_RG_W, string rein_RG_SL,
             bool[,] rein_mfIsSelected)
        {
            this.rein_LSL_TB_dt = rein_LSL_TB_dt;
            this.rein_LSL_CB_dt = rein_LSL_CB_dt;
            this.rein_BEH_MB_dt = rein_BEH_MB_dt;
            this.rein_BEH_ST_dt = rein_BEH_ST_dt;
            this.rein_W_dt = rein_W_dt;

            this.rein_LSL_TB_fc_list = rein_LSL_TB_fc_list;
            this.rein_LSL_CB_fc_list = rein_LSL_CB_fc_list;

            this.rein_RG_C = rein_RG_C;
            this.rein_RG_CLT = rein_RG_CLT;
            this.rein_RG_F = rein_RG_F;
            this.rein_RG_B = rein_RG_B;
            this.rein_RG_BS = rein_RG_BS;
            this.rein_RG_ST = rein_RG_ST;
            this.rein_RG_W = rein_RG_W;
            this.rein_RG_SL = rein_RG_SL;

            this.rein_mfIsSelected = rein_mfIsSelected;
        }

        public void setPaintParameters
            (string paint_SCL, List<string[]> paint_Area)
        {
            this.paint_SCL = paint_SCL;
            this.paint_Area = paint_Area;
        }

        public void setTilesParameters
            (string tiles_FS, string tiles_TG, List<string[]> tiles_Area)
        {
            this.tiles_FS = tiles_FS;
            this.tiles_TG = tiles_TG;
            this.tiles_Area = tiles_Area;
        }
        public void setMasonryParameters
            (string mason_CHB_EW, string mason_CHB_IW,
             List<string[]> exteriorWall,
             List<string[]> exteriorWindow,
             List<string[]> exteriorDoor,
             List<string[]> interiorWall,
             List<string[]> interiorWindow,
             List<string[]> interiorDoor,
             string mason_RTW_VS, string mason_RTW_HSL, string mason_RTW_RG, string mason_RTW_BD, string mason_RTW_RL, string mason_RTW_LTW)
        {
            this.mason_CHB_EW = mason_CHB_EW;
            this.mason_CHB_IW = mason_CHB_IW;
            this.mason_exteriorWall = exteriorWall;
            this.mason_exteriorWindow = exteriorWindow;
            this.mason_exteriorDoor = exteriorDoor;
            this.mason_interiorWall = interiorWall;
            this.mason_interiorWindow = interiorWindow;
            this.mason_interiorDoor = interiorDoor;
            this.mason_RTW_VS = mason_RTW_VS;
            this.mason_RTW_HSL = mason_RTW_HSL;
            this.mason_RTW_RG = mason_RTW_RG;
            this.mason_RTW_BD = mason_RTW_BD;
            this.mason_RTW_RL = mason_RTW_RL;
            this.mason_RTW_LTW = mason_RTW_LTW;

        }

        public void setLaborParameters
            (string labor_RD, List<string[]> labor_MP, List<string[]> labor_EQP)
        {
            this.labor_RD = labor_RD;
            this.labor_MP = labor_MP;
            this.labor_EQP = labor_EQP;
        }

        public void setMiscParameters
            (List<string[]> misc_CustomItems)
        {
            this.misc_CustomItems = misc_CustomItems;
        }
    }
}
