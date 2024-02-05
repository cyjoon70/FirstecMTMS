#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별계약/발주/실적직접비현황
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-22
// 작성내용 : 프로젝트별계약/발주/실적직접비현황
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

namespace CS.CSA002
{
    public partial class CSA002 : UIForm.FPCOMM1
    {
        #region 변수선언       
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public CSA002()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CSA002_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
           
            //기타 세팅		
            dtpDtFr.Text = SystemBase.Base.ServerTime("Y") + "-01-01";
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");	
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
          
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
                    string strQuery = "usp_CSA002 @pTYPE = 'S1'";
                    strQuery += ", @pINPUT_DT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pINPUT_DT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    Set_Section();
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

        #region 소계 합계 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;
            int iRow = 0;
            string strProjNo = "";
            bool blMinus = true;


            FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
            num.DecimalSeparator = ".";
            num.DecimalPlaces = 2;
            num.FixedPoint = true;
            num.Separator = ",";
            num.ShowSeparator = true;
            num.MaximumValue = 99999999999999;
            num.MinimumValue = -99999999999999;
            //fpSpread1.Sheets[0].Columns[4 + col_idx].CellType = num;


            //컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {
                //				if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증감")].Text != "")
                //				{
                //					if(Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증감")].Value) > 0)
                //					{
                //						blMinus = false;
                //					}
                //
                //				}
                //				if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증감_2")].Text != "")
                //				{
                //					if(Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증감_2")].Value) > 0)
                //					{
                //						blMinus = false;
                //					}
                //				}

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비목(요소별)")].Text.ToString() == "(외화금액)")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가계산금액")].CellType = num;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정금액")].CellType = num;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].CellType = num;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가절감")].CellType = num;
                }


                strProjNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                if (strProjNo == "합계")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].ColumnSpan = 3;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비목(요소별)"), i, SystemBase.Base.GridHeadIndex(GHIdx1, "총발주 & 실적")].BackColor = SystemBase.Base.gColor1;
                }

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비목(요소별)")].Text.ToString() == "직접비계")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비목(요소별)"), i, SystemBase.Base.GridHeadIndex(GHIdx1, "총발주 & 실적")].BackColor = SystemBase.Base.gColor2;
                }


                if (i + 1 != iCnt)
                {
                    if (strProjNo == fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text)
                    {
                        iRow = iRow + 1;
                    }
                    else
                    {

                        fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].RowSpan = iRow + 1;
                        fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].RowSpan = iRow + 1;
                        fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종납기")].RowSpan = iRow + 1;

                        //						if(blMinus == false)
                        //						{
                        //							fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].BackColor = Color.FromName("Red");
                        //							fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].ForeColor = Color.FromName("White");
                        //						}

                        iRow = 0;
                        blMinus = true;

                    }
                }
                else //마지막 Row
                {
                    fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].RowSpan = iRow + 1;
                    fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].RowSpan = iRow + 1;
                    fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종납기")].RowSpan = iRow + 1;

                    strProjNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;

                    //					if(blMinus == false)
                    //					{
                    //						fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].BackColor = Color.FromName("Red");
                    //						fpSpread1.Sheets[0].Cells[i - iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].ForeColor = Color.FromName("White");
                    //					}
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

                    int iRow = 4;
                    int iRowSpan = 4;

                    Waiting_Form.Activate();
                    Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

                    int iColumn = fpSpread1.Sheets[0].Columns.Count;


                    //조회조건 저장
                    oWorkSheet.Cells[1, 2] = "공장";
                    oWorkSheet.Cells[1, 3] = txtProjectNm.Text;
                    oWorkSheet.Cells[1, iColumn - 2] = "인쇄일자";
                    oWorkSheet.Cells[1, iColumn - 1] = SystemBase.Base.ServerTime("YYMMDD");

                    oWorkSheet.Cells[2, 2] = "일자";
                    oWorkSheet.Cells[2, 3] = dtpDtFr.Text + " ~ " + dtpDtTo.Text;
                    oWorkSheet.Cells[2, iColumn - 1] = "(단위 : 시간)";

                    //headers 
                    for (int j = 1; j < fpSpread1.Sheets[0].Columns.Count; j++)
                    {
                        // header 저장
                        oWorkSheet.Cells[3, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text;
                    }

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
                    {
                        string strQRow = "Q" + iRow;
                        string strCRow = "C" + iRow;
                        string strDRow = "D" + iRow;


                        //내용 저장							
                        for (int colNo = 1; colNo < fpSpread1.Sheets[0].Columns.Count; colNo++)
                        {
                            oWorkSheet.Cells[iRow, colNo + 2] = fpSpread1.Sheets[0].Cells[rowNo, colNo].Text;
                        }

                        //						if(fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text == "")
                        //						{	
                        //							string iRowSpan1 = "C" + iRowSpan;
                        //							string iRowSpan2 = "C" + (iRow-1);
                        //							if(iRowSpan < iRow-1)
                        //							{									
                        //								for(int i = iRowSpan; i < iRow-1; i++)
                        //								{
                        //									oWorkSheet.Cells[i+1, 3] = "";
                        //								}
                        //								oWorkSheet.get_Range(iRowSpan1, iRowSpan2).Merge(false);
                        //							}
                        //
                        //							oWorkSheet.get_Range(strCRow, strDRow).Merge(true);
                        //
                        //							//합계,소계 색 변경
                        //							if(fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text == "합계")
                        //								oWorkSheet.get_Range(strCRow, strQRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor1);
                        //							else 
                        //								oWorkSheet.get_Range(strCRow, strQRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor2); //소계 색변경
                        //							
                        //							oWorkSheet.get_Range(strCRow, strDRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        //						
                        //							iRowSpan = iRow + 1;
                        //							
                        //						}							

                        //순번
                        oWorkSheet.Cells[iRow, 2] = rowNo + 1;
                        oWorkSheet.get_Range("B3", "B" + iRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        oWorkSheet.get_Range("B3", "B" + iRow).Interior.Color = ColorTranslator.ToOle(Color.Beige);

                        iRow++;

                        Waiting_Form.progressBar_temp.Value = rowNo + 1;
                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (rowNo + 1).ToString() + " Row를 저장하였습니다.";
                    }


                    string strColumn = "S" + (iRow - 1);

                    //헤드 색지정,테두리 설정
                    oRange = oWorkSheet.get_Range("B3", "S3");
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
                    //SystemBase.Loggers.Log(this.Name, f.ToString());
                    //MessageBox.Show(SystemBase.Base.MessageRtn("B0050","Excel 출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
