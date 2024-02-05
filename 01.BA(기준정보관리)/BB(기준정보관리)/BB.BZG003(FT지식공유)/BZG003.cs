#region 작성정보
/*********************************************************************/
// 단위업무명 : FT 지식공유
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-07
// 작성내용 :  FT 지식공유 등록 및 조회
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
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
using System.Reflection;

namespace BB.BZG003
{
    public partial class BZG003 : UIForm.FPCOMM2_2T
    {
        #region 변수선언
        string Idx = "";
        string Num = "";
        string Lvl = "";
        string Group = "";

        string AIdx = "";
        string ANum = "";
        string AGroup = "";

        string WriteFlag = "N"; //N:새글,R:답글
        #endregion

        #region 생성자
        public BZG003()
        {
            InitializeComponent();
        }
        #endregion

        #region BZG003_Load
        private void BZG003_Load(object sender, EventArgs e)
        {
            try
            {
                string Query = " usp_BAA004 'S3',@PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread1', @PIN_ID='" + SystemBase.Base.gstrUserID + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.TranDataTable(Query);
                int G1RowCount = dt.Rows.Count + 1;

                if (G1RowCount > 0)
                {
                    G1Head1 = new string[G1RowCount];// 첫번째 Head Text
                    G1Head2 = new string[G1RowCount];// 두번째 Head Text
                    G1Head3 = new string[G1RowCount];// 세번째 Head Text
                    G1Width = new int[G1RowCount];// Cell 넓이
                    G1Align = new string[G1RowCount];// Cell 데이타 정렬방식
                    G1Type = new string[G1RowCount];// CellType 지정
                    G1Color = new int[G1RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

                    G1SEQ = new int[G1RowCount];// 키
                    
                    G1HeadCnt = Convert.ToInt32(dt.Rows[0][0].ToString());

                    /********************1번째 숨김필드 정의******************/
                    G1Head1[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 1)
                        G1Head2[0] = "";
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 3)
                        G1Head3[0] = "";
                    G1Width[0] = 0;
                    G1Align[0] = "";
                    G1Type[0] = "";
                    G1Color[0] = 0;
                    G1Etc[0] = "";
                    /********************1번째 숨김필드 정의******************/

                    for (int i = 1; i < G1RowCount; i++)
                    {
                        G1Head1[i] = dt.Rows[i - 1][1].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) >= 1)
                            G1Head2[i] = dt.Rows[i - 1][2].ToString();
                        if (Convert.ToInt32(dt.Rows[i - 1][0].ToString()) == 3)
                            G1Head3[i] = dt.Rows[i - 1][3].ToString();

                        G1Width[i] = Convert.ToInt32(dt.Rows[i - 1][4].ToString());
                        G1Align[i] = dt.Rows[i - 1][5].ToString();
                        G1Type[i] = dt.Rows[i - 1][6].ToString();
                        G1Color[i] = Convert.ToInt32(dt.Rows[i - 1][7].ToString());

                        if (G1Etc[i] == null)
                            G1Etc[i] = dt.Rows[i - 1][8].ToString();

                        G1SEQ[i] = Convert.ToInt32(dt.Rows[i - 1][9].ToString());

                    }
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                }
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;


                string Query2 = " usp_BAA004 'S3',@PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread2', @PIN_ID='" + SystemBase.Base.gstrUserID + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt2 = SystemBase.DbOpen.TranDataTable(Query2);
                int G2RowCount = dt2.Rows.Count + 1;

