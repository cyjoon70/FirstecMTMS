#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 사원정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-06
// 작성내용 : 사원정보조회
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
    WNDW.WNDW010 pu = new WNDW.WNDW010();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사원정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 사원정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 부서코드 </para>
    /// <para>Msgs[2] = 부서명 </para>
    /// <para>Msgs[3] = 사원코드 </para>
    /// <para>Msgs[4] = 사원명 </para>
    /// <para>Msgs[5] = 유효일자 </para>
    /// </summary>

    public partial class WNDW010 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;

        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        string strEmpNo = "";
        string strEmpNm = "";
        string strDeptCd = "";
        string strDeptNm = "";
        #endregion

        #region 생성자
        public WNDW010(string sEmpNo, string sEmpNm, string sDeptCd, string sDeptNm)
        {

            strEmpNo = sEmpNo;
            strEmpNm = sEmpNm;
            strDeptCd = sDeptCd;
            strDeptNm = sDeptNm;

            InitializeComponent();
        }

        public WNDW010()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW010_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtEmpNo.Value = strEmpNo;
            txtEmpNm.Value = strEmpNm;
            txtDeptCd.Value = strDeptCd;
            txtDeptNm.Value = strDeptNm;

            Grid_search(false);

            txtDeptCd.Focus();

            this.Text = "사원정보 조회";
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

            SDown = 1;

            string strUseFlag = "";

            if (rdoUseY.Checked == true) strUseFlag = "Y";		//재직
            else if (rdoUseN.Checked == true) strUseFlag = "N";	//퇴사

            string strQuery = " usp_WNDW010 'S1' ";
            strQuery += ", @pDEPT_CD ='" + txtEmpNo.Text.Trim() + "'";
            strQuery += ", @pEMP_NO ='" + txtEmpNo.Text.Trim() + "'";
            strQuery += ", @pEMP_NM ='" + txtEmpNm.Text.Trim() + "'";
            strQuery += ", @pUSE_FLAG ='" + strUseFlag + "'";
            strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
            strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
            fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

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

        #region 부서코드 팝업
        private void btnDept_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pType='A020', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Value = Msgs[0].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 부서코드 TextChanged 이벤트
        private void txtDeptCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtDeptCd.Text != "")
                {
                    string strQuery = "usp_A_COMMON @pType='A010', @pCODE = '" + txtDeptCd.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        txtDeptNm.Value = dt.Rows[0][1].ToString();
                        txtRegNm.Value = dt.Rows[0][2].ToString();
                    }
                    else
                    {
                        txtDeptNm.Value = "";
                        txtRegNm.Value = "";
                        txtDeptCd.Focus();
                    }
                }
                else
                {
                    txtDeptNm.Value = "";
                    txtRegNm.Value = "";
                    txtDeptCd.Focus();
                }
            }
            catch { }
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {

            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                string strUseFlag = "";

                if (rdoUseY.Checked == true) strUseFlag = "Y";		//재직
                else if (rdoUseN.Checked == true) strUseFlag = "N";	//퇴사

                string strQuery = " usp_WNDW010 'S1' ";
                strQuery += ", @pDEPT_CD ='" + txtEmpNo.Text.Trim() + "'";
                strQuery += ", @pEMP_NO ='" + txtEmpNo.Text.Trim() + "'";
                strQuery += ", @pEMP_NM ='" + txtEmpNm.Text.Trim() + "'";
                strQuery += ", @pUSE_FLAG ='" + strUseFlag + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }
        }
        #endregion

        #region Text에서 Enter시 조회
        private void txtDeptCd_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtEmpNo_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtEmpNm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        #endregion
    }
}
