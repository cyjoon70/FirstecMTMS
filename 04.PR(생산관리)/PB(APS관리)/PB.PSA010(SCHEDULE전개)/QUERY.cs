using System;

namespace PB.PSA010
{
	/// <summary>
	/// QUERY에 대한 요약 설명입니다.
	/// </summary>
	public class QUERY
	{
		public QUERY(){}

		#region 스케쥴 진행률/진행상태 처리
		public static string SCH_PROC_RATE()
		{
			/// <summary>
			/// 스케쥴 진행률 조회
			/// </summary>

			string Query = " SELECT * ";
			Query		+= " FROM   P_BOP_SCH_PROC_TEMP A (NOLOCK) ";
			Query		+= " WHERE  PROC_ID = '" +  PSA010.PROC_ID + "'";
            Query       += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

			return Query;

		}

		public static string SCH_PROC_CHK()
		{
			/// <summary>
			/// 현재 긴급작지나 스케쥴을 전개하고 있는지 판단
			/// </summary>

			string Query = " SELECT PROC_TYPE,    PROC_STATUS,    IN_ID ";
			Query		+= " FROM   P_BOP_SCH_PROC_TEMP A (NOLOCK) ";
			Query		+= " WHERE  PROC_TYPE IN ('E', 'S')"; // 긴급작지, 스케쥴 전개
            Query       += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

			return Query;

		}

		public static string INS_SCH_PROC(string PROC_TYPE, string PROC_STATUS, string USR_ID)
		{
			/// <summary>
			/// 스케쥴 진행정보 등록
			/// </summary>

			string Query = " INSERT INTO P_BOP_SCH_PROC_TEMP ( ";
			Query		+= "     CO_CD,      PROC_ID,         TOT_PROC_AMT,    CUR_PROC_AMT,    CUR_PROC_PER, ";
			Query		+= "     PROC_TYPE,  PROC_STATUS,     IN_ID,           IN_DT ";
			Query		+= " ) ";
			Query		+= " VALUES ( ";
			Query		+= "    '"+ SystemBase.Base.gstrCOMCD.ToString() +"', '" + PSA010.PROC_ID + "',  0,                 0,           0, ";
			Query		+= "    '" + PROC_TYPE      + "', '" + PROC_STATUS + "', '" + USR_ID + "',      GETDATE() ";
			Query		+= " ) ";

			return Query;
		}

		public static string DEL_SCH_PROC()
		{
			/// <summary>
			/// 스케쥴 진행 정보 삭제
			/// </summary>

			string Query = " DELETE ";
			Query		+= " FROM   P_BOP_SCH_PROC_TEMP ";
			Query		+= " WHERE  PROC_ID = '" +  PSA010.PROC_ID + "'";
            Query       += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

			return Query;

		}
		#endregion

		#region BOM전개시 ORDER통합삭제
		public static string DEL_OREDER_PROC()
		{
			/// <summary>
			/// 스케쥴 진행 정보 삭제
			/// </summary>

			string Query = " DELETE ";
			Query		+= " FROM   P_BOP_UNITY_ORDER ";
			Query		+= " WHERE ISNULL(WORKORDER_NO_OG, '') = ''";
            Query       += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

			return Query;

		}
		#endregion

		#region SCH전개시 배정정보 삭제
		public static string DEL_RESO_WORK_TIME()
		{
			/// <summary>
			/// 스케쥴 진행 정보 삭제
			/// </summary>

			string Query = " DELETE ";
			Query		+= " FROM  P_BOP_RESO_WORK_TIME_TEMP ";
            Query       += " WHERE  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

			return Query;
		}
		#endregion

		#region MPS 조회
		public static string MPS(string PLANT_CD, string SCHST_DT, string PTF, string PROJECT_NO, string PROJECT_SEQ)
		{
			/// <summary>
			/// MPS 조회
			/// CMLT(누적 리드타임): 같은 공정에서 누적 리드타임이 작은 공정을 우선순위로 일정을 전개한다
			/// </summary>

			string Query = " SELECT * ";
			Query		+= "   FROM P_MPS_REGISTER(NOLOCK) ";
			Query		+= "  WHERE (PLANT_CD = '"+ PLANT_CD +"' ";
			Query		+= "    AND (STATUS = 'R' OR  STATUS = 'F') ";
			Query		+= "    AND MAKEFINISH_DT <= '"+ PTF +"' ";	//PTF 일자까지만 스케줄링
			Query		+= "    AND UP_ID <> 'MTMS') ";	//PTF 일자까지만 스케줄링
            Query       += "    AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

			if(PROJECT_NO.Length > 0 && PROJECT_SEQ.Length > 0)
				Query	+= "     OR (PROJECT_NO = '"+ PROJECT_NO +"' AND PROJECT_SEQ = '"+ PROJECT_SEQ +"') ";
			else if(PROJECT_NO.Length > 0)
				Query	+= "     OR (PROJECT_NO = '"+ PROJECT_NO +"') ";
			Query		+= "  ORDER BY MPS_TYPE DESC, RANK_NO, MAKEFINISH_DT ";

			return Query;
		}
		#endregion

		#region USEKIND 전용, 공용 여부
		public static string USEKIND()
		{
			/// <summary>
			/// 전용, 공용, 전용/공용 (1,2,3)
			/// MAJOR 코드 : P011
			/// </summary>
			
			string MBOPDel	 = " SELECT MINOR_CD FROM B_COMM_CODE (NOLOCK) WHERE COMP_CODE = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' AND LANG_CD = 'KOR' AND MAJOR_CD = 'P011' AND DEF_FLAG = 'Y' ";
			return MBOPDel;
		}
		#endregion

		#region USEKIND_CFY 전용재고 감안(확정데이타가 있는경우)
		public static string USEKIND_CFY()
		{	// 재고감안 가용재고 감안 로직 다시 확인요
			string MBOPDel= "SELECT A.PROJECT_NO, A.PROJECT_SEQ, A.CHILD_ITEM_CD, ";
			MBOPDel += "	   MAX(ISNULL(C.G_STOCK_QTY, 0)) GOOD_ON_HAND_QTY, ";
			MBOPDel += "	   (SUM(ISNULL(A.CHILD_ITEM_QTY, 0)) - SUM(ISNULL(B.RESULT_QTY, 0))) RECV_QTY, ";
			MBOPDel += "	   (SUM(ISNULL(A.CHILD_ITEM_QTY, 0)) - SUM(ISNULL(B.RESULT_QTY, 0))) ISSUE_QTY, ";
			MBOPDel += "	   (MAX(ISNULL(C.G_STOCK_QTY, 0)) + (SUM(ISNULL(B.WORK_QTY, 0)) + SUM(ISNULL(B.RESULT_QTY, 0))) - (SUM(ISNULL(B.WORK_QTY, 0)) - SUM(ISNULL(B.RESULT_QTY, 0)))) USE_QTY  , ";
			MBOPDel += "	   (MAX(ISNULL(C.G_STOCK_QTY, 0)) + (SUM(ISNULL(B.WORK_QTY, 0)) + SUM(ISNULL(B.RESULT_QTY, 0))) - (SUM(ISNULL(B.WORK_QTY, 0)) - SUM(ISNULL(B.RESULT_QTY, 0)))) USE_QTY2  ";
			MBOPDel += "  FROM P_BOP_M_DETAIL A(NOLOCK)";
			MBOPDel += "  LEFT OUTER JOIN P_BOP_M_PROC_DETAIL B(NOLOCK)";
			MBOPDel += "	ON A.CO_CD		    = B.CO_CD";
            MBOPDel += "   AND A.PROJECT_SEQ	= B.PROJECT_SEQ";
			MBOPDel += "   AND A.PROJECT_SEQ	= B.PROJECT_SEQ";
			MBOPDel += "   AND A.GROUP_CD		= B.GROUP_CD";
			MBOPDel += "   AND A.PRNT_PLANT_CD	= B.PLANT_CD";
			//MBOPDel += "   AND A.PRNT_ITEM_CD	= B.PRNT_ITEM_CD";
			MBOPDel += "   AND A.CHILD_ITEM_CD	= B.ITEM_CD";
			MBOPDel += "  LEFT OUTER JOIN I_ON_HAND_STOCK C(NOLOCK)";
            MBOPDel += "	ON A.CO_CD		    = C.CO_CD";
            MBOPDel += "   AND A.CHILD_ITEM_CD	= C.ITEM_CD";
			MBOPDel += "   AND A.PRNT_PLANT_CD	= C.PLANT_CD";
			MBOPDel += "  LEFT OUTER JOIN B_ITEM_INFO D(NOLOCK)";
            MBOPDel += "	ON A.CO_CD		    = D.CO_CD";
			MBOPDel += "   AND A.CHILD_ITEM_CD  = D.ITEM_CD";
			MBOPDel += " WHERE A.CO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"'";
            MBOPDel += "   AND A.CHILD_ITEM_CD = '02'  ";
			MBOPDel += "   AND B.PROJECT_NO IS NOT NULL";
			MBOPDel += " GROUP BY A.PROJECT_NO, A.PROJECT_SEQ, A.CHILD_ITEM_CD ";

			return MBOPDel;
		}
		#endregion

