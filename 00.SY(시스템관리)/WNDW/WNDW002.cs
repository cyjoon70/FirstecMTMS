#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 거래처정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-01
// 작성내용 : 거래처정보조회
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

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.WNDW002 pu = new WNDW.WNDW002();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 거래처정보조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 거래처코드 </para>
    /// <para>Msgs[2] = 거래처명 </para>
    /// </summary>
    
    public partial class WNDW002 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;

        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수

        string strType = "";
        string strCustCd = "";
        string strSCM_YN = "";
        #endregion

        #region WNDW002 생성자
        public WNDW002(string strCustType)
        {
            //품목계정값 S-매출처, P-매입처. PS-매입매출처 
            if (strCustType == "S") strType = "S";
            if (strCustType == "P") strType = "P";
            if (strCustType == "PS") strType = "PS";
            if (strCustType == "C") strType = "C";
            if (strCustType == "T") strType = "T";

            InitializeComponent();
        }

        public WNDW002(string CustCd, string strCustType)
        {
            strCustCd = CustCd;

            //품목계정값 S-매출처, P-매입처. PS-매입매출처 
            if (strCustType == "S") strType = "S";
            if (strCustType == "P") strType = "P";
            if (strCustType == "PS") strType = "PS";
            if (strCustType == "C") strType = "C";
            if (strCustType == "T") strType = "T";

            InitializeComponent();
        }

        public WNDW002(string CustCd, string strCustType, string SCM_YN)
        {
            strCustCd = CustCd;

            //품목계정값 S-매출처, P-매입처. PS-매입매출처 
            if (strCustType == "S") strType = "S";
            if (strCustType == "P") strType = "P";
            if (strCustType == "PS") strType = "PS";
            if (strCustType == "C") strType = "C";
            if (strCustType == "T") strType = "T";

            strSCM_YN = SCM_YN;

            InitializeComponent();
        }

        public WNDW002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW002_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "거래처구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='COMM', @pCODE = 'B005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //if (strType == "") optCustType1.Checked = true;		    //거래처 구분 전체
            if (strType == "" && strType == "P") optCustType1.Checked = true;		    //거래처 구분 전체
            if (strType == "S") optCustType2.Checked = true;		//거래처 구분 매출처
            //if (strType == "P") optCustType3.Checked = true;		//거래처 구분 매입처
            if (strType == "PS") optCustType4.Checked = true;	    //거래처 구분 매입매출처
            if (strType == "T") optCustType5.Checked = true;		//거래처 구분 세금신고사업장
            if (strType == "C") optCustType6.Checked = true;		//거래처 구분 기타

            this.Text = "거래처 정보 조회";

            txtCustCd.Text = strCustCd;
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            Grid_Search(false);
            
            txtCustCd.Focus();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_Search(true); }
        #endregion

        #region 그리드조회
        private void Grid_Search(bool Msg)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                SDown = 1;

                string strQuery = " usp_WNDW002  'S1'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery = strQuery + ", @pCUST_CD ='" + txtCustCd.Text.Trim() + "'";
                strQuery = strQuery + ", @pCUST_NM ='" + txtCustNm.Text.Trim() + "'";

                string strCustType = "";
                if (optCustType2.Checked == true) strCustType = "S";
                else if (optCustType3.Checked == true) strCustType = "P";
                else if (optCustType4.Checked == true) strCustType = "PS";
                else if (optCustType5.Checked == true) strCustType = "T";
                else if (optCustType6.Checked == true) strCustType = "C";

                strQuery = strQuery + ", @pCUST_TYPE ='" + strCustType + "'";

                string strUseFlag = "";
                if (optUseFlag2.Checked == true) strUseFlag = "Y";
                else if (optUseFlag3.Checked == true) strUseFlag = "N";
                strQuery = strQuery + ", @pUSE_FLAG ='" + strUseFlag + "'";
                strQuery = strQuery + ", @pSCM_YN ='" + strSCM_YN + "'";
                strQuery = strQuery + ", @pTOPCOUNT = '" + AddRow + "'";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strQuery = strQuery + ", @pAPPLY_DT ='" + dtpDate.Text + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
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

                string strQuery = " usp_WNDW002  'S1'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery = strQuery + ", @pCUST_CD ='" + txtCustCd.Text.Trim() + "'";
                strQuery = strQuery + ", @pCUST_NM ='" + txtCustNm.Text.Trim() + "'";

                string strCustType = "";
                if (optCustType2.Checked == true) strCustType = "S";
                else if (optCustType3.Checked == true) strCustType = "P";
                else if (optCustType4.Checked == true) strCustType = "PS";
                else if (optCustType5.Checked == true) strCustType = "T";
                else if (optCustType6.Checked == true) strCustType = "C";
                
                strQuery = strQuery + ", @pCUST_TYPE ='" + strCustType + "'";

                string strUseFlag = "";
                if (optUseFlag2.Checked == true) strUseFlag = "Y";
                else if (optUseFlag3.Checked == true) strUseFlag = "N";
                strQuery = strQuery + ", @pUSE_FLAG ='" + strUseFlag + "'";
                strQuery = strQuery + ", @pSCM_YN ='" + strSCM_YN + "'";
                strQuery = strQuery + ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strQuery = strQuery + ", @pAPPLY_DT ='" + dtpDate.Text + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
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

        #region Text에서 Enter 클릭시 조회
        private void txtCustCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }

        private void txtCustNm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }

        #endregion
    }
}
