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

namespace BB.BBA009
{
    public partial class BBA009History : UIForm.FPCOMM1
    {
        #region Field
        string RollId = "";   //그룹코드
        #endregion

        #region Initialize
        public BBA009History()
        {
            InitializeComponent();
        }
        public BBA009History(string rollId) : this()
        {
            this.RollId = rollId;
        }
        #endregion

        #region 폼로드
        private void BBA009History_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 버튼재정의
            UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);        // 2021.10.06. hma 수정: 엑셀 버튼도 활성화

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
            string strSql = "usp_BBA009 ";
            strSql += "    @pTYPE ='S3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' "; // 권한그룹조회
            strSql += " ,  @pROLL_ID = '" + txtGroupId.Text + "' ";
            strSql += " ,  @pROLL_NM = '" + txtGroupNm.Text + "' ";     // 2021.11.10. hma 추가: 권한그룹명으로도 검색되도록 함. 
            strSql += " ,  @pDT_FR = '" + dtpChangeDtFr.Text + "' ";
            strSql += " ,  @pDT_TO = '" + dtpChangeDtTo.Text + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, false);
        }
        #endregion
    }
}