		#region USEKIND_CFN 전용재고 감안(확정데이타가 없는경우)
		public static string USEKIND_CFN(string KIND)
		{	// 재고수량 체크 확인 2009년 01월 20일
			string QUERY= "";

			QUERY += "SELECT D.PROJECT_NO PROJ_NO, D.PROJECT_SEQ MAKE_NO, D.ITEM_CD MATERIAL_CD, ";
			QUERY += "	     SUM(D.G_STOCK_QTY) GOOD_ON_HAND_QTY,";
			QUERY += "	     0 RECV_QTY, ";
			QUERY += "	     0 ISSUE_QTY,";
			QUERY += "	     SUM(D.G_STOCK_QTY) USE_QTY,  ";
			QUERY += "	     SUM(D.G_STOCK_QTY) USE_QTY2  ";
			QUERY += "  FROM (SELECT CO_CD, CHILD_ITEM_CD FROM P_BOP_C_DETAIL B(NOLOCK)  ";
			QUERY += "         GROUP BY CO_CD, CHILD_ITEM_CD) C  ";
			QUERY += "  LEFT OUTER JOIN I_ON_HAND_STOCK D(NOLOCK)  ";
            QUERY += "	ON C.CO_CD = D.CO_CD ";
            QUERY += "  AND C.CHILD_ITEM_CD = D.ITEM_CD ";
            QUERY += " WHERE C.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() +"' ";
            QUERY += "   AND D.ITEM_CD IS NOT NULL ";

			if(KIND == "1")	//1인경우 전용재고, 2인경우 공용재고, 3인경우 전용, 공용 모두
				QUERY += "   AND D.PROJECT_NO <> '*' AND D.PROJECT_SEQ <> '*' ";
			else if(KIND == "2")
				QUERY += "  AND D.PROJECT_NO = '*' AND D.PROJECT_SEQ = '*' ";

			QUERY += " GROUP BY D.PROJECT_NO, D.PROJECT_SEQ, D.ITEM_CD  ";

			return QUERY;
		}
		#endregion

		#region STOCK_QTY 현재고 감안
		public static string STOCK_QTY(string KIND)
		{
			string QUERY= "";
			QUERY += "SELECT D.PROJECT_NO PROJ_NO, D.PROJECT_SEQ MAKE_NO, D.ITEM_CD MATERIAL_CD";
			QUERY += "       , SUM(D.G_STOCK_QTY) GOOD_ON_HAND_QTY";
			QUERY += "       , CONVERT(DECIMAL(15,4), 0) RECV_QTY";
			QUERY += "       , CONVERT(DECIMAL(15,4), 0) ISSUE_QTY";
			QUERY += "       , SUM(D.G_STOCK_QTY) USE_QTY";
			QUERY += "       , SUM(D.G_STOCK_QTY) USE_QTY2";
			QUERY += "  FROM (SELECT A.CO_CD, B.CHILD_ITEM_CD ";
			QUERY += "          FROM (SELECT CO_CD, ITEM_CD ";
			QUERY += "                  FROM P_MPS_REGISTER A(NOLOCK) WHERE CO_CD = '"+SystemBase.Base.gstrCOMCD.ToString()+"' ";
			QUERY += "                 GROUP BY CO_CD, ITEM_CD) A ";
			QUERY += "          LEFT OUTER JOIN P_BOP_C_DETAIL B(NOLOCK) ";
			QUERY += "            ON A.CO_CD = B.CO_CD ";
            QUERY += "           AND A.ITEM_CD = B.GROUP_CD ";
            QUERY += "         WHERE A.CO_CD = '"+SystemBase.Base.gstrCOMCD.ToString()+"'";
			QUERY += "         GROUP BY A.CO_CD, B.CHILD_ITEM_CD) C";
			QUERY += "  LEFT OUTER JOIN I_ON_HAND_STOCK D(NOLOCK)";
			QUERY += "    ON C.CO_CD = D.CO_CD ";
            QUERY += "   AND C.CHILD_ITEM_CD = D.ITEM_CD";
			QUERY += " WHERE C.CO_CD = '"+SystemBase.Base.gstrCOMCD.ToString()+"'";
            QUERY += "   AND D.ITEM_CD IS NOT NULL";

			if(KIND == "1") //1인경우 전용재고, 2인경우 공용재고, 3인경우 전용,공용 모두
				QUERY += " AND D.PROJECT_NO <> '*' AND D.PROJECT_SEQ <> '*'";
			else if(KIND == "2")
				QUERY += " AND D.PROJECT_NO = '*' AND D.MFG_CHA = '*'";
			QUERY += " GROUP BY D.PROJECT_NO, D.PROJECT_SEQ, D.ITEM_CD";

			return QUERY;
		}
		#endregion

		#region USEKIND_A 공용재고 감안(확정데이타가 있는 경우)
		public static string USEKIND_A()
		{
			string STOCKQuery = "";
			STOCKQuery += " SELECT PROJECT_NO PROJ_NO, PROJECT_SEQ MAKE_NO, ITEM_CD MATERIAL_CD, G_STOCK_QTY, ";
			STOCKQuery += "        0 RECV_QTY, 0 ISSUE_QTY, G_STOCK_QTY USE_QTY, G_STOCK_QTY USE_QTY2 ";
			STOCKQuery += "   FROM I_ON_HAND_STOCK (NOLOCK) ";
            STOCKQuery += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            STOCKQuery += "    AND PROJECT_NO = '*' ";
			STOCKQuery += "    AND PROJECT_SEQ = '*'";

			return STOCKQuery;
		}
		#endregion

		#region TEMP 테이블 시작 종료시간 초기화
		public static string TEMP_RESET(string MAKEORDER_NO, string SCH_ID)
		{
			string QUERY= "";
			QUERY += " UPDATE P_BOP_M_PROC_DETAIL_TEMP SET START_DT = '', START_TM = '', START_SC = '',  ";
			QUERY += "        END_DT = '', END_TM = '', END_SC = '' ";
            QUERY += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND MAKEORDER_NO='"+ MAKEORDER_NO +"' ";
			QUERY += "    AND SCH_ID = '"+ SCH_ID +"'  ";
			QUERY += "    AND RESULT_QTY = 0  ";
			QUERY += "    AND CLOSE_FLG = 'N'  ";//마감이 안된 데이타만 가져오기
			QUERY += "    AND SCH_YN = 'Y'  ";	//스케줄전개 여부가 Y인 데이타만

			return QUERY;
		}
		#endregion

		#region MAKEQTY_SELECT(생산수량) 계산하기 위한 데이타 SELECT
		public static string MAKEQTY_SELECT(string SCH_ID, string MAKEORDER_NO, string PROJECT_NO, string PROJECT_SEQ)
		{
			string QUERY= "";

			QUERY += " SELECT PROJECT_NO, PROJECT_SEQ, GROUP_CD, PLANT_CD, ITEM_CD, SCH_ID, ";
			QUERY += "        WORKORDER_NO, NEED_QTY, NEED_QTY_UNIT ";
			QUERY += "   FROM P_BOP_M_MASTER_TEMP A(NOLOCK)  ";
            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.SCH_ID = '"		+ SCH_ID +"'  ";
			QUERY += "    AND A.MAKEORDER_NO='"	+ MAKEORDER_NO +"' ";
			QUERY += "    AND A.PROJECT_NO='"	+ PROJECT_NO +"' ";
			QUERY += "    AND A.PROJECT_SEQ='"	+ PROJECT_SEQ +"' ";
			QUERY += "    AND A.SCH_YN = 'Y' ";	//스케줄전개 여부가 Y인 데이타만

			return QUERY;
		}
		#endregion

