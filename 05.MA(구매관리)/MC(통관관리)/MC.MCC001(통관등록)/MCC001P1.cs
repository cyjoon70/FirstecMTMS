#region 작성정보
/*********************************************************************/
// 단위업무명 : 통관등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 통관등록 및 관리
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

namespace MC.MCC001
{
    public partial class MCC001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread spd;
        string[] returnVal = null;
        string strBtn = "N";
        #endregion

        #region 생성자
        public MCC001P1(FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            spd = spread;
        }

        public MCC001P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void MCC001P1_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            this.Text = "B/L참조";
                        
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            // 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //화폐단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            rdoBlNo_Y.Checked = true;

            dtpOpenDtFr.Text = "";
            dtpOpenDtTo.Text = "";

            Set_Tag("조회구분;1;;");	
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
                    string strQuery = " usp_MCC001  @pTYPE = 'P1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pBL_NO  = '" + txtBlNo.Text.Trim() + "' ";
                    strQuery += ", @pBL_RECEIPT_DT_FR = '" + dtpOpenDtFr.Text + "' ";
                    strQuery += ", @pBL_RECEIPT_DT_TO = '" + dtpOpenDtTo.Text + "' ";
                    strQuery += ", @pBENEFICIARY_CUST = '" + txtBeneficiaryCust.Text + "' ";
                    strQuery += ", @pAPPLICANT_CUST = '" + txtApplicantCust.Text + "' ";
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
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void butOk_Click(object sender, System.EventArgs e)
        {
            int col_sel = SystemBase.Base.GridHeadIndex(GHIdx1, "선택");
            string strTop = "Y";

            try
            {
                int j = spd.Sheets[0].Rows.Count;
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, col_sel].Text == "True")
                    {
                        if (strTop == "Y") RtnStr(i);
                        strTop = "N";

                        spd.ActiveSheet.ActiveRowIndex = j;
                        UIForm.FPMake.RowInsert(spd);

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Value;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Value;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Value;

                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L번호")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "송장번호")].Text;
                        spd.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L순번")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "송장순번")].Text;
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
            returnVal = new string[21];
            for (int i = 16; i < fpSpread1.Sheets[0].Columns.Count; i++)
            {
                returnVal[i - 16] = fpSpread1.Sheets[0].Cells[Row, i].Value.ToString();
            }
        }
        #endregion

        #region 조회조건팝업
        //B/L번호
        private void btnBlNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW022 pu = new WNDW.WNDW022();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBlNo.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "송장번호(구B/L번호)정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 조회");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
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

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결재방법 조회");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
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
                string[] strSearch = new string[] { "", "" };

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
        #endregion

        #region 조회조건 TextChanged
        //가격조건
        private void txtCostCond_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
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
                if (strBtn == "N")
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
            }
            catch
            {

            }
        }

        //수출자
        private void txtBeneficiaryCust_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtBeneficiaryCustNm.Text = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBeneficiaryCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //결재방법
        private void txtPaymentMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
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
        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion

        #region radio CheckedChanged
        private void rdoBlNo_Y_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoBlNo_Y.Checked == true) Set_Tag("조회구분;1;;");
        }

        private void rdoBlNo_N_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoBlNo_N.Checked == true) Set_Tag(";2;;");
        }

        private void Set_Tag(string div)
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            if (div == "조회구분;1;;")
            {
                txtBlNo.Tag = "송장번호;1;;";
                btnBlNo.Tag = ";5;;";

                txtCostCond.Tag = ";2;;";
                btnCostCond.Tag = ";2;;";

                cboCurrency.Tag = ";2;;";

                txtPurDuty.Tag = ";2;;";
                btnPurDuty.Tag = ";2;;";

                txtBeneficiaryCust.Tag = ";2;;";
                btnBeneficiaryCust.Tag = ";2;;";

                txtPaymentMeth.Tag = ";2;;";
                btnPaymentMeth.Tag = ";2;;";

                txtApplicantCust.Tag = ";2;;";
                btnApplicantCust.Tag = ";2;;";

                dtpOpenDtFr.Tag = ";2;;";
                dtpOpenDtTo.Tag = ";2;;";

                txtProjectNo.Tag = ";2;;";
                btnProj.Tag = ";2;;";

                txtProjectSeq.Tag = ";2;;";
                btnProjSeq.Tag = ";2;;";
            }
            else
            {
                txtBlNo.Tag = ";2;;";
                btnBlNo.Tag = ";2;;";

                txtCostCond.Tag = "가격조건;1;;";
                btnCostCond.Tag = ";5;;";

                cboCurrency.Tag = "화폐단위;1;;";

                txtPurDuty.Tag = "구매담당자;1;;";
                btnPurDuty.Tag = ";5;;";

                txtBeneficiaryCust.Tag = "수출자;1;;";
                btnBeneficiaryCust.Tag = ";5;;";

                txtPaymentMeth.Tag = "결재방법;1;;";
                btnPaymentMeth.Tag = ";5;;";

                txtApplicantCust.Tag = "수입자;1;;";
                btnApplicantCust.Tag = ";5;;";

                dtpOpenDtFr.Tag = "개설일자;1;;";
                dtpOpenDtTo.Tag = "개설일자;1;;";

                txtProjectNo.Tag = ";5;;";
                btnProj.Tag = ";5;;";

                txtProjectSeq.Tag = ";5;;";
                btnProjSeq.Tag = ";5;;";

                dtpOpenDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpOpenDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            }
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크
        }
        #endregion

    }
}
