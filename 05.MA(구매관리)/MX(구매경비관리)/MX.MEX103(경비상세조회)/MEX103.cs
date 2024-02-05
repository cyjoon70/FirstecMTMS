#region 작성정보
/*********************************************************************/
// 단위업무명 : 경비상세조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-15
// 작성내용 : 경비상세조회 및 관리
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

namespace MX.MEX103
{
    public partial class MEX103 : UIForm.FPCOMM2
    {
        public MEX103()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MEX103_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅	
            txtBizCd.Text = SystemBase.Base.gstrBIZCD;

            dtpExpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpExpDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 10);

            btnExpRefNo.Enabled = false;

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //기타 세팅	
            txtBizCd.Text = SystemBase.Base.gstrBIZCD;

            dtpExpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpExpDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);

            btnExpRefNo.Enabled = false;
        }
        #endregion

        #region 조회조건 팝업
        //사업장
        private void btnBizCd_Click(object sender, System.EventArgs e)
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
        private void btnExpCd_Click(object sender, System.EventArgs e)
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
        private void btnPaymentCust_Click(object sender, System.EventArgs e)
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
        private void btnExpSteps_Click(object sender, System.EventArgs e)
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

        //발생근거번호
        private void btnExpRefNo_Click(object sender, System.EventArgs e)
        {
            string strExpSteps = txtExpSteps.Text;

            //발주
            if (strExpSteps == "PO")
            {
                try
                {
                    MEX102P1 myForm = new MEX102P1();
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        txtExpRefNo.Text = Msgs;
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
                    MEX102P2 myForm = new MEX102P2();
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        txtExpRefNo.Text = Msgs;
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수입선적 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (strExpSteps == "VD") // 통관
            {
                try
                {
                    MEX102P3 myForm = new MEX102P3();
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        txtExpRefNo.Text = Msgs;
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "통관번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (strExpSteps == "VL") // L/C
            {
                try
                {
                    MEX102P4 myForm = new MEX102P4("VL");
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        txtExpRefNo.Text = Msgs;
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "L/C번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (strExpSteps == "VO") // LOCAL L/C
            {
                try
                {
                    MEX102P4 myForm = new MEX102P4("VO");
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        txtExpRefNo.Text = Msgs;
                        txtExpRefNo.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "L/C번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }

        }

        #endregion

        #region 조회조건 TextChanged
        //사업장
        private void txtBizCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBizCd.Text != "")
                {
                    txtBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtBizCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBizNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //경비항목
        private void txtExpCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtExpCd.Text != "")
                {
                    txtExpNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtExpCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M013' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtExpNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //지급처
        private void txtPaymentCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPaymentCust.Text != "")
                {
                    txtPaymentCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtPaymentCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPaymentCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //진행구분
        private void txtExpSteps_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtExpSteps.Text != "")
                {
                    txtExpStepsNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtExpSteps.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M015' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtExpStepsNm.Value = "";
                }
                if (txtExpStepsNm.Text != "")
                    btnExpRefNo.Enabled = true;
                else
                    btnExpRefNo.Enabled = false;
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProjectNo.Text != "")
                {
                    txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProjectNm.Value = "";
                }
                if (txtProjectNm.Text == "")
                    txtProjectSeq.Text = "";
            }
            catch
            {

            }
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
                    string strQuery = "usp_MEX103 @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pBIZ_CD = '" + txtBizCd.Text + "'";
                    strQuery += ", @pEXP_CD = '" + txtExpCd.Text + "'";
                    strQuery += ", @pPAYMENT_CUST = '" + txtPaymentCust.Text + "'";
                    strQuery += ", @pEXP_STEPS = '" + txtExpSteps.Text + "'";
                    strQuery += ", @pEXP_DT_FR = '" + dtpExpDtFr.Text + "'";
                    strQuery += ", @pEXP_DT_TO = '" + dtpExpDtTo.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    //버튼 타입이면 Locked 해제
                    int fpRow = fpSpread2.Sheets[0].Rows.Count;
                    if (fpRow > 0)
                    {
                        for (int i = 0; i < fpRow; i++)
                        {
                            for (int j = 0; j < fpSpread2.Sheets[0].Columns.Count; j++)
                            {
                                if (fpSpread2.Sheets[0].Columns[j].CellType.ToString() == "ButtonCellType")
                                    fpSpread2.Sheets[0].Columns[j].Locked = false;
                            }
                        }

                        Grd_Search();
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }

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

        #region 상세조회
        private void Grd_Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = "usp_MEX103 @pTYPE = 'S2'";
                strQuery += ", @pEXP_NO = '" + fpSpread2.Sheets[0].Cells[fpSpread2.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx2, "관리번호")].Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 상 팝업
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";

            //프로젝트번호지정
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호지정_2") && fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호지정")].Text == "Y")
            {
                try
                {
                    MEX103P1 myForm = new MEX103P1(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "관리번호")].Text);
                    myForm.ShowDialog();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "관리번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region fpSpread2 선택시 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    Grd_Search();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

    }
}
