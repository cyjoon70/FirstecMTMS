#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정의뢰등록/출력
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-14
// 작성내용 : 외주공정의뢰등록/출력 및 관리
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
using WNDW;

namespace PC.PCC009
{
    public partial class PCC009 : UIForm.FPCOMM2
    {
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoReqNo = ""; //요청번호
        string strBtn = "N";

        public PCC009()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PCC009_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox2);//필수체크

            //GroupBox2 입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE = 'PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            dtpSReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpSReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpReqDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            rdoSpr.Checked = true;
            rdoManual.Checked = true;
            rdoCfm_N.Checked = true;

            rdoMpr.BackColor = Color.Gainsboro;
            rdoMpr.Enabled = false;

            rdoSpr.BackColor = Color.Gainsboro;
            rdoSpr.Enabled = false;

            rdoMrp.BackColor = Color.Gainsboro;
            rdoMrp.Enabled = false;

            rdoManual.BackColor = Color.Gainsboro;
            rdoManual.Enabled = false;

            rdoCfm_Y.BackColor = Color.Gainsboro;
            rdoCfm_Y.Enabled = false;

            rdoCfm_N.BackColor = Color.Gainsboro;
            rdoCfm_N.Enabled = false;

            NewFlg = 1;
            strAutoReqNo = "";

