
#region 작성정보
/*********************************************************************/
// 단위업무명 : 감가상각전표처리
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-19
// 작성내용 : 감가상각전표처리
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
using System.Reflection;        // 2022.01.28. hma 추가: 전표 링크 위해 추가

namespace AH.ACH010
{
    public partial class ACH010 : UIForm.Buttons
    {
        #region 변수선언
        string strLAST_DEPR_YYMM1 = "";
        string strLAST_DEPR_YYMM2 = "";
        string strLinkSlipNo = "";     // 2022.01.28. hma 추가: 링크전표번호
        #endregion

        public ACH010()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACH010_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            // 2022.01.28. hma 추가(Start): 그룹웨어상태
            SystemBase.ComboMake.C1Combo(cboCSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);     // 0=>9로
            SystemBase.ComboMake.C1Combo(cboMSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);
            // 2022.01.28. hma 추가(End)

            // 2022.03.17. hma 추가(Start)
            lnkJump1.Text = "확정전표상신";         // 화면에 보여지는 링크명
            strJumpFileName1 = "AD.ACD001.ACD001";  // 호출할 화면명
            lnkJump2.Text = "반제전표상신";         // 화면에 보여지는 링크명
            strJumpFileName2 = "AD.ACD001.ACD001";  // 호출할 화면명
            strLinkSlipNo = "";                     // 2022.01.21. hma 추가

            cboCSlipGwStatus.Text = "";      // 확정전표 결재상태 초기화
            cboMSlipGwStatus.Text = "";      // 반제전표 결재상태 초기화
            btnMinusCancel.Enabled = false;  // 반제취소 버튼 비활성화
            // 2022.03.17. hma 추가(End)

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                DEPR_SET();

                //DeprSlipSearch();

                txtDeptCd.Value = SystemBase.Base.gstrDEPT;
                txtDeptNm.Value = SystemBase.Base.gstrDEPTNM;               

