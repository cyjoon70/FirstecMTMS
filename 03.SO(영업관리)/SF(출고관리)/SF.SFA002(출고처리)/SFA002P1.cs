#region 작성정보
/*********************************************************************/
// 단위업무명 : Lot 정보 조회 및 출고 저장
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-09-26
// 작성내용 : 기 등록된 Lot 정보 조회 후 출고 처리
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

namespace SF.SFA002
{
	public partial class SFA002P1 : UIForm.FPCOMM1
	{

		#region 변수선언

		public DataTable dt = null;
		public SFA002 S1 = new SFA002();
		public decimal dLotSum = 0;
		decimal dSum = 0;
		bool bLotStock = true;	// lot별 재고수량과 출고수량을 비교. (lot별 재고수량 < 출고수량) => false 

		public string strPLANT_CD = string.Empty;
		public string strPROJECT_NO = string.Empty;
		public string strPROJECT_NM = string.Empty;
		public string strPROJECT_SEQ = string.Empty;
		public string strITEM_CD = string.Empty;
		public string strITEM_NM = string.Empty;
		public string strLOT_NO = string.Empty;
		public string strBAR_CODE = string.Empty;
		public string strTRAN_NO = string.Empty;
		public string strTRAN_SEQ = string.Empty;
		public string strITEM_SPEC = string.Empty;
		public string strREM_QTY = string.Empty;		// 요청수량
		public string strWORKORDER_NO = string.Empty;	// 작업지시번호
		public string strPROC_SEQ = string.Empty;		// 공정순서
		public string strDN_NO = string.Empty;			// 출고요청번호
		public string strDN_SEQ = string.Empty;			// 출고요청순번
		#endregion

		#region 생성자
		public SFA002P1()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void SFA002P1_Load(object sender, EventArgs e)
		{
			try
			{

				this.Text = "Lot 정보조회";

				SystemBase.Validation.GroupBox_Setting(groupBox1);
				UIForm.Buttons.ReButton("010000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
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
			txtItemSp.Value = strITEM_SPEC;
			txtREM_QTY.Value = strREM_QTY;

		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{

				string strQuery = " usp_SFA002 ";
				strQuery += "  @pTYPE = 'P2'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				strQuery += ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
				strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Value + "'";
				strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Value + "'";
				strQuery += ", @pITEM_CD = '" + txtItemCd.Value + "'";
				strQuery += ", @pDN_SEQ = '" + strDN_SEQ + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

				if (fpSpread1.Sheets[0].Rows.Count == 0)
				{
					this.Close();
					return;
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002", SystemBase.Base.MessageRtn("Z0002")), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = Cursors.Default;

		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{

			txtProjectNo.Focus();
			dt = new DataTable();

			this.Cursor = Cursors.WaitCursor;

			if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
			{

				if (CheckOUT_Qty() == false)
				{
					if (bLotStock == false)
					{
						MessageBox.Show(SystemBase.Base.MessageRtn("출고수량은 Lot별 재고수량을 초과할 수 없습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						this.Cursor = Cursors.Default;
						bLotStock = true;
						return;
					}
					else
					{
						MessageBox.Show(SystemBase.Base.MessageRtn("출고수량은 출고잔량과 일치해야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						this.Cursor = Cursors.Default;
						return;
					}

				}

				if (dSum == 0)
				{
					MessageBox.Show(SystemBase.Base.MessageRtn("출고수량의 합은 0보다 커야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.Cursor = Cursors.Default;
					return;
				}

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					dt = (DataTable)fpSpread1.Sheets[0].DataSource;
				}

				// Grid CUD 값 초기화
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" ||
						fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
					{
						fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = (fpSpread1.Sheets[0].ActiveRowIndex + 1).ToString();

						#region lot history
						string strContents = string.Empty;
						strContents += ",@pCO_CD			= ''" + SystemBase.Base.gstrCOMCD + "'' ";
						strContents += ",@pPLANT_CD			= ''" + SystemBase.Base.gstrPLANT_CD + "'' ";
						strContents += ",@pBAR_CODE			= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "'' ";
						strContents += ",@pMVMT_NO			= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "'' ";
						strContents += ",@pMVMT_SEQ			= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "'' ";
						strContents += ",@pOUT_TRAN_NO		= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고번호")].Text + "'' ";
						strContents += ",@pOUT_TRAN_SEQ		= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고순번")].Text + "'' ";
						strContents += ",@pITEM_CD			= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'' ";
						strContents += ",@pTR_TYPE			= ''O'' ";
						strContents += ",@pOUT_DATE			= NULL ";
						strContents += ",@pLOT_NO			= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "'' ";
						strContents += ",@pOUT_PROJECT_NO	= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "'' ";
						strContents += ",@pOUT_PROJECT_SEQ	= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "'' ";
						strContents += ",@pOUT_QTY			= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text + "'' ";
						strContents += ",@pSTOCK_UNIT		= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'' ";
						strContents += ",@pREMARK			= ''" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'' ";
						strContents += ",@pIN_ID			= ''" + SystemBase.Base.gstrUserID + "'' ";
						strContents += ",@pUP_ID			= ''" + SystemBase.Base.gstrUserID + "'' ";

						string strSql = "usp_T_LOT_INOUT_HISTORY_CUDR ";
						strSql += " @pTYPE			= 'I1' ";
						strSql += ",@CONTENTS		= '" + strContents + "' ";					// LOT 입출고 내용
						strSql += ",@IN_ID			= '" + SystemBase.Base.gstrUserID + "' ";	// 등록자
						strSql += ",@FROM_DLL		= 'SFA002P1' ";								// 소스 DLL
						strSql += ",@IN_OUT_TYPE	= 'O' ";									// 입고('I'), 출고('O')

						DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
						#endregion
					}

					dLotSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);

				}

				this.DialogResult = DialogResult.OK;
				Close();
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;

			return;

		}
		#endregion

		#region 출고수량 <= 요청수량 체크
		private bool CheckOUT_Qty()
		{
			bool bValid = true;

			bLotStock = true;
			dSum = 0;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text) == false)
					{
						dSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
					}
					else
					{
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = 0;
					}

					if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value) <
						Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value))
					{
						bLotStock = false;
						break;
					}
				}
			}


			if (dSum != Convert.ToDecimal(txtREM_QTY.Value))
				bValid = false;
			else
				bValid = true;

			if (bLotStock == false) { bValid = false; }

			return bValid;
		}
		#endregion

	}
}
