#region 작성정보
/*********************************************************************/
// 단위업무명 : 선별형검사조건정보
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-21
// 작성내용 : 선별형검사조건정보 관리
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
using System.Reflection;

namespace PC.PBB101
{
    public partial class PBB101 : UIForm.FPCOMM1
    {
        #region 변수선언
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
        string WORKORDER_NO_OG = "";
        string strBtn = "N";
        int NewFlg = 0;          // 등록/수정 FLAG
        string NEW_NODE_TAG = "";   // NODE 설정
        string FIG_NO = "";
        #endregion

        #region 생성자
        public PBB101()
        {
            InitializeComponent();
        }
        #endregion
        
        #region SearchExec() 왼쪽 트리뷰 조회
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
            {
                if (txtSITEM_CD.Text == "" && txtSWORKORDER_NO_OG.Text == "")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0039", "품목코드||제조오더"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
                string Query = " exec usp_PBB101 'S1'";

                Query += ", @pPROJECT_NO = '" + txtPROJECT_NO.Text + "'";
                Query += ", @pPROJECT_SEQ = '" + txtPROJECT_SEQ.Text + "'";

                if (GROUP_CD != null && GROUP_CD != "")
                    Query += ", @pGROUP_CD = '" + GROUP_CD + "'";

                Query += ", @pWORKORDER_NO_OG = '" + txtSWORKORDER_NO_OG.Text + "'";
                Query += ", @pMAKEORDER_NO    = '" + txtSMAKEORDER_NO.Text + "'";
                Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";

                if (rdoLEVEL1.Checked == true)
                    Query += ", @pLEVEL = '1'";
                else
                    Query += ", @pLEVEL = '0'";
                Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataView dvwData = null;
                    UIForm.TreeView.MBOPTreeView(
                        ds.Tables[0].Rows[0]["PRNT_ITEM_CD"].ToString()
                        , 0
                        , (TreeNode)null
                        , treeView1
                        , ds
                        , dvwData
                        , imageList2
                        , ds.Tables[0].Rows[0]["FIGNO"].ToString()
                        , "W"); //WORKORDER NO, MAKEORDER NO 추가 트리뷰

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

        #region PBB101_Load
        private void PBB101_Load(object sender, System.EventArgs e)
        {
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P033', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            SystemBase.ComboMake.C1Combo(cboPROCESS_CD, "usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// BOM TYPE
            dtpSVALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true);
            SystemBase.Validation.GroupBox_Setting(groupBox8);	//컨트롤 필수 Setting
        }
        #endregion

        #region 품목코드 조회
        private void btnSITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtSITEM_CD.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSITEM_CD.Value = Msgs[2].ToString();
                    txtSITEM_NM.Value = Msgs[3].ToString();
                    dtpSVALID_DT.Focus();
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

