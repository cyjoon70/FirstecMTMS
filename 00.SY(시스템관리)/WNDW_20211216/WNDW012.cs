#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 프로젝트정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-05
// 작성내용 : 프로젝트정보조회
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
    WNDW.WNDW012 pu = new WNDW.WNDW012();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 수주정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 수주번호 </para>
    /// </summary>

    public partial class WNDW012 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수

        #endregion

        #region WNDW012 생성자
        public WNDW012()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW012_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "확정여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B034', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 0);//확정여부

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅	
            dtpSoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpSoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            Grid_Search(false);

            this.Text = "수주정보 조회";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strCfmYn = "";
                try
                {
                    SDown = 1;

                    if (rdoCfmYes.Checked == true) strCfmYn = "Y";
					else if(rdoCfmNo.Checked == true) strCfmYn = "N";

					string strQuery = "usp_WNDW012 @pTYPE = 'S1'";
					strQuery += ", @pSO_DT_FR = '" + dtpSoDtFr.Text + "'";
					strQuery += ", @pSO_DT_TO = '" + dtpSoDtTo.Text + "'";
					strQuery += ", @pSOLD_CUST = '" + txtSoldCust.Text + "'";
					strQuery += ", @pSALE_DUTY = '" + txtSaleDuty.Text + "'";
					strQuery += ", @pSO_TYPE = '" + txtSoType.Text + "'";
					strQuery += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "'";
					strQuery += ", @pSO_CONFIRM_YN = '" + strCfmYn + "'";
					strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
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

        #region 조회조건 팝업
        //주문처
        private void btnSoldCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSoldCust.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSoldCust.Text = Msgs[1].ToString();
                    txtSoldCustNm.Value = Msgs[2].ToString();
                    txtSoldCust.Focus();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }

        //영업담당자
        private void btnSaleDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON 'S011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSaleDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "영업담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSaleDuty.Text = Msgs[0].ToString();
                    txtSaleDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "영업담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //수주형태
        private void btnSoType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1='SO_TYPE', @pSPEC2 = 'SO_TYPE_NM', @pSPEC3 = 'S_SO_TYPE', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSoType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수주형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSoType.Text = Msgs[0].ToString();
                    txtSoTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //결제방법
        private void btnPaymentMeth_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPaymentMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결제방법");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPaymentMeth.Text = Msgs[0].ToString();
                    txtPaymentMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결제방법 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region TextBox코드입력시 코드명 자동입력
        //주문처
        private void txtSoldCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSoldCust.Text != "")
                {
                    txtSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtSoldCustNm.Value = "";
                }
            }
            catch { }
        }

        //영업담당자
        private void txtSaleDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSaleDuty.Text != "")
                {
                    txtSaleDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSaleDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtSaleDutyNm.Value = "";
                }
            }
            catch { }
        }

        //수주형태
        private void txtSoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSoType.Text != "")
                {
                    txtSoTypeNm.Value = SystemBase.Base.CodeName("SO_TYPE", "SO_TYPE_NM", "S_SO_TYPE", txtSoType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtSoTypeNm.Value = "";
                }
            }
            catch { }
        }

        //결제방법
        private void txtPaymentMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPaymentMeth.Text != "")
                {
                    txtPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPaymentMeth.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S004'  AND COMP_CODE  = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtPaymentMethNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            string strCfmYn = "";

            if (rdoCfmYes.Checked == true) strCfmYn = "Y";
            else if (rdoCfmNo.Checked == true) strCfmYn = "N";

            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                int cnt_prev = AddRow * SDown;
                SDown++;
                int cnt = AddRow * SDown;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = "usp_WNDW012 @pTYPE = 'S1'";
                strQuery += ", @pSO_DT_FR = '" + dtpSoDtFr.Text + "'";
                strQuery += ", @pSO_DT_TO = '" + dtpSoDtTo.Text + "'";
                strQuery += ", @pSOLD_CUST = '" + txtSoldCust.Text + "'";
                strQuery += ", @pSALE_DUTY = '" + txtSaleDuty.Text + "'";
                strQuery += ", @pSO_TYPE = '" + txtSoType.Text + "'";
                strQuery += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "'";
                strQuery += ", @pSO_CONFIRM_YN = '" + strCfmYn + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pTOPCOUNT ='" + cnt + "'";
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
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }
        private void txtProjectNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }
        private void txtEntCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }
        #endregion
    }
}
