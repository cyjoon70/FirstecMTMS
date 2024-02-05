using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;
using System.Text.RegularExpressions;
using System.Data.SqlClient;


namespace SC.QA009
{
	public partial class QA009 : UIForm.FPCOMM1
	{

		#region 변수
		string gStrAuth = string.Empty;
		#endregion

		#region 생성자
		public QA009()
		{
			InitializeComponent();
		}

		#endregion

		#region Form Load
		private void QA009_Load(object sender, EventArgs e)
		{
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 발생공정 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosOCCUR_PROC, "usp_B_COMMON @pType='COMM', @pCODE = 'SC210', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

			GetAuth();

			SetInit();
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,7);
			dtsDAY_TO.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
		}

		private void GetAuth()
		{
			DataTable dt;
			string strQuery = string.Empty;
			strQuery = "SELECT DBO.UFN_GETQMAUTH ('" + SystemBase.Base.gstrUserID + "')";

			dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dt != null) gStrAuth = dt.Rows[0][0].ToString();
		}
		#endregion

		#region 협력업체 조회 
		private void btnsCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtsCUST_CD, txtsCUST_NM);
		}

		private void txtsCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtsCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtsCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetCustInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				WNDW002 pu = new WNDW002(id.Text, "");
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					id.Value = Msgs[1].ToString();
					name.Value = Msgs[2].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region New
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			fpSpread1.Sheets[0].Rows.Count = 0;

			SetInit();
		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			SelectExec("");
		}

		private void SelectExec(string SEQ)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;

				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
				{
					if (dtsDAY_FR.Text.Substring(0, 4) != dtsDAY_TO.Text.Substring(0, 4))
					{
						MessageBox.Show("조회 시작월과 종료월은 같은 년도이어야 합니다.");
						return;
					}

					string strQuery = "";
					strQuery = " usp_SC009 @pTYPE = 'S1' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @sYYMM_FR		= '" + dtsDAY_FR.Text + "' ";
					strQuery = strQuery + ", @sYYMM_TO		= '" + dtsDAY_TO.Text + "' ";
					strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text + "' ";
					strQuery = strQuery + ", @sNPROD_TYPE	= '" + cbosOCCUR_PROC.SelectedValue + "' ";
					strQuery = strQuery + ", @sQPA_FR		= " + (string.IsNullOrEmpty(txtQPAfr.Text) ? "NULL" : txtQPAfr.Text) + " ";
					strQuery = strQuery + ", @sQPA_TO		= " + (string.IsNullOrEmpty(txtQPAto.Text) ? "NULL" : txtQPAto.Text) + " ";
					strQuery = strQuery + ", @sQSA_FR		= " + (string.IsNullOrEmpty(txtQSAfr.Text) ? "NULL" : txtQSAfr.Text) + " ";
					strQuery = strQuery + ", @sQSA_TO		= " + (string.IsNullOrEmpty(txtQSAto.Text) ? "NULL" : txtQSAto.Text) + " ";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
					fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
		#endregion

		#region RowInsExec() -- 등록
		protected override void RowInsExec()
		{
			if (string.IsNullOrEmpty(gStrAuth) || gStrAuth == "D")
			{
				MessageBox.Show("데이터 등록권한이 없습니다.");
				return;
			}

			QA009P1 myForm = new QA009P1();
			myForm.ShowDialog();
			SelectExec("");
		}
		#endregion

	}
}
