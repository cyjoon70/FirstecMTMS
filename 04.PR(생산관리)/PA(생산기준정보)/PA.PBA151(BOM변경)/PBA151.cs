#region 작성정보
/*********************************************************************/
// 단위업무명 : BOM변경
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-03-06
// 작성내용 : BOM 변경 및 조회
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
using System.Reflection;

namespace PA.PBA151
{
    public partial class PBA151 : UIForm.TREE_FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string SaveData = "", SearchData = ""; //컨트롤에 대한 조회후 데이터와 저장시 변경된 데이터 체크위한 변수
        string NEW_NODE_TAG = "";
        TreeNode node;
        int NewFlg = 0;
        #endregion

        #region PBA151
        public PBA151()
        {
            InitializeComponent();
        }
        #endregion

        #region PBA151_Load
        private void PBA151_Load(object sender, System.EventArgs e)
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

            cboSPLANT_CD.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();
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

                string Query = " exec usp_PBA151 'S1'";
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
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
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

                string Query = " usp_PBA151 @pTYPE = 'S2'";
                Query += " ,@pPRNT_PLANT_CD='" + PRNT_PLANT_CD.ToString() + "'";
                Query += " ,@pPRNT_ITEM_CD='" + PRNT_ITEM_CD.ToString() + "'";
                Query += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

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

                    dtxtMatrUrwg.Value = dt.Rows[0]["MATR_URWG"];
                }
                SystemBase.Validation.GroupBox_Setting(groupBox2); // GroupBox Setting

                if (dt.Rows[0]["ITEM_ACCT"].ToString() == "30")
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                    fpSpread1.Enabled = false;
                }
                else
                {
                    fpSpread1.Enabled = true;

                    string Query2 = " usp_PBA151 @pTYPE = 'S3' ";
                    Query2 += " ,@pCHILD_PLANT_CD='" + CHILD_PLANT_CD.ToString() + "' ";
                    Query2 += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "' ";
                    Query2 += " ,@pCHILD_BOM_NO='" + CHILD_BOM_NO.ToString() + "' ";
                    Query2 += " ,@pVALID_DT='" + dtpSVALID_DT.Text.ToString() + "' ";
                    Query2 += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(Query);
                    UIForm.FPMake.grdCommSheet(fpSpread1, Query2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }

                NewFlg = 0; // 등록/수정 FLAG를 초기화
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        #region REV버튼 클릭
        private void btnRev_Click(object sender, System.EventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    bool chk = false;

                    string strChild = "";
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                        {
                            if (chk == false)
                                chk = true;

                            if (strChild == "")
                            {
                                strChild = fpSpread1.Sheets[0].Cells[i, 0].Text;//hiiden
                            }
                            else
                            {
                                strChild = strChild + "!!" + fpSpread1.Sheets[0].Cells[i, 0].Text;//hiden
                            }
                        }
                    }

                    string strItem_Cd = txtITEM_CD.Text;				//품목코드
                    string strItem_Nm = txtITEM_NM.Text;				//품목명
                    string strRev_No = txtREV_NO.Text;					//리비젼번호
                    string strBOM_DEV_USR_ID = txtBOM_DEV_USR_ID.Text;	//설계자
                    string strBOM_MFG_USR_ID = txtBOM_MFG_USR_ID.Text;	//생산검토자
                    string strBOM_QUR_USR_ID = txtBOM_QUR_USR_ID.Text;	//품질검토자
                    string strBOM_APP_USR_ID = txtBOM_APP_USR_ID.Text;	//승인자
                    string strPRNT_BOM_NO = cboPRNT_BOM_NO.SelectedValue.ToString(); //BOM NO
                    string strMatrUrwg = dtxtMatrUrwg.Value.ToString();

                    PBA151P1 pu = new PBA151P1(strItem_Cd, strItem_Nm, strRev_No, strPRNT_BOM_NO, strBOM_DEV_USR_ID, strBOM_MFG_USR_ID, strBOM_QUR_USR_ID, strBOM_APP_USR_ID, strChild, strMatrUrwg, chk);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        showBomInfo(NEW_NODE_TAG);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0048"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        #endregion
    }
}
