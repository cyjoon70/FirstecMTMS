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
    public partial class MEA001P5 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal;
        FarPoint.Win.Spread.FpSpread spd;
        #endregion

        public MEA001P5(FarPoint.Win.Spread.FpSpread spread)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();
            spd = spread;
        }

        public MEA001P5()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MEA001P5_Load(object sender, System.EventArgs e)
        {
            this.Text = "품질증빙 일괄적용";

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MEA001  @pTYPE = 'P5'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

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
            this.Cursor = Cursors.WaitCursor;
            string strTemp = "";

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString() == "1")
                {
                    strTemp += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "증빙코드")].Text;
                }
            }


            if (strTemp != "")
            {
                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < spd.Sheets[0].Rows.Count; i++)
                    {
                        if (spd.Sheets[0].Cells[i, 2].Text == "True")
                        {
                            string strSql = "usp_MEA001 'U3' ";
                            strSql += ", @pEST_NO = '" + spd.Sheets[0].Cells[i, 6].Text + "'";
                            strSql += ", @pEST_SEQ = '" + spd.Sheets[0].Cells[i, 7].Text + "'";
                            strSql += ", @pEST_QUALITY_PROOF = '" + strTemp + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0042"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RtnStr("Y");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            RtnStr("N");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string strCode)
        {
            returnVal = strCode;
        }
        #endregion

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
        }
    }
}
