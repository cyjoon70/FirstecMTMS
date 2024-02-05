

#region 작성정보
/*********************************************************************/
// 단위업무명 : 채권상세조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-06
// 작성내용 : 채권상세조회
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

namespace AB.ACB001
{
    public partial class ACB001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSLIPNO = "";
        #endregion

        public ACB001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACB001_Load(object sender, System.EventArgs e)
        {
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            strSLIPNO = "";
            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpArDtFr.Text = YYMMDD.Substring(0, 4) + "-01-01";
            dtpArDtTo.Text = YYMMDD;
            dtpArExpDtFr.Value = null;
            dtpArExpDtTo.Value = null;
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
            PreRow = -1;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ACB001  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pAR_DT_FROM = '" + dtpArDtFr.Text + "' ";
                    strQuery += ", @pAR_DT_TO = '" + dtpArDtTo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";

                    if (dtpArExpDtFr.Text == "")
                    {
                        strQuery += ", @pAR_EXP_DT_FROM = '1900-01-01' ";
                    }
                    else
                    {
                        strQuery += ", @pAR_EXP_DT_FROM = '" + dtpArExpDtFr.Text + "' ";
                    }
                    if (dtpArExpDtTo.Text == "")
                    {
                        strQuery += ", @pAR_EXP_DT_TO = '2999-12-31' ";
                    }
                    else
                    {
                        strQuery += ", @pAR_EXP_DT_TO = '" + dtpArExpDtTo.Text + "' ";
                    }
                    if (optStatus_ING.Checked == true)
                    {
                        strQuery += ", @pEND_STATUS = 'N' ";
                    }
                    if (optStatus_END.Checked == true)
                    {
                        strQuery += ", @pEND_STATUS = 'Y' ";
                    }
                    strQuery += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                    strQuery += ", @pREMARK = '" + txtRemark.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, true, false, 0, 0, true);
                    PreRow = -1;
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 채권정보 그리드 선택
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                strSLIPNO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Text;
                if (PreRow == intRow && PreRow != -1 && intRow != -1)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                
                string strAR_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "채권번호")].Text;
                CLS_Search(strAR_NO);
                PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                strSLIPNO = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 반제정보 조회
        private void CLS_Search(string AR_NO)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACB001  'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pAR_NO ='" + AR_NO + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, false, 0, 0, true);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 텍스트 체인지
        //거래처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //계정코드
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업 클릭
        //거래처
        private void BtnCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCd.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //계정코드
        private void btntxtAcct_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAcctCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtAcctCd.Value = Msgs[0].ToString();
                    txtAcctNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 전표조회
        private void btnSlipView_Click(object sender, EventArgs e)
        {
            try
            {
                if (strSLIPNO != "")
                {
                    WNDW.WNDW026 pu = new WNDW.WNDW026(strSLIPNO);
                    pu.ShowDialog();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        
    }
}
