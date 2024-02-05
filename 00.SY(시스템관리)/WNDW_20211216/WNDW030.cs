#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 도로명주소
// 작 성 자   : 김창진
// 작 성 일   : 2013-12-05
// 작성내용   : 도로명주소
// 수 정 일   :
// 수 정 자   :
// 수정내용   :
// 비    고   :
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW030 pu = new WNDW.WNDW030();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장별품목정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 공장별품목정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 공장코드 </para>
    /// <para>Msgs[2] = 품목코드 </para>
    /// <para>Msgs[3] = 품목명 </para>
    /// </summary>

    public partial class WNDW030 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        string strZip_Code = "";

        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        #endregion

        #region WNDW030 생성자
        public WNDW030(string strZipCode)
        {
            strZip_Code = strZipCode;

            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW030_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용


            //조회 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSIDO, "usp_WNDW030 @pTYPE='P1'");	//시도
            SystemBase.ComboMake.C1Combo(cboSIGUNGU, "usp_WNDW030 @pTYPE='P2', @pSIDO ='" + cboSIDO.SelectedValue.ToString()  + "'");	//시도


            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            this.Text = "도로명 주소 조회";

            txtZIPCODE.Text = strZip_Code;
            
            Grid_search(false);

            txtZIPCODE.Focus();
        }
        #endregion

        #region 조회버튼 클릭
        protected override void SearchExec()
        { Grid_search(true); }
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

        #region 조회함수
        private void Grid_search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                // 2016.09.19. hma 추가(Start): 세종시의 경우 시군구 값이 없으므로 시군구 항목을 필수로 하지않고
                //                              세종시 이외의 시도에 대해서 시군구 입력을 체크하도록 함. 
                if ((cboSIDO.SelectedValue.ToString() != "세종특별자치시") &&
                        (cboSIGUNGU.SelectedValue.ToString() == ""))
                {
                    MessageBox.Show("시군구 항목값을 선택한후 조회하세요");
                    return;
                }
                // 2016.09.19. hma 추가(End)

                SDown = 1;

                string strQuery = " usp_WNDW030 'S1'";
                strQuery += ", @pZIPCODE ='" + txtZIPCODE.Text.Trim() + "'";
                strQuery += ", @pJIBUNJUSO ='" + txtJIBUNJUSO.Text + "'";
                strQuery += ", @pDOROJUSO ='" + txtDOROJUSO.Text + "'";
                strQuery += ", @pSIDO ='" + cboSIDO.SelectedValue.ToString()  + "'";
                strQuery += ", @pSIGUNGU ='" + cboSIGUNGU.SelectedValue.ToString() + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Text에서 Enter시 조회
        
        private void txtZIPCODE_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }

        private void txtJIBUNJUSO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }

        private void txtDOROJUSO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }

        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_WNDW030 'S1'";
                strQuery += ", @pZIPCODE ='" + txtZIPCODE.Text.Trim() + "'";
                strQuery += ", @pJIBUNJUSO ='" + txtJIBUNJUSO.Text + "'";
                strQuery += ", @pDOROJUSO ='" + txtDOROJUSO.Text + "'";
                strQuery += ", @pSIDO ='" + cboSIDO.SelectedValue.ToString() + "'";
                strQuery += ", @pSIGUNGU ='" + cboSIGUNGU.SelectedValue.ToString() + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 콤보박스 CHANGE
        private void cboSIDO_SelectedValueChanged(object sender, EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboSIGUNGU, "usp_WNDW030 @pTYPE='P2', @pSIDO ='" + cboSIDO.SelectedValue.ToString() + "'");	//시도
        }
        #endregion

    }
}
