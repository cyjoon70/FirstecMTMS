#region 작성정보
/*********************************************************************/
// 단위업무명 : 감가상각계산 처리 조회
// 작 성 자 : 한미애
// 작 성 일 : 2018-10-17
// 작성내용 : 감가상각계산조회
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW042 pu = new WNDW.WNDW042();
    pu.ShowDialog();
    if (pu.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

        textBox1.Text = Msgs[1].ToString();
        textBox2.Value = Msgs[2].ToString();
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자산정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 감가상각계산조회
    /// </summary>

    public partial class WNDW042 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strDeprMonthStart = "";
        string strDeprMonthEnd = "";
        #endregion

        #region 생성자
        public WNDW042()
        {
            InitializeComponent();
        }

        public WNDW042(string DeprMonthStart, string DeprMonthEnd)
        {
            strDeprMonthStart = DeprMonthStart;
            strDeprMonthEnd = DeprMonthEnd;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW042_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅
            dtpDeprDtFr.Value = strDeprMonthStart;  // SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01-01";
            dtpDeprDtTo.Value = strDeprMonthEnd;    // SystemBase.Base.ServerTime("YYMMDD");
            Grid_Search(false);

            this.Text = "감가상각계산조회";
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드 조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_WNDW042 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pDEPR_YYYYMM_FROM = '" + dtpDeprDtFr.Text + "' ";
                    strQuery += ", @pDEPR_YYYYMM_TO = '" + dtpDeprDtTo.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion       
    }
}