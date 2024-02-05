#region 작성정보
/*********************************************************************/
// 단위업무명 : L/C등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-25
// 작성내용 : L/C등록 및 관리
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

namespace ML.MLC001
{
    public partial class MLC001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        FarPoint.Win.Spread.FpSpread spd;
        string[] returnVal = null;
        #endregion

        #region 생성자
        public MLC001P1()
        {
            InitializeComponent();
        }

        public MLC001P1(FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            spd = spread;
        }
        #endregion

        #region 폼로드 이벤트
        private void MLC001P1_Load(object sender, System.EventArgs e)
        { 
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "발주참조팝업";
                        
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);//화폐단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpPoDtFr.Value = "";
            dtpPoDtTo.Value = "";

            Set_Tag(";1;;");	
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_MLC001  @pTYPE = 'P1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "' ";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtBeneficiaryCust.Text + "' ";
                    strQuery += ", @pAPPLICANT_CUST = '" + txtApplicantCustNm.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";
                    strQuery += ", @pCURRENCY = '" + cboCurrency.SelectedValue + "' ";
                    strQuery += ", @pCOST_COND = '" + txtCostCond.Text + "' ";
                    strQuery += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");
            string strTop = "Y";
            decimal qty;
            decimal price;
            decimal amt;
            decimal loc_amt;
            decimal xrate = 0;
            try
            {
                int j = spd.Sheets[0].Rows.Count;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                    {
                        qty = 0; amt = 0; loc_amt = 0; price = 0;

                        if (strTop == "Y")
                        {
                            RtnStr(i);
                            xrate = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);
                        }
                        strTop = "N";
                        spd.Sheets[0].Rows.Count = j + 1;
                        spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주잔량")].Value;

                        qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주잔량")].Value);
                        price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
                        amt = qty * price;
                        loc_amt = amt * xrate;

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C금액")].Value = amt;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C자국금액")].Value = loc_amt;

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                        j++;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Close();
            this.DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            Close();
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region 값 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int Row)
        {
            returnVal = new string[22];
            for (int i = 16; i < fpSpread1.Sheets[0].Columns.Count; i++)
            {
                returnVal[i - 16] = fpSpread1.Sheets[0].Cells[Row, i].Value.ToString();
            }
        }
        #endregion

        #region 버튼 Click  TextChanged
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

        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnPaymentMeth_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S004' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPaymentMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결제방법 팝업");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnCostCond_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S005' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCostCond.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00034", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "가격조건 팝업");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnBeneficiaryCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002("P");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnApplicantCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002("P");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text, "N");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectSeq.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
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
            strBtn = "N";
        }


        private void txtPaymentMeth_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtPaymentMeth.Text != "")
                    {
                        txtPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPaymentMeth.Text, " AND MAJOR_CD = 'S004' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPaymentMethNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtCostCond_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
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
            }
            catch
            {

            }
        }

        private void txtBeneficiaryCust_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
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
            }
            catch
            {

            }
        }

        private void txtApplicantCust_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
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
            }
            catch
            {

            }
        }

        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N" || txtProjectNo.Text == "" || txtProjectSeq.Text != "*")
                txtProjectSeq.Value = "";
        }

        private void txtPurDuty_Leave(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N" && txtPurDuty.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
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
                    else
                    {
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //구매담당자가 아닙니다
                        txtPurDuty.Value = "";
                        txtPurDutyNm.Value = "";
                        txtPurDuty.Focus();
                    }
                }                
            }
            catch
            {

            }
        }

        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion
        
        #region radio CheckedChanged
        private void rdoPoNo_Y_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoPoNo_Y.Checked == true) Set_Tag(";1;;");
        }

        private void rdoPoNo_N_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoPoNo_N.Checked == true) Set_Tag(";2;;");
        }

        private void Set_Tag(string div)
        {

            if (div == ";1;;")
            {
                txtPoNo.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtPoNo.Tag = ";1;;";
                txtPoNo.Enabled = true;
                btnPoNo.Enabled = true;

                dtpPoDtFr.Value = "";
                dtpPoDtTo.Value = "";

                txtCostCond.Value = "";
                txtCostCondNm.Value = "";
                cboCurrency.SelectedIndex = 0;
                txtPurDuty.Value = "";
                txtPurDutyNm.Value = "";
                txtBeneficiaryCust.Value = "";
                txtBeneficiaryCustNm.Value = "";
                txtProjectNo.Value = "";
                txtPaymentMeth.Value = "";
                txtPaymentMethNm.Value = "";
                txtApplicantCust.Value = "";
                txtApplicantCustNm.Value = "";
                txtProjectSeq.Value = "";

                txtCostCond.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtCostCond.Tag = ";2;;";
                txtCostCond.Enabled = false;
                btnCostCond.Enabled = false;

                cboCurrency.BackColor = SystemBase.Validation.Kind_Gainsboro;
                cboCurrency.Tag = ";2;;";
                cboCurrency.Enabled = false;

                txtPurDuty.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtPurDuty.Tag = ";2;;";
                txtPurDuty.Enabled = false;
                btnPurDuty.Enabled = false;

                txtBeneficiaryCust.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtBeneficiaryCust.Tag = ";2;;";
                txtBeneficiaryCust.Enabled = false;
                btnBeneficiaryCust.Enabled = false;

                txtPaymentMeth.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtPaymentMeth.Tag = ";2;;";
                txtPaymentMeth.Enabled = false;
                btnPaymentMeth.Enabled = false;

                dtpPoDtFr.Tag = ";2;;";
                dtpPoDtFr.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtpPoDtFr.Enabled = false;

                dtpPoDtTo.Tag = ";2;;";
                dtpPoDtTo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtpPoDtTo.Enabled = false;

                txtProjectNo.Tag = ";2;;";
                txtProjectNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtProjectNo.Enabled = false;
                btnProj.Enabled = false;

                txtProjectSeq.Tag = ";2;;";
                txtProjectSeq.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtProjectSeq.Enabled = false;
                btnProjSeq.Enabled = false;

                txtApplicantCust.Tag = ";2;;";
                txtApplicantCust.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtApplicantCust.Enabled = false;
                btnApplicantCust.Enabled = false;

            }
            else
            {
                txtPoNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtPoNo.Tag = ";2;;";
                txtPoNo.Enabled = false;
                btnPoNo.Enabled = false;
                txtPoNo.Text = "";

                txtCostCond.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtCostCond.Tag = ";1;;";
                txtCostCond.Enabled = true;
                btnCostCond.Enabled = true;

                cboCurrency.BackColor = SystemBase.Validation.Kind_LightCyan;
                cboCurrency.Tag = ";1;;";
                cboCurrency.Enabled = true;

                txtPurDuty.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtPurDuty.Tag = ";1;;";
                txtPurDuty.Enabled = true;
                btnPurDuty.Enabled = true;

                txtBeneficiaryCust.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtBeneficiaryCust.Tag = ";1;;";
                txtBeneficiaryCust.Enabled = true;
                btnBeneficiaryCust.Enabled = true;

                txtPaymentMeth.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtPaymentMeth.Tag = ";1;;";
                txtPaymentMeth.Enabled = true;
                btnPaymentMeth.Enabled = true;

                txtProjectNo.Tag = "";
                txtProjectNo.BackColor = SystemBase.Validation.Kind_White;
                txtProjectNo.Enabled = true;
                btnProj.Enabled = true;

                txtProjectSeq.Tag = "";
                txtProjectSeq.BackColor = SystemBase.Validation.Kind_White;
                txtProjectSeq.Enabled = true;
                btnProjSeq.Enabled = true;

                txtApplicantCust.Tag = "";
                txtApplicantCust.BackColor = SystemBase.Validation.Kind_White;
                txtApplicantCust.Enabled = true;
                btnApplicantCust.Enabled = true;

                dtpPoDtFr.Tag = ";1;;";
                dtpPoDtFr.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtpPoDtFr.Enabled = true;

                dtpPoDtTo.Tag = ";1;;";
                dtpPoDtTo.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtpPoDtTo.Enabled = true;

                //기타 세팅
                dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);
            }
        }
        #endregion

    }
}
