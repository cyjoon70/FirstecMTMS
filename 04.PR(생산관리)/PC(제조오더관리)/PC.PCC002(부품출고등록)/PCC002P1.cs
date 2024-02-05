#region 작성정보
/*********************************************************************/
// 단위업무명 : Lot 정보 조회 및 출고 저장
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-08-20
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
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PC.PCC002
{
	public partial class PCC002P1 : UIForm.FPCOMM1
	{

		#region 변수선언

		public DataTable dt = null;
		public DataTable dtBarCD = null;	// 바코드 출고 데이터
		public PCC002 M1 = new PCC002();
		public decimal dLotSum = 0;
		decimal dSum = 0;
		bool bLotStock = true;	// lot별 재고수량과 출고수량을 비교. (lot별 재고수량 < 출고수량) => false 

		// 팝업화면에서 CUD 발생여부 체크. 발생했다면 Parent Form Reload
		public string strSaveYN = string.Empty;
		
		public bool bReadOnlyYN = true;					// 검색조건 변경 여부
		public string strPLANT_CD = string.Empty;		
		public string strPROJECT_NO = string.Empty;		
		public string strPROJECT_NM = string.Empty;		
		public string strPROJECT_SEQ = string.Empty;
		public string strMVMT_SEQ = string.Empty;	
		public string strITEM_CD = string.Empty;		
		public string strITEM_NM = string.Empty;		
		public string strLOT_NO = string.Empty;			
		public string strBAR_CODE = string.Empty;		
		public string strTRAN_NO = string.Empty;		
		public string strTRAN_SEQ = string.Empty;
		public string strITEM_SPEC = string.Empty;
		public string strREM_QTY = string.Empty;		// 출고잔량
		public string strWORKORDER_NO = string.Empty;	// 작업지시번호
		public string strPROC_SEQ = string.Empty;		// 공정순서
		public string strOUT_QTY = string.Empty;		// 출고수량
		#endregion

		#region 생성자
		public PCC002P1()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void PCC002P1_Load(object sender, EventArgs e)
		{
			try
			{
				this.Text = "부품출고 LOT 선택";

				SystemBase.Validation.GroupBox_Setting(groupBox1);
				UIForm.Buttons.ReButton("010000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
				SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE='PLANT'");	//공장
				UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

				if (string.IsNullOrEmpty(strPROJECT_SEQ)) { strPROJECT_SEQ = "0"; }
				if (string.IsNullOrEmpty(strMVMT_SEQ)) { strMVMT_SEQ = "0"; }
				
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

				string strQuery = " usp_PCC002 ";
				strQuery += "  @pTYPE = 'P1'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				strQuery += ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
				strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Value + "'";
				strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Value + "'";
				strQuery += ", @pITEM_CD = '" + txtItemCd.Value + "'";
				strQuery += ", @pWORKORDER_NO = '" + strWORKORDER_NO + "'";
				strQuery += ", @pPROC_SEQ = '" + strPROC_SEQ + "'";
				
				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					SetBarCDOut();
					CheckQty();
				}
				else
				{
					this.Close();
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

		#region 바코드 출고 데이터 연동
		private void SetBarCDOut()
		{
			if (dtBarCD.Rows.Count > 0)
			{

				if (fpSpread1.Sheets[0].Rows.Count == 1)
				{
					if (string.IsNullOrEmpty(strOUT_QTY)) { strOUT_QTY = "0.0"; }
					fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = Convert.ToDecimal(strOUT_QTY);
				}
				else
				{

					for (int i = 0; i <= dtBarCD.Rows.Count - 1; i++)
					{
						for (int j = 0; j <= fpSpread1.Sheets[0].Rows.Count - 1; j++)
						{
							if (
								string.Compare(dtBarCD.Rows[i]["BAR_CODE"].ToString(), fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text, true) == 0 &&
								string.Compare(dtBarCD.Rows[i]["LOT_NO"].ToString(), fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text, true) == 0 &&
								string.Compare(dtBarCD.Rows[i]["MVMT_SEQ"].ToString(), fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text, true) == 0 &&
								string.Compare(strPROJECT_SEQ, fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text, true) == 0 &&
								Convert.ToDecimal(dtBarCD.Rows[i]["OUT_QTY"]) > 0
							   )
							{
								fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = strOUT_QTY;
							}
						}
					}
				}
			}
			else
			{
				SetAutoQty();
			}
		}
		#endregion

		#region 출고수량 자동 지정
		private void SetAutoQty()
		{
			decimal dReq = 0;		// 요청수량
			decimal dRem = 0;		// 잔량
			decimal dStock = 0;		// 재고수량
			decimal dPreQty = 0;	// 기 출고 수량
			decimal dCurQty = 0;	// 실 출고 수량

			if (string.IsNullOrEmpty(strREM_QTY) == false) { dReq = Convert.ToDecimal(strREM_QTY); }

			for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
			{
				dStock = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value);

				if (dStock >= (dReq - dPreQty))
				{
					dCurQty = dReq - dPreQty;
				}
				else
				{
					dCurQty = dStock;
					dPreQty += dStock;
					dStock = 0;
				}

				fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = dCurQty;
				
				if (dStock >= (dReq - dPreQty))
				{
					break;
				}
			}
		}
		#endregion

		#region 출고수량 조회
		private decimal GetOutQty(string BAR_CODE, string LOT_NO, string MVMT_SEQ)
		{
			decimal dRetuen = 0;

			for (int i = 0; i <= dtBarCD.Rows.Count - 1; i++)
			{
				if (
					string.Compare(dtBarCD.Rows[i]["BAR_CODE"].ToString(), BAR_CODE, true) == 0 &&
					string.Compare(dtBarCD.Rows[i]["LOT_NO"].ToString(), LOT_NO, true) == 0 &&
					string.Compare(dtBarCD.Rows[i]["MVMT_SEQ"].ToString(), MVMT_SEQ, true) == 0
				   )
				{
					dRetuen += Convert.ToDecimal(dtBarCD.Rows[i]["OUT_QTY"]);
				}
			}

			return dRetuen;
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
						MessageBox.Show(SystemBase.Base.MessageRtn("출고수량은 출고잔량을 초과할 수 없습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
					}

					dLotSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
				}

				strSaveYN = "Y";
				this.DialogResult = DialogResult.OK;
				Close();
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;

			return;

			#region 팝업에서 직접 처리하는 로직
			//else
			//{

			//    if (fpSpread1.Sheets[0].Rows.Count > 0)
			//    {

			//        string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
			//        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			//        SqlCommand cmd = dbConn.CreateCommand();
			//        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			//        try
			//        {
			//            /////////////////////////////////////////////// 저장 시작 /////////////////////////////////////////////////
			//            //그리드 상단 필수 체크
			//            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
			//            {

			//                if (CheckOUT_Qty() == false)
			//                {
			//                    if (bLotStock == false)
			//                    {
			//                        MessageBox.Show(SystemBase.Base.MessageRtn("출고수량은 Lot별 재고수량을 초과할 수 없습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			//                        this.Cursor = Cursors.Default;
			//                        bLotStock = true;
			//                        return;
			//                    }
			//                    else
			//                    {
			//                        MessageBox.Show(SystemBase.Base.MessageRtn("출고수량은 출고잔량을 초과할 수 없습니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			//                        this.Cursor = Cursors.Default;
			//                        return;
			//                    }

			//                }

			//                if (dSum == 0)
			//                {
			//                    MessageBox.Show(SystemBase.Base.MessageRtn("출고수량의 합은 0보다 커야 합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			//                    this.Cursor = Cursors.Default;
			//                    return;
			//                }

			//                //행수만큼 처리
			//                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
			//                {
			//                    string strHead = string.Empty;
			//                    string strGbn = "";


			//                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value) > 0)
			//                    {
			//                        if (string.IsNullOrEmpty(strTRAN_NO) == true)
			//                        {
			//                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
			//                        }
			//                        else

			//                            strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

			//                        if (strHead.Length > 0)
			//                        {
			//                            switch (strHead)
			//                            {
			//                                case "U": strGbn = "U1"; break;
			//                                case "I": strGbn = "I1"; break;
			//                                case "D": strGbn = "D1"; break;
			//                                default: strGbn = ""; break;
			//                            }

			//                            // 출고번호가 없으면 cud 구분자는 "I"로 변경
			//                            if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고번호")].Text) == true) { strGbn = "I1"; }

			//                            string strSql = " usp_T_OUT_INFO_CUDR ";
			//                            strSql += "  @pTYPE        = '" + strGbn + "'";
			//                            strSql += ", @pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
			//                            strSql += ", @pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
			//                            strSql += ", @pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
			//                            strSql += ", @pMVMT_NO     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
			//                            strSql += ", @pMVMT_SEQ    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";
			//                            strSql += ", @pOUT_TRAN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고번호")].Text + "' ";
			//                            strSql += ", @pOUT_TRAN_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고순번")].Text + "' ";
			//                            strSql += ", @pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
			//                            strSql += ", @pTR_TYPE     = 'O' ";
			//                            strSql += ", @pOUT_DATE    = NULL ";
			//                            strSql += ", @pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
			//                            strSql += ", @pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트 번호")].Text + "' ";
			//                            strSql += ", @pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
			//                            strSql += ", @pOUT_QTY     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text + "' ";
			//                            strSql += ", @pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
			//                            strSql += ", @pREMARK      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
			//                            strSql += ", @pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
			//                            strSql += ", @pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";

			//                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
			//                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
			//                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
			//                            if (ERRCode != "OK") { Trans.Rollback(); dLotSum = 0; goto Exit; }	// ER 코드 Return시 점프

			//                        }

			//                        dLotSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);

			//                    }

			//                }
			//            }
			//            else
			//            {
			//                Trans.Rollback();
			//                this.Cursor = Cursors.Default;
			//                return;
			//            }

			//            Trans.Commit();
			//            SearchExec();
			//            strSaveYN = "Y";
			//        }
			//        catch (Exception e)
			//        {
			//            SystemBase.Loggers.Log(this.Name, e.ToString());
			//            Trans.Rollback();
			//            ERRCode = "ER";
			//            MSGCode = e.Message;
			//        }
			//    Exit:
			//        dbConn.Close();

			//        if (ERRCode == "OK")
			//        {
			//            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			//            SearchExec();
			//        }
			//        else if (ERRCode == "ER")
			//        {
			//            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			//        }
			//        else
			//        {
			//            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
			//        }

			//        this.Cursor = Cursors.Default;
			//    }

			//}
			#endregion

		}
		#endregion

		#region 출고수량 <= 출고잔량 체크
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


			if (dSum > Convert.ToDecimal(txtREM_QTY.Value))
				bValid = false;
			else
				bValid = true;

			if (bLotStock == false) { bValid = false; }

			return bValid;
		}
		#endregion

		#region 합계 출고수량, 차이수량 계산
		private void fpSpread1_EditModeOff(object sender, EventArgs e)
		{
			try
			{
				if (CheckQty() == false) { return; }
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show("출고수량 계산 작업 중 오류가 발생했습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		
		private bool CheckQty()
		{
			bool bReturn = true;

			decimal dSum = 0; // 충 출고수량
			decimal dRem = 0; // 출고잔량

			for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
			{
				if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].ToString()) == false)
				{
					dSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
				}
			}

			if (string.IsNullOrEmpty(strREM_QTY) == false) { dRem = Convert.ToDecimal(strREM_QTY); }

			if (dRem < dSum)
			{
				MessageBox.Show("합계출고수량이 출고잔량을 초과합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = 0;
				bReturn = false;
			}

			dSum = 0;

			for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
			{
				if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].ToString()) == false)
				{
					dSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
				}
			}

			txtTotalQty.Value = dSum.ToString("##,##0.0000");
			txtBalQty.Value = (dRem - dSum).ToString("##,##0.0000");

			return bReturn;
		}
		#endregion

	}
}
