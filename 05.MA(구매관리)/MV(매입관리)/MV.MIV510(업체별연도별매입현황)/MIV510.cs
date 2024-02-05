#region 작성정보
/*********************************************************************/
// 단위업무명:  업체별년도별매입현황조회
// 작 성 자  :  한 미 애
// 작 성 일  :  2020-08-27
// 작성내용  :  업체별 3개년도 매입금액을 집계하여 조회한다.
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

namespace MV.MIV510
{
    public partial class MIV510 : UIForm.FPCOMM1
    {
        #region 생성자
        public MIV510()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void MIV510_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9); //품목계정

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpIvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-2).ToShortDateString().Substring(0, 4) + "-01-01";
            dtpIvDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            chkDIV_Type.Checked = true;
            chkIIV_Type.Checked = true;
            chkOIV_Type.Checked = true;
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
            dtpIvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-2).ToShortDateString().Substring(0,4) + "-01-01";
            dtpIvDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

            chkDIV_Type.Checked = true;
            chkIIV_Type.Checked = true;
            chkOIV_Type.Checked = true;

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
                    string strDIvType, strIIvType, strOIvType;
                    strDIvType = ""; strIIvType = ""; strOIvType = "";

                    if (chkDIV_Type.Checked == true)     // 국내매입이 체크된 경우
                        strDIvType = "DIV";
                    if (chkIIV_Type.Checked == true)   // 해외매입이 체크된 경우
                        strIIvType = "IIV";
                    if (chkOIV_Type.Checked == true)   // 외주가공매입이 체크된 경우
                        strOIvType = "OIV";

                    string strQuery = "usp_MIV510 @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSTART_YEAR = '" + dtpIvDtFr.Text + "'";
                    strQuery += ", @pEND_YEAR = '" + dtpIvDtTo.Text + "'";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "'";
                    strQuery += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pDIV_TYPE = '" + strDIvType + "'";
                    strQuery += ", @pIIV_TYPE = '" + strIIvType + "'";
                    strQuery += ", @pOIV_TYPE = '" + strOIvType + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    string strQuery1 = " usp_MIV510 @pTYPE = 'S2' ";
                    strQuery1 += ", @pSTART_YEAR = '" + dtpIvDtFr.Text + "'";
                    strQuery1 += ", @pEND_YEAR = '" + dtpIvDtTo.Text + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 3].Text = dt.Rows[i]["IV_YEAR"].ToString();
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, i + 3].Tag = dt.Rows[i]["IV_YEAR"].ToString();
                    }                    
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
