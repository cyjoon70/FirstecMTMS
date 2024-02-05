#region 작성정보
/*********************************************************************/
// 단위업무명 : M BOP 라우팅정보 수정
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-21
// 작성내용 : M BOP 라우팅정보 수정 및 관리
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

namespace PC.PBB102
{ 
    public partial class PBB102 : UIForm.FPCOMM2_2
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
        string ITEM_NM = "";
        string ROUT_NO = "";
        string MAKEORDER_NO = "";
        string WORKORDER_NO_OG = "";
        string strBtn = "N";
        string FIG_NO = "";

        int NewFlg = 0;
        string NEW_NODE_TAG = "";
        #endregion

        #region 생성자
        public PBB102()
        {
            InitializeComponent();
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

                string Query = " exec usp_PBB102 'S1'";

                Query += ", @pPROJECT_NO = '" + txtPROJECT_NO.Text + "'";
                Query += ", @pPROJECT_SEQ = '" + txtPROJECT_SEQ.Text + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD + "'";
                Query += ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'";
                Query += ", @pITEM_CD = '" + txtSITEM_CD.Text + "'";
                Query += ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'";
                Query += ", @pMAKEORDER_NO = '" + txtSMAKEORDER_NO.Text + "'";
                Query += ", @pWORKORDER_NO_OG = '" + txtSWORKORDER_NO_OG.Text + "'";
                Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                if (rdoLEVEL1.Checked == true)
                    Query += ", @pLEVEL = '1'";
                else
                    Query += ", @pLEVEL = '0'";

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
                        , ds.Tables[0].Rows[0]["FIGNO"].ToString()
                        , "W"); //WORKORDER NO, MAKEORDER NO 추가 트리뷰

