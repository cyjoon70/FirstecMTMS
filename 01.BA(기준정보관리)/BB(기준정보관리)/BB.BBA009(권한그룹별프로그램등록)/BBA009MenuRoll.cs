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
    public partial class BBA009MenuRoll : UIForm.FPCOMM1
    {
        #region Field
        string strMenuId = "";   //메뉴ID
        string strMenuNm = "";   //메뉴명
        #endregion

        #region Initialize
        public BBA009MenuRoll()
        {
            InitializeComponent();
        }
        public BBA009MenuRoll(string MenuId, string MenuNm)
        {
            strMenuId = MenuId;
            strMenuNm = MenuNm;

            InitializeComponent();
        }
        #endregion

        #region 폼로드
        private void BBA009MenuRoll_Load(object sender, EventArgs e)
        {
            //SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 버튼재정의
            UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtMenuId.Text = strMenuId;
            txtMenuNm.Text = strMenuNm;

            SearchExec();
        }

        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            // 메뉴명 가져오기
            txtMenuId.Text = "";
            txtMenuNm.Text = "";

            fpSpread1.Sheets[0].Rows.Count = 0;
        }

        protected override void SearchExec()
        {
            string strSql = "usp_BBA009 ";
            strSql += "    @pTYPE ='S4', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' "; //메뉴별권한그룹조회
            strSql += ",  @pMENU_ID = '" + txtMenuId.Text + "' ";
            strSql += ",  @pMENU_NAME = '" + txtMenuNm.Text + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, false);
        }
        #endregion
    }
}