		#region MBOP_COPY() 확정된 BOP TEMP 테이블로 복사
		public static string MBOP_COPY(
			string USER_ID, 
			string SCH_YN, 
			string MAKEORDER_NO, 
			string SCH_ID
			)
		{	// 확정된 데이타가 있을 경우 확정 데이타 복사
			string QUERY= "";
			QUERY += " DELETE FROM P_BOP_M_DETAIL_TEMP WHERE CO_CD = '"+SystemBase.Base.gstrCOMCD.ToString()+"' AND MAKEORDER_NO='"		+ MAKEORDER_NO +"' AND SCH_ID = '"+ SCH_ID +"' ";
            QUERY += " DELETE FROM P_BOP_M_MASTER_TEMP WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' AND MAKEORDER_NO='" + MAKEORDER_NO + "' AND SCH_ID = '" + SCH_ID + "' ";
            QUERY += " DELETE FROM P_BOP_M_PROC_CHILD_TEMP WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' AND MAKEORDER_NO='" + MAKEORDER_NO + "' AND SCH_ID = '" + SCH_ID + "' ";
            QUERY += " DELETE FROM P_BOP_M_PROC_DETAIL_TEMP WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' AND MAKEORDER_NO='" + MAKEORDER_NO + "' AND SCH_ID = '" + SCH_ID + "' ";
            QUERY += " DELETE FROM P_BOP_M_PROC_MASTER_TEMP WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' AND MAKEORDER_NO='" + MAKEORDER_NO + "' AND SCH_ID = '" + SCH_ID + "' ";

			QUERY += " INSERT INTO P_BOP_M_DETAIL_TEMP ( CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PRNT_PLANT_CD, PRNT_ITEM_CD,  ";
			QUERY += "        PRNT_BOM_NO, CHILD_ITEM_SEQ, CHILD_SEQ, CHILD_PLANT_CD, CHILD_ITEM_CD, CHILD_BOM_NO,  ";
			QUERY += "        PRNT_ITEM_QTY, PRNT_ITEM_UNIT, CHILD_ITEM_QTY, CHILD_ITEM_UNIT, NEED_QTY, NEED_QTY_UNIT,  ";
			QUERY += "        WORK_QTY, WORK_QTY_UNIT, REQUEST_QTY, REQUEST_QTY_UNIT, REQUEST_DT, ORDER_QTY, MAKE_QTY,  ";
			QUERY += "        MAKE_QTY_UNIT, ISSUE_MTHD, ISSUE_QTY, ISSUE_DT, RESULT_QTY, RESULT_DT, MENUAL_FLG,  ";
			QUERY += "        LOSS_RATE, SAFETY_LT, SUPPLY_TYPE, BOM_FLG, REMARK, VALID_FROM_DT, VALID_TO_DT,  ";
			QUERY += "        INSRT_USER_ID, INSRT_DT, UPDT_USER_ID, UPDT_DT, ECN_NO, USE_FLG, FIG_NO, MAT_SIZE,  ";
			QUERY += "        SCH_YN, MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, SCH_ID ) ";
			QUERY += " SELECT CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PRNT_PLANT_CD, PRNT_ITEM_CD, PRNT_BOM_NO, CHILD_ITEM_SEQ,  ";
			QUERY += "        CHILD_SEQ, CHILD_PLANT_CD, CHILD_ITEM_CD, CHILD_BOM_NO, PRNT_ITEM_QTY, PRNT_ITEM_UNIT,  ";
			QUERY += "        CHILD_ITEM_QTY, CHILD_ITEM_UNIT, NEED_QTY, NEED_QTY_UNIT, WORK_QTY, WORK_QTY_UNIT,  ";
			QUERY += "        REQUEST_QTY, REQUEST_QTY_UNIT, REQUEST_DT, ORDER_QTY, MAKE_QTY, MAKE_QTY_UNIT, ISSUE_MTHD,  ";
			QUERY += "        ISSUE_QTY, ISSUE_DT, RESULT_QTY, RESULT_DT, MENUAL_FLG, LOSS_RATE, SAFETY_LT, SUPPLY_TYPE,  ";
			QUERY += "        BOM_FLG, REMARK, VALID_FROM_DT, VALID_TO_DT, INSRT_USER_ID, INSRT_DT, '"+ USER_ID +"',  ";
			QUERY += "        GETDATE(), ECN_NO, USE_FLG, FIG_NO, MAT_SIZE, '"+ SCH_YN +"', MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, '"+ SCH_ID +"'  ";
			QUERY += "   FROM P_BOP_M_DETAIL (NOLOCK) ";
			QUERY += "  WHERE CO_CD = '"+SystemBase.Base.gstrCOMCD.ToString()+"'";
            QUERY += "    AND MAKEORDER_NO='"+ MAKEORDER_NO +"' ";
			QUERY += "     IF @@ERROR <> 0 ";
			QUERY += "     BEGIN ";
			QUERY += "           SELECT 'ER', '스케쥴 확정중 에러가 발생되었습니다.' ";
			QUERY += "           RETURN ";
			QUERY += "     END ";

			QUERY += " INSERT INTO P_BOP_M_MASTER_TEMP (CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PLANT_CD, ITEM_CD, BOM_NO, DESCRIPTION,  ";
			QUERY += "        MAJOR_FLG, VALID_FROM_DT, VALID_TO_DT, DRAWING_PATH, INSRT_USER_ID, INSRT_DT,  ";
			QUERY += "        UPDT_USER_ID, UPDT_DT, SCH_YN, MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, SCH_ID, NEED_QTY, NEED_QTY_UNIT, WORK_QTY, WORK_QTY_UNIT) ";
			QUERY += " SELECT CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PLANT_CD, ITEM_CD, BOM_NO, DESCRIPTION, MAJOR_FLG,  ";
			QUERY += "        VALID_FROM_DT, VALID_TO_DT, DRAWING_PATH, INSRT_USER_ID, INSRT_DT, '"+ USER_ID +"', GETDATE(),  ";
			QUERY += "        '"+ SCH_YN +"', MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, '"+ SCH_ID +"', NEED_QTY, NEED_QTY_UNIT, WORK_QTY, WORK_QTY_UNIT ";
			QUERY += "   FROM P_BOP_M_MASTER (NOLOCK) ";
            QUERY += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND MAKEORDER_NO='"+ MAKEORDER_NO +"' ";

			QUERY += "    AND BOM_NO <> 'S' ";

			QUERY += "     IF @@ERROR <> 0 ";
			QUERY += "     BEGIN ";
			QUERY += "            SELECT 'ER', '스케쥴 확정중 에러가 발생되었습니다.' ";
			QUERY += "            RETURN ";
			QUERY += "     END ";

			QUERY += " INSERT INTO P_BOP_M_PROC_CHILD_TEMP (CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, BOM_NO, CHILD_ITEM_SEQ, CHILD_ITEM_CD,  ";
			QUERY += "        INSRT_USER_ID, INSRT_DT, UPDT_USER_ID, UPDT_DT, PLANT_CD, ITEM_CD, ROUT_NO, PROC_SEQ, SCH_YN,  ";
			QUERY += "        MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, SCH_ID, JOB_CD) ";
			QUERY += " SELECT CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, BOM_NO, CHILD_ITEM_SEQ, CHILD_ITEM_CD, INSRT_USER_ID, INSRT_DT,  ";
			QUERY += "        '"+ USER_ID +"', GETDATE(), PLANT_CD, ITEM_CD, ROUT_NO, PROC_SEQ, '"+ SCH_YN +"', MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG,  ";
			QUERY += "        '"+ SCH_ID +"', JOB_CD ";
			QUERY += "   FROM P_BOP_M_PROC_CHILD (NOLOCK) ";
            QUERY += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND MAKEORDER_NO='"+ MAKEORDER_NO +"' ";
			QUERY += "     IF @@ERROR <> 0 ";
			QUERY += "     BEGIN ";
			QUERY += "            SELECT 'ER', '스케쥴 확정중 에러가 발생되었습니다.' ";
			QUERY += "            RETURN ";
			QUERY += "     END ";

			QUERY += " INSERT INTO P_BOP_M_PROC_DETAIL_TEMP (CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PLANT_CD, ITEM_CD, ROUT_NO,  ";
			QUERY += "        PROC_SEQ,   WC_CD,     INSIDE_FLG, RES_CD, ROUT_ORDER, JOB_CD, MFG_LT, TIME_UNIT, RUN_TIME,  ";
			QUERY += "        SETUP_TIME, WAIT_TIME, MOVE_TIME,  MOVE_QTY, QUEUE_TIME, RUN_TIME_QTY, RUN_TIME_UNIT, MILESTONE_FLG,  ";
			QUERY += "        INSP_FLG, OVERLAP_OPR, OVERLAP_LT, SUBCONTRACT_PRC, CUR_CD, BP_CD, TAX_TYPE, RUN_TIME_RULE_PLAN,  ";
			QUERY += "        RUN_TIME_RULE_SCHL, RUN_TIME_PLAN, RUN_TIME_SCHL, SETUP_TIME_IN_HOUR_PLAN, SETUP_TIME_IN_HOUR,  ";
			QUERY += "        SETUP_TIME_RULE, WHEN_TO_SETUP_RULE, WAIT_TIME_IN_HOUR, MOVE_TIME_IN_HOUR, QUEUE_TIME_IN_HOUR,  ";
			QUERY += "        ALLOC_RULE, SELECT_RULE, FREE_CHECK_FLG, HOLD_TEMP_FLG, NEXT_PROC_SEQ, OPR_SEQ, SETUP_BASE_CD,  ";
			QUERY += "        CROSS_BREAK_RULE, OVERLAP_TYPE, OVERLAP_VALUE, PROD_RATE, SETUP_RSC_GRP_CD, SETUP_RSC_CD,  ";
			QUERY += "        RSC_SCHEDULED_FLG, VALID_FROM_DT, VALID_TO_DT, INSRT_USER_ID, INSRT_DT, UPDT_USER_ID, UPDT_DT,  ";
			QUERY += "        FIX_RUN_TIME, REMARK, CUST_SIZE, NEED_QTY, NEED_QTY_UNIT, WORK_QTY, WORK_QTY_UNIT, RESULT_QTY,  ";
			QUERY += "        RESULT_QTY_UNIT, START_DT, START_TM, START_SC, END_DT, END_TM, END_SC, BF_START_DT, BF_START_TM,  ";
			QUERY += "        BF_START_SC, BF_END_DT, BF_END_TM, BF_END_SC, RESULT_START_DT, RESULT_START_TM, RESULT_END_DT,  ";
			QUERY += "        RESULT_END_TM, COMPLET_YN, WORK_TM, FRONT_GAB_TIME, ORDER_SUM, RESULT_SUM, MAKE_RATE, MENUAL_FLG,  ";
			QUERY += "        CLOSE_FLG, SCH_YN, MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, SCH_ID, FIG_NO) ";
			QUERY += " SELECT CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD,   PLANT_CD, ITEM_CD, ROUT_NO,   PROC_SEQ, WC_CD,  ";
			QUERY += "        INSIDE_FLG, RES_CD,      ROUT_ORDER, JOB_CD,   MFG_LT,  TIME_UNIT, RUN_TIME, SETUP_TIME, WAIT_TIME,  ";
			QUERY += "        MOVE_TIME, MOVE_QTY, QUEUE_TIME, RUN_TIME_QTY, RUN_TIME_UNIT, MILESTONE_FLG, INSP_FLG, OVERLAP_OPR,  ";
			QUERY += "        OVERLAP_LT, SUBCONTRACT_PRC, CUR_CD, BP_CD, TAX_TYPE, RUN_TIME_RULE_PLAN, RUN_TIME_RULE_SCHL,  ";
			QUERY += "        RUN_TIME_PLAN, RUN_TIME_SCHL, SETUP_TIME_IN_HOUR_PLAN, SETUP_TIME_IN_HOUR, SETUP_TIME_RULE,  ";
			QUERY += "        WHEN_TO_SETUP_RULE, WAIT_TIME_IN_HOUR, MOVE_TIME_IN_HOUR, QUEUE_TIME_IN_HOUR, ALLOC_RULE,  ";
			QUERY += "        SELECT_RULE, FREE_CHECK_FLG, HOLD_TEMP_FLG, NEXT_PROC_SEQ, OPR_SEQ, SETUP_BASE_CD, CROSS_BREAK_RULE,  ";
			QUERY += "        OVERLAP_TYPE, OVERLAP_VALUE, PROD_RATE, SETUP_RSC_GRP_CD, SETUP_RSC_CD, RSC_SCHEDULED_FLG,  ";
			QUERY += "        VALID_FROM_DT, VALID_TO_DT, INSRT_USER_ID, INSRT_DT, '"+ USER_ID +"', GETDATE(), FIX_RUN_TIME, REMARK,  ";
			QUERY += "        CUST_SIZE, NEED_QTY, NEED_QTY_UNIT, WORK_QTY, WORK_QTY_UNIT, RESULT_QTY, RESULT_QTY_UNIT, START_DT,  ";
			QUERY += "        START_TM, START_SC, END_DT, END_TM, END_SC, BF_START_DT, BF_START_TM, BF_START_SC, BF_END_DT, BF_END_TM,  ";
			QUERY += "        BF_END_SC, RESULT_START_DT, RESULT_START_TM, RESULT_END_DT, RESULT_END_TM, COMPLET_YN, WORK_TM,  ";
			QUERY += "        FRONT_GAB_TIME, ORDER_SUM, RESULT_SUM, MAKE_RATE, MENUAL_FLG, CLOSE_FLG, '"+ SCH_YN +"',  ";
			QUERY += "        MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, '"+ SCH_ID +"', FIG_NO  ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL (NOLOCK) ";
			QUERY += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND MAKEORDER_NO='"+ MAKEORDER_NO +"' ";

			QUERY += "    AND ROUT_NO <> '999' ";

			QUERY += "     IF @@ERROR <> 0 ";
			QUERY += "     BEGIN ";
			QUERY += "            SELECT 'ER', '스케쥴 확정중 에러가 발생되었습니다.' ";
			QUERY += "            RETURN ";
			QUERY += "     END ";

			QUERY += " INSERT INTO P_BOP_M_PROC_MASTER_TEMP (CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PLANT_CD, ITEM_CD, ROUT_NO,  ";
			QUERY += "        DESCRIPTION, BOM_NO, MAJOR_FLG, VALID_FROM_DT, VALID_TO_DT, INSRT_USER_ID, INSRT_DT,  ";
			QUERY += "        UPDT_USER_ID, UPDT_DT, ALT_RT_VALUE, SCH_YN, MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, SCH_ID) ";
			QUERY += " SELECT CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PLANT_CD, ITEM_CD, ROUT_NO, DESCRIPTION, BOM_NO,  ";
			QUERY += "        MAJOR_FLG, VALID_FROM_DT, VALID_TO_DT, INSRT_USER_ID, INSRT_DT, '"+ USER_ID +"', GETDATE(),  ";
			QUERY += "        ALT_RT_VALUE, '"+ SCH_YN +"', MAKEORDER_NO, WORKORDER_NO, WORKORDER_NO_OG, '"+ SCH_ID +"'  ";
			QUERY += "   FROM P_BOP_M_PROC_MASTER (NOLOCK) ";
			QUERY += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND MAKEORDER_NO='"+ MAKEORDER_NO +"' ";

			QUERY += "    AND ROUT_NO <> '999' ";

			QUERY += "     IF @@ERROR <> 0 ";
			QUERY += "     BEGIN ";
			QUERY += "            SELECT 'ER', '스케쥴 확정중 에러가 발생되었습니다.' ";
			QUERY += "            RETURN ";
			QUERY += "     END ";

			QUERY += " SELECT 'OK', '성공적으로 처리되었습니다.' ";

			return QUERY;

		}
		#endregion

