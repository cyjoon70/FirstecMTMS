#region 작성정보
/*********************************************************************/
// 단위업무명 : 거래처매출채권현황조회
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-24
// 작성내용 :거래처매출채권현황조회
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
using WNDW;
namespace SS.SSC009
{
    public partial class SSC009 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public SSC009()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void SSC009_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpBnDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,7) + "-01";
            dtpBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoCfmY.Checked = true;   
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpBnDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7) + "-01";
            dtpBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoCfmY.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {
                    string strQuery = " usp_SSC009 'S1'";

                    if (rdoCfmY.Checked == true)
                        strQuery += ", @pBN_CONFIRM_YN ='Y'";
                    else if (rdoCfmN.Checked == true)
                        strQuery += ", @pBN_CONFIRM_YN ='N'";

                    strQuery += ", @pBN_DT_FR  ='" + dtpBnDtFr.Text + "'";
                    strQuery += ", @pBN_DT_TO  ='" + dtpBnDtTo.Text + "'";
                    strQuery += ", @pBN_TYPE ='" + txtBnType.Text.Trim() + "'";
                    strQuery += ", @pBILL_CUST ='" + txtCustCd.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Color();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        private void Set_Color()
        {
            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, 4].Text == "zzzzzz") //소계
                {
                    for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                }
                else if (fpSpread1.Sheets[0].Cells[i, 1].Text == "zzzzzz") //합계
                {
                    for (int k = 0; k < fpSpread1.Sheets[0].ColumnCount; k++)
                    {
                        fpSpread1.Sheets[0].Cells[i, k].BackColor = SystemBase.Base.gColor1;
                    }
                }
            }
        }
        #endregion

        #region 조회조건 팝업  
        //매출채권형태
        private void btnBnType_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'BN_TYPE', @pSPEC2 = 'BN_TYPE_NM', @pSPEC3 = 'S_BN_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtBnType.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00084", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBnType.Text = Msgs[0].ToString();
                    txtBnTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //거래처
        private void butCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "S");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged  
        //매출채권형태
        private void txtBnType_TextChanged(object sender, EventArgs e)
        {
            txtBnTypeNm.Value = SystemBase.Base.CodeName("BN_TYPE", "BN_TYPE_NM", "S_BN_TYPE", txtBnType.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //거래처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion  

        #region Form Activated & Deactivated
        private void SSC009_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpBnDtFr.Focus();
        }

        private void SSC009_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
