using System;
using System.Data;
using System.Data.SqlClient;

namespace PC.PUA101
{ 
	/// <summary>
	/// ��� �ݿ��� ���� ��� �����Դϴ�.
	/// </summary>
	public class RST_UP
	{
		#region ������� ��� �ݿ�
		public static void SCH_RST_UP(
			System.Windows.Forms.Label label1
			, SqlCommand cmd
			, SqlConnection dbConn
			, SqlTransaction Trans
			)
		{

			try 
			{
				label1.Text = "��� �ݿ���...";

				string Query = "usp_PUA101 'I2'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				Query += ", @pUSR_ID   = '" + SystemBase.Base.gstrUserID + "'";

				DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);
				string ERRCode = ds.Tables[0].Rows[0][0].ToString();
				string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

				if(ERRCode == "ER")
					throw new Exception(MSGCode);
			}
			catch(Exception f)
			{
				throw f;
			}
		}
		#endregion
	}
}