		#region MAKEQTY_UP(생산수량) 계산하기 위한 데이타 SELECT
		public static string MAKEQTY_UP(string WORK_QTY, 
			string PROJECT_NO, 
			string PROJECT_SEQ, 
			string GROUP_CD, 
			string PLANT_CD, 
			string ITEM_CD, 
			string SCH_ID, 
			string WORKORDER_NO)
		{
			string QUERY= "";

			QUERY += " UPDATE A SET WORK_QTY = '"	+ WORK_QTY +"', ";
			QUERY += "        A.WORK_QTY_UNIT = A.NEED_QTY_UNIT ";
			QUERY += "   FROM P_BOP_M_MASTER_TEMP A ";
			QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.PROJECT_NO = '"		+ PROJECT_NO +"' ";
			QUERY += "    AND A.PROJECT_SEQ = '"	+ PROJECT_SEQ +"' ";
			QUERY += "    AND A.GROUP_CD = '"		+ GROUP_CD +"' ";
			QUERY += "    AND A.PLANT_CD = '"		+ PLANT_CD +"' ";
			QUERY += "    AND A.ITEM_CD = '"		+ ITEM_CD +"' ";
			QUERY += "    AND A.SCH_ID = '"			+ SCH_ID +"' ";
			QUERY += "    AND A.WORKORDER_NO = '"	+ WORKORDER_NO +"' ";

			QUERY += " UPDATE A SET WORK_QTY = '"	+ WORK_QTY +"', ";
			QUERY += "        A.WORK_QTY_UNIT = A.NEED_QTY_UNIT ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A ";
			QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.PROJECT_NO = '" + PROJECT_NO + "' ";
			QUERY += "    AND A.PROJECT_SEQ = '"	+ PROJECT_SEQ +"' ";
			QUERY += "    AND A.GROUP_CD = '"		+ GROUP_CD +"' ";
			QUERY += "    AND A.PLANT_CD = '"		+ PLANT_CD +"' ";
			QUERY += "    AND A.ITEM_CD = '"		+ ITEM_CD +"' ";		// 조건 더 추가하여야 함
			QUERY += "    AND A.SCH_ID = '"			+ SCH_ID +"' ";
			QUERY += "    AND A.WORKORDER_NO = '"	+ WORKORDER_NO +"' ";

			return QUERY;
		}
		#endregion

		#region WORKORDER_MAKE_LIST(생산수량) 계산하기 위한 데이타 SELECT
		public static string WORKORDER_MAKE_LIST(string SCH_ID, string MAKEORDER_NO)
		{
			string QUERY= "";

			QUERY += " SELECT PROJECT_NO, PROJECT_SEQ, GROUP_CD, PRNT_PLANT_CD, PRNT_ITEM_CD,  ";
			QUERY += "        PRNT_BOM_NO, CHILD_ITEM_SEQ, MAKEORDER_NO, SCH_ID ";
			QUERY += "   FROM P_BOP_M_DETAIL_TEMP(NOLOCK) ";
            QUERY += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND SCH_ID = '"+ SCH_ID +"'  ";
			QUERY += "    AND MAKEORDER_NO ='"+ MAKEORDER_NO +"' ";

			return QUERY;
		}
		#endregion

