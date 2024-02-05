#region 작성정보
/*********************************************************************/
// 단위업무명 : 지급내역등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 지급내역등록 및 관리
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

namespace MV.MIV002
{
    public partial class MIV002 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strTEMP_SLIP_NO, strSLIP_NO;
        string strAutoIvNo = ""; //매입번호
        string strBtn = "N";
        bool btnNew_is = true;
        bool form_act_chk = false;
        string strLinkSlipNo = "";     // 2022.01.17. hma 추가: 링크전표번호
        #endregion

        #region 생성자
        public MIV002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MIV002_Load(object sender, System.EventArgs e)
        {
            //필수 항목 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            //입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //화폐단위
            SystemBase.ComboMake.C1Combo(cboCSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 2022.01.17. hma 추가: 그룹웨어상태
            SystemBase.ComboMake.C1Combo(cboMSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 2022.01.17. hma 추가: 그룹웨어상태

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //조회조건
            dtpSIvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpSIvDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoSCfm_All.Checked = true;

            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;
            btnRef.Enabled = false;

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";

            lnkJump1.Text = "확정전표상신";         // 2022.02.12. hma 추가: 화면에 보여지는 링크명
            strJumpFileName1 = "AD.ACD001.ACD001";  // 2022.02.12. hma 추가: 호출할 화면명
            lnkJump2.Text = "반제전표상신";         // 2022.02.12. hma 추가: 화면에 보여지는 링크명
            strJumpFileName2 = "AD.ACD001.ACD001";  // 2022.02.12. hma 추가: 호출할 화면명
            strLinkSlipNo = "";                     // 2022.02.12. hma 추가

            cboCSlipGwStatus.Text = "";      // 2022.02.12. hma 추가: 결재상태 초기화
            cboMSlipGwStatus.Text = "";      // 2022.02.12. hma 추가: 결재상태 초기화
            btnMinusCancel.Enabled = false;  // 2022.02.12. hma 추가: 반제취소 버튼 비활성화
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            strBtn = "Y";

            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSIvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpSIvDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

                rdoSCfm_All.Checked = true;
            }

            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;		

            strAutoIvNo = "";

            //확정버튼 Disable
            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;
            btnRef.Enabled = false;
            strBtn = "N";

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";

            cboCSlipGwStatus.Text = "";      // 2022.02.12. hma 추가: 결재상태 초기화
            cboMSlipGwStatus.Text = "";      // 2022.02.12. hma 추가: 결재상태 초기화
            btnMinusCancel.Enabled = false;  // 2022.02.12. hma 추가: 반제취소 버튼 비활성화
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false)
            {
                UIForm.FPMake.RowInsert(fpSpread1);

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지급액")].Value = 0;
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액")].Value = 0;
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

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strCfmYn = "";
                if (rdoSCfm_Y.Checked == true) { strCfmYn = "Y"; }
                else if (txtSCfm_N.Checked == true) { strCfmYn = "N"; }

                string strSaveYn = "";
                if (rdoSSave_Y.Checked == true) { strSaveYn = "Y"; }
                else if (rdoSSave_N.Checked == true) { strSaveYn = "N"; }


                string strQuery = " usp_MIV002  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pIV_DT_FR = '" + dtpSIvDtFr.Text + "' ";
                strQuery += ", @pIV_DT_TO = '" + dtpSIvDtTo.Text + "' ";
                strQuery += ", @pIV_TYPE = '" + txtSIvType.Text + "' ";
                strQuery += ", @pPUR_DUTY = '" + txtSPurDuty.Text + "' ";
                strQuery += ", @pCUST_CD = '" + txtSCustCd.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text + "' ";
                strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                strQuery += ", @pIV_NO = '" + txtSIvNo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pSAVE_YN = '" + strSaveYn + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    strAutoIvNo = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호")].Text;
                    //상세정보조회
                    SubSearch(strAutoIvNo);
                }
                else
                {
                    strAutoIvNo = "";
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
            txtIvNo.Focus();
            DialogResult dsMsg;
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false)
            {

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    int iPaymentSeq = 0;

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
                        {
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

                                    string strSql = " usp_MIV002 '" + strGbn + "'";
                                    strSql += ", @pIV_NO = '" + strAutoIvNo + "' ";

                                    if (strGbn == "I2")
                                    {
                                        iPaymentSeq = 0;
                                    }
                                    else
                                    {
                                        iPaymentSeq = Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급순번")].Value);
                                    }

                                    strSql += ", @pPAYMENT_SEQ= " + iPaymentSeq;
                                    strSql += ", @pPAYMENT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text + "' ";
                                    strSql += ", @pBANK_ACCT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text + "' ";
                                    strSql += ", @pNOTE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text + "' ";
                                    strSql += ", @pPRPAYM_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text + "' ";

                                    strSql += ", @pPAYMENT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급액")].Value + "' ";
                                    strSql += ", @pPAYMENT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액")].Value + "' ";

                                    strSql += ", @pREMARK  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
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
                            SubSearch(strAutoIvNo);
                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoIvNo, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호"));
                            UIForm.FPMake.GridSetFocus(fpSpread1, iPaymentSeq.ToString(), SystemBase.Base.GridHeadIndex(GHIdx1, "지급순번"));
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
            //지급유형
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형_2"))
            {
                try
                {
                    string strQuery = " usp_M_COMMON 'M060', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00011", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "지급유형조회");	//지급유형조회
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                        GridSet(Row);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            //계좌번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2"))
            {
                try
                {
                    MIV002P1 myForm = new MIV002P1(fpSpread1, Row);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            //어음번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2"))
            {
                try
                {
                    MIV002P2 myForm = new MIV002P2(fpSpread1, Row, txtBillCustCd.Text);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = Msgs[0].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            //선급금번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2"))
            {
                try
                {
                    MIV002P3 myForm = new MIV002P3(fpSpread1, Row, txtPaymentCustCd.Text, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = Msgs[0].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형명")].Text
                    = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text, " And MAJOR_CD = 'S012' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급금유형명")].Text != "")
                {
                    GridSet(Row);
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지급액"))
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급자국금액")].Value
                    = Convert.ToDouble(txtExchRate.Value) * Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급액")].Value);
            }

        }
        #endregion

        #region 그리드 필수, 일반, 읽기적용 세팅
        private void GridSet(int Row)
        {
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text == "DP")//계좌번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|3"
                    );
            }
            else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text == "NP")//어음번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|3"
                    );
            }
            else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형")].Text == "PP")//선급금번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|0"
                    );
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|3"
                    );
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
                    strAutoIvNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호")].Text.ToString();

                    SubSearch(strAutoIvNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.			
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;
            strBtn = "Y";
            bool bMinusBtn = false;      // 2022.02.16. hma 추가: 반제취소 버튼 활성화 여부

            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_MIV002  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pIV_NO = '" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                // 2022.01.21. hma 추가(Start): 결재상태 및 반제전표번호, 반제승인 추가
                txtCSlipNo.Value = dt.Rows[0]["CFM_SLIP_NO"].ToString();
                cboCSlipGwStatus.SelectedValue = dt.Rows[0]["CFM_GW_STATUS"].ToString();
                txtMinusConfirm.Value = dt.Rows[0]["MINUS_CONFIRM_YN"].ToString();
                txtMSlipNo.Value = dt.Rows[0]["MINUS_SLIP_NO"].ToString();
                cboMSlipGwStatus.SelectedValue = dt.Rows[0]["MINUS_GW_STATUS"].ToString();
                // 2022.01.21. hma 추가(End)

                //확정여부
                if (dt.Rows[0]["CONFIRM_YN"].ToString() != "")
                {
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y")
                    {
                        rdoCfm_Y.Checked = true;
                        btnConfirmOk.Enabled = false;

                        // 2022.01.21. hma 수정(Start): 확정상태이면서 결재상태가 상신대기/반려/승인 상태이면 확정취소 버튼 활성화되게.
                        //btnConfirmCancel.Enabled = true;
                        if ((txtSlipNo.Text != "" && txtCSlipNo.Text == "") ||
                            ((txtCSlipNo.Text != "") &&
                             (cboCSlipGwStatus.SelectedValue.ToString() == "READY" || cboCSlipGwStatus.SelectedValue.ToString() == "REJECT" ||
                              (cboCSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y"))))
                            btnConfirmCancel.Enabled = true;
                        else
                            btnConfirmCancel.Enabled = false;
                        // 2022.01.21. hma 수정(End)

                        btnRef.Enabled = true;      // 전표조회
                    }
                    else
                    {
                        rdoCfm_N.Checked = true;
                        // 2022.01.21. hma 수정(Start): 미확정상태인 경우 결재상태가 승인이면서 반제승인이 Y일때 확정 버튼 활성화.(반제처리 위해)
                        //btnConfirmOk.Enabled = true;
                        if ((txtMSlipNo.Text == "") ||
                            (txtMSlipNo.Text != "" &&
                             (cboMSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y")))
                            btnConfirmOk.Enabled = true;
                        else
                            btnConfirmOk.Enabled = false;
                        // 2022.01.21. hma 수정(End)

                        // 2022.01.21. hma 수정(Start): 미확정건이지만 반제전표가 생성된 경우에는 확정취소 버튼 비활성화 처리.
                        btnConfirmCancel.Enabled = false;   // 2022.02.12. hma 수정: 아래 부분 주석 처리하고 이 부분 주석 해제. 미확정 상태일때는 확정취소 버튼 비활성화 처리.
                        //if (txtMSlipNo.Text != "" &&
                        //     (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                        //    btnConfirmCancel.Enabled = false;
                        // 2022.01.21. hma 수정(End)

                        btnRef.Enabled = false;

                        btnMinusCancel.Enabled = false;     // 2022.02.12. hma 추가
                        // 2022.01.21. hma 추가(Start): 반제전표 결재상태에 따라 반제취소 버튼 활성화 처리. 반제전표 결재상태가 상신대기, 반려이면 활성화.
                        if (txtMSlipNo.Text != "" &&
                            (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                        {
                            btnMinusCancel.Enabled = true;
                        }
                        // 2022.01.21. hma 추가(End)
                        bMinusBtn = btnMinusCancel.Enabled;     // 2022.02.16. hma 추가
                    }
                }
                else { rdoCfm_N.Checked = true; }

                txtIvNo.Value = dt.Rows[0]["IV_NO"].ToString();
                txtIvNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtIvNo.ReadOnly = true;

                dtpIvDt.Value = dt.Rows[0]["IV_DT"].ToString();
                txtIvType.Value = dt.Rows[0]["IV_TYPE_NM"].ToString();
                txtIvTypeNm.Value = dt.Rows[0]["IV_TYPE"].ToString();
                txtCustCd.Value = dt.Rows[0]["CUST_CD"].ToString();
                txtCustNm.Value = dt.Rows[0]["CUST_NM"].ToString();
                txtPurDuty.Value = dt.Rows[0]["PUR_DUTY"].ToString();
                txtPurDutyNm.Value = dt.Rows[0]["USR_NM"].ToString();
                cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"];
                txtExchRate.Value = dt.Rows[0]["EXCH_RATE"];

                txtIvAmt.Value = dt.Rows[0]["IV_AMT"];
                txtIvAmtLoc.Value = dt.Rows[0]["IV_AMT_LOC"];
                txtVatAmt.Value = dt.Rows[0]["VAT_AMT"];
                txtVatAmtLoc.Value = dt.Rows[0]["VAT_AMT_LOC"];

                txtBillCustCd.Value = dt.Rows[0]["BILL_CUST"].ToString();
                txtBillCustNm.Value = dt.Rows[0]["BILL_CUST_NM"].ToString();
                txtPaymentCustCd.Value = dt.Rows[0]["PAYMENT_CUST"].ToString();
                txtPaymentCustNm.Value = dt.Rows[0]["PAYMENT_CUST_NM"].ToString();

                if (dt.Rows[0]["PAYMENT_PLAN_DT"].ToString() != "")
                    dtpPaymentPlanDt.Value = dt.Rows[0]["PAYMENT_PLAN_DT"].ToString();

                txtVatType.Value = dt.Rows[0]["VAT_TYPE"].ToString();
                txtVatTypeNm.Value = dt.Rows[0]["VAT_TYPE_NM"].ToString();
                txtVatRate.Value = dt.Rows[0]["VAT_RATE"].ToString();

                txtPaymentMeth.Value = dt.Rows[0]["PAYMENT_METH"].ToString();
                txtPaymentMethNm.Value = dt.Rows[0]["PAYMENT_METH_NM"].ToString();
                txtPaymentTerm.Value = dt.Rows[0]["PAYMENT_TERM"].ToString();
                txtPaymentTermRemark.Value = dt.Rows[0]["PAYMENT_TERM_REMARK"].ToString();
                txtTaxBizCd.Value = dt.Rows[0]["TAX_BIZ_CD"].ToString();
                txtTaxBizNm.Value = dt.Rows[0]["TAX_BIZ_NM"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();

                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                btnRef.Enabled = true;

                if (txtSlipNo.Text != "")
                {
                    btnRef.Enabled = true;
                }
                else
                {
                    btnRef.Enabled = false;
                }

                btnMinusCancel.Enabled = bMinusBtn;     // 2022.02.16. hma 추가

                //Detail그리드 정보.
                string strSql1 = " usp_MIV002  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pIV_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 4);

                //확정여부에 따른 화면 Locking
                if ((dt.Rows[0]["CONFIRM_YN"].ToString() == "Y")
                     // 2022.01.23. hma 추가(Start): 미확정 상태이지만 반제전표가 생성해서 승인상태가 아니면
                     || (dt.Rows[0]["CONFIRM_YN"].ToString() == "N" && dt.Rows[0]["MINUS_SLIP_NO"].ToString() != "" &&
                         dt.Rows[0]["MINUS_GW_STATUS"].ToString() != "APPR"))
                    // 2022.01.23. hma 추가(End)
                {
                    //Detail Locking설정
                    UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "지급유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "지급액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|5"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|5"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선급금번호_2") + "|5"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                        );
                }
                else
                {
                    //Detail Locking해제
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        GridSet(i);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.			
            }
            strBtn = "N";
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        //매입형태
        private void btnSIvType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1 = 'IV_TYPE', @pSPEC2 = 'IV_TYPE_NM', @pSPEC3 = 'M_IV_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSIvType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "매입형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSIvType.Value = Msgs[0].ToString();
                    txtSIvTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.			
            }
            strBtn = "N";
        }

        //공급처
        private void btnSCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtSCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSCustCd.Value = Msgs[1].ToString();
                    txtSCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.			
            }
            strBtn = "N";
        }

        //구매담당자
        private void btnSPurDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSPurDuty.Value = Msgs[0].ToString();
                    txtSPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.			

            }
            strBtn = "N";
        }

        private void btnSProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtSProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtSProjectNo.Text = Msgs[3].ToString();
                    txtSProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.			
            }
            strBtn = "N";
        }

        private void btnSProjSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtSProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtSProjectSeq.Value = Msgs[0].ToString();
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

        #region 조회조건 TextChanged
        //매입형태
        private void txtSIvType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSIvType.Text != "")
                    {
                        txtSIvTypeNm.Value = SystemBase.Base.CodeName("IV_TYPE", "IV_TYPE_NM", "M_IV_TYPE", txtSIvType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSIvTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //공급처
        private void txtSCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSCustCd.Text != "")
                    {
                        txtSCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //구매담당자
        private void txtSPurDuty_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N" && txtSPurDuty.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtSPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtSPurDuty.Text != "")
                        {
                            txtSPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtSPurDutyNm.Value = "";
                        }
                    }

                }
                else if (txtSPurDuty.Text.Trim() == "") txtSPurDutyNm.Text = "";                
            }
            catch
            {

            }
        }

        private void txtSProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N") txtSProjectSeq.Value = "";
        }

        #endregion

        #region 확정, 취소
        private void btnConfirmOk_Click(object sender, System.EventArgs e)
        {
            Confirm("Y");
        }

        private void btnConfirmCancel_Click(object sender, System.EventArgs e)
        {
            Confirm("N");
        }

        private void Confirm(string strConfirmYn)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MIV002  'P0'";
                strSql += ", @pIV_NO = '" + strAutoIvNo + "' ";
                strSql += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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
                SubSearch(strAutoIvNo);
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

        #region 폼 Activated & Deactivate
        private void MIV002_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSIvDtFr.Focus();
        }

        private void MIV002_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region btnMinusCancel_Click(): 반제취소 버튼 클릭시. 반제전표 삭제 처리
        private void btnMinusCancel_Click(object sender, EventArgs e)
        {
            // 2022.02.16. hma 추가(Start): 반제취소 버튼 클릭시 반제취소 할건지 확인하고 처리하도록 함.
            DialogResult dsMsg = MessageBox.Show("반제취소 처리하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg != DialogResult.Yes)
            {
                return;
            }
            // 2022.02.16. hma 추가(End)

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MIV001  'D3'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pIV_NO = '" + txtIvNo.Text + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SubSearch(txtIvNo.Text);
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

        // 2021.12.17. hma 추가(Start): 확정전표번호로 결의전표등록 화면 열기
        #region lnkJump1_LinkClicked()
        private void lnkJump1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strJumpFileName1.Length > 0)
                {
                    string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                    string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    strLinkSlipNo = txtCSlipNo.Text;     // 확정전표번호

                    Link1Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Link
        protected override void Link1Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "ACD001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "결의전표등록"; 	// 이동할 폼명을 적어준다(메뉴명)
        }


        private object[] Params()
        {
            if (strLinkSlipNo == "")
                param = null;						// 파라메터를 하나도 넘기지 않을경우
            else
            {
                param = new object[1];				// 파라메터수가 4개인 경우
                param[0] = strLinkSlipNo;
            }
            return param;
        }
        #endregion
        // 2021.12.17. hma 추가(End)

        // 2022.01.21. hma 추가(Start): 반제전표상신 클릭시 처리. 반제전표번호로 결의전표등록 화면을 열어준다.
        #region lnkJump2_LinkClicked(): 반제전표상신 클릭시.
        private void lnkJump2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strJumpFileName2.Length > 0)
                {
                    string DllName = strJumpFileName2.Substring(0, strJumpFileName2.IndexOf("."));
                    string FrmName = strJumpFileName2.Substring(strJumpFileName2.IndexOf(".") + 1, strJumpFileName2.Length - strJumpFileName2.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    strLinkSlipNo = txtMSlipNo.Text;     // 반제전표번호

                    Link2Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName2), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Link
        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "ACD001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "결의전표등록"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion        

        #region 전표조회 이벤트
        private void btnRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlipNo.Text != "")
                {
                    WNDW.WNDW026 pu = new WNDW.WNDW026(txtSlipNo.Text);
                    pu.ShowDialog();
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("S0016"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion 
                       
    }
}