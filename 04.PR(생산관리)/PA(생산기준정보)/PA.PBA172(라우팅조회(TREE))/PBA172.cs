#region 작성정보
/*********************************************************************/
// 단위업무명 : 라우팅변경조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-15
// 작성내용 : 라우팅변경조회 관리
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

namespace PA.PBA172
{
    public partial class PBA172 : UIForm.FPCOMM2
    {
        #region 변수선언
        string CHILD_ITEM_CD = "";
        string PRNT_PLANT_CD = "";
        string PRNT_ITEM_CD = "";
        string ROUT_NO = "";
        string PROC_SEQ = "";
        string PRNT_BOM_NO = "";
        string CHILD_ITEM_SEQ = "";
        string CHILD_PLANT_CD = "";
        string CHILD_BOM_NO = "";
        string ITEM_NM = "";
        string PHANTOM_FLAG = "";

        string NEW_NODE_TAG = "";

        int NewFlg = 0; // 등록 수정 FLAG
        #endregion

        #region 생성자
        public PBA172()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void PBA119_Load(object sender, System.EventArgs e)
        {
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P033', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P002', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P028', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "시간단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z014', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B040', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpFROM_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpTO_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            SystemBase.Validation.GroupBox_Setting(groupBox8);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
        }
        #endregion
        
        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            // TREE정보 설정
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
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
                string Query = " exec usp_PBA172 'S1'";
                Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                Query += ", @pPRNT_BOM_NO = '1'";
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
                        , 0
                        , true); // 라우팅없을 것에 대한 색깔 처리

