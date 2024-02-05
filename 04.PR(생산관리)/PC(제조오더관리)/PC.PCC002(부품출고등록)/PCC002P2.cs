#region 작성정보
/*********************************************************************/
// 단위업무명 : 바코드 출고
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-10-01
// 작성내용 : 바코드 출고 등록 관리
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

namespace PC.PCC002
{
	public partial class PCC002P2 : UIForm.FPCOMM1
	{

		#region 변수
		public DataTable dtOutInfo = new DataTable();	// Parent Form 전송용 테이블. 
		public DataTable dtOut = new DataTable();		// 출고처리 원본 테이블
		DataTable dt = new DataTable();
		public DataTable dtBarCD = null;	// 바코드 출고 데이터(중복방지용 데이터 확인 테이블)
		FarPoint.Win.Spread.FpSpread spd;
		decimal dReqTtl = 0;	// 필요수량
		decimal dOutTtl = 0;	// 출고수량
		decimal dBalQty = 0;	// 잔량
		bool bCalc = true;		// 수량 계산 여부(유효한 바코드의 경우만 계산)
		#endregion

		#region 생성자
		public PCC002P2()
		{
			InitializeComponent();
		}

		public PCC002P2(FarPoint.Win.Spread.FpSpread spread)
		{
			InitializeComponent();
			spd = spread;
		}
		#endregion

		#region Form Load
		private void PCC002P2_Load(object sender, EventArgs e)
		{
			try 
			{
				this.Text = "바코드 출고처리";

				UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
				
				SystemBase.Validation.GroupBox_Setting(groupBox1);
				txtBarCode.Focus();
								
				UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

				// DataTable 칼럼 디자인을 하지 않기위해 구조만 가져옴.
				dtOutInfo = SystemBase.DbOpen.NoTranDataTable("SELECT BAR_CODE,MVMT_NO,MVMT_SEQ,OUT_TRAN_NO,OUT_TRAN_SEQ,LOT_NO,OUT_QTY, PROC_SEQ, '''' AS WORKORDER_NO, ITEM_CD FROM T_OUT_INFO WHERE BAR_CODE = ''");
				dtOut = SystemBase.DbOpen.NoTranDataTable("SELECT BAR_CODE,MVMT_NO,MVMT_SEQ,OUT_TRAN_NO,OUT_TRAN_SEQ,LOT_NO,OUT_QTY, PROC_SEQ, '''' AS WORKORDER_NO, ITEM_CD FROM T_OUT_INFO WHERE BAR_CODE = ''");
			}
			catch (Exception f)
			{
				MessageBox.Show(f.Message);
			}
		}
		#endregion

