using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using System.Text.RegularExpressions;

namespace SystemBase
{
	/// <summary>
	/// IMAGE에 대한 요약 설명입니다.
	/// </summary>
	public class FILESAVE
	{
		#region 파일 DB에 byte로 저장 후 자동채번값 Return
		public static DataSet FileInsert(string FILES_NO, string Filepath, string FileName, string FileKind, string UP_ID, string LANG_CD)
		{	// 사용예 : DataSet ds = SystemBase.IMAGE.FileInsert(FILES_NO, dlg.FileName, FileName, FileKind, SystemBase.Base.gstrUserID, SystemBase.Base.gstrLangCd);
			byte[] FILES = GetPhoto(Filepath);

			string Query = "usp_B_IMAGE";
			SqlConnection connection = new SqlConnection(SystemBase.DbOpen.DBOpen());
			SqlCommand command = new SqlCommand(); 
			command.CommandText = Query;
			command.CommandType = CommandType.StoredProcedure;
			command.Connection  = connection;
			//command.CommandTimeout = 10000000;

			command.Parameters.Add("@pType",SqlDbType.VarChar, 3).Value						= "I1";
			command.Parameters.Add("@pFILES_NO",SqlDbType.VarChar, 16).Value				= FILES_NO;
			command.Parameters.Add("@pFILES",SqlDbType.Image, FILES.Length).Value			= FILES;
			command.Parameters.Add("@pFileName",SqlDbType.VarChar, 50).Value				= FileName;
			command.Parameters.Add("@pFileKind",SqlDbType.VarChar, 5).Value					= FileKind;
			command.Parameters.Add("@pUP_ID",SqlDbType.VarChar, 20).Value					= UP_ID;
			command.Parameters.Add("@pLANG_CD",SqlDbType.VarChar, 3).Value					= LANG_CD;
			connection.Open();

			SqlDataAdapter da = new SqlDataAdapter(command);
			DataSet ds=new DataSet();
			da.Fill(ds);

			return ds;// 자동채번값 Return
		}
		#endregion

