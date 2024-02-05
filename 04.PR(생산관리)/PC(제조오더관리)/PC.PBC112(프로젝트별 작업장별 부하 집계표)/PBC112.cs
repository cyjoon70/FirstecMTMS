#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별 작업장별 부하 집계표
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 프로젝트별 작업장별 부하 집계표 관리
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

namespace PC.PBC112
{
    public partial class PBC112 : UIForm.FPCOMM1
    {
        #region 변수선언
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion
        
        #region 생성자
        public PBC112()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PBC112_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToString().Substring(0, 7);
            dtpWorkDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(1).AddMonths(-1).ToString().Substring(0,7);
            
            dtpWorkDtTo.Enabled = false;

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
            dtpWorkDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToString().Substring(0, 7);
            dtpWorkDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(1).ToString().Substring(0, 7);
            dtpWorkDtTo.Enabled = false;

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

        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //부품팝업
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtITEM_CD.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품팝업
        private void btnGroup_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtGroupCd.Text, "10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroupCd.Value = Msgs[2].ToString();
                    txtGroupNm.Value = Msgs[3].ToString();
                    txtGroupCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnJob_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtJobCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정작업코드 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtJobCd.Value = Msgs[0].ToString();
                    txtJobNm.Value = Msgs[1].ToString();
                    txtJobCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
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
        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch
            {

            }
        }
        
