#region BBA007 작성 정보
/*************************************************************/
// 단위업무명 : 사용자별 권한 그룹
// 작 성 자 :   조 홍 태
// 작 성 일 :   2013-01-22
// 작성내용 :   
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*************************************************************/
#endregion

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


namespace BB.BBA007
{
    public partial class BBA007 : UIForm.FPCOMM3
    {
        #region 변수선언
        string strRoll_Id = "";   //그룹코드
        #endregion

        #region 생성자
        public BBA007()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼로드n
        private void BBA007_Load(object sender, EventArgs e)
        {
            try
            {
                string strSql = "usp_BBA007 ";
                strSql = strSql + " @pTYPE ='S1', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' "; //권한그룹조회
                strSql = strSql + " , @pROLL_ID = '"+ txtGroupId.Text +"' ";
                strSql = strSql + " , @pROLL_NM = '" + txtGroupNm.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, true, 0, 0, false);
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
            string strSql = "usp_BBA007 ";
            strSql = strSql + " @pTYPE ='S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' "; //권한그룹조회
            strSql = strSql + " , @pROLL_ID = '" + txtGroupId.Text + "' ";
            strSql = strSql + " , @pROLL_NM = '" + txtGroupNm.Text + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0, false);

            if (fpSpread3.Sheets[0].Rows.Count > 0)
            {
                if (txtGroupId.Text != "")
                {
                    UIForm.FPMake.GridSetFocus(fpSpread3, txtGroupId.Text, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹코드"));
                    //선택 권한그룹코드
                    strRoll_Id = fpSpread3.Sheets[0].Cells[fpSpread3.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹코드")].Text.ToString();
                }
                else if(txtGroupNm.Text != "")
                {
                    UIForm.FPMake.GridSetFocus(fpSpread3, txtGroupNm.Text, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹명"));
                    //선택 권한그룹코드
                    strRoll_Id = fpSpread3.Sheets[0].Cells[fpSpread3.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹명")].Text.ToString();
                }
                else
                {
                    fpSpread3.Sheets[0].SetActiveCell(0, 1);
                    //선택 권한그룹코드
                    strRoll_Id = fpSpread3.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹코드")].Text.ToString();
                }
                
                RollMenuSearch(strRoll_Id);
            }
        }
        #endregion

        #region 권한그룹 셀 클릭시 (Selection_Changed)
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
        #endregion

        #region 오른쪽 이동
        private void Right_Move(int intRRow, string strCheck)
        {
            string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            try
            {
                int intLRow;
                if (fpSpread1.ActiveSheet.Rows.Count == 0) //등록된 사용자가 없으면 intLRow는 0
                {
                    intLRow = 0;
                    UIForm.FPMake.RowInsert(fpSpread1);
                }
                else
                {
                    UIForm.FPMake.RowInsert(fpSpread1);
                    intLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                }
            
                fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID")].Text;
                fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자명")].Text;
                fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "권한그룹")].Text = strRoll_Id ;
                
                string strSql = " usp_BBA007 ";
                strSql = strSql + "  @pTYPE    = 'I1'";
                strSql = strSql + ", @pCO_CD     = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strSql = strSql + ", @pROLL_ID     = '" + strRoll_Id + "'";
                strSql = strSql + ", @pUSR_ID = '" + fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text + "'"; //사용자 ID
                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                
                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 Exit으로 점프

                string strRank = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID")].Text;
                int intRank = strRank.Length;

                if (strCheck == "S")
                {
                    string strUpRank = fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID")].Text;
                    fpSpread2.Sheets[0].Rows.Remove(intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID"));
                }
                else
                {
                    if (strCheck != "SA") fpSpread2.Sheets[0].Rows.Remove(intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID"));
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
            if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 왼쪽 이동
        private void Left_Move(int intLRow, string strCheck)
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

                fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID")].Text = fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text;
                fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자명")].Text = fpSpread1.Sheets[0].Cells[intLRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text;

                string strSql = " usp_BBA007 ";
                strSql += "  @pTYPE     = 'D1'";
                strSql += ", @pCO_CD     = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strSql += ", @pROLL_ID  = '" + strRoll_Id + "'";
                strSql += ", @pUSR_ID  = '" + fpSpread2.Sheets[0].Cells[intRRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text + "'";
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

            if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 선택라인 오른쪽으로 보내기
        private void btnRight_Click(object sender, EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRRow = fpSpread2.ActiveSheet.GetSelection(0).Row; //선택된 열
                    Right_Move(intRRow, "A");
                    string FixRollId = strRoll_Id;

                    SearchExec();

                    UIForm.FPMake.GridSetFocus(fpSpread3, FixRollId, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹코드"));
                }
                catch
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY026"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//좌측 그리드에 선택된 행이 존재하지 않습니다.
                }
            }
        }
        #endregion

        #region 선택라인 왼쪽으로 보내기
        private void btnLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int intLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    Left_Move(intLRow, "A");
                    string FixRollId = strRoll_Id;

                    SearchExec();

                    UIForm.FPMake.GridSetFocus(fpSpread3, FixRollId, SystemBase.Base.GridHeadIndex(GHIdx3, "그룹코드"));
                }
            }
            catch
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY027"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//우측 그리드에 선택된 행이 존재하지 않습니다.
            }
        }
        #endregion

        #region 모두 오른쪽으로 보내기
        private void btnAllRight_Click(object sender, EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    for (int i = 0; i < fpSpread2.ActiveSheet.Rows.Count; i++)
                    {
                        Right_Move(i, "SA");
                    }
                    UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                    SearchExec();
                }
                catch
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY026"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//좌측 그리드에 선택된 행이 존재하지 않습니다.
                }
            }
        }
        #endregion

        #region 모두 왼쪽으로 보내기
        private void btnAllLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int intLRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    
                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                    {
                        Left_Move(i, "SA");
                    }
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                }
                SearchExec();
            }
            catch
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY027"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//우측 그리드에 선택된 행이 존재하지 않습니다.
            }
        }
        #endregion

        #region 조회문
        private void RollMenuSearch(string Roll_id)
        {
            //권한미등록사용자 조회문.
            string strSql = "usp_BBA007 ";
            strSql = strSql + "@pTYPE ='S2' ";
            strSql = strSql + ", @pCO_CD     = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strSql = strSql + ", @pROLL_ID = '" + Roll_id + "'";
            strSql = strSql + ", @pUSR_NM = '" + txtUsrNm.Text + "'";

            UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, false);

            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                fpSpread2.ActiveSheet.SetActiveCell(0, 1); //한셀선택
                fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1); //한열선택
            }
            else
                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, false);


            //권한등록사용자 조회문.
            string strSql2 = " usp_BBA007  ";
            strSql2 = strSql2 + "@pTYPE ='S3' ";
            strSql2 = strSql2 + ", @pCO_CD     = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strSql2 = strSql2 + ", @pROLL_ID = '" + Roll_id + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, false);

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                fpSpread1.ActiveSheet.SetActiveCell(0, 1);
                fpSpread1.ActiveSheet.AddSelection(0, 1, 1, 1);
            }
            else
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, false);
        }
        #endregion

        #region 사용자 찾기
        private void txtUsrNm_TextChanged(object sender, EventArgs e)
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
        #endregion

        #region 이력보기
        private void btnHistory_Click(object sender, EventArgs e)
        {

            BBA007History pu = new BBA007History(strRoll_Id);
            pu.ShowDialog();
        }
        #endregion
    }
}
