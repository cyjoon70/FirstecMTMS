#region 작성정보
/*********************************************************************/
// 단위업무명 : 발주변경
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 발주변경 및 관리
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

namespace MB.MBL001
{
    public partial class MBL001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread spd;
        string returnVal = "";

        string strPoNo = "";
        string strCostCond = "";
        string strPurDuty = "";
        string strBeneficiaryCust = "";
        string strCurrency = "";
        string strPaymentMeth = "";
        string strApplicantCust = "";
        bool bLock = false;
        #endregion

        #region 생성자
        public MBL001P1()
        {
            InitializeComponent();
        }

        public MBL001P1(FarPoint.Win.Spread.FpSpread spread,
                        string CostCond,
                        string PurDuty,
                        string BeneficiaryCust,
                        string Currency,
                        string PaymentMeth,
                        string ApplicantCust)
        {

            spd = spread;
            strCostCond = CostCond;
            strPurDuty = PurDuty;
            strBeneficiaryCust = BeneficiaryCust;
            strCurrency = Currency;
            strPaymentMeth = PaymentMeth;
            strApplicantCust = ApplicantCust;
            bLock = true;
                        
            InitializeComponent();
        }

        public MBL001P1(FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            spd = spread;
        }
        #endregion

        #region 폼로드 이벤트
        private void MBL001P1_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "발주참조팝업";
                        
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //단위

            //그리드 콤보박스 세팅			
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타세팅
            dtpPoDtFr.Value = null;
            dtpPoDtTo.Value = null;
            rdoPoNo_Y.Checked = true;

