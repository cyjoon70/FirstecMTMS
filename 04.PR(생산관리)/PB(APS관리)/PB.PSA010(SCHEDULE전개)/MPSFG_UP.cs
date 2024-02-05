using System;
using System.Data;
using System.Data.SqlClient;

namespace PB.PSA010
{
	/// <summary>
	/// MPSFG_UP�� ���� ��� �����Դϴ�.
	/// </summary>
	public class MPSFG_UP
	{
		#region MPS Ȯ������ ������ FLG ����
		public static void SCH_MPSFG_UP( SqlCommand cmd 
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, string strPTF
			, string SCH_NO
			)
		{
			label1.Text = "MPS Ȯ������ ������ �����Ͽ� �����մϴ�.";
			label2.Text = "MPS Ȯ������ ������ �����Ͽ� �����մϴ�.";

			//string MPS_REG_UP = " UPDATE A SET STATUS = 'R' FROM P_MPS_REGISTER A(NOLOCK) WHERE A.STATUS='F' AND CONVERT(VARCHAR(10), A.MAKEFINISH_DT, 121) <= '"+ strPTF.ToString().Substring(0, 10) +"' ";
			//BY JMK 2009.09.10 PTF���ں��� ����MPS�� ���������� ����� �ƴϹǷ� STATUS�� �������� ����
			//string MPS_REG_UP = " UPDATE A SET STATUS = 'R' FROM P_MPS_REGISTER A(NOLOCK) WHERE A.STATUS='F' AND CONVERT(VARCHAR(10), A.MAKEFINISH_DT, 121) <= '"+ strPTF.ToString().Substring(0, 10) +"' ";
			
			string MPS_REG_UP = "UPDATE B SET B.STATUS = 'R' FROM P_SCH_LOG_DETAIL A, P_MPS_REGISTER B ";
			MPS_REG_UP += "WHERE A.CO_CD = B.CO_CD ";
            MPS_REG_UP += "  AND A.MAKEORDER_NO = B.MAKEORDER_NO ";
            MPS_REG_UP += "  AND A.END_TYPE = 'OK' "; 
            MPS_REG_UP += "  AND B.STATUS = 'F' ";
            MPS_REG_UP += "  AND A.SCH_NO = '" + SCH_NO + "'";

			cmd.CommandText = MPS_REG_UP;
			cmd.ExecuteNonQuery();
		}
		#endregion

		#region MPS Ȯ������ ������ TEMP���� ����
		public static void SCH_TEMP_DEL( SqlCommand cmd 
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, string SCH_NO
			)
		{
			label1.Text = "�������� ���� �� TEMP���� �������Դϴ�.";
			label2.Text = "�������� ���� �� TEMP���� �������Դϴ�.";
			
			string SCH_TEMP = "";
			SCH_TEMP = " DELETE B FROM P_SCH_LOG_DETAIL A, P_BOP_M_DETAIL_TEMP B";
			SCH_TEMP += " WHERE A.MAKEORDER_NO = B.MAKEORDER_NO ";
            SCH_TEMP += " AND A.END_TYPE = 'ER' ";
            SCH_TEMP += " AND SCH_NO = '" + SCH_NO + "'";

			SCH_TEMP = " DELETE B FROM P_SCH_LOG_DETAIL A, P_BOP_M_MASTER_TEMP B";
			SCH_TEMP += " WHERE A.MAKEORDER_NO = B.MAKEORDER_NO ";
            SCH_TEMP += " AND A.END_TYPE = 'ER' ";
            SCH_TEMP += " AND SCH_NO = '" + SCH_NO + "'";

			SCH_TEMP = " DELETE B FROM P_SCH_LOG_DETAIL A, P_BOP_M_PROC_CHILD_TEMP B";
			SCH_TEMP += " WHERE A.MAKEORDER_NO = B.MAKEORDER_NO ";
            SCH_TEMP += " AND A.END_TYPE = 'ER' ";
            SCH_TEMP += " AND SCH_NO = '" + SCH_NO + "'";

			SCH_TEMP = " DELETE B FROM P_SCH_LOG_DETAIL A, P_BOP_M_PROC_DETAIL_TEMP B";
			SCH_TEMP += " WHERE A.MAKEORDER_NO = B.MAKEORDER_NO ";
            SCH_TEMP += " AND A.END_TYPE = 'ER' ";
            SCH_TEMP += " AND SCH_NO = '" + SCH_NO + "'";

			SCH_TEMP = " DELETE B FROM P_SCH_LOG_DETAIL A, P_BOP_M_PROC_MASTER_TEMP B";
			SCH_TEMP += " WHERE A.MAKEORDER_NO = B.MAKEORDER_NO ";
            SCH_TEMP += " AND A.END_TYPE = 'ER' ";
            SCH_TEMP += " AND SCH_NO = '" + SCH_NO + "'";

			cmd.CommandText = SCH_TEMP;
			cmd.ExecuteNonQuery();
		}
		#endregion
	}
}
