#region 작성정보
/*********************************************************************/
// 단위업무명 : 판단번호 정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-08-21
// 작성내용 : 판단번호조회
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
    WNDW.WNDW029 pu = new WNDW.WNDW029();
    pu.ShowDialog();
    if (pu.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

        textBox1.Text = Msgs[1].ToString();
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "판단번호 조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 판단번호 정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 지시연도 </para>
    /// <para>Msgs[2] = 판단번호 </para>
    /// <para>Msgs[3] = 차수 </para>
    /// <para>Msgs[4] = 조달업체 </para>
    /// <para>Msgs[5] = 구매부서 </para>
    /// <para>Msgs[6] = 대표품명 </para>
    /// <para>Msgs[7] = 제출용도 </para>
    /// <para>Msgs[8] = 기준년월 </para>
    /// <para>Msgs[9] = 계산기준일 </para>
    /// </summary>

    public partial class WNDW029 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        string strType = "";
        #endregion

        #region WNDW029 생성자
        public WNDW029()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW029_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "MASTER KEY 팝업";
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //제출업체
            SystemBase.ComboMake.C1Combo(cboH_MNUF_CODE, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);   //제출업체

            txtORDR_YEAR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("")).ToString().Substring(0, 4);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달업체")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "구매부서")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "제출용도")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            Grid_search(false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_search(true); }
        #endregion

        #region 그리드조회
        private void Grid_search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    SDown = 1;

                    string strQuery = " usp_WNDW029 'S1'";
                    strQuery += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
                    strQuery += ", @pORDR_YEAR = '" + txtORDR_YEAR.Text + "' ";
                    strQuery += ", @pDCSN_NUMB = '" + txtDCSN_NUMB.Text + "' ";
                    strQuery += ", @pCALC_DEGR = '" + txtCAL_C_DEGR.Text + "' ";

                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
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

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_WNDW029 'S1'";
                strQuery += ", @pMNUF_CODE = '" + cboH_MNUF_CODE.SelectedValue.ToString() + "' ";
                strQuery += ", @pORDR_YEAR = '" + txtORDR_YEAR.Text + "' ";
                strQuery += ", @pDCSN_NUMB = '" + txtDCSN_NUMB.Text + "' ";
                strQuery += ", @pCALC_DEGR = '" + txtCAL_C_DEGR.Text + "' ";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                this.Cursor = Cursors.Default;
            }
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
        private void txtProjectNo_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtProjectNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtWorkOrderNo_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemCd_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        #endregion
    }
}
