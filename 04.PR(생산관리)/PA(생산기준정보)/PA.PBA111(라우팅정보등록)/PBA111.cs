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
namespace PA.PBA111
{
    public partial class PBA111 : UIForm.FPCOMM2_2
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
        string strOldRou = "";

        string NEW_NODE_TAG = "";
        int NewFlg = 0; // 등록 수정 FLAG

        string strRou = "";

        #endregion

        #region 생성자
        public PBA111()
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
            txtREV_NO.Value = "1";
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

        public void TreeViewSearch()
        {
            try
            {
                treeView1.Nodes.Clear();
                string Query = " exec usp_PBA111 'S1'";
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

                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "수정")].Locked = true;
                    fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용_2")].Locked = true;
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

        #region 행삭제
        protected override void DelExec()
        {
            try
            {
                UIForm.FPMake.RowRemove(fpSpread2);
                //fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "수정")].Text = "false";
                //fpSpread2.Sheets[0].RowHeader.Cells[RowNum, 0].Text = "D";

                DelExe();
                for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                {
                    string strHead = fpSpread2.Sheets[0].RowHeader.Cells[j, 0].Text;
                    if (strHead == "D" || strHead == "U")
                    {
                        fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].BackColor = SystemBase.Validation.Kind_LightCyan;
                        fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Locked = false;
                    } 
                    else
                    {
                        fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                        fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Locked = true;
                    }
                    if (strHead == "D")
                    {
                        fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "SAVE")].Text = "D";
                    }
                    else if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "SAVE")].Text == "D")
                    {
                        fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "SAVE")].Text = "";
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if(strRou != strOldRou)
                        fpSpread2Search(intRow);

                    strOldRou = strRou;
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

                string Query = " usp_PBA111 @pTYPE = 'S4'";
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
            string strProc = "";

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                bool SAVE_YN = false;

                for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                {
                    if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "SAVE")].Text == "D")
                    {
                        fpSpread2.Sheets[0].RowHeader.Cells[j, 0].Text = "D";
                    }
                }
                if (rdoROUTINT1.Checked == true)
                {
                    string strSql = " usp_PBA111 '" + "U3" + "'"; 
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                    strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                    strSql += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                    strSql += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                    strSql += ", @pMAJOR_FLG = 'Y'";
                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                }


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
                                    if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "수정")].Text == "True")
                                    {
                                        strGbn = "U1";
                                        SAVE_YN = true;
                                    }
                                    else
                                        switch (strHead)
                                        {
                                            case "D": strGbn = "D1"; SAVE_YN = true; break;
                                            case "I": strGbn = "I1"; SAVE_YN = true; break;
                                            default: strGbn = ""; break;
                                        }
                                    if (strGbn == "")
                                    {
                                        continue;
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



                                    string strSql = " usp_PBA111 '" + strGbn + "'";

                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                    strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                    if (strGbn == "I")
                                        strSql += ", @pITEM_CD = '" + fpSpread2.Sheets[0].Cells[j, 0].Value + "'";
                                    else
                                        strSql += ", @pITEM_CD = '" + CHILD_ITEM_CD.ToString() + "'";

                                    if (strGbn == "U1")
                                    {
                                        if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Text == "")
                                        {
                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            MessageBox.Show(SystemBase.Base.MessageRtn("변경사유를 적어주세요."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                    }


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
                                    if (strGbn == "D1")
                                    {

                                        if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Text == "")
                                        {
                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            MessageBox.Show(SystemBase.Base.MessageRtn("변경사유를 적어주세요."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        else
                                            strSql += ", @pREV_RESAN = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Text + "(삭제)" + "'";
                                    }
                                    else if (strGbn == "U1")
                                    {
                                        if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Text == "")
                                        {
                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            MessageBox.Show(SystemBase.Base.MessageRtn("변경사유를 적어주세요."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        else
                                            strSql += ", @pREV_RESAN = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Text + "(변경)" + "'";
                                    }
                                    else if (strGbn == "I1")
                                    {
                                        if (fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Text == "")
                                        {
                                            Trans.Rollback();
                                            this.Cursor = Cursors.Default;
                                            MessageBox.Show(SystemBase.Base.MessageRtn("변경사유를 적어주세요."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        else
                                            strSql += ", @pREV_RESAN = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Text + "(신규)" + "'";
                                    }
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
                                    strSql += ", @pROUT_DOC  = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].Text + "'";
                                    strSql += ", @pROUT_SIZE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].Text + "'";
                                    strSql += ", @pMTMG_NUMB = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호")].Text + "'";
                                    //추가
                                    strSql += ", @pROUT_REC = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정기록")].Text + "'";
                                    strSql += ", @pROUT_CYClE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정주기")].Text + "'";
                                    strSql += ", @pJIG_FIXTURE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "JIG&FIXTURE")].Text + "'";
                                    strSql += ", @pNC_PGM = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "NC PGM")].Text + "'";

                                    strSql += ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pBOM_NO = '" + CHILD_BOM_NO + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

                                    //공정내용 삭제
                                    if (strGbn == "D1")
                                    {
                                        string strSql2 = " usp_WNDW035 '" + strGbn + "'";

                                        strSql2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                        strSql2 += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                        strSql2 += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                                        strSql2 += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                                        strSql2 += ", @pPROC_SEQ = '" + strProc + "'";

                                        DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql2, dbConn, Trans);
                                        ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds1.Tables[0].Rows[0][1].ToString();
                                    }

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
                    try
                    {
                        if (SAVE_YN)
                        {
                            strGbn = "U2";
                            string strSq1l = " usp_PBA111 '" + strGbn + "'";

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
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }

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
                    string strQuery = " usp_PBA111 'P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pJOB_CD", "" };
                    string[] strSearch = new string[] { fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Text, "" };

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
                        strQuery1 += " usp_PBA111 'P2' ";
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
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2") + "|1"
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
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용_2")) // 공정내용
                {
                    string PROC_PLAN = "";

                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용")].Text == "Y")
                        PROC_PLAN = "U1";
                    else
                        PROC_PLAN = "I1";
                    WNDW.WNDW035 pu = new WNDW.WNDW035("U",
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
                        else
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정내용")].Text = "N";

                        UIForm.FPMake.fpChange(fpSpread2, e.Row);
                    }

                }
                else if (e.Column == 1)
                {

                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "수정")].Text == "True")
                    {
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "수정")].Text == "True")
                            {
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드_2")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드_2")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원_2")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원_2")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조 L/T")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조 L/T")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "Queue 시간")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "Queue 시간")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "대기시간")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "대기시간")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "고정가동시간")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "고정가동시간")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동시간")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동시간")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동수량")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동수량")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시작일")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시작일")].Locked = false;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "종료일")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "종료일")].Locked = false;

                                //흰
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정기록")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정기록")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정주기")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정주기")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "JIG&FIXTURE")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "JIG&FIXTURE")].Locked = false;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "NC PGM")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "NC PGM")].Locked = false;


                                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Text == "외주")  // 자원이 외주일 경우
                                {
                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Locked = false;

                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")].BackColor = SystemBase.Validation.Kind_LightCyan;
                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")].Locked = false;
                                }
                                else
                                {
                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Locked = true;

                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")].Locked = true;
                                }
                            }

                        }

                    }
                    else
                    {
                        for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "수정")].Text == "True")
                            {
                            }
                            else if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text == "U")
                            {
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변경사유")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드_2")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드_2")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원_2")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "자원_2")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조 L/T")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "제조 L/T")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "Queue 시간")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "Queue 시간")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "대기시간")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "대기시간")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "고정가동시간")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "고정가동시간")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동시간")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동시간")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동수량")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이동수량")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시작일")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "시작일")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "종료일")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "종료일")].Locked = true;


                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정기록")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정기록")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정주기")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "공정주기")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "JIG&FIXTURE")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "JIG&FIXTURE")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "NC PGM")].BackColor = Color.White;
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "NC PGM")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Locked = true;

                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처_2")].Locked = true;

                            }

                        }
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
            rdoLEVEL1.Checked = true;

            SystemBase.Validation.GroupBoxControlsLock(groupBox1, false);

            txtREV_NO.ReadOnly = true;
            txtREV_NO.BackColor = SystemBase.Validation.Kind_White;
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

            txtREV_NO.Value = "0";      // 2018.04.21. hma 수정: 리비전번호가 0부터 생성되도록 하기 위해 기본값을 1=>0으로 변경
            dtpREV_DT.Value = "";

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
            dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            //dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            //treeView1.Nodes.Clear();

            NewFlg = 1; // 새로 등록 설정
        }
        #endregion

        #region MASTER 삭제
        protected override void DeleteExec()
        {// 행 삭제
            try
            {
                string strGbn = "";

                string ERRCode = "WR";
                string MSGCode = "P0000";
                string strProc = "";
                if (MessageBox.Show(SystemBase.Base.MessageRtn("P0046"), "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                    if (txtITEM_CD.Text != "" && txtROUTING_NO.Text != "")
                    {

                        try
                        {
                            for (int j = 0; j < fpSpread2.Sheets[0].Rows.Count; j++)
                            {

                                strGbn = "D1";
                                string strSql = " usp_PBA111 '" + strGbn + "'";

                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                strSql += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                strSql += ", @pITEM_CD = '" + CHILD_ITEM_CD.ToString() + "'";

                                strSql += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                                strSql += ", @pDESCRIPTION = '" + txtROUTING_NM.Text + "'";

                                strSql += ", @pROU_DEV_USER_ID = '" + txtROU_DEV_USR_ID.Text + "'";
                                strSql += ", @pROU_MFG_USER_ID = '" + txtROU_MFG_USR_ID.Text + "'";
                                strSql += ", @pROU_QUR_USER_ID = '" + txtROU_QUR_USR_ID.Text + "'";
                                strSql += ", @pROU_APP_USER_ID = '" + txtROU_APP_USR_ID.Text + "'";
                                strSql += ", @pREV_NO = '" + txtREV_NO.Text + "'";
                                strSql += ", @pREV_DT = '" + dtpREV_DT.Text + "'";


                                strProc = fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;

                                strSql += ", @pPROC_SEQ = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text + "'";

                                strSql += ", @pREV_RESAN = '" + "전체삭제" + "'";

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
                                strSql += ", @pROUT_DOC  = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정지침")].Text + "'";
                                strSql += ", @pROUT_SIZE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정규격")].Text + "'";
                                strSql += ", @pMTMG_NUMB = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호")].Text + "'";
                                //추가
                                strSql += ", @pROUT_REC = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정기록")].Text + "'";
                                strSql += ", @pROUT_CYClE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "공정주기")].Text + "'";
                                strSql += ", @pJIG_FIXTURE = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "JIG&FIXTURE")].Text + "'";
                                strSql += ", @pNC_PGM = '" + fpSpread2.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "NC PGM")].Text + "'";

                                strSql += ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pBOM_NO = '" + CHILD_BOM_NO + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

                                //공정내용 삭제
                                if (strGbn == "D1")
                                {
                                    string strSql2 = " usp_WNDW035 '" + strGbn + "'";

                                    strSql2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                                    strSql2 += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                    strSql2 += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                                    strSql2 += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                                    strSql2 += ", @pPROC_SEQ = '" + strProc + "'";

                                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql2, dbConn, Trans);
                                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();
                                }

                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            }
                        }
                        catch (Exception f)
                        {
                            SystemBase.Loggers.Log(this.Name, f.ToString());
                            Trans.Rollback();
                            ERRCode = "ER";
                            MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                        }
                        Trans.Commit();



                        string strSql1 = " usp_PBA111 'D3'";
                        strSql1 += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                        strSql1 += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                        strSql1 += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                        strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql1);


                        //공정내용 전체삭제

                        string strSql3 = " usp_WNDW035 'D2'";

                        strSql3 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                        strSql3 += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                        strSql3 += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                        strSql3 += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";

                        DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strSql3);


                        MessageBox.Show(dt.Rows[0][1].ToString());
                        ShowRoutInfo();
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("P0047", "전체 삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전체 삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 공정 설계자 조회 조회
        private void btnDEV_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_DEV_USR_ID.Text, txtROU_DEV_USR_NM.Text, "RD" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정설계자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_DEV_USR_ID.Value = Msgs[0].ToString();
                    txtROU_DEV_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정설계자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 생산검토자 조회 조회
        private void btnMFG_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_MFG_USR_ID.Text, txtROU_MFG_USR_NM.Text, "RM" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "생산검토자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_MFG_USR_ID.Value = Msgs[0].ToString();
                    txtROU_MFG_USR_NM.Value = Msgs[1].ToString();
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
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_QUR_USR_ID.Text, txtROU_QUR_USR_NM.Text, "RQ" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품질검토자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_QUR_USR_ID.Value = Msgs[0].ToString();
                    txtROU_QUR_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 라우팅확인자 조회 조회
        private void btnAPP_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM", "@pETC" };
                string[] strSearch = new string[] { txtROU_APP_USR_ID.Text, txtROU_APP_USR_NM.Text, "RA" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "라우팅확인자 조회", true);

                pu.Width = 500;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtROU_APP_USR_ID.Value = Msgs[0].ToString();
                    txtROU_APP_USR_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅확인자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    string Query = " usp_PBA111 @pTYPE = 'S7'";
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
                        //SystemBase.Validation.GroupBoxControlsLock(groupBox1, false);
                    }
                    else
                    {

                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 7);

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

                        txtREV_NO.Value = "1";
                        dtpREV_DT.Value = "";

                        return;
                    }
                }

                string Query2 = " usp_PBA111 @pTYPE = 'S2'";
                Query2 += " ,@pPLANT_CD='" + CHILD_PLANT_CD + "'";
                Query2 += " ,@pITEM_CD ='" + CHILD_ITEM_CD + "'";
                Query2 += " ,@pROUT_NO ='" + ROUT_NO + "'";
                Query2 += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, Query2, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 7);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                    fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

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
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호") + "|0" //임시로 일반
                                );
                        }
                    }
                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }


                /////////////////라우팅 정보////////////////////
                string Query3 = " usp_PBA111 @pTYPE = 'S5' ";
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
                    if (txtREV_NO.Value == "")
                    {
                        txtREV_NO.Value = 0;
                    }

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

                        if (dt2.Rows[0]["ROUT_NO"].ToString() != SystemBase.Base.RoutingNo("D") &&
                           dt2.Rows[0]["ROUT_NO"].ToString() != SystemBase.Base.RoutingNo("C")) // 개발/보조 라우팅이 아닐 경우
                        {
                            // 주라우팅이 될수 있다
                            rdoROUTINT1.Enabled = true;
                            rdoROUTINT2.Enabled = true;
                        }
                    }

                    SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
                    dtpTO_VALID_DT.Enabled = true;

                    // 수정 가능항목 활성화
                    rdoROUTINT1.Enabled = true;
                    rdoROUTINT2.Enabled = true;

                    txtROUTING_NM.BackColor = SystemBase.Validation.Kind_White;
                    txtROUTING_NM.ReadOnly = false;

                    txtROU_DEV_USR_ID.BackColor = SystemBase.Validation.Kind_LightCyan;
                    txtROU_DEV_USR_ID.ReadOnly = false;
                    btnDEV.Enabled = true;

                    txtROU_MFG_USR_ID.BackColor = SystemBase.Validation.Kind_LightCyan;
                    txtROU_MFG_USR_ID.ReadOnly = false;
                    btnMFG.Enabled = true;

                    txtROU_QUR_USR_ID.BackColor = SystemBase.Validation.Kind_LightCyan;
                    txtROU_QUR_USR_ID.ReadOnly = false;
                    btnQUR.Enabled = true;

                    txtROU_APP_USR_ID.BackColor = SystemBase.Validation.Kind_LightCyan;
                    txtROU_APP_USR_ID.ReadOnly = false;
                    btnAPP.Enabled = true;

                    txtREV_NO.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                    txtREV_NO.ReadOnly = true;

                    dtpREV_DT.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                    dtpREV_DT.ReadOnly = true;
                    //dtpREV_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
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

                    txtREV_NO.Value = "1";
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
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0030"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                PBA111P1 pu = new PBA111P1(txtITEM_CD.Text);

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

        #region 변경 여부 처리
        private void txtROU_DEV_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtROU_DEV_USR_ID.Text != "")
                {
                    txtROU_DEV_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtROU_DEV_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtROU_DEV_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM설계자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }

        private void txtROU_MFG_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtROU_MFG_USR_ID.Text != "")
                {
                    txtROU_MFG_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtROU_MFG_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtROU_MFG_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }

        private void txtROU_QUR_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtROU_QUR_USR_ID.Text != "")
                {
                    txtROU_QUR_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtROU_QUR_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtROU_QUR_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }

        private void txtROU_APP_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtROU_APP_USR_ID.Text != "")
                {
                    txtROU_APP_USR_NM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtROU_APP_USR_ID.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtROU_APP_USR_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM확인자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }

        private void txtREV_NO_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }

        private void rdoROUTINT1_CheckedChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }

        private void rdoROUTINT2_CheckedChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }

        private void txtROUTING_NM_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정 상태로 변경
        }
        #endregion

        #region 스프레드 값 변경 처리
        protected override void fpSpread2_ChangeEvent(int Row, int Col)
        {

            try
            {

                // 자원 컬럼
                int resCol = SystemBase.Base.GridHeadIndex(GHIdx2, "자원");
                int rouCol = SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드");
                string strRou2 = fpSpread2.Sheets[0].Cells[Row, rouCol].Text;


                // 공정정보 붙여넣기 일 경우
                if (Col == rouCol)
                {
                    string strQuery = "";
                    strQuery += " usp_PBA111 'P2' ";
                    strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += " , @pJOB_CD='" + fpSpread2.Sheets[0].Cells[Row, rouCol].Text.Trim() + "'";
                    strQuery += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        // 자원정보를 조회한다.
                        string strProdCd = dt.Rows[0]["JOB_CD"].ToString();
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text = dt.Rows[0]["CD_NM"].ToString();	//자원명
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Text = dt.Rows[0]["SETUP_TIME"].ToString();
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Text = dt.Rows[0]["RUN_TIME"].ToString();
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = dt.Rows[0]["RUN_TIME_QTY"].ToString();
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value = dt.Rows[0]["MILESTONE_FLG"].ToString();
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = dt.Rows[0]["INSP_FLG"].ToString();

                        strRou = dt.Rows[0]["JOB_CD"].ToString();	//자원코드
                    }
                    else
                    {
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text = "";	//자원명
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Text = "";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Text = "";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = "";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value = "";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = "";
                    }

                    UIForm.FPMake.fpChange(fpSpread2, Row);
                }
                else
                {
                    //					if(Col != SystemBase.Base.GridHeadIndex(GHIdx2,"설치시간") && Col != SystemBase.Base.GridHeadIndex(GHIdx2,"변동가동시간")
                    //						&& Col != SystemBase.Base.GridHeadIndex(GHIdx2,"제조 L/T") && Col != SystemBase.Base.GridHeadIndex(GHIdx2,"기준수량")
                    //						&& Col != SystemBase.Base.GridHeadIndex(GHIdx2, "Queue 시간"))
                    if (Col == rouCol)
                    {
                        if (fpSpread2.Sheets[0].Cells[Row, rouCol].Text.Trim() != "")
                        {
                            string strQuery = "";
                            strQuery += " usp_PBA111 'P2' ";
                            strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                            strQuery += " , @pJOB_CD='" + fpSpread2.Sheets[0].Cells[Row, rouCol].Text.Trim() + "'";
                            strQuery += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                            if (dt.Rows.Count > 0)
                            {
                                // 자원정보를 조회한다.
                                string strProdCd = dt.Rows[0]["JOB_CD"].ToString();
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text = dt.Rows[0]["CD_NM"].ToString();	//자원명
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Text = dt.Rows[0]["SETUP_TIME"].ToString();
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Text = dt.Rows[0]["RUN_TIME"].ToString();
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = dt.Rows[0]["RUN_TIME_QTY"].ToString();
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value = dt.Rows[0]["MILESTONE_FLG"].ToString();
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = dt.Rows[0]["INSP_FLG"].ToString();

                                strRou = dt.Rows[0]["JOB_CD"].ToString();	//자원코드
                            }
                            else
                            {
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Text = "";	//자원명
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "설치시간")].Text = "";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "변동가동시간")].Text = "";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = "";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value = "";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = "";
                            }

                            UIForm.FPMake.fpChange(fpSpread2, Row);
                        }
                    }
                }

                // 자원정보 붙여넣기 일 경우
                if (Col == resCol)
                {
                    string strQuery = "";
                    strQuery += " usp_P_COMMON 'P051' ";
                    strQuery += " , @pCOM_CD='" + fpSpread2.Sheets[0].Cells[Row, resCol].Text + "'";
                    strQuery += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        // 자원정보를 조회한다.
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = dt.Rows[0]["RES_DIS"].ToString();	//자원명
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = dt.Rows[0]["WORKCENTER_CD"].ToString();	//작업장
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();	//공정타입

                        if (dt.Rows[0]["INSIDE_FLG"].ToString() == "N")  // 자원이 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, Row,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호") + "|0" //임시로 일반
                                );
                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Value = "A";
                        }
                        else
                        {
                            // 외주란을 비활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, Row,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호") + "|0"
                                );

                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = "";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = "";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Text = "";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가")].Text = "0.00";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Text = "";
                        }
                    }
                    else
                    {
                        // 초기값 설정
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Text = "";	//자원명
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = "";	//작업장
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = "";	//공정타입
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = "";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = "";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Text = "";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가")].Text = "0.00";
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Text = "";
                    }

                    UIForm.FPMake.fpChange(fpSpread2, Row);
                }
                else
                {
                    if (fpSpread2.Sheets[0].Cells[Row, resCol].Text != "")
                    {
                        string strQuery = "";
                        strQuery += " usp_P_COMMON 'P051' ";
                        strQuery += " , @pCOM_CD='" + fpSpread2.Sheets[0].Cells[Row, resCol].Text + "'";
                        strQuery += " , @pCO_CD='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                        if (dt.Rows.Count > 0)
                        {
                            // 자원정보를 조회한다.
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = dt.Rows[0]["RES_DIS"].ToString();	//자원명
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = dt.Rows[0]["WORKCENTER_CD"].ToString();	//작업장
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();	//공정타입

                            if (dt.Rows[0]["INSIDE_FLG"].ToString() == "N")  // 자원이 외주일 경우
                            {
                                // 외주란을 활성화 시간다.
                                UIForm.FPMake.grdReMake(fpSpread2, Row,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호") + "|0" //임시로 일반
                                    );
                                // 초기값 설정
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Value = "A";
                            }
                            else
                            {
                                // 외주란을 비활성화 시간다.
                                UIForm.FPMake.grdReMake(fpSpread2, Row,
                                    SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부품관리번호") + "|0"
                                    );

                                // 초기값 설정
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = "";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = "";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Text = "";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가")].Text = "0.00";
                                fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Text = "";
                            }
                        }
                        else
                        {
                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Text = "";	//자원명
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = "";	//작업장
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = "";	//공정타입
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = "";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = "";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Text = "";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가")].Text = "0.00";
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")].Text = "";
                        }

                        UIForm.FPMake.fpChange(fpSpread2, Row);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "스프레트 값변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region  fpSpread1 CellClick
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
                        {
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                fpSpread1.Sheets[0].Cells[i, e.Column].Value = true;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                fpSpread1.Sheets[0].Cells[i, e.Column].Value = false;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region fpSpread2 콤보 셀 변경
        private void fpSpread2_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {

                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부"))
                {
                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value.ToString() == "N" && fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value.ToString() == "Y")
                    {
                        MessageBox.Show("MILESTONE이 'N'일 경우 검사여부는 'Y'가 될 수 없습니다.");
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = "N";
                        return;
                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE"))
                {
                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")].Value.ToString() == "N" && fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value.ToString() == "Y")
                    {
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")].Value = "N";
                        return;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        

    }
}