using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Windows.Forms;

namespace SystemBase
{
	public class DbOpen
	{
		#region DataSet 에러 처리
		private static DataSet DataSetError(string Msg)
		{
			DataSet ds = null;
			ds = new DataSet();
			ds.Tables.Add();

			DataRow newRow = ds.Tables[0].NewRow();
			ds.Tables[0].Rows.Add(newRow);

			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();

			ds.Tables[0].Rows[0][0] = "ER";
			ds.Tables[0].Rows[0][1] = Msg;
			ds.Tables[0].Rows[0][2] = "";
			ds.Tables[0].Rows[0][3] = "";
			ds.Tables[0].Rows[0][4] = "";

			return ds;
		}
		#endregion

		#region DataSet 에러 처리
		private static DataTable DataTableError(string Msg)
		{
			DataTable dt = null;
			dt = new DataTable();

			DataRow newRow = dt.NewRow();
			dt.Rows.Add(newRow);

			dt.Columns.Add();
			dt.Columns.Add();
			dt.Columns.Add();
			dt.Columns.Add();
			dt.Columns.Add();

			dt.Rows[0][0] = "ER";
			dt.Rows[0][1] = Msg;
			dt.Rows[0][2] = "";
			dt.Rows[0][3] = "";
			dt.Rows[0][4] = "";

			return dt;
		}
		#endregion

		#region DB Connection
		public static string DBOpen()
		{
			return SystemBase.Base.gstrDbConn;
		}
		#endregion

		#region DB Connection & Open
		public static SqlConnection DBCON()	//DB 열기
		{
			SqlConnection dbcon = null;
			string dbConString = DBOpen();
			dbcon = new SqlConnection(dbConString);
			dbcon.Open();
			return dbcon;
		}
		#endregion

		#region DB 트랜잭션으로 NonQuery 처리 600초
		public static string TranNonQuery(string Query, string Msg)	//DB 트랜잭션으로 NonQuery 처리
		{
			string RtnMsg;
			SqlConnection dbConn = DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
			cmd.Transaction = Trans;
			cmd.CommandTimeout = 6000000;

			try
			{
				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
				Trans.Commit();

				RtnMsg = Msg;
			}
			catch (Exception e)
			{
				Trans.Rollback();
				SystemBase.Loggers.Log("TranNonQuery Err", Query);
				RtnMsg = SystemBase.Base.MessageRtn("SY009") + "\n\n" + e.ToString();//아래와 같은 에러가 발생되어데이타가 처리되지 않고 롤백되었습니다
			}
			dbConn.Close();

			return RtnMsg;
		}
		#endregion

		#region DB 트랜잭션으로 NonQuery 처리 6000초
		public static string TranNonQueryLongTime(string Query, string Msg)	//DB 트랜잭션으로 NonQuery 처리
		{
			string RtnMsg;
			SqlConnection dbConn = DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
			cmd.Transaction = Trans;
			cmd.CommandTimeout = 60000000;

			try
			{
				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
				Trans.Commit();

				RtnMsg = Msg;
			}
			catch (Exception e)
			{
				Trans.Rollback();
				SystemBase.Loggers.Log("TranNonQueryLongTime Err", Query);
				RtnMsg = SystemBase.Base.MessageRtn("SY009") + "\n\n" + e.ToString();//아래와 같은 에러가 발생되어데이타가 처리되지 않고 롤백되었습니다
			}
			dbConn.Close();

			return RtnMsg;
		}
		#endregion

		#region 트랜잭션 처리 TranDataSet 사용
		public static DataSet TranDataSet(string Query, SqlConnection dbConn, SqlTransaction trans)
		{
			DataSet ds = null;
			try
			{
				SqlCommand cmd = new SqlCommand(Query, dbConn);
				cmd.Transaction = trans;
				cmd.CommandTimeout = 6000000;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				ds = new DataSet();
				da.Fill(ds);
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("TranDataSet Err", e.ToString());

				ds = DataSetError(e.Message.ToString());
			}
			return ds;
		}
		#endregion

		#region 트랜잭션 처리 DataTable 사용
		public static DataTable TranDataTable(string Query)
		{
			DataTable dt = null;
			try
			{
				SqlConnection dbConn = DBCON();
				SqlCommand cmd = new SqlCommand(Query, dbConn);
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 6000000;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				dt = new DataTable();
				da.Fill(dt);
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("TranDataTable Err", Query + "\n\n" + e.ToString());

				dt = DataTableError(e.Message.ToString());
			}

			return dt;
		}
		#endregion

