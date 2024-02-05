#region 작성정보
/*********************************************************************/
// 단위업무명:  프로젝트별작업시간집계표-공수
// 작 성 자  :  한 미 애
// 작 성 일  :  2017-02-21
// 작성내용  :  프로젝트별 월별 작업시간 집계 및 엑셀 업로드 처리
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace CW.CWAA05
{
    public partial class CWAA05 : UIForm.FPCOMM1
    {
        #region 변수선언       
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        public CWAA05()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CWAA05_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
           
            //기타 세팅		
            dtpWorkDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).AddMonths(1).ToShortDateString().Substring(0,7);
            dtpWorkDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,7);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;

            SearchExec();
        }
        #endregion


        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].RowCount = 0;

            //기타 세팅		
            dtpWorkDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).AddMonths(1).ToShortDateString().Substring(0, 7);
            dtpWorkDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);

            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
        }
        #endregion


        #region 조회조건 TO 날짜 고정
        private void dtpWorkDtFr_ValueChanged(object sender, System.EventArgs e)
        {
            dtpWorkDtTo.Value =  Convert.ToDateTime(dtpWorkDtFr.Text + "-01").AddYears(1).AddMonths(-1).ToShortDateString().Substring(0, 7);
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
                    string strDtTo = Convert.ToDateTime(dtpWorkDtTo.Value).AddMonths(1).Date.ToString().Substring(0, 7);
                    strDtTo = strDtTo.Substring(0, 7) + "-01";
                    string strDtTo1 = Convert.ToDateTime(strDtTo).AddDays(-1).ToShortDateString().Substring(0, 7);

                    string strQuery = "usp_CWAA05 @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pDEPT_TYPE = 'E' ";
                    strQuery += ", @pYYYYMM_FR = '" + strDtFr + "'";
                    strQuery += ", @pYYYYMM_TO = '" + strDtTo1 + "'";

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


        #region DelExec() 삭제 로직
        // 2017.02.23. hma 추가(Start)
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(dtpWorkDtFr.Text + " ~ " + dtpWorkDtTo.Text + " 기간의 공수 데이터를 삭제하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strDtFr = dtpWorkDtFr.Text + "-01";
                    string strDtTo = Convert.ToDateTime(dtpWorkDtTo.Value).AddMonths(1).Date.ToString().Substring(0, 7);
                    strDtTo = strDtTo.Substring(0, 7) + "-01";
                    string strDtTo1 = Convert.ToDateTime(strDtTo).AddDays(-1).ToShortDateString().Substring(0, 7);

                    string strQuery = " usp_CWAA05 'D2'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD = '" + txtPlantCd.Text + "'";
                    strQuery += ", @pDEPT_TYPE = 'E' ";
                    strQuery += ", @pYYYYMM_FR = '" + strDtFr + "'";
                    strQuery += ", @pYYYYMM_TO = '" + strDtTo1 + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fpSpread1.Sheets[0].Rows.Count = 0;
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        // 2017.02.23. hma 추가(End)
        #endregion

        #region 소계 합계 그리드 재정의
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;
            int iRow = 0;

            //조회 조건에 맞게 Head명 바꾸기
            for (int i = 4; i < 16; i++)
            {
                string strDtFr = Convert.ToDateTime(dtpWorkDtFr.Text + "-01").AddMonths(i - 4).ToString();
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, i].Text = strDtFr.Substring(2, 5).Replace("-", ".");
            }

            fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);

            //소계, 합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, 2].Text == "소 계")
                {
                    for (int j = 2; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor3;
                    }
                    fpSpread1.Sheets[0].Cells[i, 2].ColumnSpan = 2;
                }
                else if (fpSpread1.Sheets[0].Cells[i, 1].Text == "합 계")
                {
                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                    fpSpread1.Sheets[0].Cells[i, 1].ColumnSpan = 3; 
                }			
            }
        }
        #endregion

        #region 조회조건 팝업
        //공장코드
        private void btnPlantCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region 조회조건 TextChanged       
        //공장번호
        private void txtPlantCd_TextChanged(object sender, EventArgs e)
        {
            txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        #endregion


        #region 파일 Upload 버튼 클릭시 이벤트
        private void btnFileUpload_Click(object sender, EventArgs e)
        {
            try
            {
                CWAA05P1 frm1 = new CWAA05P1(txtPlantCd.Text);
                frm1.ShowDialog();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
