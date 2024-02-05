#region BBA003 작성 정보
/*************************************************************/
// 단위업무명 : 권한별 프로그램 등록
// 작 성 자 :   전 성 표
// 작 성 일 :   2012-10-16
// 작성내용 :   권한 그룹별로 프로그램을 등록후 권한그룹에 사용자를 등록하여 프로그램 사용
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*************************************************************/
#endregion


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using UIForm;

namespace BB.BBA003
{
    public partial class BBA003 : UIForm.FPCOMM3
    {
        string strRoll_Id = "";

        public BBA003()
        {
            InitializeComponent();
        }

        private void BBA003_Load(object sender, EventArgs e)
        {
            try
            {
                string strSql = "usp_BBA003 ";
                strSql = strSql + " @pTYPE ='S1', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
                strSql = strSql + " , @pROLL_ID = '" + txtGroupId.Text + "' ";
                strSql = strSql + " , @pROLL_NM = '" + txtGroupNm.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0, false);
            }
            catch
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY014"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//좌측 그리드에 선택된 행이 존재하지 않습니다.
            }

        }

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            string strSql = "usp_BBA003 ";
            strSql = strSql + " @pTYPE ='S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
            strSql = strSql + " , @pROLL_ID = '" + txtGroupId.Text + "' ";
            strSql = strSql + " , @pROLL_NM = '" + txtGroupNm.Text + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0, false);
            if (fpSpread3.Sheets[0].Rows.Count > 0)
            {
                strRoll_Id = fpSpread3.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹코드")].Text.ToString();
            } 

            RollMenuSearch(strRoll_Id);

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 그리드저장 로직
        protected override void SaveExec()
        {
            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY048"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dsMsg == DialogResult.Yes)
                {
                    this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "SY001";//처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            if (strHead.Length > 0 && strHead == "U")
                            {
                                string strMenu_id = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text;
                                string strBtnRoll = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신규")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조회")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행복사")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행추가")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행취소")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "삭제")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "저장")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Excel")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출력")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도움말")].Value.ToString();
                                strBtnRoll += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료")].Value.ToString();

                                string strSql = " usp_BBA003 ";
                                strSql = strSql + "  @pTYPE    = 'U1'";
                                strSql = strSql + ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                strSql = strSql + ", @pROLL_ID    = '" + strRoll_Id + "'";
                                strSql = strSql + ", @pMENU_ID    = '" + strMenu_id + "'";
                                strSql = strSql + ", @pBTN_CTL    = '" + strBtnRoll + "'";
                                strSql = strSql + ", @pUP_ID      = '" + SystemBase.Base.gstrUserID + "'";

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
                        MSGCode = "SY002";//에러가 발생하여 데이터 처리가 취소되었습니다.

                        this.Cursor = System.Windows.Forms.Cursors.Default;
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        SearchExec();
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }                
            }
        }
        #endregion 

        #region 권한등록 프로그램 조회
        private void RollMenuSearch(string Roll_id)
        {           
            //권한미등록프로그램 조회문.
            string strSql = " usp_BBA003  ";
            strSql = strSql + "  @pTYPE ='S2' ";
            strSql = strSql + ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strSql = strSql + ", @pROLL_ID = '" + Roll_id + "'";
            strSql = strSql + ", @pMENU_ID = '" + txtMenuId.Text + "'";
            strSql = strSql + ", @pMENU_NAME = '" + txtMenuNM.Text + "'";

            UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, false);

            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
            }
            else
                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, false);


            //권한등록프로그램 조회문.
            string strSql2 = " usp_BBA003  ";
            strSql2 = strSql2 + "  @pTYPE ='S3' ";
            strSql2 = strSql2 + ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strSql2 = strSql2 + ", @pROLL_ID = '" + Roll_id + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                fpSpread1.ActiveSheet.SetActiveCell(0, 1);
                fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
            }
            else
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
        }
        #endregion

        private void fpSpread3_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread3.Sheets[0].Rows.Count > 0)
            {
                int intRow = fpSpread3.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0) return;

                //선택 권한그룹코드
                strRoll_Id = fpSpread3.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹코드")].Text.ToString();

                RollMenuSearch(strRoll_Id);
            }
        }

        #region 버튼클릭 이벤트

        #region button all right click
        private void btnAllRight_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int intRRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    if (fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text == "*")
                    {
                        for (int i = 0; i < fpSpread2.ActiveSheet.Rows.Count; i++)
                        {
                            Right_Move(i, "SA");
                        }
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                    }
                    else
                    {
                        Right_Move(intRRow, "S");
                    }

                    SearchExec();
                }
            }
            catch
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY026"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//좌측 그리드에 선택된 행이 존재하지 않습니다.
            }
        }
        #endregion

        #region button right click
        private void btnRight_Click(object sender, EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    Right_Move(intRRow, "A");

                    SearchExec();
                }
                catch
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY026"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//좌측 그리드에 선택된 행이 존재하지 않습니다.
                }                
            }
        }
        #endregion

        #region button left click
        private void btnLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int intLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    Left_Move(intLRow, "A");

                    SearchExec();
                }
            }
            catch
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY027"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//우측 그리드에 선택된 행이 존재하지 않습니다.
            }
        }
        #endregion

        #region btnAllLeft_Click
        private void btnAllLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int intLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                    if (fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text == "*")
                    {
                        for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                        {
                            Left_Move(i, "SA");
                        }
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0,0,false, false);
                    }
                    else
                    {
                        Left_Move(intLRow, "S");
                    }

                    SearchExec();
                }
            }
            catch
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY027"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//우측 그리드에 선택된 행이 존재하지 않습니다.
            }
        }
        #endregion
        #endregion

        #region Right에서 Left로 이동
        private void Right_Move(int intRRow, string strCheck)	//strCheck - A:전체, S:부분
        {
            if (Roll_Check() == "Y")
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string strBtnRoll = "";

                try
                {
                    int intLRow;
                    if (fpSpread1.ActiveSheet.Rows.Count == 0)
                    {
                        intLRow = 0;
                        UIForm.FPMake.RowInsert(fpSpread1);
                    }
                    else
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);
                        intLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    }

                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴명")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "신규")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "신규")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "조회")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "조회")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행복사")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "행복사")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행추가")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "행추가")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행취소")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "행취소")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "행삭제")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "삭제")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "삭제")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "저장")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "저장")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Excel")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "Excel")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "출력")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "출력")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "도움말")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "도움말")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "종료")].Value = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "종료")].Text;
                    fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹")].Text = strRoll_Id ;

                    strBtnRoll = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "버튼권한")].Text;

                    
                    string strSql = " usp_BBA003 ";
                    strSql = strSql + "  @pTYPE    = 'I1'";
                    strSql = strSql + ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                    strSql = strSql + ", @pROLL_ID     = '" + strRoll_Id + "'";
                    strSql = strSql + ", @pMENU_ID    = '" + fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text + "'";
                    strSql = strSql + ", @pMENU_NAME    = '" + fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text + "'";
                    strSql = strSql + ", @pUP_MENU_ID = '" + fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text + "'";
                    strSql = strSql + ", @pBTN_CTL = '" + strBtnRoll + "'";
                    strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    string strRank = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text;
                    int intRank = strRank.Length;

                    if (strCheck == "S")
                    {
                        string strUpRank = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text;

                        fpSpread2.Sheets[0].Rows.Remove(intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID"));

                        int intSubLRow = 0;

                        //상위 메뉴도 함께 권한부여
                        for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                        {
                            if (strUpRank == fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text)
                            {
                                UIForm.FPMake.RowInsert(fpSpread1);
                                intSubLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴명")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "신규")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "신규")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "조회")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "조회")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행복사")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "행복사")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행추가")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "행추가")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행취소")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "행취소")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "행삭제")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "삭제")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "삭제")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "저장")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "저장")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Excel")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "Excel")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "출력")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "출력")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "도움말")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "도움말")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "종료")].Value = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "종료")].Text;
                                fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹")].Text = strRoll_Id;

                                strBtnRoll = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "버튼권한")].Text;

                                string strSubSql = " usp_BBA003 ";
                                strSubSql = strSubSql + "  @pTYPE    = 'I1'";
                                strSubSql = strSubSql + ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                strSubSql = strSubSql + ", @pROLL_ID     = '" + strRoll_Id + "'";
                                strSubSql = strSubSql + ", @pMENU_ID    = '" + fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text + "'";
                                strSubSql = strSubSql + ", @pMENU_NAME    = '" + fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text + "'";
                                strSubSql = strSubSql + ", @pUP_MENU_ID = '" + fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text + "'";
                                strSubSql = strSubSql + ", @pBTN_CTL = '" + strBtnRoll + "'";
                                strSubSql = strSubSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";


                                DataSet dsSub = SystemBase.DbOpen.TranDataSet(strSubSql, dbConn, Trans);
                                ERRCode = dsSub.Tables[0].Rows[0][0].ToString();
                                MSGCode = dsSub.Tables[0].Rows[0][1].ToString();
                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                strUpRank = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text;

                                fpSpread2.Sheets[0].Rows.Remove(j, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID"));
                                j = -1;
                            }
                        }

                        //상위 우선순위 체크하여 하위가 있을경우 같이 전송
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            string strCheckRank = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text;
                            if (strCheckRank.Length >= intRank)
                            {
                                if (strRank == strCheckRank.Substring(0, intRank))
                                {
                                    UIForm.FPMake.RowInsert(fpSpread1);
                                    intSubLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴명")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "신규")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "신규")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "조회")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "조회")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행복사")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "행복사")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행추가")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "행추가")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행취소")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "행취소")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "행삭제")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "행삭제")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "삭제")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "삭제")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "저장")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "저장")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "Excel")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "Excel")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "출력")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "출력")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "도움말")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "도움말")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "종료")].Value = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "종료")].Text;
                                    fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹")].Text = strRoll_Id;

                                    strBtnRoll = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "버튼권한")].Text;

                                    string strSubSql = " usp_BBA003 ";
                                    strSubSql = strSubSql + "  @pTYPE    = 'I1'";
                                    strSubSql = strSubSql + ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                    strSubSql = strSubSql + ", @pROLL_ID     = '" + strRoll_Id + "'";
                                    strSubSql = strSubSql + ", @pMENU_ID    = '" + fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text + "'";
                                    strSubSql = strSubSql + ", @pMENU_NAME    = '" + fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text + "'";
                                    strSubSql = strSubSql + ", @pUP_MENU_ID = '" + fpSpread1.Sheets[0].Cells[intSubLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text + "'";
                                    strSubSql = strSubSql + ", @pBTN_CTL = '" + strBtnRoll + "'";
                                    strSubSql = strSubSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                    DataSet dsSub = SystemBase.DbOpen.TranDataSet(strSubSql, dbConn, Trans);
                                    ERRCode = dsSub.Tables[0].Rows[0][0].ToString();
                                    MSGCode = dsSub.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                    fpSpread2.Sheets[0].Rows.Remove(i, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID"));
                                    i = i - 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (strCheck != "SA") fpSpread2.Sheets[0].Rows.Remove(intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID"));
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    Trans.Rollback();
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "좌측으로 이동"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 이동중 오류가 발생하였습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {                   
                    //SearchExec();
                    //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            else
                MessageBox.Show(SystemBase.Base.MessageRtn("B0032"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//조회조건의 사용자ID와 현재그리드상의 사용자ID가 일치하지 않습니다.

        }
        #endregion

        #region Left에서 Right로 이동할때
        private void Left_Move(int intLRow, string strCheck)		//strCheck - A:전체, S:부분
        {
            if (Roll_Check() == "Y")
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    int intRRow;
                    if (fpSpread2.ActiveSheet.Rows.Count == 0)
                    {
                        intRRow = 0;
                        UIForm.FPMake.RowInsert(fpSpread2);
                    }
                    else
                    {
                        UIForm.FPMake.RowInsert(fpSpread2);
                        intRRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    }

                    fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text = fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text;
                    fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴명")].Text = fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text;
                    fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text = fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text;

                    string strSql = " usp_BBA003 ";
                    strSql += "  @pTYPE     = 'D1'";
                    strSql += ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                    strSql += ", @pROLL_ID  = '" + strRoll_Id + "'";
                    strSql += ", @pMENU_ID  = '" + fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text + "'";
                    strSql += ", @pUP_ID    = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    string strRank = fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text;
                    int intRank = strRank.Length;

                    if (strCheck == "S")
                    {
                        fpSpread1.Sheets[0].Rows.Remove(intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID"));

                        int intSubRRow = 0;

                        //상위 우선순위 체크하여 하위가 있을경우 같이 전송
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strCheckRank = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text;
                            if (strCheckRank.Length >= intRank)
                            {
                                if (strRank == strCheckRank.Substring(0, intRank))
                                {
                                    UIForm.FPMake.RowInsert(fpSpread2);
                                    intSubRRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                                    fpSpread2.Sheets[0].Cells[intSubRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text;
                                    fpSpread2.Sheets[0].Cells[intSubRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴명")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴명")].Text;
                                    fpSpread2.Sheets[0].Cells[intSubRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상위메뉴")].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상위메뉴")].Text;
                                    fpSpread1.Sheets[0].Rows.Remove(i, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID"));

                                    string strSubSql = " usp_BBA003 ";
                                    strSubSql += "  @pTYPE     = 'D1'";
                                    strSubSql += ", @pCO_CD    = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                    strSubSql += ", @pROLL_ID  = '" + strRoll_Id + "'";
                                    strSubSql += ", @pMENU_ID  = '" + fpSpread2.Sheets[0].Cells[intSubRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text + "'";
                                    strSubSql += ", @pUP_ID    = '" + SystemBase.Base.gstrUserID + "'";

                                    DataSet dsSub = SystemBase.DbOpen.TranDataSet(strSubSql, dbConn, Trans);
                                    ERRCode = dsSub.Tables[0].Rows[0][0].ToString();
                                    MSGCode = dsSub.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프


                                    i = i - 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (strCheck != "SA") fpSpread1.Sheets[0].Rows.Remove(intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID"));
                    }

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    Trans.Rollback();
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "우측으로 이동"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 이동중 오류가 발생하였습니다.
                }

            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {                   
                    //SearchExec();
                    //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            else
                MessageBox.Show(SystemBase.Base.MessageRtn("B0032"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//조회조건의 사용자ID와 현재그리드상의 사용자ID가 일치하지 않습니다.

        }
        #endregion

        #region 권한그룹체크
        private string Roll_Check()
        {
            string strCheck = "N";
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹")].Text == strRoll_Id) strCheck = "Y";
            }
            else
                strCheck = "Y";
            return strCheck;
        }
        #endregion

        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
           // if (fpSpread1.Sheets[0].Rows.Count > 0) Left_Move(e.Row, "S");
        }

        private void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
           // if (fpSpread2.Sheets[0].Rows.Count > 0) Right_Move(e.Row, "S");
        }

        private void txtMenuId_TextChanged(object sender, EventArgs e)
        {
            if (strRoll_Id != "")
            {
                RollMenuSearch(strRoll_Id);
            }
            else
            {
                MessageBox.Show("권한그룹이 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtMenuNM_TextChanged(object sender, EventArgs e)
        {
            if (strRoll_Id != "")
            {
                RollMenuSearch(strRoll_Id);
            }
            else
            {
                MessageBox.Show("권한그룹이 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            BBA003History pu = new BBA003History(strRoll_Id);
            pu.ShowDialog();
        }
    }
}
