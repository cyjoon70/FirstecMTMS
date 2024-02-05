#region 작성정보
/*********************************************************************/
// 단위업무명 : 현재고 별 lot 재고 조회
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-11-14
// 작성내용 : 현재고 별 lot 재고 조회
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
using WNDW;

namespace IT.ITR003
{
	public partial class ITR003P3 : UIForm.FPCOMM1
	{

		#region 변수
		FarPoint.Win.Spread.FpSpread spd;
		#endregion

		#region 생성자
		public ITR003P3()
		{
			InitializeComponent();
		}

		public ITR003P3(FarPoint.Win.Spread.FpSpread fpSpread)
		{
			InitializeComponent();
			spd = fpSpread;
		}
		#endregion

		#region Form Load
		private void ITR003P3_Load(object sender, EventArgs e)
		{
			
			this.Text = "현재고";

			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//버튼 재정의(조회권한만)
			UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
			SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정
			cboItemAcct.SelectedIndex = 3;	// 원자재 지정

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			
		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{

				txtBalQty.Value = 0;
				txtMovQty.Value = 0;
				txtSumQty.Value = 0;

				string strCloseYn = "";
				if (rdoY.Checked == true)
				{
					strCloseYn = "Y";
				}
				else if (rdoN.Checked == true)
				{
					strCloseYn = "N";
				}

				string strQuery = " usp_ITR003  @pTYPE = 'P3'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "' ";
				strQuery += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue + "' ";
				strQuery += ", @pITEM_CD = '" + txtITEM_CD.Text.Trim() + "' ";
				strQuery += ", @pCLOSE_YN = '" + strCloseYn + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);


               if (fpSpread1.Sheets[0].RowCount > 0)
                {
                   
                    ////						Set_Span();]
                    int Count = 0;


                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount - 1; i++)
                    {

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text &&
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Text == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Text
                           )
                        {
                            Count++;
                        }
                        else
                        {
                            if (Count > 0)
                            {
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].RowSpan = Count+1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].RowSpan = Count + 1;
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].RowSpan = Count + 1;
                                
                                fpSpread1.Sheets[0].Cells[i - Count, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].RowSpan = Count + 1;

                            }
                            Count = 0;
                        }
                    }

