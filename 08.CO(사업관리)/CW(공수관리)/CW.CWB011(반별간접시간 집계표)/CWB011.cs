#region 작성정보
/*********************************************************************/
// 단위업무명 : 간접시간 집계표
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-08
// 작성내용 : 간접시간 집계표
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

using System.Threading;
using WNDW;
namespace CW.CWB011
{
    public partial class CWB011 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public CWB011()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CWB011_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
	
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpWorkDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToString().Substring(0,7);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpWorkDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToString().Substring(0, 7);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region 조회 조건 팝업
        //공장 코드
        private void btnPlantCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //작업장
        private void btnWcCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'P002', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Text = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region TextChanged
        //공장코드
        private void txtPlantCd_TextChanged(object sender, EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        #endregion

        #region 조회조건 TO 날짜 고정
        private void dtpWorkDtFr_ValueChanged(object sender, EventArgs e)
        {
            dtpWorkDtTo.Value = Convert.ToDateTime(dtpWorkDtFr.Value.ToString() + "-01").AddYears(1).AddMonths(-1).ToString().Substring(0,7);
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strDtFr = dtpWorkDtFr.Text + "-01";
                    string strDtTo = Convert.ToDateTime(dtpWorkDtTo.Text + "-01").AddMonths(1).AddDays(-1).ToShortDateString();

                    string strWC = "";
                    if (rdoAll.Checked == true) { strWC = "%"; }
                    else if (rdoA.Checked == true) { strWC = "1"; }
                    else if (rdoB.Checked == true) { strWC = "2"; }
                    else if (rdoC.Checked == true) { strWC = "3"; }

                    string strQuery = "usp_CWB011 @pTYPE = 'S1'";
                    strQuery += ", @pWORK_DT_FR = '" + strDtFr + "'";
                    strQuery += ", @pWORK_DT_TO = '" + strDtTo + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pWC_TYPE = '" + strWC + "'";
                    strQuery += ", @pWC_CD = '" + txtWcCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
            if (fpSpread1.Sheets[0].RowCount > 0) Set_Section();
            this.Cursor = Cursors.Default;
        }

        #endregion

        #region 합계 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;
            int iRowFr = 0;
            int iRowTo = 0;
            int iRowSpan = 0;


            //조회 조건에 맞게 Head명 바꾸기
            for (int i = 5; i < 17; i++)
            {
                string strDtFr = Convert.ToDateTime(dtpWorkDtFr.Value.ToString() + "-01").AddMonths(i - 5).ToString();
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = strDtFr.Substring(2, 5).Replace("-", ".");
            }

            //합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접항목명")].Text == "")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].ColumnSpan = 2;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;

                    //소계 합계 색 변경
                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text == "합계")
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                        else fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                }

                if (i > 0)
                {
                    string strWcCd1 = fpSpread1.Sheets[0].Cells[iRowFr, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text;
                    string strWcCd2 = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text;

                    if (strWcCd1 == strWcCd2 && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "간접항목명")].Text != "")
                    {
                        iRowSpan = (i + 1) - iRowFr;
                    }
                    else
                    {
                        iRowTo = i;
                    }
                }

                if (iRowFr == iRowTo)
                {
                    fpSpread1.Sheets[0].Cells[iRowFr, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].RowSpan = iRowSpan;

                }
                iRowFr = iRowTo;
            }
        }
        #endregion	

        #region Excel 출력
		protected override void ExcelExec()
		{
			if(fpSpread1.Sheets[0].Rows.Count <= 0)
			{
				MessageBox.Show(SystemBase.Base.MessageRtn("B0053"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			th = new Thread(new ThreadStart(Show_Waiting));
			th.Start();

			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = "Excel 다운로드 위치 지정";
			dlg.InitialDirectory = dlg.FileName;
			dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
			dlg.FilterIndex = 1;
			dlg.RestoreDirectory = true;
			dlg.FileName = this.Text.ToString().Replace(@"/","_") +".xls"; 
			dlg.OverwritePrompt = false;

			if (dlg.ShowDialog() == DialogResult.OK)
			{		
				try
				{
					Excel.Application oAppln = null;
					Excel.Workbook oWorkBook = null;
					Excel.Worksheet oWorkSheet = null;
					Excel.Range oRange = null;

					oAppln = new Excel.Application();
					oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
					oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet; 					

					int iRow = 4;
					int iRowFr = 0;
					int iRowTo = 0;	
					int iRowSpan = 0;

					Waiting_Form.Activate();
					Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";
	
					int iColumn = fpSpread1.Sheets[0].Columns.Count;
					int iSumRow = fpSpread1.Sheets[0].Rows.Count;

					oWorkSheet.Cells[1, 2] = "공장";

					oWorkSheet.Cells[1, 3] = txtPlantNm.Text;
					oWorkSheet.Cells[1, iColumn-2] = "인쇄일자";
					oWorkSheet.Cells[1, iColumn-1] = SystemBase.Base.ServerTime("YYMMDD");
					
					oWorkSheet.Cells[2, 2] = "일자"; 
					oWorkSheet.Cells[2, 3] = dtpWorkDtFr.Text + " ~ " + dtpWorkDtTo.Text;					
					oWorkSheet.Cells[2, iColumn-1] = "(단위 : 시간)";
					
					//headers 
					for (int j = 3; j < fpSpread1.Sheets[0].Columns.Count; j++)
					{								
						// header 저장
						oWorkSheet.Cells[3, j] = fpSpread1.Sheets[0].ColumnHeader.Cells[0,j].Text;
					}

					Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

					for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
					{	
						
						string strQRow = "Q"+iRow;
						string strCRow = "C"+iRow;
						string strDRow = "D"+iRow;

						//내용 저장
						for (int colNo = 3; colNo < fpSpread1.Sheets[0].Columns.Count; colNo++)
						{
							oWorkSheet.Cells[iRow, colNo] = fpSpread1.Sheets[0].Cells[rowNo,colNo].Text;
							
						}							

						//합계 Merge
						if(fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "간접항목명")].Text == "")
						{
							oWorkSheet.Cells[iRow, 4] = "";
							oWorkSheet.get_Range(strCRow, strDRow).Merge(true);	
						
							//소계 합계 색 변경
							if(fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text == "합계")
								oWorkSheet.get_Range(strCRow, strQRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor1);
							else oWorkSheet.get_Range(strCRow, strQRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor2);
							
							oWorkSheet.get_Range(strCRow, strDRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
						}

						if(rowNo > 0)
						{
							string strWcCd1 = fpSpread1.Sheets[0].Cells[iRowFr, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text;
							string strWcCd2 = fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text;
				
							if(strWcCd1 == strWcCd2 && fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "간접항목명")].Text != "")
							{						
								iRowSpan = (rowNo +1) - iRowFr;
							}
							else
							{
								iRowTo = rowNo;
							}
							if(iRowFr == iRowTo)
							{
								for(int k = 1; k < iRowSpan; k++)
								{
									oWorkSheet.Cells[(iRowFr+4+k),3] = "";
								}
								
								oWorkSheet.get_Range("C"+(iRowFr+4), "C"+ (iRowTo + (iRowSpan +3))).Merge(false);
					
							}
						}		
						
						iRowFr = iRowTo;
						
						//순번
						oWorkSheet.Cells[iRow, 2] = rowNo + 1;	
						oWorkSheet.get_Range("B3","B"+iRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;	
						oWorkSheet.get_Range("B3","B"+iRow).Interior.Color = ColorTranslator.ToOle(Color.Beige);							
						iRow++;
						
						Waiting_Form.progressBar_temp.Value = rowNo+1;
						Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (rowNo+1).ToString() + " Row를 저장하였습니다.";
					}

					string strColumn = "Q" + (iRow - 1);

					
					//헤드 색지정,테두리 설정
					oRange = oWorkSheet.get_Range("B3", "Q3");
					oRange.Interior.Color = ColorTranslator.ToOle(Color.Beige);
					oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
					oRange.Borders.LineStyle = 1;
					oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);	

					//내용 테두리 설정					
					oRange = oWorkSheet.get_Range("B4", strColumn);
					oRange.Borders.LineStyle = 1;
					oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);

					Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";

					oRange = oWorkSheet.get_Range("A1", strColumn);
					oRange.EntireColumn.AutoFit();
					oAppln.UserControl = false;
					oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

					// 엑셀 파일로 저장
					oWorkBook.SaveAs( dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);						

					System.Runtime.InteropServices.Marshal.ReleaseComObject(oWorkBook);

					Waiting_Form.label_temp.Text = "완료되었습니다.";
				}
				catch (Exception )
				{
					th.Abort();
					//SystemBase.Loggers.Log(this.Name, f.ToString());
					//MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.				
				}
			}
			th.Abort();	
		}

		private void Show_Waiting()
		{
			Waiting_Form = new UIForm.ExcelWaiting();
			Waiting_Form.ShowDialog();
		}
		#endregion	

    }
}