                cboBizAreaCd.SelectedValue = SystemBase.Base.gstrBIZCD;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        protected void DEPR_SET()
        {
            try
            {
                string strLAST_DEPR_YYMM = SystemBase.Base.CodeName("CO_CD", "LAST_APP_YYMM", "A_ASSET_DEPR_RESULT", SystemBase.Base.gstrCOMCD, "AND BIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "'");

                if (strLAST_DEPR_YYMM == "")
                {
                    strLAST_DEPR_YYMM1 = "";
                    strLAST_DEPR_YYMM2 = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
                }
                else
                {
                    string strTempDt = strLAST_DEPR_YYMM.Substring(0, 4) + "-" + strLAST_DEPR_YYMM.Substring(4, 2) + "-01";
                    strLAST_DEPR_YYMM1 = strTempDt.Substring(0, 7);
                    strLAST_DEPR_YYMM2 = Convert.ToDateTime(strTempDt).AddMonths(1).ToShortDateString().Substring(0, 7);
                }

                if (optCancel.Checked == true)
                {
                    txtDepr_YYMM.Value = strLAST_DEPR_YYMM1;
                }
                else
                {
                    txtDepr_YYMM.Value = strLAST_DEPR_YYMM2;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2022.01.28. hma 추가(Start)
        #region DeprSlipSearch(): 감가상각 전표 데이터 조회
        private void DeprSlipSearch()
        {
            string strSql = " usp_ACH010 @pTYPE = 'S1' ";
            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strSql += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
            strSql += ", @pDEPR_YYMM = '" + txtDepr_YYMM.Text.Replace("-", "") + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (dt.Rows.Count > 0)
            {
                txtConfirmYn.Value = dt.Rows[0]["DEPR_CONFIRM_YN"].ToString();
                txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();
                txtCSlipNo.Value = dt.Rows[0]["CFM_SLIP_NO"].ToString();
                cboCSlipGwStatus.SelectedValue = dt.Rows[0]["CFM_GW_STATUS"].ToString();
                txtMinusConfirm.Value = dt.Rows[0]["MINUS_CONFIRM_YN"].ToString();
                txtMSlipNo.Value = dt.Rows[0]["MINUS_SLIP_NO"].ToString();
                cboMSlipGwStatus.SelectedValue = dt.Rows[0]["MINUS_GW_STATUS"].ToString();
                txtSlipConfirmYn.Value = dt.Rows[0]["SLIP_CONFIRM_YN"].ToString();                  // 전표번호 확정여부
                txtMinusSlipConfirmYn.Value = dt.Rows[0]["MINUS_SLIP_CONFIRM_YN"].ToString();       // 반제전표 확정여부

                if (txtConfirmYn.Text == "Y")   // 확정 상태인 경우
                {
                    // 확정상태이면서 결재상태가 상신대기/반려/승인 상태이면 확정취소 버튼 활성화되게.
                    if ((txtSlipNo.Text != "" && txtCSlipNo.Text == "") ||
                        ((txtCSlipNo.Text != "") &&
                         (cboCSlipGwStatus.SelectedValue.ToString() == "READY" || cboCSlipGwStatus.SelectedValue.ToString() == "REJECT" ||
                          (cboCSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y"))))
                    {
                        optCancel.Checked = true;
                    }
                    else
                    {
                        optCancel.Checked = false;
                    }

                    btnMinusCancel.Enabled = false;
                }
                else
                {
                    // 2022.02.17. hma 추가(Start): 반제전표 결재상태에 따라 반제취소 버튼 활성화 처리. 반제전표 결재상태가 상신대기, 반려이면 활성화.
                    btnMinusCancel.Enabled = false;

                    if (txtMSlipNo.Text != "" &&
                        (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                    {
                        btnMinusCancel.Enabled = true;
                    }
                    // 2022.02.17. hma 추가(End)
                }
            }
            else
            {
                SlipClear();        // 전표 데이터가 없으면 항목들 초기화
            }
        }
        #endregion


        private void SlipClear()
        {
            txtConfirmYn.Value = "";
            txtSlipNo.Value = "";
            txtCSlipNo.Value = "";
            cboCSlipGwStatus.SelectedValue = "";
            txtMinusConfirm.Value = "";
            txtMSlipNo.Value = "";
            cboMSlipGwStatus.SelectedValue = "";
            txtSlipConfirmYn.Value = "";
            txtMinusSlipConfirmYn.Value = "";
        }
        // 2022.01.28. hma 추가(End)

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (txtDeptCd.Text == "")
                {
                    MessageBox.Show("생성부서를 확인하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
                if (txtDepr_YYMM.Text == "")
                {
                    MessageBox.Show("상각년월을 확인하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    string strDelSql = " usp_ACH010  ";
                    strDelSql += " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strDelSql += ", @pTYPE = 'I1' ";         // 2022.01.28. hma 추가
                    strDelSql += ", @pDEPR_YYMM = '" + txtDepr_YYMM.Text.Replace("-","") + "' ";
                    strDelSql += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                    if (optRun.Checked == true) strDelSql += ", @pACT_TYPE = 'R' ";
                    else if (optCancel.Checked == true) strDelSql += ", @pACT_TYPE = 'C' ";
                    strDelSql += ", @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "' ";
                    
                    strDelSql += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    strDelSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                    strDelSql += ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    this.Cursor = Cursors.Default;
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {   
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DEPR_SET();
                    DeprSlipSearch();       // 2022.01.28. hma 추가
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
        #endregion

        #region 작업구분 변경시
        private void optRun_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optCancel.Checked == true)
                {
                    txtDepr_YYMM.Value = strLAST_DEPR_YYMM1;
                }
                else
                {
                    txtDepr_YYMM.Value = strLAST_DEPR_YYMM2;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 사업장변경시
        private void cboBizAreaCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DEPR_SET();
                DeprSlipSearch();       // 2022.01.28. hma 추가
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        // 2022.01.28. hma 추가(Start)
        #region btnMinusCancel_Click(): 반제취소 버튼 클릭시. 반제전표 삭제 처리.
        private void btnMinusCancel_Click(object sender, EventArgs e)
        {
            // 2022.02.17. hma 추가(Start): 반제취소 버튼 클릭시 반제취소 할건지 확인하고 처리하도록 함.
            DialogResult dsMsg = MessageBox.Show("반제취소 처리하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg != DialogResult.Yes)
            {
                return;
            }
            // 2022.02.17. hma 추가(End)

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_ACH010 @pTYPE = 'D1'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                strSql += ", @pDEPR_YYMM = '" + txtDepr_YYMM.Text.Replace("-", "") + "' ";
                strSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

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
                DEPR_SET();
                DeprSlipSearch();
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
        // 2022.01.28. hma 추가(End)

        #region lnkJump1_LinkClicked(): 확정전표상신 링크 클릭시. 
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

        #region lnkJump2_LinkClicked(): 반제전표상신 링크 클릭시.
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

        #region Link2Exec()
        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "ACD001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "결의전표등록"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        private void txtDepr_YYMM_TextChanged(object sender, EventArgs e)
        {
            DeprSlipSearch();
        }
        #endregion        
        
    }
}
