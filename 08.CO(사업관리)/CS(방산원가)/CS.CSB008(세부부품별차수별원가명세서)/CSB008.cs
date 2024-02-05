#region 작성정보
/*********************************************************************/
// 단위업무명 : 세부부품별차수별원가명세서
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-12
// 작성내용 : 세부부품별차수별원가명세서 및 관리
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
using FarPoint.Win.Spread.CellType;
using System.Threading;
using System.IO;
using WNDW;

namespace CS.CSB008
{
    public partial class CSB008 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        bool form_act_chk = false;
        #endregion

        public CSB008()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void CSB008_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDtFr.Value = null;
            dtpDtTo.Value = null;

            rdoY.Checked = true;
            c1Label11.Text = "투입일자";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            dtpDtFr.Value = null;
            dtpDtTo.Value = null;

            rdoY.Checked = true;
            c1Label11.Text = "투입일자";
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
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

                    string strQuery = " usp_CSB008  ";
                    if (rdoY.Checked == true)
                        strQuery += " 'S3' ";
                    else
                        strQuery += " 'S4' ";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
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
                fpSpread1.Sheets[0].RowCount = 0;

                string strQuery = " usp_CSB008  ";
                if (rdoY.Checked == true)
                    strQuery += " 'S1' ";
                else
                    strQuery += " 'S2' ";

                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    fpSpread1.Sheets[0].AlternatingRows[0].BackColor = Color.FromArgb(230, 230, 230);
                    fpSpread1.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(245, 245, 245);

                    fpSpread1.Sheets[0].Columns[1].CellType = new TextCellType();
                    FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                    textCellType1.Multiline = true;
                    textCellType1.WordWrap = true;
                    fpSpread1.Sheets[0].Columns.Get(1).CellType = textCellType1;


                    string div = "", div1 = "";
                    string tempDiv = "";
                    int row_idx = 0;
                    int col_idx = 0;
                    decimal sum = 0;
                    decimal price = 0;
                    int rowspan = 1;
                    int rowfirst = 0;
                    fpSpread1.Sheets[0].ColumnCount = 4 + item_su;
                    fpSpread1.Sheets[0].ColumnHeader.Columns.Count = fpSpread1.Sheets[0].ColumnCount;
                    FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                    FarPoint.Win.Spread.CellType.NumberCellType num2 = new FarPoint.Win.Spread.CellType.NumberCellType();
                    FarPoint.Win.Spread.CellType.PercentCellType num1 = new FarPoint.Win.Spread.CellType.PercentCellType();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        if (row_idx == 0)
                        {

                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3 + col_idx].Text = dt.Rows[i]["PROJECT_SEQ"].ToString();

