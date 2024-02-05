
#region 작성정보
/*********************************************************************/
// 단위업무명 : 장부및결산관리
// 작 성 자   : 김창진
// 작 성 일   : 2014-02-26
// 작성내용   : 거래처별원장수불금액조회
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
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

namespace AE.ACE022
{
    public partial class ACE022 : UIForm.FPCOMM1
    {
        #region 생성자
        public ACE022()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ACE022_Load(object sender, System.EventArgs e)
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDateTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
            //기타 셋팅
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDateTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_ACE022  @pTYPE = 'S1'";
                    strQuery += " , @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                    strQuery += " , @pDATE_FR = '" + dtpDate.Text + "' ";
                    strQuery += " , @pDATE_TO = '" + dtpDateTo.Text + "' ";
                    strQuery += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

    }
}
