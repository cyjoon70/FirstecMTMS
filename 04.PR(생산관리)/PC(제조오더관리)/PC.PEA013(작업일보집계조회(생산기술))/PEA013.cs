#region 작성정보
/*********************************************************************/
// 단위업무명:  작업일보집계조회(생산기술)
// 작 성 자  :  한 미 애
// 작 성 일  :  2019-05-03
// 작성내용  :  TOUCH실적건에 대한 작업일보 공수 및 표준공수를 집계하여 조회한다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :  
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

namespace PC.PEA013
{
    public partial class PEA013 : UIForm.FPCOMM1
    {
        #region 생성자
        public PEA013()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void PEA013_Load(object sender, System.EventArgs e)
        {
            // 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            // 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboDirIndir, "usp_B_COMMON @pType='COMM', @pCODE = 'P062', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            // 기타 세팅
            // 작업일자: 현재일자로 From~To에 지정
            dtpWorkDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            // 목표완료일: -3개월 이전일자 ~ 현재일자로 From~To 일자 지정
            dtpComptDtFR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0, 10);
            dtpComptDtTO.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region 조회조건 팝업
        //작업자
        private void btnWorkDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P054' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";    // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };        // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDuty.Text, "" };         // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.Width = 600;
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDuty.Value = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDuty.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작업장
        private void btnWcCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"; // 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };        // 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWcCd.Text, "" };             // 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWcCd.Value = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                    txtWcCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
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
        #endregion

        #region 조회조건 TextChanged
        //작업자
        private void txtWorkDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWorkDuty.Text != "")
                {
                    txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWorkDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWcCd.Text != "")
                {
                    txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWcNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //품목코드
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
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

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpWorkDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpWorkDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strStatus = "";
                string strDataType = "";
                string strCloseY = "";

                string strDirIndir = Convert.ToString(cboDirIndir.SelectedValue);

                if (rdoAll.Checked == true) { strStatus = "A"; }
                else if (rdoING.Checked == true) { strStatus = "I"; }
                else { strStatus = "C"; }

                if (strDirIndir == "") { strDataType = "A"; }
                else if (strDirIndir == "D") { strDataType = "D"; }
                else { strDataType = "I"; }

                strCloseY = "N";
                if (chkCloseY.Checked == true)      // 마감건만 조회
                    strCloseY = "Y";

                string strQuery = " usp_PEA013  @pTYPE = 'S1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pH_RES_CD= '" + txtWorkDuty.Text + "'";
                strQuery += ", @pWC_CD = '" + txtWcCd.Text + "'";
                strQuery += ", @pWORK_DT_FR = '" + dtpWorkDtFr.Text + "'";
                strQuery += ", @pWORK_DT_TO = '" + dtpWorkDtTo.Text + "'";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                strQuery += ", @pDATA_TYPE = '" + strDataType + "'";
                strQuery += ", @pM_RES_CD = '" + txtMResCd.Text + "'";
                strQuery += ", @pSTATUS = '" + strStatus + "'";
                strQuery += ", @pPLAN_COMPT_DT_FR = '" + dtpComptDtFR.Text + "'";
                strQuery += ", @pPLAN_COMPT_DT_TO = '" + dtpComptDtTO.Text + "'";               
                strQuery += ", @pCLOSE_Y = '" + strCloseY + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                decimal dWorkHourSum = 0;
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        dWorkHourSum += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총실동시간")].Value);
                    }
                    txtWorkHourSum.ReadOnly = false;
                    txtWorkHourSum.Value = (dWorkHourSum / 60).ToString();
                    txtWorkHourSum.ReadOnly = true;
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

        #region txtMResCd_TextChanged(): 설비자원 코드 변경시 이벤트 처리
        private void txtMResCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMResCd.Text != "")
                {
                    txtMResNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtMResCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtMResNm.Value = "";
                }
            }
            catch { }
        }

        private void btnMRes_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P065' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtMResCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00066", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회");
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMResCd.Text = Msgs[0].ToString();
                    txtMResNm.Value = Msgs[1].ToString();
                    txtMResCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "설비자원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1_CellDoubleClick(): 그리드에서 더블클릭시 해당 생산오더/공수/작업장/작업자에 대한 공구상세조회 팝업창을 띄워준다
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            string strStartDt = dtpWorkDtFr.Text;
            string strEndDt = dtpWorkDtTo.Text;
            string strWCCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Text.Trim();
            string strWCNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장명")].Text.Trim();
            string strWorkDuty = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자")].Text.Trim();
            string strWorkDutyNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업자명")].Text.Trim();
            string strItemCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.Trim();
            string strItemNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text.Trim();
            string strProcSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text.Trim();
            string strJobCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정작업")].Text.Trim();
            string strJobNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업명")].Text.Trim();
            string strMResCd = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "배정설비")].Text.Trim();
            string strStatus = "A";
            if (rdoING.Checked == true)
                strStatus = "I";
            else if (rdoComplete.Checked == true)
                strStatus = "C";

            string strCloseYN = "";
            if (chkCloseY.Checked == true)
                strCloseYN = "Y";
            else
                strCloseYN = "N";

            string strComptDtFr = dtpComptDtFR.Text;
            string strComptDtTo = dtpComptDtTO.Text;

            PEA013P2 form = new PEA013P2(strStartDt, strEndDt, strWCCd, strWCNm, strWorkDuty, strWorkDutyNm, strItemCd, strItemNm, 
                                        strProcSeq, strJobCd, strJobNm, strMResCd, strStatus, strCloseYN, strComptDtFr, strComptDtTo);
            //form.strSelectDt = "1000";             

            form.Width = 1300;
            form.Height = 700;

            form.ShowDialog();
        }
        #endregion
    }
}
