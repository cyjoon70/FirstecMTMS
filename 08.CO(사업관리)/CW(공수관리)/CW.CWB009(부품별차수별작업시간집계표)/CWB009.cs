#region 작성정보
/*********************************************************************/
// 단위업무명 : 부품별차수별작업시간집계표
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-22
// 작성내용 : 부품별차수별작업시간집계표 및 관리
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
using FarPoint.Win.Spread.CellType;

namespace CW.CWB009
{
    public partial class CWB009 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        #region 생성자
        public CWB009()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void CWB009_Load(object sender, System.EventArgs e)
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
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).AddMonths(1).ToString().Substring(0,7);

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion
        
        #region 조회조건 팝업
        //공장코드
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

        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text, "N");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region 조회조건 TextChanged
        //공장코드
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

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 조회조건 TO 날짜 고정
        private void dtpWorkDtFr_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (dtpWorkDtFr.Text != "")
                {
                    dtpWorkDtTo.Value = Convert.ToDateTime(dtpWorkDtFr.Value).AddYears(1).AddMonths(-1).ToString().Substring(0, 7);
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

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_CWB009 @pTYPE = 'S2'";
                    strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                    strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                    if (dt.Rows.Count > 0)
                        detail_SearchExec(dt);
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }

        private void detail_SearchExec(DataTable dt)
        {
            try
            {
                //그리드 헤드 디자인
                fpSpread1.Sheets[0].FrozenColumnCount = 4;

                fpSpread1.Sheets[0].AlternatingRows[0].BackColor = Color.FromArgb(230, 230, 230);
                fpSpread1.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(245, 245, 245);

                fpSpread1.Sheets[0].Columns[1].CellType = new TextCellType();
                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                textCellType1.Multiline = true;
                textCellType1.WordWrap = true;
                fpSpread1.Sheets[0].Columns.Get(1).CellType = textCellType1;

                fpSpread1.Sheets[0].ColumnCount = 6 + (dt.Rows.Count * 2);
                fpSpread1.Sheets[0].ColumnHeader.Columns.Count = fpSpread1.Sheets[0].ColumnCount;
                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                FarPoint.Win.Spread.CellType.PercentCellType num1 = new FarPoint.Win.Spread.CellType.PercentCellType();
                num.DecimalSeparator = ".";
                num.DecimalPlaces = 2;
                num.FixedPoint = true;
                num.Separator = ",";
                num.ShowSeparator = true;
                num.MaximumValue = 99999999999999;
                num.MinimumValue = -99999999999999;

                int startCol1 = 4;
                int startCol2 = 5;

                for (int i = 0; i < dt.Rows.Count + 1; i++)
                {
                    if (i != dt.Rows.Count)
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, startCol1].Text = dt.Rows[i]["PROJECT_SEQ"].ToString();
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, startCol1].ColumnSpan = 2;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[1, startCol1].Text = "수량";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[1, startCol2].Text = "공수";

                        fpSpread1.Sheets[0].Columns[startCol1].CellType = num;
                        fpSpread1.Sheets[0].Columns[startCol1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[startCol1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[startCol1].Locked = true;
                        fpSpread1.Sheets[0].Columns[startCol1].Width = 80;

                        fpSpread1.Sheets[0].Columns[startCol2].CellType = num;
                        fpSpread1.Sheets[0].Columns[startCol2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[startCol2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[startCol2].Locked = true;
                        fpSpread1.Sheets[0].Columns[startCol2].Width = 80;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, startCol1].Text = "합 계";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, startCol1].ColumnSpan = 2;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[1, startCol1].Text = "수량";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[1, startCol2].Text = "공수";
                        fpSpread1.Sheets[0].Columns[startCol1].CellType = num;
                        fpSpread1.Sheets[0].Columns[startCol1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[startCol1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[startCol1].Locked = true;
                        fpSpread1.Sheets[0].Columns[startCol1].Width = 80;

                        fpSpread1.Sheets[0].Columns[startCol2].CellType = num;
                        fpSpread1.Sheets[0].Columns[startCol2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[startCol2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[startCol2].Locked = true;
                        fpSpread1.Sheets[0].Columns[startCol2].Width = 80;
                    }
                    startCol1 += 2;
                    startCol2 += 2;
                }

                //내용입력
                string strQuery = " usp_CWB009 'S1'  ";
                strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";
                strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery);

                string div = "0000", div1 = "0000", div2 = "0000"; //빈값 있음
                string tempDiv = "12345", tempDiv1 = "12345"; //빈값 있음
                int row_idx = -1;
                int col_idx = 0;
                decimal sumQty = 0;
                decimal sumTime = 0;
                int rowspan = 1, rowspan2 = 1;
                int rowfirst = 0, rowfirst2 = 0;

                for (int i = 0; i < dt1.Rows.Count; i++)
                {

                    if (div != dt1.Rows[i]["PROJECT_NM"].ToString()
                        || div1 != dt1.Rows[i]["WORK_ITEM_CD"].ToString()
                        || div2 != dt1.Rows[i]["JOB_NM"].ToString())
                    {
                        row_idx++;
                        fpSpread1.Sheets[0].RowCount = row_idx + 1;

                        fpSpread1.Sheets[0].Cells[row_idx, 1].Text = dt1.Rows[i]["PROJECT_NM"].ToString();
                        fpSpread1.Sheets[0].Cells[row_idx, 2].Text = dt1.Rows[i]["WORK_ITEM_CD"].ToString();
                        fpSpread1.Sheets[0].Cells[row_idx, 3].Text = dt1.Rows[i]["JOB_NM"].ToString();

                        if (row_idx > 0)
                        {
                            if (tempDiv == div)
                            {
                                rowspan++;
                                if (tempDiv1 == div1)
                                {
                                    rowspan2++;
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].Cells[rowfirst2, 2].RowSpan = rowspan2;
                                    rowspan2 = 1;
                                    rowfirst2 = row_idx - 1;
                                    tempDiv1 = div1;
                                }
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[rowfirst, 1].RowSpan = rowspan;
                                rowspan = 1;
                                rowfirst = row_idx - 1;
                                tempDiv = div;

                                rowspan2 = 1;
                                rowfirst2 = row_idx - 1;
                                tempDiv1 = div1;
                            }
                        }

                        sumQty = 0;
                        sumTime = 0;
                        div = dt1.Rows[i]["PROJECT_NM"].ToString();
                        div1 = dt1.Rows[i]["WORK_ITEM_CD"].ToString();
                        div2 = dt1.Rows[i]["JOB_NM"].ToString();
                    }

                    for (int j = 4; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text == dt1.Rows[i]["PROJECT_SEQ"].ToString())
                        {
                            col_idx = j;
                            break;
                        }
                    }

                    if (dt1.Rows[i]["WORK_DONE_QTY"].ToString() != "" && Convert.ToDecimal(dt1.Rows[i]["WORK_DONE_QTY"]) > 0)
                    {
                        fpSpread1.Sheets[0].Cells[row_idx, col_idx].Value = dt1.Rows[i]["WORK_DONE_QTY"];
                        sumQty += Convert.ToDecimal(dt1.Rows[i]["WORK_DONE_QTY"]);
                    }

                    if (dt1.Rows[i]["WORK_HUMAN_TIME"].ToString() != "" && Convert.ToDecimal(dt1.Rows[i]["WORK_HUMAN_TIME"]) > 0)
                    {
                        fpSpread1.Sheets[0].Cells[row_idx, 1 + col_idx].Value = dt1.Rows[i]["WORK_HUMAN_TIME"];
                        sumTime += Convert.ToDecimal(dt1.Rows[i]["WORK_HUMAN_TIME"]);
                    }

                    if (sumQty != 0)
                        fpSpread1.Sheets[0].Cells[row_idx, fpSpread1.Sheets[0].ColumnCount - 2].Value = sumQty;
                    if (sumTime != 0)
                        fpSpread1.Sheets[0].Cells[row_idx, fpSpread1.Sheets[0].ColumnCount - 1].Value = sumTime;
                }

                fpSpread1.Sheets[0].Columns[3].BackColor = fpSpread1.Sheets[0].Columns[4].BackColor;
                fpSpread1.Sheets[0].Columns[1, 2].BackColor = fpSpread1.Sheets[0].Columns[3].BackColor;

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품코드")].Text.IndexOf("소계") != -1
                        || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text.IndexOf("소계") != -1
                        || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text.IndexOf("합계") != -1)
                    {
                        //소계 합계 색 변경
                        for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text == "합계")
                                fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                            else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text.IndexOf("소계") != -1)
                                fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                            else
                                fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor3;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
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
                    Excel.Application oAppln = null;
                    Excel.Workbook oWorkBook = null;
                    Excel.Worksheet oWorkSheet = null;
                    Excel.Range oRange = null;

                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    int iRow = 5;
                    int iRowSpan1 = 5;
                    int iRowSpan2 = 5;
                    int iColumn = fpSpread1.Sheets[0].Columns.Count;

                    Waiting_Form.Activate();
                    Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

                    oWorkSheet.Cells[1, 2] = "공장";
                    oWorkSheet.Cells[1, 3] = txtPlantNm.Text;
                    oWorkSheet.Cells[1, iColumn] = "인쇄일자";
                    oWorkSheet.Cells[1, iColumn + 1] = SystemBase.Base.ServerTime("YYMMDD");

                    oWorkSheet.Cells[2, 2] = "일자";
                    oWorkSheet.Cells[2, 3] = dtpWorkDtFr.Text + " ~ " + dtpWorkDtTo.Text;
                    oWorkSheet.Cells[2, iColumn + 1] = "(단위 : 시간)";

                    //헤드 색지정,테두리 설정
                    //1번째 헤드
                    oRange = oWorkSheet.get_Range("B3", trans(iColumn + 1) + 3);
                    oRange.Interior.Color = ColorTranslator.ToOle(Color.Beige);
                    oRange.NumberFormatLocal = "@";
                    oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    oRange.Borders.LineStyle = 1;
                    oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);

                    //2번째 헤드
                    oRange = oWorkSheet.get_Range("B4", trans(iColumn + 1) + 4);
                    oRange.Interior.Color = ColorTranslator.ToOle(Color.Beige);
                    oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    oRange.Borders.LineStyle = 1;
                    oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);

                    //headers 
                    for (int j = 0; j < fpSpread1.Sheets[0].Columns.Count; j++)
                    {
                        // header 저장
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text != "")
                            oWorkSheet.Cells[3, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text;

                        oWorkSheet.Cells[4, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j].Text;

                        // header Merge(세로)
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text == fpSpread1.Sheets[0].ColumnHeader.Cells[1, j].Text)
                        {
                            //oWorkSheet.Cells[4,j-1] = "";
                            oWorkSheet.get_Range("B3", "B4").Merge(false);
                            oWorkSheet.get_Range("C3", "C4").Merge(false);
                            oWorkSheet.get_Range("D3", "D4").Merge(false);
                            oWorkSheet.get_Range("E3", "E4").Merge(false);
                        }

                        //header Merge(가로)
                        if (j > 4)
                        {
                            if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text == "")
                            {
                                oWorkSheet.get_Range(trans(j + 1) + "3", trans(j + 2) + "3").Merge(false);
                            }
                        }

                    }

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
                    {
                        string strRow = trans(iColumn + 1) + iRow;
                        string strCRow = "C" + iRow;
                        string strDRow = "D" + iRow;
                        string strERow = "E" + iRow;

                        //내용 저장							
                        for (int colNo = 0; colNo < fpSpread1.Sheets[0].Columns.Count; colNo++)
                        {
                            oWorkSheet.Cells[iRow, colNo + 2] = fpSpread1.Sheets[0].Cells[rowNo, colNo].Text;

                            //품목명 소계 Merge
                            if (fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "부품코드")].Text == "" && fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text == "")
                            {
                                string iCRowSpan1 = "C" + iRowSpan1;
                                string iCRowSpan2 = "C" + (iRow - 1);

                                if (iRowSpan1 < iRow - 1)
                                {
                                    for (int i = iRowSpan1; i < iRow - 1; i++)
                                    {
                                        oWorkSheet.Cells[i + 1, 3] = "";
                                    }
                                    oWorkSheet.get_Range(iCRowSpan1, iCRowSpan2).Merge(false);
                                }

                                oWorkSheet.get_Range(strCRow, strERow).Merge(true);

                                //품목 소계 합계 색 변경
                                if (fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text == "합계")
                                    oWorkSheet.get_Range(strCRow, strRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor1);
                                else oWorkSheet.get_Range(strCRow, strRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor2);


                                oWorkSheet.get_Range(strCRow, strERow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                                iRowSpan1 = iRow + 1;
                                iRowSpan2 = iRow + 1;
                            }

                            //부품코드 소계 Merge
                            if (fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "부품코드")].Text != "" && fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text == "")
                            {
                                string iDRowSpan1 = "D" + iRowSpan2;
                                string iDRowSpan2 = "D" + (iRow - 1);

                                if (iRowSpan2 < iRow - 1)
                                {
                                    for (int i = iRowSpan2; i < iRow - 1; i++)
                                    {
                                        oWorkSheet.Cells[i + 1, 4] = "";
                                    }
                                    oWorkSheet.get_Range(iDRowSpan1, iDRowSpan2).Merge(false);
                                }

                                oWorkSheet.get_Range(strDRow, strERow).Merge(true);

                                //부품코드 소계 색 변경
                                oWorkSheet.get_Range(strDRow, strRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor3);
                                oWorkSheet.get_Range(strDRow, strERow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                                iRowSpan2 = iRow + 1;
                            }
                        }

                        //순번
                        oWorkSheet.Cells[iRow, 2] = rowNo + 1;
                        oWorkSheet.get_Range("B5", "B" + iRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        oWorkSheet.get_Range("B5", "B" + iRow).Interior.Color = ColorTranslator.ToOle(Color.Beige);

                        iRow++;

                        Waiting_Form.progressBar_temp.Value = rowNo + 1;
                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (rowNo + 1).ToString() + " Row를 저장하였습니다.";
                    }

                    string strColumn = trans(iColumn + 1) + (iRow - 1);


                    //내용 테두리 설정					
                    oRange = oWorkSheet.get_Range("B5", strColumn);
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

        //엑셀의 Cell 찾기
        private string trans(int c)
        {
            string ret = "";
            while (c > 0)
            {
                ret = (char)(('A' - 1) + c % 26) + ret;
                c /= 26;
            }
            return ret.Replace("A@", "Z");
        }

        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting();
            Waiting_Form.ShowDialog();
        }
        #endregion		

    }
}
