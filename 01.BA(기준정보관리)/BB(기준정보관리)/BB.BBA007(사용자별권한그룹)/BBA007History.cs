using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using UIForm;
using System.Text.RegularExpressions;

namespace BB.BBA007
{
    public partial class BBA007History : UIForm.FPCOMM1
    {
        #region Field
        string RollId = "";   //그룹코드
        #endregion

        #region Initialize
        public BBA007History()
        {
            InitializeComponent();
        }
        public BBA007History(string rollId):this()
        {
            this.RollId = rollId;
        }
        #endregion

        #region 폼로드
        private void BBA007History_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 버튼재정의
            UIForm.Buttons.ReButton("110000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            // 권한명칭 가져오기
            txtGroupId.Value = this.RollId;
            txtGroupNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtGroupId.Text, " AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' and MAJOR_CD = 'CO006' ");

            dtpHistDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();      // 2021.08.17. hma 추가: 이력일자 FROM
            dtpHistDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");    // 2021.08.17. hma 추가: 이력일자 TO

            SearchExec();
        }

        protected override void SearchExec()
        {
            string strSql = "usp_BBA007 ";
            strSql +=  "    @pTYPE ='S4', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' "; //권한그룹조회
            strSql += " ,  @pROLL_ID = '" + txtGroupId.Text + "' ";
            strSql += " ,  @pUSR_ID = '" + txtUser.Text + "' ";
            strSql += " ,  @pUSR_NM = '" + txtUsernm.Text + "' ";
            strSql += " ,  @pHIST_DT_FROM = '" + dtpHistDtFr.Text + "' ";       // 2021.08.17. hma 추가: 이력일자 FROM
            strSql += " ,  @pHIST_DT_TO = '" + dtpHistDtTo.Text + "' ";         // 2021.08.17. hma 추가: 이력일자 TO

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, false);
        }
        #endregion

        private void txtGroupId_ValueChanged(object sender, EventArgs e)
        {
            txtGroupNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtGroupId.Text, " AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' and MAJOR_CD = 'CO006' ");

        }
        private void txtUser_ValueChanged(object sender, EventArgs e)
        {
            txtUsernm.Text = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUser.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
    }
}
