
#region 작성정보
/*********************************************************************/
// 단위업무명 : 매입일괄처리
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-10
// 작성내용 : 매입일괄처리 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using WNDW;

namespace MV.MIV003
{
    public partial class MIV003 : UIForm.FPCOMM1
    {
        public MIV003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MIV003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//화폐단위
            // 2022.01.24. hma 추가(Start): 결재상태
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            // 2022.01.24. hma 추가(End)

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpIvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpIvDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
            rdoCfm_N.Checked = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            dtpIvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpIvDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
            rdoCfm_N.Checked = true;
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strCfmYn = "";
                    if (rdoCfm_Y.Checked == true) { strCfmYn = "Y"; }
                    // 2022.02.16. hma 추가(Start): 반제대상 선택한 경우도 포함되게.
                    //else { strCfmYn = "N"; }
                    else if (rdoCfm_N.Checked == true) { strCfmYn = "N"; }
                    else { strCfmYn = "M"; }
                    // 2022.02.16. hma 추가(End)

                    string strQuery = " usp_MIV003 @pTYPE = 'S1'";
                    strQuery += ", @pIV_DT_FR = '" + dtpIvDtFr.Text + "'";
                    strQuery += ", @pIV_DT_TO = '" + dtpIvDtTo.Text + "'";
                    strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "'";
                    strQuery += ", @pPUR_ORG = '" + txtPurOrgCd.Text + "'";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDutyCd.Text + "'";
                    strQuery += ", @pIV_TYPE = '" + txtIvTypeCd.Text + "'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

                    // 2022.01.24. hma 추가(Start): 확정 상태이지만 확정취소를 할 수 없거나, 미확정 상태이지만 확정 처리를 할 수 없는 건에 대해서는 선택 항목 비활성화 처리
                    string strSlipNo = "";
                    string strCSlipNo = "", strCSlipConfirm = "", strCSlipGwStatus = "", strMinusConfirm = "";
                    string strMSlipNo = "", strMSlipConfirm = "", strMSlipGwStatus = "";

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        strSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;
                        strCSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표번호")].Text;
                        strCSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표승인")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Text == "")
                            strCSlipGwStatus = "";
                        else
                            strCSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Value.ToString();
                        strMinusConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제승인")].Text;
                        strMSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표번호")].Text;
                        strMSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표승인")].Text;
                        strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text == "")
                            strMSlipGwStatus = "";
                        else
                            strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Value.ToString();

