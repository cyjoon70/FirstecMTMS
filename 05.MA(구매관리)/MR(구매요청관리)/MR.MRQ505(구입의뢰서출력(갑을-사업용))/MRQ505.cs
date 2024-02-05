#region 작성정보
/*********************************************************************/
// 단위업무명 : 구입의뢰서출력(갑/을-사업용)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-13
// 작성내용 : 구입의뢰서출력(갑/을-사업용) 및 관리
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
using WNDW;

namespace MR.MRQ505
{
    public partial class MRQ505 : UIForm.Buttons
    {
        #region 생성자
        public MRQ505()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void MRQ505_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboItemDiv, "usp_M_COMMON @pTYPE = 'M032', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            //초기화
            UIForm.Buttons.ReButton("100000000001", BtnNew, BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_MRQ505 @pType='C2', @pCODE = 'B036', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);//품목계정

            //기타세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);
            txtRefProjectNo.Value = "1900";
            panel3.Enabled = false;
            rdoCfm0.Checked = true;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pType='COMM', @pCODE = 'B036', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//품목계정

            //기타세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);
            txtRefProjectNo.Value = "1900";
            rdoCfm0.Checked = true;
        }
        #endregion

        #region 조회조건 팝업
        //구매요청번호
        private void btnReqNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_MRQ505 @pTYPE = 'C3', @pCFM = '0' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "" };
                string[] strSearch = new string[] { txtReqNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00085", strQuery, strWhere, strSearch, new int[] { 0 }, "구매요청번호 조회");
                pu.Width = 700;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqNo.Text = Msgs[0].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
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

        //구입사유
        private void btnReqCause_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M019', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReqCause.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01011", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구입사유 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqCause.Text = Msgs[0].ToString();
                    txtReqCauseNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구입사유 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Text = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //발신자
        private void btnSendDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M021', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSendDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01012", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "발신자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSendDuty.Text = Msgs[0].ToString();
                    txtSendNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발신자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //수신자
        private void btnReceiveDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M020', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReceiveDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01013", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수신자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReceiveDuty.Text = Msgs[0].ToString();
                    txtReceiveNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수신자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //품목코드 FROM
        private void btnItemCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW001 pu = new WNDW001(txtItemCdFr.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCdFr.Text = Msgs[1].ToString();
                    txtItemNmFr.Value = Msgs[2].ToString();
                    txtItemCdFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //품목코드 TO
        private void btnItemCdTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW001 pu = new WNDW001(txtItemCdTo.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCdTo.Text = Msgs[1].ToString();
                    txtItemNmTo.Value = Msgs[2].ToString();
                    txtItemCdTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //요청자
        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReqId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "요청자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqId.Text = Msgs[0].ToString();
                    txtReqNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "요청자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                    txtProjectSeq.Text = "";
            }
            catch
            {

            }
        }

        //구입사유
        private void txtReqCause_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtReqCause.Text != "")
                {
                    txtReqCauseNm.Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtReqCause.Text, " AND MAJOR_CD = 'M019' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtReqCauseNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //구매담당자
        private void txtPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDuty.Text != "")
                {
                    txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //발신자
        private void txtSendDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSendDuty.Text != "")
                {
                    txtSendNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSendDuty.Text, " AND MAJOR_CD = 'M021' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSendNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //수신자
        private void txtReceiveDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtReceiveDuty.Text != "")
                {
                    txtReceiveNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtReceiveDuty.Text, " AND MAJOR_CD = 'M020' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtReceiveNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목코드 FROM
        private void txtItemCdFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdFr.Text != "")
                {
                    txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목코드 TO
        private void txtItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdTo.Text != "")
                {
                    txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmTo.Value = "";
                }
            }
            catch
            {

            }
        }

        //요청자
        private void txtReqId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtReqId.Text != "")
                {
                    txtReqNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtReqId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtReqNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 자재구분에 따른 품목계정 Changed
        //주자재
        private void rdoCfm0_CheckedChanged(object sender, System.EventArgs e)
        {
            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_MRQ505 @pType='C2', @pCODE = 'B036', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);//품목계정
        }

        //부자재
        private void rdoCfm1_CheckedChanged(object sender, System.EventArgs e)
        {
            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_MRQ505 @pType='C1', @pCODE = 'B036', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);//품목계정
        }
        #endregion		
	        
        #region 레포트 출력
        private void butPreview_Click(object sender, System.EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string RptName = "";
                    string[] RptParmValue = new string[22];   // SP 파라메타 값
                    string strType = "";

                    if (rdoCfmR1.Checked == true)
                    {
                        RptName = @"Report\MRQ505_1.rpt";    // 레포트경로+레포트명
                        strType = "R1";
                    }
                    else
                    {
                        RptName = @"Report\MRQ505_2.rpt";    // 레포트경로+레포트명
                        strType = "R2";
                    }

                    RptParmValue[0] = strType;
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    RptParmValue[3] = txtProjectNo.Text;
                    RptParmValue[4] = txtProjectSeq.Text;

                    if (rdoMpr.Checked == true)
                        RptParmValue[5] = "M";
                    else
                        RptParmValue[5] = "S";

                    RptParmValue[6] = txtReqId.Text;
                    RptParmValue[7] = txtPurDuty.Text;
                    RptParmValue[8] = cboItemAcct.SelectedValue.ToString();
                    RptParmValue[9] = cboItemDiv.SelectedValue.ToString();
                    RptParmValue[10] = txtReqNo.Text;
                    RptParmValue[11] = dtpReqDtFr.Text;
                    RptParmValue[12] = dtpReqDtTo.Text;
                    RptParmValue[13] = txtItemCdFr.Text;
                    RptParmValue[14] = txtItemCdTo.Text;
                    RptParmValue[15] = txtDocNo.Text;
                    RptParmValue[16] = txtReceiveNm.Text;
                    RptParmValue[17] = txtSendNm.Text;
                    RptParmValue[18] = txtReqCauseNm.Text;
                    RptParmValue[19] = txtRemark.Text;

                    if (rdoCfm0.Checked == true)
                        RptParmValue[20] = "0";
                    else
                        RptParmValue[20] = "1";

                    RptParmValue[21] = "";

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전	

                    frm.ShowDialog();
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }

            }
        }

        #endregion

    }
}
