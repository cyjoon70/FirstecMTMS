#region 작성정보
/*********************************************************************/
// 단위업무명 : B/L 현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-17
// 작성내용 : B/L 현황조회 관리
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
using FarPoint.Win.Spread.CellType;

namespace MB.MBL502
{
    public partial class MBL502 : UIForm.FPCOMM1
    {
        #region 생성자
        public MBL502()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void MBL502_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpBlReceiptDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpBlReceiptDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            dtpLoadingDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpLoadingDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpBlReceiptDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpBlReceiptDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            dtpLoadingDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
            dtpLoadingDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);
        }
        #endregion

        #region 조회조건 팝업
        //수출자
        private void btnBeneficiaryCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBeneficiaryCust.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBeneficiaryCust.Value = Msgs[1].ToString();
                    txtBeneficiaryCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수출자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //가격조건
        private void btnCostCond_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCostCond.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00034", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "가격조건 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCostCond.Value = Msgs[0].ToString();
                    txtCostCondNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "가격조건 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

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

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Value = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //발주번호
        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW018 pu = new WNDW.WNDW018();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPoNo.Value = Msgs[1].ToString();
                    txtPoNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    txtProjectSeq.Value = Msgs[0].ToString();
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
        //수출자
        private void txtBeneficiaryCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBeneficiaryCust.Text != "")
                {
                    txtBeneficiaryCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBeneficiaryCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBeneficiaryCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //가격조건
        private void txtCostCond_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCostCond.Text != "")
                {
                    txtCostCondNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCostCond.Text, " AND MAJOR_CD = 'S005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCostCondNm.Value = "";
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
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;
                string strCfmYn = "";

                try
                {
                    if (rdoCfmYnYes.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoCfmYnNo.Checked == true) { strCfmYn = "N"; }

                    string strQuery = "usp_MBL502 @pTYPE = 'S1'";
                    strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "'";
                    strQuery += ", @pBENEFICIARY_CUST = '" + txtBeneficiaryCust.Text + "'";
                    strQuery += ", @pCOST_COND = '" + txtCostCond.Text + "'";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "'";
                    strQuery += ", @pBL_RECEIPT_DT_FR = '" + dtpBlReceiptDtFr.Text + "'";
                    strQuery += ", @pBL_RECEIPT_DT_TO = '" + dtpBlReceiptDtTo.Text + "'";
                    strQuery += ", @pLOADING_DT_FR = '" + dtpLoadingDtFr.Text + "'";
                    strQuery += ", @pLOADING_DT_TO = '" + dtpLoadingDtTo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }
        #endregion	
        
    }
}
