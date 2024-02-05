#region 작성정보
/*********************************************************************/
// 단위업무명 : 매출채권상세조회
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 매출채권상세조회
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
namespace SS.SSC003
{
    public partial class SSC003 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public SSC003()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void SSC003_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpBnDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,7) + "-01";
            dtpBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoCfrmAll.Checked = true;           
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpBnDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7) + "-01";
            dtpBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoCfrmAll.Checked = true;    
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {
                    string strQuery = " usp_SSC003 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";

                    if (rdoCfmY.Checked == true)
                        strQuery += ", @pBN_CONFIRM_YN ='Y'";
                    else if (rdoCfmN.Checked == true)
                        strQuery += ", @pBN_CONFIRM_YN ='N'";

                    strQuery += ", @pBN_DT_FR  ='" + dtpBnDtFr.Text + "'";
                    strQuery += ", @pBN_DT_TO  ='" + dtpBnDtTo.Text + "'";
                    strQuery += ", @pBN_TYPE ='" + txtBnType.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD  = '" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pCUST_CD  = '" + txtCustCd.Text.Trim() + "'";
                    strQuery += ", @pSALE_DUTY  = '" + txtSaleDuty.Text.Trim() + "'";
                    strQuery += ", @pBN_NO  = '" + txtSBnNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO  = '" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ  = '" + txtProjectSeq.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pMF_PLAN_USER = '" + txtMfPlanUser.Text + "' ";     // 2021.06.14. hma 추가: 생산관리담당자

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;	
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업  
        //매출채권형태
        private void btnBnType_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'BN_TYPE', @pSPEC2 = 'BN_TYPE_NM', @pSPEC3 = 'S_BN_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtBnType.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00084", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBnType.Text = Msgs[0].ToString();
                    txtBnTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //주문처
        private void butCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "S");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //영업담당자
        private void btnSaleDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON 'S011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSaleDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "영업담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSaleDuty.Text = Msgs[0].ToString();
                    txtSaleDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "영업담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //프로젝트번호
        private void btnProjNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged  
        //매출채권형태
        private void txtBnType_TextChanged(object sender, EventArgs e)
        {
            txtBnTypeNm.Value = SystemBase.Base.CodeName("BN_TYPE", "BN_TYPE_NM", "S_BN_TYPE", txtBnType.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //주문처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //영업담당자
        private void txtSaleDuty_TextChanged(object sender, EventArgs e)
        {
            txtSaleDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSaleDuty.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //품목
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            if (txtProjectNm.Text == "")
            {
                txtProjectSeq.Text = "";
            }
        }
        #endregion  

        #region Form Activated & Deactivated
        private void SSC003_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpBnDtFr.Focus();
        }

        private void SSC003_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        // 2021.06.14. hma 추가(Start)
        #region btnMfPlanUser_Click(): 생산관리담당 버튼 클릭시 사용자조회 팝업 띄움.
        private void btnMfPlanUser_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B010' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMfPlanUser.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "담당자 조회");  // 생산관리 사용자조회
                pu.Width = 450;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMfPlanUser.Value = Msgs[0].ToString();
                    txtMfPlanUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region txtMfPlanUser_TextChanged(): 생산관리담당자 항목입력시 해당ID에 대한 사용자명을 보여준다.
        private void txtMfPlanUser_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtMfPlanUser.Text != "")
                {
                    txtMfPlanUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtMfPlanUser.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtMfPlanUserNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}
