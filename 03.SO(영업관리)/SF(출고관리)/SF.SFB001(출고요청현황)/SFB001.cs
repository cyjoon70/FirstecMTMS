#region 작성정보
/*********************************************************************/
// 단위업무명 : 출고요청현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-01
// 작성내용 : 출고요쳥현황 및 관리
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

namespace SF.SFB001
{
    public partial class SFB001 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strSchNo = "";
        string strBtn = "N";
        #endregion

        #region 생성자
        public SFB001()
        {
            InitializeComponent();
        }

        public SFB001(string Div)
        {
            strSchNo = Div;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SFB001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);
            dtpDeliveryDtFr.Value = null;
            dtpDeliveryDtTo.Value = null;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);
            dtpDeliveryDtFr.Value = null;
            dtpDeliveryDtTo.Value = null;
        }
        #endregion

        #region 조회조건 팝업
        //출고형태
        private void btnMoveType_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_S_COMMON @pTYPE = 'S090', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
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

        //납품처
        private void btnShipCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtShipCust.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtShipCust.Text = Msgs[1].ToString();
                    txtShipCustNm.Value = Msgs[2].ToString();
                    txtShipCust.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "납품처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //영업담당자
        private void btnSaleDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_S_COMMON @pTYPE = 'S011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSaleDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "영업담당자 조회");
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

        //차수
        private void btnProjSeq_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
        //출고요청번호
        private void btnReqNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_MRQ499 @pTYPE = 'P1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "" };
                string[] strSearch = new string[] { txtSDnNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00085", strQuery, strWhere, strSearch, new int[] { 0 }, "구매요청번호 조회");
                pu.Width = 600;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSDnNo.Text = Msgs[0].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //수주번호
        private void btnSoNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW012 pu = new WNDW012();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoNo.Text = Msgs[1].ToString();
                    txtSoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //운송방법
        private void btnTranMeth_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S013', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00035", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "운송방법");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranMeth.Text = Msgs[0].ToString();
                    txtTranMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "운송방법 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //출고형태
        private void txtMoveType_TextChanged(object sender, EventArgs e)
        {
            txtMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtMoveType.Text, " AND TRAN_TYPE = 'DI' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //납품처
        private void txtShipCust_TextChanged(object sender, EventArgs e)
        {
            txtShipCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtShipCust.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //영업담당자
        private void txtSaleDuty_TextChanged(object sender, EventArgs e)
        {
            txtSaleDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSaleDuty.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
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

        //운송방법
        private void txtTranMeth_TextChanged(object sender, EventArgs e)
        {
            txtTranMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTranMeth.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S013' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            string strDnYn = "";

            try
            {

                if (rdoDnYnYes.Checked == true) { strDnYn = "Y"; }
                else if (rdoDnYnNo.Checked == true) { strDnYn = "N"; }

                string strQuery = "usp_SFB001 @pTYPE = 'S1'";
                strQuery += ", @pDN_YN = '" + strDnYn + "'";
                strQuery += ", @pMOVE_TYPE = '" + txtMoveType.Text + "'";
                strQuery += ", @pSO_NO = '" + txtSoNo.Text + "'";
                strQuery += ", @pSHIP_CUST = '" + txtShipCust.Text + "'";
                strQuery += ", @pTRAN_METH = '" + txtTranMeth.Text + "'";
                strQuery += ", @pSALE_DUTY = '" + txtSaleDuty.Text + "'";
                strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "'";
                strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                strQuery += ", @pDN_NO = '" + txtSDnNo.Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pREF_DELV_DT_FR = '" + dtpRefDelvDtFr.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조) FROM
                strQuery += ", @pREF_DELV_DT_TO = '" + dtpRefDelvDtTo.Text + "' ";      // 2017.11.10. hma 추가: 납기일(참조) TO

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 상세조회
        private void SubSearch(string strDnNo)
        {
            string strQuery = "usp_SFB001 @pTYPE = 'S2'";
            strQuery += ", @pDN_NO = '" + strDnNo + "'";
            strQuery += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
            strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
        }
        #endregion

        #region 그리드 선택시
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                SubSearch(fpSpread2.Sheets[0].Cells[fpSpread2.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text);
            }
            catch (Exception f)
            {

            }
        }
        #endregion		
    }
}
