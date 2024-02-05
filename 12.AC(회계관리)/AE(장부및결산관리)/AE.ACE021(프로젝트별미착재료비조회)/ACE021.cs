
#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별미착재료비조회
// 작 성 자   : 김창진
// 작 성 일   : 2013-12-10
// 작성내용   : 프로젝트별미착재료비조회
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

namespace AE.ACE021
{
    public partial class ACE021 : UIForm.FPCOMM1
    {
        public ACE021()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE021_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpBlDtFr.Text = SystemBase.Base.ServerTime("Y") + "-01-01";
            dtpBlDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpBlDtFr.Text = SystemBase.Base.ServerTime("Y") + "-01-01";
            dtpBlDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_ACE021 @pTYPE = 'S1'";
                    strQuery += ", @pBL_DT_FR = '" + dtpBlDtFr.Text + "'";
                    strQuery += ", @pBL_DT_TO = '" + dtpBlDtTo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }
        #endregion

    }
}