		#region WORK_TM_UP() 작업시간 업데이트
		public static string WORK_TM_UP(string MAKEORDER_NO, string SCH_ID, string DPLY_DIR)
		{
			string QUERY= "";

			QUERY += " UPDATE A SET ";
			QUERY += "        A.WORK_TM = CASE WHEN (CASE WHEN B.RES_KIND = 'O' THEN ";
			QUERY += "                                   (CASE WHEN ISNULL(A.MFG_LT,0) = 0 THEN 11 ELSE A.MFG_LT END) * 480 ";    // MFG_LT가분단위로 적용한다.
			QUERY += "                               WHEN B.RES_KIND = 'G' THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
			QUERY += "                    END) < 1 THEN 1 ";
			QUERY += "                          ELSE (CASE WHEN B.RES_KIND = 'O' THEN ";
			QUERY += "                                   (CASE WHEN ISNULL(A.MFG_LT,0) = 0 THEN 11 ELSE A.MFG_LT END) * 480 ";    // MFG_LT가분단위로 적용한다.
			QUERY += "                               WHEN B.RES_KIND = 'G' THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
			QUERY += "                    END) END ";



//			QUERY += "        A.WORK_TM = CASE WHEN (CASE WHEN B.RES_KIND = 'O' THEN ";
//			QUERY += "                              A.MFG_LT ";    // MFG_LT가 일단위로 적용한다.
//			QUERY += "                         WHEN B.RES_KIND = 'G' THEN ";
//			QUERY += "                              ((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) + ISNULL(A.SETUP_TIME, 0))  / ISNULL(A.RUN_TIME_QTY, 1)";
//			QUERY += "                         WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
//			QUERY += "                             (((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0))) ";
//			QUERY += "                             + ISNULL(A.SETUP_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1) "; // + ISNULL(A.WAIT_TIME, 0) + ISNULL(A.MOVE_TIME, 0) + ISNULL(A.QUEUE_TIME, 0) + ISNULL(A.FIX_RUN_TIME, 0) ) ";
//			QUERY += "                         WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
//			QUERY += "                    END) < 1 THEN 1  ";
//			QUERY += "                         ELSE (CASE WHEN B.RES_KIND = 'O' THEN ";
//			QUERY += "                              A.MFG_LT ";    // MFG_LT가 일단위로 적용한다.
//			QUERY += "                         WHEN B.RES_KIND = 'G' THEN ";
//			QUERY += "                              ((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) + ISNULL(A.SETUP_TIME, 0))  / ISNULL(A.RUN_TIME_QTY, 1)";
//			QUERY += "                         WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
//			QUERY += "                             (((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0))) ";
//			QUERY += "                             + ISNULL(A.SETUP_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1) "; // + ISNULL(A.WAIT_TIME, 0) + ISNULL(A.MOVE_TIME, 0) + ISNULL(A.QUEUE_TIME, 0) + ISNULL(A.FIX_RUN_TIME, 0) ) ";
//			QUERY += "                        WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
//			QUERY += "                    END) END ";

			QUERY += "        ,A.WORK_TM_LOAD = CASE WHEN (CASE WHEN B.RES_KIND = 'O' THEN ";
			QUERY += "                                   (CASE WHEN ISNULL(A.MFG_LT,0) = 0 THEN 11 ELSE A.MFG_LT END) * 480 ";    // MFG_LT가분단위로 적용한다.
			QUERY += "                               WHEN B.RES_KIND = 'G' THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
			QUERY += "                    END) < 1 THEN 1 ";
			QUERY += "                          ELSE (CASE WHEN B.RES_KIND = 'O' THEN ";
			QUERY += "                                   (CASE WHEN ISNULL(A.MFG_LT,0) = 0 THEN 11 ELSE A.MFG_LT END) * 480 ";    // MFG_LT가분단위로 적용한다.
			QUERY += "                               WHEN B.RES_KIND = 'G' THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
			QUERY += "                    END) END ";
			QUERY += "        ,A.WORK_TM_STD = CASE WHEN (CASE WHEN B.RES_KIND = 'O' THEN ";
			QUERY += "                                   (CASE WHEN ISNULL(A.MFG_LT,0) = 0 THEN 11 ELSE A.MFG_LT END) * 480 ";    // MFG_LT가분단위로 적용한다.
			QUERY += "                               WHEN B.RES_KIND = 'G' THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
			QUERY += "                    END) < 1 THEN 1 ";
			QUERY += "                     ELSE (CASE WHEN B.RES_KIND = 'O' THEN ";
			QUERY += "                                   (CASE WHEN ISNULL(A.MFG_LT,0) = 0 THEN 11 ELSE A.MFG_LT END) * 480 ";    // MFG_LT가분단위로 적용한다.
			QUERY += "                               WHEN B.RES_KIND = 'G' THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) > 0 THEN ";
			QUERY += "                                   ROUND(((ISNULL(A.WORK_QTY, 0) * ISNULL(A.RUN_TIME, 0)) / ISNULL(A.RUN_TIME_QTY, 1)) + ISNULL(A.SETUP_TIME, 0),0) ";
			QUERY += "                               WHEN B.RES_KIND <> 'O' AND (ISNULL(B.MAKE_POW1, 0) + ISNULL(B.MAKE_POW2, 0) + ISNULL(B.MAKE_POW3, 0)) = 0 THEN 0 ";
			QUERY += "                    END) END ";

			QUERY += " FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";
			QUERY += " LEFT OUTER JOIN P_RESO_MANAGE B(NOLOCK) ";
			QUERY += " ON A.CO_CD = B.CO_CD ";
            QUERY += " AND A.RES_CD = B.RES_CD ";
			QUERY += " WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += " AND A.RESULT_QTY = 0 ";
			QUERY += " AND A.CLOSE_FLG = 'N' ";
			QUERY += " AND A.SCH_YN = 'Y' ";
			QUERY += " AND A.MAKEORDER_NO = '"	+ MAKEORDER_NO +"' ";
			QUERY += " AND A.SCH_ID = '"		+ SCH_ID +"' ";

			// 정전개 일 경우만 처리
			if(DPLY_DIR == "FWD") 
			{
				//	CMLT(누적 L/T) 업데이트 시작
				QUERY += " DECLARE @BOP_PROC_CMLT TABLE ";
				QUERY += " (CO_CD       VARCHAR(10), ";
				QUERY += " 	PROJECT_NO  VARCHAR(50), ";
				QUERY += " 	PROJECT_SEQ VARCHAR(20), ";
				QUERY += " 	GROUP_CD    VARCHAR(30), ";
				QUERY += " 	PLANT_CD    VARCHAR(4), ";
				QUERY += " 	ROUT_NO     VARCHAR(4), ";
				QUERY += " 	ITEM_CD     VARCHAR(30), ";
				QUERY += " 	FIG_NO      VARCHAR(50), ";
				QUERY += " 	SUM_WORK_TM INT, ";
				QUERY += " 	FIG_LVL     INT ";
				QUERY += " ) ";
				QUERY += " INSERT INTO @BOP_PROC_CMLT ( ";
				QUERY += " 	CO_CD,      PROJECT_NO,	PROJECT_SEQ, GROUP_CD,    PLANT_CD,    ROUT_NO, ";
				QUERY += "  ITEM_CD,    FIG_NO,      SUM_WORK_TM, FIG_LVL ";
				QUERY += " ) ";
				QUERY += " SELECT A.CO_CD,       A.PROJECT_NO,  A.PROJECT_SEQ, A.GROUP_CD,  A.PLANT_CD,    ";
				QUERY += "        A.ROUT_NO,     A.ITEM_CD,     A.FIG_NO,    A.SUM_WORK_TM, ";
				QUERY += " 	      A.FIG_LVL ";
				QUERY += " FROM   ( ";
				QUERY += "         SELECT  CO_CD,       PROJECT_NO,   PROJECT_SEQ,  GROUP_CD,   PLANT_CD, ";
				QUERY += "                 ROUT_NO,     ITEM_CD,      FIG_NO,       FIG_LVL,  ";
				QUERY += "                 SCH_ID,      MAKEORDER_NO, SUM(WORK_TM_LOAD) SUM_WORK_TM ";
				QUERY += "         FROM    P_BOP_M_PROC_DETAIL_TEMP      ";
				QUERY += "         WHERE   CO_CD = '"+SystemBase.Base.gstrCOMCD.ToString()+"'";
                QUERY += "         AND     MAKEORDER_NO = '" + MAKEORDER_NO + "'";
				QUERY += "         AND     SCH_ID = '" + SCH_ID + "' ";
				QUERY += "         AND     SCH_YN = 'Y' ";
				QUERY += "         GROUP   BY 	CO_CD,      PROJECT_NO,	PROJECT_SEQ, GROUP_CD, PLANT_CD, ROUT_NO, ";
				QUERY += "                      ITEM_CD,    FIG_NO,      FIG_LVL,  SCH_ID,   MAKEORDER_NO ";
				QUERY += "         ) A ";
				QUERY += " ORDER BY FIG_LVL; ";

				QUERY += " WITH BOP_PROC_CMLT AS ( ";
				QUERY += "       SELECT  A.CO_CD, A.PROJECT_NO, A.PROJECT_SEQ, A.GROUP_CD, A.PLANT_CD,    ";
				QUERY += "               A.ROUT_NO,    A.ITEM_CD,     A.FIG_NO,   A.SUM_WORK_TM, ";
				QUERY += "               A.SUM_WORK_TM CMLT,          A.FIG_LVL ";
				QUERY += "       FROM    @BOP_PROC_CMLT A ";
                QUERY += "       WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";  
                QUERY += "         AND FIG_LVL = 1 ";
				QUERY += "       UNION ALL ";
				QUERY += "       SELECT  A.CO_CD,      A.PROJECT_NO, A.PROJECT_SEQ, A.GROUP_CD,   A.PLANT_CD, ";
				QUERY += "               A.ROUT_NO,    A.ITEM_CD,     A.FIG_NO,     A.SUM_WORK_TM, ";
				QUERY += "              (A.SUM_WORK_TM + B.CMLT) CMLT,              A.FIG_LVL      ";
				QUERY += "       FROM    @BOP_PROC_CMLT A ";
				QUERY += "               INNER JOIN BOP_PROC_CMLT B    ";
                QUERY += "               ON  A.CO_CD = B.CO_CD ";
                QUERY += "               AND A.FIG_LVL - 1 = B.FIG_LVL ";
				QUERY += "               AND LEFT(A.FIG_NO, LEN(B.FIG_NO)) = B.FIG_NO ";
				QUERY += " ) ";
				QUERY += " UPDATE A ";
				QUERY += " SET    A.CMLT = B.CMLT, ";
				QUERY += "        A.FIG_LVL = B.FIG_LVL ";
				QUERY += " FROM   P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";
				QUERY += "        LEFT JOIN (SELECT CO_CD,       PROJECT_NO,  PROJECT_SEQ, GROUP_CD,    PLANT_CD,    ";
				QUERY += "                          ROUT_NO,     ITEM_CD,     FIG_NO,      SUM_WORK_TM, ";
				QUERY += "                          CMLT,        FIG_LVL ";
				QUERY += "                   FROM   BOP_PROC_CMLT) B ";
                QUERY += "        ON  A.CO_CD = B.CO_CD ";
                QUERY += "        AND A.PROJECT_NO = B.PROJECT_NO ";
				QUERY += "        AND A.PROJECT_SEQ = B.PROJECT_SEQ ";
				QUERY += "        AND A.GROUP_CD = B.GROUP_CD ";
				QUERY += "        AND A.PLANT_CD = B.PLANT_CD ";
				QUERY += "        AND A.ROUT_NO = B.ROUT_NO ";
				QUERY += "        AND A.ITEM_CD = B.ITEM_CD ";
                QUERY += " WHERE  A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                QUERY += " AND    A.SCH_ID = '" + SCH_ID + "' ";
				QUERY += " AND    A.MAKEORDER_NO = '" + MAKEORDER_NO + "'";

				//	CMLT(누적 L/T) 업데이트 종료
			}

			return QUERY;
		}
		#endregion

