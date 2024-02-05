﻿#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목별직접노무비세부내역서
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-23
// 작성내용 : 품목별직접노무비세부내역서 및 관리
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

namespace CS.CSB018
{
    public partial class CSB018 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public CSB018()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void CSB018_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDtFr.Value = null;
            dtpDtTo.Value = null;	
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
        
        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strWorkItemYn = "";
                    if (rdoY.Checked == true) { strWorkItemYn = "Y"; }
                    else { strWorkItemYn = "N"; }

                    string strQuery = " usp_CSB018 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProject_Seq.Text + "'";
                    strQuery += ", @pWORK_ITEM_YN = '" + strWorkItemYn + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    // 2017.03.14. hma 추가(Start): 작업구분 검색조건 추가로 매개변수 추가함.
                    string strWC = "";
                    if (rdoAll.Checked == true) { strWC = ""; }
                    else if (rdoA.Checked == true) { strWC = "1"; }
                    else if (rdoB.Checked == true) { strWC = "2"; }
                    else if (rdoC.Checked == true) { strWC = "3"; }
                    strQuery += ", @pWC_TYPE = '" + strWC + "'";
                    // 2017.03.14. hma 추가(End)

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
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
            int rowspan1 = 0;
            int rowspan2 = 1;
            int rowspan3 = 1;

