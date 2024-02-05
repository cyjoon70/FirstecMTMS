using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SO.SOA004
{
    public partial class SOA004P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string So_No = "";
        int chkValue = 0;
        #endregion

        #region 생성자
        public SOA004P1(string SoNo)
        {
            So_No = SoNo;

            InitializeComponent();

        }
        public SOA004P1()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드 이벤트
        private void SOA004P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "수주변경이력등록";

            //버튼 재정의
            UIForm.Buttons.ReButton("111111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            Search(false);

            if (fpSpread1.Sheets[0].RowCount > 0)
            {
                SubSearch(So_No, fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번")].Text);
            }
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            int chk = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                {
                    //하나이상의 등록, 수정은 할 수 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("S0015"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chk = chk + 1;
                    break;
                }
            }

            if (chk == 0)
            {
                UIForm.FPMake.RowInsert(fpSpread1);
                txtHisDetail.Text = "";
            }

        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int chk = 0;

                for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I" || fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                    {
                        //하나이상의 등록, 수정은 할 수 없습니다.
                        MessageBox.Show(SystemBase.Base.MessageRtn("S0015"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        chk = chk + 1;
                        break;
                    }
                }

                if (chk == 0)
                {
                    UIForm.FPMake.RowCopy(fpSpread1);
                    txtHisDetail.Text = "";

                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번")].Text = "";
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0030"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //좌측 그리드에 선택된 행이 존재하지 않습니다.
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region Search 조회함수
        protected override void SearchExec()
        {
            Search(true);

            if (fpSpread1.Sheets[0].RowCount > 0)
            {
                SubSearch(So_No, fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번")].Text);
            }
        }

        private void Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_SOA004  @pTYPE = 'H1'";
                strQuery += ", @pSO_NO = '" + So_No + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SubSearch 조회함수
        private void SubSearch(string SoNo, string HisSeq)
        {
            chkValue = 1;

            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_SOA004  @pTYPE = 'H2'";
                strQuery += ", @pSO_NO = '" + SoNo + "' ";
                strQuery += ", @pHIS_SEQ = '" + HisSeq + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0 && fpSpread1.Sheets[0].RowCount > 0)
                {
                    txtHisDetail.Text = dt.Rows[0]["HIS_DETAIL"].ToString();
                }
                else
                {
                    txtHisDetail.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }

        #endregion

        #region 그리드 선택시 이벤트
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

            //같은 Row 조회 되지 않게
            if (intRow < 0)
            {
                return;
            }

            if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
            {
                return;
            }

            SubSearch(fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text, fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번")].Text);
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //그리드 상단 필수 체크
            if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                string strHisSeq = "";

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
                                case "I": strGbn = "H3"; break;
                                case "U": strGbn = "H4"; break;
                                case "D": strGbn = "H5"; break;
                                default: strGbn = ""; break;
                            }

                            strHisSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번")].Text;

                            string strSql = " usp_SOA004 '" + strGbn + "'";
                            strSql += ", @pSO_NO = '" + So_No + "'";
                            strSql += ", @pHIS_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번")].Text + "'";
                            strSql += ", @pHIS_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경일자")].Text + "'";
                            strSql += ", @pHIS_SUBJECT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경제목")].Text + "'";
                            strSql += ", @pHIS_DETAIL = '" + txtHisDetail.Text + "'";
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
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Search(false);

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        SubSearch(So_No, fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번")].Text);
                        UIForm.FPMake.GridSetFocus(fpSpread1, strHisSeq, SystemBase.Base.GridHeadIndex(GHIdx1, "변경순번"));
                    }
                    else
                    {
                        txtHisDetail.Text = "";
                    }
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

        #region 변경내용 값이 변경되었을경우
        private void txtHisDetail_TextChanged(object sender, System.EventArgs e)
        {
            if (chkValue == 0)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].ActiveRowIndex, 0].Text == "" || fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].ActiveRowIndex, 0].Text == "D")
                {
                    fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].ActiveRowIndex, 0].Text = "U";
                }
            }
        }

        private void txtHisDetail_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            chkValue = 0;
        }
        #endregion
    }
}
