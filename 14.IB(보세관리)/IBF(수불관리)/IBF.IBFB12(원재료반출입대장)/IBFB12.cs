#region 작성정보
/*********************************************************************/
// 단위업무명 : 원재료반출입대장
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-11
// 작성내용 : 원재료반출입대장 관리
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
using FarPoint.Win.Spread.CellType;

namespace IBF.IBFB12
{
    public partial class IBFB12 : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB12()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void IBFB12_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[9];
            string[] FormulaField2 = new string[1]; //formula 값
            string[] FormulaField1 = new string[1]; //formula 이름


            string RptName = "";

            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            if (rdoDefault.Checked == true)
            {
                RptName = @"Report\" + "IBFB24P.rpt";
                RptParmValue[0] = "R1";
            }
            else if (rdoItem.Checked == true)
            {
                RptName = @"Report\" + "IBFB29P.rpt";
                RptParmValue[0] = "R2";
            }
            else
            {
                RptName = @"Report\" + "IBFB30P.rpt";
                RptParmValue[0] = "R3";
            }
            RptParmValue[1] = txtTRNo.Text;

            if (txtItemCd.Text.Trim() == "") RptParmValue[2] = " ";
            else RptParmValue[2] = txtItemCd.Text;

            if (dtpDT_FR.Text.Trim() == "") RptParmValue[3] = " ";
            else RptParmValue[3] = dtpDT_FR.Text;

            if (dtpDT_TO.Text.Trim() == "") RptParmValue[4] = " ";
            else RptParmValue[4] = dtpDT_TO.Text;

            if (txtDECLARE_NO.Text.Trim() == "") RptParmValue[5] = " ";
            else RptParmValue[5] = txtDECLARE_NO.Text;

            if (txtBL_NO.Text.Trim() == "") RptParmValue[6] = " ";
            else RptParmValue[6] = txtBL_NO.Text;

            if (txtNOTIFY_NO.Text.Trim() == "") RptParmValue[7] = " ";
            else RptParmValue[7] = txtNOTIFY_NO.Text;

            RptParmValue[8] = SystemBase.Base.gstrCOMCD;

            FormulaField2[0] = "\"" + txtBUSINESS_NM.Text + "\"";
            FormulaField1[0] = "BUSI_NM";


            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", FormulaField2, FormulaField1, RptName, RptParmValue);
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
                    string strQuery = "";

