#region 작성정보
/*********************************************************************/
// 단위업무명 : 재공품현황(생산현장/창고)
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-08-23
// 작성내용 : 재공품현황(생산현장/창고)
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using System.Threading;
using System.IO;
using WNDW;

namespace CS.CSB024
{
    public partial class CSB024 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        bool form_act_chk = false;
        #endregion

        public CSB024()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void CSB024_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString() ;
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString();
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    string strFlag = "";
                    if (rdoW.Checked == true)
                    {
                        strFlag = "W";
                    }
                    else if (rdoS.Checked == true)
                    {
                        strFlag = "S";
                    }

                    string strQuery = " usp_CSB024 'S2'";
                    strQuery += ", @pFLAG ='" + strFlag + "'";
                    strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    int val = Convert.ToInt32(dt.Rows[0][0]);

                    if (val > 0)
                        detail_SearchExec(val);
                    else
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        private void detail_SearchExec(int item_su)
        {
            try
            {
                string strFlag = "";
                if (rdoW.Checked == true)
                {
                    strFlag = "W";
                }
                else if (rdoS.Checked == true)
                {
                    strFlag = "S";
                }

                string strQuery = " usp_CSB024 'S1'  ";
                strQuery += ", @pFLAG ='" + strFlag + "'";
                strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].FrozenColumnCount = 5;

                    fpSpread1.Sheets[0].Columns[1, 4].CellType = new TextCellType();
                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                    textCellType1.Multiline = true;
                    textCellType1.WordWrap = true;
                    fpSpread1.Sheets[0].Columns.Get(1, 4).CellType = textCellType1;

                    fpSpread1.Sheets[0].Columns[1].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    fpSpread1.Sheets[0].Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;

                    string div = "", div1 = "";
                    string tempDiv = "";
                    int row_idx = 0;
                    int col_idx = 0;
                    decimal sum = 0;
                    decimal price = 0;
                    int colspan = 1;
                    int colfirst = 5;
                    fpSpread1.Sheets[0].ColumnCount = 5 + item_su;
                    fpSpread1.Sheets[0].ColumnHeader.Columns.Count = fpSpread1.Sheets[0].ColumnCount;
                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                    FarPoint.Win.Spread.CellType.NumberCellType num2 = new FarPoint.Win.Spread.CellType.NumberCellType();
                    FarPoint.Win.Spread.CellType.PercentCellType num1 = new FarPoint.Win.Spread.CellType.PercentCellType();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (row_idx == 0 && fpSpread1.Sheets[0].RowCount < 2)
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, 5 + col_idx].Text = dt.Rows[i]["COST_CLASS_NM"].ToString();
                            fpSpread1.Sheets[0].ColumnHeader.Cells[1, 5 + col_idx].Text = dt.Rows[i]["COST_ELEMENT_NM"].ToString();

                            if (dt.Rows[i]["COST_CLASS_NM"].ToString() == dt.Rows[i]["COST_ELEMENT_NM"].ToString())
                                fpSpread1.Sheets[0].ColumnHeader.Cells[0, 5 + col_idx].RowSpan = 2;
                            else
                                fpSpread1.Sheets[0].ColumnHeader.Cells[1, 5 + col_idx].Text = dt.Rows[i]["COST_ELEMENT_NM"].ToString();

                            div = dt.Rows[i]["COST_CLASS"].ToString();
                            div1 = dt.Rows[i]["COST_ELEMENT"].ToString();