            Set_Tag(";1;;");
            if (bLock == true) { Set_Value(); Grid_Search(false); }
        }
        #endregion
        
        #region 조회 조건값넣기
        private void Set_Value()
        {
            Set_Tag(";3;;");

            txtCostCond.Value = strCostCond;
            txtPurDuty.Value = strPurDuty;
            txtBeneficiaryCust.Value = strBeneficiaryCust;
            cboCurrency.SelectedValue = strCurrency;
            txtPaymentMeth.Value = strPaymentMeth;
            txtApplicantCust.Value = strApplicantCust;
        }
        #endregion

        #region 조회 조건 버튼 클릭시
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
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00034", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "가격조건");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "가격조건 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
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

        //수출자
        private void btnBeneficiaryCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBeneficiaryCust.Text, "P");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수출자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //결재방법
        private void btnPaymentMeth_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPaymentMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결제방법");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPaymentMeth.Value = Msgs[0].ToString();
                    txtPaymentMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재방법 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //수입자
        private void btnApplicantCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtApplicantCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtApplicantCust.Value = Msgs[1].ToString();
                    txtApplicantCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수입자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged

        //가격조건
        private void txtCostCond_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCostCond.Text != "")
                {
                    txtCostCondNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCostCond.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S005' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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

        //결재방법
        private void txtPaymentMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPaymentMeth.Text != "")
                {
                    txtPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPaymentMeth.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S004' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPaymentMethNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //수입자
        private void txtApplicantCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtApplicantCust.Text != "")
                {
                    txtApplicantCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtApplicantCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtApplicantCustNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region radio CheckedChanged
        private void rdoPoNo_Y_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoPoNo_Y.Checked == true) Set_Tag(";1;;");
        }

        private void rdoPoNo_N_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoPoNo_N.Checked == true) Set_Tag("2");
        }


        private void Set_Tag(string div)
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            if (div == ";1;;")
            {
                txtPoNo.Tag = ";1;;";

                cboCurrency.Tag = ";2;;";
                txtPurDuty.Tag = ";2;;";
                txtBeneficiaryCust.Tag = ";2;;";
                txtPaymentMeth.Tag = ";2;;";
                txtApplicantCust.Tag = ";2;;";
                txtCostCond.Tag = ";2;;";
                txtProjectNo.Tag = ";2;;";
                txtProjectSeq.Tag = ";2;;";

                btnCostCond.Tag = ";2;;";
                btnPurDuty.Tag = ";2;;";
                btnBeneficiaryCust.Tag = ";2;;";
                btnPaymentMeth.Tag = ";2;;";
                btnApplicantCust.Tag = ";2;;";

                dtpPoDtFr.Tag = ";2;;";
                dtpPoDtTo.Tag = ";2;;";

            }
            else if (div == "2")
            {
                txtPoNo.Tag = "";

                txtCostCond.Tag = ";1;;";
                cboCurrency.Tag = ";1;;";
                txtPurDuty.Tag = ";1;;";
                txtBeneficiaryCust.Tag = ";1;;";
                txtPaymentMeth.Tag = ";1;;";
                txtApplicantCust.Tag = ";1;;";
                txtProjectNo.Tag = "";
                txtProjectSeq.Tag = "";

                btnCostCond.Tag = "";
                btnPurDuty.Tag = "";
                btnBeneficiaryCust.Tag = "";
                btnPaymentMeth.Tag = "";
                btnApplicantCust.Tag = "";

                dtpPoDtFr.ReadOnly = false;
                dtpPoDtTo.ReadOnly = false;

                dtpPoDtFr.Tag = "";
                dtpPoDtTo.Tag = "";
                //기타 세팅
                dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            }
            else
            {
                panel2.Enabled = false;
                txtPoNo.Tag = "";

                cboCurrency.Tag = ";2;;";
                txtPurDuty.Tag = ";2;;";
                txtBeneficiaryCust.Tag = ";2;;";
                txtPaymentMeth.Tag = ";2;;";
                txtApplicantCust.Tag = ";2;;";
                txtCostCond.Tag = ";2;;";
                txtProjectNo.Tag = "";
                txtProjectSeq.Tag = "";

                btnCostCond.Tag = ";2;;";
                btnPurDuty.Tag = ";2;;";
                btnBeneficiaryCust.Tag = ";2;;";
                btnPaymentMeth.Tag = ";2;;";
                btnApplicantCust.Tag = ";2;;";

                dtpPoDtFr.Tag = "";
                dtpPoDtTo.Tag = "";
                //기타 세팅
                dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            }

            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_MBL001  @pTYPE = 'P1'";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "' ";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtBeneficiaryCust.Text + "' ";
                    strQuery += ", @pAPPLICANT_CUST = '" + txtApplicantCust.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";
                    strQuery += ", @pCURRENCY = '" + cboCurrency.SelectedValue + "' ";
                    strQuery += ", @pCOST_COND = '" + txtCostCond.Text + "' ";
                    strQuery += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, Msg);

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }

        private void butOk_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");
                    int iRow = -1;
                    int j = spd.Sheets[0].Rows.Count;
                    decimal iBlAmt = 0;
                    decimal iBlAmtLoc = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                        {
                            spd.ActiveSheet.ActiveRowIndex = j;
                            UIForm.FPMake.RowInsert(spd);

                            spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                            spd.Sheets[0].Cells[j, 2].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                            spd.Sheets[0].Cells[j, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
                            spd.Sheets[0].Cells[j, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                            spd.Sheets[0].Cells[j, 5].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value;
                            spd.Sheets[0].Cells[j, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고요구일")].Text;
                            spd.Sheets[0].Cells[j, 7].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경입고요구일")].Text;    // 2022.06.09. hma 추가

                            spd.Sheets[0].Cells[j, 8].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주잔량")].Value;        // 2022.06.09. hma 수정: 7=>8로
                            spd.Sheets[0].Cells[j, 9].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;            // 2022.06.09. hma 수정: 8=>9로

                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주잔량")].Value) == 0)
                            {
                                iBlAmt = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value);
                                iBlAmtLoc = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value);
                            }
                            else
                            {
                                iBlAmt = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주잔량")].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
                                iBlAmtLoc = iBlAmt * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);
                            }
                            spd.Sheets[0].Cells[j, 10].Value = iBlAmt;           // 2022.06.09. hma 수정: 9=>10으로
                            spd.Sheets[0].Cells[j, 11].Value = iBlAmtLoc;        // 2022.06.09. hma 수정: 10=>11로
                            spd.Sheets[0].Cells[j, 12].Text = "0";               // 2022.06.09. hma 수정: 11=>12로
                            spd.Sheets[0].Cells[j, 13].Text = "0";               // 2022.06.09. hma 수정: 12=>13으로
                            spd.Sheets[0].Cells[j, 14].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text;     // 2022.06.09. hma 수정: 13=>14로
                            spd.Sheets[0].Cells[j, 15].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text;     // 2022.06.09. hma 수정: 14=>15로
                            spd.Sheets[0].Cells[j, 16].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text; // 2022.06.09. hma 수정: 15=>16으로
                            spd.Sheets[0].Cells[j, 17].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text;   // 2022.06.09. hma 수정: 16=>17로
                            spd.Sheets[0].Cells[j, 18].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;         // 2022.06.09. hma 수정: 17=>18로
                            j++;

                            iRow = i;
                        }
                    }
                    if (iRow != -1)
                        RtnStr(fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
            Close();

            if (returnVal != null)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.Cancel;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            Close();
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string strCode)
        {
            returnVal = strCode;
        }
        #endregion
        
    }
}
