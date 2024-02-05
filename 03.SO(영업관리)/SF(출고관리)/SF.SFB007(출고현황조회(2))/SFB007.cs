#region 
/*********************************************************************/
// 단위업무명 : 출고현황조회2
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 출고현황조회2
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
namespace SF.SFB007
{
    public partial class SFB007 : UIForm.FPCOMM1
    {
        #region 생성자
        public SFB007()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void SFB007_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7) + "-01";
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDeliveryDtFr.Value = null;
            dtpDeliveryDtTo.Value = null;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7) + "-01";
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDeliveryDtFr.Value = null;
            dtpDeliveryDtTo.Value = null;
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
                    string strQuery = " usp_SFB007 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pDN_DT_FR  ='" + dtpDtFr.Text + "'";
                    strQuery += ", @pDN_DT_TO  ='" + dtpDtTo.Text + "'";
                    strQuery += ", @pMOVE_TYPE ='" + txtMoveType.Text.Trim() + "'";
                    strQuery += ", @pCUST_CD  = '" + txtCustCd.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text.Trim() + "'";
                    strQuery += ", @pSO_NO = '" + txtSoNo.Text + "'";
                    strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                    strQuery += ", @pDN_NO = '" + txtSDnNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조)FROM
                    strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조)TO

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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
        //출하형태
        private void btnMoveType_Click(object sender, EventArgs e)
        {

            try
            {
                string strQuery = "usp_S_COMMON @pTYPE = 'S090', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMoveType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "출고형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMoveType.Text = Msgs[0].ToString();
                    txtMoveTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //프로젝트번호 
        private void btnProjectNo_Click(object sender, System.EventArgs e)
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
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //수주번호
        private void btnSoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW012 pu = new WNDW.WNDW012();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoNo.Value = Msgs[1].ToString();
                    txtSoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged  
        //출하형태
        private void txtMoveType_TextChanged(object sender, EventArgs e)
        {
            txtMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtMoveType.Text, " AND TRAN_TYPE = 'DI' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //주문처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            if (txtProjectNm.Value == "")
            {
                txtProjectSeq.Value = "";
            }
        }
        #endregion   

    }
}
