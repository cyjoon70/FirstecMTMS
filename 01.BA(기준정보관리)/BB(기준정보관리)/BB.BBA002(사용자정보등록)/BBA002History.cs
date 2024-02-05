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

namespace BB.BBA002
{
    public partial class BBA002History : UIForm.FPCOMM1
    {
        #region Field
        string UserId = null;
        string UserNm = null;
        #endregion

        #region Initialize
        public BBA002History()
        {
            InitializeComponent();
        }
        public BBA002History(string userId, string userNm) : this()
        {
            this.UserId = userId;
            this.UserNm = userNm;
        }
        #endregion

        #region 폼로드
        private void BBA002History_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            this.Text = "사용자정보변경이력조회";       // 2021.11.09. hma 추가

            // 버튼재정의
            // 2021.11.09. hma 수정: 저장 버튼 활성화 
            UIForm.Buttons.ReButton("110000011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            txtUser.Value = this.UserId;
            txtUsernm.Value = this.UserNm;

            // 2021.11.03. hma 추가(Start): 기본적으로 1년이전 기간으로 세팅되도록 함.
            dtpHistDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString();
            dtpHistDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
            // 2021.11.03. hma 추가(End)

            SearchExec();
        }       

        protected override void SearchExec()
        {
            string strSql = "usp_BBA002 ";
            strSql += "    @pTYPE ='S3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
            strSql += " ,  @pUSR_ID = '" + txtUser.Text + "' ";
            strSql += " ,  @pUSR_NM = '" + txtUsernm.Text + "' ";
            strSql += " ,  @pHIST_DT_FROM = '" + dtpHistDtFr.Text + "' ";       // 2021.11.03. hma 추가
            strSql += " ,  @pHIST_DT_TO = '" + dtpHistDtTo.Text + "' ";         // 2021.11.03. hma 추가

            // 2021.11.09. hma 수정(Start): 비고 항목 저장을 위해 변경
            //UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            // 2021.11.09. hma 수정(End)
        }
        #endregion

        // 2021.11.09. hma 추가(Start)
        #region SaveExec(): 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                int iIDX;
                string strUsrId = "";
                string strRemark = "";

                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                    string strGbn = "";

                    iIDX = 0;
                    strUsrId = "";
                    strRemark = "";

                    if (strHead.Length > 0)
                    {
                        switch (strHead)
                        {
                            case "U": strGbn = "U2"; break;
                            default: strGbn = ""; break;
                        }

                        iIDX = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "IDX")].Value);
                        strUsrId = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text.ToString();
                        strRemark = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text.ToString();

                        string strSql = " usp_BBA002 '" + strGbn + "'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strSql = strSql + ", @pUSR_ID = '" + strUsrId + "'";
                        strSql = strSql + ", @pREMARK = '" + strRemark + "'";
                        strSql = strSql + ", @pIDX = '" + iIDX + "'";
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                SearchExec();

                //그리드 셀 포커스 이동
                UIForm.FPMake.GridSetFocus(fpSpread1, txtUser.Text);

                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (ERRCode == "ER") //ERROR
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else   //ERRCode == "WR" WARING
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        // 2021.11.09. hma 추가(End)
    }
}
