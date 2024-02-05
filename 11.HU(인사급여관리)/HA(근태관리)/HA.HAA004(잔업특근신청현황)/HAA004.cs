#region 작성정보
/*********************************************************************/
// 단위업무명 : 잔업특근신청현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-04
// 작성내용 : 잔업특근신청현황 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using WNDW;

namespace HA.HAA004
{
    public partial class HAA004 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strSchNo = "";
        string strBtn = "N";
        #endregion

        #region 생성자
        public HAA004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HAA004_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                        string strQuery = " usp_HAA004 'S1', @pDATE = '" + dtpDate.Text + "' ";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                        //					fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                        if (fpSpread2.Sheets[0].Rows.Count > 0)
                        {
                            //상세정보조회
                            SubSearch(fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "내부부서코드")].Text);
                        }
                        else
                        {
                            fpSpread1.Sheets[0].RowCount = 0;
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
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strInternalCd)
        {
            string strQuery = " usp_HAA004  'S2'";
            strQuery = strQuery + ", @pINTERNAL_CD ='" + strInternalCd + "' ";
            strQuery = strQuery + ", @pDATE  ='" + dtpDate.Text + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 2, true);
        }
        #endregion

        #region 셀클릭시 상세조회
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        if (fpSpread2.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
                        {
                            fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
                            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread2.Sheets[0].Cells[i, e.Column].Value = false;
                                }
                            }
                        }
                        else
                        {
                            fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
                            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread2.Sheets[0].Cells[i, e.Column].Value = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    SubSearch(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "내부부서코드")].Text);
                }
            }
        }
        #endregion

        #region Save
        protected override void SaveExec()
        {

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {

                    string strConfirmFlag = "N";
                    if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "마감")].Text == "True")
                    {
                        strConfirmFlag = "Y";
                    }

                    string strSql = " usp_HAA004 @pTYPE = 'U1' ";
                    strSql = strSql + ", @pDATE = '" + dtpDate.Text + "'";
                    strSql = strSql + ", @pINTERNAL_CD = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "내부부서코드")].Text + "'";
                    strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                    strSql = strSql + ", @pCONFIRM_FLAG = '" + strConfirmFlag + "' ";
                    strSql = strSql + ", @pPROC_YN = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부서장 승인여부")].Text + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                }

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                SearchExec();
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
        #endregion

        #region 잔업식권집계표 버튼클릭
        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            //조회 필수 체크
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                //string[] FormulaField = new string[2];	  //formula 값			
                string RptName = "";    // 레포트경로+레포트명
                string[] RptParmValue = new string[7];   // SP 파라메타 값
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름

                RptName = SystemBase.Base.ProgramWhere + @"\Report\HAA004.rpt";
                RptParmValue[0] = "P1";
                RptParmValue[1] = dtpDate.Text;
                RptParmValue[2] = "";
                RptParmValue[3] = "";
                RptParmValue[4] = "";
                RptParmValue[5] = "";
                RptParmValue[6] = SystemBase.Base.gstrCOMCD;

                FormulaFieldValue[0] = "";
                FormulaFieldName[0] = "";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", FormulaFieldValue, FormulaFieldName, RptName, RptParmValue); //공통크리스탈 10버전	
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("출력 대상을 찾을 수 없습니다. 조회 후 출력하세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
