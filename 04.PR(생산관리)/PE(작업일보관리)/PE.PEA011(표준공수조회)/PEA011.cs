#region 작성정보
/*********************************************************************/
// 단위업무명 : 표준공수조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-15
// 작성내용 : 표준공수조회
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
using WNDW;

namespace PE.PEA011
{
    public partial class PEA011 : UIForm.FPCOMM1
    {
        #region Form Load시
        int lastCol = 30;
        #endregion

        public PEA011()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PEA011_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크
           
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

            dtpBaseDt.Text = SystemBase.Base.ServerTime("Y");

        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //조회조건 초기화
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
            dtpBaseDt.Text = SystemBase.Base.ServerTime("Y");
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_PEA011  @pTYPE = 'S1'";
                    strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtITEM_CD.Text + "'";
                    strQuery += ", @pPLAN_YEAR = '" + dtpBaseDt.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        decimal dDirectTm = 0;
                        decimal dIndirectTm = 0;
                        decimal dTotTm = 0;

                        for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text == "양산")
                            {
                                dDirectTm = dDirectTm + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "표준공수(분)")].Text);

                                string sort = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SORT")].Text;
                                string chk = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "CHK")].Text;
                                decimal sum = 0;

                                for (int j = 0; j < fpSpread1.Sheets[0].RowCount; j++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "SORT")].Text.StartsWith(sort) == true
                                        && chk == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "CHK")].Text)
                                    {
                                        sum = sum + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "표준공수(분)")].Text);
                                    }
                                }

                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공수집계(분)")].Text = Convert.ToString(sum);
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text == "개발일정")
                            {
                                dIndirectTm = dIndirectTm + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "표준공수(분)")].Text);
                            }
                        }

                        dtxtDirectTm.ReadOnly = false;
                        dtxtDirectTm.Text = dDirectTm.ToString();
                        dtxtDirectTm.ReadOnly = true;

                        dtxtIndirectTm.ReadOnly = false;
                        dtxtIndirectTm.Text = dIndirectTm.ToString();
                        dtxtIndirectTm.ReadOnly = true;

                        dtxtTotTm.ReadOnly = false;
                        dtxtTotTm.Text = Convert.ToString((dDirectTm + dIndirectTm));
                        dtxtTotTm.ReadOnly = true;
                    }
                    else
                    {
                        dtxtDirectTm.ReadOnly = false;
                        dtxtDirectTm.Text = "0";
                        dtxtDirectTm.ReadOnly = true;

                        dtxtIndirectTm.ReadOnly = false;
                        dtxtIndirectTm.Text = "0";
                        dtxtIndirectTm.ReadOnly = true;

                        dtxtTotTm.ReadOnly = false;
                        dtxtTotTm.Text = "0";
                        dtxtTotTm.ReadOnly = true;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        // 프로젝트
        private void btnProject_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }       
        //품목
        private void btnITEM_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텍스트박스 코드 입력시 코드명 자동입력
        // 품목
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
        }
        #endregion

     

    }
}
