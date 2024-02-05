

#region 작성정보
/*********************************************************************/
// 단위업무명 : 통합재무제표조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-05
// 작성내용 : 통합재무제표조회
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
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

namespace AZ.ACZ004
{
    public partial class ACZ004 : UIForm.FPCOMM1 
    {
        public ACZ004()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACZ004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboCoCd, "usp_B_COMMON @pTYPE ='CO' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //법인
            SystemBase.ComboMake.C1Combo(cboDiv, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A119' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //전표형태

            dtpThis_YYMM_Fr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01";
            dtpThis_YYMM_To.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-12";
            dtpPre_YYMM_Fr.Text = (Convert.ToDouble(SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4)) - 1).ToString() + "-01";
            dtpPre_YYMM_To.Text = (Convert.ToDouble(SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4)) - 1).ToString() + "-12";
            

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.ComboMake.C1Combo(cboDiv, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A119' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //재무제표구분

            dtpThis_YYMM_Fr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01";
            dtpThis_YYMM_To.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-12";
            dtpPre_YYMM_Fr.Value = (Convert.ToDouble(SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4)) - 1).ToString() + "-01";
            dtpPre_YYMM_To.Value = (Convert.ToDouble(SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4)) - 1).ToString() + "-12";

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ACZ004 ";
                    strQuery += " @pTHIS_F_YYMM = '" + dtpThis_YYMM_Fr.Text.Replace("-","") + "' ";
                    strQuery += ", @pTHIS_T_YYMM = '" + dtpThis_YYMM_To.Text.Replace("-", "") + "' ";
                    strQuery += ", @pPRE_F_YYMM = '" + dtpPre_YYMM_Fr.Text.Replace("-", "") + "' ";
                    strQuery += ", @pPRE_T_YYMM = '" + dtpPre_YYMM_To.Text.Replace("-", "") + "' ";
                    strQuery += ", @pCO_CD = '" + cboCoCd.SelectedValue.ToString() + "' ";
                    if (optSearch_Type_OR.Checked == true) strQuery += ", @pSEARCH_TYPE = 'OR' ";
                    else if (optSearch_Type_RE.Checked == true) strQuery += ", @pSEARCH_TYPE = 'RE' ";
                    strQuery += ", @pDIV = '" + cboDiv.SelectedValue.ToString() + "' ";
                    strQuery += ", @pTYPE_CD = '" + cboTypeCd.SelectedValue.ToString() + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "ER")
                        {
                            MessageBox.Show(dt.Rows[0][1].ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            fpSpread1.Sheets[0].Rows.Count = 0;
                        }
                        else
                        {
                            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                        }
                    }
                    else
                    {
                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                        //MessageBox.Show("관리자에게 문의하세요(MS-SQL Qury 에러)", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region TextChanged
        private void cboDiv_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SystemBase.ComboMake.C1Combo(cboTypeCd, "usp_B_COMMON @pTYPE='REL', @pCODE = 'A120', @pSPEC1 = '" + cboDiv.SelectedValue.ToString() + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);   //재무제표유형
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