                            if (col_idx > 0)
                            {
                                if (tempDiv == div)
                                {
                                    colspan++;
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, colfirst].ColumnSpan = colspan;
                                    colspan = 1;
                                    colfirst = 5 + col_idx;
                                }
                            }
                            else
                            {
                                tempDiv = div;
                            }

                            if (div1 == "B03")
                            {
                                num2.DecimalSeparator = ".";
                                num2.DecimalPlaces = 2;
                                num2.FixedPoint = true;
                                num2.Separator = ",";
                                num2.ShowSeparator = true;
                                num2.MaximumValue = 99999999999999;
                                num2.MinimumValue = -99999999999999;
                                fpSpread1.Sheets[0].Columns[5 + col_idx].CellType = num2;
                            }
                            else
                            {
                                num.DecimalSeparator = ".";
                                num.DecimalPlaces = 0;
                                num.FixedPoint = true;
                                num.Separator = ",";
                                num.ShowSeparator = true;
                                num.MaximumValue = 99999999999999;
                                num.MinimumValue = -99999999999999;
                                fpSpread1.Sheets[0].Columns[5 + col_idx].CellType = num;
                            }


                            fpSpread1.Sheets[0].Columns[5 + col_idx].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            fpSpread1.Sheets[0].Columns[5 + col_idx].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            fpSpread1.Sheets[0].Columns[5 + col_idx].Locked = true;
                            //fpSpread1.Sheets[0].Columns[3 + col_idx].BackColor = Color.White;
                            fpSpread1.Sheets[0].Columns[5 + col_idx].Width = 90;

                        }


                        if (col_idx == 0)
                        {

                            fpSpread1.Sheets[0].RowCount = row_idx + 1;

                            fpSpread1.Sheets[0].Cells[row_idx, 1].Text = dt.Rows[i]["PROJECT_NO"].ToString();
                            fpSpread1.Sheets[0].Cells[row_idx, 2].Text = dt.Rows[i]["PROJECT_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[row_idx, 3].Text = dt.Rows[i]["M_ITEM_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[row_idx, 4].Text = dt.Rows[i]["ITEM_NM"].ToString();

                        }

                        price = Convert.ToDecimal(dt.Rows[i]["COST_PRICE"]);
                        if (price != 0)
                        {
                            fpSpread1.Sheets[0].Cells[row_idx, 5 + col_idx].Value = dt.Rows[i]["COST_PRICE"];
                            sum += Convert.ToDecimal(dt.Rows[i]["COST_PRICE"]);
                        }

                        tempDiv = div;

                        if (((i + 1) % item_su) == 0)
                        {

                            col_idx = 0; row_idx++;

                        }
                        else col_idx++;
                    }
                    fpSpread1.Sheets[0].Columns[1, 4].BackColor = fpSpread1.Sheets[0].Columns[5].BackColor;

                    for (int row = 0; row < fpSpread1.Sheets[0].Rows.Count; row++)
                    {
                        double a = 0;

                        for (int col = 5; col < fpSpread1.Sheets[0].Columns.Count; col++)
                        {
                            if (fpSpread1.Sheets[0].Cells[row, col].Text == "" || fpSpread1.Sheets[0].Cells[row, col].Text == "0")
                            {
                                a = a;
                            }
                            else
                            {
                                a = a + Convert.ToDouble(fpSpread1.Sheets[0].Cells[row, col].Value);
                            }
                        }

                        if (a == 0)
                        {
                            //fpSpread1.Sheets[0].Rows[row].Visible = false;
                            fpSpread1.Sheets[0].Rows.Remove(row, 1);
                            row_idx--;
                        }
                    }

                    /********************** Summery 하단 고정합계 **************************************/
                    fpSpread1.Sheets[0].RowCount = row_idx + 1;

                    fpSpread1.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                    fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 1].Text = "";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 2].Text = "";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 3].Text = "";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, 4].Text = "";
                    fpSpread1.Sheets[0].Rows[fpSpread1.Sheets[0].Rows.Count - 1].BackColor = SystemBase.Base.gColor1;	//System.Drawing.Color.FromName("Beige");
                    fpSpread1.Sheets[0].Rows[fpSpread1.Sheets[0].Rows.Count - 1].Locked = true;

                    for (int j = 5; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        fpSpread1.Sheets[0].Cells.Get(fpSpread1.Sheets[0].Rows.Count - 1, j).Border = complexBorder1;

                        if (fpSpread1.Sheets[0].GetCellType(fpSpread1.Sheets[0].Rows.Count - 1, j).ToString() == "NumberCellType")
                        {
                            string Str = UIForm.FPMake.IntToString(j);
                            string Area = Str + "1:" + Str + Convert.ToString(fpSpread1.Sheets[0].Rows.Count - 1);
                            Cell r = fpSpread1.ActiveSheet.Cells[fpSpread1.Sheets[0].Rows.Count - 1, j];

                            r.Formula = "SUM(" + Area + ")";
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, j].CellType = new TextCellType();
                        }
                    }
                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
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
                Waiting_Form.Activate();
                Waiting_Form.label_temp.Text = "엑셀 데이타 준비중입니다.";

                Excel.Application oAppln;
                Excel.Workbook oWorkBook;
                Excel.Worksheet oWorkSheet;
                Excel.Range oRange;

                try
                {
                    Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    int col_cnt = fpSpread1.Sheets[0].ColumnCount - 1;
                    string lastCol;
                    if (col_cnt <= 26) lastCol = Convert.ToChar(col_cnt + 64).ToString();
                    else lastCol = Convert.ToChar(Convert.ToInt16(col_cnt / 26) + 64).ToString() + Convert.ToChar(col_cnt % 26 + 64).ToString();

                    int tit_row = 4;
                    int tit_row2 = 5;
                    oWorkSheet.Cells[1, 1] = this.Text;
                    oWorkSheet.get_Range("A1", lastCol + "1").Merge(true);
                    oWorkSheet.get_Range("A1", lastCol + "1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    oWorkSheet.get_Range("A1", lastCol + "1").Font.Size = 20;

                    oWorkSheet.Cells[3, 1] = "○기 간 : " + dtpDtFr.Text + " ~ " + dtpDtTo.Text;

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    // header 저장
                    int col = 1;

                    for (int HeadColCnt = 1; HeadColCnt < fpSpread1.Sheets[0].Columns.Count; HeadColCnt++)
                    {
                        for (int HeadRowCnt = 0; HeadRowCnt < fpSpread1.Sheets[0].ColumnHeaderRowCount; HeadRowCnt++)
                        {
                            oWorkSheet.Cells[tit_row + HeadRowCnt, col] = fpSpread1.Sheets[0].ColumnHeader.Cells[HeadRowCnt, HeadColCnt].Text;
                        }
                        col++;
                    }

                    int iRow = tit_row + 2;
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

                        if (fpSpread1.Sheets[0].Cells[rowNo, 2].Text == "")
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
                    oRange = oWorkSheet.get_Range("A" + tit_row.ToString(), lastCol + tit_row2.ToString());
                    //					oRange.RowHeight = 30;
                    oRange.Interior.Color = ColorTranslator.ToOle(Color.LightGray);
                    oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //Merge
                    oWorkSheet.Application.DisplayAlerts = false;

                    oRange = oWorkSheet.get_Range("A" + tit_row.ToString(), "B" + tit_row2.ToString());
                    oRange.Merge(Type.Missing);
                    oRange = oWorkSheet.get_Range(lastCol + tit_row.ToString(), lastCol + tit_row2.ToString());
                    oRange.Merge(Type.Missing);

                    oWorkSheet.Application.DisplayAlerts = true;

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

        #region Form Activated & Deactivate
        private void CSB024_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpDtFr.Focus();
        }

        private void CSB024_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
