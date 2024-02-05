#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 부서조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-14
// 작성내용 : 공통팝업 부서조회
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
    WNDW.WNDW011 pu = new WNDW.WNDW011();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 부서정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 부서코드 </para>
    /// <para>Msgs[2] = 부서명 </para>
    /// <para>Msgs[3] = 사업장코드 </para>
    /// <para>Msgs[4] = 사업장명 </para>
    /// <para>Msgs[5] = 개편ID </para>
    /// </summary>

    public partial class WNDW011 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        string strSDT_DT = "";
        #endregion

        #region 생성자
        public WNDW011()
        {
            InitializeComponent();
        }
        public WNDW011(string STD_DT)
        {
            strSDT_DT = STD_DT;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW011_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "부서정보조회";

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            if (strSDT_DT == "")
            {
                dtpStdDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            }
            else
            {
                dtpStdDt.Text = strSDT_DT;
                dtpStdDt.Enabled = false;
            }
            Grid_search(false);

            dtpStdDt.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_search(true); }
        #endregion

        #region 조회함수
        private void Grid_search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_WNDW011 'S1' ";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                strQuery += ", @pSTD_DT ='" + dtpStdDt.Text.Trim() + "'";
                strQuery += ", @pDEPT_NM ='" + txtDeptNm.Text.Trim() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            RtnStr(e.Row);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region 그리드 선택값 입력밑 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(int R)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[fpSpread1.Sheets[0].Columns.Count];

                for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
                {
                    returnVal[i] = Convert.ToString(fpSpread1.Sheets[0].Cells[R, i].Value);
                }
            }
        }
        #endregion

        #region Text에서 Enter시 조회
        private void dtpStdDt_KeyDown(object sender, KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtDeptNm_KeyDown(object sender, KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        #endregion

        
    }
}
