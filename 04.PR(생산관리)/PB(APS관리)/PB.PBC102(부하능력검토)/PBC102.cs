#region 작성정보
/*********************************************************************/
// 단위업무명 : 부하능력검토
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-09
// 작성내용 : 부하능력검토 및 관리
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
using System.Data.SqlClient;
using WNDW;

namespace PB.PBC102
{
    public partial class PBC102 : UIForm.FPCOMM2
    {
        public PBC102()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PBC102_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboSch_id, "usp_P_COMMON 'P520', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            cboSch_id.SelectedIndex = 3;

            dtpStartDT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).ToShortDateString();
            dtpEndDT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(2).ToShortDateString();
           

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "자원명")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P050', @pCOM_CD = '', @pCOM_NM = '', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P050', @pCOM_CD = '', @pCOM_NM = '', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            cboSch_id.SelectedIndex = 3;
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
        }
        #endregion

        #region 조회조건 팝업
        private void btnWORKCENTER_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWORKCENTER_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWORKCENTER_CD.Text = Msgs[0].ToString();
                    txtWORKCENTER_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        private void txtWORKCENTER_CD_TextChanged(object sender, System.EventArgs e)
        {
            string strSql = "and LANG_CD = '" + SystemBase.Base.gstrLangCd + "' and MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'";
            txtWORKCENTER_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWORKCENTER_CD.Text, strSql);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery1 = " usp_PBC102 @pType='S1', ";
                    strQuery1 += " @pSCH_ID='" + cboSch_id.SelectedValue.ToString() + "', ";
                    strQuery1 += " @pSTART_DT='" + dtpStartDT.Value.ToString().Substring(0, 10) + "', ";
                    strQuery1 += " @pEND_DT='" + dtpEndDT.Value.ToString().Substring(0, 10) + "', ";
                    strQuery1 += " @pWC_CD='" + txtWORKCENTER_CD.Text + "' ";
                    strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery1, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].RowCount = 0;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 그리드 선택시
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    string strQuery1 = " USP_PBC102 'S2', ";
                    strQuery1 += " @pSCH_ID='" + cboSch_id.SelectedValue.ToString() + "', ";
                    strQuery1 += " @pSTART_DT='" + dtpStartDT.Value.ToString().Substring(0, 10) + "', ";
                    strQuery1 += " @pEND_DT='" + dtpEndDT.Value.ToString().Substring(0, 10) + "', ";
                    strQuery1 += " @pWC_CD='" + fpSpread2.Sheets[0].Cells[fpSpread2.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx2, "작업장")].Value + "', ";
                    strQuery1 += " @pRES_CD='" + fpSpread2.Sheets[0].Cells[fpSpread2.ActiveSheet.GetSelection(0).Row, SystemBase.Base.GridHeadIndex(GHIdx2, "자원명")].Value + "', ";
                    strQuery1 += " @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion		
    }
}
