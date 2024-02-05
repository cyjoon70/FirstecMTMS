#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별재료비내역
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-23
// 작성내용 : 프로젝트별재료비내역 및 관리
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
using FarPoint.Win;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;

namespace CS.CSB022
{
    public partial class CSB022 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public CSB022()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void CSB022_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);		
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
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
                    string strQuery = " usp_CSB022 'S1'  ";
                    strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
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

        #region 폼 Activated & Deactivated
        private void CSB022_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpDtFr.Focus();
        }

        private void CSB022_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region 프로젝트번호 TextChanged
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

        #region 프로젝트번호 팝업
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
        #endregion
    }
}