		#region 바코드 입력 이벤트
		private void txtBarCode_KeyPress(object sender, KeyPressEventArgs e)
		{
			try
			{
				if (e.KeyChar == (char)Keys.Enter)
				{
					SearchExec();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002", SystemBase.Base.MessageRtn("Z0002")), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			txtBarCode.Value = "";
			txtBarCode.Focus();
			fpSpread1.Sheets[0].Rows.Count = 0;
			dtOutInfo.Clear();
			dt.Clear();
			dReqTtl = 0;
			dOutTtl = 0;
			dBalQty = 0;
			bCalc = true;
		}
		#endregion

		#region Control Reset
		private void ControlReset()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			txtBarCode.Value = "";
			fpSpread1.Sheets[0].Rows.Count = 0;
		}
		#endregion

		#region 그리드 조회
		protected override void SearchExec()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{
				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
				{

					string strQuery = " usp_PCC002 ";
					strQuery += "  @pTYPE = 'P2' ";
					strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
					strQuery += ", @pS_BAR_CODE = '" + txtBarCode.Text + "' ";

					dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

					if (dt.Rows.Count == 0)
					{
						txtResult.ForeColor = Color.Red;
						txtResult.Value = txtBarCode.Text + " -> \r\n검색된 데이터가 없습니다.";
						txtBarCode.Value = "";
						return;
					}
					else
					{
						if (CheckDouble())
						{
							txtOutQty.Value = "";
							txtRemQty.Value = "";
							txtReqQty.Value = "";
							txtStockQty.Value = "";

							txtResult.ForeColor = Color.Red;
							txtResult.Value = txtBarCode.Text + " -> \r\n이미 처리된 바코드입니다.";
							txtBarCode.Value = "";

							return;
						}

						txtResult.Value = "";
					}

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

					SetOutInfo();

					if (bCalc)
					{
						SetControl();

					}
					else 
					{
						txtOutQty.Value = "";
						txtRemQty.Value = "";
						txtReqQty.Value = "";
						txtStockQty.Value = "";
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
			finally 
			{
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
						
		}
		#endregion

		#region 이미 처리한 바코드인지 체크
		private bool CheckDouble()
		{
			bool bReturn = false;

			// dtOutInfo Parent Form 전송용 테이블. 
			// dtBarCD 바코드 출고 데이터(중복방지용 데이터 확인 테이블)

			// 팝업화면 확인
			if (dtOutInfo.Rows.Count > 0)
			{
				for (int i = 0; i <= dt.Rows.Count - 1; i++)
				{
					for (int j = 0; j <= dtOutInfo.Rows.Count - 1; j++)
					{
						if (
							string.Compare(dtOutInfo.Rows[j]["BAR_CODE"].ToString(), dt.Rows[i]["BAR_CODE"].ToString(), true) == 0 &&
							string.Compare(dtOutInfo.Rows[j]["MVMT_NO"].ToString(), dt.Rows[i]["MVMT_NO"].ToString(), true) == 0 &&
							string.Compare(dtOutInfo.Rows[j]["MVMT_SEQ"].ToString(), dt.Rows[i]["MVMT_SEQ"].ToString(), true) == 0 &&
							string.Compare(dtOutInfo.Rows[j]["LOT_NO"].ToString(), dt.Rows[i]["LOT_NO"].ToString(), true) == 0
						   )
						{
							bReturn = true;
							break;
						}
					}
				}
			}

			// 전송된 데이터에서 확인
			if (dtBarCD.Rows.Count > 0)
			{
				for (int i = 0; i <= dt.Rows.Count - 1; i++)
				{
					for (int j = 0; j <= dtBarCD.Rows.Count - 1; j++)
					{
						if (
							string.Compare(dtBarCD.Rows[j]["BAR_CODE"].ToString(), dt.Rows[i]["BAR_CODE"].ToString(), true) == 0 &&
							string.Compare(dtBarCD.Rows[j]["MVMT_NO"].ToString(), dt.Rows[i]["MVMT_NO"].ToString(), true) == 0 &&
							string.Compare(dtBarCD.Rows[j]["MVMT_SEQ"].ToString(), dt.Rows[i]["MVMT_SEQ"].ToString(), true) == 0 &&
							string.Compare(dtBarCD.Rows[j]["LOT_NO"].ToString(), dt.Rows[i]["LOT_NO"].ToString(), true) == 0
						   )
						{
							bReturn = true;
							break;
						}
					}
				}
			}

			return bReturn;
		}
		#endregion

		#region 회면 기본 설정
		private void SetControl()
		{

			dReqTtl = 0;
			dOutTtl = 0;
			dBalQty = 0;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				txtStockQty.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Text;

				// 필요수량
				if (dt != null && dt.Rows.Count > 0)
				{
					for (int j = 0; j <= dt.Rows.Count - 1; j++)
					{
						for (int i = 0; i <= spd.Sheets[0].Rows.Count - 1; i++)
						{
							if (Convert.ToDecimal(spd.Sheets[0].Cells[i, 15].Value) >=
								Convert.ToDecimal(spd.Sheets[0].Cells[i, 14].Value))
							{
								if (
									string.Compare(spd.Sheets[0].Cells[i, 31].Text, dt.Rows[j]["PROJECT_NO"].ToString(), true) == 0 &&
									//string.Compare(spd.Sheets[0].Cells[i, 32].Text, dt.Rows[j]["PROJECT_SEQ"].ToString(), true) == 0 &&
									string.Compare(spd.Sheets[0].Cells[i, 5].Text, dt.Rows[j]["ITEM_CD"].ToString(), true) == 0
									)
								{
									dReqTtl += Convert.ToDecimal(spd.Sheets[0].Cells[i, 14].Value);
								}
							}
						}
					}

					txtReqQty.Value = dReqTtl.ToString("##,##0.0000");
				}

				// 출고수량
				if (dtOutInfo != null && dtOutInfo.Rows.Count > 0)
				{
					for (int i = 0; i <= dtOutInfo.Rows.Count - 1; i++)
					{
						dOutTtl += Convert.ToDecimal(dtOutInfo.Rows[i]["OUT_QTY"]);
					}

					txtOutQty.Value = dOutTtl.ToString("##,##0.0000");
				}

				// 잔량
				dBalQty = dReqTtl - dOutTtl;

				txtRemQty.Value = dBalQty.ToString("##,##0.0000");

			}
		}
		#endregion

		#region 출고처리
		private void SetOutInfo()
		{
			bool bValid = false;	// 출고 리스트에 대한 바코드 유효성 체크
			bool bComplete = true;	// 처리 완료 여부
			decimal dStockQty = 0;	// 재고 수량
			decimal dReqQty = 0;	// 출고 요청 수량(출고 잔량)
			decimal dPreQty = 0;	// 기 출고 수량
			bCalc = true;

			// 바코드 관련 정보
			for (int j = 0; j <= dt.Rows.Count - 1; j++)
			{
				dStockQty += Convert.ToDecimal(dt.Rows[j]["STOCK_QTY"]);

				// 출고관련 정보
				for (int i = 0; i <= spd.Sheets[0].Rows.Count - 1; i++)
				{

					if (Convert.ToDecimal(spd.Sheets[0].Cells[i, 15].Value) >=
						Convert.ToDecimal(spd.Sheets[0].Cells[i, 14].Value))
					{

						if (
							string.Compare(spd.Sheets[0].Cells[i, 31].Text, dt.Rows[j]["PROJECT_NO"].ToString(), true) == 0 &&
							string.Compare(spd.Sheets[0].Cells[i, 5].Text, dt.Rows[j]["ITEM_CD"].ToString(), true) == 0
							)
						{

							bValid = true;

							dReqQty = Convert.ToDecimal(spd.Sheets[0].Cells[i, 14].Value);	// 출고 요청 수량(출고 잔량)
							dPreQty = Convert.ToDecimal(spd.Sheets[0].Cells[i, 49].Value);	// 기 출고 수량

							if (Convert.ToDecimal(spd.Sheets[0].Cells[i, 14].Value) != Convert.ToDecimal(spd.Sheets[0].Cells[i, 19].Value))
							{
								if (dStockQty >= (dReqQty - dPreQty))
								{

									if (dReqQty - dPreQty > 0)
									{
										spd.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
										spd.Sheets[0].Cells[i, 1].Value = "True";
										spd.Sheets[0].Cells[i, 19].Value = dPreQty + dReqQty - dPreQty;
										spd.Sheets[0].Cells[i, 49].Value = dPreQty + (dReqQty - dPreQty);
										spd.Sheets[0].Cells[i, 17].Value = dt.Rows[j]["LOT_NO"];
									}

									dStockQty = dStockQty - (dReqQty - dPreQty);

									DataRow dr = dtOut.NewRow();
									dr["BAR_CODE"] = dt.Rows[j]["BAR_CODE"];
									dr["MVMT_NO"] = dt.Rows[j]["MVMT_NO"];
									dr["MVMT_SEQ"] = dt.Rows[j]["MVMT_SEQ"];
									dr["OUT_TRAN_NO"] = "";
									dr["OUT_TRAN_SEQ"] = 0;
									dr["LOT_NO"] = dt.Rows[j]["LOT_NO"];
									dr["OUT_QTY"] = dReqQty - dPreQty;
									dr["PROC_SEQ"] = dt.Rows[j]["PROC_SEQ"];
									dr["ITEM_CD"] = dt.Rows[j]["ITEM_CD"];
									dr["WORKORDER_NO"] = spd.Sheets[0].Cells[i, 4].Text;

									dtOut.Rows.Add(dr);

									DataRow dr2 = dtOut.Rows[dtOut.Rows.Count - 1];

									if (Convert.ToDecimal(dr["OUT_QTY"]) > 0)
									{
										dtOutInfo.Rows.Add(dr2.ItemArray);
									}

									bComplete = true;
									
								}
								else
								{

									if (dPreQty + dStockQty > 0)
									{
										spd.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
										spd.Sheets[0].Cells[i, 1].Value = "True";
										spd.Sheets[0].Cells[i, 19].Value = dPreQty + dStockQty;
										spd.Sheets[0].Cells[i, 49].Value = dPreQty + dStockQty;
										spd.Sheets[0].Cells[i, 17].Value = dt.Rows[j]["LOT_NO"];
									}

									txtResult.Value = "";
									txtResult.ForeColor = Color.Red;
									txtResult.Value = txtBarCode.Text + " -> \r\nLot 재고수량이 부족합니다.\r\n다른 바코드를 처리해 주세요.";
									txtBarCode.Value = "";
									bComplete = false;

									DataRow dr = dtOut.NewRow();
									dr["BAR_CODE"] = dt.Rows[j]["BAR_CODE"];
									dr["MVMT_NO"] = dt.Rows[j]["MVMT_NO"];
									dr["MVMT_SEQ"] = dt.Rows[j]["MVMT_SEQ"];
									dr["OUT_TRAN_NO"] = "";
									dr["OUT_TRAN_SEQ"] = 0;
									dr["LOT_NO"] = dt.Rows[j]["LOT_NO"];
									dr["OUT_QTY"] = dPreQty + dStockQty;
									dr["PROC_SEQ"] = dt.Rows[j]["PROC_SEQ"];
									dr["ITEM_CD"] = dt.Rows[j]["ITEM_CD"];
									dr["WORKORDER_NO"] = spd.Sheets[0].Cells[i, 4].Text;

									dtOut.Rows.Add(dr);

									DataRow dr2 = dtOut.Rows[dtOut.Rows.Count - 1];

									if (Convert.ToDecimal(dr["OUT_QTY"]) > 0)
									{
										dtOutInfo.Rows.Add(dr2.ItemArray);
									}

									//ControlReset();

									break;
								}

							}

						}
					}

				}

				if (bValid == false) 
				{
					//ControlReset();
					txtResult.Value = "";
					txtResult.ForeColor = Color.Red;
					txtResult.Value = txtBarCode.Text + " -> \r\n현 출고에는 포함되지 않는 바코드입니다.";
					txtBarCode.Value = "";
					bCalc = false;
					return; 
				}
				
				if (bComplete) 
				{
					ControlReset();
					txtResult.Value = "";
					txtResult.ForeColor = Color.Black;
					txtResult.Value = txtBarCode.Text + " -> \r\nLot별 출고 수량 배분작업이\r\n완료되었습니다.";
				}

			}
		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}
		#endregion

		#region 종료 이벤트
		private void BtnClose_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void PCC002P2_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}
		#endregion
		
	}
}
	