#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 작업시간 집계표
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-22
// 작성내용 : 프로젝트별 작업시간 집계표 및 관리
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
using WNDW;
using System.Threading;

namespace CW.CWB001
{
    public partial class CWB001 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        #region 생성자
        public CWB001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void CWB001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).AddMonths(1).ToString().Substring(0, 7);
            
            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;	
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅		
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).AddMonths(1).ToString().Substring(0, 7);
            dtpWorkDtTo.Value = Convert.ToDateTime(dtpWorkDtFr.Value).AddYears(1).AddMonths(-1);

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion

        #region 공장 코드 팝업
        private void btnPlantCd_Click(object sender, System.EventArgs e)
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

                    txtPlantCd.Value = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region TextChanged
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlantNm.Value = "";
                }
            }
            catch
            {

            }
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
                    string strDtFr = dtpWorkDtFr.Text + "-01";
                    string strDtTo = Convert.ToDateTime(dtpWorkDtTo.Value).AddMonths(1).Date.ToString().Substring(0,7);
                    strDtTo = strDtTo.Substring(0, 7) + "-01";
                    string strDtTo1 = Convert.ToDateTime(strDtTo).AddDays(-1).ToShortDateString().Substring(0,7);

                    string strQuery = "usp_CWB001 @pTYPE = 'S1'";
                    strQuery += ", @pWORK_DT_FR = '" + strDtFr + "'";
                    strQuery += ", @pWORK_DT_TO = '" + strDtTo1 + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
                if (fpSpread1.Sheets[0].RowCount > 0) Set_Section();
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region 합계 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;

            //조회 조건에 맞게 Head명 바꾸기
            for (int i = 3; i < 15; i++)
            {
                string strDtFr = Convert.ToDateTime(dtpWorkDtFr.Value).AddMonths(i - 3).ToString().Substring(0,7);
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = strDtFr.Substring(2, 5).Replace("-", ".");
            }

            //합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == "합계")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].ColumnSpan = 2;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;

                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                    }

                }
            }
        }
        #endregion

        #region 조회조건 TO 날짜 고정
        private void dtpWorkDtFr_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (dtpWorkDtTo.Text != "")
                {
                    dtpWorkDtTo.Value = Convert.ToDateTime(dtpWorkDtFr.Value).AddYears(1).AddMonths(-1);
                }
                else
                {
                    dtpWorkDtTo.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Excel 출력
        protected override void ExcelExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count <= 0)
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
            dlg.FileName = this.Text.ToString().Replace(@"/", "_") + ".xls";
            dlg.OverwritePrompt = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    Waiting_Form.Activate();
                    Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

                    int iColumn = fpSpread1.Sheets[0].Columns.Count;

                    Excel.Application oAppln = null;
                    Excel.Workbook oWorkBook = null;
                    Excel.Worksheet oWorkSheet = null;
                    Excel.Range oRange = null;

                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    oAppln.Visible = false;

                    oWorkSheet.Cells[1, 2] = "공장";

                    oWorkSheet.Cells[1, 3] = txtPlantNm.Text;
                    oWorkSheet.Cells[1, iColumn] = "인쇄일자";
                    oWorkSheet.Cells[1, iColumn + 1] = SystemBase.Base.ServerTime("YYMMDD");

                    oWorkSheet.Cells[2, 2] = "일자";
                    oWorkSheet.Cells[2, 3] = dtpWorkDtFr.Text + " ~ " + dtpWorkDtTo.Text;
                    oWorkSheet.Cells[2, iColumn + 1] = "(단위 : 시간)";

                    //headers 
                    for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                    {
                        // header 저장
                        oWorkSheet.Cells[3, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text;
                    }

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    int iRow = 4;
                    int iSumRow = fpSpread1.Sheets[0].Rows.Count;

                    for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
                    {

                        string strQRow = "Q" + iRow;
                        string strCRow = "C" + iRow;
                        string strDRow = "D" + iRow;

                        //내용 저장
                        for (int colNo = 0; colNo < fpSpread1.Sheets[0].Columns.Count; colNo++)
                        {
                            oWorkSheet.Cells[iRow, colNo + 2] = fpSpread1.Sheets[0].Cells[rowNo, colNo].Text;
                        }

                        //합계 Merge
                        if (fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == "합계")
                        {
                            oWorkSheet.Cells[iRow, 4] = "";
                            oWorkSheet.get_Range(strCRow, strDRow).Merge(true);
                            oWorkSheet.get_Range(strCRow, strQRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor1);
                            oWorkSheet.get_Range(strCRow, strDRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        }

                        //순번
                        oWorkSheet.Cells[iRow, 2] = rowNo + 1;
                        oWorkSheet.get_Range("B3", "B" + iRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        oWorkSheet.get_Range("B3", "B" + iRow).Interior.Color = ColorTranslator.ToOle(Color.Beige);
                        iRow++;

                        Waiting_Form.progressBar_temp.Value = rowNo + 1;
                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (rowNo + 1).ToString() + " Row를 저장하였습니다.";
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

                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oWorkBook);

                    Waiting_Form.label_temp.Text = "완료되었습니다.";

                }
                catch (Exception)
                {
                    th.Abort();
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
