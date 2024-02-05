#region 작성정보
/*********************************************************************/
// 단위업무명 : 내국작업신청서
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-12
// 작성내용 : 내국작업신청서 및 관리
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
using System.Text.RegularExpressions;

namespace IBC.IBFC01U
{
    public partial class IBFC01U : UIForm.FPCOMM1
    {
        #region 생성자
        public IBFC01U()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void IBFC01U_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            rdoNew.Checked = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0; 
            rdoNew.Checked = true;
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[6];

            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            string RptName = @"Report\" + "IBFC21P.rpt";
            if (rdoNew.Checked == true) RptParmValue[0] = "R1";
            else
            {
                if (rdoClose.Checked == true) RptParmValue[0] = "R2";
                else RptParmValue[0] = "R3";
            }

            RptParmValue[1] = dtpSoFrDt.Text;
            RptParmValue[2] = dtpSoToDt.Text;
            RptParmValue[3] = dtpReqDlvyFrDt.Text;
            RptParmValue[4] = dtpReqDlvyToDt.Text;
            RptParmValue[5] = SystemBase.Base.gstrCOMCD;

            //   crpPrint.Formulas(0) = "Company = '" & gstrCOMNM & "'"
            //string[] FormulaField = new string[]{"Company = " + SystemBase.Base.gstrCOMNM };	// Formula
            //string[] FormulaField = new string[]{SystemBase.Base.gstrCOMNM };	// Formula

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", null, null, RptName, RptParmValue);	//공통크리스탈 11버전

            frm.ShowDialog();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {

                try
                {
                    string strQuery = " usp_IBFC01U  ";
                    if (rdoNew.Checked == true) strQuery += " S11,";
                    else
                    {
                        if (rdoClose.Checked == true) strQuery += " S12,";
                        else strQuery += " S13,";
                    }
                    strQuery = strQuery + " @pSO_DT_FR = '" + dtpSoFrDt.Text + "',";
                    strQuery = strQuery + " @pSO_DT_TO = '" + dtpSoToDt.Text + "',";
                    strQuery = strQuery + " @pREQ_DLVY_DT_FR = '" + dtpReqDlvyFrDt.Text + "',";
                    strQuery = strQuery + " @pREQ_DLVY_DT_TO = '" + dtpReqDlvyToDt.Text + "',";
                    strQuery = strQuery + " @pREQUEST_NO = '" + txtNO.Text + "',";
                    strQuery = strQuery + " @pBP_CD = '" + txtBPCd.Text + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                    fpSpread1.Sheets[0].SetColumnAllowAutoSort(6, 2, true);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }

            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) // 그리드 상단 필수항목 체크
            {
                string ERRCode, MSGCode = "", temp = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
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
                                case "U": strGbn = "U1"; break;   //수정
                                case "D": strGbn = "D1"; break;   //삭제
                                case "I": strGbn = "I1"; break;   //입력
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_IBFC01U '" + strGbn + "'";
                            strQuery = strQuery + ", @pWORK_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text + "'";
                            strQuery = strQuery + ", @pREQUEST_NO= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국작업신청번호")].Text + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계약번호(T/R)")].Text + "'";
                            strQuery = strQuery + ", @pBP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주문처코드")].Text + "'";
                            strQuery = strQuery + ", @pBP_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주문처명")].Text + "'";
                            strQuery = strQuery + ", @pSO_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작일")].Text + "'";
                            strQuery = strQuery + ", @pREQ_DLVY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료일")].Text + "'";
                            strQuery = strQuery + ", @pPERMISSION_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "허가일")].Text + "'";
                            strQuery = strQuery + ", @pFINISH_PERMISSION_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국작업허가번호")].Text + "'";
                            strQuery = strQuery + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업의 종류")].Text.ToString().Replace("'", "''") + "'";
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            temp = ds.Tables[0].Rows[0][2].ToString();
                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                    SearchExec();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "P0019";
                }
            Exit:
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));

            }
        }
        #endregion

        #region Button Click
        private void btnBP_Click(object sender, System.EventArgs e)
        {
            try
            {
                //업체별 팝업
                string strQuery = " Nusp_BF_Comm 'BF03' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtBPCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "주문처팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBPCd.Value = Msgs[0].ToString();
                    txtBPNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }

        }

        private void butNO_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " Nusp_BF_Comm 'BF25' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtNO.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP015", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "내국작업신청번호팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtNO.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void butRef_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                IBFC01P frm = new IBFC01P(fpSpread1);
                frm.ShowDialog();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region TextChanged & KeyDown Event
        private void txtBPCd_TextChanged(object sender, System.EventArgs e)
        {
            txtBPNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "MTMS_FT.dbo.B_CUST_INFO", txtBPCd.Text, "");
        }
        
        private void dtpReqDlvyToDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpSoToDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void txtBPCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpSoFrDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpReqDlvyFrDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void txtNO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }
        #endregion
    }
}
