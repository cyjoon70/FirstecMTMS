using System;

namespace PC.PUA101
{ 
	/// <summary>
	/// QUERY�� ���� ��� �����Դϴ�.
	/// </summary>
	public class QUERY
	{
		public QUERY(){}

		#region ������ �����/������� ó��
		public static string SCH_PROC_RATE()
		{
			/// <summary>
			/// ������ ����� ��ȸ
			/// </summary>

			//string Query = " SELECT * ";
            string Query = " SELECT TOT_PROC_AMT, CUR_PROC_AMT, CUR_PROC_PER ";     // 2015.09.08. hma ����
			Query		+= " FROM   P_BOP_SCH_PROC_TEMP A (NOLOCK) ";
			Query		+= " WHERE  PROC_ID = '" +  PUA101.PROC_ID + "'";
            Query       += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return Query;

		}

		public static string SCH_PROC_CHK()
		{
			/// <summary>
			/// ���� ��������� �������� �����ϰ� �ִ��� �Ǵ�
			/// </summary>

			string Query = " SELECT PROC_TYPE,    PROC_STATUS,    IN_ID ";
			Query		+= " FROM   P_BOP_SCH_PROC_TEMP A (NOLOCK) ";
			Query		+= " WHERE  PROC_TYPE IN ('E', 'S')"; // �������, ������ ����
            Query += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return Query;

		}

		public static string INS_SCH_PROC(string PROC_TYPE, string PROC_STATUS, string USR_ID)
		{
			/// <summary>
			/// ������ �������� ���
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
			/// ������ ���� ���� ����
			/// </summary>

			string Query = " DELETE ";
			Query		+= " FROM   P_BOP_SCH_PROC_TEMP ";
			Query		+= " WHERE  PROC_ID = '" +  PUA101.PROC_ID + "'";
            Query += "   AND  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return Query;

		}
		#endregion

		#region ������� ���� ��ȸ
		public static string SRCH_EMG_INFO()
		{
			/// <summary>
			/// ������� ���� ��ȸ
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

		#region SCH������ �������� ����
		public static string DEL_RESO_WORK_TIME()
		{
			/// <summary>
			/// ������ ���� ���� ����
			/// </summary>

			string Query = " DELETE ";
			Query		+= " FROM  P_BOP_RESO_WORK_TIME_TEMP ";
            Query += "   WHERE  CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

			return Query;
		}
		#endregion
	}
}
