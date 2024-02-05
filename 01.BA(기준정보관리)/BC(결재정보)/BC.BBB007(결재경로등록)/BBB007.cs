#region 작성정보
/*********************************************************************/
// 단위업무명 : 결재경로등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-23
// 작성내용 : 결재경로등록 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
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

namespace BC.BBB007
{
    public partial class BBB007 : UIForm.FPCOMM2
    {
        #region 생성자
        public BBB007()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBB007_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboSDiv, "usp_B_COMMON @pType='COMM', @pCODE = 'B045', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboDiv, "usp_B_COMMON @pType='COMM', @pCODE = 'B045', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	//결재TYPE

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            txtSUserId.Value = SystemBase.Base.gstrUserID;
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
                string strQuery = " usp_BBB007  'S1'";
                strQuery = strQuery + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pFORM_ID='" + txtSFormId.Text + "' ";
                strQuery = strQuery + ", @pDIV	= '" + cboSDiv.SelectedValue + "' ";
                strQuery = strQuery + ", @pUSER_ID='" + txtSUserId.Text + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    Text_Set();
                }
                else
                {
                    Text_ReSet();
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
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);

                        int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "메뉴ID")].Text = txtFormId.Text;
                    }
                }
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
                    string strSql = " usp_BBB007  'D2'";
                    strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pFORM_ID='" + txtFormId.Text + "' ";
                    strSql = strSql + ", @pUSER_ID='" + txtUserId.Text + "' ";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                                    case "U": strGbn = "I1"; break;
                                    case "D": strGbn = "D1"; break;
                                    case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_BBB007 '" + strGbn + "'";
                                strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                                strSql = strSql + ", @pFORM_ID = '" + txtFormId.Text.Trim() + "'";
                                strSql = strSql + ", @pUSER_ID = '" + txtUserId.Text.Trim() + "'";
                                strSql = strSql + ", @pDIV = '" + cboDiv.SelectedValue + "'";
                                strSql = strSql + ", @pASSIGN_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text + "'";
                                strSql = strSql + ", @pASSIGN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Seq")].Text + "'";
                                strSql = strSql + ", @pASSIGN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재형태")].Value + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
                        Right_Search(txtUserId.Text, txtFormId.Text, Convert.ToString(cboDiv.SelectedValue));
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
            txtUserId.Tag = "작성자;1;;";
            txtFormId.Tag = "메뉴ID;1;;";
            cboDiv.Tag = "구분;1;;";

            btnUser.Tag = "";
            btnForm.Tag = "";
        }

        private void Text_Set()
        {
            txtUserId.Tag = ";2;;";
            txtFormId.Tag = ";2;;";
            cboDiv.Tag = ";2;;";

            btnUser.Tag = ";2;;";
            btnForm.Tag = ";2;;";
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

                    txtUserId.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "작성자")].Text.ToString();
                    txtFormId.Value = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "메뉴ID")].Text.ToString();
                    cboDiv.SelectedValue = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "구분코드")].Text.ToString();
                    Right_Search(txtUserId.Text, txtFormId.Text, Convert.ToString(cboDiv.SelectedValue));
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
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작성자"))
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text != "")
                    {
                        string strUsernm = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = strUsernm;
                    }
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
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = Msgs[2].ToString();
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = Msgs[3].ToString();

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

        #region 버튼 클릭 이벤트
        private void btnUser_Click(object sender, System.EventArgs e)	//사용자
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, txtUserNm.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Value = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnForm_Click(object sender, System.EventArgs e)	//폼아이디
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'B041' , @pSPEC1='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtFormId.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "메뉴조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtFormId.Value = Msgs[0].ToString();
                    txtFormNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "메뉴조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnSForm_Click(object sender, System.EventArgs e)	//폼아이디 조회
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE = 'B041' , @pSPEC1='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSFormId.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "메뉴조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSFormId.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "메뉴조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region 텍스트 변환시
        private void txtUserId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtUserId.Text != "")
                {
                    if (txtUserId.Text != "")
                    {
                        txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtUserNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtFormId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtFormId.Text != "")
                {
                    if (txtFormId.Text != "")
                    {
                        txtFormNm.Value = SystemBase.Base.CodeName("MENU_ID", "MENU_NAME", "CO_SYS_MENU", txtFormId.Text, "");
                    }
                    else
                    {
                        txtFormNm.Value = "";
                    }
                }
            }
            catch
            {

            }

        }
        #endregion

        #region 좌측그리드 방향키 이동시 우측조회
        private void Right_Search(string strUser, string strForm, string strDiv)
        {
            try
            {
                if (strUser.ToString() != "" && strForm.ToString() != "")
                {
                    string strSql = " usp_BBB007  'S2'";
                    strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strSql = strSql + ", @pUSER_ID = '" + strUser + "'";
                    strSql = strSql + ", @pFORM_ID = '" + strForm + "'";
                    strSql = strSql + ", @pDIV = '" + strDiv + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    if (fpSpread1.Sheets[0].Rows.Count > 0) Text_Set();
                    else Text_ReSet();

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

        #region 유저중복체크 User_Check()
        private int User_Check(string struserId)
        {
            int intCheck = 1;

            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                {
                    if (struserId == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text) { intCheck = 0; break; }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재자 중복 체크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
                intCheck = 0;
            }

            return intCheck;
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
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = dt.Rows[0][2].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = dt.Rows[0][3].ToString();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = "";
                }
            }
        }
        #endregion
    }
}