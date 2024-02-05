#region 작성정보
/*********************************************************************/
// 단위업무명:  공통팝업 전진검사요청정보조회
// 작 성 자  :  한 미 애
// 작 성 일  :  2017-11-30
// 작성내용  :  전진검사요청정보조회
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace WNDW
{
    /// <summary>
    /// 전진검사요청정보 조회
    /// <para>Msgs[1] = 검사요청번호 </para>
    /// </summary>

    public partial class WNDW041 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        string strPlantCd = "", strCustCd = "", strInspReqNo = "", strInspClassCd = "", strInspStatus = "";
        string strInspDtFr = "", strInspDtTo = "";
        #endregion

        #region WNDW041 생성자
        public WNDW041(string PlantCd, string InspReqNo, string CustCd, string InspDtFr, string InspDtTo)
        {
            strPlantCd = PlantCd;
            strInspReqNo = InspReqNo;
            strCustCd = CustCd;
            strInspDtFr = InspDtFr;
            strInspDtTo = InspDtTo;

            InitializeComponent();
        }

        public WNDW041()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW041_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            SystemBase.Validation.GroupBox_Setting(groupBox1);      //필수적용

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pTYPE	= 'PLANT'", 3); //공장
            SystemBase.ComboMake.C1Combo(cboInspClassCd, "usp_B_COMMON @pTYPE = 'COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'Q036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3, true); //검사항목
            SystemBase.ComboMake.C1Combo(cboInspStatus, "usp_B_COMMON @pTYPE = 'COMM2', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'Q003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3, true); //검사진행상태

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            if (strPlantCd != "")
            {
                cboPlantCd.SelectedValue = strPlantCd;
            }
            else
            {
                cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
            }

            txtCustCd.Text = strCustCd;
            txtInspReqNo.Text = strInspReqNo;
            cboInspClassCd.SelectedValue = strInspClassCd;
            cboInspStatus.SelectedValue = strInspStatus;
            dtpInspReqDtFr.Text = strInspDtFr;
            dtpInspReqDtTo.Text = strInspDtTo;

            Grid_search(false);

            this.Text = "전진검사요청정보 조회";
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

                    string strQuery = " usp_WNDW041 @pTYPE = 'S1'";
                    strQuery += ", @pPLANT_CD ='" + Convert.ToString(cboPlantCd.SelectedValue) + "'";
                    strQuery += ", @pINSP_REQ_NO ='" + txtInspReqNo.Text + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                    strQuery += ", @pINSP_CLASS_CD ='" + Convert.ToString(cboInspClassCd.SelectedValue) + "'";
                    strQuery += ", @pINSP_STATUS ='" + Convert.ToString(cboInspStatus.SelectedValue) + "'";
                    strQuery += ", @pINSP_REQ_DT_FR ='" + dtpInspReqDtFr.Text + "'";
                    strQuery += ", @pINSP_REQ_DT_TO ='" + dtpInspReqDtTo.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

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

        #region 조회조건 팝업창
        //품목코드
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW005 pu = new WNDW.WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextBox코드입력시 코드명 자동입력
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                    txtItemSpec.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_SPEC", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                    txtItemSpec.Value = "";
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

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_WNDW041 @pTYPE = 'S1'";
                strQuery += ", @pPLANT_CD ='" + Convert.ToString(cboPlantCd.SelectedValue) + "'";
                strQuery += ", @pINSP_REQ_NO ='" + txtInspReqNo.Text + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                strQuery += ", @pINSP_CLASS_CD ='" + Convert.ToString(cboInspClassCd.SelectedValue) + "'";
                strQuery += ", @pINSP_STATUS ='" + Convert.ToString(cboInspStatus.SelectedValue) + "'";
                strQuery += ", @pINSP_REQ_DT_FR ='" + dtpInspReqDtFr.Text + "'";
                strQuery += ", @pINSP_REQ_DT_TO ='" + dtpInspReqDtTo.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

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

        #region 그리드 선택값 입력및 전송
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
        private void txtInspReqNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        private void txtLotNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        { if (e.KeyCode == Keys.Enter) Grid_search(true); }
        #endregion
    }
}