		#region PROC_LIST() 시작 종료일자 업데이트 하기 위한 리스트 조회
		public static string PROC_LIST(string SCH_ID, string MAKEORDER_NO, string FW_BK, string PROJECT_NO, string PROJECT_SEQ)
		{
			string QUERY= "";

			QUERY += " SELECT A.FIG_NO, A.PROJECT_NO, A.PROJECT_SEQ, A.GROUP_CD, A.PLANT_CD, A.ITEM_CD, ";
			QUERY += "        A.ROUT_NO, A.PROC_SEQ, A.OPR_NO, A.SCH_ID, A.MAKEORDER_NO, A.WORKORDER_NO, A.RES_CD, C.RES_KIND, ";
			// 자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
			/*	그룹 이미 적용하여 저장하였으므로 여기서는 적용하면 안됨
			QUERY += "         CASE WHEN C.RES_KIND = 'G' AND ISNULL(A.WORK_TM, 0) > 0 AND ISNULL(J.MAKE_POW, 0) > 0 ";
			QUERY += "              THEN ISNULL(A.WORK_TM, 0)  / ISNULL(J.MAKE_POW, 0) ";
			QUERY += "              ELSE ISNULL(A.WORK_TM, 0) ";
			QUERY += "              END WORK_TM, ";
			*/
			// 자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
			QUERY += "        A.WORK_TM, '' START_DT, '' START_TM, '' END_DT, '' END_TM, A.CMLT  ";
			if(FW_BK == "F")
			{	// 정전개일 경우
				QUERY += "        ,(CASE WHEN G.PROJECT_NO IS NULL THEN 'Y' ELSE 'N' END) ST_YN";
			}

			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";
//			QUERY += "   LEFT OUTER JOIN P_BOP_M_DETAIL_TEMP B(NOLOCK) ";
//			QUERY += "     ON A.PROJECT_NO     = B.PROJECT_NO ";
//			QUERY += "    AND A.PROJECT_SEQ    = B.PROJECT_SEQ ";
//			QUERY += "    AND A.GROUP_CD       = B.GROUP_CD ";
//			QUERY += "    AND A.PLANT_CD       = B.PRNT_PLANT_CD ";
//			//QUERY += "    AND A.PRNT_ITEM_CD   = B.PRNT_ITEM_CD ";
//			QUERY += "    AND A.ITEM_CD        = B.CHILD_ITEM_CD ";
			QUERY += "   LEFT OUTER JOIN P_RESO_MANAGE C(NOLOCK) ";
            QUERY += "     ON A.CO_CD          = C.CO_CD ";
            QUERY += "    AND A.RES_CD         = C.RES_CD ";

			if(FW_BK == "F")
			{	// 정전개일 경우
				QUERY += "     LEFT OUTER JOIN (SELECT DISTINCT CO_CD, PROJECT_NO, PROJECT_SEQ, GROUP_CD, PLANT_CD, ROUT_NO ";
				QUERY += "                        FROM P_BOP_M_PROC_DETAIL_TEMP (NOLOCK)) G ";
                QUERY += "       ON A.CO_CD          = G.CO_CD ";
                QUERY += "      AND A.PROJECT_NO     = G.PROJECT_NO ";
				QUERY += "      AND A.PROJECT_SEQ    = G.PROJECT_SEQ ";
				QUERY += "      AND A.GROUP_CD       = G.GROUP_CD ";
				QUERY += "      AND A.PLANT_CD       = G.PLANT_CD ";
				//QUERY += "      AND A.ITEM_CD        = G.PRNT_ITEM_CD ";
				QUERY += "      AND A.ROUT_NO        = G.ROUT_NO ";
			}

			//	자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
			QUERY += "  LEFT OUTER JOIN ( SELECT H.CO_CD, H.BIZ_CD, H.PLANT_CD, H.RES_CD, ";
			QUERY += "                           (SUM(H.MAKE_POW1) + SUM(H.MAKE_POW2) + SUM(H.MAKE_POW3)) MAKE_POW ";
			QUERY += "                      FROM P_RESO_MANAGE H(NOLOCK) ";
			QUERY += "                      LEFT OUTER JOIN P_RESO_GROUP I(NOLOCK) ";
            QUERY += "                        ON H.CO_CD = I.CO_CD ";
            QUERY += "                       AND H.RES_CD = I.GRES_CD ";
			QUERY += "                     WHERE I.GRES_CD IS NOT NULL ";
			QUERY += "                     GROUP BY H.CO_CD, H.BIZ_CD, H.PLANT_CD, H.RES_CD) J ";
            QUERY += "    ON A.CO_CD = J.CO_CD ";
            QUERY += "    AND A.RES_CD = J.RES_CD ";
			//	자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
            QUERY += "  WHERE A.CO_CD          = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.SCH_ID         = '"+ SCH_ID +"' ";
			QUERY += "    AND A.MAKEORDER_NO   = '"+ MAKEORDER_NO +"' ";
			QUERY += "    AND A.SCH_YN         = 'Y' ";

			if(PROJECT_NO.Length > 0 && PROJECT_SEQ.Length > 0)
				QUERY += "    AND A.PROJECT_NO     = '"+ PROJECT_NO +"' AND A.PROJECT_SEQ = '"+ PROJECT_SEQ +"' ";
			else if(PROJECT_NO.Length > 0)
				QUERY += "    AND A.PROJECT_NO     = '"+ PROJECT_NO +"' ";

			if(FW_BK == "F")	// 정전개일 경우
				QUERY += "  ORDER BY A.FIG_NO, OPR_NO ";
			else				//	역전개일 경우
				QUERY += "  ORDER BY LEN(A.FIG_NO), A.FIG_NO, OPR_NO DESC, A.CMLT DESC ";

			return QUERY;
		}


		public static string PROC_LIST(string SCH_ID, string MAKEORDER_NO, string PROJECT_NO, string PROJECT_SEQ)
		{	// 무한 역전개(긴급작지용)
			string QUERY= "";

			QUERY += " SELECT A.FIG_NO, A.PROJECT_NO, A.PROJECT_SEQ, A.GROUP_CD, A.PLANT_CD, A.ITEM_CD, ";
			QUERY += "        A.ROUT_NO, A.PROC_SEQ, A.OPR_NO, A.SCH_ID, A.MAKEORDER_NO, A.WORKORDER_NO, A.RES_CD, C.RES_KIND, ";
			// 자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
			/*	그룹 이미 적용하여 저장하였으므로 여기서는 적용하면 안됨
			QUERY += "         CASE WHEN C.RES_KIND = 'G' AND ISNULL(A.WORK_TM, 0) > 0 AND ISNULL(J.MAKE_POW, 0) > 0 ";
			QUERY += "              THEN ISNULL(A.WORK_TM, 0)  / ISNULL(J.MAKE_POW, 0) ";
			QUERY += "              ELSE ISNULL(A.WORK_TM, 0) ";
			QUERY += "              END WORK_TM, ";
			*/
			// 자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
			QUERY += "        A.WORK_TM, '' START_DT, '' START_TM, '' END_DT, '' END_TM, A.CMLT  ";

			QUERY += "   FROM P_BOP_M_PROC_DETAIL A(NOLOCK) ";
			QUERY += "   LEFT OUTER JOIN P_RESO_MANAGE C(NOLOCK) ";
            QUERY += "     ON A.CO_CD          = C.CO_CD ";
            QUERY += "    AND A.RES_CD         = C.RES_CD ";

			//	자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
			QUERY += "  LEFT OUTER JOIN ( SELECT H.CO_CD, H.BIZ_CD, H.PLANT_CD, H.RES_CD, ";
			QUERY += "                           (SUM(H.MAKE_POW1) + SUM(H.MAKE_POW2) + SUM(H.MAKE_POW3)) MAKE_POW ";
			QUERY += "                      FROM P_RESO_MANAGE H(NOLOCK) ";
			QUERY += "                      LEFT OUTER JOIN P_RESO_GROUP I(NOLOCK) ";
            QUERY += "                        ON H.CO_CD = I.CO_CD ";
            QUERY += "                       AND H.RES_CD = I.GRES_CD ";
			QUERY += "                     WHERE I.GRES_CD IS NOT NULL ";
			QUERY += "                     GROUP BY H.CO_CD, H.BIZ_CD, H.PLANT_CD, H.RES_CD) J ";
            QUERY += "    ON A.CO_CD = J.CO_CD ";
            QUERY += "   AND A.RES_CD = J.RES_CD ";
			//	자원유형이 그룹인 경우 시수를 그룹의 인원수 만큼 나눠서 조회하여 능력을 늘린다.
            QUERY += "  WHERE A.CO_CD          = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.SCH_ID         = '"+ SCH_ID +"' ";
			QUERY += "    AND A.MAKEORDER_NO   = '"+ MAKEORDER_NO +"' ";
			QUERY += "    AND A.SCH_YN         = 'Y' ";

			if(PROJECT_NO.Length > 0 && PROJECT_SEQ.Length > 0)
				QUERY += "    AND A.PROJECT_NO     = '"+ PROJECT_NO +"' AND A.PROJECT_SEQ = '"+ PROJECT_SEQ +"' ";
			else if(PROJECT_NO.Length > 0)
				QUERY += "    AND A.PROJECT_NO     = '"+ PROJECT_NO +"' ";

			QUERY += "  ORDER BY A.FIG_NO, OPR_NO ";

			return QUERY;
		}
		#endregion

		#region FRONT_TIME_QUERY() 전공정 종료일자 추출
		public static string FRONT_TIME_QUERY(string RES_CD, string TM)
		{	////////////
			string QUERY= "";

			QUERY += " SELECT TOP 1 A.END_DT, A.END_TM ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";

			QUERY += "  LEFT OUTER JOIN P_RESO_MANAGE B(NOLOCK) ";
            QUERY += "    ON A.CO_CD = B.CO_CD ";
            QUERY += "   AND A.RES_CD = B.RES_CD ";

            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.RES_CD = '"		+ RES_CD +"' ";
			QUERY += "    AND B.RES_KIND <> 'O' ";

//			QUERY += "    AND PROJECT_NO = '"	+ PROJECT_NO +"' ";
//			QUERY += "    AND PROJECT_SEQ = '"	+ PROJECT_SEQ +"' ";
//			QUERY += "    AND GROUP_CD = '"		+ GROUP_CD +"' ";
//			QUERY += "    AND PRNT_ITEM_CD = '"	+ PRNT_ITEM_CD +"' ";
//			QUERY += "    AND PLANT_CD = '"		+ PLANT_CD +"' ";
//			QUERY += "    AND ITEM_CD = '"		+ ITEM_CD +"' ";
//			QUERY += "    AND ROUT_NO = '"		+ ROUT_NO +"' ";

			QUERY += "    AND A.WORK_TM > 0 ";
			QUERY += "    AND A.END_DT + A.END_TM <= '"+ TM +"' ";
			QUERY += "    AND A.END_DT > '' ";
			QUERY += "    AND A.END_DT IS NOT NULL ";
			QUERY += "    ORDER BY (A.END_DT + A.END_TM) DESC, A.OPR_NO ";

			return QUERY;
		}
		#endregion

