#region 작성정보
/*********************************************************************/
// 단위업무명 : M BOP TEMP(SCH계획 수정)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-05
// 작성내용 : M BOP TEMP(SCH계획 수정) 및 관리
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
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using WNDW;

namespace PB.PBB103
{ 
    public partial class PBB103 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        int Row = 999;
        string CHILD_ITEM_CD = "";
        string PRNT_PLANT_CD = "";
        string PRNT_ITEM_CD = "";
        string PROC_SEQ = "";

        string PRNT_BOM_NO = "";
        string CHILD_ITEM_SEQ = "";
        string CHILD_PLANT_CD = "";
        string CHILD_BOM_NO = "";
        string PROJECT_NO = "";
        string PROJECT_SEQ = "";
        string GROUP_CD = "";
        string MAKEORDER_NO = "";
        string ITEM_NM = "";

        string NEW_NODE_TAG = "";
        string WORKORDER_NO_OG = "";

        int NewFlg = 0;
        #endregion

        #region 생성자
        public PBB103()
        {
            InitializeComponent();
        }
        #endregion

        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            TreeViewSearch();
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
                string Query = " exec usp_PBB103 'S1'";
                Query += ", @pPROJECT_NO = '" + txtPROJECT_NO.Text + "'";
                Query += ", @pPROJECT_SEQ = '" + txtPROJECT_SEQ.Text + "'";
                Query += ", @pGROUP_CD = '" + txtSITEM_CD.Text + "'";
                Query += ", @pMAKEORDER_NO = '" + txtMAKEORDER_NO.Text + "'";
                Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                Query += ", @pPRNT_BOM_NO = '1'";

                if (rdoLEVEL1.Checked == true)
                    Query += ", @pLEVEL = '1'";
                else
                    Query += ", @pLEVEL = '0'";

                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataView dvwData = null;
                    UIForm.TreeView.MBOPTreeView(ds.Tables[0].Rows[0]["PRNT_ITEM_CD"].ToString()
                        , 0
                        , (TreeNode)null
                        , treeView1
                        , ds
                        , dvwData
                        , imageList2
                        , "W"); 

