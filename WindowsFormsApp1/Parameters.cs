﻿using System;
using System.Collections.Generic;
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
        public bool[] conc_cmIsSelected = { true, true, true, true, true };
        public string conc_CM_F_CG, conc_CM_F_GT, conc_CM_F_RM,
                      conc_CM_C_CG, conc_CM_C_GT, conc_CM_C_RM,
                      conc_CM_B_CG, conc_CM_B_GT, conc_CM_B_RM,
                      conc_CM_S_CG, conc_CM_S_GT, conc_CM_S_RM,
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
        public string rein_S_C_SL, rein_S_C_SZ, rein_S_C_AP, rein_S_C_MVDAB,
                      rein_S_B_T_SL, rein_S_B_T_SZ, rein_S_B_T_AP, rein_S_B_B_SL, rein_S_B_B_SZ, rein_S_B_B_AP, rein_S_B_MHDAB,
                      rein_S_S_T_SL, rein_S_S_B_SL,
                      rein_RG_C, rein_RG_F, rein_RG_B, rein_RG_ST, rein_RG_W, rein_RG_SL;
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
            this.earth_elevations = earth_elevations;
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
             string conc_CM_S_CG, string conc_CM_S_GT, string conc_CM_S_RM,
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
            this.conc_CM_S_CG = conc_CM_S_CG; 
            this.conc_CM_S_GT = conc_CM_S_GT; 
            this.conc_CM_S_RM = conc_CM_S_RM;
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
             string rein_S_C_SL, string rein_S_C_SZ, string rein_S_C_AP, string rein_S_C_MVDAB,
             string rein_S_B_T_SL, string rein_S_B_T_SZ, string rein_S_B_T_AP, string rein_S_B_B_SL, string rein_S_B_B_SZ, string rein_S_B_B_AP, string rein_S_B_MHDAB,
             string rein_S_S_T_SL, string rein_S_S_B_SL,
             string rein_RG_C, string rein_RG_F, string rein_RG_B, string rein_RG_ST, string rein_RG_W, string rein_RG_SL,
             bool[,] rein_mfIsSelected)
        {
            this.rein_LSL_TB_dt = rein_LSL_TB_dt;
            this.rein_LSL_CB_dt = rein_LSL_CB_dt;
            this.rein_BEH_MB_dt = rein_BEH_MB_dt;
            this.rein_BEH_ST_dt = rein_BEH_ST_dt;
            this.rein_W_dt = rein_W_dt;

            this.rein_LSL_TB_fc_list = rein_LSL_TB_fc_list;
            this.rein_LSL_CB_fc_list = rein_LSL_CB_fc_list;

            this.rein_S_C_SL = rein_S_C_SL;
            this.rein_S_C_SZ = rein_S_C_SZ;
            this.rein_S_C_AP = rein_S_C_AP;
            this.rein_S_C_MVDAB = rein_S_C_MVDAB;

            this.rein_S_B_T_SL = rein_S_B_T_SL;
            this.rein_S_B_T_SZ = rein_S_B_T_SZ;
            this.rein_S_B_T_AP = rein_S_B_T_AP;
            this.rein_S_B_B_SL = rein_S_B_B_SL;
            this.rein_S_B_B_SZ = rein_S_B_B_SZ;
            this.rein_S_B_B_AP = rein_S_B_B_AP;
            this.rein_S_B_MHDAB = rein_S_B_MHDAB;

            this.rein_S_S_T_SL = rein_S_S_T_SL;
            this.rein_S_S_B_SL = rein_S_S_B_SL;

            this.rein_RG_C = rein_RG_C;
            this.rein_RG_F = rein_RG_F;
            this.rein_RG_B = rein_RG_B;
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
