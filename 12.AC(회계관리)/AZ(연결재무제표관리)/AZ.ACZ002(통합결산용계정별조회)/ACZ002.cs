

#region 작성정보
/*********************************************************************/
// 단위업무명 : 통합결산용계정별조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-05
// 작성내용 : 통합결산용계정별조회
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

namespace AZ.ACZ002
{
    public partial class ACZ002 : UIForm.FPCOMM1 
    {
        public ACZ002()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACZ002_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboCoCd, "usp_B_COMMON @pTYPE ='CO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //법인
            dtpSlipYYMM_Fr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01";
            dtpSlipYYMM_To.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-12";

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpSlipYYMM_Fr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01";
            dtpSlipYYMM_To.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-12";

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
                    string strQuery = " usp_ACZ002 ";
                    strQuery += " @pSLIP_F_YYMM = '" + dtpSlipYYMM_Fr.Text.Replace("-", "") + "' ";
                    strQuery += ", @pSLIP_T_YYMM = '" + dtpSlipYYMM_To.Text.Replace("-", "") + "' ";
                    strQuery += ", @pCO_CD = '" + cboCoCd.SelectedValue.ToString() + "' ";
                    if (optSearch_Type_OR.Checked == true) strQuery += ", @pSEARCH_TYPE = 'OR' ";
                    else if (optSearch_Type_RE.Checked == true) strQuery += ", @pSEARCH_TYPE = 'RE' ";
                    if (optTYPE1.Checked == true) strQuery += ", @pQUERY_TYPE = 'CL' ";
                    else if (optTYPE2.Checked == true) strQuery += ", @pQUERY_TYPE = 'CD' ";
                    strQuery += ", @pTYPE_CD = '" + txtTypeCd.Text + "' ";

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
                            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
                        }
                    }
                    else
                    {
                        //MessageBox.Show("관리자에게 문의하세요(MS-SQL Qury 에러)", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        fpSpread1.Sheets[0].Rows.Count = 0;
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

        #region 재무제표코드 팝업
        private void btnTypeCd_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string strQuery = " usp_B_COMMON @pType='COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'A120', @pSPEC2 = 'TB' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtTypeCd.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00113", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "재무제표코드 조회");
                    pu.Width = 800;
                    pu.Height = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        txtTypeCd.Value = Msgs[0].ToString();
                        txtTypeNm.Value = Msgs[1].ToString();
                        txtTypeNm.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "재무제표코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회유형 변경시
        private void optTYPE1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optTYPE1.Checked == true)
                {
                    txtTypeCd.Tag = "재무제표유형;1;;";
                    btnTypeCd.Tag = ";;;";
                    
                }
                else
                {
                    txtTypeCd.Value = "";
                    txtTypeCd.Tag = ";2;;";
                    btnTypeCd.Tag = ";2;;";
                }
                SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 재무제표코드 TextChanged
        private void txtTypeCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTypeCd.Text, " AND MAJOR_CD = 'A120' AND REL_CD1 = 'TB' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
