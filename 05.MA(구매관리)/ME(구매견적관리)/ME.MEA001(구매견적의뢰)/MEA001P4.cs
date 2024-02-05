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
    public partial class MEA001P4 : UIForm.FPCOMM1
    {
        #region 변수선언
        string returnVal;
        FarPoint.Win.Spread.FpSpread spd;
        #endregion

        public MEA001P4(FarPoint.Win.Spread.FpSpread spread)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();
            spd = spread;
        }

        public MEA001P4()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MEA001P4_Load(object sender, System.EventArgs e)
        {
            this.Text = "거래처지정 일괄적용";
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_MEA001  @pTYPE = 'P4'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pCUST_NM = '" + txtCustNm.Text + "'";
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
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {

                //거래처 전체 삭제				
                for (int j = 0; j < spd.Sheets[0].Rows.Count; j++)
                {
                    if (spd.Sheets[0].Cells[j, 2].Text == "True")
                    {
                        if (rdoCfmDelAdd.Checked == true)
                        {
                            string strSql1 = "usp_MEA001 'D3'";
                            strSql1 += ", @pEST_NO = '" + spd.Sheets[0].Cells[j, 6].Text + "'";
                            strSql1 += ", @pEST_SEQ = '" + spd.Sheets[0].Cells[j, 7].Text + "'";
                            strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                            ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                }


                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                    {
                        for (int j = 0; j < spd.Sheets[0].Rows.Count; j++)
                        {
                            if (spd.Sheets[0].Cells[j, 2].Text == "True")
                            {
                                string strSql = "usp_MEA001 'I3' ";
                                strSql += ", @pEST_NO = '" + spd.Sheets[0].Cells[j, 6].Text + "' ";
                                strSql += ", @pEST_SEQ = '" + spd.Sheets[0].Cells[j, 7].Text + "'";
                                strSql += ", @pCUST_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text + "' ";
                                strSql += ", @pCUST_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            }
                        }
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
            this.Cursor = Cursors.Default;

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

        //거래처TextChanged
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
    }
}
