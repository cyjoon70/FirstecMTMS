#region 작성정보
/*********************************************************************/
// 단위업무명 : 직접공수표준공수상세데이터(POPUP)
// 작 성 자 : pes
// 작 성 일 : 2015-06-18
// 작성내용 : 직접공수 표준공수 상세데이터 조회
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
using FarPoint.Win.Spread;
using WNDW;

namespace PF.PFA001
{
    public partial class PFA001_P1 : UIForm.FPCOMM1
    {
        #region 변수선언

        string strTYPE ="";
        string strCO_CD ="";
        string strWORK_DT_FR ="";
        string strWORK_DT_TO ="";
        string strWC_CD ="";
        string strWORK_DUTY = "";
        string strWC_NM = "";       //작업장명 
        string strDUTY_NM = "";     //작업자명

        #endregion


        public PFA001_P1(string TYPE, string CO_CD, string WORK_DT_FR, string WORK_DT_TO, string WC_CD, string WORK_DUTY, string WC_NM, string DUTY_NM)
        {
            InitializeComponent();

            strTYPE = TYPE;
            strCO_CD = CO_CD;
            strWORK_DT_FR = WORK_DT_FR;
            strWORK_DT_TO = WORK_DT_TO;
            strWC_CD = WC_CD;
            strWORK_DUTY = WORK_DUTY;
            strWC_NM = WC_NM;       //작업장명 
            strDUTY_NM = DUTY_NM;   //작업자명
        }


        #region Form Load시
        private void PFA001_P1_Load(object sender, System.EventArgs e)
        {
            dtpWorkDtFr.Value = strWORK_DT_FR;
            dtpWorkDtTo.Value = strWORK_DT_TO;
            txtWcNm.Value = strWC_NM;
            txtWorkDutyNm.Value = strDUTY_NM;

            dtpWorkDtFr.ReadOnly = true;
            dtpWorkDtTo.ReadOnly = true;
            txtWcNm.ReadOnly = true;
            txtWorkDutyNm.ReadOnly = true;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
            SearchExec();
        }
        #endregion


        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {              
                    string strQuery = " usp_PFA001  @pTYPE = 'P1'";
                    strQuery += ", @pCO_CD = '" + strCO_CD + "'";
                    strQuery += ", @pWORK_DT_fr = '" + strWORK_DT_FR + "'";
                    strQuery += ", @pWORK_DT_TO = '" + strWORK_DT_TO + "'";
                    strQuery += ", @pWC_CD = '" + strWC_CD + "'";
                    strQuery += ", @pWORK_DUTY = '" + strWORK_DUTY + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion


    }
}
