#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 Lot 정보 조회
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-0807
// 작성내용 : 기 등록된 Lot 정보 조회 후 출고/이동에서 사용
// 수 정 일 : 
// 수 정 자 : 
// 수정내용 : 
// 비    고 : 
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW032 pu = new WNDW.WNDW032();
    pu.ShowDialog();
 
    if (pu.DialogResult == DialogResult.OK)
    {
        
    }
 
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Lot 정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
	public partial class WNDW032 : UIForm.FPCOMM1
	{

		#region 변수선언

		string[] returnVal = null;

		public bool bReadOnlyYN = true;
		public string strPLANT_CD = string.Empty;
		public string strPROJECT_NO = string.Empty;
		public string strPROJECT_NM = string.Empty;
		public string strPROJECT_SEQ = string.Empty;
		public string strITEM_CD = string.Empty;
		public string strITEM_NM = string.Empty;
		public string strLOT_NO = string.Empty;
		public string strBAR_CODE = string.Empty;
		
		#endregion
		
		#region 생성자
		public WNDW032()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void WNDW032_Load(object sender, EventArgs e)
		{

			try
			{
				SystemBase.Validation.GroupBox_Setting(groupBox1);
				UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
				SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE='PLANT'");	//공장
				UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
				SetControls();
				SearchExec();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Lot 정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		#endregion

		#region Control Setting
		private void SetControls()
		{
			if (string.IsNullOrEmpty(strPLANT_CD))
			{
				cboPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD;
			}
			else
			{
				cboPlant.SelectedValue = strPLANT_CD;
			}

			txtProjectNo.Value = strPROJECT_NO;
			txtProjectSeq.Value = strPROJECT_SEQ;
			txtItemCd.Value = strITEM_CD;
			txtItemNm.Value = strITEM_NM;

		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{

				string strQuery = " usp_WNDW032 ";
				strQuery += "  @pTYPE = 'S1'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				strQuery += ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
				strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Value + "'";
				strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Value + "'";
				strQuery += ", @pITEM_CD = '" + txtItemCd.Value + "'";
				strQuery += ", @pLOT_NO = ''";
				strQuery += ", @pBAR_CODE = ''";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

				if (fpSpread1.Sheets[0].Rows.Count == 0)
				{
					this.Close();
				}

				fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
				
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002", SystemBase.Base.MessageRtn("Z0002")), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = Cursors.Default;

		}
		#endregion

		#region 그리드 더블클릭
		private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			try
			{

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					RtnStr(e.Row);
				}

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Lot 정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 그리드 선택값 입력밑 전송
		public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

		public void RtnStr(int R)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				returnVal = new string[fpSpread1.Sheets[0].Columns.Count];

				for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
				{
					returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
				}
			}
		}
		#endregion

	}
}
