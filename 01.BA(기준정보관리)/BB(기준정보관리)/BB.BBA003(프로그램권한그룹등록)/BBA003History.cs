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

namespace BB.BBA003
{
    public partial class BBA003History : UIForm.FPCOMM1
    {
        #region Field
        string RollId = "";   //그룹코드
        #endregion

        #region Initialize
        public BBA003History()
        {
            InitializeComponent();
        }
        public BBA003History(string rollId) :this()
        {
            this.RollId = rollId;
        }
        #endregion

        #region 폼로드
        private void BBA003History_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 버튼재정의
            UIForm.Buttons.ReButton("110000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            NewExec();
            SearchExec();
        }

        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            // 권한명칭 가져오기
            txtGroupId.Value = this.RollId;
            txtGroupNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtGroupId.Text, " AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' and MAJOR_CD = 'CO006' ");


            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpChangeDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpChangeDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
        }

        protected override void SearchExec()
        {
            string strSql = "usp_BBA003 ";
            strSql += "    @pTYPE ='S5', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' "; //권한그룹조회
            strSql += " ,  @pROLL_ID = '" + txtGroupId.Text + "' ";
            strSql += " ,  @pDT_FR = '" + dtpChangeDtFr.Text + "' ";
            strSql += " ,  @pDT_TO = '" + dtpChangeDtTo.Text + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, false);
        }
        #endregion
        private void txtGroupId_ValueChanged(object sender, EventArgs e)
        {
            txtGroupNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtGroupId.Text, " AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "' and MAJOR_CD = 'CO006' ");
        }
    }
}