		#region FRONT_TIME_QUERY() 같은 레벨에 전공정 감안하기 위한 종료일자 조회
		public static string FRONT_TIME_QUERY(
			string SCH_ID, 
			string MAKEORDER_NO, 
			//string PRNT_ITEM_CD, 
			string ITEM_CD, 
			string FIG_NO)
		{
			string QUERY= "";

			QUERY += " SELECT TOP 1 A.END_DT, A.END_TM  ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";

			QUERY += "  LEFT OUTER JOIN P_RESO_MANAGE B(NOLOCK) ";
            QUERY += "    ON A.CO_CD = B.CO_CD ";
            QUERY += "   AND A.RES_CD = B.RES_CD ";

            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.SCH_ID = '"		+ SCH_ID +"' ";

			QUERY += "    AND B.RES_KIND <> 'O' ";

			QUERY += "    AND A.MAKEORDER_NO='"	+ MAKEORDER_NO +"' ";
			//QUERY += "    AND PRNT_ITEM_CD ='"+ PRNT_ITEM_CD +"' ";
			QUERY += "    AND A.ITEM_CD = '"	+ ITEM_CD +"' ";
			QUERY += "    AND LEN(A.FIG_NO) = "	+ FIG_NO +" ";
			QUERY += "    AND A.END_DT > '' ";
			QUERY += "  ORDER BY (A.START_DT + A.START_TM + A.END_DT + A.END_TM) DESC, A.OPR_NO ";

			return QUERY;
		}
		#endregion

		#region AFTER_TIME_QUERY() 정전개 - 같은 자원에 후공정 감안하기 위한 시작료일자 조회()
		public static string AFTER_TIME_QUERY(string RES_CD, string TM)
		{
			string QUERY= "";

			QUERY += " SELECT TOP 1 A.START_DT, A.START_TM  ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";

			QUERY += "  LEFT OUTER JOIN P_RESO_MANAGE B(NOLOCK) ";
            QUERY += "    ON A.CO_CD = B.CO_CD ";
            QUERY += "   AND A.RES_CD = B.RES_CD ";


            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.RES_CD = '"		+ RES_CD +"' ";
			QUERY += "    AND B.RES_KIND <> 'O' ";

//			QUERY += "    AND MAKEORDER_NO='"	+ MAKEORDER_NO +"' ";
//			QUERY += "    AND PRNT_ITEM_CD = '"	+ PRNT_ITEM_CD +"' ";
//			QUERY += "    AND LEN(FIG_NO) = "	+ FIG_NO +" ";

			QUERY += "    AND A.WORK_TM > 0 ";
			QUERY += "    AND A.START_DT + A.START_TM >= '"+ TM +"' ";
			QUERY += "    AND A.START_DT > '' ";
			QUERY += "  ORDER BY (A.START_DT + A.START_TM), A.OPR_NO ";

			return QUERY;
		}
		#endregion

		#region AFTER_TIME_QUERY() 같은 레벨에 전공정 감안하기 위한 시작일자 조회
		public static string AFTER_TIME_QUERY(
			string SCH_ID, 
			string MAKEORDER_NO, 
			//string PRNT_ITEM_CD, 
			string FIG_NO)
		{
			string QUERY= "";

			QUERY += " SELECT TOP 1 A.START_DT, A.START_TM  ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";

			QUERY += "  LEFT OUTER JOIN P_RESO_MANAGE B(NOLOCK) ";
            QUERY += "    ON A.CO_CD = B.CO_CD ";
            QUERY += "   AND A.RES_CD = B.RES_CD ";

            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.SCH_ID = '"		+ SCH_ID +"' ";
			QUERY += "    AND B.RES_KIND <> 'O' ";

			QUERY += "    AND A.MAKEORDER_NO='"	+ MAKEORDER_NO +"' ";
			//QUERY += "    AND PRNT_ITEM_CD= '"+ PRNT_ITEM_CD +"' ";
			QUERY += "    AND LEN(A.FIG_NO) = "	+ FIG_NO +" ";
			QUERY += "    AND A.START_DT > '' ";
			QUERY += "  ORDER BY (A.START_DT + A.START_TM), A.OPR_NO ";

			return QUERY;
		}
		#endregion

		#region UP_TIME_QUERY() 상위 공정 부모 시작시간 MAX 값 추출(*** MAX값보다 FRONT_GAB_TIME 시간 감안해야 할 것으로 판단됨 ***)
		public static string UP_TIME_QUERY(string SCH_ID, 
			string MAKEORDER_NO, 
			string PLANT_CD, 
			string ITEM_CD, 
			string ROUT_NO, 
			string FIG_NO, 
			string END_DT)
		{
			string QUERY= "";

			QUERY += " SELECT TOP 1 A.END_DT, A.END_TM ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";

			QUERY += "   LEFT OUTER JOIN P_RESO_MANAGE B(NOLOCK) ";
            QUERY += "    ON A.CO_CD = B.CO_CD ";
            QUERY += "   AND A.RES_CD = B.RES_CD ";

            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.MAKEORDER_NO ='" + MAKEORDER_NO + "' ";

			QUERY += "    AND B.RES_KIND <> 'O' ";

			QUERY += "    AND A.SCH_ID = '"			+ SCH_ID +"' ";
			QUERY += "    AND A.PLANT_CD = '"		+ PLANT_CD +"' ";
			//QUERY += "    AND A.PRNT_ITEM_CD = '"	+ ITEM_CD +"' ";
			QUERY += "    AND A.ROUT_NO = '"		+ ROUT_NO +"' ";

			QUERY += "    AND A.END_DT + A.END_TM <= '"	+ END_DT +"' ";
			
			//QUERY += "    AND LEN(A.FIG_NO) = "		+ FIG_NO +" ";//부모공정은 길이가 같을 수 없음
			QUERY += "    AND A.END_TM IS NOT NULL ";
			QUERY += "  ORDER BY (A.END_DT + A.END_TM) DESC ";

			return QUERY;
		}
		#endregion

		#region AFTER_TIME_QUERY() 전공정 시작 종료일자 추출
		public static string AFTER_TIME_QUERY(string PROJECT_NO, 
			string PROJECT_SEQ, 
			string GROUP_CD, 
			string PRNT_ITEM_CD, 
			string PLANT_CD, 
			string ITEM_CD, 
			string ROUT_NO, 
			string TM)
		{
			string QUERY= "";

			QUERY += " SELECT TOP 1 A.START_DT, A.START_TM ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";

			QUERY += "  LEFT OUTER JOIN P_RESO_MANAGE B(NOLOCK) ";
            QUERY += "    ON A.CO_CD = B.CO_CD ";
            QUERY += "   AND A.RES_CD = B.RES_CD ";

            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.PROJECT_NO = '" + PROJECT_NO + "' ";

			QUERY += "    AND B.RES_KIND <> 'O' ";

			QUERY += "    AND A.PROJECT_SEQ = '"+ PROJECT_SEQ +"' ";
			QUERY += "    AND A.GROUP_CD = '"+ GROUP_CD +"' ";
			//QUERY += "    AND PRNT_ITEM_CD = '"+ PRNT_ITEM_CD +"' ";
			QUERY += "    AND A.PLANT_CD = '"+ PLANT_CD +"' ";
			QUERY += "    AND A.ITEM_CD = '"+ ITEM_CD +"' ";
			QUERY += "    AND A.ROUT_NO = '"+ ROUT_NO +"' ";
			QUERY += "    AND A.END_DT + A.END_TM >= '"+ TM +"' ";
			QUERY += "    AND A.END_DT > '' ";
			QUERY += "    AND A.END_DT IS NOT NULL ";
			QUERY += "    ORDER BY A.START_DT + A.START_TM + A.END_DT + A.END_TM ";

			return QUERY;
		}
		#endregion

		#region WORK_TIME_UP() 작업 시작 종료일자 시간 업데이트
		public static string WORK_TIME_UP()
		{
			string QUERY= "";

			QUERY += " UPDATE A ";
			QUERY += " SET  START_DT = B.START_DT, ";
			QUERY += "      START_TM = B.START_TM, ";
			QUERY += "      END_DT = B.dEND_DT, ";
			QUERY += "      END_TM = B.dEND_TM, ";
			QUERY += "      FRONT_GAB_TIME = B.FRONT_GAB_TIME ";
			QUERY += " FROM P_BOP_M_PROC_DETAIL A ";
			QUERY += "      LEFT JOIN P_BOP_RESO_WORK_TIME_TEMP B(NOLOCK)";
            QUERY += " ON   A.CO_CD = B.CO_CD ";
            QUERY += " AND  A.PROJECT_NO  = B.PROJECT_NO ";
			QUERY += " AND  A.PROJECT_SEQ = B.PROJECT_SEQ ";
			QUERY += " AND  A.GROUP_CD = B.GROUP_CD ";
			QUERY += " AND  A.PLANT_CD = B.PLANT_CD ";
			QUERY += " AND  A.ITEM_CD  = B.ITEM_CD ";
			QUERY += " AND  A.ROUT_NO =  B.ROUT_NO ";
			QUERY += " AND  A.PROC_SEQ = B.PROC_SEQ ";
			QUERY += " AND  A.RES_CD = B.RES_CD ";
			QUERY += " AND  A.SCH_ID = B.SCH_ID ";
            QUERY += " WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return QUERY;
		}