		#region 트랜잭션 처리 TranDataTable 사용
		public static DataTable TranDataTable(string Query, SqlConnection dbConn, SqlTransaction trans)
		{
			DataTable dt = null;
			try
			{
				SqlCommand cmd = new SqlCommand(Query, dbConn);
				cmd.Transaction = trans;
				cmd.CommandTimeout = 6000000;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				dt = new DataTable();
				da.Fill(dt);
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("TranDataTable Err", Query + "\n\n" + e.ToString());

				dt = DataTableError(e.Message.ToString());
			}

			return dt;
		}
		#endregion

		#region TranDataTable2 DB 트랜잭션으로 NonQuery 처리 600초
		public static DataTable TranDataTable2(string Query)	//DB 트랜잭션으로 NonQuery 처리
		{
			SqlConnection dbConn = null;
			SqlTransaction Trans = null;
			DataTable dt = null;
			try
			{
				dbConn = DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 6000000;

				cmd.CommandText = Query;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				dt = new DataTable();
				da.Fill(dt);

				if (dt.Rows[0][0].ToString() == "ER") { SystemBase.Loggers.Log("TranDataTable2 Err Rollback", dt.Rows[0][1].ToString() + "\n\n" + Query); goto Exit; }

			}
			catch (Exception e)
			{
				Trans.Rollback();
				SystemBase.Loggers.Log("TranDataTable2 Err", Query + "\n\n" + e.ToString());

				dt = DataTableError(e.Message.ToString());
			}
		Exit:
			dbConn.Close();
			return dt;
		}
		#endregion

		#region 트랜잭션 처리하지 않은 DataSet 사용
		public static DataSet NoTranDataSet(string Query)
		{
			DataSet ds = null;
			try
			{
				SqlConnection dbcon = DBCON();
				string query = Query;
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				cmd.CommandTimeout = 0;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				ds = new DataSet();
				adp.Fill(ds);
				dbcon.Close();
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("NoTranDataSet Err", Query + "\n\n" + e.ToString());

				ds = DataSetError(e.Message.ToString());
			}
			return ds;
		}
		#endregion

		#region 트랜잭션 처리하지 않은 DataTable 사용
		public static DataTable NoTranDataTable(string Query)
		{
			DataTable dt = null;
			try
			{
				SqlConnection dbcon = DBCON();
				string query = Query;
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				cmd.CommandTimeout = 0;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				dt = new DataTable();
				adp.Fill(dt);
				dbcon.Close();
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("NoTranDataSet Err", Query + "\n\n" + e.ToString());

				dt = DataTableError(e.Message.ToString());
			}
			return dt;
		}
		#endregion

		#region 트랜잭션 처리하지 않은 NonQuery
		public static void NoTranNonQuery(string Query)
		{
			try
			{
				SqlConnection dbcon = DBCON();
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				cmd.ExecuteNonQuery();
				dbcon.Close();
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("NoTranNonQuery Err", Query + "\n\n" + e.ToString());
			}
		}
		#endregion

		#region 트랜잭션 처리하지 않은 NonQuery
		public static object NoTranScalar(string Query)
		{
			object obj = null;
			try
			{
				SqlConnection dbcon = DBCON();
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				obj = cmd.ExecuteScalar();
				dbcon.Close();
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("NoTranScalar Err", Query + "\n\n" + e.ToString());
			}
			return obj;
		}
		#endregion

		#region ReportDataSet	크리스탈 레포트용 데이타셋
		public static DataSet ReportDataSet(string Query, string DatasetName)
		{
			DataSet ds = null;
			try
			{
				SqlConnection dbcon = DBCON();
				string query = Query;
				SqlDataAdapter adp = new SqlDataAdapter(query, dbcon);
				ds = new DataSet();
				adp.Fill(ds, DatasetName);
				dbcon.Close();
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("NoTranDataSet Err", Query + "\n\n" + e.ToString());

				ds = DataSetError(e.Message.ToString());
			}
			return ds;
		}
		#endregion

		#region MDB용
		public static DataTable MDBOpen(string fullpath)
		{
			DataTable dt = null;
			try
			{
				string dbconn = @"Provider=Microsoft.Jet.OLEDB.4.0;User ID=;Jet OLEDB:Database Password=" + SystemBase.Base.DeCode("EOPAWTEFQFaWDbUEBQDZUCNOAYRHXQ") + ";Data Source=" + fullpath.ToString();
				string query = "select top 1 * from DBCon where TblName='MASTER' ";

				OleDbConnection dbcon = new OleDbConnection(dbconn);
				dbcon.Open();

				OleDbDataAdapter adp = new OleDbDataAdapter(query, dbcon);
				dt = new DataTable();
				adp.Fill(dt);
				dbcon.Close();
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log("MDBOpen Err", fullpath + "\n\n" + e.ToString());
			}
			return dt;
		}
		#endregion

