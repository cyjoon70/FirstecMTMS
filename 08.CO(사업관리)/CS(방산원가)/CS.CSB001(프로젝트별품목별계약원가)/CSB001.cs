#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별품목별계약원가
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-22
// 작성내용 : 프로젝트별품목별계약원가
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

namespace CS.CSB001
{
    public partial class CSB001 : UIForm.FPCOMM1
    {
        #region 변수선언       
        bool form_act_chk = false;
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public CSB001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CSB001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.ComboMake.C1Combo(cboContSeq, "usp_B_COMMON @pType='COMM', @pCODE = 'C003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);  

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;						
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_CSB001 'S2'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "' ";
                strQuery += ", @pCONT_SEQ = '" + cboContSeq.SelectedValue + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                int val = 0;

                if (dt.Rows.Count > 0)
                {
                    val = Convert.ToInt32(dt.Rows[0][0]);
                }

                if (val > 0)
                {
                    detail_SearchExec(val);
                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.Cursor = Cursors.Default;
            }
        }
        private void detail_SearchExec(int item_su)
        {

            try
            {
                string strQuery = " usp_CSB001 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "' ";
                strQuery += ", @pCONT_SEQ = '" + cboContSeq.SelectedValue + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                fpSpread1.Sheets[0].FrozenColumnCount = 3;
                fpSpread1.Sheets[0].ColumnHeader.Rows[1].Height = 45;

                fpSpread1.Sheets[0].Columns[1].CellType =  new TextCellType();
                FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
                textCellType1.Multiline = true;
                textCellType1.WordWrap = true;
                fpSpread1.Sheets[0].Columns.Get(1).CellType = textCellType1;

                //				fpSpread1.Sheets[0].AlternatingRows[0].BackColor = Color.FromArgb(230,230,230);
                //				fpSpread1.Sheets[0].AlternatingRows[1].BackColor = Color.FromArgb(245,245,245);

                string div = "", div1 = "";
                string tempDiv = "";
                int row_idx = 0;
                int col_idx = 0;
                decimal sum = 0;
                decimal rate = 0;
                decimal price = 0;
                decimal total_amt = 0;
                decimal total_price = 0;
                decimal total_cost_amt = 0;
                decimal total_con_amt = 0;

                int rowspan = 1;
                int rowfirst = 0;
                fpSpread1.Sheets[0].ColumnCount = 5 + item_su;
                fpSpread1.Sheets[0].ColumnHeader.Columns.Count = fpSpread1.Sheets[0].ColumnCount;
                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                FarPoint.Win.Spread.CellType.PercentCellType num1 = new FarPoint.Win.Spread.CellType.PercentCellType();
                FarPoint.Win.Spread.CellType.NumberCellType num2 = new FarPoint.Win.Spread.CellType.NumberCellType();
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (row_idx == 0)
                    {

                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4 + col_idx].Text = dt.Rows[i]["ITEM_CD"].ToString();
                        fpSpread1.Sheets[0].ColumnHeader.Cells[1, 4 + col_idx].Text = dt.Rows[i]["ITEM_NM"].ToString();

                        num.DecimalSeparator = ".";
                        num.DecimalPlaces = 0;
                        num.FixedPoint = true;
                        num.Separator = ",";
                        num.ShowSeparator = true;
                        num.MaximumValue = 99999999999999;
                        num.MinimumValue = -99999999999999;
                        fpSpread1.Sheets[0].Columns[4 + col_idx].CellType = num;
                        fpSpread1.Sheets[0].Columns[4 + col_idx].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                        fpSpread1.Sheets[0].Columns[4 + col_idx].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                        fpSpread1.Sheets[0].Columns[4 + col_idx].Locked = true;
                        fpSpread1.Sheets[0].Columns[4 + col_idx].BackColor = Color.White;
                        fpSpread1.Sheets[0].Columns[4 + col_idx].Width = 80;

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

                        if (dt.Rows[i]["COMMON_RATE"].ToString() != "")
                        {
                            rate = Convert.ToDecimal(dt.Rows[i]["COMMON_RATE"]);
                            if (rate != 0)
                            {
                                if (div1 == "B50" || div1 == "C90" || div1 == "E") //간접노무비, 간접경비, 일반관리비 %표시
                                {
                                    num1.DecimalSeparator = ".";
                                    num1.DecimalPlaces = 2;
                                    num1.FixedPoint = true;
                                    num1.Separator = ",";
                                    num1.ShowSeparator = true;
                                    fpSpread1.Sheets[0].Cells[row_idx, 3].CellType = num1;
                                    fpSpread1.Sheets[0].Cells[row_idx, 3].Value = Convert.ToDecimal(dt.Rows[i]["COMMON_RATE"]);
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].Cells[row_idx, 3].Value = dt.Rows[i]["COMMON_RATE"];
                                }
                            }
                        }
                        fpSpread1.Sheets[0].Cells[row_idx, 3].BackColor = fpSpread1.Sheets[0].Cells[row_idx, 4].BackColor;

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
                        if (div1 == "I02" || div1 == "Z2") //(%)
                        {
                            num1.DecimalSeparator = ".";
                            num1.DecimalPlaces = 2;
                            num1.FixedPoint = true;
                            num1.Separator = ",";
                            num1.ShowSeparator = true;
                            //							num1.MaximumValue = 99999999999999;
                            //							num1.MinimumValue = -99999999999999;
                            fpSpread1.Sheets[0].Cells[row_idx, 4 + col_idx].CellType = num1;
                            fpSpread1.Sheets[0].Cells[row_idx, 4 + col_idx].Value = Convert.ToDecimal(dt.Rows[i]["COST_PRICE"]);
                        }
                        else if (div1 == "B03") //직접공수 소수점자리
                        {
                            num2.DecimalSeparator = ".";
                            num2.DecimalPlaces = 2;
                            num2.FixedPoint = true;
                            num2.Separator = ",";
                            num2.ShowSeparator = true;
                            num2.MaximumValue = 99999999999999;
                            num2.MinimumValue = -99999999999999;
                            fpSpread1.Sheets[0].Cells[row_idx, 4 + col_idx].CellType = num2;
                            fpSpread1.Sheets[0].Cells[row_idx, 4 + col_idx].Value = dt.Rows[i]["COST_PRICE"];
                        }
                        else
                            fpSpread1.Sheets[0].Cells[row_idx, 4 + col_idx].Value = dt.Rows[i]["COST_PRICE"];
                        sum += Convert.ToDecimal(dt.Rows[i]["AMT"]);
                    }

