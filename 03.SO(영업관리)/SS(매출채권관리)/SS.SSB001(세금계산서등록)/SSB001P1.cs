#region 작성정보
/*********************************************************************/
// 단위업무명 : 매출채권정보 조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-28
// 작성내용 : 매출채권정보 조회
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

namespace SS.SSB001
{
    public partial class SSB001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        DataTable dt = null;
        #endregion

        #region 생성자
        public SSB001P1()
        {
            InitializeComponent();
        }
        #endregion

        #region SSB001P1 Form Load 이벤트
        private void SSB001P1_Load(object sender, EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //GropBox1 조회조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", true);//화폐단위
            SystemBase.ComboMake.C1Combo(cboSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당
            SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", true);//VAT유형

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpBnDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            txtTaxBizCd.Text = SystemBase.Base.CodeName("BIZ_CD", "TAX_BIZ_CD", "B_BIZ_PLACE", SystemBase.Base.gstrBIZCD, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'"); //신고사업장
            panel3.Enabled = false;

            this.Text = "매출채권 정보 조회";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            Search(true);

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 조회함수
        private void Search(bool Msg)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    //발행처 유효성체크
                    if (txtBillCustCd.Text != "" && txtBillCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "발행처")); //존재하지 않는 발행처 코드입니다.

                        txtBillCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    string strQuery = " usp_SSB001  @pTYPE = 'S4', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
                    strQuery += ", @pBN_DT_FR = '" + dtpBnDtFr.Text + "' ";
                    strQuery += ", @pBN_DT_TO = '" + dtpBnDtTo.Text + "' ";
                    strQuery += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBILL_CUST = '" + txtBillCustCd.Text + "' ";
                    strQuery += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                    strQuery += ", @pTAX_BIZ_CD = '" + txtTaxBizCd.Text + "' ";
                    strQuery += ", @pVAT_TYPE = '" + cboVatType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    string strVatIncFlag = "";
                    if (rdoUnite.Checked == true)
                    {
                        if (rdoExtra.Checked == true) { strVatIncFlag = "2"; }
                        else { strVatIncFlag = "1"; }
                    }
                    strQuery += ", @pVAT_INC_FLAG = '" + strVatIncFlag + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 종료
        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region 확인(선택된 값 넘기기)
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                DataTable dt = ((System.Data.DataTable)(fpSpread1.Sheets[0].DataSource));

                if (dt.Rows.Count > 0) { ReturnDt = dt; }
            }

            strFormClosingMsg = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 조회조건 팝업
        //발행처
        private void btnBillCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtBillCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBillCustCd.Text = Msgs[1].ToString();
                    txtBillCustNm.Value = Msgs[2].ToString();
                    txtBillCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발행처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //신고사업장
        private void btnTaxBiz_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON @pTYPE ='S070', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTaxBizCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "세금신고사업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTaxBizCd.Text = Msgs[0].ToString();
                    txtTaxBizNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "세금신고사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        private void txtBillCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBillCustCd.Text != "")
                {
                    txtBillCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBillCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBillCustNm.Value = "";
                }
            }
            catch { }
        }

        private void txtTaxBizCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTaxBizCd.Text != "")
                {
                    txtTaxBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtTaxBizCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtTaxBizNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region VAT통합구분 버튼 변경시
        private void rdoDutch_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (rdoDutch.Checked == true)
                {
                    rdoExtra.Checked = true;
                    panel3.Enabled = false;
                    //vat유형 콤보박스 재 설정 (일반)
                    SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//VAT유형
                    cboVatType.Text = "전체";
                    cboVatType.BackColor = SystemBase.Validation.Kind_White;
                }
                else
                {
                    panel3.Enabled = true;
                    //vat유형 콤보박스 재 설정 (필수)
                    SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//VAT유형
                    cboVatType.SelectedValue = "A";	//일반세금계산서
                    cboVatType.BackColor = SystemBase.Validation.Kind_LightCyan;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }

        }
        #endregion

        #region DataTable GetSet
        public DataTable ReturnDt { get { return dt; } set { dt = value; } }
        #endregion
    }
}