		public static string WORK_TIME_UP(
			string START_DT, 
			string START_TM, 
			string END_DT, 
			string END_TM, 
			int FRONT_GAB_TIME, 
			string PROJECT_NO, 
			string PROJECT_SEQ, 
			string GROUP_CD,
			//string PRNT_ITEM_CD, 
			string PLANT_CD, 
			string ITEM_CD, 
			string ROUT_NO, 
			string PROC_SEQ, 
			string RES_CD, 
			string SCH_ID)
		{
			string QUERY= "";

			QUERY += " UPDATE A SET START_DT = '"+ START_DT +"', START_TM = '"+ START_TM +"', ";
			QUERY += "        END_DT = '"+ END_DT +"', END_TM = '"+ END_TM +"', FRONT_GAB_TIME = '"+ FRONT_GAB_TIME +"'   ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK)   ";
            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.PROJECT_NO = '" + PROJECT_NO + "' ";
			QUERY += "    AND PROJECT_SEQ = '"	+ PROJECT_SEQ +"' ";
			QUERY += "    AND GROUP_CD = '"		+ GROUP_CD +"' ";
			//QUERY += "    AND PRNT_ITEM_CD = '"	+ PRNT_ITEM_CD +"' ";
			QUERY += "    AND PLANT_CD = '"		+ PLANT_CD +"' ";
			QUERY += "    AND ITEM_CD = '"		+ ITEM_CD +"' ";
			QUERY += "    AND ROUT_NO = '"		+ ROUT_NO  +"' ";
			QUERY += "    AND PROC_SEQ = '"		+ PROC_SEQ  +"' ";
			QUERY += "    AND RES_CD = '"		+ RES_CD  +"' ";
			QUERY += "    AND SCH_ID = '"		+ SCH_ID  +"' ";

			return QUERY;
		}

		public static string WORK_TIME_UP(
			string START_DT, 
			string START_TM, 
			string END_DT, 
			string END_TM, 
			string PROJECT_NO, 
			string PROJECT_SEQ, 
			string GROUP_CD,
			//string PRNT_ITEM_CD, 
			string PLANT_CD, 
			string ITEM_CD, 
			string ROUT_NO, 
			string PROC_SEQ, 
			string RES_CD, 
			string SCH_ID)
		{	// 긴급작업지시
			string QUERY= "";

			QUERY += " UPDATE A SET START_DT = '"+ START_DT +"', START_TM = '"+ START_TM +"', ";
			QUERY += "        END_DT = '"+ END_DT +"', END_TM = '"+ END_TM +"' ";	//, FRONT_GAB_TIME = '"+ FRONT_GAB_TIME +"'  
			QUERY += "   FROM P_BOP_M_PROC_DETAIL A(NOLOCK)   ";
            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.PROJECT_NO = '" + PROJECT_NO + "' ";
			QUERY += "    AND PROJECT_SEQ = '"	+ PROJECT_SEQ +"' ";
			QUERY += "    AND GROUP_CD = '"		+ GROUP_CD +"' ";
			//QUERY += "    AND PRNT_ITEM_CD = '"	+ PRNT_ITEM_CD +"' ";
			QUERY += "    AND PLANT_CD = '"		+ PLANT_CD +"' ";
			QUERY += "    AND ITEM_CD = '"		+ ITEM_CD +"' ";
			QUERY += "    AND ROUT_NO = '"		+ ROUT_NO  +"' ";
			QUERY += "    AND PROC_SEQ = '"		+ PROC_SEQ  +"' ";
			QUERY += "    AND RES_CD = '"		+ RES_CD  +"' ";
			QUERY += "    AND SCH_ID = '"		+ SCH_ID  +"' ";

			return QUERY;
		}

		#endregion

		#region GAB_TM_BACK() 후공정 FRONT_GAB_TIME 업데이트
		public static string GAB_TM_BACK(
			string PROJECT_NO, 
			string PROJECT_SEQ, 
			string GROUP_CD, 
			//string PRNT_ITEM_CD, 
			string PLANT_CD, 
			string ITEM_CD, 
			string ROUT_NO, 
			string PROC_SEQ, 
			string TM)
		{
			string QUERY= "";

			QUERY += " UPDATE A SET FRONT_GAB_TIME = '"+ TM +"' ";
			QUERY += "   FROM P_BOP_M_PROC_DETAIL_TEMP A(NOLOCK) ";
            QUERY += "  WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "    AND A.PROJECT_NO = '" + PROJECT_NO + "' ";
			QUERY += "    AND A.PROJECT_SEQ = '"	+ PROJECT_SEQ +"' ";
			QUERY += "    AND A.GROUP_CD = '"		+ GROUP_CD +"' ";
			//QUERY += "    AND A.PRNT_ITEM_CD = '"	+ PRNT_ITEM_CD +"' ";
			QUERY += "    AND A.PLANT_CD = '"		+ PLANT_CD +"' ";
			QUERY += "    AND A.ITEM_CD = '"		+ ITEM_CD +"' ";
			QUERY += "    AND A.PROC_SEQ = '"		+ PROC_SEQ +"' ";
			QUERY += "    AND A.ROUT_NO = '"		+ ROUT_NO +"' ";

			return QUERY;
		}
		#endregion

		#region SCH_MST_UP() 스케쥴 전개결과 업데이트
		public static string SCH_MST_UP(
			string SCH_ID, 
			string ACTIVE, 
			string DEPLOY, 
			string INFINITY, 
			string RESOURCE, 
			string CAPA, 
			string BF_PROCESS, 
			string SCHEDULE_BASE_DT, 
			string SCHEDULE_BASE_TM, 
			string UNFOLD_ST_DT, 
			string UNFOLD_ED_DT, 
			string UP_ID, 
			string UP_DT, 
			string PLANT_CD)
		{
			string QUERY= "";

			QUERY += " IF((SELECT COUNT(*) FROM P_CAL_SCH_MST WHERE SCH_ID = '"+ SCH_ID +"') > 0) ";
			QUERY += " BEGIN ";
			QUERY += "     UPDATE P_CAL_SCH_MST SET ";
			QUERY += "            PLANT_CD = '"			+ PLANT_CD +"', ";
			QUERY += "            ACTIVE = '"			+ ACTIVE +"', ";
			QUERY += "            DEPLOY = '"			+ DEPLOY +"', ";
			QUERY += "            INFINITY = '"			+ INFINITY +"', ";
			QUERY += "            RESOURCE = '"			+ RESOURCE +"', ";
			QUERY += "            CAPA = '"				+ CAPA +"', ";
			QUERY += "            BF_PROCESS = '"		+ BF_PROCESS +"', ";
			QUERY += "            SCHEDULE_BASE_DT = '"	+ SCHEDULE_BASE_DT +"', ";
			QUERY += "            SCHEDULE_BASE_TM = '"	+ SCHEDULE_BASE_TM +"', ";
			QUERY += "            UNFOLD_ST_DT = '"		+ UNFOLD_ST_DT +"', ";
			QUERY += "            UNFOLD_ED_DT = '"		+ UNFOLD_ED_DT +"', ";
			QUERY += "            UP_ID = '"			+ UP_ID +"', ";
			QUERY += "            UP_DT = '"			+ UP_DT +"' ";
            QUERY += "      WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            QUERY += "        AND SCH_ID = '" + SCH_ID + "' ";
			QUERY += " END ";
			QUERY += " ELSE ";
			QUERY += " BEGIN ";
			QUERY += "     INSERT INTO P_CAL_SCH_MST(CO_CD, SCH_ID, PLANT_CD, ACTIVE, DEPLOY, INFINITY, RESOURCE, CAPA, BF_PROCESS,  ";
			QUERY += "            SCHEDULE_BASE_DT, SCHEDULE_BASE_TM, UNFOLD_ST_DT, UNFOLD_ED_DT, UP_ID, UP_DT) ";
			QUERY += "     VALUES('"+SystemBase.Base.gstrCOMCD.ToString()+"', '" + SCH_ID + "', '" + PLANT_CD + "', '" + ACTIVE + "', '" + DEPLOY + "', '" + INFINITY + "', '" + RESOURCE + "', '" + CAPA + "', '" + BF_PROCESS + "',  ";
			QUERY += "            '" + SCHEDULE_BASE_DT + "', '" + SCHEDULE_BASE_TM + "', '" + UNFOLD_ST_DT + "', '" + UNFOLD_ED_DT + "', '" + UP_ID + "', '" + UP_DT + "') ";
			QUERY += " END ";

			return QUERY;
		}
		#endregion

		#region 스케줄 전개후
		public static string MSG()
		{
			/// <summary>
			/// MPS 조회
			/// CMLT(누적 리드타임): 같은 공정에서 누적 리드타임이 작은 공정을 우선순위로 일정을 전개한다
			/// </summary>

			string Query = " SELECT TOP 1 * ";
			Query		+= "   FROM P_BOP_M_PROC_DETAIL_TEMP(NOLOCK) ";
            Query += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            Query += "    AND ISNULL(START_DT, '') = ''";

			return Query;
		}
		#endregion

		#region 확정여부 조회
		public static string CONFIRM_CHECK()
		{
			/// <summary>
			/// MPS 조회
			/// CMLT(누적 리드타임): 같은 공정에서 누적 리드타임이 작은 공정을 우선순위로 일정을 전개한다
			/// </summary>

			string Query = " SELECT TOP 1 ISNULL(CONFIRM_FLAG, 'N') FROM P_SCH_LOG_MASTER(NOLOCK)";
            Query       += "  WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
			Query		+= " ORDER BY SCH_NO DESC";

			return Query;
		}
		#endregion
	}
}
