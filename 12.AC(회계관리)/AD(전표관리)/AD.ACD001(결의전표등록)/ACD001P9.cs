

#region 작성정보
/*********************************************************************/
// 단위업무명: 전표반제 팝업
// 작 성 자  : 한 미 애
// 작 성 일  : 2022-01-17
// 작성내용  : 선택한 전표에 대한 반제전표를 생성한다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  : 이 팝업에서 반제 처리를 하려고 했으나 결의전표등록에서 전표취소 버튼으로 사용하고 이 팝업은 사용 안함.
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

namespace AD.ACD001
{
    public partial class ACD001P9 : UIForm.Buttons
    {
        #region 변수선언
        string strSlipNo = "";
        string[] returnVal = null;
        string strChgFlag = "";
        #endregion

        public ACD001P9()
        {
            InitializeComponent();
        }

        public ACD001P9(string SLIP_NO)
        {
            strSlipNo = SLIP_NO;

            InitializeComponent();
        }

        #region Form Load 시
        private void ACD001P9_Load(object sender, System.EventArgs e)
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "반제전표처리";

                SystemBase.ComboMake.C1Combo(cboCreathPath, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A101', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //발생경로
                SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);     //결재상태

                txtSlipNo.Value = strSlipNo;
                dtpMinusSlipDt.Value = SystemBase.Base.ServerTime("YYMMDD");        // 현재일자로.

                strChgFlag = "N";

                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ACD001P9_FormClosing(): 폼 종료시 저장/상신 처리 여부 체크
        private void ACD001P9_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (strChgFlag == "Y")      // 저장이나 상신 처리를 한 경우 폼 닫은 후 결재선 다시 조회하도록 
            {
                RtnStr("OK");
            }
            else
            {
                RtnStr("Cancel");
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_ACD001P9 @pTYPE = 'S1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    dtpSlipDt.Value = ds.Tables[0].Rows[0]["SLIP_RES_DT"].ToString();
                    txtDeptCd.Value = ds.Tables[0].Rows[0]["DEPT_CD"].ToString();
                    txtDeptNm.Value = ds.Tables[0].Rows[0]["DEPT_NM"].ToString();
                    cboCreathPath.SelectedValue = ds.Tables[0].Rows[0]["CREATE_PATH"].ToString();
                    txtConfirm_YN.Value = ds.Tables[0].Rows[0]["CONFIRM_YN"].ToString();
                    if (ds.Tables[0].Rows[0]["GW_STATUS"].ToString() != "")
                        cboGwStatus.SelectedValue = ds.Tables[0].Rows[0]["GW_STATUS"].ToString();
                    txtMinusSlipNo.Value = ds.Tables[0].Rows[0]["MINUS_SLIP_NO"].ToString();

                    if (txtMinusSlipNo.Text != "")
                    {
                        dtpMinusSlipDt.Enabled = false;
                        btnMinusSlip.Enabled = false;
                    }
                }
                else
                {
                    dtpMinusSlipDt.Enabled = false;
                    btnMinusSlip.Enabled = false;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 선택값 입력 및 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string AssignNos)
        {
            returnVal = new string[2];
            returnVal[0] = strChgFlag;
        }

        #endregion

        private void btnMinusSlip_Click(object sender, EventArgs e)
        {
            if (txtSlipNo.Text == "")
            {
                MessageBox.Show("전표번호가 입력되지 않았습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 대상 전표번호가 미승인인지 체크
            if (txtConfirm_YN.Text != "승인")
            {
                MessageBox.Show("전표가 승인되지 않았으므로 반제처리하실 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 결재상태(그룹웨어상태)가 승인이 아닌 건은 반제 처리가 안되게 한다.
            if (cboGwStatus.Text == ""  || cboGwStatus.SelectedValue.ToString() != "APPR")
            {
                MessageBox.Show("결재 승인 상태가 아니므로 반제처리하실 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 반제전표번호가 있는 경우 반제처리 안되게.
            if (txtMinusSlipNo.Text != "")
            {
                MessageBox.Show("이미 반제처리 되었으므로 반제처리하실 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }           

            string strQuestMsg = "";
            strQuestMsg = " 반제전표를 생성하시겠습니까?";

            if (MessageBox.Show(txtSlipNo.Text + strQuestMsg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                string strNewAssignNo = "";

                this.Cursor = Cursors.WaitCursor;

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {                   
                    string ERRCode = "ER", MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_ACD001P9 @pTYPE = 'I1'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
                        strSql = strSql + ", @pMINUS_SLIP_DT = '" + dtpMinusSlipDt.Text + "'";
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                        DataSet ds11 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds11.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds11.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch
                    {
                        Trans.Rollback();
                        MSGCode = "P0001";
                    }

                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SearchExec();
                        strChgFlag = "Y";       // 변경여부를 Y로

                        RtnStr(strNewAssignNo);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
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
    }
}
