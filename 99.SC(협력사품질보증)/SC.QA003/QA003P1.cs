using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using WNDW;

namespace SC.QA003
{
	public partial class QA003P1 : UIForm.Buttons
	{
		#region 변수선언
		private string	CorrNo;
		private string	Gubun;
		#endregion

		#region 생성자
		public QA003P1(string corrNo, string gubun)
		{
			InitializeComponent();

			CorrNo = corrNo;
			Gubun = gubun;
		}			   
		#endregion

		#region Form Load
		private void QA003P1_Load(object sender, EventArgs e)
		{
			this.Text = Gubun;

			UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SelectExec();
		}
		#endregion

		#region 조회
		private void SelectExec()
		{
			string strQuery = "";
			strQuery = " usp_SC003 @pTYPE = 'P1' ";
			strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
			strQuery = strQuery + ", @pCORR_NO = '" + CorrNo + "' ";
			strQuery = strQuery + ", @pCONTENTS_TEXT = '" + Gubun + "' ";

			DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

			txtContents.Value = ds.Tables[0].Rows[0][0].ToString().Replace("\n", "\r\n");
		
			// Form Load 할때 txtContents 내용이 자동으로 전체선택되는 것을 막기위한 강제 포커스 이동처리
			this.ActiveControl = txtImsi;
		}
		#endregion



	}
}
