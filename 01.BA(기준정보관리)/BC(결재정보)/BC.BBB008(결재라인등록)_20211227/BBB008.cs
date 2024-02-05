#region 작성정보
/*********************************************************************/
// 단위업무명:  결재라인등록
// 작 성 자  :  한 미 애
// 작 성 일  :  2021-11-29
// 작성내용  :  결재라인등록
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace BC.BBB008
{
    public partial class BBB008 : UIForm.FPCOMM2
    {
        #region 생성자
        public BBB008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBB008_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboSTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B091', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B096', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "업무구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            txtSDeptCd.Value = SystemBase.Base.gstrDEPT;
            Text_ReSet();

            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            try
            {
                string strQuery = " usp_BBB008  'S1'";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pDEPT_CD ='" + txtSDeptCd.Text + "' ";
                strQuery = strQuery + ", @pTASK_TYPE = '" + cboSTaskType.SelectedValue + "' ";                

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    //Text_Set();
                }
                else
                {
                    //Text_ReSet();
                }

                SystemBase.Validation.GroupBox_Setting(groupBox2);
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                //txtSUserId.Value = SystemBase.Base.gstrUserID;
                Text_ReSet();
                SystemBase.Validation.GroupBox_Setting(groupBox2);
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "신규 버튼 클릭"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region RowInsExec 행 삭제, 추가
        protected override void RowInsExec()
        {
            try
            {
                if (txtDeptCd.Text == "")
                {
                    MessageBox.Show("부서코드를 입력하세요.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDeptCd.Focus();
                    return;
                }

                if (cboTaskType.Text == "")
                {
                    MessageBox.Show("업무구분을 입력하세요.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboTaskType.Focus();
                    return;
                }

                UIForm.FPMake.RowInsert(fpSpread1);
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;                

                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = txtDeptCd.Text;
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명1")].Text = txtDeptNm.Text;
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무코드")].Value = cboTaskType.SelectedValue.ToString();
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무구분")].Text = cboTaskType.Text;
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가 버튼 클릭"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (cboTaskType.Text == "") return;
            if (txtDeptCd.Text == "") return;

            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_BBB008  'D2'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "' ";                    

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    NewExec();
                    SearchExec();
                    fpSpread1.Sheets[0].Rows.Count = 0;
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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))// 그리드 상단 필수항목 체크
                {
                    string ERRCode = "ER", MSGCode = "P0000";
                    string strAssign_id = "";
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
                                    case "U": strGbn = "U1"; break;
                                    case "D": strGbn = "D1"; break;
                                    case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_BBB008 '" + strGbn + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                                strSql = strSql + ", @pDEPT_CD = '" + txtDeptCd.Text.Trim() + "'";
                                strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";                               
                                strSql = strSql + ", @pASSIGN_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text + "'";
                                strSql = strSql + ", @pASSIGN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서")].Text + "'";
                                strSql = strSql + ", @pASSIGN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value.ToString() + "'";
                                strSql = strSql + ", @pASSIGN_OWNER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value.ToString() + "'";
                                strSql = strSql + ", @pASSIGN_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                strAssign_id = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text;
                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        Trans.Commit();
                    }
                    catch
                    {
                        Trans.Rollback();
                        MSGCode = "P0001";
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SearchExec();
                        Right_Search(txtDeptCd.Text, Convert.ToString(cboTaskType.SelectedValue));
                        UIForm.FPMake.GridSetFocus(fpSpread1, strAssign_id);

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
        }
        #endregion

        #region Text Box Setting
        private void Text_ReSet()
        {
            txtDeptCd.Tag = "부서코드;1;;";
            btnDept.Tag = "";
            cboTaskType.Tag = "업무구분;1;;";
        }

        private void Text_Set()
        {
            txtDeptCd.Tag = ";2;;";
            btnDept.Tag = ";2;;";
            cboTaskType.Tag = ";2;;";
        }
        #endregion

        #region 좌측그리드 방향키 이동시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    txtDeptCd.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "부서")].Text.ToString();
                    cboTaskType.SelectedValue = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "업무구분")].Value.ToString();
                    Right_Search(txtDeptCd.Text, cboTaskType.SelectedValue.ToString());
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region FpSpead 컬럼 변환시 Name 조회
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        { 
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "결재자"))
                {                   
                    string strUsernm = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = strUsernm;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 Change 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text == "I")
                {
                    if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "결재자_2"))
                    {
                        string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                        string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                        string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text, "" };
                        UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            Regex rx1 = new Regex("#");
                            string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text = Msgs[0].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = Msgs[1].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명2")].Text = Msgs[3].ToString();

                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region 좌측그리드 방향키 이동시 우측조회
        private void Right_Search(string strDept, string strTaskType)
        {
            try
            {
                if (strDept.ToString() != "" && strTaskType.ToString() != "")
                {
                    string strSql = " usp_BBB008  'S2'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strSql = strSql + ", @pDEPT_CD = '" + strDept + "'";
                    strSql = strSql + ", @pTASK_TYPE = '" + strTaskType + "'";
                    strSql = strSql + ", @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                        Text_Set();
                    else
                        Text_ReSet();

                    SystemBase.Validation.GroupBox_Setting(groupBox2);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region fpSpread1_ChangeEvent
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "결재자"))
            {
                string Query = "usp_B_COMMON @pTYPE = 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text + "'  , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = dt.Rows[0][1].ToString();
                    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = dt.Rows[0][2].ToString();
                    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명2")].Text = dt.Rows[0][3].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = "";
                    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = "";
                    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명1")].Text = "";
                }
            }
        }
        #endregion

        #region 검색조건 버튼 클릭 이벤트 처리
        #region btnSDept_Click(): 좌측 부서코드 검색 버튼 클릭시. 부서코드 팝업 띄움.
        private void btnSDept_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'D022', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00015", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSDeptCd.Text = Msgs[0].ToString();
                    txtSDeptNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
            //try
            //{
            //    string strCurDate = SystemBase.Base.ServerTime("YYMMDD");
            //    WNDW.WNDW011 pu = new WNDW.WNDW011(strCurDate);
            //    pu.MaximizeBox = false;
            //    pu.ShowDialog();
            //    if (pu.DialogResult == DialogResult.OK)
            //    {
            //        string[] Msgs = pu.ReturnVal;

            //        txtDeptCd.Value = Msgs[1].ToString();
            //        txtDeptCd.Focus();
            //    }
            //}
            //catch (Exception f)
            //{
            //    SystemBase.Loggers.Log(this.Name, f.ToString());
            //    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            //}
        }
        #endregion

        #region btnDept_Click(): 우측 부서코드 검색 버튼 클릭시. 부서코드 팝업 띄움.
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'D022', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00015", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Text = Msgs[0].ToString();
                    txtDeptNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion
        #endregion

        #region txtDeptCd_TextChanged(): 텍스트 변경시 이벤트 처리
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            txtDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtDeptCd.Text, " AND REORG_ID = (SELECT REORG_ID FROM B_REORG_INFO(NOLOCK) WHERE USE_FLAG = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "')");
        }

        private void txtSDeptCd_TextChanged(object sender, EventArgs e)
        {
            txtSDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtSDeptCd.Text, " AND REORG_ID = (SELECT REORG_ID FROM B_REORG_INFO(NOLOCK) WHERE USE_FLAG = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "')");
        }
        #endregion
    }
}