#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매요청확정
// 작 성 자 : 권순철
// 작 성 일 : 2013-03-27
// 작성내용 : 구매요청확정 및 관리
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

namespace MR.MRQ002
{
    public partial class MRQ002 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strBtn = "N";
        private bool form_act_chk = false;
        #endregion

        public MRQ002()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MRQ002_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            rdoPartAll.Checked = true;
            rdoTypeAll.Checked = true;
            rdoCfm_N.Checked = true;
        }
        #endregion

        #region DelExec 행 삭제
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                if ((fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "재고참조")].Text == "N"
                    && fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "발주참조")].Text == "N"     // 2017.07.10. hma 추가
                    && fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Text == "True"
                    && fpSpread1.Focused == true) ||
                    (fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "요청상태")].Text == "요청"
                    && Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고참조량")].Value) == 0)
                    && Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발주참조량")].Value) == 0)
                    UIForm.FPMake.RowRemove(fpSpread1);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            rdoPartAll.Checked = true;
            rdoTypeAll.Checked = true;
            rdoCfm_N.Checked = true;
        }
        #endregion

        #region 조회조건 팝업
        //요청부서
        private void btnReqDeptCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'D022', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReqDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00015", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "요청부서 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqDeptCd.Text = Msgs[0].ToString();
                    txtReqDeptNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "요청부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //요청자
        private void butReqId_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "요청담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnProjNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjNo.Text = Msgs[3].ToString();
                    txtProjSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjSeq_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //품목
        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("", true, txtItemCd.Text);
                pu.MaximizeBox = false;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //요청번호
        private void btnReqNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_MRQ499 @pTYPE = 'P1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "" };
                string[] strSearch = new string[] { txtReqNo.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00085", strQuery, strWhere, strSearch, new int[] { 0 }, "구매요청번호 조회");
                pu.Width = 600;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqNo.Text = Msgs[0].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }
        #endregion

        #region 조회조건 TextChanged
        //요청부서
        private void txtReqDeptCd_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
            {
                string Query = " usp_B_COMMON 'D021' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtReqReorgId.Value = dt.Rows[0][0].ToString();
                }
                else
                {
                    txtReqReorgId.Value = "";
                }

                txtReqDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtReqDeptCd.Text, " And REORG_ID = '" + txtReqReorgId.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
        }

        //요청자
        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
            {
                txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
        }

        //품목
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
                txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text.Trim(), " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strCfmYn = "";
                    if (rdoCfm_Y.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoCfm_N.Checked == true) { strCfmYn = "N"; }

                    string strReqPart = "";
                    if (rdoMpr.Checked == true) { strReqPart = "M"; }
                    else if (rdoSpr.Checked == true) { strReqPart = "S"; }

                    string strReqType = "";
                    if (rdoMrp.Checked == true) { strReqType = "M"; }
                    else if (rdoManual.Checked == true) { strReqType = "E"; }
                    else if (rdoP.Checked == true) { strReqType = "P"; }

                    string strQuery = " usp_MRQ002  @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "' ";
                    strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "' ";
                    strQuery += ", @pREQ_PART = '" + strReqPart + "' ";
                    strQuery += ", @pREQ_TYPE = '" + strReqType + "' ";
                    strQuery += ", @pREQ_ID = '" + txtUserId.Text + "' ";
                    strQuery += ", @pREQ_DEPT_CD = '" + txtReqDeptCd.Text.Trim() + "' ";
                    strQuery += ", @pREQ_REORG_ID = '" + txtReqReorgId.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjSeq.Text.Trim() + "' ";
                    strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                    strQuery += ", @pREQ_NO = '" + txtReqNo.Text.Trim() + "' ";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text.Trim() + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 3);
                    fpSpread1.Sheets[0].RowCount = 0;

                    if (fpSpread2.Sheets[0].RowCount > 0 && rdoCfm_Y.Checked == true) Set_ReMake();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        private void Set_ReMake()
        {
            for (int i = 0; i < fpSpread2.ActiveSheet.Rows.Count; i++)
            {
                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "재고참조")].Text == "Y"
                    || fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발주참조")].Text == "Y"          // 2017.07.10. hma 추가
                    || Convert.ToInt16(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "상태")].Text) >= 1)
                    UIForm.FPMake.grdReMake(fpSpread2, i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정") + "|3");
                else
                    UIForm.FPMake.grdReMake(fpSpread2, i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정") + "|0");
            }

        }
        #endregion

        #region fpSpread1 조회
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = "";
                    strQuery = "   usp_MRQ002 @pTYPE = 'S2'";
                    strQuery += ",            @pSCH_NO = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "SCH_NO")].Text + "' ";
                    strQuery += ",            @pSCH_ID = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "SCH_ID")].Text + "' ";
                    strQuery += ",            @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string Query1 = "";
                            Query1 = " SELECT 1 FROM P_MRP_RESULT_DETAIL(NOLOCK) WHERE MAKEORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            DataTable dt1 = new DataTable();
                            dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);

                            if (dt1.Rows.Count > 0)
                            {
                                fpSpread1.Sheets[0].Cells[i, 0, i, fpSpread1.Sheets[0].Columns.Count - 1].ForeColor = Color.Blue;
                                fpSpread1.Sheets[0].Cells[i, 1].Locked = true;
                            }

                            string Query = "";
                            Query = " SELECT 1 FROM P_WORKORDER_MASTER(NOLOCK) WHERE MAKEORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "' AND ISNULL(REPORT_DT,'') <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            DataTable dt = new DataTable();
                            dt = SystemBase.DbOpen.NoTranDataTable(Query);

                            if (dt.Rows.Count > 0)
                            {
                                fpSpread1.Sheets[0].Cells[i, 0, i, fpSpread1.Sheets[0].Columns.Count - 1].ForeColor = Color.Red;
                                fpSpread1.Sheets[0].Cells[i, 1].Locked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 그리드 저장 로직
        protected override void SaveExec()
        {

            //if (UIForm.FPMake.FPUpCheck(fpSpread2, false) == true || UIForm.FPMake.FPUpCheck(fpSpread1, false) == true) 
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true || SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, "fpSpread2", false) == true)// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                this.Cursor = Cursors.WaitCursor;

                try
                {

                    for (int i = 0; i < fpSpread2.ActiveSheet.Rows.Count; i++)
                    {
                        string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            string chkCfm = "";
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Text == "True") { chkCfm = "Y"; }
                            else { chkCfm = "N"; }

                            string strSql = " usp_MRQ002";
                            strSql = strSql + " @pType = '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pREQ_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text + "'";
                            strSql = strSql + ", @pCONFIRM_YN  = '" + chkCfm + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    //Detail정보를 모두 삭제할 경우 삭제할 수 없다.
/*                    if (DelCheck() == false)
                    {
                        MSGCode = "전체삭제는 이 메뉴에서 할 수 없습니다. 확정취소하고 구매요청에서 삭제하세요!";
                        ERRCode = "ER";
                        Trans.Rollback(); goto Exit;
                    }*/
                    //if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
                    if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true))// 그리드 필수항목 체크 
                    {
                        for (int j = 0; j < fpSpread1.ActiveSheet.Rows.Count; j++)
                        {
                            string strHead1 = fpSpread1.Sheets[0].RowHeader.Cells[j, 0].Text;
                            string strGbn1 = "";
                            int act_row = fpSpread2.Sheets[0].ActiveRowIndex;
                            if (strHead1.Length > 0)
                            {
                                switch (strHead1)
                                {
                                    case "U": strGbn1 = "U2"; break;
                                    case "D": strGbn1 = "D2"; break;
                                    default: strGbn1 = ""; break;
                                }

                                string strSql = " usp_MRQ002";
                                strSql = strSql + " @pType = '" + strGbn1 + "'";
                                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                                strSql = strSql + ", @pREQ_NO = '" + fpSpread2.Sheets[0].Cells[act_row, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text + "'";
                                strSql = strSql + ", @pREQ_SEQ = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "'";
                                strSql = strSql + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                                strSql = strSql + ", @pREQ_QTY = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value + "'";
                                strSql = strSql + ", @pDELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "요청납기일")].Text + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            }
                        }
                    }
                    Trans.Commit();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0019";
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 선택
        private void btnSelectAll_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Text != "True"
                    && fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Locked == false)
                {
                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Value = 1;
                    UIForm.FPMake.fpChange(fpSpread2, i);
                }
            }
        }

        private void btnSelectCancel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Text == "True"
                    && fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Locked == false)
                {
                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Value = 0;
                    UIForm.FPMake.fpChange(fpSpread2, i);
                }
            }
        }

        #endregion

        #region fpSpread2 CellClick
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "확정")) UIForm.FPMake.fpChange(fpSpread2, e.Row);

            try
            {
                string strQuery = " usp_MRQ002  'S2'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pREQ_NO ='" + fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "요청번호")].Text + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 3);
                if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "재고참조")].Text == "N"
                    && fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발주참조")].Text == "N"      // 2017.07.10. hma 추가
                    && fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "확정")].Text == "True")
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        DataTable dt1 = null;
                        dt1 = SystemBase.DbOpen.NoTranDataTable("SELECT ITEM_ACCT FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        // 소모품일 경우 수정못하게 막는다.
                        if (dt1.Rows[0][0].ToString() == "Z1" || Convert.ToInt32(dt1.Rows[0][0]) >= 70)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청납기일") + "|3");
                        }
                        else
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청상태")].Text == "요청")
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청납기일") + "|1");
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {

                        DataTable dt2 = null;
                        dt2 = SystemBase.DbOpen.NoTranDataTable("SELECT ITEM_ACCT FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        // 소모품일 경우 수정못하게 막는다.
                        if (dt2.Rows[0][0].ToString() == "Z1" || Convert.ToInt32(dt2.Rows[0][0]) >= 70)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청납기일") + "|3");
                        }
                        else
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청상태")].Text == "요청" 
                                && Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고참조량")].Value) == 0)
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "요청납기일") + "|1");
                        }
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region MRQ002_Activated
        private void MRQ002_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpReqDtFr.Focus();
        }

        private void MRQ002_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