            int first_row_idx1 = 0;
            int first_row_idx2 = 0;
            string temp_item = "";
            string temp_proj_seq = "";
            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (i == 0)
                {
                    temp_item = fpSpread1.Sheets[0].Cells[0, 4].Text;
                    temp_proj_seq = fpSpread1.Sheets[0].Cells[0, 6].Text;
                    first_row_idx1 = i;
                    first_row_idx2 = i;
                }
                else
                {
                    if (temp_item == fpSpread1.Sheets[0].Cells[i, 4].Text)
                    {
                        rowspan2++;
                        if (temp_proj_seq == fpSpread1.Sheets[0].Cells[i, 6].Text)
                        {
                            rowspan3++;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[first_row_idx2, 6].RowSpan = rowspan3;
                            fpSpread1.Sheets[0].Cells[first_row_idx2, 6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            temp_proj_seq = fpSpread1.Sheets[0].Cells[i, 6].Text;
                            first_row_idx2 = i;
                            rowspan3 = 1;
                        }
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[first_row_idx1, 4].RowSpan = rowspan2;
                        fpSpread1.Sheets[0].Cells[first_row_idx1, 5].RowSpan = rowspan2;

                        fpSpread1.Sheets[0].Cells[first_row_idx1, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[first_row_idx1, 5].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;

                        temp_item = fpSpread1.Sheets[0].Cells[i, 4].Text;
                        first_row_idx1 = i;
                        rowspan2 = 1;

                        fpSpread1.Sheets[0].Cells[first_row_idx2, 6].RowSpan = rowspan3;
                        fpSpread1.Sheets[0].Cells[first_row_idx2, 6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        temp_proj_seq = fpSpread1.Sheets[0].Cells[i, 6].Text;
                        first_row_idx2 = i;
                        rowspan3 = 1;
                    }
                }

                if (fpSpread1.Sheets[0].Cells[i, 7].Text == "zzzz") //소계
                {
                    for (int j = 8; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                }
                else if (fpSpread1.Sheets[0].Cells[i, 5].Text == "소계") //소계
                {
                    for (int j = 4; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor3;
                    }
                }
                else if (fpSpread1.Sheets[0].Cells[i, 3].Text == "zzzzzzzz") //합계
                {
                    rowspan1 = i;
                    for (int k = 4; k < fpSpread1.Sheets[0].ColumnCount; k++)
                    {
                        fpSpread1.Sheets[0].Cells[i, k].BackColor = SystemBase.Base.gColor1;
                    }
                }
            }

            fpSpread1.Sheets[0].Cells[0, 2].RowSpan = rowspan1 + 1;
            fpSpread1.Sheets[0].Cells[0, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
        }
        #endregion

        #region 버튼 Click
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_C_COMMON 'C001', @pSPEC1 = '" + txtProject_No.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtItemCd.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 조회", false);
                pu.Width = 500;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtItemCd.Value = Msgs[0].ToString();
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
                    txtProject_No.Value = Msgs[3].ToString();
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

        //프로젝트차수
        private void btnProject_Seq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProject_Seq.Value = Msgs[0].ToString();
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
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProject_Nm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Excel
        protected override void ExcelExec()
        {
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

                    oWorkSheet.Cells[1, 1] = this.Text;
                    oWorkSheet.get_Range("A1", "P1").Merge(true);
                    oWorkSheet.get_Range("A1", "P1").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    oWorkSheet.get_Range("A1", "P1").Font.Size = 20;

                    oWorkSheet.Cells[2, 1] = "○프로젝트번호 : " + txtProject_No.Text;
                    oWorkSheet.Cells[2, 3] = "○프로젝트명 : " + txtProject_Nm.Text;
                    oWorkSheet.Cells[3, 1] = "○기 간 : " + dtpDtFr.Text + " ~ " + dtpDtTo.Text;
                    oWorkSheet.Cells[4, 1] = "○품목번호 : " + txtItemCd.Text;
                    oWorkSheet.Cells[4, 3] = "○품목명 : " + txtItemNm.Text;

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    // header 저장
                    int col = 1;

                    for (int HeadColCnt = 1; HeadColCnt < fpSpread1.Sheets[0].Columns.Count; HeadColCnt++)
                    {
                        if (HeadColCnt == 1 || HeadColCnt == 3 || HeadColCnt == 6) continue;

                        oWorkSheet.Cells[5, col] = fpSpread1.Sheets[0].ColumnHeader.Cells[0, HeadColCnt].Text;
                        col++;
                    }
                    int iRow = 6;
                    string temp_item = "";
                    int first_row_idx = 0;
                    //내용 저장
                    col = 1;
                    for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
                    {
                        col = 1;
                        for (int colNo = 1; colNo < fpSpread1.Sheets[0].Columns.Count; colNo++)
                        {
                            if (colNo == 1 || colNo == 3 || colNo == 6) continue;
                            oWorkSheet.Cells[iRow, col] = fpSpread1.Sheets[0].Cells[rowNo, colNo].Text;
                            col++;
                        }
                        if (rowNo == 0)
                        {
                            temp_item = fpSpread1.Sheets[0].Cells[rowNo, 4].Text;
                            first_row_idx = iRow;
                        }
                        else
                        {
                            if (temp_item != fpSpread1.Sheets[0].Cells[rowNo, 4].Text)
                            {
                                //Merge
                                oWorkSheet.Application.DisplayAlerts = false;

                                oRange = oWorkSheet.get_Range(oWorkSheet.Cells[first_row_idx, 2], oWorkSheet.Cells[iRow - 1, 2]);
                                oRange.Merge(Type.Missing);
                                oRange.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                                oRange = oWorkSheet.get_Range(oWorkSheet.Cells[first_row_idx, 3], oWorkSheet.Cells[iRow - 1, 3]);
                                oRange.Merge(Type.Missing);
                                oRange.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                                oWorkSheet.Application.DisplayAlerts = true;

                                first_row_idx = rowNo;
                            }
                        }
                        iRow++;
                        Waiting_Form.progressBar_temp.Value = rowNo + 1;
                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (rowNo + 1).ToString() + " Row를 저장하였습니다.";
                    }

                    //Merge
                    oWorkSheet.Application.DisplayAlerts = false;

                    oRange = oWorkSheet.get_Range(oWorkSheet.Cells[6, 1], oWorkSheet.Cells[iRow - 1, 1]);
                    oRange.Merge(Type.Missing);
                    oRange.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                    oWorkSheet.Application.DisplayAlerts = true;

                    //헤드 색지정,테두리 설정
                    oRange = oWorkSheet.get_Range("A5", "P5");
                    oRange.RowHeight = 30;
                    oRange.Interior.Color = ColorTranslator.ToOle(Color.LightGray);
                    oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //내용 테두리 설정		
                    string lastRow = "P" + Convert.ToString(iRow - 1);
                    oRange = oWorkSheet.get_Range("A5", lastRow);
                    oRange.Borders.LineStyle = 1;
                    oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);

                    oRange = oWorkSheet.get_Range("A6", lastRow);
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

        #region 폼 Activated & Deactivated
        private void CSB018_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProject_No.Focus();
        }

        private void CSB018_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
