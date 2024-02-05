

#region 작성정보
/*********************************************************************/
// 단위업무명 : 예적금입출금내역조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-14
// 작성내용 : 예적금입출금내역조회
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

namespace AG.ACG103
{
    public partial class ACG103 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        public ACG103()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACB006_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboBankCd, "SELECT BANK_CD, BANK_NM, 'N' FROM B_BANK(NOLOCK) WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //은행
            SystemBase.ComboMake.C1Combo(cboAcctPart, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B018', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //예적금유형
            SystemBase.ComboMake.C1Combo(cboOpenStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A506', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //거래상태
            SystemBase.ComboMake.C1Combo(cboCurcd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //통화
            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            dtpBasicDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpBasicDtTo.Value = SystemBase.Base.ServerTime("YYMMDD"); ;

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
                    string strQuery = " usp_ACG103  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pACCT_PART = '" + cboAcctPart.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBANK_CD = '" + cboBankCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pOPEN_STATUS = '" + cboOpenStatus.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBIZ_AREA_CD_FROM = '" + cboBizAreaCdFrom.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBIZ_AREA_CD_TO = '" + cboBizAreaCdTo.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCUR_CD = '" + cboCurcd.SelectedValue.ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                    PreRow = -1;
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 선택
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

                if (PreRow == intRow && PreRow != -1 && intRow != -1)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                string strBANK_CD = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "은행코드")].Text;
                string strACCT_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "계좌번호")].Text;

                if (fpSpread2.Sheets[0].RowHeader.Cells[intRow, 0].Text != "I")
                {
                    Sub_Search(strBANK_CD, strACCT_NO);
                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }
                PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 입출내역 조회
        private void Sub_Search(string BANK_CD, string ACCT_NO)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACG103  'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pBANK_CD ='" + BANK_CD + "' ";
                strQuery += ", @pACCT_NO ='" + ACCT_NO + "' ";
                strQuery += ", @pSLIP_DT_FROM = '" + dtpBasicDtFr.Text + "' ";
                strQuery += ", @pSLIP_DT_TO = '" + dtpBasicDtTo.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 전표조회
        private void btnSlipView_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].GetSelection(0) != null)
                {
                    int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
                    if (intRow < 0)
                    {
                        return;
                    }

                    string strSLIP_NO = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "회계전표")].Text;
                    
                    WNDW.WNDW026 pu = new WNDW.WNDW026(strSLIP_NO);
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
