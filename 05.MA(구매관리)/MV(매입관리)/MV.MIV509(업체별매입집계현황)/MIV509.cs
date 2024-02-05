#region 작성정보
/*********************************************************************/
// 단위업무명:  업체별매입집계현황
// 작 성 자  :  한 미 애
// 작 성 일  :  2019-01-18
// 작성내용  :  업체별 내자/외자/외주 매입액을 조회한다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using WNDW;
using FarPoint.Win.Spread.CellType;

namespace MV.MIV509
{
    public partial class MIV509 : UIForm.FPCOMM1
    {
        #region 생성자
        public MIV509()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void MIV509_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9); //품목계정

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpIvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0, 4) + "-01-01";      // 현재년도 1월1일로
            dtpIvDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            chkDIV_Type.Checked = true;     // 내외자구매 검색조건의 기본 선택값을 '내자'로
            rdoAmtMill.Checked = true;      // 금액단위 검색조건의 기본 선택값을 '백만원'으로.
        }
        #endregion
        
        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9); //품목계정

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpIvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,4) + "-01-01";
            dtpIvDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            chkDIV_Type.Checked = true;     // 내외자구매 검색조건의 기본 선택값을 '내자'로
            rdoAmtMill.Checked = true;      // 금액단위 검색조건의 기본 선택값을 '백만원'으로.

        }
        #endregion

        #region 조회조건 버튼 Click 
        //공급처 FROM
        private void btnCustCdFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // 공급처 TO
        private void btnCustCdTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        // 공급처 FROM
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    // 2022.04.15. hma 추가(Start): 입력된 기간이 3개년을 초과하는지 체크하여 초과시 메시지 띄우고 리턴
                    if (DateTime.Compare(Convert.ToDateTime(dtpIvDtTo.Text + "-01"), Convert.ToDateTime(dtpIvDtFr.Text + "-01")) < 0)
                    {
                        MessageBox.Show("To 매입년월이 더 작게 지정되었습니다. 확인해보세요.");
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    int iYearCnt = 0, iMaxYearCnt = 0, iColumnCnt = 0;
                    iYearCnt = Convert.ToInt16(dtpIvDtTo.Text.Substring(0, 4)) - Convert.ToInt16(dtpIvDtFr.Text.Substring(0, 4));
                    iMaxYearCnt = 3;        // 3년치까지만 조회되게 하므로.
                    iColumnCnt = 6;         // 1년치에 대한 항목 갯수가 6개이므로.

                    if (Convert.ToInt16(dtpIvDtTo.Text.Substring(0,4)) - Convert.ToInt16(dtpIvDtFr.Text.Substring(0, 4)) > 2)
                    {
                        MessageBox.Show("매입기간을 3년까지 지정하시어 조회하시기 바랍니다.");
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    string strYear1 = "", strYear2 = "", strYear3 = "", strOneYear = "";
                    strYear1 = dtpIvDtFr.Text.Substring(0, 4);
                    if (iYearCnt > 0)
                        strYear2 = Convert.ToString(Convert.ToInt16(strYear1) + 1);
                    if (iYearCnt > 1)
                        strYear3 = Convert.ToString(Convert.ToInt16(strYear2) + 1);
                    iYearCnt = iYearCnt + 1;       // 1을 더해서 1개년부터 시작하게.

                    int iStartIdx = 0;
                    iStartIdx = 3;
                    // 2022.04.15. hma 추가(End)

                    string strIvType;
                    strIvType = "";
                    if ((chkDIV_Type.Checked == false) || (chkIIV_Type.Checked == false))       // 내자나 외자에 체크되지 않은게 있는 경우
                    {
                        if (chkDIV_Type.Checked == true)        // 내자가 체크된 경우
                            strIvType = "DIV";
                        else if (chkIIV_Type.Checked == true)   // 외자가 체크된 경우
                            strIvType = "IIV";
                        else    // 내자와 외자 모두 체크되지 않은 경우
                            strIvType = "OIV";
                    }

                    int iAmtUnit;
                    if (rdoAmtMill.Checked == true)
                        iAmtUnit = 1000000;
                    else if (rdoAmtThou.Checked == true)
                        iAmtUnit = 1000;
                    else
                        iAmtUnit = 1;

                    string strQuery = "usp_MIV509 @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pIV_DT_FR = '" + dtpIvDtFr.Text.Replace("-","") + "'";      // 2022.04.25. hma 수정: 일자에서 년월 형태로 변경되어 - 문자를 없애고 매개변수 전달되게
                    strQuery += ", @pIV_DT_TO = '" + dtpIvDtTo.Text.Replace("-", "") + "'";     // 2022.04.25. hma 수정: 일자에서 년월 형태로 변경되어 - 문자를 없애고 매개변수 전달되게
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pIV_TYPE = '" + strIvType + "'";
                    strQuery += ", @pAMT_UNIT = " + iAmtUnit + " ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    // 2022.04.15. hma 추가(Start): 조회후 기간 이외는 삭제하고 맨위의 년도에 입력된 년도가 나오게 함.
                    //컬럼 틀고정
                    fpSpread1.Sheets[0].FrozenColumnCount = SystemBase.Base.GridHeadIndex(GHIdx1, "거래처명") + 1;

                    // 헤더명설정
                    for (int i = 0; i < iYearCnt; i++)
                    {
                        switch (i)
                        {
                            case 0: strOneYear = strYear1; break;
                            case 1: strOneYear = strYear2; break;
                            case 2: strOneYear = strYear3; break;
                        }

                        for (int j = 0; j < iColumnCnt; j++)
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, iStartIdx + (i * iColumnCnt) + j].Text = strOneYear + "년";
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, iStartIdx + (i * iColumnCnt) + j].Tag = strOneYear + "년";
                            fpSpread1.Sheets[0].Columns[iStartIdx + (i * iColumnCnt) + j].Visible = true;
                        }
                    }

                    // 컬럼헤더 숨김
                    for (int i = iStartIdx + (iColumnCnt * iYearCnt); i < iStartIdx + (iColumnCnt * iYearCnt) + (iColumnCnt * (iMaxYearCnt - iYearCnt)); i++)
                    {
                        fpSpread1.Sheets[0].Columns[i].Visible = false;
                    }
                    // 2022.04.15. hma 추가(End)
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }

        }
        #endregion

                
    }
}
