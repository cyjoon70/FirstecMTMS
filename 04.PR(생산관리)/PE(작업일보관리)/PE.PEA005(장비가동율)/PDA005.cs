#region 작성정보
/*********************************************************************/
// 단위업무명 : 장비가동율
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 장비가동율
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
using System.Reflection;

namespace PE.PEA005
{
    public partial class PEA005 : UIForm.FPCOMM1
    {
        #region 변수선언
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;

        UIForm.ExcelWaiting Waiting_Form = null;
		Thread th;
        #endregion

        public PEA005()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PEA005_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크
           
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            radioButton1.Checked = true;
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            radioButton1.Checked = true;         
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = "";
                    strQuery = "   usp_PEA005 @pTYPE = 'S1'";
                    strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "' ";
                    strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    { SystemBase.Validation.GroupBoxControlsLock(groupBox2, false); Set_Section(); }
                    else
                    { SystemBase.Validation.GroupBoxControlsLock(groupBox2, true); }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        #region Set_Section() 2012-03-27수정전
        //		private void Set_Section()
        //		{
        //			int iCnt = fpSpread1.Sheets[0].RowCount;
        //			int iRow = 0;		
        //			int sum_a = 0, sum_b =0, sum_c = 0;  
        //			int total_a = 0, total_b =0, total_c = 0;  
        //			int tmp = 0;
        //			int tmp1= 0, tmp2= 0, tmp3= 0;
        //			int idx1 = SystemBase.Base.GridHeadIndex(GHIdx1, "NC구분");
        //			//소계, 합계 컬럼 합치고 색 변경
        //			for(int i = 0; i < iCnt ; i++)
        //			{
        //				if(fpSpread1.Sheets[0].Cells[i,idx1].Text == "A") 
        //					sum_a += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i,9].Value);
        //				else if(fpSpread1.Sheets[0].Cells[i,idx1].Text == "B") 
        //					sum_b += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i,9].Value);
        //				else if(fpSpread1.Sheets[0].Cells[i,idx1].Text == "C") 
        //					sum_c += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i,9].Value);
        //
        //				if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "장비번호")].Text == "")
        //				{				
        //					fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "NC구분")].ColumnSpan = 2 ;
        //					
        //					for(int j=1;j < fpSpread1.Sheets[0].ColumnCount ; j++)
        //					{	
        //						if(fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].Text == "")
        //						{
        //							fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "NC구분")].ColumnSpan = 2 ;
        //
        //							fpSpread1.Sheets[0].Cells[i,j].BackColor = SystemBase.Base.gColor2;
        //							
        //							fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].RowSpan = (i+1) - iRow;
        //							iRow = i + 1;
        //							
        //						}
        //						else if(fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].Text == "합계")
        //						{
        //							fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].ColumnSpan = 3 ;
        //
        //							fpSpread1.Sheets[0].Cells[i,j].BackColor = SystemBase.Base.gColor1;							
        //						}
        //						else 
        //						{
        //							fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "NC구분")].ColumnSpan = 2 ;
        //
        //							fpSpread1.Sheets[0].Cells[i,j].BackColor = SystemBase.Base.gColor3;
        //						}
        //					}	
        //					tmp1= 0; tmp2= 0; tmp3= 0;
        //					if(fpSpread1.Sheets[0].Cells[i,5].Text != "") tmp1 = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i,5].Value);
        //					if(fpSpread1.Sheets[0].Cells[i,6].Text != "") tmp2 = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i,6].Value); 
        //					if(fpSpread1.Sheets[0].Cells[i,7].Text != "") tmp3 = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i,7].Value);
        //
        //					if(fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].Text == "")
        //					{
        //						tmp  = sum_a + sum_b + sum_c;
        //						fpSpread1.Sheets[0].Cells[i,8].Value = tmp - tmp1 - tmp2 - tmp3;
        //						fpSpread1.Sheets[0].Cells[i,9].Value = tmp ;
        //						if (tmp1 + tmp2 == 0)
        //						{
        //							fpSpread1.Sheets[0].Cells[i,10].Value = 0;
        //						}
        //						else
        //						{
        //							fpSpread1.Sheets[0].Cells[i,10].Value = ( tmp1 + tmp2)/Convert.ToDecimal(tmp) * 100 ;
        //						}
        //						sum_a = 0; sum_b =0; sum_c = 0;  	
        //					}
        //					else if(fpSpread1.Sheets[0].Cells[i,SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].Text == "합계")
        //					{
        //						tmp  = total_a + total_b + total_c;
        //						fpSpread1.Sheets[0].Cells[i,8].Value = tmp - tmp1 - tmp2 - tmp3;
        //						fpSpread1.Sheets[0].Cells[i,9].Value = tmp ;
        //						if (tmp1 + tmp2 == 0)
        //						{
        //							fpSpread1.Sheets[0].Cells[i,10].Value = 0;
        //						}
        //						else
        //						{
        //							fpSpread1.Sheets[0].Cells[i,10].Value = ( tmp1 + tmp2)/Convert.ToDecimal(tmp) * 100 ;
        //						}				
        //					}
        //					else 
        //					{
        //						if(fpSpread1.Sheets[0].Cells[i,idx1].Text == "NC소계")
        //						{
        //							tmp  = sum_a;
        //							fpSpread1.Sheets[0].Cells[i,8].Value = tmp - tmp1 - tmp2 - tmp3;
        //							fpSpread1.Sheets[0].Cells[i,9].Value = tmp ;
        //							if (tmp1 + tmp2 == 0)
        //							{
        //								fpSpread1.Sheets[0].Cells[i,10].Value = 0;
        //							}
        //							else
        //							{
        //								fpSpread1.Sheets[0].Cells[i,10].Value = ( tmp1 + tmp2)/Convert.ToDecimal(tmp) * 100 ;
        //							}		
        //						}
        //						else if(fpSpread1.Sheets[0].Cells[i,idx1].Text == "범용소계")
        //						{
        //							tmp  = sum_b;
        //							fpSpread1.Sheets[0].Cells[i,8].Value = tmp - tmp1 - tmp2 - tmp3;
        //							fpSpread1.Sheets[0].Cells[i,9].Value = tmp ;
        //							if (tmp1 + tmp2 == 0)
        //							{
        //								fpSpread1.Sheets[0].Cells[i,10].Value = 0;
        //							}
        //							else
        //							{
        //								fpSpread1.Sheets[0].Cells[i,10].Value = ( tmp1 + tmp2)/Convert.ToDecimal(tmp) * 100 ;
        //							}
        //						}
        //						else if(fpSpread1.Sheets[0].Cells[i,idx1].Text == "기타소계")
        //						{
        //							tmp  = sum_c;
        //							fpSpread1.Sheets[0].Cells[i,8].Value = tmp - tmp1 - tmp2 - tmp3;
        //							fpSpread1.Sheets[0].Cells[i,9].Value = tmp ;
        //							if (tmp1 + tmp2 == 0)
        //							{
        //								fpSpread1.Sheets[0].Cells[i,10].Value = 0;
        //							}
        //							else
        //							{
        //								fpSpread1.Sheets[0].Cells[i,10].Value = ( tmp1 + tmp2)/Convert.ToDecimal(tmp) * 100 ;
        //							}	
        //						}
        //					}
        //
        //				}
        //				else
        //				{
        //					total_a += sum_a; total_b +=sum_b; total_c += sum_c;  	
        //												
        //				}
        //			}		
        //		}
        #endregion
        #endregion

