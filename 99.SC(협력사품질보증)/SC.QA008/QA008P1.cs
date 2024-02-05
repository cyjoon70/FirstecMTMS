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

namespace SC.QA008
{
	public partial class QA008P1 : UIForm.FPCOMM1
	{

		#region 변수
		string[] returnVal = null;
		#endregion

		#region 생성자
		public QA008P1()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void QA008P1_Load(object sender, EventArgs e)
		{
			this.Text = "검사진행 조회";
			
			UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			//GroupBo x1 초기화
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			//기타 세팅	
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
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
					string strQuery = "usp_SC008 @pTYPE = 'P1'";
					strQuery += ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @sDAY_FR = '" + dtsDAY_FR.Text + "'";
					strQuery += ", @sDAY_TO = '" + dtsDAY_TO.Text + "'";
					strQuery += ", @sCUST_CD = '" + txtCUST_CD.Text + "'";
					strQuery += ", @sITEM_CD = '" + txtITEM_CD.Text + "'";

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

		#region 협력업체 조회
		private void btnCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtCUST_CD, txtCUST_NM);
		}

		private void txtCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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
                    if (fpSpread1.Sheets[0].Cells[R, i].Value != null)
                        returnVal[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                    else
                        returnVal[i] = "";

                }
			}
		}
		#endregion

		#region 품목 조회
		private void txtITEM_CD_TextChanged(object sender, EventArgs e)
		{
			txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnITEM_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005("FS1", true, txtITEM_CD.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtITEM_CD.Text = Msgs[2].ToString();
					txtITEM_NM.Value = Msgs[3].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

	}
}
