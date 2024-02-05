using System;
using System.Data;
using System.Windows.Forms;

namespace SystemBase
{
	/// <summary>
	/// SendMail에 대한 요약 설명입니다.
	/// </summary>
	public class SendMail
	{
		public SendMail()
		{
		}

		#region Send_Mail 메일발송
		public static string Send_Mail(string From, string To, string Subject, string Body, string BodyFormat)
		{
			string RtnMsg = "";

			try
			{

				string strSql = " DECLARE  @ret int ";
				strSql += "  exec @ret = usp_SEND_MAIL ";
				strSql += "  @From = '"		+ From + "'";// 발송자 메일
				strSql += ", @To = '"		+ To + "'"; 
				strSql += ", @Subject = '"	+ Subject + "'";
				strSql += ", @Body = '"		+ Body + "'";
				strSql += ", @BodyFormat='"	+ BodyFormat + "'";
				strSql += "  select @ret ";

				DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
				RtnMsg = ds.Tables[0].Rows[0][0].ToString();

			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("Send_Mail", f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0022"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return RtnMsg;
		}
		#endregion

	}
}
