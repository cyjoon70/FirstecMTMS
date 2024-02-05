#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 프로젝트정보조회
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-02-05
// 작성내용 : 프로젝트정보조회
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
    WNDW.WNDW007 pu = new WNDW.WNDW007();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 프로젝트정보 조회2
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 사업코드 </para>
    /// <para>Msgs[2] = 사업명 </para>
    /// <para>Msgs[3] = 프로젝트번호 </para>
    /// <para>Msgs[4] = 프로젝트명 </para>
    /// </summary>

    public partial class WNDW007 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수

        string strProjNo = "";
        string strClose = "";
        #endregion

        #region WNDW007 생성자
        public WNDW007(string ProjectNo)
        {
            strProjNo = ProjectNo;
            InitializeComponent();
        }

        public WNDW007(string ProjectNo, string CloseYN)
        {
            strProjNo = ProjectNo;
            strClose = CloseYN;
            InitializeComponent();
        }

        public WNDW007()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW007_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            txtProjectNo.Text = strProjNo;

            if (strClose == "Y") rdoCloseY.Checked = true;
            else if (strClose == "N")
            {
                rdoCloseN.Checked = true;
            }
            // 2016.09.08. hma 추가(Start): 전체를 기본으로 선택할 수도 있도록 함.
            else if (strClose == "A")
            {
                rdoCloseAll.Checked = true;
            }
            // 2016.09.08. hma 추가(End)
            else rdoCloseN.Checked = true;

            Grid_Search(false);

            this.Text = "프로젝트정보 조회";
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
                string close_yn = "";
                try
                {
                    SDown = 1;

                    if (rdoCloseY.Checked == true) close_yn = "Y";
                    else if (rdoCloseN.Checked == true) close_yn = "N";

                    string strQuery = " usp_WNDW007 @pTYPE = 'S1'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                    strQuery += ", @pPROJECT_NM = '" + txtProjectNm.Text + "' ";
                    strQuery += ", @pSO_DT_FR = '" + dtpSoDtFr.Text + "' ";
                    strQuery += ", @pSO_DT_TO = '" + dtpSoDtTo.Text + "' ";
                    strQuery += ", @pDELV_DT_FR = '" + dtpDelvDtFr.Text + "' ";
                    strQuery += ", @pDELV_DT_TO = '" + dtpDelvDtTo.Text + "' ";
                    strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                    strQuery += ", @pENT_NM = '" + txtEntNm.Text + "' ";
                    strQuery += ", @pCLOSE_YN = '" + close_yn + "' ";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, Msg, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Color(0);
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

        #region 조회조건 팝업창
        //사업코드
        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextBox코드입력시 코드명 자동입력
        //사업
        private void txtEntCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEntCd.Text != "")
                {
                    txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtEntNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            string close_yn = "";

            if (rdoCloseY.Checked == true) close_yn = "Y";
            else if (rdoCloseN.Checked == true) close_yn = "N";

            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                int cnt_prev = AddRow * SDown;
                SDown++;
                int cnt = AddRow * SDown;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_WNDW007 'S1'";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_NM = '" + txtProjectNm.Text + "' ";
                strQuery += ", @pSO_DT_FR = '" + dtpSoDtFr.Text + "' ";
                strQuery += ", @pSO_DT_TO = '" + dtpSoDtTo.Text + "' ";
                strQuery += ", @pDELV_DT_FR = '" + dtpDelvDtFr.Text + "' ";
                strQuery += ", @pDELV_DT_TO = '" + dtpDelvDtTo.Text + "' ";
                strQuery += ", @pENT_CD = '" + txtEntCd.Text + "' ";
                strQuery += ", @pENT_NM = '" + txtEntNm.Text + "' ";
                strQuery += ", @pCLOSE_YN = '" + close_yn + "' ";
                strQuery += ", @pTOPCOUNT ='" + cnt + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
                if (fpSpread1.Sheets[0].RowCount > cnt_prev) Set_Color(cnt_prev);

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
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }
        private void txtProjectNm_KeyDown_1(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }
        private void txtEntCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_Search(true); }
        #endregion

        #region Set_Color
        private void Set_Color(int start_idx)
        {
            int col_idx = SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부");
            int s = 0;
            if (start_idx > 0) s = start_idx - 1;
            for (int i = s; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, col_idx].Text == "Y")
                {
                   fpSpread1.Sheets[0].Cells[i, 0, i, fpSpread1.Sheets[0].ColumnCount-1].ForeColor = Color.Red;
                }
            }
        }
        #endregion
    }
}
