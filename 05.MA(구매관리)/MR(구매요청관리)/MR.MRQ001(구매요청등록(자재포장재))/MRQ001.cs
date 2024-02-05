#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매요청등록(자재/포장재)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-26
// 작성내용 : 구매요청등록(자재/포장재) 및 관리
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
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace MR.MRQ001
{
    public partial class MRQ001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoReqNo = ""; //요청번호
        string strBtn = "N";
        bool btnNew_is = true;
        private bool form_act_chk = false;
        #endregion

        #region 생성자
        public MRQ001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MRQ001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //GroupBox2 입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            dtpSReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpSReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpReqDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            rdoMpr.Checked = true;
            rdoManual.Checked = true;
            rdoCfm_N.Checked = true;

            panel04.Enabled = false;
            panel3.Enabled = false;
            panel2.Enabled = false;

            NewFlg = 1;
            strAutoReqNo = "";

        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false && rdoSpr.Checked == false && (rdoP.Checked == false || NewFlg == 1))
            {
                UIForm.FPMake.RowInsert(fpSpread1);
            }
            else
            {
                if (rdoCfm_Y.Checked == true)
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
                else if (rdoSpr.Checked == true)
                    MessageBox.Show(SystemBase.Base.MessageRtn("M0011"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //SPR일 경우 다른 작업을 할 수 없습니다.
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn("M0012"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //긴급일 경우 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false && rdoSpr.Checked == false && (rdoP.Checked == false || NewFlg == 1))
            {
                UIForm.FPMake.RowCopy(fpSpread1);
            }
            else
            {
                if (rdoCfm_Y.Checked == true)
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
                else if (rdoSpr.Checked == true)
                    MessageBox.Show(SystemBase.Base.MessageRtn("M0011"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //SPR일 경우 다른 작업을 할 수 없습니다.
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn("M0012"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //긴급일 경우 다른 작업을 할 수 없습니다.

            }
        }
        #endregion

        #region DelExec 행 삭제
        protected override void DelExec()
        {	// 행 삭제

            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false && rdoSpr.Checked == false && (rdoP.Checked == false || NewFlg == 1))
            {
                UIForm.FPMake.RowRemove(fpSpread1);
            }
            else
            {
                if (rdoCfm_Y.Checked == true)
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
                else if (rdoSpr.Checked == true)
                    MessageBox.Show(SystemBase.Base.MessageRtn("M0011"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //SPR일 경우 다른 작업을 할 수 없습니다.
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn("M0012"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //긴급일 경우 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSReqDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpSReqDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

                rdoPartM.Checked = true;
                rdoTypeAll.Checked = true;
                rdoCfmAll.Checked = true;
            }

            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpReqDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            rdoMpr.Checked = true;
            rdoManual.Checked = true;
            rdoCfm_N.Checked = true;

            panel04.Enabled = false;
            panel3.Enabled = false;
            panel2.Enabled = false;

            NewFlg = 1;
            strAutoReqNo = "";
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (rdoCfm_Y.Checked == true)
            {
                MessageBox.Show("확정된 데이타는 삭제할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (rdoSpr.Checked == true)
            {
                MessageBox.Show("요청구분이 외주가공인 데이타는 삭제할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (rdoP.Checked == true)
            {
                MessageBox.Show("요청유형이 긴급인 데이타는 삭제할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_MRQ001  'D1'";
                    strSql += ", @pREQ_NO = '" + strAutoReqNo + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Search("");
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
        #endregion

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            Search("");
        }

        private void Search(string strReqNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strCfmYn = "";
                if (rdoCfmYes.Checked == true) strCfmYn = "Y";
                else if (rdoCfmNo.Checked == true) strCfmYn = "N";

                string strReqPart = "";
                if (rdoPartM.Checked == true) strReqPart = "M";
                else if (rdoPartS.Checked == true) strReqPart = "S";

                string strReqType = "";
                if (rdoTypeM.Checked == true) strReqType = "M";
                else if (rdoTypeE.Checked == true) strReqType = "E";
                else if (rdoTypeP.Checked == true) strReqType = "P";

                string strQuery = " usp_MRQ001  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pREQ_DT_FR = '" + dtpSReqDtFr.Text + "' ";
                strQuery += ", @pREQ_DT_TO = '" + dtpSReqDtTo.Text + "' ";
                strQuery += ", @pREQ_PART = '" + strReqPart + "' ";
                strQuery += ", @pREQ_TYPE = '" + strReqType + "' ";
                strQuery += ", @pREQ_ID = '" + txtSUserId.Text.Trim() + "' ";
                strQuery += ", @pREQ_DEPT_CD = '" + txtSDeptCd.Text.Trim() + "' ";
                strQuery += ", @pREQ_REORG_ID = '" + txtSReorgId.Text.Trim() + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text.Trim() + "' ";
                strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                strQuery += ", @pREQ_NO = '" + txtSReqNo.Text.Trim() + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (strReqNo != "")
                    {
                        fpSpread2.Search(0, strReqNo, false, false, false, false, 0, 0, ref x, ref y);

                        if (x > 0)
                        {
                            fpSpread2.Sheets[0].SetActiveCell(x, y);
                        }
                        else
                        {
                            x = 0;
                        }

                    }
                    fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    strAutoReqNo = fpSpread2.Sheets[0].Cells[x, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text;
                    NewFlg = 2;

                    //상세정보조회
                    SubSearch(strAutoReqNo);
                }
                else
                {
                    NewFlg = 1;
                    strAutoReqNo = "";
                    btnNew_is = false;
                    NewExec();
                    btnNew_is = true;
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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            txtUserId.Focus();

            string strMstType = "";
            DialogResult dsMsg;
            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false)
            {
                //상단 그룹박스 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        this.Cursor = Cursors.WaitCursor;

                        string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            if (NewFlg != 0)
                            {
                                string chkPart = "";
                                if (rdoMpr.Checked == true) { chkPart = "M"; }
                                else { chkPart = "S"; }

                                string chkType = "";
                                if (rdoMrp.Checked == true) { chkType = "M"; }
                                else if (rdoManual.Checked == true) { chkType = "E"; }
                                else chkType = "P";

                                string chkCfm = "";
                                if (rdoCfm_Y.Checked == true) { chkCfm = "Y"; }
                                else { chkCfm = "N"; }

                                if (NewFlg == 1) { strMstType = "I1"; }
                                else { strMstType = "U1"; }

                                string strSql = " usp_MRQ001 '" + strMstType + "'";
                                strSql += ", @pREQ_NO = '" + txtReqNo.Text + "' ";
                                strSql += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
                                //								strSql += ", @pPUR_ORG = '"+ txtPurOrgCd.Text +"' ";
                                strSql += ", @pREQ_DT = '" + dtpReqDt.Text + "' ";
                                strSql += ", @pREQ_PART = '" + chkPart + "' ";
                                strSql += ", @pREQ_TYPE = '" + chkType + "' ";
                                strSql += ", @pREQ_ID = '" + txtUserId.Text + "' ";
                                strSql += ", @pREQ_DEPT_CD = '" + txtReqDeptCd.Text + "' ";
                                strSql += ", @pREQ_REORG_ID = '" + txtReqReorgId.Text + "' ";
                                strSql += ", @pCONFIRM_YN = '" + chkCfm + "' ";
                                strSql += ", @pREMARK1 = '" + txtRemark.Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();
                                strAutoReqNo = dt.Rows[0][2].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }

                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크                                
                            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true))// 그리드 필수항목 체크 
                            {

                                //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                                if (DelCheck() == false)
                                {
                                    string msg = SystemBase.Base.MessageRtn("B0027");
                                    dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (dsMsg == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            string strDelSql = " usp_MRQ001  'D1'";
                                            strDelSql += ", @pREQ_NO = '" + strAutoReqNo + "' ";
                                            strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit1; }	// ER 코드 Return시 점프

                                            Trans.Commit();
                                        }
                                        catch (Exception f)
                                        {
                                            SystemBase.Loggers.Log(this.Name, f.ToString());
                                            Trans.Rollback();
                                            ERRCode = "ER";
                                            MSGCode = f.Message;
                                            //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                        }
                                    Exit1:
                                        dbConn.Close();
                                        if (ERRCode == "OK")
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Search("");
                                        }
                                        else if (ERRCode == "ER")
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        else
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }

                                        return;
                                    }
                                    else
                                    {
                                        Trans.Rollback();
                                        dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        //작업이 취소되었습니다.	
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }
                                }

                                //행수만큼 처리
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                    string strGbn = "";

                                    if (strHead.Length > 0)
                                    {
                                        switch (strHead)
                                        {
                                            case "U": strGbn = "U2"; break;
                                            case "I": strGbn = "I2"; break;
                                            case "D": strGbn = "D2"; break;
                                            default: strGbn = ""; break;
                                        }
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text.Trim() == "")
                                        {
                                            ERRCode = "WR";
                                            MSGCode = "제품코드를 입력하세요!";
                                            Trans.Rollback(); goto Exit;
                                        }

                                        string strQuery = " usp_M_COMMON 'P006' , @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text
                                            + "' , @pSPEC2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text
                                            + "' , @pCODE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";


                                        DataTable dt_item = SystemBase.DbOpen.NoTranDataTable(strQuery);
                                        if (dt_item.Rows.Count <= 0)
                                        {
                                            ERRCode = "WR";
                                            MSGCode = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "프로젝트번호에 "
                                                + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "차수에 "
                                                + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + " 제품이 없습니다! ";
                                            Trans.Rollback(); goto Exit;
                                        }

                                        string strSql = " usp_MRQ001 '" + strGbn + "'";
                                        strSql += ", @pREQ_NO = '" + strAutoReqNo + "' ";

                                        if (strGbn == "I2") strSql += ", @pREQ_SEQ = 0 ";
                                        else strSql += ", @pREQ_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Value;

                                        strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                        strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
                                        strSql += ", @pMAKE_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + "' ";
                                        strSql += ", @pREQ_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
                                        strSql += ", @pREQ_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value + "' ";
                                        strSql += ", @pITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text + "' ";
                                        strSql += ", @pREQ_SL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text + "' ";
                                        strSql += ", @pREQ_LOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text + "' ";
                                        strSql += ", @pDELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text + "' ";
                                        strSql += ", @pSTD_ITEM_YN = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "표준품목여부")].Text + "' ";    // 2017.03.21. hma 추가

                                        strSql += ", @pREMARK2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                    }
                                }
                            }
                            else
                            {
                                Trans.Rollback();
                                this.Cursor = Cursors.Default;
                                return;
                            }
                            Trans.Commit();
                        }
                        catch (Exception e)
                        {
                            SystemBase.Loggers.Log(this.Name, e.ToString());
                            Trans.Rollback();
                            ERRCode = "ER";
                            MSGCode = e.Message;
                            //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                        }
                    Exit:
                        dbConn.Close();
                        if (MSGCode != "")
                        {
                            if (ERRCode == "OK")
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                                if (NewFlg == 1) Search(strAutoReqNo);
                                else SubSearch(strAutoReqNo);
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

                        this.Cursor = Cursors.Default;
                    }
                    else
                    {
                        dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //최소 한건 이상의 DETAIL정보가 존재하지 않으면 등록할 수 없습니다.
                    }
                }
            }
            else
            {
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            strBtn = "Y";
            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    WNDW005 pu = new WNDW005(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, "30");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text = Msgs[16].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs[17].ToString();

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                            = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                            = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        string Query = "Select ISNULL(ORDER_PUR_UNIT,''), ISNULL(ORDER_PUR_LT,0), ISNULL(INS_PUR_LT,0), TRACKING_FLAG, ISNULL(ORDER_MFG_LT,0) ";
                        Query += " From B_PLANT_ITEM_INFO(Nolock) Where  ITEM_CD  = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                        Query += " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Rows[0][0].ToString().Trim() == "") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = Msgs[8].ToString();
                            else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = dt.Rows[0][0].ToString();

                            int iLT = Convert.ToInt16(dt.Rows[0][1].ToString()) + Convert.ToInt16(dt.Rows[0][2].ToString());
                            int iLT2 = iLT + Convert.ToInt16(dt.Rows[0][4].ToString());
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text == "" && iLT != 0)
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text = Convert.ToDateTime(dtpReqDt.Value).AddDays(Convert.ToDouble(iLT)).ToShortDateString();
                           
                            string tracking_flag = dt.Rows[0][3].ToString();
                            if (tracking_flag == "N")
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "*";
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "*";
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계약명")].Text = "";
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "정산관계")].Text = "";
                                UIForm.FPMake.grdReMake(fpSpread1, Row,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3");

                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, Row,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|0");
                            }

                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text = dtpReqDt.Text;
                        }

                        // 2017.03.21. hma 추가: 표준품목여부 체크
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "표준품목여부")].Text =
                            StdItemCheck(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text);

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //단위
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2"))
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'Z005' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00029", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "단위팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = Msgs[0].ToString();
                    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                }
            }
            //창고
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //위치
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00030", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고위치팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //프로젝트번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
            {
                try
                {
                    WNDW007 pu = new WNDW007(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, "N");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계약명")].Text = Msgs[8].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "정산관계")].Text = Msgs[10].ToString();
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }

                    // 2017.03.21. hma 추가: 표준품목여부 체크
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "표준품목여부")].Text =
                        StdItemCheck(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //프로젝트번호차수
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'PROJ_SEQ' , @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";	// 쿼리
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                    string[] strSearch = new string[] { "", "" };
                    
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                    pu.Width = 400;
                    pu.ShowDialog();	//공통 팝업 호출

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string MSG = pu.ReturnVal.Replace("|", "#");
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(MSG);

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text = "";

                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //제품코드
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드_2"))
            {
                try
                {
                    string strQuery = " usp_M_COMMON 'P003' , @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text
                        + "' , @pSPEC2 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";	// 쿼리
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text, "" };
                    
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00106", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "제품 조회", false);
                    pu.Width = 400;
                    pu.ShowDialog();	//공통 팝업 호출

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string MSG = pu.ReturnVal.Replace("|", "#");
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(MSG);

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text = Msgs[3].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙"))
            {
                try
                {
                    if (txtReqNo.Text != "" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text.ToString() != "")
                    {
                        string strCfmYn = "";
                        if (rdoCfm_Y.Checked == true) strCfmYn = "Y";
                        else if (rdoCfm_N.Checked == true) strCfmYn = "N";

                        WNDW031 pu = new WNDW031("RM", 
                                                 txtReqNo.Text, 
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text.ToString(),
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text,
                                                 strCfmYn);

                        pu.ShowDialog();

                        if (strCfmYn == "N")
                        {
                            string strSql = " usp_MRQ001  'P1' ";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql = strSql + ", @pREQ_NO ='" + txtReqNo.Text + "' ";
                            strSql = strSql + ", @pREQ_SEQ ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text.ToString() + "' ";


                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                            if (dt.Rows.Count != 0)
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙문서")].Value = dt.Rows[0]["Q_REQ_DOC_NM"].ToString();
                            }
                        }

                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            strBtn = "N";
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                if (strBtn == "N")
                {
                    //품목코드
                    if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
                    {
                        string Query = " usp_M_COMMON @pTYPE = 'M012', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', @pNAME = '" + cboPlantCd.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text = dt.Rows[0]["RCPT_SL_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = dt.Rows[0]["RCPT_LOCATION_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                                = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                                = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "표준품목여부")].Text = dt.Rows[0]["STD_ITEM_YN"].ToString();     // 2017.03.21. hma 추가: 표준품목여부

                            string unit = dt.Rows[0]["ITEM_UNIT"].ToString();
                            string Query1 = "Select ISNULL(ORDER_PUR_UNIT,''), ISNULL(ORDER_PUR_LT,0), ISNULL(INS_PUR_LT,0), TRACKING_FLAG ";
                            Query1 += " From B_PLANT_ITEM_INFO(Nolock) Where  ITEM_CD  = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                            Query1 += " AND PLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);

                            if (dt1.Rows.Count > 0)
                            {
                                if (dt1.Rows[0][0].ToString().Trim() == "") fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = unit;
                                else fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = dt1.Rows[0][0].ToString();
                                int iLT = Convert.ToInt16(dt1.Rows[0][1].ToString()) + Convert.ToInt16(dt1.Rows[0][2].ToString());
                                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text == "" && iLT != 0)
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text = Convert.ToDateTime(dtpReqDt.Value).AddDays(Convert.ToDouble(iLT)).ToShortDateString();
                                string tracking_flag = dt1.Rows[0][3].ToString();
                                if (tracking_flag == "N")
                                {
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "*";
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "*";
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계약명")].Text = "";
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "정산관계")].Text = "";
                                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3");

                                }
                                else
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|0");
                                }

                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text = dtpReqDt.Text;
                            }
                            // 2017.03.21. hma 추가: 표준품목여부 체크
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "표준품목여부")].Text = 
                                StdItemCheck(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text);
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text = "";
                        }
                    }
                    // 창고 
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                            = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text, " AND PLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    //위치
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text.Trim() == "")
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "";
                        else
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                                = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                    }
                    // 프로젝트번호
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "*")
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text = "";
                        }

                        // 2017.03.21. hma 추가: 표준품목여부 체크
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "표준품목여부")].Text =
                            StdItemCheck(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text);
                                               
                    }
                    // 프로젝트차수
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "*"
                            || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "")
                        {
                            string seq = SystemBase.Base.CodeName("PROJECT_NO", "MAX(PROJECT_SEQ)", "S_SO_DETAIL", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, " AND PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                            if (seq == "")
                            {	//"프로젝트차수가 잘못 입력되었습니다!"
                                MessageBox.Show(SystemBase.Base.MessageRtn("B0054"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = seq;
                            }
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text = "";
                        }
                    }
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드"))
                    {
                        string strQuery2 = " usp_M_COMMON 'P003' , @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text
                            + "' , @pSPEC2 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text
                            + "' , @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                        DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(strQuery2);
                        if (dt2.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = dt2.Rows[0]["ITEM_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text = dt2.Rows[0]["DELIVERY_DT"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("해당 프로젝트번호, 프로젝트차수에 [" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + "]제품코드가 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제품명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text = "";

                        }


                    }
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
        private void txtSUserId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSUserId.Text != "")
                    {
                        txtSUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSUserNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtSDeptCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    string Query = " usp_B_COMMON 'D021' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        txtSReorgId.Text = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        txtSReorgId.Text = "";
                    }
                    if (txtSDeptCd.Text != "")
                    {
                        txtSDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtSDeptCd.Text, " And REORG_ID = '" + txtSReorgId.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSDeptNm.Value = "";
                    }
                }                
            }
            catch
            {

            }
        }

        private void txtUserId_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
            {
                txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                txtReqDeptCd.Value = SystemBase.Base.CodeName("USR_ID", "DEPT_CD", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                txtReqReorgId.Value = SystemBase.Base.CodeName("USR_ID", "REORG_ID", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                txtReqDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtReqDeptCd.Text, " AND REORG_ID='" + txtReqReorgId.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
        }

        private void txtPurOrgCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurOrgCd.Text != "")
                {
                    txtPurOrgNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPurOrgCd.Text, " AND MAJOR_CD = 'M001' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurOrgNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProjNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtProjSeq.Text = "";
        }

        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    strAutoReqNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text.ToString();

                    SubSearch(strAutoReqNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.		
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;
            strBtn = "Y";
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_MRQ001  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pREQ_NO ='" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                //요청구분
                if (dt.Rows[0]["REQ_PART"].ToString() != "")
                {
                    if (dt.Rows[0]["REQ_PART"].ToString() == "M") { rdoMpr.Checked = true; }
                    else { rdoSpr.Checked = true; }
                }
                else { rdoSpr.Checked = true; }

                //요청유형
                if (dt.Rows[0]["REQ_TYPE"].ToString() != "")
                {
                    if (dt.Rows[0]["REQ_TYPE"].ToString() == "M") { rdoMrp.Checked = true; }
                    else if (dt.Rows[0]["REQ_TYPE"].ToString() == "E") { rdoManual.Checked = true; }
                    else rdoP.Checked = true;
                }
                else { rdoManual.Checked = true; }

                //확정여부
                if (dt.Rows[0]["CONFIRM_YN"].ToString() != "")
                {
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y") { rdoCfm_Y.Checked = true; }
                    else { rdoCfm_N.Checked = true; }
                }
                else { rdoCfm_N.Checked = true; }

                dtpReqDt.Enabled = true;
                dtpReqDt.ReadOnly = false;

                txtReqNo.Value = dt.Rows[0]["REQ_NO"].ToString();
                cboPlantCd.SelectedValue = dt.Rows[0]["PLANT_CD"].ToString();
                dtpReqDt.Value = dt.Rows[0]["REQ_DT"].ToString();
                txtUserId.Value = dt.Rows[0]["REQ_ID"].ToString();
                txtUserNm.Value = dt.Rows[0]["USR_NM"].ToString();
                txtReqDeptCd.Value = dt.Rows[0]["REQ_DEPT_CD"].ToString();
                txtReqReorgId.Value = dt.Rows[0]["REQ_REORG_ID"].ToString();
                txtReqDeptNm.Value = dt.Rows[0]["DEPT_NM"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                //Detail그리드 정보.
                string strSql1 = " usp_MRQ001  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pREQ_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                //확정여부에 따른 화면 Locking
                if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y" || dt.Rows[0]["REQ_PART"].ToString() == "S" || dt.Rows[0]["REQ_TYPE"].ToString() == "P")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

                    //Detail Locking설정
                    UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                        );

                }
                else
                {
                    SystemBase.Base.GroupBoxLock(groupBox2, false);
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전용여부")].Text == "N") //프로젝트, 차수 realonly
                            //Detail Locking해제
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                );
                        else
                            //Detail Locking해제
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청창고_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                );
                    }



                }
                panel2.Enabled = false;
                panel3.Enabled = false;
                panel04.Enabled = false;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 삭제Row Count 체크
        private bool DelCheck()
        {
            bool delChk = true;
            int delCount = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                {
                    delCount++;
                }
            }

            if (delCount == fpSpread1.Sheets[0].Rows.Count)
            { delChk = false; }

            return delChk;
        }
        #endregion

        #region 버튼 Click
        private void butSUser_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSUserId.Text = Msgs[0].ToString();
                    txtSUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";

        }

        private void butSDept_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'D011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSDeptCd.Text = Msgs[0].ToString();
                    txtSDeptNm.Value = Msgs[1].ToString();
                    txtSReorgId.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnUser_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                    txtReqDeptCd.Value = Msgs[2].ToString();
                    txtReqDeptNm.Value = Msgs[3].ToString();
                    txtReqReorgId.Value = Msgs[4].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnPurOrgd_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='M001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurOrgCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매조직 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurOrgCd.Text = Msgs[0].ToString();
                    txtPurOrgNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjNo.Text = Msgs[3].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtProjSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        #endregion

        #region MRQ001_Activated
        private void MRQ001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSReqDtFr.Focus();
        }

        private void MRQ001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region 2017.03.21. hma 추가: 표준품목여부 체크
        private string StdItemCheck(string strItemCd, string ProjectNo)
        {
            string strStdItemYN = "N";
            try
            {
                string strQuery = "SELECT ISNULL(STD_ITEM_YN,'N') STD_ITEM_YN FROM S_SO_MASTER(NOLOCK) ";
                strQuery += " WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                strQuery += " AND PROJECT_NO = '" + ProjectNo + "' ";
                DataTable dt_s = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if ((dt_s.Rows.Count > 0) && (dt_s.Rows[0]["STD_ITEM_YN"].ToString() == "Y"))
                {
                    strQuery = "SELECT ISNULL(STD_ITEM_YN,'N') STD_ITEM_YN FROM B_ITEM_INFO(NOLOCK) ";
                    strQuery += " WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += " AND ITEM_CD = '" + strItemCd + "' ";
                    DataTable dt_i = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if ((dt_i.Rows.Count > 0) && (dt_i.Rows[0]["STD_ITEM_YN"].ToString() == "Y"))
                    {
                        strStdItemYN = "Y";
                    }
                }
                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            return strStdItemYN;
        }
        #endregion

    }
}