#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별실적원가명세서
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-22
// 작성내용 : 프로젝트별실적원가명세서
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

namespace CS.CSB005
{
    public partial class CSB005 : UIForm.FPCOMM1
    {
        #region 변수선언       
        bool form_act_chk = false;
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public CSB005()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CSB005_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpDtFr.Value = null;
            dtpDtTo.Value = null;

            rdoProject.Checked = true;          // 2020.03.03. hma 추가: 기본적으로 프로젝트별(전체)로 조회하도록 함.
        }
        #endregion


        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpDtFr.Value = null;
            dtpDtTo.Value = null;
        }
        #endregion


        #region SearchExec()
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //조회조건 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    // 2020.03.03. hma 추가(Start): 조회구분 체크
                    string strViewType = "PRJ";
                    if (rdoPrjSeq.Checked == true)
                        strViewType = "SEQ";
                    else if (rdoPrjItem.Checked == true)
                        strViewType = "ITEM";
                    else if (rdoPrjSeqItem.Checked == true)
                        strViewType = "SEQITEM";
                    else
                        strViewType = "PRJ";
                    // 2020.03.03. hma 추가(End)

                    string strQuery = " usp_CSB005_2 ";     // 2022.07.06. hma 수정: usp_CSB005 => usp_CSB005_2로 변경
                    strQuery += " @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text + "'";
                    strQuery += ", @pPROJECT_SEQ ='" + txtProjectSeq.Text + "'";                  
                    strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";      // 2020.03.04. hma 추가
                    strQuery += ", @pVIEW_TYPE = '" + strViewType + "' ";       // 2020.03.04. hma 추가

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                    // 2020.03.04. hma 추가(Start): 조회구분에 따라 차수 및 제품코드 항목의 Visible 세팅
                    fpSpread1.Sheets[0].Columns[1].Visible = true;
                    fpSpread1.Sheets[0].Columns[2].Visible = true;
                    if (strViewType == "PRJ")
                    {
                        //fpSpread1.Sheets[0].Columns[0].Width = 0;
                        fpSpread1.Sheets[0].Columns[1].Visible = false;
                        fpSpread1.Sheets[0].Columns[2].Visible = false;
                    }
                    else if (strViewType == "ITEM")
                    {
                        fpSpread1.Sheets[0].Columns[1].Visible = false;
                    }
                    else if (strViewType == "SEQ")
                    {
                        fpSpread1.Sheets[0].Columns[2].Visible = false;
                    }
                    // 2020.03.04. hma 추가(End)

                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Span();
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


        private void Set_Span()
        {
            int rowspan = 1;
            int first_row_idx = 0;
            string temp_cost_item = "";
            // 2020.03.04. hma 추가(Start)
            int rowspan_seq = 1;
            int rowspan_item = 1;
            int first_row_idx_seq = 0;
            int first_row_idx_item = 0;
            string temp_seq = "";
            string temp_item_cd = "";
            // 2020.03.04. hma 추가(End)

            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
            num.DecimalSeparator = ".";
            num.DecimalPlaces = 2;
            num.FixedPoint = true;
            num.Separator = ",";
            num.ShowSeparator = true;
            num.MaximumValue = 99999999999999;
            num.MinimumValue = -99999999999999;

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (i == 0)
                {
                    temp_cost_item = fpSpread1.Sheets[0].Cells[0, 3].Text;              // 2020.03.04. hma 수정: [i, 1]=>[i, 3]로 변경
                    first_row_idx = i;

                    // 2020.03.04. hma 추가(Start)
                    temp_seq = fpSpread1.Sheets[0].Cells[0, 1].Text;
                    temp_item_cd = fpSpread1.Sheets[0].Cells[0, 2].Text;
                    first_row_idx_seq = i;
                    first_row_idx_item = i;
                    // 2020.03.04. hma 추가(End)
                }
                else
                {
                    if (temp_cost_item == fpSpread1.Sheets[0].Cells[i, 3].Text)          // 2020.03.04. hma 수정: [i, 1]=>[i, 3]로 변경
                    {
                        rowspan++;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[first_row_idx, 3].RowSpan = rowspan;      // 2020.03.04. hma 수정: [first_row_idx, 1]=>[first_row_idx, 3]로 변경
                        fpSpread1.Sheets[0].Cells[first_row_idx, 3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;   // 2020.03.04. hma 수정: [first_row_idx, 1]=>[first_row_idx, 3]로 변경

                        first_row_idx = i;
                        rowspan = 1;
                        temp_cost_item = fpSpread1.Sheets[0].Cells[i, 3].Text;           // 2020.03.04. hma 수정: [i, 1]=>[i, 3]로 변경
                    }

                    // 2020.03.04. hma 추가(Start)
                    if (temp_seq == fpSpread1.Sheets[0].Cells[i, 1].Text)
                    {
                        rowspan_seq++;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[first_row_idx_seq, 1].RowSpan = rowspan_seq;
                        fpSpread1.Sheets[0].Cells[first_row_idx_seq, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                        first_row_idx_seq = i;
                        rowspan_seq = 1;
                        temp_seq = fpSpread1.Sheets[0].Cells[i, 1].Text;
                    }
                    if (temp_item_cd == fpSpread1.Sheets[0].Cells[i, 2].Text)
                    {
                        rowspan_item++;
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[first_row_idx_item, 2].RowSpan = rowspan_item;
                        fpSpread1.Sheets[0].Cells[first_row_idx_item, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;

                        first_row_idx_item = i;
                        rowspan_item = 1;
                        temp_item_cd = fpSpread1.Sheets[0].Cells[i, 2].Text;
                    }
                    // 2020.03.04. hma 추가(End)
                }

                if (fpSpread1.Sheets[0].Cells[i, 3].Text == fpSpread1.Sheets[0].Cells[i, 4].Text)   // 2020.03.04. hma 수정: [i, 1]=>[i, 3], [i, 2]=>[i, 4] 로 변경
                {
                    //fpSpread1.Sheets[0].Cells[i, 1].ColumnSpan = 2;     // 2020.03.04. hma 수정: 프로젝트차수와 품목코드 항목 추가로 컬럼병합 컬럼수를 2=>4로 변경
                    fpSpread1.Sheets[0].Cells[i, 3].ColumnSpan = 2;
                }

                if (fpSpread1.Sheets[0].Cells[i, 4].Text == "직접공수(M/H)")    // 2020.03.04. hma 수정: 프로젝트차수와 품목코드 항목 추가로 체크 컬럼번호를 2=>4로 변경
                {
                    fpSpread1.Sheets[0].Cells[i, 5].CellType = num;     // 2020.03.04. hma 수정: 3=>5로 변경
                    fpSpread1.Sheets[0].Cells[i, 6].CellType = num;     // 2020.03.04. hma 수정: 4=>6로 변경
                    fpSpread1.Sheets[0].Cells[i, 7].CellType = num;     // 2020.03.04. hma 수정: 5=>7로 변경
                    fpSpread1.Sheets[0].Cells[i, 8].CellType = num;     // 2020.03.04. hma 수정: 6=>8로 변경
                    fpSpread1.Sheets[0].Cells[i, 9].CellType = num;     // 2020.03.04. hma 수정: 7=>9로 변경
                }
            }

            // 2020.03.04. hma 추가(Start)
            if (rdoPrjSeq.Checked == true || rdoPrjSeqItem.Checked == true)
            {
                fpSpread1.Sheets[0].Cells[first_row_idx_seq, 1].RowSpan = rowspan_seq;
                fpSpread1.Sheets[0].Cells[first_row_idx_seq, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            }

            if (rdoPrjItem.Checked == true || rdoPrjSeqItem.Checked == true)
            {
                fpSpread1.Sheets[0].Cells[first_row_idx_item, 2].RowSpan = rowspan_item;
                fpSpread1.Sheets[0].Cells[first_row_idx_item, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            }
            // 2020.03.04. hma 추가(End)
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트
        private void btnProject_Click(object sender, EventArgs e)
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        //프로젝트차수
        private void btnProjectSeq_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        // 2020.03.03. hma 추가(Start): 품목코드 버튼 클릭시 팝업창 뜨도록 처리
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
        // 2020.03.03. hma 추가(End)
        #endregion
        

        #region 조회조건 TextChanged
        private void txtProject_No_TextChanged(object sender, EventArgs e)
        {
            txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }

        // 2020.03.03. hma 추가(Start): 품목코드 변경시 품목명 변경되도록 함.
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        // 2020.03.03. hma 추가(End)
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

                    string lastCol = "G";
                    int tit_row = 5;
                    oWorkSheet.Cells[1, 1] = this.Text;
                    oWorkSheet.get_Range("A1", lastCol + "1").Merge(true);
                    oWorkSheet.get_Range("A1", lastCol + "1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    oWorkSheet.get_Range("A1", lastCol + "1").Font.Size = 20;

                    oWorkSheet.Cells[2, 1] = "○프로젝트번호 : " + txtProject_No.Text;
                    oWorkSheet.Cells[3, 1] = "○프로젝트   명 : " + txtProject_Nm.Text;
                    oWorkSheet.Cells[4, 1] = "○기         간 : " + dtpDtFr.Text + " ~ " + dtpDtTo.Text;

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    // header 저장
                    int col = 1;
                    for (int HeadRowCnt = 0; HeadRowCnt < fpSpread1.Sheets[0].ColumnHeader.RowCount; HeadRowCnt++)
                    {
                        for (int HeadColCnt = 1; HeadColCnt < fpSpread1.Sheets[0].Columns.Count; HeadColCnt++)
                        {
                            if (HeadRowCnt > 0 && fpSpread1.Sheets[0].ColumnHeader.Cells[HeadRowCnt - 1, HeadColCnt].Text
                                == fpSpread1.Sheets[0].ColumnHeader.Cells[HeadRowCnt, HeadColCnt].Text)
                            {
                                fpSpread1.Sheets[0].ColumnHeader.Cells[HeadRowCnt, HeadColCnt].Text = "";
                                oRange = oWorkSheet.get_Range(oWorkSheet.Cells[tit_row, HeadColCnt], oWorkSheet.Cells[tit_row - 1, HeadColCnt]);
                                oRange.Merge(Type.Missing);
                                oRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            }

                            oWorkSheet.Cells[tit_row, col] = fpSpread1.Sheets[0].ColumnHeader.Cells[HeadRowCnt, HeadColCnt].Text;

                            col++;
                        }
                        col = 1;
                        tit_row++;
                    }

                    int iRow = tit_row;
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
                    tit_row = 5;
                    for (int HeadRowCnt = 0; HeadRowCnt < fpSpread1.Sheets[0].ColumnHeader.RowCount; HeadRowCnt++)
                    {
                        oRange = oWorkSheet.get_Range("A" + tit_row.ToString(), lastCol + tit_row);
                        oRange.RowHeight = 30;
                        oRange.Borders.LineStyle = 1;
                        oRange.Interior.Color = ColorTranslator.ToOle(Color.LightGray);
                        oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        tit_row++;
                    }

                    oWorkSheet.Cells[5, 5] = "";
                    oWorkSheet.Cells[5, 6] = "";
                    oWorkSheet.get_Range("D5", "F5").Merge(true);

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

    }
}
