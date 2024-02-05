using System;
using System.Reflection;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace SystemBase
{
	/// <summary>
	/// Logger에 대한 요약 설명입니다.
	/// </summary>
	public class Loggers
	{
		// ---------------------------------------------
		// 내부사용 상수
		// ---------------------------------------------
		private const string FILE_HEADER = "MTMS_";

		//Log Folder
		//private static string strLocation = "LOG\\";	

		//log writer
		//private static StreamWriter writer;

		//log file path
		//private static string logFilePath;

		//log file create date
		//private static DateTime logFileDate;

		private Loggers()
		{
		}
/*
		static Loggers()
		{
			strLocation = AutoConfig.MLogFolder;
		}
*/	

		public static void Log(string sClassNm, string msg)
		{
			try
			{
				string LogQuery = "INSERT INTO CO_LOG (LOG_TIME,FORM_NAME,ERR_MSG,IN_ID) ";
				LogQuery = LogQuery + " VALUES('"+SystemBase.Base.ServerTime("")+"','"+ sClassNm +"','"+ msg.Replace("'","＇").ToString() +"','"+ SystemBase.Base.gstrUserID +"') ";
				SystemBase.DbOpen.NoTranNonQuery(LogQuery);
			}
			catch//(Exception f)
			{
				//MessageBox.Show("Log Save Error \n\n"+ f.ToString());
			}
		}

		public static void Log(string sClassNm, Exception f)
		{
			try
			{
                string LogQuery = "INSERT INTO CO_LOG (LOG_TIME,FORM_NAME,ERR_MSG,IN_ID) ";
				LogQuery = LogQuery + " VALUES('"+SystemBase.Base.ServerTime("")+"','"+ sClassNm +"','"+ f.ToString().Replace("'","＇") +"','"+ SystemBase.Base.gstrUserID +"') ";
				SystemBase.DbOpen.NoTranNonQuery(LogQuery);

			}
			catch//(Exception g)
			{
				//MessageBox.Show("Log Save Error \n\n"+ g.ToString());
			}

			//			string strErrDesc = ex.Message + "\r\n" + ex.StackTrace;
			//			Log(sClassNm, strErrDesc);
		}
/*
		public static void Log(string sClassNm, string sInfoType, string sImpFileNm, Exception ex)
		{
			string strErrDesc = ex.Message + "\r\n" + ex.StackTrace;
			Log(sClassNm, strErrDesc);

			SqlConnection oConn = null;
			SqlCommand oCmd = null;

			try
			{
				// DATABASE OPEN
				oConn = new SqlConnection(AutoConfig.DbConnString);
				oConn.Open();

				oCmd = new SqlCommand("USP_SERVICE_RLOG", oConn);
				oCmd.CommandType    = CommandType.StoredProcedure;
				oCmd.CommandTimeout = Common.CommandTimeOut;

				oCmd.Parameters.Clear();
				oCmd.Parameters.Add("@InfoType"    , sInfoType               );
				oCmd.Parameters.Add("@ImpPath"     , AutoConfig.RImportFolder);
				oCmd.Parameters.Add("@ImpFileNm"   , sImpFileNm              );
				oCmd.Parameters.Add("@BackUpPath"  , Common.getBackUpPath()  );
				oCmd.Parameters.Add("@ErrDesc  "   , strErrDesc              );

				oCmd.ExecuteNonQuery();
			}
			catch
			{
				Log(sClassNm, "Connection String : " + AutoConfig.DbConnString);
			}
			finally
			{
				oCmd.Dispose();
				oConn.Close();
				oConn.Dispose();
			}
		}
*/
	}
}

/**********************************************************************
 * 사용예) SystemBase.Loggers.Log(this.Name, f.ToString());
**********************************************************************/
