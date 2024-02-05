#region 작성정보
/*********************************************************************/
// 단위업무명:  수불집계표(회계)
// 작 성 자  :  한 미 애
// 작 성 일  :  2018-10-26
// 작성내용  :  수불집계표(ITR103)을 복사하여 추가함.
//              구매입고 프로젝트변경에 의한 재고이동 데이터를 포함한 수불 
//              데이터를 집계하여 조회한다.
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
using FarPoint.Win.Spread.CellType;

namespace IT.ITR120
{
    public partial class ITR120 : UIForm.FPCOMM1
    {
        bool form_act_chk = false;

        public ITR120()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ITR120_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            txtSlFr.Text = "W03";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            fpSpread1.Sheets[0].Rows.Count = 0;
            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strSubjExclude;
                    if (chkSubjExclude.Checked == true)
                        strSubjExclude = "Y";
                    else
                        strSubjExclude = "N";

                    string strQuery = " usp_ITR120 @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pYEAR_MON_FR  ='" + mskDT_Fr.Text + "'";
                    strQuery += ", @pYEAR_MON_TO  ='" + mskDT_To.Text + "'";
                    strQuery += ", @pSL_CD ='" + txtSlFr.Text.Trim() + "'";
                    strQuery += ", @pSUBJ_EXCLUDE = '" + strSubjExclude + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                        Set_Section();
                    else
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
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

        #region 합계 그리드 재정의, 금액 표시
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;

            //합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "합계")
                {

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].ColumnSpan = 4;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;

                    //합계 색 변경
                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                    }

                    //이월급액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이월재고_2")].Text != "")
                        txtBasAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이월재고_2")].Value;
                    else
                        txtBasAmt.Value = 0;

                    //입고금액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고_2")].Text != "")
                        txtMvmtAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고_2")].Value;
                    else
                        txtMvmtAmt.Value = 0;

                    //출고금액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고_2")].Text != "")
                        txtTranAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고_2")].Value;
                    else
                        txtTranAmt.Value = 0;

                    //재고금액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고_2")].Text != "")
                        txtDnAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고_2")].Value;
                    else
                        txtDnAmt.Value = 0;

                }
            }
        }
        #endregion

        #region 팝업창 열기(품목)
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

        private void btnSlFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlFr.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlFr.Text = Msgs[0].ToString();
                    txtSlNmFr.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtSlFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlFr.Text != "")
                {
                    txtSlNmFr.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSlNmFr.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 레포트 출력
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                //string[] FormulaField = new string[2];	  //formula 값			
                string RptName = SystemBase.Base.ProgramWhere + @"\Report\ITR120.rpt";    // 레포트경로+레포트명
                string[] RptParmValue = new string[8];   // SP 파라메타 값
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                RptParmValue[2] = Convert.ToString(cboPlantCd.SelectedValue);
                RptParmValue[3] = txtItemCd.Text;
                RptParmValue[4] = Convert.ToString(cboItemAcct.SelectedValue);
                RptParmValue[5] = mskDT_Fr.Text;
                RptParmValue[6] = mskDT_To.Text;
                RptParmValue[7] = txtSlFr.Text;

                FormulaFieldValue[0] += "\"" + cboItemAcct.Text + "\"";
                FormulaFieldName[0] = "ACCT_NM";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue); //공통크리스탈 10버전				
                frm.ShowDialog();
            }
        }
        #endregion

        #region Form Activated & Deactivate
        private void ITR120_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR120_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

    }
}
