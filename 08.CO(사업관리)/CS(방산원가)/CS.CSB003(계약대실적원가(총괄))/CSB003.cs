#region 작성정보
/*********************************************************************/
// 단위업무명 : 계약대실적원가(총괄)
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-22
// 작성내용 : 계약대실적원가(총괄)
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

namespace CS.CSB003
{
    public partial class CSB003 : UIForm.FPCOMM1
    {
        #region 변수선언       
        bool form_act_chk = false;
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public CSB003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CSB003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장

            cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;

            dtpDtFr.Text = SystemBase.Base.ServerTime("Y")+ "-01-01";
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅		
            dtpDtFr.Text = SystemBase.Base.ServerTime("Y") + "-01-01";
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
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
                    string strQuery = " usp_CSB003 ";

                    if (rdoY.Checked == true)
                        strQuery += " 'S1' ";
                    else
                        strQuery += " 'S2' ";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue + "'";
                    strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Span();

                    fpSpread1.Sheets[0].RowCount = fpSpread1.Sheets[0].RowCount + 1;
                    fpSpread1.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 1].Text = "※ 실적금액의 수입제세는 수입재료비/부품비에 포함 계산";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 1].ColumnSpan = fpSpread1.Sheets[0].ColumnCount - 1;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
                this.Cursor = Cursors.Default;
            }
        }
        private void Set_Span()
        {
            int rowspan = 1;
            int first_row_idx = 0;
            string temp_item = "";

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (i == 0)
                {
                    temp_item = fpSpread1.Sheets[0].Cells[0, 1].Text;
                    first_row_idx = i;
                }
                else
                {
                    if (temp_item == fpSpread1.Sheets[0].Cells[i, 1].Text)
                    {
                        rowspan++;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[first_row_idx, 1].RowSpan = rowspan;
                        fpSpread1.Sheets[0].Cells[first_row_idx, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                        first_row_idx = i;
                        rowspan = 1;
                        temp_item = fpSpread1.Sheets[0].Cells[i, 1].Text;
                    }
                }

                if (fpSpread1.Sheets[0].Cells[i, 1].Text == fpSpread1.Sheets[0].Cells[i, 2].Text)
                {
                    fpSpread1.Sheets[0].Cells[i, 1].ColumnSpan = 2;
                }

            }

        }
        #endregion

        #region Excel
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
                Waiting_Form.label_temp.Text = "엑셀 데이타 준비중입니다.";

                Excel.Application oAppln;
                Excel.Workbook oWorkBook;
                Excel.Worksheet oWorkSheet;
                Excel.Range oRange;

                try
                {
                    Waiting_Form.Activate();
                    Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    string lastCol = "E";
                    int tit_row = 4;
                    oWorkSheet.Cells[1, 1] = this.Text;
                    oWorkSheet.get_Range("A1", lastCol + "1").Merge(true);
                    oWorkSheet.get_Range("A1", lastCol + "1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    oWorkSheet.get_Range("A1", lastCol + "1").Font.Size = 20;

                    oWorkSheet.Cells[2, 1] = "○공 장 : " + cboPlantCd.Text;
                    oWorkSheet.Cells[3, 1] = "○기 간 : " + dtpDtFr.Text + " ~ " + dtpDtTo.Text;

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    // header 저장
                    int col = 1;

                    for (int HeadColCnt = 1; HeadColCnt < fpSpread1.Sheets[0].Columns.Count; HeadColCnt++)
                    {
                        oWorkSheet.Cells[tit_row, col] = fpSpread1.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text;
                        col++;
                    }

                    int iRow = tit_row + 1;
                    string temp_item = "";
                    int first_row_idx = 1;
                    //내용 저장
                    col = 1;
                    for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
                    {
                        col = 1;
                        for (int colNo = 1; colNo < fpSpread1.Sheets[0].Columns.Count; colNo++)
                        {
                            oWorkSheet.Cells[iRow, col] = fpSpread1.Sheets[0].Cells[rowNo, colNo].Text;
                            col++;
                        }
                        if (rowNo == 0)
                        {
                            temp_item = fpSpread1.Sheets[0].Cells[rowNo, 1].Text;
                            first_row_idx = iRow;
                        }
                        else
                        {
                            if (temp_item != fpSpread1.Sheets[0].Cells[rowNo, 1].Text)
                            {
                                //Merge
                                oWorkSheet.Application.DisplayAlerts = false;

                                oRange = oWorkSheet.get_Range(oWorkSheet.Cells[first_row_idx, 1], oWorkSheet.Cells[iRow - 1, 1]);
                                oRange.Merge(Type.Missing);
                                oRange.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                                oWorkSheet.Application.DisplayAlerts = true;
                                temp_item = fpSpread1.Sheets[0].Cells[rowNo, 1].Text;
                                first_row_idx = iRow;
                            }
                        }

                        if (fpSpread1.Sheets[0].Cells[rowNo, 1].Text == fpSpread1.Sheets[0].Cells[rowNo, 2].Text)
                        {
                            //Merge
                            oWorkSheet.Application.DisplayAlerts = false;

                            oRange = oWorkSheet.get_Range(oWorkSheet.Cells[iRow, 1], oWorkSheet.Cells[iRow, 2]);
                            oRange.Merge(Type.Missing);
                            oWorkSheet.Application.DisplayAlerts = true;
                        }

                        iRow++;
                        Waiting_Form.progressBar_temp.Value = rowNo + 1;
                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (rowNo + 1).ToString() + " Row를 저장하였습니다.";
                    }


                    //헤드 색지정,테두리 설정
                    oRange = oWorkSheet.get_Range("A" + tit_row.ToString(), lastCol + tit_row);
                    oRange.RowHeight = 30;
                    oRange.Interior.Color = ColorTranslator.ToOle(Color.LightGray);
                    oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //내용 테두리 설정		
                    string lastRow = lastCol + Convert.ToString(iRow - 1);
                    oRange = oWorkSheet.get_Range("A" + tit_row.ToString(), lastRow);
                    oRange.Borders.LineStyle = 1;
                    oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);

                    int data_first_row = tit_row + 1;
                    oRange = oWorkSheet.get_Range("A" + data_first_row.ToString(), lastRow);
                    oRange.RowHeight = 15;

                    Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";
                    //range of the excel sheet
                    oRange = oWorkSheet.get_Range("A1", "IV1");
                    oRange.EntireColumn.AutoFit();
                    oAppln.UserControl = false;

                    oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

                    // 엑셀 파일로 저장
                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);

                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                }
                catch
                {
                    th.Abort();
                    //					SystemBase.Loggers.Log(this.Name, ex.ToString());
                    //					DialogResult dsMsg = MessageBox.Show(ex.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

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

        private void CSB003_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void CSB003_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

    }
}
