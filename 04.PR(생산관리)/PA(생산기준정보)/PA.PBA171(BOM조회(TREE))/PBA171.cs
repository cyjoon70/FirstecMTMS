#region 작성정보
/*********************************************************************/
// 단위업무명 : BOM조회(TREE)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-15
// 작성내용 : BOM조회(TREE) 및 관리
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

namespace PA.PBA171
{
    public partial class PBA171 : UIForm.FPCOMM1
    {
        #region 변수선언
        string NEW_NODE_TAG = "";
        #endregion

        #region 생성자
        public PBA171()
        {
            InitializeComponent();
        }
        #endregion

        #region PBA171_Load
        private void PBA171_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox8);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P033', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "등록위치")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P066', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "EBOM 단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "DL 등록여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpUSE_DATE_FR.Value = "2009-04-30";
            dtpUSE_DATE_TO.Value = "2999-12-31";
            dtpREV_DT.Value = "2009-04-30";

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            SystemBase.ComboMake.C1Combo(cboBOM_TYPE, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'S' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// BOM TYPE
            SystemBase.ComboMake.C1Combo(cboPRNT_BOM_NO, "usp_B_COMMON @pTYPE = 'REL1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'P006', @pSPEC2 = 'S', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// BOM TYPE

            SystemBase.ComboMake.C1Combo(cboPROCESS_CD, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// BOM TYPE
            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true);//그룹박스 전체 락
            SystemBase.Validation.GroupBox_Setting(groupBox8);
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

                string Query = " exec usp_PBA171 'S1'";
                Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                Query += ", @pPRNT_BOM_NO = '" + cboBOM_TYPE.SelectedValue + "'";
                if (rdoLEVEL1.Checked == true)
                    Query += ", @pLEVEL = '1'";
                else
                    Query += ", @pLEVEL = '0'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
        #endregion
                
        #region 품목코드 조회
        private void btnSITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, false);	//락 모두 해제
            // 새로운 노드
            NEW_NODE_TAG = e.Node.Tag.ToString();
            // BOM 정보 조회
            showBomInfo();
        }
        #endregion

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            try
            {
                SystemBase.Validation.GroupBox_Reset(gbxITEM_MASTER);
                SystemBase.Validation.GroupBox_Reset(groupBox8);

                rdoLEVEL1.Checked = true;

                dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
                dtpUSE_DATE_FR.Value = "2009-04-30";
                dtpUSE_DATE_TO.Value = "2999-12-31";
                dtpREV_DT.Value = "2009-04-30";

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                fpSpread1.Enabled = true;
                treeView1.Nodes.Clear();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "초기화"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                string Query = " usp_PBA171 @pTYPE = 'S2'";
                Query += " ,@pPRNT_PLANT_CD='" + PRNT_PLANT_CD.ToString() + "'";
                Query += " ,@pPRNT_ITEM_CD='" + PRNT_ITEM_CD.ToString() + "'";
                Query += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

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
                    txtPLMRevision.Value = dt.Rows[0]["PLM_REV_NO"].ToString();      // 2018.01.02. hma 추가: PLM리비전번호
                    dtpREV_DT.Value = dt.Rows[0]["REV_DT"].ToString();

                    txtREMARK.Value = dt.Rows[0]["REMARK"].ToString();
                    dtxtMatrUrwg.Value = dt.Rows[0]["MATR_URWG"];

                }
                SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true); // 락

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
                    //fpSpread1.Enabled = true;
                    //UIForm.Buttons.ReButton(BtnRCopy, "BtnRCopy", true);
                    //UIForm.Buttons.ReButton(BtnRowIns, "BtnRowIns", true);
                    //UIForm.Buttons.ReButton(BtnCancel, "BtnCancel", true);
                    //UIForm.Buttons.ReButton(BtnDel, "BtnDel", true);
                    //UIForm.Buttons.ReButton(BtnDelete, "BtnDelete", true);
                    //UIForm.Buttons.ReButton(BtnInsert, "BtnInsert", true);

                    string Query2 = " usp_PBA171 @pTYPE = 'S3' ";
                    Query2 += " ,@pCHILD_PLANT_CD='" + CHILD_PLANT_CD.ToString() + "' ";
                    Query2 += " ,@pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "' ";
                    Query2 += " ,@pCHILD_BOM_NO='" + CHILD_BOM_NO.ToString() + "' ";
                    Query2 += " ,@pVALID_DT='" + dtpSVALID_DT.Text.ToString() + "' ";
                    Query2 += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(Query);
                    UIForm.FPMake.grdCommSheet(fpSpread1, Query2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                }
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
            strQuery += " usp_P_COMMON 'P171' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
            strQuery += " , @pCOM_CD='" + txtITEM_CD.Text + "'";
            strQuery += " , @pPLANT_CD='" + cboSPLANT_CD.SelectedValue + "'";


            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")].Value = dt.Rows[0]["ITEM_TYPE"].ToString();
        }
        #endregion

        #region txtSITEM_CD_TextChanged
        private void txtSITEM_CD_TextChanged(object sender, System.EventArgs e)
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

        #endregion

    }
}
