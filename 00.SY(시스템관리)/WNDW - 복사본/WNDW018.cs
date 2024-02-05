#region 작성정보
/*********************************************************************/
// 단위업무명 : 공통팝업 구매발주정보
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-18
// 작성내용 : 구매발주정보
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
    WNDW.WNDW018 pu = new WNDW.WNDW018();
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
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
    /// <summary>
    /// 구매발주정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// <para>Msgs[1] = 구매발주번호 </para>
    /// </summary>

    public partial class WNDW018 : UIForm.FPCOMM1
    {
        #region 변수선언
        string[] returnVal = null;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;	// 조회 건수
        #endregion

        #region 생성자
        public WNDW018()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void WNDW018_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //기타 세팅
            dtpPoDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpPoDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            Grid_Search(false);

            this.Text = "구매발주정보 조회";
        }
        #endregion

        #region 조회조건 팝업
        //발주형태
        private void btnPoType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1='PO_TYPE_CD', @pSPEC2 = 'PO_TYPE_NM', @pSPEC3 = 'M_PO_TYPE', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPoType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "발주형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPoType.Text = Msgs[0].ToString();
                    txtPoTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

            }
        }

        //공급처
        private void btnCustCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //구매담당자
        private void butPurDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Text = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //발주형태
        private void txtPoType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPoType.Text != "")
                {
                    txtPoTypeNm.Value = SystemBase.Base.CodeName("PO_TYPE_CD", "PO_TYPE_NM", "M_PO_TYPE", txtPoType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtPoTypeNm.Value = "";
                }
            }
            catch { }
        }

        //공급처
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch { }
        }

        //구매담당자
        private void txtPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDuty.Text != "")
                {
                    txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
                }
                else
                {
                    txtPurDutyNm.Value = "";
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

                    string strCfmYn = "";
                    if (rdoCfmYnYes.Checked == true) strCfmYn = "Y";
                    else if (rdoCfmYnNo.Checked == true) strCfmYn = "N";

                    string strQuery = " usp_WNDW018 'S1'";
                    strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "'";
                    strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "'";
                    strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "'";
                    strQuery += ", @pPO_TYPE = '" + txtPoType.Text + "'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
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

            string strCfmYn = "";
            if (rdoCfmYnYes.Checked == true) strCfmYn = "Y";
            else if (rdoCfmYnNo.Checked == true) strCfmYn = "N";

            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                int cnt_prev = AddRow * SDown;
                SDown++;
                int cnt = AddRow * SDown;

                string strQuery = " usp_WNDW018 'S1'";
                strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "'";
                strQuery += ", @pPO_DT_FR = '" + dtpPoDtFr.Text + "'";
                strQuery += ", @pPO_DT_TO = '" + dtpPoDtTo.Text + "'";
                strQuery += ", @pPO_TYPE = '" + txtPoType.Text + "'";
                strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                strQuery += ", @pPUR_DUTY = '" + txtPurDuty.Text + "'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
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