            // 락 설정
            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, false);
            // 트리설정값 지정
            NEW_NODE_TAG = e.Node.Tag.ToString();
            // BOM 정보 조회
            ShowBomInfo();
        }
        #endregion

        #region 그리드 상단 팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text = Msgs[2].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = Msgs[3].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Value = Msgs[5].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Value = Msgs[21].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Value = Msgs[22].ToString();

                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = Msgs[8].ToString();
                    
                    setITEM_TYPE(e.Row);
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
        {// 행 추가
            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("P0003"), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string strSql = " usp_PBB101 'D2' ";
                    strSql += ", @pPRNT_PLANT_CD = '" + cboSPLANT_CD.SelectedValue + "' ";
                    strSql += ", @pPRNT_ITEM_CD = '" + txtITEM_CD.Text + "' ";

                    strSql += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                    strSql += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                    strSql += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                    strSql += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                    strSql += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                    strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
                    MessageBox.Show(dt.Rows[0][1].ToString());
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
                if (txtWORKORDER_NO.Text.ToString() != "")
                {
                    string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                    if (strOrderStatus != "CL")
                    {
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
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")].Value = "1";

                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목 기준수")].Text = "1";
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "EA";
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")].Text = "EA";
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목 기준수")].Text = "1";
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")].Text = "EA";
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "안전 L/T")].Text = "0";
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "LOSS 율")].Text = "0";
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")].Value = "F";

                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text = DateTime.Today.Date.ToString();
                        fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Text = "2999-12-31";

                        string strQuery = "";
                        strQuery += " usp_P_COMMON 'P172' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                        strQuery += " , @pCOM_CD = '" + txtITEM_CD.Text + "'";
                        strQuery += " , @pPLANT_CD= '" + cboSPLANT_CD.SelectedValue + "'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")].Text = dt.Rows[0]["ORDER_MFG_UNIT"].ToString(); ;
                            fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")].Text = dt.Rows[0]["ITEM_UNIT"].ToString(); ;
                        }
                    }
                    else
                        MessageBox.Show("공정상태가 진행중이거나 완료되어 추가할수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("제조오더가 없어 행을 추가할수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                if (strOrderStatus != "CL")
                {

                    string strGbn = ""; // 작업 구분자

                    string ERRCode = "WR";
                    string MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        if (NewFlg == 2)  // MASTER 수정 사항이 있을 경우
                        {
                            strGbn = "U2";

                            string strSql = " usp_PBB101 '" + strGbn + "'";

                            strSql += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                            strSql += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                            strSql += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                            strSql += ", @pWORKORDER_NO_OG = '" + txtWORKORDER_NO.Text + "'";

                            strSql += ", @pPLANT_CD = '" + CHILD_PLANT_CD.ToString() + "'";
                            strSql += ", @pITEM_CD = '" + CHILD_ITEM_CD.ToString() + "'";
                            strSql += ", @pPRNT_BOM_NO = '" + CHILD_BOM_NO.ToString() + "'";

                            strSql += ", @pBOM_DEV_USER_ID= '" + txtBOM_DEV_USR_ID.Text + "'";
                            strSql += ", @pBOM_MFG_USER_ID= '" + txtBOM_MFG_USR_ID.Text + "'";
                            strSql += ", @pBOM_QUR_USER_ID= '" + txtBOM_QUR_USR_ID.Text + "'";
                            strSql += ", @pBOM_APP_USER_ID= '" + txtBOM_APP_USR_ID.Text + "'";
                            strSql += ", @pREV_NO= '" + txtREV_NO.Text + "'";
                            strSql += ", @pREV_DT= '" + dtpREV_DT.Text.ToString() + "'";
                            strSql += ", @pUPDT_USER_ID= '" + SystemBase.Base.gstrUserID + "'";
                            strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }

                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))// 그리드 상단 필수항목 체크
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

                                    string strSql = " usp_PBB101 '" + strGbn + "'";

                                    strSql += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                                    strSql += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                                    strSql += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";

                                    strSql += ", @pPRNT_PLANT_CD = '" + CHILD_PLANT_CD.ToString() + "'";
                                    strSql += ", @pPRNT_ITEM_CD = '" + CHILD_ITEM_CD.ToString() + "'";
                                    strSql += ", @pPRNT_BOM_NO = '" + PRNT_BOM_NO.ToString() + "'";
                                    strSql += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                                    strSql += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";

                                    strSql += ", @pCHILD_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "'";
                                    strSql += ", @pCHILD_SEQ = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text + "'";
                                    strSql += ", @pCHILD_PLANT_CD = '" + cboSPLANT_CD.SelectedValue + "'";
                                    strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                                    strSql += ", @pCHILD_ITEM_QTY = " + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목 기준수")].Value + "";
                                    strSql += ", @pCHILD_ITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value + "'";
                                    strSql += ", @pCHILD_BOM_NO = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM TYPE")].Value.ToString() + "'";
                                    strSql += ", @pPRNT_ITEM_QTY = " + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목 기준수")].Value + "";
                                    strSql += ", @pPRNT_ITEM_UNIT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")].Value + "'";

                                    strSql += ", @pNEED_QTY = " + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "요구수량")].Value + "";
                                    strSql += ", @pNEED_QTY_UNIT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")].Value + "'";

                                    strSql += ", @pMAT_SIZE = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "재료규격")].Text + "'";
                                    strSql += ", @pLOSS_RATE = " + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "LOSS 율")].Value + "";
                                    strSql += ", @pSAFETY_LT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "안전 L/T")].Text + "'";
                                    strSql += ", @pSUPPLY_TYPE = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")].Value + "'";
                                    strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                                    strSql += ", @pVALID_FROM_DT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text + "'";
                                    strSql += ", @pVALID_TO_DT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Text + "'";
                                    strSql += ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pUPDT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pROUT_NO = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT_NO")].Text + "'";
                                    strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "'";
                                    strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
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
                        ShowBomInfo();
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
                else
                    MessageBox.Show("공정상태가 진행중이거나 완료라서 수정할수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }
        #endregion

        #region NewExec() 그리드 및 그룹박스 초기화
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox8);
            SystemBase.Validation.GroupBox_Reset(gbxITEM_MASTER);
            SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, false);

            dtpSVALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            treeView1.Nodes.Clear();
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

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 프로젝트 조회 팝업
        private void btnPROJECT_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtPROJECT_NO.Text, "S1");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPROJECT_NO.Value = Msgs[3].ToString();
                    txtPROJECT_NM.Value = Msgs[4].ToString();
                    txtPROJECT_SEQ.Value = Msgs[5].ToString();

                    GROUP_CD = Msgs[6].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region BOM 설계자 조회 조회
        private void btnDEV_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region BOM확인자 조회 조회
        private void btnAPP_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM확인자"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 수정한 내용 존재여부 체크
        private void txtBOM_DEV_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg;
        }

        private void txtBOM_MFG_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg;
        }

        private void txtBOM_QUR_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg;
        }

        private void txtBOM_APP_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg;
        }

        private void txtREV_NO_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg;
        }
        #endregion

        #region BOM 정보 조회
        public void ShowBomInfo()
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

                WORKORDER_NO_OG = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                FIG_NO = NODETAG;

                string Query = " usp_PBB101 @pTYPE = 'S2'";
                Query += ", @pPROJECT_NO='" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ='" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD='" + GROUP_CD.ToString() + "'";
                Query += ", @pPRNT_BOM_NO='" + PRNT_BOM_NO.ToString() + "'";
                Query += ", @pPLANT_CD='" + PRNT_PLANT_CD.ToString() + "'";
                Query += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO ='" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pWORKORDER_NO_OG ='" + WORKORDER_NO_OG.ToString() + "'";
                Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();
                    txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
                    cboPROCESS_CD.SelectedValue = dt.Rows[0]["ITEM_ACCT"].ToString();
                    txtITEM_SPEC.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                    dtpSTART_DT.Value = dt.Rows[0]["PLAN_START_DT"].ToString();
                    dtpEND_DT.Value = dt.Rows[0]["PLAN_COMPT_DT"].ToString();
                    txtROUT_ORD_QTY.Value = dt.Rows[0]["PRODT_ORDER_QTY"].ToString();

                    string strOrderStatus = dt.Rows[0]["ORDER_STATUS"].ToString();

                    txtORDER_STATUS.Value = strOrderStatus;

                    string strOrderText = "";
                    if (strOrderStatus == "CL") strOrderText = "CLOSE";
                    else if (strOrderStatus == "ST") strOrderText = "START";
                    else strOrderText = "RELEASE";

                    txtORDER_TEXT.Value = strOrderText;

                    txtMAKEORDER_NO.Value = dt.Rows[0]["MAKEORDER_NO"].ToString();
                    txtWORKORDER_NO.Value = dt.Rows[0]["WORKORDER_NO"].ToString();

                    txtBOM_DEV_USR_ID.Value = dt.Rows[0]["BOM_DEV_USER_ID"].ToString();
                    txtBOM_DEV_USR_NM.Value = dt.Rows[0]["BOM_DEV_USER_NM"].ToString();
                    txtBOM_MFG_USR_ID.Value = dt.Rows[0]["BOM_MFG_USER_ID"].ToString();
                    txtBOM_MFG_USR_NM.Value = dt.Rows[0]["BOM_MFG_USER_NM"].ToString();
                    txtBOM_QUR_USR_ID.Value = dt.Rows[0]["BOM_QUR_USER_ID"].ToString();
                    txtBOM_QUR_USR_NM.Value = dt.Rows[0]["BOM_QUR_USER_NM"].ToString();
                    txtBOM_APP_USR_ID.Value = dt.Rows[0]["BOM_APP_USER_ID"].ToString();
                    txtBOM_APP_USR_NM.Value = dt.Rows[0]["BOM_APP_USER_NM"].ToString();

                    txtREV_NO.Value = dt.Rows[0]["REV_NO"].ToString();
                    dtpREV_DT.Value = dt.Rows[0]["REV_DT"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                }
                SystemBase.Validation.GroupBoxControlsLock(gbxITEM_MASTER, true);
                
                // 수정 가능한 목록만 수정가능 상태 처리
                if (dt.Rows.Count > 0)
                {
                    NewFlg = 0;
                }

                string Query2 = " usp_PBB101 @pTYPE = 'S3' ";
                Query2 += ", @pCHILD_PLANT_CD='" + CHILD_PLANT_CD.ToString() + "' ";
                Query2 += ", @pCHILD_ITEM_CD='" + CHILD_ITEM_CD.ToString() + "' ";
                Query2 += ", @pCHILD_BOM_NO='" + CHILD_BOM_NO.ToString() + "' ";
                Query2 += ", @pVALID_DT='" + dtpSVALID_DT.Text.ToString() + "' ";

                Query2 += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query2 += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query2 += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query2 += ", @pMAKEORDER_NO = '" + WORKORDER_NO_OG.ToString() + "'";
                Query2 += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                Query2 += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                Order_Check();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "BOM정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 제조오더 번호 조회
        private void btnWORKORDER_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                strBtn = "Y";
                WNDW006 pu = new WNDW006(txtSWORKORDER_NO_OG.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSWORKORDER_NO_OG.Value = Msgs[1].ToString();
                    txtSWORKORDER_NO_OG.Focus();
                    txtSMAKEORDER_NO.Value = Msgs[2].ToString();
                    txtPROJECT_NO.Value = Msgs[3].ToString();
                    txtPROJECT_NM.Value = Msgs[4].ToString();
                    txtPROJECT_SEQ.Value = Msgs[5].ToString();
                    txtSITEM_CD.Value = Msgs[6].ToString();
                    txtSITEM_NM.Value = Msgs[7].ToString();

                    GROUP_CD = SystemBase.Base.CodeName("WORKORDER_NO", "GROUP_CD", "P_WORKORDER_MASTER", Msgs[1].ToString(), " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                strBtn = "N";
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region 제품오더 번호 조회
        private void btnMAKEORDER_NO_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW008 pu = new WNDW008(txtSMAKEORDER_NO.Text, "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSMAKEORDER_NO.Value = Msgs[1].ToString();
                    txtPROJECT_NO.Value = Msgs[6].ToString();
                    txtPROJECT_NM.Value = Msgs[7].ToString();
                    txtPROJECT_SEQ.Value = Msgs[8].ToString();
                    txtSITEM_CD.Value = Msgs[9].ToString();
                    txtSITEM_NM.Value = Msgs[10].ToString();
                    txtSWORKORDER_NO_OG.Value = SystemBase.Base.CodeName("MAKEORDER_NO", "WORKORDER_NO_OG", "P_BOP_M_MASTER", Msgs[1].ToString(), " AND GROUP_CD = ITEM_CD AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }
        #endregion

        #region 스프레드 값 변경 처리
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            // 자품목 코드 컬럼
            int childItemCdCol = SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드");
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드"))
            {

                string strQuery = "";
                strQuery += " usp_P_COMMON 'P172' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                strQuery += " , @pCOM_CD = '" + fpSpread1.Sheets[0].Cells[Row, childItemCdCol].Text + "'";
                strQuery += " , @pPLANT_CD= '" + cboSPLANT_CD.SelectedValue + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0 && fpSpread1.Sheets[0].Cells[Row, childItemCdCol].Text != "")
                {
                    // 자품목정보를 조회한다.
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Value = dt.Rows[0]["ITEM_ACCT"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = dt.Rows[0]["ITEM_UNIT"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")].Value = dt.Rows[0]["ITEM_UNIT"].ToString();

                    setITEM_TYPE(Row);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")].Value = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")].Value = "";
                }
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

        #region TextBox 체인지 이벤트
        //제조오더번호 변경시
        private void txtSWORKORDER_NO_OG_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "Y") return;
                strBtn = "Y";
                if (txtSWORKORDER_NO_OG.Text != "")
                {
                    string strQuery = " usp_WNDW006 'S2', @pWORKORDER_NO = '" + txtSWORKORDER_NO_OG.Text + "' ";
                    strQuery += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        txtSMAKEORDER_NO.Value = dt.Rows[0]["MAKEORDER_NO"].ToString();
                        txtPROJECT_NO.Value = dt.Rows[0]["PROJECT_NO"].ToString();
                        txtPROJECT_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();
                        txtPROJECT_SEQ.Value = dt.Rows[0]["PROJECT_SEQ"].ToString();
                        txtSITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();
                        txtSITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
                        GROUP_CD = SystemBase.Base.CodeName("WORKORDER_NO", "GROUP_CD", "P_WORKORDER_MASTER", txtSWORKORDER_NO_OG.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSMAKEORDER_NO.Value = "";
                        txtPROJECT_NO.Value = "";
                        txtPROJECT_NM.Value = "";
                        txtPROJECT_SEQ.Value = "";
                        txtSITEM_CD.Value = "";
                        txtSITEM_NM.Value = "";
                        GROUP_CD = "";
                    }
                }
                else
                {
                    txtSMAKEORDER_NO.Value = "";
                    txtPROJECT_NO.Value = "";
                    txtPROJECT_NM.Value = "";
                    txtPROJECT_SEQ.Value = "";
                    txtSITEM_CD.Value = "";
                    txtSITEM_NM.Value = "";
                    GROUP_CD = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        private void txtSMAKEORDER_NO_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "Y") return;
                strBtn = "Y";
                if (txtSMAKEORDER_NO.Text != "")
                {
                    string strQuery = " usp_WNDW008 'S2', @pMAKEORDER_NO = '" + txtSMAKEORDER_NO.Text + "' ";
                    strQuery += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        txtPROJECT_NO.Value = dt.Rows[0]["PROJECT_NO"].ToString();
                        txtPROJECT_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();
                        txtPROJECT_SEQ.Value = dt.Rows[0]["PROJECT_SEQ"].ToString();
                        txtSITEM_CD.Value = dt.Rows[0]["GROUP_CD"].ToString();
                        txtSITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
                        txtSWORKORDER_NO_OG.Value = SystemBase.Base.CodeName("MAKEORDER_NO", "WORKORDER_NO_OG", "P_BOP_M_MASTER", txtSMAKEORDER_NO.Text, " AND GROUP_CD = ITEM_CD AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                        GROUP_CD = SystemBase.Base.CodeName("WORKORDER_NO", "GROUP_CD", "P_WORKORDER_MASTER", txtSWORKORDER_NO_OG.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    }
                    else
                    {
                        txtPROJECT_NO.Value = "";
                        txtPROJECT_NM.Value = "";
                        txtPROJECT_SEQ.Value = "";
                        txtSITEM_CD.Value = "";
                        txtSITEM_NM.Value = "";
                        GROUP_CD = "";
                        txtSWORKORDER_NO_OG.Value = "";
                    }
                }
                else
                {
                    txtPROJECT_NO.Value = "";
                    txtPROJECT_NM.Value = "";
                    txtPROJECT_SEQ.Value = "";
                    txtSITEM_CD.Value = "";
                    txtSITEM_NM.Value = "";
                    GROUP_CD = "";
                    txtSWORKORDER_NO_OG.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        //프로젝트 변경
        private void txtPROJECT_NO_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPROJECT_NO.Text != "")
                {
                    string strQuery = "SELECT DISTINCT B.PROJECT_NM, A.GROUP_CD FROM P_MPS_REGISTER A(NOLOCK), S_SO_MASTER B(NOLOCK) ";
                    strQuery += " WHERE A.PROJECT_NO = B.PROJECT_NO AND  A.PROJECT_NO = '" + txtPROJECT_NO.Text + "' AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        txtPROJECT_NM.Value = dt.Rows[0][0].ToString();
                        GROUP_CD = dt.Rows[0][1].ToString();
                    }
                    else
                    {
                        txtPROJECT_NM.Value = "";
                        GROUP_CD = "";
                    }
                }
                else
                {
                    txtPROJECT_NM.Value = "";
                    txtPROJECT_SEQ.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //품목 변경
        private void txtSITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSITEM_CD.Text != "")
                {
                    txtSITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSITEM_CD.Text, " AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSITEM_NM.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 오더상태에 따른 설정변경
        public void Order_Check()
        {
            try
            {
                string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                if (strOrderStatus == "CL")
                {
                    GridReMake();
                }
                else
                {
                    GridReMake();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "오더상태변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region GridReMake() 그리드 재정의
        public void GridReMake()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (strOrderStatus == "CL")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "자품목 기준수") + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "모품목 기준수")  + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "요구수량")  + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "미출고수량")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "재료규격")  + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "안전L/T")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "LOSS율")  + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")  + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고")  + "|3");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "자품목 기준수")  + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "모품목 기준수")  + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "요구수량")  + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생산단위")  + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "재료규격")  + "|0#" + SystemBase.Base.GridHeadIndex(GHIdx1, "안전L/T")  + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "LOSS율")  + "|0#" + SystemBase.Base.GridHeadIndex(GHIdx1, "유무상구분")  + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")  + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고")  + "|0");
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 재정의"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
