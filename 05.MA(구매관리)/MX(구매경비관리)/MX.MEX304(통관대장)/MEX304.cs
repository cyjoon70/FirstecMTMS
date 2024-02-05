#region 작성정보
/*********************************************************************/
// 단위업무명 : 통관대장
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-17
// 작성내용 : 통관대장
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

namespace MX.MEX304
{
    public partial class MEX304 : UIForm.FPCOMM1
    {
        public MEX304()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MEX304_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            txtBizCd.Text = SystemBase.Base.gstrBIZCD;

            dtpExpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpExpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            btnExpRefNo.Enabled = false;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅	
            txtBizCd.Text = SystemBase.Base.gstrBIZCD;

            dtpExpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpExpDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            btnExpRefNo.Enabled = false;
        }
        #endregion

        #region 조회조건 팝업
        //사업장
        private void btnBizCd_Click_1(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='TABLE_POP', @pSPEC1 = 'BIZ_CD', @pSPEC2 = 'BIZ_NM', @pSPEC3 = 'B_BIZ_PLACE', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtBizCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00086", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업장 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBizCd.Text = Msgs[0].ToString();
                    txtBizNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //경비항목
        private void btnExpCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='COMM_POP', @pSPEC1 = 'M013', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtExpCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00087", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "경비항목 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtExpCd.Text = Msgs[0].ToString();
                    txtExpNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "경비항목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //지급처
        private void btnPaymentCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtPaymentCust.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPaymentCust.Text = Msgs[1].ToString();
                    txtPaymentCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "지급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //진행구분
        private void btnExpSteps_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pType='COMM_POP', @pSPEC1 = 'M015', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtExpSteps.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00051", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "진행구분 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtExpSteps.Text = Msgs[0].ToString();
                    txtExpStepsNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "진행구분 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //프로젝트번호
        private void btnProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                    txtProjectSeq.Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        private void btnExpRefNo_Click(object sender, EventArgs e)
        {
            string strExpSteps = txtExpSteps.Text;

            //발주
            if (strExpSteps == "PO")
            {
                try
                {
                    WNDW018 pu = new WNDW018();
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Text = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (strExpSteps == "VB") // 수입선적
            {
                try
                {
                    WNDW022 pu = new WNDW022();
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Text = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (strExpSteps == "VD") // 통관
            {
                try
                {
                    WNDW023 pu = new WNDW023();
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Text = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (strExpSteps == "VL") // L/C
            {
                try
                {
                    WNDW021 pu = new WNDW021("VL");
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Text = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (strExpSteps == "VO") // LOCAL L/C
            {
                try
                {
                    WNDW021 pu = new WNDW021("VO");
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtExpRefNo.Text = Msgs[1].ToString();
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //사업장
        private void txtBizCd_TextChanged(object sender, EventArgs e)
        {
            txtBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtBizCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //경비항목
        private void txtExpCd_TextChanged(object sender, EventArgs e)
        {
            txtExpNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtExpCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M013' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //지급처
        private void txtPaymentCust_TextChanged(object sender, EventArgs e)
        {
            txtPaymentCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtPaymentCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }       
        //진행구분
        private void txtExpSteps_TextChanged(object sender, EventArgs e)
        {
            txtExpStepsNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtExpSteps.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M015' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

            if (txtExpStepsNm.Text != "")
                btnExpRefNo.Enabled = true;
            else
                btnExpRefNo.Enabled = false;
        }       
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            if (txtProjectNm.Value == "")
                txtProjectSeq.Text = "";
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_MEX304 @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pBIZ_CD = '" + txtBizCd.Text + "'";
                    strQuery += ", @pEXP_CD = '" + txtExpCd.Text + "'";
                    strQuery += ", @pPAYMENT_CUST = '" + txtPaymentCust.Text + "'";
                    strQuery += ", @pEXP_STEPS = '" + txtExpSteps.Text + "'";
                    strQuery += ", @pEXP_DT_FR = '" + dtpExpDtFr.Text + "'";
                    strQuery += ", @pEXP_DT_TO = '" + dtpExpDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pEXP_NO = '" + txtExpNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Section();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 그리드 머지
        private void Set_Section()
        {
            int iExp_Row = 0;
            int iRef_Row = 0;

            string strExp_No = "";
            string strExp_Ref_No = "";

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                //첫 행
                if (i == 0)
                {
                    strExp_No = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text;
                    strExp_Ref_No = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근거번호")].Text;
                    iExp_Row = 0;
                    iRef_Row = 0;
                }
                //마지막 행
                else if (i == fpSpread1.Sheets[0].RowCount - 1)
                {
                    //관리번호가 같을 경우
                    if (strExp_No == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text)
                    {
                        iExp_Row = iExp_Row + 1;
                        //근거번호가 같을 경우
                        if (strExp_Ref_No == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근거번호")].Text)
                        {
                            iRef_Row = iRef_Row + 1;
                        }
                    }
                    //관리번호별 셀 병합
                    Set_EXP_MURGE(i + 1, iExp_Row);
                    //근거번호별 셀 병합
                    Set_REF_MURGE(i + 1, iRef_Row);
                }
                else
                {
                    //관리번호가 같을 경우
                    if (strExp_No == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text)
                    {
                        iExp_Row = iExp_Row + 1;

                        //근거번호가 같을 경우
                        if (strExp_Ref_No == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근거번호")].Text)
                        {
                            iRef_Row = iRef_Row + 1;
                        }
                        //근거번호가 달라지면 위의 그리드 머지
                        else
                        {
                            strExp_Ref_No = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근거번호")].Text;
                            //근거번호별 셀 병합
                            Set_REF_MURGE(i, iRef_Row);
                            iRef_Row = 0;
                        }
                    }
                    //관리번호가 달라질 경우
                    else
                    {
                        //관리번호별 셀 병합
                        Set_EXP_MURGE(i, iExp_Row);
                        //근거번호별 셀 병합
                        Set_REF_MURGE(i, iRef_Row);

                        strExp_No = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].Text;
                        strExp_Ref_No = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근거번호")].Text;

                        iExp_Row = 0;
                        iRef_Row = 0;
                    }
                }
            }
        }
        #endregion

        #region 그리드 관리번호별 셀 병합
        private void Set_EXP_MURGE(int iRow, int iCnt)
        {
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "관리번호")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "진행구분명")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호지정")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호지정")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "경비항목명")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "물대포함")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "경비금액")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "경비자국금액")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국금액")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "구매담당자명")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "확정여부")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "지급처명")].RowSpan = iCnt + 1;
        }
        #endregion

        #region 그리드 근거번호별 셀 병합
        private void Set_REF_MURGE(int iRow, int iCnt)
        {
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "근거번호")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "근거금액")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "근거원화금액")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "경비금액_2")].RowSpan = iCnt + 1;
            fpSpread1.Sheets[0].Cells[iRow - iCnt - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "경비원화금액")].RowSpan = iCnt + 1;
        }
        #endregion
    }
}
