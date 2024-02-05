

#region 작성정보
/*********************************************************************/
// 단위업무명 : 계정코드조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-06
// 작성내용 : 계정코드조회
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

namespace AA.ACA005
{
    public partial class ACA005 : UIForm.FPCOMM1 
    {
        public ACA005()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACA005_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            SystemBase.ComboMake.C1Combo(cboUseYn, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //사용여부
			
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

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
                    string strQuery = " usp_ACA005  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    if (optCode.Checked == true)
                        strQuery += ", @pACCT_CD = '" + txtCode.Text + "' ";
                    else
                        strQuery += ", @pACCT_NM = '" + txtCode.Text + "' ";
                    strQuery += ", @pUSE_YN = '" + cboUseYn.SelectedValue.ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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
