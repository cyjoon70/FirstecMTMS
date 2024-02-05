#region 작성정보
/*********************************************************************/
// 단위업무명 : 매입등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 매입등록 및 관리
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

namespace MV.MIV001
{
    public partial class MIV001P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        FarPoint.Win.Spread.FpSpread spd;
        string[] returnVal = null;
        string rdochk = "1";
        bool CHK = false; //내자 외자 구분
        string strVatType = "";
        #endregion

        #region 생성자
        public MIV001P1()
        {
            InitializeComponent();
        }

        public MIV001P1(FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            spd = spread;
        }

        public MIV001P1(FarPoint.Win.Spread.FpSpread spread, string VatType)
        {
            InitializeComponent();
            spd = spread;
            strVatType = VatType;
        }

        public MIV001P1(FarPoint.Win.Spread.FpSpread spread, bool chk, string VatType)
        {
            InitializeComponent();
            spd = spread;
            CHK = chk;
            strVatType = VatType;
        }
        #endregion   

        #region 폼로드 이벤트
        private void MIV001P1_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의(조회권한만)
            UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수 체크
            if (CHK == true)
            {
                this.Text = "매입대상 참조(외주)";
                label10.Visible = true;
                dtpReportDtFr.Visible = true;
                label9.Visible = true;
                dtpReportDtTo.Visible = true;
                panel2.Enabled = false;

                // 2016.09.21. hma 추가(Start): 외주일 경우에만 Release일자가 나오도록 처리
                lblReleaseDt.Visible = true;
                dtpReleaseDtFr.Visible = true;
                lblReleaseDtPeriod.Visible = true;
                dtpReleaseDtTo.Visible = true;
                // 2016.09.21. hma 추가(End)

                rdoPo.Checked = true;

                panel1.Height = 179;        // 2016.09.21. hma 추가: 검색조건 입력 판넬 높이 지정

                rdochk = "1";
                Set_Tag(";1;;");
            }
            else
            {
                this.Text = "매입대상 참조";
                label10.Visible = false;
                dtpReportDtFr.Visible = false;
                label9.Visible = false;
                dtpReportDtTo.Visible = false;

                // 2016.09.21. hma 추가(Start): 외주일 경우에만 Release일자가 나오도록 처리
                lblReleaseDt.Visible = false;
                dtpReleaseDtFr.Visible = false;
                lblReleaseDtPeriod.Visible = false;
                dtpReleaseDtTo.Visible = false;
                // 2016.09.21. hma 추가(End)

                rdoIm.Checked = true;

                panel1.Height = 152;        // 2016.09.21. hma 추가: 검색조건 입력 판넬 높이 지정

                rdochk = "2";
                Set_Tag(";1;;");
            }

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            rdoNo.Checked = true;

            

            Set_Titile();
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
                    string strType = "";
                    if (CHK == false)
                    {
                        if (rdochk == "1") { strType = "P11"; }
                        else if (rdochk == "2") { strType = "P12"; }
                    }
                    else
                    {
                        strType = "P13";
                    }

                    if (rdoNo.Checked == true)
                    {
                        if (txtPoNo.Text == "" && txtRcptNo.Text == "")
                        {
                            MessageBox.Show("발주번호 또는 입고번호 둘 중 하나는 입력되어야 합니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Cursor = System.Windows.Forms.Cursors.Default;
                            return;
                        }
                    }

                    string strQuery = " usp_MIV001  @pTYPE = '" + strType + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "' ";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "' ";
                    strQuery += ", @pPO_NO = '" + txtPoNo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pPO_TYPE = '" + txtPoType.Text + "' ";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pRCPT_NO = '" + txtRcptNo.Text + "' ";

                    strQuery += ", @pVAT_TYPE = '" + txtVatType.Text + "' ";

                    if (CHK == true)        // 외주인 경우
                    {
                        strQuery += ", @pREPORT_DT_FR = '" + dtpReportDtFr.Text + "' ";
                        strQuery += ", @pREPORT_DT_TO = '" + dtpReportDtTo.Text + "' ";
                        // 2016.09.21. hma 추가(Start): Release일자 검색조건 추가하여 매개변수로 넘기도록 함
                        strQuery += ", @pRELEASE_DT_FR = '" + dtpReleaseDtFr.Text + "' ";
                        strQuery += ", @pRELEASE_DT_TO = '" + dtpReleaseDtTo.Text + "' ";
                        // 2016.09.21. hma 추가(End)
                    }
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    //spread header 변경
                    if (rdochk == "1")
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text = "발주번호";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text = "발주순번";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Text = "발주수량";
                    }
                    else
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text = "입고번호";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text = "입고순번";
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Text = "입고수량";
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
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