                    if (rdoNotifyNo.Checked == true) strQuery = " usp_IBFB12  S2, ";
                    else strQuery = " usp_IBFB12  S1, ";

                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                    strQuery = strQuery + " @pITEM_CD = '" + txtItemCd.Text + "', ";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "', ";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                    strQuery = strQuery + " @pBL_NO = '" + txtBL_NO.Text + "', ";
                    strQuery = strQuery + " @pDECLARE_NO = '" + txtDECLARE_NO.Text + "', ";
                    strQuery = strQuery + " @pNOTIFY_NO = '" + txtNOTIFY_NO.Text + "',  ";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 5, false);
                    //					fpSpread1.Sheets[0].OperationMode =  FarPoint.Win.Spread.OperationMode.SingleSelect;

                    if (fpSpread1.Sheets[0].Rows.Count > 0) Spread_Compute();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        private void Spread_Compute()
        {
            decimal amt1 = 0, amt2 = 0, amt3 = 0, amt4 = 0, amt0 = 0;
            int i = 0, cnt = 1;


            try
            {
                if (fpSpread1.Sheets[0].Rows.Count == 1)
                {
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text = "";
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외화금액")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외화금액")].Text = "";
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text = "";

                    amt0 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                    amt1 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value);
                    amt2 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                    amt3 = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value);

                    if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "구분코드")].Text == "3") amt4 = amt0 + amt1 + amt2 - amt3;
                    else amt4 = amt0 + amt1 - amt2 - amt3;

                    fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value = amt4;
                    fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value = amt4 * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);

                }
                else
                {
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value) == 0) fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text = "";
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "외화금액")].Value) == 0) fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "외화금액")].Text = "";
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value) == 0) fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text = "";

                    for (i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {

                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text = "";
                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외화금액")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외화금액")].Text = "";
                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text = "";

                        if (fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text)
                        {
                            amt0 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                            amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value);

                            if (fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "구분코드")].Text == "3") amt2 -= Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                            else amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);

                            amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value);

                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Text = "";

                            if (i == fpSpread1.Sheets[0].Rows.Count - 1)
                            {
                                amt0 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                                amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value);
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분코드")].Text == "3") amt2 -= Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                                else amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                                amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value);
                                amt4 = amt0 + amt1 - amt2 - amt3;
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value = amt4;
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value = amt4 * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);

                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Text = "";
                            }

                            //--같은 품목 rowspan
                            cnt++;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].RowSpan = cnt;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].RowSpan = cnt;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].RowSpan = cnt;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].RowSpan = cnt;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].RowSpan = cnt;

                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[i - cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        }
                        else
                        {
                            amt0 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                            amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value);
                            if (fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "구분코드")].Text == "3") amt2 -= Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                            else amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                            amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value);
                            amt4 = amt0 + amt1 - amt2 - amt3;
                            fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value = amt4;
                            fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value = amt4 * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);

                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Text = "";
                            if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value) == 0) fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Text = "";

                            amt0 = 0; amt1 = 0; amt2 = 0; amt3 = 0; amt4 = 0;

                            if (i == fpSpread1.Sheets[0].Rows.Count - 1)
                            {
                                amt0 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                                amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value);
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분코드")].Text == "3") amt2 -= Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                                else amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value);
                                amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value);
                                amt4 = amt0 + amt1 - amt2 - amt3;
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value = amt4;
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value = amt4 * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);

                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_2")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_3")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_4")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량_5")].Text = "";
                                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value) == 0) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Text = "";
                                amt0 = 0; amt1 = 0; amt2 = 0; amt3 = 0; amt4 = 0;

                            }
                            cnt = 1;

                        }

                    }

                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 버튼 Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF11' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    //					txtSO_NO.Text = Msgs[1].ToString();		
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butBL_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() == "") { MessageBox.Show("먼저 Tracking No. 입력하세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); txtTRNo.Focus(); return; }
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF17' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME", "@pSPEC" };
                string[] strSearch = new string[] { txtBL_NO.Text, "", txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP011", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "반입번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBL_NO.Text = Msgs[0].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butDECLARE_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() == "") { MessageBox.Show("먼저 Tracking No. 입력하세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); txtTRNo.Focus(); return; }
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF16' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME", "@pSPEC" };
                string[] strSearch = new string[] { txtDECLARE_NO.Text, "", txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "신고번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDECLARE_NO.Text = Msgs[0].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butNOTIFY_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() == "") { MessageBox.Show("먼저 Tracking No. 입력하세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); txtTRNo.Focus(); return; }
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF18' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME", "@pSPEC" };
                string[] strSearch = new string[] { txtNOTIFY_NO.Text, "", txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP012", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "반출번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtNOTIFY_NO.Text = Msgs[0].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                //품목 팝업
                this.Cursor = Cursors.WaitCursor;

                string strQuery = " Nusp_BF_Comm 'BF22' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtItemCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemCd.Text = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void dtpDT_FR_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_FR.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT_FR.Text) == false)
//                {
//                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT_FR.Focus();
//                    dtpDT_FR.SelectAll();
//                }
            }
        }

        private void dtpDT_TO_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_TO.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT_TO.Text) == false)
//                {
//                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT_TO.Focus();
//                    dtpDT_TO.SelectAll();
//                }
            }
        }

        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }
                    txtSO_NO.Value = txtTRNo.Text.Trim();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtDECLARE_NO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtNOTIFY_NO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtBL_NO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void dtpDT_TO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }
        #endregion

        #region 폼 Activated & Deactivated
        private void IBFB12_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB12_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

    }
}








