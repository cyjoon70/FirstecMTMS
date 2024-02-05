#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 출하정보팝업
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-06
// 작성내용 : 출하정보팝업
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
    WNDW.WNDW013 pu = new WNDW.WNDW013();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 출고정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 출고번호 </para>
    /// </summary>

    public partial class WNDW013 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        #endregion

        #region 생성자
        public WNDW013()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW013_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅
            dtpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDeliveryDtFr.Value = null;
            dtpDeliveryDtTo.Value = null;
            rdoIssueY.Checked = true;

            Grid_Search(false);

            this.Text = "출고정보조회";
        }
        #endregion

        #region 조회조건 팝업
        //납품처
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "납품처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //출고형태
        private void btnMoveType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_S_COMMON @pTYPE = 'S090', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMoveTypeCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "출고형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMoveTypeCd.Text = Msgs[0].ToString();
                    txtMoveTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

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
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //납품처
        private void txtCust_TextChanged(object sender, System.EventArgs e)
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

        //출고형태
        private void txtMoveTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMoveTypeCd.Text != "")
                {
                    txtMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtMoveTypeCd.Text, " AND TRAN_TYPE = 'DI'  AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtMoveTypeNm.Value = "";
                }
            }
            catch { }
        }

        //프로젝트
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드 조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string issue = "";

                try
                {
                    SDown = 1;

                    if (rdoIssueY.Checked == true) issue = "Y";
                    else if (rdoIssueN.Checked == true) issue = "N";

                    string strQuery = " usp_WNDW013 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pDN_YN ='" + issue + "'";
                    strQuery += ", @pMOVE_TYPE ='" + txtMoveTypeCd.Text + "'";
                    strQuery += ", @pSHIP_CUST  ='" + txtCustCd.Text.Trim() + "'";
                    strQuery += ", @pSALE_DUTY ='" + txtSaleDuty.Text + "'";
                    strQuery += ", @pREQ_DT_FR  ='" + dtpDtFr.Text + "'";
                    strQuery += ", @pREQ_DT_TO ='" + dtpDtTo.Text + "'";
                    strQuery += ", @pDELIVERY_DT_FR  ='" + dtpDeliveryDtFr.Text + "'";
                    strQuery += ", @pDELIVERY_DT_TO ='" + dtpDeliveryDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text + "'";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string issue = "";

            if (rdoIssueY.Checked == true) issue = "Y";
            else if (rdoIssueN.Checked == true) issue = "N";

            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                int cnt_prev = AddRow * SDown;
                SDown++;
                int cnt = AddRow * SDown;

                string strQuery = " usp_WNDW013 'S1'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pDN_YN ='" + issue + "'";
                strQuery += ", @pMOVE_TYPE ='" + txtMoveTypeCd.Text + "'";
                strQuery += ", @pSHIP_CUST  ='" + txtCustCd.Text.Trim() + "'";
                strQuery += ", @pSALE_DUTY ='" + txtSaleDuty.Text + "'";
                strQuery += ", @pREQ_DT_FR  ='" + dtpDtFr.Text + "'";
                strQuery += ", @pREQ_DT_TO ='" + dtpDtTo.Text + "'";
                strQuery += ", @pDELIVERY_DT_FR  ='" + dtpDeliveryDtFr.Text + "'";
                strQuery += ", @pDELIVERY_DT_TO ='" + dtpDeliveryDtTo.Text + "'";
                strQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text + "'";	
                strQuery += ", @pTOPCOUNT ='" + cnt + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }

            this.Cursor = Cursors.Default;
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
                    returnVal[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                }
            }
        }
        #endregion	
    }
}
