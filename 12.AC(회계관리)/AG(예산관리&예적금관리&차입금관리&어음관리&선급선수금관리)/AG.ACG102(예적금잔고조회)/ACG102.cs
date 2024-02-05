

#region 작성정보
/*********************************************************************/
// 단위업무명 : 예적금잔고조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-13
// 작성내용 : 예적금잔고조회
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

namespace AG.ACG102
{
    public partial class ACG102 : UIForm.FPCOMM1 
    {
        public ACG102()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG102_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            dtpBasicDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            SystemBase.ComboMake.C1Combo(cboBankCd, "SELECT BANK_CD, BANK_NM, 'N' FROM B_BANK(NOLOCK) WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //은행
            SystemBase.ComboMake.C1Combo(cboAcctPart, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B018', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //예적금유형
            SystemBase.ComboMake.C1Combo(cboOpenStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A506', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //거래상태
            SystemBase.ComboMake.C1Combo(cboCurcd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //통화
            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpBasicDt.Value = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
                    string strQuery = " usp_ACG102  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pBASIC_DT = '" + dtpBasicDt.Text + "' ";
                    strQuery += ", @pACCT_PART = '" + cboAcctPart.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBANK_CD = '" + cboBankCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pOPEN_STATUS = '" + cboOpenStatus.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBIZ_AREA_CD_FROM = '" + cboBizAreaCdFrom.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBIZ_AREA_CD_TO = '" + cboBizAreaCdTo.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCUR_CD = '" + cboCurcd.SelectedValue.ToString() + "' ";
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