            btnPrint1.Enabled = false;
            btnSpr.Enabled = true;
            btnCfm.Enabled = false;
            btnCfmCancel.Enabled = false;
            rdoItemCd.Checked = true;

        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false)
            {
                UIForm.FPMake.RowInsert(fpSpread1);
            }
            else
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false)
            {
                UIForm.FPMake.RowCopy(fpSpread1);
            }
            else
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpSReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpSReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpReqDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            rdoSpr.Checked = true;
            rdoManual.Checked = true;
            rdoCfm_N.Checked = true;

            rdoMpr.BackColor = Color.Gainsboro;
            rdoMpr.Enabled = false;

            rdoSpr.BackColor = Color.Gainsboro;
            rdoSpr.Enabled = false;

            rdoMrp.BackColor = Color.Gainsboro;
            rdoMrp.Enabled = false;

            rdoManual.BackColor = Color.Gainsboro;
            rdoManual.Enabled = false;

            rdoCfm_Y.BackColor = Color.Gainsboro;
            rdoCfm_Y.Enabled = false;

            rdoCfm_N.BackColor = Color.Gainsboro;
            rdoCfm_N.Enabled = false;

            NewFlg = 1;
            strAutoReqNo = "";

            btnPrint1.Enabled = false;
            btnSpr.Enabled = true;
            btnCfm.Enabled = false;
            btnCfmCancel.Enabled = false;
            rdoItemCd.Checked = true;

        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (rdoCfm_Y.Checked == true)
            {
                DialogResult dsMsg1 = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    string strSql = " usp_PCC009  'D1'";
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
                    MSGCode = "P0001";
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    Search("", false);
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Search("", true);
        }

        private void Search(string strReqNo, bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strCfmYn = "";
                if (rdoCfmYes.Checked == true) { strCfmYn = "Y"; }
                else if (rdoCfmNo.Checked == true) { strCfmYn = "N"; }

                string strReqPart = "S";

                string strReqType = "";
                if (rdoTypeM.Checked == true) { strReqType = "M"; }
                else if (rdoTypeE.Checked == true) { strReqType = "E"; }

                string strQuery = " usp_PCC009  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pREQ_DT_FR = '" + dtpSReqDtFr.Text + "' ";
                strQuery += ", @pREQ_DT_TO = '" + dtpSReqDtTo.Text + "' ";
                strQuery += ", @pREQ_PART = '" + strReqPart + "' ";
                strQuery += ", @pREQ_TYPE = '" + strReqType + "' ";
                strQuery += ", @pREQ_ID = '" + txtSUserId.Text + "' ";
                strQuery += ", @pREQ_DEPT_CD = '" + txtSDeptCd.Text + "' ";
                strQuery += ", @pREQ_REORG_ID = '" + txtSReorgId.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text + "' ";
                strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                strQuery += ", @pWORKORDER_NO = '" + txtWorkorderNo.Text + "' ";
                strQuery += ", @pREQ_NO = '" + txtSReqNo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, Msg, 0, 0, true);

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
                    strAutoReqNo = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text;
                    NewFlg = 2;

                    //상세정보조회
                    SubSearch(strAutoReqNo);
                }
                else
                {
                    NewFlg = 1;
                    strAutoReqNo = "";
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
                                else { chkType = "E"; }

                                string chkCfm = "";
                                if (rdoCfm_Y.Checked == true) { chkCfm = "Y"; }
                                else { chkCfm = "N"; }

                                if (NewFlg == 1) { strMstType = "I1"; }
                                else { strMstType = "U1"; }

                                string strSql = " usp_PCC009 '" + strMstType + "'";
                                strSql += ", @pREQ_NO = '" + strAutoReqNo + "' ";
                                strSql += ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
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
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
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
                                            string strDelSql = " usp_PCC009  'D1'";
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
                                            MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                        }
                                    Exit1:
                                        dbConn.Close();
                                        if (ERRCode == "OK")
                                        {
                                            Search("", false);
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                                        string strSql = " usp_PCC009 '" + strGbn + "'";
                                        strSql += ", @pREQ_NO = '" + strAutoReqNo + "' ";

                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value.ToString() == "0")
                                        {
                                            MessageBox.Show("수량이 0 입니다. 확인해주시기 바랍니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            fpSpread1.Sheets[0].AddSelection(i, 1, 1, 1);
                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            return;
                                        }

                                        string strQcCheck = "N";//품질확인
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질확인")].Text == "True") { strQcCheck = "Y"; }


                                        if (strGbn == "I2") strSql += ", @pREQ_SEQ = 0 ";
                                        else strSql += ", @pREQ_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Value;
                                        strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                        strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                                        strSql += ", @pREQ_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value + "' ";
                                        strSql += ", @pWC_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장코드")].Text + "' ";
                                        strSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                                        strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "' ";
                                        strSql += ", @pJOB_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text + "' ";
                                        strSql += ", @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text + "' ";
                                        strSql += ", @pBP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text + "' ";
                                        strSql += ", @pSUBCONTRACT_PRC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value + "' ";
                                        strSql += ", @pSUBCONTRACT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value + "' ";
                                        strSql += ", @pCUR_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통화")].Value + "' ";
                                        strSql += ", @pTAX_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value + "' ";
                                        strSql += ", @pROUT_DOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서")].Text + "' ";
                                        strSql += ", @pROUT_SIZE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격")].Text + "' ";
                                        strSql += ", @pDELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "완료예정일")].Text + "' ";
                                        strSql += ", @pQC_CHECK = '" + strQcCheck + "' ";

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
                                if (NewFlg == 1) Search(strAutoReqNo, false);
                                else SubSearch(strAutoReqNo);

                                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #region TextChanged
        private void txtSUserId_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtSUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSUserId.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtSDeptCd_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    string Query = " usp_B_COMMON 'D021' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
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
                        txtSDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtSDeptCd.Text, " And REORG_ID = '" + txtSReorgId.Text + "' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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
                txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                txtReqDeptCd.Value = SystemBase.Base.CodeName("USR_ID", "DEPT_CD", "B_SYS_USER", txtUserId.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                txtReqReorgId.Value = SystemBase.Base.CodeName("USR_ID", "REORG_ID", "B_SYS_USER", txtUserId.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                txtReqDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtReqDeptCd.Text, " AND REORG_ID='" + txtReqReorgId.Text + "' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            }
        }

        private void txtPurOrgCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurOrgCd.Text != "")
                {
                    txtPurOrgNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPurOrgCd.Text, " AND MAJOR_CD = 'M001' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
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
            try
            {
                if (strBtn == "N")
                {
                    if (txtProjNo.Text != "")
                    {
                        txtProjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND SO_CONFIRM_YN = 'Y'  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                    }
                    else
                    {
                        txtProjNm.Value = "";
                    }
                }                
            }
            catch
            {

            }
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
                string strSql = " usp_PCC009  'S2' ";
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
                    else { rdoManual.Checked = true; }
                }
                else { rdoManual.Checked = true; }

                txtReqNo.Value = dt.Rows[0]["REQ_NO"].ToString();
                cboPlantCd.SelectedValue = dt.Rows[0]["PLANT_CD"].ToString();
                dtpReqDt.Value = dt.Rows[0]["REQ_DT"].ToString().Substring(0,10);
                txtUserId.Value = dt.Rows[0]["REQ_ID"].ToString();
                txtUserNm.Value = dt.Rows[0]["USR_NM"].ToString();
                txtReqDeptCd.Value = dt.Rows[0]["REQ_DEPT_CD"].ToString();
                txtReqReorgId.Value = dt.Rows[0]["REQ_REORG_ID"].ToString();
                txtReqDeptNm.Value = dt.Rows[0]["DEPT_NM"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                //Detail그리드 정보.
                string strSql1 = " usp_PCC009  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pREQ_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    //확정여부에 따른 화면 Locking
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y")
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                        rdoWorkOrder.Enabled = true;
                        rdoItemCd.Enabled = true;
                        panel9.Enabled = true;


                        //Detail Locking설정
                        UIForm.FPMake.grdReMake(fpSpread1,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "완료예정일") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품질확인") + "|3"
                            );

                    }
                    else
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

                        //Detail Locking해제
                        UIForm.FPMake.grdReMake(fpSpread1,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "통화") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|0"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "완료예정일") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품질확인") + "|0"
                            );

                    }
                }

                //확정여부
                if (dt.Rows[0]["CONFIRM_YN"].ToString() != "")
                {
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y")
                    {
                        rdoCfm_Y.Checked = true;
                        btnCfm.Enabled = false;
                        btnCfmCancel.Enabled = true;
                        btnPrint1.Enabled = true;
                    }
                    else
                    {
                        rdoCfm_N.Checked = true;
                        btnCfm.Enabled = true;
                        btnCfmCancel.Enabled = false;
                        btnPrint1.Enabled = false;
                    }
                }
                else { rdoCfm_N.Checked = true; }

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

                    txtSDeptCd.Value = Msgs[0].ToString();
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
                string strQuery = " usp_M_COMMON 'P001' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtProjNo.Text, "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00074", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트 조회", false);
                pu.Width = 500;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtProjNo.Text = Msgs[0].ToString();
                    txtProjNm.Value = Msgs[1].ToString();
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

        //제조오더번호
        private void btnWorkorderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNo.Text = Msgs[1].ToString();
                    txtWorkorderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 외주공정참조
        private void btnSpr_Click(object sender, System.EventArgs e)
        {
            try
            {
                PCC009P1 frm = new PCC009P1(fpSpread1);
                frm.ShowDialog();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 수량, 단가 입력시 금액 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처명")].Text
                    = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수량"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value
                    = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value)
                    * Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value);
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주금액")].Value
                    = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value)
                    * Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주단가")].Value);
            }
        }
        #endregion

        #region 그리드 상 팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드_2"))
            {
                try
                {
                    WNDW002 pu = new WNDW002(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text, "");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처코드")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "외주처명")].Text = Msgs[2].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙"))
            {
                try
                {
                    if (txtReqNo.Text != "" && fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString() != "")
                    {
                        string strCfmYn = "";
                        if (rdoCfm_Y.Checked == true) strCfmYn = "Y";
                        else if (rdoCfm_N.Checked == true) strCfmYn = "N";

                        WNDW031 pu = new WNDW031("RP", 
                                                 txtReqNo.Text,
                                                 fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString(),
                                                 fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                                 fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text,
                                                 fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text,
                                                 fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text,
                                                 fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text,
                                                 strCfmYn);

                        pu.ShowDialog();

                        string strSql = " usp_PCC009  'P2' ";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pREQ_NO ='" + txtReqNo.Text + "' ";
                        strSql = strSql + ", @pREQ_SEQ ='" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text.ToString() + "' ";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                        if (dt.Rows.Count != 0)
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙문서")].Value = dt.Rows[0]["Q_REQ_DOC_NM"].ToString();
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
        }
        #endregion

        #region 확정 /취소
        private void btnConfirm_Click(object sender, System.EventArgs e)
        {
            Confirm("Y");
        }

        private void btnCfmCancel_Click(object sender, System.EventArgs e)
        {
            Confirm("N");
        }

        private void Confirm(string strCfmYn)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_PCC009  'P1'";
                    strSql += ", @pREQ_NO = '" + txtReqNo.Text + "'";
                    strSql += ", @pCONFIRM_YN = '" + strCfmYn + "'";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "'";
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
                    MSGCode = "P0001";
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //상세조회
                    SubSearch(txtReqNo.Text);
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

        #region 레포트 출력
        private void btnPrint1_Click(object sender, System.EventArgs e)
        {
            if (txtReqNo.Text != "")
            {
                //조회 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = "";
                    //string[] FormulaField = new string[2];	  //formula 값			
                    if (rdoWorkOrder.Checked == true)
                    {
                        //제조오더별
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\PCC009.rpt";    // 레포트경로+레포트명
                    }
                    else
                    {
                        //품목코드별
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\PCC009_1.rpt";    // 레포트경로+레포트명
                    }
                    string[] RptParmValue = new string[4];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrLangCd;
                    RptParmValue[2] = txtReqNo.Text;
                    RptParmValue[3] = SystemBase.Base.gstrCOMCD;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
                    frm.ShowDialog();
                }
            }
        }
        #endregion		

   
	
    }
}