

#region 작성정보
/*********************************************************************/
// 단위업무명 : 부가세전표확인
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-11
// 작성내용 : 부가세전표확인
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

namespace AF.ACF009
{
    public partial class ACF009 : UIForm.FPCOMM3
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        int PreRow2 = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strIssue_Dt_From = "";
        string strIssue_Dt_To = "";
        string strIo_Flag = "";
        string strBizAreaCd = "";
        #endregion

        public ACF009()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACF009_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpIssueDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpIssueDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G2Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0);
            PreRow = -1;
            PreRow2 = -1;
            strIssue_Dt_From = "";
            strIssue_Dt_To = "";
            strIo_Flag = "";
            strBizAreaCd = "";
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
                    string strQuery = " usp_ACF009  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pISSUE_DT_FROM = '" + dtpIssueDtFr.Text + "' ";
                    strQuery += ", @pISSUE_DT_TO = '" + dtpIssueDtTo.Text + "' ";
                    if(optIoFlag_I.Checked == true) strQuery += ", @pIO_FLAG = 'I' ";
                    else strQuery += ", @pIO_FLAG = 'O' ";
                    strQuery += ", @pRPT_BIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
                    UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                    UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0);
                    PreRow = -1;
                    PreRow2 = -1;

                    strIssue_Dt_From = dtpIssueDtFr.Text;
                    strIssue_Dt_To = dtpIssueDtTo.Text;
                    if (optIoFlag_I.Checked == true) strIo_Flag = "I";
                    else strIo_Flag = "O";
                    strBizAreaCd = cboBizAreaCd.SelectedValue.ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 선택
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }

                if (PreRow == intRow && PreRow != -1 && intRow != -1)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                if (intRow == fpSpread1.Sheets[0].Rows.Count - 1)
                {
                    PreRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    PreRow2 = -1;
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                string strVAT_TYPE = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형코드")].Value.ToString();
                string strELE_BILL_YN = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서구분코드")].Value.ToString();
                SEARCH_VAT_TYPE(strVAT_TYPE, strELE_BILL_YN);
                PreRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                PreRow2 = -1;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow2 = fpSpread2.ActiveSheet.GetSelection(0).Row;
                if (intRow2 < 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }

                if (PreRow2 == intRow2 && PreRow2 != -1 && intRow2 != -1)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                if (intRow2 == fpSpread2.Sheets[0].Rows.Count - 1)
                {
                    PreRow2 = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                string strVAT_TYPE = fpSpread2.Sheets[0].Cells[intRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "계산서유형코드")].Text;
                string strELE_BILL_YN = fpSpread2.Sheets[0].Cells[intRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "계산서구분코드")].Text;
                string strCUST_CD = fpSpread2.Sheets[0].Cells[intRow2, SystemBase.Base.GridHeadIndex(GHIdx2, "거래처코드")].Text;

                SEARCH_DETAIL(strVAT_TYPE, strELE_BILL_YN, strCUST_CD);
                PreRow2 = fpSpread2.ActiveSheet.GetSelection(0).Row;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회함수
        //전표 상세조회
        private void SEARCH_VAT_TYPE(string VAT_TYPE, string ELE_BILL_YN)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACF009  'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pISSUE_DT_FROM = '" + strIssue_Dt_From + "' ";
                strQuery += ", @pISSUE_DT_TO = '" + strIssue_Dt_To + "' ";
                strQuery += ", @pIO_FLAG = '" + strIo_Flag + "' ";
                strQuery += ", @pRPT_BIZ_AREA_CD = '" + strBizAreaCd + "' ";
                strQuery += ", @pVAT_TYPE = '" + VAT_TYPE + "' ";
                strQuery += ", @pELE_BILL_YN = '" + ELE_BILL_YN + "' ";



                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, true, true, 0, 0, true);
                UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        //관리항목 조회
        private void SEARCH_DETAIL(string VAT_TYPE, string ELE_BILL_YN, string CUST_CD)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACF009  'S3'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pISSUE_DT_FROM = '" + strIssue_Dt_From + "' ";
                strQuery += ", @pISSUE_DT_TO = '" + strIssue_Dt_To + "' ";
                strQuery += ", @pIO_FLAG = '" + strIo_Flag + "' ";
                strQuery += ", @pRPT_BIZ_AREA_CD = '" + strBizAreaCd + "' ";
                strQuery += ", @pVAT_TYPE = '" + VAT_TYPE + "' ";
                strQuery += ", @pELE_BILL_YN = '" + ELE_BILL_YN + "' ";
                strQuery += ", @pCUST_CD = '" + CUST_CD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread3, strQuery, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, true, true, 0, 0, true);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion
    }
}