                    treeView1.Focus();
                    treeView1.ExpandAll();
                }
                else
                {
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

        #region PBB102_Load
        private void PBB102_Load(object sender, System.EventArgs e)
        {
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P001', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P002', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "생산단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "소요단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P028', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "MILESTONE")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "검사여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B029', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정단계")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "시간단위")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z014', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            // 외주 정보
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "통화")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'Z003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B040', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);	// 공장
            dtpSVALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");

            SystemBase.Validation.GroupBox_Setting(groupBox8);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);

            btnPROJECT.Enabled = false;
            btnSITEM_CD.Enabled = false;
            btnROUT.Enabled = false;

        }
        #endregion

        #region 품목코드 조회
        private void btnSITEM_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true);
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

            NEW_NODE_TAG = e.Node.Tag.ToString();

            // 라우팅 정보 초기화
            ROUT_NO = "";

            // 라우팅 정보 화면 출력
            ShowRoutInfo();
        }
        #endregion

        #region 행추가
        protected override void RowInsExe()
        {// 행 추가
            try
            {
                if (txtWORKORDER_NO.Text.ToString() != "")
                {
                    string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                    if (strOrderStatus != "CL")
                    {
                        if (fpSpread1.Focused == true)
                        {
                            SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0036"));
                            fpSpread2Search(fpSpread2.ActiveSheet.ActiveRowIndex);
                        }

                        int RowNum = fpSpread2.ActiveSheet.ActiveRowIndex;

                        fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "기준수량")].Text = "1";
                        fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "기준단위")].Value = "EA";
                        fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "생산량")].Value = "0";
                        fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "생산단위")].Value = "EA";
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

                        fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "시작일")].Text = DateTime.Today.Date.ToString();
                        fpSpread2.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx2, "종료일")].Text = "2999-12-31";
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

        #region 라우팅 콤보 조회
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
                string Query = " usp_PBB102 @pTYPE = 'S4'";
                Query += " ,@pPRNT_PLANT_CD='" + cboSPLANT_CD.SelectedValue + "'";
                Query += " ,@pPRNT_ITEM_CD='" + fpSpread2.Sheets[0].Cells[R, 0].Text + "'";
                Query += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query += " ,@pPROC_SEQ='" + fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text + "'";
             
                Query += ", @pROUT_NO = '" + txtROUTING_NO.Text + "' ";
                Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query += ", @pBOM_NO = '" + CHILD_BOM_NO.ToString() + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, Query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                PRNT_PLANT_CD = cboSPLANT_CD.SelectedValue.ToString();	// 라우팅 콤보
                PROC_SEQ = fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "공정")].Text;
                string strStatus = fpSpread2.Sheets[0].Cells[R, SystemBase.Base.GridHeadIndex(GHIdx2, "오더상태")].Text;

                GridReMake(strStatus);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자품목투입정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 저장
        protected override void SaveExec()
        {
            string ERRCode = "WR";
            string MSGCode = "P0000";
            string RPLMsg = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            // 자품목 등록 정보 처리
            try
            {
                for (int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[j, 0].Text;
                    string strGbn = "";

                    if (strHead == "D") strGbn = "D2";
                    else strGbn = "I2";

                    string strSql = " usp_PBB102 '" + strGbn + "'";
                    strSql += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                    strSql += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                    strSql += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                    strSql += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                    strSql += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";

                    strSql += ", @pBOM_NO = '" + CHILD_BOM_NO + "'";
                    strSql += ", @pCHILD_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "순서")].Text + "'";
                    strSql += ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                    strSql += ", @pUSER_ID = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                    strSql += ", @pPLANT_CD = '" + PRNT_PLANT_CD + "'";
                    strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "모품목코드")].Text + "'";
                    strSql += ", @pROUT_NO = '" + txtROUTING_NO.Text + "'";
                    strSql += ", @pPROC_SEQ = '" + PROC_SEQ + "'";
                    string strCheck = "N"; if (fpSpread1.Sheets[0].Cells[j, 1].Text == "True") strCheck = "Y";
                    strSql += ", @pCHECK_FLAG = '" + strCheck + "'";
                    strSql += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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

        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                if (RPLMsg != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode, RPLMsg), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #region fpSpread2_ButtonClicked 그리드 상단 버튼 클릭
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 공정조회
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드_2"))
                {
                    string strQuery = " usp_P_COMMON 'P042' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Value = Msgs[1].ToString(); //공정명

                        UIForm.FPMake.fpChange(fpSpread2, e.Row);
                    }
                }

                //부서조회
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "자원_2"))
                {
                    string strQuery = " usp_P_COMMON 'P051' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { "", "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05005", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                    pu.Width = 500;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text = Msgs[0].ToString();	//자원코드
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = Msgs[1].ToString(); //자원명
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = Msgs[2].ToString(); //작업장
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = Msgs[3].ToString();	//공정타입

                        if (Msgs[3].ToString() == "N")  // 자원이 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, e.Row,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                );
                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
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

                    WNDW002 pu = new WNDW002("P");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처")].Text = Msgs[1].ToString();
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명")].Text = Msgs[2].ToString();
                    }
                    UIForm.FPMake.fpChange(fpSpread2, e.Row);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 스프레드 붙여넣기
        private void fpSpread2_ClipboardPasting(object sender, FarPoint.Win.Spread.ClipboardPastingEventArgs e)
        {
            // 자원정보 붙여넣기 일 경우
            if (fpSpread2.Sheets[0].ActiveColumnIndex == SystemBase.Base.GridHeadIndex(GHIdx2, "자원"))
            {
                // PASTE되는 ROW, COL
                int row = fpSpread2.Sheets[0].ActiveRowIndex;
                int col = fpSpread2.Sheets[0].ActiveColumnIndex;

                IDataObject dataObject = Clipboard.GetDataObject();
                string clipVal = dataObject.GetData(DataFormats.StringFormat).ToString().Replace("\r\n", ""); // \r\n 제거

                string strQuery = "";
                strQuery += " usp_P_COMMON 'P051' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                strQuery += " , @pCOM_CD='" + clipVal + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                if (dt.Rows.Count > 0)
                {

                    // 자원정보를 조회한다.
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text = dt.Rows[0]["RES_CD"].ToString();	//자원코드
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = dt.Rows[0]["RES_DIS"].ToString();	//자원명
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = dt.Rows[0]["WORKCENTER_CD"].ToString();	//작업장
                    fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();	//공정타입

                    UIForm.FPMake.fpChange(fpSpread2, row);

                    if (dt.Rows[0]["INSIDE_FLG"].ToString() == "N")  // 자원이 외주일 경우
                    {
                        // 외주란을 활성화 시간다.
                        UIForm.FPMake.grdReMake(fpSpread2, row,
                            SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                            );
                        // 초기값 설정
                        fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                    }
                }
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox8);
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBoxControlsLock(groupBox1, false);

            txtROUTING_NO.Value = "";
            txtROUTING_NM.Value = "";


            dtpSVALID_DT.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpFROM_VALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            dtpTO_VALID_DT.Value = "2999-12-31";

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
            treeView1.Nodes.Clear();

            NewFlg = 1; // 새로등록 상태로 변경
            btnPROJECT.Enabled = false;
            btnSITEM_CD.Enabled = false;
            btnROUT.Enabled = false;
            rdoLEVEL1.Checked = true;
        }
        #endregion

        #region txtROUTING_NO_Leave
        private void txtROUTING_NO_Leave(object sender, System.EventArgs e)
        {
            ROUTING_LEAVE(false);
        }

        public void ROUTING_LEAVE(bool ROUT)
        {
            string Query = " usp_PBB102 @pTYPE = 'S2'";
            Query += ", @pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
            Query += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
            Query += ", @pROUT_NO='" + txtROUTING_NO.Text + "'";
            Query += ", @pVALID_DT='" + dtpSVALID_DT.Text + "'";
            Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
            Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
            Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
            Query += ", @pWORKORDER_NO_OG= '" + txtSWORKORDER_NO_OG.Text + "'";
            Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                fpSpread2Search(0);
            }
            else
            {
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            }

            /////////////////라우팅 정보////////////////////

            if (ROUT == true)
            {
                string Query2 = " usp_PBB102 @pTYPE = 'S5' ";
                Query2 += ", @pPROJECT_NO='" + PROJECT_NO.ToString() + "'";
                Query2 += ", @pPROJECT_SEQ='" + PROJECT_SEQ.ToString() + "'";
                Query2 += ", @pGROUP_CD='" + GROUP_CD.ToString() + "'";
                Query2 += ", @pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
                Query2 += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query2 += ", @pROUT_NO='" + txtROUTING_NO.Text + "'";
                Query2 += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query2);
                if (dt.Rows.Count > 0)
                {
                    txtITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();
                    txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
                    txtROUTING_NO.Value = dt.Rows[0]["ROUT_NO"].ToString();
                    txtROUTING_NM.Value = dt.Rows[0]["DESCRIPTION"].ToString();
                    dtpFROM_VALID_DT.Value = dt.Rows[0]["VALID_FROM_DT"].ToString();
                    dtpTO_VALID_DT.Value = dt.Rows[0]["VALID_TO_DT"].ToString();

                    if (dt.Rows[0]["MAJOR_FLG"].ToString() == "Y")
                        rdoROUTINT1.Checked = true;
                    else
                        rdoROUTINT1.Checked = false;

                }
                else
                {
                    txtROUTING_NM.Value = "";
                    dtpFROM_VALID_DT.Value = "";
                    dtpTO_VALID_DT.Value = "";

                    rdoROUTINT1.Checked = true;

                }
            }

            Order_Check();
            /////////////////라우팅 정보////////////////////

        }
        #endregion

        #region btnPROJECT_Click
        private void btnPROJECT_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBIZ_CD.Value = Msgs[1].ToString();
                    txtBIZ_NM.Value = Msgs[2].ToString();
                    txtPROJECT_NO.Value = Msgs[3].ToString();
                    txtPROJECT_NM.Value = Msgs[4].ToString();
                    txtPROJECT_SEQ.Value = Msgs[5].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 공정 설계자 조회
        private void btnDEV_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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

        #region 생산검토자 조회
        private void btnMFG_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "생산검토자"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 품질검토자 조회
        private void btnQUR_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품질검토자"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 라우팅확인자 조회
        private void btnAPP_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P140' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅확인자"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region MASTER에 수정된 항목이 있는지 상태 처리
        private void txtROU_DEV_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정항목이 있는 상태로 처리
        }

        private void txtROU_QUR_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정항목이 있는 상태로 처리
        }

        private void txtROU_APP_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정항목이 있는 상태로 처리
        }

        private void txtROU_MFG_USR_ID_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정항목이 있는 상태로 처리
        }

        private void txtREV_NO_TextChanged(object sender, System.EventArgs e)
        {
            NewFlg = NewFlg != 1 ? 2 : NewFlg; // 수정항목이 있는 상태로 처리
        }
        #endregion

        #region ROUT선택 버튼 클릭
        private void btnROUT_Click(object sender, System.EventArgs e)
        {
            try
            {

                PBB102P1 pu = new PBB102P1(PROJECT_NO, PROJECT_SEQ, GROUP_CD, CHILD_PLANT_CD, CHILD_ITEM_CD);

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

        #region ROUT정보 조회
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

                WORKORDER_NO_OG = NODETAG.Substring(0, NODETAG.IndexOf("||"));
                NODETAG = NODETAG.Substring(NODETAG.IndexOf("||") + 2, NODETAG.Length - NODETAG.IndexOf("||") - 2);

                FIG_NO = NODETAG;

                string Query = " usp_PBB102 @pTYPE = 'S2'";
                Query += " ,@pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
                Query += " ,@pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query += " ,@pVALID_DT='" + dtpSVALID_DT.Text + "'";
                Query += ", @pPROJECT_NO = '" + PROJECT_NO.ToString() + "'";
                Query += ", @pPROJECT_SEQ = '" + PROJECT_SEQ.ToString() + "'";
                Query += ", @pGROUP_CD = '" + GROUP_CD.ToString() + "'";
                Query += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                Query += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread2, Query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                    fpSpread2.ActiveSheet.AddSelection(0, 1, 1, 1);
                    fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);

                    fpSpread2Search(0);
                }
                else
                {
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                }

                /////////////////라우팅 정보////////////////////
                string Query2 = " usp_PBB102 @pTYPE = 'S5' ";
                Query2 += ", @pPROJECT_NO='" + PROJECT_NO.ToString() + "'";
                Query2 += ", @pPROJECT_SEQ='" + PROJECT_SEQ.ToString() + "'";
                Query2 += ", @pGROUP_CD='" + GROUP_CD.ToString() + "'";
                Query2 += ", @pPLANT_CD='" + CHILD_PLANT_CD.ToString() + "'";
                Query2 += ", @pITEM_CD='" + CHILD_ITEM_CD.ToString() + "'";
                Query2 += ", @pMAKEORDER_NO = '" + MAKEORDER_NO.ToString() + "'";
                Query2 += ", @pWORKORDER_NO_OG = '" + WORKORDER_NO_OG.ToString() + "'";
                Query2 += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query2);
                if (dt.Rows.Count > 0)
                {
                    txtMAKEORDER_NO.Value = dt.Rows[0]["MAKEORDER_NO"].ToString();
                    txtWORKORDER_NO.Value = dt.Rows[0]["WORKORDER_NO_OG"].ToString();

                    string strOrderStatus = dt.Rows[0]["ORDER_STATUS"].ToString();
                    txtORDER_STATUS.Value = strOrderStatus;
                    string strOrderText = "";
                    if (strOrderStatus == "CL") strOrderText = "CLOSE";
                    else if (strOrderStatus == "ST") strOrderText = "START";
                    else strOrderText = "RELEASE";

                    txtORDER_TEXT.Value = strOrderText;

                    txtWORK_QTY.Value = dt.Rows[0]["WORK_QTY"].ToString();
                    txtITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();
                    txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
                    txtROUTING_NO.Value = dt.Rows[0]["ROUT_NO"].ToString();
                    txtROUTING_NM.Value = dt.Rows[0]["DESCRIPTION"].ToString();
                    dtpFROM_VALID_DT.Value = dt.Rows[0]["VALID_FROM_DT"].ToString();
                    dtpTO_VALID_DT.Value = dt.Rows[0]["VALID_TO_DT"].ToString();

                    txtROU_DEV_USR_ID.Value = dt.Rows[0]["ROU_DEV_USER_ID"].ToString();
                    txtROU_DEV_USR_NM.Value = dt.Rows[0]["ROU_DEV_USER_NM"].ToString();
                    txtROU_MFG_USR_ID.Value = dt.Rows[0]["ROU_MFG_USER_ID"].ToString();
                    txtROU_MFG_USR_NM.Value = dt.Rows[0]["ROU_MFG_USER_NM"].ToString();
                    txtROU_QUR_USR_ID.Value = dt.Rows[0]["ROU_QUR_USER_ID"].ToString();
                    txtROU_QUR_USR_NM.Value = dt.Rows[0]["ROU_QUR_USER_NM"].ToString();
                    txtROU_APP_USR_ID.Value = dt.Rows[0]["ROU_APP_USER_ID"].ToString();
                    txtROU_APP_USR_NM.Value = dt.Rows[0]["ROU_APP_USER_NM"].ToString();
                    txtREV_NO.Value = dt.Rows[0]["REV_NO"].ToString();
                    dtpREV_DT.Value = dt.Rows[0]["REV_DT"].ToString();
                    txtRemark.Value = dt.Rows[0]["WORK_REMARK"].ToString();

                    NewFlg = 0;

                }
                else
                {
                    txtMAKEORDER_NO.Value = "";
                    txtWORKORDER_NO.Value = "";
                    txtORDER_STATUS.Value = "";
                    txtWORK_QTY.Value = "";
                    txtITEM_CD.Value = CHILD_ITEM_CD.ToString();
                    txtITEM_NM.Value = ITEM_NM.ToString();
                    txtROUTING_NO.Value = "";
                    txtROUTING_NM.Value = "";
                    dtpFROM_VALID_DT.Value = "";
                    dtpTO_VALID_DT.Value = "2999-12-31";

                    txtROU_DEV_USR_ID.Value = "";
                    txtROU_DEV_USR_NM.Value = "";
                    txtROU_MFG_USR_ID.Value = "";
                    txtROU_MFG_USR_NM.Value = "";
                    txtROU_QUR_USR_ID.Value = "";
                    txtROU_QUR_USR_NM.Value = "";
                    txtROU_APP_USR_ID.Value = "";
                    txtROU_APP_USR_NM.Value = "";
                    txtREV_NO.Value = "";
                    dtpREV_DT.Value = "";
                    txtRemark.Value = "";
                }
                /////////////////라우팅 정보////////////////////
                ///
                SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
                //				Order_Check();

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 스프레드 값 변경 처리
        protected override void fpSpread2_ChangeEvent(int Row, int Col)
        {
            try
            {
                // 자원 컬럼
                int rouCol = SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드");
                int resCol = SystemBase.Base.GridHeadIndex(GHIdx2, "자원");

                // 공정정보 붙여넣기 일 경우
                if (fpSpread2.Sheets[0].Cells[Row, rouCol].Text != "")
                {
                    string strQuery = "";
                    strQuery += " usp_P_COMMON 'P042' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += " , @pETC='P001'";
                    strQuery += " , @pCOM_CD='" + fpSpread2.Sheets[0].Cells[Row, rouCol].Text.Trim() + "'";
                    strQuery += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

                        // 자원정보를 조회한다.
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정코드")].Text = dt.Rows[0]["MINOR_CD"].ToString();	//자원코드
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정명")].Value = dt.Rows[0]["CD_NM"].ToString();	//자원명

                        UIForm.FPMake.fpChange(fpSpread2, Row);

                    }
                }

                // 자원정보 붙여넣기 일 경우
                if (fpSpread2.Sheets[0].Cells[Row, resCol].Text != "")
                {
                    string strQuery = "";
                    strQuery += " usp_P_COMMON 'P051' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += " , @pCOM_CD='" + fpSpread2.Sheets[0].Cells[Row, resCol].Text + "'";
                    strQuery += " ,@pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {

                        // 자원정보를 조회한다.
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원")].Text = dt.Rows[0]["RES_CD"].ToString();	//자원코드
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value = dt.Rows[0]["RES_DIS"].ToString();	//자원명
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value = dt.Rows[0]["WORKCENTER_CD"].ToString();	//작업장
                        fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")].Value = dt.Rows[0]["INSIDE_FLG"].ToString();	//공정타입

                        UIForm.FPMake.fpChange(fpSpread2, Row);

                        if (dt.Rows[0]["INSIDE_FLG"].ToString() == "N")  // 자원이 외주일 경우
                        {
                            // 외주란을 활성화 시간다.
                            UIForm.FPMake.grdReMake(fpSpread2, Row,
                                SystemBase.Base.GridHeadIndex(GHIdx2, "외주처") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주처명") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "통화") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "외주공정단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "부가세유형") + "|1"
                                );
                            // 초기값 설정
                            fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "통화")].Value = "KRW";
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "스프레드 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 작업지시번호
        private void btnWORKORDER_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtSWORKORDER_NO_OG.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSWORKORDER_NO_OG.Value = Msgs[1].ToString();
                    txtSWORKORDER_NO_OG.Focus();
                    txtSMAKEORDER_NO.Value = Msgs[2].ToString();
                    txtPROJECT_NO.Value = Msgs[3].ToString();
                    txtPROJECT_SEQ.Value = Msgs[5].ToString();
                    txtSITEM_CD.Value = Msgs[6].ToString();
                    txtSITEM_NM.Value = Msgs[7].ToString();
                    txtBIZ_CD.Value = SystemBase.Base.CodeName("PROJECT_NO", "ENT_CD", "S_SO_MASTER", txtPROJECT_NO.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    txtBIZ_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtBIZ_CD.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    GROUP_CD = SystemBase.Base.CodeName("WORKORDER_NO", "GROUP_CD", "P_WORKORDER_MASTER", Msgs[1].ToString(), " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업지시번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 프로젝트번명 자동기입
        private void txtPROJECT_NO_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                txtPROJECT_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtPROJECT_NO.Text, " AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 변경"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 제조오더번호 자동기입
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
                        txtBIZ_CD.Value = SystemBase.Base.CodeName("PROJECT_NO", "ENT_CD", "S_SO_MASTER", txtPROJECT_NO.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                        txtBIZ_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtBIZ_CD.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
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
                        txtBIZ_NM.Value = "";
                        txtBIZ_CD.Value = "";
                        GROUP_CD = "";
                    }
                }
                else
                {
                    txtSMAKEORDER_NO.Text = "";
                    txtPROJECT_NO.Text = "";
                    txtPROJECT_NM.Text = "";
                    txtPROJECT_SEQ.Text = "";
                    txtSITEM_CD.Text = "";
                    txtSITEM_NM.Text = "";
                    txtBIZ_NM.Text = "";
                    txtBIZ_CD.Text = "";
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
        #endregion

        #region 오더상태에 따른 설정변경
        public void Order_Check()
        {
            try
            {
                string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                if (strOrderStatus == "CL")
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox1, true);
                }
                else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox1, false);
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
        public void GridReMake(string Status)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (Status == "CL")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, "1|3");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, "1|0");
                        }
                    }
                }

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    string strOrderStatus = txtORDER_STATUS.Text.ToString().Trim();

                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        if (strOrderStatus == "CL")
                        {
                            UIForm.FPMake.grdReMake(fpSpread2, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "시간단위") + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제조L/T") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "Queue 시간") + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "대기시간") + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "고정가동시간") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량") + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "기준단위") + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생산량") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "시작일자") + "|3#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간") + "|3");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread2, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "시간단위") + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "제조 L/T") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "Queue 시간") + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "설치시간") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "대기시간") + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "고정가동시간") + "|1#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량") + "|0#" + SystemBase.Base.GridHeadIndex(GHIdx1, "기준수량") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "기준단위") + "|0#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생산량") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "식작일자") + "|1#" + SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간") + "|0");
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

        #region 제품오더변경
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
                        txtBIZ_CD.Value = SystemBase.Base.CodeName("PROJECT_NO", "ENT_CD", "S_SO_MASTER", txtPROJECT_NO.Text, " AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                        txtBIZ_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtBIZ_CD.Text, " AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                        txtSWORKORDER_NO_OG.Value = SystemBase.Base.CodeName("MAKEORDER_NO", "WORKORDER_NO_OG", "P_BOP_M_MASTER", txtSMAKEORDER_NO.Text, " AND GROUP_CD = ITEM_CD  AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                        GROUP_CD = SystemBase.Base.CodeName("WORKORDER_NO", "GROUP_CD", "P_WORKORDER_MASTER", txtSWORKORDER_NO_OG.Text, " AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                    }
                    else
                    {

                        txtPROJECT_NO.Value = "";
                        txtPROJECT_NM.Value = "";
                        txtPROJECT_SEQ.Value = "";
                        txtSITEM_CD.Value = "";
                        txtSITEM_NM.Value = "";
                        txtBIZ_NM.Value = "";
                        txtBIZ_CD.Value = "";
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
                    txtBIZ_NM.Value = "";
                    txtBIZ_CD.Value = "";
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
        #endregion

        #region 제품오더클릭
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
                    txtSWORKORDER_NO_OG.Value = SystemBase.Base.CodeName("MAKEORDER_NO", "WORKORDER_NO_OG", "P_BOP_M_MASTER", Msgs[1].ToString(), " AND GROUP_CD = ITEM_CD AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                    txtBIZ_CD.Value = SystemBase.Base.CodeName("PROJECT_NO", "ENT_CD", "S_SO_MASTER", txtPROJECT_NO.Text, " AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                    txtBIZ_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtBIZ_CD.Text, " AND A.CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");

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

        
    }
}
