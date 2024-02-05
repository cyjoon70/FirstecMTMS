#region 작성정보
/*********************************************************************/
// 단위업무명 : 출고요청현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-01
// 작성내용 : 출고요쳥현황 및 관리
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

namespace QD.QDE014
{
    public partial class QDE014P1 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strInspReqNo = "";
        string strItemCd = "";
        int iRow = 0;
        FarPoint.Win.Spread.FpSpread spd;
        string strDisposalCd = "";
        string strProcSeq = "";
        #endregion

        #region 생성자
        public QDE014P1()
        {
            InitializeComponent();
        }

        public QDE014P1(FarPoint.Win.Spread.FpSpread spread, int Row, string InspReqNo, string ItemCd)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
            spd = spread;
            iRow = Row;
            strInspReqNo = InspReqNo;
            strItemCd = ItemCd;
        }
        #endregion

        #region Form Load 시
        private void QDE014P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "품질판정등록";

            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크

            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose);

            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            txtInspReqNo.Value = strInspReqNo;
            txtItemCd.Value = strItemCd;
            panel2.Enabled = false;

            SearchExec();
        }
        #endregion

        #region 검토자 팝업
        private void btnExaminerCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtExaminerCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검토자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtExaminerCd.Text = Msgs[0].ToString();
                    txtExaminerNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            txtItemSpec.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_SPEC", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //귀책부서
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            txtDeptNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDeptCd.Text, " AND MAJOR_CD = 'Q026'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작성자
        private void txtInspectorCd_TextChanged(object sender, EventArgs e)
        {
            txtInspectorNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInspectorCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작업자
        private void txtWorkerCd_TextChanged(object sender, EventArgs e)
        {
            txtWorkerNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkerCd.Text, " AND RES_KIND = 'L' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작업관리자
        private void txtManagerCd_TextChanged(object sender, EventArgs e)
        {
            txtManagerNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtManagerCd.Text, " AND RES_KIND = 'L' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //검토자
        private void txtExaminerCd_TextChanged(object sender, EventArgs e)
        {
            txtExaminerNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtExaminerCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_QDE014  @pTYPE = 'P1'";
                strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1, true);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    txtDeptCd.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "귀책부서")].Text;
                    txtWcCd.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Text;
                    txtInspectorCd.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "작성자")].Text;

                    string strTdecInspYn = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "기술판정검사여부")].Text;

                    if (strTdecInspYn == "Y")
                    { rdoTdecInspYnYes.Checked = true; }
                    else
                    { rdoTdecInspYnNo.Checked = true; }

                    if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "결함내용")].Text != "")
                    { txtQdefContent.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "결함내용")].Value.ToString(); }
                    else
                    { txtQdefContent.Value = ""; }

                    txtWorkerCd.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "작업자")].Text;
                    txtManagerCd.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "작업관리자")].Text;

                    if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "결함원인")].Text != "")
                    { txtDcauContent.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "결함원인")].Value.ToString(); }
                    else
                    { txtDcauContent.Value = ""; }

                    if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "재발방지대책")].Text != "")
                    { txtPrevContent.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "재발방지대책")].Value.ToString(); }
                    else
                    { txtPrevContent.Value = ""; }

                    if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "기술판정")].Text != "")
                    { txtTdecContent.Value = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "기술판정")].Value.ToString(); }
                    else
                    { txtTdecContent.Value = ""; }

                    if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "품질판정")].Text != "")
                    { txtQdecContent.Text = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "품질판정")].Value.ToString(); }
                    else
                    { txtQdecContent.Text = ""; }

                    txtExaminerCd.Text = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "검토자")].Text;

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        //						for(int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i ++)
                        //						{
                        //							if(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부적합처리코드")].Text == "22"
                        //								|| fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부적합처리코드")].Text == "23"
                        //								|| fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "부적합처리코드")].Text == "24")
                        //							{
                        //								SubSearch(i);
                        //							}
                        //						}

                        SubSearch(0);

                        //화면 Look, 버튼 설정
                        if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "처리결과등록여부")].Text == "Y")
                        {
                            txtQdecContent.Tag = ";2;;";
                            txtExaminerCd.Tag = ";2;;";
                            btnExaminerCd.Tag = ";2;;";

                            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                        }
                        else
                        {
                            txtQdecContent.Tag = "";
                            txtExaminerCd.Tag = "검토자;1;;";
                            btnExaminerCd.Tag = ";;true;;";

                            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                        }

                        SystemBase.Validation.GroupBox_Setting(groupBox1);
                    }
                    else
                    {
                        fpSpread2.Sheets[0].Rows.Count = 0;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //입력폼 필수 체트
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    if (fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "처리결과등록여부")].Text != "Y")
                    {
                        string strSql = " usp_QDE014 'U1'";
                        strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                        strSql += ", @pQDEC_CONTENT = '" + txtQdecContent.Text + "'";
                        strSql += ", @pEXAMINER_CD = '" + txtExaminerCd.Text + "'";
                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }

                    //그리드 상단 필수 체크
                    if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
                    {

                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";

                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U2"; break;
                                    default: strGbn = ""; break;
                                }

                                string strSql1 = " usp_QDE014 '" + strGbn + "'";
                                strSql1 += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "' ";

                                if (strProcSeq == "")
                                { strSql1 += ", @pPROC_SEQ = '*'"; }
                                else
                                { strSql1 += ", @pPROC_SEQ = '" + strProcSeq + "'"; }

                                strSql1 += ", @pDISPOSAL_CD = '" + strDisposalCd + "'";
                                strSql1 += ", @pMRB_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRB코드")].Text + "' ";
                                strSql1 += ", @pQTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리수량")].Value + "' ";
                                strSql1 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            }
                        }

                        //처리수량체크
                        string strSql2 = " usp_QDE014 'C1'";
                        strSql2 += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                        strSql2 += ", @pDISPOSAL_CD = '" + strDisposalCd + "'";
                        strSql2 += ", @pPROC_SEQ = '" + strProcSeq + "'";
                        strSql2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql2, dbConn, Trans);
                        ERRCode = ds2.Tables[0].Rows[0][0].ToString();

                        if (ERRCode != "OK")
                        {
                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();
                            Trans.Rollback();
                            goto Exit;
                        }	// ER 코드 Return시 점프

                    }
                    else
                    {
                        Trans.Rollback();
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
                else
                {
                    Trans.Rollback();
                    this.Cursor = Cursors.Default;
                    return;
                }
                Trans.Commit();
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = e.Message;
                //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                spd.Sheets[0].Cells[iRow, 15].Value = "Y";

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
        #endregion

        #region DeleteExec()
        protected override void DeleteExec()
        {

            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("Q0002"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string strSql = " usp_QDE014 'D1'";
                    strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프							
                }
                else
                {
                    Trans.Rollback();
                    this.Cursor = Cursors.Default;
                    return;
                }
                Trans.Commit();
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = e.Message;
                //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                spd.Sheets[0].Cells[iRow, 15].Value = 0;

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
        #endregion

        #region MRB 정보조회
        private void SubSearch(int iRow)
        {
            this.Cursor = Cursors.WaitCursor;

            strDisposalCd = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "부적합처리코드")].Text;
            strProcSeq = fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "발생공정")].Text;

            try
            {
                if (strDisposalCd == "22" || strDisposalCd == "23" || strDisposalCd == "24")
                {
                    string strQuery = " usp_QDE014  @pTYPE = 'P2'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";

                    if (strProcSeq == "")
                    { strQuery += ", @pPROC_SEQ = '*'"; }
                    else
                    { strQuery += ", @pPROC_SEQ = '" + strProcSeq + "'"; }

                    strQuery += ", @pDISPOSAL_CD = '" + fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "부적합처리코드")].Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
                }
                else
                {
                    fpSpread1.Sheets[0].Rows.Count = 0;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.				
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpSpread1 Change 이벤트
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //불량수
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "처리수량"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, Column].Text != "")
                {
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, Column].Value) < 0)
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("처리수량이 0 보다 작을수 없습니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, Column].Value = 0;
                    }
                }
            }
        }
        #endregion

        #region fpSpread2_LeaveCell
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (e.Row != e.NewRow)
            {
                try
                {
                    //상세정보조회
                    SubSearch(e.NewRow);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.				
                }
            }
        }
        #endregion

        #region Button Click
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            QDE014P2 myForm = new QDE014P2(strInspReqNo);
            myForm.ShowDialog();
        }
        #endregion
    }
}
