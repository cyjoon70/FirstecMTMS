using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace ME.MEA001
{
    public partial class MEA001P3 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal;
        string returnStr;
        string strEstNo;
        string strEstSeq;
        string strState = "N";
        string strTemp = "";
        bool locked = false;
        bool saved = false;
        #endregion

        public MEA001P3(string EstNo, string EstSeq, bool locking, bool saving)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            strEstNo = EstNo;
            strEstSeq = EstSeq;
            locked = locking;
            saved = saving;
        }

        public MEA001P3()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MEA001P3_Load(object sender, System.EventArgs e)
        {
            this.Text = "품질증빙팝업";

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MEA001  @pTYPE = 'P3'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pEST_NO = '" + strEstNo + "'";
                strQuery += ", @pEST_SEQ = '" + strEstSeq + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                if (locked)
                    UIForm.FPMake.grdReMake(fpSpread1, "1|3");

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString() == "1")
                {
                    strTemp += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙코드")].Text;
                }

            }
            if (saved)
            {
                string ERRCode = "";
                string MSGCode = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string strSql = "usp_MEA001 'U3' ";
                strSql += ", @pEST_NO = '" + strEstNo + "'";
                strSql += ", @pEST_SEQ = '" + strEstSeq + "'";
                strSql += ", @pEST_QUALITY_PROOF = '" + strTemp + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                ERRCode = dt.Rows[0][0].ToString();
                if (ERRCode == "OK")
                {
                    dbConn.Close();
                }
                else if (ERRCode == "ER")
                {
                    MSGCode = dt.Rows[0][1].ToString();
                    dbConn.Close();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    dbConn.Close();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
            strState = "Y";
            this.Close();
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            strState = "N";
            this.Close();
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }
        public string ReturnStr { get { return returnStr; } set { returnStr = value; } }

        public void RtnStr(string strCode, string strValue)
        {
            returnVal = strCode;
            returnStr = strValue;
        }
        #endregion

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }

        private void MEA001P3_Closing(object sender, FormClosingEventArgs e)
        {
            if (strState == "Y")
            {
                RtnStr("Y", strTemp);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                RtnStr("N", "");
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