                        spd.Sheets[0].Rows.Count = j + 1;
                        spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";

                        spd.Sheets[0].Cells[j, 2].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value;
                        spd.Sheets[0].Cells[j, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                        spd.Sheets[0].Cells[j, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
                        spd.Sheets[0].Cells[j, 5].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;
                        spd.Sheets[0].Cells[j, 8].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;
                        spd.Sheets[0].Cells[j, 9].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미매입량")].Value;

                        if (strTop == "Y")
                        {
                            returnVal[8] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text;
                            returnVal[9] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text;
                            returnVal[10] = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text;
                        }

                        spd.Sheets[0].Cells[j, 10].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;

                        spd.Sheets[0].Cells[j, 11].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value;
                        spd.Sheets[0].Cells[j, 12].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value;
                        spd.Sheets[0].Cells[j, 13].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Text;
                        spd.Sheets[0].Cells[j, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                        spd.Sheets[0].Cells[j, 7].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;
                        spd.Sheets[0].Cells[j, 20].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Text;

                        if (rdochk == "2")
                        {
                            spd.Sheets[0].Cells[j, 23].Text = fpSpread1.Sheets[0].Cells[i, 3].Text;   //입고번호
                            spd.Sheets[0].Cells[j, 24].Text = fpSpread1.Sheets[0].Cells[i, 4].Text;   //입고순번
                        }
                        spd.Sheets[0].Cells[j, 21].Text = fpSpread1.Sheets[0].Cells[i, 29].Text;
                        spd.Sheets[0].Cells[j, 22].Text = fpSpread1.Sheets[0].Cells[i, 30].Text;

                        strTop = "N";
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

        private void btnVatType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B040' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtVatType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00032", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "VAT유형 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtVatType.Value = Msgs[0].ToString();
                    txtVatTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region 값 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int Row)
        {
            returnVal = new string[11];

            for (int i = 21; i < fpSpread1.Sheets[0].Columns.Count - 4; i++)
            {
                returnVal[i - 21] = fpSpread1.Sheets[0].Cells[Row, i].Text.ToString();
            }
        }
        #endregion

        #region 조회조건 팝업
        private void btnPoNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                try
                {
                    WNDW018 pu = new WNDW018();
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtPoNo.Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRcpt_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW019 pu = new WNDW.WNDW019();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtRcptNo.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매입고정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPoType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                if (rdochk == "1")
                {
                    string strQuery = "";

                    if (CHK == true) { strQuery = " usp_MIV001 'P4'  , @pSPEC1 ='Y', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"; }
                    else { strQuery = " usp_MIV001 'P4'  , @pSPEC1 ='N', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"; }

                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtPoType.Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "발주형태 팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        txtPoType.Value = Msgs[0].ToString();
                        txtPoTypeNm.Value = Msgs[1].ToString();

                    }
                }
                else
                {
                    string strQuery = " usp_M_COMMON 'M027'  , @pSPEC1 ='N' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtPoType.Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "입고형태 팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        txtPoType.Value = Msgs[0].ToString();
                        txtPoTypeNm.Value = Msgs[1].ToString();

                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
            strBtn = "N";
        }

        //거래처
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
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
            strBtn = "N";
        }

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text, "N");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Value = Msgs[3].ToString();

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

