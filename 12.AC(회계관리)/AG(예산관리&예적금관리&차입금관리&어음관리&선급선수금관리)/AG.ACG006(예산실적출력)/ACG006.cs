

#region 작성정보
/*********************************************************************/
// 단위업무명 : 예산실적출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-13
// 작성내용 : 예산실적출력
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

namespace AG.ACG006
{
    public partial class ACG006 : UIForm.Buttons
    {
        string strREORG_ID_FROM = "";
        string strREORG_ID_TO = "";
        public ACG006()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG006_Load(object sender, System.EventArgs e)
        {
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            strREORG_ID_FROM = SystemBase.Base.gstrREORG_ID;
            strREORG_ID_TO = SystemBase.Base.gstrREORG_ID;
            dtpEstYYMMFr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01-01";
            dtpEstYYMMTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strTempREORG_ID = "";
                    if (txtDeptCdFrom.Text == "" && txtDeptCdTo.Text == "")
                    {
                        strTempREORG_ID = "";
                    }
                    else if (strREORG_ID_FROM == strREORG_ID_TO)
                    {
                        strTempREORG_ID = strREORG_ID_FROM;
                    }
                    else if (strREORG_ID_FROM != strREORG_ID_TO && txtDeptCdFrom.Text == "")
                    {
                        strTempREORG_ID = strREORG_ID_TO;
                    }
                    else if (strREORG_ID_FROM != strREORG_ID_TO && txtDeptCdTo.Text == "")
                    {
                        strTempREORG_ID = strREORG_ID_FROM;
                    }
                    else
                    {
                        MessageBox.Show("부서의 개편ID가 다릅니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string RptName = "";
                    if(optPrint_1.Checked == true)
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG006_1.rpt";    // 레포트경로+레포트명
                    else if(optPrint_2.Checked == true)
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG006_2.rpt";    // 레포트경로+레포트명
                    else if(optPrint_3.Checked == true)
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG006_3.rpt";    // 레포트경로+레포트명

                    string[] RptParmValue = new string[9];   // SP 파라메타 값

                    if(optPrint_1.Checked == true)
                        RptParmValue[0] = "P1";
                    else if(optPrint_2.Checked == true)
                        RptParmValue[0] = "P2";
                    else if(optPrint_3.Checked == true)
                        RptParmValue[0] = "P3";

                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = dtpEstYYMMFr.Text;
                    RptParmValue[3] = dtpEstYYMMTo.Text;
                    RptParmValue[4] = strTempREORG_ID;
                    RptParmValue[5] = txtDeptCdFrom.Text;
                    RptParmValue[6] = txtDeptCdTo.Text;
                    RptParmValue[7] = txtEstCdFrom.Text;
                    RptParmValue[8] = txtEstCdTo.Text;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                    //UIForm.PRINT10 frm = new UIForm.PRINT10( this.Text + "출력", null, RptName, RptParmValue);	//공통크리스탈 10버전
                    frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텍스트 체인지
        //부서FROM
        private void txtDeptCdFrom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDeptNmFrom.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtDeptCdFrom.Text, " AND REORG_ID = '" + strREORG_ID_FROM + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //부서TO
        private void txtDeptCdTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDeptNmTo.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtDeptCdTo.Text, " AND REORG_ID = '" + strREORG_ID_FROM + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtEstCdFrom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtEstNmFrom.Value = SystemBase.Base.CodeName("EST_CD", "EST_NM", "A_ESTIMATE_CODE", txtEstCdFrom.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtEstCdTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtEstNmTo.Value = SystemBase.Base.CodeName("EST_CD", "EST_NM", "A_ESTIMATE_CODE", txtEstCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업 클릭
        //부서FROM
        private void BtnDeptFrom_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCdFrom.Value = Msgs[1].ToString();
                    strREORG_ID_FROM = Msgs[5].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //부서TO
        private void BtnDeptTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCdTo.Value = Msgs[1].ToString();
                    strREORG_ID_TO = Msgs[5].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        private void btnEstFrom_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A040', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { "", "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00114", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "예산코드 조회");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtEstCdFrom.Text = Msgs[0].ToString();
                    txtEstCdFrom.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "예산 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnEstTo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A040', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { "", "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00114", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "예산코드 조회");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtEstCdTo.Text = Msgs[0].ToString();
                    txtEstCdTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "예산 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion
        
    }
}