                if (G2RowCount > 0)
                {
                    G2Head1 = new string[G2RowCount];// 첫번째 Head Text
                    G2Head2 = new string[G2RowCount];// 두번째 Head Text
                    G2Head3 = new string[G2RowCount];// 세번째 Head Text
                    G2Width = new int[G2RowCount];// Cell 넓이
                    G2Align = new string[G2RowCount];// Cell 데이타 정렬방식
                    G2Type = new string[G2RowCount];// CellType 지정
                    G2Color = new int[G2RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)

                    G2SEQ = new int[G2RowCount];// 키

                    G2HeadCnt = Convert.ToInt32(dt2.Rows[0][0].ToString());

                    /********************1번째 숨김필드 정의******************/
                    G2Head1[0] = "";
                    if (Convert.ToInt32(dt2.Rows[0][0].ToString()) >= 1)
                        G2Head2[0] = "";
                    if (Convert.ToInt32(dt2.Rows[0][0].ToString()) == 3)
                        G2Head3[0] = "";
                    G2Width[0] = 0;
                    G2Align[0] = "";
                    G2Type[0] = "";
                    G2Color[0] = 0;
                    G2Etc[0] = "";
                    /********************1번째 숨김필드 정의******************/

                    for (int i = 1; i < G2RowCount; i++)
                    {
                        G2Head1[i] = dt2.Rows[i - 1][1].ToString();
                        if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) >= 1)
                            G2Head2[i] = dt2.Rows[i - 1][2].ToString();
                        if (Convert.ToInt32(dt2.Rows[i - 1][0].ToString()) == 3)
                            G2Head3[i] = dt2.Rows[i - 1][3].ToString();

                        G2Width[i] = Convert.ToInt32(dt2.Rows[i - 1][4].ToString());
                        G2Align[i] = dt2.Rows[i - 1][5].ToString();
                        G2Type[i] = dt2.Rows[i - 1][6].ToString();
                        G2Color[i] = Convert.ToInt32(dt2.Rows[i - 1][7].ToString());

                        if (G2Etc[i] == null)
                            G2Etc[i] = dt2.Rows[i - 1][8].ToString();