//                         fpSpread1.Sheets[0].
                    //fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    //fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(5, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(6, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(7, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(8, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(9, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(10, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(11, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(12, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(i, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    //fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    //fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(5, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    //fpSpread1.Sheets[0].SetColumnMerge(6, FarPoint.Win.Spread.Model.MergePolicy.Restricted);

                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i , SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                              
                    }
                }

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally 
			{
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
						
		}
		#endregion

		#region 버튼 Click
		private void btnOk_Click(object sender, System.EventArgs e)
		{

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				int j = spd.Sheets[0].Rows.Count;
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"이동수량")].Value) > 0)
					{

						spd.Sheets[0].ActiveRowIndex = spd.Sheets[0].RowCount;

						UIForm.FPMake.RowInsert(spd);
						spd.Sheets[0].Rows.Count = j + 1;
						spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";

						spd.Sheets[0].Cells[j, 2].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;			//품목코드
						spd.Sheets[0].Cells[j, 4].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;			//품명
						spd.Sheets[0].Cells[j, 5].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;				//규격
						spd.Sheets[0].Cells[j, 11].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;	//프로젝트번호    // 2015.12.17. hma 수정: 10=>11
                        spd.Sheets[0].Cells[j, 13].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text;	    //프로젝트명      // 2022.06.13. hma 추가
                        spd.Sheets[0].Cells[j, 14].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;			//프로젝트차수    // 2015.12.17. hma 수정: 12=>13      // 2022.06.13. hma: 13=>14
                        spd.Sheets[0].Cells[j, 21].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;			//창고            // 2015.12.17. hma 수정: 19=>20      // 2022.06.13. hma: 20=>21
                        spd.Sheets[0].Cells[j, 23].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text;			//창고명          // 2015.12.17. hma 수정: 21=>22      // 2022.06.13. hma: 22=>23
                        spd.Sheets[0].Cells[j, 24].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;			//창고위치        // 2015.12.17. hma 수정: 22=>23      // 2022.06.13. hma: 23=>24
                        spd.Sheets[0].Cells[j, 26].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text;			//위치명          // 2015.12.17. hma 수정: 24=>25      // 2022.06.13. hma: 25=>26
						spd.Sheets[0].Cells[j, 6].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value;			//재고단위
                        spd.Sheets[0].Cells[j, 8].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value;		//수불수량        // 2015.12.17. hma 수정: 38=>8
						spd.Sheets[0].Cells[j, 7].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value;		//이동수량
                        spd.Sheets[0].Cells[j, 32].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Value;			//바코드          // 2015.12.17. hma 수정: 30=>31      // 2022.06.13. hma: 31=>32
                        spd.Sheets[0].Cells[j, 33].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Value;		//입고번호        // 2015.12.17. hma 수정: 31=>32      // 2022.06.13. hma: 32=>33
                        spd.Sheets[0].Cells[j, 34].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Value;		//입고순번        // 2015.12.17. hma 수정: 32=>33      // 2022.06.13. hma: 33=>34
                        spd.Sheets[0].Cells[j, 17].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value;			//Lot No          // 2015.12.17. hma 수정: 15=>16      // 2022.06.13. hma: 16=>17
                        spd.Sheets[0].Cells[j, 16].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Value;		//Lot 추적 여부   // 2015.12.17. hma 수정: 14=>15      // 2022.06.13. hma: 15=>16
                        spd.Sheets[0].Cells[j, 27].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단가")].Value;       //출고단가(재고단가)    // 2015.12.17. hma 수정: 25=>26     // 2022.06.13. hma: 26=>27
                        spd.Sheets[0].Cells[j, 28].Value = 
							Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고단가")].Value) *
                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value);					//출고금액         // 2015.12.17. hma 수정: 26=>27      // 2022.06.13. hma: 27=>28
                        spd.Sheets[0].Cells[j, 31].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value;       //기존수량         // 2015.12.17. hma 수정: 29=>30      // 2022.06.13. hma: 30=>31

                        j++;

					}
				}
				

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{

			}
		}
		#endregion

		#region Grid Event
		private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{

			decimal dMovQty = 0;
			decimal dSumQty = 0;
			decimal dBalQty = 0;

			try
			{

				fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

				if (string.IsNullOrEmpty(txtMovQty.Text))
				{
					MessageBox.Show("이동할 수량을 입력해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
					return;
				}
				else
				{
					if (Convert.ToDecimal(txtMovQty.Value) <= 0)
					{
						MessageBox.Show("이동할 수량은 0보다 커야 합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
						return;
					}
				}

				if (ValQty(e.Row) == false)
				{
					MessageBox.Show("[" + e.Row.ToString() + "]행 : 이동한 수량은 재고수량보다 클 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
					return;
				}
								
				dMovQty = Convert.ToDecimal(txtMovQty.Value);
				dSumQty = GetSumQty();
				dBalQty = dMovQty - dSumQty;

				if (dBalQty < 0)
				{
					MessageBox.Show("입력된 수량의 합이 이동할 수량을 초과합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
					return;
				}

				txtSumQty.Value = dSumQty.ToString();
				txtBalQty.Value = dBalQty.ToString();

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{

			}
		}

		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{

			decimal dMovQty = 0;
			decimal dSumQty = 0;
			decimal dBalQty = 0;
			decimal dStockQty = 0;

			try
			{

				if (string.IsNullOrEmpty(txtMovQty.Text))
				{
					MessageBox.Show("이동할 수량을 입력해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				else
				{
					if (Convert.ToDecimal(txtMovQty.Value) <= 0)
					{
						MessageBox.Show("이동할 수량은 0보다 커야 합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}

				if (ValQty(fpSpread1.Sheets[0].ActiveRowIndex) == false)
				{
					MessageBox.Show("[" + fpSpread1.Sheets[0].ActiveRowIndex.ToString() + "]행 : 이동한 수량은 재고수량보다 클 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
					return;
				}

				dMovQty = Convert.ToDecimal(txtMovQty.Value);
				dSumQty = GetSumQty();
				dBalQty = dMovQty - dSumQty;

				if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0)
				{
					dStockQty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Value);
				}
				else 
				{
					dStockQty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value);				
				}

				if (dBalQty < 0)
				{
					MessageBox.Show("입력된 수량의 합이 이동할 수량을 초과합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
					return;
				}

				txtSumQty.Value = dSumQty.ToString();
				txtBalQty.Value = dBalQty.ToString();

				fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.RowMode;

				if (dStockQty >= dBalQty)
				{
					fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = Convert.ToDecimal(txtBalQty.Value);
				}
				else
				{

					if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0)
					{
						fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value =
						Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Value);
					}
					else
					{
						fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value =
							Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value);
					}

					
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{

			}
		}
		#endregion

		#region 품목조회
		private void btnITEM_Click(object sender, System.EventArgs e)
		{

			try
			{
				WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), cboItemAcct.SelectedValue.ToString(), txtITEM_CD.Text);
				pu1.ShowDialog();

				if (pu1.DialogResult == DialogResult.OK)
				{
					string[] Msgs1 = pu1.ReturnVal;

					txtITEM_CD.Value = Msgs1[2].ToString();
					txtITEM_NM.Value = Msgs1[3].ToString();
				}			
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				
			}

		}

		private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
		{

			try
			{
				if (txtITEM_CD.Text != "")
				{
					txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
				}
				else
				{
					txtITEM_NM.Value = "";
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{

			}
		}		
		#endregion

		#region 이동할 수량 관련 이벤트
		private void txtMovQty_TextChanged(object sender, EventArgs e)
		{

			try
			{

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{

					for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
					{
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
					}
				}

				txtSumQty.Value = 0;
				txtBalQty.Value = 0;
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{

			}
		}

		private void txtMovQty_Click(object sender, EventArgs e)
		{
			txtMovQty.SelectAll();
		}
		#endregion

		#region 이동한 수량 합계 조회
		private decimal GetSumQty()
		{
			decimal dSum = 0;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					dSum = dSum + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value);
				}
			}

			return dSum;
		}
		#endregion

		#region 이동수량 유효성 검사
		private bool ValQty(int row)
		{
			bool bSave = true;

			if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0)
			{
				if (
				Convert.ToDecimal(fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Value) <
				Convert.ToDecimal(fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value)
			   )
				{
					bSave = false;
				}	
			}
			else
			{
				if (
					Convert.ToDecimal(fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value) <
					Convert.ToDecimal(fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value)
				   )
				{
					bSave = false;
				}	
			}
			
			return bSave;
		}
		#endregion

	}
}
