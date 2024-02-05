#region 작성정보
/*********************************************************************/
// 단위업무명 : 매입등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 매입등록 및 관리
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
using WNDW;

namespace SS.SSA002
{
    public partial class SSA002P3 : UIForm.FPCOMM1
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread fpGrid;
        string strCustCd = "", strPrrcptFg = "";

        int ActiveRow = 0;
        string returnVal = "";
        string strPrrcptNo = "";
        #endregion

        #region 생성자
        public SSA002P3(FarPoint.Win.Spread.FpSpread fpRtrGrid, int Row, string CustCd, string PrrcptFg, string PrrcptNo)
        {
            fpGrid = fpRtrGrid;
            ActiveRow = Row;
            strCustCd = CustCd;
            strPrrcptFg = PrrcptFg;
            strPrrcptNo = PrrcptNo;

            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }

        public SSA002P3()
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion

        #region 폼로드 이벤트
        private void SSA002P3_Load(object sender, System.EventArgs e)
        {
            UIForm.Buttons.ReButton("001000000011", BtnNew, BtnPrint, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnHelp, BtnExcel, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수체크

            this.Text = "선수금번호 조회";

            txtCustCd.Value = strCustCd;
            txtPrrcptFgCd.Value = strPrrcptFg;
            txtPrrcptNoFr.Value = strPrrcptNo;
            dtpPrrcptDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpPrrcptDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            Search(false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search(true);
        }
        #endregion

        #region 조회함수
        private void Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_SSA002  @pTYPE = 'C3' ";
                    strQuery += ", @pDATE_FR = '" + dtpPrrcptDtFr.Text + "' ";
                    strQuery += ", @pDATE_TO = '" + dtpPrrcptDtTo.Text + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pPRRCPT_NO_FR = '" + txtPrrcptNoFr.Text + "' ";
                    strQuery += ", @pPRRCPT_NO_TO = '" + txtPrrcptNoTo.Text + "' ";
                    strQuery += ", @pPRRCPT_FG = '" + txtPrrcptFgCd.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 종료
        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region 확인(선택된 값 넘기기)
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            try
            {
                RtnStr(fpSpread1.Sheets[0].GetSelection(0).Row);

                strFormClosingMsg = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch { }
        }
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                RtnStr(e.Row);
                strFormClosingMsg = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch { }
        }
        private void RtnStr(int R)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    returnVal = "";
                    for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                    {
                        if (returnVal.Length > 0)
                            returnVal = returnVal + "#" + fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                        else
                            returnVal = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                    }
                }
            }
            catch { }
        }
        #endregion

        #region 조회조건 코드 입력시 코드명 자동입력
        //거래처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //선수금유형
        private void txtPrrcptFgCd_TextChanged(object sender, System.EventArgs e)
        {
            txtPrrcptFgNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPrrcptFgCd.Text, " And MAJOR_CD = 'S012' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion
    }
}
