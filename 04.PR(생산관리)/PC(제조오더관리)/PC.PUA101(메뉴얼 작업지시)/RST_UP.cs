using System;
using System.Data;
using System.Data.SqlClient;

namespace PC.PUA101
{ 
	/// <summary>
	/// 결과 반영에 대한 요약 설명입니다.
	/// </summary>
	public class RST_UP
	{
		#region 긴급작지 결과 반영
		public static void SCH_RST_UP(
			System.Windows.Forms.Label label1
			, SqlCommand cmd
			, SqlConnection dbConn
			, SqlTransaction Trans
			)
		{

			try 
			{
				label1.Text = "결과 반영중...";

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
