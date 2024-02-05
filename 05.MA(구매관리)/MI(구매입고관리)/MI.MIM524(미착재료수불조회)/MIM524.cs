
#region 작성정보
/*********************************************************************/
// 단위업무명 : 미착재료비수불
// 작 성 자 : 한미애
// 작 성 일 : 2015-04-29
// 작성내용 : 미착재료비수불
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

namespace MI.MIM524
{
    public partial class MIM524 : UIForm.FPCOMM2
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public MIM524()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void MIM524_Load(object sender, System.EventArgs e) 
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpNotRcvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 7);
            dtpNotRcvDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 7);

            cmbItemAcct.Items.Add("전체");
            cmbItemAcct.Items.Add("원자재");
            cmbItemAcct.SelectedIndex = 0;

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpNotRcvDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 7);
            dtpNotRcvDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 7);

        }
        #endregion

        #region SearchExec()  그리드 조회
        protected override void SearchExec()
        {
            SelectList();
        }
        #endregion


        #region 조회조건 팝업
        // 품목
        private void cbtnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true, txtItemCd.Text);
                pu.MaximizeBox = false;
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	// 데이터 조회 중 오류가 발생하였습니다.

            }
        }

        // 프로젝트
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        private void MIM524_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpNotRcvDtFr.Focus();
        }

        private void MIM524_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

        // 데이터 조회
        private void SelectList()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (rdoGoods.Checked == true)       // 물품대를 선택한 경우
                {
                    GridCommPanel1.Visible = true;
                    GridCommPanel2.Visible = false;

                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                    {
                        string strQuery = " usp_MIM524 'S1'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                        strQuery += ", @pYYYYMM_FR ='" + dtpNotRcvDtFr.Text.Replace("-", "") + "'";
                        strQuery += ", @pYYYYMM_TO ='" + dtpNotRcvDtTo.Text.Replace("-", "") + "'";
                        strQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text + "'";
                        strQuery += ", @pBL_NO ='" + txtBL_NO.Text + "'";
                        strQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                        strQuery += ", @pITEM_ACCT ='" + cmbItemAcct.Text + "'";

                        UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1, true);
                        fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    }
                }
                else    // 부대비를 선택한 경우
                {
                    GridCommPanel1.Visible = false;
                    GridCommPanel2.Visible = true;

                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                    {
                        string strQuery = " usp_MIM524 'S2'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                        strQuery += ", @pYYYYMM_FR ='" + dtpNotRcvDtFr.Text.Replace("-", "") + "'";
                        strQuery += ", @pYYYYMM_TO ='" + dtpNotRcvDtTo.Text.Replace("-", "") + "'";
                        strQuery += ", @pPROJECT_NO ='" + txtProjectNo.Text + "'";
                        strQuery += ", @pBL_NO ='" + txtBL_NO.Text + "'";
                        strQuery += ", @pITEM_CD ='" + txtItemCd.Text + "'";
                        strQuery += ", @pITEM_ACCT ='" + cmbItemAcct.Text + "'";

                        UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 1, true);
                        fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;

        }

        private void rdoExpense_CheckedChanged(object sender, EventArgs e)
        {
            SelectList();

        }
       
    }
}
