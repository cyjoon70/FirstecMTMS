#region 작성정보
/*********************************************************************/
// 단위업무명 : 일반경비품목별배부
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-04
// 작성내용 : 일반경비품목별배부 및 관리
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

namespace CZ.CZA020
{
    public partial class CZA020 : UIForm.FPCOMM2
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public CZA020()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CZA020_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            //기타 세팅
            dtpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            dtpDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strDiv = "";
                    if (rdoY.Checked == true) strDiv = "Y";
                    else if (rdoN.Checked == true) strDiv = "N";

                    string strQuery = " usp_CZA020 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pDT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pDT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pDIVISION_YN ='" + strDiv + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);
                    fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                    fpSpread1.Sheets[0].RowCount = 0;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                int row = 0;
                try
                {
                    string strIvNo = ""; ;
                    string strIvSeq = "";
                    string strMDivYn = "N";

                    /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                    //그리드 상단 필수 체크
                    if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
                    {
                        row = fpSpread2.Sheets[0].ActiveRowIndex;
                        string strDt = fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "매입일자")].Text;
                        strIvNo = fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호")].Text;
                        strIvSeq = fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text;
                        string strProjNo = fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
                        string strPorjSeq = fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text;
                        string strCostElement = fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "경비항목코드")].Text;
                        decimal amt = Convert.ToDecimal(fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "경비금액")].Value);
                        string strDivYn = "";
                        decimal sum = 0;
                        int chk = 0;

                        //--경비금액과 디테일 합계금액 일치하느지 check!
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text.Trim() == "" ||
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value.ToString() == "0")
                                {
                                    MSGCode = "배부체크된 것은 금액을 필수로 입력해야 됩니다!";
                                    Trans.Rollback(); goto Exit;
                                }
                                sum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);

                                chk++;
                            }
                        }

                        if (amt != sum && chk > 0)
                        {
                            MSGCode = "금액의 합이 경비금액과 일치해야 합니다!";
                            Trans.Rollback(); goto Exit;

                        }

                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";

                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    default: strGbn = ""; break;
                                }
                                if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                                {
                                    strDivYn = "Y";
                                    strMDivYn = "Y";
                                }
                                else strDivYn = "N";

                                string strSql = " usp_CZA020 '" + strGbn + "'";
                                strSql += ", @pIV_NO = '" + strIvNo + "' ";
                                strSql += ", @pIV_SEQ = '" + strIvSeq + "' ";
                                strSql += ", @pPROJECT_NO = '" + strProjNo + "' ";
                                strSql += ", @pPROJECT_SEQ = '" + strPorjSeq + "' ";
                                strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주품목")].Text + "' ";
                                strSql += ", @pINPUT_DT = '" + strDt + "' ";
                                strSql += ", @pCOST_ELEMENT = '" + strCostElement + "' ";
                                if (strDivYn == "N")
                                    strSql += ", @pEXPENSE_AMT = '0' ";
                                else
                                    strSql += ", @pEXPENSE_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value + "' ";
                                strSql += ", @pDIVISION_YN  = '" + strDivYn + "' ";
                                strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            }
                        }
                    }
                    else
                    {
                        Trans.Rollback();
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    Trans.Commit();

                    //--마스터 배부 저장!
                    string strSql1 = " usp_CZA020 'U2'";
                    strSql1 += ", @pIV_NO = '" + strIvNo + "' ";
                    strSql1 += ", @pIV_SEQ = '" + strIvSeq + "' ";
                    strSql1 += ", @pDIVISION_YN = '" + strMDivYn + "' ";
                    strSql1 += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);
                    ERRCode = dt1.Rows[0][0].ToString();
                    if (ERRCode == "ER")
                        MSGCode = dt1.Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                if (MSGCode != "")
                {
                    if (ERRCode == "OK")
                    {
                        SearchExec();
                        fpSpread2.Sheets[0].SetActiveCell(row, 1);
                        fpSpread2.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Center, FarPoint.Win.Spread.HorizontalPosition.Center);
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


        }
        #endregion

        #region fpSpread2_CellClick
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {

            string strIvNo = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호")].Text;
            string strIvSeq = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text;
            string strProjNo = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;
            string strPorjSeq = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "차수")].Text;
            //상세정보조회
            SubSearch(strIvNo, strIvSeq, strProjNo, strPorjSeq);
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode1, string strCode2, string strCode3, string strCode4)
        {
            string strQuery = " usp_CZA020  'S2'";
            strQuery = strQuery + ", @pIV_NO ='" + strCode1 + "'";
            strQuery = strQuery + ", @pIV_SEQ ='" + strCode2 + "'";
            strQuery = strQuery + ", @pPROJECT_NO ='" + strCode3 + "'";
            strQuery = strQuery + ", @pPROJECT_SEQ ='" + strCode4 + "'";
            strQuery = strQuery + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                int idx1 = SystemBase.Base.GridHeadIndex(GHIdx1, "배부");
                int idx2 = SystemBase.Base.GridHeadIndex(GHIdx1, "금액");

                if (fpSpread1.Sheets[0].Cells[i, idx1].Text == "True")
                    UIForm.FPMake.grdReMake(fpSpread1, i, idx2 + "|1");
                else
                    UIForm.FPMake.grdReMake(fpSpread1, i, idx2 + "|0");

            }
        }
        #endregion

        #region  프로젝트
        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text, "N");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtProject_No_TextChanged(object sender, EventArgs e)
        {
            txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        #endregion

        #region 배부체크시 필수체크표시
        //		private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        //		{
        //			int idx1 = SystemBase.Base.GridHeadIndex(GHIdx1,"배부");
        //			int idx2 = SystemBase.Base.GridHeadIndex(GHIdx1,"금액");
        //			for(int i=0; i<fpSpread1.Sheets[0].Rows.Count;i++)
        //			{
        //				if(fpSpread1.Sheets[0].Cells[i,idx1].Text == "True")
        //					UIForm.FPMake.grdReMake(fpSpread1, i, idx2 + "|1");
        //				else
        //					UIForm.FPMake.grdReMake(fpSpread1, i, idx2 + "|0");
        //			}
        //		}
        #endregion

        #region fpSpread1_ChangeEvent
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            int idx1 = SystemBase.Base.GridHeadIndex(GHIdx1, "배부");
            int idx2 = SystemBase.Base.GridHeadIndex(GHIdx1, "금액");

            if (Column == idx1)
            {
                if (fpSpread1.Sheets[0].Cells[Row, idx1].Text == "True")
                    UIForm.FPMake.grdReMake(fpSpread1, Row, idx2 + "|1");
                else
                    UIForm.FPMake.grdReMake(fpSpread1, Row, idx2 + "|0");
            }

        }
        #endregion

        #region Activated, Deactivated
        private void CZA020_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpDtFr.Focus();
        }

        private void CZA020_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
