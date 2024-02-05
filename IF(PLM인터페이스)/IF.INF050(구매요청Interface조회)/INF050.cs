#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매요청진행조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-14
// 작성내용 : 구매요청진행조회 관리
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

namespace IF.INF050
{
    public partial class INF050 : UIForm.FPCOMM1
    {
        #region 생성자
        public INF050()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void INF050_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;

        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
			this.Cursor = Cursors.WaitCursor;
			string rdoCfm;

			if (rdoCfmY.Checked)
				rdoCfm = "B";
			else rdoCfm = "A";

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
			{
				string strQuery = " usp_IF_INF050  'S1'";
				strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery = strQuery + ", @pPLANT_CD ='" + SystemBase.Base.gstrPLANT_CD.ToString() + "' ";
				strQuery = strQuery + ", @pGUBUN ='" + rdoCfm + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
			}
			this.Cursor = Cursors.Default;

		}
        #endregion       

    }
}
