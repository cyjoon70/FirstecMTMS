using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace QD.QDE012
{
    public partial class QDE012P2 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strInspReqNo = "";
        string strCfm = "";
        string strAssignNo = "";
        #endregion

        #region 생성자
        public QDE012P2()
        {
            InitializeComponent();
        }

        public QDE012P2(string InspReqNo)
        {
            InitializeComponent();

            strInspReqNo = InspReqNo;
            strCfm = "N";
        }

        public QDE012P2(string InspReqNo, string cfm_yn)
        {
            InitializeComponent();

            strInspReqNo = InspReqNo;
            strCfm = cfm_yn;
        }
        #endregion

        #region 폼로드 이벤트
        private void QDE012P2_Load(object sender, System.EventArgs e)
        {
            this.Text = "알리미등록";

            //화면 Look, 버튼 설정
            if (strCfm == "Y")
            {
                UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
            else
            {
                UIForm.Buttons.ReButton("001111110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QDE012  @pTYPE = 'P2'";
                strQuery += ", @pINSP_REQ_NO = '" + strInspReqNo + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1);

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    UIForm.FPMake.grdReMake(fpSpread1, "2|5");
                    strAssignNo = fpSpread1.Sheets[0].Cells[0, 4].Text;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)
            {
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
                                //								case "U": strGbn = "U1"; break;
                                case "I": strGbn = "I2"; break;
                                case "D": strGbn = "D2"; break;
                                default: strGbn = ""; break;
                            }

                            // 그리드 상단 필수항목 체크

                            string strSql = " usp_QDE012 '" + strGbn + "'";
                            strSql += ", @pASSIGN_NO = '" + strAssignNo + "'";
                            strSql += ", @pINSP_REQ_NO = '" + strInspReqNo + "'";
                            strSql += ", @pASSIGN_ID  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자")].Text + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            strAssignNo = ds.Tables[0].Rows[0][2].ToString();

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

        #region DelExec()
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("B0047"), "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string strSql = " usp_QDE012 'D3'";
                    strSql += ", @pASSIGN_NO = '" + strAssignNo + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프							
                }
                else
                {
                    Trans.Rollback();
                    this.Cursor = Cursors.Default;
                    return;
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
        #endregion

        #region 그리드 버튼 클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사용자_2"))
            {
                string strQuery = " usp_B_COMMON 'B010' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = Msgs[1].ToString();
                }
            }
        }
        #endregion

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사용자"))
            {
                string strQuery = " usp_B_COMMON 'B012', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자")].Text + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = dt.Rows[0][1].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = "";
                }
            }
        }
        #endregion
    }
}
