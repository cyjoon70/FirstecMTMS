#region 작성정보
/*********************************************************************/
// 단위업무명 : 정비품BOM조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 정비품BOM조회
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

namespace PA.PBA134
{
    public partial class PBA134 : UIForm.TREE_FPCOMM1
    {
        #region 변수선언
        int NewFlg = 0;
        string NEW_NODE_TAG = "";

        string PRNT_PLANT_CD = "";
        string PRNT_ITEM_CD = "";
        string PRNT_BOM_NO = "";
        string CHILD_ITEM_SEQ = "";
        string CHILD_PLANT_CD = "";
        string CHILD_ITEM_CD = "";
        string CHILD_BOM_NO = "";
        string PROJECT_NO = "";
        string PROJECT_SEQ = "";
        string GROUP_CD = "";
        string MAKEORDER_NO = "";
        string ITEM_NM = "";
        TreeNode node;
        #endregion

        #region PBA134
        public PBA134()
        {
            InitializeComponent();
        }
        #endregion

        #region PBA134_Load
        private void PBA134_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox8);
            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P033', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'C' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ", 0);	// BOM TYPE
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ", 0);	// 공장
            //SystemBase.ComboMake.Combo(cboBOM_TYPE, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '"+ SystemBase.Base.gstrLangCd +"', @pCOM_CD = 'P007'", 0);	// BOM TYPE
            SystemBase.ComboMake.C1Combo(cboBOM_NO, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'C' , @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ");	// BOM TYPE
            SystemBase.ComboMake.C1Combo(cboPROCESS_CD, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD= '" + SystemBase.Base.gstrCOMCD + "' ", 0);	// BOM TYPE

            //기타세팅
            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpUSE_DATE_FR.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpUSE_DATE_TO.Value = "2999-12-31";
            dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
            {
                TreeViewSearch(true);
            }
        }
        public void TreeViewSearch(bool chk)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
            {
                try
                {
                    treeView1.Nodes.Clear();

                    string Query = " exec usp_PBA134 'S1'";

                    Query += ", @pPROJECT_NO = '" + txtPROJECT_NO.Text + "'";
                    Query += ", @pPROJECT_SEQ = '" + txtPROJECT_SEQ.Text + "'";
                    Query += ", @pGROUP_CD = '" + txtSITEM_CD.Text + "'";
                    Query += ", @pMAKEORDER_NO = '" + txtMAKEORDER_NO.Text + "'";

                    Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                    Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                    Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";

                    if (rdoLEVEL1.Checked == true)
                        Query += ", @pLEVEL = '1'";
                    else
                        Query += ", @pLEVEL = '0'";
                    Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataView dvwData = null;
                        UIForm.TreeView.CBOPTreeView(
                             "*"
                             , "*"
                             , 0
                             , (TreeNode)null
                             , treeView1
                             , ds
                             , dvwData
                             , imageList1
                             , false
                             );

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
                        SystemBase.Validation.GroupBox_Reset(gbxITEM_MASTER);
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
        }
        #endregion

        #region 품목코드 조회
        private void btnSITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSITEM_CD.Text, txtSITEM_NM.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, new int[] { 1, 2 }, "품목코드 조회", true);
                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSITEM_CD.Text = Msgs[1].ToString();
                    txtSITEM_NM.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region 트리 클릭 이벤트
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, false);	//락 모두 해제

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

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox8);
                SystemBase.Validation.GroupBox_Reset(gbxITEM_MASTER);

                SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true);	//초기화


                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                treeView1.Nodes.Clear();

                //기타세팅
                dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
                dtpUSE_DATE_FR.Value = SystemBase.Base.ServerTime("YYMMDD");
                dtpUSE_DATE_TO.Value = "2999-12-31";
                dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
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
                    cboPROCESS_CD.SelectedValue = Msgs[5].ToString();	// 품목계정

                    dtpUSE_DATE_TO.Value = Msgs[21].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 프로젝트 조회 팝업
        private void btnPROJECT_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P023', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPROJECT_NO.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00099", strQuery, strWhere, strSearch, "프로젝트 조회", new int[] { 0, 2 }, false);
                pu.Width = 870;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    txtPROJECT_NO.Text = pu.ReturnValue[0].ToString();
                    txtPROJECT_NM.Value = pu.ReturnValue[1].ToString();
                    txtPROJECT_SEQ.Text = pu.ReturnValue[2].ToString();
                    txtSITEM_CD.Text = pu.ReturnValue[3].ToString();
                    txtSITEM_NM.Value = pu.ReturnValue[4].ToString();
                    txtMAKEORDER_NO.Text = pu.ReturnValue[6].ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message);
            }
        }
        #endregion

        #region BOM 설계자 조회 조회
        private void btnDEV_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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


        #region TextChanged 이벤트
        //품목코드
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

        #region 봄 정보 조회
        private void showBomInfo(string NEW_NODE_TAG)
        {
            try
            {
                string NODETAG = NEW_NODE_TAG;

                PRNT_PLANT_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PRNT_ITEM_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PRNT_BOM_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_ITEM_SEQ = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_PLANT_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_ITEM_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                CHILD_BOM_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PROJECT_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                PROJECT_SEQ = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                GROUP_CD = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                ITEM_NM = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                MAKEORDER_NO = NODETAG;

                string Query = " usp_PBA134 @pTYPE = 'S2'";
                Query += " ,@pCHILD_PLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
                Query += " ,@pCHILD_BOM_NO='" + CHILD_BOM_NO.ToString() + "'";
                Query += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";

                Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();
                    txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
                    cboPROCESS_CD.SelectedValue = dt.Rows[0]["ITEM_ACCT"].ToString();
                    txtITEM_SPEC.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                    dtpUSE_DATE_FR.Value = dt.Rows[0]["USE_DATE_FR"].ToString();
                    dtpUSE_DATE_TO.Value = dt.Rows[0]["USE_DATE_TO"].ToString();
                    cboBOM_NO.SelectedValue = dt.Rows[0]["BOM_NO"].ToString();
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
                SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true); // 락

                // 수정 가능한 목록만 수정가능 상태 처리
                if (dt.Rows.Count > 0 && dt.Rows[0]["MASTER_EXISTS_YN"].ToString() == "Y")
                {
                    txtBOM_DEV_USR_ID.ReadOnly = false;
                    txtBOM_DEV_USR_ID.BackColor = Color.White;
                    txtBOM_MFG_USR_ID.ReadOnly = false;
                    txtBOM_MFG_USR_ID.BackColor = Color.White;
                    txtBOM_QUR_USR_ID.ReadOnly = false;
                    txtBOM_QUR_USR_ID.BackColor = Color.White;
                    txtBOM_APP_USR_ID.ReadOnly = false;
                    txtBOM_APP_USR_ID.BackColor = Color.White;

                    txtREV_NO.ReadOnly = false;
                    txtREV_NO.BackColor = Color.White;
                    dtpREV_DT.Enabled = true;

                    btnDEV.Enabled = true;
                    btnMFG.Enabled = true;
                    btnQUR.Enabled = true;
                    btnAPP.Enabled = true;
                }

                string Query2 = " usp_PBA134 @pTYPE = 'S3' ";
                Query2 += ", @pCHILD_PLANT_CD='" + CHILD_PLANT_CD.ToString() + "' ";
                Query2 += ", @pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "' ";
                Query2 += ", @pCHILD_BOM_NO='" + CHILD_BOM_NO.ToString() + "' ";
                Query2 += ", @pVALID_DT='" + dtpSVALID_DT.Text.ToString() + "' ";

                Query2 += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query2 += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query2 += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query2 += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query2 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                NewFlg = 0; // 등록/수정 플래그를 초기화   
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            strQuery += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value = dt.Rows[0]["ITEM_TYPE"].ToString();
        }
        #endregion

       
    }
}
