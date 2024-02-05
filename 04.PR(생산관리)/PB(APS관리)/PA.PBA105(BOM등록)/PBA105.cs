#region 작성정보
/*********************************************************************/
// 단위업무명 : BOM등록
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-03-06
// 작성내용 : BOM 등록 및 조회
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
using SystemBase;

namespace PA.PBA105
{
    public partial class PBA105 : UIForm.TREE_FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        string NEW_NODE_TAG = "";
        TreeNode node;
        #endregion

        #region PBA105
        public PBA105()
        {
            InitializeComponent();
        }
        #endregion

        #region PBA105_Load
        private void PBA105_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P033', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            SystemBase.ComboMake.C1Combo(cboBOM_TYPE, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'S' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");	// BOM TYPE
            SystemBase.ComboMake.C1Combo(cboPRNT_BOM_NO, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'S' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");	// BOM TYPE

            SystemBase.ComboMake.C1Combo(cboITEM_ACCT, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);	// BOM TYPE

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);//그룹박스 전체 락

            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpUSE_DATE_FR.Value = "2001-01-01";
            dtpUSE_DATE_TO.Value = "2999-12-31";
        }
        #endregion

        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                TreeViewSearch(true);
            }
        }
        public void TreeViewSearch(bool chk)
        {
            try
            {
                treeView1.Nodes.Clear();

                string Query = " exec usp_PBA105 'S1'";
                Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                Query += ", @pPRNT_BOM_NO = '" + cboBOM_TYPE.SelectedValue + "'";
                if (rdoLEVEL1.Checked == true)
                    Query += ", @pLEVEL = '1'";
                else
                    Query += ", @pLEVEL = '0'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataView dvwData = null;
                    UIForm.TreeView.CommonTreeView(ds.Tables[0].Rows[0]["PRNT_ITEM_CD"].ToString()
                        , ds.Tables[0].Rows[0]["FIGNO"].ToString()
                        , (TreeNode)null
                        , treeView1
                        , ds
                        , dvwData
                        , imageList1
                        , 0);

                    treeView1.Focus();
                    treeView1.ExpandAll();

                    //AutoScroll 기능 : Scroll을 제일 위로 올려준다.
                    if (chk == true)
                    {
                        treeView1.Nodes[treeView1.Nodes.Count - 1].EnsureVisible();

                        treeView1.SelectedNode = this.treeView1.TopNode;
                    }
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox2);
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "TreeView 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 품목코드 조회
        private void btnSITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSITEM_CD.Text, txtSITEM_NM.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    txtSITEM_CD.Text = pu.ReturnValue[1].ToString();
                    txtSITEM_NM.Value = pu.ReturnValue[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 트리 클릭 이벤트
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);	//락 모두 해제

            node = e.Node;

            // 새로운 노드
            NEW_NODE_TAG = e.Node.Tag.ToString();
            // BOM 정보 조회
            showBomInfo(NEW_NODE_TAG);

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 상단 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    if (txtITEM_CD.Text != Msgs[2].ToString())
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Value = Msgs[5].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = Msgs[8].ToString();
                        setITEM_TYPE(Row); // 조달구분설정
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("P0049", "품목코드 입력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text = "";
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region MASTER 삭제
        protected override void DeleteExec()
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    if (MessageBox.Show(SystemBase.Base.MessageRtn("P0048"), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string strSql = " usp_PBA105 'D2'";
                        strSql += ", @pPRNT_PLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                        strSql += ", @pPRNT_ITEM_CD = '" + txtITEM_CD.Text + "'";
                        strSql += ", @pPRNT_BOM_NO = '" + cboPRNT_BOM_NO.SelectedValue + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
                        MessageBox.Show(dt.Rows[0][1].ToString());
                    }
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0062", "전체 삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전체 삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 행추가
        protected override void RowInsExec()
        {// 행 추가
            try
            {
                if (txtITEM_CD.Text == "")
                {
                    //NewExec();
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0043", "행추가"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {

                    // MASTER 처리 (자품목 정보가 없을 경우 추가)
                    if (fpSpread1.Sheets[0].Rows.Count == 0)
                    {
                        // MASTER LOCK해제
                        mstEditUnlock();
                    }

                    UIForm.FPMake.RowInsert(fpSpread1);
                    int RowNum = fpSpread1.ActiveSheet.ActiveRowIndex;
                    int Seq = 0;

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {	// Max 순서 추출
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text != "" && Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text) > Seq)
                            Seq = Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text);
                    }
                    Seq = Seq + 1;

                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text = Seq.ToString();
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")].Value = Convert.ToString(cboPRNT_BOM_NO.SelectedValue);

                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목 기준수")].Text = "1";
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = "EA";
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목 기준수")].Text = "1";
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")].Value = "EA";
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "안전 L/T")].Text = "0";
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "LOSS 율")].Text = "0";
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")].Value = "F";

                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text = DateTime.Today.Date.ToString();
                    fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Text = "2999-12-31";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                gBox = new GroupBox[] { groupBox2};
                SystemBase.Validation.Control_Check(gBox, ref SaveData);

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if (SearchData == SaveData && UIForm.FPMake.HasSaveData(fpSpread1) == false)
                {
                    //변경되거나 처리할 데이터가 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                string ERRCode = "WR";
                string MSGCode = "P0000";
                string strChildItemCd = "";
                string strChildSeq = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = "usp_PBA105 'I2'";
                    strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                    strSql += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                    strSql += ", @pPRNT_BOM_NO = '" + cboPRNT_BOM_NO.SelectedValue + "'";
                    strSql += ", @pBOM_DEV_USER_ID = '" + txtBOM_DEV_USR_ID.Text + "'";
                    strSql += ", @pBOM_MFG_USER_ID = '" + txtBOM_MFG_USR_ID.Text + "'";
                    strSql += ", @pBOM_QUR_USER_ID = '" + txtBOM_QUR_USR_ID.Text + "'";
                    strSql += ", @pBOM_APP_USER_ID = '" + txtBOM_APP_USR_ID.Text + "'";
                    strSql += ", @pREV_NO = '" + txtREV_NO.Text + "'";
                    strSql += ", @pREV_DT = '" + dtpREV_DT.Text + "'";

                    strSql += ", @pVALID_FROM_DT = '" + dtpUSE_DATE_FR.Text + "'";
                    strSql += ", @pVALID_TO_DT = '" + dtpUSE_DATE_TO.Text + "'";
                    strSql += ", @pDRAWING_PATH = '" + txtDRAWING_PATH.Text + "'";
                    strSql += ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                    ERRCode = dt.Rows[0][0].ToString();
                    MSGCode = dt.Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)// 그리드 상단 필수항목 체크
                        {
                            for (int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
                            {
                                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[j, 0].Text;
                                
                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "D": strGbn = "D1"; break;
                                        case "U": strGbn = "U1"; break;
                                        case "I": strGbn = "I1"; break;
                                        default: strGbn = ""; break;
                                    }

                                    strChildItemCd = txtITEM_CD.Text;
                                    strChildSeq = fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text;

                                    strSql = " usp_PBA105 '" + strGbn + "'";
                                    strSql += ", @pPRNT_PLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                    strSql += ", @pPRNT_ITEM_CD = '" + txtITEM_CD.Text + "'";
                                    strSql += ", @pPRNT_BOM_NO = '" + cboPRNT_BOM_NO.SelectedValue + "'";
                                    strSql += ", @pCHILD_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "'";
                                    strSql += ", @pCHILD_SEQ = '" + strChildSeq + "'";
                                    strSql += ", @pCHILD_PLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                    strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                                    strSql += ", @pCHILD_BOM_NO = '" + cboPRNT_BOM_NO.SelectedValue + "'";
                                    strSql += ", @pPRNT_ITEM_QTY = " + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목 기준수")].Value + "";
                                    strSql += ", @pPRNT_ITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")].Text + "'";
                                    strSql += ", @pCHILD_ITEM_QTY = " + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목 기준수")].Value + "";
                                    strSql += ", @pCHILD_ITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                                    strSql += ", @pMAT_SIZE = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "재료규격")].Text + "'";
                                    strSql += ", @pLOSS_RATE = " + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "LOSS 율")].Value + "";
                                    strSql += ", @pSAFETY_LT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "안전 L/T")].Text + "'";
                                    strSql += ", @pSUPPLY_TYPE = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")].Value + "'";
                                    strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                                    strSql += ", @pVALID_FROM_DT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text + "'";
                                    strSql += ", @pVALID_TO_DT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Text + "'";
                                    strSql += ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";

                                    strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                    strSql += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                            }
                        }
                        Trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }

            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    //컨트롤 체크값 초기화
                    SearchData = "";
                    //컨트롤 체크 함수
                    gBox = new GroupBox[] { groupBox2};
                    SystemBase.Validation.Control_Check(gBox, ref SearchData);

                    //TreeViewSearch(false);
                    //AutoScroll 기능 : Scroll을 제일 위로 올려준다.

                    // BOM 정보 조회
                    showBomInfo(NEW_NODE_TAG);
                  
                    //그리드 셀 포커스 이동
                    UIForm.FPMake.GridSetFocus(fpSpread1, strChildSeq, SystemBase.Base.GridHeadIndex(GHIdx1, "순서"));

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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox2);

                dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");

                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);	//초기화

                dtpUSE_DATE_FR.Value = "2001-01-01";
                dtpUSE_DATE_TO.Value = "2999-12-31";

                //버튼 재정의
                UIForm.Buttons.ReButton("111111110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                treeView1.Nodes.Clear();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "초기화"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 싱글 품목마스타 조회
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                //string strItemType = "03"; //제품
                WNDW.WNDW005 pu = new WNDW.WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[2].ToString();		// 자품목코드
                    txtITEM_NM.Value = Msgs[3].ToString();		// 자품목명
                    txtITEM_SPEC.Value = Msgs[7].ToString();		// 규격
                    cboITEM_ACCT.SelectedValue = Msgs[5].ToString();	// 품목계정

                    dtpUSE_DATE_FR.Value = Msgs[21].ToString();
                    dtpUSE_DATE_TO.Value = Msgs[22].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region MASTER EDIT LOCK해지
        private void mstEditUnlock()
        {
            // 수정가능항목 ROCK 풀기
            txtBOM_DEV_USR_ID.BackColor = SystemBase.Validation.Kind_White;
            txtBOM_DEV_USR_ID.ReadOnly = false;
            btnDEV.Enabled = true;

            txtBOM_MFG_USR_ID.BackColor = SystemBase.Validation.Kind_White;
            txtBOM_MFG_USR_ID.ReadOnly = false;
            btnMFG.Enabled = true;

            txtBOM_QUR_USR_ID.BackColor = SystemBase.Validation.Kind_White;
            txtBOM_QUR_USR_ID.ReadOnly = false;
            btnQUR.Enabled = true;

            txtBOM_APP_USR_ID.BackColor = SystemBase.Validation.Kind_White;
            txtBOM_APP_USR_ID.ReadOnly = false;
            btnAPP.Enabled = true;

            txtREV_NO.BackColor = SystemBase.Validation.Kind_White;
            txtREV_NO.ReadOnly = false;
            dtpREV_DT.Enabled = true;

        }
        #endregion

        #region BOM 설계자 조회 조회
        private void btnDEV_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtBOM_DEV_USR_ID.Text, txtBOM_DEV_USR_NM.Text, "BD" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "BOM설계자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBOM_DEV_USR_ID.Value = Msgs[0].ToString();
                    txtBOM_DEV_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM설계자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 생산검토자 조회 조회
        private void btnMFG_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"; 
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtBOM_MFG_USR_ID.Text, txtBOM_MFG_USR_NM.Text, "BM" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "생산검토자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBOM_MFG_USR_ID.Value = Msgs[0].ToString();
                    txtBOM_MFG_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 품질검토자 조회 조회
        private void btnQUR_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtBOM_QUR_USR_ID.Text, txtBOM_QUR_USR_NM.Text, "BQ" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품질검토자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBOM_QUR_USR_ID.Value = Msgs[0].ToString();
                    txtBOM_QUR_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region BOM확인자 조회 조회
        private void btnAPP_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtBOM_APP_USR_ID.Text, txtBOM_APP_USR_NM.Text, "BA" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "BOM확인자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBOM_APP_USR_ID.Value = Msgs[0].ToString();
                    txtBOM_APP_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM확인자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 봄 정보 조회
        private void showBomInfo(string NEW_NODE_TAG)
        {
            try
            {
                //컨트롤 체크값 초기화
                SearchData = "";

                string NODETAG = NEW_NODE_TAG;

                string PRNT_PLANT_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                string PRNT_ITEM_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                string PRNT_BOM_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                string CHILD_ITEM_SEQ = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                string CHILD_PLANT_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                string CHILD_ITEM_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                string CHILD_BOM_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                string ITEM_NM = NODETAG;

                string Query = " usp_PBA105 @pTYPE = 'S2'";
                Query += " ,@pPRNT_PLANT_CD='" + PRNT_PLANT_CD.ToString() + "'";
                Query += " ,@pPRNT_ITEM_CD='" + PRNT_ITEM_CD.ToString() + "'";
                Query += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtITEM_CD.Value = dt.Rows[0]["CHILD_ITEM_CD"].ToString();
                    txtITEM_NM.Value = ITEM_NM.ToString();
                    cboITEM_ACCT.SelectedValue = dt.Rows[0]["ITEM_ACCT"].ToString();
                    txtITEM_SPEC.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                    dtpUSE_DATE_FR.Value = dt.Rows[0]["USE_DATE_FR"].ToString();
                    dtpUSE_DATE_TO.Value = dt.Rows[0]["USE_DATE_TO"].ToString();
                    cboPRNT_BOM_NO.SelectedValue = dt.Rows[0]["PRNT_BOM_NO"].ToString();
                    txtDRAWING_PATH.Value = dt.Rows[0]["DRAWING_PATH"].ToString();

                    txtBOM_DEV_USR_ID.Value = dt.Rows[0]["BOM_DEV_USER_ID"].ToString();
                    txtBOM_MFG_USR_ID.Value = dt.Rows[0]["BOM_MFG_USER_ID"].ToString();
                    txtBOM_QUR_USR_ID.Value = dt.Rows[0]["BOM_QUR_USER_ID"].ToString();
                    txtBOM_APP_USR_ID.Value = dt.Rows[0]["BOM_APP_USER_ID"].ToString();

                    txtBOM_DEV_USR_NM.Value = dt.Rows[0]["BOM_DEV_USER_NM"].ToString();
                    txtBOM_MFG_USR_NM.Value = dt.Rows[0]["BOM_MFG_USER_NM"].ToString();
                    txtBOM_QUR_USR_NM.Value = dt.Rows[0]["BOM_QUR_USER_NM"].ToString();
                    txtBOM_APP_USR_NM.Value = dt.Rows[0]["BOM_APP_USER_NM"].ToString();

                    txtREV_NO.Value = dt.Rows[0]["REV_NO"].ToString();
                    dtpREV_DT.Value = dt.Rows[0]["REV_DT"].ToString();

                    txtREMARK.Value = dt.Rows[0]["REMARK"].ToString();
                }
                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true); // 락
                
                if (dt.Rows.Count > 0 && dt.Rows[0]["MASTER_EXISTS_YN"].ToString() == "Y")  // MASTER에 등록되어 있을 경우
                {
                    // MASTER LOCK해제
                    mstEditUnlock();
                }

                //컨트롤 체크값 초기화
                SearchData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref SearchData);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["ITEM_ACCT"].ToString() == "30")
                    {
                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                        fpSpread1.Enabled = false;

                        UIForm.Buttons.ReButton(BtnRCopy, "BtnRCopy", false);
                        UIForm.Buttons.ReButton(BtnRowIns, "BtnRowIns", false);
                        UIForm.Buttons.ReButton(BtnCancel, "BtnCancel", false);
                        UIForm.Buttons.ReButton(BtnDel, "BtnDel", false);
                        UIForm.Buttons.ReButton(BtnDelete, "BtnDelete", false);
                        UIForm.Buttons.ReButton(BtnInsert, "BtnInsert", false);
                    }
                    else
                    {
                        fpSpread1.Enabled = true;
                        UIForm.Buttons.ReButton(BtnRCopy, "BtnRCopy", true);
                        UIForm.Buttons.ReButton(BtnRowIns, "BtnRowIns", true);
                        UIForm.Buttons.ReButton(BtnCancel, "BtnCancel", true);
                        UIForm.Buttons.ReButton(BtnDel, "BtnDel", true);
                        UIForm.Buttons.ReButton(BtnDelete, "BtnDelete", true);
                        UIForm.Buttons.ReButton(BtnInsert, "BtnInsert", true);

                        string Query2 = " usp_PBA105 @pTYPE = 'S3' ";
                        Query2 += " ,@pCHILD_PLANT_CD='" + CHILD_PLANT_CD.ToString() + "' ";
                        Query2 += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "' ";
                        Query2 += " ,@pCHILD_BOM_NO='" + CHILD_BOM_NO.ToString() + "' ";
                        Query2 += " ,@pVALID_DT='" + dtpSVALID_DT.Text.ToString() + "' ";
                        Query2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                        DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(Query);
                        UIForm.FPMake.grdCommSheet(fpSpread1, Query2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 스프레드 값 변경 처리
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            try
            {
                // 자품목 코드 컬럼
                int childItemCdCol = SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드");

                // 공정정보 붙여넣기 일 경우
                if (fpSpread1.Sheets[0].Cells[Row, childItemCdCol].Text != "")
                {

                    string strQuery = "";
                    strQuery += " usp_P_COMMON 'P170' ";
                    strQuery += " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, childItemCdCol].Text + "'";
                    strQuery += " , @pPLANT_CD= '" + cboSPLANT_CD.SelectedValue + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        if (txtITEM_CD.Text != fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text)
                        {

                            // 자품목정보를 조회한다.
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Value = dt.Rows[0]["ITEM_ACCT"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = dt.Rows[0]["ITEM_UNIT"].ToString();
                            setITEM_TYPE(Row);

                            UIForm.FPMake.fpChange(fpSpread1, Row);
                        }
                        else
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("P0049", "품목코드 입력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text = "";
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자품목조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조달구분 설정(부모품목의 조달구분으로 설정한다)
        private void setITEM_TYPE(int Row)
        {
            string strQuery = "";
            strQuery += " usp_P_COMMON 'P171' ";
            strQuery += " , @pCOM_CD='" + txtITEM_CD.Text + "'";
            strQuery += " , @pPLANT_CD='" + cboSPLANT_CD.SelectedValue + "'";
            strQuery += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value = dt.Rows[0]["ITEM_TYPE"].ToString();
        }
        #endregion

        #region txtITEM_CD_Leave(마스터 정보확인)
        private void txtITEM_CD_Leave(object sender, System.EventArgs e)
        {
            try
            {
                string strCheck = "0";
                strCheck = SystemBase.Base.CodeName("ITEM_CD", "COUNT(1)", "P_BOP_MASTER", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");

                if (strCheck != "0")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0044", "마스터 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtITEM_CD.Value = "";
                    txtITEM_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged 이벤트
        //품목코드(조회)
        private void txtSITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSITEM_CD.Text != "")
                {
                    txtSITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSITEM_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //품목코드(입력)
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목명 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //BOM설계자
        private void txtBOM_DEV_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBOM_DEV_USR_ID.Text != "")
                {
                    txtBOM_DEV_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtBOM_DEV_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBOM_DEV_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM설계자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //생산검토자
        private void txtBOM_MFG_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBOM_MFG_USR_ID.Text != "")
                {
                    txtBOM_MFG_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtBOM_MFG_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBOM_MFG_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //품질검토자
        private void txtBOM_QUR_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBOM_QUR_USR_ID.Text != "")
                {
                    txtBOM_QUR_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtBOM_QUR_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBOM_QUR_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //BOM확인자
        private void txtBOM_APP_USR_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBOM_APP_USR_ID.Text != "")
                {
                    txtBOM_APP_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtBOM_APP_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBOM_APP_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM확인자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
