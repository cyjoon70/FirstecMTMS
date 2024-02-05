using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SC.QA004
{
	public partial class QA004P1 : UIForm.FPCOMM1
	{

		#region 변수
		string CustCd = string.Empty;
		string[] returnVal = null;
		#endregion

		#region 생성자
		public QA004P1(string Cust_Cd)
		{
			InitializeComponent();

			CustCd = Cust_Cd;
		}
		#endregion

		#region Form Load
		private void QA004P1_Load(object sender, EventArgs e)
		{
			this.Text = "발주조회";
			
			UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			//GroupBo x1 초기화
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			//기타 세팅	
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			txtpCUST_CD.Text = CustCd;

			SearchExec();
		}
		#endregion

		#region SearchExec()
		protected override void SearchExec()
		{
			//조회조건 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
			{
				this.Cursor = Cursors.WaitCursor;

				try
				{
					string strQuery = "usp_SC_COMM @pTYPE = 'S1'";
					strQuery += ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @pCUST_CD = '" + txtpCUST_CD.Text + "'";
					strQuery += ", @pPO_DT_FR = '" + dtsDAY_FR.Text + "'";
					strQuery += ", @pPO_DT_TO = '" + dtsDAY_TO.Text + "'";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
				}

				this.Cursor = Cursors.Default;
			}

		}
		#endregion

		#region 발주처 텍스트 변경 이벤트
		private void txtpCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtpCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtpCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 그리드 선택 값 전송
		private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			RtnStr(e.Row);
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

		public void RtnStr(int R)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
				for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
				{
					returnVal[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
				}
			}
		}
		#endregion

	}
}