		#region MDB용Messenger
		public static DataTable MSGMDBOpen(string fullpath)
		{
			DataTable dt = null;
			try
			{
				string dbconn = @"Provider=Microsoft.Jet.OLEDB.4.0;User ID=;Jet OLEDB:Database Password=" + SystemBase.Base.DeCode("EOPAWTEFQFaWDbUEBQDZUCNOAYRHXQ") + ";Data Source=" + fullpath.ToString();
				string query = "select top 1 USERID, PASSWD, CKTime, IP, DBNAME, ID, DBPWD from DBCon where TblName='MESSENGER' ";

				OleDbConnection dbcon = new OleDbConnection(dbconn);
				dbcon.Open();

				OleDbDataAdapter adp = new OleDbDataAdapter(query, dbcon);
				dt = new DataTable();
				adp.Fill(dt);
				dbcon.Close();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
				SystemBase.Loggers.Log("MSGMDBObopen Err", fullpath + "\n\n" + e.ToString());
			}
			return dt;
		}
		#endregion

		#region MDB용Messenger
		public static void MSGMDBOpen(string fullpath, string USERID, string PASSWD, string CKTime)
		{
			try
			{
				string dbconn = @"Provider=Microsoft.Jet.OLEDB.4.0;User ID=;Jet OLEDB:Database Password=" + SystemBase.Base.DeCode("EOPAWTEFQFaWDbUEBQDZUCNOAYRHXQ") + ";Data Source=" + fullpath.ToString();
				string query = "UPDATE DBCon SET USERID='" + USERID + "', PASSWD = '" + SystemBase.Base.EnCode(PASSWD) + "', CKTime = '" + CKTime + "' where TBLNAME='MESSENGER' ";

				OleDbConnection dbcon = new OleDbConnection(dbconn);
				dbcon.Open();

				OleDbCommand cmd = new OleDbCommand(query, dbcon);
				cmd.ExecuteNonQuery();
				dbcon.Close();

			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString(), "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region MDB용Messenger
		public static void MSGMDBOpen(string fullpath, string USERID, string PASSWD)
		{
			try
			{
				string dbconn = @"Provider=Microsoft.Jet.OLEDB.4.0;User ID=;Jet OLEDB:Database Password=" + SystemBase.Base.DeCode("EOPAWTEFQFaWDbUEBQDZUCNOAYRHXQ") + ";Data Source=" + fullpath.ToString();
				string query = "UPDATE DBCon SET USERID='" + USERID + "', PASSWD = '" + SystemBase.Base.EnCode(PASSWD) + "' where TBLNAME='MESSENGER' ";

				OleDbConnection dbcon = new OleDbConnection(dbconn);
				dbcon.Open();

				OleDbCommand cmd = new OleDbCommand(query, dbcon);
				cmd.ExecuteNonQuery();
				dbcon.Close();

			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString(), "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Eecle Connet
		public static void Eecle_Connet(string FileName)
		{
			try
			{
				SystemBase.Base.gstrExcelConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties=\"Excel 12.0;IMEX=1;Persist Security Info=False;HDR=NO\"";

			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString(), "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 쿼리 결과로 사전을 생성 (2014.7.16, 이재광)
		/// <summary>
		/// 쿼리 결과로 사전을 생성합니다. 쿼리 결과는 필드가 2개 필요하며 첫 번째가 키, 두 번째가 값이 됩니다.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static Dictionary<string, string> CreateDictionary(string query)
		{
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(query);
			Dictionary<string, string> dic = new Dictionary<string, string>();

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (dt.Columns.Count > 1)
					dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
				else
					dic.Add(dt.Rows[i][0].ToString(), dt.Rows[i][0].ToString());
			}

			return dic;
		}
		#endregion
	}

	#region MainMenu Query
	/*
	 if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[MainMenu]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [dbo].[MainMenu]
	GO

	CREATE TABLE [dbo].[MainMenu] (
		[Num] [int] IDENTITY (1, 1) NOT NULL ,
		[MenuID] [varchar] (20) COLLATE Korean_Wansung_CI_AS NULL ,
		[MenuName] [varchar] (20) COLLATE Korean_Wansung_CI_AS NULL ,
		[MenuParent] [varchar] (20) COLLATE Korean_Wansung_CI_AS NULL ,
		[Enable] [varchar] (2) COLLATE Korean_Wansung_CI_AS NULL ,
		[MDIForm] [varchar] (20) COLLATE Korean_Wansung_CI_AS NULL ,
		[UserID] [varchar] (10) COLLATE Korean_Wansung_CI_AS NULL 
	) ON [PRIMARY]
	GO
	*/
	#endregion
}