        #region  TextChanged
        private void txtPoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (rdochk == "1")
                    {
                        if (CHK == true)
                        { 
                            if (txtPoType.Text != "")
                            {
                                txtPoTypeNm.Value = SystemBase.Base.CodeName("PO_TYPE_CD", "PO_TYPE_NM", "M_PO_TYPE", txtPoType.Text, " AND IV_YN	= 'Y' AND IM_YN  = 'N' AND SUBCONTRACT_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                            }
                            else
                            {
                                txtPoTypeNm.Value = "";
                            }
                        }
                        else
                        { 
                            if (txtPoType.Text != "")
                            {
                                txtPoTypeNm.Value = SystemBase.Base.CodeName("PO_TYPE_CD", "PO_TYPE_NM", "M_PO_TYPE", txtPoType.Text, " AND IV_YN	= 'Y' AND IM_YN  = 'N' AND SUBCONTRACT_YN = 'N' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                            }
                            else
                            {
                                txtPoTypeNm.Value = "";
                            }
                        }
                    }
                    else
                    {
                        if (txtPoType.Text != "")
                        {
                            txtPoTypeNm.Value = SystemBase.Base.CodeName("IO_TYPE", "IO_TYPE_NM", "M_MVMT_TYPE", txtPoType.Text, " AND RCPT_YN	= 'Y' AND IM_YN  = 'N' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                        }
                        else
                        {
                            txtPoTypeNm.Value = "";
                        }
                    }
                }                
            }
            catch
            {

            }
        }

        //거래처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtCustCd.Text != "")
                    {
                        txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //구매담당자
        private void txtPurDuty_Leave(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtPurDuty.Text.Trim() != "")
            {
                string temp = "";
                temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                if (temp != "")
                    txtPurDutyNm.Text = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                else
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //구매담당자가 아닙니다
                    txtPurDuty.Value = "";
                    txtPurDutyNm.Value = "";
                    txtPurDuty.Focus();
                }
            }
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

        private void txtVatType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtVatType.Text != "")
                {
                    txtVatTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtVatType.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'B040' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtVatTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        #endregion

        #region radio CheckedChanged
        private void rdoPo_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoPo.Checked == true)
            {
                rdochk = "1";
                Set_Titile();

                if (rdoAll.Checked == true)
                {
                    Set_Tag(";2;;");
                }
                else
                {
                    Set_Tag(";1;;");
                }
            }
        }

        private void rdoIm_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoIm.Checked == true)
            {
                rdochk = "2";
                Set_Titile();

                if (rdoAll.Checked == true)
                {
                    Set_Tag(";2;;");
                }
                else
                {
                    Set_Tag(";1;;");
                }
            }
        }


        private void Set_Titile()
        {
            if (rdochk == "1")
            {
                label4.Text = "발주일자";
                label8.Text = "발주형태";
                txtPoType.Value = "";
            }
            else
            {
                label4.Text = "창고입고일자";     // 2016.09.19. hma 수정: 입고일자=>창고입고일자
                label8.Text = "입고형태";
                txtPoType.Value = "";
            }
        }

        private void rdoAll_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoAll.Checked == true) Set_Tag(";2;;");
        }

        private void rdoNo_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoNo.Checked == true) Set_Tag(";1;;");
        }

        private void Set_Tag(string div)
        {
            string VatType = txtVatType.Text;

            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtVatType.Value = VatType;

            if (div == ";1;;")
            {
                if (rdoPo.Checked == true)
                {
                    txtPoNo.Tag = "발주번호;1;;";
                    btnPoNo.Tag = ";5;;";

                    txtRcptNo.Tag = ";2;;";
                    btnRcpt.Tag = ";2;;";
                }
                else
                {
                    txtPoNo.Tag = "";
                    btnPoNo.Tag = ";5;;";

                    txtRcptNo.Tag = "";
                    btnRcpt.Tag = ";5;;";
                }

                dtpPoDtFr.Tag = ";2;;";
                dtpPoDtTo.Tag = ";2;;";

                txtPoType.Tag = ";2;;";
                btnPoType.Tag = ";2;;";

                txtCustCd.Tag = ";2;;";
                btnCust.Tag = ";2;;";

                txtProjectNo.Tag = ";2;;";
                btnProj.Tag = ";2;;";

                txtVatType.Tag = ";2;;";
                btnVatType.Tag = ";2;;";
                txtVatType.Value = "";
            }
            else
            {
                txtPoNo.Tag = ";2;;";
                btnPoNo.Tag = ";2;;";

                txtRcptNo.Tag = ";2;;";
                btnRcpt.Tag = ";2;;";

                dtpPoDtFr.Tag = ";1;;";
                dtpPoDtTo.Tag = ";1;;";
                dtpPoDtFr.ReadOnly = false;
                dtpPoDtTo.ReadOnly = false;
                dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
                dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

                // 2016.09.21. hma 추가(Start): Release일자 기본값 지정. 발주일자 기간을 한달이 아닌 3개월 기준으로 처리되도록 함.
                if (dtpReleaseDtFr.Visible == true)
                {
                    dtpPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString().Substring(0, 10);
                    dtpPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

                    dtpReleaseDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 10);
                    dtpReleaseDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
                }
                // 2016.09.21. hma 추가(End)

                if (rdoPo.Checked == true)
                {
                    txtPoType.Tag = "발주형태;1;;";
                    btnPoType.Tag = ";5;;";
                }
                else
                {
                    txtPoType.Tag = "입고형태;1;;";
                    btnPoType.Tag = ";5;;";
                }
                txtCustCd.Tag = "거래처;1;;";
                btnCust.Tag = ";5;;";

                txtProjectNo.Tag = ";5;;";
                btnProj.Tag = ";5;;";

                txtVatType.Tag = "VAT유형;1;;";
                btnVatType.Tag = ";5;;";
                txtVatType.Value = "A";
            }
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크
        }
        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
        #endregion

    }
}