                    treeView1.Focus();
                    treeView1.ExpandAll();
                }
                else
                {
                    MessageBox.Show("조회된 결과가 없습니다.");
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region PBB103_Load
        private void PBB103_Load(object sender, System.EventArgs e)
        {

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = "F#T|무상#유상";
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P002', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "자원")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P050', @pCOM_CD = '', @pCOM_NM = '', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "생산단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")] = "Y#N|사내#외주";

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")] = "Y#N|Y#N";
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")] = "Y#N|Y#N";
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "시간단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z014', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            // 외주 정보
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B040', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            dtpFROM_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpTO_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);


            SystemBase.Validation.GroupBox_Setting(groupBox8);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
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
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, new int[] { 1, 2 }, "품목코드 조회", true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSITEM_CD.Value = Msgs[1].ToString();
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
        private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {

            NEW_NODE_TAG = e.Node.Tag.ToString();
            // 라우팅정보 화면 출력
            ShowRoutInfo();
        }
        #endregion

        #region txtSITEM_CD KEY 이벤트
        private void txtSITEM_CD_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                txtSITEM_NM.Value = "";
        }
        #endregion

        #region 자품목투입정보 조회
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Enabled == true)
            {
                if (Row != e.Row)
                {
                    Row = e.Row;
                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                        fpSpread2Search(e.Row);
                }
            }
        }

        public void fpSpread2Search(int R)
        {
            try
            {
                string Query = " usp_PBB103 @pTYPE = 'S4'";
                Query += " ,@pPRNT_PLANT_CD='" + cboSPLANT_CD.SelectedValue + "'";
                Query += " ,@pPRNT_ITEM_CD='" + fpSpread2.Sheets[0].Cells[R, 0].Text + "'";
                Query += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query += " ,@pPROC_SEQ='" + fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text + "'";
                Query += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";

                Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pBOM_NO = '" + CHILD_BOM_NO.ToString() + "'";
                Query += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

                PRNT_PLANT_CD = cboSPLANT_CD.SelectedValue.ToString();	// 라우팅 콤보
                PROC_SEQ = fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBoxControlsLock(groupBox8, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, false);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 2, false, false);

            dtpFROM_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpTO_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            string strItemCd = txtITEM_CD.Text;
            string strItemNm = txtITEM_NM.Text;

            if (strItemCd != "")
            {
                try
                {
                    string ERRCode = "ER", MSGCode = "B0015";
                    string strCount = "0", strMsg = "";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    string strCountSql = "usp_PBB103 'M1'";
                    strCountSql += ", @pPROJECT_NO ='" + PROJECT_NO.ToString() + "'";
                    strCountSql += ", @pPROJECT_SEQ ='" + PROJECT_SEQ.ToString() + "'";
                    strCountSql += ", @pGROUP_CD ='" + GROUP_CD.ToString() + "'";
                    strCountSql += ", @pMAKEORDER_NO ='" + MAKEORDER_NO.ToString() + "'";
                    strCountSql += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                    strCountSql += ", @pITEM_CD ='" + strItemCd + "'";
                    strCountSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet dsCount = SystemBase.DbOpen.NoTranDataSet(strCountSql);
                    ERRCode = dsCount.Tables[0].Rows[0][0].ToString();
                    strCount = dsCount.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "OK")
                    {
                        strMsg = SystemBase.Base.MessageRtn("P0042", strItemCd + "||" + strItemNm + "||" + strCount);
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn(strMsg), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (dsMsg == DialogResult.Yes)
                        {
                            try
                            {
                                string strSql = "usp_PBB103 'D1'";
                                strSql += ", @pPROJECT_NO ='" + PROJECT_NO.ToString() + "'";
                                strSql += ", @pPROJECT_SEQ ='" + PROJECT_SEQ.ToString() + "'";
                                strSql += ", @pGROUP_CD ='" + GROUP_CD.ToString() + "'";
                                strSql += ", @pMAKEORDER_NO ='" + MAKEORDER_NO.ToString() + "'";
                                strSql += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                                strSql += ", @pITEM_CD ='" + strItemCd + "'";
                                strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                Trans.Commit();

                                SystemBase.Validation.GroupBoxControlsLock(groupBox8, false);
                                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 2, false, false);

                                SearchExec();
                            }
                            catch (Exception e)
                            {
                                Trans.Rollback();
                                SystemBase.Loggers.Log(this.Name, e.ToString());
                                SystemBase.MessageBoxComm.Show(e.ToString());
                            }
                        Exit:
                            dbConn.Close();
                            SystemBase.MessageBoxComm.Show(MSGCode);
                        }
                    }
                    else
                    {
                        strMsg = strCount;
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn(strMsg), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    SystemBase.MessageBoxComm.Show(f.ToString());
                }
            }
        }
        #endregion

        #region btnPROJECT_Click
        private void btnPROJECT_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P024', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPROJECT_NO.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00099", strQuery, strWhere, strSearch, "프로젝트 조회", new int[] { 0, 1 }, true);
                pu.Width = 870;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    txtPROJECT_NO.Value = pu.ReturnValue[0].ToString();
                    txtPROJECT_NM.Value = pu.ReturnValue[1].ToString();
                    txtPROJECT_SEQ.Value = pu.ReturnValue[2].ToString();
                    txtSITEM_CD.Value = pu.ReturnValue[3].ToString();
                    txtSITEM_NM.Value = pu.ReturnValue[4].ToString();
                    txtMAKEORDER_NO.Value = pu.ReturnValue[6].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region 라우팅정보 화면 출력
        private void ShowRoutInfo()
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

                MAKEORDER_NO = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                WORKORDER_NO_OG = NODETAG;

                /////////////////라우팅 정보////////////////////
                string Query = " usp_PBB103 @pTYPE = 'S5' ";
                Query += ", @pPROJECT_NO='" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ='" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD='" + GROUP_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO ='" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                Query += ", @pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
                Query += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += ", @pMAJOR_FLG = '" + getMajorFlg() + "'";           // 주라우팅 여부
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtITEM_CD.Value = dt.Rows[0][0].ToString();
                    txtITEM_NM.Value = dt.Rows[0][1].ToString();
                    txtROUTING_NO.Value = dt.Rows[0][2].ToString();
                    txtROUTING_NM.Value = dt.Rows[0][3].ToString();
                    dtpFROM_VALID_DT.Value = dt.Rows[0][4].ToString();
                    dtpTO_VALID_DT.Value = dt.Rows[0][5].ToString();

                    if (dt.Rows[0]["MAJOR_FLG"].ToString() == "Y")
                        rdoROUTINT1.Checked = true;
                    else
                        rdoROUTINT2.Checked = true;

                    SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
                    NewFlg = 0;
                }
                else
                {
                    // 제품이나 반제품일 경우 라우팅 등록 활성화
                    Query = " usp_PBB103 @pTYPE = 'S6' ";
                    Query += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                    Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    // 제품,반제품의 경우 ROUTING 정보가 없을 경우 등록 가능하도록 설정
                    if (dt.Rows.Count > 0 &&
                        (dt.Rows[0]["ITEM_ACCT"].ToString() == "10" ||
                         dt.Rows[0]["ITEM_ACCT"].ToString() == "20"))
                    {
                        NewExec();
                    }
                    else
                    {
                        // 화면 초기화 및 LOCK
                        txtITEM_CD.Value = "";
                        txtITEM_NM.Value = "";
                        txtROUTING_NO.Value = "";
                        txtROUTING_NM.Value = "";

                        SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
                    }

                    return;
                }

                /////////////////라우팅 정보////////////////////
                Query = " usp_PBB103 @pTYPE = 'S2'";
                Query += ", @pPLANT_CD = '" + CHILD_PLANT_CD + "'";
                Query += ", @pITEM_CD = '" + CHILD_ITEM_CD + "'";
                Query += ", @pMAJOR_FLG = '" + getMajorFlg() + "'";       // 주라우팅 여부
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                Query += ", @pPROJECT_NO = '" + PROJECT_NO + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO + "'";
                Query += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG + "'";
                Query += ", @pBOM_NO   = '" + CHILD_BOM_NO + "'";
                Query += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 2, false, false);
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                    fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                    fpSpread2Search(0);

                    // 외주 관련 활성화
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value != null &&
                            fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value.ToString() == "N")  // 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, i,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                );
                        }
                    }
                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region 주 OR 창정비 라우팅 여부
        private String getMajorFlg()
        {
            if (CHILD_BOM_NO == "C") // 교환일경우
            {
                return "Y"; // 주라우팅 반환
            }
            return "N";     // 보조라우팅 반환
        }
        #endregion

        
    }
}
