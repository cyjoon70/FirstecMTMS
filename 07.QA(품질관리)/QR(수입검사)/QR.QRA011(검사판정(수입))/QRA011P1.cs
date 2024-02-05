#region 작성정보
/*********************************************************************/
// 단위업무명 : Lot 정보 조회 및 불량 수량 저장
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-09-04
// 작성내용 : 기 등록된 Lot 정보 조회 후 불량 처리
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

namespace QR.QRA011
{
	public partial class QRA011P1 : UIForm.FPCOMM1
	{

		#region 변수선언

		public DataTable dt = new DataTable();
		public QRA011 Q1 = new QRA011();

		// 팝업화면에서 CUD 발생여부 체크. 발생했다면 Parent Form Reload
		public string strSaveYN = string.Empty;

		public bool bReadOnlyYN = true;					// 검색조건 변경 여부
		public string strPLANT_CD = string.Empty;
		public string strPROJECT_NO = string.Empty;
		public string strPROJECT_NM = string.Empty;
		public string strPROJECT_SEQ = string.Empty;
		public string strITEM_CD = string.Empty;
		public string strITEM_NM = string.Empty;
		public string strLOT_NO = string.Empty;
		public string strBAR_CODE = string.Empty;

		public string strINSP_REQ_NO = string.Empty;
		public string strINSP_ITEM_CD = string.Empty;
		public string strINSP_SERIES = string.Empty;
		
		public string strMVMT_NO = string.Empty;
		public string strMVMT_SEQ = string.Empty; 
		public string strGUBUN = string.Empty ;
		public decimal dDefectQty = 0;

		public decimal dLotSum = 0;
		public bool bQtyVld = true;

		#endregion

		#region 생성자
		public QRA011P1()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void QRA011P1_Load(object sender, EventArgs e)
		{

			this.Text = "LOT 불량처리";

			SystemBase.Validation.GroupBox_Setting(groupBox1);
			UIForm.Buttons.ReButton("010000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
			SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE='PLANT'");	//공장
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			SetControls();
			SearchExec();
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
			txtProjectNM.Value = strPROJECT_NM;
			txtItemCd.Value = strITEM_CD;
			txtItemNm.Value = strITEM_NM;
			txtINSP_REQ_NO.Value = strINSP_REQ_NO;

		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				dt.Clear();

				string strQuery = " usp_T_DEFECT_INFO_CUDR ";
				strQuery += "  @pTYPE = 'P1'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				strQuery += ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
				strQuery += ", @pINSP_REQ_NO = '" + strINSP_REQ_NO + "'";
				strQuery += ", @pMVMT_NO = '" + strMVMT_NO + "'";
				strQuery += ", @pMVMT_SEQ = '" + strMVMT_SEQ + "'";
				strQuery += ", @pGUBUN = '" + strGUBUN + "'";
				strQuery += ", @pINSP_ITEM_CD = '" + strINSP_ITEM_CD + "'";
				strQuery += ", @pINSP_SERIES = '" + strINSP_SERIES + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

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

			this.Cursor = Cursors.WaitCursor;

			if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
			{

				if (Check_Qty() == false)
				{
					MessageBox.Show(SystemBase.Base.MessageRtn("불량수량은 입고수량을 초과할 수 없습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.Cursor = Cursors.Default;
					return;
				}

				if (dLotSum == 0)
				{
					MessageBox.Show(SystemBase.Base.MessageRtn("불량수량은 0보다 커야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.Cursor = Cursors.Default;
					return;
				}

				if (dLotSum != dDefectQty && bQtyVld == false)
				{
					MessageBox.Show(SystemBase.Base.MessageRtn("LOT불량수량과 검사불량수량은 일치해야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.Cursor = Cursors.Default;
					return;
				}

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					dt = (DataTable)fpSpread1.Sheets[0].DataSource;

					for (int i=0; i <= dt.Rows.Count - 1;i++)
					{
						if (string.IsNullOrEmpty(dt.Rows[i]["INSP_REQ_NO"].ToString())) { dt.Rows[i]["INSP_REQ_NO"] = strINSP_REQ_NO; }
						if (string.IsNullOrEmpty(dt.Rows[i]["INSP_ITEM_CD"].ToString())) { dt.Rows[i]["INSP_ITEM_CD"] = strINSP_ITEM_CD; }
						if (string.IsNullOrEmpty(dt.Rows[i]["INSP_SERIES"].ToString())) { dt.Rows[i]["INSP_SERIES"] = strINSP_SERIES; }
					}
				}

				// Grid CUD 값 초기화
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U" ||
						fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
					{
						fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = (fpSpread1.Sheets[0].ActiveRowIndex + 1).ToString();
					}
				}

				strSaveYN = "Y";
				this.DialogResult = DialogResult.OK;
				Close();
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 유효성 검사 : 불량수량 <= 입고수량
		private bool Check_Qty()
		{
			bool bValid = true;

			dLotSum = 0;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value) >
						Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value))
					{
						bValid = false;
						dLotSum = 0;
						break;
					}
					else
					{
						dLotSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value);
					}
				}
			}

			return bValid;
		}
		#endregion
		
		#region 최종 입고 수량 자동 계산
		private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Text) == false)
				{
					fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "최종입고수량")].Value =
						Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value) -
						 Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value);
				}
				else
				{
					fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "최종입고수량")].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value);
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

	}
}
