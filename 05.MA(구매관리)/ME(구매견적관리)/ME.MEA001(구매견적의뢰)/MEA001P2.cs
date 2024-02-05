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
    public partial class MEA001P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strBtn = "N";
        string returnVal;
        string strEstNo;
        string strEstSeq;
        string strIs = "N";
        bool locked = false;
        bool save = false;
        #endregion

        public MEA001P2(string EstNo, string EstSeq, string Is, bool locking)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            strEstNo = EstNo;
            strEstSeq = EstSeq;
            strIs = Is;
            locked = locking;
        }

        public MEA001P2()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MEA001P2_Load(object sender, System.EventArgs e)
        {
            this.Text = "견적제출팝업";

            if (locked)
                UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            else
                UIForm.Buttons.ReButton("011111011000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            if (strIs != "N") SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MEA001  @pTYPE = 'P2'";
                strQuery += ", @pEST_NO = '" + strEstNo + "'";
                strQuery += ", @pEST_SEQ = '" + strEstSeq + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                if (locked)
                    UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ColumnCount, 3);
                else
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Value.ToString() == "*")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명") + "|1");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명") + "|3");
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

            //그리드 상단 필수 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U2"; break;
                                case "I": strGbn = "I2"; break;
                                case "D": strGbn = "D2"; break;
                                default: strGbn = ""; break;
                            }

                            string strSql = " usp_MEA001 '" + strGbn + "'";
                            strSql += ", @pEST_NO = '" + strEstNo + "'";
                            strSql += ", @pEST_SEQ = '" + strEstSeq + "'";
                            strSql += ", @pCUST_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text + "'";
                            strSql += ", @pCUST_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text + "'";
                            strSql += ", @pCUST_DUTY_DEPT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "담당부서")].Text + "'";
                            strSql += ", @pCUST_DUTY_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자")].Text + "'";
                            strSql += ", @pCUST_DUTY_POS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자직책")].Text + "'";
                            strSql += ", @pCUST_DUTY_TEL = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전화번호")].Text + "'";
                            strSql += ", @pCUST_DUTY_FAX = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "팩스번호")].Text + "'";
                            strSql += ", @pEST_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = e.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    save = true;
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 그리드 버튼 클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "거래처_2"))
            {
                strBtn = "Y";
                try
                {
                    WNDW002 pu = new WNDW002(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text, "P", "1");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명")].Text = Msgs[2].ToString();
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
                strBtn = "N";
            }
        }
        #endregion

        #region 그리드 Change
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            int i1 = SystemBase.Base.GridHeadIndex(GHIdx1, "거래처");
            int i2 = SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명");
            if (e.Column == i1 && strBtn == "N")
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, i1].Text != "*")
                    fpSpread1.Sheets[0].Cells[e.Row, i2].Text = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", fpSpread1.Sheets[0].Cells[e.Row, i1].Text, " AND SCM_YN = 1 AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                else
                    UIForm.FPMake.grdReMake(fpSpread1, e.Row, i2 + "|1");


            }
        }
        #endregion

        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            int rowcnt = 0;
            int idx = SystemBase.Base.GridHeadIndex(GHIdx1, "거래처");
            string cust_cds = "";

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                {
                    rowcnt++;
                    if (cust_cds == "") cust_cds = fpSpread1.Sheets[0].Cells[i, idx].Text;
                    else cust_cds += "," + fpSpread1.Sheets[0].Cells[i, idx].Text;
                }
            }
            if (rowcnt > 0)
            {
                if (save || cust_cds != "") RtnStr(cust_cds);
                else RtnStr("N");
            }
            else
            {
                if (save) RtnStr(cust_cds);
                else RtnStr("N");
            }

            this.Close();
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string strCode)
        {
            returnVal = strCode;
        }
        #endregion
    }
}
