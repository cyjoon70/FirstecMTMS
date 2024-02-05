#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 구매요청정보팝업
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-16
// 작성내용 : 구매요청정보팝업
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
    WNDW.WNDW016 pu = new WNDW.WNDW016();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 구매요청정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 구매요청번호 </para>
    /// </summary>

    public partial class WNDW016 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        #endregion

        #region 생성자
        public WNDW016()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW016_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            Grid_Search(false);

            this.Text = "구매요청정보 조회";
        }
        #endregion

        #region 조회조건 팝업
        //요청자
        private void btnUser_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "요청자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Text = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
                    txtUserId.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "요청자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //요청부서
        private void butReqDept_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'D011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReqDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReqDeptCd.Text = Msgs[0].ToString();
                    txtReqDeptNm.Value = Msgs[1].ToString();
                    txtReqReorgId.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "요청부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //요청자
        private void txtUserId_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtUserId.Text != "")
                {
                    txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtUserNm.Value = "";
                }
            }
            catch { }
        }

        //요청부서
        private void txtReqDeptCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                string Query = " usp_B_COMMON 'D021', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    txtReqReorgId.Value = dt.Rows[0][0].ToString();
                    txtReqDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtReqDeptCd.Text, " And REORG_ID = '" + txtReqReorgId.Value + "'  AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtReqReorgId.Value = "";
                    txtReqDeptNm.Value = "";
                }
            }
            catch { }
            
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
                    SDown = 1;

                    string strReqPart = "";
                    if (rdoPartM.Checked == true) { strReqPart = "M"; }
                    else if (rdoPartS.Checked == true) { strReqPart = "S"; }

                    string strReqType = "";
                    if (rdoTypeM.Checked == true) { strReqType = "M"; }
                    else if (rdoTypeE.Checked == true) { strReqType = "E"; }
                    else if (rdoP.Checked == true) { strReqType = "P"; }

                    string strconfirm = "";
                    if (rdoConfirmY.Checked == true) { strconfirm = "Y"; }
                    else if(rdoConfirmN.Checked == true) {strconfirm = "N";}

                    string strQuery = " usp_WNDW016 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "' ";
                    strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "' ";
                    strQuery += ", @pREQ_PART = '" + strReqPart + "' ";
                    strQuery += ", @pREQ_TYPE = '" + strReqType + "' ";
                    strQuery += ", @pREQ_ID = '" + txtUserId.Text + "' ";
                    strQuery += ", @pREQ_DEPT_CD = '" + txtReqDeptCd.Text + "' ";
                    strQuery += ", @pREQ_REORG_ID = '" + txtReqReorgId.Text + "' ";
                    strQuery += ", @pCONFIRM_FLAG = '" + strconfirm + "' ";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

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

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string strReqPart = "";
            if (rdoPartM.Checked == true) { strReqPart = "M"; }
            else if (rdoPartS.Checked == true) { strReqPart = "S"; }

            string strReqType = "";
            if (rdoTypeM.Checked == true) { strReqType = "M"; }
            else if (rdoTypeE.Checked == true) { strReqType = "E"; }
            else if (rdoP.Checked == true) { strReqType = "P"; }

            string strconfirm = "";
            if (rdoConfirmY.Checked == true) { strconfirm = "Y"; }
            else if (rdoConfirmN.Checked == true) { strconfirm = "N"; }

            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                int cnt_prev = AddRow * SDown;
                SDown++;
                int cnt = AddRow * SDown;

                string strQuery = " usp_WNDW016 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "' ";
                strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "' ";
                strQuery += ", @pREQ_PART = '" + strReqPart + "' ";
                strQuery += ", @pREQ_TYPE = '" + strReqType + "' ";
                strQuery += ", @pREQ_ID = '" + txtUserId.Text + "' ";
                strQuery += ", @pREQ_DEPT_CD = '" + txtReqDeptCd.Text + "' ";
                strQuery += ", @pREQ_REORG_ID = '" + txtReqReorgId.Text + "' ";
                strQuery += ", @pCONFIRM_FLAG = '" + strconfirm + "' ";
                strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);
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
                    returnVal[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
                }
            }
        }
        #endregion	
    }
}