                    tempDiv = div;

                    if (((i + 1) % item_su) == 0)
                    {
                        int col = 4 + item_su;
                        if (row_idx == 0)
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, col].Text = "합 계";
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, col].RowSpan = 2;
                            fpSpread1.Sheets[0].Columns[col].CellType = num;
                            fpSpread1.Sheets[0].Columns[col].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                            fpSpread1.Sheets[0].Columns[col].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                            fpSpread1.Sheets[0].Columns[col].Locked = true;
                            fpSpread1.Sheets[0].Columns[col].BackColor = Color.White;
                            fpSpread1.Sheets[0].Columns[col].Width = 80;
                        }

                        if (div1 == "B03")
                        {
                            fpSpread1.Sheets[0].Cells[row_idx, col].CellType = num2;
                            fpSpread1.Sheets[0].Cells[row_idx, col].Value = sum;
                        }
                        else if (div1 == "I02") //(%)
                        {
                            fpSpread1.Sheets[0].Cells[row_idx, col].CellType = num1;
                            fpSpread1.Sheets[0].Cells[row_idx, col].Value = (total_amt / total_price);
                        }
                        else if (div1 == "Z2") //(예가율%)
                        {
                            fpSpread1.Sheets[0].Cells[row_idx, col].CellType = num1;
                            fpSpread1.Sheets[0].Cells[row_idx, col].Value = (total_con_amt / total_cost_amt);
                        }
                        else
                        {	// 공수(M/H) ,산학연용역,해외출장비, 연구개발비 -- 합계 표시 안함
                            if (sum != 0 || (div1 != "B02" || div1 != "C02" || div1 != "C04" || div1 != "C05"))
                                fpSpread1.Sheets[0].Cells[row_idx, col].Value = sum;
                        }

                        if (div1 == "F") total_price = sum; //총원가
                        else if (div1 == "I01") total_amt = sum; //산정(금액)
                        else if (div1 == "Z") total_cost_amt = sum; //원가계산금액
                        else if (div1 == "Z1") total_con_amt = sum; //계약금액

                        col_idx = 0; row_idx++; sum = 0;

                    }
                    else col_idx++;

                }
                //계약금액 추가 2010.04.29
                detail_SearchExec2(num);

                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    EventArgs ev = new EventArgs();
                    cboContSeq_SelectedValueChanged(cboContSeq, ev);
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void detail_SearchExec2(FarPoint.Win.Spread.CellType.NumberCellType numType)
        {
            FarPoint.Win.Spread.CellType.PercentCellType num1 = new FarPoint.Win.Spread.CellType.PercentCellType();

            decimal total_cost_amt = 0;
            decimal total_con_amt = 0;

            num1.DecimalSeparator = ".";
            num1.DecimalPlaces = 2;
            num1.FixedPoint = true;
            num1.Separator = ",";
            num1.ShowSeparator = true;


            fpSpread1.Sheets[0].ColumnCount++;
            int c_idx = fpSpread1.Sheets[0].ColumnCount - 1;

            fpSpread1.Sheets[0].ColumnHeader.Cells[0, c_idx].Text = "총합계";
            fpSpread1.Sheets[0].ColumnHeader.Cells[0, c_idx].RowSpan = 2;
            fpSpread1.Sheets[0].Columns[c_idx].CellType = numType;
            fpSpread1.Sheets[0].Columns[c_idx].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            fpSpread1.Sheets[0].Columns[c_idx].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            fpSpread1.Sheets[0].Columns[c_idx].Locked = true;
            fpSpread1.Sheets[0].Columns[c_idx].BackColor = Color.White;
            fpSpread1.Sheets[0].Columns[c_idx].Width = 110;

            string strQuery = " usp_CSB001 'S3'";
            strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
            strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "' ";
            strQuery += ", @pCONT_SEQ = '" + cboContSeq.SelectedValue + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                
                fpSpread1.Sheets[0].Cells[i, c_idx].Value = dt.Rows[i]["AMT"];

                if (dt.Rows[i]["COST_ELEMENT"].ToString() == "I02" || dt.Rows[i]["COST_ELEMENT"].ToString() == "Z2")
                {
                    fpSpread1.Sheets[0].Cells[i, c_idx].CellType = num1;
                    fpSpread1.Sheets[0].Cells[i, c_idx].Value = fpSpread1.Sheets[0].Cells[i, c_idx - 1].Value;
                }

                if (dt.Rows[i]["COST_CLASS"].ToString() == "Z")
                {
                    total_cost_amt = Convert.ToDecimal(dt.Rows[i]["AMT"]);      //원가계산금액
                }
                else if (dt.Rows[i]["COST_CLASS"].ToString() == "Z1")
                {
                    total_con_amt = Convert.ToDecimal(dt.Rows[i]["AMT"]); //계약금액
                }

                if (i == dt.Rows.Count - 1)
                {

                    fpSpread1.Sheets[0].Cells[i, c_idx].CellType = num1;
                    fpSpread1.Sheets[0].Cells[i, c_idx].Value = (total_con_amt / total_cost_amt);
                }

            }
        }
        #endregion

        #region 조회조건 팝업
        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {

                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
            
            if (txtProjectNm.Text != "")
            {
                string strQuery = " usp_CSB001 'C1' ";
                strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    cboContSeq.SelectedValue = dt.Rows[0][0].ToString();
                    if (dt.Rows[0][1].ToString() != "")
                        dtpCont_App_Dt.Value = dt.Rows[0][1].ToString();
                    else
                        dtpCont_App_Dt.Value = null;
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

                    oWorkSheet.Cells[2, 1] = "○프로젝트번호 : " + txtProjectNo.Text;
                    oWorkSheet.Cells[2, 3] = "○프로젝트명 : " + txtProjectNm.Text;

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
                catch   //(Exception ex)
                {
                    th.Abort();
                    //										SystemBase.Loggers.Log(this.Name, ex.ToString());
                    //										DialogResult dsMsg = MessageBox.Show(ex.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); 			 
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

        #region Form Activated & Deactivated
        private void CSB001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProjectNo.Focus();
        }

        private void CSB001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region cboContSeq_SelectedValueChanged
        private void cboContSeq_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_CSB001 'C2' ";
                strQuery += ", @pPROJECT_NO='" + txtProjectNo.Text.Trim() + "' ";
                strQuery += ", @pCONT_SEQ='" + cboContSeq.SelectedValue.ToString() + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() != "")
                        dtpCont_App_Dt.Value = dt.Rows[0][0].ToString();
                    else
                        dtpCont_App_Dt.Value = null;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }	
        }
        #endregion



    }
}
