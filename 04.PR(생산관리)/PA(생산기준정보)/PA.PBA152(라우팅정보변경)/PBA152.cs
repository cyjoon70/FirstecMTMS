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

namespace PA.PBA152
{
    public partial class PBA152 : UIForm.FPCOMM2_2
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

        string strRou = "";
        string ASSIGN_NO = "";

        #endregion

        #region 생성자
        public PBA152()
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

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpFROM_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpTO_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            SystemBase.Validation.GroupBox_Setting(groupBox8);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
            btnROUT.Enabled = true;
        }
        #endregion
        
        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            // TREE정보 설정
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
                TreeViewSearch();
        }

        public void TreeViewSearch()
        {
            try
            {
                treeView1.Nodes.Clear();
                string Query = " exec usp_PBA152 'S1'";
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

        #region 행추가
        protected override void RowInsExec()
        {// 행 추가
            try
            {
                if (txtITEM_CD.Text != "" && txtROUTING_NO.Text != "")
                {
                    UIForm.FPMake.RowInsert(fpSpread2);
                    int RowNum = fpSpread2.ActiveSheet.ActiveRowIndex;

                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = "1";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")].Value = "EA";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가")].Value = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value = "Y";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = "N";

                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "시간단위")].Value = "M";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "제조 L/T")].Text = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "Queue 시간")].Text = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Text = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "대기시간")].Text = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "고정가동시간")].Text = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Text = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "이동시간")].Text = "0";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "이동수량")].Text = "0";

                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "시작일")].Text = "2001-01-01";
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "종료일")].Text = "2999-12-31";
                    strRou = "";
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0045", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    strRou = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Text;

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
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                    return;
                }

                PROC_SEQ = fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;

                string Query = " usp_PBA152 @pTYPE = 'S4'";
                Query += " ,@pPRNT_PLANT_CD='" + cboSPLANT_CD.SelectedValue + "'";
                Query += " ,@pPRNT_ITEM_CD='" + fpSpread2.Sheets[0].Cells[R, 0].Text + "'";
                Query += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query += " ,@pPROC_SEQ='" + PROC_SEQ + "'";
                Query += " ,@pROUT_NO='" + ROUT_NO + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
            string strProc = "";

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    strGbn = "U2";
                    string strSq1l = " usp_PBA152 '" + strGbn + "'";

                    strSq1l += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                    strSq1l += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                    strSq1l += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                    strSq1l += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";

                    strSq1l += ", @pMAJOR_FLG = '" + (rdoROUTINT1.Checked ? "Y" : "N") + "'";
                    strSq1l += ", @pDESCRIPTION = '" + txtROUTING_NM.Text + "'";

                    strSq1l += ", @pROU_DEV_USER_ID = '" + txtROU_DEV_USR_ID.Text + "'";
                    strSq1l += ", @pROU_MFG_USER_ID = '" + txtROU_MFG_USR_ID.Text + "'";
                    strSq1l += ", @pROU_QUR_USER_ID = '" + txtROU_QUR_USR_ID.Text + "'";
                    strSq1l += ", @pROU_APP_USER_ID = '" + txtROU_APP_USR_ID.Text + "'";

                    strSq1l += ", @pREV_NO = '" + txtREV_NO.Text + "'";
                    strSq1l += ", @pREV_DT = '" + dtpREV_DT.Text + "'";

                    strSq1l += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";

                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSq1l, dbConn, Trans);
                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                if (fpSpread2.Focused == true || (fpSpread1.Focused != true && UIForm.FPMake.HasSaveData(fpSpread2)))
                {
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, "fpSpread2", false) == true) // 그리드 상단 필수항목 체크
                    {
                        try
                        {
                            for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                            {
                                string strHead = fpSpread2.Sheets[0].RowHeader.Cells[j, 0].Text;

                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "D": strGbn = "D1"; break;
                                        case "U": strGbn = "U1"; break;
                                        case "I": strGbn = "I1"; break;
                                        default: continue;
                                    }

                                    if (strHead == "U" || strHead == "I")
                                    {
                                        // 설치시간 + 변동시간이 0이 되지 않게 처리
                                        if ((Convert.ToInt32(fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Value) +
                                            Convert.ToInt32(fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Value)) <= 0)
                                        {
                                            MSGCode = "P0032";

                                            Trans.Rollback();
                                            goto Exit;
                                        }

                                        // INSIDE FLG 필수값 검사
                                        if (strHead == "I") // 기존에 등록된 공정 코드값을 등록하는지 여부 검사
                                        {
                                            for (int k = 0; k < fpSpread2.Sheets[0].Rows.Count; k++)
                                            {
                                                if (j == k)   // 다음 ROW 검사
                                                    continue;

                                                if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text ==
                                                    fpSpread2.Sheets[0].Cells[k, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text)
                                                {
                                                    MSGCode = "P0037";
                                                    RPLMsg = "공정";

                                                    Trans.Rollback();
                                                    goto Exit;
                                                }
                                            }
                                        }
                                    }

                                    string strSql = " usp_PBA152 '" + strGbn + "'";

                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                    strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                    if (strGbn == "I")
                                        strSql += ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[j, 0].Value + "'";
                                    else
                                        strSql += ", @pITEM_CD = '" + CHILD_ITEM_CD.ToString() + "'";

                                    strSql += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                                    strSql += ", @pDESCRIPTION = '" + txtROUTING_NM.Text + "'";

                                    strSql += ", @pROU_DEV_USER_ID = '" + txtROU_DEV_USR_ID.Text + "'";
                                    strSql += ", @pROU_MFG_USER_ID = '" + txtROU_MFG_USR_ID.Text + "'";
                                    strSql += ", @pROU_QUR_USER_ID = '" + txtROU_QUR_USR_ID.Text + "'";
                                    strSql += ", @pROU_APP_USER_ID = '" + txtROU_APP_USR_ID.Text + "'";
                                    strSql += ", @pREV_NO = '" + txtREV_NO.Text + "'";
                                    strSql += ", @pREV_DT = '" + dtpREV_DT.Text + "'";

                                    if (rdoROUTINT1.Checked == true)
                                        strSql += ", @pMAJOR_FLG = 'Y'";
                                    else
                                        strSql += ", @pMAJOR_FLG = 'N'";

                                    strProc = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;

                                    strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text + "'";
                                    strSql += ", @pINSIDE_FLG = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value + "'";
                                    strSql += ", @pJOB_CD = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Value + "'";
                                    strSql += ", @pWC_CD = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value + "'";
                                    strSql += ", @pRES_CD = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text + "'";
                                    strSql += ", @pTIME_UNIT = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "시간단위")].Value + "'";
                                    strSql += ", @pMFG_LT = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "제조 L/T")].Value + "'";
                                    strSql += ", @pQUEUE_TIME = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "Queue 시간")].Value + "";

                                    strSql += ", @pSETUP_TIME = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Value + "";

                                    strSql += ", @pWAIT_TIME = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "대기시간")].Value + "";
                                    strSql += ", @pFIX_RUN_TIME = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "고정가동시간")].Value + "";
                                    strSql += ", @pRUN_TIME = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Value + "";
                                    strSql += ", @pMOVE_TIME = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "이동시간")].Value + "";
                                    strSql += ", @pMOVE_QTY = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "이동수량")].Value + "";
                                    strSql += ", @pRUN_TIME_QTY = " + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Value + "";
                                    strSql += ", @pRUN_TIME_UNIT = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")].Value + "'";
                                    strSql += ", @pBP_CD = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text + "'";
                                    strSql += ", @pCUR_CD = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Text + "'";
                                    strSql += ", @pSUBCONTRACT_PRC = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가")].Value + "'";
                                    strSql += ", @pTAX_TYPE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Value + "'";
                                    strSql += ", @pMILESTONE_FLG = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value + "'";
                                    strSql += ", @pINSP_FLG = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value + "'";
                                    strSql += ", @pROUT_ORDER = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")].Value + "'";
                                    strSql += ", @pVALID_FROM_DT = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "시작일")].Text + "'";
                                    strSql += ", @pVALID_TO_DT = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "종료일")].Text + "'";
                                    strSql += ", @pREMARK = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "비고")].Text + "'";
                                    strSql += ", @pCUST_SIZE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "국방규격")].Text + "'";
                                    strSql += ", @pROUT_DOC  = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정문서")].Text + "'";
                                    strSql += ", @pROUT_SIZE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].Text + "'";
                                    strSql += ", @pMTMG_NUMB = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호")].Text + "'";

                                    strSql += ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pBOM_NO = '" + CHILD_BOM_NO + "'";

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
                    else
                    {
                        Trans.Rollback();
                        return;
                    }
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
                                string strSql = " usp_PBA152 '" + strGbn + "'";

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

                    UIForm.FPMake.GridSetFocus(fpSpread2, strProc, SystemBase.Base.GridHeadIndex(GHIdx2, "공정"));
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
        }
        #endregion

        #region fpSpread2_ButtonClicked 그리드 상단 버튼 클릭
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 공정조회
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드_2"))
                {
                    string strQuery = " usp_P_COMMON 'P042', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC", "@pLANG_CD" };
                    string[] strSearch = new string[] { "", "", "P001", SystemBase.Base.gstrLangCd };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("WB5101", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Text = Msgs[0].ToString(); //공정코드
                        //						fpSpread2.Sheets[0].Cells[e.Row,SystemBase.Base.GridHeadIndex(GHIdx2,"공정명")].Value	= Msgs[1].ToString(); //공정명

                        
                        string strQuery1 = "";
                        strQuery1 += " usp_PBA152 'P2' ";
                        strQuery1 += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                        strQuery1 += " , @pJOB_CD='" + Msgs[0].ToString() + "'";
                        strQuery1 += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                        if (dt.Rows.Count > 0)
                        {
                            // 자원정보를 조회한다.
                            string strProdCd = dt.Rows[0]["JOB_CD"].ToString();
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text = dt.Rows[0]["CD_NM"].ToString();	//자원명
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Text = dt.Rows[0]["SETUP_TIME"].ToString();
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Text = dt.Rows[0]["RUN_TIME"].ToString();
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = dt.Rows[0]["RUN_TIME_QTY"].ToString();
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value = dt.Rows[0]["MILESTONE_FLG"].ToString();
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = dt.Rows[0]["INSP_FLG"].ToString();
                        }
                        else
                        {
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text = "";	//자원명
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Text = "";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Text = "";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = "";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value = "";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = "";
                        }


                        UIForm.FPMake.fpChange(fpSpread2, e.Row);
                    }
                }
                // 자원조회
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "자원_2"))
                {

                    string strQuery = " usp_P_COMMON 'P051', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { "", "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05005", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text = Msgs[0].ToString();	//자원코드
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = Msgs[1].ToString();	//자원명
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = Msgs[2].ToString();	//작업장
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = Msgs[3].ToString();	//공정타입

                        UIForm.FPMake.fpChange(fpSpread2, e.Row);

                        if (Msgs[3].ToString() == "N")  // 자원이 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호") + "|0" //임시로 일반
                            );
                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Value = "A";
                        }
                        else
                        {
                            // 외주란을 비활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호") + "|0"
                                );

                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = "";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = "";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Text = "";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가")].Text = "0.00";
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Text = "";
                        }
                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")) // 외주조회
                {
                    // 공정 타입이 외주일 경우 처리
                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value == null ||
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value.ToString() == "Y")
                    {
                        SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0031"));
                        return;
                    }

                    WNDW.WNDW002 pu = new WNDW.WNDW002("P");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = Msgs[1].ToString();
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = Msgs[2].ToString();

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

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox8);

            rdoLEVEL1.Checked = true;

            SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

            txtROUTING_NO.Value = "";
            txtROUTING_NM.Value = "";
            dtpFROM_VALID_DT.Value = "2001-01-01";
            dtpTO_VALID_DT.Value = "2999-12-31";

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

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            treeView1.Nodes.Clear();

            NewFlg = 1; // 새로 등록 설정
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
                    string Query = " usp_PBA152 @pTYPE = 'S7'";
                    Query += " ,  @pPLANT_CD='" + CHILD_PLANT_CD + "'";
                    Query += " ,  @pITEM_CD='" + CHILD_ITEM_CD + "'";
                    Query += " ,  @pVALID_DT='" + dtpSVALID_DT.Text + "'";
                    Query += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                    if (dt.Rows.Count > 0)
                    {
                        ROUT_NO = dt.Rows[0]["ROUT_NO"].ToString();
                        txtROUTING_NO.Value = ROUT_NO;
                        txtROUTING_NM.Value = dt.Rows[0]["DESCRIPTION"].ToString();
                    }
                    else
                    {

                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                        SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

                        btnROUT.Enabled = true;

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

                string Query2 = " usp_PBA152 @pTYPE = 'S2'";
                Query2 += " ,@pPLANT_CD='" + CHILD_PLANT_CD + "'";
                Query2 += " ,@pITEM_CD ='" + CHILD_ITEM_CD + "'";
                Query2 += " ,@pROUT_NO ='" + ROUT_NO + "'";
                Query2 += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, Query2, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                    fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }


                /////////////////라우팅 정보////////////////////
                string Query3 = " usp_PBA152 @pTYPE = 'S5' ";
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

                        //if (dt2.Rows[0]["ROUT_NO"].ToString() != SystemBase.Base.RoutingNo("D") &&
                        //   dt2.Rows[0]["ROUT_NO"].ToString() != SystemBase.Base.RoutingNo("C")) // 개발/보조 라우팅이 아닐 경우
                        //{
                        //    // 주라우팅이 될수 있다
                        //    rdoROUTINT1.Enabled = true;
                        //    rdoROUTINT2.Enabled = true;
                        //}
                    }

                    btnRev.Enabled = true;
                }
                else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

                    btnROUT.Enabled = true;

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

                    btnRev.Enabled = false;

                }
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

        #region rdoROUTINT1_CheckedChanged
        private void rdoROUTINT1_CheckedChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
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

                    dtpFROM_VALID_DT.Value = Msgs[21].ToString();
                    dtpTO_VALID_DT.Value = Msgs[22].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region REV 버튼 클릭
        private void btnRev_Click(object sender, System.EventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    bool chk = false;

                    string strRout_Seq = "";
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Text == "True")
                        {
                            if (chk == false)
                                chk = true;

                            if (strRout_Seq == "")
                            {
                                strRout_Seq = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;
                            }
                            else
                            {
                                strRout_Seq = strRout_Seq + "!!" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;
                            }
                        }
                    }
                    if (chk == false) return;

                    string strItem_Cd = txtITEM_CD.Text;				//품목코드
                    string strItem_Nm = txtITEM_NM.Text;				//품목명
                    string strRout_No = txtROUTING_NO.Text;				//라우팅번호
                    string strRout_Nm = txtROUTING_NM.Text;				//라우팅명
                    string strRev_No = txtREV_NO.Text;					//리비젼번호
                    string strBOM_DEV_USR_ID = txtROU_DEV_USR_ID.Text;	//설계자
                    string strBOM_MFG_USR_ID = txtROU_MFG_USR_ID.Text;	//생산검토자
                    string strBOM_QUR_USR_ID = txtROU_QUR_USR_ID.Text;	//품질검토자
                    string strBOM_APP_USR_ID = txtROU_APP_USR_ID.Text;	//승인자

                    PBA152P1 pu = new PBA152P1(strItem_Cd, strItem_Nm, strRout_No, strRout_Nm, strRev_No, strBOM_DEV_USR_ID, strBOM_MFG_USR_ID, strBOM_QUR_USR_ID, strBOM_APP_USR_ID, strRout_Seq);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        ShowRoutInfo();
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("B0048"));
                }
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
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0030"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                PBA152P2 pu = new PBA152P2(txtITEM_CD.Text);

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

    }
}