		#region TRANSACTION 적용하여 DB에 이미지 byte로 저장 후 자동채번값 Return
		public static DataSet FileInsert(string FILES_NO, int FILES_SEQ, string Filepath, string FileName, string FileKind, string UP_ID, string LANG_CD, SqlConnection dbConn, SqlTransaction trans)
		{	// 사용예 : DataSet ds = SystemBase.IMAGE.FileInsert(FILES_NO, dlg.FileName, FileName, FileKind, SystemBase.Base.gstrUserID, SystemBase.Base.gstrLangCd, dbConn, trans);
			DataSet ds = null;
			try
			{
				byte[] FILES = GetPhoto(Filepath);

				string Query = "usp_B_IMAGE";
				SqlCommand command = new SqlCommand(Query, dbConn); 
				command.Transaction = trans;
				command.CommandTimeout = 6000000;
				command.CommandType = CommandType.StoredProcedure;

				command.Parameters.Add("@pType",SqlDbType.VarChar, 3).Value						= "I1";
				command.Parameters.Add("@pFILES_NO",SqlDbType.VarChar, FILES_NO.Length).Value	= FILES_NO;
				command.Parameters.Add("@pFILES_SEQ",SqlDbType.Int).Value						= FILES_SEQ;

				command.Parameters.Add("@pFILES",SqlDbType.Image, FILES.Length).Value			= FILES;
				command.Parameters.Add("@pFileName",SqlDbType.VarChar, FileName.Length).Value	= FileName;
				command.Parameters.Add("@pFileKind",SqlDbType.VarChar, FileKind.Length).Value	= FileKind;
				command.Parameters.Add("@pUP_ID",SqlDbType.VarChar, UP_ID.Length).Value			= UP_ID;
				command.Parameters.Add("@pLANG_CD",SqlDbType.VarChar, LANG_CD.Length).Value		= LANG_CD;
				//dbConn.Open();

				SqlDataAdapter da = new SqlDataAdapter(command);
				ds=new DataSet();
				da.Fill(ds);

			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("DataSet FileInsert", f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("파일등록 중 예기치 못한 오류가 발생하였습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return ds;// 자동채번값 Return
		}
		#endregion

		#region 파일 DB에 byte로 저장 후 자동채번값 Return
		public static void FileInsert(string FILES_NO)
		{	// 사용예 : DataSet ds = SystemBase.IMAGE.FileInsert(FILES_NO, dlg.FileName, FileName, FileKind, SystemBase.Base.gstrUserID, SystemBase.Base.gstrLangCd);
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "전체(*.*)|*.*|gif 이미지(*.gif)|*.gif|jpg 이미지(*.jpg)|*.jpg|bmp 이미지(*.bmp)|*.bmp|xls Excel(*.xls)|*.xls";
			dlg.Multiselect = true;

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string ERRCode = "ER", MSGCode = "SY021";
				string MSGRow = "";

				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.CommandTimeout = 10000000;

				try
				{  

					string[] fileNames = dlg.FileNames;

					foreach (string FilePath in fileNames)
					{
						string FileFullName = FilePath.Substring(FilePath.ToString().LastIndexOf(@"\")+1, FilePath.Length-FilePath.ToString().LastIndexOf(@"\")-1);
						string FileName = FileFullName.Substring(0, FileFullName.ToString().LastIndexOf("."));
						string FileKind = FileFullName.Substring(FileFullName.ToString().LastIndexOf(".")+1, FileFullName.Length-FileFullName.ToString().LastIndexOf(".")-1);

						byte[] FILES = GetPhoto(FilePath);

						string Query = "usp_B_IMAGE";
						SqlConnection connection = new SqlConnection(SystemBase.DbOpen.DBOpen());
						SqlCommand command = new SqlCommand();
						command.CommandText = Query;
						command.CommandType = CommandType.StoredProcedure;
						command.Connection  = connection;
						command.CommandTimeout = 10000000;

						command.Parameters.Add("@pType",SqlDbType.VarChar, 3).Value						= "I2";
						command.Parameters.Add("@pFILES_NO",SqlDbType.VarChar, FILES_NO.Length).Value	= FILES_NO;
						command.Parameters.Add("@pFILES",SqlDbType.Image, FILES.Length).Value			= FILES;
						command.Parameters.Add("@pFileName",SqlDbType.VarChar, FileName.Length).Value	= FileName;
						command.Parameters.Add("@pFileKind",SqlDbType.VarChar, FileKind.Length).Value	= FileKind;
						command.Parameters.Add("@pUP_ID",SqlDbType.VarChar, SystemBase.Base.gstrUserID.Length).Value= SystemBase.Base.gstrUserID;
						command.Parameters.Add("@pFILES_SIZE",SqlDbType.Int).Value						= FILES.Length;

						connection.Open();

						SqlDataAdapter da = new SqlDataAdapter(command);
						DataSet df=new DataSet();
						da.Fill(df);

						ERRCode = df.Tables[0].Rows[0][0].ToString();
						MSGCode	= df.Tables[0].Rows[0][1].ToString();

						if(ERRCode == "ER"){Trans.Rollback();goto Exit;}
					}
					Trans.Commit();
				}
				catch(Exception f)
				{
					SystemBase.Loggers.Log("FileInsert", f.ToString());
					Trans.Rollback();
					MSGCode = "SY009";
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode,MSGRow), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			Exit:
				dbConn.Close();

				if(ERRCode != "ER")
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode,MSGRow), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode,MSGRow), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

			}
			//return ds;// 자동채번값 Return
		}
		#endregion

		#region GetPhoto 파일 byte[] 배열로 저장 함수
		public static byte[] GetPhoto(string filePath)
		{
			FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			BinaryReader reader = new BinaryReader(stream);
			byte[] photo = reader.ReadBytes((int)stream.Length);

			reader.Close();
			stream.Close();
			return photo;
		}
		#endregion

		#region DB FileShow SELECT 하여 로컬에 Temp 파일로 저장하기
		public static string FileShow(System.Windows.Forms.PictureBox Images, string FILES_NO)
		{	// 사용예 : FilePath = SystemBase.IMAGE.FileShow(PictureBox1, FILES_NO);
			string FilePath = "";
			try
			{
				string Query = "SELECT FILES, FILE_KIND FROM B_IMAGE WHERE FILES_NO = '"+ FILES_NO +"' ";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

				if(dt.Rows.Count > 0)
				{
					if(Images.Image != null)
						Images.Image.Dispose();

					byte[] MyData = null;
					MyData = (byte[])dt.Rows[0]["FILES"];
					int ArraySize = new int();
					ArraySize = MyData.GetUpperBound(0); 

					FileStream fs = new FileStream( SystemBase.Base.ProgramWhere + @"\images\temp."+dt.Rows[0]["FILE_KIND"].ToString().Trim(), FileMode.Create, FileAccess.Write);
					fs.Write(MyData, 0,ArraySize+1);
					fs.Close();

					if( dt.Rows[0]["FILE_KIND"].ToString().Trim().ToUpper() == "GIF" || dt.Rows[0]["FILE_KIND"].ToString().Trim().ToUpper() == "JPG" || dt.Rows[0]["FILE_KIND"].ToString().Trim().ToUpper() == "BMP" )
					{	// 이미지파일인 경우
						Images.Image = new Bitmap( SystemBase.Base.ProgramWhere + @"\images\temp."+dt.Rows[0]["FILE_KIND"].ToString().Trim());
					}
					else
					{
						Images.Image = null;
					}

					//					else
					//					{	// 이미지파일이 아닌경우
					//						System.Diagnostics.Process.Start(SystemBase.Base.ProgramWhere + @"\images\temp."+dt.Rows[0]["FILE_KIND"].ToString().Trim());
					//					}
					FilePath =  SystemBase.Base.ProgramWhere + @"\images\temp."+dt.Rows[0]["FILE_KIND"].ToString().Trim();
				}
				else
				{
					if(Images.Image != null)
						Images.Image.Dispose();

					Images.Image = null;
					FilePath = "";
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("FileShow", f.ToString());
				MessageBox.Show("파일 저장 중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return FilePath;
		}

		public static void FileShow(string FILES_NO, string FILE_PATH)
		{	// 사용예 : FilePath = SystemBase.IMAGE.FileShow(PictureBox1, FILES_NO);
			try
			{
				string Query = "SELECT FILES, FILE_KIND FROM B_IMAGE WHERE FILES_NO = '"+ FILES_NO +"' ";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

				if(dt.Rows.Count > 0)
				{
					byte[] MyData = null;
					MyData = (byte[])dt.Rows[0]["FILES"];
					int ArraySize = new int();
					ArraySize = MyData.GetUpperBound(0); 

					FileStream fs = new FileStream( FILE_PATH, FileMode.Create, FileAccess.Write);
					fs.Write(MyData, 0,ArraySize+1);
					fs.Close();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("FileShow", f.ToString());
				MessageBox.Show("파일 저장 중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		public static string FileShow(string FILES_NO)
		{	// 사용예 : FilePath = SystemBase.IMAGE.FileShow(PictureBox1, FILES_NO);
			string FilePath = "";
			try
			{
				string Query = "SELECT FILES, FILE_KIND FROM B_IMAGE WHERE FILES_NO = '"+ FILES_NO +"' ";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

				if(dt.Rows.Count > 0)
				{
					byte[] MyData = null;
					MyData = (byte[])dt.Rows[0]["FILES"];
					int ArraySize = new int();
					ArraySize = MyData.GetUpperBound(0); 

					FileStream fs = new FileStream( SystemBase.Base.ProgramWhere + @"\images\temp."+dt.Rows[0]["FILE_KIND"].ToString().Trim(), FileMode.Create, FileAccess.Write);
					fs.Write(MyData, 0,ArraySize+1);
					fs.Close();
					FilePath =  SystemBase.Base.ProgramWhere + @"\images\temp."+dt.Rows[0]["FILE_KIND"].ToString().Trim();
				}
				else
				{
					FilePath = "";
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("FileShow", f.ToString());
				MessageBox.Show("파일 저장 중 예기치 못한 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return FilePath;
		}
		#endregion

	}

}

