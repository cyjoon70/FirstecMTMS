#region 작성정보
/*********************************************************************/
// 단위업무명 : BOM출고방법변경
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-15
// 작성내용 : BOM출고방법변경 및 관리
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
using WNDW;
using System.Text.RegularExpressions;

namespace PA.PBA174
{
    public partial class PBA174 : UIForm.FPCOMM1
    {
        #region 변수선언
        string NEW_NODE_TAG = "";
        string ASSIGN_NO = "";
        #endregion

        #region 생성자
        public PBA174()
        {
            InitializeComponent();
        }

        public PBA174(string Assign_NO)
        {
            // 알리미 클릭시- 결제
            ASSIGN_NO = Assign_NO;
            InitializeComponent();
        }
        #endregion

        #region PBA174_Load
        private void PBA174_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            SystemBase.ComboMake.C1Combo(cboBOM_TYPE, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'S' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// BOM TYPE
            SystemBase.ComboMake.C1Combo(cboPRNT_BOM_NO, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'S' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// BOM TYPE

            SystemBase.ComboMake.C1Combo(cboPROCESS_CD, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// BOM TYPE
            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true);//그룹박스 전체 락
            SystemBase.Validation.GroupBox_Setting(groupBox8);

            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpUSE_DATE_FR.Value = "2009-04-30";
            dtpUSE_DATE_TO.Value = "2999-12-31";
            dtpREV_DT.Value = "2009-04-30";

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
        }
        #endregion
        
        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox8);
                SystemBase.Validation.GroupBox_Reset(gbxITEM_MASTER);

                //그리드 초기화
                fpSpread1.Sheets[0].Rows.Count = 0;

                treeView1.Nodes.Clear();

                dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
                dtpUSE_DATE_FR.Value = "2009-04-30";
                dtpUSE_DATE_TO.Value = "2999-12-31";
                dtpREV_DT.Value = "2009-04-30";
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "초기화"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
                
        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
            {
                TreeViewSearch();
            }
        }
        private void txtSSCH_CD_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                TreeViewSearch();
            }
        }

        public void TreeViewSearch()
        {
            try
            {
                treeView1.Nodes.Clear();

                string Query = " exec usp_PBA174 'S1'";
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
                        , imageList2
                        , 0);

                    treeView1.Focus();
                    treeView1.ExpandAll();
                }
                else
                {
                    SystemBase.Base.GroupBoxReset(gbxITEM_MASTER);
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
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSITEM_CD.Text, txtSITEM_NM.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    txtSITEM_CD.Value = pu.ReturnValue[1].ToString();
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
        private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            // 새로운 노드
            NEW_NODE_TAG = e.Node.Tag.ToString();
            // BOM 정보 조회
            showBomInfo();
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 상단 필수항목 체크
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR";
                string MSGCode = "P0000";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead.Length > 0)
                        {
                            string strSql = "";
                            strSql = " usp_PBA174 'U1'";

                            strSql += ", @pCHILD_PLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                            strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                            strSql += ", @pISSUED_MTHD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text.ToUpper() + "'";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

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
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }

            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSITEM_CD.Text = txtITEM_CD.Text;
                    showBomInfo();
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

        #region 싱글 품목마스타 조회
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                //string strItemType = "03"; //제품
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Value = Msgs[2].ToString();		// 자품목코드
                    txtITEM_NM.Value = Msgs[3].ToString();		// 자품목명
                    txtITEM_SPEC.Value = Msgs[7].ToString();		// 규격
                    cboPROCESS_CD.SelectedValue = Msgs[5].ToString();	// 품목계정

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

        #region 봄 정보 조회
        private void showBomInfo()
        {
            try
            {
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

                string Query = " usp_PBA174 @pTYPE = 'S2'";
                Query += " ,@pPRNT_PLANT_CD='" + PRNT_PLANT_CD.ToString() + "'";
                Query += " ,@pPRNT_ITEM_CD='" + PRNT_ITEM_CD.ToString() + "'";
                Query += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtITEM_CD.Value = dt.Rows[0]["CHILD_ITEM_CD"].ToString();
                    txtITEM_NM.Value = ITEM_NM.ToString();
                    cboPROCESS_CD.SelectedValue = dt.Rows[0]["ITEM_ACCT"];
                    txtITEM_SPEC.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                    dtpUSE_DATE_FR.Value = dt.Rows[0]["USE_DATE_FR"].ToString();
                    dtpUSE_DATE_TO.Value = dt.Rows[0]["USE_DATE_TO"].ToString();
                    cboPRNT_BOM_NO.SelectedValue = dt.Rows[0]["PRNT_BOM_NO"];
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
                    ASSIGN_NO = dt.Rows[0]["ASSIGN_NO"].ToString();

                }

                if (dt.Rows[0]["ITEM_ACCT"].ToString() == "30")
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                    fpSpread1.Enabled = false;
                }
                else
                {
                    string Query2 = " usp_PBA174 @pTYPE = 'S3' ";
                    Query2 += " ,@pCHILD_PLANT_CD='" + CHILD_PLANT_CD.ToString() + "' ";
                    Query2 += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "' ";
                    Query2 += " ,@pCHILD_BOM_NO='" + CHILD_BOM_NO.ToString() + "' ";
                    Query2 += " ,@pVALID_DT='" + dtpSVALID_DT.Text.ToString() + "' ";
                    Query2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(Query);
                    UIForm.FPMake.grdCommSheet(fpSpread1, Query2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
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
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법"))
            {
                try
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법명")].Text
                        = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text, " AND MAJOR_CD = 'B030' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법명")].Text == "")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text = "M";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법명")].Text = "수동";
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고방법조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region txtSITEM_CD_TextChanged(조회)
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
        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'B030' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "출고방법 조회");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고방법 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
