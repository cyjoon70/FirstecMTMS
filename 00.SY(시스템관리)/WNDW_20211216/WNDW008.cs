#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 제품오더정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-05
// 작성내용 : 제품오더정보조회
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW008 pu = new WNDW.WNDW008();
    pu.ShowDialog();
    if (pu.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

        textBox1.Text = Msgs[1].ToString();
        textBox2.Value = Msgs[2].ToString();
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 제품오더정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 제품오더번호 </para>
    /// </summary>

    public partial class WNDW008 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        string strMakeOrderNo = "";
        string strStatus = "P";
        string strItemCd = "";
        #endregion

        #region WNDW008 생성자
        public WNDW008(string MakeOrderNo)
        {
            strMakeOrderNo = MakeOrderNo;
            InitializeComponent();
        }

        public WNDW008(string MakeOrderNo, string Status)
        {
            strMakeOrderNo = MakeOrderNo;
            if (Status != "")
            { strStatus = Status; }

            InitializeComponent();
        }

        public WNDW008(string MakeOrderNo, string ItemCd, string Status)
        {
            strMakeOrderNo = MakeOrderNo;
            strItemCd = ItemCd;
            if (Status != "")
            { strStatus = Status; }

            InitializeComponent();
        }

        public WNDW008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW008_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            SystemBase.ComboMake.C1Combo(cboSTATUS, "usp_P_COMMON @pTYPE	= 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P012', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 0); //진행상태
            cboSTATUS.SelectedValue = strStatus;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtMakeOrderNo.Text = strMakeOrderNo;
            txtItemCd.Text = strItemCd;
            Grid_search(false);

            this.Text = "제품오더정보 조회";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_search(true); }
        #endregion

        #region 그리드조회
        private void Grid_search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    SDown = 1;

                    string strQuery = " usp_WNDW008 @pTYPE = 'S1'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_NM = '" + txtProjectNm.Text + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                    strQuery += ", @pITEM_NM = '" + txtItemNm.Text + "' ";
                    strQuery += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pCUST_NM = '" + txtCustNm.Text + "' ";
                    strQuery += ", @pMAKE_DT_FR = '" + dtpMakeDtFr.Text + "' ";
                    strQuery += ", @pMAKE_DT_TO = '" + dtpMakeDtTo.Text + "' ";
                    strQuery += ", @pDELV_DT_FR = '" + dtpDelvDtFr.Text + "' ";
                    strQuery += ", @pDELV_DT_TO = '" + dtpDelvDtTo.Text + "' ";
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                    strQuery += ", @pENT_NM = '" + txtEntNm.Text + "' ";
                    strQuery += ", @pMPS_STATUS = '" + cboSTATUS.SelectedValue + "' ";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업창
        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //거래처
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사업코드
        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextBox코드입력시 코드명 자동입력
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch { }
        }

        //거래처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch { }
        }

        //사업
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_WNDW008 'S1'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_NM = '" + txtProjectNm.Text + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery += ", @pITEM_NM = '" + txtItemNm.Text + "' ";
                strQuery += ", @pMAKEORDER_NO = '" + txtMakeOrderNo.Text + "' ";
                strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                strQuery += ", @pCUST_NM = '" + txtCustNm.Text + "' ";
                strQuery += ", @pMAKE_DT_FR = '" + dtpMakeDtFr.Text + "' ";
                strQuery += ", @pMAKE_DT_TO = '" + dtpMakeDtTo.Text + "' ";
                strQuery += ", @pDELV_DT_FR = '" + dtpDelvDtFr.Text + "' ";
                strQuery += ", @pDELV_DT_TO = '" + dtpDelvDtTo.Text + "' ";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                strQuery += ", @pENT_NM = '" + txtEntNm.Text + "' ";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];
                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
                }
            }
        }
        #endregion

        #region Text에서 Enter시 조회
        private void txtProjectNo_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtProjectNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtMakeOrderNo_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemCd_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtCustCd_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtCustNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtEntCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtEntNm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        #endregion
    }
}
