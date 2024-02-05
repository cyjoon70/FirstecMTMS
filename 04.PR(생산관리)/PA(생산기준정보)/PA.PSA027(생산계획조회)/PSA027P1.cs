#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주조회팦업
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-11
// 작성내용 : 수주현황조회
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

namespace PB.PSA027
{
    public partial class PSA027P1 : UIForm.FPCOMM1
    {
        string[] returnVal = null;

        public PSA027P1()
        {
            InitializeComponent();
        }
        
        #region Form Load 시
        private void PSA027P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "수주번호팝업";

            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);


            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "확정여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B034', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' ", 0);//확정여부


            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            //기타 세팅	
            dtpSoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSoDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
            
        }
        #endregion

        #region 조회조건 팝업

        //주문처
        private void btnSoldCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSoldCust.Text, "");
                pu.MaximizeBox = false;
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
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("PSA027P1", "주문처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //영업담당자
        private void btnSaleDuty_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON 'S011'";
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
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'SO_TYPE', @pSPEC2 = 'SO_TYPE_NM', @pSPEC3 = 'S_SO_TYPE'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSoType.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수주형태조회");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "팝업 호출"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //결제방법
        private void btnPaymentMeth_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S004'";
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

        #region 조회조건 TextChanged
        //주문처
        private void txtSoldCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSoldCust.Text != "")
                {
                    txtSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCust.Text, "");
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
                    txtSaleDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSaleDuty.Text, "");
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
                    txtSoTypeNm.Value = SystemBase.Base.CodeName("SO_TYPE", "SO_TYPE_NM", "S_SO_TYPE", txtSoType.Text, "");
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
                    txtPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPaymentMeth.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S004'");
                }
                else
                {
                    txtPaymentMethNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                string strCfmYn = "";

                try
                {
                    if (rdoCfmYes.Checked == true) strCfmYn = "Y";
                    else if (rdoCfmNo.Checked == true) strCfmYn = "N";

                    string strQuery = "usp_PSA027 @pTYPE = 'P1'";
                    strQuery += ", @pSO_DT_FR = '" + dtpSoDtFr.Text + "'";
                    strQuery += ", @pSO_DT_TO = '" + dtpSoDtTo.Text + "'";
                    strQuery += ", @pSOLD_CUST = '" + txtSoldCust.Text + "'";
                    strQuery += ", @pSALE_DUTY = '" + txtSaleDuty.Text + "'";
                    strQuery += ", @pSO_TYPE = '" + txtSoType.Text + "'";
                    strQuery += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "'";
                    strQuery += ", @pSO_CONFIRM_YN = '" + strCfmYn + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
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