                        G2SEQ[i] = Convert.ToInt32(dt2.Rows[i - 1][9].ToString());

                    }
                    UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
                }
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                SearchExec();
                c1DockingTab1.TabPages[0].TabVisible = true;
                c1DockingTab1.TabPages[1].TabVisible = false;
                c1DockingTab1.TabPages[2].TabVisible = false;
                c1DockingTab1.SelectedIndex = 0;

                this.Text = "FT 지식공유";

                linkLabel1.Text = "공지사항";  //링크명
                strJumpFileName1 = "BB.BZG001.BZG001"; //호출할 화면명
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            string strQuery = " USP_BZG003  'S1'";
            strQuery = strQuery + ", @pSEARCH_TEXT ='" + txtSearchText.Text + "' ";
            strQuery = strQuery + ", @pCHK_CONTENTS ='" + chkContent.Checked.ToString() + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    string Head = "", Last = "", Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제목")].Text;
                    int MaxLvl = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ANS_LEVEL")].Text);

                    if (MaxLvl > 0)
                    {
                        for (int k = 0; k < MaxLvl; k++)
                        {
                            Head = Head + "   ";
                        }

                        Text = Head + "┗" + " [답변] " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제목")].Text;
                    }

                    if (Convert.ToDateTime(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "등록일")].Value) >= DateTime.Now.AddDays(-1))
                    {
                        Last = "...................................New";
                    }

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제목")].Text = Text + Last;
                }
            }

        }
        #endregion

        #region 답변리스트 조회
        private void MemoSearch(string AnsGroup, string Idx)
        {
            string strQuery = " USP_BZG003  'S3'";
            strQuery = strQuery + ", @pANS_GROUP ='" + AnsGroup + "' ";
            strQuery = strQuery + ", @pIDX ='" + Idx + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
        }
        #endregion

        #region 검색버튼
        private void btnBoardSearch_Click(object sender, System.EventArgs e)
        {
            SearchExec();

            c1DockingTab1.TabPages[0].TabVisible = true;
            c1DockingTab1.TabPages[1].TabVisible = false;
            c1DockingTab1.TabPages[2].TabVisible = false;

            c1DockingTab1.SelectedIndex = 0;

        }
        #endregion

        #region fpSpread1_CellDoubleClick() 조회
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    c1DockingTab1.TabPages[0].TabVisible = false;
                    c1DockingTab1.TabPages[1].TabVisible = false;
                    c1DockingTab1.TabPages[2].TabVisible = true;
                    c1DockingTab1.SelectedIndex = 2;

                    Idx = fpSpread1.Sheets[0].Cells[e.Row, 0].Text; //IDX
                    Num = fpSpread1.Sheets[0].Cells[e.Row, 1].Text; //ANS_NUM
                    Lvl = fpSpread1.Sheets[0].Cells[e.Row, 2].Text; //ANS_LEVEL
                    Group = fpSpread1.Sheets[0].Cells[e.Row, 3].Text; //ANS_GROUP

                    string strQuery = " USP_BZG003  'S2'";
                    strQuery = strQuery + ", @pIDX ='" + Idx + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    //질문글 상세정보
                    if (dt.Rows.Count > 0)
                    {
                        txtQTitle.Value = dt.Rows[0]["TITLE"].ToString();
                        txtQShowContent.Value = dt.Rows[0]["CONTENTS"].ToString();
                        lblQInId.Value = dt.Rows[0]["IN_ID"].ToString();
                        lblQInNm.Value = dt.Rows[0]["IN_NM"].ToString();
                        lblQDt.Value = dt.Rows[0]["IN_DT"].ToString();
                        lblQHit.Value = dt.Rows[0]["HIT"].ToString();
                        txtQFileCnt.Value = dt.Rows[0]["FILES_CNT"].ToString();
                        txtQFileName.Value = dt.Rows[0]["FILES_NO"].ToString();
                    }

                    //작성자와 로그인유저ID가 같으면 수정가능하게 아니면 LOCK
                    if (dt.Rows[0]["IN_ID"].ToString() == SystemBase.Base.gstrUserID)
                    {
                        btnQUpDate.Visible = true;
                        btnQDel.Visible = true;
                        cboQPoint.Enabled = false;
                        txtQTitle.ReadOnly = false;
                        txtQShowContent.ReadOnly = false;
                        txtQTitle.BackColor = Color.White;
                        txtQShowContent.BackColor = Color.White;
                    }
                    else
                    {
                        btnQUpDate.Visible = false;
                        btnQDel.Visible = false;
                        cboQPoint.Enabled = true;
                        txtQTitle.ReadOnly = true;
                        txtQShowContent.ReadOnly = true;
                        txtQTitle.BackColor = Color.WhiteSmoke;
                        txtQShowContent.BackColor = Color.WhiteSmoke;
                    }

                    //답변세팅
                    btnAInsert.Visible = true;
                    btnAUpDate.Visible = false;
                    btnADel.Visible = false;
                    cboAPoint.Enabled = false;
                    txtATitle.ReadOnly = true;
                    txtAShowContent.ReadOnly = true;
                    txtATitle.BackColor = Color.WhiteSmoke;
                    txtAShowContent.BackColor = Color.WhiteSmoke;

                    txtATitle.Value = "";
                    txtAShowContent.Value = "";
                    lblAInId.Value = "";
                    lblAInNm.Value = "";
                    lblADt.Value = "";
                    lblAHit.Value = "";
                    txtAFileCnt.Value = "";
                    txtAFileName.Value = "";

                    //답변리스트
                    MemoSearch(Group, Idx);

                    //조회수 업데이트
                    HitUpdate(Idx);

                    //평점콤보박스세팅
                    SystemBase.ComboMake.C1Combo(cboQPoint, "usp_BZG003 'C4'", 0);
                    SystemBase.ComboMake.C1Combo(cboAPoint, "usp_BZG003 'C4'", 0);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 조회수 업데이트
        private void HitUpdate(string idx)
        {
            string ERRCode = "ER", MSGCode = "P0000";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strQuery = " USP_BZG003  'C3'";
                strQuery = strQuery + ", @pIDX = '" + Idx + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Text, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            { 

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
        #endregion

        #region 내용 목록버튼
        private void btnList_Click(object sender, System.EventArgs e)
        {
            c1DockingTab1.TabPages[0].TabVisible = true;
            c1DockingTab1.TabPages[1].TabVisible = false;
            c1DockingTab1.TabPages[2].TabVisible = false;
            c1DockingTab1.SelectedIndex = 0;

            //취소가 되면 첨부되었던 파일을 삭제해 준다.
            if (txtAFileName.Text != "" || txtAFileName.Text != "0")
            {
                string ERRCode = "", MSGCode = "", MSGText = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_BZG003 'C2'";
                    strSql = strSql + ", @pFILES_NO = '" + txtAFileName.Text + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                    MSGText = ds.Tables[0].Rows[0][2].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                
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

            SystemBase.Validation.GroupBox_Reset(groupBox7);

            SearchExec();
        }
        #endregion

        #region 글쓰기 목록버튼
        private void btnList2_Click(object sender, System.EventArgs e)
        {
            c1DockingTab1.TabPages[0].TabVisible = true;
            c1DockingTab1.TabPages[1].TabVisible = false;
            c1DockingTab1.TabPages[2].TabVisible = false;
            c1DockingTab1.SelectedIndex = 0;

            //취소가 되면 첨부되었던 파일을 삭제해 준다.
            if (txtAttFile.Text != "" || txtAttFile.Text != "0")
            {
                string ERRCode = "", MSGCode = "", MSGText = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_BZG003 'C2'";
                    strSql = strSql + ", @pFILES_NO = '" + txtFileName.Text + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                    MSGText = ds.Tables[0].Rows[0][2].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                   
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

            SystemBase.Validation.GroupBox_Reset(groupBox4);

            SearchExec();

        }
        #endregion

        #region 글쓰기
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            WriteFlag = "N";

            SystemBase.Validation.GroupBox_Reset(groupBox4);

            c1DockingTab1.TabPages[0].TabVisible = false;
            c1DockingTab1.TabPages[1].TabVisible = true;
            c1DockingTab1.TabPages[2].TabVisible = false;
            c1DockingTab1.SelectedIndex = 1;

            txtTitle.Focus();
        }
        #endregion

        #region 등록
        private void btnSave2_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
            {
                string ERRCode = "ER", MSGCode = "P0000";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string strType = "I1";
                if (WriteFlag == "R") { strType = "I2"; }

                try
                {
                    string strQuery = " USP_BZG003  '" + strType + "'";
                    strQuery = strQuery + ", @pTITLE ='" + txtTitle.Text + "' ";
                    strQuery = strQuery + ", @pCONTENTS ='" + txtContents.Text + "' ";
                    if (txtAttFile.Text != "")
                    { strQuery = strQuery + ", @pFILES_CNT ='" + txtAttFile.Text + "' "; }
                    strQuery = strQuery + ", @pFILES_NO ='" + txtFileName.Text + "' ";
                    if (Idx != "")
                    { strQuery = strQuery + ", @pIDX = '" + Idx + "' "; }
                    strQuery = strQuery + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Text, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox4); //초기화

                    c1DockingTab1.TabPages[0].TabVisible = true;
                    c1DockingTab1.TabPages[1].TabVisible = false;
                    c1DockingTab1.TabPages[2].TabVisible = false;
                    c1DockingTab1.SelectedIndex = 0;

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

            }
        }
        #endregion

        #region 공지사항
        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            if (strJumpFileName1.Length > 0)
            {
                string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                {	// 폼이 이미 열려있으면 닫기
                    if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                    {
                        MdiParent.MdiChildren[k].BringToFront(); //화면을 앞으로 가져오고.. 
                        MdiParent.MdiChildren[k].Close();
                        break;
                    }
                }
                Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1));
                myForm.MdiParent = this.MdiParent;
                SystemBase.Base.RodeFormID = "BZG001";
                SystemBase.Base.RodeFormText = "공지사항";
                myForm.Show();
            }
        }
        #endregion

        #region 포인트 조회
        private void linkLabel2_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                BZG003P2 frm = new BZG003P2();
                frm.ShowDialog();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }

        }
        #endregion

        #region 글쓰기 첨부파일 추가
        private void btnFileUp_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtFileName.Text.Length == 0)
                {
                    string Query = " usp_BZG003 'C1'";
                    Query = Query + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    Query = Query + ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
                    Query = Query + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows[0][0].ToString() == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(dt.Rows[0][1].ToString(), ""), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        txtFileName.Text = dt.Rows[0][1].ToString();
                    }
                }

                //UIForm.FileUpDown frm = new UIForm.FileUpDown(txtFileName.Text, "Y#Y#Y", txtAttFile);
                //frm.ShowDialog();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "글쓰기 첨부파일 추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 질문, 답변 첨부파일 추가
        private void btnQFileUp_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtQFileName.Text.Length == 0)
                {
                    string Query = " usp_BZG003 'C1'";
                    Query = Query + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    Query = Query + ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
                    Query = Query + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows[0][0].ToString() == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(dt.Rows[0][1].ToString(), ""), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        txtQFileName.Text = dt.Rows[0][1].ToString();
                    }
                }

                //로긴유저와 작성자가 같으면 업로드, 삭제 가능하고, 작성자가 아니면 다운만 가능하게
                if (lblQInId.Text == SystemBase.Base.gstrUserID)
                {
                    //UIForm.FileUpDown frm = new UIForm.FileUpDown(txtQFileName.Text, "Y#Y#Y", txtQFileCnt);
                    //frm.ShowDialog();
                }
                else
                {
                    //UIForm.FileUpDown frm = new UIForm.FileUpDown(txtQFileName.Text, "N#Y#N", txtQFileCnt);
                    //frm.ShowDialog();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "질문, 답변 첨부파일 추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //답변 파일첨부
        private void btnAFileUp_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtAFileName.Text.Length == 0)
                {
                    string Query = " usp_BZG003 'C1'";
                    Query = Query + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    Query = Query + ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "'";
                    Query = Query + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows[0][0].ToString() == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(dt.Rows[0][1].ToString(), ""), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        txtAFileName.Text = dt.Rows[0][1].ToString();
                    }
                }

                //로긴유저와 작성자가 같으면 업로드, 삭제 가능하고, 작성자가 아니면 다운만 가능하게
                if (lblAInId.Text == "" || lblAInId.Text == SystemBase.Base.gstrUserID)
                {
                    //UIForm.FileUpDown frm = new UIForm.FileUpDown(txtAFileName.Text, "Y#Y#Y", txtAFileCnt);
                    //frm.ShowDialog();
                }
                else
                {
                    //UIForm.FileUpDown frm = new UIForm.FileUpDown(txtAFileName.Text, "N#Y#N", txtAFileCnt);
                    //frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "답변 첨부파일 추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 답변 등록
        private void btnAInsert_Click(object sender, System.EventArgs e)
        {
            WriteFlag = "R";

            SystemBase.Validation.GroupBox_Reset(groupBox4);

            c1DockingTab1.TabPages[0].TabVisible = false;
            c1DockingTab1.TabPages[1].TabVisible = true;
            c1DockingTab1.TabPages[2].TabVisible = false;
            c1DockingTab1.SelectedIndex = 1;

            txtTitle.Focus();
        }
        #endregion

        #region 답변조회
        private void AnsSearch()
        {
            string strQuery = " USP_BZG003  'S2'";
            strQuery = strQuery + ", @pTITLE ='" + txtATitle.Text + "' ";
            strQuery = strQuery + ", @pCONTENTS ='" + txtAShowContent.Text + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

        }
        #endregion

        #region 답변 리스트 클릭
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                AIdx = fpSpread2.Sheets[0].Cells[e.Row, 0].Text;
                ANum = fpSpread2.Sheets[0].Cells[e.Row, 1].Text;
                AGroup = fpSpread2.Sheets[0].Cells[e.Row, 3].Text;

                string strQuery = " USP_BZG003  'S2'";
                strQuery = strQuery + ", @pIDX ='" + AIdx + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                //질문글 상세정보
                if (dt.Rows.Count > 0)
                {
                    txtATitle.Text = dt.Rows[0]["TITLE"].ToString();
                    txtAShowContent.Text = dt.Rows[0]["CONTENTS"].ToString();
                    lblAInId.Text = dt.Rows[0]["IN_ID"].ToString();
                    lblAInNm.Text = dt.Rows[0]["IN_NM"].ToString();
                    lblADt.Text = dt.Rows[0]["IN_DT"].ToString();
                    lblAHit.Text = dt.Rows[0]["HIT"].ToString();
                    txtAFileCnt.Text = dt.Rows[0]["FILES_CNT"].ToString();
                    txtAFileName.Text = dt.Rows[0]["FILES_NO"].ToString();
                }

                //작성자와 로그인유저ID가 같으면 수정가능하게 아니면 LOCK
                if (dt.Rows[0]["IN_ID"].ToString() == SystemBase.Base.gstrUserID)
                {
                    btnAUpDate.Visible = true;
                    btnADel.Visible = true;
                    txtATitle.ReadOnly = false;
                    txtAShowContent.ReadOnly = false;
                    cboAPoint.Enabled = false;
                    txtATitle.BackColor = Color.White;
                    txtAShowContent.BackColor = Color.White;
                }
                else
                {
                    btnAUpDate.Visible = false;
                    btnADel.Visible = false;
                    txtATitle.ReadOnly = true;
                    cboAPoint.Enabled = true;
                    txtAShowContent.ReadOnly = true;
                    txtATitle.BackColor = Color.WhiteSmoke;
                    txtAShowContent.BackColor = Color.WhiteSmoke;
                }
            }
        }
        #endregion

        #region 수정
        //수정 공통함수
        private void Update(string Idx, string Flag)
        {
            System.Windows.Forms.GroupBox Gbox;

            if (Flag == "Q") { Gbox = groupBox6; }
            else { Gbox = groupBox7; }

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(Gbox))
            {
                string ERRCode = "ER", MSGCode = "P0000";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string strTitle = txtQTitle.Text;
                string strContents = txtQShowContent.Text;
                string strFilesCnt = txtQFileCnt.Text;
                string strFilesNo = txtQFileName.Text;

                if (Flag != "Q")
                {
                    strTitle = txtATitle.Text;
                    strContents = txtAShowContent.Text;
                    strFilesCnt = txtAFileCnt.Text;
                    strFilesNo = txtAFileName.Text;
                }

                try
                {
                    string strQuery = " USP_BZG003  'U1'";
                    strQuery = strQuery + ", @pTITLE ='" + strTitle + "' ";
                    strQuery = strQuery + ", @pCONTENTS ='" + strContents + "' ";
                    if (strFilesCnt != "")
                    { strQuery = strQuery + ", @pFILES_CNT ='" + strFilesCnt + "' "; }
                    strQuery = strQuery + ", @pFILES_NO ='" + strFilesNo + "' ";
                    strQuery = strQuery + ", @pIDX = '" + Idx + "' ";
                    strQuery = strQuery + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Text, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                
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
        }

        //질문수정
        private void btnQUpDate_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("B0067"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Update(Idx, "Q");
            }
        }
        //답변수정
        private void btnAUpDate_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("B0067"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Update(AIdx, "A");
            }
        }
        #endregion

        #region 삭제
        //공통삭제함수
        private void Delete(string Idx)
        {
            string ERRCode = "ER", MSGCode = "P0000";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strQuery = " USP_BZG003  'D1'";
                strQuery = strQuery + ", @pIDX = '" + Idx + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Text, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                c1DockingTab1.TabPages[0].TabVisible = true;
                c1DockingTab1.TabPages[1].TabVisible = false;
                c1DockingTab1.TabPages[2].TabVisible = false;
                c1DockingTab1.SelectedIndex = 0;

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
        }

        //질문삭제
        private void btnQDel_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("B0047"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Delete(Idx);
            }
        }
        //답변삭제
        private void btnADel_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("B0047"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Delete(AIdx);
            }
        }
        #endregion

        #region 평점주기
        //공통 평점 함수
        private void Point(string Idx, string PointQty)
        {
            string ERRCode = "ER", MSGCode = "P0000";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strQuery = " USP_BZG003  'C5'";
                strQuery = strQuery + ", @pIDX = '" + Idx + "' ";
                strQuery = strQuery + ", @pPOINT = '" + PointQty + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Text, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
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
        }

        //질문 평점
        private void cboQPoint_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("B0048", cboQPoint.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Point(Idx, cboQPoint.SelectedValue.ToString());
            }
        }

        //답변 평점
        private void cboAPoint_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("B0048", cboAPoint.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Point(AIdx, cboAPoint.SelectedValue.ToString());
            }
        }
        #endregion	
    }
}
