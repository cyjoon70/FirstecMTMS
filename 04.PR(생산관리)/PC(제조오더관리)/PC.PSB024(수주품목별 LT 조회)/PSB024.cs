#region 작성정보
/*********************************************************************/
// 단위업무명 : 제조오더현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-16
// 작성내용 : 제조오더현황조회 및 관리
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

namespace PC.PSB024
{
    public partial class PSB024 : UIForm.FPCOMM1
    {
        #region 생성자
        public PSB024()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PSB024_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보
            SystemBase.ComboMake.C1Combo(cboOrderFlag, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'P026', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);  //지시구분
            SystemBase.ComboMake.C1Combo(cboJobFlag, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P038', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //작업구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpDeliveryDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpDeliveryDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(2).ToString().Substring(0,10);

            dtpReportDtFr.Value = null;
            dtpReportDtTo.Value = null;
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpDeliveryDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpDeliveryDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(2).ToString().Substring(0,10);

            dtpReportDtFr.Value = null;
            dtpReportDtTo.Value = null;
        }
        #endregion

        #region 조회조건 팝업
        //제품코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "10");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Value = "";
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

        //거래처 From
        private void btnSoldCustFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtSoldCustFr.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoldCustFr.Value = Msgs[1].ToString();
                    txtSoldCustNmFr.Value = Msgs[2].ToString();
                    txtSoldCustFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

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
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                {
                    txtProjectSeq.Value = "";
                }
            }
            catch
            {

            }
        }

        //거래처 From
        private void txtSoldCustFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSoldCustFr.Text != "")
                {
                    txtSoldCustNmFr.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCustFr.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSoldCustNmFr.Value = "";
                }
            }
            catch
            {

            }
        }
        //제품코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                if (Convert.ToDateTime(dtpDeliveryDtFr.Value) <= Convert.ToDateTime(dtpDeliveryDtTo.Value)) // 납기일From 이 To 보다 크면 조회내용이 없다.
                {
                    try
                    {
                        string strQuery = "usp_PSB024 @pTYPE = 'S1'";
                        strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                        strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                        strQuery += ", @pPROJECT_SEQ_TO = '" + txtProjectSeqTo.Text + "'";
                        strQuery += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
                        strQuery += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
                        strQuery += ", @pSOLD_CUST = '" + txtSoldCustFr.Text + "'";
                        strQuery += ", @pREPORT_DT_FR = '" + dtpReportDtFr.Text + "'";
                        strQuery += ", @pREPORT_DT_TO = '" + dtpReportDtTo.Text + "'";
                        strQuery += ", @pSO_NO = '" + txtSoNo.Text + "'";
                        strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                        strQuery += ", @pORDER_FLAG = '" + cboOrderFlag.SelectedValue.ToString() + "'";
                        strQuery += ", @pJOB_FLAG = '" + cboJobFlag.SelectedValue.ToString() + "'";
                       
                        string strCloseYn = "";
                        if (rdoCfmYes.Checked == true)
                        {
                            strCloseYn = "Y";
                        }
                        else if (rdoCfmNo.Checked == true)
                        {
                            strCloseYn = "N";
                        }

                        string strSchStatus = "";
                        if (rdoSchF.Checked == true)
                        {
                            strSchStatus = "F";
                        }
                        else if (rdoSchP.Checked == true)
                        {
                            strSchStatus = "P";
                        }
                        else if (rdoSchC.Checked == true)
                        {
                            strSchStatus = "C";
                        }

                        strQuery += ", @pCLOSE_YN = '" + strCloseYn + "'";
                        strQuery += ", @pMPS_STATUS = '" + strSchStatus + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                        if (fpSpread1.Sheets[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산L/T")].ForeColor = Color.Blue;
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질L/T")].ForeColor = Color.Green;
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총L/T")].ForeColor = Color.Red;
                            }
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                    }
                }
                else
                {
                    //그리드 초기화
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

    }
}
