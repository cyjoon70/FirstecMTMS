using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using UIForm;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;

namespace BB.BBA009
{
    public partial class BBA009 : UIForm.FPCOMM2
    {
        #region Field
        /// <summary>권한그룹</summary>
        string RollId = null;
        #endregion

        #region Initialize
        public BBA009()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드
        private void BBA009_Load(object sender, EventArgs e)
        {
            try
            {
                string strSql = "usp_BBA009 ";
                strSql = strSql + " @pTYPE ='S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                strSql = strSql + " , @pROLL_ID = '" + txtGroupId.Text + "' ";
                strSql = strSql + " , @pROLL_NM = '" + txtGroupNm.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, false);
            }
            catch
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//좌측 그리드에 선택된 행이 존재하지 않습니다.
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            string strSql = "usp_BBA009 ";
            strSql +=  "   @pTYPE ='S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
            strSql +=  " , @pROLL_ID = '" + txtGroupId.Text + "' ";
            strSql +=  " , @pROLL_NM = '" + txtGroupNm.Text + "' ";
            UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, false);
 
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion


        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            // 그리드 상단 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
            {
                string ERRCode = "WR", MSGCode = "M0014"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int row = 0; row < fpSpread1.Sheets[0].Rows.Count; row++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                default: strGbn = ""; break;
                            }

                            string strBtnRoll = fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "신규")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "조회")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "행복사")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "행추가")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "행취소")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "삭제")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "저장")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Excel")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출력")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "도움말")].Value.ToString();
                            strBtnRoll += fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "종료")].Value.ToString();

                            string strSql = " usp_BBA009 " ;
                            strSql += "  @pTYPE         = '" + strGbn + "'";
                            strSql += ", @pCO_CD        = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql += ", @pROLL_ID      = '" + RollId + "'";
                            strSql += ", @pMENU_ID      = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text + "'";
                            strSql += ", @pMENU_NAME    = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text + "'";
                            strSql += ", @pUP_MENU_ID   = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text + "'";
                            strSql += ", @pBTN_CTL      = '" + strBtnRoll + "'";
                            strSql += ", @pMENU_CHECKED = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text + "'";
                            strSql += ", @pUP_ID        = '" + SystemBase.Base.gstrUserID + "'";

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
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AllMenuSearch();
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


        #region 권한그룹 선택
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0) return;

                //선택 권한그룹코드
                RollId = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "그룹코드")].Text.ToString();

                AllMenuSearch();
            }
        }
        #endregion

        #region 권한등록 프로그램 조회
        private void AllMenuSearch()
        {
            //권한등록프로그램 조회문.
            string strSql = " usp_BBA009  ";
            strSql +=  "  @pTYPE ='S2' ";
            strSql +=  ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strSql += ", @pROLL_ID = '" + RollId + "'";
            strSql += ", @pViewType = '" + (rdoAll.Checked ? "A" : rdoN.Checked ? "N" :"Y") + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                fpSpread1.ActiveSheet.SetActiveCell(0, 1);
                fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
            }
            else
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);


            Application.DoEvents();
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "False") RowLocking(i, true);

        }
        void RowLocking(int Row, bool Lock)
        {
            if (Lock)
            {
                //Detail Locking설정
                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID") + "|3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명") + "|3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴") + "|3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "신규") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "조회") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행복사") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행추가") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행취소") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "삭제") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "저장") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "Excel") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "출력") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "도움말") + " |3#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹") + " |3");
            }
            else
            {
                ////Detail Locking설정
                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID") + "|0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명") + "|0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "신규") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "조회") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행복사") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행추가") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행취소") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "삭제") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "저장") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "Excel") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "출력") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "도움말") + " |0#" +
                    SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹") + " |0");
            }
        }
        #endregion

        #region 
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선택"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text != "True") RowLocking(Row, true);
                else RowLocking(Row, false);
            }
        }

        #region btnHistory_Click(): 변경이력조회 버튼 클릭: 권한그룹별변경이력조회 팝업창 띄움.
        private void btnHistory_Click(object sender, EventArgs e)
        {
            BBA009History pu = new BBA009History(RollId);
            pu.ShowDialog();
        }
        #endregion

        private void rdoCheckedChange(object sender, EventArgs e)
        {
            AllMenuSearch();
        }
        #endregion

        // 2021.10.06. hma 추가(Start)
        #region btnMenuRoll_Click(): 메뉴별권한그룹 버튼 클릭: 메뉴별권한그룹을 조회하는 팝업창 띄움.
        private void btnMenuRoll_Click(object sender, EventArgs e)
        {
            string strSelectedMenuId;
            string strSelectedMenuNm;
            int iCurRow;

            strSelectedMenuId = "";
            strSelectedMenuNm = "";

            iCurRow = fpSpread1.Sheets[0].ActiveRowIndex;
            if (fpSpread1.Sheets[0].ActiveRowIndex >= 0)
            {
                strSelectedMenuId = fpSpread1.Sheets[0].Cells[iCurRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text;
                strSelectedMenuNm = fpSpread1.Sheets[0].Cells[iCurRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text;

                BBA009MenuRoll pu = new BBA009MenuRoll(strSelectedMenuId, strSelectedMenuNm);
                pu.ShowDialog();
            }            
        }
        #endregion
        // 2021.10.06. hma 추가(End)
    }
}