                    treeView1.Focus();
                    treeView1.ExpandAll();
                }
                else
                {
                    SystemBase.Validation.GroupBox_Reset(groupBox1);
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

            NEW_NODE_TAG = e.Node.Tag.ToString();

            // 라우팅 정보 초기화
            ROUT_NO = "";

            // 라우팅 정보 화면 출력
            ShowRoutInfo();

        }
        #endregion

        #region 라우팅 콤보 조회
        private void txtSITEM_CD_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                txtSITEM_NM.Value = "";
        }
        #endregion

        #region 자품목투입정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2Search(intRow);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자품목투입정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void fpSpread2Search(int R)
        {
            try
            {

                /// 팬텀일 경우 자품목 정보를 표시하지 않는다.
                if (PHANTOM_FLAG != "N")
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                    return;
                }

                PROC_SEQ = fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;

                string Query = " usp_PBA172 @pTYPE = 'S4'";
                Query += " ,@pPRNT_PLANT_CD='" + cboSPLANT_CD.SelectedValue + "'";
                Query += " ,@pPRNT_ITEM_CD='" + fpSpread2.Sheets[0].Cells[R, 0].Text + "'";
                Query += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query += " ,@pPROC_SEQ='" + PROC_SEQ + "'";
                Query += " ,@pROUT_NO='" + ROUT_NO + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region SaveExec() 저장
        protected override void SaveExec()
        {

            string strGbn = "";

            string ERRCode = "WR";
            string MSGCode = "P0000";
            string RPLMsg = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            // MASTER 수정항목이 있을 경우
            if (NewFlg == 2)  // 수정한 내용이 있을 경우
            {
                strGbn = "U1";
                string strSql = " usp_PBA172 '" + strGbn + "'";

                strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                strSql += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                strSql += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                strSql += ", @pMAJOR_FLG = '" + (rdoROUTINT1.Checked ? "Y" : "N") + "'";
                strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
            }

            if (UIForm.FPMake.HasSaveData(fpSpread1))
            {
                try
                {
                    for (int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[j, 0].Text;

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U":
                                    if (fpSpread1.Sheets[0].Cells[j, 1].Text == "True")
                                    {
                                        strGbn = "I2";
                                    }
                                    else
                                    {
                                        strGbn = "D2";
                                    }
                                    break;
                                default: strGbn = ""; break;
                            }
                            string strSql = " usp_PBA111 '" + strGbn + "'";

                            strSql += ", @pBOM_NO = '" + PRNT_BOM_NO + "'";
                            strSql += ", @pCHILD_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text + "'";
                            strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                            strSql += ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                            strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목코드")].Text + "'";
                            strSql += ", @pROUT_NO = '" + ROUT_NO.ToString() + "'";
                            strSql += ", @pPROC_SEQ = '" + PROC_SEQ.ToString() + "'";
                            strSql += ", @pMAJOR_FLG = '" + (rdoROUTINT1.Checked ? "Y" : "N") + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            }

            Trans.Commit();

        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                if (RPLMsg != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode, RPLMsg), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                ROUT_NO = txtROUTING_NO.Text;
                ShowRoutInfo();
            }
            else if (ERRCode == "ER")
            {
                if (RPLMsg != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode, RPLMsg), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (RPLMsg != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode, RPLMsg), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {


            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox8);

            rdoLEVEL1.Checked = true;

            SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpFROM_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpTO_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            treeView1.Nodes.Clear();

            NewFlg = 0; // 새로 등록 설정
        }
        #endregion

        #region 제품 OR 라운팅 정보 화면 출력
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

                ITEM_NM = NODETAG;

                // 주라우팅 정보를 조회한다.
                if (ROUT_NO == "")
                {
                    string Query = " usp_PBA172 @pTYPE = 'S7'";
                    Query += " ,  @pPLANT_CD='" + CHILD_PLANT_CD + "'";
                    Query += " ,  @pITEM_CD='" + CHILD_ITEM_CD + "'";
                    Query += " ,  @pVALID_DT='" + dtpSVALID_DT.Text + "'";
                    Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        ROUT_NO = dt.Rows[0]["ROUT_NO"].ToString();
                        txtROUTING_NO.Value = ROUT_NO;
                        txtROUTING_NM.Value = dt.Rows[0]["DESCRIPTION"].ToString();
                    }
                    else
                    {

                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

                        SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

                        txtITEM_CD.Value = CHILD_ITEM_CD.ToString();
                        txtITEM_NM.Value = ITEM_NM.ToString();
                        txtROUTING_NO.Value = "";
                        txtROUTING_NM.Value = "";
                        dtpFROM_VALID_DT.Value = "";
                        dtpTO_VALID_DT.Value = "2999-12-31";
                        rdoROUTINT1.Checked = true;

                        txtROU_DEV_USR_ID.Value = "";
                        txtROU_MFG_USR_ID.Value = "";
                        txtROU_QUR_USR_ID.Value = "";
                        txtROU_APP_USR_ID.Value = "";

                        txtROU_DEV_USR_NM.Value = "";
                        txtROU_MFG_USR_NM.Value = "";
                        txtROU_QUR_USR_NM.Value = "";
                        txtROU_APP_USR_NM.Value = "";

                        txtREV_NO.Value = "";
                        dtpREV_DT.Value = "";

                        return;
                    }
                }

                string Query2 = " usp_PBA172 @pTYPE = 'S2'";
                Query2 += " ,@pPLANT_CD='" + CHILD_PLANT_CD + "'";
                Query2 += " ,@pITEM_CD ='" + CHILD_ITEM_CD + "'";
                Query2 += " ,@pROUT_NO ='" + ROUT_NO + "'";
                Query2 += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, Query2, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
                
                
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용")].Value != null)
                        fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용")].Value = 'Y';
                    else                                                                
                        fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용")].Value = 'N';



                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용_2")].Locked = false;
                }

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                    fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                }


                /////////////////라우팅 정보////////////////////
                string Query3 = " usp_PBA172 @pTYPE = 'S5' ";
                Query3 += ", @pPLANT_CD='" + CHILD_PLANT_CD + "'";
                Query3 += ", @pITEM_CD='" + CHILD_ITEM_CD + "'";
                Query3 += ", @pROUT_NO='" + ROUT_NO + "'";
                Query3 += ", @pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query3 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(Query3);

                if (dt2.Rows.Count > 0)
                {
                    txtITEM_CD.Value = dt2.Rows[0]["ITEM_CD"].ToString();
                    txtITEM_NM.Value = dt2.Rows[0]["ITEM_NM"].ToString();
                    txtROUTING_NO.Value = dt2.Rows[0]["ROUT_NO"].ToString();
                    txtROUTING_NM.Value = dt2.Rows[0]["DESCRIPTION"].ToString();
                    dtpFROM_VALID_DT.Value = dt2.Rows[0]["VALID_FROM_DT"].ToString();
                    dtpTO_VALID_DT.Value = dt2.Rows[0]["VALID_TO_DT"].ToString();

                    txtROU_DEV_USR_ID.Value = dt2.Rows[0]["ROU_DEV_USER_ID"].ToString();
                    txtROU_MFG_USR_ID.Value = dt2.Rows[0]["ROU_MFG_USER_ID"].ToString();
                    txtROU_QUR_USR_ID.Value = dt2.Rows[0]["ROU_QUR_USER_ID"].ToString();
                    txtROU_APP_USR_ID.Value = dt2.Rows[0]["ROU_APP_USER_ID"].ToString();

                    txtROU_DEV_USR_NM.Value = dt2.Rows[0]["ROU_DEV_USER_NM"].ToString();
                    txtROU_MFG_USR_NM.Value = dt2.Rows[0]["ROU_MFG_USER_NM"].ToString();
                    txtROU_QUR_USR_NM.Value = dt2.Rows[0]["ROU_QUR_USER_NM"].ToString();
                    txtROU_APP_USR_NM.Value = dt2.Rows[0]["ROU_APP_USER_NM"].ToString();

                    txtREV_NO.Value = dt2.Rows[0]["REV_NO"].ToString();
                    dtpREV_DT.Value = dt2.Rows[0]["REV_DT"].ToString();

                    // 팬텀 설정
                    PHANTOM_FLAG = dt2.Rows[0]["PHANTOM_FLAG"].ToString();

                    if (dt2.Rows[0]["MAJOR_FLG"].ToString() == "Y")
                    {
                        rdoROUTINT1.Checked = true;
                    }
                    else
                    {
                        rdoROUTINT2.Checked = true;
                    }

                    btnROUT.Enabled = true;
                }
                else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

                    txtITEM_CD.Value = CHILD_ITEM_CD.ToString();
                    txtITEM_NM.Value = ITEM_NM.ToString();
                    txtROUTING_NO.Value = "";
                    txtROUTING_NM.Value = "";
                    dtpFROM_VALID_DT.Value = "";
                    dtpTO_VALID_DT.Value = "2999-12-31";
                    rdoROUTINT1.Checked = true;

                    txtROU_DEV_USR_ID.Value = "";
                    txtROU_MFG_USR_ID.Value = "";
                    txtROU_QUR_USR_ID.Value = "";
                    txtROU_APP_USR_ID.Value = "";

                    txtROU_DEV_USR_NM.Value = "";
                    txtROU_MFG_USR_NM.Value = "";
                    txtROU_QUR_USR_NM.Value = "";
                    txtROU_APP_USR_NM.Value = "";

                    txtREV_NO.Value = "";
                    dtpREV_DT.Value = "";

                }
                rdoROUTINT1.Enabled = true;
                rdoROUTINT2.Enabled = true;
                /////////////////라우팅 정보////////////////////

                // 자품목 조회
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                    fpSpread2Search(0);

                NewFlg = 0; // 수정/등록 플래그 초기화
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 품목코드 조회 변경시
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

        #region 라우팅 정보 조회
        private void btnROUT_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text == "")  // 품목코드 검사
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0030"));
                    return;
                }

                PBA172P1 pu = new PBA172P1(txtITEM_CD.Text);

                pu.Width = 320;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    ROUT_NO = pu.ReturnVal[0];

                    // 라우팅 정보 화면 출력
                    ShowRoutInfo();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region rdoROUTINT1_CheckedChanged
        private void rdoROUTINT1_CheckedChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }
        #endregion

         #region fpSpread2_ButtonClicked 그리드 상단 버튼 클릭
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용_2")) // 공정내용
                {

                    string PROC_PLAN = "X";
                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용")].Text == "Y")
                        PROC_PLAN = "U1";

                    WNDW.WNDW035 pu = new WNDW.WNDW035("X",
                                                       txtITEM_CD.Text,
                                                       txtROUTING_NO.Text,
                                                       fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text,
                                                       fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Text,
                                                       fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text,
                                                       fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text,
                                                       fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Text,
                                                       PROC_PLAN);
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.Cancel)
                    {
                        string Msgs = pu.ReturnData();

                        if (Msgs != "")
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용")].Text = "Y";
                        // fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread2, e.Row);
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}