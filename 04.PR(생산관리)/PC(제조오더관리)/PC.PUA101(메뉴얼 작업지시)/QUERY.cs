using System;

namespace PC.PUA101
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

			//string Query = " SELECT * ";
            string Query = " SELECT TOT_PROC_AMT, CUR_PROC_AMT, CUR_PROC_PER ";     // 2015.09.08. hma 수정
			Query		+= " FROM   P_BOP_SCH_PROC_TEMP A (NOLOCK) ";
			Query		+= " WHERE  PROC_ID = '" +  PUA101.PROC_ID + "'";
            Query       += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

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
            Query += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return Query;

		}

		public static string INS_SCH_PROC(string PROC_TYPE, string PROC_STATUS, string USR_ID)
		{
			/// <summary>
			/// 스케쥴 진행정보 등록
			/// </summary>

			string Query = " INSERT INTO P_BOP_SCH_PROC_TEMP ( ";
			Query		+= "     PROC_ID,    TOT_PROC_AMT,    CUR_PROC_AMT,    CUR_PROC_PER, ";
			Query		+= "     PROC_TYPE,  PROC_STATUS,     IN_ID,           IN_DT,   CO_CD ";
			Query		+= " ) ";
			Query		+= " VALUES ( ";
			Query		+= "    '" + PUA101.PROC_ID + "',  0,                 0,           0, ";
            Query += "    '" + PROC_TYPE + "', '" + PROC_STATUS + "', '" + USR_ID + "',      GETDATE(), '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
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
			Query		+= " WHERE  PROC_ID = '" +  PUA101.PROC_ID + "'";
            Query += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return Query;

		}
		#endregion

		#region 긴급작지 정보 조회
		public static string SRCH_EMG_INFO()
		{
			/// <summary>
			/// 긴급작지 정보 조회
			/// </summary>
			string Query   = " SELECT PROJECT_NO, PROJECT_SEQ,  GROUP_CD,   ITEM_CD, ";
			Query  += "        ITEM_QTY,   MAKEORDER_NO, DELIVERY_DT ";
			Query  += " FROM   P_EMG_REGISTER ";
			Query  += " WHERE  CONF_OBJ_FLG = '1' ";
            Query += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
			Query  += " ORDER  BY MAKEORDER_NO ";

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
            Query += "   WHERE  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return Query;
		}
		#endregion
	}
}
