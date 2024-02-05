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
        #region 변수
        string strAdminYn = "N";
        string strFinanceYn = "N";
        string strDefaultAssignOwner = "MAKE";      // 기본 결재구분을 MAKE(발의)로 처리되게 하기 위해 선언.
        string strDefaultAssignType = "B";          // 기본 결재단계을 B(검토)로 처리되게 하기 위해 선언.
        #endregion


        #region 생성자
        public BBB008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBB008_Load(object sender, System.EventArgs e)
        {
            txtSUserId.Value = SystemBase.Base.gstrUserID;      // 사용자ID 검색조건에 로그인한 사용자ID로
            txtUserId.Value = SystemBase.Base.gstrUserID;

            SystemBase.ComboMake.C1Combo(cboSTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboSRoutNo, "usp_B_COMMON @pType='COMM', @pCODE = 'B097', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            SystemBase.ComboMake.C1Combo(cboTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            SystemBase.ComboMake.C1Combo(cboRoutNo, "usp_B_COMMON @pType='COMM', @pCODE = 'B097', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재선번호")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B097', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B091', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'REL1', @pSPEC1 = 'Y', @pCODE = 'B096', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);  // 등록조회여부가 Y인 건

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "결재선번호")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B097', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "업무구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            cboTaskType.SelectedIndex = 0;
            cboRoutNo.SelectedIndex = 0;

            //AssignDescr_Search();
            Check_RollGroup();

            //Text_ReSet();
            SearchExec();

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
                strQuery = strQuery + ", @pUSR_ID ='" + txtSUserId.Text + "' ";
                strQuery = strQuery + ", @pROUT_NO = '" + cboSRoutNo.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @pTASK_TYPE = '" + cboSTaskType.SelectedValue.ToString() + "' ";                

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    // 조회시 맨 첫번째 라인의 결재번호에 대한 내용이 우측에 조회되도록 함.
                    txtUserId.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID")].Text.ToString();
                    cboRoutNo.SelectedValue = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "결재선번호")].Value.ToString();
                    txtRoutNm.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "결재선명세")].Value.ToString();
                    chkMajorYn.Checked = (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "기본결재선")].Value.ToString() == "Y" ? true : false);
                    cboTaskType.SelectedValue = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "업무구분")].Value.ToString();

                    Right_Search(txtUserId.Text, cboRoutNo.SelectedValue.ToString(), cboTaskType.SelectedValue.ToString());
                }
                else
                {
                    fpSpread1.Sheets[0].Rows.Count = 0;
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

                txtSUserId.Value = SystemBase.Base.gstrUserID;

                Text_ReSet();
                SystemBase.Validation.GroupBox_Setting(groupBox2);

                txtUserId.Value = SystemBase.Base.gstrUserID;
                txtAdminRollYn.Value = strAdminYn;
                txtFinanceDeptYn.Value = strFinanceYn;

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                AssignDescr_Search();

                Right_Search_Grid(txtUserId.Text, cboRoutNo.SelectedValue.ToString(), cboTaskType.SelectedValue.ToString());    // 2022.02.09. hma 추가: 해당 결재선번호에 대한 데이터가 조회되게
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
                if (txtUserId.Text == "")
                {
                    MessageBox.Show("사용자ID를 입력하세요.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUserId.Focus();
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

                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text = txtUserId.Text;
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = txtUserNm.Text;
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재선번호")].Value = cboRoutNo.SelectedValue.ToString();
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재선명세")].Text = txtRoutNm.Text;
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무코드")].Value = cboTaskType.SelectedValue.ToString();
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무구분")].Text = cboTaskType.Text;
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value = strDefaultAssignType;
                fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value = strDefaultAssignOwner;
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
            if (txtUserId.Text == "") return;

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
                    strSql = strSql + ", @pUSR_ID = '" + txtUserId.Text + "' ";
                    strSql = strSql + ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "' ";
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
            // 필수항목 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))// 그리드 상단 필수항목 체크
                {
                    //RoutHdr_Save();     // 헤더정보 저장
                    if (txtRoutNm.Text == "")
                    {
                        MessageBox.Show("선택된 결재선번호에 대한 명세를 입력해주세요.");
                        txtRoutNm.Focus();
                    }

                    string ERRCode = "ER", MSGCode = "P0000";
                    DataSet ds;
                    string strResultMsg = "";

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_BBB008 'I1'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strSql = strSql + ", @pUSR_ID = '" + txtUserId.Text.Trim() + "'";
                        strSql = strSql + ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "'";
                        strSql = strSql + ", @pROUT_NM = '" + txtRoutNm.Text.Trim() + "'";
                        strSql = strSql + ", @pMAJOR_YN = '" + (chkMajorYn.Checked == true ? "Y" : "N") + "'";
                        strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        strSql = "";

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
                                    case "D": strGbn = "D1"; break;
                                    case "I": strGbn = "I2"; break;
                                    default: strGbn = ""; break;
                                }

                                strSql = " usp_BBB008 '" + strGbn + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                                strSql = strSql + ", @pUSR_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text + "'";
                                strSql = strSql + ", @pROUT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재선번호")].Value.ToString() + "'";
                                strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";                               
                                strSql = strSql + ", @pASSIGN_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text + "'";
                                strSql = strSql + ", @pASSIGN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서")].Text + "'";
                                strSql = strSql + ", @pASSIGN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value.ToString() + "'";
                                strSql = strSql + ", @pASSIGN_OWNER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value.ToString() + "'";
                                strSql = strSql + ", @pASSIGN_REMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                //strAssign_id = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text;
                                ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                strResultMsg = MSGCode;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                        // 저장된 상태의 해당 업무구분/사용자/결재선번호 결재라인 데이터 체크
                        strSql = " usp_BBB008 'C2'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strSql = strSql + ", @pUSR_ID = '" + txtUserId.Text.Trim() + "'";
                        strSql = strSql + ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "'";
                        strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";

                        ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (MSGCode == "")
                            MSGCode = strResultMsg;

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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
                        Right_Search(txtUserId.Text, cboRoutNo.SelectedValue.ToString(), cboTaskType.SelectedValue.ToString());
                        //UIForm.FPMake.GridSetFocus(fpSpread1, strAssign_id);
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
            if (strAdminYn == "Y")
            {
                txtUserId.Value = String.Empty;
                txtUserNm.Value = String.Empty;
                cboRoutNo.SelectedIndex = 0;
                txtRoutNm.Value = String.Empty;
                cboRoutNo.SelectedIndex = 0;
                cboTaskType.SelectedIndex = 0;
                chkMajorYn.Checked = false;
            }
            else
            {
                txtRoutNm.Value = String.Empty;
                chkMajorYn.Checked = false;
            }

            fpSpread1.Sheets[0].Rows.Count = 0;

            txtUserId.Tag = "사용자ID;1;;";
            cboRoutNo.Tag = "결재선번호;1;;";
            txtRoutNm.Tag = "결재선명세;1;;";
            btnUser.Tag = "";
            cboTaskType.Tag = "업무구분;1;;";
        }

        private void Text_Set()
        {
            txtUserId.Tag = ";2;;";
            cboRoutNo.Tag = ";2;;";
            btnUser.Tag = ";2;;";
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

                    txtUserId.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "사용자ID")].Text.ToString();
                    cboRoutNo.SelectedValue = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결재선번호")].Value.ToString();
                    txtRoutNm.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결재선명세")].Value.ToString();
                    chkMajorYn.Checked = (fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "기본결재선")].Value.ToString() == "Y" ? true : false);
                    cboTaskType.SelectedValue = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "업무구분")].Value.ToString();

                    Right_Search(txtUserId.Text, cboRoutNo.SelectedValue.ToString(), cboTaskType.SelectedValue.ToString());
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
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자부서")].Text = Msgs[3].ToString();
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
        private void Right_Search(string strUserId, string strRoutNo, string strTaskType)
        {
            try
            {
                if (strUserId.ToString() != "" && strRoutNo.ToString() != "" && strTaskType.ToString() != "")
                {
                    string strSql = " usp_BBB008  'S2'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strSql = strSql + ", @pUSR_ID = '" + strUserId + "'";
                    strSql = strSql + ", @pROUT_NO = '" + strRoutNo + "'";
                    strSql = strSql + ", @pTASK_TYPE = '" + strTaskType + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        Text_Set();
                    }
                    else
                    {
                        Text_ReSet();
                    }

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

        // 2022.02.09. hma 추가(Start)
        #region 우측 그리드 데이터 조회만 되게. 초기화 버튼 클릭시 사용 위해
        private void Right_Search_Grid(string strUserId, string strRoutNo, string strTaskType)
        {
            try
            {
                if (strUserId.ToString() != "" && strRoutNo.ToString() != "" && strTaskType.ToString() != "")
                {
                    string strSql = " usp_BBB008  'S2'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strSql = strSql + ", @pUSR_ID = '" + strUserId + "'";
                    strSql = strSql + ", @pROUT_NO = '" + strRoutNo + "'";
                    strSql = strSql + ", @pTASK_TYPE = '" + strTaskType + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

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
        // 2022.02.09. hma 추가(End)

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
        #region btnSUser_Click(): 좌측 사용자 검색 버튼 클릭시. 사용자 팝업 띄움.
        private void btnSUser_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'B011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSUserId.Text = Msgs[0].ToString();
                    txtSUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region btnUser_Click(): 우측 사용자 검색 버튼 클릭시. 사용자 팝업 띄움.
        private void btnUser_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'B011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
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
        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtSUserId_TextChanged(object sender, EventArgs e)
        {
            txtSUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 결재라인등록(멀티)
        private void btnCharge_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("사용자ID가 입력되지 않았습니다. 사용자ID를 입력해주세요.");
                return;
            }
            if (string.IsNullOrEmpty(cboTaskType.Text))
            {
                MessageBox.Show("업무구분이 선택되지 않았습니다. 업무구분을 선택해주세요.");
                return;
            }

            // 2022.02.09. hma 추가(Start): 그리드 행수 저장. 0인 경우 결재자등록(멀티) 팝업에서 선택하여 행추가시 결재선번호 자동으로 들어가게 하기 위해.
            int iGridCnt = 0, iAssignSeq = 0;               
            iGridCnt = fpSpread1.Sheets[0].Rows.Count;
            // 2022.02.09. hma 추가(End)

            WNDW.WNDW050 Dialog = new WNDW.WNDW050();
            Dialog.ShowDialog();

            if (Dialog.DialogResult == DialogResult.OK)
            {
                DataTable FormDt = new DataTable();
                FormDt = Dialog.ReturnDt;

                if (FormDt != null)
                {
                    for (int i = 0; i < FormDt.Rows.Count; i++)
                    {                       
                        UIForm.FPMake.RowInsert(fpSpread1);//행추가
                        int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text = txtUserId.Text;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = txtUserNm.Text;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재선번호")].Value = cboRoutNo.SelectedValue.ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재선명세")].Text = txtRoutNm.Text;

                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무코드")].Value = cboTaskType.SelectedValue.ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무구분")].Text = cboTaskType.Text;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "기본결재선")].Text = (chkMajorYn.Checked ? "Y" : "N");

                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text = FormDt.Rows[i]["결재자"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = FormDt.Rows[i]["결재자명"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value = FormDt.Rows[i]["결재단계"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value = strDefaultAssignOwner;

                        // 2022.02.09. hma 추가(Start): 그리드 라인이 0인 경우에만 결재순서 자동 지정되게.
                        if (iGridCnt == 0)
                        {
                            iAssignSeq = iAssignSeq + 10;
                            fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서")].Text = Convert.ToString(iAssignSeq);
                        }
                        // 2022.02.09. hma 추가(End)
                    }
                }
            }
        }
        #endregion

        #region cboRoutNo_SelectedValueChanged(): 결재선번호 변경시 해당 결재선에 대한 명세를 보여준다.
        private void cboRoutNo_SelectedValueChanged(object sender, EventArgs e)
        {
            AssignDescr_Search();

            RoutNo_Grid_Search();
        }

        private void AssignDescr_Search()
        { 
            // 선택된 결재선번호에 대한 명세와 기본결재선여부를 가져와서 보여준다.
            string strQuery = " usp_BBB008 @pTYPE = 'S3'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery += ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "' ";
            strQuery += ", @pUSR_ID = '" + txtUserId.Text.Trim() + "' ";
            strQuery += ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtRoutNm.Value = ds.Tables[0].Rows[0]["ROUT_NM"].ToString();
                chkMajorYn.Checked = (ds.Tables[0].Rows[0]["MAJOR_YN"].ToString() == "Y" ? true : false);
            }
            else
            {
                txtRoutNm.Text = "";
                chkMajorYn.Checked = false;

                fpSpread1.Sheets[0].Rows.Count = 0;
            }
        }
        #endregion

        #region RoutNo_Grid_Search(): 선택된 결재선번호에 대한 결재라인정보 조회한다.
        private void RoutNo_Grid_Search()
        {
            string strSql = " usp_BBB008  'S2'";
            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
            strSql = strSql + ", @pUSR_ID = '" + txtUserId.Text + "'";
            strSql = strSql + ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "'";
            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region btnRoutNmSave_Click(): 입력된 결재선번호 명세를 저장한다.
        private void btnRoutNmSave_Click(object sender, EventArgs e)
        {
            RoutHdr_Save();
        }

        #region RoutHdr_Save(): 결재라인헤더 저장. 이때는 기본결재선 갯수를 체크하지 않는다. 변경하고자 할 경우를 위해.
        private void RoutHdr_Save()
        { 
            if (txtRoutNm.Text == "")
            {
                MessageBox.Show("선택된 결재선번호에 대한 명세를 입력해주세요.");
                txtRoutNm.Focus();
            }
            else
            { 
                string ERRCode = "ER", MSGCode = "P0000";
                string strAssign_id = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_BBB008 'U1'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pUSR_ID = '" + txtUserId.Text.Trim() + "'";
                    strSql = strSql + ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pROUT_NM = '" + txtRoutNm.Text.Trim() + "'";
                    strSql = strSql + ", @pMAJOR_YN = '" + (chkMajorYn.Checked == true ? "Y" : "N") + "'";
                    strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                    Right_Search(txtUserId.Text, cboRoutNo.SelectedValue.ToString(), cboTaskType.SelectedValue.ToString());
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
        #endregion
        #endregion

        #region Check_RollGroup(): 사용자ID에 대한 관리자권한여부 체크해서 관리자권한이 없는 사용자인 경우 사용자 항목 및 검색 버튼 비활성화 처리.
        private void Check_RollGroup()
        {
            string strQuery = " usp_BBB008 @pTYPE = 'C1'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery += ", @pUSR_ID = '" + txtUserId.Text.Trim() + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            txtAdminRollYn.ReadOnly = false;
            txtFinanceDeptYn.ReadOnly = false;

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtAdminRollYn.Value = ds.Tables[0].Rows[0]["ADMIN_YN"].ToString();
                txtFinanceDeptYn.Value = ds.Tables[0].Rows[0]["FINANCE_YN"].ToString();

                strAdminYn = ds.Tables[0].Rows[0]["ADMIN_YN"].ToString();
                strFinanceYn = ds.Tables[0].Rows[0]["FINANCE_YN"].ToString();
            }
            else
            {
                txtAdminRollYn.Value = "";
                txtFinanceDeptYn.Value = "";
            }

            txtAdminRollYn.ReadOnly = true;
            txtFinanceDeptYn.ReadOnly = true;

            if (txtAdminRollYn.Text == "Y")
            {
                txtSUserId.Enabled = true;
                txtUserId.Enabled = true;
                btnSUser.Enabled = true;
                btnUser.Enabled = true;
            }
            else
            {
                txtSUserId.Enabled = false;
                txtUserId.Enabled = false;
                btnSUser.Enabled = false;
                btnUser.Enabled = false;
            }
        }
        #endregion
    }
}