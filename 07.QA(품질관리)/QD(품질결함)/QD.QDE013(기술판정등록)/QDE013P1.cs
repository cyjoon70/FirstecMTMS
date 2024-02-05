using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace QD.QDE013
{
    public partial class QDE013P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strInspReqNo = "";
        string strItemCd = "";
        int iRow = 0;
        FarPoint.Win.Spread.FpSpread spd;
        private System.Windows.Forms.Button btnAddUser;
        #endregion

        #region 생성자
        public QDE013P1()
        {
            InitializeComponent();
        }

        public QDE013P1(FarPoint.Win.Spread.FpSpread spread, int Row, string InspReqNo, string ItemCd)
        {
            InitializeComponent();

            spd = spread;
            iRow = Row;
            strInspReqNo = InspReqNo;
            strItemCd = ItemCd;
        }
        #endregion

        #region Form Load 시
        private void QDE013P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "기술판정등록";

            //버튼 재정의
            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            txtInspReqNo.Value = strInspReqNo;
            txtItemCd.Value = strItemCd;
            panel2.Enabled = false;

            SearchExec();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_QDE013  @pTYPE = 'P1'";
                    strQuery += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 1, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        txtDeptCd.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "귀책부서")].Text;
                        txtWcCd.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text;
                        txtInspectorCd.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "작성자")].Text;

                        string strTdecInspYn = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "기술판정검사여부")].Text;

                        if (strTdecInspYn == "Y")
                        { rdoTdecInspYnYes.Checked = true; }
                        else
                        { rdoTdecInspYnNo.Checked = true; }

                        if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결함내용")].Text != "")
                        { txtQdefContent.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결함내용")].Value.ToString(); }
                        else
                        { txtQdefContent.Value = ""; }

                        txtWorkerCd.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text;
                        txtManagerCd.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "작업관리자")].Text;

                        if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결함원인")].Text != "")
                        { txtDcauContent.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결함원인")].Value.ToString(); }
                        else
                        { txtDcauContent.Value = ""; }

                        if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "재발방지대책")].Text != "")
                        { txtPrevContent.Value = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "재발방지대책")].Value.ToString(); }
                        else
                        { txtPrevContent.Value = ""; }

                        if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "기술판정")].Text != "")
                        { txtTdecContent.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "기술판정")].Value.ToString(); }
                        else
                        { txtTdecContent.Text = ""; }

                        //화면 Look, 버튼 설정
                        if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "품질판정등록여부")].Text == "Y")
                        {
                            txtTdecContent.Tag = ";2;;";

                            UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                        }
                        else
                        {
                            txtTdecContent.Tag = "";

                            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                        }

                        SystemBase.Validation.GroupBox_Setting(groupBox1);
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

                    string strSql = " usp_QDE013 'U1'";
                    strSql += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                    strSql += ", @pTDEC_CONTENT = '" + txtTdecContent.Text + "'";
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
                spd.Sheets[0].Cells[iRow, 14].Value = 1;

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

        #region DelExec()
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (MessageBox.Show(SystemBase.Base.MessageRtn("B0047"), "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string strSql = " usp_QDE013 'D1'";
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
                spd.Sheets[0].Cells[iRow, 14].Value = 0;

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

        #region Button Click
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            QDE013P2 myForm = new QDE013P2(strInspReqNo);
            myForm.ShowDialog();
        }
        #endregion

    }
}
