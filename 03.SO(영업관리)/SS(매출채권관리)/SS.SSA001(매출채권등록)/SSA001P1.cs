#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주/출고참조
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-26
// 작성내용 : 매출채권등록에서 수주/출고참조팝업
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

namespace SS.SSA001
{
    public partial class SSA001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBnNo = "", strCustCd = "";
        DataTable dt = null;
        #endregion

        #region 생성자
        public SSA001P1(string BnNo, string CustCd)
        {
            strBnNo = BnNo;
            strCustCd = CustCd;

            InitializeComponent();
        }

        public SSA001P1()
        {
            InitializeComponent();
        }
        #endregion

        #region SSA001P1 Form Load 이벤트
        private void SSA001P1_Load(object sender, EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //GropBox1 조회조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboBnType, "usp_S_COMMON @pTYPE = 'S050', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//매출형태
            SystemBase.ComboMake.C1Combo(cboPaymentMeth, "usp_B_COMMON @pType='COMM', @pCODE = 'S004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", true);//결재방법
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", true);//화폐단위
            SystemBase.ComboMake.C1Combo(cboSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당
            SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//VAT유형

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//출고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpDnDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpDnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            panel3.Enabled = false;
            cboVatType.BackColor = SystemBase.Validation.Kind_Gainsboro;

            //발주처
            if (strCustCd != "")
            {
                txtSoldCustCd.Text = strCustCd;
                txtSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", strCustCd, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            }
            else
            {
                txtSoldCustCd.Text = "";
                txtSoldCustNm.Value = "";
            }

            this.Text = "수주/출고 정보 조회";
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
                    //주문처 유효성체크
                    if (txtSoldCustCd.Text != "" && txtSoldCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "주문처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                        txtSoldCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }
                    //납품처 유효성체크
                    if (txtShipCustCd.Text != "" && txtShipCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "납품처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                        txtShipCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    string strType = "";
                    if (rdoDn.Checked == true) { strType = "S4"; }
                    else { strType = "S5"; }

                    string strVatIncFlag = "1";
                    if (rdoExtra.Checked == true) { strVatIncFlag = "2"; }
                    else { strVatIncFlag = "1"; }

                    string strQuery = " usp_SSA001  @pTYPE = '" + strType + "', @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
                    strQuery += ", @pDN_DT_FR = '" + dtpDnDtFr.Text + "' ";
                    strQuery += ", @pDN_DT_TO = '" + dtpDnDtTo.Text + "' ";
                    strQuery += ", @pBN_TYPE = '" + cboBnType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSOLD_CUST = '" + txtSoldCustCd.Text + "' ";
                    strQuery += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                    strQuery += ", @pPAYMENT_METH = '" + cboPaymentMeth.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSHIP_CUST = '" + txtShipCustCd.Text + "' ";
                    strQuery += ", @pSHIP_CUST_NM = '" + txtShipCustNm.Text + "' ";
                    strQuery += ", @pVAT_TYPE = '" + cboVatType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pPROJECT_NO= '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                    strQuery += ", @pVAT_INC_FLAG = '" + strVatIncFlag + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);

                    if (rdoDn.Checked == true)
                    {
                        fpSpread1.Sheets[0].Columns[2].Width = 110;
                        fpSpread1.Sheets[0].Columns[3].Width = 60;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "출고단위";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "출고수량";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Text = "출고금액";
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Columns[2].Width = 0;
                        fpSpread1.Sheets[0].Columns[3].Width = 0;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "수주단위";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 8].Text = "수주수량";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, 9].Text = "수주금액";
                    }

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
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
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

        //납품처
        private void btnShipCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtShipCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtShipCustCd.Text = Msgs[1].ToString();
                    txtShipCustNm.Value = Msgs[2].ToString();
                    txtShipCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "납품처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //주문처
        private void btnSoldCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtShipCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoldCustCd.Text = Msgs[1].ToString();
                    txtSoldCustNm.Value = Msgs[2].ToString();
                    txtSoldCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "주문처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch { }
        }
        //납품처
        private void txtShipCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtShipCustCd.Text != "")
                {
                    txtShipCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtShipCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtShipCustNm.Value = "";
                }
            }
            catch { }
        }

        //납품처
        private void txtSoldCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSoldCustCd.Text != "")
                {
                    txtSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSoldCustNm.Value = "";
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
                    cboVatType.BackColor = Color.White;
                }
                else
                {
                    panel3.Enabled = true;
                    //vat유형 콤보박스 재 설정 (필수)
                    SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//VAT유형
                    cboVatType.SelectedValue = "A";	//일반세금계산서
                    cboVatType.BackColor = Color.LightCyan;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }

        }
        #endregion

        #region 라디오버튼 CheckedChanged
        //참조대상 변경
        private void rdoDn_CheckedChanged(object sender, System.EventArgs e)
        {
            Set_Titile();
        }

        private void rdoSo_CheckedChanged(object sender, System.EventArgs e)
        {
            Set_Titile();
        }

        private void Set_Titile()
        {
            if (rdoDn.Checked == true)
            {
                c1Label1.Text = "출고일자";
            }
            else
            {
                c1Label1.Text = "수주일자";
            }
        }
        #endregion

        #region DataTable GetSet
        public DataTable ReturnDt { get { return dt; } set { dt = value; } }
        #endregion
    }
}
