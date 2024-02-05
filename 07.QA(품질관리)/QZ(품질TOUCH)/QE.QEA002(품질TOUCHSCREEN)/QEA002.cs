#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질 TOUCH SCREEN
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-08
// 작성내용 : 품질 TOUCH SCREEN 등록 및 관리
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
using System.Globalization;
using System.Text.RegularExpressions;

namespace QE.QEA002
{
    public partial class QEA002 : UIForm.FPCOMM2
    {
        #region 생성자
        public QEA002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void QEA002_Load(object sender, System.EventArgs e)
        {
            try
            {
                //필수체크
                SystemBase.Validation.GroupBox_Setting(groupBox1);

                txtHResCd.Value = SystemBase.Base.gstrUserID.ToString().Replace("FST", "");

                string Query = " SELECT A.RES_DIS ";
                Query = Query + "	  , A.WORKCENTER_CD ";
                Query = Query + "	  , WC_NM = (SELECT CD_NM FROM B_COMM_CODE(NOLOCK) WHERE MAJOR_CD = 'P002' AND MINOR_CD = A.WORKCENTER_CD  AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "') ";
                Query = Query + "  FROM P_RESO_MANAGE A(NOLOCK) ";
                Query = Query + " WHERE A.RES_CD = '" + txtHResCd.Text + "' ";
                Query = Query + " AND A.CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtHResNm.Value = dt.Rows[0][0].ToString();
                    txtWcCd.Value = dt.Rows[0][1].ToString();
                    txtWcNm.Value = dt.Rows[0][2].ToString();
                }
                else
                {
                    MessageBox.Show("작업자에 대한 정보가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //작업자 상태 세팅
                if (txtHResCd.Text != "")
                {
                    string strQuery = " SELECT TOP 1 WORK_STATUS FROM Q_TOUCH_WORKING(NOLOCK) WHERE H_RES_CD = '" + txtHResCd.Text + "'  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt1.Rows.Count > 0)
                    {
                        txtResStatus.Value = dt1.Rows[0][0].ToString();
                    }
                    else
                    {
                        txtResStatus.Value = "01"; //대기상태
                    }
                }

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                Btn_Setting(false);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 여는"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region Btn_Setting-버튼세팅
        private void Btn_Setting(bool btnFlag)
        {           
            btnLeft.Enabled = btnFlag;
            btnRight.Enabled = btnFlag;
            btnRestStart.Enabled = btnFlag;
            btnRestEnd.Enabled = btnFlag;
            btnReadyStart.Enabled = btnFlag;
            btnReadyEnd.Enabled = btnFlag;
            btnWorkStart.Enabled = btnFlag;
            btnWorkEnd.Enabled = btnFlag;
            btnWorkClose.Enabled = btnFlag;
            btnIndirectStart.Enabled = btnFlag;
            btnIndirectEnd.Enabled = btnFlag;
            txtIndirectCd.Enabled = btnFlag;
            btnInDir.Enabled = btnFlag;

            if (btnFlag == false)
            {
                btnLeft.FlatStyle = FlatStyle.Flat;
                btnLeft.BackColor = System.Drawing.SystemColors.Control;
                btnRight.FlatStyle = FlatStyle.Flat;
                btnRight.BackColor = System.Drawing.SystemColors.Control;
                btnRestStart.FlatStyle = FlatStyle.Flat;
                btnRestStart.BackColor = System.Drawing.SystemColors.Control;
                btnRestEnd.FlatStyle = FlatStyle.Flat;
                btnRestEnd.BackColor = System.Drawing.SystemColors.Control;
                btnReadyStart.FlatStyle = FlatStyle.Flat;
                btnReadyStart.BackColor = System.Drawing.SystemColors.Control;
                btnReadyEnd.FlatStyle = FlatStyle.Flat;
                btnReadyEnd.BackColor = System.Drawing.SystemColors.Control;
                btnWorkStart.FlatStyle = FlatStyle.Flat;
                btnWorkStart.BackColor = System.Drawing.SystemColors.Control;
                btnWorkEnd.FlatStyle = FlatStyle.Flat;
                btnWorkEnd.BackColor = System.Drawing.SystemColors.Control;
                btnWorkClose.FlatStyle = FlatStyle.Flat;
                btnWorkClose.BackColor = System.Drawing.SystemColors.Control;
                btnIndirectStart.FlatStyle = FlatStyle.Flat;
                btnIndirectStart.BackColor = System.Drawing.SystemColors.Control;
                btnIndirectEnd.FlatStyle = FlatStyle.Flat;
                btnIndirectEnd.BackColor = System.Drawing.SystemColors.Control;
                btnInDir.FlatStyle = FlatStyle.Flat;
                btnInDir.BackColor = System.Drawing.SystemColors.Control;
            }
            else
            {               
                btnLeft.FlatStyle = FlatStyle.Standard;
                btnLeft.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnRight.FlatStyle = FlatStyle.Standard;
                btnRight.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnRestStart.FlatStyle = FlatStyle.Standard;
                btnRestStart.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnRestEnd.FlatStyle = FlatStyle.Standard;
                btnRestEnd.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnReadyStart.FlatStyle = FlatStyle.Standard;
                btnReadyStart.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnReadyEnd.FlatStyle = FlatStyle.Standard;
                btnReadyEnd.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnWorkStart.FlatStyle = FlatStyle.Standard;
                btnWorkStart.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnWorkEnd.FlatStyle = FlatStyle.Standard;
                btnWorkEnd.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnWorkClose.FlatStyle = FlatStyle.Standard;
                btnWorkClose.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnIndirectStart.FlatStyle = FlatStyle.Standard;
                btnIndirectStart.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnIndirectEnd.FlatStyle = FlatStyle.Standard;
                btnIndirectEnd.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                btnInDir.FlatStyle = FlatStyle.Standard;
                btnInDir.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            }
        }
        #endregion

        #region 개별버튼 BtnEnable
        private void BtnEnable(System.Windows.Forms.Button Btn, bool Flag)
        {
            Btn.Enabled = Flag;

            if (Flag == true)
            {
                Btn.FlatStyle = FlatStyle.Standard;
                Btn.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            }
            else
            {
                Btn.BackColor = System.Drawing.SystemColors.Control;
                Btn.FlatStyle = FlatStyle.Flat;
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            MainSearch();
            SubSearch();

            string Query = "SELECT WORK_STATUS, WORK_NO FROM P_WORKORDER_TOUCH_MASTER(NOLOCK) WHERE H_RES_CD = '" + txtHResCd.Text + "' AND WORK_DT = '" + DateTime.Now.ToShortDateString() + "' ";
            Query += " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtResStatus.Value = dt.Rows[0][0].ToString();

                if (dt.Rows[0][0].ToString() == "04")
                {
                    string strQuery = "SELECT INDIRECT_CD FROM P_WORKORDER_TOUCH_INDIRECT(NOLOCK) WHERE WORK_NO = '" + dt.Rows[0][1].ToString() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ORDER BY WORK_SEQ DESC ";

                    DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt1.Rows.Count > 0)
                    {
                        txtIndirectCd.Value = dt1.Rows[0][0].ToString();
                    }
                    else
                    {
                        txtIndirectCd.Value = "";
                    }
                }
            }
            else
            {
                txtResStatus.Value = "01";
            }

            WorkStatusProc(txtResStatus.Text);

            if (txtResStatus.Text == "01")
            {
                lblMsg.Text = "작업 대기 상태 입니다.";
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 작업상태에 따른 처리
        private void WorkStatusProc(string WorkStatus)
        {
            switch (WorkStatus)
            {
                case "01":	//대기
                    Btn_Setting(true);
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;

                    break;

                case "02":	//용무시작
                    Btn_Setting(false);
                    BtnEnable(btnRestEnd, true);
                    break;

                case "03":	//용무완료
                    Btn_Setting(true);
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;
                    break;

                case "04":	//간접시작
                    Btn_Setting(false);
                    BtnEnable(btnIndirectEnd, true);
                    txtIndirectCd.Enabled = false;
                    btnInDir.Enabled = false;
                    break;

                case "05":	//간접완료
                    Btn_Setting(true);
                    txtIndirectCd.Value = "";
                    txtIndirectNm.Value = "";
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    break;

                case "06":	//준비시작
                    Btn_Setting(false);
                    BtnEnable(btnReadyEnd, true);
                    break;

                case "07":	//준비완료
                    Btn_Setting(true);
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;
                    break;

                case "08":	//실동시작
                    Btn_Setting(false);
                    BtnEnable(btnWorkEnd, true);
                    break;

                case "09":	//실동완료
                    Btn_Setting(true);
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;
                    break;

                case "10":	//동실시작
                    Btn_Setting(false);
                    BtnEnable(btnWorkEnd, true);
                    break;

                case "11":	//동실완료
                    Btn_Setting(true);
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;
                    break;

                case "12":	//마감
                    Btn_Setting(true);
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;
                    break;

                default:
                    Btn_Setting(false);
                    BtnEnable(btnRestEnd, false);
                    BtnEnable(btnIndirectEnd, false);
                    BtnEnable(btnReadyEnd, false);
                    BtnEnable(btnWorkEnd, false);
                    txtIndirectCd.Enabled = true;
                    btnInDir.Enabled = true;
                    break;
            }
        }
        #endregion

        #region 작업대기Order조회
        private void MainSearch()
        {
            string strSql1 = "usp_QEA002  'S1'";
            strSql1 = strSql1 + ", @pH_RES_CD='" + txtHResCd.Text + "' ";
            strSql1 = strSql1 + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
        }
        #endregion

        #region 작업중Order조회
        private void SubSearch()
        {
            string strSql1 = "usp_QEA002  'S2'";
            strSql1 = strSql1 + ", @pH_RES_CD='" + txtHResCd.Text + "' ";
            strSql1 = strSql1 + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread2, strSql1, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
        }
        #endregion

        #region 작업Order이동
        //작업 Order로 이동
        private void btnRight_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True") //체크된것만
                    {
                        string strSql = "";
                        strSql += " usp_QEA002 'C1'";
                        strSql += ", @pINSP_REQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "'";
                        strSql += ", @pH_RES_CD = '" + txtHResCd.Text + "'";
                        strSql += ", @pSHEET_DT ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "배정일자")].Text + "'";

                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode);
                lblMsg.ForeColor = Color.Blue;

                SearchExec();
            }
            else if (ERRCode == "ER")
            { lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode); lblMsg.ForeColor = Color.Red; }
            else
            { lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode); lblMsg.ForeColor = Color.Red; }

            this.Cursor = Cursors.Default;
        }
        //작업대기 Order로 이동
        private void btnLeft_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread2.Sheets[0].Cells[i, 1].Text == "True") //체크된것만
                    {
                        string strSql = "";
                        strSql += " usp_QEA002 'C2'";
                        strSql += ", @pINSP_REQ_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "'";
                        strSql += ", @pH_RES_CD = '" + txtHResCd.Text + "'";
                        strSql += ", @pSHEET_DT ='" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "배정일자")].Text + "'";

                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode);
                lblMsg.ForeColor = Color.Blue;

                SearchExec();
            }
            else if (ERRCode == "ER")
            { lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode); lblMsg.ForeColor = Color.Red; }
            else
            { lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode); lblMsg.ForeColor = Color.Red; }

            this.Cursor = Cursors.Default;
        }
        #endregion

        //#region Fpspread2 체크 헤드 이벤트
        //private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        //{
        //    if (fpSpread2.Sheets[0].Rows.Count > 0)
        //    {
        //        if (fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
        //        {
        //            if (e.ColumnHeader == true)
        //            {
        //                if (fpSpread2.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
        //                {
        //                    fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
        //                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
        //                    {
        //                        fpSpread2.Sheets[0].Cells[i, e.Column].Value = false;
        //                    }
        //                }
        //                else
        //                {
        //                    fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
        //                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
        //                    {
        //                        fpSpread2.Sheets[0].Cells[i, e.Column].Value = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //#endregion

        #region 버튼 이벤트
        //용무여유시작
        private void btnRestStart_Click(object sender, System.EventArgs e)
        {
            Proc("02", "N");
        }

        //용무여유완료
        private void btnRestEnd_Click(object sender, System.EventArgs e)
        {
            Proc("03", "N");
        }

        //간접시작
        private void btnIndirectStart_Click(object sender, System.EventArgs e)
        {
            Proc("04", "N");
        }

        //간접완료
        private void btnIndirectEnd_Click(object sender, System.EventArgs e)
        {
            Proc("05", "N");
        }

        //준비시작
        private void btnReadyStart_Click(object sender, System.EventArgs e)
        {
            Proc("06", "Y");
        }

        //준비완료
        private void btnReadyEnd_Click(object sender, System.EventArgs e)
        {
            Proc("07", "N");
        }

        //실동시작
        private void btnWorkStart_Click(object sender, System.EventArgs e)
        {
            int chk = 0;

            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    chk++;
                }
            }

            if (chk > 1)
            {
                Proc("10", "Y"); //동시실동시작
            }
            else
            {
                Proc("08", "N"); //실동시작
            }
        }

        //실동완료
        private void btnWorkEnd_Click(object sender, System.EventArgs e)
        {
            int chk = 0;

            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    chk++;
                }
            }

            if (chk > 1)
            {
                Proc("11", "N"); //동시실동완료
            }
            else
            {
                Proc("09", "N"); //실동완료
            }
        }

        //마감
        private void btnWorkClose_Click(object sender, System.EventArgs e)
        {
            Proc("12", "N");
        }
        #endregion

        #region 상태에 따른 작업 처리 로직
        private void Proc(string Status, string AllWorkFlag)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
            string AllWorkNo = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (AllWorkFlag == "Y") //동시시작이면
                {
                    DataTable AllWorkDt = SystemBase.DbOpen.NoTranDataTable("usp_QEA002 'C3', @pH_RES_CD = '" + txtHResCd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                    if (AllWorkDt.Rows.Count > 0)
                    {
                        AllWorkNo = AllWorkDt.Rows[0][0].ToString();
                    }
                    else
                    {
                        Trans.Rollback();
                        MessageBox.Show("동시작업번호 채번 중 오류가 발생하였습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }

                if (Status == "02" || Status == "03" || Status == "04" || Status == "05")
                {
                    string strSql = "";
                    strSql += " usp_QEA002 'P1'";
                    strSql += ", @pH_RES_CD = '" + txtHResCd.Text + "'";
                    strSql += ", @pWORK_STATUS ='" + Status + "'";
                    strSql += ", @pWC_CD ='" + txtWcCd.Text + "'";
                    strSql += ", @pINDIRECT_CD = '" + txtIndirectCd.Text + "' ";
                    strSql += ", @pINDIRECT_NM = '" + txtIndirectNm.Text + "' ";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                }
                else
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        string strSql = "";
                        strSql += " usp_QEA002 'P1'";
                        strSql += ", @pINSP_REQ_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사의뢰번호")].Text + "'";
                        strSql += ", @pH_RES_CD = '" + txtHResCd.Text + "'";
                        strSql += ", @pSHEET_DT ='" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "배정일자")].Text + "'";
                        strSql += ", @pWORK_STATUS ='" + Status + "'";
                        strSql += ", @pWC_CD ='" + txtWcCd.Text + "'";
                        strSql += ", @pINSP_CLASS_CD ='" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사구분코드")].Text + "'";
                        strSql += ", @pREF_NO ='" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "참조번호")].Text + "'";
                        strSql += ", @pREF_SEQ ='" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "참조순번")].Text + "'";
                        strSql += ", @pSHEET_QTY ='" + Convert.ToInt32(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "배정수량")].Value) + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        if (AllWorkFlag == "Y")
                        {
                            strSql += ", @pALLWORK_NO ='" + AllWorkNo + "'";
                        }
                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode);
                lblMsg.ForeColor = Color.Blue;

                //				txtResStatus.Text = Status;
                SearchExec();
            }
            else if (ERRCode == "ER")
            { lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode); lblMsg.ForeColor = Color.Red; }
            else
            { lblMsg.Text = SystemBase.Base.MessageRtn(MSGCode); lblMsg.ForeColor = Color.Red; }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 활성화 될때마다 조회
        private void QEA002_Activated(object sender, System.EventArgs e)
        {
            SearchExec();
        }
        #endregion

        #region 간접항목 text_Change이벤트
        private void txtIndirectCd_TextChanged(object sender, System.EventArgs e)
        {
            txtIndirectNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtIndirectCd.Text, " AND MAJOR_CD = 'P025' AND MINOR_CD <> 'Z01'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 간접항목 팝업
        private void btnInDir_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P613' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtIndirectCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00072", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "간접항목조회");	//비가동항목 조회
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIndirectCd.Text = Msgs[0].ToString();
                    txtIndirectNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "간접항목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
          

    }
}