                        // 확정상태인 경우 결재상태가 상신대기/반려/승인(반제승인Y) 상태이면 확정취소 가능.
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True")
                        {
                            if ((strSlipNo != "" && strCSlipNo == "") ||
                                ((strCSlipNo != "") &&
                                 (strCSlipGwStatus == "READY" || strCSlipGwStatus == "REJECT" ||            // 확정전표결재상태가 상신대기/반려 이거나
                                  (strCSlipGwStatus == "APPR" && strMinusConfirm == "Y"))))                 // 확정전표결재상태가 승인이면서 반제승인이 Y인 경우
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|0"      // 일반
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|3"    // 2022.01.28. hma 추가: 확정건은 반제취소 버튼 비활성화
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                       SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3"       // 읽기전용이면서 필수항목에서 제외
                                       + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|3"     // 2022.01.28. hma 추가: 확정건은 반제취소 버튼 비활성화
                                   );
                            }
                        }
                        else
                        {
                            // 미확정 상태인 경우
                            // 미확정상태이지만 반제전표 결재상태가 승인이면서 반제승인이 Y인 경우에도 확정 가능.
                            if ((strMSlipNo == "") ||
                                (strMSlipNo != "" &&
                                 (strMSlipGwStatus == "APPR" && strMinusConfirm == "Y")))
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|0"
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "확정") + "|3"
                                    );
                            }

                            // 미확정건이지만 반제전표가 생성되어 결재상태가 상신대기/반려이면 반제취소 버튼 활성화하여 반제전표 삭제하고 승인 상태로 변경되게.
                            if (strMSlipNo != "" &&
                                 (strMSlipGwStatus == "READY" || strMSlipGwStatus == "REJECT"))
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|0"
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|3"
                                    );
                            }
                        }
                    }
                    // 2022.01.24. hma 추가(End)
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

        #region SaveExec()
        protected override void SaveExec()
        {
            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strPreConfirm = "";          // 2022.01.24. hma 추가: 이전확정상태
                    for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
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

                            string chkCfm = "";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정")].Text == "True") { chkCfm = "Y"; }
                            else { chkCfm = "N"; }

                            // 2022.01.24. hma 추가(Start): 이전확정상태와 입력한 확정상태가 같은 건만 처리하도록 함.
                            strPreConfirm = "";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정상태")].Text == "True") { strPreConfirm = "Y"; }
                            else { strPreConfirm = "N"; }

                            if (chkCfm != strPreConfirm)
                            {
                            // 2022.01.24. hma 추가(End)
                                string strSql = "usp_MIV003 @pTYPE = '" + strGbn + "'";
                                strSql += ", @pCONFIRM_YN = '" + chkCfm + "'";
                                strSql += ", @pIV_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입번호")].Text + "'";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	//ER 코드 Return시 점프
                            }
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
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

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 조회조건 팝업
        //구매조직
        private void btnPurOrg_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='COMM_POP', @pSPEC1 = 'M001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurOrgCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매조직 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurOrgCd.Text = Msgs[0].ToString();
                    txtPurOrgNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매조직 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //매입형태
        private void btnIvType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1 = 'IV_TYPE', @pSPEC2 = 'IV_TYPE_NM', @pSPEC3 = 'M_IV_TYPE', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIvTypeCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "매입형태조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIvTypeCd.Text = Msgs[0].ToString();
                    txtIvTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매입형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //구매담당자
        private void btnPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDutyCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDutyCd.Text = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //공급처
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //구매조직
        private void txtPurOrgCd_TextChanged(object sender, System.EventArgs e)
        {
            txtPurOrgNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPurOrgCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M001' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //매입형태
        private void txtIvTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            txtIvTypeNm.Value = SystemBase.Base.CodeName("IV_TYPE", "IV_TYPE_NM", "M_IV_TYPE", txtIvTypeCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //구매담당자
        private void txtPurDutyCd_TextChanged(object sender, System.EventArgs e)
        {
            txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDutyCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        //공급처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion


        #region 전체선택 전체 취소 버튼 클릭
        private void btnSelectAll_Click(object sender, System.EventArgs e)
        {
            int col = SystemBase.Base.GridHeadIndex(GHIdx1, "확정");

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, col].Text != "True" && fpSpread1.Sheets[0].Cells[i, col].Locked == false)
                {
                    fpSpread1.Sheets[0].Cells[i, col].Value = 1;
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                }
            }
        }

        private void btnSelectCancel_Click(object sender, System.EventArgs e)
        {
            int col = SystemBase.Base.GridHeadIndex(GHIdx1, "확정");

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, col].Text == "True" && fpSpread1.Sheets[0].Cells[i, col].Locked == false)
                {
                    fpSpread1.Sheets[0].Cells[i, col].Value = 0;
                    fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                }
            }
        }
        #endregion


        // 2022.01.24. hma 추가(Start)
        #region fpSpread1_ButtonClicked() 그리드 버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소"))
                {
                    // 2022.02.16. hma 추가: 메시지 확인
                    DialogResult dsMsg = MessageBox.Show("반제취소 처리하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dsMsg == DialogResult.Yes)
                    {
                        string strIvNo = "";
                        string strCSlipNo = "", strCSlipConfirm = "", strCSlipGwStatus = "", strMinusConfirm = "";
                        string strMSlipNo = "", strMSlipConfirm = "", strMSlipGwStatus = "";

                        int i;
                        i = e.Row;

                        strIvNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입번호")].Text;
                        strCSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표번호")].Text;
                        strCSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표승인")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Text == "")
                            strCSlipGwStatus = "";
                        else
                            strCSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Value.ToString();
                        strMinusConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제승인")].Text;
                        strMSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표번호")].Text;
                        strMSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표승인")].Text;
                        strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text == "")
                            strMSlipGwStatus = "";
                        else
                            strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Value.ToString();

                        // 확정전표 결재상태가 승인이고, 반제전표 결재상태가 상신대기/반려인 경우 반제전표 삭제 가능하게 함.
                        if (strCSlipGwStatus == "APPR" && strMinusConfirm == "Y" && strMSlipNo != "" &&
                            (strMSlipGwStatus == "READY" || strMSlipGwStatus == "REJECT"))
                        {
                            MinusSlipDelete(strIvNo);
                        }
                        else
                        {
                            MessageBox.Show("확정전표 결재상태가 승인이고 반제전표 결재상태가 상신대기/반려인 경우 반제취소 가능합니다.");
                            return;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion

        #region MinusSlipDelete(): 해당 매입번호에 대한 반제전표 삭제 처리
        private void MinusSlipDelete(string IV_NO)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MIV003  'D1'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pIV_NO = '" + IV_NO + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
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
        }
        #endregion
        // 2022.01.24. hma 추가(End)

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {

        }
        #endregion
    }
}
