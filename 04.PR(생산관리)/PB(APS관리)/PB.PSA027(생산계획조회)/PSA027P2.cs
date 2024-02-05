#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산계획조회팦업
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-15
// 작성내용 : 생산계획조회팦업
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
    public partial class PSA027P2 : UIForm.FPCOMM1
    {
        string strProjNo = "";
        string strProjSeq = "";
        string strEntNm = "";
        string strDtFr = "";
        string strDtTo = "";

        public PSA027P2()
        {
            InitializeComponent();
        }
        public PSA027P2(string ProjNo, string ProjSeq, string EntNm, string DtFr, string DtTo)
        {
            strProjNo = ProjNo;
            strProjSeq = ProjSeq;
            strEntNm = EntNm;
            strDtFr = DtFr;
            strDtTo = DtTo;

            InitializeComponent();
        }

        #region Form Load 시
        private void PSA027P2_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtProjectNo.Value = strProjNo;
            txtProjectSeq.Value = strProjSeq;
            txtEntCd.Value = strEntNm;

            SearchExec();
            
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                string strQuery = "usp_PSA027 @pTYPE = 'S2'";
                strQuery += ", @pPROJECT_NO = '" + strProjNo + "'";
                strQuery += ", @pPROJECT_SEQ = '" + strProjSeq + "'";
                strQuery += ", @pDELIVERY_DT_FR = '" + strDtFr + "'";
                strQuery += ", @pDELIVERY_DT_TO = '" + strDtTo + "'";
                strQuery += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }
     
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

    }
}