        #region 소계 합계 그리드 재정의
        private void Set_Section()
        {
            fpSpread1.Sheets[0].Columns[2, 4].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].Text == "합계")
                {
                    fpSpread1.Sheets[0].Cells[i, 2, i, 4].ColumnSpan = 3;
                    fpSpread1.Sheets[0].Cells[i, 2].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Cells[i, 2, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = SystemBase.Base.gColor1;
                }
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "장비번호")].Text == "범용소계"
                    || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "장비번호")].Text == "NC소계")
                {
                    fpSpread1.Sheets[0].Cells[i, 3, i, 4].ColumnSpan = 2;
                    fpSpread1.Sheets[0].Cells[i, 3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Cells[i, 3, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = SystemBase.Base.gColor3;
                }
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "장비번호")].Text
                    == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소속")].Text + " 소계")
                {
                    fpSpread1.Sheets[0].Cells[i, 3, i, 4].ColumnSpan = 2;
                    fpSpread1.Sheets[0].Cells[i, 3].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Cells[i, 3, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = SystemBase.Base.gColor2;
                }
            }
        }
        #endregion

        #region 출력
        private void btnPrivew_Click(object sender, EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count <= 0)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0053"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Excel 다운로드 위치 지정";
            dlg.InitialDirectory = dlg.FileName;
            dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            string strFileName = "";
            int iDiv = 0;			// 0:장비가동현황, 1:장비가동율, 2:장비고장율, 3:장비별준비시간/가공시간, 4:NC 장비가동율, 5:범용장비 가동율, 6:반별 장비가동율
            if (radioButton1.Checked == true) { strFileName = radioButton1.Text; iDiv = 0; }
            else if (radioButton2.Checked == true) { strFileName = radioButton2.Text; iDiv = 1; }
            else if (radioButton3.Checked == true) { strFileName = radioButton3.Text; iDiv = 2; }
            else if (radioButton4.Checked == true) { strFileName = radioButton4.Text; iDiv = 3; }
            else if (radioButton5.Checked == true) { strFileName = radioButton5.Text; iDiv = 4; }
            else if (radioButton6.Checked == true) { strFileName = radioButton6.Text; iDiv = 5; }
            else if (radioButton7.Checked == true) { strFileName = radioButton7.Text; iDiv = 6; }

            dlg.FileName = strFileName.Replace(@"/", "_") + ".xls";
            dlg.OverwritePrompt = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    th = new Thread(new ThreadStart(Show_Waiting));
                    th.Start();

                    Excel.Application oAppln = null;
                    Excel.Workbook oWorkBook = null;
                    Excel.Worksheet oWorkSheet = null;

                    oAppln = new Excel.Application();
                    oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
                    oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

                    oAppln.Visible = false;

                    //내용입력
                    int iExcelRow = 0;
                    int iChartNb = 0;
                    string[] Name = null;
                    string[] Title = null;

                    Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {

                    start:
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "장비번호")].Text != "")
                        {
                            if (iDiv <= 5)
                            {
                                if (iDiv == 4 && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NC구분")].Text != "A") //NC
                                {
                                    i++;
                                    if (fpSpread1.Sheets[0].Rows.Count < i)
                                    {
                                        int iValue = fpSpread1.Sheets[0].Rows.Count + 1;
                                        Waiting_Form.progressBar_temp.Value = iValue;
                                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + iValue.ToString() + " Row를 저장하였습니다.";
                                        Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";
                                        goto end;
                                    }
                                    else { goto start; }
                                }

                                if (iDiv == 5 && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NC구분")].Text != "B") //범용
                                {
                                    i++;
                                    if (fpSpread1.Sheets[0].Rows.Count < i)
                                    {
                                        int iValue = fpSpread1.Sheets[0].Rows.Count + 1;
                                        Waiting_Form.progressBar_temp.Value = iValue;
                                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + iValue.ToString() + " Row를 저장하였습니다.";
                                        Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";
                                        goto end;
                                    }
                                    else { goto start; }
                                }

                                if (iExcelRow % 33 == 0)
                                {
                                    iExcelRow++;
                                    int k = 0;

                                    if (iDiv == 0)
                                    {
                                        Name = new string[4];
                                        Title = new string[3] { "장비가동현황", "장비명", "시간(분)" };
                                        //headers 
                                        for (int j = 0; j < Name.Length; j++)
                                        {
                                            // header 저장
                                            oWorkSheet.Cells[iExcelRow, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 5].Text;
                                            Name[k] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 5].Text;
                                            k++;
                                        }
                                    }
                                    else if (iDiv == 1)
                                    {
                                        Name = new string[1];
                                        Title = new string[3] { "장비가동율", "장비명", "백분율(%)" };
                                        //headers 
                                        for (int j = 0; j < Name.Length; j++)
                                        {
                                            // header 저장
                                            oWorkSheet.Cells[iExcelRow, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 10].Text;
                                            Name[k] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 10].Text;
                                            k++;
                                        }
                                    }
                                    else if (iDiv == 2)
                                    {
                                        Name = new string[1];
                                        Title = new string[3] { "장비고장율", "장비명", "백분율(%)" };
                                        //headers 
                                        for (int j = 0; j < Name.Length; j++)
                                        {
                                            // header 저장
                                            oWorkSheet.Cells[iExcelRow, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 11].Text;
                                            Name[k] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 11].Text;
                                            k++;
                                        }
                                    }
                                    else if (iDiv == 3)
                                    {
                                        Name = new string[1];
                                        Title = new string[3] { "장비별 준비시간/가공시간", "장비명", "백분율(%)" };
                                        //headers 
                                        for (int j = 0; j < Name.Length; j++)
                                        {
                                            // header 저장
                                            oWorkSheet.Cells[iExcelRow, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 12].Text;
                                            Name[k] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, j + 12].Text;
                                            k++;
                                        }
                                    }
                                    else if (iDiv == 4)
                                    {
                                        Name = new string[2];
                                        Title = new string[3] { "NC 장비가동율", "장비명", "백분율(%)" };

                                        //headers 
                                        for (int j = 0; j < Name.Length; j++)
                                        {
                                            // header 저장
                                            oWorkSheet.Cells[iExcelRow, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, (j * 2) + 10].Text;
                                            Name[k] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, (j * 2) + 10].Text;
                                            k++;
                                        }
                                    }
                                    else if (iDiv == 5)
                                    {
                                        Name = new string[2];
                                        Title = new string[3] { "범용장비 가동율", "장비명", "백분율(%)" };

                                        //headers 
                                        for (int j = 0; j < Name.Length; j++)
                                        {
                                            // header 저장
                                            oWorkSheet.Cells[iExcelRow, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, (j * 2) + 10].Text;
                                            Name[k] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, (j * 2) + 10].Text;
                                            k++;
                                        }
                                    }

                                    iChartNb++;
                                    Set_Chart(oWorkSheet, iExcelRow + 1, iExcelRow + 32, Name, iChartNb, Title);
                                }

                                if (iDiv == 0)
                                {
                                    oWorkSheet.Cells[iExcelRow + 1, 1] = fpSpread1.Sheets[0].Cells[i, 4].Text + " ";	//장비번호
                                    oWorkSheet.Cells[iExcelRow + 1, 2] = fpSpread1.Sheets[0].Cells[i, 5].Text;	//준비시간
                                    oWorkSheet.Cells[iExcelRow + 1, 3] = fpSpread1.Sheets[0].Cells[i, 6].Text;	//가공시간
                                    oWorkSheet.Cells[iExcelRow + 1, 4] = fpSpread1.Sheets[0].Cells[i, 7].Text;	//고장
                                    oWorkSheet.Cells[iExcelRow + 1, 5] = fpSpread1.Sheets[0].Cells[i, 8].Text;	//유휴
                                }
                                else if (iDiv == 1)
                                {
                                    oWorkSheet.Cells[iExcelRow + 1, 1] = fpSpread1.Sheets[0].Cells[i, 4].Text + " ";	//장비번호
                                    oWorkSheet.Cells[iExcelRow + 1, 2] = fpSpread1.Sheets[0].Cells[i, 10].Text;	//가동율
                                    oWorkSheet.get_Range("B" + iExcelRow + 1, "B2").NumberFormatLocal = "0.00_ ";
                                }
                                else if (iDiv == 2)
                                {
                                    oWorkSheet.Cells[iExcelRow + 1, 1] = fpSpread1.Sheets[0].Cells[i, 4].Text + " ";	//장비번호
                                    oWorkSheet.Cells[iExcelRow + 1, 2] = fpSpread1.Sheets[0].Cells[i, 11].Text;	//고장율
                                    oWorkSheet.get_Range("B" + iExcelRow + 1, "B2").NumberFormatLocal = "0.00_ ";
                                }
                                else if (iDiv == 3)
                                {
                                    oWorkSheet.Cells[iExcelRow + 1, 1] = fpSpread1.Sheets[0].Cells[i, 4].Text + " ";	//장비번호
                                    oWorkSheet.Cells[iExcelRow + 1, 2] = fpSpread1.Sheets[0].Cells[i, 12].Text;	//준비/가공시간
                                    oWorkSheet.get_Range("B" + iExcelRow + 1, "B2").NumberFormatLocal = "0.00_ ";
                                }
                                else if (iDiv == 4 || iDiv == 5)
                                {
                                    oWorkSheet.Cells[iExcelRow + 1, 1] = fpSpread1.Sheets[0].Cells[i, 4].Text + " ";	//장비번호
                                    oWorkSheet.Cells[iExcelRow + 1, 2] = fpSpread1.Sheets[0].Cells[i, 10].Text;	//가동율
                                    oWorkSheet.get_Range("B" + iExcelRow + 1, "B2").NumberFormatLocal = "0.00_ ";
                                    oWorkSheet.Cells[iExcelRow + 1, 3] = fpSpread1.Sheets[0].Cells[i, 12].Text;	//준비/가공시간
                                    oWorkSheet.get_Range("B" + iExcelRow + 1, "B2").NumberFormatLocal = "0.00_ ";
                                }

                                iExcelRow++;
                            }
                            else
                            {
                                i++;
                                if (fpSpread1.Sheets[0].Rows.Count < i)
                                {
                                    int iValue = fpSpread1.Sheets[0].Rows.Count + 1;
                                    Waiting_Form.progressBar_temp.Value = iValue;
                                    Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + iValue.ToString() + " Row를 저장하였습니다.";
                                    Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";
                                    goto end;
                                }
                                else { goto start; }
                            }
                        }
                        else if (iDiv == 6)
                        {

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text.IndexOf("소계") >= 0)
                            { }
                            else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text == "합계")
                            { }
                            else
                            {
                                i++;
                                if (fpSpread1.Sheets[0].Rows.Count < i)
                                {
                                    int iValue = fpSpread1.Sheets[0].Rows.Count + 1;
                                    Waiting_Form.progressBar_temp.Value = iValue;
                                    Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + iValue.ToString() + " Row를 저장하였습니다.";
                                    Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";
                                    goto end;
                                }
                                else { goto start; }
                            }

                            if (iExcelRow % 33 == 0)
                            {
                                iExcelRow++;
                                int k = 0;

                                if (iDiv == 6)
                                {
                                    Name = new string[2];
                                    Title = new string[3] { "반별 장비가동율", "장비명", "백분율(%)" };

                                    //headers 
                                    for (int j = 0; j < Name.Length; j++)
                                    {
                                        // header 저장
                                        oWorkSheet.Cells[iExcelRow, j + 2] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, (j * 2) + 10].Text;
                                        Name[k] = fpSpread1.Sheets[0].ColumnHeader.Cells[1, (j * 2) + 10].Text;
                                        k++;
                                    }
                                }

                                iChartNb++;
                                Set_Chart(oWorkSheet, iExcelRow + 1, iExcelRow + 32, Name, iChartNb, Title);
                            }

                            if (iDiv == 6)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, 3].Text == "")
                                {
                                    oWorkSheet.Cells[iExcelRow + 1, 1] = "TOTAL ";
                                }
                                else
                                {
                                    oWorkSheet.Cells[iExcelRow + 1, 1] = fpSpread1.Sheets[0].Cells[i, 3].Text.Replace("소계", "").ToString();
                                }

                                oWorkSheet.Cells[iExcelRow + 1, 2] = fpSpread1.Sheets[0].Cells[i, 10].Text;	//가동율
                                oWorkSheet.get_Range("B" + iExcelRow + 1, "B2").NumberFormatLocal = "0.00_ ";
                                oWorkSheet.Cells[iExcelRow + 1, 3] = fpSpread1.Sheets[0].Cells[i, 12].Text;	//준비/가공시간
                                oWorkSheet.get_Range("B" + iExcelRow + 1, "B2").NumberFormatLocal = "0.00_ ";
                            }

                            iExcelRow++;
                        }

                        Waiting_Form.progressBar_temp.Value = i + 1;
                        Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (i + 1).ToString() + " Row를 저장하였습니다.";
                    }


                    Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";

                end:
                    oAppln.UserControl = false;
                    oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

                    // 엑셀 파일로 저장
                    oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oWorkBook);

                    Waiting_Form.label_temp.Text = "완료되었습니다.";
                }
                catch (Exception f)
                {
                    //					MessageBox.Show(f.ToString());
                    //					SystemBase.Loggers.Log(this.Name, f.ToString());
                    //					MessageBox.Show(SystemBase.Base.MessageRtn("B0050","엑셀출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Waiting_Form.Close();
                    th.Abort();
                }
            }
        }
        private void Show_Waiting()
		{
			Waiting_Form = new UIForm.ExcelWaiting();
			Waiting_Form.ShowDialog();
		}
		//차트
		private void Set_Chart(Excel._Worksheet oWS, int sRow, int eRow, string[] seriesName, int ChartNb, string[] Name)
		{
			try
			{
				Excel._Workbook oWB;
				Excel.Series oSeries;
				Excel.Range oResizeRange;
				Excel._Chart oChart;				

				oWB = (Excel._Workbook)oWS.Parent;

				oChart = (Excel._Chart)oWB.Charts.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value );

				//데이터 입력

				oResizeRange = oWS.get_Range("B" + sRow + ":E" + eRow, Missing.Value).get_Resize(Missing.Value, seriesName.Length);

				oChart.ChartWizard(oResizeRange, Excel.XlChartType.xl3DColumn, Missing.Value
					, Excel.XlRowCol.xlColumns, Missing.Value, Missing.Value, Missing.Value, 
					Name[0], Name[1], Name[2], Missing.Value);

				//X축레이블
				oSeries = (Excel.Series)oChart.SeriesCollection(1);
				oSeries.XValues = oWS.get_Range("A" + sRow, "A"+ eRow);


				//범례이름			
				for(int i = 1; i <= seriesName.Length; i++)
				{
					oSeries = (Excel.Series)oChart.SeriesCollection(i);
					oSeries.Name = seriesName[i-1];
				}				

				Excel.Axis axis = (Excel.Axis)oChart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
				axis.TickLabels.Orientation = Excel.XlTickLabelOrientation.xlTickLabelOrientationUpward;
				axis.TickLabelSpacing = 1;

				oChart.ChartArea.Font.Size = 5;
				oChart.ChartArea.Font.Name = "굴림체";
				oChart.PlotArea.Width = 700;
				oChart.Legend.Position = Excel.XlLegendPosition.xlLegendPositionTop;

				//차트 넣기
				oChart.Location(Excel.XlChartLocation.xlLocationAsObject, oWS.Name );
		
				//사이즈세팅
				oResizeRange = (Excel.Range)oWS.Rows.get_Item(sRow-1, Missing.Value);
				oWS.Shapes.Item(ChartNb).Top = (float)(double)oResizeRange.Top;
				oResizeRange = (Excel.Range)oWS.Columns.get_Item(1, Missing.Value);
				oWS.Shapes.Item(ChartNb).Left = (float)(double)oResizeRange.Left;
				oResizeRange = (Excel.Range)oWS.Columns.get_Item(13, Missing.Value);
				oWS.Shapes.Item(ChartNb).Width = (float)(double)oResizeRange.Left;
				oResizeRange = (Excel.Range)oWS.Rows.get_Item(34, Missing.Value);
				oWS.Shapes.Item(ChartNb).Height = (float)(double)oResizeRange.Top;
			}
			catch(Exception f)
			{
				MessageBox.Show(f.ToString());
			}
		}
		#endregion
    }
}
