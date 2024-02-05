

#region 작성정보
/*********************************************************************/
// 단위업무명 : 계정코드등록
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-05
// 작성내용 : 계정코드등록
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace AA.ACA002
{
    
    public partial class ACA002 : UIForm.Buttons    {
        #region 변수선언
        string SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        string strACCT_CD = "";
        string strFIGNO = "";
        string strENTRY_YN = "";

        #endregion
        public ACA002()
        {
            InitializeComponent();
        }
        

        #region Form Load 시
        private void ACA002_Load(object sender, System.EventArgs e)
        {
            Control_Load();
            SearchExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            optEntry_Y.Checked = true;
            optUse_Y.Checked = true;
            optDR.Checked = true;

            cboBizAreaCd.SelectedValue = SystemBase.Base.gstrBIZCD;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                TreeViewSearch();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "TreeView 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            GroupBox[] gBox = null;
            string strGbn = "";

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                //컨트롤 체크값 초기화
                SaveData = "";
                //컨트롤 체크 함수
                gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref SaveData);

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if (SearchData == SaveData)
                {
                    //변경되거나 처리할 데이터가 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    
                    if (txtAcctCd.ReadOnly == true)
                    {
                        strGbn = "U1";
                    }
                    else
                    {
                        strGbn = "I1";
                    }

                    string strSql = " usp_ACA002 '" + strGbn + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strSql = strSql + ", @pACCT_CD = '" + txtAcctCd.Text.ToUpper().Trim() + "'";
                    strSql = strSql + ", @pUP_ACCT_CD= '" + txtUpAcctCd.Text + "'";
                    strSql = strSql + ", @pACCT_NM = '" + txtAcctNm.Text + "'";
                    strSql = strSql + ", @pACCT_FULL_NM = '" + txtAcctfullNm.Text + "'";
                    if (optDR.Checked == true)
                    {
                        strSql = strSql + ", @pDR_CR = 'DR'";
                    }
                    else
                    {
                        strSql = strSql + ", @pDR_CR = 'CR'";
                    }
                    
                    if (optEntry_Y.Checked == true)
                    {
                        strSql = strSql + ", @pENTRY_YN = 'Y'";
                    }
                    else
                    {
                        strSql = strSql + ", @pENTRY_YN = 'N'";
                    }
                    if (optUse_Y.Checked == true)
                    {
                        strSql = strSql + ", @pUSE_YN = 'Y'";
                    }
                    else
                    {
                        strSql = strSql + ", @pUSE_YN = 'N'";
                    }
                    strSql = strSql + ", @pACCT_TYPE = '" + cboAcctType.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pSTATEMENT_DIV = '" + cboStatementDiv.SelectedValue.ToString() + "'";

                    strSql = strSql + ", @pSUB_TYPE = '" + cboSubType.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD1 = '" + cboCtrlCd1.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL1 = '" + cboCtrlNull1.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD2 = '" + cboCtrlCd2.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL2 = '" + cboCtrlNull2.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD3 = '" + cboCtrlCd3.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL3 = '" + cboCtrlNull3.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD4 = '" + cboCtrlCd4.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL4 = '" + cboCtrlNull4.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD5 = '" + cboCtrlCd5.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL5 = '" + cboCtrlNull5.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD6 = '" + cboCtrlCd6.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL6 = '" + cboCtrlNull6.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD7 = '" + cboCtrlCd7.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL7 = '" + cboCtrlNull7.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_CD8 = '" + cboCtrlCd8.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pCTRL_NULL8 = '" + cboCtrlNull8.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pREMARK= '" + txtRemark.Text + "'";
                    strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                    strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    string strTempAcctCd = txtAcctCd.Text;
                    treeAdd(strGbn);
                    //SearchExec();
                    strACCT_CD = strTempAcctCd;
                    Right_Search(strACCT_CD);

                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER") //ERROR
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else   //ERRCode == "WR" WARING
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DelExec() 계정 삭제 로직
        protected override void DelExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (txtAcctCd.ReadOnly == true)
            {
                string strMsg = "계정코드[" + txtAcctCd.Text + "] 계정명[" + txtAcctNm.Text + "] 삭제하시겠습니까?";
                if (MessageBox.Show(strMsg, "삭제", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_ACA002 'D1'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pACCT_CD  = '" + txtAcctCd.Text + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        //SearchExec();
                        treeAdd("D1");
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 트리조회
        public void TreeViewSearch()
        {
            try
            {
                treeView1.Nodes.Clear();


                string Query = " usp_ACA002  'S1'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataView dvwData = null;
                    CommonTreeView_Acct(ds.Tables[0].Rows[0]["UP_ACCT_CD"].ToString()
                        , ds.Tables[0].Rows[0]["FIGNO"].ToString()
                        , (TreeNode)null
                        , treeView1
                        , ds
                        , dvwData
                        , imageList1
                        , 0
                        , false);

                    treeView1.Focus();
                    treeView1.Nodes[0].Expand();
                    //treeView1.ExpandAll();
                }
                else
                {
                    SystemBase.Base.GroupBoxReset(groupBox1);

                    MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "TreeView 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 노드 추가 수정 삭제
        public void treeAdd(string TYPE_CD)
        {
            try
            {
                if (TYPE_CD == "I1")
                {
                    //추가

                    TreeNode node = treeView1.SelectedNode;   //첫번째 노드를 objNode에 담습니다.
                    string strNewFIGNO = "";
                    string strNode = "";
                    if (node.Nodes.Count == 0)
                    {
                        strNewFIGNO = node.Tag.ToString() + "001";
                    }
                    else
                    {
                        strNode = node.Nodes[node.Nodes.Count - 1].Tag.ToString();
                        strNewFIGNO = strNode.Substring(strNode.IndexOf("||") + 2, strNode.Length - strNode.IndexOf("||") - 2);
                        strNewFIGNO = Convert.ToString(Convert.ToInt64(strNewFIGNO) + 1);
                    }
                    TreeNode zNode;
                    strNode = txtAcctCd.Text + "||" + strNewFIGNO;

                    zNode = node.Nodes.Add("[" + txtAcctCd.Text + "]" + txtAcctNm.Text);
                    zNode.Tag = strNode;
                    if (optEntry_Y.Checked == true)
                    {
                        zNode.ImageIndex = 2;
                        zNode.SelectedImageIndex = 2;
                        strENTRY_YN = "2";
                    }
                    else
                    {
                        zNode.ImageIndex = 1;
                        zNode.SelectedImageIndex = 1;
                        strENTRY_YN = "1";
                    }
                    strACCT_CD = strNode.Substring(0, strNode.IndexOf("||"));
                    strFIGNO = strNode.Substring(strNode.IndexOf("||") + 2, strNode.Length - strNode.IndexOf("||") - 2);
                }
                else if (TYPE_CD == "U1")
                {
                    //수정
                    TreeNode tNode = treeView1.SelectedNode;
                    tNode.Text = "[" + txtAcctCd.Text + "]" + txtAcctNm.Text;
                    if (optEntry_Y.Checked == true)
                    {
                        tNode.ImageIndex = 2;
                        tNode.SelectedImageIndex = 2;
                        strENTRY_YN = "2";
                    }
                    else
                    {
                        tNode.ImageIndex = 1;
                        tNode.SelectedImageIndex = 1;
                        strENTRY_YN = "1";
                    }
                }
                else if (TYPE_CD == "D1")
                {
                    //삭제
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }

                
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 계정코드등록 트리

        public void CommonTreeView_Acct(
            string iParent,
            string iFigNo,
            TreeNode pNode,
            System.Windows.Forms.TreeView treeView1,
            DataSet ds,
            DataView dvwData,
            ImageList imageList1,
            int starts,
            bool isNeedColor)
        {
            try
            {
                treeView1.ImageList = imageList1;

                if (iParent.ToString() == iParent)
                {	// 루트 메뉴인 경우
                    dvwData = new DataView(ds.Tables[0]);
                    dvwData.RowFilter = "[UP_ACCT_CD] = '" + iParent + "' AND [FIGNO] LIKE '" + iFigNo + "%'";
                    starts++;
                }
                else
                {	// 하위 메뉴
                    if (starts > 0)
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[UP_ACCT_CD] = '" + iParent + "'";
                    }
                    else
                    {
                        dvwData = new DataView(ds.Tables[0]);
                        dvwData.RowFilter = "[ACCT_CD] = '" + iParent.ToString() + "'";
                        starts++;
                    }
                }

                foreach (DataRowView Row in dvwData)
                {
                    TreeNode zNode;

                    if (pNode == null)
                    {
                        zNode = treeView1.Nodes.Add("[" + Row["ACCT_CD"].ToString()  + "]" + Row["ACCT_NM"].ToString());
                        zNode.Tag = Row["ACCT_CD"].ToString() + "||" + Row["FIGNO"].ToString();
                        if (Row["ENTRY_YN"].ToString() == "X")
                        {
                            zNode.ImageIndex = 0;
                            zNode.SelectedImageIndex = 0;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "N")
                        {
                            zNode.ImageIndex = 1;
                            zNode.SelectedImageIndex = 1;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "Y")
                        {
                            zNode.ImageIndex = 2;
                            zNode.SelectedImageIndex = 2;
                        }

                        string strNode = zNode.FullPath;

                        CommonTreeView_Acct(Row["ACCT_CD"].ToString(), Row["FIGNO"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                    else
                    {
                        zNode = pNode.Nodes.Add("[" + Row["ACCT_CD"].ToString() + "]" + Row["ACCT_NM"].ToString());
                        zNode.Tag = Row["ACCT_CD"].ToString() + "||" + Row["FIGNO"].ToString();
                        if (Row["ENTRY_YN"].ToString() == "X")
                        {
                            zNode.ImageIndex = 0;
                            zNode.SelectedImageIndex = 0;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "N")
                        {
                            zNode.ImageIndex = 1;
                            zNode.SelectedImageIndex = 1;
                        }
                        else if (Row["ENTRY_YN"].ToString() == "Y")
                        {
                            zNode.ImageIndex = 2;
                            zNode.SelectedImageIndex = 2;
                        }

                        string strNode = zNode.FullPath;

                        CommonTreeView_Acct(Row["ACCT_CD"].ToString(), Row["FIGNO"].ToString(), zNode, treeView1, ds, dvwData, imageList1, starts, isNeedColor);
                    }
                    //// 폰트 색깔 처리 추가
                    //if (isNeedColor && Row["COLOR"].ToString() == "FR")
                    //    zNode.ForeColor = Color.Red;
                    //else
                    //    zNode.ForeColor = Color.Black;
                }
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log("TreeView 생성오류", e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TreeView 생성"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #endregion

        #region 화면 컨트롤 SETTING
        private void Control_Load()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //////////////////////////// 콤보박스 SETTING ////////////////////////////////////////////////////////////////////////
            //SystemBase.ComboMake.C1Combo(cboDrCr, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A112'", 9);   //차대구분
            SystemBase.ComboMake.C1Combo(cboAcctType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A100', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //계정특성
            SystemBase.ComboMake.C1Combo(cboStatementDiv, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A119', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //재무재표구분
            SystemBase.ComboMake.C1Combo(cboSubType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A146', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //SUB시스테유형
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //본지적관계사업장

            string strSql = "SELECT CTRL_CD, CTRL_NM, 'N' FROM A_SLIP_CTRL_CODE WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
            SystemBase.ComboMake.C1Combo(cboCtrlCd1, strSql, 9);      //관리항목1
            SystemBase.ComboMake.C1Combo(cboCtrlNull1, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수1
            SystemBase.ComboMake.C1Combo(cboCtrlCd2, strSql, 9);      //관리항목2
            SystemBase.ComboMake.C1Combo(cboCtrlNull2, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수2
            SystemBase.ComboMake.C1Combo(cboCtrlCd3, strSql, 9);      //관리항목3
            SystemBase.ComboMake.C1Combo(cboCtrlNull3, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수3
            SystemBase.ComboMake.C1Combo(cboCtrlCd4, strSql, 9);      //관리항목4
            SystemBase.ComboMake.C1Combo(cboCtrlNull4, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수4
            SystemBase.ComboMake.C1Combo(cboCtrlCd5, strSql, 9);      //관리항목5
            SystemBase.ComboMake.C1Combo(cboCtrlNull5, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수5
            SystemBase.ComboMake.C1Combo(cboCtrlCd6, strSql, 9);      //관리항목6
            SystemBase.ComboMake.C1Combo(cboCtrlNull6, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수6
            SystemBase.ComboMake.C1Combo(cboCtrlCd7, strSql, 9);      //관리항목7
            SystemBase.ComboMake.C1Combo(cboCtrlNull7, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수7
            SystemBase.ComboMake.C1Combo(cboCtrlCd8, strSql, 9);      //관리항목8
            SystemBase.ComboMake.C1Combo(cboCtrlNull8, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A102', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //관리항목 필수8

            //////////////////////////// 라디오버튼 SETTING /////////////////////////////////////////////////////////////////////////
            optEntry_Y.Checked = true;
            optUse_Y.Checked = true;
            optDR.Checked = true;

            cboBizAreaCd.SelectedValue = SystemBase.Base.gstrBIZCD;
        }
        #endregion

        #region 트리 선택
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                string strNode = e.Node.Tag.ToString();
                strACCT_CD = strNode.Substring(0, strNode.IndexOf("||"));
                strFIGNO = strNode.Substring(strNode.IndexOf("||") + 2, strNode.Length - strNode.IndexOf("||") - 2);
                strENTRY_YN = e.Node.ImageIndex.ToString();
                if (strACCT_CD != SystemBase.Base.gstrCOMCD)
                {
                    Right_Search(strACCT_CD);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 우측 조회
        private void Right_Search(string ACCT_CD)
        {
            try
            {
                 string strQuery = " usp_ACA002  'S2'";
                 strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
                 strQuery = strQuery + ", @pACCT_CD ='" + ACCT_CD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {
                    txtUpAcctCd.Value = dt.Rows[0]["UP_ACCT_CD"].ToString();
                    txtUpAcctNm.Value = dt.Rows[0]["UP_ACCT_NM"].ToString();

                    txtAcctCd.Value = dt.Rows[0]["ACCT_CD"].ToString();
                    txtAcctNm.Value = dt.Rows[0]["ACCT_NM"].ToString();
                    txtAcctfullNm.Value = dt.Rows[0]["ACCT_FULL_NM"].ToString();
                    if (dt.Rows[0]["DR_CR"].ToString() == "DR")
                    {
                        optDR.Checked = true;
                    }
                    else
                    {
                        optCR.Checked = true;
                    }
                    if (dt.Rows[0]["ENTRY_YN"].ToString() == "Y")
                    {
                        optEntry_Y.Checked = true;
                    }
                    else
                    {
                        optEntry_N.Checked = true;
                    }
                    if (dt.Rows[0]["USE_YN"].ToString() == "Y")
                    {
                        optUse_Y.Checked = true;
                    }
                    else
                    {
                        optUse_N.Checked = true;
                    }
                    cboAcctType.SelectedValue = dt.Rows[0]["ACCT_TYPE"].ToString();
                    cboStatementDiv.SelectedValue = dt.Rows[0]["STATEMENT_DIV"].ToString();
                    cboSubType.SelectedValue = dt.Rows[0]["SUB_TYPE"].ToString();
                    cboBizAreaCd.SelectedValue = dt.Rows[0]["BIZ_AREA_CD"].ToString();
                    cboCtrlCd1.SelectedValue = dt.Rows[0]["CTRL_CD1"].ToString();
                    cboCtrlNull1.SelectedValue = dt.Rows[0]["CTRL_NULL1"].ToString();
                    cboCtrlCd2.SelectedValue = dt.Rows[0]["CTRL_CD2"].ToString();
                    cboCtrlNull2.SelectedValue = dt.Rows[0]["CTRL_NULL2"].ToString();
                    cboCtrlCd3.SelectedValue = dt.Rows[0]["CTRL_CD3"].ToString();
                    cboCtrlNull3.SelectedValue = dt.Rows[0]["CTRL_NULL3"].ToString();
                    cboCtrlCd4.SelectedValue = dt.Rows[0]["CTRL_CD4"].ToString();
                    cboCtrlNull4.SelectedValue = dt.Rows[0]["CTRL_NULL4"].ToString();
                    cboCtrlCd5.SelectedValue = dt.Rows[0]["CTRL_CD5"].ToString();
                    cboCtrlNull5.SelectedValue = dt.Rows[0]["CTRL_NULL5"].ToString();
                    cboCtrlCd6.SelectedValue = dt.Rows[0]["CTRL_CD6"].ToString();
                    cboCtrlNull6.SelectedValue = dt.Rows[0]["CTRL_NULL6"].ToString();
                    cboCtrlCd7.SelectedValue = dt.Rows[0]["CTRL_CD7"].ToString();
                    cboCtrlNull7.SelectedValue = dt.Rows[0]["CTRL_NULL7"].ToString();
                    cboCtrlCd8.SelectedValue = dt.Rows[0]["CTRL_CD8"].ToString();
                    cboCtrlNull8.SelectedValue = dt.Rows[0]["CTRL_NULL8"].ToString();
                    txtRemark.Text = dt.Rows[0]["REMARK"].ToString();
                }
                else
                {
                    //그룹박스 초기화
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                }
                //키값 컨트롤 읽기전용으로 셋팅
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2);

                //컨트롤 체크값 초기화
                SearchData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = null;
                gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref SearchData);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 계정추가 선택
        private void MenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (strENTRY_YN == "2")
                {
                    MessageBox.Show("전표기표여부 'Y'에 하위 계정코드를 등록할 수 없습니다.");
                    return;
                }
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox2);
                cboBizAreaCd.SelectedValue = SystemBase.Base.gstrBIZCD;
                txtUpAcctCd.Value = strACCT_CD;
                txtUpAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", strACCT_CD, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                optEntry_Y.Checked = true;
                optUse_Y.Checked = true;
                optDR.Checked = true;

                //컨트롤 체크값 초기화
                SearchData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = null;
                gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref SearchData);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 트리 마우스 우클릭
        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button.ToString() == "Right")
                {
                    TreeView tv = (TreeView)sender;
                    tv.SelectedNode = tv.GetNodeAt(e.X, e.Y);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

    }
}
