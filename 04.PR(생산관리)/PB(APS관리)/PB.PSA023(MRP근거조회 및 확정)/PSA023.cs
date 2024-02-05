#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP근거조회 및 확정
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-14
// 작성내용 : MRP근거조회 및 확정 관리
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

namespace PB.PSA023
{
    public partial class PSA023 : UIForm.FPCOMM2
    {
        string strMrpNo = "";

        public PSA023()
        {
            InitializeComponent();
        }

        public PSA023(string Div)
        {
            strMrpNo = Div;
            InitializeComponent();
        }

        #region Form Load 시
        private void PSA023_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(GridCommGroupBox1);
            SystemBase.Validation.GroupBox_Setting(GridCommGroupBox2);

            txtPlantCd.Value = SystemBase.Base.gstrPLANT_CD;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            btnConfirm.Enabled = false;
            btnUnConfirm.Enabled = false;

            if (strMrpNo != "")
            {
                txtMrpNo.Text = strMrpNo;
                optCfmFlag1.Checked = true;
                SearchExec();
            }

        }
        #endregion
        
        #region 조회 버튼 클릭시 팝업창 조회
        private void btnPlant_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtPlantCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPlantCd.Text = Msgs[0].ToString();
                    txtPlantNm.Value = Msgs[1].ToString();
                    txtPlantCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProjectNo.Text, "S1", "C");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = Msgs[5].ToString();

                    txtProjectNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtPlantCd.Text, true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMrp_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strCfmFlag = "N"; if (optCfmFlag1.Checked == true) strCfmFlag = "Y";

                string strQuery = "usp_P_COMMON 'P200' , @pCOM_NM = 'S' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                string[] strWhere = new string[] { "@pCOM_CD", "@pETC" };
                string[] strSearch = new string[] { txtMrpNo.Text, txtProjectNo.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00064", strQuery, strWhere, strSearch, new int[] { 0, 3 }, "MRP ID 조회");
                pu.Width = 800;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtMrpNo.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "MRP 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(GridCommGroupBox1);
            SystemBase.Validation.GroupBox_Reset(GridCommGroupBox2);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);


            txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;

            btnConfirm.Enabled = false;
            btnUnConfirm.Enabled = false;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }
        #endregion

        #region 조회함수
        private void Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_PSA023  'S1'";
                    strQuery = strQuery + ", @pPLANT_CD ='" + txtPlantCd.Text.ToString() + "' ";
                    strQuery = strQuery + ", @pMRP_ID ='" + txtMrpNo.Text.ToString() + "' ";
                    strQuery = strQuery + ", @pPROJECT_NO ='" + txtProjectNo.Text.ToString() + "' ";
                    strQuery = strQuery + ", @pITEM_CD ='" + txtItemCd.Text.ToString() + "' ";
                    string strCfmFlag = "N"; if (optCfmFlag1.Checked == true) strCfmFlag = "Y";
                    strQuery = strQuery + ", @pCFM_FLAG ='" + strCfmFlag.ToString() + "' ";
                    strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["MRP_DT"].ToString() != "") txtMrpDt.Value = dt.Rows[0]["MRP_DT"].ToString();
                        if (dt.Rows[0]["CFM_DT"].ToString() != "") txtConfirmDt.Value = dt.Rows[0]["CFM_DT"].ToString();
                        if (dt.Rows[0]["STOCK_YN"].ToString() == "1") chkStockYn.Checked = true;
                        if (dt.Rows[0]["ENABLE_STOCK_YN"].ToString() == "1") chkEnableStockYn.Checked = true;
                        if (dt.Rows[0]["ENABLE_DAY_YN"].ToString() == "1") chkEnableDayYn.Checked = true;
                        if (dt.Rows[0]["SAFE_STOCK_YN"].ToString() == "1") chkSafeStockYn.Checked = true;

                        if (txtConfirmDt.Text == "")
                        {
                            btnConfirm.Enabled = true;
                            btnUnConfirm.Enabled = false;
                        }
                        else
                        {
                            btnConfirm.Enabled = false;
                            btnUnConfirm.Enabled = true;
                        }

                        string strQuery1 = " usp_PSA023  'S2'";
                        strQuery1 += ", @pPLANT_CD ='" + txtPlantCd.Text.ToString() + "' ";
                        strQuery1 += ", @pPROJECT_NO ='" + txtProjectNo.Text + "' ";
                        strQuery1 += ", @pPROJECT_SEQ ='" + txtProjectSeq.Text + "' ";
                        strQuery1 += ", @pMRP_ID ='" + txtMrpNo.Text + "' ";
                        strQuery1 += ", @pITEM_CD ='" + txtItemCd.Text + "' ";
                        strQuery1 += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0);

                        if (txtConfirmDt.Text == "")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1
                                , SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|0#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일") + "|0"
                                );
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1
                                , SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|3#"
                                + SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일") + "|3"
                                );
                        }
                    }
                    else
                    {
                        SystemBase.Validation.GroupBox_Reset(GridCommGroupBox1);
                        SystemBase.Validation.GroupBox_Reset(GridCommGroupBox2);

                        UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                        UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

                        btnConfirm.Enabled = false;
                        btnUnConfirm.Enabled = false;

                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpSpread1 클릭시 근거조회(fpSpread2)
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전용품여부")].Text == "1") chkTrackingFlag.Checked = true;
                    else chkTrackingFlag.Checked = false;

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT SIZING")].Text == "") dtxtLotSizing.Value = 0;
                    else dtxtLotSizing.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT SIZING")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "최소발주량")].Text == "") dtxtMinMrpQty.Value = 0;
                    dtxtMinMrpQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "최소발주량")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "최대발주량")].Text == "") dtxtMaxMrpQty.Value = 0;
                    dtxtMaxMrpQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "최대발주량")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고정발주량")].Text == "") dtxtFixMrpQty.Value = 0;
                    dtxtFixMrpQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "고정발주량")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "올림수")].Text == "") dtxtRoundQty.Value = 0;
                    dtxtRoundQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "올림수")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "안전재고량")].Text == "") dtxtSafetyQty.Value = 0;
                    dtxtSafetyQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "안전재고량")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기발주수량")].Text == "") dtxtPurOrderQty.Value = 0;
                    dtxtPurOrderQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기발주수량")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기입고수량")].Text == "") dtxtPurRcptQty.Value = 0;
                    dtxtPurRcptQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기입고수량")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "현재고수량")].Text == "") dtxtOnHandQty.Value = 0;
                    dtxtOnHandQty.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "현재고수량")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T")].Text == "") dtxtPurLt.Value = 0;
                    dtxtPurLt.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "구매L/T")].Text.ToString();

                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간")].Text == "") dtxtRoundPerd.Value = 0;
                    dtxtRoundPerd.Value = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "올림기간")].Text.ToString();

                    string strMrpId = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP ID")].Text.ToString();
                    string strPlantCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text.ToString();
                    string strProjNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text.ToString();
                    string strMakeNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text.ToString();
                    string strMaterial = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString();

                    string strSql = " usp_PSA023  'S3'";
                    strSql += ", @pMRP_ID ='" + strMrpId + "' ";
                    strSql += ", @pPLANT_CD ='" + strPlantCd + "' ";
                    strSql += ", @pPROJECT_NO ='" + strProjNo + "' ";
                    strSql += ", @pPROJECT_SEQ ='" + strMakeNo + "' ";
                    strSql += ", @pITEM_CD ='" + strMaterial + "' ";
                    strSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strSql, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);

                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "차수"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "품목명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "규격"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "모품목코드"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "모품목명"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread2.Sheets[0].SetColumnMerge(SystemBase.Base.GridHeadIndex(GHIdx1, "투입예정일"), FarPoint.Win.Spread.Model.MergePolicy.Always);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "상세조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            string fcsStr = "";
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "P0000";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead == "U")
                        {
                            string strDate = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입일")].Text.ToString();
                            string strMrpId = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP ID")].Text.ToString();
                            string strMrpSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP SEQ")].Text.ToString();

                            fcsStr = strDate;

                            string strSql = " usp_PSA023 'U1'";
                            strSql = strSql + ", @pMRP_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value + "'";
                            strSql = strSql + ", @pMRP_PUR_REQ_DT = '" + strDate + "'";
                            strSql = strSql + ", @pMRP_ID = '" + strMrpId + "'";
                            strSql = strSql + ", @pMRP_SEQ = '" + strMrpSeq + "'";
                            strSql = strSql + ", @pUPDT_ID	 = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0019";
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Search(false);
                    UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr); //저장 후 그리드 포커스 이동
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
        }
        #endregion

        #region btnConfirm_Click() 확정로직
        private void btnConfirm_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string MrpNo = txtMrpNo.Text;

            try
            {
                //MRP [||]를 확정 하시겠습니까?
                if (MessageBox.Show(SystemBase.Base.MessageRtn("P0017", txtMrpNo.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    PSA023P1 frm = new PSA023P1(
                        SystemBase.Base.gstrLangCd,
                        SystemBase.Base.gstrCOMCD,
                        SystemBase.Base.gstrBIZCD,
                        SystemBase.Base.gstrPLANT_CD,
                        SystemBase.Base.gstrREORG_ID,
                        SystemBase.Base.gstrDEPT,
                        SystemBase.Base.gstrUserID,
                        txtMrpNo.Text
                        );
                    frm.ShowDialog();
                    if (frm.DialogResult == DialogResult.OK)
                    {
                        optCfmFlag1.Checked = true;
                        txtMrpNo.Text = MrpNo;
                        Search(false);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "MRP 확정"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region btnConfirm_Click() 확정취소로직
        private void btnUnConfirm_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string MrpNo = txtMrpNo.Text;

            //MRP [||]를 확정취소 하시겠습니까?
            if (MessageBox.Show(SystemBase.Base.MessageRtn("P0018", txtMrpNo.Text), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0002";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strQuery = "";

                    strQuery = " usp_PSA023 'C2' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pBIZ_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
                    strQuery += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                    strQuery += ", @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "' ";
                    strQuery += ", @pDEPT_CD = '" + SystemBase.Base.gstrDEPT + "' ";
                    strQuery += ", @pUPDT_ID= '" + SystemBase.Base.gstrUserID + "'";
                    strQuery += ", @pMRP_ID = '" + txtMrpNo.Text + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();

                }
                catch (Exception f)
                {
                    Trans.Rollback();
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    ERRCode = "ER";
                    MSGCode = "P0001";
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    optCfmFlag2.Checked = true;
                    txtMrpNo.Text = MrpNo;
                    Search(false);

                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 코드 입력시 코드명 자동입력
        private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlantCd.Text != "")
                {
                    txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, "  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlantNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, "  AND CO_CO='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                {
                    txtProjectSeq.Text = "";
                }
            }
            catch
            {

            }
            
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, "  AND CO_CO='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 확정여부 변경시 이벤트
        private void optCfmFlag1_CheckedChanged(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Reset(GridCommGroupBox1);
            SystemBase.Validation.GroupBox_Reset(GridCommGroupBox2);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            btnConfirm.Enabled = false;
            btnUnConfirm.Enabled = false;
        }
        #endregion
	
    }
}