                            num.DecimalSeparator = ".";
                            num.DecimalPlaces = 0;
                            num.FixedPoint = true;
                            num.Separator = ",";
                            num.ShowSeparator = true;
                            num.MaximumValue = 99999999999999;
                            num.MinimumValue = -99999999999999;
                            fpSpread1.Sheets[0].Columns[3 + col_idx].CellType = num;
                            fpSpread1.Sheets[0].Columns[3 + col_idx].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            fpSpread1.Sheets[0].Columns[3 + col_idx].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            fpSpread1.Sheets[0].Columns[3 + col_idx].Locked = true;
                            //fpSpread1.Sheets[0].Columns[3 + col_idx].BackColor = Color.White;
                            fpSpread1.Sheets[0].Columns[3 + col_idx].Width = 80;

                        }


                        if (col_idx == 0)
                        {

                            fpSpread1.Sheets[0].RowCount = row_idx + 1;

                            fpSpread1.Sheets[0].Cells[row_idx, 1].Text = dt.Rows[i]["COST_CLASS_NM"].ToString();
                            if (dt.Rows[i]["COST_CLASS_NM"].ToString() == dt.Rows[i]["COST_ELEMENT_NM"].ToString())
                                fpSpread1.Sheets[0].Cells[row_idx, 1].ColumnSpan = 2;
                            else
                                fpSpread1.Sheets[0].Cells[row_idx, 2].Text = dt.Rows[i]["COST_ELEMENT_NM"].ToString();

                            div = dt.Rows[i]["COST_CLASS"].ToString();
                            div1 = dt.Rows[i]["COST_ELEMENT"].ToString();

                            //fpSpread1.Sheets[0].Cells[row_idx,3].BackColor = fpSpread1.Sheets[0].Cells[row_idx,4].BackColor;

                            if (div1 == "B03")
                            {
                                num2.DecimalSeparator = ".";
                                num2.DecimalPlaces = 2;
                                num2.FixedPoint = true;
                                num2.Separator = ",";
                                num2.ShowSeparator = true;
                                num2.MaximumValue = 99999999999999;
                                num2.MinimumValue = -99999999999999;
                                fpSpread1.Sheets[0].Cells[row_idx, 3 + col_idx].CellType = num2;
                            }

                            if (row_idx > 0)
                            {
                                if (tempDiv == div)
                                {
                                    rowspan++;
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].Cells[rowfirst, 1].RowSpan = rowspan;
                                    rowspan = 1;
                                    rowfirst = row_idx;
                                }
                            }
                        }

                        price = Convert.ToDecimal(dt.Rows[i]["COST_PRICE"]);
                        if (price != 0)
                        {
                            fpSpread1.Sheets[0].Cells[row_idx, 3 + col_idx].Value = dt.Rows[i]["COST_PRICE"];
                            sum += Convert.ToDecimal(dt.Rows[i]["COST_PRICE"]);
                        }

                        tempDiv = div;

                        if (((i + 1) % item_su) == 0)
                        {
                            if (div1 == dt.Rows[i]["COST_ELEMENT"].ToString())
                            {
                                int col = 3 + item_su;
                                if (row_idx == 0)
                                {
                                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, col].Text = "총 계";
                                    fpSpread1.Sheets[0].Columns[col].CellType = num;
                                    fpSpread1.Sheets[0].Columns[col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                                    fpSpread1.Sheets[0].Columns[col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                                    fpSpread1.Sheets[0].Columns[col].Locked = true;
                                    //fpSpread1.Sheets[0].Columns[col].BackColor = Color.White;
                                    fpSpread1.Sheets[0].Columns[col].Width = 80;
                                }
                                if (div1 == "B03") fpSpread1.Sheets[0].Cells[row_idx, col].CellType = num2;

                                if (sum == 0)
                                {
                                    fpSpread1.Sheets[0].Cells[row_idx, col].Value = null;
                                }
                                else
                                {
                                    if (fpSpread1.Sheets[0].Cells[row_idx, 1].Text == "단위당가격")
                                    {
                                        double TotAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[row_idx - 2, col].Value);
                                        double TotQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[row_idx - 1, col].Value);

                                        if (TotQty == 0)
                                        {
                                            fpSpread1.Sheets[0].Cells[row_idx, col].Value = 0;
                                        }
                                        else
                                        {
                                            fpSpread1.Sheets[0].Cells[row_idx, col].Value = TotAmt / TotQty;
                                        }
                                    }
                                    else
                                    {
                                        fpSpread1.Sheets[0].Cells[row_idx, col].Value = sum;
                                    }
                                }
                                col_idx = 0; row_idx++; sum = 0;
                            }

                        }
                        else col_idx++;

                    }

                    fpSpread1.Sheets[0].Columns[1, 2].BackColor = fpSpread1.Sheets[0].Columns[3].BackColor;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 버튼 Click
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_C_COMMON 'C001', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtItemCd.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 조회", false);
                pu.Width = 500;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtItemCd.Text = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // 프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }

        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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

                    oWorkSheet.Cells[1, 1] = this.Text;
                    oWorkSheet.get_Range("A1", lastCol + "1").Merge(true);
                    oWorkSheet.get_Range("A1", lastCol + "1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    oWorkSheet.get_Range("A1", lastCol + "1").Font.Size = 20;

                    oWorkSheet.Cells[2, 1] = "○프로젝트번호 : " + txtProject_No.Text;
                    oWorkSheet.Cells[2, 3] = "○프로젝트명 : " + txtProject_Nm.Text;
                    oWorkSheet.Cells[3, 1] = "○기 간 : " + dtpDtFr.Text + " ~ " + dtpDtTo.Text;
                    oWorkSheet.Cells[4, 1] = "○품목번호 : " + txtItemCd.Text;
                    oWorkSheet.Cells[4, 3] = "○품목명 : " + txtItemNm.Text;

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    // header 저장
                    int tit_row = 5;
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
                    oRange = oWorkSheet.get_Range("A" + tit_row.ToString(), lastCol + tit_row.ToString());
                    oRange.RowHeight = 30;
                    oRange.Interior.Color = ColorTranslator.ToOle(Color.LightGray);
                    oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //Merge
                    oWorkSheet.Application.DisplayAlerts = false;

                    oRange = oWorkSheet.get_Range("A" + tit_row.ToString(), "B" + tit_row.ToString());
                    oRange.Merge(Type.Missing);
                    oRange = oWorkSheet.get_Range(lastCol + tit_row.ToString(), lastCol + tit_row.ToString());
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
        private void CSB008_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProject_No.Focus();
        }

        private void CSB008_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region 라디오버튼 클릭
        private void rdoY_Click(object sender, System.EventArgs e)
        {
            c1Label11.Text = "투입일자";
        }

        private void rdonN_Click(object sender, System.EventArgs e)
        {
            c1Label11.Text = "창고입고일자";
        }
        #endregion
    }
}