        // 품목
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        //제품
        private void txtGroupCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroupCd.Text != "")
                {
                    txtGroupNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroupCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGroupNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //작업
        private void txtJobCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtJobCd.Text != "")
                {
                    txtJobNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtJobNm.Value = "";
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
                    string strDtFr = dtpWorkDtFr.Text + "-01";
                    string strDtTo = dtpWorkDtFr.Text + "-01";
                    strDtTo = Convert.ToDateTime(strDtTo).AddYears(1).AddMonths(1).AddDays(-1).ToShortDateString();

                    string WorkFlg = "";
                    if (rdoWORKALL.Checked == true) { WorkFlg = "A"; }
                    else if (rdoWORK_1.Checked == true) { WorkFlg = "B"; }
                    else if (rdoWORK_2.Checked == true) { WorkFlg = "C"; }
                    else if (rdoWORK_3.Checked == true) { WorkFlg = "D"; }

                    string MilestoneFlg = "";
                    if (rdoYes.Checked == true) { MilestoneFlg = "Y"; }
                    else if (rdoNo.Checked == true) { MilestoneFlg = "N"; }

                    string strReportFlag = "";
                    if (rdoDivY.Checked == true) strReportFlag = "Y";
                    else if (rdoDivN.Checked == true) strReportFlag = "N";

                    string OrderStatusFlag = "";
                    if (rdoRl.Checked == true) { OrderStatusFlag = "RL"; }
                    else if (rdoSt.Checked == true) { OrderStatusFlag = "ST"; }
                    else if (rdoRlSt.Checked == true) { OrderStatusFlag = "RLST"; }
                    else if (rdoCl.Checked == true) { OrderStatusFlag = "CL"; }

                    string strQuery = "usp_PBC112 @pTYPE = 'S1'";
                    strQuery += ", @pWORK_DT_FR = '" + strDtFr + "'";
                    strQuery += ", @pWORK_DT_TO = '" + strDtTo + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pWC_CD = '" + txtWcCd.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pORDER_STATUS = '" + OrderStatusFlag + "' ";
                    strQuery += ", @pMILESTONE_FLG = '" + MilestoneFlg + "' ";
                    strQuery += ", @pREPORT_FLAG ='" + strReportFlag + "'";
                    strQuery += ", @pWORK_FLG ='" + WorkFlg + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text +"' ";
                    strQuery += ", @pJOB_CD = '" + txtJobCd.Text + "' ";
                    strQuery += ", @pGROUP_CD = '" + txtGroupCd.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtITEM_CD.Text + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pSO_DELIVERY_DT_FR = '" + dtpSoDeliveryDtFr.Text + "'";     // 2020.06.09. hma 추가: 수주납기일(참조) FROM
                    strQuery += ", @pSO_DELIVERY_DT_TO = '" + dtpSoDeliveryDtTo.Text + "'";     // 2020.06.09. hma 추가: 수주납기일(참조) TO

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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

            //조회 조건에 맞게 Head명 바꾸기
            for (int i = 5; i < 17; i++)
            {
                string strDtFr = Convert.ToDateTime(dtpWorkDtFr.Value).AddMonths(i - 5).ToString().Substring(0,7);
                
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = strDtFr.Substring(2, 5).Replace("-", ".");
            }


            fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
            fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Always);

            //소계, 합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text == "합계") // 합계색변경
                    fpSpread1.Sheets[0].Cells[i, 1, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = SystemBase.Base.gColor2;

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text == "소계")
                    fpSpread1.Sheets[0].Cells[i, 4, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = SystemBase.Base.gColor3; //소계 색변경

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text == "총합계")
                    fpSpread1.Sheets[0].Cells[i, 1, i, fpSpread1.Sheets[0].Columns.Count - 1].BackColor = SystemBase.Base.gColor1; //소계 색변경
				
            }
        }
        #endregion

        #region 조회조건 TO 날짜 고정
        private void dtpWorkDtFr_ValueChanged(object sender, System.EventArgs e)
        {
            dtpWorkDtTo.Value = Convert.ToDateTime(dtpWorkDtFr.Value).AddYears(1).AddMonths(-1).ToString().Substring(0,7);
        }

        #endregion

        #region Excel 출력
        //protected override void ExcelExec()
        //{
        //    if (fpSpread1.Sheets[0].Rows.Count <= 0)
        //    {
        //        MessageBox.Show(SystemBase.Base.MessageRtn("B0053"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    th = new Thread(new ThreadStart(Show_Waiting));
        //    th.Start();

        //    SaveFileDialog dlg = new SaveFileDialog();
        //    dlg.Title = "Excel 다운로드 위치 지정";
        //    dlg.InitialDirectory = dlg.FileName;
        //    dlg.Filter = "전체(*.*)|*.*|Excel Files(*.xls)|*.xls";
        //    dlg.FilterIndex = 1;
        //    dlg.RestoreDirectory = true;
        //    dlg.FileName = this.Text.ToString().Replace(@"/", "_") + ".xls";
        //    dlg.OverwritePrompt = false;

        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            Excel.Application oAppln = null;
        //            Excel.Workbook oWorkBook = null;
        //            Excel.Worksheet oWorkSheet = null;
        //            Excel.Range oRange = null;

        //            oAppln = new Excel.Application();
        //            oWorkBook = (Excel.Workbook)(oAppln.Workbooks.Add(true));
        //            oWorkSheet = (Excel.Worksheet)oWorkBook.ActiveSheet;

        //            int iRow = 4;
        //            int iRowSpan = 4;

        //            Waiting_Form.Activate();
        //            Waiting_Form.label_temp.Text = "엑셀 HEAD를 생성중입니다.";

        //            int iColumn = fpSpread1.Sheets[0].Columns.Count;


        //            //조회조건 저장
        //            oWorkSheet.Cells[1, 2] = "공장";
        //            oWorkSheet.Cells[1, 3] = txtPlantNm.Text;
        //            oWorkSheet.Cells[1, iColumn - 2] = "인쇄일자";
        //            oWorkSheet.Cells[1, iColumn - 1] = SystemBase.Base.ServerTime("YYMMDD");

        //            oWorkSheet.Cells[2, 2] = "일자";
        //            oWorkSheet.Cells[2, 3] = dtpWorkDtFr.Text + " ~ " + dtpWorkDtTo.Text;
        //            oWorkSheet.Cells[2, iColumn - 1] = "(단위 : 시간)";

        //            //headers 
        //            for (int j = 3; j < fpSpread1.Sheets[0].Columns.Count; j++)
        //            {
        //                // header 저장
        //                oWorkSheet.Cells[3, j] = fpSpread1.Sheets[0].ColumnHeader.Cells[0, j].Text;
        //            }

        //            Waiting_Form.progressBar_temp.Maximum = fpSpread1.Sheets[0].Rows.Count;

        //            for (int rowNo = 0; rowNo < fpSpread1.Sheets[0].Rows.Count; rowNo++)
        //            {
        //                string strQRow = "Q" + iRow;
        //                string strCRow = "C" + iRow;
        //                string strDRow = "D" + iRow;


        //                //내용 저장							
        //                for (int colNo = 3; colNo < fpSpread1.Sheets[0].Columns.Count; colNo++)
        //                {
        //                    oWorkSheet.Cells[iRow, colNo] = fpSpread1.Sheets[0].Cells[rowNo, colNo].Text;
        //                }

        //                if (fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "공정명")].Text == "")
        //                {
        //                    string iRowSpan1 = "C" + iRowSpan;
        //                    string iRowSpan2 = "C" + (iRow - 1);
        //                    if (iRowSpan < iRow - 1)
        //                    {
        //                        for (int i = iRowSpan; i < iRow - 1; i++)
        //                        {
        //                            oWorkSheet.Cells[i + 1, 3] = "";
        //                        }
        //                        oWorkSheet.get_Range(iRowSpan1, iRowSpan2).Merge(false);
        //                    }

        //                    oWorkSheet.get_Range(strCRow, strDRow).Merge(true);

        //                    //합계,소계 색 변경
        //                    if (fpSpread1.Sheets[0].Cells[rowNo, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text == "합계")
        //                        oWorkSheet.get_Range(strCRow, strQRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor1);
        //                    else
        //                        oWorkSheet.get_Range(strCRow, strQRow).Interior.Color = ColorTranslator.ToOle(SystemBase.Base.gColor2); //소계 색변경

        //                    oWorkSheet.get_Range(strCRow, strDRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

        //                    iRowSpan = iRow + 1;

        //                }

        //                //순번
        //                oWorkSheet.Cells[iRow, 2] = rowNo + 1;
        //                oWorkSheet.get_Range("B3", "B" + iRow).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                oWorkSheet.get_Range("B3", "B" + iRow).Interior.Color = ColorTranslator.ToOle(Color.Beige);

        //                iRow++;

        //                Waiting_Form.progressBar_temp.Value = rowNo + 1;
        //                Waiting_Form.label_temp.Text = "총" + fpSpread1.Sheets[0].Rows.Count.ToString() + " Row 중 " + (rowNo + 1).ToString() + " Row를 저장하였습니다.";
        //            }


        //            string strColumn = "Q" + (iRow - 1);

        //            //헤드 색지정,테두리 설정
        //            oRange = oWorkSheet.get_Range("B3", "Q3");
        //            oRange.Interior.Color = ColorTranslator.ToOle(Color.Beige);
        //            oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //            oRange.Borders.LineStyle = 1;
        //            oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);

        //            //내용 테두리 설정					
        //            oRange = oWorkSheet.get_Range("B4", strColumn);
        //            oRange.Borders.LineStyle = 1;
        //            oRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexAutomatic, Excel.XlColorIndex.xlColorIndexAutomatic);

        //            Waiting_Form.label_temp.Text = "엑셀 Sheet를 열고 있습니다.";

        //            oRange = oWorkSheet.get_Range("A1", strColumn);
        //            oRange.EntireColumn.AutoFit();

        //            oAppln.UserControl = false;
        //            oAppln.Visible = true;	// 저장후 저장된 내용 실행여부

        //            // 엑셀 파일로 저장
        //            oWorkBook.SaveAs(dlg.FileName, Excel.XlFileFormat.xlWorkbookNormal, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, false, false, null, null, null);

        //            System.Runtime.InteropServices.Marshal.ReleaseComObject(oWorkBook);

        //            Waiting_Form.label_temp.Text = "완료되었습니다.";

        //        }
        //        catch (Exception)
        //        {
        //            th.Abort();
        //        }
        //    }
        //    th.Abort();
        //}

        //private void Show_Waiting()
        //{
        //    Waiting_Form = new UIForm.ExcelWaiting();
        //    Waiting_Form.ShowDialog();
        //}
        #endregion

        
    }
}
